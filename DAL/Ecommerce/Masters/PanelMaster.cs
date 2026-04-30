using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Ecommerce.Masters
{
    public class PanelMaster
    {

        String ConnectionString;

        //public static object EcomPanelMaster { get; internal set; }

        public PanelMaster(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable Fill()
        {
            SqlCommand Cmd = new SqlCommand("EcomPanelMasterExtSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 0);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataSet FillAdditionals()
        {
            SqlCommand Cmd = new SqlCommand("EcomPanelMasterExtSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 1);
            DataSet ds = new DataSet();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;

            //SqlDataAdapter General = new SqlDataAdapter("SELECT max(EcomPanelMaster.OrderNo) As LastID FROM EcomPanelMaster;"
            //    + "SELECT * FROM MaMisc WHERE [Key] = 'EcommercePanelTypes' AND Active = 1;"
            //    + "SELECT * FROM MaMisc WHERE [Key] = 'EcommerceItemSortby' AND Active = 1;",
            //    new SqlConnection(ConnectionString));
            //DataSet Results = new DataSet();
            //General.Fill(Results);
            //return Results;
        }

        public DataTable PanelMappingEntities(int PanelTypeID, int DestnID)
        {
            SqlCommand Cmd = new SqlCommand("EcomPanelMasterExtSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@PanelTypeID", PanelTypeID);
            Cmd.Parameters.AddWithValue("@DestnID", DestnID);
            Cmd.Parameters.AddWithValue("@Mode", 2);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataSet GetPanelMaster(Int64 ID)
        {
            SqlCommand Cmd = new SqlCommand("EcomPanelMasterExtSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 3);
            Cmd.Parameters.AddWithValue("@ID", ID);
            DataSet ds = new DataSet();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;
        }

    }
}
