using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
/*using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;
*/
/*
using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.Diagnostics;
//using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Shared;
using Microsoft.WindowsAzure.Storage.Table;
*/
using System.Security.Cryptography ;
using System.Diagnostics;
using NetroMedia.SharedFunctions;
using System.Data;
using System.Data.SqlClient;

using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Azure.Storage.Queues;
using Azure.Storage.Blobs.Models;

namespace NetroAzureUpload
{
    //LogFiles
    //ID | DATE_ENTERED | DATE_MODIFIED | HASH | FILE_NAME | SERVER_NAME | SERVER_ID | LOG_URL

    public class DuplicatedLogFiles : TableEntity // TableServiceEntity
    {

        public string SERVICE_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_ID { get; set; }
        public string HASH { get; set; }        
        public string DATE_ENTERED { get; set; }
        public string PROCESSED { get; set; }
        

        public DuplicatedLogFiles()
        { }
    }

        public class UploadedLogFiles : TableEntity //TableServiceEntity
    {

        public string SERVICE_TYPE { get; set; }
        public string FILE_NAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_ID { get; set; }
        public string LOG_URL { get; set; }        
        public string HASH { get; set; }
        public string PROCESSED { get; set; }
        public string DATE_ENTERED { get; set; }
        public string DATE_MODIFIED { get; set; }

        public UploadedLogFiles(string SERVICE_TYPE, string HASH)
            {
                this.PartitionKey = SERVICE_TYPE;
                this.RowKey = HASH;

            }

         public UploadedLogFiles() { }

    }

        public class ServerInventory : TableEntity //TableServiceEntity
    {

            public string SERVICE_TYPE { get; set; }
            public string SERVER_ID { get; set; }
            public string SERVER_NAME { get; set; }
            public string LAST_FILE_UPLOAD { get; set; }
            public string LAST_FILE_UP_DATETIME { get; set; }
            public string LAST_FILE_PROCESS { get; set; }
            public string LAST_FILE_PROC_DATETIME { get; set; }

            public ServerInventory(string SERVICE_TYPE, string SERVER_ID)
            {
                this.PartitionKey = SERVICE_TYPE;
                this.RowKey = SERVER_ID ;

            }

            public ServerInventory() { }

        }

    class blobinfo_
    {
        public CloudBlockBlob blob;
        public string filename;
        public string MD5;
        public string blobname;
        public string creationDatetime;
        public string serverIP;
        public string connstring;
        public string servername;
        public string filetype;
    }
    public class UploadFiles
    {
        BlobContainerClient blobStorage;
        string ConfigSetterString = "";
        string blobRoot = "";
        string logsRoot = "";
        CloudStorageAccount account;
        QueueClient queue;
        string aztTableName = "UPLOADEDLOGFILES5";
        string dupTableName = "DUPLICATEDLOGFILES5";
        string serverinvTableName = "SERVERINVENTORY";



        DataTable LoadTable(string query, string connect)
        {
            DataTable dTable = new DataTable();
            if (query == "")
            {
                return dTable;
            }
            SqlConnection dbConnection = new SqlConnection(connect);
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, dbConnection);

            dbConnection.Open();
            dAdapter.Fill(dTable);
            dbConnection.Close();

            return dTable;
        }

        void ExecuteSql(string query, string connect)
        {
            if (query == "")
            {
                return ;
            }
            SqlConnection dbConnection = new SqlConnection(connect);
            dbConnection.Open();
            SqlCommand cmd = new SqlCommand(query, dbConnection);
            cmd.ExecuteNonQuery();

            dbConnection.Close();

            
        }


