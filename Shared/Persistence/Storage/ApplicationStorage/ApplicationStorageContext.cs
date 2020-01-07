using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.Application
{
    public class ApplicationStorageContext : IApplicationStorageContext
    {
        public ApplicationStorageContext(IConfiguration configuration)
        {

            Settings = new ApplicationStorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("ApplicationStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("ApplicationStorage")
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
        public ApplicationStorageSettings Settings { get; set; }
    }
}
