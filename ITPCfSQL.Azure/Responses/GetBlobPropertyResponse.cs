using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Responses
{
    public class GetBlobPropertyResponse : AzureResponseWithETag
    {
        public DateTime LastModified { get; set; }
        public Dictionary<string, string> MetaNames { get; private set; }
        public Enumerations.BlobType BlobType { get; private set; }
        public DateTime? CopyCompletionTime { get; set; }
        public string CopyStatusDescription { get; set; }
        public string CopyId { get; set; }
        public long? BytesCopied { get; set; }
        public long? BytesTotal { get; set; }
        public string CopySource { get; set; }
        public Enumerations.BlobCopyStatus? BlobCopyStatus { get; set; }
        public Enumerations.LeaseDuration? LeaseDuration { get; set; }
        public Enumerations.LeaseState? LeaseState { get; set; }
        public Enumerations.LeaseStatus? LeaseStatus { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ContentMD5 { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentLanguage { get; set; }
        public string CacheControl { get; set; }
        public long? BlobSequenceNumber { get; set; }

        public GetBlobPropertyResponse(Dictionary<string, string> dHeaders)
            : base(dHeaders)
        {
            LastModified = DateTime.Parse(dHeaders["Last-Modified"]);
            
            MetaNames = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> sElem in dHeaders.Where(item => item.Key.StartsWith("x-ms-meta-")))
                MetaNames.Add(sElem.Key, sElem.Value);


            BlobType = (Enumerations.BlobType)Enum.Parse(typeof(Enumerations.BlobType), dHeaders["x-ms-blob-type"], true);

            if (dHeaders.ContainsKey("x-ms-copy-completion-time"))
                CopyCompletionTime = DateTime.Parse(dHeaders["x-ms-copy-completion-time"]);

            if (dHeaders.ContainsKey("x-ms-copy-status-description"))
                CopyStatusDescription = dHeaders["x-ms-copy-status-description"];

            if (dHeaders.ContainsKey("x-ms-copy-id"))
                CopyStatusDescription = dHeaders["x-ms-copy-id"];

            if (dHeaders.ContainsKey("x-ms-copy-progress"))
            {
                string val = dHeaders["x-ms-copy-progress"];
                int idx = val.IndexOf('/');
                BytesCopied = long.Parse(val.Substring(0, idx));
                BytesTotal = long.Parse(val.Substring(idx + 1));
            }

            if (dHeaders.ContainsKey("x-ms-copy-source"))
                CopyStatusDescription = dHeaders["x-ms-copy-source"];

            if (dHeaders.ContainsKey("x-ms-copy-status"))
            {
                BlobCopyStatus = (Enumerations.BlobCopyStatus)Enum.Parse(typeof(Enumerations.BlobCopyStatus), dHeaders["x-ms-copy-status"], true);
            }

            if (dHeaders.ContainsKey("x-ms-lease-duration"))
            {
                LeaseDuration = (Enumerations.LeaseDuration)Enum.Parse(typeof(Enumerations.LeaseDuration), dHeaders["x-ms-lease-duration"], true);
            }

            if (dHeaders.ContainsKey("x-ms-lease-state"))
            {
                LeaseState = (Enumerations.LeaseState)Enum.Parse(typeof(Enumerations.LeaseState), dHeaders["x-ms-lease-state"], true);
            }

            if (dHeaders.ContainsKey("x-ms-lease-status"))
            {
                LeaseStatus = (Enumerations.LeaseStatus)Enum.Parse(typeof(Enumerations.LeaseStatus), dHeaders["x-ms-lease-status"], true);
            }

            ContentLength = long.Parse(dHeaders["Content-Length"]);

            if (dHeaders.ContainsKey("Content-Type"))
                ContentType = dHeaders["Content-Type"];
            else
                ContentType = "application/octet-stream";

            if (dHeaders.ContainsKey("Content-MD5"))
                ContentMD5 = dHeaders["Content-MD5"];

            if (dHeaders.ContainsKey("Content-Encoding"))
                ContentEncoding = dHeaders["Content-Encoding"];
            if (dHeaders.ContainsKey("Content-Language"))
                ContentLanguage = dHeaders["Content-Language"];
            if (dHeaders.ContainsKey("Cache-Control"))
                CacheControl = dHeaders["Cache-Control"];

            if (dHeaders.ContainsKey("x-ms-blob-sequence-number"))
                BlobSequenceNumber = long.Parse(dHeaders["x-ms-blob-sequence-number"]);
        }
    }
}
