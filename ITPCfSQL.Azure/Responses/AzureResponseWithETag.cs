using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
