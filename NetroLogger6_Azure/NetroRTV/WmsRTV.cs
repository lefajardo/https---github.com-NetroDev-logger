/*using System;
using System.Collections.Generic;
using System.Text;
using NetroMedia.SQL;
using NetroMedia.DataAdapter;
using NetroMedia.DataModule.Service;
using NetroMedia.wmscounters;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.IO;
using NetroMedia.DBlog;

namespace NetroRTV
{
    public class WmsRTV
    {

        SqlManager sqlmgr;
        AppConfig config;
        EventLog evtLog = new EventLog();

      
        public void processWMScounters()
        {

            config = ConfigManager.LoadFromFile();

            wmscounters counters;
            sqlmgr = new SqlManager(getConnectionString());
            //WmsCountersClient sqlmgr;          

            evtLog.Source = "Application";
            evtLog.Log = "Application";

            string response = "";
            response = "";
            
            try
            {


                counters = new wmscounters();
                //sqlmgr = new WmsCountersClient();

                //evtLog.WriteEntry("querying server " + config.public_ip_this_server);

                response = counters.ppCounters2();

                //response = response.Replace("'", "&apos;");

                savePubPointCounters(response, config);

            }
            catch  ( Exception ex )
            {
                evtLog.WriteEntry(" error " + ex.Message );
                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WMSCounters, 0, "*NetroRTV succeed saving wms rtv info");
                //response

                //if (config.send_debug_emails)
                //{

                //    StreamWriter fileWriter = null;
                //    try
                //    {

                //        fileWriter = File.CreateText("c:\\RTV_Err_" + Guid.NewGuid().ToString());
                //        fileWriter.Write(response);
                //    }
                //    finally
                //    {
                //        if (fileWriter != null)
                //            fileWriter.Close();
                //    }
                //}

            }

            
            counters = null;

        }

        private string getConnectionString()
        {
            return "data source=" + config.RTV_Server // .CENTRAL_DB_IP
                        + ";initial catalog=" + config.RTV_Database // .CENTRAL_DB_Database
                        + ";password=" + config.RTV_Password // .CENTRAL_DB_Pwd
                        + ";user id=" + config.RTV_User // .CENTRAL_DB_User 
                        + (config.RTV_Encrypt == "YES" ? ";Encrypt=yes" : "") + ";";
                //    + ";persist security info=True;user id=" + config.RTV_User // .CENTRAL_DB_User 
                        
        }


        public void savePubPointCounters(string xmlServerInfo, AppConfig Config_)
        {
           AppConfig   _appConfig = Config_;

            string wmsIP = "";
            string wmsStatus = "";
            //string wmsType = "PubPoint";
            string wmsCountercName = "";
            string wmsCountercValue = "";
            string wmsCounterSetName = "";
            
            string wmshostname = "";
            string wmsPubPointName = "";
            string wmsPubPointType = "";
            string wmsPubPointStatus = "";
            //DataRow dt;
            int value_ = 0;
            int value2_ = 0;
            int ttl = 0;
            string xmlPacket = "";

            string cConnectedPlayers = "";
            string pConnectedPlayers = "";
            string cPlayerAllocatedBandwidth = "";
            string pPlayerAllocatedBandwidth = "";

                
            //try {

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlServerInfo);
            XmlNode root = xDoc.SelectSingleNode("//wms");

            xmlPacket = "<items>";
            ttl = 0;


            if (root.Attributes["found"].Value == "1")
            {

                wmsIP = root.Attributes["ip"].Value;
                wmsStatus = root.Attributes["status"].Value;
                wmshostname = root.Attributes["hostname"].Value;


                XmlNode pps = xDoc.SelectSingleNode("//pps");

                XmlNode ppnode = pps.FirstChild;
                do
                {


                    wmsPubPointName = ppnode.Attributes["name"].Value;
                    wmsPubPointType = ppnode.Attributes["type"].Value;
                    wmsPubPointStatus = ppnode.Attributes["status"].Value;

                    XmlNode rootC = ppnode.FirstChild;
                    //foreach (XmlNode rootC in ppnode.ChildNodes)
                    do
                    {


                        wmsCounterSetName = rootC.Attributes["name"].Value;
                        //if (wmsCounterSetName == "Current")
                        //{
                        //    wmsCounterSetLastReset = DateTime.Now.ToString();
                        //}
                        //else
                        //{
                        //    wmsCounterSetLastReset = rootC.Attributes["lastreset"].Value;
                        //}

                        foreach (XmlNode child in rootC.ChildNodes)
                        {
                            wmsCountercName = child.Attributes["name"].Value;
                            wmsCountercValue = child.Attributes["value"].Value;

                            switch (child.Attributes["name"].Value) { 
                            
                                case "cConnectedPlayers":
                                    cConnectedPlayers = child.Attributes["value"].Value;
                                    break;
                                case "pConnectedPlayers":
                                    pConnectedPlayers = child.Attributes["value"].Value;
                                    break;
                                case "cPlayerAllocatedBandwidth":
                                    cPlayerAllocatedBandwidth = child.Attributes["value"].Value;
                                    break;
                                case "pPlayerAllocatedBandwidth":
                                    pPlayerAllocatedBandwidth = child.Attributes["value"].Value;
                                    break;
                            }

                        }


                        value_ = 0;
                        value2_ = 0;

                        int.TryParse(cConnectedPlayers, out value_);
                        int.TryParse(cPlayerAllocatedBandwidth , out value2_);

                        if (value_ > 0 || value2_ > 0)
                        {
                            xmlPacket += "<item ";
                            xmlPacket += " ID = '" + Guid.NewGuid().ToString() + "' ";
                            xmlPacket += " DATE_MODIFIED = '" + DateTime.Now.ToLongTimeString() + "' ";
                            xmlPacket += " SERVER_IP = '" + _appConfig.public_ip_this_server + "' ";
                            xmlPacket += "PUB_POINT='" + wmsPubPointName + "' ";
                            xmlPacket += "CURRENT_CONNECTIONS='" + cConnectedPlayers + "' ";
                            xmlPacket += "PEAK_CONNECTIONS='" + pConnectedPlayers + "' ";
                            xmlPacket += "CURRENT_THROUGHPUT='" + cPlayerAllocatedBandwidth + "' ";
                            xmlPacket += "PEAK_THROUGHPUT='" + pPlayerAllocatedBandwidth + "' ";
                            xmlPacket += "PUB_POINT_TYPE = '" + wmsPubPointType + "' ";
                            xmlPacket += "SERVICE_TYPE = 'WMS' ";
                            xmlPacket += " />";
                            ttl++;
                        }
                        


                        rootC = rootC.NextSibling;

                    } while (rootC != null);


                    ppnode = ppnode.NextSibling;

                } while (ppnode != null);

                xmlPacket += "</items>";

                if (_appConfig.send_debug_emails)
                {
                    evtLog.WriteEntry(" sending " + ttl.ToString() + " to " + config.public_ip_this_server);
                    evtLog.WriteEntry(" sending " + xmlPacket.Length.ToString());

                    using (StreamWriter sw = new StreamWriter(Guid.NewGuid() + ".xml", false, Encoding.Default))
                    {
                        sw.Write(xmlPacket);
                        sw.Close();
                    }
                }

               

                



                List<SqlParameter> prms = new List<SqlParameter>();

                prms.Add(new SqlParameter("@SERVER_IP", _appConfig.public_ip_this_server));
                prms.Add(new SqlParameter("@COUNTER_XML", xmlPacket));
                sqlmgr.ExecuteStoredProcedure("spCOUNTERS_UpdateXml", prms);
                sqlmgr = null;
                prms = null;

                Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WMSCounters, 0, "*NetroRTV succeed saving wms rtv info");

            }

        }

    }
}
*/