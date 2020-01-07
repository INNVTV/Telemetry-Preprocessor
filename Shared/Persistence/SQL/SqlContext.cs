using Shared.Persistence.SqlServer.Models;
using Microsoft.Extensions.Configuration;
//using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.SqlServer
{
    public class SqlContext : ISqlContext
    {
        public SqlContext(IConfiguration configuration)
        {
            Settings = new SqlSettings();

            #region Map appsettings.json

            Settings.Server = configuration
                .GetSection("Sql")
                .GetSection("Server").Value;

            Settings.Database = configuration
                .GetSection("Sql")
                .GetSection("Database").Value;

            Settings.Username = configuration
                .GetSection("Sql")
                .GetSection("Username").Value;

            Settings.Password = configuration
                .GetSection("Sql")
                .GetSection("Password").Value;

            #endregion

            #region Generate ConnectionString
            ConnectionString = Helpers.SqlConnectionStrings.GenerateConnectionString(Settings);

            #endregion

            #region Generate sql conection

            //SqlConnection = new ReliableSqlConnection(ConnectionString, Helpers.RetryPolicies.GetRetryPolicy());

            #endregion
        }

        //public DocumentClient Client { get; set; }
        public SqlSettings Settings { get; set; }
        public string ConnectionString { get; set; }

        //public ReliableSqlConnection SqlConnection { get; set;}
    }
}
