using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
//using System.Net;
//using System.Net.Mail;
using System.Diagnostics;
using System.Globalization;
using log4net;
using log4net.Config;
using System.IO;
using System.Windows.Forms;
/*
using NLog;
using NLog.Targets;
using NLog.Config;
*/


//using NLog.Win32.Targets;

namespace NetroMedia.SharedFunctions
{
    public  static class Netro_Log4
    {
        #region Fields
        static AppConfig _appConfig = ConfigManager.LoadFromFile();
        //static Logger logger;        
        static log4net.ILog logger;
        #endregion

        public const string ConfigFileName = "log4net.config";

        public static void Configure()
        {
            Type type = typeof(Netro_Log4);
            
            string path = Path.Combine(Application.StartupPath , ConfigFileName);
            FileInfo configFile = new FileInfo(path);
            XmlConfigurator.ConfigureAndWatch(configFile);
            logger = LogManager.GetLogger(type);
            //log.ToString();
        }


        static Netro_Log4() {

            Configure();

            try
            {

         
                /*

                // Step 1. Create configuration object 
                LoggingConfiguration config = new LoggingConfiguration();

                // Step 2. Create targets and add them to the configuration 
                //         ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
                //       config.AddTarget("console", consoleTarget);

                FileTarget fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);

                // Step 3. Set target properties 
                //consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
                fileTarget.FileName = "${basedir}/file.txt";
                fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";

                // Step 4. Define rules
                // LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
                // config.LoggingRules.Add(rule1);

                LoggingRule rule21 = new LoggingRule("*", LogLevel.Debug, fileTarget);
                LoggingRule rule22 = new LoggingRule("*", LogLevel.Error, fileTarget);
                LoggingRule rule23 = new LoggingRule("*", LogLevel.Fatal, fileTarget);
                LoggingRule rule24 = new LoggingRule("*", LogLevel.Info, fileTarget);
                LoggingRule rule25 = new LoggingRule("*", LogLevel.Trace, fileTarget);
                LoggingRule rule26 = new LoggingRule("*", LogLevel.Warn, fileTarget);

                config.LoggingRules.Add(rule21);
                config.LoggingRules.Add(rule22);
                config.LoggingRules.Add(rule23);
                config.LoggingRules.Add(rule24);
                config.LoggingRules.Add(rule25);
                config.LoggingRules.Add(rule26);

                // Step 5. Activate the configuration
                LogManager.Configuration = config;



                logger = LogManager.GetLogger("NetroLogger");


                */
            }
            catch { 

            }
        }

        private static string getConnectionStringCentral()
        {

            return  "data source=" + _appConfig.CENTRAL_DB_IP 
                    + ";initial catalog=" + _appConfig.CENTRAL_DB_Database 
                    + ";password=" + _appConfig.CENTRAL_DB_Pwd 
                    + ";persist security info=True;user id=" + _appConfig.CENTRAL_DB_User + ";";
                              
        }

        public static void writeServiceLog(ServiceToParse service, int InsertedRecords, string message)
        {

            

            try
            {

                SqlManager mgCentral = new SqlManager(getConnectionStringCentral());
                List<SqlParameter> parametersDashboard = new List<SqlParameter>();

                switch (service)
                {
                    case ServiceToParse.WinMedia:
                        parametersDashboard.Add(new SqlParameter("@serviceid", _appConfig.ServiceIDWm.ToString()));
                        break;
                    case ServiceToParse.QuickTime:
                        parametersDashboard.Add(new SqlParameter("@serviceid", _appConfig.ServiceID.ToString()));
                        break;
                    case ServiceToParse.Wowza:
                        parametersDashboard.Add(new SqlParameter("@serviceid", _appConfig.ServiceIDWow.ToString()));
                        break;
                    case ServiceToParse.FTP :
                        parametersDashboard.Add(new SqlParameter("@serviceid", _appConfig.FTP_ServiceID.ToString()));
                        break;
                    case ServiceToParse.WMSCounters :
                        parametersDashboard.Add(new SqlParameter("@serviceid", _appConfig.WMS_ServiceID.ToString()));
                        break;
                    case ServiceToParse.ServiceChecker  :
                        parametersDashboard.Add(new SqlParameter("@serviceid", "94f464c3-ca3e-4f91-a56c-5c33cfb6f003"));
                        break;
                           
            
                }

                parametersDashboard.Add(new SqlParameter("@servername", _appConfig.public_ip_this_server ));
                parametersDashboard.Add(new SqlParameter("@rows", InsertedRecords));
                parametersDashboard.Add(new SqlParameter("@lastMessage", message));

                mgCentral.ExecuteStoredProcedure("spNR_SAVE_DASHBOARD_INFO", parametersDashboard);

            }
            catch { }

            try {
            logger.Info (   string.Format( "Server [{0}] - App [{1}] - Rows [{2}] - Last Message [{3}] - ", 
                                    _appConfig.public_ip_this_server, service.ToString(), 
                                    InsertedRecords.ToString(),  message));
            }
            catch { }

    
        }


