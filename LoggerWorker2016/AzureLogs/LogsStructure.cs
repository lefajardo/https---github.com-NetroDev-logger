using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureLogs
{
    public class LogsStructure
    {
        public string logname;
        public List<string> logcontents;
        public DateTime logenddatetime;

        public LogsStructure() {
            logcontents = new List<string>();
        }
    }
}
