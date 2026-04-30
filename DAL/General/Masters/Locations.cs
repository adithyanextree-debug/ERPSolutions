using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.General.Masters
{
    public class Locations
    {
        String ConnectionString;
        public Locations(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public Object AvailableLocations()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ConnectionString);
            cmd.CommandText = "SELECT * FROM Locations WHERE  Active='true'";
            cmd.Connection.Open();
            Object Purchase = cmd.ExecuteScalar();
            return Purchase;
        }
    }
}
