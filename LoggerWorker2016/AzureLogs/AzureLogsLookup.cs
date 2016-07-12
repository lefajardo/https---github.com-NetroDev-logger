using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using System.Globalization;
using System.Net;
using Microsoft.WindowsAzure;
using System.IO;
using System.Xml;
using Microsoft.WindowsAzure.Storage;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
namespace AzureLogs
{
    public class AzureLogsLookup
    {


        const string ConnectionStringKey = "ConnectionString";
        const string LogStartTime = "StartTime";
        const string LogEndTime = "EndTime";
        const string LogEntryTypes = "LogType";

        string _ConnectionString = "";


        public AzureLogsLookup(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
        public List<LogsStructure> getLogs()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_ConnectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            DateTime startTimeOfSearch = DateTime.UtcNow.AddDays( -77 ).ToUniversalTime();
            DateTime endTimeOfSearch = DateTime.UtcNow.ToUniversalTime();

            List<CloudBlockBlob> blobList = ListLogFiles(blobClient, "blob", startTimeOfSearch, endTimeOfSearch, "READ");

            return loadLogsContents(blobList);
          //  DisplayLogs(blobList, "");
        }

        List<LogsStructure> loadLogsContents(List<CloudBlockBlob> blobList)
        {
            List<LogsStructure> logDetail = new List<LogsStructure>();

            foreach (CloudBlockBlob blob in blobList)
            {

                LogsStructure logdata = new LogsStructure();
                logdata.logenddatetime = DateTime.Parse(blob.Metadata[LogEndTime]).ToUniversalTime();
                logdata.logname = blob.Uri.AbsoluteUri;


                using (Stream stream = blob.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string logEntry;
                        while ((logEntry = reader.ReadLine()) != null)
                        {
                            logdata.logcontents.Add(logEntry);
                        }
                    }
                }
                logDetail.Add(logdata);
            }
            return logDetail;
        }

        /// <summary>
        /// Given a service, start time, end time, and operation types (i.e. READ/WRITE/DELETE) to search for, this method
        /// iterates through blob logs and selects the ones that match the service and time range.
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="serviceName">The name of the service interested in</param>
        /// <param name="startTimeForSearch">Start time for the search</param>
        /// <param name="endTimeForSearch">End time for the search</param>
        /// <param name="operationTypes">A ',' separated operation types used as logging level</param>
        /// <returns></returns>
        static List<CloudBlockBlob> ListLogFiles(
        CloudBlobClient blobClient,
        string serviceName,
        DateTime startTimeForSearch,
        DateTime endTimeForSearch,
        string operationTypes)
        {
            List<CloudBlockBlob> selectedLogs = new List<CloudBlockBlob>();

            // convert a ',' separated log type to a "flag" enum 
            LoggingLevel loggingLevelsToFind = GetLoggoingLevel(operationTypes);

            // form the prefix to search. Based on the common parts in start and end time, this prefix is formed
            string prefix = GetSearchPrefix(serviceName, startTimeForSearch, endTimeForSearch);

          //  Console.WriteLine("Prefix used = {0}", prefix);

            // List the blobs using the prefix
            IEnumerable<IListBlobItem> blobs = blobClient.ListBlobs(
                prefix,true, BlobListingDetails.Metadata);


            // iterate through each blob and figure the start and end times in the metadata
            foreach (IListBlobItem item in blobs)
            {
                CloudBlockBlob log = item as CloudBlockBlob;
                if (log != null)
                {
                    DateTime startTime = DateTime.Parse(log.Metadata[LogStartTime]).ToUniversalTime();
                    DateTime endTime = DateTime.Parse(log.Metadata[LogEndTime]).ToUniversalTime();
                    string logTypes = log.Metadata[LogEntryTypes].ToUpper();

                    LoggingLevel levelsInLog = GetLoggoingLevel(logTypes);

                    // we will exclude the file if the time range does not match or it does not contain the log type 
                    // we are searching for
                    bool exclude = (startTime > endTimeForSearch
                        || endTime < startTimeForSearch
                        || (loggingLevelsToFind & levelsInLog) == LoggingLevel.None);

                    //Console.WriteLine("{0} Log {1} Start={2:U} End={3:U} Types={4}.",
                    //    exclude ? "Ignoring" : "Selected",
                    //    log.Uri.AbsoluteUri,
                    //    startTime,
                    //    endTime,
                    //    logTypes);

                    if (!exclude)
                    {
                        selectedLogs.Add(log);
                    }
                }
            }

            return selectedLogs;
        }

        /// <summary>
        /// Given service name, start time for search and end time for search, creates a prefix that can be used
        /// to efficiently get a list of logs that may match the search criteria
        /// </summary>
        /// <param name="service"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        static string GetSearchPrefix(string service, DateTime startTime, DateTime endTime)
        {
            StringBuilder prefix = new StringBuilder("$logs/");

            prefix.AppendFormat("{0}/", service);

            // if year is same then add the year
            if (startTime.Year == endTime.Year)
            {
                prefix.AppendFormat("{0}/", startTime.Year);
            }
            else
            {
                return prefix.ToString();
            }

            // if month is same then add the month
            if (startTime.Month == endTime.Month)
            {
                prefix.AppendFormat("{0:D2}/", startTime.Month);
            }
            else
            {
                return prefix.ToString();
            }

            // if day is same then add the day
            if (startTime.Day == endTime.Day)
            {
                prefix.AppendFormat("{0:D2}/", startTime.Day);
            }
            else
            {
                return prefix.ToString();
            }

            // if hour is same then add the hour
            if (startTime.Hour == endTime.Hour)
            {
                prefix.AppendFormat("{0:D2}00", startTime.Hour);
            }

            return prefix.ToString();
        }


        /// <summary>
        /// Displays all log entries containing a keyword
        /// </summary>
        /// <param name="blobList"></param>
        /// <param name="keyword"></param>
        static void DisplayLogs(List<CloudBlockBlob> blobList, string keyword)
        {
            Console.WriteLine("Log entries matching '{0}' are: ", keyword);

            foreach (CloudBlockBlob blob in blobList)
            {
                using (Stream stream = blob.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string logEntry;
                        while ((logEntry = reader.ReadLine()) != null)
                        {
                            if (logEntry.Contains(keyword))
                            {
                                Console.WriteLine(logEntry);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("End searching for '{0}'", keyword);
        }


        /// <summary>
        /// Given a ',' separated list of log types aka operation types, it returns the logging level 
        /// </summary>
        /// <param name="loggingLevels"></param>
        /// <returns></returns>
        static LoggingLevel GetLoggoingLevel(string loggingLevels)
        {
            string[] loggingLevelList = loggingLevels.ToUpper().Split(',');
            LoggingLevel level = LoggingLevel.None;
            foreach (string logLevel in loggingLevelList)
            {
                if (string.Equals("READ", logLevel))
                {
                    level |= LoggingLevel.Read;
                }
                else if (string.Equals("WRITE", logLevel))
                {
                    level |= LoggingLevel.Write;
                }
                else if (string.Equals("DELETE", logLevel))
                {
                    level |= LoggingLevel.Delete;
                }
            }

            return level;
        }

    }
}
