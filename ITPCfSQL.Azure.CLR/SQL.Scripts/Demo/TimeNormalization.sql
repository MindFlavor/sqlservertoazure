USE [DemoAzureCLR];
GO

---
CREATE TABLE tbl(Evt DATETIME, Val FLOAT);
TRUNCATE TABLE tbl;
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:00', 100);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:00.600', 50);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:05.600', 6000);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:07.000', -14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:08.340', -300);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:08.770', 0);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:08.950', 720);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.001', -14);

SELECT * FROM [Normalization].TimeNormalize('SELECT * FROM tbl', 0, 0,0,1,0);



DROP TABLE #import;
CREATE TABLE #import([Server] NVARCHAR(255), [Status] NVARCHAR(255), Cnt  NVARCHAR(255), EventTime  NVARCHAR(255), Extra NVARCHAR(255));

INSERT INTO #import
EXEC [Streaming].FileStringSplitToTable 'D:\GIT\SQLServerToAzure\Test.Console\testdata\timedata.csv', ',';

SELECT 
	[Server] + ' - ' + [Status]		AS 'Group'
FROM #import
GROUP BY
	[Server] + ' - ' + [Status]
ORDER BY 	
	[Server] + ' - ' + [Status];

SELECT 
	CONVERT(DATETIME, EventTime)	AS 'EventTime'
	,CONVERT(FLOAT, Cnt)			AS 'Cnt'
FROM #import
WHERE 
	[Server] + ' - ' + [Status]	= 'EPDBACP501 - dormant'
ORDER BY 
	CONVERT(DATETIME, EventTime);


SELECT * FROM [Normalization].TimeNormalize(
	'SELECT 		
		CONVERT(DATETIME, EventTime)	AS ''EventTime''
		,CONVERT(FLOAT, Cnt)			AS ''Cnt''
	FROM #import
	WHERE 
		[Server] + '' - '' + [Status]	= ''EPDBACP501 - dormant''
	ORDER BY 
		CONVERT(DATETIME, EventTime);'
, 0, 0,0,1,0);


------------------
--- Everything ---
------------------
SELECT 
	 [Status]
FROM #import
GROUP BY
	[Status]
ORDER BY 	
	[Status];

SELECT 
	CONVERT(DATETIME, EventTime)	AS 'EventTime'
	,SUM(CONVERT(FLOAT, Cnt))		AS 'Cnt'
FROM #import
WHERE 
	[Status]	= 'dormant'
GROUP BY CONVERT(DATETIME, EventTime)
ORDER BY 
	CONVERT(DATETIME, EventTime);

SELECT * FROM [Normalization].TimeNormalize(
	'SELECT 
	CONVERT(DATETIME, EventTime)	AS ''EventTime''
	,SUM(CONVERT(FLOAT, Cnt))		AS ''Cnt''
FROM #import
WHERE 
	[Status]	= ''dormant''
GROUP BY CONVERT(DATETIME, EventTime)
ORDER BY 
	CONVERT(DATETIME, EventTime);'
, 0, 0,0,1,0);


SELECT * FROM [Normalization].TimeNormalize_TestType(
	'SELECT 		
		CONVERT(DATETIME, EventTime)	AS ''EventTime''
		,CONVERT(FLOAT, Cnt)			AS ''Cnt''
	FROM #import
	WHERE 
		[Server] + '' - '' + [Status]	= ''EPDBACP501 - dormant''
	ORDER BY 
		CONVERT(DATETIME, EventTime);'
, 0, 0,0,1,0);


