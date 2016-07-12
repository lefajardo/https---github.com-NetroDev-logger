using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;


namespace NetroMedia.SharedFunctions
{
    public class WowzaRTV
    {
        SqlManager sqlmgr;
        AppConfig5_Azure config;

        public WowzaRTV(AppConfig5_Azure config_) { 
            config = config_;
        }

        public void processWowzaCounters()
        {
        
            wowcounters counters;

            try
            {

                sqlmgr = new SqlManager(getConnectionStringRTV());
                
                System.Net.HttpWebResponse response;
                               
                
                counters = new wowcounters(config);
                
                response = counters.ppCounters("");
                
                if (response != null)
                {
                    savePubPointCounters(response, config.ServerPublicIP);
                    
                }
                else
                {
                    Netro_Log4.writeLocalServiceLog("WOW RTV", Environment.MachineName, 0, "response is null " );                
              
                }
            }
            catch (Exception ex)
            {
                Netro_Log4.writeLocalServiceLog("WOW Counters", Environment.MachineName, 0, "error wowza RTV " + ex.Message);                
                
            }
            sqlmgr = null;
            counters = null;
        }
        private string getConnectionStringRTV()
        {
            

            return "data source=" + config.RTV_Server 
                        + ";initial catalog=" + config.RTV_Database 
                        + ";password=" + config.RTV_Password 
                        + ";user id=" + config.RTV_User 
                        + (config.RTV_Encrypt == "YES" ? ";Encrypt=yes" : "") +";";
                
                        
        }

        void savePubPointCounters(System.Net.HttpWebResponse xmlServerInfo, string server)
        {
            
        
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
                Netro_Log4.writeLocalServiceLog("WOW Counters", Environment.MachineName, 0, "error importing wowza RTV XML " + ex.Message);                
                
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
                            xmlPacket += " SERVER_IP = '" + server + "' ";
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

            List<SqlParameter> prms = new List<SqlParameter>();

            prms.Add(new SqlParameter("@SERVER_IP", server));
            prms.Add(new SqlParameter("@COUNTER_XML", xmlPacket));
            sqlmgr.ExecuteStoredProcedure("spCOUNTERS_UpdateXml", prms);
            sqlmgr = null;
            prms = null;
            
        }



    }
}
