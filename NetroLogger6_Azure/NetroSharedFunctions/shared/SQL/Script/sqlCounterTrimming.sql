IF not  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[COUNTER_DATA_TRIMMING]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [dbo].[COUNTER_DATA_TRIMMING](
	[LastTrimming] [datetime] NOT NULL
) ON [PRIMARY]

GO

CREATE PROCEDURE spTrimCounterData AS
BEGIN
	SET NOCOUNT ON;

	Declare @LastExecution  datetime

	set @LastExecution  = getdate()

	declare @rIndex int

	set @rindex = (select top 1 recordIndex from CounterData order by recordIndex desc)

	  --print @rIndex

	BEGIN TRY
		begin transaction

			delete from counterData where recordindex < @rindex

			if ( select count(*) from COUNTER_DATA_TRIMMING ) = 0
				begin
					insert into COUNTER_DATA_TRIMMING ( LastTrimming ) values (@LastExecution)
				end
			else
				begin
					update COUNTER_DATA_TRIMMING set  LastTrimming = @LastExecution
				end

		commit 
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK
	END CATCH	



END
GO
