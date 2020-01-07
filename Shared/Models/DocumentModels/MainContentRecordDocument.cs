using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.DocumentModels
{
    public class MainContentRecordDocument
    {
        public MainContentRecordDocument(string name)
        {
            // Set our document partitioning property
            DocumentType = Shared.Constants.CosmosPartitions.Content;

            //Create our Id
            Id = Guid.NewGuid().ToString();
            Name = name;
        }

        [JsonProperty(PropertyName = "id")] //<-- Required for all Documents
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_docType")]
        public string DocumentType { get; internal set; } //<-- Our paritioning property

        public string Name { get; set; }
        public int Views { get; set; }
    }
}