        string processkey(string value) {
            return value.Replace(" ", "-").Replace("_", "-").Replace("/", "-").Replace(@"\", "-");
        }

        public bool fileAlreadyProcessed(string filetype, string filename, string serverip, string md5, string connstring, string servername) {
            bool rt = false;
            /*
            CloudTableClient ctc = new CloudTableClient( new Uri( account.TableEndpoint.AbsoluteUri), account.Credentials);
            
            var cloudtable = ctc.GetTableReference(aztTableName);
            cloudtable.CreateIfNotExists();
            */

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstring);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.

            CloudTable cloudTable = tableClient.GetTableReference(aztTableName);
            cloudTable.CreateIfNotExists();

            try
            {


                // Construct the query operation for all customer entities where PartitionKey="Smith".
                TableQuery<UploadedLogFiles> query = 
                    new TableQuery<UploadedLogFiles>().Where(
                      TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, filetype),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, md5)
                     )
                );

                foreach (UploadedLogFiles table in cloudTable.ExecuteQuery(query))
                {

                    /*
                    UploadedLogFiles table = (from e in tsc.CreateQuery<UploadedLogFiles>(aztTableName)
                                       where e.PartitionKey == filetype && e.RowKey == md5 // && e.md5 == md5
                                       select e).FirstOrDefault();
                    */
                    //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format( "find in uploaded logs fir {0} and -{1}- " , filetype , md5));

                    //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("table hash value =  -{0}-  ", table.HASH.Trim()));

                    string hs = table.HASH.Trim();
                    if (hs == md5.Trim())
                    {

                        Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("record already exists. filetype ={0}  md5 = {1} serverid={2}  servername={3}  date={4} name={5} ", filetype, md5, table.SERVER_ID, table.SERVER_NAME, table.DATE_ENTERED, table.FILE_NAME));

                        addfileToDupeLog(filetype, filename, serverip, servername, md5, connstring);

                        rt = true;
                    }

                    break;
                }
                    /*
                else
                {
                    //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("hash not found  "));
                }*/
            }
            catch (Exception ex ) {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, string.Format("error in hash compare {0} ", ex.Message ));
            }




            return rt;
        }

        void addfileToDupeLog(string filetype, string filename_, string serverip, string servername, string md5_, string connstring)
        {

            //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "add file to dupe log " + filename_);
            try
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstring);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.

                CloudTable cloudTable = tableClient.GetTableReference(dupTableName);
                cloudTable.CreateIfNotExists();

                /*
                CloudTableClient ctc = new CloudTableClient(new Uri( account.TableEndpoint.ToString()), account.Credentials);


                var cloudtable = ctc.GetTableReference(dupTableName);
                cloudtable.CreateIfNotExists();
                */

                serverip = processkey(serverip);
                filename_ = processkey(filename_);

                DuplicatedLogFiles lgs = new DuplicatedLogFiles();


                lgs.SERVICE_TYPE = filetype;
                lgs.FILE_NAME = filename_;
                lgs.SERVER_NAME = servername;
                lgs.SERVER_ID = serverip;

                lgs.HASH = md5_;
                lgs.PROCESSED = "N";
                lgs.DATE_ENTERED = DateTime.UtcNow.ToString();

                TableBatchOperation batchOperation = new TableBatchOperation();
                batchOperation.Insert(lgs);
                cloudTable.ExecuteBatch(batchOperation);
                //  Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "upd 3 " + filename_);
                //tsc.AddObject(dupTableName, lgs);
                //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "upd 4" + filename_);
                //tsc.SaveChanges();


                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Added to duplicate log table");
            }
            catch {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "file already exists in dupe log");
            }
        }



        void UpdateProcessedFile(string filetype, string filename_, string serverip,  string servername, string bloburl, string connstring, string md5_)
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstring);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            
            CloudTable cloudTable = tableClient.GetTableReference(aztTableName);
            cloudTable.CreateIfNotExists();

            
            
            

            
