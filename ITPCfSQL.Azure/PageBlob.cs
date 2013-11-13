using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure
{
    public class PageBlob : Blob
    {
        public PageBlob(Container container, string name)
            : base(container, name)
        {
            BlobType = Enumerations.BlobType.PageBlob;
        }
        public PageBlob(Container container)
            : base(container)
        {
            BlobType = Enumerations.BlobType.PageBlob;
        }

        public string UpdatePage(
            long lStartRange,
            System.IO.Stream inputStream = null, 
            long? lBytesToPut = null,
            string contentMD5 = null,
            Guid? leaseID = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return Internal.InternalMethods.PutPage(
                this.Container.AzureBlobService.AccountName, this.Container.AzureBlobService.SharedKey, this.Container.AzureBlobService.UseHTTPS,
                this.Container.Name,
                this.Name,
                Enumerations.PutPageOperation.Update,
                lStartRange, lBytesToPut,
                inputStream,
                contentMD5,
                leaseID,
                timeoutSeconds,
                xmsclientrequestId);
        }

        public string ClearPage(
            long lStartRange, long lBytesToPut,
            Guid? leaseID = null,
            int timeoutSeconds = 0,
            Guid? xmsclientrequestId = null)
        {
            return Internal.InternalMethods.PutPage(
                this.Container.AzureBlobService.AccountName, this.Container.AzureBlobService.SharedKey, this.Container.AzureBlobService.UseHTTPS,
                this.Container.Name,
                this.Name,
                Enumerations.PutPageOperation.Clear,
                lStartRange, lBytesToPut,
                null,
                null,
                leaseID,
                timeoutSeconds,
                xmsclientrequestId);
        }
    }
}
