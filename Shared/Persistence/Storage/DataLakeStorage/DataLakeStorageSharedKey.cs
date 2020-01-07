using Azure.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.DataLake
{
    public class DataLakeStorageSharedKey : IDataLakeStorageSharedKey
    {
        public DataLakeStorageSharedKey(IConfiguration configuration)
        {

            Settings = new DataLakeStorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("DataLakeStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("DataLakeStorage")
                .GetSection("Key").Value;

            #endregion

            #region Create StorageAccount

            SharedKeyCredentials = new StorageSharedKeyCredential(Settings.Name, Settings.Key);;

            #endregion

            #region Generate URIs

            URIs = new StorageURIs();
            URIs.DistributedFileSystem = $"https://{Settings.Name}.dfs.core.windows.net"; 

            #endregion

        }

        public StorageSharedKeyCredential SharedKeyCredentials { get; set; }
        public DataLakeStorageSettings Settings { get; set; }
        public StorageURIs URIs { get; set; }
    }
}
