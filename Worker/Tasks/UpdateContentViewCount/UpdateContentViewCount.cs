using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worker.Models.Persistence;
using Worker.Models.TableEntities;

namespace Worker.Tasks
{
    public static class UpdateContentViewCount
    {
        public static async Task<bool> RunAsync(List<SourceActivityLog> telemetryData, IApplicationStorageAccount applicationStorageAccount)
        {
            // This task updates the view count for each content id in the telemetry data.
            var tableName = "contentviewcounts";

            // First we group by same content id for this time span
            var groupedContent = telemetryData
            .GroupBy(u => u.ContentId)
            .Select(grp => grp.ToList())
            .ToList();
            
            foreach(var contentViewList in groupedContent)
            {
                // We get the count of each list and append to the view count for each content
                string accountId = contentViewList[0].AccountId;
                string contentId = contentViewList[0].ContentId;
                int newViews = contentViewList.Count;

                #region Connect to Views table and append count to contentid partition

                var dataLakeCloudStorageAccount = CloudStorageAccount.Parse(
                string.Concat(
                    "DefaultEndpointsProtocol=https;AccountName=",
                    applicationStorageAccount.Settings.Name,
                    ";AccountKey=",
                    applicationStorageAccount.Settings.Key)
                );

                CloudTableClient tableClient = dataLakeCloudStorageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference(tableName);

                // Create the table if it does not exist
                await table.CreateIfNotExistsAsync();

                // query to see if content has a view count record
                //TableQuery<DestinationViewCount> query = new TableQuery<DestinationViewCount>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, accountId));
                //var results = await table.ExecuteQuerySegmentedAsync<DestinationViewCount>(query, null);
                //var contentRecord = results.FirstOrDefault();

                TableOperation retreive = TableOperation.Retrieve<DestinationViewCount>(accountId, contentId);
                TableResult tableResult = await table.ExecuteAsync(retreive);

                var contentRecord = (DestinationViewCount)tableResult.Result;

                if (contentRecord == null)
                {
                    // This is the first view count coming in for this content item. Create a record and load initial views.
                    contentRecord = new DestinationViewCount();
                    contentRecord.AccountId = accountId;
                    contentRecord.ContentId = contentId;
                    contentRecord.ViewCount = newViews;

                    TableOperation insert = TableOperation.Insert(contentRecord);
                    var insertResult = await table.ExecuteAsync(insert);
                }
                else
                {
                    // Append to previous view count
                    contentRecord.ViewCount = contentRecord.ViewCount + newViews;

                    TableOperation update = TableOperation.Replace(contentRecord);
                    var updateResult = await table.ExecuteAsync(update);

                }

                #endregion
            }

            return true;
        }

    }
}
