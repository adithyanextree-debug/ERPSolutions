using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.Inventory.Transactions
{
    public class Purchase
    {
        String ConnectionString;
        public Purchase(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public DataTable Fill()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT FiTransactions.ID, FiTransactions.Date,FiTransactions.TransactionNo,FiTransactions.AddedDate,FiTransactions.AccountID,Parties.Name FROM FiTransactions Join Parties ON Parties.AccountID = FiTransactions.AccountID WHERE FiTransactions.PageID = 15", connection);
            DataTable dt = new DataTable();
            new SqlDataAdapter(cmd).Fill(dt);
            return new DataTable();
        }
    }
}
