using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models;
using Shared.Persistence.Storage.Preprocessor;
using Shared.Persistence.Storage.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    class GetTelemetryData
    {
        public async static Task<GetTelemetryDataResponse> RunAsync(TemporalState temporalState, ITelemetryStorageContext telemetryStorageContext)
        {
            var response = new GetTelemetryDataResponse();

            #region Notes
            /*
             In this example we query to fiter out only telemetry that has a 'ContentId'

             In some scenarios you may want to partition your temetry tables to focus on one entity type such as:
                'platform', 'account', 'user', 'content', etc...

            You may aso have multiple preprocessors focusing on different aspects of each entity type.
            In this exampe the focus is purely on Content Views

            */
            #endregion

            try
            {
                var tableName = Shared.Constants.TableNames.TelemetryBase + temporalState.TelemetryTableSegment;

                CloudTableClient tableClient = telemetryStorageContext.StorageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference(tableName);

                TableQuery<SourceTelemetryLog> query = new TableQuery<SourceTelemetryLog>()
                    .Where(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, temporalState.TelemetryTablePartition),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("Activity", QueryComparisons.Equal, "View") //<-- we only want records that have an activity type of "View"
                    )).Take(1000);

                var results = await table.ExecuteQuerySegmentedAsync<SourceTelemetryLog>(query, null);

                response.TelemetryData = results.ToList();

                // Iterate until we get all results
                while (results.ContinuationToken != null)
                {
                    results = await table.ExecuteQuerySegmentedAsync<SourceTelemetryLog>(query, results.ContinuationToken);
                    response.TelemetryData.AddRange(results);
                }

                response.IsSuccess = true;
            }
            catch(Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
            }
            
            return response;
        }
    }

    class GetTelemetryDataResponse
    {
        public GetTelemetryDataResponse()
        {
            TelemetryData = new List<SourceTelemetryLog>();
            IsSuccess = false;
            ErrorMessage = String.Empty;
        }
        public List<SourceTelemetryLog> TelemetryData { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; } 
    }
}
