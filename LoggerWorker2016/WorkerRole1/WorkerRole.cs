using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

using System.Globalization;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;


using AzureLogs;
//using AzureStorageTable;

using NetroLogger_WorkerRole;

namespace NetroLoggerWorker
{
    public class WorkerRole : RoleEntryPoint
    {

        int iMaxThreads = 4;//32//16;//5;

        static int minuteToFire = 15;

        static string logsContainerName = "netrologs5";
        static string blobRootStr = "";
        static System.Threading.Timer oTimerStart;
        static System.Threading.Timer oTimerReach15Min;
        static System.Threading.Timer oTimerHourly;

        static LookupService ls = new LookupService();
        static bool inProcess = false;

        //2012/02/06 Lester Add a new thread to read logs for azure native logs.

        static CloudStorageAccount storageAccount = null;
        static CloudBlobContainer container = null;
        static CloudQueue queue = null;
        static CloudQueueClient queueStorage = null;
        static CloudBlobClient blobStorage = null;


        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            /*    Trace.TraceInformation("WorkerRole1 is running");

                try
                {
                    this.RunAsync(this.cancellationTokenSource.Token).Wait();
                }
                finally
                {
                    this.runCompleteEvent.Set();
                }*/


            blobRootStr = RoleEnvironment.GetConfigurationSettingValue("BlobRootString");
            //storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

            blobStorage = storageAccount.CreateCloudBlobClient();
            container = blobStorage.GetContainerReference(logsContainerName);
            queueStorage = storageAccount.CreateCloudQueueClient();
            queue = queueStorage.GetQueueReference("netrologsqueue5");
            //trace.TraceInformation("Creating container and queue...");
            bool containerAndQueueCreated = false;
            while (!containerAndQueueCreated)
            {
                try
                {
                    container.CreateIfNotExists();
                    var permissions = container.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    container.SetPermissions(permissions);
                    permissions = container.GetPermissions();
                    queue.CreateIfNotExists();
                    containerAndQueueCreated = true;
                }
                catch (StorageException e)
                {
                    /* if (e.ErrorCode == StorageException.TransportError)
                     {
                         Trace.TraceError(string.Format("Connect failure! The most likely reason is that the local " +
                             "Development Storage tool is not running or your storage account configuration is incorrect. " +
                             "Message: '{0}'", e.Message));
                         System.Threading.Thread.Sleep(5000);
                     }
                     else
                     {
                         throw;
                     }*/

                    throw;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(string.Format("Connect failure! The most likely reason is that the local " +
                            "Development Storage tool is not running or your storage account configuration is incorrect. " +
                            "Message: '{0}'", ex.Message));
                }
            }


            //queue.RetrieveApproximateMessageCount();
            //trace.TraceInformation("Aprox msg count  =" + queue.ApproximateMessageCount.ToString());

            ThreadInfo_ tiThreadInfo = new ThreadInfo_();
            Trace.TraceInformation("Starting Threads...Total =" + iMaxThreads.ToString());
            

            //this other thread will generate rollups for the logs processed above
            //the logic is:  the thread will wait 15 minutes, so it always run
            // at 1.15, 2.15, 11.15, etc, so they will always see the latets processed
            // logs 
            // also it will recalculate past rollups if some new record is found

           reprocessoldBlobs();

            /*
            try
            {
                startRollUpThread();
            }
            catch { }
            */


            // one thread to look for lob blobs left without process and that the 
            // queue was expired ?



            //this bunch of code start the log processor threads for the wowza, wms and darwin log files
            //coming in a queue 
            /*
            for (int iThreadNumber = 0; iThreadNumber < iMaxThreads; iThreadNumber++)
            {
                Trace.TraceInformation("Starting Thread =" + iThreadNumber.ToString());
                ParameterizedThreadStart serverThread = new ParameterizedThreadStart(ThreadWorkerNetroLogs);
                Thread Thread = new Thread(serverThread);
                tiThreadInfo.queue = queue;
                tiThreadInfo.blobStorage = blobStorage;
                tiThreadInfo.ThreadData = iThreadNumber.ToString();
                tiThreadInfo.ls = ls;
                Thread.Start(tiThreadInfo);
            }
            */

