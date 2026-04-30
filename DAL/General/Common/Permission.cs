using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.General.Common
{
    public class Permission
    {
        String ConnectionString;
        public Permission(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public DataTable GetCompaniesFromMaster()
        {
            try
            {
                SqlConnection Con = new SqlConnection(SharedClass.MasterConnectionString);
                SqlCommand Cmd = new SqlCommand("select ID,Code,Name,DatabaseName,ServerName,ServerIP,IsRemote from Company where Active=1 order by isnull(IsDefault,0) desc,Name", Con);
                Cmd.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable GetPermissions(Object UserID, Object PageID, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("PermissionsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetPermissions");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@PageID", PageID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                //sdr.Close();
                //Con.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable GetUserAuthentication(String Username, String Password, Object BranchID = null)
        {
            DataTable dt = new DataTable();
            if (ConnectionString != "")
            {
                try
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("spAuthendication", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Mode", 1);
                    Cmd.Parameters.AddWithValue("@Username", Username);
                    Cmd.Parameters.AddWithValue("@Password", Password);
                    //Cmd.Parameters.AddWithValue("@Password", "svyDtPOkie//NnTmLTW6wB1GKmNK8tBe2NrJrqaxiJw=");
                    if (BranchID != null && BranchID.ToString() != String.Empty)
                    {
                        Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    }
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return dt;
        }
        public DataTable GetCompanyConnectionDetails(Int64 ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection Con = new SqlConnection(SharedClass.MasterConnectionString);
                SqlCommand Cmd = new SqlCommand("SecuritySP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 0);
                Cmd.Parameters.AddWithValue("@ID", ID);
                new SqlDataAdapter(Cmd).Fill(dt);
            }
            catch (Exception)
            {

            }
            return dt;
        }
        public DataTable GetBranches()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("select ID,Company from MaCompanies where ActiveFlag=1", Con);
                Cmd.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public void UpdateUserTrack(Object UserID, String TableName, DateTime ActionDate, String Reason, Int16 ActionID, Object RowID, String MachineName, String ModuleName, String Reference, Decimal Amount)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("UserTrackSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateUserTrack");
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@TableName", TableName);
                Cmd.Parameters.AddWithValue("@ActionDate", ActionDate);
                Cmd.Parameters.AddWithValue("@Reason", Reason);
                Cmd.Parameters.AddWithValue("@ActionID", ActionID);
                Cmd.Parameters.AddWithValue("@RowID", RowID);
                Cmd.Parameters.AddWithValue("@MachineName", MachineName);
                Cmd.Parameters.AddWithValue("@ModuleName", ModuleName);
                Cmd.Parameters.AddWithValue("@Reference", Reference);
                Cmd.Parameters.AddWithValue("@Amount", Amount);
                Cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public void UpdateUserTrack(Object UserID, String TableName, DateTime ActionDate, String Reason, Int16 ActionID, Object RowID, String MachineName, String ModuleName, String Reference, Decimal Amount, SqlConnection Con, SqlTransaction tx)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("UserTrackSP", Con, tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateUserTrack");
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@TableName", TableName);
                Cmd.Parameters.AddWithValue("@ActionDate", ActionDate);
                Cmd.Parameters.AddWithValue("@Reason", Reason);
                Cmd.Parameters.AddWithValue("@ActionID", ActionID);
                Cmd.Parameters.AddWithValue("@RowID", RowID);
                Cmd.Parameters.AddWithValue("@MachineName", MachineName);
                Cmd.Parameters.AddWithValue("@ModuleName", ModuleName);
                Cmd.Parameters.AddWithValue("@Reference", Reference);
                Cmd.Parameters.AddWithValue("@Amount", Amount);
                Cmd.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public bool NextPahseProceed(Object TransactionID, Object UserID)
        {
            try
            {
                bool NextPhase = false;
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("PreventEditAfterNextStep", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                NextPhase = Convert.ToBoolean(Cmd.ExecuteScalar());
                return NextPhase;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public async Task<DataTable> GetEmailCredentials()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("GenEmailSettingsSP", Con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                Cmd.Parameters.AddWithValue("@Mode", 3);
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                //await (PaymentGatewayLog("", "", "Get Mail Settings Failed", ex.Message));
            }
            return dt;
        }

       
    }
}
