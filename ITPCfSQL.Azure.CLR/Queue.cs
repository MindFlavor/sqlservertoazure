using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace ITPCfSQL.Azure.CLR
{
    public class Queue
    {
        #region Direct
        [SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "_ListQueuesCallback", IsDeterministic = false, IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = ("Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML"))]
        public static System.Collections.IEnumerable ListQueues(SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString prefix,
            SqlBoolean IncludeMetadata,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);

            List<ITPCfSQL.Azure.Queue> lQueues = aqs.ListQueues(
                prefix != null ? prefix.Value : null,
                IncludeMetadata.Value,
                timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            return lQueues;
        }

        [SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "_ListQueuesCallback", IsDeterministic = false, IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = ("Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML"))]
        public static System.Collections.IEnumerable ListQueues_Faulty(SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
               SqlString prefix,
               SqlBoolean IncludeMetadata,
               SqlInt32 timeoutSeconds,
               SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);

            List<ITPCfSQL.Azure.Queue> lQueues = aqs.ListQueues_Faulty(
                prefix != null ? prefix.Value : null,
                IncludeMetadata.Value,
                timeoutSeconds.Value,
                xmsclientrequestId != null ? xmsclientrequestId.Value : null);

            return lQueues;
        }

        [SqlProcedure]
        public static void ListQueuesProc(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString prefix,
            SqlBoolean IncludeMetadata,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);

            //SqlContext.Pipe.Send("Created " + aqs);

            List<ITPCfSQL.Azure.Queue> lQueues = aqs.ListQueues(
                prefix != null ? prefix.Value : null,
                IncludeMetadata.Value,
                timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            for (int i = 0; i < lQueues.Count; i++)
            {
                SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { 
                new SqlMetaData("Name", System.Data.SqlDbType.NVarChar, 4000), 
                new SqlMetaData("Url", System.Data.SqlDbType.NVarChar, 4000),
                new SqlMetaData("Metadata", System.Data.SqlDbType.Xml)
                });

                record.SetString(0, lQueues[i].Name);
                record.SetString(1, lQueues[i].Url.ToString());

                if (IncludeMetadata)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    using (System.Xml.XmlWriter wr = System.Xml.XmlWriter.Create(ms))
                    {
                        wr.WriteStartElement("MetadataList");

                        foreach (string s in lQueues[i].Metadata.Keys)
                        {
                            wr.WriteStartElement(s);
                            wr.WriteString(lQueues[i].Metadata[s]);
                            wr.WriteEndElement();
                        }

                        wr.WriteEndElement();

                        wr.Flush();
                        wr.Close();
                    }

                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    record.SetSqlXml(2, new SqlXml(ms));
                }
                if (i == 0)
                    SqlContext.Pipe.SendResultsStart(record);

                SqlContext.Pipe.SendResultsRow(record);

                if ((i + 1) >= lQueues.Count)
                    SqlContext.Pipe.SendResultsEnd();
            }
        }

        [SqlProcedure]
        public static void CreateQueue(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString queueName,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            aqs.CreateQueue(queueName.Value, timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }

        [SqlProcedure]
        public static void EnqueueMessage(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString queueName,
            SqlXml xmlMessage,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ITPCfSQL.Azure.Queue q = aqs.GetQueue(queueName.Value);
            q.PutMessage(new Message(xmlMessage.Value),
                timeoutSeconds.IsNull ? 60 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }

        [SqlProcedure]
        public static void DequeueMessage(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString queueName,
            SqlInt32 visibilityTimeoutSeconds, // we won't support peek so we should do it fast
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ITPCfSQL.Azure.Queue q = aqs.GetQueue(queueName.Value);
            Message msg = q.Get(visibilityTimeoutSeconds.Value, timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            if (msg == null) // empty queue
                return;

            msg.Delete(timeoutSeconds.Value,
                 xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);


            try // try to convert it to XML, if it fails, return a simple string
            {
                SqlXml xml;
                using (System.IO.StringReader sr = new System.IO.StringReader(msg.Body))
                {
                    using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(sr))
                    {
                        xml = new SqlXml(reader);
                    }
                }

                Utils.PushSingleRecordResult(xml.Value, System.Data.SqlDbType.Xml);
            }
            catch (Exception exce)
            {
                SqlContext.Pipe.Send("Cannot parse as XML:" + exce.Message);
                Utils.PushSingleRecordResult(msg.Body, System.Data.SqlDbType.NVarChar);
            }
        }

        [SqlProcedure]
        public static void RetrieveApproximateMessageCount(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString queueName,
            SqlInt32 timeoutSeconds,
            SqlString xmsclientrequestId)
        {
            ITPCfSQL.Azure.AzureQueueService aqs = new AzureQueueService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            ITPCfSQL.Azure.Queue q = aqs.GetQueue(queueName.Value);
            int iRet = q.RetrieveApproximateMessageCount(timeoutSeconds.Value, xmsclientrequestId.Value);

            Utils.PushSingleRecordResult(iRet, System.Data.SqlDbType.Int);
        }
        #endregion

        #region Embedded
        [SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "_ListQueuesCallback", IsDeterministic = false, IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = ("Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML"))]
        public static System.Collections.IEnumerable ListQueues_Embedded(
            SqlString logicalConnectionName,
            SqlString prefix,
            SqlBoolean IncludeMetadata,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return ListQueues(config.AccountName, config.SharedKey, config.UseHTTPS,
                prefix, IncludeMetadata, timeoutSeconds, xmsclientrequestId);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "_ListQueuesCallback", IsDeterministic = false, IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = ("Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML"))]
        public static System.Collections.IEnumerable ListQueues_Faulty_Embedded(
            SqlString logicalConnectionName,
            SqlString prefix,
            SqlBoolean IncludeMetadata,
            SqlInt32 timeoutSeconds,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return ListQueues_Faulty(config.AccountName, config.SharedKey, config.UseHTTPS,
                prefix, IncludeMetadata, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void ListQueuesProc_Embedded(
            SqlString logicalConnectionName,
            SqlString prefix,
            SqlBoolean IncludeMetadata,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            ListQueuesProc(config.AccountName, config.SharedKey, config.UseHTTPS,
                prefix, IncludeMetadata, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void CreateQueue_Embedded(
            SqlString logicalConnectionName,
            SqlString queueName,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            CreateQueue(config.AccountName, config.SharedKey, config.UseHTTPS,
                queueName, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void EnqueueMessage_Embedded(
            SqlString logicalConnectionName,
            SqlString queueName,
            SqlXml xmlMessage,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            EnqueueMessage(config.AccountName, config.SharedKey, config.UseHTTPS,
                queueName, xmlMessage, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void DequeueMessage_Embedded(
            SqlString logicalConnectionName,
            SqlString queueName,
            SqlInt32 visibilityTimeoutSeconds, // we won't support peek so we should do it fast
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            DequeueMessage(config.AccountName, config.SharedKey, config.UseHTTPS,
                queueName, visibilityTimeoutSeconds, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void RetrieveApproximateMessageCount_Embedded(
            SqlString logicalConnectionName,
            SqlString queueName,
            SqlInt32 timeoutSeconds,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            RetrieveApproximateMessageCount(config.AccountName, config.SharedKey, config.UseHTTPS,
                queueName, timeoutSeconds, xmsclientrequestId);
        }
        #endregion

        #region Callbacks

        public static void _ListQueuesCallback(Object obj,
            out SqlString name,
            out SqlString url,
            out SqlXml metadata)
        {
            if (!(obj is ITPCfSQL.Azure.Queue))
                throw new ArgumentException("Expected " + typeof(ITPCfSQL.Azure.Queue).ToString() + ", received " + obj.GetType().ToString());

            ITPCfSQL.Azure.Queue q = (ITPCfSQL.Azure.Queue)obj;

            name = q.Name;
            url = q.Url.ToString();

            if ((q.Metadata != null) && (q.Metadata.Count > 0))
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                using (System.Xml.XmlWriter wr = System.Xml.XmlWriter.Create(ms))
                {
                    wr.WriteStartElement("MetadataList");

                    foreach (string s in q.Metadata.Keys)
                    {
                        wr.WriteStartElement(s);
                        wr.WriteString(q.Metadata[s]);
                        wr.WriteEndElement();
                    }

                    wr.WriteEndElement();

                    wr.Flush();
                    wr.Close();
                }

                ms.Seek(0, System.IO.SeekOrigin.Begin);
                metadata = new SqlXml(ms);
            }
            else
                metadata = null;
        }
        #endregion
    }
}
