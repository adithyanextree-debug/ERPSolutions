using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.General.Common
{
    public class Menu
    {
        String ConnectionString;
        public Menu(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public DataTable FillMenu(Object BranchID, Object UserID, String Language = "English")
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("MenuSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillMenu");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@Language", Language);
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataRow FillMenuByPageID(Object PageID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("MenuSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillMenuByPageID");
                Cmd.Parameters.AddWithValue("@PageID", PageID);
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt.Rows[0];
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public String GetDynamicVoucherUrl(String BasicVType)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("MenuSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetDynamicVoucherUrl");
                Cmd.Parameters.AddWithValue("@BasicVType", BasicVType);
                Object Obj = Cmd.ExecuteScalar();
                Con.Close();
                return Obj.ToString();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="VoucherID"></param>
        /// <param name="Key">for Passing key as a parameter for MaMisc Page</param>
        /// <returns></returns>
        public DataTable LoadWindowsForm(Object PageID = null, Object VoucherID = null, Object Key = null)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("MenuSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "LoadWindowsForm");
                    if (PageID != null)
                    {
                        Cmd.Parameters.AddWithValue("@PageID", PageID);
                    }
                    if (VoucherID != null)
                    {
                        Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                    }
                    if (Key != null)
                    {
                        Cmd.Parameters.AddWithValue("@Key", Key);
                    }
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable LoadWindowsFormByTransactionID(Object TransactionID)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("MenuSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "LoadWindowsFormByTransactionID");
                    Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}
