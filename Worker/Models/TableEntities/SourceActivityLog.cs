using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.TableEntities
{
    public class SourceActivityLog : TableEntity
    {
        public string ContentId { get; set; }
        public string Activity { get; set; }
        public string Value { get; set; }
        public string UserId { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string Second { get; set; }

    }
}
