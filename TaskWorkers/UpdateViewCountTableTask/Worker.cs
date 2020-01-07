using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Models.QueueMessages;
using Shared.Persistence.Storage.Application;
using Shared.Persistence.Storage.Preprocessor;

namespace UpdateViewCountTableTask
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IApplicationStorageContext _applicationStorageContext;
        private readonly IPreprocessorStorageContext _preprocessorStorageContext;

        // For exponential back-off
        private int minInterval;
        private int interval;
        private int exponent;
        private int maxInterval;

        public Worker(ILogger<Worker> logger, IApplicationStorageContext applicationStorageContext, IPreprocessorStorageContext preprocessorStorageContext)
        {
            _logger = logger;
            _applicationStorageContext = applicationStorageContext;
            _preprocessorStorageContext = preprocessorStorageContext;

            minInterval = 1;
            interval = minInterval;
            exponent = 3;
            maxInterval = 90;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={_preprocessorStorageContext.Settings.Name};AccountKey={_preprocessorStorageContext.Settings.Key}";
            var queueName = Shared.Constants.QueueNames.ViewCountTableTask;

            QueueClient queue = new QueueClient(connectionString, queueName);

            while (!stoppingToken.IsCancellationRequested)
            {
                var rawMessages = await queue.ReceiveMessagesAsync(maxMessages: 1);
                var rawMessage = rawMessages.Value.FirstOrDefault();

                ContentViewsQueueMessage message = null;

                try
                {
                    message = new ContentViewsQueueMessage(rawMessage.MessageText);
                }
                catch
                {
                    // Message data is corrupt. Deque, log issue and skip.
                    await queue.DeleteMessageAsync(rawMessage.MessageId, rawMessage.PopReceipt);
                }

                if (message != null)
                {
                    // Run Task(s)
                    var result = await Tasks.UpdateViewCountsTable.RunAsync(_applicationStorageContext, message);

                    // Delete message and reset interval
                    await queue.DeleteMessageAsync(rawMessage.MessageId, rawMessage.PopReceipt);
                    interval = minInterval;
                    _logger.LogInformation($"Interval reset to {interval}");
                }
                else
                {
                    _logger.LogInformation($"Sleep for {interval} seconds");
                    await Task.Delay((interval * 1000), stoppingToken);

                    interval = Math.Min(maxInterval, interval * exponent);
                }
            }
        }
    }
}
