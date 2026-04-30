using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSample.DAL.General.Common
{
    public class CommandText
    {
        String ConnectionString;
        public CommandText(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public String CommandTexts(String Criteria, Object PrimaryVoucherID = null, Object BranchID = null, Object PartyID = null, Object LocationID = null,
            Boolean? IsSizeItem = null, Boolean IsMargin = false, Object VoucherID = null, Object ItemID = null, Boolean ISTransitLoc = false,
            Boolean IsFinishedGood = false, Boolean IsRawMaterial = false, Object ModeID = null, Object PageID = null, DateTime? VoucherDate = null, Object TransactionID = null
            )
        {
            if (ConnectionString == "") return "";
            if (Criteria == "") return "";
            StringBuilder _CommandText = new StringBuilder();
            _CommandText.Append("select dbo.GetCommandText('");
            _CommandText.Append(Criteria);
            _CommandText.Append("'");
            if (PrimaryVoucherID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(PrimaryVoucherID.ToString());
            }
            if (BranchID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(BranchID.ToString());
            }
            if (PartyID == null || PartyID == DBNull.Value)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(PartyID.ToString());
            }
            if (LocationID == null || LocationID.ToString() == "")
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(LocationID.ToString());
            }
            if (IsSizeItem == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",'");
                _CommandText.Append(IsSizeItem);
                _CommandText.Append("'");
            }
            _CommandText.Append(",'");
            _CommandText.Append(IsMargin);
            _CommandText.Append("'");


            if (VoucherID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(VoucherID.ToString());
            }
            if (ItemID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(ItemID.ToString());
            }
            //check if location to be used is InTransit location
            _CommandText.Append(",'");
            _CommandText.Append(ISTransitLoc);
            _CommandText.Append("'");
            if (IsFinishedGood)
            {
                _CommandText.Append(",1");
            }
            else
            {
                _CommandText.Append(",null");
            }
            if (IsRawMaterial)
            {
                _CommandText.Append(",1");
            }
            else
            {
                _CommandText.Append(",null");
            }
            if (ModeID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(ModeID);
            }
            if (PageID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(PageID);
            }
            if (VoucherDate == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append("'");
                _CommandText.Append(Convert.ToDateTime(VoucherDate).Month);
                _CommandText.Append("/");
                _CommandText.Append(Convert.ToDateTime(VoucherDate).Day);
                _CommandText.Append("/");
                _CommandText.Append(Convert.ToDateTime(VoucherDate).Year);
                _CommandText.Append("'");
            }
            if (TransactionID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(TransactionID);
            }
            _CommandText.Append(")");
            SqlConnection Con = new SqlConnection(ConnectionString);
            Con.Open();
            SqlCommand Cmd = new SqlCommand(_CommandText.ToString(), Con);
            Object _OutPut = Cmd.ExecuteScalar();
            Con.Close();
            return _OutPut.ToString();
        }
        public String ItemCommandText(String Criteria, Object PrimaryVoucherID = null, Object BranchID = null, Object PartyID = null, Object LocationID = null,
            Boolean? IsSizeItem = null, Boolean IsMargin = false, Object VoucherID = null, Object ItemID = null, Boolean ISTransitLoc = false,
            Boolean IsFinishedGood = false, Boolean IsRawMaterial = false, Object ModeID = null, Object PageID = null, DateTime? VoucherDate = null, Object TransactionID = null
            )
        {
            if (ConnectionString == "") return "";
            if (Criteria == "") return "";
            StringBuilder _CommandText = new StringBuilder();
            _CommandText.Append("select dbo.GetCommandText('");
            _CommandText.Append(Criteria);
            _CommandText.Append("'");
            if (PrimaryVoucherID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(PrimaryVoucherID.ToString());
            }
            if (BranchID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(BranchID.ToString());
            }
            if (PartyID == null || PartyID == DBNull.Value)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(PartyID.ToString());
            }
            if (LocationID == null || LocationID.ToString() == "")
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(LocationID.ToString());
            }
            if (IsSizeItem == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",'");
                _CommandText.Append(IsSizeItem);
                _CommandText.Append("'");
            }
            _CommandText.Append(",'");
            _CommandText.Append(IsMargin);
            _CommandText.Append("'");


            if (VoucherID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(VoucherID.ToString());
            }
            if (ItemID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(ItemID.ToString());
            }
            //check if location to be used is InTransit location
            _CommandText.Append(",'");
            _CommandText.Append(ISTransitLoc);
            _CommandText.Append("'");
            if (IsFinishedGood)
            {
                _CommandText.Append(",1");
            }
            else
            {
                _CommandText.Append(",null");
            }
            if (IsRawMaterial)
            {
                _CommandText.Append(",1");
            }
            else
            {
                _CommandText.Append(",null");
            }
            if (ModeID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(ModeID);
            }
            if (PageID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(PageID);
            }
            if (VoucherDate == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append("'");
                _CommandText.Append(Convert.ToDateTime(VoucherDate).Month);
                _CommandText.Append("/");
                _CommandText.Append(Convert.ToDateTime(VoucherDate).Day);
                _CommandText.Append("/");
                _CommandText.Append(Convert.ToDateTime(VoucherDate).Year);
                _CommandText.Append("'");
            }
            if (TransactionID == null)
            {
                _CommandText.Append(",null");
            }
            else
            {
                _CommandText.Append(",");
                _CommandText.Append(TransactionID);
            }
            _CommandText.Append(")");
            SqlConnection Con = new SqlConnection(ConnectionString);
            Con.Open();
            SqlCommand Cmd = new SqlCommand(_CommandText.ToString(), Con);
            Object _OutPut = Cmd.ExecuteScalar();
            Con.Close();
            return _OutPut.ToString();
        }
        public Object GetScalar(String CommandText)
        {
            SqlCommand sqlCommand = new SqlCommand(CommandText, new SqlConnection(ConnectionString));
            Object obj = sqlCommand.ExecuteScalar();
            return obj;
        }
        public DataTable GetTable(String CommandText)
        {
            SqlCommand sqlCommand = new SqlCommand(CommandText, new SqlConnection(ConnectionString));
            DataTable dt = new DataTable();
            new SqlDataAdapter(sqlCommand).Fill(dt);
            return dt;
        }
        /// <summary>
        /// Rteurn lookup list
        /// </summary>
        /// <param name="Criteria">lookup criteria</param>
        /// <param name="Searchtext"></param>
        /// <returns></returns>
        public DataTable GetTable(Models.Common.LookupModel lookupModel)
        {
            SqlCommand Cmd = new SqlCommand("LookupSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Criteria", lookupModel.Criteria);
            Cmd.Parameters.AddWithValue("@SearchText", lookupModel.SearchText);
            Cmd.Parameters.AddWithValue("@IntParam1", lookupModel.IntParam1);
            Cmd.Parameters.AddWithValue("@IntParam2", lookupModel.IntParam2);
            Cmd.Parameters.AddWithValue("@IntParam3", lookupModel.IntParam3);
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }
    }
}
