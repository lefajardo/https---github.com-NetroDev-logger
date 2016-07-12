using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using NetroAzureUpload;
using NetroMedia.InternalServices.Proxy;

namespace TestService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string servicesd = InternalServicesProxy.Services("0903CF0A-70F2-4ED0-9F27-579BAD87B7D2");
            //UploadFiles upl = new UploadFiles();
            //upl.upload(@"C:\netro\sql\SQL Scripts NetroReports\Functions", "*.sql", @"C:\netro\sql\SQL Scripts NetroReports\Functions\fnContent.2.sql", "DefaultEndpointsProtocol=https;AccountName=netro;AccountKey=Xdy1RZeapgoJJDbhF6zdQFIxItf+2dJyQu+sjSSbjuzsWecPMBLHzRwEQIF+K89ZEUg8ZpepAuSXL9KCONMpmQ==", "http://netro.blob.core.windows.net", "netrologs", Environment.MachineName,"Y","");
            //upl = null;
        }
    }
}
