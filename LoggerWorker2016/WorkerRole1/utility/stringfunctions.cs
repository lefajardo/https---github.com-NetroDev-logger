using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetroLogger_WorkerRole
{
    public   class stringfunctions
    {
        public static string fnContent(string xsuri) { 
            string content ;
            int protocolpos = 0;
            int mountpoint = 0; 
        //    string @serverip ;

            protocolpos = xsuri.IndexOf("//") + 2;
		    if ( protocolpos > 0 ) {
                content = xsuri.Substring( protocolpos );
            }
            else {
                content = xsuri;
            }
		
            mountpoint = content.IndexOf("/");
            if( mountpoint >= 0){
                content  = content.Substring ( mountpoint );
            }

            return content;
        }
        public static string fnServerIP(string xsuri) { 
            	string content = "";
                int protocolpos = 0;
                int mountpoint = 0;
                string serverip = "";

                protocolpos = xsuri.IndexOf( "//")+2;
            if ( protocolpos > 0 ) {
                content = xsuri.Substring( protocolpos );
            }
            else {
                content = xsuri;
            }
            mountpoint = content.IndexOf("/");
            if( mountpoint >= 0){
                serverip   = content.Substring ( 0, mountpoint );
            }

            return serverip;

        }

        public static int fnIPStringToNumber(string vcIPAddress)
        {

            int biOctetA = 0;
            int biOctetB = 0;
            int biOctetC = 0;
            int biOctetD = 0;
            int biIP = 0;

            string[] tblIP = vcIPAddress.Split(new char[] { '.' });

            if (tblIP.Length == 4)
            {

                biOctetA = Sql.ToInteger(tblIP[0]) * 256 * 256 * 256;
                biOctetB = Sql.ToInteger(tblIP[1]) * 256 * 256;
                biOctetC = Sql.ToInteger(tblIP[2]) * 256;
                biOctetD = Sql.ToInteger(tblIP[3]);
                biIP = biOctetA + biOctetB + biOctetC + biOctetD;
            }


            return biIP;
        }
        
        

        public static string fnPubPoint(string uri)
        {
            string content = uri;
            int  slashpos = 0;
            int count = 0;

	count = 1;

            if( content.StartsWith ("//" ))
            {
                content = content.Substring(2);
            }
	if( content.StartsWith (@"\\" ))
            {
                content = content.Substring(2);
            }
	if( content.StartsWith ("/" ) || content.StartsWith (@"\" ))
            {
                content = content.Substring(1);
            }

            slashpos = content.IndexOf("//");
	        if( slashpos >= 0 ){
                content = content.Substring( slashpos + 2 );
                count = 2;
            }

            slashpos = content.IndexOf(@"\\");
	        if( slashpos >= 0 ){
                content = content.Substring( slashpos + 2 );
                count = 2;
            }
            if( count == 2){
                slashpos = content.IndexOf(@"\");
                if ( slashpos >= 0 )
                {
                    content = content.Substring(slashpos + 1);
                    count --;
                }
                slashpos = content.IndexOf("/");
                if ( slashpos >= 0 )
                {
                    content = content.Substring(slashpos + 1);
                    count --;
                }
            }
             slashpos = content.IndexOf(@"\");
                if ( slashpos >= 0 )
                {
                    content = content.Substring(slashpos + 1);
                    count --;
                }
                slashpos = content.IndexOf("/");
                if ( slashpos >= 0 )
                {
                    content = content.Substring(0, slashpos );
                    count --;
                }

	

            return content;
        }

    }
}
