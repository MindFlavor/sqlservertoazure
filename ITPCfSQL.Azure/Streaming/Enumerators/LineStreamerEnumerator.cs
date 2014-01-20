using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Streaming.Enumerators
{
    public class LineStreamerEnumerator : System.Collections.IEnumerator, IDisposable
    {
        protected System.IO.StreamReader sr = null;
        protected string _CurrentLine = null;

        public LineStreamerEnumerator(System.IO.Stream s)
        {
            sr = new System.IO.StreamReader(s);
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _CurrentLine; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _CurrentLine = sr.ReadLine();
            return _CurrentLine != null;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        #region IDisposable / destructor
        void IDisposable.Dispose()
        {
            if (sr != null)
            {
                sr.Dispose();
                sr = null;
            }
        }

        // Finalizers are not supported in SQL CLR
        //~LineStreamerEnumerator()
        //{
        //    if (sr != null)
        //        sr.Dispose();
        //}
        #endregion
    }
}
