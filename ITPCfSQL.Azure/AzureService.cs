using System;
using System.Collections.Generic;
using System.Text;

namespace ITPCfSQL.Azure
{
    public abstract class AzureService
    {        
        #region Members
        public string AccountName { get; protected set; }
        public string SharedKey { get; set; }
        public bool UseHTTPS { get; set; }
        #endregion

        public AzureService(string AccountName, string SharedKey, bool UseHTTPS)
        {
            this.AccountName = AccountName;
            this.SharedKey = SharedKey;
            this.UseHTTPS = UseHTTPS;
        }
    }
}
