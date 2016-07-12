using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
//using Telerik.Web.UI.Widgets;
//using ICSharpCode.SharpZipLib.Zip ;
//using MediaInfoLib;
using System.Net;

namespace NetroMedia.SharedFunctions
{
    public class DiskFunctions
    {

        #region constants

        const int oneMB = 1; // will store bytes instead of MBs   //1024 * 1024; // ( 1024 bytes -1k- * 1024 ) = 1 MB


        #endregion

        //EventLog Logger;

        public DiskFunctions()
        {
        //    Logger = new EventLog("Application");
            //Logger.Source = "Application";
        }


        

        public string executeApplication(string workingDirectory, string appName, string parameters) { 
            string returnMsg;

            try
            {

                Process proc = new Process();
                ProcessStartInfo info = new ProcessStartInfo(appName, parameters);
                info.WorkingDirectory = workingDirectory;
                info.UseShellExecute = false;
                //info.RedirectStandardOutput = True
                //info.RedirectStandardError = True
                proc.StartInfo = info;

                if (proc.Start())
                {

                    proc.WaitForExit();
                    returnMsg = "OK";

                }
                else
                {
                    returnMsg = "Cant Start the process";
                }
            }
            catch (Exception ex) {

                returnMsg = "Error: " + ex.Message;

            }

            return returnMsg;

        }


        public double GetSizeOfDirectory(string rootDirectory)
        {
            double DirectorySize = 0;

            try
            {

                DirectoryInfo currentDir = new DirectoryInfo(rootDirectory);

                // process the directory files
                foreach (FileInfo file in currentDir.GetFiles("*.*"))
                {
                    DirectorySize += (file.Length / oneMB);
                }

                // process the subdirectories
                foreach (DirectoryInfo subDir in currentDir.GetDirectories())
                {
                    // recursive call
                    DirectorySize += GetSizeOfDirectory(subDir.FullName);
                }

                return DirectorySize;
            }
            catch
            {
                return -1;
            }


        }

        public void Unzip( string file, string outputFolder ){
            /*
            ZipInputStream s = new ZipInputStream(File.OpenRead(file));
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = outputFolder;
                string fileName = Path.GetFileName(theEntry.Name);
                // create directory 
                if (directoryName != "")
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (fileName != String.Empty)
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace("\\ ", "\\");
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                        FileStream streamWriter = File.Create(fullPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();

            */
        }

            public void SaveFile( string fileName, byte[] content) {

                try
                {

                    BinaryWriter bw = new BinaryWriter(
                    new FileStream(fileName, FileMode.Create));

                    bw.Write(content);
                    bw.Flush();
                    bw.Close();

                }
                catch (Exception )
                { 
                
                }

            }

