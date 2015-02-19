using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Responses.SQLDatabase.ListServers
{


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sqlazure/2010/12/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/sqlazure/2010/12/", IsNullable = false, ElementName = "Servers")]
    public partial class ResponseWithoutGeneric
    {

        private ServersServer[] serverField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Server")]
        public ServersServer[] Server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/sqlazure/2010/12/")]
    public partial class ServersServer
    {

        private string nameField;

        private string administratorLoginField;

        private string locationField;

        private string fullyQualifiedDomainNameField;

        private decimal versionField;

        private string stateField;

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
        public string AdministratorLogin
        {
            get
            {
                return this.administratorLoginField;
            }
            set
            {
                this.administratorLoginField = value;
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

        /// <remarks/>
        public decimal Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
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
    }




}
