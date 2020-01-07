using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.Application
{
    public interface IApplicationStorageContext
    {
        CloudStorageAccount StorageAccount { get; set; }
        ApplicationStorageSettings Settings { get; set; }
    }

    public class ApplicationStorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
