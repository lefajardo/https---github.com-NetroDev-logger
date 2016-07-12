using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using NetroMedia.wmscounters;

[WebService(Namespace = "http://netromedia.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class wmsCountersV3 : System.Web.Services.WebService
{
    public wmsCountersV3()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }



    [WebMethod]
    public string DashboardCounters(string pubPointName)
    {
        wmscounters wmscounters_ = new wmscounters();
        string result = wmscounters_.DashboardCounters(pubPointName);
        wmscounters_ = null;
        return result;
    }


    [WebMethod]
    public string ppCount()
    {
        wmscounters wmscounters_ = new wmscounters();
        string result = wmscounters_.ppCount();
        wmscounters_ = null;
        return result;
    }

    [WebMethod]
    public string ppCounters(string pubPointName)
    {
        wmscounters wmscounters_ = new wmscounters();
        string result = wmscounters_.ppCounters(pubPointName);
        wmscounters_ = null;
        return result;
    }


    [WebMethod]
    public string ppStatus(string pubPointName)
    {
        wmscounters wmscounters_ = new wmscounters();
        string result = wmscounters_.ppStatus(pubPointName);
        wmscounters_ = null;
        return result;
    }


    [WebMethod]
    public string premiumCounters(string pubPointName)
    {
        wmscounters wmscounters_ = new wmscounters();
        string result = wmscounters_.premiumCounters(pubPointName);
        wmscounters_ = null;
        return result;
    }


    [WebMethod]
    public void resetAllPPCounters()
    {
        wmscounters wmscounters_ = new wmscounters();
        wmscounters_.resetAllPPCounters();
        wmscounters_ = null;

    }


    [WebMethod]
    public void resetAllServerCounters()
    {
        wmscounters wmscounters_ = new wmscounters();
        wmscounters_.resetAllServerCounters();
        wmscounters_ = null;

    }

    [WebMethod]
    public void resetPPPeakCounters(string pubPointName)
    {
        wmscounters wmscounters_ = new wmscounters();
        wmscounters_.resetPPPeakCounters(pubPointName);
        wmscounters_ = null;

    }

    [WebMethod]
    public void resetPPTotalCounters(string pubPointName)
    {
        wmscounters wmscounters_ = new wmscounters();
        wmscounters_.resetPPTotalCounters(pubPointName);
        wmscounters_ = null;

    }

    [WebMethod]
    public void resetServerPeakCounters()
    {
        wmscounters wmscounters_ = new wmscounters();
        wmscounters_.resetServerPeakCounters();
        wmscounters_ = null;

    }

    [WebMethod]
    public void resetServerTotalCounters()
    {
        wmscounters wmscounters_ = new wmscounters();
        wmscounters_.resetServerTotalCounters();
        wmscounters_ = null;

    }

    [WebMethod]
    public string serverCounters()
    {
        wmscounters wmscounters_ = new wmscounters();
        string result = wmscounters_.serverCounters();
        wmscounters_ = null;
        return result;
    }

}