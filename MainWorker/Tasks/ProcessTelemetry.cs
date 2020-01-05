using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    public static class ProcessTelemetry
    {
        public async static Task<bool> RunAsync(TemporalState temporalState)
        {
            return true;
        }
   }
}
