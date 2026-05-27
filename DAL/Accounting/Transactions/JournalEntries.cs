using Microsoft.Data.SqlClient;
using System.Data;

namespace ERPSample.DAL.Accounting.Transactions
{
    public class JournalEntries
    {
        String ConnectionString;

        public JournalEntries(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        public DataTable FillTransactions(Int64 ID)
        {
            SqlCommand Cmd = new SqlCommand("VoucherSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Criteria", "FillTransactions");
            Cmd.Parameters.AddWithValue("@ID", ID);
            DataTable ds = new DataTable();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;
        }
        public DataTable FillTransactionEntries(Int64 TransactionID)
        {
            SqlCommand Cmd = new SqlCommand("VoucherSP", new SqlConnection(ConnectionString));
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("Criteria", "FillTransactionEntries");
            Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
            DataTable ds = new DataTable();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;
        }
        public string DeleteTransactions(Int64 ID)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand("VoucherSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactions");
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.Connection.Open();
                Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                return "1";
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    return Ex.Message;

                }
                return Ex.Message;
            }
        }
        public string DeleteTransactionEntries(Int64 ID)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand("VoucherSaveSP", new SqlConnection(ConnectionString));
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 2);
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.Connection.Open();
                Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                return "1";
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    return Ex.Message;

                }
                return Ex.Message;
            }
        }
        public string InsertTransactionEntries(Models.FiTransactionEntries FiTransactionEntries)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactionEntries");
                Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionEntries.TransactionID);
                Cmd.Parameters.AddWithValue("@DrCr", FiTransactionEntries.DrCr);
                Cmd.Parameters.AddWithValue("@Nature", FiTransactionEntries.Nature);
                Cmd.Parameters.AddWithValue("@AccountID", FiTransactionEntries.AccountID);
                Cmd.Parameters.AddWithValue("@Amount", FiTransactionEntries.Amount);
                Cmd.Parameters.AddWithValue("@FCAmount", FiTransactionEntries.FCAmount);
                Cmd.Parameters.AddWithValue("@BankDate", FiTransactionEntries.BankDate);
                Cmd.Parameters.AddWithValue("@RefPageTypeID", FiTransactionEntries.RefPageTypeID);
                Cmd.Parameters.AddWithValue("@CurrencyID", FiTransactionEntries.CurrencyID);
                Cmd.Parameters.AddWithValue("@ExchangeRate", FiTransactionEntries.ExchangeRate);
                Cmd.Parameters.AddWithValue("@RefPageTableID", FiTransactionEntries.RefPageTableID);
                Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransactionEntries.ReferenceNo);
                Cmd.Parameters.AddWithValue("@TranType", FiTransactionEntries.TranType);
                Cmd.Parameters.AddWithValue("@DueDate", FiTransactionEntries.DueDate);
                Cmd.Parameters.AddWithValue("@RefTransID", FiTransactionEntries.RefTransID);
                Cmd.Parameters.AddWithValue("@TaxPerc", FiTransactionEntries.TaxPerc);
                if (FiTransactionEntries.Description != null && FiTransactionEntries.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", FiTransactionEntries.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }

                Cmd.ExecuteNonQuery();
                Con.Close();
                Con.Open();
                SqlCommand cmd = new SqlCommand("Select Top  1 ID FROM FiTransactionEntries ORDER BY ID DESC", Con);
                SqlDataReader LastInsertedID = cmd.ExecuteReader();
                long PanelID = 0;
                while (LastInsertedID.Read())
                {
                    PanelID = Convert.ToInt32(LastInsertedID["ID"]);

                }
                Con.Close();
                return PanelID.ToString();

            }
            catch (Exception Ex)
            {
                throw; // This will break and pass the error to the controller's try/catch
            }

        }
        public string InsertTransactions(Models.FiTransactions FiTransaction)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactions");
                Cmd.Parameters.AddWithValue("@Date", FiTransaction.Date);
                Cmd.Parameters.AddWithValue("@EffectiveDate", FiTransaction.EffectiveDate);
                Cmd.Parameters.AddWithValue("@VoucherID", FiTransaction.VoucherID);
                Cmd.Parameters.AddWithValue("@SerialNo", FiTransaction.SerialNo);
                Cmd.Parameters.AddWithValue("@TransactionNo", FiTransaction.TransactionNo);
                Cmd.Parameters.AddWithValue("@IsPostDated", FiTransaction.IsPostDated);
                Cmd.Parameters.AddWithValue("@CurrencyID", FiTransaction.CurrencyID);
                Cmd.Parameters.AddWithValue("@CompanyID", FiTransaction.CompanyID);
                Cmd.Parameters.AddWithValue("@ApprovalStatus", FiTransaction.ApprovalStatus);
                Cmd.Parameters.AddWithValue("@StatusID", FiTransaction.StatusID);
                Cmd.Parameters.AddWithValue("@Posted", FiTransaction.Posted);
                Cmd.Parameters.AddWithValue("@Active", FiTransaction.AddedBy);
                Cmd.Parameters.AddWithValue("@IsAutoEntry", FiTransaction.IsAutoEntry);
                Cmd.Parameters.AddWithValue("@Cancelled", FiTransaction.Cancelled);
                Cmd.Parameters.AddWithValue("@AddedBy", FiTransaction.AddedBy);
                Cmd.Parameters.AddWithValue("@AddedDate", FiTransaction.AddedDate);
                Cmd.Parameters.AddWithValue("@ApprovedBy", FiTransaction.AddedBy);
                Cmd.Parameters.AddWithValue("@PageID", FiTransaction.PageID);

                if (FiTransaction.Description != null && FiTransaction.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", FiTransaction.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }

                if (FiTransaction.ReferenceNo != null && FiTransaction.ReferenceNo.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransaction.ReferenceNo);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ReferenceNo", DBNull.Value);
                }
                Cmd.Parameters.AddWithValue("@RefPageTypeID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@RefPageTableID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@FinYearID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentType", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentNo", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentDate", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentBank", DBNull.Value);
                Cmd.Parameters.AddWithValue("@ApprovedDate", DBNull.Value);
                Cmd.Parameters.AddWithValue("@ApproveNote", DBNull.Value);
                Cmd.Parameters.AddWithValue("@Action", DBNull.Value);
                Cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@RefTransID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@CostCentreID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@MachineName", DBNull.Value);
                Cmd.Parameters.AddWithValue("@CommonNarration", DBNull.Value);
                Cmd.ExecuteNonQuery();
                Con.Close();
                Con.Open();
                SqlCommand cmd = new SqlCommand("Select Top  1 ID FROM FiTransactions ORDER BY ID DESC", Con);
                SqlDataReader LastInsertedID = cmd.ExecuteReader();
                long PanelID = 0;
                while (LastInsertedID.Read())
                {
                    PanelID = Convert.ToInt32(LastInsertedID["ID"]);

                }
                Con.Close();
                return PanelID.ToString();

            }
            catch (Exception Ex)
            {
                throw; // This will break and pass the error to the controller's try/catch
            }

        }
        public string UpdateTransactions(Models.FiTransactions FiTransaction)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactions");
                Cmd.Parameters.AddWithValue("@ID", FiTransaction.ID);
                Cmd.Parameters.AddWithValue("@Date", FiTransaction.Date);
                Cmd.Parameters.AddWithValue("@EffectiveDate", FiTransaction.EffectiveDate);
                Cmd.Parameters.AddWithValue("@VoucherID", FiTransaction.VoucherID);
                Cmd.Parameters.AddWithValue("@SerialNo", FiTransaction.SerialNo);
                Cmd.Parameters.AddWithValue("@TransactionNo", FiTransaction.TransactionNo);
                Cmd.Parameters.AddWithValue("@IsPostDated", FiTransaction.IsPostDated);
                Cmd.Parameters.AddWithValue("@CurrencyID", FiTransaction.CurrencyID);
                Cmd.Parameters.AddWithValue("@CompanyID", FiTransaction.CompanyID);
                Cmd.Parameters.AddWithValue("@ApprovalStatus", FiTransaction.ApprovalStatus);
                Cmd.Parameters.AddWithValue("@StatusID", FiTransaction.StatusID);
                Cmd.Parameters.AddWithValue("@Posted", FiTransaction.Posted);
                Cmd.Parameters.AddWithValue("@Active", FiTransaction.AddedBy);
                Cmd.Parameters.AddWithValue("@IsAutoEntry", FiTransaction.IsAutoEntry);
                Cmd.Parameters.AddWithValue("@Cancelled", FiTransaction.Cancelled);
                Cmd.Parameters.AddWithValue("@AddedBy", FiTransaction.AddedBy);
                Cmd.Parameters.AddWithValue("@ApprovedBy", DBNull.Value);
                Cmd.Parameters.AddWithValue("@PageID", FiTransaction.PageID);
                Cmd.Parameters.AddWithValue("@AddedDate", FiTransaction.AddedDate);
                if (FiTransaction.Description != null && FiTransaction.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", FiTransaction.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                if (FiTransaction.ReferenceNo != null && FiTransaction.ReferenceNo.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransaction.ReferenceNo);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ReferenceNo", DBNull.Value);
                }
                Cmd.Parameters.AddWithValue("@RefPageTypeID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@RefPageTableID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@FinYearID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentType", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentNo", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentDate", DBNull.Value);
                Cmd.Parameters.AddWithValue("@InstrumentBank", DBNull.Value);
                Cmd.Parameters.AddWithValue("@ApprovedDate", DBNull.Value);
                Cmd.Parameters.AddWithValue("@ApproveNote", DBNull.Value);
                Cmd.Parameters.AddWithValue("@Action", DBNull.Value);
                Cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@RefTransID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@CostCentreID", DBNull.Value);
                Cmd.Parameters.AddWithValue("@MachineName", DBNull.Value);
                Cmd.Parameters.AddWithValue("@CommonNarration", DBNull.Value);
                var Check = Cmd.ExecuteNonQuery();
                Con.Close();
                if (Check != null)
                {
                    return FiTransaction.ID.ToString();
                }
                else
                {
                    return "NULL";
                }

            }
            catch (Exception Ex)
            {
                throw; // This will break and pass the error to the controller's try/catch
            }

        }
        public string UpdateTransactionEntries(Models.FiTransactionEntries FiTransactionEntries)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactionEntries");
                Cmd.Parameters.AddWithValue("@ID", FiTransactionEntries.ID);
                Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionEntries.TransactionID);
                Cmd.Parameters.AddWithValue("@DrCr", FiTransactionEntries.DrCr);
                Cmd.Parameters.AddWithValue("@Nature", FiTransactionEntries.Nature);
                Cmd.Parameters.AddWithValue("@AccountID", FiTransactionEntries.AccountID);
                Cmd.Parameters.AddWithValue("@Amount", FiTransactionEntries.Amount);
                Cmd.Parameters.AddWithValue("@FCAmount", FiTransactionEntries.FCAmount);
                Cmd.Parameters.AddWithValue("@BankDate", FiTransactionEntries.BankDate);
                Cmd.Parameters.AddWithValue("@RefPageTypeID", FiTransactionEntries.RefPageTypeID);
                Cmd.Parameters.AddWithValue("@CurrencyID", FiTransactionEntries.CurrencyID);
                Cmd.Parameters.AddWithValue("@ExchangeRate", FiTransactionEntries.ExchangeRate);
                Cmd.Parameters.AddWithValue("@RefPageTableID", FiTransactionEntries.RefPageTableID);
                Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransactionEntries.ReferenceNo);
                Cmd.Parameters.AddWithValue("@TranType", FiTransactionEntries.TranType);
                Cmd.Parameters.AddWithValue("@DueDate", FiTransactionEntries.DueDate);
                Cmd.Parameters.AddWithValue("@RefTransID", FiTransactionEntries.RefTransID);
                Cmd.Parameters.AddWithValue("@TaxPerc", FiTransactionEntries.TaxPerc);
                if (FiTransactionEntries.Description != null && FiTransactionEntries.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", FiTransactionEntries.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                var Check = Cmd.ExecuteNonQuery();
                Con.Close();
                if (Check != null)
                {
                    return FiTransactionEntries.ID.ToString();
                }
                else
                {
                    return "NULL";
                }
            }
            catch (Exception Ex)
            {
                throw; // This will break and pass the error to the controller's try/catch
            }

        }
    }
}
