using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Test.Console
{
    public class TestBuffering
    {
        static byte[] buffer = new byte[1024 * 64];
        public static void Do()
        {
            Uri uri = new Uri("https://www.dati.lombardia.it/api/views/xy9p-k9bj/rows.csv?accessType=DOWNLOAD");
            //Uri uri = new Uri("https://www.dati.lombardia.it/api/views/q563-n2qm/rows.xml?accessType=DOWNLOAD");

            {
                WebRequest req = WebRequest.Create(uri);
                req.Method = "GET";

                using (System.IO.Stream s = req.GetResponse().GetResponseStream())
                {
                    int iEnd = 1;
                    double dBytesRead = 0.0D;
                    double dBPS = 0.0D;

                    DateTime dtStart = DateTime.Now;
                    DateTime dtLastShow = DateTime.MinValue;


                    while (iEnd > 0)
                    {
                        iEnd = s.Read(buffer, 0, buffer.Length);

                        dBytesRead += iEnd;
                        if ((DateTime.Now - dtLastShow).TotalSeconds > 1)
                        {
                            dtLastShow = DateTime.Now;
                            dBPS = dBytesRead / (DateTime.Now - dtStart).TotalSeconds;

                            System.Console.WriteLine("Read {0:N0} KB ({1:N0} bytes/sec)", dBytesRead, dBPS);
                        }
                    }

                    System.Console.WriteLine("Total time: {0:N0} seconds ", (DateTime.Now - dtStart).TotalSeconds);
                }
            }

            {
                WebRequest req = WebRequest.Create(uri);
                req.Method = "GET";

                using (System.IO.Stream s = req.GetResponse().GetResponseStream())
                {
                    // simulate wait
                    for (int i = 5; i > 0; i--)
                    {
                        System.Console.WriteLine("Wait {0:N0} seconds...", i);
                        System.Threading.Thread.Sleep(1000);
                    }

                    int iEnd = 1;
                    double dBytesRead = 0.0D;
                    double dBPS = 0.0D;

                    DateTime dtStart = DateTime.Now;
                    DateTime dtLastShow = DateTime.MinValue;

                    while (iEnd > 0)
                    {
                        iEnd = s.Read(buffer, 0, buffer.Length);

                        dBytesRead += iEnd;
                        if ((DateTime.Now - dtLastShow).TotalSeconds > 1)
                        {
                            dtLastShow = DateTime.Now;
                            dBPS = dBytesRead / (DateTime.Now - dtStart).TotalSeconds;

                            System.Console.WriteLine("Read {0:N0} KB ({1:N0} bytes/sec)", dBytesRead, dBPS);
                        }
                    }

                    System.Console.WriteLine("Total time: {0:N0} seconds ", (DateTime.Now - dtStart).TotalSeconds);
                }
            }

            System.Console.ReadKey();
        }
    }
}
