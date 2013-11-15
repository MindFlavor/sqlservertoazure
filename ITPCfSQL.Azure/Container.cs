using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ITPCfSQL.Azure
{
    public class Container
    {
        #region Properties
        public AzureBlobService AzureBlobService { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }
        public string Etag { get; set; }
        public DateTime LastModified { get; set; }
        public Enumerations.LeaseStatus LeaseStatus { get; set; }
        public Enumerations.LeaseState LeaseState { get; set; }
        public Enumerations.LeaseDuration? LeaseDuration { get; set; }
        #endregion

        public void Delete(Guid? leaseID = null, int timeoutSeconds = 0, string xmsclientrequestId = null)
        {
            Internal.InternalMethods.DeleteContainer(
                AzureBlobService.AccountName, AzureBlobService.SharedKey, AzureBlobService.UseHTTPS,
                this.Name,
                leaseID: leaseID,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Blob GetBlob(string blobName)
        {
            return new Blob(this, blobName);
        }

        public PageBlob GetPageBlob(string blobName)
        {
            return new PageBlob(this, blobName);
        }

        public List<Blob> ListBlobs(
            string prefix = null,
            bool includeSnapshots = false,
            bool includeMetadata = false,
            bool includeCopy = false,
            bool includeUncommittedBlobs = false,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null
            )
        {
            string strNextMarker = null;
            List<Blob> lBlobs = new List<Blob>();

            do
            {
                string res = Internal.InternalMethods.ListBlobs(
                    AzureBlobService.AccountName, AzureBlobService.SharedKey, AzureBlobService.UseHTTPS,
                    this.Name, prefix,
                    IncludeSnapshots: includeSnapshots,
                    IncludeMetadata: includeMetadata,
                    IncludeCopy: includeCopy,
                    IncludeUncommittedBlobs: includeUncommittedBlobs,
                    timeoutSeconds: timeoutSeconds,
                    xmsclientrequestId: xmsclientrequestId);

                System.Xml.XmlDocument doc = new XmlDocument();
                doc.LoadXml(res);

                lBlobs.AddRange(Blob.ParseFromXMLEnumerationResults(this, doc));
                strNextMarker = Blob.GetNextMarkerFromXMLEnumerationResults(doc);

            } while (!string.IsNullOrEmpty(strNextMarker));

            return lBlobs;
        }

        public BlockBlob CreateBlockBlob(
            string blobName,
            System.IO.Stream inputStream,
            Guid? leaseID = null,
            string contentType = "application/octet-stream",
            string contentEncoding = null,
            string contentLanguage = null,
            string contentMD5 = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string retContentMD5 = Internal.InternalMethods.PutBlob(
                AzureBlobService.AccountName, AzureBlobService.SharedKey, AzureBlobService.UseHTTPS,
                this.Name, blobName, Enumerations.BlobType.BlockBlob, inputStream,
                contentMD5,
                leaseID, contentType, contentEncoding, contentLanguage, 
                timeoutSeconds: timeoutSeconds, xmsclientrequestId: xmsclientrequestId);

            string strUrl = string.Format("{0:S}://{1:S}.blob.core.windows.net/{2:S}/{3:S}",
             AzureBlobService.UseHTTPS ? "https" : "http", AzureBlobService.AccountName, this.Name, blobName);

            return new BlockBlob(this)
            {
                Name = blobName,
                Url = new Uri(strUrl),
                ContentEncoding = contentEncoding,
                ContentLanguage = contentLanguage,
                ContentMD5 = retContentMD5,
                ContentType = contentType
            };
        }

        public PageBlob CreatePageBlob(
            string blobName,
            long lBlobSize,
            Guid? leaseID = null,
            string contentType = "application/octet-stream",
            string contentEncoding = null,
            string contentLanguage = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            string retContentMD5 = Internal.InternalMethods.PutBlob(
                AzureBlobService.AccountName, AzureBlobService.SharedKey, AzureBlobService.UseHTTPS,
                this.Name, blobName, Enumerations.BlobType.PageBlob,
                null, null,
                leaseID, contentType, contentEncoding, contentLanguage,
                lBlobSize,
                timeoutSeconds: timeoutSeconds, xmsclientrequestId: xmsclientrequestId);

            string strUrl = string.Format("{0:S}://{1:S}.blob.core.windows.net/{2:S}/{3:S}",
             AzureBlobService.UseHTTPS ? "https" : "http", AzureBlobService.AccountName, this.Name, blobName);

            return new PageBlob(this)
            {
                Name = blobName,
                Url = new Uri(strUrl),
                ContentEncoding = contentEncoding,
                ContentLanguage = contentLanguage,
                ContentMD5 = retContentMD5,
                ContentType = contentType
            };
        }

        #region Static parsing methods
        public static Container ParseFromXMLNode(AzureBlobService abs, XmlNode node)
        {
            Container cont = new Container()
            {
                Name = node.SelectSingleNode("Name").InnerText,
                Url = new Uri(node.SelectSingleNode("Url").InnerText),
                LastModified = DateTime.Parse(node.SelectSingleNode("Properties/Last-Modified").InnerText),
                Etag = node.SelectSingleNode("Properties/Etag").InnerText,
                LeaseState = (Enumerations.LeaseState)Enum.Parse(typeof(Enumerations.LeaseState), node.SelectSingleNode("Properties/LeaseState").InnerText, true),
                LeaseStatus = (Enumerations.LeaseStatus)Enum.Parse(typeof(Enumerations.LeaseStatus), node.SelectSingleNode("Properties/LeaseStatus").InnerText, true),
                AzureBlobService = abs
            };

            XmlNode n = node.SelectSingleNode("Properties/LeaseDuration");
            if (n != null)
                cont.LeaseDuration = (Enumerations.LeaseDuration)Enum.Parse(typeof(Enumerations.LeaseDuration), n.InnerText, true);

            return cont;
        }

        public static IEnumerable<Container> ParseFromXMLEnumerationResults(AzureBlobService abs, XmlDocument doc)
        {
            foreach (XmlNode n in doc.SelectNodes("EnumerationResults/Containers/Container"))
            {
                yield return ParseFromXMLNode(abs, n);
            }
        }

        public static string GetNextMarkerFromXMLEnumerationResults(XmlDocument doc)
        {
            XmlNode n = doc.SelectSingleNode("EnumerationResults/NextMarker");
            if (n != null)
                return n.InnerText;
            else
                return null;
        }
        #endregion

        #region Lease
        protected internal Responses.LeaseBlobResponse LeaseContainer(
            Enumerations.LeaseOperation leaseOperation,
            Guid? leaseId = null,
            int? leaseBreakPeriod = null,
            int? leaseDuration = null, // use -1 for infinite
            Guid? proposedLeaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            Dictionary<string, string> dResp = ITPCfSQL.Azure.Internal.InternalMethods.LeaseContainer(
                this.AzureBlobService.AccountName, this.AzureBlobService.SharedKey, this.AzureBlobService.UseHTTPS,
                this.Name,
                leaseOperation, leaseId, leaseBreakPeriod, leaseDuration,
                proposedLeaseId, timeoutSeconds, xmsclientrequestId);

            return new Responses.LeaseBlobResponse(dResp);
        }

        public Responses.LeaseBlobResponse AcquireFixedLease(
            int leaseDuration,
            Guid? proposedLeaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Acquire,
                leaseDuration: leaseDuration,
                proposedLeaseId: proposedLeaseId,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse AcquireInfiniteLease(
                Guid? proposedLeaseId = null,
                int timeoutSeconds = 0,
                Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Acquire,
                leaseDuration: -1,
                proposedLeaseId: proposedLeaseId,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse RenewLease(
            Guid leaseId,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Renew,
                leaseId: leaseId,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse ChangeLease(
            Guid leaseId,
            Guid proposedLeaseId,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Change,
                leaseId: leaseId,
                proposedLeaseId: proposedLeaseId,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse ReleaseLease(
            Guid leaseId,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Release,
                leaseId: leaseId,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse BreakLeaseWithGracePeriod(
            int leaseBreakPeriod,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Break,
                leaseBreakPeriod: leaseBreakPeriod,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse BreakLeaseImmediately(
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseContainer(Enumerations.LeaseOperation.Break,
                leaseBreakPeriod: 0,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }
        #endregion
    }
}
