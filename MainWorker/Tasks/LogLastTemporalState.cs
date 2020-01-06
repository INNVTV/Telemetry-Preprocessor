using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models;
using Shared.Persistence.Storage.Preprocessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    public class LogLastTemporalState
    {
        public async static Task<bool> RunAsync(IPreprocessorStorageContext preprocessorStorageContext, TemporalState temporalState, int recordsProcessed, int messagesSent)
        {

            CloudTableClient tableClient = preprocessorStorageContext.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(Shared.Constants.TableNames.MainWorkerLog);

            var mainWorkerLog = new MainWorkerLog(temporalState.TemporalStateId, recordsProcessed, messagesSent);

            TableOperation operation = TableOperation.Insert((mainWorkerLog));

            try
            {
                var result = table.Execute(operation);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
