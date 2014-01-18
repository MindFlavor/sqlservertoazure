using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Streaming.Enumerators
{
    public class XMLPlainLevelEnumerator : System.Collections.IEnumerator, IDisposable
    {
        #region Members
        protected System.Xml.XmlReader reader = null;

        protected int iCurrentLevel = 0;
        protected Dictionary<string, string> dRow = null;
        #endregion

        #region Properties
        public int LevelToFind { get; protected set; }
        #endregion

        #region Constructors
        public XMLPlainLevelEnumerator(System.IO.Stream s, int LevelToFind)
        {
            this.LevelToFind = LevelToFind;
            reader = System.Xml.XmlReader.Create(s);
            reader.MoveToContent();
            iCurrentLevel = 0;
        }
        #endregion

        #region IDisposable / destructor
        void IDisposable.Dispose()
        {
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
        }

        // Finalizers are not supported in SQL CLR
        //~XMLPlainLevelEnumerator()
        //{
        //    if (reader != null)
        //        reader.Dispose();
        //}
        #endregion

        #region IEnumerator
        object System.Collections.IEnumerator.Current
        {
            get { return dRow; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            Dictionary<string, string> dResult = null;

            string strParent = null;
            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    iCurrentLevel++;

                    if (iCurrentLevel == LevelToFind)
                        dResult = new Dictionary<string, string>();

                    strParent = reader.Name;
                }
                else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                {
                    iCurrentLevel--;


                    if ((iCurrentLevel + 1) == LevelToFind)
                    {
                        // produce row!
                        dRow = dResult;
                        return true;
                    }

                }
                else if (reader.NodeType != System.Xml.XmlNodeType.Whitespace)
                {
                    dResult[strParent] = reader.Value;
                }
            }

            return false;
        }

        void System.Collections.IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
