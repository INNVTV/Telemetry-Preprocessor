## Example of using within a command or query

    using (var connection = new SqlConnection(_sqlContext.ConnectionString))
    using (var command = new SqlCommand())
    {
        //command.CommandType = System.Data.CommandType.StoredProcedure;
        //command.CommandText = "GetInventoryByBeaconId";
        //command.Parameters.Add(new System.Data.SqlClient.SqlParameter("BeaconId", request.BeaconId));
    
        var sqlStatement = new StringBuilder();
        sqlStatement.Append("SELECT * FROM Inventory ");
        sqlStatement.Append("WHERE BeaconId='");
        sqlStatement.Append(request.BeaconId);
        sqlStatement.Append("'");
    
        command.CommandText = sqlStatement.ToString();
        command.Connection = connection;
        command.Connection.Open();
    
        var reader = command.ExecuteReader();
    
        while(reader.Read())
        {
            inventory = new Domain.Entities.Inventory();
    
            inventory.UnitNumber =          (String)reader["UnitNumber"];
            inventory.SerialNumber =        (String)reader["SerialNumber"];
            inventory.MachineModel =        (String)reader["MachineModel"];
            inventory.Manufacturer =        (String)reader["Manufacturer"];
            inventory.BeaconId =            (String)reader["BeaconId"];
            inventory.Description =         (String)reader["Description"];
            inventory.Branch =              (String)reader["Branch"];
            inventory.ActualReceiptDate =   (DateTime)reader["ActualReceiptDate"];
            inventory.DateEntered =         (DateTime)reader["DateEntered"];
            inventory.DateModified =        (DateTime)reader["DateModified"];
        }
    
        //command.Connection.Close();
    
    }
