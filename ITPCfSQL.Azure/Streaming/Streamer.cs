using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Streaming
{
    public abstract class Streamer : IDisposable
    {
        protected System.IO.Stream stream = null;
        public Streamer(System.IO.Stream stream)
        {
            this.stream = stream;
        }

        void IDisposable.Dispose()
        {
            if (stream != null)
                stream.Dispose();
        }
    }
}