            public void SaveFileChunk(string fileName, byte[] content, long offset)
            {

                try
                {
                    if (offset == 0)	// new file, create an empty file
                        File.Create( fileName ).Close();

                    // open a file stream and write the buffer.  Don't open with FileMode.Append because the transfer may wish to start a different point
                    using (FileStream fs = new FileStream( fileName , FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.Write( content , 0, content.Length);
                    }

                }
                catch (Exception )
                {

                }

            }

            public void CreateDirectory(string rootDirectory)
            {
                CreateDirectory_(rootDirectory);
            }
            public string  CreateDirectory_(string rootDirectory) {
                
                try
                {
                    DirectoryInfo dirToCreate;

                    char[] sep = { '\\' };

                    string[] TreeD = rootDirectory.Split(sep);

                    string curDir = TreeD[0];

                    for (int i = 1; i < TreeD.Length; i++)
                    {

                        curDir = curDir + @"\" + TreeD[i];

                        

                        dirToCreate = new DirectoryInfo(curDir);

                        if (!dirToCreate.Exists)
                        {
                            dirToCreate.Create();
                        }


                    }

                    

                    return "OK"  ;
                }
                catch (Exception ex) {
                    return ex.Message  ;
                }


        }


            public void Move(string FileDirectory, string newname)
            {
                // try to drop as a file
                try
                {
                    if (!MoveFile(FileDirectory, newname ))
                        MoveDirectory(FileDirectory, newname);
                }
                catch { }
            }

            public bool MoveFile(string file, string newname)
            {
                FileInfo file_ = new FileInfo(file);

                if (file_.Exists)
                {
                    file_.MoveTo( newname );
                    return true;
                }
                else
                {
                    return false;
                }

            }



        public byte[] GetFile(string name)
        {
            byte[] content;
            try
            {
                using (FileStream inputStream = File.OpenRead(name))
                {
                    long size = inputStream.Length;
                    content = new byte[size];
                    inputStream.Read(content, 0, Convert.ToInt32(size));
                }
                return content; 
            }
            catch (Exception)
            { 
                return null;
            }
        }


            public void MoveDirectory(string rootDirectory, string newname)
            {
                DirectoryInfo dirToCreate = new DirectoryInfo(rootDirectory);

                if (dirToCreate.Exists)
                {
                    dirToCreate.MoveTo( newname );  // recursive
                }

            }




            public void Remove(string FileDirectory)
            {
                // try to drop as a file
                try
                {
                    if (!RemoveFile(FileDirectory))
                        RemoveDirectory(FileDirectory);
                }
                catch { }
            }

            public bool  RemoveFile(string file)
            {
                FileInfo  file_ = new FileInfo( file );

                if (file_.Exists)
                {
                    file_.Delete();
                    return true;
                }
                else
                {
                    return false;
                }

            }


            public bool DirectoryIsEmpty(string rootDirectory)
            {
                DirectoryInfo dirToEvaluate = new DirectoryInfo(rootDirectory);
                bool isEmpty = true;

                if (dirToEvaluate.GetDirectories().Length > 0)
                {
                    isEmpty = false;
                }

                if (dirToEvaluate.GetFiles().Length > 0)
                {
                    isEmpty = false;
                }

                return isEmpty;
            }

        public void RemoveDirectory(string rootDirectory)
        {
            DirectoryInfo dirToCreate = new DirectoryInfo(rootDirectory);

            if (dirToCreate.Exists)
            {
                dirToCreate.Delete(true);  // recursive
            }

        }

        public string getTree_ListDirectory( string rootDirectory)
        {

            string retDir = "";


            retDir = "<?xml version='1.0'?>\n" +
                            "<DirectoryContents>\n";
            

            try
            {
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Starting Directory List -> [" + rootDirectory + "]");
                //Logger.WriteEntry("Starting Directory List -> [" + rootDirectory  + "]", EventLogEntryType.Information );

                //string retDir = @"\;0" + "|";

                retDir += fillTreeNode("", rootDirectory, true);


            }
            catch (Exception ex)
            {

                //Logger.WriteEntry("Error in List Directory  -> [" + ex.Message + "]", EventLogEntryType.Error);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Error in List Directory  -> [" + ex.Message + "]");

                retDir += errMsg();

                //return "-1";
                //<?xml version='1.0'?>\n" + "<Error msg='" + ex.Message + "' />";
            }

            retDir += "</DirectoryContents>\n";

            //Logger.WriteEntry("Finished the scan -> [" + retDir + "]", EventLogEntryType.Information);
            Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Finished the scan -> [" + retDir + "]");

            return retDir;


        }

        private string  fillTreeNode(string currentRoot, string currentPath, bool firstLevel)
        {
            string retDir = "";
            

            try
            {


                //Logger.WriteEntry("Starting Directory List -> [" + currentPath  + "]", EventLogEntryType.Information);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Starting Directory List -> [" + currentPath + "]");

                DirectoryInfo curDir = new DirectoryInfo(currentPath);

                foreach (FileInfo Files in curDir.GetFiles())
                {
                    
                    retDir += "<File Name='" + currentRoot + (  firstLevel ? "" : @"/" ) + Files.Name.ToString() + "' Size='" + shortenSize(Files.Length.ToString()) + 
                        "' Attribute='File' DE='" + Files.CreationTimeUtc.ToString() + "' " + 
                        " DM='" + Files.LastAccessTimeUtc.ToString() + "' " +
                        " Date_Modified ='" + Files.LastAccessTimeUtc.ToString() + "' "
                        +  " />"; 
                }


                foreach (DirectoryInfo childDir in curDir.GetDirectories())
                {

                    retDir += "<File Name='" + currentRoot + (firstLevel ? "" : @"/") + childDir.Name.ToString() + @"/' Size='' Attribute='Dir' " + 
                         "DE='" +  childDir.CreationTimeUtc.ToString() + "' " + 
                        " DM='" +  childDir.LastWriteTimeUtc.ToString() + "'"  +
                        " Date_Modified ='" + childDir.LastWriteTimeUtc.ToString() + "'" 
                        + " />";

                    retDir += fillTreeNode(currentRoot + (firstLevel ? "" : @"/") + childDir.Name, currentPath + @"/" + childDir.Name, false);                    

                }

            }
            catch (Exception ex) {
                //Logger.WriteEntry("Error in List Directory  -> [" + ex.Message + "]", EventLogEntryType.Error);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Error in List Directory  -> [" + ex.Message + "]");
                 
                retDir = errMsg();
                
            }

            return retDir;

        }



        public string getDirectoryFiles(string rootDirectory)
        {
            string retDir = "";
            int count = 0;


            try
            {

                retDir = "<?xml version='1.0'?>\n" +
                          "<DirectoryContents>\n";

                DirectoryInfo curDir = new DirectoryInfo(rootDirectory);

                
                foreach (FileInfo Files in curDir.GetFiles())
                {

                    retDir += "<File Name='" + Files.Name.ToString() + "' Size='" + shortenSize(Files.Length.ToString()) +
                        "' DE='" + Files.CreationTimeUtc.ToString() + "' DU = '" + 
                        //MediaInfoLib.MediaInfoLib.getDuration_HMS( Files.FullName ) +
                        "'"
                        + " />";
                    

                    count++;

                }

                if (count == 0) {
                    retDir += "<File Name='' Size='0' DE='' DU= '0' " + " />";
                          
                }

                retDir += "</DirectoryContents>\n";
           

            }
            catch (Exception )
            {


                retDir = "<?xml version='1.0'?>\n" +
                          "<DirectoryContents>\n" +
                          "<File Name='' Size='0' DE='' DU= '0' " + " />" +
                          "</DirectoryContents>\n";

            }

            return retDir;

        }



        public string getDirectory_Children(string rootDirectory)
        {

            string retDir = "";


            try
            {


                retDir += getChildren( rootDirectory);


            }
            catch (Exception )
            {
            }            

            return retDir;


        }

        private string getChildren( string currentPath)
        {
            string retDir = "";


            try
            {


                DirectoryInfo curDir = new DirectoryInfo(currentPath);

                foreach (DirectoryInfo childDir in curDir.GetDirectories())
                {

                    retDir +=  childDir.Name.ToString() + "|" ;                    

                }

            }
            catch (Exception )
            {
        

            }

            return retDir;

        }



        private string shortenSize(string byteSize)
        {
            long tmpSize = Convert.ToInt64(byteSize);
            int i = 0;
            long size = 0;
            long div = 1;
            string[] Sizes = { "N", "b", "Kb", "Mb", "Gb", "Tb", "*" };

            do
            {

                i += 1;

                size = tmpSize / div;

                if (size < 999)
                {
                    return size.ToString() + ' ' + Sizes[i];
                }

                div *= 1024;


            } while (size > 999);

            return size.ToString() + ' ' + Sizes[i];



        }

        private string errMsg()
        {
            return "<File Name='Can not process the request' Size='' Attribute='' DE='' DM='' Date_Modified='1/1/1980' />";
        }

        int rowID;

        private int addRow(DataTable dt, string name, int parent, string mime, bool dir, int size)
        {
            DataRow dr;

            rowID++;

            dr = dt.NewRow();
            dr["ItemID"] = rowID;
            dr["Name"] = name;
            dr["ParentID"] = parent;
            dr["MimeType"] = mime;
            dr["IsDirectory"] = dir;
            dr["Size"] = size;
            dt.Rows.Add(dr);

            return rowID;
        }

        public string getTree_ListDirectoryV2(string rootDirectory)
        {
            string retDir = "";

            DataTable directoryDetail = new DataTable(  );
            directoryDetail.TableName = "DirectoryView";

            directoryDetail.Columns.Add(new DataColumn("ItemID", typeof(int)));
            directoryDetail.Columns.Add(new DataColumn("Name", typeof(string)));
            directoryDetail.Columns.Add(new DataColumn("ParentID", typeof(int)));
            directoryDetail.Columns.Add(new DataColumn("MimeType", typeof(string)));
            directoryDetail.Columns.Add(new DataColumn("IsDirectory", typeof(bool)));
            directoryDetail.Columns.Add(new DataColumn("Size", typeof(int)));


            rowID = 0;
               
            


            try
            {

                //Logger.WriteEntry("Starting Directory List -> [" + rootDirectory + "]", EventLogEntryType.Information);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Starting Directory List -> [" + rootDirectory + "]");

            


            int node1 = addRow(directoryDetail, "Root", 0, "", true, 0);

            

            int t = fillTreeNodeV2(directoryDetail, node1, rootDirectory, true);


            }
            catch (Exception ex)
            {

                //Logger.WriteEntry("Error in List Directory  -> [" + ex.Message + "]", EventLogEntryType.Error);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Error in List Directory  -> [" + ex.Message + "]");
                       
            }

            StringWriter sw = new StringWriter();
            directoryDetail.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            retDir  = sw.ToString();

            //Logger.WriteEntry("Finished the scan -> [" + retDir.Substring(0,250) + "]", EventLogEntryType.Information);
            Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Finished the scan -> [" + retDir.Substring(0, 250) + "]");

            return retDir ;


        }

        private string getMimeType(string extension)
        {

            extension = extension.ToUpper();

            switch (extension)
            {

                case ".JS": return "application/javascript";
                case ".PDF": return "application/pdf";
                case ".ZIP": return "application/zip";
                case ".MPEG": return "video/mpeg";
                case ".WMA":
                case ".WME": return "audio/x-ms-wma";
                case ".WMV": return "video/x-ms-wmv";
                case ".WAV": return "audio/x-wav";
                case ".GIF": return "image/gif";
                case ".MP4": return "video/mp4";
            }

            return "";

        }


       

        private int fillTreeNodeV2(DataTable list, int parentNodeID, string currentPath, bool firstLevel)
        {
            //string retDir = "";


            try
            {

                
            //Logger.WriteEntry("Starting Directory List -> [" + currentPath + "]", EventLogEntryType.Information);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Starting Directory List -> [" + currentPath + "]");

            DirectoryInfo curDir = new DirectoryInfo(currentPath);

            foreach (FileInfo Files in curDir.GetFiles())
            {
                int node1 = addRow(list, Files.Name.ToString(), parentNodeID, getMimeType(Files.Extension.ToString()), false, int.Parse(Files.Length.ToString()));

                

            }


            foreach (DirectoryInfo childDir in curDir.GetDirectories())
            {
                int node2 = addRow(list, childDir.Name.ToString(), parentNodeID, "", true, 0);

                int t = fillTreeNodeV2(list, node2, currentPath + @"/" + childDir.Name, false);
                
            }

            }
            catch (Exception ex)
            {
                //Logger.WriteEntry("Error in List Directory  -> [" + ex.Message + "]", EventLogEntryType.Error);
                Netro_Log4.writeLocalServiceLog("FTP ", Environment.MachineName, 0, "Error in List Directory  -> [" + ex.Message + "]");

            //    retDir = errMsg();

            }

            


            return 1;

        }






    }
       
    public class CryptoFunctions {

        string SaltCrypt = "N3tr0C4p6t";
        
    public string getMD5( string input ) 
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash  = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        
        for( int i = 0; i < hash.Length ; i++ ){

            sb.Append(hash[i].ToString("X2"));

        }

        return sb.ToString().ToLower();

        }




        public string ToBase64(byte[] data)
        {

            return Convert.ToBase64String(data);

        }

        public byte[] FromBase64(string base64)
        {

            return Convert.FromBase64String(base64);

        }

         public string crypt(string txt)
        {
            UnicodeEncoding enc = new UnicodeEncoding();
            return ToBase64(enc.GetBytes(SaltCrypt + txt));
        }
        public string decrypt(string txt)
        {

            UnicodeEncoding enc = new UnicodeEncoding();
            return enc.GetString(FromBase64(txt)).Replace(SaltCrypt, "");
        }

        


    }

   
}
