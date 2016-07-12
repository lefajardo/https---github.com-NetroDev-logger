using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using WMSServerLib;
using System.Diagnostics;

namespace NetroMedia.SharedFunctions
{
    public class wmscounters
    {

        IWMSPublishingPoint PPOINT;
//        string mvarServerStatus ;
        object[,]  mvarAllCurrentCounters;
  //      object[,] mvarAllTotalCounters;
        
        DateTime mvarAllTotalCountersStart;
        DateTime mvarAllPeakCountersStart;
        object[,] mvarAllPeakCounters;

        EventLog Logger = new EventLog();
        

		/*
			
			object[,] mvarServerCurrentCounters ;

			object[,] mvarServerTotalCounters ;

			DateTime  mvarServerTotalCountersStart ;

			object[,] mvarServerPeakCounters ;

			DateTime  mvarServerPeakCountersStart ;

        */

            private string getPPTypeName(WMS_PUBLISHING_POINT_TYPE typeEnum)
            {
                string retval = "";
                switch (typeEnum)
                {
                    case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_ON_DEMAND:
                        retval = "On-Demand";
                        break;
                    case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_BROADCAST:
                        retval = "Broadcast";
                        break;
                    case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_CACHE_PROXY_ON_DEMAND:
                        retval = "CACHE_PROXY_ONDEMAND";
                        break;
                    case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_CACHE_PROXY_BROADCAST:
                        retval = "CACHE_PROXY_BROADCAST";
                        break;
                    default:
                        retval = "N/A";
                        break;
                }
                return retval;
            }
        


            private Boolean loadPPCountersV3(IWMSPublishingPoint PPOINT)
            {
                Boolean retval;
                //retval = false;



                mvarAllCurrentCounters = (object[,])PPOINT.CurrentCounters.AllCounters;

                //mvarAllTotalCounters = (object[,])PPOINT.TotalCounters.AllCounters;

                mvarAllTotalCountersStart = PPOINT.TotalCounters.CountersStartTime;

                mvarAllPeakCounters = (object[,])PPOINT.PeakCounters.AllCounters;

                mvarAllPeakCountersStart = PPOINT.PeakCounters.CountersStartTime;

                retval = true;

                return retval;
            }



            private string GetIPAddress()
            {
                string strHostName = Dns.GetHostName();

                // Then using host name, get the IP address list..


                IPAddress[] addresses = Dns.GetHostEntry(strHostName).AddressList;


                if (addresses.Length > 0)
                {
                    return addresses[0].ToString();
                }
                else
                {
                    return "";
                }

            }


