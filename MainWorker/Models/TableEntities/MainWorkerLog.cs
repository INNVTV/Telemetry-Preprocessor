using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainWorker.Models.TableEntities
{
    public class MainWorkerLog : TableEntity
    {

        public MainWorkerLog()
        {

        }

        public MainWorkerLog(string temporalId, int recordsProcessed, int messagesSent)
        {
            RowKey = temporalId;

            // Reverse ticks in order for latest record to always be at the top of our table
            PartitionKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
            
            RecordsProcessed = recordsProcessed;
            MessagesSent = messagesSent;
            DateProcessed = DateTime.UtcNow;
        }

        public string TemporalId
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public int RecordsProcessed { get; set; }
        public int MessagesSent { get; set; }
        public DateTime DateProcessed { get; set; }

    }
}
