-- *********************************************************************************
-- In SQL Server Management studio open this file and press CTRL+Shift+M to ********
-- configure it BEFORE executing it.										********
-- *********************************************************************************

USE [master];
GO

CREATE ASYMMETRIC KEY [AzureKey] FROM EXECUTABLE FILE = 'C:\GIT\SQLServerToAzure\ITPCfSQL.Azure.CLR\bin\debug\ITPCfSQL.Azure.CLR.dll'
--CREATE ASYMMETRIC KEY [AzureKey] FROM EXECUTABLE FILE = '<ITPCfSQL.Azure.CLR.dll path, nvarchar(4000), DLLPath>'

CREATE LOGIN [AzureLogin] FROM ASYMMETRIC KEY [AzureKey];

GRANT EXTERNAL ACCESS ASSEMBLY TO [AzureLogin];
GO

CREATE DATABASE DemoAzureCLR;
GO
ALTER DATABASE [DemoAzureCLR] SET RECOVERY SIMPLE;
GO
USE [DemoAzureCLR];
GO
EXEC sp_changedbowner 'sa';
GO
CREATE SCHEMA [Azure];
GO
CREATE SCHEMA [Azure.Embedded];
GO
CREATE SCHEMA [Azure.Management];
GO
CREATE SCHEMA [Streaming]
GO

CREATE ASSEMBLY [ITPCfSQL.Azure.CLR] FROM 'C:\GIT\SQLServerToAzure\ITPCfSQL.Azure.CLR\bin\debug\ITPCfSQL.Azure.CLR.dll'
--CREATE ASSEMBLY [ITPCfSQL.Azure.CLR] FROM '<ITPCfSQL.Azure.CLR.dll path, nvarchar(4000), DLLPath>'

WITH PERMISSION_SET=EXTERNAL_ACCESS;
GO

-------
-- Std
-------
-- Blob
CREATE FUNCTION Azure.ListContainers(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].ListContainers;
GO

CREATE PROCEDURE Azure.CreateContainer(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, @ContainerName NVARCHAR(4000), @ContainerPublicReadAccess NVARCHAR(255), @xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateContainer;
GO

CREATE PROCEDURE Azure.DeleteContainer(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, @ContainerName NVARCHAR(4000), @leaseId UNIQUEIDENTIFIER = NULL, @xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DeleteContainer;
GO

CREATE FUNCTION Azure.GetContainerACL(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit,
	@ContainerName NVARCHAR(255),
	@LeaseId UNIQUEIDENTIFIER = NULL,
	@TimeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS TABLE
	(
		Id NVARCHAR(64),
        Start DATETIME,
        Expiry DATETIME,
        Permission NVARCHAR(255),
        ContainerPublicReadAccess NVARCHAR(255)) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].GetContainerACL;
GO

CREATE PROCEDURE Azure.ChangeContainerPublicAccessMethod(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@ContainerPublicReadAccess NVARCHAR(255),
    @LeaseId UNIQUEIDENTIFIER = NULL,
    @timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].ChangeContainerPublicAccessMethod;
GO

CREATE PROCEDURE Azure.AddContainerACL(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@ContainerPublicReadAccess NVARCHAR(255),
	@accessPolicyId NVARCHAR(64),
    @start DATETIME, 
	@expiry DATETIME,
    @canRead BIT = 0,
    @canWrite BIT = 0,
    @canDeleteBlobs BIT = 0,
    @canListBlobs BIT = 0,
    @LeaseId UNIQUEIDENTIFIER = NULL,
    @timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].AddContainerACL;
GO

CREATE PROCEDURE Azure.RemoveContainerACL(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@accessPolicyId NVARCHAR(64),
    @LeaseId UNIQUEIDENTIFIER = NULL,
    @timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].RemoveContainerACL;
GO

CREATE FUNCTION Azure.ListBlobs(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit,
	@container NVARCHAR(4000), 
	@includeSnapshots BIT,
    @includeMetadata BIT,
    @includeCopy BIT,
    @includeUncommittedBlobs BIT,
	@xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), 
           BlobSequenceNumber INT,
           BlobType NVARCHAR(255),
           ContentEncoding NVARCHAR(255),
           ContentLanguage NVARCHAR(255),
           ContentLength BIGINT,
           ContentMD5 NVARCHAR(255),
           ContentType NVARCHAR(255),
           CopyId UNIQUEIDENTIFIER,
           CopySource NVARCHAR(255),
           CopyStatus NVARCHAR(255),
           CopyCurrentPosition BIGINT,
           CopyTotalLength BIGINT,
           CopyCompletionTime DATETIME,
           LastModified DATETIME,
           LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255),
		   Metadata XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].ListBlobs;
