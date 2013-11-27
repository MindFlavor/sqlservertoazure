using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Management
{
    #region Deployment
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class Deployment
    {

        private string nameField;

        private string deploymentSlotField;

        private string privateIDField;

        private string statusField;

        private string labelField;

        private string urlField;

        private string configurationField;

        private DeploymentRoleInstance[] roleInstanceListField;

        private byte upgradeDomainCountField;

        private DeploymentRole[] roleListField;

        private object sdkVersionField;

        private bool lockedField;

        private bool rollbackAllowedField;

        private System.DateTime createdTimeField;

        private System.DateTime lastModifiedTimeField;

        private object extendedPropertiesField;

        private DeploymentPersistentVMDowntime persistentVMDowntimeField;

        private DeploymentVirtualIPs virtualIPsField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string DeploymentSlot
        {
            get
            {
                return this.deploymentSlotField;
            }
            set
            {
                this.deploymentSlotField = value;
            }
        }

        /// <remarks/>
        public string PrivateID
        {
            get
            {
                return this.privateIDField;
            }
            set
            {
                this.privateIDField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string Label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public string Configuration
        {
            get
            {
                return this.configurationField;
            }
            set
            {
                this.configurationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("RoleInstance", IsNullable = false)]
        public DeploymentRoleInstance[] RoleInstanceList
        {
            get
            {
                return this.roleInstanceListField;
            }
            set
            {
                this.roleInstanceListField = value;
            }
        }

        /// <remarks/>
        public byte UpgradeDomainCount
        {
            get
            {
                return this.upgradeDomainCountField;
            }
            set
            {
                this.upgradeDomainCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Role", IsNullable = false)]
        public DeploymentRole[] RoleList
        {
            get
            {
                return this.roleListField;
            }
            set
            {
                this.roleListField = value;
            }
        }

        /// <remarks/>
        public object SdkVersion
        {
            get
            {
                return this.sdkVersionField;
            }
            set
            {
                this.sdkVersionField = value;
            }
        }

        /// <remarks/>
        public bool Locked
        {
            get
            {
                return this.lockedField;
            }
            set
            {
                this.lockedField = value;
            }
        }

        /// <remarks/>
        public bool RollbackAllowed
        {
            get
            {
                return this.rollbackAllowedField;
            }
            set
            {
                this.rollbackAllowedField = value;
            }
        }

        /// <remarks/>
        public System.DateTime CreatedTime
        {
            get
            {
                return this.createdTimeField;
            }
            set
            {
                this.createdTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime LastModifiedTime
        {
            get
            {
                return this.lastModifiedTimeField;
            }
            set
            {
                this.lastModifiedTimeField = value;
            }
        }

        /// <remarks/>
        public object ExtendedProperties
        {
            get
            {
                return this.extendedPropertiesField;
            }
            set
            {
                this.extendedPropertiesField = value;
            }
        }

        /// <remarks/>
        public DeploymentPersistentVMDowntime PersistentVMDowntime
        {
            get
            {
                return this.persistentVMDowntimeField;
            }
            set
            {
                this.persistentVMDowntimeField = value;
            }
        }

        /// <remarks/>
        public DeploymentVirtualIPs VirtualIPs
        {
            get
            {
                return this.virtualIPsField;
            }
            set
            {
                this.virtualIPsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleInstance
    {

        private string roleNameField;

        private string instanceNameField;

        private string instanceStatusField;

        private byte instanceUpgradeDomainField;

        private byte instanceFaultDomainField;

        private string instanceSizeField;

        private object instanceStateDetailsField;

        private string ipAddressField;

        private DeploymentRoleInstanceInstanceEndpoint[] instanceEndpointsField;

        private string powerStateField;

        private string hostNameField;

        private string remoteAccessCertificateThumbprintField;

        /// <remarks/>
        public string RoleName
        {
            get
            {
                return this.roleNameField;
            }
            set
            {
                this.roleNameField = value;
            }
        }

        /// <remarks/>
        public string InstanceName
        {
            get
            {
                return this.instanceNameField;
            }
            set
            {
                this.instanceNameField = value;
            }
        }

        /// <remarks/>
        public string InstanceStatus
        {
            get
            {
                return this.instanceStatusField;
            }
            set
            {
                this.instanceStatusField = value;
            }
        }

        /// <remarks/>
        public byte InstanceUpgradeDomain
        {
            get
            {
                return this.instanceUpgradeDomainField;
            }
            set
            {
                this.instanceUpgradeDomainField = value;
            }
        }

        /// <remarks/>
        public byte InstanceFaultDomain
        {
            get
            {
                return this.instanceFaultDomainField;
            }
            set
            {
                this.instanceFaultDomainField = value;
            }
        }

        /// <remarks/>
        public string InstanceSize
        {
            get
            {
                return this.instanceSizeField;
            }
            set
            {
                this.instanceSizeField = value;
            }
        }

        /// <remarks/>
        public object InstanceStateDetails
        {
            get
            {
                return this.instanceStateDetailsField;
            }
            set
            {
                this.instanceStateDetailsField = value;
            }
        }

        /// <remarks/>
        public string IpAddress
        {
            get
            {
                return this.ipAddressField;
            }
            set
            {
                this.ipAddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("InstanceEndpoint", IsNullable = false)]
        public DeploymentRoleInstanceInstanceEndpoint[] InstanceEndpoints
        {
            get
            {
                return this.instanceEndpointsField;
            }
            set
            {
                this.instanceEndpointsField = value;
            }
        }

        /// <remarks/>
        public string PowerState
        {
            get
            {
                return this.powerStateField;
            }
            set
            {
                this.powerStateField = value;
            }
        }

        /// <remarks/>
        public string HostName
        {
            get
            {
                return this.hostNameField;
            }
            set
            {
                this.hostNameField = value;
            }
        }

        /// <remarks/>
        public string RemoteAccessCertificateThumbprint
        {
            get
            {
                return this.remoteAccessCertificateThumbprintField;
            }
            set
            {
                this.remoteAccessCertificateThumbprintField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleInstanceInstanceEndpoint
    {

        private string nameField;

        private string vipField;

        private ushort publicPortField;

        private ushort localPortField;

        private string protocolField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Vip
        {
            get
            {
                return this.vipField;
            }
            set
            {
                this.vipField = value;
            }
        }

        /// <remarks/>
        public ushort PublicPort
        {
            get
            {
                return this.publicPortField;
            }
            set
            {
                this.publicPortField = value;
            }
        }

        /// <remarks/>
        public ushort LocalPort
        {
            get
            {
                return this.localPortField;
            }
            set
            {
                this.localPortField = value;
            }
        }

        /// <remarks/>
        public string Protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlInclude(typeof(PersistentVMRole))]
    public partial class DeploymentRole
    {

        private string roleNameField;

        private object osVersionField;

        private string roleTypeField;

        private DeploymentRoleConfigurationSets configurationSetsField;

        private DeploymentRoleDataVirtualHardDisks dataVirtualHardDisksField;

        private DeploymentRoleOSVirtualHardDisk oSVirtualHardDiskField;

        private string roleSizeField;

        private string defaultWinRmCertificateThumbprintField;

        /// <remarks/>
        public string RoleName
        {
            get
            {
                return this.roleNameField;
            }
            set
            {
                this.roleNameField = value;
            }
        }

        /// <remarks/>
        public object OsVersion
        {
            get
            {
                return this.osVersionField;
            }
            set
            {
                this.osVersionField = value;
            }
        }

        /// <remarks/>
        public string RoleType
        {
            get
            {
                return this.roleTypeField;
            }
            set
            {
                this.roleTypeField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleConfigurationSets ConfigurationSets
        {
            get
            {
                return this.configurationSetsField;
            }
            set
            {
                this.configurationSetsField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleDataVirtualHardDisks DataVirtualHardDisks
        {
            get
            {
                return this.dataVirtualHardDisksField;
            }
            set
            {
                this.dataVirtualHardDisksField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleOSVirtualHardDisk OSVirtualHardDisk
        {
            get
            {
                return this.oSVirtualHardDiskField;
            }
            set
            {
                this.oSVirtualHardDiskField = value;
            }
        }

        /// <remarks/>
        public string RoleSize
        {
            get
            {
                return this.roleSizeField;
            }
            set
            {
                this.roleSizeField = value;
            }
        }

        /// <remarks/>
        public string DefaultWinRmCertificateThumbprint
        {
            get
            {
                return this.defaultWinRmCertificateThumbprintField;
            }
            set
            {
                this.defaultWinRmCertificateThumbprintField = value;
            }
        }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class PersistentVMRole : DeploymentRole
    { }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleConfigurationSets
    {

        private DeploymentRoleConfigurationSetsConfigurationSet configurationSetField;

        /// <remarks/>
        public DeploymentRoleConfigurationSetsConfigurationSet ConfigurationSet
        {
            get
            {
                return this.configurationSetField;
            }
            set
            {
                this.configurationSetField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlInclude(typeof(NetworkConfigurationSet))]
    public partial class DeploymentRoleConfigurationSetsConfigurationSet
    {

        private string configurationSetTypeField;

        private DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint[] inputEndpointsField;

        private object subnetNamesField;

        /// <remarks/>
        public string ConfigurationSetType
        {
            get
            {
                return this.configurationSetTypeField;
            }
            set
            {
                this.configurationSetTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("InputEndpoint", IsNullable = false)]
        public DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint[] InputEndpoints
        {
            get
            {
                return this.inputEndpointsField;
            }
            set
            {
                this.inputEndpointsField = value;
            }
        }

        /// <remarks/>
        public object SubnetNames
        {
            get
            {
                return this.subnetNamesField;
            }
            set
            {
                this.subnetNamesField = value;
            }
        }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class NetworkConfigurationSet : DeploymentRoleConfigurationSetsConfigurationSet
    { }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint
    {

        private int localPortField;

        private string nameField;

        private int portField;

        private string protocolField;

        private string vipField;

        private bool enableDirectServerReturnField;

        /// <remarks/>
        public int LocalPort
        {
            get
            {
                return this.localPortField;
            }
            set
            {
                this.localPortField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public int Port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public string Protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }

        /// <remarks/>
        public string Vip
        {
            get
            {
                return this.vipField;
            }
            set
            {
                this.vipField = value;
            }
        }

        /// <remarks/>
        public bool EnableDirectServerReturn
        {
            get
            {
                return this.enableDirectServerReturnField;
            }
            set
            {
                this.enableDirectServerReturnField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleDataVirtualHardDisks
    {

        private DeploymentRoleDataVirtualHardDisksDataVirtualHardDisk dataVirtualHardDiskField;

        /// <remarks/>
        public DeploymentRoleDataVirtualHardDisksDataVirtualHardDisk DataVirtualHardDisk
        {
            get
            {
                return this.dataVirtualHardDiskField;
            }
            set
            {
                this.dataVirtualHardDiskField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleDataVirtualHardDisksDataVirtualHardDisk
    {

        private string hostCachingField;

        private string diskLabelField;

        private string diskNameField;

        private byte lunField;

        private bool lunFieldSpecified;

        private byte logicalDiskSizeInGBField;

        private string mediaLinkField;

        /// <remarks/>
        public string HostCaching
        {
            get
            {
                return this.hostCachingField;
            }
            set
            {
                this.hostCachingField = value;
            }
        }

        /// <remarks/>
        public string DiskLabel
        {
            get
            {
                return this.diskLabelField;
            }
            set
            {
                this.diskLabelField = value;
            }
        }

        /// <remarks/>
        public string DiskName
        {
            get
            {
                return this.diskNameField;
            }
            set
            {
                this.diskNameField = value;
            }
        }

        /// <remarks/>
        public byte Lun
        {
            get
            {
                return this.lunField;
            }
            set
            {
                this.lunField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LunSpecified
        {
            get
            {
                return this.lunFieldSpecified;
            }
            set
            {
                this.lunFieldSpecified = value;
            }
        }

        /// <remarks/>
        public byte LogicalDiskSizeInGB
        {
            get
            {
                return this.logicalDiskSizeInGBField;
            }
            set
            {
                this.logicalDiskSizeInGBField = value;
            }
        }

        /// <remarks/>
        public string MediaLink
        {
            get
            {
                return this.mediaLinkField;
            }
            set
            {
                this.mediaLinkField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleOSVirtualHardDisk
    {

        private string hostCachingField;

        private string diskNameField;

        private string mediaLinkField;

        private string sourceImageNameField;

        private string osField;

        /// <remarks/>
        public string HostCaching
        {
            get
            {
                return this.hostCachingField;
            }
            set
            {
                this.hostCachingField = value;
            }
        }

        /// <remarks/>
        public string DiskName
        {
            get
            {
                return this.diskNameField;
            }
            set
            {
                this.diskNameField = value;
            }
        }

        /// <remarks/>
        public string MediaLink
        {
            get
            {
                return this.mediaLinkField;
            }
            set
            {
                this.mediaLinkField = value;
            }
        }

        /// <remarks/>
        public string SourceImageName
        {
            get
            {
                return this.sourceImageNameField;
            }
            set
            {
                this.sourceImageNameField = value;
            }
        }

        /// <remarks/>
        public string OS
        {
            get
            {
                return this.osField;
            }
            set
            {
                this.osField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentPersistentVMDowntime
    {

        private System.DateTime startTimeField;

        private System.DateTime endTimeField;

        private string statusField;

        /// <remarks/>
        public System.DateTime StartTime
        {
            get
            {
                return this.startTimeField;
            }
            set
            {
                this.startTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime EndTime
        {
            get
            {
                return this.endTimeField;
            }
            set
            {
                this.endTimeField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentVirtualIPs
    {

        private DeploymentVirtualIPsVirtualIP virtualIPField;

        /// <remarks/>
        public DeploymentVirtualIPsVirtualIP VirtualIP
        {
            get
            {
                return this.virtualIPField;
            }
            set
            {
                this.virtualIPField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentVirtualIPsVirtualIP
    {

        private string addressField;

        private bool isDnsProgrammedField;

        private string nameField;

        /// <remarks/>
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public bool IsDnsProgrammed
        {
            get
            {
                return this.isDnsProgrammedField;
            }
            set
            {
                this.isDnsProgrammedField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
    #endregion
}
