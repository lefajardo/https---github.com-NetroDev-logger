/*using System;
using System.Collections.Generic;
using System.Text;
using NetroMedia.SQL;
using NetroMedia.DataAdapter;
using NetroMedia.DataModule.Service;
using NetroMedia.wowcounters;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using NetroMedia.DBlog;

namespace NetroRTV
{
    public class WowzaRTV
    {
        SqlManager sqlmgr;
        AppConfig config;
        EventLog evtLog = new EventLog();

        public void processWowzaCounters()
        {


            evtLog.Source = "Application";
            evtLog.Log = "Application";

            wowcounters counters;

            //evtLog.WriteEntry("Starting the rtv check");
            

            try
            {
                
                config = ConfigManager.LoadFromFile();
                
                sqlmgr = new SqlManager(getConnectionString());
                
                System.Net.HttpWebResponse response;
                
                
                
                counters = new wowcounters();
                
                response = counters.ppCounters("");
                
                if (response != null)
                {
                    savePubPointCounters(response, config, config.public_ip_this_server);
                    
                }
                else
                {
                    evtLog.WriteEntry("response is null");
                    Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WOWCounters , 0, "*Error NetroRTV respose is null " );
                }
            }
            catch (Exception ex)
            {
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WOWCounters , 0, "*Error NetroRTV " + ex.Message);
                
            }
            sqlmgr = null;
            counters = null;
        }
        private string getConnectionString()
        {
            //evtLog.WriteEntry("conn string");

            return "data source=" + config.RTV_Server // .CENTRAL_DB_IP
                        + ";initial catalog=" + config.RTV_Database // .CENTRAL_DB_Database
                        + ";password=" + config.RTV_Password // .CENTRAL_DB_Pwd
                        + ";user id=" + config.RTV_User // .CENTRAL_DB_User 
                        + (config.RTV_Encrypt == "YES" ? ";Encrypt=yes" : "") +";";
                //    + ";persist security info=True;user id=" + config.RTV_User // .CENTRAL_DB_User 
                        
        }

        void savePubPointCounters(System.Net.HttpWebResponse xmlServerInfo, AppConfig Config_, string server)
        {
            AppConfig _appConfig = Config_;
        
            int ttl = 0;
            string xmlPacket;

            DataSet dsCounters = new DataSet();
            DataTable Applications = null;

            int value_ = 0;
            int value2_ = 0;


            try
            {
                dsCounters.ReadXml(xmlServerInfo.GetResponseStream());
                Applications = dsCounters.Tables["Application"];
            }
            catch (Exception ex)
            {
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WOWCounters, 0, "*Error Importing wowza XML " + ex.Message);
                //evtLog.WriteEntry(" error in import xml" + ex.Message);
                return;
            }
            

            xmlPacket = "<items>";


            ttl = 0;

            foreach (DataRow dr in Applications.Rows)
            {
                if (dr["Status"].ToString() == "loaded")
                {

                     value_ = 0;
                        value2_ = 0;

                        int.TryParse(dr["ConnectionsCurrent"].ToString(), out value_);
                        value2_ = (int)Math.Truncate(float.Parse(dr["MessagesOutBytesRate"].ToString()) / 1000);
                        

                        if (value_ > 0 || value2_ > 0)
                        {
                            xmlPacket += "<item ";
                            xmlPacket += " ID = '" + Guid.NewGuid().ToString() + "' ";
                            xmlPacket += " DATE_MODIFIED = '" + DateTime.Now.ToLongTimeString() + "' ";
                            xmlPacket += " SERVER_IP = '" + _appConfig.public_ip_this_server + "' ";
                            xmlPacket += "PUB_POINT='" + dr["Name"].ToString() + "' ";
                            xmlPacket += "CURRENT_CONNECTIONS='" + dr["ConnectionsCurrent"].ToString() + "' ";
                            xmlPacket += "PEAK_CONNECTIONS='0' ";
                            xmlPacket += "CURRENT_THROUGHPUT='" + Math.Truncate(float.Parse(dr["MessagesOutBytesRate"].ToString()) * 8 / 1000 ).ToString() + "' ";
                            xmlPacket += "PEAK_THROUGHPUT='0' ";
                            xmlPacket += "PUB_POINT_TYPE = '' ";
                            xmlPacket += "SERVICE_TYPE = 'WMZ' ";
                            xmlPacket += " />";
                            ttl++;
                        }
                    

                }
            }

            xmlPacket += "</items>";

            if (_appConfig.send_debug_emails)
            {
                evtLog.WriteEntry(" sending " + ttl.ToString() + " to " + config.public_ip_this_server);
                evtLog.WriteEntry(" sending " + xmlPacket);
            }

            List<SqlParameter> prms = new List<SqlParameter>();

            prms.Add(new SqlParameter("@SERVER_IP", _appConfig.public_ip_this_server));
            prms.Add(new SqlParameter("@COUNTER_XML", xmlPacket));
            sqlmgr.ExecuteStoredProcedure("spCOUNTERS_UpdateXml", prms);
            sqlmgr = null;
            prms = null;


            Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WOWCounters, 0, "*NetroRTV succeed saving wowza rtv info");

        }



    }
}
*/