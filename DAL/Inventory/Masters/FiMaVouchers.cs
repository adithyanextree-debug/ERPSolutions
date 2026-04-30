using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.Inventory.Masters
{
    public class FiMaVouchers
    {
        String ConnectionString;
        public FiMaVouchers(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public string AvailablePurchaseVouchers()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ConnectionString);
            cmd.CommandText = "SELECT ID FROM FiMaVouchers WHERE Alias='PU' and Active='true'";
            cmd.Connection.Open();
            string Purchase = Convert.ToString(cmd.ExecuteScalar());
            return Purchase;
            
        }
        public Object AvailableSalesVouchers()
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ConnectionString);
                cmd.CommandText = "SELECT * FROM FiMaVouchers WHERE Alias='SA' and Active='true'";
                cmd.Connection.Open();
                Object Purchase = cmd.ExecuteScalar();
                return Purchase;
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                cmd.Connection.Close();
            }

            
        }
    }
}
