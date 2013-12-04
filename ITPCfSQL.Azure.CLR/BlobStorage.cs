using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using ITPCfSQL.Azure;

namespace ITPCfSQL.Azure.CLR
{
    public class BlobStorage
    {
        #region Utils
        public static void PushLeaseBlobResponse(Responses.LeaseBlobResponse lbr)
        {
            SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { 
                new SqlMetaData("LeaseId", System.Data.SqlDbType.UniqueIdentifier), 
                new SqlMetaData("Date", System.Data.SqlDbType.DateTime),
                new SqlMetaData("LeaseBreakTimeSeconds", System.Data.SqlDbType.Int),
                new SqlMetaData("RequestId", System.Data.SqlDbType.UniqueIdentifier), 
                new SqlMetaData("Version", System.Data.SqlDbType.NVarChar, 4000)
                });

            if (lbr.LeaseId.HasValue)
                record.SetGuid(0, lbr.LeaseId.Value);
            record.SetDateTime(1, lbr.Date);

            if (lbr.LeaseTimeSeconds.HasValue)
                record.SetInt32(2, lbr.LeaseTimeSeconds.Value);
            record.SetGuid(3, lbr.RequestID);
            record.SetString(4, lbr.Version);

            SqlContext.Pipe.SendResultsStart(record);
            SqlContext.Pipe.SendResultsRow(record);
            SqlContext.Pipe.SendResultsEnd();
        }
        #endregion

        #region Blob
        #region Full-Blown
        [SqlProcedure]
        public static void AcquireBlobFixedLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlInt32 leaseDuration,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.AcquireFixedLease(
                leaseDuration.Value,
                proposedLeaseId.IsNull ? (Guid?)null : proposedLeaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void AcquireBlobInfiniteLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.AcquireInfiniteLease(
                proposedLeaseId.IsNull ? (Guid?)null : proposedLeaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void RenewBlobLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.RenewLease(
                leaseId.IsNull ? Guid.Empty : leaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void ChangeBlobLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.ChangeLease(
                leaseId.IsNull ? Guid.Empty : leaseId.Value,
                proposedLeaseId.IsNull ? Guid.Empty : proposedLeaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void ReleaseBlobLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.ReleaseLease(
                leaseId.IsNull ? Guid.Empty : leaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void BreakBlobLeaseWithGracePeriod(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlInt32 leaseBreakPeriod,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.BreakLeaseWithGracePeriod(
                leaseBreakPeriod.IsNull ? 0 : leaseBreakPeriod.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void BreakBlobLeaseImmediately(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlString blobName,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);
            Blob blob = cont.GetBlob(blobName.Value);

            Responses.LeaseBlobResponse lbr = blob.BreakLeaseImmediately(
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }
        #endregion

        #region Embedded
        [SqlProcedure]
        public static void AcquireBlobFixedLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlInt32 leaseDuration,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            AcquireBlobFixedLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                leaseDuration, proposedLeaseId, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void AcquireBlobInfiniteLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            AcquireBlobInfiniteLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                proposedLeaseId, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void RenewBlobLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            RenewBlobLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                leaseId,                 
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void ChangeBlobLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            ChangeBlobLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                leaseId, proposedLeaseId,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void ReleaseBlobLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            ReleaseBlobLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                leaseId, 
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void BreakBlobLeaseWithGracePeriod_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlInt32 leaseBreakPeriod,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            BreakBlobLeaseWithGracePeriod(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                leaseBreakPeriod,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void BreakBlobLeaseImmediately_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlString blobName,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            BreakBlobLeaseImmediately(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, blobName,
                timeoutSeconds, xmsclientrequestId);
        }
        #endregion
        #endregion

        #region Container
        #region Full-Blown
        [SqlProcedure]
        public static void AcquireContainerFixedLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlInt32 leaseDuration,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.AcquireFixedLease(
                leaseDuration.Value,
                proposedLeaseId.IsNull ? (Guid?)null : proposedLeaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void AcquireContainerInfiniteLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.AcquireInfiniteLease(
                proposedLeaseId.IsNull ? (Guid?)null : proposedLeaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void RenewContainerLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.RenewLease(
                leaseId.IsNull ? Guid.Empty : leaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void ChangeContainerLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlGuid leaseId,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.ChangeLease(
                leaseId.IsNull ? Guid.Empty : leaseId.Value,
                proposedLeaseId.IsNull ? Guid.Empty : proposedLeaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void ReleaseContainerLease(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.ReleaseLease(
                leaseId.IsNull ? Guid.Empty : leaseId.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void BreakContainerLeaseWithGracePeriod(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlInt32 leaseBreakPeriod,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.BreakLeaseWithGracePeriod(
                leaseBreakPeriod.IsNull ? 0 : leaseBreakPeriod.Value,
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }

        [SqlProcedure]
        public static void BreakContainerLeaseImmediately(
            SqlString accountName, SqlString sharedKey, SqlBoolean useHTTPS,
            SqlString containerName,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            AzureBlobService abs = new AzureBlobService(accountName.Value, sharedKey.Value, useHTTPS.Value);
            Container cont = abs.GetContainer(containerName.Value);

            Responses.LeaseBlobResponse lbr = cont.BreakLeaseImmediately(
                timeoutSeconds.IsNull ? 0 : timeoutSeconds.Value,
                xmsclientrequestId.IsNull ? (Guid?)null : xmsclientrequestId.Value);

            PushLeaseBlobResponse(lbr);
        }
        #endregion

        #region Embedded
        [SqlProcedure]
        public static void AcquireContainerFixedLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlInt32 leaseDuration,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            AcquireContainerFixedLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                leaseDuration, proposedLeaseId, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void AcquireContainerInfiniteLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            AcquireContainerInfiniteLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                proposedLeaseId, timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void RenewContainerLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            RenewContainerLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                leaseId,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void ChangeContainerLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlGuid leaseId,
            SqlGuid proposedLeaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            ChangeContainerLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                leaseId, proposedLeaseId,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void ReleaseContainerLease_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlGuid leaseId,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            ReleaseContainerLease(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                leaseId,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void BreakContainerLeaseWithGracePeriod_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlInt32 leaseBreakPeriod,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            BreakContainerLeaseWithGracePeriod(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                leaseBreakPeriod,
                timeoutSeconds, xmsclientrequestId);
        }

        [SqlProcedure]
        public static void BreakContainerLeaseImmediately_Embedded(
            SqlString logicalConnectionName,
            SqlString containerName,
            SqlInt32 timeoutSeconds,
            SqlGuid xmsclientrequestId)
        {
            Configuration.EmbeddedConfiguration config = Configuration.EmbeddedConfiguration.GetConfigurationFromEmbeddedFile(logicalConnectionName.Value);

            BreakContainerLeaseImmediately(
                config.AccountName, config.SharedKey, config.UseHTTPS,
                containerName, 
                timeoutSeconds, xmsclientrequestId);
        }
        #endregion

        #region Callbacks

        #endregion
        #endregion
    }
}
