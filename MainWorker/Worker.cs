using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared.Persistence.Storage.Preprocessor;
using Shared.Persistence.Storage.Telemetry;

namespace MainWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelemetryStorageContext _telemetryStorageContext;
        private readonly IPreprocessorStorageContext _preprocessorStorageContext;

        public Worker(ILogger<Worker> logger, ITelemetryStorageContext telemetryStorageContext, IPreprocessorStorageContext preprocessorStorageContext)
        {
            _logger = logger;
            _telemetryStorageContext = telemetryStorageContext;
            _preprocessorStorageContext = preprocessorStorageContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Once we are caught up within 2 minutes of temporal data from our telemetry data we slow down our processing
            double bufferMinutesLimit = -2;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Get next temporal state to run
                var temporalState = await Tasks.NextTemporalState.GetAsync(_preprocessorStorageContext);

                // Get buffer delta in minutes to determine if worker should continue processing telemetry or sleep
                DateTime nextRunDateTime = Shared.Transformations.ConvertTemporalStateToDateTime(temporalState);
                DateTime currentDateTime = DateTime.UtcNow;
                double bufferMinutes = (nextRunDateTime - currentDateTime).TotalMinutes;
             
                if (bufferMinutes <= bufferMinutesLimit)
                {
                    int recordsProcessed = 0;

                    // Get next batch of telemetry, clean data, send message queus to workers
                    var result = await Tasks.ProcessTelemetry.RunAsync(temporalState);
                   
                    if(result)
                    {
                        // Log last run temporal state
                        var loggingResult = await Tasks.LogLastTemporalState.RunAsync(_preprocessorStorageContext, temporalState, recordsProcessed);

                        _logger.LogInformation($"Temporal state: {temporalState.TemporalStateId} processed.");

                        // Sleep for 1 second
                        _logger.LogInformation("Main worker sleeping for 1 second.");
                        await Task.Delay(1000, stoppingToken);
                    }
                    else
                    {
                        //TODO: Log issues running task
                        //TODO: Alert engineers
                    }
                }
                else
                {
                    _logger.LogInformation("Caught up to buffer limit.");

                    // Sleep for 1 minute
                    _logger.LogInformation("Main worker sleeping for 1 minute.");
                    await Task.Delay(60000, stoppingToken);
                }
            }
        }
    }
}
