using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models.QueueMessages;
using Shared.Persistence.Storage.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UpdateViewCountTableTask.Tasks
{
    class UpdateViewCountsTable
    {
        public async static Task<bool> RunAsync(IApplicationStorageContext applicationStorageContext, ContentViewsQueueMessage message)
        {

            CloudTableClient tableClient = applicationStorageContext.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(Shared.Constants.TableNames.ContentViewCounts);

            // Create the table if it does not exist
            await table.CreateIfNotExistsAsync();


            // query to see if content has a view count record

            TableOperation retreive = TableOperation.Retrieve<ViewCount>(message.AccountId, message.ContentId);
            TableResult tableResult = await table.ExecuteAsync(retreive);

            var viewCountRecord = (ViewCount)tableResult.Result;

            if (viewCountRecord == null)
            {
                // This is the first view count coming in for this content item. Create a record and load initial views.
                viewCountRecord = new ViewCount();
                viewCountRecord.AccountId = message.AccountId;
                viewCountRecord.ContentId = message.ContentId;
                viewCountRecord.ViewCount = message.Views;

                TableOperation insert = TableOperation.Insert(viewCountRecord);
                var insertResult = await table.ExecuteAsync(insert);
            }
            else
            {
                // Append to previous view count
                viewCountRecord.ViewCount = viewCountRecord.ViewCount + message.Views;

                TableOperation update = TableOperation.Replace(viewCountRecord);
                var updateResult = await table.ExecuteAsync(update);

            }

            return true;

        }
    }
}
