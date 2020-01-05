using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainWorker.Models.TableEntities
{
    public class MainWorkerLog : TableEntity
    {
        MainWorkerLog(string temporalId)
        {
            PartitionKey = temporalId;
            RowKey = string.Format("{0:d19}+{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"));

            RecordsProcessed = 0;
            DateProcessed = DateTime.UtcNow;
        }

        public string TemporalId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public int RecordsProcessed { get; set; }
        public DateTime DateProcessed { get; set; }

    }
}
