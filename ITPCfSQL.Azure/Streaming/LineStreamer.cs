using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Streaming.Enumerators;

namespace ITPCfSQL.Azure.Streaming
{
    public class LineStreamer : Streamer, System.Collections.IEnumerable
    {
        public LineStreamer(System.IO.Stream stream)
            : base(stream)
        { }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new LineStreamerEnumerator(stream);
        }
    }
}
