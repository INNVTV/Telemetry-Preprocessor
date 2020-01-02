using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Worker.Models.Persistence.DocumentDatabase;
using Worker.Models.TableEntities;

namespace Worker.Tasks
{
    public static class CleanAndUpdateContentViewCount
    {
        public static async Task<bool> RunAsync(List<SourceActivityLog> telemetryData, IDocumentContext documentContext)
        {
           

            return true;
        }

    }
}
