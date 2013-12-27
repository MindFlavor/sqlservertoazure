using System;
using System.Collections.Generic;
using System.Text;

namespace ITPCfSQL.Azure
{
    public class Table
    {
        #region Properties
        public AzureTableService AzureTableService { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }
        #endregion

        internal Table() { }

        public Table(AzureTableService AzureTableService, string Name)
        {
            this.AzureTableService = AzureTableService;
            this.Name = Name;
            Url = GenerateUriFromParameters(this);
        }

        public static Uri GenerateUriFromParameters(Table t)
        {
            return new Uri(string.Format("{0:S}://{1:S}.table.core.windows.net/Tables('{2:S}')",
                t.AzureTableService.UseHTTPS ? "https" : "http", t.AzureTableService.AccountName, t.Name));
        }

        public void InsertOrUpdate(TableEntity te, string xmsclientrequestId = null)
        {
            ITPCfSQL.Azure.Internal.InternalMethods.InsertOrReplaceTableEntity(
                 AzureTableService.AccountName, AzureTableService.SharedKey, AzureTableService.UseHTTPS, this.Name, te, xmsclientrequestId);
        }

        public void Delete(TableEntity te, string xmsclientrequestId = null)
        {
            ITPCfSQL.Azure.Internal.InternalMethods.DeleteEntity(
                 AzureTableService.AccountName, AzureTableService.SharedKey, AzureTableService.UseHTTPS, this.Name, te, xmsclientrequestId);
        }

        public void Drop(Guid? xmsclientrequestId = null)
        {
            ITPCfSQL.Azure.Internal.InternalMethods.DeleteTable(
                 AzureTableService.AccountName, AzureTableService.SharedKey, AzureTableService.UseHTTPS, this.Name, xmsclientrequestId);
        }

        public System.Collections.Generic.IEnumerable<TableEntity> Query(string xmsclientrequestId = null)
        {
            List<TableEntity> lEntities = new List<TableEntity>();
            string res = ITPCfSQL.Azure.Internal.InternalMethods.QueryTable(
                AzureTableService.UseHTTPS, AzureTableService.SharedKey, AzureTableService.AccountName, this.Name, xmsclientrequestId);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            using (System.IO.StringReader sr = new System.IO.StringReader(res))
            {
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(sr))
                {
                    doc.Load(reader);
                }
            }

            System.Xml.XmlNamespaceManager man = new System.Xml.XmlNamespaceManager(doc.NameTable);
            man.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            man.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            man.AddNamespace("f", "http://www.w3.org/2005/Atom");

            foreach (System.Xml.XmlNode node in doc.SelectNodes("f:feed/f:entry", man))
            {
                System.Xml.XmlNode nCont = node.SelectSingleNode("f:content/m:properties", man);
                System.Xml.XmlNode nTimeStamp = nCont.SelectSingleNode("d:Timestamp", man);
                TableEntity te = new TableEntity()
                    {
                        PartitionKey = nCont.SelectSingleNode("d:PartitionKey", man).InnerText,
                        RowKey = nCont.SelectSingleNode("d:RowKey", man).InnerText,
                        TimeStamp = DateTime.Parse(nTimeStamp.InnerText)
                    };

                // custom attrib handling
                System.Xml.XmlNode nFollow = nTimeStamp.NextSibling;
                while (nFollow != null)
                {
                    te.Attributes[nFollow.LocalName] = nFollow.InnerText;
                    nFollow = nFollow.NextSibling;
                }

                yield return te;
            }
        }
    }
}
