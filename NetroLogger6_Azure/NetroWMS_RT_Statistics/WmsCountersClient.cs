using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using NetroMedia.DataModule.Service;

namespace NetroMedia.SQL
{
    public class WmsCountersClient
    {

        AppConfig _appConfig;

        private string getConnectionString(){
            return  "data source=" + _appConfig.CENTRAL_DB_IP 
                        + ";initial catalog=" + _appConfig.CENTRAL_DB_Database 
                        + ";password=" + _appConfig.CENTRAL_DB_Pwd 
                        + ";persist security info=True;user id=" + _appConfig.CENTRAL_DB_User  + ";";

        }


        private void saveTable(string table, DataTable dtServerCounters)
        {

            SqlManager sqlmgr = new SqlManager(getConnectionString());

             sqlmgr.SaveTable("Select * from " + table + " where ServerIP = '-1' ",dtServerCounters );

             sqlmgr = null;

        }

        private DataTable  getDataTable(string table){
                
                SqlManager sqlmgr = new SqlManager(getConnectionString());

                return sqlmgr.LoadTable( "Select * from " + table + " where ServerIP = '-1' ");

        }

        public void processPacket(Guid packet, AppConfig Config_)
        {
            _appConfig = Config_;
            SqlManager sqlmgr = new SqlManager(getConnectionString());
            List<SqlParameter> prms = new List<SqlParameter>();

            prms.Add( new SqlParameter("@Packet",packet ));
            sqlmgr.ExecuteStoredProcedure("WmsCounterPP_InsUpd", prms);
            sqlmgr = null;
            prms = null;
        }



        public void savePubPointCounters(string xmlServerInfo, AppConfig Config_, string server, Guid Packet)
        {
            _appConfig = Config_;

            string wmsIP = "";
            string wmsStatus = "";
            //string wmsType = "PubPoint";
            string wmsCountercName = "";
            string wmsCountercValue = "";
            string wmsCounterSetName = "";
            string wmsCounterSetLastReset = "";
            string wmshostname = "";
            string wmsPubPointName = "";
            string wmsPubPointType = "";
            string wmsPubPointStatus = "";
            DataRow dt;
            int value_ = 0;


            //try {

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlServerInfo);
            XmlNode root = xDoc.SelectSingleNode("//wms");

            if (root.Attributes["found"].Value == "1")
            {

                



                wmsIP = root.Attributes["ip"].Value;
                wmsStatus = root.Attributes["status"].Value;
                wmshostname = root.Attributes["hostname"].Value;

                DataTable dtServerCounters = getDataTable("WMSPubPointCountersStage");

                XmlNode pps = xDoc.SelectSingleNode("//pps");

                XmlNode ppnode = pps.FirstChild ;
                do
                //foreach (XmlNode ppnode in pps.ChildNodes)
                {


                    wmsPubPointName = ppnode.Attributes["name"].Value;
                    wmsPubPointType = ppnode.Attributes["type"].Value;
                    wmsPubPointStatus = ppnode.Attributes["status"].Value;

                    XmlNode rootC = ppnode.FirstChild;
                    //foreach (XmlNode rootC in ppnode.ChildNodes)
                    do
                    {


                        wmsCounterSetName = rootC.Attributes["name"].Value;
                        if (wmsCounterSetName == "Current")
                        {
                            wmsCounterSetLastReset = DateTime.Now.ToString();
                        }
                        else
                        {
                            wmsCounterSetLastReset = rootC.Attributes["lastreset"].Value;
                        }

                        foreach (XmlNode child in rootC.ChildNodes)
                        {
                            wmsCountercName = child.Attributes["name"].Value;
                            wmsCountercValue = child.Attributes["value"].Value;


                            try
                            {

                                value_ = -1;
                                int.TryParse(wmsCountercValue, out value_);
                                if (value_ > 0)
                                {

                                    // add rows to the counters table
                                    dt = dtServerCounters.NewRow();
                                    dt["ID"] = Guid.NewGuid();
                                    dt["ServerIP"] = wmsIP;
                                    dt["ServerName"] = wmshostname;
                                    dt["ServerStatus"] = wmsStatus;
                                    dt["PubPointName"] = wmsPubPointName;
                                    dt["PubPointType"] = wmsPubPointType;
                                    dt["PubPointStatus"] = wmsPubPointStatus;
                                    dt["CounterType"] = wmsCounterSetName;
                                    dt["CounterLastReset"] = wmsCounterSetLastReset;
                                    dt["CounterName"] = wmsCountercName;
                                    dt["CounterValue"] = wmsCountercValue;
                                    dt["DatetimeSaved"] = DateTime.Now;
                                    dt["Packet"] = Packet;

                                    dtServerCounters.Rows.Add(dt);

                                }
                            }
                            catch { }

                        }

                        rootC = rootC.NextSibling;

                    } while (rootC != null);

                    ppnode = ppnode.NextSibling;

                } while (ppnode != null);
                //saveTable("WMSPubPointCountersStage", dtServerCounters);


            }


