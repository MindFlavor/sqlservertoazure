using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Responses.SQLDatabase.ListDatabases
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false, ElementName = "ServiceResources")]
    public partial class Response
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

        private string parentLinkField;

        private byte idField;

        private string editionField;

        private byte maxSizeGBField;

        private bool maxSizeGBFieldSpecified;

        private string collationNameField;

        private System.DateTime creationDateField;

        private string isFederationRootField;

        private string isSystemObjectField;

        private object sizeMBField;

        private ulong maxSizeBytesField;

        private string serviceObjectiveIdField;

        private string assignedServiceObjectiveIdField;

        private string serviceObjectiveAssignmentStateField;

        private string serviceObjectiveAssignmentStateDescriptionField;

        private byte serviceObjectiveAssignmentErrorCodeField;

        private object serviceObjectiveAssignmentErrorDescriptionField;

        private string serviceObjectiveAssignmentSuccessDateField;

        private string recoveryPeriodStartDateField;

        private string isSuspendedField;

        private string isEncryptedField;

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
        public string ParentLink
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
        public byte Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string Edition
        {
            get
            {
                return this.editionField;
            }
            set
            {
                this.editionField = value;
            }
        }

        /// <remarks/>
        public byte MaxSizeGB
        {
            get
            {
                return this.maxSizeGBField;
            }
            set
            {
                this.maxSizeGBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MaxSizeGBSpecified
        {
            get
            {
                return this.maxSizeGBFieldSpecified;
            }
            set
            {
                this.maxSizeGBFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string CollationName
        {
            get
            {
                return this.collationNameField;
            }
            set
            {
                this.collationNameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime CreationDate
        {
            get
            {
                return this.creationDateField;
            }
            set
            {
                this.creationDateField = value;
            }
        }

        /// <remarks/>
        public string IsFederationRoot
        {
            get
            {
                return this.isFederationRootField;
            }
            set
            {
                this.isFederationRootField = value;
            }
        }

        /// <remarks/>
        public string IsSystemObject
        {
            get
            {
                return this.isSystemObjectField;
            }
            set
            {
                this.isSystemObjectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object SizeMB
        {
            get
            {
                return this.sizeMBField;
            }
            set
            {
                this.sizeMBField = value;
            }
        }

        /// <remarks/>
        public ulong MaxSizeBytes
        {
            get
            {
                return this.maxSizeBytesField;
            }
            set
            {
                this.maxSizeBytesField = value;
            }
        }

        /// <remarks/>
        public string ServiceObjectiveId
        {
            get
            {
                return this.serviceObjectiveIdField;
            }
            set
            {
                this.serviceObjectiveIdField = value;
            }
        }

        /// <remarks/>
        public string AssignedServiceObjectiveId
        {
            get
            {
                return this.assignedServiceObjectiveIdField;
            }
            set
            {
                this.assignedServiceObjectiveIdField = value;
            }
        }

        /// <remarks/>
        public string ServiceObjectiveAssignmentState
        {
            get
            {
                return this.serviceObjectiveAssignmentStateField;
            }
            set
            {
                this.serviceObjectiveAssignmentStateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ServiceObjectiveAssignmentStateDescription
        {
            get
            {
                return this.serviceObjectiveAssignmentStateDescriptionField;
            }
            set
            {
                this.serviceObjectiveAssignmentStateDescriptionField = value;
            }
        }

        /// <remarks/>
        public byte ServiceObjectiveAssignmentErrorCode
        {
            get
            {
                return this.serviceObjectiveAssignmentErrorCodeField;
            }
            set
            {
                this.serviceObjectiveAssignmentErrorCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object ServiceObjectiveAssignmentErrorDescription
        {
            get
            {
                return this.serviceObjectiveAssignmentErrorDescriptionField;
            }
            set
            {
                this.serviceObjectiveAssignmentErrorDescriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ServiceObjectiveAssignmentSuccessDate
        {
            get
            {
                return this.serviceObjectiveAssignmentSuccessDateField;
            }
            set
            {
                this.serviceObjectiveAssignmentSuccessDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string RecoveryPeriodStartDate
        {
            get
            {
                return this.recoveryPeriodStartDateField;
            }
            set
            {
                this.recoveryPeriodStartDateField = value;
            }
        }

        /// <remarks/>
        public string IsSuspended
        {
            get
            {
                return this.isSuspendedField;
            }
            set
            {
                this.isSuspendedField = value;
            }
        }

        /// <remarks/>
        public string IsEncrypted
        {
            get
            {
                return this.isEncryptedField;
            }
            set
            {
                this.isEncryptedField = value;
            }
        }
    }


}
