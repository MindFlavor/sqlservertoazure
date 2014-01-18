using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Streaming.Enumerators;

namespace ITPCfSQL.Azure.Streaming
{
    public class XMLPlainLevelStreamer : Streamer, System.Collections.IEnumerable
    {
        public int Level { get; protected set; }

        public XMLPlainLevelStreamer(System.IO.Stream stream, int Level)
            : base(stream)
        {
            this.Level = Level;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new XMLPlainLevelEnumerator(stream, Level);
        }
    }
}
