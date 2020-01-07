using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Cosmos.MainApplication
{
    public interface IMainApplicationCosmosContext
    {
        CosmosClient Client { get; set; }
        Database Database { get; set; }
        Container Container { get; set; }
        Settings Settings { get; set; }
    }

    public class Settings
    {
        public string Url { get; set; }
        public string Key { get; set; }
        public string ReadOnlyKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
