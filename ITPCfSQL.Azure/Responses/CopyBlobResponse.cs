using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Responses
{
    public class CopyBlobResponse : AzureResponseWithETag
    {
        public DateTime LastModified { get; set; }
        public string CopyId { get; set; }
        public Enumerations.BlobCopyStatus BlobCopyStatus { get; set; }

        public CopyBlobResponse(Dictionary<string, string> dHeaders)
            : base(dHeaders)
        {
            LastModified = DateTime.Parse(dHeaders["Last-Modified"]);
            CopyId = dHeaders["x-ms-copy-id"];
            BlobCopyStatus = (Enumerations.BlobCopyStatus)Enum.Parse(typeof(Enumerations.BlobCopyStatus), dHeaders["x-ms-copy-status"], true);
        }
    }
}
