using System;
using System.Collections.Generic;
using System.Text;

namespace ITPCfSQL.Azure.Responses
{
    public class LeaseBlobResponse : AzureResponse
    {
        public Guid? LeaseId { get; set; }
        public int? LeaseTimeSeconds { get; set; }

        public LeaseBlobResponse(Dictionary<string, string> dHeaders)
            : base(dHeaders)
        {
            if(dHeaders.ContainsKey("x-ms-lease-id"))
            LeaseId = Guid.Parse(dHeaders["x-ms-lease-id"]);

            if (dHeaders.ContainsKey("x-ms-lease-time"))
                LeaseTimeSeconds = int.Parse(dHeaders["x-ms-lease-time"]);
        }
    }
}
