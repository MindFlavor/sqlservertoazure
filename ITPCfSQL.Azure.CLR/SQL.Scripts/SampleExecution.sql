USE [DemoAzureCLR];
GO

SELECT * FROM [Azure].ListContainers(
	'<Azure Account Name, nvarchar(4000), AzureAccountName>',																					
	'<Azure Shared Access Key, nvarchar(4000), [paste your key here]>', 
	1, 
	NULL)
GO

EXEC [Azure].CreateContainer   
	'<Azure Account Name, nvarchar(4000), AzureAccountName>',																					
	'<Azure Shared Access Key, nvarchar(4000), [paste your key here]>',
	1,
	'testcontainer',
	'Blob'
GO

SELECT [Azure].CreateOrReplaceBlockBlobFunction(
	'<Azure Account Name, nvarchar(4000), AzureAccountName>',																					
	'<Azure Shared Access Key, nvarchar(4000), [paste your key here]>',
	1,
	'testcontainer',
	'textfromsql.txt',
	CONVERT(VARBINARY(MAX), N'This file was created by SQL Server! ' + @@SERVERNAME + ': ' + CONVERT(NVARCHAR, GETDATE())),
	'text/plain',
	NULL,
	NULL)
GO

SELECT [Azure].CreateOrReplaceBlockBlobFunction(
	'<Azure Account Name, nvarchar(4000), AzureAccountName>',																					
	'<Azure Shared Access Key, nvarchar(4000), [paste your key here]>',
	1,
	'testcontainer',
	'paperotto.jpg',
	A.BulkColumn,
	'image/jpeg',
	NULL,
	NULL)
FROM OPENROWSET(BULK 'c:\temp\paperotto.jpg', SINGLE_BLOB) A
GO

SELECT * FROM [Azure].ListBlobs(
	'<Azure Account Name, nvarchar(4000), AzureAccountName>',																					
	'<Azure Shared Access Key, nvarchar(4000), [paste your key here]>',
	1,
	'testcontainer',
	1, 
	1, 
	1, 
	1, 
	NULL)
GO

--- Cleanup

EXEC [Azure].DeleteContainer 	
	'<Azure Account Name, nvarchar(4000), AzureAccountName>',																					
	'<Azure Shared Access Key, nvarchar(4000), [paste your key here]>', 
	1,
	'testcontainer'
GO