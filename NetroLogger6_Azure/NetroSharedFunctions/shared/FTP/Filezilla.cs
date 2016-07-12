using System;
using System.Collections.Generic;
using System.Text;

using NetroMedia.SharedFunctions;
using System.IO;
using System.Xml;

namespace NetroMedia.NetroFileZillaInterface
{
    public class Filezilla
    {
        private string WebPath = "";

        public Filezilla() {
            WebPath = @"C:\Program Files\FileZilla Server";
        }

        public Filezilla( string PathWeb)
        {
            WebPath = PathWeb;
        }

        #region Fields
        private AppConfig _appConfig;
        #endregion

        public void createUser(string userName, string password, string homeDir, string DOrestartServer)
        {

            _appConfig = ConfigManager.LoadFromFileWeb( WebPath  );

            DiskFunctions Directory = new DiskFunctions();
            CryptoFunctions MD5 = new CryptoFunctions();

            bool alreadyExist = false;

            // create user directory
            Directory.CreateDirectory(homeDir);

            //insert the user into the Filezilla Server' XML file
            XmlDocument FileZillaConfiguration = new XmlDocument();
            FileZillaConfiguration.Load( _appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            XmlNodeList  FileZillaUsers = FileZillaConfiguration.SelectNodes("//User");

            // search for the user to see if its already created in this server
            foreach( XmlNode FZUser in FileZillaUsers ){

                if (FZUser.Attributes[0].Value  == userName) {
                    alreadyExist = true;
                }

            }

            if (!alreadyExist){

                // add a new user in the filezilla server xml config file

                // root element for <Users>
                XmlElement root = FileZillaConfiguration.DocumentElement;
                XmlNode Users = root.SelectSingleNode("Users");


                    // New User root
                    XmlNode newUser = newNode(FileZillaConfiguration, "User", "Name", userName, "");

                        // Add the child nodes, password, etc                
                        newUser.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "Pass", MD5.getMD5(password)));
                        newUser.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "Enabled", "1"));

                        // Permissions
                        XmlNode newUserPermisions = FileZillaConfiguration.CreateNode(XmlNodeType.Element, "Permissions", "");

