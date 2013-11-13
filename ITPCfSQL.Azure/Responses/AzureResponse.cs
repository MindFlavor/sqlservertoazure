using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Responses
{
    public class AzureResponse
    {
        public Guid RequestID { get; set; }
        public string Version { get; set; }
        public DateTime Date { get; set; }

        public AzureResponse(Dictionary<string, string> dHeaders)
        {
            RequestID = Guid.Parse(dHeaders["x-ms-request-id"]);
            Version = dHeaders["x-ms-version"];
            Date = DateTime.Parse(dHeaders["Date"]);
        }
    }
}
