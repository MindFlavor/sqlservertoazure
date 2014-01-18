using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ITPCfSQL.Azure.Internal
{
    public class Signature
    {
        public static void AddAzureAuthorizationHeaderFromSharedKey(System.Net.HttpWebRequest req, string sharedKey)
        {
            String AuthorizationHeader = "SharedKey " + GetAccountNameFromUri(req.RequestUri) + ":" + GenerateAzureSignatureFromSharedKey(req, sharedKey);

            req.Headers.Add("Authorization", AuthorizationHeader);
        }

        public static void AddAzureAuthorizationHeaderLiteFromSharedKey(System.Net.HttpWebRequest req, string sharedKey)
        {
            string StringToSign = req.Headers["x-ms-date"] + "\n";
            StringToSign += GenerateCanonicalizedResource(req.RequestUri);

            string signature = StringToSign;
            //System.Diagnostics.Trace.TraceInformation("GenerateAzureSignatureFromSharedKey: Generated signature: " + StringToSign);

            byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(signature);
            System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(sharedKey));

            string strHash2Base64 = Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));

            String AuthorizationHeader = "SharedKeyLite " + GetAccountNameFromUri(req.RequestUri) + ":" + strHash2Base64;

            req.Headers.Add("Authorization", AuthorizationHeader);
        }

        public static string SignTheStringToSign(string stringToSign, string sharedKey)
        {
            byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);
            System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(sharedKey));

            return Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));
        }


        public static string GenerateAzureSignatureFromSharedKey(System.Net.HttpWebRequest req, string sharedKey)
        {
            StringBuilder sb = new StringBuilder();

            string CanonicalizedResource = GenerateCanonicalizedResource(req.RequestUri);
            string CanonicalizedHeader = GenerateCanonicalizedHeader(req);


            #region Method (aka VERB)
            sb.Append(req.Method + "\n");
            #endregion

            #region Content-Encoding
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "Content-Encoding") != null)
                sb.Append(req.Headers["Content-Encoding"] + "\n");
            else
                sb.Append("\n");
            #endregion

            #region Content-Language
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "Content-Language") != null)
                sb.Append(req.Headers["Content-Language"] + "\n");
            else
                sb.Append("\n");
            #endregion

            #region Content-Length
            if (((req.Method == "GET") || (req.Method == "HEAD")) && (req.ContentLength == 0))
                sb.Append("\n");

            else if ((req.Method == "DELETE") && (req.ContentLength < 0))
                sb.Append("\n");
            else
                sb.Append(req.ContentLength + "\n");
            #endregion

            #region Content-MD5
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "Content-MD5") != null)
                sb.Append(req.Headers["Content-MD5"] + "\n");
            else
                sb.Append("\n");
            #endregion

            #region Content-Type
            if (!string.IsNullOrEmpty(req.ContentType))
                sb.Append(req.ContentType + "\n");
            else
                sb.Append("\n");
            #endregion

            #region Date
            if (req.Date != DateTime.MinValue)
                sb.Append(req.Date.ToString("R") + "\n");
            else
                sb.Append("\n");
            #endregion

            #region If-Modified-Since
            if (req.IfModifiedSince != DateTime.MinValue)
                sb.Append(req.IfModifiedSince.ToString("R") + "\n");
            else
                sb.Append("\n");
            #endregion

            #region If-Match
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "If-Match") != null)
                sb.Append(req.Headers["If-Match"] + "\n");
            else
                sb.Append("\n");
            #endregion

            #region If-None-Match
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "If-None-Match") != null)
                sb.Append(req.Headers["If-None-Match"] + "\n");
            else
                sb.Append("\n");
            #endregion

            #region If-Unmodified-Since
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "If-Unmodified-Since") != null)
                sb.Append(req.Headers["If-Unmodified-Since"] + "\n");
            else
                sb.Append("\n");
            #endregion

            #region Range
            if (req.Headers.AllKeys.FirstOrDefault(item => item == "Range") != null)
                sb.Append(req.Headers["Range"] + "\n");
            else
                sb.Append("\n");
            #endregion

            sb.Append(CanonicalizedHeader);
            sb.Append(CanonicalizedResource);

            string signature = sb.ToString();
            //System.Diagnostics.Trace.TraceInformation("GenerateAzureSignatureFromSharedKey: Generated signature: " + signature);

            return SignTheStringToSign(signature, sharedKey);

            //byte[] SignatureBytes = System.Text.Encoding.UTF8.GetBytes(signature);
            //System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(sharedKey));

            //string strHash2Base64 = Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));
            ////System.Diagnostics.Trace.TraceInformation("GenerateAzureSignatureFromSharedKey: Generated hash: " + strHash2Base64);
            //return strHash2Base64;
        }


        public static string GenerateCanonicalizedHeader(System.Net.HttpWebRequest req)
        {
            StringBuilder sb = new StringBuilder();
            string[] strHeadersSorted = req.Headers.AllKeys.AsParallel().Where(item => item.StartsWith("x-ms-")).OrderBy(item => item).ToArray();

            foreach (string sHeader in strHeadersSorted)
            {
                sb.AppendFormat(
                    "{0:S}:{1:S}\n",
                    sHeader.ToLowerInvariant(),
                    req.Headers[sHeader].Trim()
                    );
            }

            //System.Diagnostics.Trace.TraceInformation("GenerateCanonicalizedHeader: Generated string: " + sb.ToString());
            return sb.ToString();
        }

        public static string GetAccountNameFromUri(Uri uri)
        {
            int idx = uri.Host.IndexOf('.');
            if (idx == -1)
            {
                idx = uri.PathAndQuery.IndexOf('/') + 1;
                int iEnd = uri.PathAndQuery.IndexOfAny(new char[] { '?', '/' }, idx);
                if (iEnd == -1)
                    iEnd = uri.PathAndQuery.Length;
                return uri.PathAndQuery.Substring(idx, iEnd - idx);
            }
            else
                return uri.Host.Substring(0, idx);
        }

        public static string GenerateCanonicalizedResource(string uri)
        {
            return GenerateCanonicalizedResource(new Uri(uri));
        }

        public static string GenerateCanonicalizedResource(Uri uri)
        {
            StringBuilder sb = new StringBuilder("/");

            sb.AppendFormat("{0:S}", GetAccountNameFromUri(uri));

            string[] tokens = uri.AbsolutePath.Split(new char[] { '/' });
            bool fAdded = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (!string.IsNullOrEmpty(tokens[i]))
                {
                    sb.AppendFormat("/{0:S}", tokens[i]);
                    fAdded = true;
                }
            }
            if (!fAdded)
                sb.Append("/");

            Dictionary<string, List<string>> dParams = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(uri.Query))
            {
                foreach (string sParam in uri.Query.Substring(1).Split(new char[] { '&' }))
                {
                    int idx = sParam.IndexOf('=');
                    string sKey = Uri.UnescapeDataString(sParam.Substring(0, idx)).ToLowerInvariant();
                    string sValue = Uri.UnescapeDataString(sParam.Substring(idx + 1, sParam.Length - idx - 1));

                    if (!dParams.ContainsKey(sKey))
                    {
                        if ((sKey == "nextpartitionkey") || (sKey == "nextrowkey"))
                            continue;
                        dParams[sKey] = new List<string>();
                    }
                    dParams[sKey].Add(sValue);
                }
            }

            KeyValuePair<string, List<string>>[] kvpairs = dParams.AsParallel().OrderBy(item => item.Key).ToArray();

            if (kvpairs.Length > 0)
                sb.Append("\n");

            for (int iPair = 0; iPair < kvpairs.Length; iPair++)
            {
                StringBuilder sbParam = new StringBuilder();
                sbParam.AppendFormat("{0:S}:", kvpairs[iPair].Key);
                string[] vals = kvpairs[iPair].Value.AsParallel().OrderBy(item => item).ToArray();
                for (int i = 0; i < vals.Length; i++)
                {
                    sbParam.Append(vals[i]);
                    if (i + 1 < vals.Length)
                        sbParam.Append(",");
                }

                if ((iPair + 1) < kvpairs.Length)
                    sbParam.Append("\n");

                sb.Append(sbParam);
            }

            //System.Diagnostics.Trace.TraceInformation("GenerateCanonicalizedResource: Generated string: " + sb.ToString());
            return sb.ToString();//.Replace("%82", new string(new char[] { (char)0xEF, (char)0xBF, (char)0xBD }));
        }

        public static Uri GenerateSharedAccessSignatureURI(
            Uri ResourceURI,
            string azureStorageSharedKey,
            string Permissions,
            string Resource,
            DateTime? ValidityStart,
            DateTime? ValidityExpiry,
            string startPK,
            string endPK,
            string Identifier = null,
            string Version = "2012-02-12")
        {
            StringBuilder sb = new StringBuilder(ResourceURI.AbsoluteUri);

            string signedstart = null;
            string signedexpiry = null;

            if (ValidityStart.HasValue)
                signedstart = ValidityStart.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (ValidityExpiry.HasValue)
                signedexpiry = ValidityExpiry.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");// ("O"); 

            string strPathWithoutParams = ResourceURI.AbsoluteUri;
            char fChar = '?';
            if (ResourceURI.PathAndQuery.Contains('?'))
            {
                fChar = '&';
                strPathWithoutParams = strPathWithoutParams.Substring(0, strPathWithoutParams.IndexOf('?'));
            }

            #region If container, make sure to strip off the potential blob and replace container with $root...
            if (Resource == "c")
            {
                int iSkip = "https://".Length;
                if (strPathWithoutParams.StartsWith("http://"))
                    iSkip = "http://".Length;

                int idx0 = strPathWithoutParams.IndexOf('/', iSkip);
                int idx1 = strPathWithoutParams.IndexOf('/', idx0 + 1);
                if (idx1 != -1)
                    strPathWithoutParams = strPathWithoutParams.Substring(0, idx1);// +"/$root";
            }
            #endregion

            sb.Append(fChar + "sv=" + Version);
            if (!string.IsNullOrEmpty(signedstart))
                sb.Append("&st=" + signedstart);
            if (!string.IsNullOrEmpty(signedexpiry))
                sb.Append("&se=" + signedexpiry);

            sb.Append("&sr=" + Resource);

            if (!string.IsNullOrEmpty(Permissions))
                sb.Append("&sp=" + Permissions);

            if (!string.IsNullOrEmpty(startPK))
                sb.Append("&spk=" + startPK);
            if (!string.IsNullOrEmpty(endPK))
                sb.Append("&epk=" + endPK);

            if (!string.IsNullOrEmpty(Identifier))
                sb.Append("&si=" + Identifier);

            string canonicalizedresource = Signature.GenerateCanonicalizedResource(strPathWithoutParams);

            // calculate signature!
            string StringToSign =
                   Permissions + "\n" +
                   signedstart + "\n" +
                   signedexpiry + "\n" +
                   canonicalizedresource + "\n" +
                   Identifier + "\n" +
                   Version;

            string signature = Signature.SignTheStringToSign(StringToSign, azureStorageSharedKey);
            signature = Uri.EscapeDataString(signature);

            sb.Append("&sig=" + signature);
            return new Uri(sb.ToString());
        }
    }
}
