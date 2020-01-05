using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class Transformations
    {
        public static DateTime ConvertTemporalStateToDateTime(TemporalState temporalState)
        {
            return new DateTime(
                Convert.ToInt32(temporalState.Year),
                Convert.ToInt32(temporalState.Month),
                Convert.ToInt32(temporalState.Day),
                Convert.ToInt32(temporalState.Hour),
                Convert.ToInt32(temporalState.Minute),
                59);
        }
    }
}
