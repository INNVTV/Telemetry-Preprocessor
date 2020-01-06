using MainWorker.Models.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models;
using Shared.Persistence.Storage.Preprocessor;
using Shared.Persistence.Storage.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWorker.Tasks
{
    public static class ProcessTelemetry
    {
        public async static Task<bool> RunAsync()
        {
            return true;
        }
    }
}
