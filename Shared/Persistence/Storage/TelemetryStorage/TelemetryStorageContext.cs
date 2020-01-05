using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.Telemetry
{
    public class TelemetryStorageContext : ITelemetryStorageContext
    {
        public TelemetryStorageContext(IConfiguration configuration)
        {

            Settings = new TelemetryStorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("TelemetryStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("TelemetryStorage")
                .GetSection("Key").Value;

            #endregion

            #region Create StorageAccount

            StorageAccount = CloudStorageAccount.Parse(
                string.Concat(
                    "DefaultEndpointsProtocol=https;AccountName=",
                    Settings.Name,
                    ";AccountKey=",
                    Settings.Key)
                );

            #endregion

        }

        public CloudStorageAccount StorageAccount { get; set; }
        public TelemetryStorageSettings Settings { get; set; }
    }
}
