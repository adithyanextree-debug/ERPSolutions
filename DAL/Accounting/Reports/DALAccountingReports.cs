using Microsoft.Data.SqlClient;
using System.Data;
using ERPSample.Models.Accounting.Reports;
namespace ERPSample.DAL.Accounting.Reports
{
    public class DALAccountingReports
    {
        String ConnectionString;
        public DALAccountingReports(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable AccountSatetement(Object AccountID, DateTime FromDate, DateTime ToDate, Object BranchID, Boolean OpeningBalance = true, Object VoucherID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("AccountStatementSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@VTypeID", VoucherID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Opening", OpeningBalance);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable AccountStatmentAccountDetails(Object AccountID, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("AccountStatmentAccountDetailsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable PaymentAnalysis(Object AccountID, DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("PerfomanceAnalysisSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                //Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
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

        public DataTable VATComputation(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString == "") return dt;
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VATComputationSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable GroupStatement(Object AccountID, DateTime FromDate, DateTime ToDate, Object BranchID, Boolean AllGroup, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("GroupSummarySP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@AllGroup", AllGroup);
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

        public DataTable FillDashboardPanels()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("DashboardSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillPanels");
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet FillDashboardData(Object BranchID, DateTime? FromDate = null, DateTime? ToDate = null, Object AccountID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("DashboardSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillData");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable TrialBalance(DateTime FromDate, DateTime ToDate, Object BranchID, Boolean Opening, Boolean OpeningBalance, Boolean ClosingBalance, int TransactionType)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("TrialBalanceSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Opening", Opening);
                Cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);
                Cmd.Parameters.AddWithValue("@ClosingBalance", ClosingBalance);
                Cmd.Parameters.AddWithValue("@TransactionType", TransactionType);
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

        public DataTable TrialBalance(DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString,
            Boolean Opening, Boolean OpeningBalance, Boolean ClosingBalance, Boolean LedgerOnly, int TransactionType)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("TrialBalanceSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Opening", Opening);
                Cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);
                Cmd.Parameters.AddWithValue("@ClosingBalance", ClosingBalance);
                Cmd.Parameters.AddWithValue("@LedgerOnly", LedgerOnly);
                Cmd.Parameters.AddWithValue("@TransactionType", TransactionType);
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

        public DataSet BalanceSheet(DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString, Boolean TwoSided)
        {
            try
            {
                DataSet ds = new DataSet();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("BalanceSheetSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                    Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@TwoSided", TwoSided);
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(ds);
                }
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet ProfitAndLoss(DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString, Boolean TwoSided)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("ProfitAndLossSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@TwoSided", TwoSided);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                //SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataSet ds = new DataSet();
                ada.Fill(ds);
                //sdr.Close();
                //Con.Close();
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet CorporateTax(DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("CorporateTaxSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Mode", 1);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataSet ds = new DataSet();
                ada.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable IncomeExpenseSummary(DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("IncomeExpenseSummarySP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                ada.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// Billwise Statement
        /// </summary>
        /// <param name="FromDate">Start Date</param>
        /// <param name="ToDate">End Date</param>
        /// <param name="BranchID">Specify the Branch</param>
        /// <param name="AccountID">Specify the Account</param>
        /// <returns>Billwise Statement</returns>
        public DataTable BillwiseStatement(ERPSample.Models.Accounting.Reports.BillwiseStatementModel billwisestatementmodel, Int64 BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BillwiseSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                //if (billwisestatementmodel.EffectiveDate == true)
                //{
                //    Cmd.Parameters.AddWithValue("@EffDateFrom", billwisestatementmodel.StartDate);
                //    Cmd.Parameters.AddWithValue("@EffDateTo", billwisestatementmodel.EndDate);
                //}
                //else if (billwisestatementmodel.VDate == true)
                //{
                Cmd.Parameters.AddWithValue("@EffDateFrom", null);
                Cmd.Parameters.AddWithValue("@EffDateTo", null);
                Cmd.Parameters.AddWithValue("@DateFrom", billwisestatementmodel.StartDate);
                Cmd.Parameters.AddWithValue("@DateUpto", billwisestatementmodel.EndDate);
                //}
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (billwisestatementmodel.AccountID != null && billwisestatementmodel.AccountID != 0)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", billwisestatementmodel.AccountID);
                }
                if (billwisestatementmodel.Receivables == true)
                {
                    Cmd.Parameters.AddWithValue("@Receivables", billwisestatementmodel.Receivables);
                }
                if (billwisestatementmodel.Payables == true)
                {
                    Cmd.Parameters.AddWithValue("@Payables", billwisestatementmodel.Payables);
                }
                if (billwisestatementmodel.Detailed == true)
                {
                    Cmd.Parameters.AddWithValue("@Detailed", billwisestatementmodel.Detailed);
                }
                if (billwisestatementmodel.Pending == true)
                {
                    Cmd.Parameters.AddWithValue("@PendingBill", billwisestatementmodel.Pending);
                }
                if ((billwisestatementmodel.DueDaysFrom != null && billwisestatementmodel.DueDaysUpto != null) && (billwisestatementmodel.DueDaysFrom != "" && billwisestatementmodel.DueDaysUpto != ""))
                {
                    Cmd.Parameters.AddWithValue("@DueDaysFrom", billwisestatementmodel.DueDaysFrom);
                    Cmd.Parameters.AddWithValue("@DueDaysUpto", billwisestatementmodel.DueDaysUpto);
                }
                if (billwisestatementmodel.AccCategoryID != null && billwisestatementmodel.AccCategoryID != 0)
                {
                    Cmd.Parameters.AddWithValue("@AccCategoryID", billwisestatementmodel.AccCategoryID);
                }
                if (billwisestatementmodel.AccGroup != null && billwisestatementmodel.AccGroup != 0)
                {
                    Cmd.Parameters.AddWithValue("@AccGroup", billwisestatementmodel.AccGroup);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                //sdr.Close();
                //Con.Close();
                // dt = dt.Select("SELECT * group by ");
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //For accountcategory validation[lookup] on 22-09-2023
        public DataTable accountcategory(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.CommandText = "select [Description] From FiMaAccountCategory where ID=" + ID;

                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //For accgroup validation[lookup] on 22-09-2023
        public DataTable accgroup(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.CommandText = "select [Name] From FiMaAccounts where ID=" + ID;

                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //For account validation[lookup] on 22-09-2023
        public DataTable account(int ID)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.CommandText = "select [Name] From FiMaAccounts where ID=" + ID;

                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable PartyOutstandings(DateTime DateFrom, DateTime DateUpto, Object BranchID, String Nature, Object AccountID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("PartyOutstandingSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Nature", Nature);
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// Buyer List Report
        /// </summary>
        /// <param name="DateFrom">Start Date</param>
        /// <param name="DateUpto">End Date</param>
        /// <param name="BranchID">Branch ID</param>
        /// <param name="Mode">Preferred/Actual</param>
        /// <param name="AccountID">Party AccountID</param>
        /// <returns></returns>
        public DataTable BuyerListReport(DateTime DateFrom, DateTime DateUpto, Object BranchID, String Mode, Object AccountID = null, Object ItemID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BuyerListSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Mode", Mode);
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                if (ItemID != null)
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        // Finance Voucher Summary Details from database.
        public DataTable VoucherSummary(DateTime FromDate, DateTime ToDate, Object BranchID, Object VoucherID, bool Detailed = false, bool AutoEntry = false)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("FinTransactionsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillFinanceVoucherSummary");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                if (Detailed)
                {
                    Cmd.Parameters.AddWithValue("@Detailed", 1);
                }
                if (AutoEntry)
                {
                    Cmd.Parameters.AddWithValue("@AutoEntry", 1);
                }
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

        public DataTable ConsolidatedMonthwise(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("ConsolidatedMonthwiseSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
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

        public DataTable FillChequeRegister(Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("ChequeRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillChequeRegister");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillUserTrack(Object DateFrom = null, Object DateUpto = null, String TableName = "", String Reason = "",
            String Action = "All", Object RowID = null, String MachineName = "", String ModuleName = "", String Reference = "",
            Decimal Amount = 0, String Username = "")
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("UserTrackSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillUserTrack");
                if (DateFrom != null)
                {
                    Cmd.Parameters.AddWithValue("@DateFrom", Convert.ToDateTime(DateFrom));
                }
                if (DateUpto != null)
                {
                    Cmd.Parameters.AddWithValue("@DateUpto", Convert.ToDateTime(DateUpto));
                }
                if (TableName != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@TableName", TableName);
                }
                if (Reason != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@Reason", Reason);
                }
                if (Action != "All")
                {
                    if (Action == "Insert")
                    {
                        Cmd.Parameters.AddWithValue("@ActionID", 0);
                    }
                    else if (Action == "Update")
                    {
                        Cmd.Parameters.AddWithValue("@ActionID", 1);
                    }
                    else if (Action == "Delete")
                    {
                        Cmd.Parameters.AddWithValue("@ActionID", 2);
                    }
                }
                if (RowID != null && RowID.ToString() != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@RowID", RowID);
                }
                if (MachineName != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@MachineName", MachineName);
                }
                if (ModuleName != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@ModuleName", ModuleName);
                }
                if (Reference != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@Reference", Reference);
                }
                if (Amount > 0)
                {
                    Cmd.Parameters.AddWithValue("@Amount", Amount);
                }
                if (Username != "")
                {
                    Cmd.Parameters.AddWithValue("@Username", Username);
                }
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet FillAccountReconcilation(DateTime FromDate, DateTime ToDate, Object AccountID, Object BranchID, Boolean Pending, Object VTypeID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("AccountReconcilationSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillAccountReconcilation");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                if (VTypeID != null)
                {
                    Cmd.Parameters.AddWithValue("@VTypeID", VTypeID);
                }
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Pending", Pending);
                DataSet ds = new DataSet();
                new SqlDataAdapter(Cmd).Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void UpdateAccountReconcilation(DataTable dt, Object BranchID)
        {
            SqlConnection Con = null;
            SqlTransaction tx = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                tx = Con.BeginTransaction();
                SqlCommand Cmd;
                foreach (DataRow dr in dt.Rows)
                {
                    Cmd = new SqlCommand("AccountReconcilationSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateAccountReconcilation");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                    Cmd.Parameters.AddWithValue("@BankDate", dr["BankDate"]);
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.ExecuteNonQuery();
                }
                tx.Commit();
                Con.Close();
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    tx.Rollback();
                    Con.Close();
                }
                throw Ex;
            }
        }

        public DataTable GetBudgets(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BudgetSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBudgets");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                //Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
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

        public DataTable GetBudgetDetails(Object TID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BudgetSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBudgetDetails");
                Cmd.Parameters.AddWithValue("@TID", TID);
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

        public DataTable GetSalesProjection(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("SalesProjectionSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBudgets");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                //Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
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

        public DataTable GetSalesProjectionDetails(Object TID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("SalesProjectionSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBudgetDetails");
                Cmd.Parameters.AddWithValue("@TID", TID);
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

        public DataTable StockRegister(DateTime FromDate, DateTime ToDate, Object ItemID, Object BranchID, String BatchNo = "")
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("StockRegisterSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (BatchNo != "")
                {
                    Cmd.Parameters.AddWithValue("@BatchNo", BatchNo);
                }
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// eReturns from Purchase / Sales
        /// </summary>
        /// <param name="DateFrom">Start Date</param>
        /// <param name="DateUpto">End Date</param>
        /// <param name="BranchID">BarnchID</param>
        /// <param name="BasicVType">Purchase / Sales Invoice</param>
        /// <returns></returns>
        public DataTable eReturns(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object AccountID, String Emirate, String TaxRegNo)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("eReturnsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                if (Emirate != "" && Emirate != "All")
                {
                    Cmd.Parameters.AddWithValue("@Emirate", Emirate);
                }
                if (TaxRegNo != "")
                {
                    Cmd.Parameters.AddWithValue("@TaxRegNo", TaxRegNo);
                }
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable SizeWiseStock(DateTime Date, Object BranchID)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    SqlCommand Cmd = new SqlCommand("SizeWiseStockSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Date", Date);
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(sdr);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable ItemsCatalogue(Object BranchID)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("ItemCatalogueSP", Con);
                    Cmd.CommandTimeout = 600;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable StockRegisterLocationwise(DateTime FromDate, DateTime ToDate, Object BranchID, Object LocationID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("StockRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "Locationwise");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@LocationID", LocationID);
                Cmd.CommandTimeout = 600;
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable StockRegisterTypeOfWoodWise(DateTime FromDate, DateTime ToDate, Object BranchID, Object LocationID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("StockRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "TypeOfWoodWise");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (LocationID != null)
                {
                    Cmd.Parameters.AddWithValue("@LocationID", LocationID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable StockRegisterCommoditywise(DateTime FromDate, DateTime ToDate, Object BranchID, Object LocationID = null, Object TypeOfWoodID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("StockRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "Commoditywise");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (LocationID != null)
                {
                    Cmd.Parameters.AddWithValue("@LocationID", LocationID);
                }
                if (TypeOfWoodID != null)
                {
                    Cmd.Parameters.AddWithValue("@TypeOfWoodID", TypeOfWoodID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable StockRegisterItemwise(DateTime FromDate, DateTime ToDate, Object BranchID, Object LocationID = null, Object TypeOfWoodID = null, Object CommodityID = null, Object ItemID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("StockRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "Itemwise");
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (LocationID != null)
                {
                    Cmd.Parameters.AddWithValue("@LocationID", LocationID);
                }
                if (TypeOfWoodID != null)
                {
                    Cmd.Parameters.AddWithValue("@TypeOfWoodID", TypeOfWoodID);
                }
                if (ItemID != null)
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                }
                if (CommodityID != null)
                {
                    Cmd.Parameters.AddWithValue("@CommodityID", CommodityID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable SalesProjectionRegister(DateTime FromDate, DateTime ToDate, Object BranchID, Boolean Detailed)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("SalesProjectionSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Detailed", Detailed);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable BudgetVsActualItemwise(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object ItemID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BudgetVsActualItemwiseSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet CashFlowStatement(DateTime FromDate, DateTime ToDate, Object BranchID, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("CashFlowStatementSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet CashFlowStatement(String Criteria, DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                DataSet ds = new DataSet();
                if (ConnectionString == "") return ds;
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("CashFlowStatementSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", Criteria);
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FinancialRatios(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("FinancialRatiosSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable PendingWorkflowVouchers(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object UserID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("PendingWorkflowVouchersSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (UserID != null)
                {
                    Cmd.Parameters.AddWithValue("@UserID", UserID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable PendingApprovalVouchers(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object UserID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("PendingApprovalVouchersSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (UserID != null)
                {
                    Cmd.Parameters.AddWithValue("@UserID", UserID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable VoucherStatistics(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object UserID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherStatisticsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (UserID != null)
                {
                    Cmd.Parameters.AddWithValue("@UserID", UserID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet FillApprovalSummary(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object UserID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("ApprovalSummarySP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillInventoryRegister(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object BasicVTypeID, Object VTypeID = null,
            Boolean Detailed = false, Boolean Inventory = false, Boolean Columnar = false, Boolean GroupItem = false, String Criteria = "",
            Object AccountID = null, Object PaymentTypeID = null, Object ItemID = null, Object CounterID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("InventoryRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (BasicVTypeID != null && BasicVTypeID.ToString() != "-1" && BasicVTypeID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@BasicVTypeID", BasicVTypeID);
                }
                if (VTypeID != null && VTypeID.ToString() != "-1" && VTypeID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@VTypeID", VTypeID);
                }
                Cmd.Parameters.AddWithValue("@Detailed", Detailed);
                Cmd.Parameters.AddWithValue("@Inventory", Inventory);
                Cmd.Parameters.AddWithValue("@Columnar", Columnar);
                Cmd.Parameters.AddWithValue("@IsGroupItemReport", GroupItem);
                if (Criteria != "")
                {
                    Cmd.Parameters.AddWithValue("@Criteria", Criteria);
                }
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                if (PaymentTypeID != null)
                {
                    Cmd.Parameters.AddWithValue("@PaymentTypeID", PaymentTypeID);
                }
                if (ItemID != null)
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                }
                if (CounterID != null)
                {
                    Cmd.Parameters.AddWithValue("@CounterID", CounterID);
                }
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable ItemwiseInventoryRegister(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object BasicVTypeID, Object VTypeID = null,
            Object AccountID = null, Object SalesManID = null, Object ItemID = null, Object BrandID = null, Object OriginID = null, Object ColorID = null,
            Object CommodityID = null, Object LocationID = null, String Manufacturer = "", String GroupBy = "", Object AreaID = null, String VoucherNo = "")
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("ItemwiseInventorySP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (BasicVTypeID != null && BasicVTypeID.ToString() != "-1" && BasicVTypeID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@BasicVTypeID", BasicVTypeID);
                }
                if (VTypeID != null && VTypeID.ToString() != "-1" && VTypeID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@VTypeID", VTypeID);
                }
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                if (BrandID != null)
                {
                    Cmd.Parameters.AddWithValue("@BrandID", BrandID);
                }
                if (OriginID != null)
                {
                    Cmd.Parameters.AddWithValue("@OriginID", OriginID);
                }
                if (ColorID != null)
                {
                    Cmd.Parameters.AddWithValue("@ColorID", ColorID);
                }
                if (CommodityID != null)
                {
                    Cmd.Parameters.AddWithValue("@CommodityID", CommodityID);
                }
                if (LocationID != null)
                {
                    Cmd.Parameters.AddWithValue("@LocationID", LocationID);
                }
                if (Manufacturer != "")
                {
                    Cmd.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                }
                if (GroupBy != "")
                {
                    Cmd.Parameters.AddWithValue("@GroupBy", GroupBy);
                }
                if (ItemID != null)
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                }
                if (SalesManID != null)
                {
                    Cmd.Parameters.AddWithValue("@SalesManID", SalesManID);
                }
                if (AreaID != null)
                {
                    Cmd.Parameters.AddWithValue("@AreaID", AreaID);
                }
                if (VoucherNo != "")
                {
                    Cmd.Parameters.AddWithValue("@VoucherNo", VoucherNo);
                }

                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet SalesReport(DateTime DateFrom, DateTime DateUpto, Object BranchID, Boolean Detailed = false,
           Object AccountID = null, Object SalesManID = null, Object ItemID = null, Object BrandID = null, Object OriginID = null, Object ColorID = null,
           Object CommodityID = null, Object LocationID = null, String Manufacturer = "", Object AreaID = null,
           String VoucherNo = "", String Criteria = "")
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("SalesReportSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);

                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                if (BrandID != null)
                {
                    Cmd.Parameters.AddWithValue("@BrandID", BrandID);
                }
                if (OriginID != null)
                {
                    Cmd.Parameters.AddWithValue("@OriginID", OriginID);
                }
                if (ColorID != null)
                {
                    Cmd.Parameters.AddWithValue("@ColorID", ColorID);
                }
                if (CommodityID != null)
                {
                    Cmd.Parameters.AddWithValue("@CommodityID", CommodityID);
                }
                if (LocationID != null)
                {
                    Cmd.Parameters.AddWithValue("@LocationID", LocationID);
                }
                if (Manufacturer != "")
                {
                    Cmd.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                }
                if (ItemID != null)
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                }
                if (AreaID != null)
                {
                    Cmd.Parameters.AddWithValue("@AreaID", AreaID);
                }
                if (VoucherNo != "")
                {
                    Cmd.Parameters.AddWithValue("@VoucherNo", VoucherNo);
                }
                if (Criteria != "")
                {
                    Cmd.Parameters.AddWithValue("@Criteria", "Datewise");
                }
                Cmd.Parameters.AddWithValue("@Detailed", Detailed);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FinanceRegister(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object BasicVTypeID, Object VTypeID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("FinanceRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (BasicVTypeID != null)
                {
                    Cmd.Parameters.AddWithValue("@BasicVTypeID", BasicVTypeID);
                }
                if (VTypeID != null)
                {
                    Cmd.Parameters.AddWithValue("@VTypeID", VTypeID);
                }
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public DataTable CostCentreWisePandL(DateTime FromDate, DateTime ToDate, Object BranchID, Object CostCategory, Object CostCentre, bool Costcenterwise, bool Detailed, bool ViewCostcenter)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("ProfitAndLossSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CostCentreWisePandL");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@CostCentreCategoryID", CostCategory);
                Cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                Cmd.Parameters.AddWithValue("@CostCenterwise", Costcenterwise);
                Cmd.Parameters.AddWithValue("@ViewCostcenter", ViewCostcenter);
                Cmd.Parameters.AddWithValue("@Detailed", Detailed);
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

        public DataTable CostCentreWiseBalanceSheet(DateTime FromDate, DateTime ToDate, Object BranchID, Object CostCategory, Object CostCentre, bool Costcenterwise, bool Detailed, bool ViewCostcenter)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BalanceSheetSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CostcenterwiseBalanceSheet");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@CostCentreCategoryID", CostCategory);
                Cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                Cmd.Parameters.AddWithValue("@CostCenterwise", Costcenterwise);
                Cmd.Parameters.AddWithValue("@ViewCostcenter", ViewCostcenter);
                Cmd.Parameters.AddWithValue("@Detailed", Detailed);
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
        public DataTable CostCategorySummary(DateTime FromDate, DateTime ToDate, Object BranchID, Object CostCategory, Object CostCentre)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("CostCentreReportSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CostCategorySummary");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@CostCentreCategoryID", CostCategory);
                Cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
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
        public DataTable CostCenterBreakUp(DateTime FromDate, DateTime ToDate, Object BranchID, Object CostCategory, Object CostCentre)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("CostCentreReportSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CostCenterBreakUp");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@CostCentreCategoryID", CostCategory);
                Cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
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

        public DataTable AccountBreakUp(DateTime FromDate, DateTime ToDate, Object BranchID, Object AccountID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("CostCentreReportSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "AccountBreakUp");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
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

        public DataTable VehicleMileageReport(DateTime FromDate, DateTime ToDate, Object BranchID, Object VehicleID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VehicleReportsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "VehicleMileage");
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@VehicleID", VehicleID);
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
        public DataTable ItemSearchItem(String Value, Object BranchID, Object ItemID = null)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("ItemSearchItemSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Value", Value);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                if (ItemID != null)
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                }
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable GratuityProvision(DateTime Date)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("HRGratuityProvisionSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Date", Date);
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable CashBankBook(DateTime FromDate, DateTime ToDate, Object BranchID, Object GroupAccount = null)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    SqlCommand Cmd = new SqlCommand("CashBankBookSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                    Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    if (GroupAccount != null)
                    {
                        Cmd.Parameters.AddWithValue("@GroupAccount", GroupAccount);
                    }
                    SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(sdr);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable ItemAging(DateTime DateFrom, DateTime DateUpto, Object BranchID, Object ItemID = null, Object BrandID = null, Object OriginID = null, String Barcode = "", Object CommodityID = null)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    SqlCommand Cmd = new SqlCommand("ItemAgingSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                    Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    if (ItemID != null)
                    {
                        Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                    }
                    if (BrandID != null)
                    {
                        Cmd.Parameters.AddWithValue("@BrandID", BrandID);
                    }
                    if (CommodityID != null)
                    {
                        Cmd.Parameters.AddWithValue("@CommodityID", CommodityID);
                    }
                    if (OriginID != null)
                    {
                        Cmd.Parameters.AddWithValue("@OriginID", OriginID);
                    }
                    if (Barcode != "")
                    {
                        Cmd.Parameters.AddWithValue("@Barcode", Barcode);
                    }
                    SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(sdr);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable AgingReport(DateTime DateFrom, DateTime DateUpto, Object BranchID, String Nature, Object AccountID = null, Object StaffID = null)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    SqlCommand Cmd = new SqlCommand("AgingSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                    Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@Nature", Nature);
                    if (AccountID != null)
                    {
                        Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                    }
                    if (StaffID != null)
                    {
                        Cmd.Parameters.AddWithValue("@StaffID", StaffID);
                    }
                    SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(sdr);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable SalesContractAnalysis(Object FromDate, Object ToDate, Object BranchID, Object SalesContractID, Object SalesInvoiceID,
            Boolean Detailed, Boolean PendingBills, Object PartyID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("SalesContractAnalysisSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@SalesContractID", SalesContractID);
                Cmd.Parameters.AddWithValue("@SalesInvoiceID", SalesInvoiceID);
                Cmd.Parameters.AddWithValue("@Detailed", Detailed);
                Cmd.Parameters.AddWithValue("@PendingBill", PendingBills);
                Cmd.Parameters.AddWithValue("@PartyID", PartyID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable MonthWiseProfitAndLoss(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("MonthWiseProfitandLossSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                ada.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable MonthWiseBalanceSheet(DateTime FromDate, DateTime ToDate, Object BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("MonthWiseBalanceSheetSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                ada.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet WeeklyCollectionReport(DateTime FromDate, DateTime ToDate, Object BranchID, Object PartyID, Object StartDay, Boolean Weekly)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("WeeklyCollectionReportSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@FromDate", FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", ToDate);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@PartyID", PartyID);
                Cmd.Parameters.AddWithValue("@Startday", StartDay);
                Cmd.Parameters.AddWithValue("@Weekly", Weekly);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataSet ds = new DataSet();
                ada.Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable BranchWiseProfitandLoss(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("BranchWiseReportsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "ProfitandLoss");
                Cmd.Parameters.AddWithValue("@Datefrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                ada.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable BranchWiseBalanceSheet(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("BranchWiseReportsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "BalanceSheet");
                Cmd.Parameters.AddWithValue("@Datefrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                SqlDataAdapter ada = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                ada.Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet CostCentreReport(Object BranchID, DateTime DateFrom, DateTime DateUpto, Object CostCentreID = null, Object AccountID = null)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand("CostCentreSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CostCentreReport");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Datefrom", DateFrom);
                Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                if (CostCentreID != null)
                {
                    Cmd.Parameters.AddWithValue("@CostCentreID", CostCentreID);
                }
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                DataSet ds = new DataSet();
                new SqlDataAdapter(Cmd).Fill(ds);
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}
