using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.TableEntities
{
    public class DestinationViewCount : TableEntity
    {
        public string AccountId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public string ContentId
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public int ViewCount { get; set; }

    }
}
