using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace NetroLogger_WorkerRole
{
    public static class blobmanagement
    {


        static string FieldsStr = "#fields:";
        static string SoftwareStr = "#software:";
        static string VersionStr = "#version:";
        static string StartDateWMSStr = "#startdate:";
        static string StartDateWZMStr = "#start-date:";
        static string StartDateDSSStr = "#date:";

        #region blobmanagement
        public static void moveblobtoProcessed(CloudBlockBlob blob, string sourceBlobname, CloudBlobClient blobStorage, bool processSuccesful)
        {
            try
            {
                string destinationBlobname = "";
                if (processSuccesful)
                {
                    if (sourceBlobname.IndexOf("netrologs5") >= 0)
                    {
                        destinationBlobname = sourceBlobname.Replace("netrologs5", "netrologsprocessed5");
                            }
                    else
                    {
                        destinationBlobname =  sourceBlobname;
                    }
                    //destinationBlobname = destinationBlobname.Replace("netrolog-do-not-process5", "netrologsprocessed5");

                }
                else
                {
                    destinationBlobname = sourceBlobname.Replace("netrologs5", "netrolog-do-not-process5");
                }

                //var blobClient = storageAccount.CreateCloudBlobClient();

                //CloudBlockBlob blobDest = (CloudBlockBlob)blobStorage.GetBlobReferenceFromServer( new Uri(  destinationBlobname));
                CloudBlobContainer cbcontainer = blobStorage.GetContainerReference("netrologsprocessed5");
                CloudBlockBlob blobDest = cbcontainer.GetBlockBlobReference(destinationBlobname);

                try
                {
                    // blob exists
                    // this validation generates a traceable error that cant be hidden in the log

                    blobDest.FetchAttributes();

                    destinationBlobname += Guid.NewGuid().ToString().Replace("-", "");

                    blobDest = (CloudBlockBlob)blobStorage.GetBlobReferenceFromServer(new Uri(destinationBlobname));

                }
                catch
                {

                }


                blobDest.StartCopyFromBlob(blob);
                blob.DeleteIfExists();
            }
            catch
            {
                Trace.TraceInformation("Thread [" + sourceBlobname + "] error deleting queue msg ");
            }



        }




        public static bool processBlob(CloudBlockBlob blob, string servername, string blobURL, LookupService ls, CloudStorageAccount  storageAccount)
        {

            bool ret = false;
            //download blob to stream
            MemoryStream blobcontents = new MemoryStream();

            DateTime s1 = DateTime.Now;
            //Trace.TraceInformation("starting to download " + DateTime.Now.ToString());

            //Trace.TraceInformation("Blob url = [" + blobURL  + "] starting to download "); 
            try
            {
                blob.DownloadToStream(blobcontents);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Error in process datelog " + ex.Message);
            }

             //check for zero length blobs


            //Trace.TraceInformation("Blob url = [" + blobURL + "] getting hash "); 
            string md5v = crypt.GetMD5HashFromFile (blobcontents); // System.BitConverter.ToString(retVal).Replace("-", "").Replace(" ", "");

            string savedMD5 = blob.Metadata["MD5"].ToString().Replace("-", "").Replace(" ", "");

            var data = new DataTable();

            if (md5v == savedMD5)
            {

                //  Trace.TraceInformation("Blob url = [" + blobURL + "] same hash ");
                //Trace.TraceInformation("start process file " + DateTime.Now.ToString());

                //move log content to a string array
                blobcontents.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(blobcontents);
                string[] textContent = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                //    Trace.TraceInformation("Blob url = [" + blobURL + "] text processed ");



                string SoftwareValue = "";
                string VersionValue = "";
                string StartDateValue = "";
                int Servicetype = 0;
                char separator = ' ';

                //Trace.TraceInformation("Blob url = [" + blobURL + "] service type ");

                switch (blob.Metadata["FILETYPE"].ToString())
                {
                    case "DSS":
                        {
                            Servicetype = 1;
                            separator = ' ';
                        }
                        break;
                    case "Windows Media":
                        {
                            Servicetype = 3;
                            separator = ' ';
                        }
                        break;
                    case "Wowza":
                        {
                            Servicetype = 2;
                            separator = '\t';
                        }
                        break;
                    case "IIS":
                        {
                            Servicetype = 4;
                            separator = ' ';
                        }
                        break;
                }

                try
                {
                    //    Trace.TraceInformation("Blob url = [" + blobURL + "] process text " + textContent.Length.ToString());
                    bool alreadyCreateColumns = false;
                    foreach (string line in textContent)
                    {

                        //log header fields
                        if (line.ToLower().IndexOf(SoftwareStr) >= 0)
                        {
                            SoftwareValue = line.Substring(SoftwareStr.Length);
                        }
                        if (line.ToLower().IndexOf(VersionStr) >= 0)
                        {
                            VersionValue = line.Substring(VersionStr.Length);
                        }
                        if (line.ToLower().IndexOf(StartDateWMSStr) >= 0)
                        {
                            StartDateValue = line.Substring(StartDateWMSStr.Length);
                            //        Servicetype = 3;
                        }
                        if (line.ToLower().IndexOf(StartDateWZMStr) >= 0)
                        {
                            StartDateValue = line.Substring(StartDateWZMStr.Length);
                            //          Servicetype = 2;
                        }
                        if (line.ToLower().IndexOf(StartDateDSSStr) >= 0 && line.Length > 17)
                        {
                            StartDateValue = line.Substring(StartDateDSSStr.Length);
                            //            Servicetype = 1;
                        }

                        //log fields definition
                        if (line.ToLower().IndexOf(FieldsStr) >= 0)
                        {
                            if (!alreadyCreateColumns)
                            {

                                alreadyCreateColumns = true;

                                //Trace.TraceInformation("Blob url = [" + blobURL + "] add columns ");
                                var headers = line.Substring(FieldsStr.Length).Trim().Split(separator);
                                foreach (var header in headers)
                                {
                                    //Trace.TraceInformation("Blob url = [" + blobURL + "] column = " + header.ToLower().Replace("-", ""));
                                    data.Columns.Add(header.ToLower().Replace("-", "").Replace("(", "").Replace(")", ""));
                                }
                                data.Columns.Add("logcreatedate", typeof(DateTime));
                                data.Columns.Add("enddatetime", typeof(DateTime));
                                data.Columns.Add("logfilename", typeof(string));
                                data.Columns.Add("harvestedfrom", typeof(string));
                                data.Columns.Add("servername", typeof(string));
                                data.Columns.Add("format_code", typeof(int));
                                data.Columns.Add("longip", typeof(long));
                                data.Columns.Add("countrya2", typeof(string));
                                data.Columns.Add("state", typeof(string));
                                data.Columns.Add("city", typeof(string));
                                data.Columns.Add("country", typeof(string));
                                data.Columns.Add("datelog", typeof(DateTime));
                                data.Columns.Add("longitude", typeof(decimal));
                                data.Columns.Add("latitude", typeof(decimal));
                                data.Columns.Add("dDuration", typeof(int));
                                data.Columns["time"].DataType = typeof(DateTime);
                                data.Columns["date"].DataType = typeof(DateTime);

                            }
                        }

                        //174.132.133.234

                        //log content to stage datatable 

                        if (line.ToLower().IndexOf("/") > 0 && line.ToLower().Substring(0, 1) != "#" && line.IndexOf(separator) >= 0)
                        {

                            try
                            {
                                Trace.TraceInformation("Blob url = [" + blobURL + "] process one line " + line);
                                // detail line, is not a comment, and have a pubpoint /xxxxxx
                                DataRow dr = null;
                                try
                                {
                                    dr = data.Rows.Add(line.Trim().Split(separator));
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation("Error in populating row " + ex.Message);
                                }
                                dr["format_code"] = Servicetype;
                                dr["servername"] = servername;
                                dr["harvestedfrom"] = servername;
                                dr["logfilename"] = blob.Metadata["FULLNAME"].ToString();
                                try
                                {
                                    if (blob.Metadata["CREATIONDATETIME"] != null)
                                    {
                                        dr["logcreatedate"] = blob.Metadata["CREATIONDATETIME"].ToString();
                                    }
                                    else
                                    {
                                        dr["logcreatedate"] = (DateTime)dr["date"];
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation("Error in process logcreatedate  " + ex.Message);
                                }

                                try
                                {
                                    dr["datelog"] = ((DateTime)dr["date"]).AddHours(((DateTime)dr["time"]).Hour).AddMinutes(((DateTime)dr["time"]).Minute).AddSeconds(((DateTime)dr["time"]).Second);
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation("Error in process datelog " + ex.Message);
                                }


                                if (Servicetype == 2)
                                {
                                    // wowza. Request by Will on 2009-06-04
                                    if (dr["xvhost"].ToString() == "mpd-netro-ca")
                                    {
                                        dr["xapp"] = "mpd-netro-ca";
                                    }
                                }

                                //Trace.TraceInformation("Blob url = [" + blobURL + "] get geo object ");

                                // Location l = ls.getLocation("oracle.com");
                                Location l = null;
                                try
                                {
                                    l = ls.getLocation(dr["cip"].ToString());
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation("Error in process geo info 1 " + ex.Message);
                                }
                                try
                                {
                                    if (l != null)
                                    {
                                        dr["countrya2"] = Sql.ToString(l.countryCode);
                                        dr["state"] = Sql.ToString(l.regionName);
                                        dr["city"] = Sql.ToString(l.city);
                                        dr["country"] = Sql.ToString(l.countryName);
                                        dr["latitude"] = Sql.ToDouble(l.latitude);
                                        dr["longitude"] = Sql.ToDouble(l.longitude);
                                    }
                                    else
                                    {
                                        dr["countrya2"] = "0";
                                        dr["state"] = "N/A";
                                        dr["city"] = "N/A";
                                        dr["country"] = "N/A";
                                        dr["latitude"] = 0;
                                        dr["longitude"] = 0;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation("Error in process geo info 2 " + ex.Message);
                                }
                                //Trace.TraceInformation("Blob url = [" + blobURL + "] geo info ok ");

                                try
                                {
                                    if (Sql.IsEmptyString(dr["xduration"]))
                                    {
                                        dr["xduration"] = 1;
                                    }

                                    //Trace.TraceInformation("xduration " + dr["xduration"].ToString());
                                    if (dr["xduration"].Equals("-"))
                                    {
                                        dr["xduration"] = 1;
                                    }
                                    dr["dduration"] = Math.Ceiling(Sql.ToDecimal(dr["xduration"]));
                                    if (dr["dduration"].Equals(0))
                                    {
                                        dr["dduration"] = 1;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation("Error in process duration " + ex.Message);
                                }



                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation("Error in process line " + ex.Message);
                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("Error in process blob " + ex.Message);
                }
                if (data.Rows.Count > 0)
                {
                    //Trace.TraceInformation("Blob url = [" + blobURL + "] call save data ");
                    ret = saveLog(data, s1, blobURL, savedMD5, blob.Metadata["SERVERIP"].ToString(), blob.Metadata["SERVERNAME"].ToString(),  storageAccount, blob);


                }
                else
                {
                    Trace.TraceInformation("file with zero rows ");
                    ret = true;
                }
                data = null;

            }
            else
            {
                Trace.TraceInformation("not the same hash ");
            }
            if (ret)
            {
                return ret;
            }
            else
            {
                return ret;
            }
        }

        private static bool saveLog(DataTable data, DateTime s1, string url, string hash, string serverip, string servername,  CloudStorageAccount  storageAccount, CloudBlockBlob blob)
        {



            bool ret = false;
            string logfilename = "";

            DateTime date_entered = DateTime.Now;
            DateTime hour_log = DateTime.Now;

            SqlConnection dbConnection = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("AzureSqlConnectionString").ToString());

            try
            {
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on data connection " + ex.Message);
            }


            //   SqlTransaction trn = dbConnection.BeginTransaction();

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 1 * FROM LOG_STAGING WHERE pubpoint = '-1'", dbConnection);

            // Create a dataset object
            DataSet ds = new DataSet("LogsSet");
            adapter.Fill(ds, "LOG_STAGING");

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);


            adapter.InsertCommand = builder.GetInsertCommand();
            //  adapter.InsertCommand.Transaction = trn;



            Guid batchid = Guid.NewGuid();

            // Create a data table object and add a new row
            DataTable LogTable = ds.Tables["LOG_STAGING"];


            SqlDataAdapter adapterBatch = new SqlDataAdapter("SELECT * FROM LOG_BATCHES WHERE server_name = '-1' ", dbConnection);
            SqlCommandBuilder builderBatch = new SqlCommandBuilder(adapterBatch);
            DataSet dsBatch = new DataSet("LogsSetBatch");
            adapterBatch.Fill(dsBatch, "LOG_BATCHES");
            DataTable LogTableBatch = dsBatch.Tables["LOG_BATCHES"];

            SqlDataAdapter adapterLFiles = new SqlDataAdapter("SELECT * FROM LOG_FILES WHERE server_ip = '-1' ", dbConnection);
            SqlCommandBuilder builderLFiles = new SqlCommandBuilder(adapterLFiles);
            DataSet dsLFiles = new DataSet("LogsSetLFiles");
            adapterLFiles.Fill(dsLFiles, "LOG_FILES");
            DataTable LogTableLFiles = dsLFiles.Tables["LOG_FILES"];

            SqlDataAdapter adapterLFRU = new SqlDataAdapter(string.Format("SELECT * FROM LOG_FILE_ROLLUP5 WHERE log_id = '{0}' ", Guid.Empty), dbConnection);
            SqlCommandBuilder builderLFRU = new SqlCommandBuilder(adapterLFRU);
            DataSet dsLFRU = new DataSet("LogsSetLFRU");
            adapterLFRU.Fill(dsLFRU, "LOG_FILE_ROLLUP5");
            DataTable LogTableLFRU = dsLFRU.Tables["LOG_FILE_ROLLUP5"];


            //Trace.TraceInformation(" start to save " + DateTime.Now.ToString());


            foreach (DataRow dr in data.Rows)
            {

                try
                {
                    switch (dr["format_code"].ToString())
                    {
                        case "3":
                            if (dr["cstatus"].ToString().Trim() != "200" || dr["scbytes"].ToString().Trim() == "-")
                            {
                                continue;
                            }
                            break;
                    }

                    DataRow row = LogTable.NewRow();

                    try
                    {
                        row["logfilename"] = dr["logfilename"];

                        row["logcreatedate"] = dr["logcreatedate"];

                        logfilename = dr["logfilename"].ToString();
                        hour_log = DateTime.Parse(dr["logcreatedate"].ToString());
                        // put a 0 milisecs, secs and minutes to get a clean -HH datetime
                        hour_log = hour_log.AddMilliseconds(-hour_log.Millisecond);
                        hour_log = hour_log.AddSeconds(-hour_log.Second);
                        hour_log = hour_log.AddMinutes(-hour_log.Minute);
                        // truncate hours to get a clean day :00:00
                        date_entered = date_entered.AddHours(-hour_log.Hour);


                        row["format_code"] = dr["format_code"];

                        //wms
                        row["cip"] = dr["cip"];
                    }

                    catch (Exception ex)
                    {
                        Trace.TraceInformation("Error in datetime " + ex.Message);
                    }

                    switch (dr["format_code"].ToString())
                    {
                        case "1":
                            // dss
                            try
                            {
                                row["datetime"] = ((DateTime)dr["datelog"]).AddSeconds(Sql.ToDouble(dr["dduration"]) * -1);

                                row["enddatetime"] = dr["datelog"];
                                row["pubpoint"] = stringfunctions.fnPubPoint(dr["csuristem"].ToString());
                                
                                row["protocol"] = dr["protocol"];                                
                                
                                
                                row["csuseragent"] = dr["csuseragent"].ToString().Length > 250 ? dr["csuseragent"].ToString().Substring(0, 250) : dr["csuseragent"];
                                row["cplayerid"] = dr["csuseragent"];
                                row["csuristem"] = stringfunctions.fnContent(dr["csuristem"].ToString());
                                row["csuristem"] = row["csuristem"].ToString().Length > 250 ? row["csuristem"].ToString().Substring(0, 250) : row["csuristem"];
                                

                                row["csmedianame"] = stringfunctions.fnPubPoint(dr["csuristem"].ToString());
                                row["csmedianame"] = row["csmedianame"].ToString().Length > 250 ? row["csmedianame"].ToString().Substring(0, 250) : row["csmedianame"];
                                row["csurl"] = row["csmedianame"] ;
                                row["sip"] = row["csmedianame"];
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation("Error in datetime " + ex.Message);
                            }

                            break;
                        case "2":
                            //wowza 
                            try
                            {
                                row["datetime"] = ((DateTime)dr["datelog"]).AddSeconds(Sql.ToDouble(dr["dduration"]) * -1);

                                row["enddatetime"] = dr["datelog"];
                                row["pubpoint"] = dr["xapp"];
                                
                                row["sip"] = stringfunctions.fnServerIP(dr["xsuri"].ToString());
                                
                                row["protocol"] = dr["cproto"];
                                
                                row["csuseragent"] = dr["cuseragent"].ToString().Length > 250 ? dr["cuseragent"].ToString().Substring(0, 250) : dr["cuseragent"];
                                row["cplayerid"] = row["csuseragent"];
                                row["csuristem"] = stringfunctions.fnContent(dr["csuristem"].ToString());
                                row["csuristem"] = row["csuristem"].ToString().Length > 250 ? row["csuristem"].ToString().Substring(0, 250) : row["csuristem"];
                                row["csurl"] = dr["xsuri"].ToString().Length > 250 ? dr["xsuri"].ToString().Substring(0, 250) : dr["xsuri"];
                                row["csmedianame"] = dr["xsuriquery"].ToString().Length > 250 ? dr["xsuriquery"].ToString().Substring(0, 250) : dr["xsuriquery"];

                                
                            }

                            catch (Exception ex)
                            {
                                Trace.TraceInformation("Error in datetime " + ex.Message);
                            }
                            break;
                        case "3":
                            try
                            {
                                row["pubpoint"] = stringfunctions.fnPubPoint(dr["csuristem"].ToString());
                                row["datetime"] = dr["datelog"];
                                row["enddatetime"] = ((DateTime)dr["datelog"]).AddSeconds(Sql.ToDouble(dr["dduration"]));

                                row["sip"] = stringfunctions.fnServerIP(dr["csurl"].ToString());
                                row["protocol"] = dr["protocol"];

                                row["csuseragent"] = dr["csuseragent"].ToString().Length > 250 ? dr["csuseragent"].ToString().Substring(0, 250) : dr["csuseragent"];
                                row["cplayerid"] = dr["csuseragent"];
                                row["csuristem"] = stringfunctions.fnContent(dr["csurl"].ToString());
                                row["csuristem"] = row["csuristem"].ToString().Length > 250 ? row["csuristem"].ToString().Substring(0, 250) : row["csuristem"];
                                row["csurl"] = dr["csurl"].ToString().Length > 250 ? dr["csurl"].ToString().Substring(0, 250) : dr["csurl"];
                                row["csmedianame"] = dr["csmedianame"].ToString().Length > 250 ? dr["csmedianame"].ToString().Substring(0, 250) : dr["csmedianame"];

                            }

                            catch (Exception ex)
                            {
                                Trace.TraceInformation("Error in datetime " + ex.Message);
                            }
                            break;
                        case "4":
                            row["pubpoint"] = stringfunctions.fnPubPoint(dr["csuristem"].ToString());
                            row["datetime"] = dr["datelog"];
                            try
                            {
                                row["enddatetime"] = ((DateTime)dr["datelog"]).AddSeconds(Sql.ToDouble(dr["dduration"]));
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation("Error in datetime " + ex.Message);
                            }
                            row["protocol"] = dr["protocol"];

                            row["csuristem"] = stringfunctions.fnContent(dr["csurl"].ToString());
                            row["csuristem"] = row["csuristem"].ToString().Length > 250 ? row["csuristem"].ToString().Substring(0, 250) : row["csuristem"];
                            row["csurl"] = dr["csurl"].ToString().Length > 250 ? dr["csurl"].ToString().Substring(0,250) : dr["csurl"];                            
                            row["csmedianame"] = dr["csmedianame"].ToString().Length > 250 ? dr["csmedianame"].ToString().Substring(0,250) : dr["csmedianame"];

                            ;

                            break;
                    }



                    row["cstarttime"] = 0;
                    row["xduration"] = dr["dduration"];
                    row["cstatus"] = 200;

                    row["cplayerversion"] = " ";

                    row["cos"] = " ";
                    row["avgbandwidth"] = 0;

                    row["transport"] = " ";
                    try
                    {
                        if (dr["scbytes"].ToString() == "-")
                        {
                            dr["scbytes"] = "1";
                        }
                        row["KiloBytesSent"] = Sql.ToLong(dr["scbytes"].ToString()) / 1024;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceInformation("Error in datetime " + ex.Message);
                    }
                    row["cbytes"] = 0;

                    row["server"] = serverip;
                    row["stotalclients"] = 0;
                    row["scontentpath"] = " ";


                    row["cMaxBandwidth"] = 0;
                    row["csmediarole"] = " ";
                    row["sproxied"] = 0;
                    row["harvestedfrom"] = dr["servername"];
                    row["longip"] = stringfunctions.fnIPStringToNumber(dr["cip"].ToString());
                    row["countrya2"] = dr["countrya2"];
                    //    row["enddatetime"] = dr["enddatetime"];
                    row["state"] = dr["state"];
                    row["city"] = dr["city"];
                    row["country"] = dr["country"];
                    row["long"] = dr["longitude"];
                    row["lat"] = dr["latitude"];
                    //row["longitude"] = dr["longitude"];
                    //row["latitude"] = dr["latitude"];

                    row["server_name"] = dr["servername"];
                    row["batch_id"] = batchid;
                    row["batch_completed"] = 0;

                    row["day"] = DateTime.Parse(row["datetime"].ToString()).Day;
                    row["month"] = DateTime.Parse(row["datetime"].ToString()).Month;
                    row["year"] = DateTime.Parse(row["datetime"].ToString()).Year;
                    row["hour"] = DateTime.Parse(row["datetime"].ToString()).Hour;
                    row["processedrollup"] = 0;



                    LogTable.Rows.Add(row);

                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on add row " + ex.Message);
                }

            }

            Trace.TraceInformation("total rows to insert " + LogTable.Rows.Count.ToString());

            if (LogTable.Rows.Count == 0)
            {
                // the file had rows, but none of it was stop event
                ret = true;
                goto zeroRows;
            }


            //saveAzureLog(LogTable);

            //return true;

            DateTime s2 = DateTime.Now;

            /*     SqlDataAdapter adapterHRU = new SqlDataAdapter(string.Format("SELECT * FROM HOURLY_ROLLUP WHERE hour_dt = '{0}' ",hour_log ), dbConnection);
                 SqlCommandBuilder builderHRU = new SqlCommandBuilder(adapterHRU);
                 DataSet dsHRU = new DataSet("LogsSetHRU");
                 adapterHRU.Fill(dsHRU, "HOURLY_ROLLUP");
                 DataTable LogTableHRU = dsHRU.Tables["HOURLY_ROLLUP"];
                 */
            long connLog = 0;
            double kbLog = 0;


            try
            {


                try
                {

                    foreach (DataRow dr in LogTable.Rows)
                    {
                        if (Sql.IsEmptyString(dr["KiloBytesSent"]))
                        {
                            dr["KiloBytesSent"] = 1;
                        }
                    }

                    connLog = LogTable.Rows.Count;
                    kbLog = Convert.ToDouble(LogTable.Compute("sum(KiloBytesSent)", null)); // "KiloBytesSent > 0"));

                    //DataRow[] hourlypps = LogTable.Select("");
                    var result = from row in LogTable.AsEnumerable()
                                 group row by row["pubpoint"] into temp
                                 select new
                                 {
                                     pp = temp.Key.ToString(),
                                     tr = temp.Count(),
                                     kb = temp.Sum(t => Convert.ToInt64(t["KiloBytesSent"]))
                                 };
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on sum " + ex.Message);
                }


                // Update data adapter
                //Trace.TraceInformation("Send log staging " + DateTime.Now.ToString());

                try
                {
                    adapter.Update(ds, "LOG_STAGING");
                }
                catch (Exception ex)
                {
                    //pub the table into a csv string, so i can debug in my dev station
                    StringBuilder sb = new StringBuilder();

                    string[] columnNames = LogTable.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName).
                                                      ToArray();
                    sb.AppendLine(string.Join("\t", columnNames));

                    foreach (DataRow row in LogTable.Rows)
                    {
                        string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                        ToArray();
                        sb.AppendLine(string.Join("\t", fields));
                    }

                    String resx = sb.ToString();


                    Trace.TraceInformation(logfilename + " err on log_staging " + ex.Message + " " + resx);
                }

                //Trace.TraceInformation("send log batch" + DateTime.Now.ToString());

                DataRow rowBatch = LogTableBatch.NewRow();
                try
                {
                    rowBatch["BATCH_ID"] = batchid;
                    rowBatch["BATCH_COMPLETED"] = 1;
                    rowBatch["SERVER_NAME"] = servername;
                    LogTableBatch.Rows.Add(rowBatch);
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on sum " + ex.Message);
                }

                //adapterBatch.InsertCommand.Transaction = trn;                

                try
                {
                    adapterBatch.Update(dsBatch, "LOG_BATCHES");
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on log_batches " + ex.Message);
                }

                Guid logid = Guid.NewGuid();

                DataRow rowLFiles = LogTableLFiles.NewRow();
                try
                {
                    rowLFiles["LOG_ID"] = logid;
                    rowLFiles["DATE_ENTERED"] = DateTime.UtcNow;
                    rowLFiles["HASH"] = hash;
                    rowLFiles["LOGFILENAME"] = logfilename;
                    rowLFiles["SERVER_NAME"] = servername;
                    rowLFiles["SERVER_IP"] = serverip;
                    rowLFiles["LOG_URL"] = url.Replace("netrologs5", "netrologsprocessed5");

                    LogTableLFiles.Rows.Add(rowLFiles);
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on sum " + ex.Message);
                }
                //   adapterLFiles.InsertCommand.Transaction = trn;
                try
                {
                    adapterLFiles.Update(dsLFiles, "LOG_FILES");
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on log_files " + ex.Message);
                }


                DataRow rowLFRU = LogTableLFRU.NewRow();
                try
                {
                    rowLFRU["LOG_ID"] = logid;
                    rowLFRU["DATE_ENTERED"] = date_entered;
                    rowLFRU["CONNECTIONS"] = connLog;
                    rowLFRU["KILOBYTES_SENT"] = kbLog;
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on sum " + ex.Message);
                }
                try
                {
                    LogTableLFRU.Rows.Add(rowLFRU);
                    // adapterLFRU.InsertCommand.Transaction = trn;
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on log_file roll up " + ex.Message);
                }
                try
                {
                    adapterLFRU.Update(dsLFRU, "LOG_FILE_ROLLUP5");
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("err on sum " + ex.Message);
                }

                try
                {
                    //update back the blob
                    blob.Metadata["BATCHID"] = batchid.ToString();
                    blob.SetMetadata();

                }
                catch { }

                //hour_log = hour_log.AddMinutes(-hour_log.Minute);
                // truncate hours to get a clean day :00:00
                //date_entered = date_entered.AddHours(-date_entered.Hour);


                ret = true;

                //trn.Commit();

            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on updating db " + ex.Message);
                //  trn.Rollback();
            }

            
            //Trace.TraceInformation( string.Format(  "{4} rows. started at {0} finished at {1} , Total secs {2} save secs {3} ",s1.ToString(),  s3.ToString(), secs, secs1,data.Rows.Count ));
            /*
            try
            {
                azurestorage.saveAzureLog(LogTable, LogTableBatch, LogTableLFiles, LogTableLFRU, storageAccount);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on saving azure storage " + ex.Message);
            }
            */

        zeroRows:

            try
            {

                adapterBatch = null;
                LogTableBatch = null;
                dsBatch = null;
                builderBatch = null;

                adapter = null;
                LogTable = null;
                ds = null;
                //builder = null;

                dbConnection.Close();
                dbConnection = null;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("err on sum " + ex.Message);
            }



            return ret;

        }


        #endregion blobmanagement


    }
}
