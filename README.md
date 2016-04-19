# SQL Server to Azure
#### Microsoft SQL Server CLR Azure Methods
[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](LICENSE)

## Introduction
With the advent of Microsoft SQL Server 2012 SP1 CU2 Microsoft allows you to leverage many Windows Azure storage features right from SQL Server (such as backup/restore off an Azure blob).
With Microsoft SQL Server 2014 it will be possible to place datafiles and transaction logs directly in the Windows Azure containers as blobs. 
 Many of those features, however, may require you to leave SSMS and resort to other means in order to control the Windows Azure platform. 

## Why SQL Server?
There are many libraries for Azure interaction. C#, PowerShell and so on. But what about SQL Server and its T-SQL? Since SQL Server 2014 the integration with Azure is good enough to warrant a dedicated library. 

* Do you want to enumerate your blobs? Query the relevant table! 
* Do you need to import/export from Azure tables in your SQL Server Database? Use a simple ```SELECT INTO```!
* Need to push events to an Azure Queue? Call a simple stored procedure!

### Examples
For example, as detailed in this Igor Pagliai’s post ([http://blogs.msdn.com/b/igorpag/archive/2013/10/23/create-a-sql-server-2014-database-directly-on-azure-blob-storage-with-sqlxi.aspx](http://blogs.msdn.com/b/igorpag/archive/2013/10/23/create-a-sql-server-2014-database-directly-on-azure-blob-storage-with-sqlxi.aspx)), you cannot break blob leases using SQL Server. With this CLR library, all you have to do is to call the right stored procedure: 
```SQL
EXEC [Azure].BreakBlobLeaseImmediately  
	'AccountName',
	'SharedKey',
	1,
	'ContainerName',
	'BlobName'
```
And you are ready to go :).

As another example, with this code you enumerate all your containers:
```SQL
SELECT * FROM  [Azure].ListContainers(
	'AccountName', 
	'SharedKey', 
	1, 
	NULL);			
```

Each container can be enumerated too:

```SQL
SELECT * FROM  [Azure].ListBlobs(
	'AccountName', 
        'SharedKey', 
	1, 
	'ContainerName',		
	1,						
	1,						
	1,				
	1,						
	NULL);				
```

More examples here: [TechNet Blogs: Italian Premier Center for SQL Server](http://blogs.technet.com/b/italian_premier_center_for_sql_server/) or in my MSDN blog [http://blogs.msdn.com/b/frcogno](http://blogs.msdn.com/b/frcogno).


*Feel free to contribute with ideas!* :)

### About the Windows Azure REST API from SQLCLR
This library will allow you to work with blobs, tables and queues, to handle ACLs and metadata. 

I recently had a session at SQLRally Amsterdam about this topic, as soon as the video becomes available I will add it here.

Right now it supports:
* [Windows Azure Queue storage](http://msdn.microsoft.com/en-us/library/windowsazure/dd179363.aspx).
* [Windows Azure Table storage](http://msdn.microsoft.com/en-us/library/windowsazure/dd179423.aspx).
* [Windows Azure blob storage](http://msdn.microsoft.com/en-us/library/windowsazure/dd135733.aspx) (both page and block).

There are still missing functionality. For example the [Put block list](http://msdn.microsoft.com/en-us/library/windowsazure/dd179467.aspx) functionality is not implemented yet.

Note that the library target is framework 4.5 so it will work out of the box with Microsoft SQL Server 2012/2014. If you need to use it with any previous SQL Server version you might need to recompile it. 

### How to try it
In a nutshell, all you have to do is:

1. Download/clone the code and compile it.
2. Put the DLLs into a convenient location (ie reachable from Microsoft SQL Server).
3. Run the following script but make sure to replace the parameters first (ie with CTRL-Shift-M in SSMS).
4. Map the SQLCLR functions/SPs to the corresponding T-SQL ones. For that, you can use the [Creation.sql script](https://github.com/MindFlavor/sqlservertoazure/blob/master/ITPCfSQL.Azure.CLR/SQL.Scripts/Creation.sql) shipped with the library.

```SQL
CREATE ASYMMETRIC KEY ['<asymmetric_key_name, sysname, SampleAzureKey>'] FROM EXECUTABLE FILE = '<full_file_path, nvarchar(4000), Enter the full DLL path>'
CREATE LOGIN ['<asymmetric_login_name, sysname, SampleAzureLogin>'] FROM ASYMMETRIC KEY ['<asymmetric_key_name, sysname, SampleAzureKey>'];

GRANT EXTERNAL ACCESS ASSEMBLY TO ['<asymmetric_login_name, sysname, SampleAzureLogin>'];
GO

CREATE ASSEMBLY [ITPCfSQL.Azure.CLR] FROM '<full_file_path, nvarchar(4000), Enter the full DLL path>'
WITH PERMISSION_SET=EXTERNAL_ACCESS;
GO
```

### License

Please see [LICENSE](https://github.com/MindFlavor/sqlservertoazure/blob/master/LICENSE) file.

Happy Coding,
[Francesco Cogno](mailto:francesco.cogno@outlook.com).

