using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
//using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Shared.Persistence.SqlServer.Helpers
{
    internal static class RetryPolicies
    {
        /*
        public static RetryPolicy GetRetryPolicy()
        {
            //Set retry policy
            var retryPolicy = new RetryPolicy<DefaultRetryStrategy>(5, new TimeSpan(0, 0, 3));

            var retryInterval = TimeSpan.FromSeconds(3);
            var strategy = new FixedInterval("fixed", 4, retryInterval);
            var strategies = new List<RetryStrategy> { strategy };
            var manager = new RetryManager(strategies, "fixed");
            RetryManager.SetDefault(manager);

            return retryPolicy;
        }

        internal class DefaultRetryStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                if (ex != null && ex is SqlException)
                {
                    foreach (SqlError error in (ex as SqlException).Errors)
                    {
                        switch (error.Number)
                        {
                            case 1205:
                                //System.Diagnostics.Debug.WriteLine("SQL Error: Deadlock condition. Retrying...");
                                return true;

                            case -2:
                                //System.Diagnostics.Debug.WriteLine("SQL Error: Timeout expired. Retrying...");
                                return true;
                        }
                    }
                }
                // For all others, do not retry.
                return false;
            }
        }*/
    }
}
