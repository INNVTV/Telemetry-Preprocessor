using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.TableEntities
{
    public class DestinationViewReport : TableEntity
    {
        public string ContentId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public string ReportHour
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public int ViewCount { get; set; }

    }
}
