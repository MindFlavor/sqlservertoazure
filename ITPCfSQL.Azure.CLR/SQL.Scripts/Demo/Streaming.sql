USE [DemoAzureCLR];
GO


SELECT TOP 1 * FROM [Streaming].StreamNetXMLPlainLevel('https://www.dati.lombardia.it/api/views/q563-n2qm/rows.xml?accessType=DOWNLOAD', 2);

SELECT * FROM [Streaming].StreamFileXMLPlainLevel('C:\GIT\SQLServerToAzure\Test.Console\testdata\rowsos.xml', 2);

SELECT * FROM [Streaming].StreamNetLine('https://www.dati.lombardia.it/api/views/sd8x-w4h3/rows.csv?accessType=DOWNLOAD');

SELECT * FROM [Streaming].BlockingNetLine('https://www.dati.lombardia.it/api/views/sd8x-w4h3/rows.csv?accessType=DOWNLOAD');


SELECT * FROM sys.dm_os_memory_clerks
WHERE type like '%clr%'
SELECT COUNT(*) FROM [Streaming].StreamFileLine('C:\GIT\itpcfsqlrepo\Blog\CRL Streaming\impianti.csv');
SELECT * FROM sys.dm_os_memory_clerks
WHERE type like '%clr%'


SELECT * FROM sys.dm_os_memory_clerks
WHERE type like '%clr%'
SELECT COUNT(*) FROM [Streaming].BlockingFileLine('C:\GIT\itpcfsqlrepo\Blog\CRL Streaming\impianti.csv');
SELECT * FROM sys.dm_os_memory_clerks
WHERE type like '%clr%'

DBCC DROPCLEANBUFFERS;


GROUP BY type

SELECT * FROM sys.dm_os_buffer_descriptors WHERE database_id = 14;
GROUP BY page_type

SELECT page_type FROM sys.dm_os_buffer_descriptors
GROUP BY page_type


SELECT* FROM sys.dm_clr_tasks