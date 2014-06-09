USE [DemoAzureCLR];
GO

---
CREATE TABLE tbl(Evt DATETIME, Val FLOAT);
TRUNCATE TABLE tbl;
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:00', 15);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:01', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:02', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:03', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:04', 34);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:05', 30);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:06', 15);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:07', 15);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:08', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:09', 15);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10', 14);

SELECT * FROM tbl;
SELECT * FROM [Normalization].TimeNormalize('SELECT * FROM tbl', 0, 0,0,1,0);

TRUNCATE TABLE tbl;
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.000', 0);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.123', 230);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.145', 244);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.153', 240);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.165', 248);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.175', 246);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.215', 100);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10.218', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:11.218', 13);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:12.218', 13);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:13.218', 13);
--SELECT * FROM tbl WHERE Evt > '2014-01-01 17:00:10.000'
SELECT * FROM tbl;
SELECT * FROM [Normalization].TimeNormalize('SELECT * FROM tbl', 0, 0,0,1,0);


TRUNCATE TABLE tbl;
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:00:10', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:11', 13);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:12', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:13', 14);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:14', 13);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:15', 33);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:16', 7);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:17', 33);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:02:18', 30);
INSERT INTO tbl(Evt, Val) VALUES('20140101 17:14:17', 14);
SELECT * FROM tbl;
SELECT * FROM [Normalization].TimeNormalize('SELECT * FROM tbl', 0, 0,0,1,0);


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
	[Server] + ' - ' + [Status]	= 'TTTBACP504 - dormant'
ORDER BY 
	CONVERT(DATETIME, EventTime);


SELECT * FROM [Normalization].TimeNormalize(
	'SELECT 		
		CONVERT(DATETIME, EventTime)	AS ''EventTime''
		,CONVERT(FLOAT, Cnt)			AS ''Cnt''
	FROM #import
	WHERE 
		[Server] + '' - '' + [Status]	= ''TTTBACP504 - dormant''
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


