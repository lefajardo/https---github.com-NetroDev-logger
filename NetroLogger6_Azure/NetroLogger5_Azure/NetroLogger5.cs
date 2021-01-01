using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Configuration;
using NetroAzureUpload;
using NetroMedia.SharedFunctions;
using System.Threading;
using System.Net;
using System.Reflection;
using NetroMedia.InternalServices.Proxy;

namespace NetroLogger5_Azure 
{

    class serviceData {
        public string path = "";
        public string filter = "";
        public string logstype = "";
        public string processCurFile = "N";
    }


    public partial class NetroLogger5 : ServiceBase
    {
        static AppConfig5_Azure _appConfig;
        string[] services;

        public NetroLogger5()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                InitializeWatcher();
            }
            catch (Exception ex ){
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Error initializing. Err[{0}]", ex.Message));
            }
        }

        protected override void OnStop()
        {
        }

        protected void setValues() {

            /*

            if (_appConfig.EnabledWow) {
                path1 = _appConfig.FolderWowNonLog;
                filter1 = _appConfig.FileMaskWowNonLog;
                logstype1 = "WZM";
                processCurFile = _appConfig.ProcessCurrentLogWow ? "Y": "N";
            }
            if (_appConfig.EnabledWm )
            {
                path1 = _appConfig.FolderWmNonLog ;
                filter1 = _appConfig.FileMaskWmNonLog ;
                logstype1 = "WMS";
                processCurFile = _appConfig.ProcessCurrentLogWm ? "Y" : "N";
            }
            if (_appConfig.Enabled )
            {
                path1 = _appConfig.Folder ;
                filter1 = _appConfig.FileMask;
                logstype1 = "DSS";
                processCurFile = _appConfig.ProcessCurrentLog ? "Y" : "N";
            }
             */ 
        }

        System.Threading.Timer oTimerDaily;
        System.Threading.Timer oTimerReach15Min;
        int minuteToFire = 30;


        void VersionChecker()
        {
            int currentMinute = DateTime.Now.Minute;

            if (currentMinute < minuteToFire)
            {
                currentMinute = minuteToFire - currentMinute;
            }
            else
            {
                currentMinute = 60 + minuteToFire - currentMinute;
            }

            //for test purpose
            currentMinute = 1;

            TimerCallback oCallback15 = new TimerCallback(timer_Tick15);
            oTimerReach15Min = new Timer(oCallback15, null,
                currentMinute * 60 * 1000,
                currentMinute * 60 * 1000);

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Netro_Log4.writeLocalServiceLog( "NetroLogger5", Environment.MachineName , 0, "Version: " + version.ToString().Trim());
            
                

        }

        void timer_Tick15(Object stateInfo)
        {
            int minutesPerHour = 25; // 60;

            try
            {
                oTimerReach15Min.Dispose();
                oTimerReach15Min = null;
            }
            catch { }

            TimerCallback oCallbackStart = new TimerCallback(waitFor15Min_tick);
            oTimerDaily = new Timer(oCallbackStart, null, 1000 * 60 * minutesPerHour, 1000 * 60 * minutesPerHour);
            
        }

        void waitFor15Min_tick(Object stateInfo)
        {
            checkforNewLoggerVersion();
        }

        void checkforNewLoggerVersion() {


            try
            {

                string server = _appConfig.UpdatesServer;

                WebClient webClient = new WebClient();
                MemoryStream versiondata = new MemoryStream(webClient.DownloadData(string.Format("{0}/loggerversion.txt", server)));
                webClient.Dispose();

                versiondata.Position = 0;
                StreamReader sr = new StreamReader(versiondata);
                string loggerVersion = sr.ReadToEnd();

                var version = Assembly.GetExecutingAssembly().GetName().Version;


                if (loggerVersion.Trim().CompareTo(version.ToString().Trim()) > 0)
                {
                    //get the new logger, unpack and replace

                    webClient = new WebClient();
                    webClient.DownloadFile(string.Format("{0}/loggerfiles.zip", server), @".\loggerfiles.zip");
                    webClient.Dispose();

                    string parameters = String.Format("/k \"{0}\"", "updatelogger.bat");
                    System.Diagnostics.Process.Start("cmd", parameters);

                }
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Error in check for new version. Err[{0}]", ex.Message));
            }

        }


        void InitializeWatcher()
        {
  
//            VersionChecker();

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Loading config file");
            _appConfig = ConfigManager.LoadFromFile5();
            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Getting IP");
            try
            {
                _appConfig.ServerPublicIP = InternalServicesProxy.CallerIP();
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error Calling soapzor " + ex.Message);
            }
            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "IP for this server " + _appConfig.ServerPublicIP);

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "key = " + _appConfig.Portal_Public_Key);



            //Lester Dec. 24,2020, the webservice is no longer working to receive the services that are enabled for this server. Will read from the config file

            if (string.IsNullOrEmpty(_appConfig.Service2))
            {
                services = new string[] { _appConfig.Service1 };
                InitializeWatcher1(_appConfig.Service1, 1);
            }
            else
            {
                services = new string[] { _appConfig.Service1, _appConfig.Service2 };
                InitializeWatcher1(_appConfig.Service1, 1);
                InitializeWatcher1(_appConfig.Service2, 2);
            }
            

            /*Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Services " + servicesd);
            string servicesd = InternalServicesProxy.Services(_appConfig.Portal_Public_Key);
            services = servicesd.Split(new char[] { '/' });

            if (services.Length > 1)
            {
                InitializeWatcher1(services[0], 1);
                InitializeWatcher1(services[1], 2);
            }
            else
            {
                InitializeWatcher1(services[0], 1);
            }*/

        }

        serviceData getWatcherData(string serviceid)
        {
            serviceData data = new serviceData();
            switch (serviceid) { 
                case "Windows Media":
                    data.filter = _appConfig.LogsPattern_Wms;
                    data.path = _appConfig.LogsPath_Wms;
                    data.processCurFile = _appConfig.LogsProcessCurrentFile_Wms;
                     
                    break;
                case "Wowza":
                    data.filter = _appConfig.LogsPattern_Wowza;
                    data.path = _appConfig.LogsPath_Wowza;
                    data.processCurFile = _appConfig.LogsProcessCurrentFile_Wowza;
                    break;
                case "DSS":
                    data.filter = _appConfig.LogsPattern_Dss;
                    data.path = _appConfig.LogsPath_Dss;
                    data.processCurFile = _appConfig.LogsProcessCurrentFile_Dss ;
                    break;
            }
            return data;
        }

        void InitializeWatcher1(string serviceid, int watcher)
        {

            if (string.IsNullOrEmpty(serviceid)) return;

            if (watcher == 1)
            {
                try
                {
                    serviceData watcherdata = getWatcherData(serviceid);

                    NewFiles.BeginInit();

                    NewFiles.Path = watcherdata.path;
                    NewFiles.Filter = watcherdata.filter;
                    NewFiles.NotifyFilter = (NotifyFilters.FileName); // | NotifyFilters.LastAccess | NotifyFilters.LastWrite);

                    if (serviceid == "Wowza" || serviceid == "DSS")
                    {
                        NewFiles.Renamed += new RenamedEventHandler(NewFiles_Renamed1);
                    }
                    else
                    {
                        if (serviceid == "Windows Media")
                        {
                            NewFiles.Created += new FileSystemEventHandler(NewFiles_Created1);
                        }
                    }

                    // Begin watching.
                    NewFiles.EnableRaisingEvents = true;


                    NewFiles.EndInit();
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Service Started "+serviceid);
                }
                catch (Exception ex)
                {
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Error on service started " + ex.Message);
                }
            }
            if (watcher == 2)
            {
                try
                {
                    serviceData watcherdata = getWatcherData(serviceid);

                    NewFiles2.BeginInit();

                    NewFiles2.Path = watcherdata.path;
                    NewFiles2.Filter = watcherdata.filter;
                    NewFiles2.NotifyFilter = (NotifyFilters.FileName); // | NotifyFilters.LastAccess | NotifyFilters.LastWrite);

                    if (serviceid == "Wowza" || serviceid == "DSS")
                    {
                        NewFiles2.Renamed += new RenamedEventHandler(NewFiles_Renamed2);
                    }
                    else
                    {
                        if (serviceid == "Windows Media")
                        {
                            NewFiles2.Created += new FileSystemEventHandler(NewFiles_Created2);
                        }
                    }

                    // Begin watching.
                    NewFiles2.EnableRaisingEvents = true;


                    NewFiles2.EndInit();
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Service Started");
                }
                catch (Exception ex)
                {
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Error on service started " + ex.Message);
                }
            }

        }
                    /*
                    
                     * NewFiles.BeginInit();

                    NewFiles.Path = Properties.Settings.Default.LogsPath1;
                    NewFiles.Filter = Properties.Settings.Default.LogsPattern1;
                    NewFiles.NotifyFilter = (NotifyFilters.FileName); // | NotifyFilters.LastAccess | NotifyFilters.LastWrite);

                    // Add event handlers.
                    if (Properties.Settings.Default.LogsType1 == "WZM" || Properties.Settings.Default.LogsType1 == "DSS")
                    {
                        NewFiles.Renamed += new RenamedEventHandler(NewFiles_Renamed);
                    }
                    else
                    {
                        if (Properties.Settings.Default.LogsType1 == "WMS")
                        {
                            NewFiles.Created += new FileSystemEventHandler(NewFiles_Created);
                        }
                    }
                    
                    // Begin watching.
                    NewFiles.EnableRaisingEvents = true;


                    NewFiles.EndInit();
                    */

                    /*
                    if (Properties.Settings.Default.Logs2Enabled)
                    {

                        NewFiles2.BeginInit();
                        NewFiles2.Path = Properties.Settings.Default.LogsPath2;
                        NewFiles2.Filter = Properties.Settings.Default.LogsPattern2;
                        NewFiles2.NotifyFilter = (NotifyFilters.FileName);

                        switch (Properties.Settings.Default.LogsType2)
                        {
                            case "WMS":
                                NewFiles2.Created += new FileSystemEventHandler(NewFiles_Created2);
                                break;
                            case "WZM":
                                NewFiles2.Renamed += new RenamedEventHandler(NewFiles_Renamed2);
                                break;
                            case "DSS":
                                NewFiles2.Renamed += new RenamedEventHandler(NewFiles_Renamed2);
                                break;
                            case "IIS":
                                NewFiles2.Renamed += new RenamedEventHandler(NewFiles_Renamed2);
                                break;
                        }

                        // Begin watching.
                        NewFiles2.EnableRaisingEvents = true;
                        NewFiles2.EndInit();

                    }

                    */


             
        

        bool fileAlreadyUploaded(string filename)
        {
            bool ret = false;
            return ret;
        }

        void NewFiles_Created1(object sender, FileSystemEventArgs e)
        {
            //string[] services
            //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Event[New file created]" + e.FullPath );
            try
            {
                serviceData watcherdata = getWatcherData(services[0]);

                UploadFiles upl = new UploadFiles();
                upl.upload(watcherdata.path , watcherdata.filter , e.FullPath,
                    _appConfig.AzureStorage_BlobsKey , _appConfig.AzureStorage_BlobsRoot ,
                    _appConfig.AzureStorage_BlobsBucket , _appConfig.ServerPublicIP,
                    watcherdata.processCurFile , getConnectionString(),
                    services[0]);
                upl = null;
                moveFilesUploaded(watcherdata.path , watcherdata.filter ,
                    services[0], _appConfig.ServerPublicIP ,
                    getConnectionString());

                /*
                    upl.upload(Properties.Settings.Default.LogsPath1, Properties.Settings.Default.LogsPattern1, e.FullPath,
                    Properties.Settings.Default.NetroAzureKey, Properties.Settings.Default.BlobRoot,
                    Properties.Settings.Default.AzureLogsRoot, Properties.Settings.Default.PublicIPThisServer,
                    Properties.Settings.Default.ProcessCurrentFile1, getConnectionString(),
                    Properties.Settings.Default.LogsType1);
                upl = null;
                moveFilesUploaded(Properties.Settings.Default.LogsPath1, Properties.Settings.Default.LogsPattern1,
                    Properties.Settings.Default.LogsType1, Properties.Settings.Default.PublicIPThisServer,
                    getConnectionString());

                 */
            }
            catch (Exception ex) {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format( "Error in new files created. Err[{0}]" ,ex.Message ));
            }
        }

        void NewFiles_Created2(object sender, FileSystemEventArgs e)
        {
            //string[] services
            //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Event[New file created]" + e.FullPath );
            try
            {
                serviceData watcherdata = getWatcherData(services[1]);

                UploadFiles upl = new UploadFiles();
                upl.upload(watcherdata.path, watcherdata.filter, e.FullPath,
                    _appConfig.AzureStorage_BlobsKey, _appConfig.AzureStorage_BlobsRoot,
                    _appConfig.AzureStorage_BlobsBucket, _appConfig.ServerPublicIP,
                    watcherdata.processCurFile, getConnectionString(),
                    services[1]);
                upl = null;
                moveFilesUploaded(watcherdata.path, watcherdata.filter,
                    services[1], _appConfig.ServerPublicIP,
                    getConnectionString());

            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Error in new files created. Err[{0}]", ex.Message));
            }
        }
        void moveFilesUploaded(string path, string filesPattern, string filetype, string serverIp, string connString)
        {

            string uploadpath = path + "\\uploaded";
                DirectoryInfo directoryContentsU = new DirectoryInfo(uploadpath );
                if (!directoryContentsU.Exists )
                {
                    //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("directory dont exists. creating "));
                    try
                    {
                        directoryContentsU.Create();
                    }
                    catch ( Exception ex ) {
                        Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Error in uploaded dir create " + ex.Message ));
                    }
                }

                DirectoryInfo directoryContents = new DirectoryInfo(path);
                UploadFiles upl = new UploadFiles( _appConfig.AzureStorage_BlobsKey,
                    _appConfig.AzureStorage_BlobsRoot , _appConfig.AzureStorage_BlobsBucket );

                foreach (FileInfo fileCurrent in directoryContents.GetFiles(filesPattern))
                {

                        //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("start to evaluate " + fileCurrent.FullName  ));



                    if ( fileCurrent.CreationTimeUtc <= DateTime.UtcNow.AddDays(-2) )
                    {

                        //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("not same day " + fileCurrent.FullName));
                        string md5 = upl.GetMD5HashFromFile(fileCurrent.FullName);
                        //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("got hash " + fileCurrent.FullName));
                        if (upl.fileAlreadyProcessed(filetype, fileCurrent.FullName, serverIp, md5, _appConfig.AzureStorage_BlobsKey, 
                            Environment.MachineName))
                        {
                            //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Move file to uploaded folder " + fileCurrent.FullName));
                            try {
                                string newfile = uploadpath +  "\\" + fileCurrent.Name.ToLower();
                                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Move to " + newfile ));
                                fileCurrent.MoveTo(newfile);   
                            }
                            catch (Exception ex)
                            {
                                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Error in move " + ex.Message));
                            }
                        }
                    }
                }
        }

        
        void NewFiles_Renamed1(object sender, RenamedEventArgs e)
        {

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Service Event[File renamed]" + e.FullPath);
            try{
                UploadFiles upl = new UploadFiles();
                serviceData watcherdata = getWatcherData(services[0]);
                upl.upload(watcherdata.path, watcherdata.filter, e.FullPath,
                                    _appConfig.AzureStorage_BlobsKey, _appConfig.AzureStorage_BlobsRoot,
                                    _appConfig.AzureStorage_BlobsBucket, _appConfig.ServerPublicIP,
                                    watcherdata.processCurFile, getConnectionString(),
                                    services[0]);
                upl = null;
                moveFilesUploaded(watcherdata.path, watcherdata.filter,
                    services[0], _appConfig.ServerPublicIP,
                    getConnectionString());
/*
                upl.upload(Properties.Settings.Default.LogsPath1, Properties.Settings.Default.LogsPattern1, e.FullPath, 
                    Properties.Settings.Default.NetroAzureKey, Properties.Settings.Default.BlobRoot, 
                    Properties.Settings.Default.AzureLogsRoot, Properties.Settings.Default.PublicIPThisServer, 
                    Properties.Settings.Default.ProcessCurrentFile1, getConnectionString(), 
                    Properties.Settings.Default.LogsType1 );
                upl = null;
                //moveFilesUploaded(path1, filter1);
                //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Start to move Uploaded fukes . "));
                moveFilesUploaded(Properties.Settings.Default.LogsPath1, Properties.Settings.Default.LogsPattern1,
                    Properties.Settings.Default.LogsType1, Properties.Settings.Default.PublicIPThisServer,
                    getConnectionString() );
 * */
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Start Upload. Err[{0}]", ex.Message));
            }
            
        }

        void NewFiles_Renamed2(object sender, RenamedEventArgs e)
        {

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Service Event[File renamed]" + e.FullPath);
            try
            {
                UploadFiles upl = new UploadFiles();
                serviceData watcherdata = getWatcherData(services[1]);
                upl.upload(watcherdata.path, watcherdata.filter, e.FullPath,
                                    _appConfig.AzureStorage_BlobsKey, _appConfig.AzureStorage_BlobsRoot,
                                    _appConfig.AzureStorage_BlobsBucket, _appConfig.ServerPublicIP,
                                    watcherdata.processCurFile, getConnectionString(),
                                    services[1]);
                upl = null;
                moveFilesUploaded(watcherdata.path, watcherdata.filter,
                    services[1], _appConfig.ServerPublicIP,
                    getConnectionString());                
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Start Upload for renamed file. Err[{0}]", ex.Message));
            }

        }


