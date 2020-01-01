using Azure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence.StorageSharedKey
{
    public interface IStorageSharedKeyContext
    {
        StorageSharedKeyCredential SharedKeyCredentials { get; set; }
        StorageSettings Settings { get; set; }
        StorageURIs URIs { get; set; }
    }

    public class StorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class StorageURIs
    {
        public string DistributedFileSystem { get; set; }
    }
}
