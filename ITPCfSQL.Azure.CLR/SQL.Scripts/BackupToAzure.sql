IF EXISTS(SELECT * FROM tempdb.sys.objects WHERE name = '##progressTable')
DROP TABLE ##progressTable;

SET NOCOUNT ON;
BACKUP DATABASE [AdventureWorks] TO DISK='C:\temp\AdventureWorks.bak' WITH INIT, COMPRESSION;
GO

DECLARE @size BIGINT = [Azure].GetFileSizeBytes('C:\temp\AdventureWorks.bak');

CREATE TABLE ##progressTable(ID INT IDENTITY(1,1) PRIMARY KEY CLUSTERED, Chunk INT NOT NULL, OpStatus NVARCHAR(MAX) NULL);

DECLARE @PageCount INT = CEILING(CONVERT(FLOAT, @size) / (1024.0*32));

PRINT CONVERT(VARCHAR, @pageCount) +' 32KB-pages to upload (total ' +  CONVERT(VARCHAR, @size) + ' bytes). Added ' + CONVERT(VARCHAR,(@PageCount*1024*32) - @size) + ' bytes as padding.';

-- Prestage the progess table
DECLARE @iCnt INT = 0;
WHILE @iCnt < @pageCount BEGIN
	INSERT INTO ##progressTable(Chunk, OpStatus) VALUES(@iCnt, 'ToUpload')
	SET @iCnt += 1;
END

-- Create the "sparse" page blob.
EXEC [Azure].CreateOrReplacePageBlob    
	'your_account',
	'your_shared_key', 1,
	'your_container',
	'AdventureWorks.bak',
	@size

PRINT '"Sparse" blob created.'

DECLARE @pageNum INT, @ID INT;
DECLARE @len INT;
WHILE(EXISTS(SELECT * FROM ##progressTable WHERE OpStatus IS NOT NULL))
BEGIN
	SELECT TOP 1 @pageNum = Chunk, @ID = ID FROM ##progressTable WHERE OpStatus IS NOT NULL;

	SET @len = 1024*32;
	IF ((1024*32*@pageNum)+@len)>@size 
		SET @len = @size -(1024*32*@pageNum);

	PRINT 'Uploading page number ' + CONVERT(VARCHAR, @pageNum) + '. ' +  CONVERT(VARCHAR, @len) + ' bytes to upload.';

	UPDATE ##progressTable SET OpStatus = [Azure].PutPageFunction(
		'your_account',	'your_shared_key', 1,
		'your_container',
		'AdventureWorks.bak',
		[Azure].GetFileBlock(
			'C:\temp\AdventureWorks.bak',
			1024*32*@pageNum,
			@len,
			'Read'),
		1024*32*@pageNum,
		@len,
		NULL,
		NULL,
		0,
		NULL)
	WHERE ID = @ID;
END

SELECT * FROM [Azure].ListBlobs(
	'your_account',
	'your_shared_key', 1,
	'your_container',
	1,1,1,1,
	NULL);