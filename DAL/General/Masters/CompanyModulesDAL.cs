using ERPSample.Models.General.Masters;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.General.Masters
{
    public class CompanyModulesDAL
    {
        String ConnectionString;
        public CompanyModulesDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public DataTable Fill()
        {
            SqlCommand Cmd = new SqlCommand("UserSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode",7);
            SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.BigInt);
            outputParam.Direction = ParameterDirection.Output;
            Cmd.Parameters.Add(outputParam);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataSet GetCompanyandModule()
        {
            SqlCommand Cmd = new SqlCommand("UserSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Mode", 8);
            SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.BigInt);
            outputParam.Direction = ParameterDirection.Output;
            Cmd.Parameters.Add(outputParam);
            DataSet ds = new DataSet();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;
        }

        public string InsertCompanyModules(CompanyModulesModel model)
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlTransaction tx = con.BeginTransaction();

            try
            {
                // Remove the company's previous module mappings
                using (SqlCommand deleteCmd = new SqlCommand(
                    "DELETE FROM CompanyModules WHERE CompanyID = @CompanyID", con, tx))
                {
                    deleteCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = model.CompanyID;
                    deleteCmd.ExecuteNonQuery();
                }

                // Insert each selected module
                foreach (int moduleID in model.ModuleIDs)
                {
                    using SqlCommand cmd = new SqlCommand("UserSP", con, tx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 9);
                    cmd.Parameters.AddWithValue("@CompanyID", model.CompanyID);
                    cmd.Parameters.AddWithValue("@ModuleID", moduleID);
                    SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return model.CompanyID.ToString();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public DataTable RowClick(int ID)
        {
            using (SqlConnection Con = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("UserSP", Con))
            {
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.AddWithValue("@Mode", 10);
                Cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = ID;
                SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.BigInt);
                outputParam.Direction = ParameterDirection.Output;
                Cmd.Parameters.Add(outputParam);
                DataTable dt = new DataTable();

                using (SqlDataAdapter adapter = new SqlDataAdapter(Cmd))
                {
                    adapter.Fill(dt);
                }

                return dt;
            }
        }

        public int DeleteCompanyModule(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("UserSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 11);
                Cmd.Parameters.AddWithValue("@ID", ID);
                SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.BigInt);
                outputParam.Direction = ParameterDirection.Output;
                Cmd.Parameters.Add(outputParam);
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