GO

CREATE FUNCTION Azure.DownloadBlockBlob(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit,
	@container NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DownloadBlockBlob;
GO

CREATE FUNCTION Azure.DownloadPageBlob(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit,
	@container NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER = NULL,
	@startPosition BIGINT = NULL, @length INT = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DownloadPageBlob;
GO


CREATE PROCEDURE Azure.CreateOrReplaceBlockBlob(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@contentType NVARCHAR(255) = 'application/octect-stream', @contentEncoding NVARCHAR(255) = NULL,
	@contentMD5 NVARCHAR(255) = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateOrReplaceBlockBlob;
GO

CREATE PROCEDURE Azure.PutPage(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@startPositionBytes BIGINT, @bytesToUpload INT = NULL,
	@leaseId UNIQUEIDENTIFIER = NULL,
	@contentMD5 NVARCHAR(255) = NULL,
	@timeoutSeconds INT = 0, @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].PutPage;
GO

CREATE FUNCTION Azure.PutPageFunction(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@startPositionBytes BIGINT, @bytesToUpload INT = NULL,
	@leaseId UNIQUEIDENTIFIER = NULL,
	@contentMD5 NVARCHAR(255) = NULL,
	@timeoutSeconds INT = 0, @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
	RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].PutPage_Function;
GO

CREATE FUNCTION Azure.CreateOrReplaceBlockBlobFunction(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@contentType NVARCHAR(255) = 'application/octet-stream', @contentEncoding NVARCHAR(255) = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
	RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateOrReplaceBlockBlob_Function;
GO

CREATE PROCEDURE Azure.DeleteBlob(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@blobDeletionMethod NVARCHAR(255),
    @leaseID UNIQUEIDENTIFIER = NULL,
    @snapshotDateTimeToDelete DATETIME = NULL,
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DeleteBlob;
GO

CREATE PROCEDURE Azure.CopyBlob(@destinationAccount NVARCHAR(255), @destinationSharedKey NVARCHAR(255), @useHTTPS bit, 
	@sourceAccountName NVARCHAR(255),
	@sourceContainerName NVARCHAR(4000), @sourceBlobName NVARCHAR(4000), 
    @sourceLeaseId UNIQUEIDENTIFIER = NULL, @destinationLeaseId UNIQUEIDENTIFIER = NULL,
    @destinationContainerName NVARCHAR(4000), @destinationBlobName NVARCHAR(4000), 
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CopyBlob;
GO

CREATE FUNCTION Azure.GetBlobProperties(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit,
	@container NVARCHAR(4000), 	@blob NVARCHAR(4000), 
	@snapshotDateTime DATETIME = NULL, 
	@xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(
		   ETag NVARCHAR(255), 
           BlobSequenceNumber BIGINT,
           BlobType NVARCHAR(255),
           ContentEncoding NVARCHAR(255),
           ContentLanguage NVARCHAR(255),
           ContentLength BIGINT,
           ContentMD5 NVARCHAR(255),
           ContentType NVARCHAR(255),
           CopyId UNIQUEIDENTIFIER,
           CopySource NVARCHAR(255),
           CopyStatus NVARCHAR(255),
           CopyStatusDescription NVARCHAR(4000),
           CopyCurrentPosition BIGINT,
           CopyTotalLength BIGINT,
           CopyCompletionTime DATETIME,
           LastModified DATETIME,
           LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255),
		   CacheControl NVARCHAR(255),
		   [Date] DATETIME,
		   RequestId UNIQUEIDENTIFIER,
		   [Version] NVARCHAR(255)
		   ) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].GetBlobProperties;
GO

CREATE PROCEDURE [Azure].UpdateBlobMetadata(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
	@container NVARCHAR(4000), 	@blob NVARCHAR(4000), 
	@attributeList XML,
	@leaseId UNIQUEIDENTIFIER = NULL,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].UpdateBlobMetadata;
GO

CREATE PROCEDURE Azure.CreateOrReplacePageBlob(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@maximumSize BIGINT, @leaseId UNIQUEIDENTIFIER = NULL,
	@contentType NVARCHAR(255) = 'application/octet-stream', @contentEncoding NVARCHAR(255) = NULL,
	@contentLanguage NVARCHAR(255) = NULL,
	@timeout INT = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateOrReplacePageBlob;
GO

-- Queue
CREATE PROCEDURE [Azure].CreateQueue(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
    @queueName NVARCHAR(255),
    @timeoutSeconds INT = 0,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].CreateQueue;
GO

CREATE FUNCTION [Azure].ListQueues(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
	@prefix NVARCHAR(255) = NULL,
    @includeMetadata BIT = 0,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].ListQueues;
GO

CREATE FUNCTION [Azure].ListQueues_Faulty(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
	@prefix NVARCHAR(255) = NULL,
    @includeMetadata BIT = 0,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId NVARCHAR(255) = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].ListQueues_Faulty;
GO

CREATE PROCEDURE [Azure].EnqueueMessage(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
	@queueName NVARCHAR(255),
    @xmlMessage XML,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].EnqueueMessage;
GO

CREATE PROCEDURE [Azure].DequeueMessage(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
	@queueName NVARCHAR(255),
    @visibilityTimeoutSeconds INT = 10, -- lower time might recover from failure faster.
    @timeoutSeconds INT = 60,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].DequeueMessage;
GO

CREATE PROCEDURE [Azure].RetrieveApproximateMessageCount(
	@accountName NVARCHAR(255), @sharedKey NVARCHAR(255), @useHTTPS BIT = 1,
	@queueName NVARCHAR(255),
    @timeoutSeconds INT = 60,
    @xmsclientrequestId NVARCHAR(255) = NEWID)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].RetrieveApproximateMessageCount;
GO

-- Table
CREATE PROCEDURE Azure.CreateTable(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, @TableName NVARCHAR(4000), @xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].CreateTable;
GO

CREATE PROCEDURE Azure.DropTable(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, @TableName NVARCHAR(4000), @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].DropTable;
GO

CREATE FUNCTION Azure.ListTables(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, @xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(MAX)) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].ListTables;
GO

CREATE FUNCTION Azure.QueryTable(@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, @tableName NVARCHAR(4000), @xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(PartitionKey NVARCHAR(4000), RowKey NVARCHAR(4000), [TimeStamp] DATETIME, Attributes XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].QueryTable;
GO

CREATE PROCEDURE Azure.DeleteEntity(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@TableName NVARCHAR(4000), 
	@PartitionKey NVARCHAR(4000), @RowKey NVARCHAR(4000),
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].DeleteEntity;
GO

CREATE PROCEDURE Azure.InsertOrReplaceEntity(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@TableName NVARCHAR(4000), 
	@PartitionKey NVARCHAR(4000), @RowKey NVARCHAR(4000), @AttributeList XML,
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].InsertOrReplaceEntity;
GO

----------
--Embedded
----------

-- Blob
CREATE FUNCTION [Azure.Embedded].ListContainers(@LogicalConnectionName NVARCHAR(255),
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), LastModified DATETIME, LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255)) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].ListContainers_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].CreateContainer(@LogicalConnectionName NVARCHAR(255), @ContainerName NVARCHAR(4000), @ContainerPublicReadAccess NVARCHAR(255), @xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateContainer_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].DeleteContainer(@LogicalConnectionName NVARCHAR(255), @ContainerName NVARCHAR(4000), @leaseId UNIQUEIDENTIFIER = NULL, @xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DeleteContainer_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].ListBlobs(@LogicalConnectionName NVARCHAR(255),
	@container NVARCHAR(4000), 
	@includeSnapshots BIT,
    @includeMetadata BIT,
    @includeCopy BIT,
    @includeUncommittedBlobs BIT,
	@xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(MAX), ETag NVARCHAR(255), 
           BlobSequenceNumber INT,
           BlobType NVARCHAR(255),
           ContentEncoding NVARCHAR(255),
           ContentLanguage NVARCHAR(255),
           ContentLength BIGINT,
           ContentMD5 NVARCHAR(255),
           ContentType NVARCHAR(255),
           CopyId UNIQUEIDENTIFIER,
           CopySource NVARCHAR(255),
           CopyStatus NVARCHAR(255),
           CopyCurrentPosition BIGINT,
           CopyTotalLength BIGINT,
           CopyCompletionTime DATETIME,
           LastModified DATETIME,
           LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255),
		   Metadata XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].ListBlobs_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].DownloadBlockBlob(
	@LogicalConnectionName NVARCHAR(255),
	@container NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DownloadBlockBlob_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].DownloadPageBlob(
	@LogicalConnectionName NVARCHAR(255),
	@container NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER = NULL,
	@startPosition BIGINT = NULL, @length INT = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DownloadPageBlob_Embedded;
GO


CREATE PROCEDURE [Azure.Embedded].CreateOrReplaceBlockBlob(@LogicalConnectionName NVARCHAR(255),
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@contentType NVARCHAR(255) = 'application/octect-stream', @contentEncoding NVARCHAR(255) = NULL,
	@contentMD5 NVARCHAR(255) = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateOrReplaceBlockBlob_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].CreateOrReplaceBlockBlobFunction(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@contentType NVARCHAR(255) = 'application/octect-stream', @contentEncoding NVARCHAR(255) = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
	RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateOrReplaceBlockBlob_Function_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].DeleteBlob(@LogicalConnectionName NVARCHAR(255),
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@blobDeletionMethod NVARCHAR(255),
    @leaseID UNIQUEIDENTIFIER = NULL,
    @snapshotDateTimeToDelete DATETIME = NULL,
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].DeleteBlob_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].CopyBlob(@DestinationLogicalConnectionName NVARCHAR(255),
	@sourceAccountName NVARCHAR(255),
	@sourceContainerName NVARCHAR(4000), @sourceBlobName NVARCHAR(4000), 
    @sourceLeaseId UNIQUEIDENTIFIER = NULL, @destinationLeaseId UNIQUEIDENTIFIER = NULL,
    @destinationContainerName NVARCHAR(4000), @destinationBlobName NVARCHAR(4000), 
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CopyBlob_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].GetBlobProperties(@LogicalConnectionName NVARCHAR(255),
	@container NVARCHAR(4000), 	@blob NVARCHAR(4000), 
	@snapshotDateTime DATETIME = NULL, 
	@xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(
		   ETag NVARCHAR(255), 
           BlobSequenceNumber BIGINT,
           BlobType NVARCHAR(255),
           ContentEncoding NVARCHAR(255),
           ContentLanguage NVARCHAR(255),
           ContentLength BIGINT,
           ContentMD5 NVARCHAR(255),
           ContentType NVARCHAR(255),
           CopyId UNIQUEIDENTIFIER,
           CopySource NVARCHAR(255),
           CopyStatus NVARCHAR(255),
           CopyStatusDescription NVARCHAR(4000),
           CopyCurrentPosition BIGINT,
           CopyTotalLength BIGINT,
           CopyCompletionTime DATETIME,
           LastModified DATETIME,
           LeaseDuration NVARCHAR(255), LeaseState NVARCHAR(255), LeaseStatus NVARCHAR(255),
		   CacheControl NVARCHAR(255),
		   [Date] DATETIME,
		   RequestId UNIQUEIDENTIFIER,
		   [Version] NVARCHAR(255)
		   ) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].GetBlobProperties_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].UpdateBlobMetadata(
	@LogicalConnectionName NVARCHAR(255),
	@container NVARCHAR(4000), 	@blob NVARCHAR(4000), 
	@attributeList XML,
	@leaseId UNIQUEIDENTIFIER = NULL,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].UpdateBlobMetadata_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].CreateOrReplacePageBlob( 
	@logicalConnectionName NVARCHAR(255),
	@containerName NVARCHAR(255), @blobName NVARCHAR(255),
	@maximumSize BIGINT, @leaseId UNIQUEIDENTIFIER = NULL,
	@contentType NVARCHAR(255) = 'application/octect-stream', @contentEncoding NVARCHAR(255) = NULL,
	@contentLanguage NVARCHAR(255) = NULL,
	@timeout INT = NULL,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].CreateOrReplacePageBlob_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].PutPage(
	@logicalConnectionName NVARCHAR(255),
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@startPositionBytes INT, @bytesToUpload INT = NULL,
	@leaseId UNIQUEIDENTIFIER = NULL,
	@contentMD5 NVARCHAR(255) = NULL,
	@timeoutSeconds INT = 0, @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].PutPage_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].PutPageFunction(
	@logicalConnectionName NVARCHAR(255),
	@ContainerName NVARCHAR(4000), @BlobName NVARCHAR(4000), 
	@buffer VARBINARY(MAX), 
	@startPositionBytes BIGINT, @bytesToUpload INT = NULL,
	@leaseId UNIQUEIDENTIFIER = NULL,
	@contentMD5 NVARCHAR(255) = NULL,
	@timeoutSeconds INT = 0, @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
	RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.AzureBlob].PutPage_Function_Embedded;
