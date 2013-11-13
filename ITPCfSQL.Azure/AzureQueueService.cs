using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class AzureQueueService : AzureService
    {
        public AzureQueueService(string AccountName, string SharedKey, bool UseHTTPS)
            : base(AccountName, SharedKey, UseHTTPS)
        { }

        #region Queue methods
        public List<Queue> ListQueues(
            string prefix = null,
            bool IncludeMetadata = false,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            List<Queue> lQueues = new List<Queue>();
            string strNextMarker = null;
            do
            {
                string sRet = Internal.InternalMethods.ListQueues(UseHTTPS, SharedKey, AccountName, prefix, strNextMarker,
                    IncludeMetadata: IncludeMetadata, timeoutSeconds: timeoutSeconds, xmsclientrequestId: xmsclientrequestId);

                //Microsoft.SqlServer.Server.SqlContext.Pipe.Send("After Internal.InternalMethods.ListQueues = " + sRet);

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                using (System.IO.StringReader sr = new System.IO.StringReader(sRet))
                {
                    doc.Load(sr);
                }

                foreach (System.Xml.XmlNode node in doc.SelectNodes("EnumerationResults/Queues/Queue"))
                {
                    lQueues.Add(Queue.ParseFromXmlNode(this, node));
                };

                strNextMarker = doc.SelectSingleNode("EnumerationResults/NextMarker").InnerText;

                //Microsoft.SqlServer.Server.SqlContext.Pipe.Send("strNextMarker == " + strNextMarker);

            } while (!string.IsNullOrEmpty(strNextMarker));

            return lQueues;
        }

        public Queue CreateQueue(
            string queueName,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string str = Internal.InternalMethods.CreateQueue(UseHTTPS, SharedKey, AccountName, queueName, timeoutSeconds, xmsclientrequestId);

            string strUrl = string.Format("{0:S}/{1:S}",
                Internal.InternalMethods.GetQueueUrl(this.UseHTTPS, this.AccountName),
                queueName);

            return new Queue()
            {
                AzureQueueService = this,
                Name = queueName,
                Url = new Uri(strUrl)
            };
        }

        public Queue GetQueue(string queueName)
        {
            return new Queue()
            {
                AzureQueueService = this,
                Name = queueName,
                Url = new Uri(string.Format("{0:S}/{1:S}",
                    Internal.InternalMethods.GetQueueUrl(this.UseHTTPS, this.AccountName),
                    queueName))
            };
        }
        #endregion


        #region Faulty methods
        public List<Queue> ListQueues_Faulty(
            string prefix = null,
            bool IncludeMetadata = false,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            List<Queue> lQueues = new List<Queue>();
            string strNextMarker = null;
            do
            {
                string sRet = Internal.InternalMethods.ListQueues_Faulty(UseHTTPS, SharedKey, AccountName, prefix, strNextMarker,
                    IncludeMetadata: IncludeMetadata, timeoutSeconds: timeoutSeconds, xmsclientrequestId: xmsclientrequestId);

                //Microsoft.SqlServer.Server.SqlContext.Pipe.Send("After Internal.InternalMethods.ListQueues = " + sRet);

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                using (System.IO.StringReader sr = new System.IO.StringReader(sRet))
                {
                    doc.Load(sr);
                }

                foreach (System.Xml.XmlNode node in doc.SelectNodes("EnumerationResults/Queues/Queue"))
                {
                    lQueues.Add(Queue.ParseFromXmlNode(this, node));
                };

                strNextMarker = doc.SelectSingleNode("EnumerationResults/NextMarker").InnerText;

                //Microsoft.SqlServer.Server.SqlContext.Pipe.Send("strNextMarker == " + strNextMarker);

            } while (!string.IsNullOrEmpty(strNextMarker));

            return lQueues;
        }
        #endregion
    }
}
