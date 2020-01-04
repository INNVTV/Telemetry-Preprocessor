using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
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

            TelemetryTableSegment = Year + Month + Day + Hour;
            TelemetryTablePartition = Minute;

            TemporalStateId = TelemetryTableSegment + TelemetryTablePartition;
        }

        public TemporalState(DateTime dateTime)
        {
            Year = dateTime.Year.ToString("d4");

            Month = dateTime.Month.ToString("d2");

            Day = dateTime.Day.ToString("d2");

            Hour = dateTime.ToString("HH");

            Minute = dateTime.Minute.ToString("d2");

            TelemetryTableSegment = Year + Month + Day + Hour;
            TelemetryTablePartition = Minute;

            TemporalStateId = TelemetryTableSegment + TelemetryTablePartition;
        }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }

        // The table segment name used to determine which telemetry table holds the hours worth of data to process
        public string TelemetryTableSegment { get; set; }
        
        // The partition id of the telemetry table to pull from 
        public string TelemetryTablePartition { get; set; }

        // Used for logging temporal state run by preprocessor
        public string TemporalStateId { get; set; }

    }
}
