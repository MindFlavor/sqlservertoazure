IF EXISTS(SELECT * FROM tempdb.sys.objects WHERE name = '##progressTable')
DROP TABLE ##progressTable;

SET NOCOUNT ON;
BACKUP DATABASE [DemoAzureCLR] TO DISK='C:\temp\DemoAzureCLR.bak' WITH INIT, COMPRESSION;
GO

DECLARE @size BIGINT = [Azure].GetFileSizeBytes('C:\temp\DemoAzureCLR.bak');

CREATE TABLE ##progressTable(ID INT IDENTITY(1,1) PRIMARY KEY CLUSTERED, OpStatus NVARCHAR(MAX) NULL);

DECLARE @PageCount INT = CEILING(CONVERT(FLOAT, @size) / (1024.0*8.0));

PRINT CONVERT(VARCHAR, @pageCount) +' 8KB-pages to upload (total ' +  CONVERT(VARCHAR, @size) + ' bytes). Added ' + CONVERT(VARCHAR,(@PageCount*1024*8) - @size) + ' bytes for padding.';

-- Prestage the progess table
DECLARE @iCnt INT = 0;
WHILE @iCnt < @pageCount BEGIN
	INSERT INTO ##progressTable(OpStatus) VALUES('ToUpload')
	SET @iCnt += 1;
END

-- Create the "sparse" page blob.
EXEC [Azure].CreateOrReplacePageBlob    
	'frcogno',
	'j6KXtMqeNAopi2fF8ADi9BHYdXfiIim3yMFGSvVwQcMIdb0xNwh46Wv9xEzYbLEcqzb5rkcVAtyPqBaK0lVJWg==', 1,
	'testcontainer',
	'DemoAzureCLR.bak',
	@size

PRINT '"Sparse" blob created.'


DECLARE @pageNum INT;
WHILE(EXISTS(SELECT * FROM ##progressTable WHERE OpStatus IS NOT NULL))
BEGIN
	SELECT TOP 1 @pageNum = ID FROM ##progressTable WHERE OpStatus IS NOT NULL;

	PRINT 'Uploading page number ' + CONVERT(VARCHAR, @pageNum);

	UPDATE ##progressTable SET OpStatus = [Azure].PutPageFunction(
		'frcogno',	'j6KXtMqeNAopi2fF8ADi9BHYdXfiIim3yMFGSvVwQcMIdb0xNwh46Wv9xEzYbLEcqzb5rkcVAtyPqBaK0lVJWg==', 1,
		'testcontainer',
		'DemoAzureCLR.bak',
		[Azure].GetFileBlock(
			'C:\temp\DemoAzureCLR.bak',
			1024*@pageNum,
			1024*8,
			'Read'),
		1024*@pageNum,
		1024*8,
		NULL,
		NULL,
		0,
		NULL)
	WHERE ID = @pageNum;
END

SELECT * FROM [Azure].ListBlobs(
	'frcogno',
	'j6KXtMqeNAopi2fF8ADi9BHYdXfiIim3yMFGSvVwQcMIdb0xNwh46Wv9xEzYbLEcqzb5rkcVAtyPqBaK0lVJWg==', 1,
	'testcontainer',
	1,1,1,1,
	NULL);


--DROP TABLE ##progressTable;

