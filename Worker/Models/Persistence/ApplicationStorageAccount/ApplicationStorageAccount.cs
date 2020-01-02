using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence
{
    public class ApplicationStorageAccount : IApplicationStorageAccount
    {
        public ApplicationStorageAccount(IConfiguration configuration)
        {

            Settings = new ApplicationStorageSettings();

            #region Map appsettings.json

            Settings.Name = configuration
                .GetSection("Azure")
                .GetSection("ApplicationStorage")
                .GetSection("Name").Value;

            Settings.Key = configuration
                .GetSection("Azure")
                .GetSection("ApplicationStorage")
                .GetSection("Key").Value;

            #endregion


        }
        public ApplicationStorageSettings Settings { get; set; }
    }
}
