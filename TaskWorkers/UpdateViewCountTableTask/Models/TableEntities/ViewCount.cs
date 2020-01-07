using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainWorker.Models.TableEntities
{
    public class ViewCount : TableEntity
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

        public int Views { get; set; }

    }
}
