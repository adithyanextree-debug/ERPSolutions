using ERPSample.Models.Inventory.Reports.Purchase;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Reports.Purchase
{
    // DAL/PurchaseRegisterDAL.cs
    public class PurchaseRegisterDAL
    {
        String ConnectionString;
        public PurchaseRegisterDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public (DataTable data, PurchaseRegisterModel summary) GetData(PurchaseRegisterModel filter)
        {
            var dt = new DataTable();

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("InventoryRegisterSP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required params - always pass
            cmd.Parameters.AddWithValue("@BranchID", filter.BranchID);
            cmd.Parameters.AddWithValue("@DateFrom", filter.FromDate);
            cmd.Parameters.AddWithValue("@DateUpto", filter.ToDate);
            cmd.Parameters.AddWithValue("@BasicVTypeID", filter.VTypeID);
            cmd.Parameters.AddWithValue("@Detailed", filter.IsDetailed);
            cmd.Parameters.AddWithValue("@Inventory", filter.IsInventory);
            cmd.Parameters.AddWithValue("@Columnar", filter.IsColumnar);
            cmd.Parameters.AddWithValue("@IsGroupItemReport", filter.IsGroupItem);

            // Optional - VTypeID (only if not -1 and not empty)
            if (filter.VTypeID != null &&
                filter.VTypeID.ToString() != "-1" &&
                filter.VTypeID.ToString() != "")
            {
                cmd.Parameters.AddWithValue("@VTypeID", filter.VTypeID);
            }

            // Optional - Criteria
            if (filter.Criteria == true)
            {
                cmd.Parameters.AddWithValue("@Criteria", "extract");
            }

            // Optional - AccountID (Party)
            if (filter.AccountID != null && filter.AccountID != 0)
            {
                cmd.Parameters.AddWithValue("@AccountID", filter.AccountID);
            }

            // Optional - PaymentTypeID
            if (!string.IsNullOrEmpty(filter.PaymentTypeID))
            {
                cmd.Parameters.AddWithValue("@PaymentTypeID", filter.PaymentTypeID);
            }

            // Optional - ItemID
            if (filter.ItemID != null && filter.ItemID != 0)
            {
                cmd.Parameters.AddWithValue("@ItemID", filter.ItemID);
            }

            // Optional - CounterID
            if (filter.CounterID != null && filter.CounterID != 0)
            {
                cmd.Parameters.AddWithValue("@CounterID", filter.CounterID);
            }

            conn.Open();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            // Summary calculations - use decimal? to handle DBNull
            //filter.TotalDebit = dt.AsEnumerable().Sum(r => r.Field<decimal?>("Debit") ?? 0);
            //filter.TotalCredit = dt.AsEnumerable().Sum(r => r.Field<decimal?>("Credit") ?? 0);
            //filter.CashDebit = dt.AsEnumerable()
            //                       .Where(r => r.Field<string>("VType") == "Cash")
            //                       .Sum(r => r.Field<decimal?>("Debit") ?? 0);
            //filter.CashCredit = dt.AsEnumerable()
            //                       .Where(r => r.Field<string>("VType") == "Cash")
            //                       .Sum(r => r.Field<decimal?>("Credit") ?? 0);
            //filter.Taxable = dt.AsEnumerable().Sum(r => r.Field<decimal?>("Taxable") ?? 0);
            //filter.NonTaxable = dt.AsEnumerable().Sum(r => r.Field<decimal?>("NonTaxable") ?? 0);

            return (dt, filter);
        }
    }
}
