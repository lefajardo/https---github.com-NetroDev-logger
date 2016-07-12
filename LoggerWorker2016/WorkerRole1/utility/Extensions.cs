using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core;

public static class BlobExtensions
    {
        public static bool Exists(this CloudBlockBlob blob)
        {
            try
            {
                if (blob.Exists())
                //blob.FetchAttributes();
                return true;
            }
            catch (StorageException e)
            {
            return false;
                /*if (  e.e   //e.ErrorCode ==  StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }*/
            }
        return false;
    }
    }
