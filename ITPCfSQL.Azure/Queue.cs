using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class Queue
    {
        #region Constants
        public const string META_PREFIX = "x-ms-meta-";
        public const string META_APPROX_MESSAGE_COUNT = "x-ms-approximate-messages-count";
        #endregion

        #region Members
        protected Dictionary<string, string> _metadata = new Dictionary<string, string>();
        #endregion

        #region Properties
        public AzureQueueService AzureQueueService { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }
        public Dictionary<string, string> Metadata { get { return _metadata; } }
        #endregion

        #region Constructors

        #endregion

        #region Factory methods
        internal static Queue ParseFromXmlNode(AzureQueueService AzureQueueService, System.Xml.XmlNode nRoot)
        {
            Queue q = new Queue()
            {
                AzureQueueService = AzureQueueService,
                Name = nRoot.SelectSingleNode("Name").InnerText,
                Url = new Uri(nRoot.SelectSingleNode("Url").InnerText)
            };

            System.Xml.XmlNode nMeta = nRoot.SelectSingleNode("Metadata");
            if (nMeta != null)
            {
                foreach (System.Xml.XmlNode nMetaElem in nMeta.ChildNodes)
                {
                    q.Metadata[nMetaElem.Name] = nMetaElem.InnerText;
                }
            }

            return q;
        }
        #endregion

        #region Metadata methods
        public void UpdateMetadata(int timeoutSeconds = 0, string xmsclientrequestId = null)
        {

            Dictionary<string, string> dicTemp = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in Metadata)
            {
                dicTemp[META_PREFIX + kvp.Key] = kvp.Value;
            }

            Internal.InternalMethods.SetQueueMetadata(
                this.AzureQueueService.UseHTTPS,
                this.AzureQueueService.SharedKey, this.AzureQueueService.AccountName,
                Name, dicTemp, timeoutSeconds, xmsclientrequestId);
        }

        public void RedownloadMetadata(int timeoutSeconds = 0, string xmsclientrequestId = null)
        {
            Dictionary<string, string> dMetadata = Internal.InternalMethods.GetQueueMetadata(
                this.AzureQueueService.AccountName,
                this.AzureQueueService.SharedKey,
                this.AzureQueueService.UseHTTPS,
                 Name, timeoutSeconds);

            foreach (string key in dMetadata.Keys.Where(item => item.StartsWith(META_PREFIX)))
            {
                Metadata[key.Substring(META_PREFIX.Length)] = dMetadata[key];
            }
        }

        public int RetrieveApproximateMessageCount(int timeoutSeconds = 0, string xmsclientrequestId = null)
        {
            Dictionary<string, string> dMetadata = Internal.InternalMethods.GetQueueMetadata(
                this.AzureQueueService.AccountName,
                this.AzureQueueService.SharedKey,
                 this.AzureQueueService.UseHTTPS,
                Name, timeoutSeconds);

            return int.Parse(dMetadata[META_APPROX_MESSAGE_COUNT]);
        }
        #endregion

        public string Clear(
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            return Internal.InternalMethods.ClearQueue(
                this.AzureQueueService.UseHTTPS,
                this.AzureQueueService.SharedKey,
                this.AzureQueueService.AccountName,
                Name,
                timeoutSeconds, xmsclientrequestId);
        }

        public void Delete(int timeoutSeconds = 0, string xmsclientrequestId = null)
        {
            Internal.InternalMethods.DeleteQueue(
                this.AzureQueueService.UseHTTPS,
                this.AzureQueueService.SharedKey, this.AzureQueueService.AccountName,
                Name, timeoutSeconds, xmsclientrequestId);
        }

        #region Messages
        public Dictionary<string, string> PutMessage(
            Message message,
            int timeoutSeconds = 0,
           Guid? xmsclientrequestId = null)
        {
            int visibilitytimeoutSeconds = 0;
            if (message.TimeNextVisible.HasValue)
                visibilitytimeoutSeconds = Math.Max(0, (int)(message.TimeNextVisible.Value - DateTime.Now).TotalSeconds);

            int messageTTLSeconds = (int)(message.ExpirationTime - DateTime.Now).TotalSeconds;

            var ret = Internal.InternalMethods.PutMessage(
                this.AzureQueueService.UseHTTPS,
                this.AzureQueueService.SharedKey,
                this.AzureQueueService.AccountName,
                Name,
                string.Format(
                    "<QueueMessage><MessageText>{0:S}</MessageText></QueueMessage>",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message.Body))),
                    visibilitytimeoutSeconds,
                    messageTTLSeconds,
                    timeoutSeconds,
                    xmsclientrequestId);

            message.InsertTime = DateTime.Now;
            message.Queue = this;
            return ret;
        }

        public Message Get(
           int visibilitytimeoutSeconds,
           int timeoutSeconds = 0,
          Guid? xmsclientrequestId = null)
        {
            List<Message> msg = GetMessages(visibilitytimeoutSeconds, 1, timeoutSeconds, xmsclientrequestId);
            if (msg.Count > 0)
                return msg[0];
            else
                return null;
        }

        public List<Message> GetMessages(
           int visibilitytimeoutSeconds,
           int numofmessages = 0,
           int timeoutSeconds = 0,
          Guid? xmsclientrequestId = null)
        {
            string sRet = Internal.InternalMethods.GetMessages(
                     this.AzureQueueService.UseHTTPS,
                     this.AzureQueueService.SharedKey,
                     this.AzureQueueService.AccountName,
                     Name,
                     visibilitytimeoutSeconds,
                     numofmessages,
                     timeoutSeconds,
                     xmsclientrequestId);

            using (System.IO.StringReader sr = new System.IO.StringReader(sRet))
            {
                return FormatMessagesFromInputStream(this, sr);
            }
        }

        public Message Peek(
           int timeoutSeconds = 0,
          string xmsclientrequestId = null)
        {
            List<Message> msg = PeekMessages(1, timeoutSeconds, xmsclientrequestId);
            if (msg.Count > 0)
                return msg[0];
            else
                return null;
        }

        public List<Message> PeekMessages(
               int numofmessages = 0,
               int timeoutSeconds = 0,
               string xmsclientrequestId = null)
        {
            string sRet = Internal.InternalMethods.PeekMessages(
                this.AzureQueueService.UseHTTPS,
                this.AzureQueueService.SharedKey,
                this.AzureQueueService.AccountName,
                Name,
                numofmessages,
                timeoutSeconds,
                xmsclientrequestId);

            using (System.IO.StringReader sr = new System.IO.StringReader(sRet))
            {
                return FormatMessagesFromInputStream(this, sr);
            }
        }


        #region General purpose methods
        protected static List<Message> FormatMessagesFromInputStream(Queue queue, System.IO.TextReader tr)
        {
            List<Message> lMessages = new List<Message>();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(tr);

            foreach (System.Xml.XmlNode n in doc.SelectNodes("QueueMessagesList/QueueMessage"))
            {
                Message msg = new Message()
                {
                    MessageId = Guid.Parse(n.SelectSingleNode("MessageId").InnerText),
                    InsertTime = DateTime.Parse(n.SelectSingleNode("InsertionTime").InnerText),
                    ExpirationTime = DateTime.Parse(n.SelectSingleNode("ExpirationTime").InnerText),
                    DequeueCount = int.Parse(n.SelectSingleNode("DequeueCount").InnerText),
                    Body = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(n.SelectSingleNode("MessageText").InnerText)),
                    Queue = queue
                };

                if (n.SelectSingleNode("PopReceipt") != null)
                    msg.PopReceipt = n.SelectSingleNode("PopReceipt").InnerText;

                if (n.SelectSingleNode("TimeNextVisible") != null)
                    msg.TimeNextVisible = DateTime.Parse(n.SelectSingleNode("TimeNextVisible").InnerText);

                lMessages.Add(msg);
            }

            return lMessages;
        }
        #endregion
        #endregion
    }

}
