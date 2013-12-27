using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace ITPCfSQL.Azure.CLR
{
    public class Table
    {
        #region Full-Blown
        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_ListTablesCallback",
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX)")]
        public static System.Collections.IEnumerable ListTables(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureTableService ats = new AzureTableService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            return ats.ListTables(xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }

        [SqlProcedure]
        public static void CreateTable(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString tableName,
            SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureTableService ats = new AzureTableService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ats.CreateTable(tableName.Value, xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }

        [SqlProcedure]
        public static void DropTable(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString tableName,
            SqlGuid xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureTableService ats = new AzureTableService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ats.GetTable(tableName.Value).Drop(
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }

        [SqlProcedure]
        public static void InsertOrReplaceEntity(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString tableName,
            SqlString partitionKey,
            SqlString rowKey,
            SqlXml AttributeList,
            SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureTableService ats = new AzureTableService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ITPCfSQL.Azure.Table table = ats.GetTable(tableName.Value);

            ITPCfSQL.Azure.TableEntity te = new TableEntity(partitionKey.Value, rowKey.Value);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(AttributeList.Value);

            foreach (System.Xml.XmlNode nAttrib in doc.FirstChild.ChildNodes)
            {
                te.Attributes[nAttrib.Name] = nAttrib.InnerText;
            }

            table.InsertOrUpdate(te,
                xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }

        [SqlProcedure]
        public static void DeleteEntity(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString tableName,
            SqlString partitionKey,
            SqlString rowKey,
            SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureTableService ats = new AzureTableService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ITPCfSQL.Azure.Table table = ats.GetTable(tableName.Value);
            table.Delete(new ITPCfSQL.Azure.TableEntity() { RowKey = rowKey.Value, PartitionKey = partitionKey.Value, TimeStamp = DateTime.Now },
                (xmsclientrequestId != null ? xmsclientrequestId.Value : null));
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_QueryTableCallback",
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "PartitionKey NVARCHAR(4000), RowKey NVARCHAR(4000), TimeStamp DATETIME, Attributes XML")]
        public static System.Collections.IEnumerable QueryTable(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString tableName,
            SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureTableService ats = new AzureTableService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ITPCfSQL.Azure.Table t = ats.GetTable(tableName.Value);

            return t.Query(xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }
        #endregion

        #region Embedded
        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_ListTablesCallback",
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX)")]
        public static System.Collections.IEnumerable ListTables_Embedded(
            SqlString logicalConnectionName,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return ListTables(config.AccountName, config.SharedKey, config.UseHTTPS,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void CreateTable_Embedded(
            SqlString logicalConnectionName,
            SqlString tableName,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            CreateTable(config.AccountName, config.SharedKey, config.UseHTTPS,
                tableName, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void InsertOrReplaceEntity_Embedded(
            SqlString logicalConnectionName,
            SqlString tableName,
            SqlString partitionKey,
            SqlString rowKey,
            SqlXml AttributeList,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            InsertOrReplaceEntity(config.AccountName, config.SharedKey, config.UseHTTPS,
                tableName, partitionKey, rowKey, AttributeList, xmsclientrequestId);
        }

        public static void DeleteEntity_Embedded(
            SqlString logicalConnectionName,
            SqlString tableName,
            SqlString partitionKey,
            SqlString rowKey,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            DeleteEntity(config.AccountName, config.SharedKey, config.UseHTTPS,
                tableName, partitionKey, rowKey, xmsclientrequestId);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_QueryTableCallback",
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "PartitionKey NVARCHAR(4000), RowKey NVARCHAR(4000), TimeStamp DATETIME, Attributes XML")]
        public static System.Collections.IEnumerable QueryTable_Embedded(
            SqlString logicalConnectionName,
            SqlString tableName,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return QueryTable(config.AccountName, config.SharedKey, config.UseHTTPS,
                tableName, xmsclientrequestId);
        }
        #endregion

        #region Callbacks
        public static void _ListTablesCallback(Object obj,
            out SqlString name,
            out SqlString url)
        {
            if (!(obj is ITPCfSQL.Azure.Table))
                throw new ArgumentException("Expected " + typeof(ITPCfSQL.Azure.Table).ToString() + ", received " + obj.GetType().FullName);

            ITPCfSQL.Azure.Table t = (ITPCfSQL.Azure.Table)obj;

            name = t.Name;
            url = t.Url.ToString();
        }

        public static void _QueryTableCallback(Object obj,
            out SqlString PartitionKey,
            out SqlString RowKey,
            out SqlDateTime TimeStamp,
            out SqlXml Attributes)
        {
            if (!(obj is ITPCfSQL.Azure.TableEntity))
                throw new ArgumentException("Expected " + typeof(ITPCfSQL.Azure.TableEntity).ToString() + ", received " + obj.GetType().ToString());

            ITPCfSQL.Azure.TableEntity entity = (ITPCfSQL.Azure.TableEntity)obj;

            PartitionKey = entity.PartitionKey;
            RowKey = entity.RowKey;
            TimeStamp = entity.TimeStamp;

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.Xml.XmlWriter wr = System.Xml.XmlWriter.Create(ms))
                {

                    wr.WriteStartElement("AttributeList");

                    foreach (KeyValuePair<string, string> kvp in entity.Attributes)
                    {
                        wr.WriteStartElement(kvp.Key);
                        wr.WriteString(kvp.Value);
                        wr.WriteEndElement();
                    }

                    wr.WriteEndElement();
                }

                ms.Seek(0, System.IO.SeekOrigin.Begin);

                using (System.Xml.XmlReader r = System.Xml.XmlReader.Create(ms))
                {
                    Attributes = new SqlXml(r);
                }
            }
        }

        #endregion
    }
}
