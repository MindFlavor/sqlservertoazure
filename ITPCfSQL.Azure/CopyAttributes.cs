using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class CopyAttributes
    {
        public Guid CopyId { get; set; }
        public Uri CopySource { get; set; }
        public string CopyStatus { get; set; }
        public long CopyCurrentPosition { get; set; }
        public long CopyTotalLength { get; set; }
        public DateTime? CopyCompletionTime { get; set; }
    }
}
