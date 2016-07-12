using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
//using NetroMedia.InternalServices.Proxy;

namespace NetroMedia.SharedFunctions
{
    public class ConfigManager
    {
        #region Fields

        private static string _configFileName = "mainconfig.config";
        private static string _configFileName5 = "mainconfig5.config";

        #endregion

        #region Properties

        public static string ConfigFileName
        {
            get { return _configFileName; }
        }

        public static string ConfigFileName5
        {
            get { return _configFileName5; }
        }

        #endregion

        #region Public methods

        public static string getConnectionString(AppConfig cnf)
        {

            return "data source=" + cnf.CENTRAL_DB_IP 
                    + ";initial catalog=" + cnf.CENTRAL_DB_Database 
                    + ";password=" + cnf.CENTRAL_DB_Pwd 
                    + ";persist security info=True;user id=" + cnf.CENTRAL_DB_User  + ";";

        }

        public static AppConfig LoadFromFile()
        {
            return LoadFromFile(Application.StartupPath + "\\" + _configFileName);
        }
        public static AppConfig5_Azure LoadFromFile5()
        {
            return LoadFromFile5(Application.StartupPath + "\\" + _configFileName5);
        }

        public static void CopyConfigFile( string destinationFolder)
        {
            FileInfo configFile = new FileInfo( Application.StartupPath + "\\" + _configFileName);

            try
            {

                configFile.CopyTo(destinationFolder + "\\" + _configFileName);
            }
            catch { }

            
        }

        public static AppConfig LoadFromFileWeb(string Path)
        {
            return LoadFromFile(Path + "\\" + _configFileName);
        }

        public static void SaveToFile(AppConfig cnf)
        {
            SaveToFile(Application.StartupPath + "\\" + _configFileName, cnf);
        }

        public static void SaveToFile5(AppConfig5_Azure cnf)
        {
            SaveToFile(Application.StartupPath + "\\" + _configFileName, cnf);
        }

        public static AppConfig LoadFromFile(string fileName)
        {

            /*
            if (!File.Exists(fileName))
            {
                // return new AppConfig();
                Exception myException = new Exception("Config file not found "+fileName );

                // set the HelpLink and Source properties
                myException.HelpLink = "load from file";
                myException.Source = "load from file";

                // throw the Exception object
                throw myException;

               
            }
            */
            try
            {
                FileStream flStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite  );

                XmlSerializer ser = new XmlSerializer(typeof(AppConfig));
                AppConfig cnf = (AppConfig)ser.Deserialize(flStream);

                flStream.Close();

                //try
                //{
                //    cnf.public_ip_this_server = InternalServicesProxy.CallerIP();
                //}
                //catch { }

                return cnf;
            }
            catch {
                Netro_Log4.writeLocalServiceLog("Load Config", Environment.MachineName, 0, "Cant open config file " + fileName );
            }

            return new AppConfig();
        }

        public static AppConfig5_Azure LoadFromFile5(string fileName)
        {

            
            try
            {
                FileStream flStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XmlSerializer ser = new XmlSerializer(typeof(AppConfig5_Azure));
                AppConfig5_Azure cnf = (AppConfig5_Azure)ser.Deserialize(flStream);

                flStream.Close();
                
                return cnf;
            }
            catch
            {
                Netro_Log4.writeLocalServiceLog("Load Config", Environment.MachineName, 0, "Cant open config file " + fileName);
            }

            return new AppConfig5_Azure();
        }


        public static void SaveToFile(string fileName, AppConfig5_Azure cnf)
        {
            try
            {
                FileStream flStream = new FileStream(fileName, FileMode.Create);

                XmlSerializer ser = new XmlSerializer(typeof(AppConfig5_Azure));

                ser.Serialize(flStream, cnf);

                flStream.Close();
            }
            catch {
                Netro_Log4.writeLocalServiceLog("Save Config", Environment.MachineName, 0, "Cant save config file " + fileName );
            }
        }


        public static void SaveToFile(string fileName, AppConfig cnf)
        {
            try
            {
                //try
                //{
                //    cnf.public_ip_this_server = InternalServicesProxy.CallerIP();
                //}
                //catch { }

                FileStream flStream = new FileStream(fileName, FileMode.Create);

                XmlSerializer ser = new XmlSerializer(typeof(AppConfig));

                ser.Serialize(flStream, cnf);

                flStream.Close();
            }
            catch {
                Netro_Log4.writeLocalServiceLog("Save Config", Environment.MachineName, 0, "Cant save config file " + fileName );
            }
        }

        #endregion
    }
}
