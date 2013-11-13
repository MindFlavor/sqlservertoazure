using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITPCfSQL.Azure;

namespace ITPCfSQL.Azure.CLR
{
    public class AzureBlob
    {
        #region Full-Blown
        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_ListContainers",
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)")]
        public static System.Collections.IEnumerable ListContainers(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            return abs.ListContainers(null, true, 0,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }

        [SqlProcedure]
        public static void CreateContainer(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString containerPublicReadAccess,
            SqlString xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);

            Enumerations.ContainerPublicReadAccess cpra;

            if (!Enum.TryParse<Enumerations.ContainerPublicReadAccess>(containerPublicReadAccess.Value, out cpra))
            {
                StringBuilder sb = new StringBuilder("\"" + containerPublicReadAccess.Value + "\" is an invalid ContainerPublicReadAccess value. Valid values are: ");
                foreach (string s in Enum.GetNames(typeof(Enumerations.ContainerPublicReadAccess)))
                    sb.Append("\"" + s + "\" ");
                throw new ArgumentException(sb.ToString());
            }

            abs.CreateContainer(containerName.Value,
                cpra,
                xmsclientrequestId: xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }


        [SqlProcedure]
        public static void DeleteContainer(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlGuid leaseId,
            SqlString xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            abs.GetContainer(containerName.Value).Delete(
                leaseID: leaseId.IsNull ? (Guid?)null : leaseId.Value,
                xmsclientrequestId: xmsclientrequestId.IsNull ? null : xmsclientrequestId.Value);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_ListBlobs",
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), BlobSequenceNumber INT, BlobType NVARCHAR(255), ContentEncoding NVARCHAR(255), ContentLanguage NVARCHAR(255), ContentLength BIGINT, ContentMD5 NVARCHAR(255), ContentType NVARCHAR(255), CopyId UNIQUEIDENTIFIER, CopySource NVARCHAR(255), CopyStatus NVARCHAR(255), CopyCurrentPosition BIGINT, CopyTotalLength BIGINT, CopyCompletionTime DATETIME, LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255), Metadata XML")]
        public static System.Collections.IEnumerable ListBlobs(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlBoolean includeSnapshots,
            SqlBoolean includeMetadata,
            SqlBoolean includeCopy,
            SqlBoolean includeUncommittedBlobs,
            SqlString xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            return cont.ListBlobs(null,
                includeSnapshots.Value, includeMetadata.Value, includeCopy.Value, includeUncommittedBlobs.Value, 0,
                xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlBinary DownloadBlockBlob(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            ITPCfSQL.Azure.BlockBlob blob = new ITPCfSQL.Azure.BlockBlob(cont, blobName.Value);

            byte[] bBuffer = new byte[1024];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                int iDownloaded = 0;
                using (System.IO.Stream s = blob.Download(
                    xmsclientrequestId: xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value))
                {
                    while ((iDownloaded = s.Read(bBuffer, 0, bBuffer.Length)) > 0)
                    {
                        ms.Write(bBuffer, 0, iDownloaded);
                    }
                }

                return new SqlBinary(ms.ToArray());
            }
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlBinary DownloadPageBlob(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlInt64 startPosition, SqlInt32 length,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            ITPCfSQL.Azure.PageBlob blob = new ITPCfSQL.Azure.PageBlob(cont, blobName.Value);

            byte[] bBuffer = new byte[1024];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                int iDownloaded = 0;
                using (System.IO.Stream s = blob.Download(
                    leaseId.IsNull ? (Guid?)null : leaseId.Value,
                    (DateTime?)null,
                    startPosition.IsNull ? (long?)null : startPosition.Value,
                    length.IsNull ? (long?)null : length.Value,
                    xmsclientrequestId: xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value))
                {
                    while ((iDownloaded = s.Read(bBuffer, 0, bBuffer.Length)) > 0)
                    {
                        ms.Write(bBuffer, 0, iDownloaded);
                    }
                }

                return new SqlBinary(ms.ToArray());
            }
        }

        [SqlProcedure]
        public static void CreateOrReplaceBlockBlob(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName, SqlString blobName,
            SqlBytes binaryBuffer, SqlString contentType, SqlString contentEncoding,
            SqlString contentMD5,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            cont.CreateBlockBlob(blobName.Value, binaryBuffer.Stream,
                null,
                contentType.IsNull ? "application/octet-stream" : contentType.Value,
                contentEncoding.IsNull ? null : contentEncoding.Value,
                null,
                contentMD5.IsNull ? (null) : contentMD5.Value,
                0,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }

        [SqlProcedure]
        public static void CreateOrReplacePageBlob(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName, SqlString blobName, SqlInt64 maximumSize,
            SqlGuid leaseId,
            SqlString contentType, SqlString contentEncoding, SqlString contentLanguage,
            SqlInt32 timeoutSeconds, SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            cont.CreatePageBlob(blobName.Value, maximumSize.Value,
                leaseId.IsNull ? (Guid?)null : leaseId.Value,
                contentType.IsNull ? "application/octet-stream" : contentType.Value,
                contentEncoding.IsNull ? null : contentEncoding.Value,
                contentLanguage.IsNull ? null : contentLanguage.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }

        [SqlProcedure]
        public static void PutPage(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName, SqlString blobName,
            SqlBytes binaryBuffer,
            SqlInt32 startPositionBytes, SqlInt32 bytesToUpload,
            SqlGuid leaseId, SqlString contentMD5,
            SqlInt32 timeoutSeconds, SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            PageBlob pb = cont.GetPageBlob(blobName.Value);

            if (binaryBuffer.IsNull)
            {
                // clear
                pb.ClearPage(
                     startPositionBytes.Value,
                     bytesToUpload.Value,
                     leaseId.IsNull ? (Guid?)null : leaseId.Value,
                     timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                     xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
            }
            else
            {
                pb.UpdatePage(
                    startPositionBytes.Value,
                    binaryBuffer.Stream,
                    bytesToUpload.IsNull ? (long?)null : bytesToUpload.Value,
                    contentMD5.IsNull ? (null) : contentMD5.Value,
                    leaseId.IsNull ? (Guid?)null : leaseId.Value,
                    timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                    xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
            }
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlString CreateOrReplaceBlockBlob_Function(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName, SqlString blobName,
            SqlBytes binaryBuffer, SqlString contentType, SqlString contentEncoding,
            SqlGuid xmsclientrequestId)
        {
            try
            {
                CreateOrReplaceBlockBlob(
                    accountName, sharedKey, useHTTPS,
                    containerName, blobName,
                    binaryBuffer, contentType, contentEncoding,
                    SqlString.Null,
                    xmsclientrequestId);

                return SqlString.Null;
            }
            catch (Exception exce)
            {
                return exce.ToString();
            }
        }

        [SqlProcedure]
        public static void DeleteBlob(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName, SqlString blobName,
            SqlString blobDeletionMethod,
            SqlGuid leaseID,
            SqlDateTime snapshotDateTimeToDelete,
            SqlString xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            ITPCfSQL.Azure.Blob bb = new Azure.Blob(cont, blobName.Value);

            Enumerations.BlobDeletionMethod bbm;

            if (!Enum.TryParse<Enumerations.BlobDeletionMethod>(blobDeletionMethod.Value, out bbm))
            {
                StringBuilder sb = new StringBuilder("\"" + blobDeletionMethod.Value + "\" is an invalid blobDeletionMethod value. Valid values are: ");
                foreach (string s in Enum.GetNames(typeof(Enumerations.BlobDeletionMethod)))
                    sb.Append("\"" + s + "\" ");
                throw new ArgumentException(sb.ToString());
            }

            bb.Delete(bbm,
                leaseID.IsNull ? (Guid?)null : leaseID.Value,
                snapshotDateTimeToDelete.IsNull ? (DateTime?)null : snapshotDateTimeToDelete.Value,
                0,
                xmsclientrequestId != null ? xmsclientrequestId.Value : null);
        }

        [SqlProcedure]
        public static void CopyBlob(
            SqlString destinationAccount, SqlString destinationSharedKey, SqlBoolean useHTTPS,
            SqlString sourceAccountName,
            SqlString sourceContainerName, SqlString sourceBlobName,
            SqlGuid sourceLeaseId, SqlGuid destinationLeaseId,
            SqlString destinationContainerName, SqlString destinationBlobName,
            SqlString xmsclientrequestId)
        {
            AzureBlobService absDest = new AzureBlobService(destinationAccount.Value, destinationSharedKey.Value, useHTTPS.Value);
            Container contDest = absDest.GetContainer(destinationContainerName.Value);
            ITPCfSQL.Azure.Blob bbDest = new Azure.Blob(contDest, destinationBlobName.Value);

            AzureBlobService absSrc = new AzureBlobService(sourceAccountName.Value, "", useHTTPS.Value);
            Container contSrc = absSrc.GetContainer(sourceContainerName.Value);
            ITPCfSQL.Azure.Blob bbSrc = new Azure.Blob(contSrc, sourceBlobName.Value);

            Responses.CopyBlobResponse resp = bbSrc.Copy(bbDest,
                sourceLeaseID: sourceLeaseId.IsNull ? (Guid?)null : sourceLeaseId.Value,
                destinationLeaseID: destinationLeaseId.IsNull ? (Guid?)null : destinationLeaseId.Value,
                xmsclientrequestId: xmsclientrequestId.IsNull ? null : xmsclientrequestId.Value);

            SqlDataRecord record = new SqlDataRecord(
                new SqlMetaData[]
                {
                    new SqlMetaData("BlobCopyStatus", System.Data.SqlDbType.NVarChar, 255),
                    new SqlMetaData("CopyId", System.Data.SqlDbType.NVarChar, 255), 
                    new SqlMetaData("Date", System.Data.SqlDbType.DateTime), 
                    new SqlMetaData("ETag", System.Data.SqlDbType.NVarChar, 255), 
                    new SqlMetaData("LastModified", System.Data.SqlDbType.DateTime), 
                    new SqlMetaData("RequestID", System.Data.SqlDbType.UniqueIdentifier), 
                    new SqlMetaData("Version", System.Data.SqlDbType.NVarChar, 255)
                });

            SqlContext.Pipe.SendResultsStart(record);

            record.SetString(0, resp.BlobCopyStatus.ToString());
            record.SetString(1, resp.CopyId);
            record.SetDateTime(2, resp.Date);
            record.SetString(3, resp.ETag);
            record.SetDateTime(4, resp.LastModified);
            record.SetGuid(5, resp.RequestID);
            record.SetString(6, resp.Version);

            SqlContext.Pipe.SendResultsRow(record);
            SqlContext.Pipe.SendResultsEnd();
        }

        [SqlFunction(
        DataAccess = DataAccessKind.None,
        FillRowMethodName = "_ListBlobProperties",
        IsDeterministic = false,
        IsPrecise = true,
        SystemDataAccess = SystemDataAccessKind.None,
        TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), BlobSequenceNumber INT, BlobType NVARCHAR(255), ContentEncoding NVARCHAR(255), ContentLanguage NVARCHAR(255), ContentLength BIGINT, ContentMD5 NVARCHAR(255), ContentType NVARCHAR(255), CopyId UNIQUEIDENTIFIER, CopySource NVARCHAR(255), CopyStatus NVARCHAR(255), CopyCurrentPosition BIGINT, CopyTotalLength BIGINT, CopyCompletionTime DATETIME, LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)")]
        public static System.Collections.IEnumerable GetBlobProperties(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName, SqlString blobName,
            SqlDateTime snapshotDateTime,
            SqlString xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            List<Responses.GetBlobPropertyResponse> lResps = new List<Responses.GetBlobPropertyResponse>();

            lResps.Add(cont.GetBlob(blobName.Value).GetProperties(
                snapshotDateTime: snapshotDateTime.IsNull ? (DateTime?)null : snapshotDateTime.Value));

            return lResps;
        }


        [SqlProcedure]
        public static void UpdateBlobMetadata(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlXml AttributeList,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(AttributeList.Value);


            if (doc.FirstChild.ChildNodes.Count == 1)
            {
                // single row, multiple attribs...  FOR XML RAW?
                foreach (System.Xml.XmlAttribute attrib in doc.FirstChild.FirstChild.Attributes)
                {
                    blob.Metadata[attrib.Name] = attrib.Value;
                }
            }
            else
            {
                foreach (System.Xml.XmlNode nAttrib in doc.FirstChild.ChildNodes)
                {
                    if (!(string.IsNullOrEmpty(nAttrib.InnerText)))
                        blob.Metadata[nAttrib.Name] = nAttrib.InnerText;
                    else if (((nAttrib.Attributes != null) && (nAttrib.Attributes.Count > 0)))
                    {
                        blob.Metadata[nAttrib.Name] = nAttrib.Attributes[0].Value;
                    }
                }
            }

            blob.SaveMetadata(
                leaseId.IsNull ? (Guid?)null : leaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);
        }
        #endregion

        #region Embedded
        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_ListContainers",
            IsDeterministic = true,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)")]
        public static System.Collections.IEnumerable ListContainers_Embedded(
            SqlString logicalConnectionName,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);
            return ListContainers(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void CreateContainer_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString containerPublicReadAccess,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            CreateContainer(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, containerPublicReadAccess, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void DeleteContainer_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlGuid leaseId,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            DeleteContainer(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, leaseId, xmsclientrequestId);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            FillRowMethodName = "_ListBlobs",
            IsDeterministic = true,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None,
            TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), BlobSequenceNumber INT, BlobType NVARCHAR(255), ContentEncoding NVARCHAR(255), ContentLanguage NVARCHAR(255), ContentLength BIGINT, ContentMD5 NVARCHAR(255), ContentType NVARCHAR(255), CopyId UNIQUEIDENTIFIER, CopySource NVARCHAR(255), CopyStatus NVARCHAR(255), CopyCurrentPosition BIGINT, CopyTotalLength BIGINT, CopyCompletionTime DATETIME, LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)")]
        public static System.Collections.IEnumerable ListBlobs_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlBoolean includeSnapshots,
            SqlBoolean includeMetadata,
            SqlBoolean includeCopy,
            SqlBoolean includeUncommittedBlobs,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return ListBlobs(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName,
                includeSnapshots, includeMetadata, includeCopy, includeUncommittedBlobs,
                xmsclientrequestId);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            IsDeterministic = true,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlBinary DownloadBlockBlob_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return DownloadBlockBlob(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                xmsclientrequestId);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlBinary DownloadPageBlob_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlInt64 startPosition, SqlInt32 length,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return DownloadPageBlob(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                leaseId, startPosition, length,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void CreateOrReplaceBlockBlob_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName, SqlString blobName,
            SqlBytes binaryBuffer, SqlString contentType, SqlString contentEncoding,
            SqlString contentMD5,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            CreateOrReplaceBlockBlob(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName, binaryBuffer, contentType, contentEncoding,
                contentMD5,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void PutPage_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName, SqlString blobName,
            SqlBytes binaryBuffer,
            SqlInt32 startPositionBytes, SqlInt32 bytesToUpload,
            SqlGuid leaseId, SqlString contentMD5,
            SqlInt32 timeoutSeconds, SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            PutPage(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                binaryBuffer, startPositionBytes, bytesToUpload, leaseId,
                contentMD5, timeoutSeconds,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void CreateOrReplacePageBlob_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName, SqlString blobName, SqlInt64 maximumSize,
            SqlGuid leaseId,
            SqlString contentType, SqlString contentEncoding, SqlString contentLanguage,
            SqlInt32 timeoutSeconds, SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            CreateOrReplacePageBlob(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName, maximumSize, leaseId,
                contentType, contentEncoding, contentLanguage,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlFunction(
            DataAccess = DataAccessKind.None,
            IsDeterministic = false,
            IsPrecise = true,
            SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlString CreateOrReplaceBlockBlob_Function_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName, SqlString blobName,
            SqlBytes binaryBuffer, SqlString contentType, SqlString contentEncoding,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return CreateOrReplaceBlockBlob_Function(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                binaryBuffer, contentType, contentEncoding,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void DeleteBlob_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName, SqlString blobName,
            SqlString blobDeletionMethod,
            SqlGuid leaseID,
            SqlDateTime snapshotDateTimeToDelete,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            DeleteBlob(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                blobDeletionMethod, leaseID, snapshotDateTimeToDelete,
                xmsclientrequestId);
        }

        [SqlProcedure]
        public static void CopyBlob_Embedded(
            SqlString destinationLogicalConnectionName,
            SqlString sourceAccountName,
            SqlString sourceContainerName, SqlString sourceBlobName,
            SqlGuid sourceLeaseId, SqlGuid destinationLeaseId,
            SqlString destinationContainerName, SqlString destinationBlobName,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(destinationLogicalConnectionName.Value);

            CopyBlob(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                sourceAccountName, sourceContainerName, sourceBlobName, sourceLeaseId,
                destinationLeaseId, destinationContainerName, destinationBlobName,
                xmsclientrequestId);
        }

        [SqlFunction(
        DataAccess = DataAccessKind.None,
        FillRowMethodName = "_ListBlobProperties",
        IsDeterministic = true,
        IsPrecise = true,
        SystemDataAccess = SystemDataAccessKind.None,
        TableDefinition = "Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), BlobSequenceNumber INT, BlobType NVARCHAR(255), ContentEncoding NVARCHAR(255), ContentLanguage NVARCHAR(255), ContentLength BIGINT, ContentMD5 NVARCHAR(255), ContentType NVARCHAR(255), CopyId UNIQUEIDENTIFIER, CopySource NVARCHAR(255), CopyStatus NVARCHAR(255), CopyCurrentPosition BIGINT, CopyTotalLength BIGINT, CopyCompletionTime DATETIME, LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)")]
        public static System.Collections.IEnumerable GetBlobProperties_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName, SqlString blobName,
            SqlDateTime snapshotDateTime,
            SqlString xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            return GetBlobProperties(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                snapshotDateTime, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void UpdateBlobMetadata_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlXml AttributeList,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            UpdateBlobMetadata(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                AttributeList,
                leaseId, timeoutSeconds, xmsclientrequestId);
        }
        #endregion

        #region Callbacks
        public static void _ListContainers(Object obj,
            out SqlString name,
            out SqlString url,
            out SqlString ETag,
            out SqlDateTime lastModified,
            out SqlString leaseDuration,
            out SqlString leaseState,
            out SqlString leaseStatus)
        {
            if (!(obj is Container))
                throw new ArgumentException("Expected " + typeof(Container).ToString() + ", received " + obj.GetType().FullName);

            Container container = (Container)obj;

            name = container.Name;
            url = container.Url != null ? container.Url.ToString() : null;
            ETag = container.Etag;
            lastModified = container.LastModified;

            if (container.LeaseDuration.HasValue)
                leaseDuration = container.LeaseDuration.Value.ToString();
            else
                leaseDuration = null;

            leaseState = container.LeaseState.ToString();
            leaseStatus = container.LeaseStatus.ToString();
        }

        public static void _ListBlobs(Object obj,
           out SqlString name,
           out SqlString url,
           out SqlString ETag,
           out SqlInt32 BlobSequenceNumber,
           out SqlString blobType,
           out SqlString ContentEncoding,
           out SqlString ContentLanguage,
           out SqlInt64 ContentLength,
           out SqlString ContentMD5,
           out SqlString ContentType,
           out SqlGuid CopyId,
           out SqlString CopySource,
           out SqlString CopyStatus,
           out SqlInt64 CopyCurrentPosition,
           out SqlInt64 CopyTotalLength,
           out SqlDateTime CopyCompletionTime,
           out SqlDateTime LastModified,
           out SqlString LeaseDuration,
           out SqlString LeaseState,
           out SqlString LeaseStatus,
           out SqlXml Metadata)
        {
            if (!(obj is Blob))
                throw new ArgumentException("Expected " + typeof(Blob).ToString() + ", received " + obj.GetType().FullName);

            #region Null init
            name = SqlString.Null;
            url = SqlString.Null;
            ETag = SqlString.Null;
            BlobSequenceNumber = SqlInt32.Null;
            blobType = SqlString.Null;
            ContentEncoding = SqlString.Null;
            ContentLanguage = SqlString.Null;
            ContentLength = SqlInt32.Null;
            ContentMD5 = SqlString.Null;
            ContentType = SqlString.Null;
            CopyId = SqlGuid.Null;
            CopySource = SqlString.Null;
            CopyStatus = SqlString.Null;
            CopyCurrentPosition = SqlInt64.Null;
            CopyTotalLength = SqlInt64.Null;
            CopyCompletionTime = SqlDateTime.Null;
            LastModified = SqlDateTime.Null;
            LeaseDuration = SqlString.Null;
            LeaseState = SqlString.Null;
            LeaseStatus = SqlString.Null;
            Metadata = SqlXml.Null;
            #endregion

            Blob blob = (Blob)obj;

            name = blob.Name;
            url = blob.Url.ToString();
            ETag = blob.Etag;

            BlobSequenceNumber = blob.BlobSequenceNumber;
            blobType = blob.BlobType.ToString();
            ContentEncoding = blob.ContentEncoding;
            ContentLanguage = blob.ContentLanguage;

            ContentLength = blob.ContentLength;
            ContentMD5 = blob.ContentMD5;
            ContentType = blob.ContentType;
            if (blob.CopyAttributes != null)
            {
                CopyId = blob.CopyAttributes.CopyId;
                CopySource = blob.CopyAttributes.CopyStatus != null ? blob.CopyAttributes.CopySource.ToString() : SqlString.Null;
                CopyStatus = blob.CopyAttributes.CopyStatus;
                CopyCurrentPosition = blob.CopyAttributes.CopyCurrentPosition;
                CopyTotalLength = blob.CopyAttributes.CopyTotalLength;
                if (blob.CopyAttributes.CopyCompletionTime.HasValue)
                    CopyCompletionTime = blob.CopyAttributes.CopyCompletionTime.Value;
                else
                    CopyCompletionTime = SqlDateTime.Null;
            }

            if (blob.LastModified.HasValue)
                LastModified = blob.LastModified.Value;
            else
                LastModified = SqlDateTime.Null;

            if (blob.LeaseDuration.HasValue)
                LeaseDuration = blob.LeaseDuration.Value.ToString();
            else
                LeaseDuration = SqlString.Null;

            LeaseState = blob.LeaseState.ToString();

            LeaseStatus = blob.LeaseStatus.ToString();

            #region Metadata output
            if ((blob.Metadata != null) && (blob.Metadata.Count > 0))
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                using (System.Xml.XmlWriter wr = System.Xml.XmlWriter.Create(ms))
                {
                    wr.WriteStartElement("MetadataList");

                    foreach (string s in blob.Metadata.Keys)
                    {
                        wr.WriteStartElement(s);
                        wr.WriteString(blob.Metadata[s]);
                        wr.WriteEndElement();
                    }

                    wr.WriteEndElement();

                    wr.Flush();
                    wr.Close();
                }

                ms.Seek(0, System.IO.SeekOrigin.Begin);
                Metadata = new SqlXml(ms);
            }
            #endregion
        }

        public static void _ListBlobProperties(Object obj,
           out SqlString ETag,
           out SqlInt64 BlobSequenceNumber,
           out SqlString blobType,
           out SqlString ContentEncoding,
           out SqlString ContentLanguage,
           out SqlInt64 ContentLength,
           out SqlString ContentMD5,
           out SqlString ContentType,
           out SqlGuid CopyId,
           out SqlString CopySource,
           out SqlString CopyStatus,
           out SqlString CopyStatusDescription,
           out SqlInt64 CopyCurrentPosition,
           out SqlInt64 CopyTotalLength,
           out SqlDateTime CopyCompletionTime,
           out SqlDateTime LastModified,
           out SqlString LeaseDuration,
           out SqlString LeaseState,
           out SqlString LeaseStatus,
           out SqlString CacheControl,
           out SqlDateTime Date,
           out SqlGuid RequestId,
           out SqlString Version
               )
        {
            if (!(obj is Responses.GetBlobPropertyResponse))
                throw new ArgumentException("Expected " + typeof(Responses.GetBlobPropertyResponse).ToString() + ", received " + obj.GetType().FullName);

            #region Null init
            ETag = SqlString.Null;
            BlobSequenceNumber = SqlInt32.Null;
            blobType = SqlString.Null;
            ContentEncoding = SqlString.Null;
            ContentLanguage = SqlString.Null;
            ContentLength = SqlInt32.Null;
            ContentMD5 = SqlString.Null;
            ContentType = SqlString.Null;
            CopyId = SqlGuid.Null;
            CopySource = SqlString.Null;
            CopyStatus = SqlString.Null;
            CopyCurrentPosition = SqlInt64.Null;
            CopyTotalLength = SqlInt64.Null;
            CopyCompletionTime = SqlDateTime.Null;
            LastModified = SqlDateTime.Null;
            LeaseDuration = SqlString.Null;
            LeaseState = SqlString.Null;
            LeaseStatus = SqlString.Null;
            #endregion

            Responses.GetBlobPropertyResponse resp = (Responses.GetBlobPropertyResponse)obj;

            ETag = resp.ETag;

            if (resp.BlobSequenceNumber.HasValue)
                BlobSequenceNumber = resp.BlobSequenceNumber.Value;
            blobType = resp.BlobType.ToString();
            ContentEncoding = resp.ContentEncoding;
            ContentLanguage = resp.ContentLanguage;

            ContentLength = resp.ContentLength;
            ContentMD5 = resp.ContentMD5;
            ContentType = resp.ContentType;

            if (!string.IsNullOrEmpty(resp.CopyId))
                CopyId = Guid.Parse(resp.CopyId);

            if (resp.BlobCopyStatus.HasValue)
                CopyStatus = resp.BlobCopyStatus.ToString();

            CopySource = resp.CopySource;

            if (resp.BytesCopied.HasValue)
                CopyCurrentPosition = resp.BytesCopied.Value;
            if (resp.BytesTotal.HasValue)
                CopyTotalLength = resp.BytesTotal.Value;

            CacheControl = resp.CacheControl;
            ContentEncoding = resp.ContentEncoding;
            ContentLanguage = resp.ContentLanguage;
            ContentLength = resp.ContentLength;
            ContentMD5 = resp.ContentMD5;

            if (resp.CopyCompletionTime.HasValue)
                CopyCompletionTime = resp.CopyCompletionTime.Value;
            else
                CopyCompletionTime = SqlDateTime.Null;

            CopyStatusDescription = resp.CopyStatusDescription;

            Date = resp.Date;

            LastModified = resp.LastModified;

            if (resp.LeaseDuration.HasValue)
                LeaseDuration = resp.LeaseDuration.Value.ToString();

            if (resp.LeaseState.HasValue)
                LeaseState = resp.LeaseState.Value.ToString();
            if (resp.LeaseStatus.HasValue)
                LeaseStatus = resp.LeaseStatus.Value.ToString();

            RequestId = resp.RequestID;
            Version = resp.Version;
        }
        #endregion
    }
}
