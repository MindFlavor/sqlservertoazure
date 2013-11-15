using System;
using System.Collections.Generic;
using System.Text;

namespace ITPCfSQL.Azure
{
    public class TableEntity
    {
        private Dictionary<string, string> _Attributes = new Dictionary<string, string>();

        #region Memebers
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public Dictionary<string, string> Attributes { get { return _Attributes; } }
        #endregion

        public TableEntity() { }

        public TableEntity(string PartitionKey, string RowKey)
            :this(PartitionKey, RowKey, DateTime.Now)
        { }

        public TableEntity(string PartitionKey, string RowKey, DateTime TimeStamp) 
        {
            this.TimeStamp = TimeStamp;
            this.RowKey = RowKey;
            this.PartitionKey = PartitionKey;
        }
    }                       
}
