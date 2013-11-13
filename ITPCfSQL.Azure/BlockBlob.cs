using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class BlockBlob : Blob
    {
        public BlockBlob(Container container, string name)
            : base(container, name)
        {
            BlobType = Enumerations.BlobType.BlockBlob;
        }

        public BlockBlob(Container container)
            : base(container)
        {
            BlobType = Enumerations.BlobType.BlockBlob;
        }
    }
}
