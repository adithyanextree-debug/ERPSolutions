using ERPSample.Models.Inventory.Reports;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Reports
{
    public class MonthlySalesSummaryDAL
    {
        String ConnectionString;
        public MonthlySalesSummaryDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public (DataTable data, MonthlySalesSummaryModel summary) GetData(MonthlySalesSummaryModel filter)
        {
            var dt = new DataTable();

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("MonthlyInventorySummarySP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required params - always pass
            cmd.Parameters.AddWithValue("@BranchID", filter.BranchID);
            cmd.Parameters.AddWithValue("@DateFrom", filter.FromDate);
            cmd.Parameters.AddWithValue("@DateUpto", filter.ToDate);
            conn.Open();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return (dt, filter);
        }
    }
}
