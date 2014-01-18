using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.CLR.Streaming
{
    public class Enumerator : System.Collections.IEnumerator
    {
        object System.Collections.IEnumerator.Current
        {
            get { throw new NotImplementedException(); }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            throw new NotImplementedException();
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
    }
}
