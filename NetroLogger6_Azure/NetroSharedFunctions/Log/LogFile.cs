using System;
using System.Collections.Generic;
using System.Text;

namespace NetroMedia
{
    public class LogFile
    {
        #region Fields

        public string FileName  { get; set; }
        public string Created   { get; set; }
        public string Software  { get; set; }
        public string Version   { get; set; }
        public string Date      { get; set; }
        public string Remark    { get; set; }

        public List<LogItem> Items { get; set; }

        #endregion

        #region Constructor

        public LogFile()
        {
            Items = new List<LogItem>();
        }

        #endregion
    }
}
