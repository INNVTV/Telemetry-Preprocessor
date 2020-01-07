using Shared.Models.QueueMessages;
using Shared.Persistence.Cosmos.MainApplication;
using System.Threading.Tasks;

namespace UpdateMainRecordTask.Tasks
{
    class UpdateMainApplicationCosmosRecord
    {
        public async static Task<bool> RunAsync(IMainApplicationCosmosContext mainApplicationCosmosContext, ContentViewsQueueMessage message)
        {
            return true;
        }
    }
}
