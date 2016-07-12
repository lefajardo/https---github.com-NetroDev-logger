using System;
using System.Collections.Generic;
using System.Text;
//using NetroMedia.SharedFunctions;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.IO;


namespace  NetroMedia.SharedFunctions
{
    public class WmsRTV
    {

        SqlManager sqlmgr;
        AppConfig5_Azure config;

        public WmsRTV(AppConfig5_Azure config_) {
            config = config_;
        }

      
        public void processWMScounters()
        {

            
            wmscounters counters;
            sqlmgr = new SqlManager(getConnectionStringRTV());
            

            string response = "";
            response = "";

            //Netro_Log4.writeLocalServiceLog("WMS RTV", config.public_ip_this_server, 0, "Start to query server");
            
            try
            {


                counters = new wmscounters();
                //sqlmgr = new WmsCountersClient();



                //evtLog.WriteEntry("querying server " + config.public_ip_this_server);

                response = counters.ppCounters2();

                response = response.Replace("'", "&apos;");

                savePubPointCounters(response, config.ServerPublicIP);

               // Netro_Log4.writeLocalServiceLog("WMS RTV", config.public_ip_this_server, 0, "Success query server");
            }
            catch  ( Exception ex )
            {
                //evtLog.WriteEntry(" error " + ex.Message );

                Netro_Log4.writeLocalServiceLog("WMS RTV", config.ServerPublicIP, 0, "Error. " + ex.Message );

                
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

        private string getConnectionStringRTV()
        {
            return "data source=" + config.RTV_Server 
                        + ";initial catalog=" + config.RTV_Database 
                        + ";password=" + config.RTV_Password 
                        + ";user id=" + config.RTV_User 
                        + (config.RTV_Encrypt == "YES" ? ";Encrypt=yes" : "") + ";";
                
                        
        }


        public void savePubPointCounters(string xmlServerInfo, string server)
        {
           

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
                            xmlPacket += " SERVER_IP = '" + server + "' ";
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

                                



                List<SqlParameter> prms = new List<SqlParameter>();

                prms.Add(new SqlParameter("@SERVER_IP", server));
                prms.Add(new SqlParameter("@COUNTER_XML", xmlPacket));
                sqlmgr.ExecuteStoredProcedure("spCOUNTERS_UpdateXml", prms);
                sqlmgr = null;
                prms = null;

                //Netro_Log4.writeLocalServiceLog("WMS RTV", config.public_ip_this_server, 0, "*NetroRTV succeed saving wms rtv info");
                //Netro_Log4.writeServiceLog(NetroMedia.ServiceToParse.WMSCounters, 0, "*NetroRTV succeed saving wms rtv info");

            }

        }

    }
}
