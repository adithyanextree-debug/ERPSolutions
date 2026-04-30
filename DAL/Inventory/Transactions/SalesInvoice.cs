using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.Inventory.Transactions
{
    public class SalesInvoice
    {
        String ConnectionString;
        public SalesInvoice(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public DataSet Fill(Int64 ID)
        {
            SqlDataAdapter General = new SqlDataAdapter("SELECT ItemCode,ItemName,PartNo,OemNo,ArabicName,ModelNo,Manufacturer,Weight,ExpiryPeriod,Remarks,ArabicLongDescription,LongDescription,Active,StockItem,IsExpiry,PurchaseUnit,SellingUnit,CategoryID,BrandID FROM InvItemMaster WHERE ID = @ID;"
                + "select * from InvItemImages where ItemID = @ID;",
                new SqlConnection(ConnectionString));
            General.SelectCommand.Parameters.AddWithValue("@ID", ID);
            DataSet Results = new DataSet();
            General.Fill(Results);
            return Results;
        }
        
    }
}
