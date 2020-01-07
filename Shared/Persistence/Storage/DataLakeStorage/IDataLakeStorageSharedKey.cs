using Azure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.DataLake
{
    public interface IDataLakeStorageSharedKey
    {
        StorageSharedKeyCredential SharedKeyCredentials { get; set; }
        DataLakeStorageSettings Settings { get; set; }
        StorageURIs URIs { get; set; }
    }

    public class DataLakeStorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class StorageURIs
    {
        public string DistributedFileSystem { get; set; }
    }
}
