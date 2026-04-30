using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.General.Masters
{
    public class Parties
    {
        String ConnectionString;
        public Parties(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public string AvailablePurchaseVouchers()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ConnectionString);
            cmd.CommandText = "SELECT ID FROM FiMaVouchers WHERE Alias='PU' and Active='true'";
            cmd.Connection.Open();
            string Purchase = (string)cmd.ExecuteScalar();
            return Purchase;

        }
    }
}
