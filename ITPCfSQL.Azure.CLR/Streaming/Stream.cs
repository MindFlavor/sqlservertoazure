using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.CLR.Streaming
{
    public class Stream : System.Collections.IEnumerable
    {
        public delegate void RowParsedDelegate(Dictionary<string, string> row);

        public void ParseStream(System.IO.Stream s, int iLevelToFind, RowParsedDelegate rpd)
        {
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(s))
            {
                reader.MoveToContent();

                int iLevel = 0;

                Dictionary<string, string> dResult = null;
                string strParent = null;

                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        iLevel++;

                        if (iLevel == iLevelToFind)
                            dResult = new Dictionary<string, string>();

                        strParent = reader.Name;
                    }
                    else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                    {
                        iLevel--;


                        if ((iLevel + 1) == iLevelToFind)
                        {
                            // produce row!
                            if (rpd != null)
                                rpd(dResult);
                        }

                    }
                    else if (reader.NodeType != System.Xml.XmlNodeType.Whitespace)
                    {
                        dResult[strParent] = reader.Value;
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
