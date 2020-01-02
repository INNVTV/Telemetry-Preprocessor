using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker.Models.Configuration;
using Worker.Models.Persistence;
using Worker.Models.Persistence.DocumentDatabase;
using Worker.Models.Persistence.RedisCache;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Pull in AppSettings
                    IConfiguration configuration = hostContext.Configuration;
                    Models.Configuration.Settings settings = new Models.Configuration.Settings(configuration);

                    // Create Persistence models
                    //IDocumentContext documentContext = new DocumentContext(configuration);
                    IApplicationStorageAccount applicationStorageAccount = new ApplicationStorageAccount(configuration); //<-- Legacy
                    IDataLakeStorageSharedKey dataLakeStorageSharedKey = new DataLakeStorageSharedKey(configuration); //<-- Latest (in preview)
                    //IRedisContext redisContext = new RedisContext(configuration);

                    services.AddSingleton(settings);
                    services.AddSingleton(applicationStorageAccount);
                    services.AddSingleton(dataLakeStorageSharedKey);

                    services.AddHostedService<Worker>();
                });
    }
}
