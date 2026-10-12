using ERPSample.Models.Inventory.Reports;
using ERPSample.Models.Inventory.Reports.Purchase;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Reports
{
    public class eReturnsDAL
    {
        String ConnectionString;
        public eReturnsDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public (DataTable data, eReturnsModel summary) GetData(eReturnsModel filter)
        {
            var dt = new DataTable();

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("eReturnsSP", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required params - always pass
            cmd.Parameters.AddWithValue("@BranchID", filter.BranchID);
            cmd.Parameters.AddWithValue("@DateFrom", filter.DateFrom);
            cmd.Parameters.AddWithValue("@DateUpto", filter.DateTo);
            // Optional - AccountID (Party)
            if (filter.AccountID != null && filter.AccountID != 0)
            {
                cmd.Parameters.AddWithValue("@AccountID", filter.AccountID);
            }

            // Optional - PaymentTypeID
            if (!string.IsNullOrEmpty(filter.Emirate))
            {
                cmd.Parameters.AddWithValue("@Emirate", filter.Emirate);
            }
            if (!string.IsNullOrEmpty(filter.TaxRegNo))
            {
                cmd.Parameters.AddWithValue("@TaxRegNo", filter.TaxRegNo);
            }
            if (filter.InvoiceAmountIP != null && filter.InvoiceAmountIP != 0)
            {
                cmd.Parameters.AddWithValue("@InvoiceAmountIP", filter.InvoiceAmountIP);
            }
            if (filter.InvoiceAmountOP != null && filter.InvoiceAmountOP != 0)
            {
                cmd.Parameters.AddWithValue("@InvoiceAmountOP", filter.InvoiceAmountOP);
            }

            conn.Open();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return (dt, filter);
        }
    }
}