GO


-- Queue
CREATE PROCEDURE [Azure.Embedded].CreateQueue(
	@logicalConnectionName NVARCHAR(255),
    @queueName NVARCHAR(255),
    @timeoutSeconds INT = 0,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].CreateQueue_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].ListQueues(
	@logicalConnectionName NVARCHAR(255),
	@prefix NVARCHAR(255) = NULL,
    @includeMetadata BIT = 0,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].ListQueues_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].ListQueues_Faulty(
	@logicalConnectionName NVARCHAR(255),
	@prefix NVARCHAR(255) = NULL,
    @includeMetadata BIT = 0,
    @timeoutSeconds INT = 60,
    @xmsclientrequestId NVARCHAR(255) = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(4000), Metadata XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].ListQueues_Faulty_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].EnqueueMessage(
	@logicalConnectionName NVARCHAR(255),
	@queueName NVARCHAR(255),
    @xmlMessage XML,
    @timeoutSeconds INT = 0,
    @xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].EnqueueMessage_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].DequeueMessage(
	@logicalConnectionName NVARCHAR(255),
	@queueName NVARCHAR(255),
    @visibilityTimeoutSeconds INT = 10, -- lower time might recover from failure faster.
    @timeoutSeconds INT = 60,
    @xmsclientrequestId  UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].DequeueMessage_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].RetrieveApproximateMessageCount(
	@logicalConnectionName NVARCHAR(255),
	@queueName NVARCHAR(255),
    @timeoutSeconds INT = 60,
    @xmsclientrequestId NVARCHAR(255) = NEWID)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Queue].RetrieveApproximateMessageCount_Embedded;
