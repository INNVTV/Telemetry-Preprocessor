using Worker.Models.Persistence.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence.SqlServer.Helpers
{
    public class SqlConnectionStrings
    {
        public static string GenerateConnectionString(SqlSettings sqlSettings)
        {
            StringBuilder connectionString = new StringBuilder();

            //SQL Linux/Docker Connection String: --------------

            connectionString.Append("Server=");
            connectionString.Append(sqlSettings.Server);

            connectionString.Append(";Database=");
            connectionString.Append(sqlSettings.Database);

            connectionString.Append(";User=");
            connectionString.Append(sqlSettings.Username);

            connectionString.Append(";Password=");
            connectionString.Append(sqlSettings.Password);


            // SQL Azure Connection String: ---------------

            /*
            connectionString.Append("Server=");
            connectionString.Append(sqlSettings.Server);
            connectionString.Append(";Database=");
            connectionString.Append(sqlSettings.Database);
            connectionString.Append(";User Id=");
            connectionString.Append(sqlSettings.Username);
            connectionString.Append(";Password=");
            connectionString.Append(sqlSettings.Password);
            connectionString.Append(";MultipleActiveResultSets=true");
            connectionString.Append(";Trusted_Connection=False;Encrypt=True;Persist Security Info=True;"); //<-- Adding Persist Security Info=True; resolved SQL connectivity errors when making multiple calls
            connectionString.Append("Connection Timeout=45;"); //<-- Tripled the default timeout
            */

            return connectionString.ToString();
        }
    }

}
