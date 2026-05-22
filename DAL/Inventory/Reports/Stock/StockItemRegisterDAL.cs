using ERPSample.Models.Inventory.Reports.Purchase;
using ERPSample.Models.Inventory.Reports.Stock;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Reports.Stock
{
    public class StockItemRegisterDAL
    {

        String ConnectionString;
        public StockItemRegisterDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public (List<Dictionary<string, object>> data, int totalRecords, StockItemRegisterModel summary)
        GetData(StockItemRegisterModel filter, int offset = 0, int pageSize = 50)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("InventoryItemsReportSP", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 300
            };

            cmd.Parameters.AddWithValue("@Criteria", "InventoryItemWiseReport");
            cmd.Parameters.AddWithValue("@BranchID", (object?)filter.BranchID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)filter.Date ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LocationID", (object?)filter.LocationID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CommodityID", (object?)filter.CommodityID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ItemID", (object?)filter.ItemID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsItemwise", (object?)filter.IsItemwise ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Barcode", (object?)filter.Barcode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OriginID", (object?)filter.OriginID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ColorID", (object?)filter.ColorID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BrandID", (object?)filter.BrandID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BatchNo", (object?)filter.BatchNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SupplierID", (object?)filter.SupplierID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerID", (object?)filter.CustomerID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AccountID", (object?)filter.AccountID ?? DBNull.Value);

            conn.Open();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            int totalRecords = dt.Rows.Count;

            // Apply paging in memory (since SP returns all rows)
            var pagedRows = dt.Rows.Cast<DataRow>()
                .Skip(offset)
                .Take(pageSize)
                .Select(row => {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                        dict[col.ColumnName] = row[col] == DBNull.Value ? "" : row[col];
                    return dict;
                })
                .ToList();

            return (pagedRows, totalRecords, filter);
        }
        //public (DataTable data, StockItemRegisterModel summary) GetData(StockItemRegisterModel filter)
        //{
        //    var dt = new DataTable();
        //    using var conn = new SqlConnection(ConnectionString);
        //    using var cmd = new SqlCommand("InventoryItemsReportSP", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure,
        //        CommandTimeout = 300  // increase from default 30 seconds

        //    };

        //    // Add parameters
        //    cmd.Parameters.AddWithValue("@Criteria", "InventoryItemWiseReport");
        //    cmd.Parameters.AddWithValue("@BranchID", (object?)filter.BranchID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@ToDate", (object?)filter.Date ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@LocationID", (object?)filter.LocationID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@CommodityID", (object?)filter.CommodityID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@ItemID", (object?)filter.ItemID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@IsItemwise", (object?)filter.IsItemwise ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@Barcode", (object?)filter.Barcode ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@OriginID", (object?)filter.OriginID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@ColorID", (object?)filter.ColorID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@BrandID", (object?)filter.BrandID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@BatchNo", (object?)filter.BatchNo ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@SupplierID", (object?)filter.SupplierID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@CustomerID", (object?)filter.CustomerID ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@AccountID", (object?)filter.AccountID ?? DBNull.Value);

        //    conn.Open();
        //    using var adapter = new SqlDataAdapter(cmd);
        //    adapter.Fill(dt);

        //    return (dt, filter);
        //}
    }
}
