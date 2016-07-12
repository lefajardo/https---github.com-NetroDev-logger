using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using System.Diagnostics;
using NetroMedia.NetroFileZillaInterface;
using System.Data.SqlClient;
//using NetroMedia.SharedFunctions;
//using InternalServices.Proxy.InternalServices.Soap ;

namespace NetroMedia.SharedFunctions
{
    public class NetroDataManager2
    {

        private AppConfig _appConfig;
        private string WebPath = "";
        private string machineName = "";

        //EventLog Logger = new EventLog();

        public NetroDataManager2( string hostName) {
            machineName = hostName;
        }

        public NetroDataManager2(string WebServerPath, string hostName)
        {
            WebPath = WebServerPath;
            machineName = hostName;
        }
        /*

        public DataTable Server_Line_Items()
        {

            DataTable ret;

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);
            
            Soapzor soapsrv = new Soapzor();

            ret = soapsrv.ServerLineItems(new Guid(_appConfig.Portal_Public_Key), soapsrv.CallerIP());
            soapsrv = null;
            return ret;

        }
        */


        public void userAdd(string loginID, string password, string homeDir, string Quota_MB, string restartServer)
        {
            
            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            Filezilla FTPManager = new Filezilla(WebPath);

            
            //Logger.Source = "NetroFTPUserManager2";    

            //parameters.Add(new SqlParameter("@userID", userID));
            parameters.Add(new SqlParameter("@userName", loginID));
            parameters.Add(new SqlParameter("@userPassword", password));
            parameters.Add(new SqlParameter("@userHomeDir", homeDir));
            parameters.Add(new SqlParameter("@userQuota_MB", Quota_MB));
            parameters.Add(new SqlParameter("@server", machineName));
            

            try
            {

                mgr.ExecuteStoredProcedure("spFTP_AddNewUser", parameters);
                FTPManager.createUser(loginID, password, homeDir, restartServer);

                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "User created " + loginID);
                //Logger.WriteEntry("User created " + loginID, EventLogEntryType.Information);

            }
            catch (Exception ex) {


                if (_appConfig.send_debug_emails)
                {

                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error creating user [" + loginID + "] -> [" + ex.Message + "]");
                }
                    Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error creating user [" + loginID + "] -> [" + ex.Message + "]");


                    Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error creating user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error creating user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);
            
            }

