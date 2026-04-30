using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Inventory.Masters
{
    public class InvAreaMaster
    {
        String ConnectionString;
        public InvAreaMaster(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable Fill()
        {
            SqlCommand Cmd = new SqlCommand("InvAreaMasterSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 1);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataTable SelectCode()
        {

            SqlCommand Cmd = new SqlCommand("InvAreaMasterSP", new SqlConnection(ConnectionString));
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
                Cmd.CommandText = "SELECT Count(*) As Count FROM InvAreaMaster WHERE Description=@Value";
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

        public DataTable GetStateDropdown()
        {
            SqlCommand Cmd = new SqlCommand("InvAreaMasterSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 6);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataTable RowClick(Int64 ID)
        {
            SqlCommand Cmd = new SqlCommand("InvAreaMasterSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 4);
            Cmd.Parameters.AddWithValue("@ID", ID);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }


        public string InsertInvAreaMaster(Models.Inventory.Masters.InvAreaMaster ma, SqlConnection Conn, SqlTransaction Tx)
        {
            try
            {

                SqlCommand Cmd = new SqlCommand("InvAreaMasterSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 3);
                Cmd.Parameters.AddWithValue("@ID", ma.ID);
                Cmd.Parameters.AddWithValue("@Code", ma.Code);
                Cmd.Parameters.AddWithValue("@Description", ma.Description);
                Cmd.Parameters.AddWithValue("@ArDescription", ma.ArDescription);
                Cmd.Parameters.AddWithValue("@StateID", ma.StateID);
                Cmd.Parameters.AddWithValue("@Active", ma.Active);
               
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


        public int DeleteInvAreaMaster(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("InvAreaMasterSP", new SqlConnection(ConnectionString));
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
