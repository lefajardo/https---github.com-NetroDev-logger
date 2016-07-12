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
using NetroMedia.wmscounters;
using NetroMedia.DBlog;


namespace NetroWMS_RT_Statistics
{
    public partial class NetroWMS_RT_Statistics : ServiceBase
    {
        public NetroWMS_RT_Statistics()
        {
            InitializeComponent();
        }


        #region components

        System.Threading.Timer oTimer;

        #endregion

        AppConfig config = new AppConfig();

        protected override void OnStart(string[] args)
        {
            config = ConfigManager.LoadFromFile();

            SetTimer();
        }

        private void SetTimer()
        {

            TimerCallback oCallback = new TimerCallback(timer_Tick);
            oTimer = new Timer(oCallback, null, Convert.ToInt32(config.WMS_SecondsToWaitAfterACall) * 1000, Convert.ToInt32(config.WMS_SecondsToWaitAfterACall) * 1000);

        }


        private void timer_Tick(Object stateInfo)
        {

            queryWMS();
        }

        protected override void OnStop()
        {
            config = null;
            oTimer.Dispose();
        }

        int totalFaults = 0;

        void queryWMS()
        {



            config = ConfigManager.LoadFromFile();

            wmscounters counters ;
            WmsCountersClient sqlmgr ;
            EventLog evtLog;

            evtLog = new EventLog();
            evtLog.Source = "NetroWMS_RT_Statistics";



            try
            {

                string response = "";
                Guid Packet = Guid.Empty; ;
                


                response = "";
                Packet = Guid.NewGuid();
                
                counters = new wmscounters();
                sqlmgr = new WmsCountersClient();

                //evtLog.WriteEntry("querying server " + config.public_ip_this_server);

                response = counters.ppCounters("");

                sqlmgr.savePubPointCounters(response, config, config.public_ip_this_server, Packet);

               // sqlmgr.processPacket(Packet, config);

                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WMSCounters, 0, "Success retrieving wms counters " + " " + config.public_ip_this_server );

                totalFaults = 0;

            }
            catch (Exception ex)
            {

                
                totalFaults ++;
                if (totalFaults == 120)
                {
                   // Netro_Log4.sendEmail(NetroMedia.ServiceToParse.WMSCounters, 0, "*Error retrieving wms counters " + " " + ex.Message);
                }
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WMSCounters, 0, "*Error retrieving wms counters " + " " + ex.Message);
                
                //evtLog.WriteEntry(ex.Message);
            }


            sqlmgr = null;
            counters = null;

        }




    }
}
