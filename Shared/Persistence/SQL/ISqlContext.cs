﻿using Shared.Persistence.SqlServer.Models;
//using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.SqlServer
{
    public interface ISqlContext
    {
        //DocumentClient Client { get; set; }
        SqlSettings Settings { get; set; }

        string ConnectionString { get; set; }

        //ReliableSqlConnection SqlConnection { get; set; }
    }


}