        //}
        //catch //(Exception ex)
        //{



        //}


        }


        /*        public void saveServerCounters(string xmlServerInfo, AppConfig Config_, string server) {
                    _appConfig = Config_;

                    string wmsIP = "";
                    string wmsStatus = "";
                    string wmsType = "Server";
                    string wmsCountercName = "";
                    string wmsCountercValue = "";
                    string wmsCounterSetName = "";
                    string wmsCounterSetLastReset = "";
                    string wmshostname = "";
                    DataRow dt;
        

                    //try to save the xml info to the database, if any error occurs
                    //the xml will be saved to a xml file, pending to be reprocessed again


                    try
                    {

                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(xmlServerInfo);
                        XmlNode root = xDoc.SelectSingleNode("//wms");

                        if (root.Attributes["found"].Value == "1")
                        {

                            DataTable dtServerCounters = getDataTable("WMSServerCounters");



                            wmsIP = root.Attributes["ip"].Value;
                            wmsStatus = root.Attributes["wmsstatus"].Value;
                            wmshostname = root.Attributes["hostname"].Value;

                            XmlNode pps = xDoc.SelectSingleNode("//countersets");




                            root = xDoc.SelectSingleNode("//currentcounterset");

                            wmsCounterSetName = "Current";
                            wmsCounterSetLastReset = DateTime.Now.ToString();

                            foreach (XmlNode child in root.ChildNodes)
                            {
                                wmsCountercName = child.Attributes["name"].Value;
                                wmsCountercValue = child.Attributes["value"].Value;


                                // add rows to the counters table
                                dt = dtServerCounters.NewRow();
                                dt["ID"] = Guid.NewGuid();
                                dt["ServerIP"] = wmsIP;
                                dt["ServerName"] = wmshostname;
                                dt["ServerStatus"] = wmsStatus;
                                dt["CounterType"] = wmsCounterSetName;
                                dt["CounterLastReset"] = wmsCounterSetLastReset;
                                dt["CounterName"] = wmsCountercName;
                                dt["CounterValue"] = wmsCountercValue;
                                dt["DatetimeSaved"] = DateTime.Now;
                                dtServerCounters.Rows.Add(dt);


                            }

                            root = xDoc.SelectSingleNode("//totalcounterset");

                            wmsCounterSetName = root.Attributes["name"].Value;
                            wmsCounterSetLastReset = root.Attributes["lastreset"].Value;

                            foreach (XmlNode child in root.ChildNodes)
                            {
                                wmsCountercName = child.Attributes["name"].Value;
                                wmsCountercValue = child.Attributes["value"].Value;


                                // add rows to the counters table
                                dt = dtServerCounters.NewRow();
                                dt["ID"] = Guid.NewGuid();
                                dt["ServerIP"] = wmsIP;
                                dt["ServerName"] = wmshostname;
                                dt["ServerStatus"] = wmsStatus;
                                dt["CounterType"] = wmsCounterSetName;
                                dt["CounterLastReset"] = wmsCounterSetLastReset;
                                dt["CounterName"] = wmsCountercName;
                                dt["CounterValue"] = wmsCountercValue;
                                dt["DatetimeSaved"] = DateTime.Now;
                                dtServerCounters.Rows.Add(dt);



                            }

                            root = xDoc.SelectSingleNode("//peakcounterset");

                            wmsCounterSetName = root.Attributes["name"].Value;
                            wmsCounterSetLastReset = root.Attributes["lastreset"].Value;

                            foreach (XmlNode child in root.ChildNodes)
                            {
                                wmsCountercName = child.Attributes["name"].Value;
                                wmsCountercValue = child.Attributes["value"].Value;

                                // add rows to the counters table
                                dt = dtServerCounters.NewRow();
                                dt["ID"] = Guid.NewGuid();
                                dt["ServerIP"] = wmsIP;
                                dt["ServerName"] = wmshostname;
                                dt["ServerStatus"] = wmsStatus;
                                dt["CounterType"] = wmsCounterSetName;
                                dt["CounterLastReset"] = wmsCounterSetLastReset;
                                dt["CounterName"] = wmsCountercName;
                                dt["CounterValue"] = wmsCountercValue;
                                dt["DatetimeSaved"] = DateTime.Now;
                                dtServerCounters.Rows.Add(dt);



                            }

                            saveTable("WMSServerCounters", dtServerCounters);


                        }

                    } catch( Exception ex ) {

                        File.WriteAllText("SC"+ server +
                            DateTime.Now.ToString().Replace(" ","").Replace(":",""), xmlServerInfo );



                    }

                }

                */
    }
}
