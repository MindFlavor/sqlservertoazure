using ITPCfSQL.Azure.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITPCfSQL.Azure.Internal
{
    public class InternalMethods
    {
        public const string AZURE_VERSION = "2013-06-01";

        #region Queue
        public static string GetQueueUrl(bool useHTTPS, string accountName)
        {
            if (accountName != "devstoreaccount1")
                return string.Format("{0:S}://{1:S}.queue.core.windows.net", useHTTPS ? "https" : "http", accountName);
            else
                return string.Format("{0:S}://localhost:10001/{1:S}", useHTTPS ? "https" : "http", accountName);
        }

        public static string ListQueues_Faulty(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string prefix = null,
            string nextMarker = null,
            int maxResults = 0,
            bool IncludeMetadata = false,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}?comp=list", GetQueueUrl(useHTTPS, accountName));
            if (!string.IsNullOrEmpty(nextMarker))
                strUrl += string.Format("&marker={0:S}", nextMarker);
            if (maxResults > 0)
                strUrl += string.Format("&maxresults={0:S}", maxResults.ToString());
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());
            if (IncludeMetadata)
                strUrl += "&include=metadata";
            if (!string.IsNullOrEmpty(prefix))
                strUrl += string.Format("&prefix={0:S}", prefix);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            //Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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


        public static string ListQueues(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string prefix = null,
            string nextMarker = null,
            int maxResults = 0,
            bool IncludeMetadata = false,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}?comp=list", GetQueueUrl(useHTTPS, accountName));
            if (!string.IsNullOrEmpty(nextMarker))
                strUrl += string.Format("&marker={0:S}", nextMarker);
            if (maxResults > 0)
                strUrl += string.Format("&maxresults={0:S}", maxResults.ToString());
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());
            if (IncludeMetadata)
                strUrl += "&include=metadata";
            if (!string.IsNullOrEmpty(prefix))
                strUrl += string.Format("&prefix={0:S}", prefix);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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

        public static string CreateQueue(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null
            )
        {
            string strUrl = string.Format("{0:S}/{1:S}", GetQueueUrl(useHTTPS, accountName), queueName);

            if (timeoutSeconds > 0)
                strUrl += string.Format("?timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            else
                Request.Headers.Add("x-ms-client-request-id", Guid.NewGuid().ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
        }

        public static string DeleteQueue(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
            )
        {
            string strUrl = string.Format("{0:S}/{1:S}", GetQueueUrl(useHTTPS, accountName), queueName);

            if (timeoutSeconds > 0)
                strUrl += string.Format("?timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public static string ClearQueue(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
        )
        {
            string strUrl = string.Format("{0:S}/{1:S}/messages", GetQueueUrl(useHTTPS, accountName), queueName);

            if (timeoutSeconds > 0)
                strUrl += string.Format("?timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public static Dictionary<string, string> SetQueueMetadata(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            Dictionary<string, string> kvpMetadata,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
         )
        {
            string strUrl = string.Format("{0:S}/{1:S}", GetQueueUrl(useHTTPS, accountName), queueName);

            strUrl += "?comp=metadata";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            foreach (KeyValuePair<string, string> kvp in kvpMetadata)
            {
                Request.Headers.Add(kvp.Key, kvp.Value);
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public static Dictionary<string, string> GetQueueMetadata(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string queueName,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}", GetQueueUrl(useHTTPS, accountName), queueName);

            strUrl += "?comp=metadata";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "HEAD";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        public static string GetQueueACL(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}", GetQueueUrl(useHTTPS, accountName), queueName);

            strUrl += "?comp=acl";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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
        #endregion

        #region Queue service
        public static string GetQueueServiceProperties(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            int timeoutSeconds = 0)
        {
            string strUrl = string.Format("{0:S}", GetQueueUrl(useHTTPS, accountName));

            strUrl += "?restype=service&comp=properties";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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
        #endregion

        #region Message handling
        public static string GetMessages(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            int visibilitytimeoutSeconds,
            int numofmessages = 0,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/messages", GetQueueUrl(useHTTPS, accountName), queueName);

            strUrl += string.Format("?visibilitytimeout={0:S}", visibilitytimeoutSeconds.ToString());
            if (numofmessages > 0)
                strUrl += string.Format("&numofmessages={0:S}", numofmessages.ToString());
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            else
                Request.Headers.Add("x-ms-client-request-id", Guid.NewGuid().ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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

        public static string PeekMessages(
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string queueName,
            int numofmessages = 0,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/messages", GetQueueUrl(useHTTPS, accountName), queueName);

            strUrl += "?peekonly=true";
            if (numofmessages > 0)
                strUrl += string.Format("&numofmessages={0:S}", numofmessages.ToString());
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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

        public static Dictionary<string, string> DeleteMessage(
             bool useHTTPS,
                string sharedKey,
                string accountName,
                string queueName,
                Guid messageId,
                string popreceipt,
                int timeoutSeconds = 0,
                Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/messages/{2:S}", GetQueueUrl(useHTTPS, accountName), queueName, messageId.ToString());


            strUrl += "?popreceipt=" + popreceipt;
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            else
                Request.Headers.Add("x-ms-client-request-id", Guid.NewGuid().ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public static Dictionary<string, string> UpdateMessage(
                bool useHTTPS,
                string sharedKey,
                string accountName,
                string queueName,
                string messageBody,
                Guid messageId,
                string popreceipt,
                int visibilitytimeoutSeconds,
                int timeoutSeconds = 0,
                string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/messages/{2:S}", GetQueueUrl(useHTTPS, accountName), queueName, messageId.ToString());

            strUrl += "?popreceipt=" + popreceipt;
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());
            strUrl += string.Format("&visibilitytimeout={0:S}", visibilitytimeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Request.GetRequestStream(), Encoding.UTF8))
            {
                sw.Write(messageBody);
            }

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);



            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public static Dictionary<string, string> PutMessage(
                bool useHTTPS,
                string sharedKey,
                string accountName,
                string queueName,
                string message,
                int visibilitytimeoutSeconds = 0,
                int messageTTLSeconds = 0,
                int timeoutSeconds = 0,
                Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/messages", GetQueueUrl(useHTTPS, accountName), queueName);

            string sQueryFormat = "?";

            if (visibilitytimeoutSeconds > 0)
            {
                strUrl += string.Format("{0:S}visibilitytimeout={1:S}", sQueryFormat, visibilitytimeoutSeconds.ToString());
                sQueryFormat = "&";
            }
            if (timeoutSeconds > 0)
            {
                strUrl += string.Format("{0:S}timeout={1:S}", sQueryFormat, timeoutSeconds.ToString());
                sQueryFormat = "&";
            }
            if (messageTTLSeconds > 0)
            {
                strUrl += string.Format("{0:S}messagettl={1:S}", sQueryFormat, messageTTLSeconds.ToString());
                sQueryFormat = "&";
            }

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "POST";

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion


            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Request.GetRequestStream(), Encoding.UTF8))
            {
                sw.Write(message);
            }

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
        }
        #endregion

        #region Table
        public static string GetTableUrl(bool useHTTPS, string accountName)
        {
            if (accountName != "devstoreaccount1")
                return string.Format("{0:S}://{1:S}.table.core.windows.net", useHTTPS ? "https" : "http", accountName);
            else
                return string.Format("{0:S}://localhost:10002/{1:S}", useHTTPS ? "https" : "http", accountName);
        }

        public static string CreateTable(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string tableName,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/Tables", GetTableUrl(useHTTPS, accountName));

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "POST";

            #region Debug
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Request.GetRequestStream(), Encoding.UTF8))
            {
                string content = string.Format(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><entry xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" xmlns=\"http://www.w3.org/2005/Atom\"> <title /> <updated>{0:S}</updated> <author><name/></author><id/><content type=\"application/xml\"><m:properties> <d:TableName>{1:S}</d:TableName> </m:properties> </content> </entry>",
                    System.Xml.XmlConvert.ToString(DateTime.Now, System.Xml.XmlDateTimeSerializationMode.Local), tableName);
                sw.Write(content);
            }
            #endregion

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/atom+xml,application/xml";
            Request.ContentType = "application/atom+xml";
            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            Request.Headers.Add("DataServiceVersion", "1.0;NetFx");
            Request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
        }

        public static void DeleteTable(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string tableName,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/Tables('{1:S}')", GetTableUrl(useHTTPS, accountName), tableName);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.ContentLength = 0;
            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if ((xmsclientrequestId.HasValue) && (xmsclientrequestId.Value != Guid.Empty))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.ToString());

            Request.Accept = "application/atom+xml,application/xml";
            Request.ContentType = "application/atom+xml";
            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "SQLToAzure";
            Request.Headers.Add("DataServiceVersion", "1.0;NetFx");
            Request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
                }
            }
        }

        public static string QueryTables(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/Tables", GetTableUrl(useHTTPS, accountName));

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";

            Request.ContentLength = 0;


            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2011-08-18");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/atom+xml,application/xml";
            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            Request.Headers.Add("DataServiceVersion", "1.0;NetFx");
            Request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");
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
            bool useHTTPS,
            string sharedKey,
            string accountName,
            string tableName,
            string NextPartitionKey,
            string NextRowKey,
            out string continuationNextPartitionKey,
            out string continuationNextRowKey,
            string xmsclientrequestId = null)
        {
            continuationNextPartitionKey = null;
            continuationNextRowKey = null;

            string strUrl = string.Format("{0:S}/{1:S}()", GetTableUrl(useHTTPS, accountName), tableName);

            char cNext = '?';
            if (!string.IsNullOrEmpty(NextPartitionKey))
            {
                strUrl += cNext + "NextPartitionKey=" + NextPartitionKey;
                cNext = '&';
            }
            if (!string.IsNullOrEmpty(NextRowKey))
            {
                strUrl += cNext + "NextRowKey=" + NextRowKey;
                cNext = '&';
            }

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";

            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2011-08-18");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/atom+xml,application/xml";
            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            Request.Headers.Add("DataServiceVersion", "2.0;NetFx");
            Request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.Headers.AllKeys.Contains("x-ms-continuation-NextPartitionKey"))
                    continuationNextPartitionKey = response.Headers["x-ms-continuation-NextPartitionKey"];
                if (response.Headers.AllKeys.Contains("x-ms-continuation-NextRowKey"))
                    continuationNextRowKey = response.Headers["x-ms-continuation-NextRowKey"];

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


        public static string DeleteEntity(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string tableName,
            TableEntity te,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}(PartitionKey='{2:S}',RowKey='{3:S}')", GetTableUrl(useHTTPS, accountName), tableName,
                te.PartitionKey, te.RowKey);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";

            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2011-08-18");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/atom+xml,application/xml";
            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            Request.Headers.Add("DataServiceVersion", "2.0;NetFx");
            Request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");

            Request.Headers.Add("If-Match", "*");
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public static string InsertOrReplaceTableEntity(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string tableName,
            TableEntity te,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}(PartitionKey='{2:S}',RowKey='{3:S}')", GetTableUrl(useHTTPS, accountName), tableName,
                te.PartitionKey, te.RowKey);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";

            #region Content
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Request.GetRequestStream(), Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" xmlns=\"http://www.w3.org/2005/Atom\"><title /><updated>{0:S}</updated><author><name /></author><id>{1:S}</id><content type=\"application/xml\"><m:properties>",
                    System.Xml.XmlConvert.ToString(DateTime.Now, System.Xml.XmlDateTimeSerializationMode.Utc), strUrl));


                System.Xml.XmlDocument docAttr = new System.Xml.XmlDocument();
                var attrElem = docAttr.CreateElement("root");

                #region Custom fields
                foreach (KeyValuePair<string, string> kvp in te.Attributes)
                {
                    attrElem.InnerText = kvp.Value;
                    sb.AppendFormat("<d:{0:S}>{1:S}</d:{0:S}>",
                        kvp.Key, kvp.Value);
                }
                #endregion


                System.Xml.XmlDocument docPartitionKey = new System.Xml.XmlDocument();
                var nodePartitionKey = docPartitionKey.CreateElement("root");
                nodePartitionKey.InnerXml = te.PartitionKey;

                var nodeRowKey = docPartitionKey.CreateElement("root2");
                nodeRowKey.InnerXml = te.RowKey;

                sb.AppendFormat("<d:PartitionKey>{0:S}</d:PartitionKey><d:RowKey>{1:S}</d:RowKey><d:Timestamp m:type=\"Edm.DateTime\">{2:S}</d:Timestamp>",
                    nodePartitionKey.InnerText,
                    nodeRowKey.InnerText,
                    System.Xml.XmlConvert.ToString(te.TimeStamp, System.Xml.XmlDateTimeSerializationMode.Utc));

                sb.Append("</m:properties></content></entry>");

                string s = sb.ToString();

                sw.Write(s);
            }
            #endregion

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2011-08-18");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            Request.Accept = "application/atom+xml,application/xml";
            Request.Headers.Add("Accept-Charset", "UTF-8");
            Request.UserAgent = "REST!";
            Request.Headers.Add("DataServiceVersion", "2.0;NetFx");
            Request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");

            Request.ContentType = "application/atom+xml";
            #endregion

            Signature.AddAzureAuthorizationHeaderLiteFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            }
        }
        #endregion

        #region Blob storage
        public static string GetBlobStorageUrl(bool useHTTPS, string accountName)
        {
            if (accountName != "devstoreaccount1")
                return string.Format("{0:S}://{1:S}.blob.core.windows.net", useHTTPS ? "https" : "http", accountName);
            else
                return string.Format("{0:S}://localhost:10000/{1:S}", useHTTPS ? "https" : "http", accountName);
        }

        #region Containers
        public static string ListContainers(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string prefix = null,
            string nextMarker = null,
            int maxResults = 0,
            bool IncludeMetadata = false,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}?comp=list", GetBlobStorageUrl(useHTTPS, accountName));

            if (!string.IsNullOrEmpty(nextMarker))
                strUrl += string.Format("&marker={0:S}", nextMarker);
            if (maxResults > 0)
                strUrl += string.Format("&maxresults={0:S}", maxResults.ToString());
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());
            if (IncludeMetadata)
                strUrl += "&include=metadata";
            if (!string.IsNullOrEmpty(prefix))
                strUrl += string.Format("&prefix={0:S}", prefix);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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

        public static string CreateContainer(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            Enumerations.ContainerPublicReadAccess containerAccessMethod,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
            )
        {
            string strUrl = string.Format("{0:S}/{1:S}?restype=container", GetBlobStorageUrl(useHTTPS, accountName), containerName);

            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            switch (containerAccessMethod)
            {
                case Enumerations.ContainerPublicReadAccess.Off:
                    break; // nothing to add
                case Enumerations.ContainerPublicReadAccess.Blob:
                    Request.Headers.Add("x-ms-blob-public-access", "blob");
                    break;
                case Enumerations.ContainerPublicReadAccess.Container:
                    Request.Headers.Add("x-ms-blob-public-access", "container");
                    break;
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
        }

        public static string DeleteContainer(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            Guid? leaseID,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
            )
        {
            string strUrl = string.Format("{0:S}/{1:S}?restype=container", GetBlobStorageUrl(useHTTPS, accountName), containerName);
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-lease-id", leaseID.ToString());
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Accepted, response.StatusCode);
            }
        }


        public static SharedAccessSignature.SharedAccessSignatureACL GetContainerACL
            (
                string accountName,
                string sharedKey,
                bool useHTTPS,
                string containerName,
                out ContainerPublicReadAccess containerPublicAccess,
                Guid? leaseID = null,
                int timeoutSeconds = 0,
                Guid? xmsclientrequestId = null
            )
        {
            containerPublicAccess = ContainerPublicReadAccess.Off;

            string strUrl = string.Format("{0:S}/{1:S}?restype=container&comp=acl", GetBlobStorageUrl(useHTTPS, accountName), containerName);
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2013-08-15");
            if (xmsclientrequestId.HasValue && xmsclientrequestId.Value != Guid.Empty)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-lease-id", leaseID.ToString());
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(SharedAccessSignature.SharedAccessSignatureACL));

                System.Xml.Serialization.XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Headers.AllKeys.FirstOrDefault(item => item == "x-ms-blob-public-access") != null)
                    {
                        switch (response.Headers["x-ms-blob-public-access"])
                        {
                            case "container":
                                containerPublicAccess = ContainerPublicReadAccess.Container;
                                break;
                            case "blob":
                                containerPublicAccess = ContainerPublicReadAccess.Blob;
                                break;
                            case "true":
                                containerPublicAccess = ContainerPublicReadAccess.PublicPrior20090919;
                                break;
                            default:
                                throw new ArgumentException("Received unsupported value (" + response.Headers["x-ms-blob-public-access"] + ") as x-ms-blob-public-access header. What is that?");
                        }
                    }

                    using (System.IO.Stream s = response.GetResponseStream())
                    {
                        return (SharedAccessSignature.SharedAccessSignatureACL)ser.Deserialize(s);
                    }
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        public static Dictionary<string, string> SetContainerACL(
                string accountName,
                string sharedKey,
                bool useHTTPS,
                string containerName,
                Guid? leaseID = null,
                SharedAccessSignature.SharedAccessSignatureACL queueACL = null,
                ContainerPublicReadAccess containerPublicAccess = ContainerPublicReadAccess.Off,
                int timeoutSeconds = 0,
                Guid? xmsclientrequestId = null
            )
        {
            if (queueACL == null)
                queueACL = new SharedAccessSignature.SharedAccessSignatureACL();

            string strUrl = string.Format("{0:S}/{1:S}?restype=container&comp=acl", GetBlobStorageUrl(useHTTPS, accountName), containerName);
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2013-08-15");
            if (xmsclientrequestId.HasValue && xmsclientrequestId.Value != Guid.Empty)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-lease-id", leaseID.ToString());
            }

            switch (containerPublicAccess)
            {
                case ContainerPublicReadAccess.Blob:
                    Request.Headers.Add("x-ms-blob-public-access", "blob");
                    break;
                case ContainerPublicReadAccess.Container:
                    Request.Headers.Add("x-ms-blob-public-access", "container");
                    break;
                case ContainerPublicReadAccess.Off:
                    //Don't add anything.
                    break;
                case ContainerPublicReadAccess.PublicPrior20090919:
                    throw new ArgumentException(ContainerPublicReadAccess.PublicPrior20090919.ToString() + " is not supported in SetContainerACL. It's legacy!");
            }
            #endregion

            #region Add content
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(SharedAccessSignature.SharedAccessSignatureACL));

            System.Xml.Serialization.XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (System.IO.Stream s = Request.GetRequestStream())
            {
                ser.Serialize(s, queueACL, namespaces);
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                Dictionary<string, string> dHeaders = new Dictionary<string, string>();
                foreach (string he in response.Headers)
                {
                    dHeaders.Add(he, response.Headers[he]);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return dHeaders;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }
        #endregion

        #region Blobs
        public static string ListBlobs(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string prefix = null,
            string nextMarker = null,
            int maxResults = 0,
            bool IncludeSnapshots = false,
            bool IncludeMetadata = false,
            bool IncludeUncommittedBlobs = false,
            bool IncludeCopy = false,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}?restype=container&comp=list", GetBlobStorageUrl(useHTTPS, accountName), containerName);

            if (!string.IsNullOrEmpty(nextMarker))
                strUrl += string.Format("&marker={0:S}", nextMarker);
            if (maxResults > 0)
                strUrl += string.Format("&maxresults={0:S}", maxResults.ToString());
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            bool fIncludeFirst = true;
            if (IncludeMetadata || IncludeSnapshots || IncludeCopy || IncludeUncommittedBlobs)
            {
                strUrl += "&include=";
                if (IncludeCopy)
                {
                    strUrl += "copy";
                    fIncludeFirst = false;
                }
                if (IncludeMetadata)
                {
                    if (!fIncludeFirst)
                        strUrl += ",";
                    else
                        fIncludeFirst = false;
                    strUrl += "metadata";
                }
                if (IncludeSnapshots)
                {
                    if (!fIncludeFirst)
                        strUrl += ",";
                    else
                        fIncludeFirst = false;
                    strUrl += "snapshots";
                }
                if (IncludeUncommittedBlobs)
                {
                    if (!fIncludeFirst)
                        strUrl += ",";
                    else
                        fIncludeFirst = false;
                    strUrl += "uncommittedblobs";
                }
            }
            if (!string.IsNullOrEmpty(prefix))
                strUrl += string.Format("&prefix={0:S}", prefix);

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

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

        public static string PutBlob(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string blobName,
            Enumerations.BlobType blobType,
            System.IO.Stream inputStream,
            string contentMD5 = null,
            Guid? leaseID = null,
            string contentType = "application/octet-stream",
            string contentEncoding = null,
            string contentLanguage = null,

            long PageBlobContentLengthBytes = 0, // only for page blobs must be 512 byte aligned.
            long PageBlobSequenceNumber = 0, // only for page blobs

            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            #region Ex-Ante checks
            if (blobType == Enumerations.BlobType.PageBlob && inputStream != null)
                throw new ArgumentException("Cannot specify an inputStream with PageBlobs.");

            if ((inputStream == null) && !(string.IsNullOrEmpty(contentMD5)))
                throw new ArgumentException("Cannot specify a contentMD5 with an empty (null) inputStream.");
            #endregion

            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), containerName, blobName);

            if (timeoutSeconds > 0)
                strUrl += string.Format("?timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";

            #region Body handling
            {
                if (inputStream != null)
                {
                    byte[] bBuffer = new byte[512];
                    int iRead;

                    System.Security.Cryptography.MD5 sscMD5 = null;

                    if (string.IsNullOrEmpty(contentMD5))
                        sscMD5 = System.Security.Cryptography.MD5.Create();
                    while ((iRead = inputStream.Read(bBuffer, 0, bBuffer.Length)) > 0)
                    {
                        Request.GetRequestStream().Write(bBuffer, 0, iRead);

                        if (string.IsNullOrEmpty(contentMD5))
                            sscMD5.TransformBlock(bBuffer, 0, iRead, bBuffer, 0);

                        // yield
                        System.Threading.Thread.Sleep(0);
                    }

                    if (string.IsNullOrEmpty(contentMD5))
                    {
                        sscMD5.TransformFinalBlock(bBuffer, 0, 0);
                        contentMD5 = Convert.ToBase64String(sscMD5.Hash);
                    }
                }
            }
            Request.GetRequestStream().Close();
            #endregion

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            if (!string.IsNullOrEmpty(contentType))
                Request.ContentType = contentType;
            if (!string.IsNullOrEmpty(contentEncoding))
                Request.Headers.Add("Content-Encoding", contentEncoding);
            if (!string.IsNullOrEmpty(contentLanguage))
                Request.Headers.Add("Content-Language", contentLanguage);

            if (!string.IsNullOrEmpty(contentMD5))
                Request.Headers.Add("Content-MD5", contentMD5);

            Request.Headers.Add("x-ms-blob-type", blobType.ToString());

            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
                Request.Headers.Add("x-ms-lease-id", leaseID.Value.ToString());

            if (blobType == Enumerations.BlobType.PageBlob)
            {
                if ((PageBlobContentLengthBytes % 512) != 0)
                    throw new ArgumentException(
                        string.Format(
                        "PageBlobContentLengthBytes must be 512 bytes aligned (PageBlobContentLengthBytes == {0:N0} bytes, {0:N0} modulus 512 == {1:N0}).",
                        PageBlobContentLengthBytes, PageBlobContentLengthBytes % 512));

                Request.Headers.Add("x-ms-blob-content-length", PageBlobContentLengthBytes.ToString());

                if (PageBlobSequenceNumber != 0)
                    Request.Headers.Add("x-ms-blob-sequence-number", PageBlobSequenceNumber.ToString());
            }

            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return contentMD5;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
        }

        public static string PutPage(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string blobName,
            Enumerations.PutPageOperation Operation,
            long lStartRange, long? lBytesToPut,
            System.IO.Stream inputStream = null,
            string contentMD5 = null,
            Guid? leaseID = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            #region Ex-ante checks
            if (Operation == Enumerations.PutPageOperation.Clear && inputStream != null)
                throw new ArgumentException("Cannot specify a clear operation with a valid inputstream (pass null as inputstream).");

            if ((inputStream.Length % 512) != 0)
                throw new ArgumentException(
                    string.Format("PutPages operations require a 512-byte aligned blocks. Inputstream.lenght is {0:N0} bytes instead.",
                    inputStream.Length));

            if ((lStartRange % 512) != 0)
                throw new ArgumentException(
                    string.Format("PutPages operations require a 512-byte aligned blocks. lStartRange is {0:N0} instead.",
                    lStartRange));

            if (Operation == Enumerations.PutPageOperation.Clear && !lBytesToPut.HasValue)
                throw new ArgumentException("If you use Clear operation your must specify an end range.");

            //if (Operation == Enumerations.PutPageOperation.Update && lEndRange.HasValue)
            //    if (lEndRange.Value != (lStartRange + inputStream.Length))
            //        throw new ArgumentException("If you use Update operation your cannot specify an end range different than lStartRange+inputStream.Length (your value " +
            //            "is " + lEndRange.Value + ".");

            if (lBytesToPut.HasValue && (lBytesToPut.Value % 512) != 0)
                throw new ArgumentException(
                    string.Format("PutPages operations require a 512-byte aligned blocks. lBytesToPut is {0:N0} instead.",
                    lBytesToPut));

            if ((!lBytesToPut.HasValue) && (inputStream.Length % 512) != 0)
                throw new ArgumentException(
                    string.Format("PutPages operations require a 512-byte aligned blocks. lBytesToPut is null and inputStream.Length is {0:N0}.",
                    inputStream.Length));
            #endregion

            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), containerName, blobName);
            strUrl += "?comp=page";

            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";

            #region Body handling
            {
                byte[] bBuffer = new byte[512];
                int iRead;

                System.Security.Cryptography.MD5 sscMD5 = null;

                if (string.IsNullOrEmpty(contentMD5))
                    sscMD5 = System.Security.Cryptography.MD5.Create();

                long lToRead = inputStream.Length;
                if (lBytesToPut.HasValue && lBytesToPut.Value < lToRead)
                    lToRead = lBytesToPut.Value;
                long lPosition = 0L;

                while (lPosition < lToRead)
                {
                    iRead = inputStream.Read(bBuffer, 0, bBuffer.Length);
                    Request.GetRequestStream().Write(bBuffer, 0, iRead);

                    if (string.IsNullOrEmpty(contentMD5))
                        sscMD5.TransformBlock(bBuffer, 0, iRead, bBuffer, 0);

                    // yield
                    System.Threading.Thread.Sleep(0);
                    lPosition += iRead;
                }

                if (string.IsNullOrEmpty(contentMD5))
                {
                    sscMD5.TransformFinalBlock(bBuffer, 0, 0);
                    contentMD5 = Convert.ToBase64String(sscMD5.Hash);
                }
            }
            Request.GetRequestStream().Close();
            #endregion

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            Request.Headers.Add("Content-MD5", contentMD5);

            Request.Headers.Add("x-ms-page-write", Operation == Enumerations.PutPageOperation.Clear ? "clear" : "update");


            string strRange;
            if (lBytesToPut.HasValue && (lBytesToPut.Value < inputStream.Length))
                strRange = "bytes=" + lStartRange + "-" + ((lBytesToPut + lStartRange) - 1);
            else
                strRange = "bytes=" + lStartRange + "-" + ((inputStream.Length + lStartRange) - 1);

            Request.Headers.Add("x-ms-range", strRange);

            if (Operation == Enumerations.PutPageOperation.Clear)
                Request.ContentLength = 0;

            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
                Request.Headers.Add("x-ms-lease-id", leaseID.Value.ToString());

            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return contentMD5;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
            }
        }

        public static string DeleteBlob(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string blobName,
            Enumerations.BlobDeletionMethod BlobDeletionMethod,
            Guid? leaseID = null,
            DateTime? snapshotDateTimeToDelete = null,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
            )
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), containerName, blobName);

            char cConc = '?';
            if (timeoutSeconds > 0)
            {
                strUrl += string.Format(cConc + "timeout={0:S}", timeoutSeconds.ToString());
                cConc = '&';
            }
            if (snapshotDateTimeToDelete.HasValue)
            {
                strUrl += string.Format(cConc + "snapshot={0:S}", snapshotDateTimeToDelete.Value.ToString("R"));
                cConc = '&';
            }

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "DELETE";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);
            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-lease-id", leaseID.ToString());
            }

            switch (BlobDeletionMethod)
            {
                case Enumerations.BlobDeletionMethod.All:
                    Request.Headers.Add("x-ms-delete-snapshots", "include");
                    break;
                case Enumerations.BlobDeletionMethod.SnapshotsOnly:
                    Request.Headers.Add("x-ms-delete-snapshots", "only");
                    break;
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return str;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Accepted, response.StatusCode);
            }
        }

        public static Dictionary<string, string> GetBlob(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string blobName,
            out System.IO.Stream inputStream,
            string range = null,
            bool getContentMD5 = false,
            Guid? leaseID = null,
            DateTime? snapshotDateTime = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            System.Net.HttpWebResponse response = null;
            System.Net.HttpWebRequest Request = _GetBlobCreateRequest(accountName, sharedKey, useHTTPS,
                containerName, blobName, range, getContentMD5, leaseID, snapshotDateTime, timeoutSeconds, xmsclientrequestId);

            try
            {
                try
                {
                    response = (System.Net.HttpWebResponse)Request.GetResponse();
                }
                catch
                {
                    if (response != null)
                        response.Dispose();

                    Request = _GetBlobCreateRequest(accountName, sharedKey, useHTTPS,
                    containerName, blobName, range, getContentMD5, leaseID, snapshotDateTime, timeoutSeconds, xmsclientrequestId);
                    response = (System.Net.HttpWebResponse)Request.GetResponse();
                }

                inputStream = new System.IO.MemoryStream((int)response.ContentLength);
                byte[] bBuffer = new byte[512];
                int iRead;
                while ((iRead = response.GetResponseStream().Read(bBuffer, 0, bBuffer.Length)) > 0)
                    inputStream.Write(bBuffer, 0, iRead);

                inputStream.Seek(0, System.IO.SeekOrigin.Begin);

                Dictionary<string, string> dHeaders = new Dictionary<string, string>();

                foreach (string he in response.Headers)
                {
                    dHeaders.Add(he, response.Headers[he]);
                }
                if (string.IsNullOrEmpty(range))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return dHeaders;
                    }
                    else
                        throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                    {
                        return dHeaders;
                    }
                    else
                        throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.PartialContent, response.StatusCode);
                }
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
        }

        private static System.Net.HttpWebRequest _GetBlobCreateRequest(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string blobName,
            string range = null,
            bool getContentMD5 = false,
            Guid? leaseID = null,
            DateTime? snapshotDateTime = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), containerName, blobName);

            char cConc = '?';
            if (timeoutSeconds > 0)
            {
                strUrl += string.Format(cConc + "timeout={0:S}", timeoutSeconds.ToString());
                cConc = '&';
            }
            if (snapshotDateTime.HasValue)
            {
                strUrl += string.Format(cConc + "snapshot={0:S}", snapshotDateTime.Value.ToString("R"));
                cConc = '&';
            }

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "GET";
            Request.ContentLength = 0;

            #region Add HTTP headers
            DateTime dt = DateTime.UtcNow;
            string strDate = dt.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            if (!string.IsNullOrEmpty(range))
            {
                Request.Headers.Add("x-ms-range", range);

                if (getContentMD5)
                {
                    Request.Headers.Add("x-ms-range-get-content-md5", "true");
                }
            }
            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
                Request.Headers.Add("x-ms-lease-id", leaseID.Value.ToString());

            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            return Request;
        }

        public static Dictionary<string, string> CopyBlob(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string destinationContainerName,
            string destinationBlobName,
            Uri sourceBlobUri,
            Guid? sourceLeaseID = null,
            Guid? destinationLeaseID = null,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
    )
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), destinationContainerName, destinationBlobName);

            char cConc = '?';
            if (timeoutSeconds > 0)
            {
                strUrl += string.Format(cConc + "timeout={0:S}", timeoutSeconds.ToString());
                cConc = '&';
            }

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            Request.Headers.Add("x-ms-copy-source", sourceBlobUri.ToString());

            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            if ((destinationLeaseID.HasValue) && (destinationLeaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-lease-id", destinationLeaseID.ToString());
            }

            if ((sourceLeaseID.HasValue) && (sourceLeaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-source-lease-id", sourceLeaseID.ToString());
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    Dictionary<string, string> dHeaders = new Dictionary<string, string>();

                    foreach (string he in response.Headers)
                    {
                        dHeaders.Add(he, response.Headers[he]);
                    }

                    return dHeaders;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Accepted, response.StatusCode);
            }
        }

        public static Dictionary<string, string> GetBlobProperty(
            string accountName,
            string sharedKey,
            bool useHTTPS,
            string containerName,
            string blobName,
            Guid? leaseID = null,
            DateTime? snapshotDateTime = null,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
        )
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), containerName, blobName);

            char cConc = '?';
            if (timeoutSeconds > 0)
            {
                strUrl += string.Format(cConc + "timeout={0:S}", timeoutSeconds.ToString());
                cConc = '&';
            }
            if (snapshotDateTime.HasValue && (snapshotDateTime.Value != DateTime.MinValue))
            {
                strUrl += string.Format(cConc + "snapshot={0:S}", snapshotDateTime.Value.ToString("R"));
                cConc = '&';
            }

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "HEAD";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");

            if (!string.IsNullOrEmpty(xmsclientrequestId))
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId);

            if ((leaseID.HasValue) && (leaseID.Value != Guid.Empty))
            {
                Request.Headers.Add("x-ms-lease-id", leaseID.Value.ToString());
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Dictionary<string, string> dHeaders = new Dictionary<string, string>();

                    foreach (string he in response.Headers)
                    {
                        dHeaders.Add(he, response.Headers[he]);
                    }

                    return dHeaders;
                }
                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        public static Dictionary<string, string> SetBlobMetadata(
            string accountName, string sharedKey, bool useHTTPS,
            string container,
            string blobName,
            Dictionary<string, string> kvpMetadata,
            Guid? leaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null
 )
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), container, blobName);

            strUrl += "?comp=metadata";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            if (leaseId.HasValue)
                Request.Headers.Add("x-ms-lease-id", leaseId.Value.ToString());

            foreach (KeyValuePair<string, string> kvp in kvpMetadata)
            {
                Request.Headers.Add(kvp.Key, kvp.Value);
            }
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        public static Dictionary<string, string> GetBlobMetadata(
            string accountName, string sharedKey, bool useHTTPS,
            string container,
            string blobName,
            Guid? leaseId = null,
            DateTime? snapshotDateTime = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), container, blobName);

            strUrl += "?comp=metadata";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());
            if (snapshotDateTime.HasValue)
                strUrl += string.Format("&snapshot={0:S}", snapshotDateTime.Value.ToString("R"));

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "HEAD";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            if (leaseId.HasValue)
                Request.Headers.Add("x-ms-lease-id", leaseId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string header in response.Headers)
                    {
                        d.Add(header, response.Headers[header]);
                    }

                    return d;
                }

                else
                    throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        #region Lease
        public static Dictionary<string, string> LeaseBlob(
            string accountName, string sharedKey, bool useHTTPS,
            string container,
            string blobName,
            Enumerations.LeaseOperation leaseOperation,
            Guid? leaseId = null,
            int? leaseBreakPeriod = null,
            int? leaseDuration = null, // use -1 for infinite
            Guid? proposedLeaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}/{2:S}", GetBlobStorageUrl(useHTTPS, accountName), container, blobName);

            strUrl += "?comp=lease";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (leaseId.HasValue)
                Request.Headers.Add("x-ms-lease-id", leaseId.Value.ToString());
            Request.Headers.Add("x-ms-lease-action", leaseOperation.ToString().ToLowerInvariant());
            if (leaseBreakPeriod.HasValue)
            {
                if (leaseBreakPeriod < 0 || leaseBreakPeriod > 60)
                    throw new ArgumentException("lease period should be between 0 and 60 seconds (passed value is " + leaseBreakPeriod + ")");
                Request.Headers.Add("x-ms-lease-break-period", leaseBreakPeriod.Value.ToString());
            }
            if (leaseDuration.HasValue)
            {
                if ((leaseDuration != -1) && (leaseDuration < 15 || leaseDuration > 60))
                    throw new ArgumentException("lease period should be between 15 and 60 seconds or -1 for infinite leases. (passed value is " + leaseBreakPeriod + ")");
                Request.Headers.Add("x-ms-lease-duration", leaseDuration.Value.ToString());
            }
            if (proposedLeaseId.HasValue)
                Request.Headers.Add("x-ms-proposed-lease-id", proposedLeaseId.Value.ToString());
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                foreach (string header in response.Headers)
                {
                    d.Add(header, response.Headers[header]);
                }

                switch (leaseOperation)
                {
                    case Enumerations.LeaseOperation.Acquire:
                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Break:
                        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Accepted, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Change:
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Release:
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Renew:
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                        break;
                }
                return d;
            }
        }

        public static Dictionary<string, string> LeaseContainer(
            string accountName, string sharedKey, bool useHTTPS,
            string container, // use $root for root container
            Enumerations.LeaseOperation leaseOperation,
            Guid? leaseId = null,
            int? leaseBreakPeriod = null,
            int? leaseDuration = null, // use -1 for infinite
            Guid? proposedLeaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string strUrl = string.Format("{0:S}/{1:S}", GetBlobStorageUrl(useHTTPS, accountName), container);

            strUrl += "?comp=lease&restype=container";
            if (timeoutSeconds > 0)
                strUrl += string.Format("&timeout={0:S}", timeoutSeconds.ToString());

            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strUrl);
            Request.Method = "PUT";
            Request.ContentLength = 0;

            #region Add HTTP headers
            string strDate = DateTime.UtcNow.ToString("R");

            Request.Headers.Add("x-ms-date", strDate);
            Request.Headers.Add("x-ms-version", "2012-02-12");
            if (leaseId.HasValue)
                Request.Headers.Add("x-ms-lease-id", leaseId.Value.ToString());
            Request.Headers.Add("x-ms-lease-action", leaseOperation.ToString().ToLowerInvariant());
            if (leaseBreakPeriod.HasValue)
            {
                if (leaseBreakPeriod < 0 || leaseBreakPeriod > 60)
                    throw new ArgumentException("lease period should be between 0 and 60 seconds (passed value is " + leaseBreakPeriod + ")");
                Request.Headers.Add("x-ms-lease-break-period", leaseBreakPeriod.Value.ToString());
            }
            if (leaseDuration.HasValue)
            {
                if ((leaseDuration != -1) && (leaseDuration < 15 || leaseDuration > 60))
                    throw new ArgumentException("lease period should be between 15 and 60 seconds or -1 for infinite leases. (passed value is " + leaseBreakPeriod + ")");
                Request.Headers.Add("x-ms-lease-duration", leaseDuration.Value.ToString());
            }
            if (proposedLeaseId.HasValue)
                Request.Headers.Add("x-ms-proposed-lease-id", proposedLeaseId.Value.ToString());
            if (xmsclientrequestId.HasValue)
                Request.Headers.Add("x-ms-client-request-id", xmsclientrequestId.Value.ToString());
            #endregion

            Signature.AddAzureAuthorizationHeaderFromSharedKey(Request, sharedKey);

            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)Request.GetResponse())
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                foreach (string header in response.Headers)
                {
                    d.Add(header, response.Headers[header]);
                }

                switch (leaseOperation)
                {
                    case Enumerations.LeaseOperation.Acquire:
                        if (response.StatusCode != System.Net.HttpStatusCode.Created)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Created, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Break:
                        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.Accepted, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Change:
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Release:
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                        break;
                    case Enumerations.LeaseOperation.Renew:
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            throw new Exceptions.UnexpectedResponseTypeCodeException(System.Net.HttpStatusCode.OK, response.StatusCode);
                        break;
                }
                return d;
            }
        }
        #endregion

        #endregion
        #endregion

        #region Private util methods
        internal static T PerformSimpleGet<T>(Uri uri, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate,
            string azureVersion)
        {
            System.Net.HttpWebRequest Request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
            Request.Method = "GET";
            Request.Headers.Add(Constants.HEADER_VERSION, azureVersion);
            Request.ClientCertificates.Add(certificate);

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