GO

-- Table
CREATE PROCEDURE [Azure.Embedded].CreateTable(@LogicalConnectionName NVARCHAR(255), @TableName NVARCHAR(4000), @xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].CreateTable_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].ListTables(@LogicalConnectionName NVARCHAR(255), @xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(Name NVARCHAR(4000), Url NVARCHAR(MAX)) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].ListTables_Embedded;
GO

CREATE FUNCTION [Azure.Embedded].QueryTable(@LogicalConnectionName NVARCHAR(255), @tableName NVARCHAR(4000), @xmsclientrequestId NVARCHAR(4000) = NULL)
RETURNS TABLE(PartitionKey NVARCHAR(4000), RowKey NVARCHAR(4000), [TimeStamp] DATETIME, Attributes XML) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].QueryTable_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].DeleteEntity(
	@LogicalConnectionName NVARCHAR(255), 
	@TableName NVARCHAR(4000), 
	@PartitionKey NVARCHAR(4000), @RowKey NVARCHAR(4000),
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].DeleteEntity_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].InsertOrReplaceEntity(
	@LogicalConnectionName NVARCHAR(255), 
	@TableName NVARCHAR(4000), 
	@PartitionKey NVARCHAR(4000), @RowKey NVARCHAR(4000), @AttributeList XML,
	@xmsclientrequestId NVARCHAR(4000) = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Table].InsertOrReplaceEntity_Embedded;
