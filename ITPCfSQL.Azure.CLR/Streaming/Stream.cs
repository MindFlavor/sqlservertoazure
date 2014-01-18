using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using ITPCfSQL.Azure.Streaming;

namespace ITPCfSQL.Azure.CLR.Streaming
{
    public class Stream
    {
        [SqlFunction(
               DataAccess = DataAccessKind.None,
               SystemDataAccess = SystemDataAccessKind.None,
               FillRowMethodName = "_StreamXMLPlainLevel",
               IsDeterministic = false,
               IsPrecise = true,
               TableDefinition = (@"Entry XML"))]
        public static System.Collections.IEnumerable StreamNetXMLPlainLevel(
           SqlString uri, SqlInt32 XMLLevel)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(uri.Value);
            request.Method = "GET";
            System.IO.Stream s = request.GetResponse().GetResponseStream();

            return new XMLPlainLevelStreamer(s, XMLLevel.Value);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None,
            FillRowMethodName = "_StreamXMLPlainLevel",
            IsDeterministic = false,
            IsPrecise = true,
            TableDefinition = (@"Entry XML"))]
        public static System.Collections.IEnumerable StreamFileXMLPlainLevel(
           SqlString fileName, SqlInt32 XMLLevel)
        {
            System.IO.FileStream fs = new System.IO.FileStream(
                 fileName.Value,
                 System.IO.FileMode.Open,
                 System.IO.FileAccess.Read,
                 System.IO.FileShare.Read);
            return new XMLPlainLevelStreamer(fs, XMLLevel.Value);
        }

        [SqlFunction(
           DataAccess = DataAccessKind.None,
           SystemDataAccess = SystemDataAccessKind.None,
           FillRowMethodName = "_StreamLine",
           IsDeterministic = false,
           IsPrecise = true,
           TableDefinition = (@"Line NVARCHAR(MAX)"))]
        public static System.Collections.IEnumerable StreamNetLine(
           SqlString uri)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(uri.Value);
            request.Method = "GET";
            System.IO.Stream s = request.GetResponse().GetResponseStream();

            return new LineStreamer(s);
        }

        [SqlFunction(
           DataAccess = DataAccessKind.None,
           SystemDataAccess = SystemDataAccessKind.None,
           FillRowMethodName = "_StreamLine",
           IsDeterministic = false,
           IsPrecise = true,
           TableDefinition = (@"Line NVARCHAR(MAX)"))]
        public static System.Collections.IEnumerable StreamFileLine(
           SqlString fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(
                            fileName.Value,
                            System.IO.FileMode.Open,
                            System.IO.FileAccess.Read,
                            System.IO.FileShare.Read);

            return new LineStreamer(fs);
        }


        #region Blog function -- to delete
        [SqlFunction(
           DataAccess = DataAccessKind.None,
           SystemDataAccess = SystemDataAccessKind.None,
           FillRowMethodName = "_StreamLine",
           IsDeterministic = false,
           IsPrecise = true,
           TableDefinition = (@"Line NVARCHAR(MAX)"))]
        public static System.Collections.IEnumerable BlockingNetLine(
           SqlString uri)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(uri.Value);
            request.Method = "GET";
            List<string> lStreams = new List<string>();

            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
            {

                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                        lStreams.Add(str);
                }
            }

            return lStreams;
        }

        [SqlFunction(
           DataAccess = DataAccessKind.None,
           SystemDataAccess = SystemDataAccessKind.None,
           FillRowMethodName = "_StreamLine",
           IsDeterministic = false,
           IsPrecise = true,
           TableDefinition = (@"Line NVARCHAR(MAX)"))]
        public static System.Collections.IEnumerable BlockingFileLine(
           SqlString fileName)
        {
            List<string> lStreams = new List<string>();

            using (System.IO.FileStream fs = new System.IO.FileStream(
                            fileName.Value,
                            System.IO.FileMode.Open,
                            System.IO.FileAccess.Read,
                            System.IO.FileShare.Read))
            {

                using (System.IO.StreamReader sr = new System.IO.StreamReader(fs))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                        lStreams.Add(str);
                }
            }

            return lStreams;
        }

        #endregion

        #region Callbacks
        protected static void _StreamXMLPlainLevel(object obj, out SqlXml entry)
        {
            entry = SqlXml.Null;

            Dictionary<string, string> dRow = obj as Dictionary<string, string>;

            #region Row Output
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            using (System.Xml.XmlWriter wr = System.Xml.XmlWriter.Create(ms))
            {
                wr.WriteStartElement("Row");

                foreach (KeyValuePair<string, string> kvp in dRow)
                {
                    wr.WriteStartElement(kvp.Key);
                    wr.WriteString(kvp.Value);
                    wr.WriteEndElement();
                }

                wr.WriteEndElement();

                wr.Flush();
                wr.Close();
            }
            #endregion

            ms.Seek(0, System.IO.SeekOrigin.Begin);
            entry = new SqlXml(ms);
        }

        protected static void _StreamLine(object obj, out SqlString line)
        {
            line = obj as string;
        }
        #endregion
    }
}
