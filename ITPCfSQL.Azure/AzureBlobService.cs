using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class AzureBlobService : AzureService
    {
        public AzureBlobService(string AccountName, string SharedKey, bool UseHTTPS)
            :base(AccountName, SharedKey, UseHTTPS)
        {}

        #region Container methods
        public List<Container> ListContainers(
            string prefix = null,
            bool IncludeMetadata = false,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            List<Container> lContainers = new List<Container>();
            string strNextMarker = null;
            do
            {
                string sRet = Internal.InternalMethods.ListContainers(
                    AccountName, SharedKey, UseHTTPS,
                    prefix, strNextMarker,
                    IncludeMetadata: IncludeMetadata, timeoutSeconds: timeoutSeconds, xmsclientrequestId: xmsclientrequestId);

                //Microsoft.SqlServer.Server.SqlContext.Pipe.Send("After Internal.InternalMethods.ListQueues = " + sRet);

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                using (System.IO.StringReader sr = new System.IO.StringReader(sRet))
                {
                    doc.Load(sr);
                }

                lContainers.AddRange(Container.ParseFromXMLEnumerationResults(this, doc));

                strNextMarker = Container.GetNextMarkerFromXMLEnumerationResults(doc);

            } while (!string.IsNullOrEmpty(strNextMarker));

            return lContainers;
        }

        public Container CreateContainer(
            string containerName,
            Enumerations.ContainerPublicReadAccess containerPublicReadAccess = Enumerations.ContainerPublicReadAccess.Off,
            int timeoutSeconds = 0,
            string xmsclientrequestId = null)
        {
            string str = Internal.InternalMethods.CreateContainer(
                AccountName, SharedKey, UseHTTPS,
                containerName, containerPublicReadAccess,
                timeoutSeconds, xmsclientrequestId);

            string strUrl = string.Format("{0:S}/{1:S}",
                Internal.InternalMethods.GetBlobStorageUrl(this.UseHTTPS, this.AccountName),
                containerName);

            return new Container()
            {
                AzureBlobService = this,
                Name = containerName,                
                Url = new Uri(strUrl),
            };
        }

        public Container GetContainer(string containerName)
        {
            string strUrl = string.Format("{0:S}/{1:S}",
                Internal.InternalMethods.GetBlobStorageUrl(this.UseHTTPS, this.AccountName),
                containerName);
                
            return new Container()
            {
                AzureBlobService = this,
                Name = containerName,
                Url = new Uri(strUrl),
            };
        }
        #endregion
    }
}
