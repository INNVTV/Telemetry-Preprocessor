using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models;
using Shared.Persistence.Storage.Preprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    public static class NextTemporalState
    {
        public async static Task<TemporalState> GetAsync(IPreprocessorStorageContext preprocessorStorageContext)
        {
            TemporalState nextTemporalState = null;

            // Attempt to get next temporal state from main activity log
            CloudTableClient tableClient = preprocessorStorageContext.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(Shared.Constants.TableNames.MainWorkerLog);

            await table.CreateIfNotExistsAsync();

            TableQuery<MainWorkerLog> query = new TableQuery<MainWorkerLog>().Take(1);

            var results = await table.ExecuteQuerySegmentedAsync<MainWorkerLog>(query, null);
            var lastLoggedState = results.ToList().FirstOrDefault();

            if(lastLoggedState == null)
            {
                // If this is first processing create default temporal state (-24 hours from current UTC time) - This will start processing from the last day of logs
                var currentUtcDateTime = DateTime.UtcNow;
                nextTemporalState = new TemporalState(currentUtcDateTime.AddHours(-24));
            }
            else
            {
                // Add 1 minute to previous logged state by converting in /out of date times
                var lastTemporalState = new TemporalState(lastLoggedState.TemporalId);
                DateTime lastRunDateTime = Shared.Transformations.ConvertTemporalStateToDateTime(lastTemporalState);
                DateTime nextRunDateTime = lastRunDateTime.AddMinutes(1);

                // Create our next temporal state to process
                nextTemporalState = new TemporalState(nextRunDateTime);
            }

            return nextTemporalState;
        }
   }
}
