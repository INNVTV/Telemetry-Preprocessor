using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Worker.Models.Persistence.DocumentDatabase
{
    public class DocumentContext : IDocumentContext
    {
        public DocumentContext(IConfiguration configuration)
        {
            Settings = new Settings();

            #region Map appsettings.json

            Settings.Url = configuration
                .GetSection("Azure")
                .GetSection("CosmosDb")
                .GetSection("Url").Value;

            Settings.Key = configuration
                .GetSection("Azure")
                .GetSection("CosmosDb")
                .GetSection("Key").Value;

            Settings.ReadOnlyKey = configuration
                .GetSection("Azure")
                .GetSection("CosmosDb")
                .GetSection("ReadOnlyKey").Value;

            Settings.Database = configuration
                .GetSection("Azure")
                .GetSection("CosmosDb")
                .GetSection("Database").Value;

            Settings.Collection = configuration
                .GetSection("Azure")
                .GetSection("CosmosDb")
                .GetSection("Collection").Value;
            #endregion

            #region Generate the document client

            // Using a Singleton in your Di Container will ensure you have a DocumentClient instance always stored away for re-use.
            Client = new CosmosClient(
                    Settings.Url,
                    Settings.Key,
                    new CosmosClientOptions
                    {
                        MaxRetryAttemptsOnRateLimitedRequests = 6,
                        MaxRetryWaitTimeOnRateLimitedRequests = new TimeSpan(0, 0, 0, 0, 500)
                    });


            // We also keep Database and Container on hand within the DI Container
            Database = Client.GetDatabase(Settings.Database);
            Container = Client.GetContainer(Settings.Database, Settings.Collection);

            #endregion
        }

        public CosmosClient Client { get; set; }
        public Database Database { get; set; }
        public Container Container { get; set; }
        public Settings Settings { get; set; }
    }
}
