using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Queues;

namespace Shared.Models.QueueMessages
{
    public class ContentViewsQueueMessage
    {
        public ContentViewsQueueMessage(string queueMessageString)
        {
            var stringArray = queueMessageString.Split(",");

            AccountId = stringArray[0];
            ContentId = stringArray[1];
            Views = Convert.ToInt32(stringArray[2]);
            Year = stringArray[3];
            Month = stringArray[4];
            Day = stringArray[5];
            Hour = stringArray[6];
            Minute = stringArray[7];           
        }

        public ContentViewsQueueMessage(string accountId, string contentId, int views, string year, string month, string day, string hour, string minute)
        {
            AccountId = accountId;
            ContentId = contentId;
            Views = views;
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        public string AccountId { get; set; }
        public string ContentId { get; set; }
        public int Views { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }

        public string ToQueueMessageString()
        {
            return $"{AccountId},{ContentId},{Views},{Year},{Month},{Day},{Hour},{Minute}";
        }

    }
}
