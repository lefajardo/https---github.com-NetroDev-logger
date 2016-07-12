using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;


namespace NetroLogger_WorkerRole
{
    

    
    public class logStagingDailyAzTbl : TableEntity
    {

        public logStagingDailyAzTbl()
        {}

        public logStagingDailyAzTbl(string pubpoint_, string day_)
        {
            PUBPOINT = pubpoint_;
            DATETIME_DAY = day_;
            this.PartitionKey = pubpoint_;
            this.RowKey = day_;
        }
        public string PUBPOINT { get; set; }
        public string DATETIME_DAY { get; set; }
        public string CONNECTIONS { get; set; }
        public string KILOBYTES_SENT { get; set; }
    }

    public class logStagingAzTbl : TableEntity
    {

        public logStagingAzTbl(){   
        }

        public logStagingAzTbl(string pubpoint_, string server_) {
            PUBPOINT = pubpoint_;
            SERVER = server_;
            this.PartitionKey = pubpoint_;
            this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString() + "__" + server_;
        }
        public string LOGFILENAME { get; set; }
        public string LOGCREATEDATE { get; set; }
        public string FORMAT_CODE { get; set; }
        public string CIP { get; set; }
        public string DATETIME { get; set; }
        public string ENDDATETIME { get; set; }
        public string PUBPOINT { get; set; }
        public string CSURISTEM { get; set; }
        public string SIP { get; set; }
        public string CSURL { get; set; }
        public string PROTOCOL { get; set; }
        public string CSMEDIANAME { get; set; }
        public string CPLAYERID { get; set; }
        public string CSUSERAGENT { get; set; }
        public string CSTARTTIME { get; set; }
        public string XDURATION { get; set; }
        public string CSTATUS { get; set; }
        public string CPLAYERVERSION { get; set; }
        public string COS { get; set; }
        public string AVGBANDWIDTH { get; set; }
        public string TRANSPORT { get; set; }
        public string KILOBYTESSENT { get; set; }
        public string CBYTES { get; set; }
        public string SERVER { get; set; }
        public string STOTALCLIENTS { get; set; }
        public string SCONTENTPATH { get; set; }
        public string CMAXBANDWIDTH { get; set; }
        public string CSMEDIAROLE { get; set; }
        public string SPROXIED { get; set; }
        public string HARVESTEDFROM { get; set; }
        public string LONGIP { get; set; }
        public string COUNTRYA2 { get; set; }
       
        public string STATE { get; set; }
        public string CITY { get; set; }
        public string COUNTRY { get; set; }
        public string SERVER_NAME{ get; set; }
        public string LONGITUDE { get; set; }
        public string LATITUDE { get; set; }
        public string BATCH_ID { get; set; }
        public string BATCH_COMPLETED { get; set; }
        public string DAY{ get; set; }
        public string MONTH{ get; set; }
        public string YEAR { get; set; }
        public string HOUR { get; set; }
        //public string processedrollup { get; set; }
    }

    public class logsToRullUp {
        public string SERVER { get; set; }
        public string DAY { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string HOUR { get; set; }

        public logsToRullUp() {
        }

        public logsToRullUp( string server_, string day_, string month_, string year_, string hour_)
        {
            SERVER  = server_;
            DAY = day_;
            MONTH = month_;
            YEAR  = year_;
            HOUR = hour_;
        }
    }

    public class UploadedLogFiles : TableEntity
    {

        public string SERVICE_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_ID { get; set; }
        public string LOG_URL { get; set; }
        public string HASH { get; set; }
        public string PROCESSED { get; set; }
        public string DATE_ENTERED { get; set; }
        public string DATE_MODIFIED { get; set; }
        public string DATE_PROCESSED { get; set; }
        public string BATCH_ID { get; set; }

        public UploadedLogFiles(string SERVICE_TYPE, string HASH)
        {
            this.PartitionKey = SERVICE_TYPE;
            this.RowKey = HASH;

        }

        public UploadedLogFiles() { }

    }

    public class logBatchesAzTbl : TableEntity
    {
        public logBatchesAzTbl() { 
        }
        
        public logBatchesAzTbl(Guid batchID)
        {
            BATCH_ID = batchID ;
            
            this.PartitionKey = "";
            this.RowKey = batchID.ToString() ;
        }
        public Guid  BATCH_ID { get; set; }
        public string BATCH_COMPLETED { get; set; }
        public string ROLLUP_COMPLETED { get; set; }
        
    }

