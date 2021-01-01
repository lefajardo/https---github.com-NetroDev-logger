using System;
using System.Collections.Generic;
using System.Text;
using InternalServices.Proxy.InternalServices.Soap;
using System.Data;
using System.Net;

namespace NetroMedia.InternalServices.Proxy
{
    public static class InternalServicesProxy
    {

        public static DataTable Server_Line_Items(string portalKey)
        {

            DataTable ret;

            
            Soapzor soapsrv = new Soapzor();

            ret = soapsrv.ServerLineItems(new Guid(portalKey), soapsrv.CallerIP());
            soapsrv = null;
            return ret;

        }

        public static DataTable Server_Line_Items(string ServerIP, string portalKey)
        {

            DataTable ret;

            
            Soapzor soapsrv = new Soapzor();
            ret = soapsrv.ServerLineItems(new Guid(portalKey), ServerIP);
            soapsrv = null;
            return ret;
        }

        public static string Services(string portalKey)
        {
            string ret = "";
            Soapzor soapsrv = new Soapzor();

            ret = soapsrv.ServersPrimaryService(new Guid(portalKey), "174.120.123.2");
      
            //ret = soapsrv.services .CallerIP();
            //ret = soapsrv.
             //null;
            return ret;
        }

        public static string CallerIP()
        {
            string ret = "";
            /*Soapzor soapsrv = new Soapzor();
            ret = soapsrv.CallerIP();
            soapsrv = null;
            */
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            ret = a3[0];
            
            return ret;
        }

    }
}
