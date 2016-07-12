using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading;
using NetroMedia.SharedFunctions;
using NetroMedia.InternalServices.Proxy;
using System.Xml;
using System.Data.SqlClient;


namespace NetroRTV5
{
    public partial class NetroRTV : ServiceBase
    {


        #region components

        System.Threading.Timer oTimer;        
        AppConfig5_Azure config;
        string[] services;

        

        #endregion

        public NetroRTV()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            config = ConfigManager.LoadFromFile5();
            config.ServerPublicIP = InternalServicesProxy.CallerIP();
            string servicesd = InternalServicesProxy.Services(config.Portal_Public_Key);
            Netro_Log4.writeLocalServiceLog("NetroRTV", Environment.MachineName, 0, "Services " + servicesd);
            services = servicesd.Split(new char[] { '/' });


            SetTimer();
        }

        protected override void OnStop()
        {
            config = null;
            oTimer.Dispose();
        }

        private void SetTimer()
        {

            TimerCallback oCallback = new TimerCallback(timer_Tick);
            oTimer = new Timer(oCallback, null, Convert.ToInt32(config.WMS_SecondsToWaitAfterACall) * 1000, Convert.ToInt32(config.WMS_SecondsToWaitAfterACall) * 1000);

        }


        private void timer_Tick(Object stateInfo)
        {
            foreach (string svc in services)
            {
                switch (svc)
                {
                    case "Wowza":
                        queryWOWZA();
                        break;
                    case "Windows Media":
                        queryWMS();
                        break;
                }
            }
        }

        private void queryWOWZA() {
            
            WowzaRTV WowzaProcessor = new WowzaRTV(config);            
            WowzaProcessor.processWowzaCounters();
            WowzaProcessor = null;
        }

        private void queryWMS()
        {
            WmsRTV WmsProcessor = new WmsRTV(config);
            WmsProcessor.processWMScounters();
            WmsProcessor = null;
        }

    }
}
