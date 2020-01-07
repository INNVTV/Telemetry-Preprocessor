using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Constants
{
    public static class TableNames
    {
        // Telemetry Storage
        public readonly static string TelemetryBase = "activitybyhour";

        // Preprocessor Storage
        public readonly static string MainWorkerLog = "mainworkerlog";

        // Application Storage
        public readonly static string ContentViewCounts = "contentviewcounts";
        public readonly static string ContentViewHourlyReports = "contentviewreportsbyhour";
    }
}
