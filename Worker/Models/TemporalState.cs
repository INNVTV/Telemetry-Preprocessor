using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models
{
    public class TemporalState
    {
        public TemporalState(string year, string month, string day, string minute)
        {
            Year = year;
            Month = month;
            Day = day;
            Minute = minute;

            Table = Year + Month + Day;
            Partition = Minute;
        }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Minute { get; set; }
        public string Table { get; set; }
        public string Partition { get; set; }

    }
}
