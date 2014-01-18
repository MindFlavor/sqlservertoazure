using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.Internal.JSON
{
    public class InternalMethods
    {
        public static string QueryTables(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/Tables", ITPCfSQL.Azure.Internal.InternalMethods.GetTableUrl(useHTTPS, accountName));

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";

            Request.ContentLength = 0;


            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2013-08-15");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/json;odata=fullmetadata";
            Request.ContentType = "application/json";

            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        public static string QueryTable(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string tableName,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}()", ITPCfSQL.Azure.Internal.InternalMethods.GetTableUrl(useHTTPS, accountName), tableName);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";

            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2013-08-15");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/json;odata=nometadata";
            Request.ContentType = "application/json";

            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
