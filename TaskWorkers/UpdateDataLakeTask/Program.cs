using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Persistence.Storage.DataLake;
using Shared.Persistence.Storage.Preprocessor;

namespace UpdateDataLakeTask
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
                    IDataLakeStorageSharedKey dataLakeStorageSharedKey = new DataLakeStorageSharedKey(configuration); //<-- Latest (in preview)

                    services.AddSingleton(settings);
                    services.AddSingleton(preprocessorStorageContext);
                    services.AddSingleton(dataLakeStorageSharedKey);

                    services.AddHostedService<Worker>();
                });
    }
}
