using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using NetroMedia.DataModule.Service;
//using NetroMedia.DataAdapter;
//using NetroMedia.SQL;
using System.Data.SqlClient;
//using NetroMedia.InternalServices.Proxy;
using NetroMedia.SharedFunctions;
namespace ConfigConsole
{
    class Program
    {
        static void Main(string[] args)
        {
/*            if (args.Length == 0)
            {
                showUsage();
            }
            else
            {*/
            processArgs(); //args);
            //}
        }

        static AppConfig5_Azure _appConfig;

        static void processArgs() //string[] args)
        {
             
            #region Fields

            
            string service = "";
            string logsPath = "";
            string savedLogsPath = "";
            string user = "netrostats";
            string pwd = "nstat32ho9t3t9on3";
            string db = "NetroLog";
            string instance = @".\SQLEXPRESS";
            string maskWms = "*.log";
            string maskWow = "*access.log.2*";
            string public_ip = "";

            try
            {

            #endregion
                _appConfig = new AppConfig5_Azure(); // ConfigManager.LoadFromFile();
/*
                foreach (string parm in args)
                {

                    string prm = parm.Substring(0, 2).ToLower();

                    switch (prm) 
                    {
                        case "-s":
                            service = parm.Substring(2).ToUpper();
                            break;
                        case "-l":
                            logsPath = parm.Substring(2).ToUpper();
                            break;
                        case "-p":
                            savedLogsPath = parm.Substring(2).ToUpper();
                            break;

                        case "-u":
                            user = parm.Substring(2).ToUpper();
                            break;

                        case "-c":
                            pwd = parm.Substring(2).ToUpper();
                            break;

                        case "-d":
                            db = parm.Substring(2).ToUpper();
                            break;

                        case "-i":
                            instance = parm.Substring(2).ToUpper();
                            break;

                        case "-m":
                            maskWms = parm.Substring(2).ToUpper();
                            maskWow = parm.Substring(2).ToUpper();
                            break;

                        case "-a":
                            public_ip = parm.Substring(2).ToUpper();                            
                            break;
                            

                    }

                }

                */


/*                _appConfig.CENTRAL_DB_IP = "174.129.246.39,1433";
                _appConfig.CENTRAL_DB_User = "netrostats";
                _appConfig.CENTRAL_DB_Pwd = "nstat32ho9t3t9on3";
                _appConfig.CENTRAL_DB_Database = "NETROLOG";
                */
                /*if (public_ip.Length == 0)
                {
                    Console.WriteLine("\n Please input the public IP for this server xxx.xxx.xxx.xxx \n:");
                    public_ip = Console.ReadLine();
                }*/

                //public_ip = InternalServicesProxy.CallerIP();
                //validate required fields -s & -l & -a
                /*
                if (service.Trim().Length == 0 || logsPath.Trim().Length == 0 || public_ip.Trim().Length == 0)
                {
                    showUsage();
                }
                else
                {
                    switch (service)
                    {
                        case "WMS":
                            _appConfig.DatabaseWmNonLog = db;
                            _appConfig.PasswordWmNonLog = pwd;
                            _appConfig.ServerIPWmNonLog = instance;
                            _appConfig.UserNameWmNonLog = user;
                            _appConfig.FolderWmNonLog = logsPath;
                            _appConfig.FileMaskWmNonLog = maskWms;
                            _appConfig.EnabledWm = true;
                            _appConfig.TableNameWm = "logactions";
                            _appConfig.FolderToMoveProcessedFilesWm = savedLogsPath;
                            if (savedLogsPath.Trim().Length > 0)
                            {
                                _appConfig.CanPurgeOldFilesWm = true;
                                _appConfig.PurgeFilesOlderThanWm = 30;
                            }
                            else
                            {
                                _appConfig.CanPurgeOldFilesWm = false;
                                _appConfig.PurgeFilesOlderThanWm = 0;
                            }

                            _appConfig.ProcessCurrentLogWm = false;
                            _appConfig.ServiceIDWm = "6693685f-f106-40ee-8609-e996f56aed71";

                            _appConfig.WMS_Enabled = true;



                            break;

                        case "WOWZA":

                            _appConfig.DatabaseWowNonLog = db;
                            _appConfig.PasswordWowNonLog = pwd;
                            _appConfig.ServerIPWowNonLog = instance;
                            _appConfig.UserNameWowNonLog = user;
                            _appConfig.FolderWowNonLog = logsPath;
                            _appConfig.FileMaskWowNonLog = maskWow;
                            _appConfig.EnabledWow = true;
                            _appConfig.TableNameWow = "logactions";
                            _appConfig.FolderToMoveProcessedFilesWow = savedLogsPath;
                            _appConfig.CanPurgeOldFilesWow = false;
                            _appConfig.PurgeFilesOlderThanWow = 0;
                            _appConfig.ProcessCurrentLogWow = true;
                            _appConfig.ServiceIDWow = "95125068-50c2-45cb-aff3-5fa8f18839a6";

                            break;

                        case "DSS":

                            _appConfig.Database = db;
                            _appConfig.Password = pwd;
                            _appConfig.ServerIP = instance;
                            _appConfig.UserName = user;
                            _appConfig.Folder = logsPath;
                            _appConfig.FileMask = maskWow;
                            _appConfig.Enabled = true;
                            _appConfig.TableName = "logactions";
                            _appConfig.FolderToMoveProcessedFiles = savedLogsPath;
                            _appConfig.CanPurgeOldFiles = false;
                            _appConfig.PurgeFilesOlderThan = 0;

                            _appConfig.ServiceID = "748598cb-e4d6-4b08-b380-d8690deac11c";

                            break;

                        case "FTP":
                            _appConfig.FTP_Enabled = true;
                            break;

                    }


                    _appConfig.public_ip_this_server = public_ip;
                    _appConfig.Portal_Public_Key = "BFBE7C55-FDDA-43C8-BE0D-3D4F5643BA8E";

                    _appConfig.use_timer_to_keep_alive_sql = true;
                    _appConfig.keep_alive_timer_query_interval = 3;

                    _appConfig.FTP_ServiceID = "be6ef443-b6e7-4390-bdd7-e16bbfcf6510";
                    _appConfig.WMS_ServiceID = "4adfd222-af19-42ed-be77-3ff805675326";


                    _appConfig.FTP_RunEachHours = 2;
                    _appConfig.FTP_FilezillaServerPath = @"C:\Program Files(x86)\FileZilla Server";

                    _appConfig.AzureBlobsKey = "DefaultEndpointsProtocol=https;AccountName=netrologs;AccountKey=Bi0MLDHWSs/xF8d2I8eoniVAF12XQDYKOVBOe5plu3wtq8D14tZswtDAtqpdy2VP2BAS1DB/Dt47mmS0SlYS/w==";
                      
                    _appConfig.AzureBlobsRoot = "http://netrologs.blob.core.windows.net/";
                    _appConfig.AzureBlobsBucket = "streaminglogs";

                    _appConfig.WMS_serversToQuery = "";
                    _appConfig.WMS_SecondsToWaitAfterACall = "30";

                    _appConfig.log_files_minimun_length = 50;
                */
                    ConfigManager.SaveToFile5 (_appConfig);

                  /*  setDashBoardState(_appConfig.ServiceID, _appConfig.Enabled);
                    setDashBoardState(_appConfig.ServiceIDWm, _appConfig.EnabledWm);
                    setDashBoardState(_appConfig.ServiceIDWow, _appConfig.EnabledWow);
                    setDashBoardState(_appConfig.FTP_ServiceID, _appConfig.FTP_Enabled  );
                    setDashBoardState(_appConfig.WMS_ServiceID, _appConfig.WMS_Enabled);
                
                }*/
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }


        static string getConnectionString_Central()
        {

            /*return "data source=" + _appConfig.CENTRAL_DB_IP
            + ";initial catalog=" + _appConfig.CENTRAL_DB_Database
            + ";password=" + _appConfig.CENTRAL_DB_Pwd
            + ";persist security info=True;user id=" + _appConfig.CENTRAL_DB_User + ";";
            */
            return "";
        }

        static void setDashBoardState(string serviceid, bool state)
        {
            /*
            ConfigManager.SaveToFile5(_appConfig);

            try
            {

                SqlManager sqlm = new SqlManager(getConnectionString_Central());

                List<SqlParameter> param = new List<SqlParameter>();

                param.Add(new SqlParameter("@serviceid", serviceid));
                param.Add(new SqlParameter("@servername", _appConfig.public_ip_this_server));
                param.Add(new SqlParameter("@state", state));

                sqlm.ExecuteStoredProcedure("spNR_SET_DASHBOARD_STATE", param);
            }
            catch { }
            */
        }

        static void showUsage() {

            Console.Clear();
            Console.WriteLine(" ");
            Console.WriteLine(" Netromedia (c) command line config tool");
            Console.WriteLine(" ");
            //Console.WriteLine("     Usage: ConfigConsole -sSERVICE -lLOGPATH  -pSAVELOGDIR -uUSER -cPASSWORD -dDATABASE -iINSTANCE -mMASK  -aPUBLIC_IP");
            Console.WriteLine("     Usage: ConfigConsole -sSERVICE -lLOGPATH  -pSAVELOGDIR -mMASK  -aPUBLIC_IP");
            Console.WriteLine(" ");
            Console.WriteLine(" Required parameters:  -s, -l"); //, -a ");
            Console.WriteLine(" ");
            Console.WriteLine(" Options:"); 
            Console.WriteLine(" ");            
            Console.WriteLine(" SERVICE: WMS/WOWZA/DSS/FTP");
            Console.WriteLine(" ");
        /*    Console.WriteLine(@" INSTANCE: Default = .\SQLEXPRESS");
            Console.WriteLine(" ");
            Console.WriteLine(" USER: Default = netrostats");
            Console.WriteLine(" ");
            Console.WriteLine(" DATABASE: Default = NetroLog");
            Console.WriteLine(" ");*/
            Console.WriteLine(" MASK: Default = WMS[*.log]");
            Console.WriteLine("                 WOWZA[*access.log.2*]");

            

        }
    }
}
