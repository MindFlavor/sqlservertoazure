using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ITPCfSQL.Azure.CLR
{
    public class Management
    {
        #region Common methods
        public static X509Certificate2 RetrieveCertificateInStore(string certificateThumbprint)
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                X509FindType.FindByThumbprint,
                certificateThumbprint,
                false);
            certStore.Close();

            if ((certCollection == null) || (certCollection.Count == 0))
                throw new ArgumentException("Certificate with thumbprint " + certificateThumbprint + " not found in the " +
                    "certificate store[Name=" + certStore.Name + ", Location=" + certStore.Location.ToString() + "]");

            return certCollection[0];
        }
        #endregion

        [SqlFunction(
               DataAccess = DataAccessKind.None,
               SystemDataAccess = SystemDataAccessKind.None,
               FillRowMethodName = "_GetServicesCallback",
               IsDeterministic = false,
               IsPrecise = true,
               TableDefinition = (@"
                    ServiceName							NVARCHAR(4000),
	                Url									NVARCHAR(4000),
	                DefaultWinRmCertificateThumbprint	NVARCHAR(255),
	                AffinityGroup						NVARCHAR(4000),
	                DateCreated							DATETIME,
	                DateLastModified					DATETIME,
	                [Description]						NVARCHAR(MAX),
	                Label								NVARCHAR(4000),
	                Location							NVARCHAR(4000),
	                [Status]							NVARCHAR(255)"))]
        public static System.Collections.IEnumerable GetServices(
            SqlString certificateThumbprint,
            SqlGuid sgSubscriptionId)
        {
            X509Certificate2 certificate = RetrieveCertificateInStore(certificateThumbprint.Value);
            Azure.Management.HostedServices result = Azure.Internal.Management.GetServices(certificate, sgSubscriptionId.Value);
            return result.HostedService;
        }

        [SqlFunction(
                DataAccess = DataAccessKind.None,
                SystemDataAccess = SystemDataAccessKind.None,
                FillRowMethodName = "GetDeploymentsPersistentVMRolesWithInputEndpointsCallback",
                IsDeterministic = false,
                IsPrecise = true,
                TableDefinition = (@"
	                Name								NVARCHAR(4000),
	                DeploymentSlot						NVARCHAR(255),
	                PrivateID							NVARCHAR(255),
	                [Status]							NVARCHAR(255),
	                Label								NVARCHAR(4000),
	                Url									NVARCHAR(4000),
	                Configuration						NVARCHAR(MAX),
	                UpgradeDomainCount					INT,
	                VMName								NVARCHAR(4000),
	                OsVersion							NVARCHAR(4000),
	                RoleSize							NVARCHAR(255),
	                DefaultWinRmCertificateThumbprint	NVARCHAR(4000),
	                EndpointName						NVARCHAR(4000),
	                LocalPort							INT,
	                Port								INT,
	                Protocol							NVARCHAR(255),
	                Vip									NVARCHAR(255)
                "))]
        public static System.Collections.IEnumerable GetDeploymentsPersistentVMRolesWithInputEndpoints(
            SqlString certificateThumbprint,
            SqlGuid sgSubscriptionId,
            SqlString serviceName,
            SqlString deploymentSlots)
        {
            X509Certificate2 certificate = RetrieveCertificateInStore(certificateThumbprint.Value);
            Azure.Management.Deployment result = Azure.Internal.Management.GetDeployments(
                certificate,
                sgSubscriptionId.Value,
                serviceName.Value,
                deploymentSlots.Value);

            List<CallbackSupport.DeploymentsPersistentVMRolesWithInputEndpoints> lDps = new List<CallbackSupport.DeploymentsPersistentVMRolesWithInputEndpoints>();

            for (int iRole = 0; iRole < result.RoleList.Length; iRole++)
            {
                for (int iEndPoint = 0; iEndPoint < result.RoleList[iRole].ConfigurationSets.ConfigurationSet.InputEndpoints.Length; iEndPoint++)
                {
                    lDps.Add(new CallbackSupport.DeploymentsPersistentVMRolesWithInputEndpoints()
                    {
                        Deployment = result,
                        DeploymentRole = result.RoleList[iRole],
                        InputEndpoint = result.RoleList[iRole].ConfigurationSets.ConfigurationSet.InputEndpoints[iEndPoint]
                    });
                }
            }

            return lDps;
        }

        [SqlProcedure]
        public static void AddInputEndpointToPersistentVM(
            SqlString certificateThumbprint,
            SqlGuid sgSubscriptionId,
            SqlString serviceName,
            SqlString deploymentSlots,
            SqlString vmName,
            SqlString EndpointName,
            SqlInt32 LocalPort,
            SqlBoolean EnableDirectServerReturn,
            SqlInt32 Port,
            SqlString Protocol,
            SqlString Vip,
            SqlBoolean fBlocking)
        {
            X509Certificate2 certificate = RetrieveCertificateInStore(certificateThumbprint.Value);
            Azure.Management.Deployment result = Azure.Internal.Management.GetDeployments(
                certificate,
                sgSubscriptionId.Value,
                serviceName.Value,
                deploymentSlots.Value);

            var vmToAdd = (ITPCfSQL.Azure.Management.PersistentVMRole)result.RoleList.FirstOrDefault(item => item.RoleName == vmName.Value);

            if (vmToAdd == null)
                throw new ArgumentException("No PersistentVMRole with name " + vmName.Value + " found in sgSubscriptionId = " +
                    sgSubscriptionId.Value + ", serviceName = " + serviceName.Value + ", deploymentSlots = "
                    + deploymentSlots.Value + ".");

            ITPCfSQL.Azure.Management.DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint[] driiesTemp = new
                    ITPCfSQL.Azure.Management.DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint[vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints.Length + 1];

            Array.Copy(vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints, driiesTemp, vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints.Length);

            driiesTemp[driiesTemp.Length - 1] = new ITPCfSQL.Azure.Management.DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint()
            {
                Name = EndpointName.Value,
                LocalPort = LocalPort.Value,
                EnableDirectServerReturn = EnableDirectServerReturn.Value,
                Port = Port.Value,
                Protocol = Protocol.Value,
                Vip = Vip.Value
            };

            vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints = driiesTemp;

            var output = ITPCfSQL.Azure.Internal.Management.UpdatePersistentVMRole(
                certificate,
                sgSubscriptionId.Value,
                serviceName.Value,
                result.Name,
                vmToAdd);

            PushOperationStatus(
                certificate,
                sgSubscriptionId.Value,
                new Guid(output[ITPCfSQL.Azure.Internal.Constants.HEADER_REQUEST_ID]),
                fBlocking.IsNull ? false : fBlocking.Value);
        }

        [SqlProcedure]
        public static void RemoveEndpointFromPersistentVM(
            SqlString certificateThumbprint,
            SqlGuid sgSubscriptionId,
            SqlString serviceName,
            SqlString deploymentSlots,
            SqlString vmName,
            SqlString EndpointName,
            SqlBoolean fBlocking)
        {
            X509Certificate2 certificate = RetrieveCertificateInStore(certificateThumbprint.Value);
            Azure.Management.Deployment result = Azure.Internal.Management.GetDeployments(
                certificate,
                sgSubscriptionId.Value,
                serviceName.Value,
                deploymentSlots.Value);

            var vmToAdd = (ITPCfSQL.Azure.Management.PersistentVMRole)result.RoleList.FirstOrDefault(item => item.RoleName == vmName.Value);

            if (vmToAdd == null)
                throw new ArgumentException("No PersistentVMRole with name " + vmName.Value + " found in sgSubscriptionId = " +
                    sgSubscriptionId.Value + ", serviceName = " + serviceName.Value + ", deploymentSlots = "
                    + deploymentSlots.Value + ".");

            // Remove the endpoint
            ITPCfSQL.Azure.Management.DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint[] driiesTemp = new
            ITPCfSQL.Azure.Management.DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint[vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints.Length - 1];
            int iTemp = 0;
            foreach (ITPCfSQL.Azure.Management.DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint ie in vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints)
            {
                if (!(ie.Name == EndpointName.Value))
                    driiesTemp[iTemp++] = ie;
            }

            vmToAdd.ConfigurationSets.ConfigurationSet.InputEndpoints = driiesTemp;

            var output = ITPCfSQL.Azure.Internal.Management.UpdatePersistentVMRole(
                certificate,
                sgSubscriptionId.Value,
                serviceName.Value,
                result.Name,
                vmToAdd);

            PushOperationStatus(
                certificate,
                sgSubscriptionId.Value,
                new Guid(output[ITPCfSQL.Azure.Internal.Constants.HEADER_REQUEST_ID]),
                fBlocking.IsNull ? false : fBlocking.Value);
        }

        [SqlFunction(
                DataAccess = DataAccessKind.None,
                SystemDataAccess = SystemDataAccessKind.None,
                IsDeterministic = false,
                IsPrecise = true)]
        public static SqlString GetOperationStatus(
            SqlString certificateThumbprint,
            SqlGuid sgSubscriptionId,
            SqlGuid sgOperationId)
        {
            X509Certificate2 certificate = RetrieveCertificateInStore(certificateThumbprint.Value);
            return ITPCfSQL.Azure.Internal.Management.GetOperationStatus(certificate, sgSubscriptionId.Value, sgOperationId.Value).Status;
        }

        #region Support methods
        private static void PushOperationStatus
            (X509Certificate2 cert,
            Guid gSubscriptionId,
            Guid gOperationId,
            bool fBlocking,
            int iSleepMS = 1000)
        {
            Azure.Management.Operation op = ITPCfSQL.Azure.Internal.Management.GetOperationStatus(cert, gSubscriptionId, gOperationId);

            if (fBlocking)
            {
                while (op.Status == "InProgress")
                {
                    System.Threading.Thread.Sleep(iSleepMS);
                    op = ITPCfSQL.Azure.Internal.Management.GetOperationStatus(cert, gSubscriptionId, gOperationId);
                }
            }

            PushOperationStatus(op);
        }

        private static void PushOperationStatus
             (Azure.Management.Operation op)
        {
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData[] 
                {
                    new SqlMetaData("OperationId", System.Data.SqlDbType.NVarChar, 255),
                    new SqlMetaData("Status", System.Data.SqlDbType.NVarChar, 255)
                });

            SqlContext.Pipe.SendResultsStart(sdr);
            sdr.SetString(0, op.ID);
            sdr.SetString(1, op.Status);
            SqlContext.Pipe.SendResultsRow(sdr);
            SqlContext.Pipe.SendResultsEnd();
        }
        #endregion

        #region Callbacks
        public static void _GetServicesCallback(
            object obj,
            out SqlString ServiceName,
            out SqlString Url,
            out SqlString DefaultWinRmCertificateThumbprint,
            out SqlString AffinityGroup,
            out SqlDateTime DateCreated,
            out SqlDateTime DateLastModified,
            out SqlString Description,
            out SqlString Label,
            out SqlString Location,
            out SqlString Status)
        {
            Azure.Management.HostedServicesHostedService hs = (Azure.Management.HostedServicesHostedService)obj;

            DefaultWinRmCertificateThumbprint = hs.DefaultWinRmCertificateThumbprint;
            Url = hs.Url;
            ServiceName = hs.ServiceName;

            AffinityGroup = hs.HostedServiceProperties.AffinityGroup;
            DateCreated = hs.HostedServiceProperties.DateCreated;
            DateLastModified = hs.HostedServiceProperties.DateLastModified;
            Description = hs.HostedServiceProperties.Description;

            Label = hs.HostedServiceProperties.Label;
            Location = hs.HostedServiceProperties.Location;
            Status = hs.HostedServiceProperties.Status;
        }

        public static void GetDeploymentsPersistentVMRolesWithInputEndpointsCallback(
            object obj,
            out SqlString Name,
            out SqlString DeploymentSlot,
            out SqlString PrivateID,
            out SqlString Status,
            out SqlString Label,
            out SqlString Url,
            out SqlString Configuration,
            out SqlInt32 UpgradeDomainCount,
            out SqlString VMName,
            out SqlString OsVersion,
            out SqlString RoleSize,
            out SqlString DefaultWinRmCertificateThumbprint,

            out SqlString EndpointName,
            out SqlInt32 LocalPort,
            out SqlInt32 Port,
            out SqlString Protocol,
            out SqlString Vip)
        {
            CallbackSupport.DeploymentsPersistentVMRolesWithInputEndpoints item = (CallbackSupport.DeploymentsPersistentVMRolesWithInputEndpoints)obj;

            Name = item.Deployment.Name;
            DeploymentSlot = item.Deployment.DeploymentSlot;
            PrivateID = item.Deployment.PrivateID;
            Status = item.Deployment.Status;
            Label = item.Deployment.Label;
            Url = item.Deployment.Url;
            Configuration = item.Deployment.Configuration;

            UpgradeDomainCount = item.Deployment.UpgradeDomainCount;

            VMName = item.DeploymentRole.RoleName;
            OsVersion = item.DeploymentRole.OsVersion == null ? null : item.DeploymentRole.OsVersion.ToString();
            RoleSize = item.DeploymentRole.RoleSize;
            DefaultWinRmCertificateThumbprint = item.DeploymentRole.DefaultWinRmCertificateThumbprint;

            EndpointName = item.InputEndpoint.Name;
            LocalPort = item.InputEndpoint.LocalPort;
            Port = item.InputEndpoint.Port;
            Protocol = item.InputEndpoint.Protocol;
            Vip = item.InputEndpoint.Vip;
        }
        #endregion
    }
}
