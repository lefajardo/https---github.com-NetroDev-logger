using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Microsoft.WindowsAzure.Storage.Table.Protocol;

namespace NetroLogger_WorkerRole
{
    public class azurestorage
    {
        static string tableName_logstaging = "LOGMASTER5";
        static string tableName_logsbatch = "LOGBATCHES5";
        static string tableName_loghourly = "LOGHOURLY5";
        static string tableName_logdaily = "LOGDAILY5";
        static string tableName_noprocessed = "LOGSNOPROCESSED5";
        #region rollups_processing

        static void processLogFileforDailyRollUp(List<logsToRullUp> fileToProcess, CloudStorageAccount storageAccount)
        {

            foreach (logsToRullUp dayToProcess in fileToProcess)
            {
                List<logFilesHourlyRollUpAzTbl> hourlyLog = new List<logFilesHourlyRollUpAzTbl>();

                //ResultSegment<logFilesHourlyRollUpAzTbl2> qryRows = null;
                List<logFilesHourlyRollUpAzTbl2> rows = new List<logFilesHourlyRollUpAzTbl2>();

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable logTable = tableClient.GetTableReference(tableName_loghourly);
                IEnumerable<logFilesHourlyRollUpAzTbl2> query = null;

                try
                {
                    query = (from e in logTable.CreateQuery<logFilesHourlyRollUpAzTbl2>()
                                                                     where e.DAY == dayToProcess.DAY
                                                                                  && e.MONTH == dayToProcess.MONTH
                                                                                  && e.YEAR == dayToProcess.YEAR
                                                                                  && e.SERVER == dayToProcess.SERVER
                                                                     select e);
                    foreach (logFilesHourlyRollUpAzTbl2 row in query)
                    {

                        bool found = false;
                        Trace.TraceInformation("new row {0}-{1} ", row.DATETIME_HOUR, row.PUBPOINT);

                        foreach (logFilesHourlyRollUpAzTbl rw in hourlyLog)
                        {
                            if (rw.PUBPOINT == row.PUBPOINT &&
                                 rw.DATETIME_HOUR == (new DateTime(
                                     int.Parse(row.YEAR), int.Parse(row.MONTH), int.Parse(row.DAY))).ToString("yyyyMMdd"))
                            {
                                rw.KILOBYTES_SENT += float.Parse(row.KILOBYTES_SENT);
                                rw.CONNECTIONS += float.Parse(row.CONNECTIONS);
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            hourlyLog.Add(new logFilesHourlyRollUpAzTbl((new DateTime(
                                     int.Parse(row.YEAR), int.Parse(row.MONTH), int.Parse(row.DAY))).ToString("yyyyMMdd"),
                            row.PUBPOINT, float.Parse(row.CONNECTIONS), float.Parse(row.KILOBYTES_SENT), row.SERVER,
                            row.DAY, row.MONTH, row.YEAR, ""));

                        }
                    }
                }
                catch { }

                
                Trace.TraceInformation("total records to insert  {0} ", hourlyLog.Count);

                // before saving, must to delete previous daily data

                
                CloudTable ctc = tableClient.GetTableReference(tableName_logdaily);
                ctc.CreateIfNotExists();
                
                
                foreach (logFilesHourlyRollUpAzTbl rowDaily in hourlyLog)
                {

                    IEnumerable<logFilesHourlyRollUpAzTbl2> logs =
                            (from e in ctc.CreateQuery<logFilesHourlyRollUpAzTbl2>()
                             where e.DATETIME_HOUR == rowDaily.DATETIME_HOUR &&
                                    e.SERVER == rowDaily.SERVER
                             select e);
                    List<logFilesHourlyRollUpAzTbl2> logsList = logs.ToList<logFilesHourlyRollUpAzTbl2>();

                    foreach (logFilesHourlyRollUpAzTbl2 row in logsList)
                    {
                        try
                        {
                            TableOperation to = TableOperation.Delete(row);
                            
                        }
                        catch { }
                    }

                }


                
                // save logs daily rollup
                saveLogHourly(hourlyLog, tableName_logdaily, storageAccount);


            }
        }

        static logsToRullUp processLogFileforHourlyRollUp(Guid batchID, CloudStorageAccount storageAccount)
        {
            logsToRullUp ret = new logsToRullUp();

            List<logFilesHourlyRollUpAzTbl> hourlyLog = new List<logFilesHourlyRollUpAzTbl>();

            ResultSegment<logStagingAzTbl> qryRows = null;

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable logTable = tableClient.GetTableReference(tableName_logstaging);
            IEnumerable<logStagingAzTbl> query = null;

            List<logStagingAzTbl> rows = new List<logStagingAzTbl>();

            query = (from e in logTable.CreateQuery<logStagingAzTbl>()
                     where e.BATCH_ID == batchID.ToString()
                     select e);
            /*
            CloudTableQuery<logStagingAzTbl> q = (from e in tsc1.CreateQuery<logStagingAzTbl>(tableName_logstaging)
                                                  where e.BATCH_ID == batchID.ToString()
                                                  select e).AsTableServiceQuery<logStagingAzTbl>();

            do
            {
                qryRows = q.EndExecuteSegmented(q.BeginExecuteSegmented(qryRows
                    != null ? qryRows.ContinuationToken : null, null, null));
                rows.AddRange(qryRows.Results);
            } while (qryRows.ContinuationToken != null);
            */
            foreach (logStagingAzTbl row in rows)
            {

                bool found = false;
                Trace.TraceInformation("new row {0}-{1}-{2} ", row.DATETIME,
                    DateTime.Parse(row.DATETIME).ToString("yyyy-MM-dd-HH"),
                        row.PUBPOINT);
                foreach (logFilesHourlyRollUpAzTbl rw in hourlyLog)
                {
                    if (rw.PUBPOINT == row.PUBPOINT &&
                         rw.DATETIME_HOUR == DateTime.Parse(row.DATETIME).ToString("yyyyMMddHH"))
                    {
                        rw.KILOBYTES_SENT += float.Parse(row.KILOBYTESSENT);
                        rw.CONNECTIONS += 1;
                        found = true;
                    }
                }
                if (!found)
                {
                    hourlyLog.Add(new logFilesHourlyRollUpAzTbl(DateTime.Parse(row.DATETIME).ToString("yyyyMMddHH"),
                    row.PUBPOINT, 1, float.Parse(row.KILOBYTESSENT), row.SERVER,
                    row.DAY, row.MONTH, row.YEAR, row.HOUR));

                    ret.SERVER = row.SERVER;
                    ret.DAY = row.DAY;
                    ret.MONTH = row.MONTH;
                    ret.YEAR = row.YEAR;
                }
            }

            //q = null;
            qryRows = null;
            //tsc1 = null;

            Trace.TraceInformation("total records to insert  {0} ", hourlyLog.Count);

            saveLogHourly(hourlyLog, tableName_loghourly, storageAccount);

            return ret;
        }

        public static void generateRollupsAzureStorage( CloudStorageAccount storageAccount)
        {
            int total = 0;

            // this timer tick will start each hour

            // get all unique list of log filenames for the unprocessed row lines in the logstaging tables
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable logTable = tableClient.GetTableReference(tableName_logstaging);
            IEnumerable<logStagingAzTbl> query = null;

            CloudTable ctc = tableClient.GetTableReference(tableName_logsbatch);
            ctc.CreateIfNotExists();

            /*
            CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName_logstaging);
            TableServiceContext tsc = ctc.GetDataServiceContext();

            tsc.ResolveType = (unused) => typeof(logBatchesAzTbl);
*/
            IQueryable<logBatchesAzTbl> logs =
                        (from e in logTable.CreateQuery<logBatchesAzTbl>()
                         where e.ROLLUP_COMPLETED == "N"
                         select e);


            List<logBatchesAzTbl> logsList = logs.ToList<logBatchesAzTbl>();
            List<logsToRullUp> fileToProcess = new List<logsToRullUp>();

            try
            {
                foreach (logBatchesAzTbl row in logsList)
                {
                    // create the hourly rollup for pubpoints
                    logsToRullUp daytoProcess = processLogFileforHourlyRollUp(row.BATCH_ID, storageAccount);

                    // add the log to a list of days that need to be recalculated
                    // due that new log files where added to it
                    total = 0;

                    total = fileToProcess.Count(log => log.DAY == daytoProcess.DAY &&

                                                            log.MONTH == daytoProcess.MONTH &&
                                                            log.YEAR == daytoProcess.YEAR &&
                                                            log.SERVER == daytoProcess.SERVER);

                    if (total == 0)
                    {
                        try
                        {
                            fileToProcess.Add(new logsToRullUp(daytoProcess.SERVER, daytoProcess.DAY,
                                daytoProcess.MONTH, daytoProcess.YEAR, ""));
                        }
                        catch
                        {
                        }
                    }

                    //mark the batch as processed

                    row.ROLLUP_COMPLETED = "Y";
                    TableOperation tu = TableOperation.Replace(row);
                    ctc.Execute(tu);
                    //tsc.UpdateObject(row);
                    //tsc.SaveChanges();

                }
            }
            catch
            {
            }
            // recalculate the days
            try
            {
                processLogFileforDailyRollUp(fileToProcess, storageAccount);
            }
            catch
            {
            }

        }
        static void saveLogHourly(List<logFilesHourlyRollUpAzTbl> hourlyLog, string tablename, CloudStorageAccount storageAccount)
        {



            //CloudTable logTable = tableClient.GetTableReference(tableName_logstaging);
            //IEnumerable<logStagingAzTbl> query = null;

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable ctc = tableClient.GetTableReference(tablename);
            ctc.CreateIfNotExists();


            //CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            //ctc.CreateTableIfNotExist(tablename);

      //      TableServiceContext tsc = ctc.GetDataServiceContext();
      //      tsc.ResolveType = (unused) => typeof(logFilesHourlyRollUpAzTbl);

            //            logFilesHourlyRollUpAzTbl logrow = null;

            foreach (logFilesHourlyRollUpAzTbl logrow in hourlyLog)
            {

                logFilesHourlyRollUpAzTbl2 logrow2 = new logFilesHourlyRollUpAzTbl2();

                try
                {
                    logrow2.PartitionKey = logrow.PUBPOINT;
                    logrow2.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString() + logrow.DATETIME_HOUR;
                    logrow2.CONNECTIONS = logrow.CONNECTIONS.ToString();
                    logrow2.DATETIME_HOUR = logrow.DATETIME_HOUR;
                    logrow2.DAY = logrow.DAY;
                    logrow2.HOUR = logrow.HOUR;
                    logrow2.KILOBYTES_SENT = logrow.KILOBYTES_SENT.ToString();
                    logrow2.MONTH = logrow.MONTH;
                    logrow2.PUBPOINT = logrow.PUBPOINT;
                    logrow2.SERVER = logrow.SERVER;
                    logrow2.YEAR = logrow.YEAR;

                    TableOperation to = TableOperation.Insert(logrow2);
                    ctc.Execute(to);

                    //tsc.AddObject(tablename, logrow2);
                    //Trace.TraceInformation("before posting in azure storage 3");
                    //tsc.SaveChanges();
                    //Trace.TraceInformation("after posting in azure storage 3");
                }
                catch
                {


                }

            }

        }


        #endregion rollups_processing

        #region saveAzure_LogsTables

        public static void saveAzureLog(DataTable LogTable, DataTable LogTableBatch,
            DataTable LogTableLFiles, DataTable LogTableLFRU, CloudStorageAccount storageAccount)
        {
            saveLogs(LogTable, storageAccount);
            saveBatch(LogTableBatch, storageAccount);
            saveLogFile(LogTableLFiles, storageAccount);
            saveLogFHRU(LogTableLFRU, storageAccount);
        }

        static void saveLogFHRU(DataTable LogTableLFRU, CloudStorageAccount storageAccount)
        {
            string tableName = "LOGFILEROLLUP";
            /*
            CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName);

            TableServiceContext tsc = ctc.GetDataServiceContext();
            tsc.ResolveType = (unused) => typeof(logFilesRollUpAzTbl);
*/
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable ctc = tableClient.GetTableReference(tableName);
            ctc.CreateIfNotExists();



            foreach (DataRow dr in LogTableLFRU.Rows)
            {

                logFilesRollUpAzTbl logrow = new logFilesRollUpAzTbl(new Guid(dr["LOG_ID"].ToString()));

                try
                {
                    logrow.LOG_ID = new Guid(dr["LOG_ID"].ToString());

                    logrow.DATE_ENTERED = dr["DATE_ENTERED"].ToString();
                    logrow.CONNECTIONS = dr["CONNECTIONS"].ToString();
                    logrow.KILOBYTES_SENT = dr["KILOBYTES_SENT"].ToString();
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on logfilesrollupaztbl " + ex.Message);
                }

                try
                {
                    TableOperation to = TableOperation.Insert(logrow);
                    ctc.Execute(to);
                    //tsc.AddObject(tableName, logrow);
                    //tsc.SaveChanges();
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on inserting logfilesrollupaztbl2  " + ex.Message);
                }

            }

        }

        static void saveLogFile(DataTable LogTableLFiles, CloudStorageAccount storageAccount)
        {
            string tableName = "LOGFILES";
            /*
            CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName);

            TableServiceContext tsc = ctc.GetDataServiceContext();
            tsc.ResolveType = (unused) => typeof(logFilesAzTbl);
            */

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable ctc = tableClient.GetTableReference(tableName);
            ctc.CreateIfNotExists();

            foreach (DataRow dr in LogTableLFiles.Rows)
            {

                logFilesAzTbl logrow = new logFilesAzTbl(dr["HASH"].ToString());

                try
                {
                    logrow.LOG_ID = new Guid(dr["LOG_ID"].ToString());

                    logrow.DATE_ENTERED = dr["DATE_ENTERED"].ToString();
                    logrow.LOGFILENAME = dr["LOGFILENAME"].ToString();
                    logrow.SERVER_NAME = dr["SERVER_NAME"].ToString();
                    logrow.SERVER_IP = dr["SERVER_IP"].ToString();
                    logrow.LOG_URL = dr["LOG_URL"].ToString();
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on insert logfilesaztbl " + ex.Message);
                }


                try
                {
                    TableOperation to = TableOperation.Insert(logrow);
                    ctc.Execute(to);

                    //tsc.AddObject(tableName, logrow);
                    //tsc.SaveChanges();
                }
                catch (Exception ex)
                {


                    Trace.TraceInformation("err on commit logfilesaztbl " + ex.Message);

                }

            }

        }

        static void saveBatch(DataTable LogTableBatch, CloudStorageAccount storageAccount)
        {
            //string tableName = "LOGBATCHES";
            /*CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName_logsbatch);

            TableServiceContext tsc = ctc.GetDataServiceContext();
            tsc.ResolveType = (unused) => typeof(logBatchesAzTbl);
            */

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable ctc = tableClient.GetTableReference(tableName_logsbatch);
            ctc.CreateIfNotExists();

            foreach (DataRow dr in LogTableBatch.Rows)
            {

                logBatchesAzTbl logrow = new logBatchesAzTbl(new Guid(dr["BATCH_ID"].ToString()));

                try
                {
                    logrow.BATCH_COMPLETED = "1";
                    logrow.ROLLUP_COMPLETED = "N";
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on marking batchesaztbl " + ex.Message);
                }

                try
                {
                    TableOperation to = TableOperation.Insert(logrow);
                    ctc.Execute(to);
                    //tsc.AddObject(tableName_logsbatch, logrow);
                    //tsc.SaveChanges();
                }
                catch (Exception ex)
                {


                    Trace.TraceInformation("err on commiting batchesaztbl " + ex.Message);

                }

            }

        }

        /*
        static void saveErrLog(string server, string filename, string section, string errmsg)
        {
            string tableName = "LOGIMPORTERROR";
            CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName);

            TableServiceContext tsc = ctc.GetDataServiceContext();
            tsc.ResolveType = (unused) => typeof(logErrorssAzTbl);

            logErrorssAzTbl logrow = new logErrorssAzTbl(server);
            try
            {
                logrow.SECTION_ERR = section;
                logrow.ERR_DESCRIPTION = errmsg;
                logrow.LOGFILENAME = filename;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on renumbering " + ex.Message);
            }
            try
            {

                tsc.AddObject(tableName, logrow);
                //Trace.TraceInformation("before posting in azure storage 1" );
                tsc.SaveChanges();
                //Trace.TraceInformation("after posting in azure storage 1");
            }
            catch (Exception ex)
            {


                Trace.TraceInformation("err on renumbering " + ex.Message);

            }



        }
*/

        static void saveLogs(DataTable LogTable, CloudStorageAccount storageAccount)
        {
            /*
            CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName_logstaging);

            TableServiceContext tsc = ctc.GetDataServiceContext();
            tsc.ResolveType = (unused) => typeof(logStagingAzTbl);

            */


            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable ctc = tableClient.GetTableReference(tableName_logstaging);
            ctc.CreateIfNotExists();

            logStagingAzTbl logrow = null;

            foreach (DataRow dr in LogTable.Rows)
            {

                try
                {
                    logrow = new logStagingAzTbl(dr["pubpoint"].ToString(), dr["server"].ToString());

                    logrow.LOGFILENAME = dr["logfilename"].ToString();
                    logrow.LOGCREATEDATE = dr["logcreatedate"].ToString();
                    logrow.FORMAT_CODE = dr["format_code"].ToString();
                    logrow.CIP = dr["cip"].ToString();
                    logrow.DATETIME = dr["datetime"].ToString();
                    logrow.ENDDATETIME = dr["enddatetime"].ToString();
                    logrow.PUBPOINT = dr["pubpoint"].ToString();
                    logrow.CSURISTEM = dr["csuristem"].ToString();
                    logrow.SIP = dr["sip"].ToString();
                    logrow.CSURL = dr["csurl"].ToString();
                    logrow.PROTOCOL = dr["protocol"].ToString();
                    logrow.CSMEDIANAME = dr["csmedianame"].ToString();
                    logrow.CPLAYERID = dr["cplayerid"].ToString();
                    logrow.CSUSERAGENT = dr["csuseragent"].ToString();
                    logrow.CSTARTTIME = dr["cstarttime"].ToString();
                    logrow.XDURATION = dr["xduration"].ToString();
                    logrow.CSTATUS = dr["cstatus"].ToString();
                    logrow.CPLAYERVERSION = dr["cplayerversion"].ToString();
                    logrow.COS = dr["cos"].ToString();
                    logrow.AVGBANDWIDTH = dr["avgbandwidth"].ToString();
                    logrow.TRANSPORT = dr["transport"].ToString();
                    logrow.KILOBYTESSENT = dr["KiloBytesSent"].ToString();
                    logrow.CBYTES = dr["cbytes"].ToString();
                    logrow.SERVER = dr["server"].ToString();
                    logrow.STOTALCLIENTS = dr["stotalclients"].ToString();
                    logrow.SCONTENTPATH = dr["scontentpath"].ToString();
                    logrow.CMAXBANDWIDTH = dr["cMaxBandwidth"].ToString();
                    logrow.CSMEDIAROLE = dr["csmediarole"].ToString();
                    logrow.SPROXIED = dr["sproxied"].ToString();
                    logrow.HARVESTEDFROM = dr["harvestedfrom"].ToString();
                    logrow.LONGIP = dr["longip"].ToString();
                    logrow.COUNTRYA2 = dr["countrya2"].ToString();
                    logrow.STATE = dr["state"].ToString();
                    logrow.CITY = dr["city"].ToString();
                    logrow.COUNTRY = dr["country"].ToString();
                    logrow.SERVER_NAME = dr["server_name"].ToString();
                    logrow.BATCH_ID = dr["batch_id"].ToString();
                    logrow.BATCH_COMPLETED = dr["batch_completed"].ToString();
                    logrow.LONGITUDE = dr["long"].ToString();
                    logrow.LATITUDE = dr["lat"].ToString();

                    logrow.DAY = DateTime.Parse(dr["datetime"].ToString()).Day.ToString();
                    logrow.MONTH = DateTime.Parse(dr["datetime"].ToString()).Month.ToString();
                    logrow.YEAR = DateTime.Parse(dr["datetime"].ToString()).Year.ToString();
                    logrow.HOUR = DateTime.Parse(dr["datetime"].ToString()).Hour.ToString();


                }

                catch (Exception ex)
                {
                    Trace.TraceInformation("err on inserting log record " + ex.Message);
                }

                try
                {
                    TableOperation to = TableOperation.Insert(logrow);
                    ctc.Execute(to);

                    //tsc.AddObject(tableName_logstaging, logrow);
                    //Trace.TraceInformation("before posting in azure storage 3");
                    //tsc.SaveChanges();
                    //Trace.TraceInformation("after posting in azure storage 3");
                }
                catch ( Exception ex1 )
                {

                    bool done = false;

                    while (!done)
                    {
                        try
                        {

                            TableOperation to1 = TableOperation.Delete(logrow);
                            ctc.Execute(to1);

                            //tsc.DeleteObject(logrow);
                            logrow.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString() + "__" + dr["server"].ToString() + "__" + Guid.NewGuid().ToString();

                            TableOperation to = TableOperation.Insert(logrow);
                            ctc.Execute(to);

                            //tsc.AddObject(tableName_logstaging, logrow);

                            //Trace.TraceInformation("before posting in azure storage 2");
                            //tsc.SaveChanges();

                            done = true;
                            //Trace.TraceInformation("after posting in azure storage ");
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation("err on commiting log table " + ex.Message);
                        }
                    }
                }

            }




            //AzureTableLogStorage.saveAzureLog(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"), LogTable);         
        }

        #endregion saveAzure_LogsTables

        #region unprocessedlogs
        public static void addLogFileToUnprocessedTable(CloudBlockBlob blob, CloudStorageAccount storageAccount)
        {
            /*
            CloudTableClient ctc = new CloudTableClient(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            ctc.CreateTableIfNotExist(tableName_noprocessed);

            TableServiceContext tsc = ctc.GetDataServiceContext();
            tsc.ResolveType = (unused) => typeof(logFilesNOPROCESSAzTbl);
            */
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable ctc = tableClient.GetTableReference(tableName_noprocessed);
            ctc.CreateIfNotExists();

            logFilesNOPROCESSAzTbl logrow = null;


            try
            {
                logrow = new logFilesNOPROCESSAzTbl(blob.Metadata["FILETYPE"], blob.Metadata["MD5"]);

                logrow.DATE_ENTERED = blob.Metadata["CREATIONDATETIME"];
                logrow.DATE_NOPROCESS = DateTime.UtcNow.ToLongDateString();
                logrow.LOG_URL = blob.Uri.ToString();
                logrow.LOGFILENAME = blob.Metadata["FULLNAME"];
                logrow.SERVER_IP = blob.Metadata["SERVERIP"];
                logrow.SERVER_NAME = blob.Metadata["SERVERNAME"];

            }

            catch (Exception ex)
            {
                Trace.TraceInformation("err on inserting log no process " + ex.Message);
            }

            try
            {
                TableOperation to = TableOperation.Insert(logrow);
                ctc.Execute(to);

                //tsc.AddObject(tableName_noprocessed, logrow);
                //Trace.TraceInformation("before posting in azure storage 3");
                //tsc.SaveChanges();
                //Trace.TraceInformation("after posting in azure storage 3");
            }
            catch
            {



                try
                {

                    TableOperation to = TableOperation.Delete(logrow);
                    ctc.Execute(to);

  //                  tsc.DeleteObject(logrow);
//                    tsc.SaveChanges();

                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on commiting 3 " + ex.Message);
                }
            }





        }
        #endregion unprocessedlogs

    }
}
