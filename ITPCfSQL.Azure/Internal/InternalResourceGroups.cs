using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Internal
{
    public class InternalResourceGroups
    {
        public const string AZURE_VERSION = "2015-01-01";
        internal static string GetManagementURI()
        {
            return "https://management.azure.com";
        }
        public static object ListResourceGroups(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            Guid subscriptionId,
            //string apiVersion,
            int iTop = -1,
            string skipToken = null,
            string filter = null)
        {
            string strUrl =
                GetManagementURI() + "/" +
                "subscriptions" + "/" +
                subscriptionId.ToString() + "/" +
                "resourcegroups?" +
                "api-version=" + AZURE_VERSION;
            if (iTop > 0)
                strUrl += "&$top=" + iTop;

            if (!string.IsNullOrEmpty(skipToken))
                strUrl += "&$skiptoken=" + skipToken;

            if (!string.IsNullOrEmpty(filter))
                strUrl += "&$filter=" + filter;


            Uri uri = new Uri(strUrl);

            return PerformSimpleGet<Responses.SQLDatabase.ListDatabases.Response>(uri);
        }

        #region Private util methods
        internal static T PerformSimpleGet<T>(Uri uri)
        {
            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "GET";
            Request.ContentType = "application/json";
            //Request.Headers.Add(Constants.HEADER_VERSION, azureVersion);
            //Request.ClientCertificates.Add(certificate);

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //string s = (new System.IO.StreamReader(response.GetResponseStream())).ReadToEnd();

                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)ser.Deserialize(response.GetResponseStream());
            }
            else
                throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        #endregion
    }
}
