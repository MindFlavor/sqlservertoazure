using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.SharedAccessSignature
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "SignedIdentifiers")]
    public partial class SharedAccessSignatureACL
    {
        [System.Xml.Serialization.XmlElementAttribute("SignedIdentifier")]
        public List<SignedIdentifier> SignedIdentifier { get; set; }
    }

    public class SignedIdentifier
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value.Length > 64)
                    throw new ArgumentException("SignedIdentifier Ids must be 64 chars or less.");

                _id = value;
            }
        }

        public AccessPolicy AccessPolicy { get; set; }
    }

    public class AccessPolicy
    {
        [System.Xml.Serialization.XmlElementAttribute("Start")]
        public string StartSerialized
        {
            get { return Start.ToString("yyyy-MM-ddTHH:mm:ssZ"); }
            set { Start = DateTime.Parse(value); }
        }

        [System.Xml.Serialization.XmlElementAttribute("Expiry")]
        public string ExpirySerialized
        {
            get { return Expiry.ToString("yyyy-MM-ddTHH:mm:ssZ"); }
            set { Expiry = DateTime.Parse(value); }
        }

        [System.Xml.Serialization.XmlIgnore]
        public DateTime Start { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public DateTime Expiry { get; set; }

        public string Permission { get; set; }
    }
}
