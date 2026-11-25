using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ERPSample.Models;
using ERPSample.Models.General.Users;
using Microsoft.Data.SqlClient;
using System.Data;
using static ERPSample.Models.Inventory.Transactions.TransactionHelperClass;

namespace ERPSample.DAL.General.Users
{
    public class UsersDAL
    {
        String ConnectionString;
        public UsersDAL(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable Fill(long BranchID)
        {
            SqlCommand Cmd = new SqlCommand("spNewEmployees", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Criteria", "FillEmployeeMaster");
            Cmd.Parameters.AddWithValue("@BranchID", BranchID);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataTable GetDesignations()
        {
            SqlCommand Cmd = new SqlCommand("DropDownListSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Criteria", "FillDesigination");
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        public DataTable GetRoles(long id)
        {
            SqlCommand Cmd = new SqlCommand("DropDownListSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Criteria", "FillRoles");
            Cmd.Parameters.AddWithValue("@IntParam", id);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        //exec spNewEmployees @Criteria=N'FillUserRights',@RoleID=93
        public DataTable UserRoles(int id)
        {
            SqlCommand Cmd = new SqlCommand("spNewEmployees", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Criteria", "FillUserRights");
            Cmd.Parameters.AddWithValue("@RoleID", id);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        //public int ChangePermissiomStatus(int id, int active)
        //{

        //    SqlCommand Cmd = new SqlCommand();
        //    try
        //    {
        //        Cmd.Connection = new SqlConnection(ConnectionString);
        //        Cmd.CommandText = "UserSP";
        //        Cmd.CommandType = CommandType.StoredProcedure;
        //        Cmd.Parameters.AddWithValue("@ID", id);
        //        Cmd.Parameters.AddWithValue("@Active", active);
        //        Cmd.Parameters.AddWithValue("@Mode", 2);
        //        Cmd.Connection.Open();
        //        Cmd.Transaction = Cmd.Connection.BeginTransaction();
        //        int val = Convert.ToInt16(Cmd.ExecuteScalar());
        //        Cmd.Transaction.Commit();
        //        Cmd.Connection.Close();
        //        return val;

        //    }
        //    catch (Exception Ex)
        //    {
        //        if (Cmd.Connection != null && Cmd.Connection.State == ConnectionState.Open)
        //        {
        //            Cmd.Transaction.Rollback();
        //            Cmd.Connection.Close();
        //        }
        //        return -1;
        //    }
        //}

        public int InsertMaUserRights(Models.General.Users.MaUserRights mausertrights)
        {
            SqlCommand Cmd = new SqlCommand();
            try
            {
                Cmd = new SqlCommand("spNewEmployees");
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.AddWithValue("@Criteria", "InsertMaUserRights");
                Cmd.Parameters.AddWithValue("@ID", mausertrights.ID);
                Cmd.Parameters.AddWithValue("@UserDetailsID", mausertrights.UserDetailsID);
                Cmd.Parameters.AddWithValue("@PageMenuID", mausertrights.PageMenuID);
                Cmd.Parameters.AddWithValue("@IsView", mausertrights.IsView);
                Cmd.Parameters.AddWithValue("@IsCreate", mausertrights.IsCreate);
                Cmd.Parameters.AddWithValue("@IsEdit", mausertrights.IsEdit);
                Cmd.Parameters.AddWithValue("@IsCancel", mausertrights.IsCancel);
                Cmd.Parameters.AddWithValue("@IsDelete", mausertrights.IsDelete);
                Cmd.Parameters.AddWithValue("@IsApprove", mausertrights.IsApprove);
                Cmd.Parameters.AddWithValue("@IsEditApproved", mausertrights.IsEditApproved);
                Cmd.Parameters.AddWithValue("@IsHigherApprove", mausertrights.IsHigherApprove);
                Cmd.Parameters.AddWithValue("@IsPrint", mausertrights.IsPrint);
                Cmd.Parameters.AddWithValue("@IsEMail", mausertrights.IsEmail);
                Cmd.Connection.Open();
                Cmd.Transaction = Cmd.Connection.BeginTransaction();
                int val = Convert.ToInt16(Cmd.ExecuteScalar());
                Cmd.Transaction.Commit();
                Cmd.Connection.Close();
                return val;

            }
            catch (Exception Ex)
            {
                if (Cmd.Connection != null && Cmd.Connection.State == ConnectionState.Open)
                {
                    Cmd.Transaction.Rollback();
                    Cmd.Connection.Close();
                }
                return -1;
            }
        }

        public int ChangeRolesStatus(Models.General.Users.MaUserRights mausertrights)
        {
            SqlCommand Cmd = new SqlCommand();
            try
            {
                Cmd = new SqlCommand("spNewEmployees");
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.AddWithValue("@Criteria", "UpdateMaUserRights");
                Cmd.Parameters.AddWithValue("@ID", mausertrights.ID);
                Cmd.Parameters.AddWithValue("@PageMenuID", mausertrights.PageMenuID);
                Cmd.Parameters.AddWithValue("@IsView", mausertrights.IsView);
                Cmd.Parameters.AddWithValue("@IsCreate", mausertrights.IsCreate);
                Cmd.Parameters.AddWithValue("@IsEdit", mausertrights.IsEdit);
                Cmd.Parameters.AddWithValue("@IsCancel", mausertrights.IsCancel);
                Cmd.Parameters.AddWithValue("@IsDelete", mausertrights.IsDelete);
                Cmd.Parameters.AddWithValue("@IsApprove", mausertrights.IsApprove);
                Cmd.Parameters.AddWithValue("@IsEditApproved", mausertrights.IsEditApproved);
                Cmd.Parameters.AddWithValue("@IsHigherApprove", mausertrights.IsHigherApprove);
                Cmd.Parameters.AddWithValue("@IsPrint", mausertrights.IsPrint);
                Cmd.Parameters.AddWithValue("@IsEMail", mausertrights.IsEmail);
                Cmd.Connection.Open();
                Cmd.Transaction = Cmd.Connection.BeginTransaction();
                int val = Convert.ToInt16(Cmd.ExecuteScalar());
                Cmd.Transaction.Commit();
                Cmd.Connection.Close();
                return val;

            }
            catch (Exception Ex)
            {
                if (Cmd.Connection != null && Cmd.Connection.State == ConnectionState.Open)
                {
                    Cmd.Transaction.Rollback();
                    Cmd.Connection.Close();
                }
                return -1;
            }
        }


        public int CheckUserNameIsUnique(UsersModel user,SqlConnection Con,SqlTransaction Tx)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(@"SELECT COUNT(1) FROM dbo.MaEmployees WHERE UserName = @UserName AND ID <> @ID", Con, Tx))
                {
                    cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = user.MaEmployees.UserName;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = user.MaEmployees.ID;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return -1;
            }
        }


        public int InsertUsers(UsersModel model, SqlConnection Con, SqlTransaction Tx)
        {
            if (model?.MaEmployees == null)
                throw new ArgumentException("Employee details are required.");

            DataTable employeesDT =
                MaEmployeesHelper.ToDataTable(new[] { model.MaEmployees });

            DataTable detailsDT =
                MaEmployeeDetailsHelper.ToDataTable(model.MaEmployeeDetails);

            DataTable rightsDT =
                MaUserRightsHelper.ToDataTable(model.MaUserRights);

            using SqlCommand cmd = new SqlCommand("dbo.UserSP", Con, Tx);
            cmd.CommandType = CommandType.StoredProcedure;
            int mode = model.MaEmployees.ID > 0 ? 5 : 1;

            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = mode;

            SqlParameter employeeParam = cmd.Parameters.Add("@MaEmployees", SqlDbType.Structured);
            employeeParam.TypeName = "dbo.udtMaEmployees";
            employeeParam.Value = employeesDT;

            SqlParameter detailParam = cmd.Parameters.Add("@MaEmployeeDetails", SqlDbType.Structured);
            detailParam.TypeName = "dbo.udtMaEmployeeDetails";
            detailParam.Value = detailsDT;

            SqlParameter rightsParam = cmd.Parameters.Add("@MaUserRights", SqlDbType.Structured);
            rightsParam.TypeName = "dbo.udtMaUserRights";
            rightsParam.Value = rightsDT;

            SqlParameter newIDParam = cmd.Parameters.Add("@NewID", SqlDbType.Int);
            newIDParam.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            if (newIDParam.Value == DBNull.Value)
                throw new Exception("User insertion failed. No ID was returned.");

            return Convert.ToInt32(newIDParam.Value);
        }

        public string InsertUsers1(UsersModel model, SqlConnection Con, SqlTransaction Tx)
        {

            using SqlCommand Cmd = new SqlCommand("UserSP", Con, Tx);
            Cmd.CommandType = CommandType.StoredProcedure;

            Cmd.Parameters.AddWithValue("@Criteria", "InsertMaEmployees");
            Cmd.Parameters.AddWithValue("@FirstName", (object)model.MaEmployees.FirstName ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@MiddleName", (object)model.MaEmployees.MiddleName ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@LastName", (object)model.MaEmployees.LastName ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@Address", (object)model.MaEmployees.Address ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@EmailID", (object)model.MaEmployees.EmailID ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@ResidenceNumber", (object)model.MaEmployees.ResidenceNumber ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@OfficeNumber", (object)model.MaEmployees.OfficeNumber ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@MobileNumber", (object)model.MaEmployees.MobileNumber ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@DesignationID", (object)model.MaEmployees.DesignationID ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@Active", model.MaEmployees.Active);
            Cmd.Parameters.AddWithValue("@EmployeeType", (object)model.MaEmployees.EmployeeType ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@UserName", (object)model.MaEmployees.UserName ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@Password", (object)model.MaEmployees.Password ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@GmailID", (object)model.MaEmployees.GmailID ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@IsLocationRestrictedUser", model.MaEmployees.IsLocationRestrictedUser);
            Cmd.Parameters.AddWithValue("@PhotoID", (object)model.MaEmployees.PhotoID ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@CreatedBranchID", (object)model.MaEmployees.CreatedBranchID ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@CreatedBy", model.MaEmployees.CreatedBy);
            Cmd.Parameters.AddWithValue("@CreatedOn", model.MaEmployees.CreatedOn);
            Cmd.Parameters.AddWithValue("@AccountID", (object)model.MaEmployees.AccountID ?? DBNull.Value);
            Cmd.Parameters.AddWithValue("@ImagePath", (object)model.MaEmployees.ImagePath ?? DBNull.Value);

            SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.BigInt);
            outputParam.Direction = ParameterDirection.Output;
            Cmd.Parameters.Add(outputParam);
            Cmd.ExecuteNonQuery();
            return outputParam.Value != DBNull.Value
                ? outputParam.Value.ToString()
                : "";
        }

        //public string UpdateUsers(UsersModel model, SqlConnection Con, SqlTransaction Tx)
        //{
        //    using SqlCommand Cmd = new SqlCommand("spNewEmployees", Con, Tx);
        //    Cmd.CommandType = CommandType.StoredProcedure;

        //    Cmd.Parameters.AddWithValue("@Criteria", "UpdateMaEmployees");
        //    Cmd.Parameters.AddWithValue("@FirstName", (object)model.FirstName ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@MiddleName", (object)model.MiddleName ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@LastName", (object)model.LastName ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@EmailID", (object)model.EmailID ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@ResidenceNumber", (object)model.ResidenceNumber ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@OfficeNumber", (object)model.OfficeNumber ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@MobileNumber", (object)model.MobileNumber ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@DesignationID", (object)model.DesignationID ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@Active", model.Active);
        //    Cmd.Parameters.AddWithValue("@EmployeeType", (object)model.EmployeeType ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@UserName", (object)model.UserName ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@Password", (object)model.Password ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@GmailID", (object)model.GmailID ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@IsLocationRestrictedUser", model.IsLocationRestrictedUser);
        //    Cmd.Parameters.AddWithValue("@PhotoID", (object)model.PhotoID ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@CreatedBranchID", (object)model.CreatedBranchID ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
        //    Cmd.Parameters.AddWithValue("@CreatedOn", model.CreatedOn);
        //    Cmd.Parameters.AddWithValue("@AccountID", (object)model.AccountID ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@ImagePath", (object)model.ImagePath ?? DBNull.Value);
        //    Cmd.Parameters.AddWithValue("@ID", model.ID);

        //    Cmd.ExecuteNonQuery();

        //    return model.ID.ToString();
        //}

        public DataSet RowClick(long ID)
        {
            using (SqlConnection Con = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("spNewEmployees", Con))
            {
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.Add("@Criteria", SqlDbType.VarChar).Value = "FillUsers";
                Cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = ID;

                DataSet ds = new DataSet();

                using (SqlDataAdapter adapter = new SqlDataAdapter(Cmd))
                {
                    adapter.Fill(ds);
                }

                return ds;
            }
        }

        public int DeleteEmployeeDetails(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("spNewEmployees", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "DeleteMaEmployeeDetails");
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


        public int DeleteUsers(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("spNewEmployees", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "DeleteMaEmployees");
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