    public class logErrorssAzTbl : TableEntity
    {
        public logErrorssAzTbl()
        { 
        }

        public logErrorssAzTbl(string server_)
        {
            SERVER  = server_;

            this.PartitionKey = server_;
            this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString() + "__" + server_;
        }
        public string LOGFILENAME { get; set; }
        public string SERVER { get; set; }
        public string SECTION_ERR { get; set; }
        public string ERR_DESCRIPTION { get; set; }

    }

    
    public class logFilesAzTbl : TableEntity
    {

        public logFilesAzTbl()
        { }

        public logFilesAzTbl(String hash)
        {
            HASH = hash  ;            
            this.PartitionKey = "";
            this.RowKey = hash.ToString() ;
        }
        public Guid  LOG_ID { get; set; }
        public string DATE_ENTERED { get; set; }
        public string HASH { get; set; }
        public string LOGFILENAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_IP { get; set; }
        public string LOG_URL { get; set; }
        
    }

    public class logFilesNOPROCESSAzTbl : TableEntity
    {

        public logFilesNOPROCESSAzTbl()
        { }

        public logFilesNOPROCESSAzTbl(String filetype, String hash)
        {
            HASH = hash;
            FILETYPE = filetype; 
            this.PartitionKey = filetype ;
            this.RowKey = hash.ToString();
        }        
        public string DATE_ENTERED { get; set; }
        public string DATE_NOPROCESS { get; set; }
        public string FILETYPE { get; set; }
        public string HASH { get; set; }
        public string LOGFILENAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_IP { get; set; }
        public string LOG_URL { get; set; }

    }


    public class logFilesRollUpAzTbl : TableEntity
    {

        public logFilesRollUpAzTbl()
        {
        }

        public logFilesRollUpAzTbl(Guid logID)
        {
            LOG_ID = logID; 
            this.PartitionKey = "";
            this.RowKey = logID.ToString() ;
        }

        public Guid  LOG_ID { get; set; }
        public string DATE_ENTERED { get; set; }
        public string CONNECTIONS { get; set; }
        public string KILOBYTES_SENT { get; set; }
        
    }


    public class logFilesHourlyRollUpAzTbl : TableEntity
    {

        public logFilesHourlyRollUpAzTbl()
        {
        }

        public logFilesHourlyRollUpAzTbl(string hour_ , string pubpoint_,
            float connections, float kbsent, string server, string day,
            string month, string year, string hour
            )
        {
            DATETIME_HOUR = hour_ ;
            PUBPOINT = pubpoint_;
            this.PartitionKey = pubpoint_;
            this.RowKey = hour_.ToString();
            CONNECTIONS = connections;
            KILOBYTES_SENT = kbsent;
            SERVER = server;
            DAY = day;
            MONTH = month;
            YEAR = year;
            HOUR = hour;
        }

        public string DATETIME_HOUR { get; set; }
        public string PUBPOINT { get; set; }
        public float CONNECTIONS { get; set; }
        public float KILOBYTES_SENT { get; set; }
        public string SERVER { get; set; }
        public string DAY { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string HOUR { get; set; }


    }

    public class logFilesHourlyRollUpAzTbl2 : TableEntity
    {

        public logFilesHourlyRollUpAzTbl2()
        {
        }

        public logFilesHourlyRollUpAzTbl2(string hour_, string pubpoint_,
            string connections, string kbsent, string server, string day,
            string month, string year, string hour
            )
        {
            DATETIME_HOUR = hour_;
            PUBPOINT = pubpoint_;
            this.PartitionKey = pubpoint_;
            this.RowKey = hour_.ToString();
            CONNECTIONS = connections;
            KILOBYTES_SENT = kbsent;
            SERVER = server;
            DAY = day;
            MONTH = month;
            YEAR = year;
            HOUR = hour;
        }

        public string DATETIME_HOUR { get; set; }
        public string PUBPOINT { get; set; }
        public string CONNECTIONS { get; set; }
        public string KILOBYTES_SENT { get; set; }
        public string SERVER { get; set; }
        public string DAY { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string HOUR { get; set; }


    }
    

}
