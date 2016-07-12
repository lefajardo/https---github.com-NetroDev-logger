using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Net;
//using NetroMedia.DataModule.Service;

namespace NetroMedia.SharedFunctions
{
    public class wowcounters
    {
        //EventLog Logger = new EventLog();

        AppConfig5_Azure _config;

        public wowcounters(AppConfig5_Azure config_)
        { 
            _config = config_;
        }


        //string server_address = "http://localhost:8086/connectioncounts";

        public HttpWebResponse ppCounters(string ppName)
        {

            HttpWebResponse strResponse = null;
            
            HttpWebRequest request;
            //HttpWebResponse response;

           // Logger.Source = "Application";
           // Logger.Log = "Application";

            
            

            try
            {

                //request  = WebRequest.Create(server_address) as HttpWebRequest;
                ////  = WebRequest.Create("http://148.obj.netromedia.net:8086/connectioncounts") as HttpWebRequest;                
                //request.Credentials = new NetworkCredential("netroadmin", "netroadmin");


                request = WebRequest.Create( _config.Wowzacounters_Address ) as HttpWebRequest;
                request.Credentials = new NetworkCredential( _config.Wowzacounters_User , _config.Wowzacounters_Pwd );

                // Get response  
                strResponse = request.GetResponse() as HttpWebResponse;

                
                

            }
            catch (Exception ex ) {
                // empty xml     
                Netro_Log4.writeLocalServiceLog("WOW Counters", Environment.MachineName , 0, "error pulling info from wowcounters " + ex.Message );                
                //Logger.WriteEntry(" error pulling info from wowcounters " + ex.Message );
            }

            //response = null;
            request = null;



            return strResponse;



        }



    }
}
