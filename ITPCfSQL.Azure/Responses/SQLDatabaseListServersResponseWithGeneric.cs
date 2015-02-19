using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Responses
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false, ElementName = "ServiceResources")]
    public partial class SQLDatabaseListServersResponseWithGeneric
    {

        private ServiceResourcesServiceResource[] serviceResourceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ServiceResource")]
        public ServiceResourcesServiceResource[] ServiceResource
        {
            get
            {
                return this.serviceResourceField;
            }
            set
            {
                this.serviceResourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class ServiceResourcesServiceResource
    {

        private string nameField;

        private string typeField;

        private string stateField;

        private string selfLinkField;

        private object parentLinkField;

        private string fullyQualifiedDomainNameField;

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
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string SelfLink
        {
            get
            {
                return this.selfLinkField;
            }
            set
            {
                this.selfLinkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ParentLink
        {
            get
            {
                return this.parentLinkField;
            }
            set
            {
                this.parentLinkField = value;
            }
        }

        /// <remarks/>
        public string FullyQualifiedDomainName
        {
            get
            {
                return this.fullyQualifiedDomainNameField;
            }
            set
            {
                this.fullyQualifiedDomainNameField = value;
            }
        }
    }


}
