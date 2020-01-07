using Microsoft.Azure.Cosmos;
using Shared.Models.DocumentModels;
using Shared.Models.QueueMessages;
using Shared.Persistence.Cosmos.MainApplication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    /// <summary>
    /// This task checks the ContentId against main database to ensure it exists.
    /// It removes any Ids that cannot be verified.
    /// We also extract the name of the object for downstream task workers to have without the need for a lookup.
    /// </summary>
    public class VerifyCleanExtract
    {
        public async static Task<List<ContentViewsQueueMessage>> RunAsync(IMainApplicationCosmosContext mainApplicationCosmosContext, List<ContentViewsQueueMessage> queueMessages)
        {
            var partitionKey = Shared.Constants.CosmosPartitions.Content;
            List<string> toRemove = new List<string>();

            foreach (var queueMessage in queueMessages)
            {
                // Attempt to get the document model for this id
                MainContentRecordDocument document = null;
                try
                {
                    ItemResponse<MainContentRecordDocument> itemResponse =
                        await mainApplicationCosmosContext.Container.ReadItemAsync<MainContentRecordDocument>(
                            queueMessage.ContentId, new PartitionKey(partitionKey));

                    document = itemResponse.Resource;
                }
                catch (CosmosException ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        document = null;
                    }
                    else
                    {
                        throw ex;
                    }
                }

                if (document == null)
                {
                    // Add this id to list of items for removal
                    toRemove.Add(queueMessage.ContentId);
                }
                else
                {
                    // Extract the name from our main record so it is available to downstream worker tasks:
                    queueMessage.Name = document.Name;
                }
            }


            // Clean any items marked for removal
            queueMessages.RemoveAll(x => toRemove.Contains(x.ContentId));

            return queueMessages;
        }
    }
}
