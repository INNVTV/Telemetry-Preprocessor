using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Configuration
{
    public class Settings
    {
        public Settings(IConfiguration configuration)
        {
            // New up our root classes
            Application = new ApplicationSettings();

            // Map appsettings.json
            Application.Version = configuration.GetSection("Application").GetSection("Version").Value;

        }
        public ApplicationSettings Application { get; set; }

    }

    #region Classes

    #region Application

    public class ApplicationSettings
    {
        public string Version { get; set; }
    }

    #endregion


    #endregion
}