GO

------
-- General purpose
-----
CREATE FUNCTION [Azure].ToXmlDate (@dt DATETIME)
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlDate;
GO
CREATE FUNCTION [Azure].ToXmlString (@txt NVARCHAR(MAX))
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlString;
GO

CREATE FUNCTION [Azure].ToXmlInt64 (@i BIGINT)
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlInt64;
GO
CREATE FUNCTION [Azure].ToXmlDouble (@d FLOAT)
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlDouble;
GO
CREATE FUNCTION [Azure].ToXmlBinary(@d VARBINARY(MAX))
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlBinary;
GO
CREATE FUNCTION [Azure].ToXmlGuid(@d UNIQUEIDENTIFIER)
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlGuid;
GO
CREATE FUNCTION [Azure].ToXmlStatement(@stmt NVARCHAR(MAX))
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ToXmlStatement;
GO
CREATE FUNCTION [Azure].ComputeMD5AsBase64(@byteArray VARBINARY(MAX))
RETURNS NVARCHAR(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ComputeMD5AsBase64;
GO
CREATE FUNCTION [Azure].GetFileSizeBytes(@fileName NVARCHAR(4000))
RETURNS BIGINT EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].GetFileSizeBytes;
GO
CREATE FUNCTION [Azure].GetFileBlock(
	@fileName NVARCHAR(4000),
	@offsetBytes BIGINT,
	@lengthBytes INT,
	@fileShareOption NVARCHAR(255) = 'Read' 
)
RETURNS VARBINARY(MAX) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].GetFileBlock;
GO

