using System;
using System.Collections.Generic;
using System.Text;

namespace ITPCfSQL.Azure
{
    public class AzureTableService : AzureService
    {
        public AzureTableService(string AccountName, string SharedKey, bool UseHTTPS)
            : base(AccountName, SharedKey, UseHTTPS)
        { }

        public System.Collections.Generic.IEnumerable<Table> ListTables(string xmsclientrequestId = null)
        {
            List<Table> lTables = new List<Table>();

            string strResult = Internal.InternalMethods.QueryTables(UseHTTPS, SharedKey, AccountName, xmsclientrequestId);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            using (System.Xml.XmlReader re = System.Xml.XmlReader.Create(new System.IO.StringReader(strResult)))
            {
                doc.Load(re);
            }
            System.Xml.XmlNamespaceManager man = new System.Xml.XmlNamespaceManager(doc.NameTable);
            man.AddNamespace("f", "http://www.w3.org/2005/Atom");
            man.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            man.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");

            foreach (System.Xml.XmlNode n in doc.SelectNodes("//f:feed/f:entry", man))
            {
                yield return new Table()
                {
                    AzureTableService = this,
                    Url = new Uri(n.SelectSingleNode("f:id", man).InnerText),
                    Name = n.SelectSingleNode("f:content/m:properties/d:TableName", man).InnerText
                };

            }
        }

        public Table GetTable(
            string tableName)
        {
            return new Table(this, tableName);
        }

        public Table CreateTable(
            string tableName,
            string xmsclientrequestId = null)
        {

            string strResult = Internal.InternalMethods.CreateTable(UseHTTPS, SharedKey, AccountName, tableName, xmsclientrequestId);
            Table t = new Table()
            {
                AzureTableService = this,
                Name = tableName                
            };
            t.Url = Table.GenerateUriFromParameters(t);

            return t;
        }
    }
}
