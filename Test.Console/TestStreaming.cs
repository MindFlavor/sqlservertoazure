using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Streaming;

namespace Test.Console
{
    class TestStreaming
    {
        public static void LineStreamerFromHTTPS()
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create("https://www.dati.lombardia.it/api/views/q563-n2qm/rows.csv?accessType=DOWNLOAD");
            request.Method = "GET";

            LineStreamer lineStreamer = new LineStreamer(request.GetResponse().GetResponseStream());
            foreach (string str in lineStreamer)
            {
                System.Console.WriteLine(str);
            }
        }

        public static void XMLPlainLevelStreamerFromHTTPS()
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create("https://www.dati.lombardia.it/api/views/q563-n2qm/rows.xml?accessType=DOWNLOAD");
            request.Method = "GET";
            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
            {
                _XMLPlainLevelStreamer(s);
            }
        }

        public static void XMLPlainLevelStreamerFromFile()
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(
                ".\\testdata\\rowsos.xml",
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read))
            {
                _XMLPlainLevelStreamer(fs);
            }
        }

        private static void _XMLPlainLevelStreamer(System.IO.Stream s)
        {
            using (XMLPlainLevelStreamer xmlPlainLevelStreamer = new XMLPlainLevelStreamer(s, 2))
            {
                int iCnt = 0;
                foreach (Dictionary<string, string> dResult in xmlPlainLevelStreamer)
                {
                    System.Console.WriteLine(
                        "{0:D4}) {1:N0} keys",
                        iCnt++, dResult.Keys.Count);
                    foreach (KeyValuePair<string, string> kvp in dResult)
                    {
                        System.Console.WriteLine(
                            "\t{0:S}: {1:S}",
                            kvp.Key, kvp.Value);
                    }
                }
            }
        }
    }
}