            //process azure logs
            // startAzureLogsReaderThread();

            // wait until infite to let the threads work
            System.Threading.Thread.Sleep(Timeout.Infinite);


        }


        #region processoldBlobs
        static void reprocessoldBlobs()
        {
            int minutesPerHour = 60;
            reprocessoldBlobs2(ls);
            TimerCallback oCallbackStart = new TimerCallback(waitFor15Min_tickBlob);
            oTimerHourly = new Timer(oCallbackStart, null, 1000 * 60 * minutesPerHour, 1000 * 60 * minutesPerHour);
            
        }

        static void waitFor15Min_tickBlob(Object stateInfo)
        {
            reprocessoldBlobs2(ls);
        }
        
        static void reprocessoldBlobs2(LookupService ls)
        { 
            if (inProcess) return;

            inProcess = true;

            BlobRequestOptions options = new BlobRequestOptions();
            

            //options.UseFlatBlobListing = true;

            try
            {
                

                foreach (CloudBlockBlob blobItem in container.ListBlobs(null, true, BlobListingDetails.None))
                {
                   // if (blobItem.Uri.ToString().IndexOf("WZM154") < 0) continue;
                    try
                    {
                        // if the blob is there after 7 days means something happened to its queue pair
                        //if ((DateTime.Now - blobItem.Properties.LastModifiedUtc).TotalMinutes > (60 * 24 * 1))
                        //Console.WriteLine(blobItem.Uri);
                        //{
                        blobItem.FetchAttributes();
                        string shortBlobname = stripAzureRoot(blobItem.Uri.ToString());
                        shortBlobname = stripAzureRoot(shortBlobname);

                        //Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] msg blobname = " + Sql.ToString(blobname.Trim()));
                        CloudBlobContainer cbcontainer = blobStorage.GetContainerReference("netrologs5");
                        CloudBlockBlob blob = cbcontainer.GetBlockBlobReference(shortBlobname);

                        if (blobItem.Metadata["BATCHID"] == "0")
                        {
                            //if (blobmanagement.processBlob(blobItem, getServerName(shortBlobname), blobItem.Uri.ToString(), ls, storageAccount))
                                if (blobmanagement.processBlob(blob, getServerName(shortBlobname), blobItem.Uri.ToString(), ls, storageAccount))
                                {
                                    
                                    //Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] deleting queue and moving to procssed ");
                                    blobmanagement.moveblobtoProcessed(blob, shortBlobname, blobStorage, true);
                                    //blobmanagement.moveblobtoProcessed(blobItem, shortBlobname, blobStorage, true);
                            }
                        }
                        /*else
                        {
                            //for any reason the blob is there but was processed

                            blobmanagement.moveblobtoProcessed(blobItem, shortBlobname, blobStorage, false);
                        }*/
                        //}
                    }
                    catch { }
                }
            }
            catch { }

            inProcess = false;

        }
        #endregion

        #region rollUpGeneratorThread




        static void requeueOldBlobs() //CloudQueue  queue, CloudBlobContainer container )
        {
            Trace.TraceInformation("Starting Thread to create rollups ");
            ParameterizedThreadStart serverThread = new ParameterizedThreadStart(ThreadWorkerRequeOldBlobs);
            ThreadInfo_ tiThreadInfo = new ThreadInfo_();
            Thread Thread = new Thread(serverThread);
            //tiThreadInfo.queue = queue;
            //tiThreadInfo.container  = container ;
            Thread.Start(tiThreadInfo);


        }

        static void ThreadWorkerRequeOldBlobs(object ThreadData)

