using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence.StorageAccount
{
    public interface IStorageContext
    {
        CloudStorageAccount StorageAccount { get; set; }
        StorageSettings Settings { get; set; }
    }

    public class StorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
