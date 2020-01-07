using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models.QueueMessages;
using Shared.Persistence.Storage.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UpdateViewReportsTableTask.Tasks
{
    class UpdateViewReportsTable
    {
        public async static Task<bool> RunAsync(IApplicationStorageContext applicationStorageContext, ContentViewsQueueMessage message)
        {

            CloudTableClient tableClient = applicationStorageContext.StorageAccount.CreateCloudTableClient();

            // Append YYYYMMDD to the table name
            var tablename = Shared.Constants.TableNames.ContentViewHourlyReports + message.Year + message.Month + message.Day;
            CloudTable table = tableClient.GetTableReference(tablename);
            
            // Create the table if it does not exist
            await table.CreateIfNotExistsAsync();

            // query to see if content has a view report record on this day
            TableOperation retreive = TableOperation.Retrieve<ViewReport>(message.ContentId, message.Hour);
            TableResult tableResult = await table.ExecuteAsync(retreive);

            var viewReportRecord = (ViewReport)tableResult.Result;

            if (viewReportRecord == null)
            {
                // This is the first view count coming in for this content item. Create a record and load initial views.
                viewReportRecord = new ViewReport();
                viewReportRecord.ContentId = message.ContentId;
                viewReportRecord.Hour = message.Hour;
                viewReportRecord.ViewCount = message.Views;

                TableOperation insert = TableOperation.Insert(viewReportRecord);
                var insertResult = await table.ExecuteAsync(insert);
            }
            else
            {
                // Append to previous view count
                viewReportRecord.ViewCount = viewReportRecord.ViewCount + message.Views;

                TableOperation update = TableOperation.Replace(viewReportRecord);
                var updateResult = await table.ExecuteAsync(update);

            }

            return true;

        }
    }
}