------
-- Blob Lease
-----

-- Direct
CREATE PROCEDURE Azure.AcquireBlobFixedLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseDuration INT = 60, 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireBlobFixedLease;
GO

CREATE PROCEDURE Azure.AcquireBlobInfiniteLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireBlobInfiniteLease;
GO

CREATE PROCEDURE Azure.RenewBlobLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].RenewBlobLease;
GO

CREATE PROCEDURE Azure.ChangeBlobLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@proposedLeaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ChangeBlobLease;
GO

CREATE PROCEDURE Azure.ReleaseBlobLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ReleaseBlobLease;
GO

CREATE PROCEDURE Azure.BreakBlobLeaseWithGracePeriod(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseBreakPeriod INT = 15,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakBlobLeaseWithGracePeriod;
GO

CREATE PROCEDURE Azure.BreakBlobLeaseImmediately(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakBlobLeaseImmediately;
GO

-- Embedded
CREATE PROCEDURE [Azure.Embedded].AcquireBlobFixedLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseDuration INT = 60, 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireBlobFixedLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].AcquireBlobInfiniteLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireBlobInfiniteLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].RenewBlobLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].RenewBlobLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].ChangeBlobLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@proposedLeaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ChangeBlobLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].ReleaseBlobLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ReleaseBlobLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].BreakBlobLeaseWithGracePeriod(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@leaseBreakPeriod INT = 15,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakBlobLeaseWithGracePeriod_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].BreakBlobLeaseImmediately(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), @blobName NVARCHAR(4000), 
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakBlobLeaseImmediately_Embedded;
GO


------
-- Container Lease
-----

-- Direct
CREATE PROCEDURE Azure.AcquireContainerFixedLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000),
	@leaseDuration INT = 60, 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireContainerFixedLease;
GO

CREATE PROCEDURE Azure.AcquireContainerInfiniteLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000),
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireContainerInfiniteLease;
GO

CREATE PROCEDURE Azure.RenewContainerLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].RenewContainerLease;
GO

CREATE PROCEDURE Azure.ChangeContainerLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@proposedLeaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ChangeContainerLease;
GO

