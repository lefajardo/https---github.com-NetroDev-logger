using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

using System.IO;
using System.Threading;
using NetroMedia.SQL;
using NetroMedia.DataModule.Service;
using NetroMedia.wowcounters;
using NetroMedia.DBlog;
using NetroMedia.DataAdapter;
using System.Xml;
using System.Data.SqlClient ;

namespace NetroWOW_RT_Statistics
{
    public partial class NetroWOW_RT_Statistics : ServiceBase
    {
        public NetroWOW_RT_Statistics()
        {
            InitializeComponent();
        }


        #region components

        System.Threading.Timer oTimer;

        #endregion
        SqlManager sqlmgr;
        AppConfig config ;
        EventLog evtLog = new EventLog();
        string xmlPacket = "";

        protected override void OnStart(string[] args)
        {
            config = ConfigManager.LoadFromFile();
            
            evtLog.Source = "Application";
            evtLog.Log = "Application";
            SetTimer();
        }
        
        private void SetTimer()
        {

            TimerCallback oCallback = new TimerCallback(timer_Tick);
            oTimer = new Timer(oCallback, null, Convert.ToInt32(config.WMS_SecondsToWaitAfterACall) * 1000, Convert.ToInt32(config.WMS_SecondsToWaitAfterACall) * 1000);

        }


        private void timer_Tick(Object stateInfo)
        {

            queryWOW();
        }

        protected override void OnStop()
        {
            config = null;
            oTimer.Dispose();
        }

        //int totalFaults = 0;

        void queryWOW()
        {


            wowcounters counters;

            //evtLog.WriteEntry("Starting the rtv check");
            sqlmgr = new SqlManager(getConnectionString());

            try
            {

                System.Net.HttpWebResponse  response ;
                Guid Packet = Guid.Empty; ;
                config = ConfigManager.LoadFromFile();    


                //response = "";
                Packet = Guid.NewGuid();
                
                counters = new wowcounters();
                //sqlmgr = new sqlmana WmsCountersClient();

                //evtLog.WriteEntry("querying server " + config.public_ip_this_server);

                response = counters.ppCounters("");

            
                if (response != null)
                {
                    savePubPointCounters(response, config, config.public_ip_this_server, Packet);
            
                    //processPacket(Packet, config);
                }
                else {
                    evtLog.WriteEntry("response is null");
                }

                //Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WOWCounters , 0, "Success retrieving wms counters " + " " + config.public_ip_this_server);

                //totalFaults = 0;

            

            }
            catch (Exception ex)
            {


              //  totalFaults++;
               // if (totalFaults == 120)
               // {
                    //Netro_Log4.sendEmail(NetroMedia.ServiceToParse.WOWCounters, 0, "*Error retrieving wms counters " + " " + ex.Message);
               // }
                //Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WOWCounters, 0, "*Error retrieving wms counters " + " " + ex.Message);

                evtLog.WriteEntry("error " + ex.Message);
            }


            sqlmgr = null;
            counters = null;

        }
        public void processPacket(Guid packet, AppConfig Config_)
        {
            AppConfig  _appConfig = Config_;

            List<SqlParameter> prms = new List<SqlParameter>();

            prms.Add(new SqlParameter("@Packet", packet));
            sqlmgr.ExecuteStoredProcedure("WmsCounterPP_InsUpd", prms);
            sqlmgr = null;
            prms = null;
        }
        private string getConnectionString()
        {
            return "data source=" + config.RTV_Server // .CENTRAL_DB_IP
                        + ";initial catalog=" + config.RTV_Database // .CENTRAL_DB_Database
                        + ";password=" + config.RTV_Password // .CENTRAL_DB_Pwd
                        + ";persist security info=True;user id=" + config.RTV_User // .CENTRAL_DB_User 
                    //    + ";persist security info=True;user id=" + config.RTV_User // .CENTRAL_DB_User 

                        + ";";

        }

