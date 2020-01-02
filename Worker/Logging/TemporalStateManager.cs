using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Worker.Models;

namespace Worker.Logging
{
    public static class TemporalStateManager
    {
        public static async Task<TemporalState> GetLastTemporalStateAsync()
        {
            return new TemporalState("2019", "12", "30", "02", "12");
        }

        public static async Task<TemporalState> GetNextTemporalStateAsync()
        {
            return new TemporalState("2019", "12", "30", "02", "12");
        }

        public static async Task<bool> UpdateLastTemporalStateAsync(TemporalState currentState)
        {
            return true;
        }
    }
}