CREATE PROCEDURE Azure.ReleaseContainerLease(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ReleaseContainerLease;
GO

CREATE PROCEDURE Azure.BreakContainerLeaseWithGracePeriod(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@leaseBreakPeriod INT = 15,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakContainerLeaseWithGracePeriod;
GO

CREATE PROCEDURE Azure.BreakContainerLeaseImmediately(
	@AccountName NVARCHAR(255), @SharedKey NVARCHAR(255), @useHTTPS bit, 
	@ContainerName NVARCHAR(4000), 
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakContainerLeaseImmediately;
GO

-- Embedded
CREATE PROCEDURE [Azure.Embedded].AcquireContainerFixedLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), 
	@leaseDuration INT = 60, 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireContainerFixedLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].AcquireContainerInfiniteLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), 
	@proposedLeaseId UNIQUEIDENTIFIER = NULL,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].AcquireContainerInfiniteLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].RenewContainerLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000),
	@leaseId UNIQUEIDENTIFIER,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].RenewContainerLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].ChangeContainerLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), 
	@leaseId UNIQUEIDENTIFIER,
	@proposedLeaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ChangeContainerLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].ReleaseContainerLease(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000),  
	@leaseId UNIQUEIDENTIFIER,	
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].ReleaseContainerLease_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].BreakContainerLeaseWithGracePeriod(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), 
	@leaseBreakPeriod INT = 15,
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakContainerLeaseWithGracePeriod_Embedded;
GO

CREATE PROCEDURE [Azure.Embedded].BreakContainerLeaseImmediately(
	@LogicalConnectionName NVARCHAR(255), 
	@ContainerName NVARCHAR(4000), 
	@timeoutSeconds INT = 0,
	@xmsclientrequestId UNIQUEIDENTIFIER = NULL)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.BlobStorage].BreakContainerLeaseImmediately_Embedded;
GO

CREATE FUNCTION [Azure].GetContainerFromUri(
	@resourceUri NVARCHAR(4000))
RETURNS NVARCHAR(255) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].GetContainerFromUri;
GO

CREATE FUNCTION [Azure].GenerateBlobSharedAccessSignatureURI(
	@resourceUri NVARCHAR(4000),
	@sharedKey NVARCHAR(4000),
    @permissions NVARCHAR(8),
	@resourceType NVARCHAR(4),
	@validityStart DATETIME, 
	@validityEnd DATETIME,
    @identifier NVARCHAR(4000))
RETURNS NVARCHAR(255) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].GenerateBlobSharedAccessSignatureURI;
GO

CREATE FUNCTION [Azure].GenerateDirectBlobSharedAccessSignatureURI(
	@resourceUri NVARCHAR(4000),
	@sharedKey NVARCHAR(4000),
    @permissions NVARCHAR(8),
	@resourceType NVARCHAR(4),
	@validityStart DATETIME, 
	@validityEnd DATETIME)
RETURNS NVARCHAR(255) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].GenerateDirectBlobSharedAccessSignatureURI;
GO

CREATE FUNCTION [Azure].GeneratePolicyBlobSharedAccessSignatureURI(
	@resourceUri NVARCHAR(4000),
	@sharedKey NVARCHAR(4000),
	@resourceType NVARCHAR(4),
    @identifier NVARCHAR(4000))
RETURNS NVARCHAR(255) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].GeneratePolicyBlobSharedAccessSignatureURI;
GO


------------------
--- Management ---
------------------
CREATE FUNCTION [Azure.Management].GetServices(
	@certificateThumbprint NVARCHAR(255), 
	@subscriptionId UNIQUEIDENTIFIER) 
RETURNS TABLE(
	ServiceName							NVARCHAR(4000),
	Url									NVARCHAR(4000),
	DefaultWinRmCertificateThumbprint	NVARCHAR(255),
	AffinityGroup						NVARCHAR(4000),
	DateCreated							DATETIME,
	DateLastModified					DATETIME,
	[Description]						NVARCHAR(MAX),
	Label								NVARCHAR(4000),
	Location							NVARCHAR(4000),
	[Status]							NVARCHAR(255)
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Management].GetServices;
GO

CREATE FUNCTION [Azure.Management].GetDeploymentsPersistentVMRolesWithInputEndpoints(
	@certificateThumbprint NVARCHAR(255), 
	@subscriptionId UNIQUEIDENTIFIER,
	@serviceName NVARCHAR(255),
    @deploymentSlots NVARCHAR(255)) 
