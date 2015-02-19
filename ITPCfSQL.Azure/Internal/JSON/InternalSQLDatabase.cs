using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Internal.JSON
{
    public class InternalSQLDatabase
    {
        public const string AZURE_VERSION = "2013-06-01";
        internal static string GetManagementURI()
        {
            return "https://management.core.windows.net:8443";
        }

        public static string GetServices(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId)
        {
            Uri uri = new Uri(
                GetManagementURI() + "/" +
                subscriptionId.ToString() + "/" +
                "services" + "/" +
                "sqlservers/servers?contentview=generic");

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "GET";
            Request.Headers.Add(Constants.HEADER_VERSION, AZURE_VERSION);
            Request.ClientCertificates.Add(certificate);

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                return (new System.IO.StreamReader(response.GetResponseStream())).ReadToEnd();
            }
            else
                throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
