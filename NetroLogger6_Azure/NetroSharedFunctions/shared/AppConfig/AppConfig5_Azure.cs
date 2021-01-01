using System;
using System.Collections.Generic;
using System.Text;
//using NetroMedia.InternalServices.Proxy;

namespace NetroMedia.SharedFunctions
{
    [Serializable]
    public class AppConfig5_Azure
    {
        #region Fields

        public string Portal_Public_Key { get; set; }

        public string LogsPath_Wowza { get; set; }
        public string LogsPath_Wms { get; set; }
        public string LogsPath_Dss { get; set; }

        public string LogsPattern_Wowza { get; set; }
        public string LogsPattern_Wms { get; set; }
        public string LogsPattern_Dss { get; set; }

        public string LogsProcessCurrentFile_Wowza { get; set; }
        public string LogsProcessCurrentFile_Wms { get; set; }
        public string LogsProcessCurrentFile_Dss { get; set; }

        public string AzureStorage_BlobsKey { get; set; }
        public string AzureStorage_BlobsRoot { get; set; }
        public string AzureStorage_BlobsBucket { get; set; }

        public string AzureSql_Server { get; set; }
        public string AzureSql_User { get; set; }
        public string AzureSql_Password { get; set; }
        public string AzureSql_Database { get; set; }

        public string UpdatesServer { get; set; }
        public string ServerPublicIP { get; set; }

        public string WMS_SecondsToWaitAfterACall { get; set; }

        public string Wowzacounters_Address { get; set; }
        public string Wowzacounters_User { get; set; }
        public string Wowzacounters_Pwd { get; set; }

        public string RTV_Service_Type { get; set; }
        public string RTV_Server { get; set; }
        public string RTV_Database { get; set; }
        public string RTV_User { get; set; }
        public string RTV_Password { get; set; }
        public string RTV_Encrypt { get; set; }


        public string Service1 { get; set; }
        public string Service2 { get; set; }

        #endregion

        #region Constructor

        public AppConfig5_Azure()
        {
            Wowzacounters_Address = "http://localhost:8086/connectioncounts";
            Wowzacounters_User = "netroadmin";
            Wowzacounters_Pwd = "netroadmin";


            Service1 = "";
            Service2 = "";

            RTV_Service_Type = "WMS";
            RTV_Server = "tcp:cqfxnpw8jr.database.windows.net";
            RTV_Database = "NetroCounters";
            RTV_User = "NetroRTV";
            RTV_Password = "453t4nbh354ITHHUtt34t34ct3245ntmb";
            RTV_Encrypt = "Yes";


            Portal_Public_Key = "BFBE7C55-FDDA-43C8-BE0D-3D4F5643BA8E";
            LogsPath_Wowza = @"C:\Netro\Wowza\logs";
            LogsPath_Wms = @"C:\WINDOWS\system32\Logfiles\WMS\[Global]";
            LogsPath_Dss=@"C:\Program Files\Darwin Streaming Server\Logs";

            LogsPattern_Wowza  = "*access.log.2*";
            LogsPattern_Wms  = "*.log";
            LogsPattern_Dss = "StreamingServer.*.log";

            LogsProcessCurrentFile_Wowza = "N";
            LogsProcessCurrentFile_Wms = "N";
            LogsProcessCurrentFile_Dss = "Y";

            UpdatesServer = "uploads.netromedia.com:90";


            AzureStorage_BlobsKey = "DefaultEndpointsProtocol=https;AccountName=netrologs;AccountKey=Bi0MLDHWSs/xF8d2I8eoniVAF12XQDYKOVBOe5plu3wtq8D14tZswtDAtqpdy2VP2BAS1DB/Dt47mmS0SlYS/w==";
            AzureStorage_BlobsRoot = "http://netro.blob.core.windows.net";
            AzureStorage_BlobsBucket = "netrologs";

            AzureSql_Server = "trshcqg14q.database.windows.net";
            AzureSql_User = "NetroStats";
            AzureSql_Password = "nstaHHT8@hdxp348gt32ho9@t3t9on3@$$$heotthdxTTDIeo385";
            AzureSql_Database = "NetroLog";

            WMS_SecondsToWaitAfterACall = "30";


        }

        #endregion
    }
}
