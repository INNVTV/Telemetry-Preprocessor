using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            exponent = 2;
            maxInterval = 90;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //var message = queue.GetMessage(); if (message != null)
                //{


                    //interval = minInterval;
                    //_logger.LogInformation($"Interval reset to {interval}");
                //}
                //else
                //{
                    _logger.LogInformation($"Sleep for {interval} seconds");
                    await Task.Delay((interval * 1000), stoppingToken);

                    interval = Math.Min(maxInterval, interval * exponent);

                //}
            }
        }
    }
}