RETURNS TABLE(
	Name								NVARCHAR(4000),
	DeploymentSlot						NVARCHAR(255),
	PrivateID							NVARCHAR(255),
	[Status]							NVARCHAR(255),
	Label								NVARCHAR(4000),
	Url									NVARCHAR(4000),
	Configuration						NVARCHAR(MAX),
	UpgradeDomainCount					INT,
	VMName								NVARCHAR(4000),
	OsVersion							NVARCHAR(4000),
	RoleSize							NVARCHAR(255),
	DefaultWinRmCertificateThumbprint	NVARCHAR(4000),
	EndpointName						NVARCHAR(4000),
	LocalPort							INT,
	Port								INT,
	Protocol							NVARCHAR(255),
	Vip									NVARCHAR(255)
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Management].GetDeploymentsPersistentVMRolesWithInputEndpoints;
GO

CREATE PROCEDURE [Azure.Management].AddInputEndpointToPersistentVM(
	@certificateThumbprint		NVARCHAR(255), 
	@subscriptionId				UNIQUEIDENTIFIER,
	@serviceName				NVARCHAR(255),
    @deploymentSlots			NVARCHAR(255),
	@vmName						NVARCHAR(255),
    @EndpointName				NVARCHAR(255),
    @LocalPort					INT,
    @EnableDirectServerReturn	BIT,
    @Port						INT,
    @Protocol					NVARCHAR(255),
    @Vip						NVARCHAR(255),
	@Blocking					BIT					= 0
)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Management].AddInputEndpointToPersistentVM;
GO

CREATE PROCEDURE [Azure.Management].RemoveEndpointFromPersistentVM(
	@certificateThumbprint		NVARCHAR(255), 
	@subscriptionId				UNIQUEIDENTIFIER,
	@serviceName				NVARCHAR(255),
    @deploymentSlots			NVARCHAR(255),
	@vmName						NVARCHAR(255),
    @EndpointName				NVARCHAR(255),
	@Blocking					BIT					= 0
)
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Management].RemoveEndpointFromPersistentVM;
GO

CREATE FUNCTION [Azure.Management].GetOperationStatus (
	@certificateThumbprint		NVARCHAR(255), 
	@subscriptionId				UNIQUEIDENTIFIER,
	@operationId				UNIQUEIDENTIFIER
)
RETURNS NVARCHAR(255) EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Management].GetOperationStatus;
GO

--------------------
--- Certificates ---
--------------------
CREATE FUNCTION [Azure.Management].ListCertificates()
RETURNS TABLE(
	FriendlyName NVARCHAR(MAX), 
	IssuerName NVARCHAR(MAX), 
	SubjectName NVARCHAR(MAX), 
	Thumbprint NVARCHAR(255),
	HasPrivateKey BIT,
	NotAfter DATETIME,
	NotBefore DATETIME,
	SerialNumber NVARCHAR(255), 
	SignatureAlgorithm NVARCHAR(255), 
	[Subject] NVARCHAR(255)
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Utils].ListCertificates;
GO

--------------------
---- Streaming -----
--------------------
CREATE FUNCTION [Streaming].StreamNetXMLPlainLevel(@URI NVARCHAR(4000), @XMLLevel INT)
RETURNS TABLE(
	[Entry] XML
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Streaming.Stream].StreamNetXMLPlainLevel;
GO

CREATE FUNCTION [Streaming].StreamFileXMLPlainLevel(@fileName NVARCHAR(4000), @XMLLevel INT)
RETURNS TABLE(
	[Entry] XML
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Streaming.Stream].StreamFileXMLPlainLevel;
GO

CREATE FUNCTION [Streaming].StreamNetLine(@URI NVARCHAR(4000))
RETURNS TABLE(
	[Line] NVARCHAR(MAX)
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Streaming.Stream].StreamNetLine;
GO

CREATE FUNCTION [Streaming].StreamFileLine(@fileName NVARCHAR(4000))
RETURNS TABLE(
	[Line] NVARCHAR(MAX)
) 
AS EXTERNAL NAME [ITPCfSQL.Azure.CLR].[ITPCfSQL.Azure.CLR.Streaming.Stream].StreamFileLine;
GO


USE [master]
GO