            public string ppCounters2()
            {

                StringBuilder FS = new StringBuilder();



                IWMSPublishingPoints pp;
                WMSServer oWMS = new WMSServer();

                Logger.Source = "Application";

                //Logger.WriteEntry(DateTime.Now.ToString() + "Starting ");

                pp = oWMS.PublishingPoints;
                FS.Append("<?xml version='1.0' ?> ");
                FS.Append("<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status + "' hostname='" + Dns.GetHostName() + "' found='1' >");
                FS.Append("<pps>");

                pp = oWMS.PublishingPoints;
                foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
                {


                    fillcounters2(p, FS);


                }

                FS.Append("</pps>");
                FS.Append("</wms>");



                return FS.ToString();
            }


            private void fillcounters2(IWMSPublishingPoint p, StringBuilder FS)
            {
                if (loadPPCountersV3(p))
                {


                    object[,] cc = mvarAllCurrentCounters;

                    FS.Append("<pp name='" + mangleXML(p.Name) + "' found='1' status='" + p.Status + "' type='" + p.Type + "' typename='" + getPPTypeName(p.Type) + "'>");
                    FS.Append("<currentcounterset type='pp' name='Current'>");
                    for (int i = 0; i < cc.Length / 2; i++)
                    {

                        if (cc[i, 0].ToString() == "PlayerAllocatedBandwidth" || cc[i, 0].ToString() == "ConnectedPlayers")
                        {
                            FS.Append("<counterc name='c" + cc[i, 0].ToString() + "' value='" + cc[i, 1].ToString() + "' />");
                        }

                    }
                    //FS.Append("</currentcounterset>");

                    //FS.Append("<peakcounterset type='pp' name='Peak' lastreset='" + mvarAllPeakCountersStart + "'>");
                    object[,] pc = mvarAllPeakCounters;

                    for (int i = 0; i < pc.Length / 2; i++)
                    {
                        if (pc[i, 0].ToString() == "PlayerAllocatedBandwidth" || pc[i, 0].ToString() == "ConnectedPlayers")
                        {
                            FS.Append("<counterp name='p" + pc[i, 0] + "' value='" + pc[i, 1] + "' />");
                        }
                    }
                    FS.Append("</currentcounterset>");
                    //FS.Append("</peakcounterset>");
                    FS.Append("</pp>");
                }
                else
                    FS.Append("<pp name='" + mangleXML(p.Name) + "' found='0' status=''></pp>");
            }


        private string mangleXML(string toMangle) {
            string tmp = toMangle;
            
            tmp = tmp.Replace("&", "&amp;");
            //tmp = tmp.Replace(" ", "&nbsp;");
            tmp = tmp.Replace("<", "&lt;");
            tmp = tmp.Replace(">", "&gt;");
            tmp = tmp.Replace(Convert.ToChar(34).ToString(), "&quot;"); // quote
            //tmp = tmp.Replace("(", "&#40;");
            //tmp = tmp.Replace(")", "&#41;");
            //tmp = tmp.Replace(Convert.ToChar(39).ToString(), "&#39;"); // apostrhophe
            //tmp = tmp.Replace(":", "&#58;");
            //tmp = tmp.Replace(";", "&#59;");
            //tmp = tmp.Replace("/", "&#47;");
            
            


            return tmp;

        }

/*
        private Boolean getPubPoint(string pName)
        {

            
            bool getPubPoint_ = false;
            WMSServer oWMS = null;
            if (pName.Length > 0)
            {
                oWMS = new WMSServer();

                mvarServerStatus = oWMS.Status.ToString();
                
                PPOINT = null;
                
                foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
                {
                
                    if (p.Name == pName)
                    {

                        getPubPoint_ = true;

                        PPOINT = p;
                        break;
                    }
                }


                
                oWMS = null;

             
            }
            return getPubPoint_;
        }
        




        private Boolean loadCounters(string ppname)
        {
            Boolean retval;
            retval = false;

            if (getPubPoint(ppname))
            {


                mvarAllCurrentCounters = (object[,])PPOINT.CurrentCounters.AllCounters;

                mvarAllTotalCounters = (object[,])PPOINT.TotalCounters.AllCounters;

                mvarAllTotalCountersStart = PPOINT.TotalCounters.CountersStartTime;

                mvarAllPeakCounters = (object[,])PPOINT.PeakCounters.AllCounters;

                mvarAllPeakCountersStart = PPOINT.PeakCounters.CountersStartTime;

                retval = true;
            }
            return retval;
        }





        private Boolean loadPPCountersV3(IWMSPublishingPoint  PPOINT)
        {
            Boolean retval;
            //retval = false;



                mvarAllCurrentCounters = (object[,])PPOINT.CurrentCounters.AllCounters;

                //mvarAllTotalCounters = (object[,])PPOINT.TotalCounters.AllCounters;

                mvarAllTotalCountersStart = PPOINT.TotalCounters.CountersStartTime;

                mvarAllPeakCounters = (object[,])PPOINT.PeakCounters.AllCounters;

                mvarAllPeakCountersStart = PPOINT.PeakCounters.CountersStartTime;

                retval = true;

            return retval;
        }


        public string ppCounters(string ppName)
        {

                StringBuilder FS = new StringBuilder();

             //   try
             //   {

                    IWMSPublishingPoints pp;
                    WMSServer oWMS = new WMSServer();

                //Logger.Source = "Application";

                //Logger.WriteEntry(DateTime.Now.ToString() + "Starting ");

                //Netro_Log4.writeLocalServiceLog("ppCounters", Environment.MachineName, 0, "Start to retrieve ppCounters");

                pp = oWMS.PublishingPoints;
                FS.Append("<?xml version='1.0' ?> ");
                FS.Append("<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status + "' hostname='" + Dns.GetHostName() + "' found='1' >");
                FS.Append("<pps>");

                pp = oWMS.PublishingPoints;
                foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
                {

                    if (ppName.Length == 0)
                    {

                        fillcounters(p, FS);

                    }
                    else
                        if (ppName.IndexOf(",") > -1)
                        {
                            if ((ppName + ",").IndexOf(p.Name + ",") > -1)
                            {
                                fillcounters(p, FS);
                            }
                        }
                        else
                        {
                            if (ppName == p.Name)
                            {
                                fillcounters(p, FS);
                            }
                        }
                }

                FS.Append("</pps>");
                FS.Append("</wms>");
               //// Logger.WriteEntry(DateTime.Now.ToString() + "String Generated ");
               // //if (ppName.Length == 0)
               // //{
               // //    //resetAllPPCounters();
               // //}

            //}
            //catch (Exception ex)
            //{
            //    Logger.WriteEntry(" error " + ex.Message + " " + DateTime.Now.ToString() );

            //    //return a empty string
            //    FS.Append("<?xml version='1.0' ?> ");
            //    FS.Append("<wms ip='' status='' hostname='' found='1' >");
            //    FS.Append("<pps>");
            //    FS.Append("</pps>");
            //    FS.Append("</wms>");
            //}



            return FS.ToString();
        }

        

        public string ppCounters()
        {
            return ppCounters("");
           
        }

        private void fillcounters(IWMSPublishingPoint p, StringBuilder FS)
        {
            if (loadPPCountersV3(p))
            {


                object[,] cc = mvarAllCurrentCounters;

                FS.Append("<pp name='" + mangleXML(p.Name) + "' found='1' status='" + p.Status + "' type='" + p.Type + "' typename='" + getPPTypeName(p.Type) + "'>");
                FS.Append("<currentcounterset type='pp' name='Current'>");
                for (int i = 0; i < cc.Length / 2; i++)
                {

                    if (cc[i, 0].ToString() == "PlayerAllocatedBandwidth" || cc[i, 0].ToString() == "ConnectedPlayers")
                    {
                        FS.Append("<counterc name='" + cc[i, 0].ToString() + "' value='" + cc[i, 1].ToString() + "' />");
                    }

                }
                //FS.Append("</currentcounterset>");
               

                //FS.Append("<peakcounterset type='pp' name='Peak' lastreset='" + mvarAllPeakCountersStart + "'>");
                object[,] pc = mvarAllPeakCounters;

                for (int i = 0; i < pc.Length / 2; i++)
                {
                    if (pc[i, 0].ToString() == "PlayerAllocatedBandwidth" || pc[i, 0].ToString() == "ConnectedPlayers" )
                    {
                        FS.Append("<counterp name='" + pc[i, 0] + "' value='" + pc[i, 1] + "' />");
                    }
                }
                //FS.Append("</peakcounterset>");
                FS.Append("</currentcounterset>");
                FS.Append("</pp>");
            }
            else
                FS.Append("<pp name='" + mangleXML(p.Name) + "' found='0' status=''></pp>");
        }


	

        private string getPPTypeName(WMS_PUBLISHING_POINT_TYPE typeEnum)
        {
            string retval = "";
            switch (typeEnum)
            {
                case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_ON_DEMAND:
                    retval = "On-Demand";
                    break;
                case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_BROADCAST:
                    retval = "Broadcast";
                    break;
                case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_CACHE_PROXY_ON_DEMAND:
                    retval = "CACHE_PROXY_ONDEMAND";
                    break;
                case WMS_PUBLISHING_POINT_TYPE.WMS_PUBLISHING_POINT_TYPE_CACHE_PROXY_BROADCAST:
                    retval = "CACHE_PROXY_BROADCAST";
                    break;
                default:
                    retval = "N/A";
                    break;
            }
            return retval;
        }
        



        public string premiumCounters(string pName)
        {

            WMSServer oWMS;
            IWMSServerCurrentCounters oSCurCounters;
            oWMS = new WMSServer();
            StringBuilder FS = new StringBuilder();


            if (pName.Length > 0)
            {
                if (getPubPoint(pName))
                {
                    oSCurCounters = oWMS.CurrentCounters;
                    FS.Append("<?xml version='1.0' ?> ");
                    FS.Append("<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status + "'");
                    FS.Append(" ppconns='" + PPOINT.CurrentCounters.ConnectedPlayers.ToString() + "'");


                    FS.Append(" srvconns='" + oSCurCounters.ConnectedPlayers.ToString() + "'");

                    FS.Append(" srvbandwidth='" + oSCurCounters.PlayerAllocatedBandwidth.ToString() + "'");
                    FS.Append(" name='" + pName + "' found='1' />");
                }
            }
            else
            {
                FS.Append("<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status + "'");
                FS.Append(" ppconns='' ppbandwidth='' srvconns='' srvbandwidth='' name='" + pName + "' found='0'  />");
            }
            return FS.ToString();

        }


        public string serverCounters()
        {

            StringBuilder FS = new StringBuilder();

            if (loadServerCounters())
            {
                FS.Append("<?xml version='1.0' ?> ");
                FS.Append("<wms ip='" + GetIPAddress() + "' wmsstatus='" + mvarServerStatus + "' found='1' hostname='" + Dns.GetHostName() + "' >");
                FS.Append("<countersets>");
                FS.Append("<currentcounterset type='Server' name='Current'>");


                object[,] cc = mvarServerCurrentCounters;

                for (int i = 0; i < cc.Length / 2; i++)
                {

                    FS.Append("<counterc name='" + cc[i, 0] + "' value='" + cc[i, 1] + "' />");
                }

                FS.Append("</currentcounterset>");


                FS.Append("<totalcounterset type='server' name='Total' lastreset='" + mvarServerTotalCountersStart.ToString() + "'>");
                object[,] tcc = mvarServerTotalCounters;

                for (int i = 0; i < tcc.Length / 2; i++)
                {

                    FS.Append("<countert name='" + tcc[i, 0] + "' value='" + tcc[i, 1] + "' />");
                }
                FS.Append("</totalcounterset>");


                FS.Append("<peakcounterset type='server' name='Peak' lastreset='" + mvarServerPeakCountersStart.ToString() + "'>");
                object[,] pc = mvarServerPeakCounters;
                for (int i = 0; i < pc.Length / 2; i++)
                {
                    FS.Append("<counterp name='" + pc[i, 0] + "' value='" + pc[i, 1] + "' />");
                }
                FS.Append("</peakcounterset>");
                FS.Append("</countersets>");
                FS.Append("</wms>");


               // resetAllServerCounters();
            }
            else
            {

                FS.Append("<server ip='" + GetIPAddress() + "' wmsstatus='" + mvarServerStatus + " found='0' '></server>");
            }
            return FS.ToString();
        }

        private bool loadServerCounters()
        {
            WMSServer oWMS = null ;
            oWMS = new WMSServer();


            mvarServerStatus = oWMS.Status.ToString();

            mvarServerCurrentCounters = (object[,])oWMS.CurrentCounters.AllCounters;

            mvarServerTotalCounters = (object[,])oWMS.TotalCounters.AllCounters;

            mvarServerTotalCountersStart = oWMS.TotalCounters.CountersStartTime;

            mvarServerPeakCounters = (object[,])oWMS.PeakCounters.AllCounters;

            mvarServerPeakCountersStart = oWMS.PeakCounters.CountersStartTime;


            oWMS = null;
            return true;
        }

        public string ppCount()
        {
            IWMSPublishingPoints pp;


            WMSServer oWMS;
            oWMS = new WMSServer();
            pp = oWMS.PublishingPoints;

            return "<?xml version='1.0' ?> " + "<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status.ToString() + "' numpps='" + pp.Count + "' />";

        }

        public string ppStatus(){

            return ppStatus("");

        }

        public string ppStatus(string pName)
        {


            WMSServer oWMS;
            oWMS = new WMSServer();
            IWMSPublishingPoints pp;
            bool found = false;
            StringBuilder FS = new StringBuilder();
            FS.Append("<?xml version='1.0' ?> ");
            FS.Append("<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status.ToString() + "'>");




            pp = oWMS.PublishingPoints;
            foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
            {

                if (pName.Length == 0)
                {
                    FS.Append("<pp name='" + p.Name + "' type='" + p.Type + "' typename='" + getPPTypeName(p.Type) + "' found='1' status='" + p.Status + "' />");
                    found = true;
                }
                else
                {
                    if ((pName + ",").IndexOf(p.Name) > -1)
                    {
                        FS.Append("<pp name='" + p.Name + "' type='" + p.Type + "' typename='" + getPPTypeName((p.Type)) + "'  found='1' status='" + p.Status + "' />");
                        found = true;
                    }
                }
            }

            if (!found)
            {
                FS.Append("<pp name='" + pName + "' type='' typename='' found='0' status='' />");
            }


            return FS.ToString() + "</wms>";

        }


        public string  DashboardCounters( ) {
            return DashboardCounters("") ;
        }

        public string DashboardCounters(string pName)
        {

            WMSServer oWMS;
            oWMS = new WMSServer();

            //IWMSPublishingPoints pp;
            bool found = false;
            StringBuilder FS = new StringBuilder();
            FS.Append("<?xml version='1.0' ?> ");
            FS.Append("<wms ip='" + GetIPAddress() + "' status='" + oWMS.Status.ToString() + "'>");



            
            foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
            {

                if (pName.Length == 0)
                {
                    FS.Append("<pp name='" + p.Name + "' found='1' type='" + p.Type + "' typename='" + getPPTypeName(p.Type) + "' " + " status='" + p.Status + "'" + " curconn='" + p.CurrentCounters.ConnectedPlayers.ToString() + "'" + " httpplayers='" + p.CurrentCounters.StreamingHTTPPlayers.ToString() + "'" + " mmsplayers='" + p.CurrentCounters.StreamingMMSPlayers.ToString() + "'" + " distconnections='" + p.CurrentCounters.OutgoingDistributionConnections.ToString() + "'" + " distbandwidth='" + p.CurrentCounters.OutgoingDistributionAllocatedBandwidth.ToString() + "'" + " playerbandwidth='" + p.CurrentCounters.PlayerAllocatedBandwidth.ToString() + "'" + " totalconns='" + p.TotalCounters.ConnectedPlayers.ToString() + "'" + " totalbytes='" + p.TotalCounters.PlayerBytesSent.ToString() + "'" + " peakbytes='" + p.PeakCounters.PlayerAllocatedBandwidth.ToString() + "' />");

                    found = true;
                }
                else
                {
                    if ((pName + ",").IndexOf(p.Name) > -1)
                    {
                        FS.Append("<pp name='" + p.Name + "' found='1' type='" + p.Type + "' typename='" + getPPTypeName(p.Type) + "' " + " status='" + p.Status + "'" + " curconn='" + p.CurrentCounters.ConnectedPlayers.ToString() + "'" + " httpplayers='" + p.CurrentCounters.StreamingHTTPPlayers.ToString() + "'" + " mmsplayers='" + p.CurrentCounters.StreamingMMSPlayers.ToString() + "'" + " distconnections='" + p.CurrentCounters.OutgoingDistributionConnections.ToString() + "'" + " distbandwidth='" + p.CurrentCounters.OutgoingDistributionAllocatedBandwidth.ToString() + "'" + " playerbandwidth='" + p.CurrentCounters.PlayerAllocatedBandwidth.ToString() + "'" + " totalconns='" + p.TotalCounters.ConnectedPlayers.ToString() + "'" + " totalbytes='" + p.TotalCounters.PlayerBytesSent.ToString() + "'" + " peakbytes='" + p.PeakCounters.PlayerAllocatedBandwidth.ToString() + "' />");
                        found = true;
                    }
                }
            }

            if (!found)
            {
                FS.Append("<pp name='" + pName + "' found='0'  />");

            }


            return FS.ToString() + "</wms>";
        }


    public void resetPPPeakCounters(){
        resetPPPeakCounters("");
    }

        public void resetPPPeakCounters(string pName)
        {



            WMSServer oWMS;
            oWMS = new WMSServer();



            foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
            {

                if (pName.Length == 0)
                {
                    p.PeakCounters.Reset();
                }
                else
                {
                    if ((pName + ",").IndexOf(p.Name) > -1)
                    {
                        p.PeakCounters.Reset();
                    }
                }
            }
            oWMS = null;
        }

	public void resetServerPeakCounters() {
		WMSServer oWMS ;
		oWMS = new WMSServer();
        oWMS.PeakCounters.Reset();
        oWMS = null;
	}
	
	public void resetServerTotalCounters(){
		WMSServer oWMS ;
		oWMS = new WMSServer();
        oWMS.TotalCounters.Reset();
        oWMS = null;
	}
	
	public void resetAllServerCounters(){
		WMSServer oWMS ;
		oWMS = new WMSServer();        
		oWMS.TotalCounters.Reset();
		oWMS.PeakCounters.Reset();
        oWMS = null;
	}

        public void resetAllPPCounters()
        {

            WMSServer oWMS;
            oWMS = new WMSServer();

            foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
            {



                p.TotalCounters.Reset();

                p.PeakCounters.Reset();
            }



            oWMS = null;
        }


        public void resetPPTotalCounters(){
            resetPPTotalCounters("");
        }

        public void resetPPTotalCounters(string pName ) {

            WMSServer oWMS;
            oWMS = new WMSServer();

            foreach (IWMSPublishingPoint p in oWMS.PublishingPoints)
            {


                if (pName.Length == 0)
                {
                p.TotalCounters.Reset();
                }
                else
                {
                    if ((pName + ",").IndexOf(p.Name) > -1)
                    {
                        p.TotalCounters.Reset();
                    }
                }

                
            }



            oWMS = null;		

        }
*/
    }
}
