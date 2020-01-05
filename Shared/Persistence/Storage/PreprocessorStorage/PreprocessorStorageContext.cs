using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.Preprocessor
{
    public class PreprocessorStorageContext : IPreprocessorStorageContext
    {
        public PreprocessorStorageContext(IConfiguration configuration)
        {

            Settings = new PreprocessorStorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("PreprocessorStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("PreprocessorStorage")
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
        public PreprocessorStorageSettings Settings { get; set; }
    }
}
