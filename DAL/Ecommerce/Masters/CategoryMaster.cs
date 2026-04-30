using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Ecommerce.Masters
{
    public class CategoryMaster
    {
        String ConnectionString;
        public CategoryMaster(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable Fill()
        {
            SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 6);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataTable RowClick(Int64 ID, int? TenantID)
        {
            SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 7);
            Cmd.Parameters.AddWithValue("@ID", ID);
            Cmd.Parameters.AddWithValue("@TenantID", TenantID);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }


        public string InsertCategory(Models.MaMisc ma, SqlConnection Conn, SqlTransaction Tx)
        {
            try
            {

                SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 8);
                Cmd.Parameters.AddWithValue("@ID", ma.ID);
                Cmd.Parameters.AddWithValue("@Code", ma.code);
                Cmd.Parameters.AddWithValue("@Description", ma.Description);
                Cmd.Parameters.AddWithValue("@ArabicDescription", ma.ArabicDescription);
                Cmd.Parameters.AddWithValue("@Value", ma.Value);
                Cmd.Parameters.AddWithValue("@ImagePath", ma.ImagePath);
                Cmd.Parameters.AddWithValue("@Active", ma.Active);
                if (ma.Key != null && ma.Key.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Key", ma.Key);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Key", "ItemCategory");
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


        public int DeleteCategory(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 4);
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

        public DataTable SelectCode()
        {

            SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 9);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;

        }

        //Checks whether dupliacte value is entered  [07/07/2023  ADITHYA K A]//
        public int CheckDuplicateEntry(string Value)
        {
            try
            {

                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                Cmd.CommandText = "SELECT Count(*) FROM MaMisc WHERE MaMisc.[Key] = 'ItemCategory' and Value=@Value";
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
    }
}
