using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence
{
    public interface IApplicationStorageAccount
    {
        ApplicationStorageSettings Settings { get; set; }
    }

    public class ApplicationStorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
