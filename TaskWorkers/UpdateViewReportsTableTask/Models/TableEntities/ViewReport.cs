using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainWorker.Models.TableEntities
{
    public class ViewReport : TableEntity
    {
        public string ContentId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public string Hour
        {
            get { return RowKey; }
            set { RowKey = value; }
        }
        public int ViewCount { get; set; }
    }
}