        public void savePubPointCounters(System.Net.HttpWebResponse  xmlServerInfo, AppConfig Config_, string server, Guid Packet)
        {
            AppConfig  _appConfig = Config_;
            //DataRow dt;
            int ttl = 0;

            DataSet dsCounters = new DataSet();
            try
            {
                dsCounters.ReadXml(xmlServerInfo.GetResponseStream());
            }
            catch ( Exception ex ) {
                evtLog.WriteEntry(" error in import xml" + ex.Message );
            }
            DataTable Applications = dsCounters.Tables["Application"];
            
            //DataTable dtServerCounters = sqlmgr.LoadTable( "Select * from WMSPubPointCountersStage where ServerIP = '-1' ");

            xmlPacket = "<items>";


            ttl = 0;

            foreach (DataRow dr in Applications.Rows)
            {
                if (dr["Status"].ToString() == "loaded")
                {

                    xmlPacket += "<item ";
                    xmlPacket += " ID = '" + Guid.NewGuid().ToString() + "' ";
                    xmlPacket += " DATE_MODIFIED = '" + DateTime.Now.ToLongTimeString() + "' ";
                    xmlPacket += " SERVER_IP = '" + _appConfig.public_ip_this_server + "' ";
                    xmlPacket += "PUB_POINT='" + dr["Name"].ToString() + "' ";
                    xmlPacket += "CURRENT_CONNECTIONS='" + dr["ConnectionsCurrent"].ToString() + "' ";
                    xmlPacket += "PEAK_CONNECTIONS='0' ";
                    xmlPacket += "CURRENT_THROUGHPUT='" + Math.Truncate( float.Parse( dr["MessagesOutBytesRate"].ToString() )/1000 ).ToString() + "' ";
                    xmlPacket += "PEAK_THROUGHPUT='0' ";
                    xmlPacket += "PUB_POINT_TYPE = '' ";
                    xmlPacket += "SERVICE_TYPE = 'WMZ' ";
                    xmlPacket += " />";
                    ttl ++;

            /*
                    dt = dtServerCounters.NewRow();
                    dt["ID"] = Guid.NewGuid();
                    dt["ServerIP"] = _appConfig.public_ip_this_server ;
                    dt["ServerName"] = Environment.MachineName ;
                    dt["ServerStatus"] = "WOWZA_SERVER_RUNNING";
                    dt["PubPointName"] = dr["Name"].ToString();
                    dt["PubPointType"] = "";
                    dt["PubPointStatus"] = "WOWZA_PUBLISHING_POINT_RUNNING";
                    dt["CounterType"] = "Current";
                    dt["CounterLastReset"] = DateTime.Now ;
                    dt["CounterName"] = "ConnectedPlayers";
                    dt["CounterValue"] = dr["ConnectionsCurrent"].ToString() ;
                    dt["DatetimeSaved"] = DateTime.Now;
                    dt["Packet"] = Packet;
                    dtServerCounters.Rows.Add(dt);
            
                    dt = dtServerCounters.NewRow();
                    dt["ID"] = Guid.NewGuid();
                    dt["ServerIP"] = _appConfig.public_ip_this_server ;
                    dt["ServerName"] = Environment.MachineName ;
                    dt["ServerStatus"] = "WOWZA_SERVER_RUNNING";
                    dt["PubPointName"] = dr["Name"].ToString();
                    dt["PubPointType"] = "";
                    dt["PubPointStatus"] = "WOWZA_PUBLISHING_POINT_RUNNING";
                    dt["CounterType"] = "Current";
                    dt["CounterLastReset"] = DateTime.Now ;
                    dt["CounterName"] = "PlayerAllocatedBandwidth";
                    dt["CounterValue"] = Math.Truncate( float.Parse( dr["MessagesOutBytesRate"].ToString() )/1000 );
                    dt["DatetimeSaved"] = DateTime.Now;
                    dt["Packet"] = Packet;
                    dtServerCounters.Rows.Add(dt);
             */
                }
            }

            xmlPacket += "</items>";

            evtLog.WriteEntry(" sending " + ttl.ToString() + " to " + config.public_ip_this_server);
            evtLog.WriteEntry(" sending " + xmlPacket );            

            List<SqlParameter> prms = new List<SqlParameter>();

            prms.Add(new SqlParameter("@SERVER_IP", _appConfig.public_ip_this_server ));
            prms.Add(new SqlParameter("@COUNTER_XML", xmlPacket ));
            sqlmgr.ExecuteStoredProcedure("spCOUNTERS_UpdateXml", prms);
            sqlmgr = null;
            prms = null;

            

            
           //     sqlmgr.SaveTable("Select * from WMSPubPointCountersStage where ServerIP = '-1' ", dtServerCounters);
            



        }



        
    }
}