                            // user directory
                            XmlNode newUserDirectory = newNode(FileZillaConfiguration, "Permission", "Dir", homeDir, "");

                                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "FileRead", "1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","FileWrite","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","FileDelete","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","FileAppend","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","DirCreate","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","DirDelete","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","DirList","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","DirSubdirs","1"));
                                newUserDirectory.AppendChild(newNode( FileZillaConfiguration,"Option", "Name","IsHome","1"));

                        // add the home dir to the permissions
                        newUserPermisions.AppendChild(newUserDirectory);

                    // add the enabled option to the user node
                    newUser.AppendChild(newUserPermisions);
                
                // Add the new User Element
                Users.AppendChild(newUser);
                FileZillaConfiguration.Save(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");
                

            }

            Directory = null;
            FileZillaConfiguration = null;
            FileZillaUsers = null;
            MD5 = null;

            if (DOrestartServer == "1")
            {
                restartServer();
            }
        }

        public void createUserLocal(string userName, string password, string homeDir, string DOrestartServer, AppConfig _appConfig)
        {

        //    _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            DiskFunctions Directory = new DiskFunctions();
            CryptoFunctions MD5 = new CryptoFunctions();

            bool alreadyExist = false;

            // create user directory
            Directory.CreateDirectory(homeDir);

            //insert the user into the Filezilla Server' XML file
            XmlDocument FileZillaConfiguration = new XmlDocument();
            FileZillaConfiguration.Load(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            XmlNodeList FileZillaUsers = FileZillaConfiguration.SelectNodes("//User");

            // search for the user to see if its already created in this server
            foreach (XmlNode FZUser in FileZillaUsers)
            {

                if (FZUser.Attributes[0].Value == userName)
                {
                    alreadyExist = true;
                }

            }

            if (!alreadyExist)
            {

                // add a new user in the filezilla server xml config file

                // root element for <Users>
                XmlElement root = FileZillaConfiguration.DocumentElement;
                XmlNode Users = root.SelectSingleNode("Users");


                // New User root
                XmlNode newUser = newNode(FileZillaConfiguration, "User", "Name", userName, "");

                // Add the child nodes, password, etc                
                newUser.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "Pass", MD5.getMD5(password)));
                newUser.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "Enabled", "1"));

                // Permissions
                XmlNode newUserPermisions = FileZillaConfiguration.CreateNode(XmlNodeType.Element, "Permissions", "");

                // user directory
                XmlNode newUserDirectory = newNode(FileZillaConfiguration, "Permission", "Dir", homeDir, "");

                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "FileRead", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "FileWrite", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "FileDelete", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "FileAppend", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "DirCreate", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "DirDelete", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "DirList", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "DirSubdirs", "1"));
                newUserDirectory.AppendChild(newNode(FileZillaConfiguration, "Option", "Name", "IsHome", "1"));

                // add the home dir to the permissions
                newUserPermisions.AppendChild(newUserDirectory);

                // add the enabled option to the user node
                newUser.AppendChild(newUserPermisions);

                // Add the new User Element
                Users.AppendChild(newUser);
                FileZillaConfiguration.Save(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");


            }

            Directory = null;
            FileZillaConfiguration = null;
            FileZillaUsers = null;
            MD5 = null;

            if (DOrestartServer == "1")
            {
                restartServerLocal(_appConfig );
            }
        }

        private XmlNode newNode(XmlDocument doc, string title, string attribName, string attribValue, string nodeText) {

            XmlNode Option = doc.CreateNode(XmlNodeType.Element, title, "");
            XmlAttribute OptOption = doc.CreateAttribute(attribName );
            OptOption.Value = attribValue ;
            Option.Attributes.Append(OptOption);
            if (nodeText.Length > 0)
            {
                Option.InnerText = nodeText ;
            }

            return Option;

        }


        public void updatePwd(string userName, string Password)
        {


            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            //insert the user into the Filezilla Server' XML file
            XmlDocument FileZillaConfiguration = new XmlDocument();
            FileZillaConfiguration.Load(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            CryptoFunctions MD5 = new CryptoFunctions();

            XmlNodeList FileZillaUsers = FileZillaConfiguration.SelectNodes("//User");

            // search for the user to see if its already created in this server
            foreach (XmlNode FZUser in FileZillaUsers)
            {

                if (FZUser.Attributes[0].Value == userName)
                {
                    
                    foreach (XmlNode Opts in FZUser.ChildNodes)
                    {
                        if (Opts.Name == "Option")
                        {
                            if (Opts.Attributes[0].Value == "Pass" )
                            {

                                
                                    Opts.InnerText = MD5.getMD5(Password);
                                
                             
                            }
                        }
                    }


                }

            }

            FileZillaConfiguration.Save(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");


            FileZillaConfiguration = null;
            FileZillaUsers = null;
            MD5 = null;
            restartServer();


        }


        public void UploadRights(string userName, bool assignRights, AppConfig _appConfig , string homedir, string pwd) {

            bool updated = false;
            /*
            if (local)
            {
                _appConfig = ConfigManager.LoadFromFile();
            }
            else
            {
                _appConfig = ConfigManager.LoadFromFileWeb(WebPath);
            }
            */
            //insert the user into the Filezilla Server' XML file
            XmlDocument FileZillaConfiguration = new XmlDocument();
            FileZillaConfiguration.Load(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            XmlNodeList FileZillaUsers = FileZillaConfiguration.SelectNodes("//User");

            // search for the user to see if its already created in this server
            foreach (XmlNode FZUser in FileZillaUsers)
            {

                if (FZUser.Attributes[0].Value == userName)
                {


                        //XmlNodeList FileOptions = FZUser.SelectNodes("//Permission");
                        foreach (XmlNode FZOption in FZUser.ChildNodes )
                        {
                            if (FZOption.Name == "Permissions")
                            {
                                if (FZOption.ChildNodes.Count > 0) {

                                    if (FZOption.FirstChild.ChildNodes.Count > 0)
                                    {

                                        
                                                
                                                foreach (XmlNode Opts in FZOption.FirstChild.ChildNodes)
                                                {
                                                    if (Opts.Name == "Option")
                                                    {
                                                        if ( Opts.Attributes[0].Value == "FileWrite" || 
                                                            Opts.Attributes[0].Value == "FileAppend" || 
                                                            Opts.Attributes[0].Value == "DirCreate" || 
                                                            Opts.Attributes[0].Value == "DirDelete"){

                                                            if (assignRights)
                                                            {      // Assign rights     
                                                                Opts.InnerText = "1";
                                                            }
                                                            else
                                                            { // remove rights
                                                                Opts.InnerText = "0";
                                                            }

                                                            updated = true;

                                                        }
                                                    }
                                                }
                                        
                                        
                                    }
                                    
                                }
                            }
                        }
                        

                    }

            }

            FileZillaConfiguration.Save(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            
            FileZillaConfiguration = null;
            FileZillaUsers = null;
            

            if (!updated) { 

                createUserLocal(userName, pwd, homedir, "0",_appConfig );

            }

          //  restartServer();


            

        }

        public void restartServer() {

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            DiskFunctions process = new DiskFunctions();

            string appToRun = _appConfig.FTP_FilezillaServerPath + @"\FileZilla server.exe";

            process.executeApplication(_appConfig.FTP_FilezillaServerPath, appToRun, "/reload-config");


            process = null;

            
        }

        public void restartServerLocal( AppConfig _appConfig )
        {

            //_appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            DiskFunctions process = new DiskFunctions();

            string appToRun = _appConfig.FTP_FilezillaServerPath + @"\FileZilla server.exe";

            process.executeApplication(_appConfig.FTP_FilezillaServerPath, appToRun, "/reload-config");


            process = null;


        }


        public void deleteUser(string userName, string homeDir)
        {

            _appConfig = ConfigManager.LoadFromFileWeb(WebPath);

            DiskFunctions Directory = new DiskFunctions();

            // delete user directory
            Directory.RemoveDirectory(homeDir);


            //insert the user into the Filezilla Server' XML file
            XmlDocument FileZillaConfiguration = new XmlDocument();
            FileZillaConfiguration.Load(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            XmlNodeList FileZillaUsers = FileZillaConfiguration.SelectNodes("//User");

            // search for the user to see if its already created in this server
            foreach (XmlNode FZUser in FileZillaUsers)
            {

                if (FZUser.Attributes[0].Value == userName)
                {

                    FZUser.ParentNode.RemoveChild(FZUser);
                    
                }

            }

            FileZillaConfiguration.Save(_appConfig.FTP_FilezillaServerPath + @"\FileZilla Server.xml");

            Directory = null;
            FileZillaConfiguration = null;
            FileZillaUsers = null;

            restartServer();

        }

    }
}
