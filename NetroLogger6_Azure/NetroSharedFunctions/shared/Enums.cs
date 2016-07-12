using System;
using System.Collections.Generic;
using System.Text;

namespace NetroMedia
{
    public enum FileFormat
    {
        Format1 = 1,
        Format2 = 2
    }

    public enum ServiceToParse
    {
        Unknown = 0,
        QuickTime = 1,
        SHOUTcast = 5,
        ICEcast = 4,
        WinMedia = 3,
        Wowza = 2,
        MP3 = 6,
        Red5 = 7,
        FTP = 8,
        WMSCounters = 9,
        ServiceChecker = 10,
        CustomReports = 11,
        WOWCounters = 12
    }
}
