using ERPSample.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Masters
{
    public class ColorMaster
    {
        String ConnectionString;
        public ColorMaster(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable Fill()
        {
            SqlCommand Cmd = new SqlCommand("MaMiscColorMasterSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 1);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataTable SelectCode()
        {

            SqlCommand Cmd = new SqlCommand("MaMiscColorMasterSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 2);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;

        }

        public int CheckDuplicateEntry(string Value)
        {
            try
            {

                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                Cmd.CommandText = "SELECT Count(*) As Count FROM MaMisc WHERE [Key]='Item Color' and Value=@Value";
                Cmd.Parameters.AddWithValue("@Value", Value);
                int count = (int)Cmd.ExecuteScalar();
                Cmd.Connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable RowClick(Int64 ID)
        {
            SqlCommand Cmd = new SqlCommand("MaMiscColorMasterSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 4);
            Cmd.Parameters.AddWithValue("@ID", ID);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }


        public string InsertColorMaster(MaMisc ma, SqlConnection Conn, SqlTransaction Tx)
        {
            try
            {

                SqlCommand Cmd = new SqlCommand("MaMiscColorMasterSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 3);
                Cmd.Parameters.AddWithValue("@ID", ma.ID);
                Cmd.Parameters.AddWithValue("@Code", ma.code);
                Cmd.Parameters.AddWithValue("@Value", ma.Value);
                Cmd.Parameters.AddWithValue("@Description", ma.Description);
               // Cmd.Parameters.AddWithValue("@ArDescription", ma.ArDescription);
                Cmd.Parameters.AddWithValue("@Active", ma.Active);
                if (ma.Key != null && ma.Key.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Key", ma.Key);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Key", "Item Color");
                }
                Cmd.Connection.Open();
                string val = Convert.ToString(Cmd.ExecuteScalar());
                Cmd.Connection.Close();
                return val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int DeleteColorMaster(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("MaMiscColorMasterSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 5);
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.Connection.Open();
                int val = Cmd.ExecuteNonQuery();
                Cmd.Parameters.Clear();
                Cmd.Connection.Close();
                return val;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

    }
}
