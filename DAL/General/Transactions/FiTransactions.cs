using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.General.Transactions
{
    public class FiTransactions
    {
        String ConnectionString;
        public FiTransactions(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public int LastID(int Page)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ConnectionString);
            cmd.CommandText = "SELECT TOP 1 ID FROM FiTransactions WHERE PageID=@PageID  ORDER By ID DESC";
            cmd.Parameters.AddWithValue("@PageID", Page);
            cmd.Connection.Open();
            int Purchase = (int)cmd.ExecuteScalar();
            return Purchase;
            
        }
    }
}
