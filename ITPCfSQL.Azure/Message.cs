using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    [Serializable]
    public class Message
    {
        #region Properties
        public DateTime ExpirationTime { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? TimeNextVisible { get; set; }

        public int DequeueCount { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        internal Guid MessageId { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        internal string PopReceipt { get; set; }

        public Queue Queue { get; internal set; }
        public string Body { get; set; }
        #endregion

        #region Constructors
        internal Message() { }

        public Message(string Body, DateTime ExpirationTime)
        {
            this.Body = Body;

            this.ExpirationTime = ExpirationTime;
        }

        public Message(string Body)
            : this(Body, DateTime.Now.AddDays(7))
        { }
        #endregion

        public Dictionary<string, string> Delete(
            int timeoutSeconds = 0,
           Guid? xmsclientrequestId = null)
        {
            return Internal.InternalMethods.DeleteMessage(
                Queue.AzureQueueService.UseHTTPS,
                Queue.AzureQueueService.SharedKey,
                Queue.AzureQueueService.AccountName,
                Queue.Name,
                MessageId,
                PopReceipt,
                timeoutSeconds,
                xmsclientrequestId);
        }

        public Dictionary<string, string> Update(
            int timeoutSeconds = 0,
           string xmsclientrequestId = null)
        {
            int visibilitytimeoutSeconds = 0;
            if (TimeNextVisible.HasValue)
                visibilitytimeoutSeconds = Math.Max(0, (int)(TimeNextVisible.Value - DateTime.Now).TotalSeconds);

            int messageTTLSeconds = (int)(ExpirationTime - DateTime.Now).TotalSeconds;

            var ret = Internal.InternalMethods.UpdateMessage(
                this.Queue.AzureQueueService.UseHTTPS,
                this.Queue.AzureQueueService.SharedKey,
                this.Queue.AzureQueueService.AccountName,
                this.Queue.Name,
                string.Format(
                    "<QueueMessage><MessageText>{0:S}</MessageText></QueueMessage>",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.Body))),
                    this.MessageId,
                    this.PopReceipt,
                    visibilitytimeoutSeconds,
                timeoutSeconds,
                xmsclientrequestId);            
            return ret;
        }
    }
}
