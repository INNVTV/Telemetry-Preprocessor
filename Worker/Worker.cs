using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker.Models.Persistence;
using Worker.Models.TableEntities;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Models.Configuration.Settings _settings;
        private readonly IApplicationStorageAccount _applicationStorageAccount;
        private readonly IDataLakeStorageSharedKey _dataLakeStorageSharedKey;

        public Worker(ILogger<Worker> logger, Models.Configuration.Settings settings, IApplicationStorageAccount applicationStorageAccount, IDataLakeStorageSharedKey dataLakeStorageSharedKey)
        {
            _logger = logger;
            _settings = settings;
            _applicationStorageAccount = applicationStorageAccount;
            _dataLakeStorageSharedKey = dataLakeStorageSharedKey;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{_settings.Application.Name} ({_settings.Application.Version}) running at: {DateTimeOffset.Now}");


                // Get next temporal state to run
                var temporalState = await Logging.TemporalStateManager.GetNextTemporalStateAsync();

                _logger.LogInformation($"Next temporal state: {temporalState.TemporalTableSegment} | {temporalState.TablePartition}");

                // Get all telemetry data to process for current temporal state:

                #region Query table data

                var tableName = _settings.Application.TemporalSourceTable + temporalState.TemporalTableSegment;

                var dataLakeCloudStorageAccount = CloudStorageAccount.Parse(
                string.Concat(
                    "DefaultEndpointsProtocol=https;AccountName=",
                    _dataLakeStorageSharedKey.Settings.Name,
                    ";AccountKey=",
                    _dataLakeStorageSharedKey.Settings.Key)
                );

                CloudTableClient tableClient = dataLakeCloudStorageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference(tableName);

                TableQuery<SourceActivityLog> query = new TableQuery<SourceActivityLog>()
                    .Where(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, temporalState.TablePartition),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("Activity", QueryComparisons.Equal, "View") //<-- we only want records that have an activity type of "View"
                    )).Take(1000);

                var results = await table.ExecuteQuerySegmentedAsync<SourceActivityLog>(query, null);

                List<SourceActivityLog> telemetryData = results.ToList();

                // Iterate until we get all results
                while(results.ContinuationToken != null)
                {
                    results = await table.ExecuteQuerySegmentedAsync<SourceActivityLog>(query, results.ContinuationToken);
                    telemetryData.AddRange(results);
                }

                #endregion


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

                var taskOne = await Tasks.UpdateContentViewCount.RunAsync(telemetryData, _applicationStorageAccount);
                var taskTwo = await Tasks.UpdateContentViewApplicationReports.RunAsync(telemetryData, _applicationStorageAccount, temporalState.TemporalTableSegment);
                var taskThree = await Tasks.UpdateContentViewDataLakeReports.RunAsync(telemetryData, _dataLakeStorageSharedKey, temporalState);

                #endregion

                // TODO: Add timers, logging, etc...

                // TODO: Add rollbacks if some tasks fail

                if(taskOne && taskTwo && taskThree)
                {
                    // Update last run
                    var logged = await Logging.TemporalStateManager.UpdateLastTemporalStateAsync(temporalState);
                }

                await Task.Delay(_settings.Application.Frequency, stoppingToken);
            }
        }
    }
}
