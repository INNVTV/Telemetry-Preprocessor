using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Queues;

namespace Shared.Models.QueueMessages
{
    public class ContentViewsQueueMessage
    {
        private string deliminator = "^^^^";

        public ContentViewsQueueMessage(string queueMessageString)
        {
            var stringArray = queueMessageString.Split(deliminator);

            Name = stringArray[0];
            AccountId = stringArray[1];
            ContentId = stringArray[2];
            Views = Convert.ToInt32(stringArray[3]);
            Year = stringArray[4];
            Month = stringArray[5];
            Day = stringArray[6];
            Hour = stringArray[7];
            Minute = stringArray[8];           
        }

        public ContentViewsQueueMessage(string name, string accountId, string contentId, int views, string year, string month, string day, string hour, string minute)
        {
            Name = name;
            AccountId = accountId;
            ContentId = contentId;
            Views = views;
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }

        public string Name { get; set; }
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
            return
                $"{Name}{deliminator}" +
                $"{AccountId}{deliminator}" +
                $"{ContentId}{deliminator}" +
                $"{Views}{deliminator}" +
                $"{Year}{deliminator}" +
                $"{Month}{deliminator}" +
                $"{Day}{deliminator}" +
                $"{Hour}{deliminator}" +
                $"{Minute}";
        }

    }
}
