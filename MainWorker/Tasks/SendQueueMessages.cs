using Azure.Storage.Queues;
using Azure.Storage;
using Shared.Models.QueueMessages;
using Shared.Persistence.Storage.Preprocessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    public class SendQueueMessages
    {
        public async static Task<bool> RunAsync(IPreprocessorStorageContext preprocessorStorageContext, List<ContentViewsQueueMessage> queueMessages)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={preprocessorStorageContext.Settings.Name};AccountKey={preprocessorStorageContext.Settings.Key}";

            // Get a reference to and create each of the queues we will be sending messages to

            // Task Worker 1
            QueueClient mainRecordTask = new QueueClient(connectionString, Shared.Constants.QueueNames.MainRecordTask);
            mainRecordTask.Create();

            // Task Worker 2
            QueueClient countTableTask = new QueueClient(connectionString, Shared.Constants.QueueNames.ViewCountTableTask);
            countTableTask.Create();

            // Task Worker 3
            QueueClient reportsTableTask = new QueueClient(connectionString, Shared.Constants.QueueNames.ReportsTableTask);
            reportsTableTask.Create();

            // Task Worker 4
            QueueClient dataLakeTask = new QueueClient(connectionString, Shared.Constants.QueueNames.DataLakeTask);
            dataLakeTask.Create();

            foreach (var message in queueMessages)
            {
                mainRecordTask.SendMessage(message.ToQueueMessageString());
                countTableTask.SendMessage(message.ToQueueMessageString());
                reportsTableTask.SendMessage(message.ToQueueMessageString());
                dataLakeTask.SendMessage(message.ToQueueMessageString());
            }

            return true;
        }
    }
}