        {

            //CloudQueue queue = ((ThreadInfo_)ThreadData).queue;
            //CloudBlobClient blobStorage = ((ThreadInfo_)ThreadData).blobStorage ;
            //CloudBlobContainer container = ((ThreadInfo_)ThreadData).container ; 

            BlobRequestOptions options = new BlobRequestOptions();
            //options.UseFlatBlobListing = true;

            try
            {
                foreach (CloudBlockBlob blobItem in container.ListBlobs(null, true, BlobListingDetails.None ))
                {

                    try
                    {
                        // if the blob is there after 7 days means something happened to its queue pair
                        if (((TimeSpan)(DateTime.Now - blobItem.Properties.LastModified)).TotalMinutes > (60 * 24 * 1))
                        //Console.WriteLine(blobItem.Uri);
                        {
                            blobItem.FetchAttributes();
                            if (blobItem.Metadata["BATCHID"] == "0")
                            {
                                CloudQueueMessage msg = new CloudQueueMessage(blobItem.Uri.ToString());
                                queue.AddMessage(msg);
                            }
                            else
                            {
                                //for any reason the blob is there but was processed
                                string shortBlobname = stripAzureRoot(blobItem.Uri.ToString());
                                blobmanagement.moveblobtoProcessed(blobItem, shortBlobname, blobStorage, false);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        static void startRollUpThread()
        {
            Trace.TraceInformation("Starting Thread to create rollups ");
            ParameterizedThreadStart serverThread = new ParameterizedThreadStart(ThreadWorkerTimerRollups);
            Thread Thread = new Thread(serverThread);
            Thread.Start();
        }

        static void ThreadWorkerTimerRollups(object ThreadData)
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


        }
        static void timer_Tick15(Object stateInfo)
        {
            int minutesPerHour = 50; // 60;

            try
            {
                oTimerReach15Min.Dispose();
                oTimerReach15Min = null;
            }
            catch { }

            TimerCallback oCallbackStart = new TimerCallback(waitFor15Min_tick);
            oTimerHourly = new Timer(oCallbackStart, null, 1000 * 60 * minutesPerHour, 1000 * 60 * minutesPerHour);
            generateRollups();
        }

        static void waitFor15Min_tick(Object stateInfo)
        {
            generateRollups();
        }

        static void generateRollups()
        {
            //lester apr 26/13
            // must to create one thread per each kind of storage
            try
            {

                ParameterizedThreadStart serverThread = new ParameterizedThreadStart(processazutestorage);
                Thread Thread = new Thread(serverThread);

                Thread.Start(storageAccount);


            }
            catch { }
            //try
            //{
            //    ParameterizedThreadStart serverThread = new ParameterizedThreadStart(processsqlstorage);
            //    Thread Thread = new Thread(serverThread);
            //    Thread.Start();

            //}
            //catch { }

            try
            {
                requeueOldBlobs(); //queue, container);
            }
            catch { }
        }

        static void processazutestorage(object ThreadData)
        {
            try
            {
                azurestorage.generateRollupsAzureStorage((CloudStorageAccount)ThreadData);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("****************** error running rollups " + ex.Message);
            }
        }

        static void processsqlstorage(object ThreadData)
        {
            try
            {
                Trace.TraceInformation("****************** running rollups ");
                sqlstorage.generateRollupsSqlStorage();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("****************** error running rollups " + ex.Message);
            }
        }



        #endregion

        #region azurelogs_processor
        static void startAzureLogsReaderThread()
        {
            Trace.TraceInformation("Starting Thread to read the azure logs ");
            ParameterizedThreadStart serverThread = new ParameterizedThreadStart(ThreadWorkerAzureLogs);
            Thread Thread = new Thread(serverThread);
            /*tiThreadInfo.queue = queue;
            tiThreadInfo.blobStorage = blobStorage;
            tiThreadInfo.ThreadData = iThreadNumber.ToString();*/
            //Thread.Start(tiThreadInfo);
            Thread.Start();
        }

        static void ThreadWorkerAzureLogs(object ThreadData)
        {

            TimerCallback oCallbackStart = new TimerCallback(azurelogs_tick);
            oTimerStart = new Timer(oCallbackStart, null, 1000 * 60 * 3, 1000 * 60 * 3);

        }


        static DataTable addAzureLogColumns(DataTable data)
        {

            data.Columns.Add("cip", typeof(string));
            data.Columns.Add("dduration", typeof(decimal));
            data.Columns.Add("csuristem", typeof(string));
            data.Columns.Add("pubpoint", typeof(string));
            data.Columns.Add("format_code", typeof(int));
            data.Columns.Add("logfilename", typeof(string));
            data.Columns.Add("csurl", typeof(string));
            data.Columns.Add("protocol", typeof(string));
            data.Columns.Add("csmedianame", typeof(string));
            data.Columns.Add("xapp", typeof(string));
            data.Columns.Add("xsuri", typeof(string));
            data.Columns.Add("csuseragent", typeof(string));
            data.Columns.Add("scbytes", typeof(string));
            data.Columns.Add("harvestedfrom", typeof(string));

            data.Columns.Add("logcreatedate", typeof(DateTime));
            data.Columns.Add("enddatetime", typeof(DateTime));
            data.Columns.Add("datelog", typeof(DateTime));

            data.Columns.Add("servername", typeof(string));

            data.Columns.Add("longip", typeof(long));
            data.Columns.Add("countrya2", typeof(string));
            data.Columns.Add("state", typeof(string));
            data.Columns.Add("city", typeof(string));
            data.Columns.Add("country", typeof(string));

            data.Columns.Add("longitude", typeof(decimal));
            data.Columns.Add("latitude", typeof(decimal));
            data.Columns.Add("dDuration", typeof(int));
            data.Columns["time"].DataType = typeof(DateTime);
            data.Columns["date"].DataType = typeof(DateTime);

            return data;
        }

        static void azurelogs_tick(Object stateInfo)
        {
            try
            {
                process_azurelogs();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(" " + ex.Message);
            }

        }
        static void process_azurelogs()
        {
            DataTable data = new DataTable();

            data = addAzureLogColumns(data);

            AzureLogsLookup all = new AzureLogsLookup(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString").ToString());

            List<LogsStructure> logs = all.getLogs();

            foreach (LogsStructure log in logs)
            {
                int lineNo = 0;
                DateTime s1 = DateTime.Now;

                foreach (string line in log.logcontents)
                {
                    if (lineNo > 0)
                    {
                        string[] lineDetail = line.Split(new char[] { ';' });
                        DataRow dr = data.NewRow();



                        dr["dduration"] = Sql.ToDecimal(lineDetail[5]);
                        dr["cip"] = lineDetail[15];
                        dr["protocol"] = "http";

                        dr["format_code"] = 4;
                        dr["servername"] = "netro";
                        dr["harvestedfrom"] = "netro";
                        dr["logfilename"] = log.logname;
                        if (log.logenddatetime != null)
                        {
                            dr["logcreatedate"] = log.logenddatetime.ToString();
                        }
                        else
                        {
                            dr["logcreatedate"] = (DateTime)dr["date"];
                        }

                        dr["datelog"] = ((DateTime)dr["date"]).AddHours(((DateTime)dr["time"]).Hour).AddMinutes(((DateTime)dr["time"]).Minute).AddSeconds(((DateTime)dr["time"]).Second);

                        dr["csurl"] = lineDetail[11];
                        dr["csuristem"] = lineDetail[11];
                        dr["csmedianame"] = lineDetail[12].Replace("\"", "");

                        LookupService ls = new LookupService();
                        // Location l = ls.getLocation("oracle.com");
                        Location l = ls.getLocation(dr["cip"].ToString());
                        if (l != null)
                        {
                            dr["countrya2"] = Sql.ToString(l.countryCode);
                            dr["state"] = Sql.ToString(l.regionName);
                            dr["city"] = Sql.ToString(l.city);
                            dr["country"] = Sql.ToString(l.countryName);
                            dr["latitude"] = Sql.ToDouble(l.latitude);
                            dr["longitude"] = Sql.ToDouble(l.longitude);
                        }

                        // azure returns duration in miliseconds

                        try
                        {
                            dr["dduration"] = Math.Ceiling(Sql.ToDecimal(dr["xduration"]) / 1000);
                        }
                        catch
                        {
                            dr["dduration"] = 1;
                        }
                        if (dr["dduration"].Equals(0))
                        {
                            dr["dduration"] = 1;
                        }


                    }
                    lineNo++;
                }

                if (data.Rows.Count > 0)
                {
                    //    saveLog(data, s1);
                    data = null;
                    data = new DataTable();
                    data = addAzureLogColumns(data);
                }
            }
        }

        #endregion azurelogs_processor

        #region logsthread
        static void ThreadWorkerNetroLogs(object ThreadData)
        {

            string[] ThreadInfo = ((ThreadInfo_)ThreadData).ThreadData.ToString().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            CloudQueue queue = ((ThreadInfo_)ThreadData).queue;
            CloudBlobClient blobStorage = ((ThreadInfo_)ThreadData).blobStorage;
            LookupService ls = ((ThreadInfo_)ThreadData).ls;

            // Now that the queue and the container have been created in the above initialization process, get messages
            // from the queue and process them individually.
            while (true) // && iMaxMsgs < 1)
            {
                try
                {

                    // Trace.TraceInformation("before queue get: " );

                    CloudQueueMessage msg = queue.GetMessage();
                    if (msg == null)
                    {
                        Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] message null: sleeping..... ");
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    if (msg.DequeueCount > 1)
                    {
                        Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] message found old queue: delete and then sleep..... ");
                        try
                        {
                            queue.DeleteMessage(msg);
                        }
                        catch { }

                        continue;
                    }


                    string blobname = msg.AsString;

                    if (blobname.Length == 0)
                    {

                        try
                        {
                            queue.DeleteMessage(msg);
                        }
                        catch
                        {
                            Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] error. Blog name empty deleting queue msg ");
                            //saveErrLog( string server, string filename, string section , string errmsg )
                        }

                        continue;
                    }


                    string shortBlobname = stripAzureRoot(blobname);

                    //Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] msg blobname = " + Sql.ToString(blobname.Trim()));
                    CloudBlobContainer cbcontainer = blobStorage.GetContainerReference("netrologs5");
                    CloudBlockBlob blob = cbcontainer.GetBlockBlobReference(shortBlobname);
                    //CloudBlockBlob blob = (CloudBlockBlob)blobStorage.getcon .GetBlobReferenceFromServer(new Uri(shortBlobname));  
                    //blobStorage.GetBlobReference(shortBlobname);

                    try
                    {

                        // blob exists
                        blob.FetchAttributes();
                        if (blob.Metadata["BATCHID"] == "0")
                        {
                            Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] starting to process blob ");
                            //blob still to be processed
                            if (blobmanagement.processBlob(blob, getServerName(shortBlobname), blobname, ls, storageAccount))
                            {
                                Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] deleting queue and moving to procssed ");
                                try
                                {
                                    queue.DeleteMessage(msg);
                                }
                                catch
                                {
                                    Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] error deleting queue msg ");
                                }
                                blobmanagement.moveblobtoProcessed(blob, shortBlobname, blobStorage, true);
                            }
                            else
                            {
                                azurestorage.addLogFileToUnprocessedTable(blob, storageAccount);
                                Trace.TraceInformation("Thread [" + ThreadInfo[0] + "]  cant process blob ");
                                try
                                {
                                    queue.DeleteMessage(msg);
                                }
                                catch
                                {
                                    Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] error. Blog name empty deleting queue msg ");
                                    //saveErrLog( string server, string filename, string section , string errmsg )
                                }


                            }
                        }
                        else
                        {
                            Trace.TraceInformation("Thread [" + ThreadInfo[0] + "]  blob alreadly processed");
                            try
                            {
                                blobmanagement.moveblobtoProcessed(blob, shortBlobname, blobStorage, false);
                            }
                            catch { Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] error deleting queue msg "); }

                            try
                            {
                                queue.DeleteMessage(msg);
                            }
                            catch
                            {
                                Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] error. Blog name empty deleting queue msg ");
                                //saveErrLog( string server, string filename, string section , string errmsg )
                            }


                            //blob.Delete();
                        }


                    }
                    catch ( Exception ex )
                    {
                        Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] blob associated with queue msg doesnt exists ");
                        // blob doesnt exists
                        try
                        {
                            queue.DeleteMessage(msg);
                        }
                        catch { Trace.TraceInformation("Thread [" + ThreadInfo[0] + "] error deleting queue msg "); }

                        continue;
                    }




                    //original code
                    // RequestState state = new RequestState(sUrl, gAccountID, msg, queue, gKEY);
                    // AsyncMethodCaller caller = new AsyncMethodCaller(state.Process);
                    // IAsyncResult ar = caller.BeginInvoke(state, new AsyncCallback(DownloadCompleted), state);
                }
                catch (Exception e)
                {
                    Trace.TraceError(string.Format("Thread [" + ThreadInfo[0] + "] Exception when processing queue item. Message: '{0}'", e.Message));
                }
            }

            //Trace.TraceInformation("Skipping loop");
            System.Threading.Thread.Sleep(5000);

        }

        static string getServerName(string blobname)
        {
            int ps = blobname.IndexOf(logsContainerName);
            string sname = "";
            if (ps > 0)
            {
                sname = blobname.Substring(ps + logsContainerName.Length + 1);
            }
            else
                sname = blobname;
            ps = sname.IndexOf("/");
            sname = sname.Substring(0, ps);
            return sname;
        }

        static string stripAzureRoot(string blobname)
        {
            if (blobname.Contains("https"))
            {
                blobname = blobname.Replace("https", "http");
            }

            blobname = blobname.Replace(blobRootStr + "/netrologs5/", "");

            return blobname;
        }

        private static HttpWebRequest CreateHttpRequest(Uri uri, string httpMethod, TimeSpan timeout)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Timeout = (int)timeout.TotalMilliseconds;
            request.ReadWriteTimeout = (int)timeout.TotalMilliseconds;
            request.Method = httpMethod;
            request.ContentLength = 0;
            request.Headers.Add("x-ms-date",
            DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2009-09-19");
            return request;
        }
        static MemoryStream downloadblob(string blobURL, long Length)
        {
            MemoryStream fileData = new MemoryStream();


            try
            {

                HttpWebRequest hwr = CreateHttpRequest(new Uri(blobURL), "GET", new TimeSpan(0, 0, 120));


                // A buffer to fill per read request.
                byte[] buffer = new byte[4 * 1024 * 1024];

                // Read chunk.
                using (HttpWebResponse response = hwr.GetResponse() as
                    HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        int offsetInChunk = 0;
                        int remaining = (int)Length;


                        while (remaining > 0)
                        {

                            int read = stream.Read(buffer, 0, remaining);
                            if (read != 0)
                            {

                                lock (fileData)
                                {
                                    fileData.Position = (offsetInChunk);
                                    fileData.Write(buffer, 0, (int)read);
                                }

                            }


                            offsetInChunk += read;
                            remaining -= read;
                            // Interlocked.Add(ref bytesDownloaded, read);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(blobURL + " Error R5 reading file from cloud  " + ex.Message);

            }
            return fileData;
        }


        #endregion logsthread


        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = iMaxThreads; // 12;

            #region Setup CloudStorageAccount Configuration Setting Publisher
            /*
            // This code sets up a handler to update CloudStorageAccount instances when their corresponding
            // configuration settings change in the service configuration file.
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                // Provide the configSetter with the initial value
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

                RoleEnvironment.Changed += (sender, arg) =>
                {
                    if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>()
                        .Any((change) => (change.ConfigurationSettingName == configName)))
                    {
                        // The corresponding configuration setting has changed, propagate the value
                        if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
                        {
                            // In this case, the change to the storage account credentials in the
                            // service configuration is significant enough that the role needs to be
                            // recycled in order to use the latest settings. (for example, the 
                            // endpoint has changed)
                            RoleEnvironment.RequestRecycle();
                        }
                    }
                };
            });
            */
            #endregion

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
    public class ThreadInfo_
    {
        public string ThreadData;
        public CloudQueue queue;
        public CloudBlobClient blobStorage;
        public LookupService ls;
        public CloudBlobContainer container;
    }
}
