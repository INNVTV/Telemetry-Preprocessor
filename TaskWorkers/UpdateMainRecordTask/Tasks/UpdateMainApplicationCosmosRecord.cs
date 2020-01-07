using Microsoft.Azure.Cosmos;
using Shared.Models.DocumentModels;
using Shared.Models.QueueMessages;
using Shared.Persistence.Cosmos.MainApplication;
using System.Threading.Tasks;

namespace UpdateMainRecordTask.Tasks
{
    class UpdateMainApplicationCosmosRecord
    {
        public async static Task<bool> RunAsync(IMainApplicationCosmosContext mainApplicationCosmosContext, ContentViewsQueueMessage message)
        {
            var partitionKey = Shared.Constants.CosmosPartitions.Content;

            // GET the DOCUMENT MODEL
            MainContentRecordDocument document = null;
            try
            {
                ItemResponse<MainContentRecordDocument> itemResponse =
                    await mainApplicationCosmosContext.Container.ReadItemAsync<MainContentRecordDocument>(
                        message.ContentId, new PartitionKey(partitionKey));

                document = itemResponse.Resource;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                else
                {
                    return false;
                    //throw ex;
                }
            }
            if (document == null)
            {
                return false;
            }

            // Update View Count
            document.Views = document.Views + message.Views;


            // Update the document record
            var result = await
                mainApplicationCosmosContext.Container.ReplaceItemAsync<MainContentRecordDocument>(
                    document,
                    document.Id,
                    new PartitionKey(partitionKey));


            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
