using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Models.QueueMessages;
using Shared.Persistence.Cosmos.MainApplication;
using Shared.Persistence.Storage.Preprocessor;

namespace UpdateMainRecordTask
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMainApplicationCosmosContext _mainApplicationCosmosContext;
        private readonly IPreprocessorStorageContext _preprocessorStorageContext;

        // For exponential back-off
        private int minInterval;
        private int interval;
        private int exponent;
        private int maxInterval;

        public Worker(ILogger<Worker> logger, IMainApplicationCosmosContext mainApplicationCosmosContext, IPreprocessorStorageContext preprocessorStorageContext)
        {
            _logger = logger;
            _mainApplicationCosmosContext = mainApplicationCosmosContext;
            _preprocessorStorageContext = preprocessorStorageContext;

            minInterval = 1;
            interval = minInterval;
            exponent = 3;
            maxInterval = 90;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={_preprocessorStorageContext.Settings.Name};AccountKey={_preprocessorStorageContext.Settings.Key}";
            var queueName = Shared.Constants.QueueNames.MainRecordTask;

            QueueClient queue = new QueueClient(connectionString, queueName);
            queue.Create();

            while (!stoppingToken.IsCancellationRequested)
            {
                var rawMessages = await queue.ReceiveMessagesAsync(maxMessages: 1);
                var rawMessage = rawMessages.Value.FirstOrDefault();

                ContentViewsQueueMessage message = null;

                if (rawMessage != null)
                {
                    #region Attempt to parse raw message

                    try
                    {
                        message = new ContentViewsQueueMessage(rawMessage.MessageText);
                    }
                    catch
                    {
                        // Message data is corrupt. Deque, log issue and skip.
                        await queue.DeleteMessageAsync(rawMessage.MessageId, rawMessage.PopReceipt);
                    }

                    #endregion
                }

                if (message != null)
                {
                    _logger.LogInformation($"Processing message: {rawMessage.MessageText}");

                    // Run Task(s)
                    var result = await Tasks.UpdateMainApplicationCosmosRecord.RunAsync(_mainApplicationCosmosContext, message);

                    // Delete message and reset interval
                    await queue.DeleteMessageAsync(rawMessage.MessageId, rawMessage.PopReceipt);
                    _logger.LogInformation($"Message processed and dequeued");

                    interval = minInterval;
                    _logger.LogInformation($"Interval reset to {interval}. Checking queue for new messages...");
                }
                else
                {
                    _logger.LogInformation($"Queue empty. Sleeping for {interval} seconds");
                    await Task.Delay((interval * 1000), stoppingToken);

                    interval = Math.Min(maxInterval, interval * exponent);
                }
            }
        }
    }
}
