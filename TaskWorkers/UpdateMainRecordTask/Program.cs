using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Persistence.Cosmos.MainApplication;
using Shared.Persistence.Storage.Preprocessor;

namespace UpdateMainRecordTask
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
                    Shared.Configuration.Settings settings = new Shared.Configuration.Settings(configuration);

                    // Create Persistence models
                    IPreprocessorStorageContext preprocessorStorageContext = new PreprocessorStorageContext(configuration);
                    IMainApplicationCosmosContext mainApplicationCosmosContext = new MainApplicationCosmosContext(configuration);

                    services.AddSingleton(settings);
                    services.AddSingleton(preprocessorStorageContext);
                    services.AddSingleton(mainApplicationCosmosContext);

                    services.AddHostedService<Worker>();
                });
    }
}
