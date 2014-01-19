using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Streaming
{
    public class StringSplit : System.Collections.IEnumerable
    {
        #region Memebers
        #endregion

        #region Properties
        public string Content { get; protected set; }
        public string Delimiter { get; protected set; }
        #endregion

        #region Constructors
        public StringSplit(string String, string Delimiter)
        {
            this.Content = String;
            this.Delimiter = Delimiter;
        }
        #endregion

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerators.StringSplitEnumerator(Content, Delimiter);
        }
    }
}
