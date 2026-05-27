using ERPSample.Models.Inventory.Reports;
using ERPSample.Models.Inventory.Reports.Purchase;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Reports
{
    public class InventoryProfitDAL
    {

        String ConnectionString;
        public InventoryProfitDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public (DataTable data, InventoryProfitModel summary) GetData(InventoryProfitModel filter)
        {
            var dt = new DataTable();

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("InventoryProfitSP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required params - always pass
            cmd.Parameters.AddWithValue("@BranchID", filter.BranchID);
            cmd.Parameters.AddWithValue("@DateFrom", filter.FromDate);
            cmd.Parameters.AddWithValue("@DateUpto", filter.ToDate);
            cmd.Parameters.AddWithValue("@Detailed", filter.IsDetailed);
            // Optional - Criteria  
            cmd.Parameters.AddWithValue("@Criteria", filter.Criteria);
            // Optional - AccountID (Party)
            if (filter.AccountID != null && filter.AccountID != 0)
            {
                cmd.Parameters.AddWithValue("@AccountID", filter.AccountID);
            }
            // Optional - ItemID
            if (filter.ItemID != null && filter.ItemID != 0)
            {
                cmd.Parameters.AddWithValue("@ItemID", filter.ItemID);
            }

            conn.Open();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return (dt, filter);
        }
    }
}
