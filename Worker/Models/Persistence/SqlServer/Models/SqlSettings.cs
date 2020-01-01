using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Models.Persistence.SqlServer.Models
{
    public class SqlSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
