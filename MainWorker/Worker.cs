using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared.Models.QueueMessages;
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
            double bufferMinutesLimit = -1.5;

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
                    int messagesSent = 0;

                    // Get next batch of telemetry, clean data, send message queus to workers
                    var telemetryDataResponse = await Tasks.GetTelemetryData.RunAsync(temporalState, _telemetryStorageContext);

                    if(telemetryDataResponse.IsSuccess)
                    {
                        var telemetryData = telemetryDataResponse.TelemetryData;

                        if (telemetryData != null && telemetryData.Count > 0)
                        {
                            // Process telemetry (merge data, convert to queue messages for task workers)
                            var queueMessages = await Tasks.ProcessTelemetry.RunAsync(telemetryData, temporalState);

                            // Send messages to task workers
                            var messageQueueResponse = await Tasks.SendQueueMessages.RunAsync(_preprocessorStorageContext, queueMessages);

                            // Update logging metrics
                            recordsProcessed = telemetryData.Count;
                            messagesSent = queueMessages.Count;
                        }

                        // Log last run temporal state
                        var loggingResult = await Tasks.LogLastTemporalState.RunAsync(_preprocessorStorageContext, temporalState, recordsProcessed, messagesSent);

                        _logger.LogInformation($"Temporal state: {temporalState.TemporalStateId} processed {recordsProcessed} records, sent {messagesSent} messages.");

                        // Sleep for 1 second
                        _logger.LogInformation("Main worker sleeping for 10 milliseconds.");
                        await Task.Delay(10, stoppingToken);

                    }
                    else
                    {                      
                        _logger.LogError($"There were issues getting telemetry data: {telemetryDataResponse.ErrorMessage}");

                        //TODO: Log issues running task
                        //TODO: Alert engineers

                        // Sleep for 1 second
                        _logger.LogInformation("Main worker sleeping for 1 second.");
                        await Task.Delay(1000, stoppingToken);
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
