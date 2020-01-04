using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UpdateMainRecordTask
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        // For exponential back-off
        private int minInterval;
        private int interval;
        private int exponent;
        private int maxInterval;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            minInterval = 1;
            interval = minInterval;
            exponent = 3;
            maxInterval = 90;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = "";
            var queueName = "";
            QueueClient queue = new QueueClient(connectionString, queueName);

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await queue.ReceiveMessagesAsync(maxMessages:1);
                var message = messages.Value.FirstOrDefault();
                
                if (message != null)
                {
                    // Run Task(s)

                    // Delete message and reset interval
                    await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
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
