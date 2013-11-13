using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class Blob
    {
        #region Constants
        public const string METADATA_PREFIX = "x-ms-meta-";
        #endregion

        #region Members
        protected Dictionary<string, string> _metadata = new Dictionary<string, string>();
        #endregion

        #region Properties
        public Container Container { get; set; }

        public string Name { get; set; }
        public Uri Url { get; set; }
        public string Etag { get; set; }
        public DateTime? LastModified { get; set; }
        public Enumerations.LeaseStatus LeaseStatus { get; set; }
        public Enumerations.LeaseState LeaseState { get; set; }
        public Enumerations.LeaseDuration? LeaseDuration { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentLanguage { get; set; }
        public string ContentMD5 { get; set; }
        public int BlobSequenceNumber { get; set; }
        public Enumerations.BlobType BlobType { get; set; }
        public CopyAttributes CopyAttributes { get; set; }
        public Dictionary<string, string> Metadata { get { return _metadata; } }
        #endregion

        public Blob(Container container)
            : this(container, null)
        { }

        public Blob(Container container, string name)
        {
            this.Container = container;
            this.Name = name;

            if (!string.IsNullOrEmpty(this.Name))
            {
                Url = new Uri(Container.Url.ToString() + "/" + Name);
            }
        }

        public static Blob ParseFromXMLNode(Container container, XmlNode node)
        {
            Enumerations.BlobType blobType = (Enumerations.BlobType)Enum.Parse(
                typeof(Enumerations.BlobType),
                node.SelectSingleNode("Properties/BlobType").InnerText,
                true);

            Blob blob = null;
            switch (blobType)
            {
                case Enumerations.BlobType.BlockBlob:
                    blob = new BlockBlob(container);
                    break;
                case Enumerations.BlobType.PageBlob:
                    blob = new PageBlob(container);
                    break;
                default:
                    throw new ArgumentException("BlobType of type " + blobType.ToString() + " is not supported.");
            }

            blob.Name = node.SelectSingleNode("Name").InnerText;
            blob.Url = new Uri(node.SelectSingleNode("Url").InnerText);
            blob.LeaseState = (Enumerations.LeaseState)Enum.Parse(typeof(Enumerations.LeaseState), node.SelectSingleNode("Properties/LeaseState").InnerText, true);
            blob.LeaseStatus = (Enumerations.LeaseStatus)Enum.Parse(typeof(Enumerations.LeaseStatus), node.SelectSingleNode("Properties/LeaseStatus").InnerText, true);
            blob.ContentLength = long.Parse(node.SelectSingleNode("Properties/Content-Length").InnerText);
            blob.BlobType = blobType;

            System.Xml.XmlNode nMeta = node.SelectSingleNode("Metadata");
            if (nMeta != null)
            {
                foreach (System.Xml.XmlNode nMetaElem in nMeta.ChildNodes)
                {
                    blob.Metadata[nMetaElem.Name] = nMetaElem.InnerText;
                }
            }

            XmlNode n = node.SelectSingleNode("Properties/LeaseDuration");
            if (n != null)
                blob.LeaseDuration = (Enumerations.LeaseDuration)Enum.Parse(typeof(Enumerations.LeaseDuration), n.InnerText, true);

            n = node.SelectSingleNode("Properties/Last-Modified");
            if (n != null)
                blob.LastModified = DateTime.Parse(n.InnerText);

            n = node.SelectSingleNode("Properties/Etag");
            if (n != null)
                blob.Etag = n.InnerText;

            n = node.SelectSingleNode("Properties/Content-Type");
            if (n != null)
                blob.ContentType = n.InnerText;

            n = node.SelectSingleNode("Properties/Content-Encoding");
            if (n != null)
                blob.ContentEncoding = n.InnerText;

            n = node.SelectSingleNode("Properties/Content-Language");
            if (n != null)
                blob.ContentLanguage = n.InnerText;

            n = node.SelectSingleNode("Properties/Content-MD5");
            if (n != null)
                blob.ContentMD5 = n.InnerText;

            n = node.SelectSingleNode("Properties/CopyId");
            if (n != null)
            {
                blob.CopyAttributes = new CopyAttributes()
                {
                    CopyId = Guid.Parse(node.SelectSingleNode("Properties/CopyId").InnerText),
                    CopySource = new Uri(node.SelectSingleNode("Properties/CopySource").InnerText),
                    CopyStatus = node.SelectSingleNode("Properties/CopyStatus").InnerText
                };

                n = node.SelectSingleNode("Properties/CopyCompletionTime");
                if (n != null)
                    blob.CopyAttributes.CopyCompletionTime = DateTime.Parse(n.InnerText);

                n = node.SelectSingleNode("Properties/CopyProgress");
                if (n != null)
                {
                    string[] tokens = n.InnerText.Split(new char[] { '/' });
                    blob.CopyAttributes.CopyCurrentPosition = long.Parse(tokens[0]);
                    blob.CopyAttributes.CopyTotalLength = long.Parse(tokens[1]);
                }
            }

            return blob;
        }

        public static IEnumerable<Blob> ParseFromXMLEnumerationResults(Container container, XmlDocument doc)
        {
            foreach (XmlNode n in doc.SelectNodes("EnumerationResults/Blobs/Blob"))
            {
                yield return ParseFromXMLNode(container, n);
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

        public void Delete(
            Enumerations.BlobDeletionMethod BlobDeletionMethod,
            Guid? leaseID = null,
            DateTime? snapshotDateTimeToDelete = null,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            Internal.InternalMethods.DeleteBlob(
                this.Container.AzureBlobService.AccountName, this.Container.AzureBlobService.SharedKey, this.Container.AzureBlobService.UseHTTPS,
                this.Container.Name, this.Name,
                BlobDeletionMethod, leaseID, snapshotDateTimeToDelete, timeoutSeconds, xmsclientrequestId);
        }

        public Responses.CopyBlobResponse Copy(
            Blob destinationBlob,
            Guid? sourceLeaseID = null,
            Guid? destinationLeaseID = null,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            Dictionary<string, string> res = Internal.InternalMethods.CopyBlob(
                destinationBlob.Container.AzureBlobService.AccountName, destinationBlob.Container.AzureBlobService.SharedKey, destinationBlob.Container.AzureBlobService.UseHTTPS,
                destinationBlob.Container.Name, destinationBlob.Name,
                this.Url,
                sourceLeaseID, destinationLeaseID,
                timeoutSeconds, xmsclientrequestId);

            return new Responses.CopyBlobResponse(res);
        }

        public Responses.GetBlobPropertyResponse GetProperties(
            DateTime? snapshotDateTime = null,
            Guid? leaseID = null,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            Dictionary<string, string> res = Internal.InternalMethods.GetBlobProperty(
                Container.AzureBlobService.AccountName, Container.AzureBlobService.SharedKey, Container.AzureBlobService.UseHTTPS,
                Container.Name, Name,
                leaseID, snapshotDateTime, timeoutSeconds, xmsclientrequestId);

            return new Responses.GetBlobPropertyResponse(res);
        }

        public void SaveMetadata(
            Guid? leaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            Dictionary<string, string> dToUpdate = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in Metadata)
                dToUpdate.Add(METADATA_PREFIX + kvp.Key, kvp.Value);

            Internal.InternalMethods.SetBlobMetadata(
                Container.AzureBlobService.AccountName, Container.AzureBlobService.SharedKey, Container.AzureBlobService.UseHTTPS,
                Container.Name, Name,
                dToUpdate, leaseId, timeoutSeconds, xmsclientrequestId);
        }

        public void DownloadMetadata(
            Guid? leaseId = null,
            DateTime? snapshotDateTime = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            Dictionary<string, string> dRet = Internal.InternalMethods.GetBlobMetadata(
               Container.AzureBlobService.AccountName, Container.AzureBlobService.SharedKey, Container.AzureBlobService.UseHTTPS,
               Container.Name, Name,
               leaseId, snapshotDateTime, timeoutSeconds, xmsclientrequestId);

            Metadata.Clear();
            foreach (KeyValuePair<string, string> kvp in dRet.Where(item => item.Key.StartsWith(METADATA_PREFIX)))
            {
                Metadata[kvp.Key.Substring(METADATA_PREFIX.Length)] = kvp.Value;
            }
        }

        public System.IO.Stream Download(
            Guid? leaseId = null,
            DateTime? snapshotDateTime = null,
            long? lStartPosition = null, long? lLength = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null
            )
        {
            string strRange = string.Empty;
            if (lStartPosition.HasValue)
                strRange = "bytes=" + lStartPosition.Value + "-";
            if (lLength.HasValue)
            {
                if (string.IsNullOrEmpty(strRange))
                    strRange = "bytes=0-";
                strRange += (lStartPosition.Value + lLength.Value).ToString();
            }

            System.IO.Stream s;
            Internal.InternalMethods.GetBlob(
                Container.AzureBlobService.AccountName, Container.AzureBlobService.SharedKey, Container.AzureBlobService.UseHTTPS,
                Container.Name, Name, out s, strRange,
                false, leaseId, snapshotDateTime,
                timeoutSeconds, xmsclientrequestId);

            return s;
        }


        #region Lease
        protected internal Responses.LeaseBlobResponse LeaseBlob(
            Enumerations.LeaseOperation leaseOperation,
            Guid? leaseId = null,
            int? leaseBreakPeriod = null,
            int? leaseDuration = null, // use -1 for infinite
            Guid? proposedLeaseId = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            Dictionary<string, string> dResp = ITPCfSQL.Azure.Internal.InternalMethods.LeaseBlob(
                this.Container.AzureBlobService.AccountName, this.Container.AzureBlobService.SharedKey, this.Container.AzureBlobService.UseHTTPS,
                this.Container.Name,
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
            return LeaseBlob(Enumerations.LeaseOperation.Acquire,
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
            return LeaseBlob(Enumerations.LeaseOperation.Acquire,
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
            return LeaseBlob(Enumerations.LeaseOperation.Renew,
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
            return LeaseBlob(Enumerations.LeaseOperation.Change,
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
            return LeaseBlob(Enumerations.LeaseOperation.Release,
                leaseId: leaseId,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse BreakLeaseWithGracePeriod(
            int leaseBreakPeriod,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseBlob(Enumerations.LeaseOperation.Break,
                leaseBreakPeriod: leaseBreakPeriod,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }

        public Responses.LeaseBlobResponse BreakLeaseImmediately(
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return LeaseBlob(Enumerations.LeaseOperation.Break,
                leaseBreakPeriod: 0,
                timeoutSeconds: timeoutSeconds,
                xmsclientrequestId: xmsclientrequestId);
        }
        #endregion

    }
}
