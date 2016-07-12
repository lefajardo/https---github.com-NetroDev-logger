using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;

namespace clearlogs
{ 
    class Program
    {

         static       public CloudBlobContainer Initialize(string containerName)
        {

            if (containerName != blobRoot)
            {

                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    //configSetter("DefaultEndpointsProtocol=https;AccountName=netro;AccountKey=Xdy1RZeapgoJJDbhF6zdQFIxItf+2dJyQu+sjSSbjuzsWecPMBLHzRwEQIF+K89ZEUg8ZpepAuSXL9KCONMpmQ==");

                    configSetter("DefaultEndpointsProtocol=https;AccountName=netro;AccountKey=Xdy1RZeapgoJJDbhF6zdQFIxItf+2dJyQu+sjSSbjuzsWecPMBLHzRwEQIF+K89ZEUg8ZpepAuSXL9KCONMpmQ==");

                });
                CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("DefaultEndpointsProtocol=https;AccountName=netro;AccountKey=Xdy1RZeapgoJJDbhF6zdQFIxItf+2dJyQu+sjSSbjuzsWecPMBLHzRwEQIF+K89ZEUg8ZpepAuSXL9KCONMpmQ==");    //CloudStorageAccount.FromConfigurationSetting("DataConnectionString"); 
                blobStorage = account.CreateCloudBlobClient();
                blobStorage.RetryPolicy = RetryPolicies.RetryExponential(5, TimeSpan.FromMilliseconds(1000));
                CloudBlobContainer BlobContainer = blobStorage.GetContainerReference(containerName); // + containerName);
                //Create the CloudBlobContainer if it does not exist.
                BlobRequestOptions ro = new BlobRequestOptions();
                ro.Timeout = new TimeSpan(0, 1, 0);

                BlobContainer.CreateIfNotExist();

                return BlobContainer;
            }
            else {
                return null;
            }
        }


          static CloudBlobClient blobStorage;
                       static string blobRoot = "http://netro.blob.core.windows.net/"; //
        static void Main(string[] args)
        {
                  

              string pproot = blobRoot + "netrologs";
                //Console.WriteLine("pproot " + pproot + " fn " + filename );

                CloudBlobContainer rootContainer = Initialize(pproot); //"pubpoints");

                //string fileToGetInfo = blobRoot + filename;

                //string ppsubdir = getSubdir(filename);

                CloudBlobDirectory topLevelDirectory =
                    blobStorage.GetBlobDirectoryReference(blobRoot + "netrologs");
                IEnumerable<IListBlobItem> listBlobItems = topLevelDirectory.ListBlobs();
                foreach (var blobItem in listBlobItems)
                {

                 
                    if (blobItem.GetType() == typeof(CloudBlockBlob))
                    {

                        ((CloudBlockBlob)blobItem).Delete();


                    }
/*                    else
                    {
                        foreach (var blobItem2 in ((CloudBlobDirectory) blobItem).ListBlobs())
                        {
                            ((CloudBlockBlob)blobItem2).Delete();
                        }
                    }
  */                  
                }
            }
        }
    }