/*            CloudTableClient ctc = new CloudTableClient( new Uri( account.TableEndpoint.ToString()), account.Credentials);            
            var cloudtable = ctc.GetTableReference(aztTableName);
            cloudtable.CreateIfNotExists();
*/


            

            serverip = processkey(serverip);
            filename_ = processkey(filename_);

            UploadedLogFiles lgs = new UploadedLogFiles(filetype , md5_);
            
                lgs.SERVICE_TYPE = filetype;
                lgs.FILE_NAME = filename_;
                lgs.SERVER_NAME = servername;
                lgs.SERVER_ID = serverip;
                lgs.LOG_URL = bloburl;
                lgs.HASH = md5_;
                lgs.PROCESSED = "N";
                lgs.DATE_ENTERED = DateTime.UtcNow.ToString();
                lgs.DATE_MODIFIED = "-";

            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(lgs);

            cloudTable.ExecuteBatch(batchOperation);

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "File uploaded to logs table");
            
        }

        public void UpdateServerInventory
            (string filetype, string filename_, string serverip, string servername, string fileDateTime, string connstring)
        {


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstring);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.

            CloudTable cloudTable = tableClient.GetTableReference(serverinvTableName);
            cloudTable.CreateIfNotExists();

            /*
            CloudTableClient ctc = new CloudTableClient( new Uri( account.TableEndpoint.ToString()), account.Credentials);
      
            var cloudtable = ctc.GetTableReference(aztTableName);
            cloudtable.CreateIfNotExists();
            */


            serverip = processkey(serverip);
            filename_ = processkey(filename_);


            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<ServerInventory>(filetype, serverip);

                // Execute the operation.
                TableResult retrievedResult = cloudTable.Execute(retrieveOperation);

                // Assign the result to a CustomerEntity object.
                ServerInventory table = (ServerInventory)retrievedResult.Result;

                if (table != null)
                {
                    table.LAST_FILE_UP_DATETIME = fileDateTime;
                    table.LAST_FILE_UPLOAD = filename_;
                    TableOperation updateOperation = TableOperation.Replace(table);

                    // Execute the operation.
                    cloudTable.Execute(updateOperation);
                }
                  
            }
            catch
            {
                // record is new
                ServerInventory lgs = new ServerInventory(filetype, serverip);

                lgs.SERVICE_TYPE = filetype;
                lgs.SERVER_NAME = servername;
                lgs.SERVER_ID = serverip;
                lgs.LAST_FILE_UP_DATETIME = fileDateTime;
                lgs.LAST_FILE_UPLOAD = filename_;


                TableBatchOperation batchOperation = new TableBatchOperation();
                batchOperation.Insert(lgs);
                cloudTable.ExecuteBatch(batchOperation);

            }


            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "File uploaded to logs table");

        }

        public UploadFiles()
        { 
        }
        public UploadFiles(string configSetter_, string blobRoot_, string logsRoot_ )
        { 
        
            ConfigSetterString = configSetter_;
            blobRoot = blobRoot_;
            logsRoot = logsRoot_;

            BlobContainerClient rootContainer = Initialize( logsRoot_ );

        }

        public void upload(string path, string filesPattern, string currentFile, string configSetter_, string blobRoot_, string logsRoot_, string serverIp, string processCurrentFile, string connString, string filetype)
        {

            ConfigSetterString = configSetter_;
            blobRoot = blobRoot_;
            logsRoot = logsRoot_;

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Initialize container " + logsRoot_ );

            BlobContainerClient rootContainer = Initialize( logsRoot_ );

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Get Directory reference " + path);

            DirectoryInfo directoryContents = new DirectoryInfo(path);

            foreach (FileInfo fileCurrent in directoryContents.GetFiles(filesPattern))
            {

                try
                {

                    string md5 = GetMD5HashFromFile(fileCurrent.FullName);

                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Starting to Process " + fileCurrent.FullName + " creation time = " + fileCurrent.CreationTimeUtc.ToString());

                    if (path.ToLower() + fileCurrent.Name.ToLower() != currentFile.ToLower())
                    {
                      //  if (!fileAlreadyProcessed(filetype, fileCurrent.FullName, serverIp, md5, connString, Environment.MachineName))
                       // {

                            UploadToAzure(fileCurrent.Name, fileCurrent.FullName, Environment.MachineName, fileCurrent.CreationTimeUtc, serverIp, ConfigSetterString, md5, filetype);

                        //}
                    }
                    else
                    {

                        if (processCurrentFile == "Y")
                        {
                            if (!fileAlreadyProcessed(filetype, fileCurrent.FullName, serverIp, md5, ConfigSetterString, Environment.MachineName))
                            {
                                
                                    UploadToAzure(fileCurrent.Name, fileCurrent.FullName, Environment.MachineName, fileCurrent.CreationTimeUtc, serverIp, ConfigSetterString, md5, filetype);
                                
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error on upload to azure[" + fileCurrent.Name + "] " + ex.Message);

                }
            }
            GC.Collect();
        }

        public void UploadToAzure(string filename, string fullname, string serverName, DateTime creationTime, string serverIP , string connString, string md5, string filetype )
        {

            string sourcefilefullname = blobRoot + "/" + logsRoot + "/"  + serverName + "/" +   filename ;
            string sourcefile =  serverName + "/" + filename;

            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Upload from: [" + fullname + "] To: [" + sourcefile  + "]");


            BlobClient blob = blobStorage.GetBlobClient(sourcefile);
            if (!blob.Exists())
            {

                blob.Upload(fullname);
                IDictionary<string, string> metadata = new Dictionary<string, string>();

                BlobHttpHeaders headers = new BlobHttpHeaders
                {
                    // Set the MIME ContentType every time the properties 
                    // are updated or the field will be cleared
                    ContentType = "text/plain",
                    ContentLanguage = "en-us",
                    
                };

                blob.SetHttpHeaders(headers);

                
                metadata["MD5"] = md5;
                metadata["FULLNAME"] = fullname;
                metadata["BATCHID"] = "0";
                metadata["FILETYPE"] = filetype;
                metadata["SERVERIP"] = serverIP;
                metadata["SERVERNAME"] = serverName;
                metadata["CREATIONDATETIME"] = string.Format("{0:s}", creationTime); 

                blob.SetMetadata(metadata);

                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, " queue info[" + queue.Uri + "] ");

                try
                {
                    
                    queue.SendMessage(sourcefilefullname);

                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Added to queue file[" + blob.Name + "] ");
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder(ex.Message);
                    Exception e = ex.InnerException;
                    while (e != null)
                    {
                        sb.AppendLine("--->");
                        sb.AppendLine(e.Message);

                        e = e.InnerException;

                    }
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error adding to queue  file[" + blob.Name + "] " + sb.ToString());
                }
                try
                {


                    UpdateProcessedFile(filetype, fullname, serverIP, serverName, blob.Name, connString, md5);

                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "update processed file[" + blob.Name + "] ");
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder(ex.Message);
                    Exception e = ex.InnerException;
                    while (e != null)
                    {
                        sb.AppendLine("--->");
                        sb.AppendLine(e.Message);

                        e = e.InnerException;

                    }
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error updating processed file  file[" + blob.Name + "] " + sb.ToString());
                }
                try
                {


                    UpdateServerInventory(filetype, fullname, serverIP, serverName, string.Format("{0:s}", DateTime.UtcNow),connString);

                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "update processed file[" + blob.Name + "] ");
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder(ex.Message);
                    Exception e = ex.InnerException;
                    while (e != null)
                    {
                        sb.AppendLine("--->");
                        sb.AppendLine(e.Message);

                        e = e.InnerException;

                    }
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error updating inventory file[" + blob.Name + "] " + sb.ToString());
                }
                GC.Collect();


            }

        }
        /*
        void PutOperationCompleteCallback(IAsyncResult result)
        {

            blobinfo_ bi = (blobinfo_)result.AsyncState;
            //Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Finished upload async file[" + bi.filename + "] " );
            string filename = bi.filename;
            bi.blob.EndUploadFromStream(result);

            bi.blob.Properties.ContentType = "text/plain";
            bi.blob.Metadata["MD5"] = bi.MD5 ;
            bi.blob.Metadata["FULLNAME"] = bi.filename;
            bi.blob.Metadata["BATCHID"] = "0";
            bi.blob.Metadata["FILETYPE"] = bi.filetype ;
            bi.blob.Metadata["SERVERIP"] = bi.serverIP ;
            bi.blob.Metadata["SERVERNAME"] = bi.servername ;
            bi.blob.Metadata["CREATIONDATETIME"] = bi.creationDatetime ;
            bi.blob.SetMetadata();
//            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Adding to queue file[" + bi.blobname  + "] ");
            try
            {

                CloudQueueMessage msg = new CloudQueueMessage(bi.blobname);
                queue.SendMessage(bi.blobname);
                msg = null;

                
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Added to queue file[" + bi.blobname + "] ");
            }
            catch (Exception ex) {
                StringBuilder sb = new StringBuilder( ex.Message );
                Exception e = ex.InnerException;
                while (e != null)
                {
                    sb.AppendLine("--->");
                    sb.AppendLine(e.Message);

                    e = e.InnerException;

                }
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error adding to queue  file[" + bi.blobname + "] " + sb.ToString() );
            }
            try
            {

                
                UpdateProcessedFile(bi.filetype, bi.filename, bi.serverIP, bi.servername, bi.blobname, bi.connstring, bi.MD5);

                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "update processed file[" + bi.blobname + "] ");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(ex.Message);
                Exception e = ex.InnerException;
                while (e != null)
                {
                    sb.AppendLine("--->");
                    sb.AppendLine(e.Message);

                    e = e.InnerException;

                }
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error adding to queue  file[" + bi.blobname + "] " + sb.ToString());
            }
            try
            {


                UpdateServerInventory(bi.filetype, bi.filename, bi.serverIP, bi.servername,string.Format("{0:s}",DateTime.UtcNow),con);

                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "update processed file[" + bi.blobname + "] ");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(ex.Message);
                Exception e = ex.InnerException;
                while (e != null)
                {
                    sb.AppendLine("--->");
                    sb.AppendLine(e.Message);

                    e = e.InnerException;

                }
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Error adding to queue  file[" + bi.blobname + "] " + sb.ToString());
            }
            GC.Collect();
        }
        */
        void bcreateQueue() {

            queue = new QueueClient(ConfigSetterString, "netrologsqueue5");

            //CloudQueueClient queueStorage = account.CreateCloudQueueClient();

            //queueStorage.GetQueueReference("netrologsqueue5");
            //Trace.TraceInformation("Creating container and queue...");

            bool containerAndQueueCreated = false;
			while (!containerAndQueueCreated)
			{
				try
				{
                    queue.CreateIfNotExists();// .CreateIfNotExist();
					containerAndQueueCreated = true;
				}
				catch (StorageException e)
				{
                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Connection failure[" + e.InnerException  + "] " );
                     
				}
				catch //(Exception ex)
				{
					
				}
			}

        }

        public string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            byte[] retVal = sha512.ComputeHash(file);

            
            file.Close();

            return System.BitConverter.ToString(retVal ).Replace("-","").Replace(" ","");
            
        }

        BlobContainerClient Initialize(string containerName)
        {


            Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Initialize Azure");
            try
            {
                if (containerName != blobRoot)
                {

                    BlobServiceClient blobServiceClient = new BlobServiceClient(ConfigSetterString);
                    
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                    blobStorage = containerClient;

                    /*
                    CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                    {
                        configSetter(ConfigSetterString); 

                    });
                    */
                    /*
                    //account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                    account = CloudStorageAccount.Parse(ConfigSetterString);

                    blobStorage = account.CreateCloudBlobClient();
                    //container = blobStorage.GetContainerReference(logsContainerName);

                   // account = CloudStorageAccount.FromConfigurationSetting(ConfigSetterString); 

                    //blobStorage = account.CreateCloudBlobClient();
                    //blobStorage.RetryPolicy = RetryPolicies.RetryExponential(5, TimeSpan.FromMilliseconds(1000));
                    CloudBlobContainer BlobContainer = blobStorage.GetContainerReference(containerName);

                    BlobRequestOptions ro = new BlobRequestOptions();
                    //ro.Timeout = new TimeSpan(0, 1, 0);

                    BlobContainer.CreateIfNotExists();// .CreateIfNotExist();

                    BlobContainerPermissions permissions = new BlobContainerPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    BlobContainer.SetPermissions(permissions);
                    */

                    bcreateQueue();

                    Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Azure blob & queue containers initialized");

                    return containerClient;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("NetroLogger5", Environment.MachineName, 0, "Err in Initialize Azure" + ex.Message );
                
                return null;
            }
        }

    }
}