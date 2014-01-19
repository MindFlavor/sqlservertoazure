using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Streaming.Enumerators
{
    public class StringSplitEnumerator : System.Collections.IEnumerator
    {
        #region Memebers
        protected string CurrentToken = null;
        protected int CurrentPosition = 0;
        #endregion

        #region Properties
        public string Content { get; protected set; }
        public string Delimiter { get; protected set; }
        protected int StringLength { get; set; }
        #endregion

        #region Constructors
        public StringSplitEnumerator(string Content, string Delimiter)
        {
            this.Content = Content;
            this.Delimiter = Delimiter;
            this.StringLength = Content.Length;
        }
        #endregion

        object System.Collections.IEnumerator.Current
        {
            get { return CurrentToken; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            int iNextPos = Content.IndexOf(Delimiter, CurrentPosition);

            if (iNextPos != -1)
            {
                CurrentToken = Content.Substring(CurrentPosition, iNextPos - CurrentPosition);
            }
            else
            {
                CurrentToken = Content.Substring(CurrentPosition);
            }

            CurrentPosition = iNextPos + 1;

            return iNextPos != -1;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
    }
}
