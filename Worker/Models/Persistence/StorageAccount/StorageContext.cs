using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence.StorageAccount
{
    public class StorageContext : IStorageContext
    {
        public StorageContext(IConfiguration configuration)
        {

            Settings = new StorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("Azure")
                .GetSection("ApplicationStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("Azure")
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
        public StorageSettings Settings { get; set; }
    }
}
