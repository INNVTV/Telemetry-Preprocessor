using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Configuration
{
    public class Settings
    {
        public Settings(IConfiguration configuration)
        {
            // New up our root classes
            Application = new ApplicationSettings();
            Hosting = new HostingConfiguration();

            // Map appsettings.json
            Application.Name = configuration.GetSection("Application").GetSection("Name").Value;
            Application.Namespace = configuration.GetSection("Application").GetSection("Namespace").Value;
            Application.TemporalSourceTable = configuration.GetSection("Application").GetSection("TemporalSourceTable").Value;
            Application.Version = configuration.GetSection("Application").GetSection("Version").Value;
            Application.Frequency = Convert.ToInt32(configuration.GetSection("Application").GetSection("Frequency").Value);

            #region Hosting configuration details (if available)

            try
            {
                // Azure WebApp provides these settings when deployed.
                Hosting.SiteName = configuration["WEBSITE_SITE_NAME"];
                Hosting.InstanceId = configuration["WEBSITE_INSTANCE_ID"];
            }
            catch
            {
            }


            #endregion
        }
        public ApplicationSettings Application { get; set; }
        public HostingConfiguration Hosting { get; set; }

    }

    #region Classes

    #region Application

    public class ApplicationSettings
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string TemporalSourceTable { get; set; }
        public string Version { get; set; }
        public int Frequency { get; set; }
    }


    #endregion

    #region Hosting

    /// <summary>
    /// Only used in Azure WebApp hosted deployments.
    /// Returns info on the WebApp instance for the current process. 
    /// Can be used to log which WebApp instance a process ran on.
    /// </summary>
    public class HostingConfiguration
    {

        public string SiteName { get; set; }
        public string InstanceId { get; set; }
    }

    #endregion

    #endregion
}
