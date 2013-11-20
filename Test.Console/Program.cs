using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure.Internal;
using ITPCfSQL.Azure;
using System.Web;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri uri = new Uri("http://frcogno.blob.core.windows.net/privatecont/blob.txt");

            Uri newUri = Signature.GenerateSharedAccessSignatureURI(
                uri,
                "shared_key_sign",
                "r",
                "b",
                DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)),
                DateTime.Now.Add(new TimeSpan(1, 0, 0, 0)));

            ///////
            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(newUri);
            Request.Method = "GET";
            Request.ContentLength = 0;

            DateTime dt = DateTime.UtcNow;
            string strDate = dt.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
        }
    }
}