/*
        void NewFiles_Created2(object sender, FileSystemEventArgs e)
        {
            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Event[New file created2]" + e.FullPath);
            try
            {
                UploadFiles upl = new UploadFiles();
                upl.upload(Properties.Settings.Default.LogsPath2, Properties.Settings.Default.LogsPattern2, e.FullPath, Properties.Settings.Default.NetroAzureKey, Properties.Settings.Default.BlobRoot, Properties.Settings.Default.AzureLogsRoot, Properties.Settings.Default.PublicIPThisServer, Properties.Settings.Default.ProcessCurrentFile1, getConnectionString(), Properties.Settings.Default.LogsType2);
                upl = null;
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Start Upload 2. Err[{0}]", ex.Message));
            }
        }

        void NewFiles_Renamed2(object sender, RenamedEventArgs e)
        {

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Netrologger5 Azure Service Event[File renamed]" + e.FullPath);
            try
            {
                UploadFiles upl = new UploadFiles();
                upl.upload(Properties.Settings.Default.LogsPath2, Properties.Settings.Default.LogsPattern2, e.FullPath, Properties.Settings.Default.NetroAzureKey, Properties.Settings.Default.BlobRoot, Properties.Settings.Default.AzureLogsRoot, Properties.Settings.Default.PublicIPThisServer, Properties.Settings.Default.ProcessCurrentFile1, getConnectionString(), Properties.Settings.Default.LogsType2);
                upl = null;
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("Start Upload. Err[{0}]", ex.Message));
            }

        }
        */
        
        public static string getConnectionString()
        {

            return "data source=" + _appConfig.AzureSql_Server
                + ";initial catalog=" + _appConfig.AzureSql_Database
                    + ";password=" + _appConfig.AzureSql_Password 
                    + ";user id=" + _appConfig.AzureSql_User  + ";Encrypt=yes;";
            /*
            return "data source=" + Properties.Settings.Default.NetroAzureServer 
                + ";initial catalog=" + Properties.Settings.Default.NetroAzureDBName
                    + ";password=" + Properties.Settings.Default.NetroAzureDBPwd
                    + ";user id=" + Properties.Settings.Default.NetroAzureDBUser + ";Encrypt=yes;";
            */
        }



    }
}
