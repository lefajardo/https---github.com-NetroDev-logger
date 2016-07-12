using System;
using System.Collections.Generic;
using System.Text;

namespace NetroMedia
{
    public class LogItem
    {
        public int UID                       { get; set; }

        public string C_IP                  { get; set; }
        public string Date                  { get; set; }
        public string Time                  { get; set; }
        public string C_DNS                 { get; set; }
        public string CS_URI_Stem           { get; set; }
        public string C_StartTime           { get; set; }
        public string X_Duration            { get; set; }
        public string C_Rate                { get; set; }
        public string C_Status              { get; set; }
        public string C_PlayerID            { get; set; }
        public string C_PlayerVersion       { get; set; }
        public string C_PlayerLanguage      { get; set; }
        public string CS_UeserAgent         { get; set; }
        public string C_OS                  { get; set; }
        public string C_OSVersion           { get; set; }
        public string C_CPU                 { get; set; }
        public string FileLength            { get; set; }
        public string FileSize              { get; set; }
        public string AvgBandWidth          { get; set; }
        public string Protocol              { get; set; }
        public string Transport             { get; set; }
        public string AudioCodec            { get; set; }
        public string VideoCodec            { get; set; }
        public string SC_Bytes              { get; set; }
        public string CS_Bytes              { get; set; }
        public string C_Bytes               { get; set; }
        public string S_Pkts_Sent           { get; set; }
        public string C_Pkts_Received       { get; set; }
        public string C_Pkts_Lost_Client    { get; set; }
        public string C_BufferCount         { get; set; }
        public string C_TotalBufferTime     { get; set; }
        public string C_Quality             { get; set; }
        public string S_IP                  { get; set; }
        public string S_DNS                 { get; set; }
        public string S_TotalClients        { get; set; }
        public string S_CPU_Util            { get; set; }
        public string CS_URI_Query          { get; set; }
        public string C_UserName            { get; set; }
        public string SC_Realm              { get; set; }
        public string SERVER_NAME { get; set; }
        public ServiceToParse FORMAT_CODE { get; set; }
        public string LOGFILENAME	{ get; set; }
        public DateTime LOGCREATEDATE { get; set; }


    }
}
