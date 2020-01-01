using Azure.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence.StorageSharedKey
{
    public class StorageSharedKeyContext : IStorageSharedKeyContext
    {
        public StorageSharedKeyContext(IConfiguration configuration)
        {

            Settings = new StorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("Azure")
                .GetSection("DataLakeStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("Azure")
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
        public StorageSettings Settings { get; set; }
        public StorageURIs URIs { get; set; }
    }
}
