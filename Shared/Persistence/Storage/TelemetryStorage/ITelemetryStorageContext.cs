using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.Telemetry
{
    public interface ITelemetryStorageContext
    {
        CloudStorageAccount StorageAccount { get; set; }
        TelemetryStorageSettings Settings { get; set; }
    }

    public class TelemetryStorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
