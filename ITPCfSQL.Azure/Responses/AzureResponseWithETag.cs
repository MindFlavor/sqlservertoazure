using System;
using System.Collections.Generic;
using System.Text;


namespace ITPCfSQL.Azure.Responses
{
    public class AzureResponseWithETag : AzureResponse
    {
        public string ETag { get; set; }

        public AzureResponseWithETag(Dictionary<string, string> dHeaders)
            :base(dHeaders)
        {
            ETag = dHeaders["ETag"];
        }
    }
}
