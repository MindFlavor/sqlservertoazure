using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Management
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class HostedServices
    {

        private HostedServicesHostedService[] hostedServiceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("HostedService")]
        public HostedServicesHostedService[] HostedService
        {
            get
            {
                return this.hostedServiceField;
            }
            set
            {
                this.hostedServiceField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class HostedServicesHostedService
    {

        private string urlField;

        private string serviceNameField;

        private HostedServicesHostedServiceHostedServiceProperties hostedServicePropertiesField;

        private string defaultWinRmCertificateThumbprintField;

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
        public string ServiceName
        {
            get
            {
                return this.serviceNameField;
            }
            set
            {
                this.serviceNameField = value;
            }
        }

        /// <remarks/>
        public HostedServicesHostedServiceHostedServiceProperties HostedServiceProperties
        {
            get
            {
                return this.hostedServicePropertiesField;
            }
            set
            {
                this.hostedServicePropertiesField = value;
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

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class HostedServicesHostedServiceHostedServiceProperties
    {

        private string descriptionField;

        private string affinityGroupField;

        private string locationField;

        private string labelField;

        private string statusField;

        private System.DateTime dateCreatedField;

        private System.DateTime dateLastModifiedField;

        private object extendedPropertiesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string AffinityGroup
        {
            get
            {
                return this.affinityGroupField;
            }
            set
            {
                this.affinityGroupField = value;
            }
        }

        /// <remarks/>
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
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
        public System.DateTime DateCreated
        {
            get
            {
                return this.dateCreatedField;
            }
            set
            {
                this.dateCreatedField = value;
            }
        }

        /// <remarks/>
        public System.DateTime DateLastModified
        {
            get
            {
                return this.dateLastModifiedField;
            }
            set
            {
                this.dateLastModifiedField = value;
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
    }


}
