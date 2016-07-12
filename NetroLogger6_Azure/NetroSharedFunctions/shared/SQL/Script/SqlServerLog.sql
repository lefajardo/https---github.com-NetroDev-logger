-- Copyright (C) 2008  NetroMedia. All rights reserved.
-- NOTICE: This code has not been licensed under any public license.
--drop table dbo.LOG_STAGING
if not exists (select * from dbo.sysobjects where id = object_id(N'dbo.LOG_STAGING') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
  begin
	print 'Create Table dbo.LOG_STAGING';
	Create Table dbo.LOG_STAGING
		( 	UID				int		IDENTITY	(1, 1) NOT NULL 
			,LOGFILENAME	varchar				(255)
			,LOGCREATEDATE	datetime			NULL
			,FORMAT_CODE	int					NULL
			,CIP			varchar				(255)
			,DateTime		datetime			NULL 
			,PubPoint		varchar				(255)  
			,csUriStem		varchar				(255)  
			,cStarttime		int					NULL
			,xDuration		int					NULL
			,cStatus		int					NULL
			,cPlayerid		varchar				(255)
			,cPlayerversion varchar				(255)  
			,csUserAgent	varchar				(255)
			,cOs			varchar				(255)
			,avgbandwidth	int					NULL
			,protocol		varchar				(255)
			,transport		varchar				(255)
			,KiloBytesSent	bigint				NULL
			,cBytes			bigint				NULL
			,sIp			varchar				(255)
			,Server			varchar				(255)
			,sTotalclients	int					NULL
			,sContentPath	varchar				(255)
			,csUrl			varchar				(255)
			,csMediaName	varchar				(255)
			,cMaxBandwidth	int					NULL
			,csMediaRole	varchar				(255)
			,sProxied		int					NULL
			,harvestedfrom	varchar				(255)  
			,longIP			bigint				NULL 
			,countryA2		char				(2)
			,endDateTime	datetime			NULL 
			,state			varchar				(250)
			,city			varchar				(250)
			,country		varchar				(250)
			,SERVER_NAME	varchar				(250)

		)

	--create index IDX_LOG_STAGING_ASSIGNED_USER_ID on dbo.LOG_STAGING (ASSIGNED_USER_ID, DELETED, ID)
	
	
  end
