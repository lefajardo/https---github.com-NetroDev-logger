﻿using System;
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

    /*
    public static class AnalyticsSettingsExtension
    {
        static string RequestIdHeaderName = "x-ms-request-id";
        static string VersionHeaderName = "x-ms-version";
        static string Sep2009Version = "2009-09-19";
        static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        #region Analytics
        /// <summary>
        /// Set blob analytics settings
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        public static void SetServiceSettings(this CloudBlobClient client, AnalyticsSettings settings)
        {
            SetSettings(client.BaseUri, client.Credentials, settings, false );
        }

        /// <summary>
        /// Set queue analytics settings
        /// </summary>
        /// <param name="client"></param>
        /// <param name="baseUri"></param>
        /// <param name="settings"></param>
        public static void SetServiceSettings(this CloudQueueClient client, Uri baseUri, AnalyticsSettings settings)
        {
            SetSettings(baseUri, client.Credentials, settings, false );
        }

        /// <summary>
        /// Set blob analytics settings
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        public static void SetServiceSettings(this CloudTableClient client, AnalyticsSettings settings)
        {
            SetSettings(client.BaseUri, client.Credentials, settings, true );
        }

        /// <summary>
        /// Set analytics settings
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="credentials"></param>
        /// <param name="settings"></param>
        /// <param name="useSharedKeyLite"></param>
        public static void SetSettings(Uri baseUri, StorageCredentials credentials, AnalyticsSettings settings, bool useSharedKeyLite)
        {
            UriBuilder builder = new UriBuilder(baseUri);
            builder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "comp=properties&restype=service&timeout={0}", 
                DefaultTimeout.TotalSeconds);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri);
            request.Headers.Add(VersionHeaderName, Sep2009Version);
            request.Method = "PUT";

            StorageCredentialsAccountAndKey accountAndKey = credentials as StorageCredentialsAccountAndKey;
            using (MemoryStream buffer = new MemoryStream())
            {
                XmlTextWriter writer = new XmlTextWriter(buffer, Encoding.UTF8);
                SettingsSerializerHelper.SerializeAnalyticsSettings(writer, settings);
                writer.Flush();
                buffer.Seek(0, SeekOrigin.Begin);
                request.ContentLength = buffer.Length;

                if (useSharedKeyLite)
                {
                    credentials.SignRequestLite(request);
                }
                else
                {
                    credentials.SignRequest(request);
                }

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer.GetBuffer(), 0, (int)buffer.Length);
                }

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine("Response Request Id = {0} Status={1}", response.Headers[RequestIdHeaderName], response.StatusCode);
                        if (HttpStatusCode.Accepted != response.StatusCode)
                        {
                            throw new Exception("Request failed with incorrect response status.");
                        }
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine(
                        "Response Request Id={0} Status={1}",
                        e.Response != null ? e.Response.Headers[RequestIdHeaderName] : "Response is null",
                        e.Status);
                    throw;
                }

            }
        }

        /// <summary>
        /// Get blob analytics settings
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static AnalyticsSettings GetServiceSettings(this CloudBlobClient client)
        {
            return GetSettings(client.BaseUri, client.Credentials, false );
        }

        /// <summary>
        /// Get queue analytics settings
        /// </summary>
        /// <param name="client"></param>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        public static AnalyticsSettings GetServiceSettings(this CloudQueueClient client, Uri baseUri)
        {
            return GetSettings(baseUri, client.Credentials, false );
        }

        /// <summary>
        /// Get table analytics settings
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static AnalyticsSettings GetServiceSettings(this CloudTableClient client)
        {
            return GetSettings(client.BaseUri, client.Credentials, true );
        }

        /// <summary>
        /// Get analytics settings
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="credentials"></param>
        /// <param name="useSharedKeyLite"></param>
        /// <returns></returns>
        public static AnalyticsSettings GetSettings(Uri baseUri, StorageCredentials credentials, bool useSharedKeyLite)
        {
            UriBuilder builder = new UriBuilder(baseUri);
            builder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "comp=properties&restype=service&timeout={0}",
                DefaultTimeout.TotalSeconds);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(builder.Uri);
            request.Headers.Add(VersionHeaderName, Sep2009Version);
            request.Method = "GET";

            StorageCredentialsAccountAndKey accountAndKey = credentials as StorageCredentialsAccountAndKey;

            if (useSharedKeyLite)
            {
                credentials.SignRequestLite(request);
            }
            else
            {
                credentials.SignRequest(request);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Console.WriteLine("Response Request Id={0} Status={1}", response.Headers[RequestIdHeaderName], response.StatusCode);

                    if (HttpStatusCode.OK != response.StatusCode)
                    {
                        throw new Exception("expected HttpStatusCode.OK");
                    }

                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            string responseString = streamReader.ReadToEnd();
                            Console.WriteLine(responseString);

                            XmlReader reader = XmlReader.Create(new MemoryStream(ASCIIEncoding.UTF8.GetBytes(responseString)));
                            return SettingsSerializerHelper.DeserializeAnalyticsSettings(reader);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(
                    "Response Request Id={0} Status={1}",
                    e.Response != null ? e.Response.Headers[RequestIdHeaderName] : "Response is null",
                    e.Status);
                throw;
            }
        }
        #endregion
    }
*/
}