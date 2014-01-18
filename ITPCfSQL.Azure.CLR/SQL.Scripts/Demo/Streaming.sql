USE [DemoAzureCLR];
GO


SELECT TOP 1 * FROM [Streaming].StreamNetXMLPlainLevel('https://www.dati.lombardia.it/api/views/q563-n2qm/rows.xml?accessType=DOWNLOAD', 2);

SELECT * FROM [Streaming].StreamFileXMLPlainLevel('C:\GIT\SQLServerToAzure\Test.Console\testdata\rowsos.xml', 2);

SELECT * FROM [Streaming].StreamNetLine('https://www.dati.lombardia.it/api/views/sd8x-w4h3/rows.csv?accessType=DOWNLOAD');

SELECT * FROM [Streaming].BlockingNetLine('https://www.dati.lombardia.it/api/views/sd8x-w4h3/rows.csv?accessType=DOWNLOAD');


SELECT COUNT(*) FROM [Streaming].StreamNetLine('https://www.dati.lombardia.it/api/views/sd8x-w4h3/rows.csv?accessType=DOWNLOAD');

SELECT COUNT(*) FROM [Streaming].BlockingNetLine('https://www.dati.lombardia.it/api/views/sd8x-w4h3/rows.csv?accessType=DOWNLOAD');