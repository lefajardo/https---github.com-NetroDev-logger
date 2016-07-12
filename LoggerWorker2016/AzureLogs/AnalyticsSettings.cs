using System;

namespace AzureLogs
{

    [Flags]
    public enum LoggingLevel
    {
        None = 0,
        Delete = 2,
        Write = 4,
        Read = 8,
    }

    [Flags]
    public enum MetricsType
    {
        None = 0x0,
        ServiceSummary = 0x1,
        ApiSummary = 0x2,
        All = ServiceSummary | ApiSummary,
    }

    /// <summary>
    /// The analytic settings that can set/get
    /// </summary>
    public class AnalyticsSettings
    {
        public static string Version = "1.0";

        public AnalyticsSettings()
        {
            this.LogType = LoggingLevel.None;
            this.LogVersion = AnalyticsSettings.Version;
            this.IsLogRetentionPolicyEnabled = false;
            this.LogRetentionInDays = 0;

            this.MetricsType = MetricsType.None;
            this.MetricsVersion = AnalyticsSettings.Version;
            this.IsMetricsRetentionPolicyEnabled = false;
            this.MetricsRetentionInDays = 0;
        }

        /// <summary>
        /// The type of logs subscribed for
        /// </summary>
        public LoggingLevel LogType { get; set; }

        /// <summary>
        /// The version of the logs
        /// </summary>
        public string LogVersion { get; set; }

        /// <summary>
        /// Flag indicating if retention policy is set for logs in $logs
        /// </summary>
        public bool IsLogRetentionPolicyEnabled { get; set; }

        /// <summary>
        /// The number of days to retain logs for under $logs container
        /// </summary>
        public int LogRetentionInDays { get; set; }

        /// <summary>
        /// The metrics version
        /// </summary>
        public string MetricsVersion { get; set; }

        /// <summary>
        /// A flag indicating if retention policy is enabled for metrics
        /// </summary>
        public bool IsMetricsRetentionPolicyEnabled { get; set; }

        /// <summary>
        /// The number of days to retain metrics data
        /// </summary>
        public int MetricsRetentionInDays { get; set; }

        private MetricsType metricsType = MetricsType.None;

        /// <summary>
        /// The type of metrics subscribed for
        /// </summary>
        public MetricsType MetricsType
        {
            get
            {
                return metricsType;
            }

            set
            {
                if (value == MetricsType.ApiSummary)
                {
                    throw new ArgumentException("Including just ApiSummary is invalid.");
                }

                this.metricsType = value;
            }
        }
    }
}