            FTPManager = null;
            mgr = null;

        }

        public void updateQuota(string loginID, string Quota_MB)
        {

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            
            //Logger.Source = "NetroFTPUserManager2";   

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            parameters.Add(new SqlParameter("@userID", loginID));            
            parameters.Add(new SqlParameter("@userQuota_MB", Quota_MB));
            parameters.Add(new SqlParameter("@server", machineName));
        
            try
            {

                mgr.ExecuteStoredProcedure("spFTP_UpdateQuota", parameters);


            }
            catch (Exception ex)
            {

                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error updating quota [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error updating quota [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error updating assigned quota user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error updating assigned quota user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }

            mgr = null;


        }


        public void updatePwd(string loginID, string Password)
        {

            //string userName = UserNameByID(userID);

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            
            //Logger.Source = "NetroFTPUserManager2";   

            Filezilla FTPManager = new Filezilla(WebPath);

            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@userPassword", Password));
            parameters.Add(new SqlParameter("@server", machineName));            

            try
            {

                mgr.ExecuteStoredProcedure("spFTP_UpdatePwd", parameters);
                FTPManager.updatePwd(loginID, Password);

            }
            catch (Exception ex)
            {

                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error changing password  [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error changing password  [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error changing password user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error changing password user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }

            mgr = null;
            FTPManager = null;


        }


        public void userSuspendUploads(string loginID, string homedir, string pwd, AppConfig _appConfig)
        {

            //string userName = UserNameByID(userID);


            //Logger.WriteEntry("user suspend being start  ", EventLogEntryType.Warning  );

            //_appConfig = ConfigManager.LoadFromFile();

            //Logger.WriteEntry("user suspend being local [" + ConfigManager.getConnectionString(_appConfig) + "] ", EventLogEntryType.Warning  );

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            
            //Logger.Source = "NetroFTPManager";   

            Filezilla FTPManager = new Filezilla();

            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@server", machineName));            


            try
            {

                mgr.ExecuteStoredProcedure("spFTP_SuspendUser", parameters);
                FTPManager.UploadRights(loginID, false, _appConfig  , homedir, pwd);

            }
            catch (Exception ex)
            {

                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error suspending uploads  [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error suspending uploads  [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error updating assigned quota user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error suspending uploads user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }


            FTPManager = null;
            mgr = null;

        }

        public void userSuspendUploadsWeb(string loginID, string homedir, string pwd)
        {

            //string userName = UserNameByID(userID);

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            
            //Logger.Source = "NetroFTPManager";

            Filezilla FTPManager = new Filezilla(WebPath);

            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@server", machineName));


            try
            {

                mgr.ExecuteStoredProcedure("spFTP_SuspendUser", parameters);
                FTPManager.UploadRights(loginID, false,_appConfig , homedir, pwd);

            }
            catch (Exception ex)
            {

                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error suspending uploads  [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error suspending uploads  [" + loginID + "] -> [" + ex.Message + "]");

                //Logger.WriteEntry("Error suspending uploads user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);
                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error suspending uploads user [" + loginID + "] -> [" + ex.Message + "]");                

            }


            FTPManager = null;
            mgr = null;

        }


        public void userDrop(string loginID)
        {

            //string userName = UserNameByID(userID);
            string HomeDir = UserHomeDirectoryByID(loginID);

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            
            //Logger.Source = "NetroFTPUserManager2";   

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            Filezilla FTPManager = new Filezilla(WebPath);

            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@server", machineName));

            try
            {

                mgr.ExecuteStoredProcedure("spFTP_DropUser", parameters);
                FTPManager.deleteUser(loginID, HomeDir);
            }
            catch (Exception ex)
            {

                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error deleting user [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error deleting user [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error deleting user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error deleting user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }

            FTPManager = null;
            mgr = null;

        }

        public void restartServer() {
            Filezilla FTPManager = new Filezilla(WebPath);
            FTPManager.restartServer();
            FTPManager = null; 
        }

        public void restartServerLocal(AppConfig _appConfig)
        {
            Filezilla FTPManager = new Filezilla(WebPath);
            FTPManager.restartServerLocal(_appConfig);
            FTPManager = null;
        }

        public void userActivate(string loginID, bool local, string homedir, string pwd, AppConfig _appConfig)
        {

            //string userName = UserNameByID(userID);
            Filezilla FTPManager;
            //Logger.WriteEntry("user activate start ", EventLogEntryType.Warning);
            if (local)
            {

                //_appConfig = ConfigManager.LoadFromFile ();
                FTPManager = new Filezilla();
              //  Logger.WriteEntry("user activate being local [" + ConfigManager.getConnectionString(_appConfig) + "] ", EventLogEntryType.Warning  );
            }
            else
            {
                //_appConfig = ConfigManager.LoadFromFileWeb(WebPath);
                FTPManager = new Filezilla(WebPath);
                //Logger.WriteEntry("user activate being web [" + ConfigManager.getConnectionString(_appConfig) + "] ", EventLogEntryType.Warning );
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            
            //Logger.Source = "NetroFTPManager";


            

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));
            
             
            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@server", machineName));
            try
            {

                mgr.ExecuteStoredProcedure("spFTP_ActivateUser", parameters);
                FTPManager.UploadRights(loginID, true, _appConfig , homedir, pwd);

            }
            catch (Exception ex)
            {

                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error activating user [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error activating user [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error activating user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error activating user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }

            FTPManager = null;
            mgr = null;

        }

        public DataTable   LoadFTPUsers()
        {
            DataTable FtpUsers = new DataTable();

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            
            //Logger.Source = "NetroFTPUserManager2";   

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));            

            try
            {

                FtpUsers = mgr.LoadTable("Select * from vwEnabled_FTP_Users where server = '" + machineName  + "'");
                

            }
            catch { }

            mgr = null;

            return FtpUsers;


        }

        public DataTable LoadFTPUsersLocal()
        {
            DataTable FtpUsers = new DataTable();

            _appConfig = ConfigManager.LoadFromFile();

            
//            Logger.Source = "NetroFTPUserManager2";   

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            try
            {

                FtpUsers = mgr.LoadTable("Select * from vwEnabled_FTP_Users where server = '" + machineName + "'");


            }
            catch { }

            mgr = null;

            return FtpUsers;


        }


/*        public string UserNameByID(string loginID)
        {
            string userName = "";

            DataTable FtpUsers = new DataTable();

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));
    

            try
            {
                FtpUsers = mgr.LoadTable("Select * from vsFTP_Users where ID = '{" + loginID + "}'");
                if ( FtpUsers.Rows.Count > 0 ){
                userName = FtpUsers.Rows[0]["userLogin"].ToString();
                }
                
            }
            catch { }

            mgr = null;

            return userName;


        }
*/

        public string UserHomeDirectoryByID(string loginID)
        {
            string HomeDirectory = "";

            DataTable FtpUsers = new DataTable();

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            
            //Logger.Source = "NetroFTPUserManager2";   

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            try
            {
                FtpUsers = mgr.LoadTable("Select * from vwEnabled_FTP_Users where loginID = '{" + loginID + "}' and server = '" + machineName + "'");
                if (FtpUsers.Rows.Count > 0)
                {
                    HomeDirectory = FtpUsers.Rows[0]["HomeDir"].ToString();
                }

            }
            catch { }

            mgr = null;

            return HomeDirectory;


        }



        public void userUpdateQuotaUsed(string loginID, double currentQuota, int newStatus, int previousStatus, string homedir, string pwd)
        {


            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            List<SqlParameter> parameters = new List<SqlParameter>();

            
            //Logger.Source = "NetroFTPUserManager2";   

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@currentQuota", currentQuota  ));
            parameters.Add(new SqlParameter("@status", newStatus  ));
            parameters.Add(new SqlParameter("@server", machineName));

            try
            {

                    mgr.ExecuteStoredProcedure("spFTP_UpdateQuotaUsage_Status", parameters);

                    if ( newStatus == 0 && previousStatus == 1)
                    {
                        userActivate(loginID, true, homedir, pwd, _appConfig);
                    };

                    if ( newStatus == 1 && previousStatus == 0)
                    {
                        userSuspendUploads(loginID, homedir, pwd, _appConfig);
                    };
                    
                
            }
            catch (Exception ex)
            {


                if (_appConfig.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error updating current Quota user [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error updating current Quota user user [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Error updating current Quota user [" + loginID + "] -> [" + ex.Message + "]");
                //Logger.WriteEntry("Error updating current Quota user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }


            mgr = null;

            


        }



        public void userUpdateWithCRMData(string loginID, string password, string homeDir, string Quota_MB, string Stage,AppConfig _appConfig)
        {

                

            //_appConfig = ConfigManager.LoadFromFile();

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig));

            Filezilla FTPManager = new Filezilla(WebPath);

            
            //Logger.Source = "NetroFTPUserManager2";    

            //parameters.Add(new SqlParameter("@userID", userID));
            parameters.Add(new SqlParameter("@userName", loginID));
            parameters.Add(new SqlParameter("@userPassword", password));
            parameters.Add(new SqlParameter("@userHomeDir", homeDir));
            parameters.Add(new SqlParameter("@userQuota_MB", Quota_MB));
            parameters.Add(new SqlParameter("@server", machineName));
            parameters.Add(new SqlParameter("@stage", Stage));
            

            try
            {

                mgr.ExecuteStoredProcedure("spFTP_UpdateUserWithCRMData", parameters);

                Netro_Log4.writeLocalServiceLog("Data Manager", Environment.MachineName, 0, "User created " + loginID);                
                
                //Logger.WriteEntry("User created " + loginID, EventLogEntryType.Information);

            }
            catch (Exception ex) {


                if (_appConfig.send_debug_emails)
                {

                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error creating user [" + loginID + "] -> [" + ex.Message + "]");
                }
                    Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error creating user [" + loginID + "] -> [" + ex.Message + "]");


                    Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error creating user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error creating user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            
            }

            FTPManager = null;
            mgr = null;

        }

        public void userUpdateQuotaUsedLocal(string loginID, double currentQuota, int newStatus, int previousStatus, string homeDir , string pwd, AppConfig _appConfig2 )
        {


            //Logger.WriteEntry("user update quota local start  ", EventLogEntryType.Warning  );

            //_appConfig = ConfigManager.LoadFromFile();

            List<SqlParameter> parameters = new List<SqlParameter>();

            
            //Logger.Source = "NetroFTPManager";

            //Logger.WriteEntry("conn string [" + ConfigManager.getConnectionString(_appConfig2) + "] ", EventLogEntryType.Information );

            SqlManager mgr = new SqlManager(ConfigManager.getConnectionString(_appConfig2));

            parameters.Add(new SqlParameter("@userID", loginID));
            parameters.Add(new SqlParameter("@currentQuota", currentQuota));
            parameters.Add(new SqlParameter("@status", newStatus));
            parameters.Add(new SqlParameter("@server", machineName));

            try
            {

                mgr.ExecuteStoredProcedure("spFTP_UpdateQuotaUsage_Status", parameters);


                // 2010-09-07 Due that sometimes we try to update the Filezilla config file it
                // is being used by another process, now the user activation/deactivation will
                // be inforced in every run.
/*                
                if (newStatus == 0 && previousStatus == 1)
                {
                    userActivate(loginID, true);
                };

                if (newStatus == 1 && previousStatus == 0)
                {
                    userSuspendUploads(loginID);
                };
  */

                //Logger.WriteEntry("quota update succed, now activating/disabling account [" + ConfigManager.getConnectionString(_appConfig) + "] ", EventLogEntryType.Information );


                if (newStatus == 0 )
                {
                    userActivate(loginID, true, homeDir,pwd,_appConfig2 );
                };

                if (newStatus == 1 )
                {
                    userSuspendUploads(loginID, homeDir, pwd,_appConfig2 );
                };
                

            }
            catch (Exception ex)
            {

                if (_appConfig2.send_debug_emails)
                {
                    Netro_Log4.sendEmail(NetroMedia.ServiceToParse.FTP, 0, "*Error updating current Quota user [" + loginID + "] -> [" + ex.Message + "]");
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.FTP, 0, "*Error updating current Quota user user [" + loginID + "] -> [" + ex.Message + "]");

                Netro_Log4.writeLocalServiceLog("Data manager", Environment.MachineName, 0, "Error updating current Quota user [" + loginID + "] -> [" + ex.Message + "]");                
                //Logger.WriteEntry("Error updating current Quota user [" + loginID + "] -> [" + ex.Message + "]", EventLogEntryType.Error);

            }


            mgr = null;




        }



        
               





    }
}
