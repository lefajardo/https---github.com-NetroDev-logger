using System;
using System.Collections.Generic;
using System.Text;
//using NetroMedia.InternalServices.Proxy;

namespace NetroMedia.SharedFunctions
{
    [Serializable]
    public class AppConfig
    {
        #region Fields

        public string Database { get; set; }
        public string ServerIP { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Folder { get; set; }
        public string FileMask { get; set; }
        public bool Enabled { get; set; }
        public string TableName { get; set; }
        public string FolderToMoveProcessedFiles { get; set; }
        public bool CanPurgeOldFiles { get; set; }
        public int PurgeFilesOlderThan { get; set; }
        public bool ProcessCurrentLog { get; set; }
        public string ServiceID { get; set; }




        public string DatabaseNonLog { get; set; }
        public string ServerIPNonLog { get; set; }
        public string UserNameNonLog { get; set; }
        public string PasswordNonLog { get; set; }
        public string FolderNonLog { get; set; }
        public string FileMaskNonLog { get; set; }
        public bool EnabledNonLog { get; set; }
        public string TableNameNonLog { get; set; }
        public string ServiceIDNonLog { get; set; }

        public string DatabaseIceNonLog { get; set; }
        public string ServerIPIceNonLog { get; set; }
        public string UserNameIceNonLog { get; set; }
        public string PasswordIceNonLog { get; set; }
        public string FolderIceNonLog { get; set; }
        public string FileMaskIceNonLog { get; set; }
        public bool EnabledIce { get; set; }
        public string TableNameIce { get; set; }
        public string ServiceIDIce { get; set; }

        public string DatabaseWmNonLog { get; set; }
        public string ServerIPWmNonLog { get; set; }
        public string UserNameWmNonLog { get; set; }
        public string PasswordWmNonLog { get; set; }
        public string FolderWmNonLog { get; set; }
        public string FileMaskWmNonLog { get; set; }
        public bool EnabledWm { get; set; }
        public string TableNameWm { get; set; }
        public string FolderToMoveProcessedFilesWm { get; set; }
        public bool CanPurgeOldFilesWm { get; set; }
        public int PurgeFilesOlderThanWm { get; set; }
        public bool ProcessCurrentLogWm { get; set; }
        public string ServiceIDWm { get; set; }



        public string DatabaseWowNonLog { get; set; }
        public string ServerIPWowNonLog { get; set; }
        public string UserNameWowNonLog { get; set; }
        public string PasswordWowNonLog { get; set; }
        public string FolderWowNonLog { get; set; }
        public string FileMaskWowNonLog { get; set; }
        public bool EnabledWow { get; set; }
        public string TableNameWow { get; set; }
        public string FolderToMoveProcessedFilesWow { get; set; }
        public bool CanPurgeOldFilesWow { get; set; }
        public int PurgeFilesOlderThanWow { get; set; }
        public bool ProcessCurrentLogWow { get; set; }
        public string ServiceIDWow { get; set; }


        public string DatabaseShoutNonLog { get; set; }
        public string ServerIPShoutNonLog { get; set; }
        public string UserNameShoutNonLog { get; set; }
        public string PasswordShoutNonLog { get; set; }
        public string FolderShoutNonLog { get; set; }
        public string FileMaskShoutNonLog { get; set; }
        public bool EnabledShout { get; set; }
        public string TableNameShout { get; set; }
        public string ServiceIDShout { get; set; }

        public bool EnabledCounterTrimmer { get; set; }
        public string SecondsIntervalCounterTrimmer { get; set; }
        public string DatabaseCounterTrimmer { get; set; }
        public string ServerIPCounterTrimmer { get; set; }
        public string UserNameCounterTrimmer { get; set; }
        public string PasswordCounterTrimmer { get; set; }

        public double Scheduler_RunEachHours { get; set; }
        public string Scheduler_Command1 { get; set; }



        // Installer Config

        public string CENTRAL_DB_IP { get; set; }
        public string CENTRAL_DB_User { get; set; }
        public string CENTRAL_DB_Pwd { get; set; }
        public string CENTRAL_DB_Database { get; set; }

        public string NETRO_ROOT_DIR { get; set; }

        public string WMS_INSTALL_SCRIPTS { get; set; }
        public string WOWZA_INSTALL_SCRIPTS { get; set; }
        public string COMMON_SCRIPTS { get; set; }

        public string email_from { get; set; }
        public string email_to { get; set; }
        public string email_smtp { get; set; }
        public string email_user { get; set; }
        public string email_pwd { get; set; }

        public bool use_timer_to_keep_alive_sql { get; set; }
        public int keep_alive_timer_query_interval { get; set; }
        public string public_ip_this_server { get; set; }


        // coming from netroftp, now must use CENTRAL_DB_IP..... 

        //public string Database { get; set; }
        //public string ServerIP { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }

        public string FTP_Folder { get; set; }
        public double FTP_RunEachHours { get; set; }
        public string FTP_FilezillaServerPath { get; set; }
        public int FTP_Debug { get; set; }
        public string FTP_ServiceID { get; set; }
        public bool FTP_Enabled { get; set; }

        public string WMS_serversToQuery { get; set; }
        public string WMS_SecondsToWaitAfterACall { get; set; }
        public string WMS_ServiceID { get; set; }
        public bool WMS_Enabled { get; set; }


        public int log_files_minimun_length { get; set; }

        public bool send_debug_emails { get; set; }

        public string Wowzacounters_Address { get; set; }
        public string Wowzacounters_User { get; set; }
        public string Wowzacounters_Pwd { get; set; }

        public string RTV_Service_Type { get; set; }
        public string RTV_Server { get; set; }
        public string RTV_Database { get; set; }
        public string RTV_User { get; set; }
        public string RTV_Password { get; set; }
        public string RTV_Encrypt { get; set; }

        public string Portal_Public_Key { get; set; }

        public string AzureBlobsKey { get; set; }
        public string AzureBlobsRoot { get; set; }
        public string AzureBlobsBucket { get; set; }

        #endregion

        #region Constructor

        public AppConfig()
        {



            send_debug_emails = false;

            Scheduler_RunEachHours = 0;
            Scheduler_Command1 = "";

            Database = "netrolog";
            ServerIP = "";
            UserName = "netrostats";
            Password = "";
            Folder = "";
            FileMask = "";
            Enabled = false;
            TableName = "";
            FileMaskNonLog = "";
            EnabledNonLog = false;
            TableNameNonLog = "";
            ProcessCurrentLog = false;


            DatabaseNonLog = "netrolog";
            ServerIPNonLog = "";
            UserNameNonLog = "netrostats";
            PasswordNonLog = "";
            FolderNonLog = "";
            FileMaskNonLog = "";
            EnabledNonLog = false;
            TableNameNonLog = "";

            DatabaseIceNonLog = "netrolog";
            ServerIPIceNonLog = "";
            UserNameIceNonLog = "netrostats";
            PasswordIceNonLog = "";
            FolderIceNonLog = "";
            FileMaskIceNonLog = "";
            EnabledIce = false;
            TableNameIce = "";

            DatabaseWmNonLog = "netrolog";
            ServerIPWmNonLog = "";
            UserNameWmNonLog = "netrostats";
            PasswordWmNonLog = "";
            FolderWmNonLog = "";
            FileMaskWmNonLog = "";
            EnabledWm = false;
            TableNameWm = "";
            FolderToMoveProcessedFilesWm = "";
            CanPurgeOldFilesWm = false;
            PurgeFilesOlderThanWm = 0;
            ProcessCurrentLogWm = false;

            DatabaseWowNonLog = "netrolog";
            ServerIPWowNonLog = "";
            UserNameWowNonLog = "netrostats";
            PasswordWowNonLog = "";
            FolderWowNonLog = "";
            FileMaskWowNonLog = "";
            EnabledWow = false;
            TableNameWow = "";



            DatabaseShoutNonLog = "netrolog";
            ServerIPShoutNonLog = "";
            UserNameShoutNonLog = "netrostats";
            PasswordShoutNonLog = "";
            FolderShoutNonLog = "";
            FileMaskShoutNonLog = "";
            EnabledShout = false;
            TableNameShout = "";


            EnabledCounterTrimmer = false;
            SecondsIntervalCounterTrimmer = "0";
            DatabaseCounterTrimmer = "";
            ServerIPCounterTrimmer = "";
            UserNameCounterTrimmer = "";
            PasswordCounterTrimmer = "";


            CENTRAL_DB_IP = "174.129.246.39,1433";
            CENTRAL_DB_User = "netrostats";
            CENTRAL_DB_Pwd = "nstat32ho9t3t9on3";
            CENTRAL_DB_Database = "netrolog";

            WMS_INSTALL_SCRIPTS = "";
            WOWZA_INSTALL_SCRIPTS = "";
            COMMON_SCRIPTS = "";

            NETRO_ROOT_DIR = @"C:\Netro";


            ServiceID = "748598cb-e4d6-4b08-b380-d8690deac11c";
            ServiceIDIce = "";
            ServiceIDNonLog = "";
            ServiceIDWm = "6693685f-f106-40ee-8609-e996f56aed71";
            ServiceIDShout = "";
            ServiceIDWow = "95125068-50c2-45cb-aff3-5fa8f18839a6";

            use_timer_to_keep_alive_sql = true;
            keep_alive_timer_query_interval = 2;

            FTP_Folder = "";
            FTP_RunEachHours = 2;
            FTP_FilezillaServerPath = @"C:\Program Files(x86)\FileZilla Server";
            FTP_Debug = 0;

            WMS_serversToQuery = "";
            WMS_SecondsToWaitAfterACall = "30";

            FTP_ServiceID = "be6ef443-b6e7-4390-bdd7-e16bbfcf6510";
            WMS_ServiceID = "4adfd222-af19-42ed-be77-3ff805675326";

            FTP_Enabled = false;
            WMS_Enabled = false;


            //try
            //{
            //    public_ip_this_server = InternalServicesProxy.CallerIP();
            //}
            //catch
            //{
            //    public_ip_this_server = "";
            //}


            log_files_minimun_length = 50;

            Wowzacounters_Address = "http://localhost:8086/connectioncounts";
            Wowzacounters_User = "netroadmin";
            Wowzacounters_Pwd = "netroadmin";

            Portal_Public_Key = "BFBE7C55-FDDA-43C8-BE0D-3D4F5643BA8E";

            RTV_Service_Type = "WMS";
            RTV_Server = "tcp:cqfxnpw8jr.database.windows.net";
            RTV_Database = "NetroCounters";
            RTV_User = "NetroRTV";
            RTV_Password = "453t4nbh354ITHHUtt34t34ct3245ntmb";
            RTV_Encrypt = "Yes";


        }

        #endregion
    }
} 
