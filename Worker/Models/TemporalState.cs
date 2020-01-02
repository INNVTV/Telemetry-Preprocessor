using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models
{
    public class TemporalState
    {
        public TemporalState(string year, string month, string day, string hour, string minute)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;

            TemporalTableSegment = Year + Month + Day + Hour;
            TablePartition = Minute;
        }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string TemporalTableSegment { get; set; }
        public string TablePartition { get; set; }

    }
}
