using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models;
using Shared.Models.QueueMessages;
using Shared.Persistence.Storage.Preprocessor;
using Shared.Persistence.Storage.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    public static class ProcessTelemetry
    {
        public async static Task<List<ContentViewsQueueMessage>> RunAsync(List<SourceTelemetryLog> telemetryData, TemporalState temporalState)
        {
            var queueMessages = new List<ContentViewsQueueMessage>();

            // First we group by same content id for this time span
            var groupedContent = telemetryData
            .GroupBy(u => u.ContentId)
            .Select(grp => grp.ToList())
            .ToList();

            foreach (var contentViewList in groupedContent)
            {
                string accountId = contentViewList[0].AccountId;
                string contentId = contentViewList[0].ContentId;
                int newViews = contentViewList.Count; //<-- The count from each list represents total view counts for this content item in the current minute being processed

                // Note: Consider veriying that the contentId is valid before sending out message queues.

                // Generate a new message and add it to the list to be sent to task workers
                queueMessages.Add(new ContentViewsQueueMessage(
                    accountId,
                    contentId,
                    newViews,
                    temporalState.Year,
                    temporalState.Month,
                    temporalState.Day,
                    temporalState.Hour,
                    temporalState.Minute));
            }

            return queueMessages;
        }
    }
}
