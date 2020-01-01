using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker.Models.Persistence.StorageAccount;
using Worker.Models.Persistence.StorageSharedKey;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Models.Configuration.Settings _settings;
        private readonly IStorageContext _storageContext;
        private readonly IStorageSharedKeyContext _storageSharedKeyContext;

        public Worker(ILogger<Worker> logger, Models.Configuration.Settings settings, IStorageContext storageContext, IStorageSharedKeyContext storageSharedKeyContext)
        {
            _logger = logger;
            _settings = settings;
            _storageContext = storageContext;
            _storageSharedKeyContext = storageSharedKeyContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{_settings.Application.Name} ({_settings.Application.Version}) running at: {DateTimeOffset.Now}");


                // Get next temporal state to run
                var temporalState = await Logging.TemporalStateManager.GetNextTemporalStateAsync();

                _logger.LogInformation($"Next temporal state:{temporalState.Table}|{temporalState.Partition}");

                // Get telemetry data to process for current temporal state

                #region Notes
                /*
                 
                 In this example we query to fiter out only telemetry that has a 'ContentId'

                 In some scenarios you may want to partition your temetry tables to focus on one entity type such as:
                    'platform', 'account', 'user', 'content', etc...

                You may aso have multiple preprocessors focusing on different aspects of each entity type.
                In this exampe the focus is purely on Content Views

                */
                #endregion

                #region Run All Tasks for Content

                var one = await Tasks.UpdateContentViewCount.RunAsync();
                var two = await Tasks.UpdateContentViewApplicationReports.RunAsync();
                var three = await Tasks.UpdateContentViewDataLakeReports.RunAsync();

                #endregion

                // TODO: Add timers, logging, etc...

                // Update last run
                var logged = await Logging.TemporalStateManager.UpdateLastTemporalStateAsync(temporalState);

                await Task.Delay(_settings.Application.Frequency, stoppingToken);
            }
        }
    }
}
