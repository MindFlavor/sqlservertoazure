using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ITPCfSQL.Azure.Internal
{
    public class Management
    {
        public const string AZURE_VERSION = "2013-06-01";

        internal static string GetManagementURI()
        {
            return "https://management.core.windows.net";
        }

        // TODO from XML to classes!
        public static ITPCfSQL.Azure.Management.HostedServices GetServices(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "hostedservices");

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "GET";
            Request.Headers.Add(Constants.HEADER_VERSION, AZURE_VERSION);
            Request.ClientCertificates.Add(certificate);

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(
                    typeof(ITPCfSQL.Azure.Management.HostedServices));

                return (ITPCfSQL.Azure.Management.HostedServices)ser.Deserialize(response.GetResponseStream());
            }
            else
                throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        public static ITPCfSQL.Azure.Management.Deployment GetDeployments(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId, string serviceName, string deploymentSlot = "Production")
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "hostedservices" + "/" +
                serviceName + "/" +
                "deploymentslots" + "/" +
                deploymentSlot);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "GET";
            Request.Headers.Add(Constants.HEADER_VERSION, AZURE_VERSION);
            Request.ClientCertificates.Add(certificate);

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(
                    typeof(ITPCfSQL.Azure.Management.Deployment));

                return (ITPCfSQL.Azure.Management.Deployment)ser.Deserialize(response.GetResponseStream());
            }
            else
                throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        public static Dictionary<string, string> UpdatePersistentVMRole(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId,
            string serviceName,
            string deploymentName,
            ITPCfSQL.Azure.Management.PersistentVMRole vmRoleConfig)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "hostedservices" + "/" +
                serviceName + "/" +
                "deployments" + "/" +
                deploymentName + "/" +
                "Roles" + "/" +
                vmRoleConfig.RoleName);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "PUT";
            Request.Headers.Add(Constants.HEADER_VERSION, AZURE_VERSION);
            Request.ClientCertificates.Add(certificate);

            #region Add XML data
            Request.ContentType = "application/xml; charset=utf-8";

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(ITPCfSQL.Azure.Management.PersistentVMRole));
            ser.Serialize(Request.GetRequestStream(), vmRoleConfig);
            #endregion

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Dictionary<string, string> dRes = new Dictionary<string, string>();
                foreach (string header in response.Headers)
                {
                    dRes.Add(header, response.Headers[header]);
                }

                return dRes;
            }
            else
                throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Accepted, response.StatusCode);
        }

        public static XmlDocument GetOperationStatus(
             System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
             Guid subscriptionId, string operationId)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "operations" + "/" +
                operationId);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "GET";
            Request.Headers.Add(Constants.HEADER_VERSION, AZURE_VERSION);
            Request.ClientCertificates.Add(certificate);

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.Xml.XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());

                return doc;
            }
            else
                throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
