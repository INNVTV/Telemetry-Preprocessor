using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Logging
{
    public static class TemporalState
    {
        public static async Task<string> GetLastTemporalStateAsync()
        {
            return "";
        }

        public static async Task<string> GetNextTemporalStateAsync()
        {
            return "";
        }

        public static async Task<bool> UpdateLastTemporalStateAsync()
        {
            return true;
        }
    }
}