        public static ServiceToParse getServiceID(string service) {
            if ( service == _appConfig.ServiceIDWm){
                    return ServiceToParse.WinMedia;                    
            }
            if ( service == _appConfig.ServiceID.ToString() ){
                    return ServiceToParse.QuickTime;                     
            }
            if ( service ==  _appConfig.ServiceIDWow.ToString()){
                return ServiceToParse.Wowza;
            }
            if ( service ==  _appConfig.FTP_ServiceID.ToString() ){
                return ServiceToParse.FTP;                    
            }
            if (service == _appConfig.WMS_ServiceID.ToString())
            {
                return ServiceToParse.WMSCounters;
            }
            if ( service == "94f464c3-ca3e-4f91-a56c-5c33cfb6f003" )
            {
                return ServiceToParse.ServiceChecker ;
            }
            return ServiceToParse.WinMedia;   
        }

        public static void writeServiceLog(string service, string servername, int InsertedRecords, string message)
        {

            try
            {
                
                  logger.Info(string.Format("Server [{0}] - App [{1}] - Rows [{2}] - Last Message [{3}] - ",
                                        servername, service,                 
                                        InsertedRecords.ToString(), message));
                  
            }
            catch { }


            try
            {

            SqlManager mgCentral = new SqlManager(getConnectionStringCentral());
            List<SqlParameter> parametersDashboard = new List<SqlParameter>();

            parametersDashboard.Add(new SqlParameter("@serviceid", service ));
            parametersDashboard.Add(new SqlParameter("@servername", servername ));
            parametersDashboard.Add(new SqlParameter("@rows", InsertedRecords));
            parametersDashboard.Add(new SqlParameter("@lastMessage", message));

            mgCentral.ExecuteStoredProcedure("spNR_SAVE_DASHBOARD_INFO", parametersDashboard);

            }
            catch { }


        }

        public static void writeLocalServiceLog(string service, string servername, int InsertedRecords, string message)
        {

            try
            {
                
                logger.Info(string.Format("Server [{0}] - App [{1}] - Rows [{2}] - Last Message [{3}] - ",
                                        servername, service,
                                        InsertedRecords.ToString(), message));
                  
            }
            catch ( Exception ex ) { 
                int i = 1;
            }



        }

        public static void sendEmail(string service, int InsertedRecords, string message, string serverIP)
        {


            Emailer email = new Emailer();

            email.sendEmail( getServiceID( service), InsertedRecords, message, serverIP);
        }





        public static void sendEmail(ServiceToParse service, int InsertedRecords, string message)
        {


            Emailer email = new Emailer();

            email.sendEmail(service, InsertedRecords, message);

            /*
                try
                {

                    // create the email message
                    string[] toAd = _appConfig.email_to.Split(';');
                    MailMessage emailMessage = new MailMessage();
                    emailMessage.From = new MailAddress(_appConfig.email_from);
                    foreach (string toAddress in toAd)
                    {
                        emailMessage.To.Add(new MailAddress(toAddress));
                        //   _appConfig.email_to  
                    }

                    emailMessage.Subject = "A error occured with a service " + service.ToString();
                    emailMessage.Body = message;

                    // create smtp client at mail server location
                    SmtpClient client = new SmtpClient(_appConfig.email_smtp);
                    client.Port = 587;
                    client.EnableSsl = true;

                    client.Credentials = new NetworkCredential(_appConfig.email_user, _appConfig.email_pwd);

                    // send message
                    client.Send(emailMessage);


                }
                catch 
                {
                    
                }
            */
        }

        public static DataTable getDashboard()
        {

            

            try
            {

                SqlManager mgCentral = new SqlManager(getConnectionStringCentral());
                return mgCentral.LoadTable("select * from vwSERVICES_DASHBOARD");

            }
            catch
            {
                return new DataTable();
            }


        }



    }
}
