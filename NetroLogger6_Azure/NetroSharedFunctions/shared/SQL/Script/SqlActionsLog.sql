IF not  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ACTIONS_LOG]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [dbo].[ACTIONS_LOG](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](250)  NULL,
	[Created] [nvarchar](250)  NULL,
	[Date] [nvarchar](250)  NULL,
	[Software] [nvarchar](250)  NULL,
	[Version] [nvarchar](250)  NULL,
	[ItemsCount] [int] NOT NULL CONSTRAINT [DF_ActionsLog_ItemsCount]  DEFAULT (0),
	[AddRecordDate] [datetime] NOT NULL CONSTRAINT [DF_ActionsLog_AddRecordDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_ActionsLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
