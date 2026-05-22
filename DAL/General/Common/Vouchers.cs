//using Microsoft.CodeAnalysis.Operations;
//using Microsoft.VisualStudio.Services.Profile;
using DocumentFormat.OpenXml.Office.Word;
using ERPSample.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
//using ERPSample.Models.Ecommerce.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Threading;
using static ERPSample.Models.Inventory.Transactions.TransactionHelperClass;
namespace ERPSample.DAL.General.Common
{
    public class Vouchers
    {
        String ConnectionString;
        public Vouchers(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public Object UserID { get; set; }

        public void DeleteVoucherAllocationByVID(Object VID, String ConnectionString)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "DeleteVoucherAllocationByVID");
                Cmd.Parameters.AddWithValue("@VID", VID);
                Cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public void FillMaVouchers(DataTable dtMaVouchers, Object VTypeID = null, Object PageID = null)
        {
            SqlConnection Con = null;
            try
            {
                dtMaVouchers.Clear();
                Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("LeftGridMasterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                if (PageID == null)
                {
                    Cmd.Parameters.AddWithValue("@Criteria", "FillMaVouchers");
                    Cmd.Parameters.AddWithValue("@ID", VTypeID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Criteria", "FillMaVouchersUsingPageID");
                    Cmd.Parameters.AddWithValue("@ID", PageID);
                }
                new SqlDataAdapter(Cmd).Fill(dtMaVouchers);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataRow FillVoucherRow(Object VTypeID = null, Object PageID = null)
        {
            SqlConnection Con = null;
            try
            {
                DataTable dtVoucherRow = new DataTable();
                Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("LeftGridMasterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                if (PageID == null)
                {
                    Cmd.Parameters.AddWithValue("@Criteria", "FillMaVouchers");
                    Cmd.Parameters.AddWithValue("@ID", VTypeID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Criteria", "FillMaVouchersUsingPageID");
                    Cmd.Parameters.AddWithValue("@ID", PageID);
                }
                new SqlDataAdapter(Cmd).Fill(dtVoucherRow);
                return dtVoucherRow.Rows[0];
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void Fill(DataSet dsVoucher, Object TransactionID, Boolean ClearBeforeFill = true)
        {
            try
            {
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "Fill");
                    Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    //sda.MissingSchemaAction = MissingSchemaAction.Ignore; 
                    if (ClearBeforeFill)
                    {
                        if (dsVoucher.EnforceConstraints)
                        {
                            dsVoucher.EnforceConstraints = false;
                        }
                        foreach (DataTable dt in dsVoucher.Tables)
                        {
                            if (!(dt.TableName == "dtMaster" || dt.TableName == "FiMaVouchers"))
                            {
                                dt.Clear();
                            }
                        }
                    }
                    sda.TableMappings.Add("Table", "FiTransactions");
                    sda.TableMappings.Add("Table1", "FiTransactionEntries");
                    sda.TableMappings.Add("Table2", "FiVoucherAllocation");
                    //FiJournalVoucherAllocation missing
                    sda.TableMappings.Add("Table3", "FiCheques");
                    sda.TableMappings.Add("Table4", "TransCollnAllocations");
                    sda.TableMappings.Add("Table5", "InvTransItems");
                    sda.TableMappings.Add("Table6", "InvTransItemDetails");
                    //sda.TableMappings.Add("Table7", "InvTransSubItems");
                    sda.TableMappings.Add("Table7", "TransItemExpenses");
                    sda.TableMappings.Add("Table8", "Documents");
                    sda.TableMappings.Add("Table9", "FiTransactionAdditionals");
                    sda.TableMappings.Add("Table10", "TransExpenses");
                    sda.TableMappings.Add("Table11", "DocumentRequests");
                    sda.TableMappings.Add("Table12", "DocumentReferences");
                    sda.TableMappings.Add("Table13", "TransReferences");
                    sda.TableMappings.Add("Table14", "TransLoadSchedules");
                    sda.TableMappings.Add("Table15", "TransCollections");
                    sda.TableMappings.Add("Table16", "TransEmployees");
                    sda.TableMappings.Add("Table17", "VMFuelLog");
                    sda.TableMappings.Add("Table18", "DocumentImages");
                    sda.TableMappings.Add("Table19", "HRFinalSettlement");
                    sda.TableMappings.Add("Table20", "TransCostAllocations");
                    sda.Fill(dsVoucher);
                    if (ClearBeforeFill && !dsVoucher.EnforceConstraints)
                    {
                        dsVoucher.EnforceConstraints = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        //Fill TransRefrence Table
        public void FillTransactionReferences(DataTable dtTransReferences, Object TransactionID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionReferences");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtTransReferences);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillVoucher(DataTable dtVoucher, Object BranchID, Object MaPageMenuID)
        {
            try
            {
                dtVoucher.Clear();
                if (ConnectionString == "") return;
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("LeftGridMasterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillVoucher");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@MaPageMenuID", MaPageMenuID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtVoucher);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillVoucher(Object BranchID, Object MaPageMenuID, Boolean Posted)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("LeftGridMasterSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "FillVoucher");
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@MaPageMenuID", MaPageMenuID);
                    Cmd.Parameters.AddWithValue("@Posted", Posted);
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillRecentVouchers(Object BranchID, Object MaPageMenuID, Boolean Posted)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("LeftGridMasterSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "FillRecentVouchers");
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@MaPageMenuID", MaPageMenuID);
                    Cmd.Parameters.AddWithValue("@Posted", Posted);
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillVoucher(DataTable dtVoucher, Object TransactionID)
        {

            try
            {
                dtVoucher.Clear();
                if (ConnectionString == "") return;
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillVoucherByTransactionID");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtVoucher);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillVoucher(DataTable dtVoucher, Object BranchID, Object MaPageMenuID, Object DateFrom, Object DateUpto, String VoucherNo = "", String PartyName = "", String Narration = "")
        {
            try
            {
                dtVoucher.Clear();
                if (ConnectionString == "") return;
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillVoucher");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@MaPageMenuID", MaPageMenuID);
                if (DateFrom != null && DateFrom.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                }
                if (DateUpto != null && DateUpto.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@DateUpto", DateUpto);
                }
                if (VoucherNo != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@TransactionNo", VoucherNo);
                }
                if (PartyName != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@PartyName", PartyName);
                }
                if (Narration != String.Empty)
                {
                    Cmd.Parameters.AddWithValue("@Narration", Narration);
                }
                new SqlDataAdapter(Cmd).Fill(dtVoucher);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillMaVoucherPurchaseVoucher(DataTable dtMaVouchers, Object VoucherName)
        {
            SqlConnection Con = null;
            try
            {
                dtMaVouchers.Clear();
                Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("FillMaVouchersSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                //Cmd.Parameters.AddWithValue("@Criteria", "FillMaVoucherPurchaseVoucher");
                Cmd.Parameters.AddWithValue("@VoucherName", VoucherName);
                new SqlDataAdapter(Cmd).Fill(dtMaVouchers);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransactions(DataTable dtTransactions, Object ID)
        {
            try
            {
                dtTransactions.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactions");
                Cmd.Parameters.AddWithValue("@ID", ID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtTransactions);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransactions(DataTable dtTransactions, Object ID, Object VoucherName)
        {
            try
            {
                dtTransactions.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionsPaymentorReceipt");
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.Parameters.AddWithValue("@VoucherName", VoucherName);
                new SqlDataAdapter(Cmd).Fill(dtTransactions);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransactions(DataTable dtTransactions, Object ID, Int32 VoucherID)
        {
            try
            {
                dtTransactions.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionsUsingRefTransIDandVoucID");
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                new SqlDataAdapter(Cmd).Fill(dtTransactions);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillParties(Object AccountID)
        {
            try
            {
                DataTable dtTransactions = new DataTable();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillPartyDetails");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                new SqlDataAdapter(Cmd).Fill(dtTransactions);
                return dtTransactions;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public DataTable FillEmailID(Object Category)
        {
            try
            {
                DataTable dtTransactions = new DataTable();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillEmailID");
                Cmd.Parameters.AddWithValue("@Category", Category);
                SqlDataAdapter Adp = new SqlDataAdapter(Cmd);
                Adp.Fill(dtTransactions);
                return dtTransactions;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillAttendance(DataTable dtAttendance, Object ID)
        {
            try
            {
                dtAttendance.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillAttendance");
                Cmd.Parameters.AddWithValue("@ID", ID);
                new SqlDataAdapter(Cmd).Fill(dtAttendance);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransactionEntries(DataTable dtTransactionEntries, Object TransactionID)
        {
            try
            {
                dtTransactionEntries.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionEntries");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtTransactionEntries);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransCriteria(DataTable dtTransCriteria, Object TransactionID)
        {
            try
            {
                dtTransCriteria.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransCriteria");
                new SqlDataAdapter(Cmd).Fill(dtTransCriteria);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillInvBatchWiseItems(DataTable dtInvBatchWiseItems, Object TransactionID)
        {
            try
            {
                dtInvBatchWiseItems.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvBatchWiseItems");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtInvBatchWiseItems.Load(sdr);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public DataTable FillAccounts(String ConnectionString, int BranchID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@Criteria", "FillAccounts");
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillDocumentType()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillDocumentType");
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillPrimaryVoucherDetails(object VoucherID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillPrimaryVoucherDetails");
                Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillDefaultCashDetails()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillDefaultCashDetails");
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillVoucherAllocation(DataTable dtVoucherAllocation, Object VID, Object DrCr)
        {
            try
            {
                dtVoucherAllocation.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillVoucherAllocation");
                Cmd.Parameters.AddWithValue("@VID", VID);
                Cmd.Parameters.AddWithValue("@DrCr", DrCr);
                new SqlDataAdapter(Cmd).Fill(dtVoucherAllocation);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransCollnAllocations(DataTable dtTransCollnAllocations, Object VID)
        {
            try
            {
                dtTransCollnAllocations.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransCollnAllocations");
                Cmd.Parameters.AddWithValue("@VID", VID);
                new SqlDataAdapter(Cmd).Fill(dtTransCollnAllocations);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransEmployees(DataTable dtTransEmployees, Object VID)
        {
            try
            {
                dtTransEmployees.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransEmployees");
                Cmd.Parameters.AddWithValue("@VID", VID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtTransEmployees.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransReminders(DataTable dtTransReminders, Object VID)
        {
            try
            {
                dtTransReminders.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VehicleSp", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransReminders");
                Cmd.Parameters.AddWithValue("@ID", VID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtTransReminders.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillVMFuelLog(DataTable dtVMFuelLog, Object VID)
        {
            try
            {
                dtVMFuelLog.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillVMFuelLog");
                Cmd.Parameters.AddWithValue("@VID", VID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtVMFuelLog.Load(sdr);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransexpenses(DataTable dtTransexpenses, Object VID)
        {
            try
            {
                dtTransexpenses.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransexpenses");
                Cmd.Parameters.AddWithValue("@VID", VID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtTransexpenses.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransactionExpenses(DataTable dtTransexpenses, Object TransactionID)
        {
            try
            {
                dtTransexpenses.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionExpenses");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtTransexpenses);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillVoucherAllocationUsingRef(DataTable dtVoucherAllocation, Object VID, Object DrCr)
        {
            SqlConnection Con = null;
            try
            {
                dtVoucherAllocation.Clear();
                if (dtVoucherAllocation.Constraints.Contains("FiTransactions_FiVoucherAllocation")) dtVoucherAllocation.Constraints.Remove(dtVoucherAllocation.Constraints["FiTransactions_FiVoucherAllocation"]);
                Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillVoucherAllocationUsingRef");
                Cmd.Parameters.AddWithValue("@VID", VID);
                Cmd.Parameters.AddWithValue("@DrCr", DrCr);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtVoucherAllocation);
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        //public void FillLotAllocation(DataTable dtLotAllocation, Object VID)
        //{
        //    try
        //    {
        //    dtLotAllocation.Clear();
        //    SqlConnection Con = new SqlConnection(ConnectionString);
        //    Con.Open();
        //    SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
        //    Cmd.CommandType = CommandType.StoredProcedure;
        //    Cmd.Parameters.AddWithValue("@Criteria", "FillLotAllocation");
        //    Cmd.Parameters.AddWithValue("@VID", VID);
        //    SqlDataReader sdr = Cmd.ExecuteReader();
        //    dtLotAllocation.Load(sdr);
        //    sdr.Close();
        //    Con.Close();
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

        public DataTable FillVoucherAllocation(int AccountID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                // dtBillWise.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("BillWiseStmtSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@DateFrom", FromDate);
                Cmd.Parameters.AddWithValue("@DateUpto", ToDate);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                //dtBillWise.Load(sdr);
                //sdr.Close();
                //Con.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillJournalVouchers(DataTable dtVoucherAllocation, Object TransactionID)
        {
            try
            {
                dtVoucherAllocation.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillJournalVouchers");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtVoucherAllocation.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillJournalVoucherUsingRef(DataTable dtVoucherAllocation, Object TransactionID)
        {
            try
            {
                dtVoucherAllocation.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillJournalVoucherUsingRef");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtVoucherAllocation.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransactionAdditionals(DataTable dtFiTransactionAdditionals, Object TransactionID)
        {
            try
            {
                dtFiTransactionAdditionals.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillFiTransactionAdditionals");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtFiTransactionAdditionals);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillPackDetails(object TransItemID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillPackDetails");
                Cmd.Parameters.AddWithValue("@TransItemID", TransItemID);
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

        public void FillInvTransItems(DataTable dtInvTransItems, Object TransactionID)
        {
            try
            {
                dtInvTransItems.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvTransItems");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtInvTransItems);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillSRVServiceTrans(DataTable dtSRVServiceTrans, Object TransactionID)
        {
            try
            {
                dtSRVServiceTrans.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("SRVServiceSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", 1);
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.MissingSchemaAction = MissingSchemaAction.Ignore;
                sda.Fill(dtSRVServiceTrans);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        /// <summary>
        /// Fill ItemMaster for Cutting Voucher
        /// </summary>
        public void FillInvItemMaster(DataTable dtInvItemMaster, Object TransactionID)
        {
            try
            {
                dtInvItemMaster.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvItemMaster");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtInvItemMaster.Load(sdr);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillInvTransItemDetails(DataTable dtInvTransItems, Object TransactionID)
        {
            try
            {
                dtInvTransItems.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvTransItemDetails");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtInvTransItems.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransItemExpenses(DataTable dtInvTransItemExpenses, Object TransactionID)
        {
            try
            {
                dtInvTransItemExpenses.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionItemExpenses");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                //SqlDataReader sdr = Cmd.ExecuteReader();
                //dtInvTransItems.Load(sdr);
                //sdr.Close();
                SqlDataAdapter adr = new SqlDataAdapter(Cmd);
                adr.Fill(dtInvTransItemExpenses);
                Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillInvTransSubItems(DataTable dtInvTransItems, Object ItemID)
        {
            SqlConnection Con = null;
            try
            {
                dtInvTransItems.Clear();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvTransSubItems");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                SqlDataAdapter Adp = new SqlDataAdapter(Cmd);
                Adp.Fill(dtInvTransItems);
                Con.Close();
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public void FillInvTransSubItemsByTransactionID(DataTable dtInvTransItems, Object TransactionID)
        {
            SqlConnection Con = null;
            try
            {
                dtInvTransItems.Clear();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvTransSubItemsByTransactionID");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.Fill(dtInvTransItems);
                Con.Close();
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public void FillTransCollection(DataTable dtTransCollection, Object TransactionID)
        {
            SqlConnection Con = null;
            try
            {
                dtTransCollection.Clear();
                Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransCollections");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                new SqlDataAdapter(Cmd).Fill(dtTransCollection);
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public void FillInvSubItemMaster(DataTable dtInvTransItems, Object ItemID)
        {
            try
            {
                dtInvTransItems.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvSubItemMaster");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtInvTransItems.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillTransLoadSchedules(DataTable dtTransLoadSchedules, Object TransactionID)
        {
            SqlConnection Con = null;
            try
            {
                dtTransLoadSchedules.Clear();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("[VoucherAdditionalsSP]", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransLoadSchedules");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.Fill(dtTransLoadSchedules);
                Con.Close();
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        // TransExpense from Summary
        public void FillBankExpenseFromSummary(DataTable dtTransExpenses, Object TansactionID)
        {
            try
            {
                dtTransExpenses.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransactionExpenses");
                Cmd.Parameters.AddWithValue("@TransactionID", TansactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtTransExpenses.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillJournals(int BranchID, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillJournals");
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

        public void FillHRTimeSheetDetails(DataTable dtHRTimesheetDetails, Object TansactionID)
        {
            try
            {
                dtHRTimesheetDetails.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillHRTimeSheetDetails");
                Cmd.Parameters.AddWithValue("@TransactionID", TansactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtHRTimesheetDetails.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillHRSalary(DataTable dtHRSalary, Object TansactionID, Object AccountID)
        {
            try
            {
                dtHRSalary.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillHRSalary");
                Cmd.Parameters.AddWithValue("@TransactionID", TansactionID);
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtHRSalary.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillHRSalaryInCome(DataTable dtHRSalaryInCome, Object TansactionID)
        {
            try
            {
                dtHRSalaryInCome.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillHRSalaryInCome");
                Cmd.Parameters.AddWithValue("@SID", TansactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtHRSalaryInCome.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void FillHRSalaryDeduction(DataTable dtHRSalaryDeduction, Object TansactionID)
        {
            try
            {
                dtHRSalaryDeduction.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillHRSalaryDeduction");
                Cmd.Parameters.AddWithValue("@SID", TansactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtHRSalaryDeduction.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public String GetBasicVTypeByVID(Object VID, String ConnectionString)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBasicVTypeByVID");
                Cmd.Parameters.AddWithValue("@ID", VID);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output.ToString();
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public DataTable GetBillWiseDetails(Object AccountID, Object BranchID, Object DrCr)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBillWiseDetails");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@DrCr", DrCr);
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

        public DataTable GetBillWise(Object AccountID, Object BranchID, Object DrCr, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBillWise");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@DrCr", DrCr);
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

        public DataTable GetBillsAndRefs(Object AccountID, Object BranchID, Object DrCr, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                // Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBillsAndRefs");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@DrCr", DrCr);
                //SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //DataTable dt = new DataTable();
                //dt.Load(sdr);
                SqlDataAdapter adp = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);


                //sdr.Close();
                //Con.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable GetIDFromMaMiscusingDevCode()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetIDFromMaMiscusingDevCode");
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dt = new DataTable();
                dt.Load(sdr);
                //SqlDataAdapter adp = new SqlDataAdapter(Cmd);
                //DataTable dt = new DataTable();
                //adp.Fill(dt);


                //sdr.Close();
                //Con.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable GetBillsAndRefsForEdit(Object AccountID, Object TransactionID, Object DrCr, String ConnectionString)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetBillsAndRefsForEdit");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                Cmd.Parameters.AddWithValue("@DrCr", DrCr);
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

        public DataTable FillInvItemDetails()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvItemDetails");
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

        public DataTable FillUnits()
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillUnitMaster");
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

        public bool PagePermission(object UserID, Object PageID, Object BranchID, string PageMode)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                bool IsRights = false;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "PermissionChecking");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@PageID", PageID);
                Cmd.Parameters.AddWithValue("@PageMode", PageMode);
                IsRights = Convert.ToBoolean(Cmd.ExecuteScalar());
                Con.Close();
                return IsRights;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetTransactionNo(Object VoucherID, Object BranchID)
        {
            try
            {
                Object TransactionNo = 0;
                // Use 'using' to ensure connection is properly disposed of after the operation
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    Con.Open();
                    SqlCommand Cmd = new SqlCommand("GetNextVoucherNoSP", Con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                    // Execute the scalar query and get the result
                    TransactionNo = Cmd.ExecuteScalar();
                    Con.Close();//Added By Rafi on 15/11/2025
                }
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                // Exception handling (re-throwing in this case)
                throw Ex;
            }
        }


        public Boolean IsVoucherNumberExist(Object BranchID, Object VoucherID, String VoucherNo)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = 0;
                SqlCommand Cmd = new SqlCommand("GetNextVoucherNoSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "IsVoucherNumberExist");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                Cmd.Parameters.AddWithValue("@VoucherNo", VoucherNo);
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return Convert.ToBoolean(TransactionNo);
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetAutoItemCode()
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = 0;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetAutoItemCode");
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetAutoPartyCode(String Nature = "")
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = 0;
                SqlCommand Cmd = new SqlCommand("PartyMasterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetAutoPartyCode");
                if (Nature != "")
                {
                    Cmd.Parameters.AddWithValue("@Nature", Nature);
                }
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetLatestVoucherDate(Object UserID, Object BranchID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = 0;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetLatestVoucherDate");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetCreditPeriodofParty(object PartyID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = 0;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetCreditPeriodofParty");
                Cmd.Parameters.AddWithValue("@AccountID", PartyID);
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetLocInTransit()
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object LocInTransit = 0;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetLocInTransit");
                LocInTransit = Cmd.ExecuteScalar();
                Con.Close();
                return LocInTransit;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object RefTransEntryID(Object ChequeID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object RefTransEntryID = null;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "RefTransEntryID");
                Cmd.Parameters.AddWithValue("@ChequeID", ChequeID);
                RefTransEntryID = Cmd.ExecuteScalar();
                Con.Close();
                return RefTransEntryID;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object FindAccountID(Object RefTransEntryID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object AccountID = null;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FindAccountID");
                Cmd.Parameters.AddWithValue("@VEID", RefTransEntryID);
                AccountID = Cmd.ExecuteScalar();
                Con.Close();
                return AccountID;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public void FillTransCostAllocaions(DataTable dtTransCostAllocation, Object TransEntryID, Object TransactionID)
        {
            try
            {
                dtTransCostAllocation.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("CostCentreSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillTransCostAllocations");
                Cmd.Parameters.AddWithValue("@VEID", TransEntryID);
                Cmd.Parameters.AddWithValue("@ID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtTransCostAllocation.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// Save TransEmployee
        /// </summary>
        /// <param name="dtTransEmployees"></param>
        /// 
        public void SaveTransEmployees(DataTable dtTransEmployees, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransEmployees.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransEmployees");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@HREmployeeID", dr["HREmployeeID"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@Note", dr["Note"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransEmployees");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@HREmployeeID", dr["HREmployeeID"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@Note", dr["Note"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransEmployees");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveVMFuelLog(DataTable dtVMFuelLog, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtVMFuelLog.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertVMFuelLog");
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Reading", dr["Reading"]);
                    Cmd.Parameters.AddWithValue("@Note", dr["Note"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateVMFuelLog");
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Reading", dr["Reading"]);
                    Cmd.Parameters.AddWithValue("@Note", dr["Note"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteVMFuelLog");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveTransReminders(DataTable dtTransReminders, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransReminders.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransReminders");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@ExpiryValue", dr["ExpiryValue"]);
                    Cmd.Parameters.AddWithValue("@RemindOn", dr["RemindOn"]);
                    Cmd.Parameters.AddWithValue("@Note", dr["Note"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransReminders");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@ExpiryValue", dr["ExpiryValue"]);
                    Cmd.Parameters.AddWithValue("@RemindOn", dr["RemindOn"]);
                    Cmd.Parameters.AddWithValue("@Note", dr["Note"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VehicleSp", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransReminders");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }

        }

        public void SaveDocumentImages(DataTable dtDocumentImages, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtDocumentImages.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("DocumentsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertDocumentImages");
                    Cmd.Parameters.AddWithValue("@DocID", dr["DocID"]);
                    Cmd.Parameters.AddWithValue("@Path", dr["Path"]);
                    Cmd.Parameters.AddWithValue("@FileName", dr["FileName"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@DocFormatID", dr["DocFormatID"]);
                    Cmd.Parameters.AddWithValue("@DocStatusID", dr["DocStatusID"]);
                    Cmd.Parameters.AddWithValue("@VersionNo", dr["VersionNo"]);
                    Cmd.Parameters.AddWithValue("@SourceID", dr["SourceID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("DocumentsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateDocumentImages");
                    Cmd.Parameters.AddWithValue("@DocID", dr["DocID"]);
                    Cmd.Parameters.AddWithValue("@Path", dr["Path"]);
                    Cmd.Parameters.AddWithValue("@FileName", dr["FileName"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@DocFormatID", dr["DocFormatID"]);
                    Cmd.Parameters.AddWithValue("@DocStatusID", dr["DocStatusID"]);
                    Cmd.Parameters.AddWithValue("@VersionNo", dr["VersionNo"]);
                    Cmd.Parameters.AddWithValue("@SourceID", dr["SourceID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("DocumentsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteDocumentImages");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }

        }

        public void SaveHRFinalSettlement(DataTable dtHRFinalSettlement, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHRFinalSettlement.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertHRFinalSettlement");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@Type", dr["Type"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@FromDate", dr["FromDate"]);
                    Cmd.Parameters.AddWithValue("@ToDate", dr["ToDate"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@GrDays", dr["GrDays"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateHRFinalSettlement");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@Type", dr["Type"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@FromDate", dr["FromDate"]);
                    Cmd.Parameters.AddWithValue("@ToDate", dr["ToDate"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@GrDays", dr["GrDays"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteHRFinalSettlement");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveHRTimeSheetDetails(DataTable dtHRTimesheetDetails, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHRTimesheetDetails.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertHRTimeSheetDetails");
                    Cmd.Parameters.AddWithValue("@TID", dr["TID"]);
                    Cmd.Parameters.AddWithValue("@ProjectID", dr["ProjectID"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@OT", dr["OT"]);
                    Cmd.Parameters.AddWithValue("@FOT", dr["FOT"]);
                    Cmd.Parameters.AddWithValue("@HOT", dr["HOT"]);
                    Cmd.Parameters.AddWithValue("@Bonus", dr["Bonus"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateHRTimeSheetDetails");
                    Cmd.Parameters.AddWithValue("@TID", dr["TID"]);
                    Cmd.Parameters.AddWithValue("@ProjectID", dr["ProjectID"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@OT", dr["OT"]);
                    Cmd.Parameters.AddWithValue("@FOT", dr["FOT"]);
                    Cmd.Parameters.AddWithValue("@HOT", dr["HOT"]);
                    Cmd.Parameters.AddWithValue("@Bonus", dr["Bonus"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteHRTimeSheetDetails");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveHRSalary(DataTable dtHRSalary, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHRSalary.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertHRSalary");
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Month", dr["Month"]);
                    Cmd.Parameters.AddWithValue("@Year", dr["Year"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@FromDate", dr["FromDate"]);
                    Cmd.Parameters.AddWithValue("@ToDate", dr["ToDate"]);
                    Cmd.Parameters.AddWithValue("@TotalHours", dr["TotalHours"]);
                    Cmd.Parameters.AddWithValue("@TotalRate", dr["TotalRate"]);
                    Cmd.Parameters.AddWithValue("@Gross", dr["Gross"]);
                    Cmd.Parameters.AddWithValue("@Deduction", dr["Deduction"]);
                    Cmd.Parameters.AddWithValue("@NetSalary", dr["NetSalary"]);
                    Cmd.Parameters.AddWithValue("@DaysWorked", dr["DaysWorked"]);
                    Cmd.Parameters.AddWithValue("@TotalDays", dr["TotalDays"]);
                    Cmd.Parameters.AddWithValue("@BasicSalary", dr["BasicSalary"]);
                    Cmd.Parameters.AddWithValue("@TotalAllowance", dr["TotalAllowance"]);
                    Cmd.Parameters.AddWithValue("@RefNo", dr["RefNo"]);
                    Cmd.Parameters.AddWithValue("@Advance", dr["Advance"]);
                    Cmd.Parameters.AddWithValue("@PrintStatus", dr["PrintStatus"]);
                    Cmd.Parameters.AddWithValue("@PostID", dr["PostID"]);
                    Cmd.Parameters.AddWithValue("@PostNetID", dr["PostNetID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@SalaryDate", dr["SalaryDate"]);
                    Cmd.Parameters.AddWithValue("@PaymentModeID", dr["PaymentModeID"]);
                    Cmd.Parameters.AddWithValue("@Leave", dr["Leave"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateHRSalary");
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Month", dr["Month"]);
                    Cmd.Parameters.AddWithValue("@Year", dr["Year"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@FromDate", dr["FromDate"]);
                    Cmd.Parameters.AddWithValue("@ToDate", dr["ToDate"]);
                    Cmd.Parameters.AddWithValue("@TotalHours", dr["TotalHours"]);
                    Cmd.Parameters.AddWithValue("@TotalRate", dr["TotalRate"]);
                    Cmd.Parameters.AddWithValue("@Gross", dr["Gross"]);
                    Cmd.Parameters.AddWithValue("@Deduction", dr["Deduction"]);
                    Cmd.Parameters.AddWithValue("@NetSalary", dr["NetSalary"]);
                    Cmd.Parameters.AddWithValue("@DaysWorked", dr["DaysWorked"]);
                    Cmd.Parameters.AddWithValue("@TotalDays", dr["TotalDays"]);
                    Cmd.Parameters.AddWithValue("@BasicSalary", dr["BasicSalary"]);
                    Cmd.Parameters.AddWithValue("@TotalAllowance", dr["TotalAllowance"]);
                    Cmd.Parameters.AddWithValue("@RefNo", dr["RefNo"]);
                    Cmd.Parameters.AddWithValue("@Advance", dr["Advance"]);
                    Cmd.Parameters.AddWithValue("@PrintStatus", dr["PrintStatus"]);
                    Cmd.Parameters.AddWithValue("@PostID", dr["PostID"]);
                    Cmd.Parameters.AddWithValue("@PostNetID", dr["PostNetID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@SalaryDate", dr["SalaryDate"]);
                    Cmd.Parameters.AddWithValue("@PaymentModeID", dr["PaymentModeID"]);
                    Cmd.Parameters.AddWithValue("@Leave", dr["Leave"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteHRSalary");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveHRSalaryInCome(DataTable dtHRSalaryIncome, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHRSalaryIncome.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertHRSalaryInCome");
                    Cmd.Parameters.AddWithValue("@SID", dr["SID"]);
                    Cmd.Parameters.AddWithValue("@Particulars", dr["Particulars"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@DaysWorked", dr["DaysWorked"]);
                    Cmd.Parameters.AddWithValue("@Numbers", dr["Numbers"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Actual", dr["Actual"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateHRSalaryInCome");
                    Cmd.Parameters.AddWithValue("@SID", dr["SID"]);
                    Cmd.Parameters.AddWithValue("@Particulars", dr["Particulars"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@DaysWorked", dr["DaysWorked"]);
                    Cmd.Parameters.AddWithValue("@Numbers", dr["Numbers"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Actual", dr["Actual"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteHRSalaryInCome");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveHRSalaryDeduction(DataTable dtHRSalaryDeduction, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHRSalaryDeduction.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertHRSalaryDeduction");
                    Cmd.Parameters.AddWithValue("@SID", dr["SID"]);
                    Cmd.Parameters.AddWithValue("@DeductionID", dr["DeductionID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateHRSalaryDeduction");
                    Cmd.Parameters.AddWithValue("@SID", dr["SID"]);
                    Cmd.Parameters.AddWithValue("@DeductionID", dr["DeductionID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteHRSalaryDeduction");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveInvBatchWiseItems(DataTable dtInvBatchWiseItems, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtInvBatchWiseItems.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertInvBatchWiseItems");
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["InvTransItemID"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@BatchNo", dr["BatchNo"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@Pcs", dr["Pcs"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["BranchID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvBatchWiseItems");
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["InvTransItemID"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@BatchNo", dr["BatchNo"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@Pcs", dr["Pcs"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["BranchID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvBatchWiseItems");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveGeneralFindings(DataTable dtGeneralFindings, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtGeneralFindings.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {

                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertGeneralFindings");

                    Cmd.Parameters.AddWithValue("@Name", dr["Name"]);
                    Cmd.Parameters.AddWithValue("@Value", dr["Value"]);
                    Cmd.Parameters.AddWithValue("@Range", dr["Range"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);

                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateGeneralFindings");
                    Cmd.Parameters.AddWithValue("@Name", dr["Name"]);
                    Cmd.Parameters.AddWithValue("@Value", dr["Value"]);
                    Cmd.Parameters.AddWithValue("@Range", dr["Range"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteGeneralFindings");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveScanning(DataTable dtScanning, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtScanning.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {

                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertScanning");

                    Cmd.Parameters.AddWithValue("@ScanId", dr["ScanId"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransactionID", dr["RefTransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@Result", dr["Result"]);
                    Cmd.Parameters.AddWithValue("@Cost", dr["Cost"]);

                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateScanning");
                    Cmd.Parameters.AddWithValue("@ScanId", dr["ScanId"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransactionID", dr["RefTransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@Result", dr["Result"]);
                    Cmd.Parameters.AddWithValue("@Cost", dr["Cost"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteScanning");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveLabTest(DataTable dtLabTest, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtLabTest.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {

                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertLabTest");

                    Cmd.Parameters.AddWithValue("@LabTestId", dr["LabTestId"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransactionID", dr["RefTransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@Result", dr["Result"]);
                    Cmd.Parameters.AddWithValue("@Price", dr["Price"]);
                    Cmd.Parameters.AddWithValue("@Reference", dr["Reference"]);

                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateLabTest");
                    Cmd.Parameters.AddWithValue("@LabTestId", dr["LabTestId"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransactionID", dr["RefTransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@Result", dr["Result"]);
                    Cmd.Parameters.AddWithValue("@Price", dr["Price"]);
                    Cmd.Parameters.AddWithValue("@Reference", dr["Reference"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteLabTest");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveConsultationDetails(DataTable dtHMSConsultationDetails, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHMSConsultationDetails.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    //ID, , , , , , , , , , , , , , , , , , , , , TransactionID
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertConsultingDetails");

                    Cmd.Parameters.AddWithValue("@Inpatient", dr["Inpatient"]);
                    Cmd.Parameters.AddWithValue("@Outpatient", dr["Outpatient"]);
                    Cmd.Parameters.AddWithValue("@EmergencyCase", dr["EmergencyCase"]);
                    Cmd.Parameters.AddWithValue("@DurationOfIllness", dr["DurationOfIllness"]);
                    Cmd.Parameters.AddWithValue("@PrincipalCode", dr["PrincipalCode"]);
                    Cmd.Parameters.AddWithValue("@2ndCode", dr["2ndCode"]);
                    Cmd.Parameters.AddWithValue("@3rdCode", dr["3rdCode"]);
                    Cmd.Parameters.AddWithValue("@4thCode", dr["4thCode"]);
                    Cmd.Parameters.AddWithValue("@Chronic", dr["Chronic"]);
                    Cmd.Parameters.AddWithValue("@Congential", dr["Congential"]);
                    Cmd.Parameters.AddWithValue("@RTA", dr["RTA"]);
                    Cmd.Parameters.AddWithValue("@WorkRelated", dr["WorkRelated"]);
                    Cmd.Parameters.AddWithValue("@Vaccination", dr["Vaccination"]);
                    Cmd.Parameters.AddWithValue("@CheckUp", dr["CheckUp"]);
                    Cmd.Parameters.AddWithValue("@Psychiatric", dr["Psychiatric"]);
                    Cmd.Parameters.AddWithValue("@Infertility", dr["Infertility"]);
                    Cmd.Parameters.AddWithValue("@Pregnancy", dr["Pregnancy"]);
                    Cmd.Parameters.AddWithValue("@SportsRelated", dr["SportsRelated"]);
                    Cmd.Parameters.AddWithValue("@Cleaning", dr["Cleaning"]);
                    Cmd.Parameters.AddWithValue("@Others", dr["Others"]);
                    Cmd.Parameters.AddWithValue("@BP", dr["BP"]);
                    Cmd.Parameters.AddWithValue("@Pulse", dr["Pulse"]);
                    Cmd.Parameters.AddWithValue("@Temperature", dr["Temperature"]);
                    Cmd.Parameters.AddWithValue("@WalkIn", dr["WalkIn"]);
                    Cmd.Parameters.AddWithValue("@Referral", dr["Referral"]);
                    Cmd.Parameters.AddWithValue("@NewVisit", dr["NewVisit"]);
                    Cmd.Parameters.AddWithValue("@FollowUp", dr["FollowUp"]);
                    Cmd.Parameters.AddWithValue("@CmfYes", dr["CmfYes"]);
                    Cmd.Parameters.AddWithValue("@CmfNo", dr["CmfNo"]);


                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);

                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateConsultingDetails");

                    Cmd.Parameters.AddWithValue("@Inpatient", dr["Inpatient"]);
                    Cmd.Parameters.AddWithValue("@Outpatient", dr["Outpatient"]);
                    Cmd.Parameters.AddWithValue("@EmergencyCase", dr["EmergencyCase"]);
                    Cmd.Parameters.AddWithValue("@DurationOfIllness", dr["DurationOfIllness"]);
                    Cmd.Parameters.AddWithValue("@PrincipalCode", dr["PrincipalCode"]);
                    Cmd.Parameters.AddWithValue("@2ndCode", dr["2ndCode"]);
                    Cmd.Parameters.AddWithValue("@3rdCode", dr["3rdCode"]);
                    Cmd.Parameters.AddWithValue("@4thCode", dr["4thCode"]);
                    Cmd.Parameters.AddWithValue("@Chronic", dr["Chronic"]);
                    Cmd.Parameters.AddWithValue("@Congential", dr["Congential"]);
                    Cmd.Parameters.AddWithValue("@RTA", dr["RTA"]);
                    Cmd.Parameters.AddWithValue("@WorkRelated", dr["WorkRelated"]);
                    Cmd.Parameters.AddWithValue("@Vaccination", dr["Vaccination"]);
                    Cmd.Parameters.AddWithValue("@CheckUp", dr["CheckUp"]);
                    Cmd.Parameters.AddWithValue("@Psychiatric", dr["Psychiatric"]);
                    Cmd.Parameters.AddWithValue("@Infertility", dr["Infertility"]);
                    Cmd.Parameters.AddWithValue("@Pregnancy", dr["Pregnancy"]);
                    Cmd.Parameters.AddWithValue("@SportsRelated", dr["SportsRelated"]);
                    Cmd.Parameters.AddWithValue("@Cleaning", dr["Cleaning"]);
                    Cmd.Parameters.AddWithValue("@Others", dr["Others"]);
                    Cmd.Parameters.AddWithValue("@BP", dr["BP"]);
                    Cmd.Parameters.AddWithValue("@Pulse", dr["Pulse"]);
                    Cmd.Parameters.AddWithValue("@Temperature", dr["Temperature"]);
                    Cmd.Parameters.AddWithValue("@WalkIn", dr["WalkIn"]);
                    Cmd.Parameters.AddWithValue("@Referral", dr["Referral"]);
                    Cmd.Parameters.AddWithValue("@NewVisit", dr["NewVisit"]);
                    Cmd.Parameters.AddWithValue("@FollowUp", dr["FollowUp"]);
                    Cmd.Parameters.AddWithValue("@CmfYes", dr["CmfYes"]);
                    Cmd.Parameters.AddWithValue("@CmfNo", dr["CmfNo"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteConsultingDetails");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveToothDetails(DataTable dtHMSToothDetails, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHMSToothDetails.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {

                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertToothDetails");

                    Cmd.Parameters.AddWithValue("@Code", dr["Code"]);
                    Cmd.Parameters.AddWithValue("@DescriptionorService", dr["DescriptionorService"]);
                    Cmd.Parameters.AddWithValue("@Quantity", dr["Quantity"]);
                    Cmd.Parameters.AddWithValue("@ToothNo", dr["ToothNo"]);
                    Cmd.Parameters.AddWithValue("@Cost", dr["Cost"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);

                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateToothDetails");

                    Cmd.Parameters.AddWithValue("@Code", dr["Code"]);
                    Cmd.Parameters.AddWithValue("@DescriptionorService", dr["DescriptionorService"]);
                    Cmd.Parameters.AddWithValue("@Quantity", dr["Quantity"]);
                    Cmd.Parameters.AddWithValue("@ToothNo", dr["ToothNo"]);
                    Cmd.Parameters.AddWithValue("@Cost", dr["Cost"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);

                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteToothDetails");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveEyeTest(DataTable dtHMSEyeTest, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtHMSEyeTest.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {

                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertEyeTest");

                    Cmd.Parameters.AddWithValue("@RSphere", dr["RSphere"]);
                    Cmd.Parameters.AddWithValue("@RCylinder", dr["RCylinder"]);
                    Cmd.Parameters.AddWithValue("@RAxis", dr["RAxis"]);
                    Cmd.Parameters.AddWithValue("@RPrism", dr["RPrism"]);
                    Cmd.Parameters.AddWithValue("@RVA", dr["RVA"]);
                    Cmd.Parameters.AddWithValue("@LSphere", dr["LSphere"]);
                    Cmd.Parameters.AddWithValue("@LCylinder", dr["LCylinder"]);
                    Cmd.Parameters.AddWithValue("@LAxis", dr["LAxis"]);
                    Cmd.Parameters.AddWithValue("@LPrism", dr["LPrism"]);
                    Cmd.Parameters.AddWithValue("@LVA", dr["LVA"]);
                    Cmd.Parameters.AddWithValue("@PD", dr["PD"]);
                    Cmd.Parameters.AddWithValue("@Type", dr["Type"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);

                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateEyeTest");

                    Cmd.Parameters.AddWithValue("@RSphere", dr["RSphere"]);
                    Cmd.Parameters.AddWithValue("@RCylinder", dr["RCylinder"]);
                    Cmd.Parameters.AddWithValue("@RAxis", dr["RAxis"]);
                    Cmd.Parameters.AddWithValue("@RPrism", dr["RPrism"]);
                    Cmd.Parameters.AddWithValue("@RVA", dr["RVA"]);
                    Cmd.Parameters.AddWithValue("@LSphere", dr["LSphere"]);
                    Cmd.Parameters.AddWithValue("@LCylinder", dr["LCylinder"]);
                    Cmd.Parameters.AddWithValue("@LAxis", dr["LAxis"]);
                    Cmd.Parameters.AddWithValue("@LPrism", dr["LPrism"]);
                    Cmd.Parameters.AddWithValue("@LVA", dr["LVA"]);
                    Cmd.Parameters.AddWithValue("@PD", dr["PD"]);
                    Cmd.Parameters.AddWithValue("@Type", dr["Type"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);

                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("HMSDoctorConsultationSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteEyeTest");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void Save(DataSet dsVoucher, SqlConnection Con = null, SqlTransaction Tx = null)
        {
            Boolean CommitTransaction = false;
            try
            {
                if (Con == null || Tx == null)
                {
                    Con = new SqlConnection(ConnectionString);
                    Con.Open();
                    Tx = Con.BeginTransaction();
                    CommitTransaction = true;
                }
                SaveTransactions(dsVoucher.Tables["FiTransactions"], Con, Tx);
                if (dsVoucher.Tables.Contains("FiTransactionEntries") && dsVoucher.Tables["FiTransactionEntries"].Rows.Count > 0)
                {
                    SaveTransactionEntries(dsVoucher.Tables["FiTransactionEntries"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiVoucherAllocation") && dsVoucher.Tables["FiVoucherAllocation"].Rows.Count > 0)
                {
                    SaveVoucherAllocation(dsVoucher.Tables["FiVoucherAllocation"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiJournalVoucherAllocation") && dsVoucher.Tables["FiJournalVoucherAllocation"].Rows.Count > 0)
                {
                    SaveVoucherAllocation(dsVoucher.Tables["FiJournalVoucherAllocation"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiCheques") && dsVoucher.Tables["FiCheques"].Rows.Count > 0)
                {
                    SaveCheques(dsVoucher.Tables["FiCheques"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiChequesTran") && dsVoucher.Tables["FiChequesTran"].Rows.Count > 0)
                {
                    SaveFiChequesTran(dsVoucher.Tables["FiChequesTran"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCollnAllocations") && dsVoucher.Tables["TransCollnAllocations"].Rows.Count > 0)
                {
                    SaveTransCollnAllocations(dsVoucher.Tables["TransCollnAllocations"], Con, Tx);
                }
                // SaveLotAllocation(dsVoucher.Tables["InvLotAllocation"], Con, Tx);

                // Don't change this order.TransReference must be inserted before InvTransitems. For Delivery Note and Sales Invoice.
                if (dsVoucher.Tables.Contains("TransReferences") && dsVoucher.Tables["TransReferences"].Rows.Count > 0)
                {
                    SaveTransReferences(dsVoucher.Tables["TransReferences"], Con, Tx);
                }

                if (dsVoucher.Tables.Contains("InvTransItems") && dsVoucher.Tables["InvTransItems"].Rows.Count > 0)
                {
                    SaveInvTransItems(dsVoucher.Tables["InvTransItems"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("SRVServiceTrans") && dsVoucher.Tables["SRVServiceTrans"].Rows.Count > 0)
                {
                    SaveSRVServiceTrans(dsVoucher.Tables["SRVServiceTrans"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvTransItemDetails") && dsVoucher.Tables["InvTransItemDetails"].Rows.Count > 0)
                {
                    SaveInvTransItemDetails(dsVoucher.Tables["InvTransItemDetails"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvSubItemMaster") && dsVoucher.Tables["InvSubItemMaster"].Rows.Count > 0)
                {
                    SaveInvSubItemMaster(dsVoucher.Tables["InvSubItemMaster"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvTransSubItems") && dsVoucher.Tables["InvTransSubItems"].Rows.Count > 0)
                {
                    SaveInvTransSubItems(dsVoucher.Tables["InvTransSubItems"], Con, Tx);
                }

                // Save Item Expenses before Additionals fro Purchase AvgStock update.
                if (dsVoucher.Tables.Contains("TransItemExpenses") && dsVoucher.Tables["TransItemExpenses"].Rows.Count > 0)
                {
                    SaveTransItemExpenses(dsVoucher.Tables["TransItemExpenses"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("Documents") && dsVoucher.Tables["Documents"].Rows.Count > 0)
                {
                    SaveDocuments(dsVoucher.Tables["Documents"], Con, Tx);
                }

                //if (dsVoucher.Tables.Contains("DocumentImages"))
                //{
                //    SaveAttach(dsVoucher.Tables["DocumentImages"], Con, Tx);
                //}
                // Save Batchwise item details before VoucherAdditionals.
                if (dsVoucher.Tables.Contains("InvBatchWiseItems") && dsVoucher.Tables["InvBatchWiseItems"].Rows.Count > 0)
                {
                    SaveInvBatchWiseItems(dsVoucher.Tables["InvBatchWiseItems"], Con, Tx);
                }

                if (dsVoucher.Tables.Contains("FiTransactionAdditionals") && dsVoucher.Tables["FiTransactionAdditionals"].Rows.Count > 0)
                {
                    SaveAdditionals(dsVoucher.Tables["FiTransactionAdditionals"], Con, Tx);//please dont change this order(it will cause looping problem)
                }
                if (dsVoucher.Tables.Contains("TransExpenses") && dsVoucher.Tables["TransExpenses"].Rows.Count > 0)
                {
                    SaveTransactionExpenses(dsVoucher.Tables["TransExpenses"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("DocumentRequests") && dsVoucher.Tables["DocumentRequests"].Rows.Count > 0)
                {
                    SaveDocumentRequests(dsVoucher.Tables["DocumentRequests"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("DocumentReferences") && dsVoucher.Tables["DocumentReferences"].Rows.Count > 0)
                {
                    SaveDocumentReferences(dsVoucher.Tables["DocumentReferences"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCriteria") && dsVoucher.Tables["TransCriteria"].Rows.Count > 0)
                {
                    SaveTransCriteria(dsVoucher.Tables["TransCriteria"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransLoadSchedules") && dsVoucher.Tables["TransLoadSchedules"].Rows.Count > 0)
                {
                    SaveTransLoadSchedules(dsVoucher.Tables["TransLoadSchedules"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCollections") && dsVoucher.Tables["TransCollections"].Rows.Count > 0)
                {
                    saveTransCollections(dsVoucher.Tables["TransCollections"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCostAllocations") && dsVoucher.Tables["TransCostAllocations"].Rows.Count > 0)
                {
                    SaveTransCostAllocations(dsVoucher.Tables["TransCostAllocations"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransEmployees") && dsVoucher.Tables["TransEmployees"].Rows.Count > 0)
                {
                    SaveTransEmployees(dsVoucher.Tables["TransEmployees"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("VMFuelLog") && dsVoucher.Tables["VMFuelLog"].Rows.Count > 0)
                {
                    SaveVMFuelLog(dsVoucher.Tables["VMFuelLog"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransReminders") && dsVoucher.Tables["TransReminders"].Rows.Count > 0)
                {
                    SaveTransReminders(dsVoucher.Tables["TransReminders"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("DocumentImages") && dsVoucher.Tables["DocumentImages"].Rows.Count > 0)
                {
                    SaveDocumentImages(dsVoucher.Tables["DocumentImages"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HRFinalSettlement") && dsVoucher.Tables["HRFinalSettlement"].Rows.Count > 0)
                {
                    SaveHRFinalSettlement(dsVoucher.Tables["HRFinalSettlement"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HRTimeSheetDetails") && dsVoucher.Tables["HRTimeSheetDetails"].Rows.Count > 0)
                {
                    SaveHRTimeSheetDetails(dsVoucher.Tables["HRTimeSheetDetails"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HRSalary") && dsVoucher.Tables["HRSalary"].Rows.Count > 0)
                {
                    SaveHRSalary(dsVoucher.Tables["HRSalary"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HRSalaryIncome") && dsVoucher.Tables["HRSalaryIncome"].Rows.Count > 0)
                {
                    SaveHRSalaryInCome(dsVoucher.Tables["HRSalaryIncome"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HRSalaryDeduction") && dsVoucher.Tables["HRSalaryDeduction"].Rows.Count > 0)
                {
                    SaveHRSalaryDeduction(dsVoucher.Tables["HRSalaryDeduction"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvBatchWiseItems") && dsVoucher.Tables["InvBatchWiseItems"].Rows.Count > 0)
                {
                    SaveInvBatchWiseItems(dsVoucher.Tables["InvBatchWiseItems"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("dtHMSGeneralFindings") && dsVoucher.Tables["dtHMSGeneralFindings"].Rows.Count > 0)
                {
                    SaveGeneralFindings(dsVoucher.Tables["dtHMSGeneralFindings"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HMSScanning") && dsVoucher.Tables["HMSScanning"].Rows.Count > 0)
                {
                    SaveScanning(dsVoucher.Tables["HMSScanning"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HMSLabTest") && dsVoucher.Tables["HMSLabTest"].Rows.Count > 0)
                {
                    SaveLabTest(dsVoucher.Tables["HMSLabTest"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HMSConsultationDetails") && dsVoucher.Tables["HMSConsultationDetails"].Rows.Count > 0)
                {
                    SaveConsultationDetails(dsVoucher.Tables["HMSConsultationDetails"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HMSToothDetails") && dsVoucher.Tables["HMSToothDetails"].Rows.Count > 0)
                {
                    SaveToothDetails(dsVoucher.Tables["HMSToothDetails"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("HMSEyeTest") && dsVoucher.Tables["HMSEyeTest"].Rows.Count > 0)
                {
                    SaveEyeTest(dsVoucher.Tables["HMSEyeTest"], Con, Tx);
                }
                if (dsVoucher.Tables["FiTransactions"].Rows.Count > 0)
                {
                    //////////////User Track Updation//////////////////////////////////////////
                    DataRow dr = dsVoucher.Tables["FiTransactions"].Rows[0];
                    String _Reason = "";
                    Int16 _Action = -1;
                    decimal _Amount = 0;
                    String _RefNo = "";
                    Object _RowID = "";
                    Object _UserID;
                    if (UserID != null) { _UserID = UserID; } else { _UserID = null; }
                    String _MachineName = "";
                    if (dr.RowState == DataRowState.Added)
                    {
                        _RowID = dr["ID"];
                        _RefNo = dr["TransactionNo"].ToString();
                        _Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                        _Action = 0;
                        _Reason = "Added";
                        _UserID = dr["AddedBy"];
                    }
                    else if (dr.RowState == DataRowState.Modified)
                    {
                        _RowID = dr["ID"];
                        _RefNo = dr["TransactionNo"].ToString();
                        _Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                        _Action = 1;
                        _Reason = dr["Description"].ToString();
                        _UserID = dr["EditedBy"];
                    }
                    else if (dr.RowState == DataRowState.Deleted)
                    {
                        _RowID = dr["ID", DataRowVersion.Original];
                        _RefNo = dr["TransactionNo", DataRowVersion.Original].ToString();
                        foreach (DataRow row in dsVoucher.Tables["FiTransactionEntries"].Rows)
                        {
                            if (row["Debit", DataRowVersion.Original] == DBNull.Value)
                            {
                                _Amount += 0;
                            }
                            else
                            {
                                _Amount += Convert.ToDecimal(row["Debit", DataRowVersion.Original]);
                            }
                        }
                        //_Amount = Convert.ToDecimal(dr["Amount", DataRowVersion.Original]);
                        _Action = 2;
                        _Reason = dsVoucher.Tables["UserTrack"].Rows[0]["Reason"].ToString();
                        _UserID = dsVoucher.Tables["UserTrack"].Rows[0]["UserID"];
                    }

                    /*if (System.Web.HttpContext.Current == null)
                    {
                        _MachineName = Environment.MachineName;
                    }
                    else
                    {
                        _MachineName = System.Web.HttpContext.Current.Request.UserHostAddress;
                    }*/
                    /*new DAL.General.Common.Permission().UpdateUserTrack(_UserID, "FiTransactions", DateTime.Now, _Reason, _Action, _RowID, _MachineName, dsVoucher.Tables["FiMaVouchers"].Rows[0]["Name"].ToString(), _RefNo, _Amount, Con, Tx);*/
                    //////////////User Track Updation//////////////////////////////////////////
                }

                if (dsVoucher.Tables["FiTransactions"].Rows.Count > 0 && dsVoucher.Tables["FiTransactions"].Rows[0].RowState != DataRowState.Deleted)
                {
                    EntriesAmountValidation(dsVoucher.Tables["FiTransactions"].Rows[0]["ID"], Con, Tx);
                }
                if (CommitTransaction)
                {
                    Tx.Commit();
                }

                /////////////Sends email to approver asynchronously///
                SendEmailToApprovers(dsVoucher);

                //////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                if (CommitTransaction)
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Tx.Rollback();
                    }
                }
                throw ex;
            }
            finally
            {
                if (CommitTransaction)
                {
                    Con.Close();
                }
            }
        }

        private DataSet DataSetVoucher { get; set; }

        public void SendEmailToApprovers(DataSet dsVoucher)
        {
            DataSetVoucher = dsVoucher;
            //ConnectionString = ConnectionString;

            Thread thread = new Thread(new ThreadStart(SendEmail));
            thread.IsBackground = true;
            thread.Name = "SendEmail";
            thread.Start();
        }

        public void EntriesAmountValidation(Object TransactionID, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd = new SqlCommand("EntriesAmountCheck", Con, Tx);
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
            Cmd.ExecuteNonQuery();
        }

        private void SendEmail()
        {
            try
            {
                SendEmailToApprover(DataSetVoucher.Tables["FiTransactions"].Rows[0]["ID"]);
            }
            catch (Exception)
            {
            }
        }

        public void SaveBy(DataSet dsVoucher, SqlConnection Con, SqlTransaction Tx)
        {
            try
            {
                SaveTransactions(dsVoucher.Tables["FiTransactions"], Con, Tx);
                if (dsVoucher.Tables.Contains("FiTransactionEntries"))
                {
                    SaveTransactionEntries(dsVoucher.Tables["FiTransactionEntries"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiVoucherAllocation"))
                {
                    SaveVoucherAllocation(dsVoucher.Tables["FiVoucherAllocation"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiJournalVoucherAllocation"))
                {
                    SaveVoucherAllocation(dsVoucher.Tables["FiJournalVoucherAllocation"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiCheques"))
                {
                    SaveCheques(dsVoucher.Tables["FiCheques"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiChequesTran"))
                {
                    SaveFiChequesTran(dsVoucher.Tables["FiChequesTran"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCollnAllocations"))
                {
                    SaveTransCollnAllocations(dsVoucher.Tables["TransCollnAllocations"], Con, Tx);
                }
                // SaveLotAllocation(dsVoucher.Tables["InvLotAllocation"], Con, Tx);

                if (dsVoucher.Tables.Contains("InvTransItems"))
                {
                    SaveInvTransItems(dsVoucher.Tables["InvTransItems"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvTransItemDetails"))
                {
                    SaveInvTransItemDetails(dsVoucher.Tables["InvTransItemDetails"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvSubItemMaster"))
                {
                    SaveInvSubItemMaster(dsVoucher.Tables["InvSubItemMaster"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("InvTransSubItems"))
                {
                    SaveInvTransSubItems(dsVoucher.Tables["InvTransSubItems"], Con, Tx);
                }

                // Save Item Expenses before Additionals fro Purchase AvgStock update.
                if (dsVoucher.Tables.Contains("TransItemExpenses"))
                {
                    SaveTransItemExpenses(dsVoucher.Tables["TransItemExpenses"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("Documents"))
                {
                    SaveDocuments(dsVoucher.Tables["Documents"], Con, Tx);
                }

                if (dsVoucher.Tables.Contains("DocumentImages"))
                {
                    SaveAttach(dsVoucher.Tables["DocumentImages"], Con, Tx);
                }
                // Save Batchwise item details before VoucherAdditionals.
                if (dsVoucher.Tables.Contains("InvBatchWiseItems"))
                {
                    SaveInvBatchWiseItems(dsVoucher.Tables["InvBatchWiseItems"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("FiTransactionAdditionals"))
                {
                    SaveAdditionals(dsVoucher.Tables["FiTransactionAdditionals"], Con, Tx);//please dont change this order(it will cause looping problem)
                }
                if (dsVoucher.Tables.Contains("TransExpenses"))
                {
                    SaveTransactionExpenses(dsVoucher.Tables["TransExpenses"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("DocumentRequests"))
                {
                    SaveDocumentRequests(dsVoucher.Tables["DocumentRequests"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("DocumentReferences"))
                {
                    SaveDocumentReferences(dsVoucher.Tables["DocumentReferences"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCriteria"))
                {
                    SaveTransCriteria(dsVoucher.Tables["TransCriteria"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransReferences"))
                {
                    SaveTransReferences(dsVoucher.Tables["TransReferences"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransLoadSchedules"))
                {

                    //SaveTransItemExpenses(dsVoucher.Tables["TransItemExpenses"], Con, Tx);  This has been move out from IF condition
                    SaveTransLoadSchedules(dsVoucher.Tables["TransLoadSchedules"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("TransCollections"))
                {
                    saveTransCollections(dsVoucher.Tables["TransCollections"], Con, Tx);
                }

                if (dsVoucher.Tables.Contains("TransEmployees"))
                {
                    SaveTransEmployees(dsVoucher.Tables["TransEmployees"], Con, Tx);
                }
                if (dsVoucher.Tables.Contains("VMFuelLog"))
                {
                    SaveVMFuelLog(dsVoucher.Tables["VMFuelLog"], Con, Tx);
                }

                if (dsVoucher.Tables.Contains("TransCostAllocations"))
                {
                    SaveTransCostAllocations(dsVoucher.Tables["TransCostAllocations"], Con, Tx);
                }

                if (dsVoucher.Tables.Contains("TransReminders"))
                {
                    SaveTransReminders(dsVoucher.Tables["TransReminders"], Con, Tx);
                }
                //////////////User Track Updation//////////////////////////////////////////
                DataRow dr = dsVoucher.Tables["FiTransactions"].Rows[0];
                String _Reason = "";
                Int16 _Action = -1;
                decimal _Amount = 0;
                String _RefNo = "";
                Object _RowID = "";
                if (dr.RowState == DataRowState.Added)
                {
                    _RowID = dr["ID"];
                    _RefNo = dr["TransactionNo"].ToString();
                    _Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                    _Action = 0;
                    _Reason = "Added";
                }
                else if (dr.RowState == DataRowState.Modified)
                {
                    _RowID = dr["ID"];
                    _RefNo = dr["TransactionNo"].ToString();
                    _Amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                    _Action = 1;
                    _Reason = dr["Description"].ToString();
                }
                else if (dr.RowState == DataRowState.Deleted)
                {
                    _RowID = dr["ID", DataRowVersion.Original];
                    _RefNo = dr["TransactionNo", DataRowVersion.Original].ToString();
                    foreach (DataRow row in dsVoucher.Tables["FiTransactionEntries"].Rows)
                    {
                        if (row["Debit", DataRowVersion.Original] == DBNull.Value)
                        {
                            _Amount += 0;
                        }
                        else
                        {
                            _Amount += Convert.ToDecimal(row["Debit", DataRowVersion.Original]);
                        }
                    }
                    //_Amount = Convert.ToDecimal(dr["Amount", DataRowVersion.Original]);
                    _Action = 2;
                    _Reason = dsVoucher.Tables["UserTrack"].Rows[0]["Reason"].ToString();
                }
                //15-09-2021 --   Uncomment on session 
                //new DAL.General.Common.Permission().UpdateUserTrack(System.Web.HttpContext.Current.Session["intUserID"], "FiTransactions", DateTime.Now, _Reason, _Action, _RowID, System.Web.HttpContext.Current.Request.UserHostAddress, dsVoucher.Tables["FiMaVouchers"].Rows[0]["Name"].ToString(), _RefNo, _Amount, Con, Tx);
                //////////////User Track Updation//////////////////////////////////////////
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateApprovalDetails(DataSet dsVoucher)
        {
            SqlConnection Con = null;
            SqlTransaction Tx = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Tx = Con.BeginTransaction();
                UpdateApprovalDetails(dsVoucher, Con, Tx);
                Tx.Commit();
            }
            catch (Exception)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Tx.Rollback();
                }
                throw;
            }
            finally
            {
                Con.Close();
            }
        }

        public void SaveApprovalDetails(DataSet dsVoucher)
        {
            SqlConnection Con = null;
            SqlTransaction Tx = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Tx = Con.BeginTransaction();
                SaveApprovalDetails(dsVoucher.Tables["FiTransactions"], Con, Tx);
                Tx.Commit();
            }
            catch (Exception)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Tx.Rollback();
                }
                throw;
            }
            finally
            {
                Con.Close();
            }
        }

        // Save vouchers with InvItemMaster.
        public void SaveDetails(SqlConnection Con, SqlTransaction tx, DataSet dsVoucher)
        {
            Con = new SqlConnection(ConnectionString);
            Con.Open();
            tx = Con.BeginTransaction();
            SqlCommand Cmd;
            foreach (DataRow dr in dsVoucher.Tables["InvItemMaster"].Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("ItemMasterSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "Insert");
                    Cmd.Parameters.AddWithValue("@ItemCode", dr["ItemCode"]);
                    Cmd.Parameters.AddWithValue("@ItemName", dr["ItemName"]);
                    Cmd.Parameters.AddWithValue("@SellingPrice", dr["SellingPrice"]);
                    Cmd.Parameters.AddWithValue("@OEMNo", dr["OEMNo"]);
                    Cmd.Parameters.AddWithValue("@PartNo", dr["PartNo"]);
                    Cmd.Parameters.AddWithValue("@CategoryID", dr["CategoryID"]);
                    Cmd.Parameters.AddWithValue("@Manufacturer", dr["Manufacturer"]);
                    Cmd.Parameters.AddWithValue("@BarCode", dr["BarCode"]);
                    Cmd.Parameters.AddWithValue("@ModelNo", dr["ModelNo"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@ROL", dr["ROL"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@IsGroup", dr["IsGroup"]);
                    Cmd.Parameters.AddWithValue("@StockItem", dr["StockItem"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@ParentID", dr["ParentID"]);
                    Cmd.Parameters.AddWithValue("@InvAccountID", dr["InvAccountID"]);
                    Cmd.Parameters.AddWithValue("@CostAccountID", dr["CostAccountID"]);
                    Cmd.Parameters.AddWithValue("@PurchaseAccountID", dr["PurchaseAccountID"]);
                    Cmd.Parameters.AddWithValue("@SalesAccountID", dr["SalesAccountID"]);
                    Cmd.Parameters.AddWithValue("@CreatedDate", dr["CreatedDate"]);
                    Cmd.Parameters.AddWithValue("@CreatedUserID", dr["CreatedUserID"]);
                    Cmd.Parameters.AddWithValue("@Location", dr["Location"]);
                    Cmd.Parameters.AddWithValue("@CashPrice", dr["CashPrice"]);
                    Cmd.Parameters.AddWithValue("@CreditPrice", dr["CreditPrice"]);
                    Cmd.Parameters.AddWithValue("@ROQ", dr["ROQ"]);
                    Cmd.Parameters.AddWithValue("@CommodityID", dr["CommodityID"]);
                    Cmd.Parameters.AddWithValue("@ShipMark", dr["ShipMark"]);
                    Cmd.Parameters.AddWithValue("@PaintMark", dr["PaintMark"]);
                    Cmd.Parameters.AddWithValue("@QualityID", dr["QualityID"]);
                    Cmd.Parameters.AddWithValue("@Weight", dr["Weight"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["BranchID"]);
                    Cmd.Parameters.AddWithValue("@AvgCost", dr["AvgCost"]);
                    Cmd.Parameters.AddWithValue("@IsUniqueItem", dr["IsUniqueItem"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("ItemMasterSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "Update");
                    Cmd.Parameters.AddWithValue("@ItemCode", dr["ItemCode"]);
                    Cmd.Parameters.AddWithValue("@ItemName", dr["ItemName"]);
                    Cmd.Parameters.AddWithValue("@SellingPrice", dr["SellingPrice"]);
                    Cmd.Parameters.AddWithValue("@OEMNo", dr["OEMNo"]);
                    Cmd.Parameters.AddWithValue("@PartNo", dr["PartNo"]);
                    Cmd.Parameters.AddWithValue("@CategoryID", dr["CategoryID"]);
                    Cmd.Parameters.AddWithValue("@Manufacturer", dr["Manufacturer"]);
                    Cmd.Parameters.AddWithValue("@BarCode", dr["BarCode"]);
                    Cmd.Parameters.AddWithValue("@ModelNo", dr["ModelNo"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@ROL", dr["ROL"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@IsGroup", dr["IsGroup"]);
                    Cmd.Parameters.AddWithValue("@StockItem", dr["StockItem"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@ParentID", dr["ParentID"]);
                    Cmd.Parameters.AddWithValue("@InvAccountID", dr["InvAccountID"]);
                    Cmd.Parameters.AddWithValue("@CostAccountID", dr["CostAccountID"]);
                    Cmd.Parameters.AddWithValue("@PurchaseAccountID", dr["PurchaseAccountID"]);
                    Cmd.Parameters.AddWithValue("@SalesAccountID", dr["SalesAccountID"]);
                    Cmd.Parameters.AddWithValue("@ModifiedDate", dr["ModifiedDate"]);
                    Cmd.Parameters.AddWithValue("@ModifiedUserID", dr["ModifiedUserID"]);
                    Cmd.Parameters.AddWithValue("@Location", dr["Location"]);
                    Cmd.Parameters.AddWithValue("@CashPrice", dr["CashPrice"]);
                    Cmd.Parameters.AddWithValue("@CreditPrice", dr["CreditPrice"]);
                    Cmd.Parameters.AddWithValue("@ROQ", dr["ROQ"]);
                    Cmd.Parameters.AddWithValue("@CommodityID", dr["CommodityID"]);
                    Cmd.Parameters.AddWithValue("@ShipMark", dr["ShipMark"]);
                    Cmd.Parameters.AddWithValue("@PaintMark", dr["PaintMark"]);
                    Cmd.Parameters.AddWithValue("@QualityID", dr["QualityID"]);
                    Cmd.Parameters.AddWithValue("@Weight", dr["Weight"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["BranchID"]);
                    Cmd.Parameters.AddWithValue("@AvgCost", dr["AvgCost"]);
                    Cmd.Parameters.AddWithValue("@IsUniqueItem", dr["IsUniqueItem"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
            SaveBy(dsVoucher, Con, tx);

            foreach (DataRow dr in dsVoucher.Tables["InvItemMaster"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("ItemMasterSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "Delete");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
            tx.Commit();
        }

        public void FillCheques(DataTable dtCheques, Object TransactionID)
        {
            try
            {
                dtCheques.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillCheques");
                Cmd.Parameters.AddWithValue("@ID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.Fill(dtCheques);
                Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private void SaveTransactions(DataTable dtTransactions, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransactions.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactions");
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@EffectiveDate", dr["EffectiveDate"]);
                    Cmd.Parameters.AddWithValue("@VoucherID", dr["VoucherID"]);
                    Cmd.Parameters.AddWithValue("@SerialNo", dr["SerialNo"]);
                    Cmd.Parameters.AddWithValue("@TransactionNo", dr["TransactionNo"]);
                    Cmd.Parameters.AddWithValue("@IsPostDated", dr["IsPostDated"]);
                    Cmd.Parameters.AddWithValue("@CurrencyID", dr["CurrencyID"]);
                    Cmd.Parameters.AddWithValue("@PageID", dr["PageID"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate", dr["ExchangeRate"]);
                    Cmd.Parameters.AddWithValue("@RefPageTypeID", dr["RefPageTypeID"]);
                    Cmd.Parameters.AddWithValue("@RefPageTableID", dr["RefPageTableID"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@CompanyID", dr["CompanyID"]);
                    Cmd.Parameters.AddWithValue("@FinYearID", dr["FinYearID"]);
                    Cmd.Parameters.AddWithValue("@InstrumentType", dr["InstrumentType"]);
                    Cmd.Parameters.AddWithValue("@InstrumentNo", dr["InstrumentNo"]);
                    Cmd.Parameters.AddWithValue("@InstrumentDate", dr["InstrumentDate"]);
                    Cmd.Parameters.AddWithValue("@InstrumentBank", dr["InstrumentBank"]);
                    Cmd.Parameters.AddWithValue("@CommonNarration", dr["CommonNarration"]);
                    Cmd.Parameters.AddWithValue("@AddedBy", dr["AddedBy"]);
                    Cmd.Parameters.AddWithValue("@ApprovedBy", dr["ApprovedBy"]);
                    Cmd.Parameters.AddWithValue("@AddedDate", dr["AddedDate"]);
                    Cmd.Parameters.AddWithValue("@ApprovedDate", dr["ApprovedDate"]);
                    Cmd.Parameters.AddWithValue("@ApprovalStatus", dr["ApprovalStatus"]);
                    Cmd.Parameters.AddWithValue("@ApproveNote", dr["ApproveNote"]);
                    Cmd.Parameters.AddWithValue("@Action", dr["Action"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    //Cmd.Parameters.AddWithValue("@StatusID", dr["StatusID"]);
                    Cmd.Parameters.AddWithValue("@IsAutoEntry", dr["IsAutoEntry"]);
                    Cmd.Parameters.AddWithValue("@Posted", dr["Posted"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@Cancelled", dr["Cancelled"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@CheckCount", dr["CheckCount"]);
                    Cmd.Parameters.AddWithValue("@ApprovalDays", dr["ApprovalDays"]);
                    Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                    Cmd.Parameters.AddWithValue("@CostCentreID", dr["CostCentreID"]);
                    Cmd.Parameters.AddWithValue("@MachineName", dr["MachineName"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactions");
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@EffectiveDate", dr["EffectiveDate"]);
                    Cmd.Parameters.AddWithValue("@VoucherID", dr["VoucherID"]);
                    Cmd.Parameters.AddWithValue("@SerialNo", dr["SerialNo"]);
                    Cmd.Parameters.AddWithValue("@TransactionNo", dr["TransactionNo"]);
                    Cmd.Parameters.AddWithValue("@IsPostDated", dr["IsPostDated"]);
                    Cmd.Parameters.AddWithValue("@CurrencyID", dr["CurrencyID"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate", dr["ExchangeRate"]);
                    Cmd.Parameters.AddWithValue("@RefPageTypeID", dr["RefPageTypeID"]);
                    Cmd.Parameters.AddWithValue("@RefPageTableID", dr["RefPageTableID"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@CompanyID", dr["CompanyID"]);
                    Cmd.Parameters.AddWithValue("@FinYearID", dr["FinYearID"]);
                    Cmd.Parameters.AddWithValue("@InstrumentType", dr["InstrumentType"]);
                    Cmd.Parameters.AddWithValue("@InstrumentNo", dr["InstrumentNo"]);
                    Cmd.Parameters.AddWithValue("@InstrumentDate", dr["InstrumentDate"]);
                    Cmd.Parameters.AddWithValue("@InstrumentBank", dr["InstrumentBank"]);
                    Cmd.Parameters.AddWithValue("@CommonNarration", dr["CommonNarration"]);
                    Cmd.Parameters.AddWithValue("@AddedBy", dr["AddedBy"]);
                    Cmd.Parameters.AddWithValue("@ApprovedBy", dr["ApprovedBy"]);
                    Cmd.Parameters.AddWithValue("@AddedDate", dr["AddedDate"]);
                    Cmd.Parameters.AddWithValue("@ApprovedDate", dr["ApprovedDate"]);
                    Cmd.Parameters.AddWithValue("@ApprovalStatus", dr["ApprovalStatus"]);
                    Cmd.Parameters.AddWithValue("@ApproveNote", dr["ApproveNote"]);
                    Cmd.Parameters.AddWithValue("@Action", dr["Action"]);
                    Cmd.Parameters.AddWithValue("@StatusID", dr["StatusID"]);
                    Cmd.Parameters.AddWithValue("@IsAutoEntry", dr["IsAutoEntry"]);
                    Cmd.Parameters.AddWithValue("@Posted", dr["Posted"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@Cancelled", dr["Cancelled"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@CheckCount", dr["CheckCount"]);
                    Cmd.Parameters.AddWithValue("@ApprovalDays", dr["ApprovalDays"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                    Cmd.Parameters.AddWithValue("@CostCentreID", dr["CostCentreID"]);
                    Cmd.Parameters.AddWithValue("@PageID", dr["PageID"]);
                    Cmd.Parameters.AddWithValue("@MachineName", dr["MachineName"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactions");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveTransactionEntries(DataTable dtTransactionEntries, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransactionEntries.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactionEntries");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DrCr", dr["DrCr"]);
                    Cmd.Parameters.AddWithValue("@Nature", dr["Nature"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["DrCr"].ToString() == "D" ? dr["Debit"] : dr["Credit"]);
                    Cmd.Parameters.AddWithValue("@FCAmount", dr["FCAmount"]);
                    Cmd.Parameters.AddWithValue("@BankDate", dr["BankDate"]);
                    Cmd.Parameters.AddWithValue("@CurrencyID", dr["CurrencyID"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate", dr["ExchangeRate"]);
                    Cmd.Parameters.AddWithValue("@RefPageTypeID", dr["RefPageTypeID"]);
                    Cmd.Parameters.AddWithValue("@RefPageTableID", dr["RefPageTableID"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@TaxPerc", dr["TaxPerc"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactionEntries");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DrCr", dr["DrCr"]);
                    Cmd.Parameters.AddWithValue("@Nature", dr["Nature"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["DrCr"].ToString() == "D" ? dr["Debit"] : dr["Credit"]);
                    Cmd.Parameters.AddWithValue("@FCAmount", dr["FCAmount"]);
                    Cmd.Parameters.AddWithValue("@BankDate", dr["BankDate"]);
                    Cmd.Parameters.AddWithValue("@CurrencyID", dr["CurrencyID"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate", dr["ExchangeRate"]);
                    Cmd.Parameters.AddWithValue("@RefPageTypeID", dr["RefPageTypeID"]);
                    Cmd.Parameters.AddWithValue("@RefPageTableID", dr["RefPageTableID"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@TaxPerc", dr["TaxPerc"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactionEntries");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveVoucherAllocation(DataTable dtVoucherAllocation, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtVoucherAllocation.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertVoucherAllocation");
                    Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateVoucherAllocation");
                    Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteVoucherAllocation");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveTransCollnAllocations(DataTable dtTransCollnAllocations, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransCollnAllocations.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransCollnAllocations");
                    Cmd.Parameters.AddWithValue("@TransCollectionID", dr["TransCollectionID"]);
                    Cmd.Parameters.AddWithValue("@VAllocationID", dr["VAllocationID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "TransCollnAllocations_ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransCollnAllocations");
                    Cmd.Parameters.AddWithValue("@TransCollectionID", dr["TransCollectionID"]);
                    Cmd.Parameters.AddWithValue("@VAllocationID", dr["VAllocationID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransCollnAllocations");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }

        }

        private void SaveCheques(DataTable dtCheques, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtCheques.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertCheques");
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@CardType", dr["CardType"]);
                    Cmd.Parameters.AddWithValue("@Commission", dr["Commission"]);
                    Cmd.Parameters.AddWithValue("@ChequeNo", dr["ChequeNo"]);
                    Cmd.Parameters.AddWithValue("@ChequeDate", dr["ChequeDate"]);
                    Cmd.Parameters.AddWithValue("@ClrDays", dr["ClrDays"]);
                    Cmd.Parameters.AddWithValue("@BankID", dr["BankID"]);
                    Cmd.Parameters.AddWithValue("@BankName", dr["BankName"]);
                    Cmd.Parameters.AddWithValue("@PartyID", dr["PartyID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateCheques");
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@CardType", dr["CardType"]);
                    Cmd.Parameters.AddWithValue("@Commission", dr["Commission"]);
                    Cmd.Parameters.AddWithValue("@ChequeNo", dr["ChequeNo"]);
                    Cmd.Parameters.AddWithValue("@ChequeDate", dr["ChequeDate"]);
                    Cmd.Parameters.AddWithValue("@ClrDays", dr["ClrDays"]);
                    Cmd.Parameters.AddWithValue("@BankID", dr["BankID"]);
                    Cmd.Parameters.AddWithValue("@BankName", dr["BankName"]);
                    Cmd.Parameters.AddWithValue("@PartyID", dr["PartyID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteCheques");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveFiChequesTran(DataTable dtFiChequesTran, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;

            foreach (DataRow dr in dtFiChequesTran.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertFiChequesTran");
                    Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@ChequeID", dr["ChequeID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateFiChequesTran");
                    Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@ChequeID", dr["ChequeID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteFiChequesTran");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveApprovalDetails(DataTable dtTransactions, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransactions.Rows)
            {
                Cmd = new SqlCommand("VoucherSP", Con, tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "ApprovalUpdate");
                Cmd.Parameters.AddWithValue("@ApprovedBy", dr["ApprovedBy"]);
                Cmd.Parameters.AddWithValue("@ApprovedDate", dr["ApprovedDate"]);
                Cmd.Parameters.AddWithValue("@ApprovalStatus", dr["ApprovalStatus"]);
                Cmd.Parameters.AddWithValue("@ApproveNote", dr["ApproveNote"]);
                Cmd.Parameters.AddWithValue("@StatusID", dr["StatusID"]);
                Cmd.Parameters.AddWithValue("@RefPageTypeID", dr["RefPageTypeID"]);
                Cmd.Parameters.AddWithValue("@Posted", dr["Posted"]);
                //Cmd.Parameters.AddWithValue("@XmlApproval", ApprovalDetails);
                Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                Cmd.ExecuteNonQuery();
                //Console.WriteLine(Cmd.Parameters["@NewID"].Value);
            }
        }

        private void UpdateApprovalDetails(DataSet dsVouchers, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dsVouchers.Tables["FiTransactions"].Rows)
            {
                Cmd = new SqlCommand("VoucherSP", Con, tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateApprovalDetails");
                Cmd.Parameters.AddWithValue("@ApprovedBy", dr["ApprovedBy"]);
                Cmd.Parameters.AddWithValue("@ApprovedDate", dr["ApprovedDate"]);
                Cmd.Parameters.AddWithValue("@ApprovalStatus", dr["ApprovalStatus"]);
                Cmd.Parameters.AddWithValue("@ApproveNote", dr["ApproveNote"]);
                Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                Cmd.Parameters.AddWithValue("@Posted", dr["Posted"]);
                Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                Cmd.ExecuteNonQuery();
            }
            foreach (DataRow dr in dsVouchers.Tables["FiTransactionAdditionals"].Rows)
            {
                Cmd = new SqlCommand("VoucherSP", Con, tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateApprovalDetailsAdditionals");
                Cmd.Parameters.AddWithValue("@RecommendByID", dr["RecommendByID"]);
                Cmd.Parameters.AddWithValue("@RecommendDate", dr["RecommendDate"]);
                Cmd.Parameters.AddWithValue("@RecommendStatus", dr["RecommendStatus"]);
                Cmd.Parameters.AddWithValue("@RecommendNote", dr["RecommendNote"]);
                Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                Cmd.ExecuteNonQuery();
            }
            foreach (DataRow dr in dsVouchers.Tables["InvTransItems"].Rows)
            {
                Cmd = new SqlCommand("VoucherSP", Con, tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateApprovalDetailsInvTransItems");
                Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                Cmd.ExecuteNonQuery();
            }

            //foreach (DataRow dr in dsVouchers.Tables["InvTransSubItems"].Rows)
            //{
            //    Cmd = new SqlCommand("VoucherSP", Con, tx);
            //    Cmd.CommandType = CommandType.StoredProcedure;
            //    Cmd.Parameters.AddWithValue("@Criteria", "UpdateApprovalDetailsInvTransSubItems");
            //    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
            //    Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
            //    Cmd.ExecuteNonQuery();
            //}
        }

        private void JournalVoucherAllocation(DataTable dtVoucherAllocation, SqlConnection Con, SqlTransaction tx)
        {
            try
            {
                SqlCommand Cmd;
                foreach (DataRow dr in dtVoucherAllocation.Rows)
                {
                    if (dr.RowState == DataRowState.Added)
                    {
                        Cmd = new SqlCommand("VoucherSP", Con, tx);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "InsertVoucherAllocation");
                        Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
                        Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                        Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                        Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                        //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                        Cmd.ExecuteNonQuery();
                        dr["ID"] = Cmd.Parameters["@NewID"].Value;
                        dr.EndEdit();
                    }
                    if (dr.RowState == DataRowState.Modified)
                    {
                        Cmd = new SqlCommand("VoucherSP", Con, tx);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "UpdateVoucherAllocation");
                        Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
                        Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                        Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                        Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                        Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                        Cmd.ExecuteNonQuery();
                    }
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        Cmd = new SqlCommand("VoucherSP", Con, tx);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "DeeleteVoucherAllocation");
                        Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //private void SaveLotAllocation(DataTable dtInvLotAllocation, SqlConnection Con, SqlTransaction tx)
        //{
        //    SqlCommand Cmd;
        //    foreach (DataRow dr in dtInvLotAllocation.Rows)
        //    {
        //        if (dr.RowState == DataRowState.Added)
        //        {
        //            Cmd = new SqlCommand("VoucherSP", Con, tx);
        //            Cmd.CommandType = CommandType.StoredProcedure;
        //            Cmd.Parameters.AddWithValue("@Criteria", "InsertLotAllocation");
        //            Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
        //            Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
        //            Cmd.Parameters.AddWithValue("@LotID", dr["LotID"]);
        //            Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
        //            Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
        //            Cmd.ExecuteNonQuery();
        //            dr["ID"] = Cmd.Parameters["@NewID"].Value;
        //            dr.EndEdit();
        //        }
        //        if (dr.RowState == DataRowState.Modified)
        //        {
        //            Cmd = new SqlCommand("VoucherSP", Con, tx);
        //            Cmd.CommandType = CommandType.StoredProcedure;
        //            Cmd.Parameters.AddWithValue("@Criteria", "UpdateLotAllocation");
        //            Cmd.Parameters.AddWithValue("@VID", dr["VID"]);
        //            Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
        //            Cmd.Parameters.AddWithValue("@LotID", dr["LotID"]);
        //            Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
        //            Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
        //            Cmd.ExecuteNonQuery();
        //        }
        //        if (dr.RowState == DataRowState.Deleted)
        //        {
        //            Cmd = new SqlCommand("VoucherSP", Con, tx);
        //            Cmd.CommandType = CommandType.StoredProcedure;
        //            Cmd.Parameters.AddWithValue("@Criteria", "DeeleteLotAllocation");
        //            Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
        //            Cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        private void SaveAdditionals(DataTable dtFiTransactionAdditionals, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtFiTransactionAdditionals.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertFiTransactionAdditionals");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransID1", dr["RefTransID1"]);
                    Cmd.Parameters.AddWithValue("@RefTransID2", dr["RefTransID2"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@ModeID", dr["ModeID"]);
                    Cmd.Parameters.AddWithValue("@MeasureTypeID", dr["MeasureTypeID"]);
                    Cmd.Parameters.AddWithValue("@LoadMeasureTypeID", dr["LoadMeasureTypeID"]);
                    Cmd.Parameters.AddWithValue("@ConsignTermID", dr["ConsignTermID"]);
                    Cmd.Parameters.AddWithValue("@FromLocationID", dr["FromLocationID"]);
                    Cmd.Parameters.AddWithValue("@ToLocationID", dr["ToLocationID"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate1", dr["ExchangeRate1"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate2", dr["ExchangeRate2"]);
                    Cmd.Parameters.AddWithValue("@AdvanceExRate", dr["AdvanceExRate"]);
                    Cmd.Parameters.AddWithValue("@CustomsExRate", dr["CustomsExRate"]);
                    Cmd.Parameters.AddWithValue("@ApprovalDays", dr["ApprovalDays"]);
                    Cmd.Parameters.AddWithValue("@WorkflowDays", dr["WorkflowDays"]);
                    Cmd.Parameters.AddWithValue("@PostedBranchID", dr["PostedBranchID"]);
                    Cmd.Parameters.AddWithValue("@ShipBerthDate", dr["ShipBerthDate"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@Name", dr["Name"]);
                    Cmd.Parameters.AddWithValue("@Code", dr["Code"]);
                    Cmd.Parameters.AddWithValue("@Address", dr["Address"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@SystemRate", dr["SystemRate"]);
                    Cmd.Parameters.AddWithValue("@Period", dr["Period"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@LCOptionID", dr["LCOptionID"]);
                    Cmd.Parameters.AddWithValue("@LCNo", dr["LCNo"]);
                    Cmd.Parameters.AddWithValue("@LCAmt", dr["LCAmt"]);
                    Cmd.Parameters.AddWithValue("@AvailableLCAmt", dr["AvailableLCAmt"]);
                    Cmd.Parameters.AddWithValue("@CreditAmt", dr["CreditAmt"]);
                    Cmd.Parameters.AddWithValue("@MarginAmt", dr["MarginAmt"]);
                    Cmd.Parameters.AddWithValue("@InterestAmt", dr["InterestAmt"]);
                    Cmd.Parameters.AddWithValue("@AvailableAmt", dr["AvailableAmt"]);
                    Cmd.Parameters.AddWithValue("@AllocationPerc", dr["AllocationPerc"]);
                    Cmd.Parameters.AddWithValue("@InterestPerc", dr["InterestPerc"]);
                    Cmd.Parameters.AddWithValue("@TolerencePerc", dr["TolerencePerc"]);
                    Cmd.Parameters.AddWithValue("@CountryID", dr["CountryID"]);
                    Cmd.Parameters.AddWithValue("@CountryOfOriginID", dr["CountryOfOriginID"]);
                    Cmd.Parameters.AddWithValue("@MaxDays", dr["MaxDays"]);
                    Cmd.Parameters.AddWithValue("@DocumentNo", dr["DocumentNo"]);
                    Cmd.Parameters.AddWithValue("@DocumentDate", dr["DocumentDate"]);
                    Cmd.Parameters.AddWithValue("@BEMaxDays", dr["BEMaxDays"]);
                    Cmd.Parameters.AddWithValue("@EntryDate", dr["EntryDate"]);
                    Cmd.Parameters.AddWithValue("@EntryNo", dr["EntryNo"]);
                    Cmd.Parameters.AddWithValue("@ApplicationCode", dr["ApplicationCode"]);
                    Cmd.Parameters.AddWithValue("@BankAddress", dr["BankAddress"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@AcceptDate", dr["AcceptDate"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                    Cmd.Parameters.AddWithValue("@OpenDate", dr["OpenDate"]);
                    Cmd.Parameters.AddWithValue("@CloseDate", dr["CloseDate"]);
                    Cmd.Parameters.AddWithValue("@StartDate", dr["StartDate"]);
                    Cmd.Parameters.AddWithValue("@EndDate", dr["EndDate"]);
                    Cmd.Parameters.AddWithValue("@ClearDate", dr["ClearDate"]);
                    Cmd.Parameters.AddWithValue("@ReceiveDate", dr["ReceiveDate"]);
                    Cmd.Parameters.AddWithValue("@SubmitDate", dr["SubmitDate"]);
                    Cmd.Parameters.AddWithValue("@EndTime", dr["EndTime"]);
                    Cmd.Parameters.AddWithValue("@HandOverTime", dr["HandOverTime"]);
                    Cmd.Parameters.AddWithValue("@LorryHireRate", dr["LorryHireRate"]);
                    Cmd.Parameters.AddWithValue("@QtyPerLoad", dr["QtyPerLoad"]);
                    Cmd.Parameters.AddWithValue("@PassNo", dr["PassNo"]);
                    Cmd.Parameters.AddWithValue("@ReferenceDate", dr["ReferenceDate"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@AuditNote", dr["AuditNote"]);
                    Cmd.Parameters.AddWithValue("@Terms", dr["Terms"]);
                    Cmd.Parameters.AddWithValue("@FirmID", dr["FirmID"]);
                    Cmd.Parameters.AddWithValue("@RouteID", dr["RouteID"]);
                    Cmd.Parameters.AddWithValue("@VehicleID", dr["VehicleID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@AccountID2", dr["AccountID2"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["OtherBranchID"]);
                    Cmd.Parameters.AddWithValue("@IsClosed", dr["IsClosed"]);
                    if (dr["WeekDays"] != DBNull.Value)
                    {
                        Cmd.Parameters.AddWithValue("@WeekDays", dr["WeekDays"]);
                    }
                    if (dr["BankWeekDays"] != DBNull.Value)
                    {
                        Cmd.Parameters.AddWithValue("@BankWeekDays", dr["BankWeekDays"]);
                    }
                    Cmd.Parameters.AddWithValue("@RecommendByID", dr["RecommendByID"]);
                    Cmd.Parameters.AddWithValue("@RecommendDate", dr["RecommendDate"]);
                    Cmd.Parameters.AddWithValue("@RecommendNote", dr["RecommendNote"]);
                    Cmd.Parameters.AddWithValue("@RecommendStatus", dr["RecommendStatus"]);
                    Cmd.Parameters.AddWithValue("@IsHigherApproval", dr["IsHigherApproval"]);
                    Cmd.Parameters.AddWithValue("@LCApplnTransID", dr["LCApplnTransID"]);
                    Cmd.Parameters.AddWithValue("@InLocID", dr["InLocID"]);
                    Cmd.Parameters.AddWithValue("@OutLocID", dr["OutLocID"]);
                    Cmd.Parameters.AddWithValue("@Hours", dr["Hours"]);
                    Cmd.Parameters.AddWithValue("@Year", dr["Year"]);
                    Cmd.Parameters.AddWithValue("@AreaID", dr["AreaID"]);
                    Cmd.Parameters.AddWithValue("@TaxFormID", dr["TaxFormID"]);
                    Cmd.Parameters.AddWithValue("@PriceCategoryID", dr["PriceCategoryID"]);
                    Cmd.Parameters.AddWithValue("@DepartmentID", dr["DepartmentID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "TransactionID", DataRowVersion.Original, null));
                    Cmd.CommandTimeout = 600;
                    Cmd.ExecuteNonQuery();
                    // dr["TransactionID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateFiTransactionAdditionals");
                    Cmd.Parameters.AddWithValue("@RefTransID1", dr["RefTransID1"]);
                    Cmd.Parameters.AddWithValue("@RefTransID2", dr["RefTransID2"]);
                    Cmd.Parameters.AddWithValue("@TypeID", dr["TypeID"]);
                    Cmd.Parameters.AddWithValue("@ModeID", dr["ModeID"]);
                    Cmd.Parameters.AddWithValue("@MeasureTypeID", dr["MeasureTypeID"]);
                    Cmd.Parameters.AddWithValue("@LoadMeasureTypeID", dr["LoadMeasureTypeID"]);
                    Cmd.Parameters.AddWithValue("@ConsignTermID", dr["ConsignTermID"]);
                    Cmd.Parameters.AddWithValue("@FromLocationID", dr["FromLocationID"]);
                    Cmd.Parameters.AddWithValue("@ToLocationID", dr["ToLocationID"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate1", dr["ExchangeRate1"]);
                    Cmd.Parameters.AddWithValue("@ExchangeRate2", dr["ExchangeRate2"]);
                    Cmd.Parameters.AddWithValue("@AdvanceExRate", dr["AdvanceExRate"]);
                    Cmd.Parameters.AddWithValue("@CustomsExRate", dr["CustomsExRate"]);
                    Cmd.Parameters.AddWithValue("@ApprovalDays", dr["ApprovalDays"]);
                    Cmd.Parameters.AddWithValue("@WorkflowDays", dr["WorkflowDays"]);
                    Cmd.Parameters.AddWithValue("@PostedBranchID", dr["PostedBranchID"]);
                    Cmd.Parameters.AddWithValue("@ShipBerthDate", dr["ShipBerthDate"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@Name", dr["Name"]);
                    Cmd.Parameters.AddWithValue("@Code", dr["Code"]);
                    Cmd.Parameters.AddWithValue("@Address", dr["Address"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@SystemRate", dr["SystemRate"]);
                    Cmd.Parameters.AddWithValue("@Period", dr["Period"]);
                    Cmd.Parameters.AddWithValue("@Days", dr["Days"]);
                    Cmd.Parameters.AddWithValue("@LCOptionID", dr["LCOptionID"]);
                    Cmd.Parameters.AddWithValue("@LCNo", dr["LCNo"]);
                    Cmd.Parameters.AddWithValue("@LCAmt", dr["LCAmt"]);
                    Cmd.Parameters.AddWithValue("@AvailableLCAmt", dr["AvailableLCAmt"]);
                    Cmd.Parameters.AddWithValue("@CreditAmt", dr["CreditAmt"]);
                    Cmd.Parameters.AddWithValue("@MarginAmt", dr["MarginAmt"]);
                    Cmd.Parameters.AddWithValue("@InterestAmt", dr["InterestAmt"]);
                    Cmd.Parameters.AddWithValue("@AvailableAmt", dr["AvailableAmt"]);
                    Cmd.Parameters.AddWithValue("@AllocationPerc", dr["AllocationPerc"]);
                    Cmd.Parameters.AddWithValue("@InterestPerc", dr["InterestPerc"]);
                    Cmd.Parameters.AddWithValue("@TolerencePerc", dr["TolerencePerc"]);
                    Cmd.Parameters.AddWithValue("@CountryID", dr["CountryID"]);
                    Cmd.Parameters.AddWithValue("@CountryOfOriginID", dr["CountryOfOriginID"]);
                    Cmd.Parameters.AddWithValue("@MaxDays", dr["MaxDays"]);
                    Cmd.Parameters.AddWithValue("@DocumentNo", dr["DocumentNo"]);
                    Cmd.Parameters.AddWithValue("@DocumentDate", dr["DocumentDate"]);
                    Cmd.Parameters.AddWithValue("@BEMaxDays", dr["BEMaxDays"]);
                    Cmd.Parameters.AddWithValue("@EntryDate", dr["EntryDate"]);
                    Cmd.Parameters.AddWithValue("@EntryNo", dr["EntryNo"]);
                    Cmd.Parameters.AddWithValue("@ApplicationCode", dr["ApplicationCode"]);
                    Cmd.Parameters.AddWithValue("@BankAddress", dr["BankAddress"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@AcceptDate", dr["AcceptDate"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                    Cmd.Parameters.AddWithValue("@OpenDate", dr["OpenDate"]);
                    Cmd.Parameters.AddWithValue("@CloseDate", dr["CloseDate"]);
                    Cmd.Parameters.AddWithValue("@StartDate", dr["StartDate"]);
                    Cmd.Parameters.AddWithValue("@EndDate", dr["EndDate"]);
                    Cmd.Parameters.AddWithValue("@ClearDate", dr["ClearDate"]);
                    Cmd.Parameters.AddWithValue("@ReceiveDate", dr["ReceiveDate"]);
                    Cmd.Parameters.AddWithValue("@SubmitDate", dr["SubmitDate"]);
                    Cmd.Parameters.AddWithValue("@EndTime", dr["EndTime"]);
                    Cmd.Parameters.AddWithValue("@HandOverTime", dr["HandOverTime"]);
                    Cmd.Parameters.AddWithValue("@LorryHireRate", dr["LorryHireRate"]);
                    Cmd.Parameters.AddWithValue("@QtyPerLoad", dr["QtyPerLoad"]);
                    Cmd.Parameters.AddWithValue("@PassNo", dr["PassNo"]);
                    Cmd.Parameters.AddWithValue("@ReferenceDate", dr["ReferenceDate"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@AuditNote", dr["AuditNote"]);
                    Cmd.Parameters.AddWithValue("@Terms", dr["Terms"]);
                    Cmd.Parameters.AddWithValue("@RouteID", dr["RouteID"]);
                    Cmd.Parameters.AddWithValue("@FirmID", dr["FirmID"]);
                    Cmd.Parameters.AddWithValue("@VehicleID", dr["VehicleID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@AccountID2", dr["AccountID2"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["OtherBranchID"]);
                    if (dr["WeekDays"] != DBNull.Value)
                    {
                        Cmd.Parameters.AddWithValue("@WeekDays", dr["WeekDays"]);
                    }
                    if (dr["BankWeekDays"] != DBNull.Value)
                    {
                        Cmd.Parameters.AddWithValue("@BankWeekDays", dr["BankWeekDays"]);
                    }
                    Cmd.Parameters.AddWithValue("@RecommendByID", dr["RecommendByID"]);
                    Cmd.Parameters.AddWithValue("@RecommendDate", dr["RecommendDate"]);
                    Cmd.Parameters.AddWithValue("@RecommendNote", dr["RecommendNote"]);
                    Cmd.Parameters.AddWithValue("@RecommendStatus", dr["RecommendStatus"]);
                    Cmd.Parameters.AddWithValue("@IsHigherApproval", dr["IsHigherApproval"]);
                    Cmd.Parameters.AddWithValue("@LCApplnTransID", dr["LCApplnTransID"]);
                    Cmd.Parameters.AddWithValue("@InLocID", dr["InLocID"]);
                    Cmd.Parameters.AddWithValue("@OutLocID", dr["OutLocID"]);
                    Cmd.Parameters.AddWithValue("@Hours", dr["Hours"]);
                    Cmd.Parameters.AddWithValue("@Year", dr["Year"]);
                    Cmd.Parameters.AddWithValue("@AreaID", dr["AreaID"]);
                    Cmd.Parameters.AddWithValue("@TaxFormID", dr["TaxFormID"]);
                    Cmd.Parameters.AddWithValue("@PriceCategoryID", dr["PriceCategoryID"]);
                    Cmd.Parameters.AddWithValue("@DepartmentID", dr["DepartmentID"]);
                    Cmd.Parameters.AddWithValue("@IsClosed", dr["IsClosed"]);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID", DataRowVersion.Original]);
                    Cmd.CommandTimeout = 600;
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteFiTransactionAdditionals");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }

        }

        private void SaveInvTransItems(DataTable dtInvTransItems, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtInvTransItems.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertInvTransItems");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@SerialNo", dr["SerialNo"]);
                    Cmd.Parameters.AddWithValue("@RefTransID1", dr["RefTransID1"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@FOCQty", dr["FOCQty"]);
                    Cmd.Parameters.AddWithValue("@BasicQty", dr["BasicQty"]);
                    Cmd.Parameters.AddWithValue("@Pcs", dr["Pcs"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@AdvanceRate", dr["AdvanceRate"]);
                    Cmd.Parameters.AddWithValue("@OtherRate", dr["OtherRate"]);
                    Cmd.Parameters.AddWithValue("@MasterMiscID1", dr["MasterMiscID1"]);
                    Cmd.Parameters.AddWithValue("@RowType", dr["RowType"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@InvAvgCostID", dr["InvAvgCostID"]);
                    Cmd.Parameters.AddWithValue("@IsReturn", dr["IsReturn"]);
                    Cmd.Parameters.AddWithValue("@Discount", dr["Discount"]);
                    Cmd.Parameters.AddWithValue("@Additional", dr["Additional"]);
                    Cmd.Parameters.AddWithValue("@Factor", dr["Factor"]);
                    Cmd.Parameters.AddWithValue("@CommodityID", dr["CommodityID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@LengthFt", dr["LengthFt"]);
                    Cmd.Parameters.AddWithValue("@LengthIn", dr["LengthIn"]);
                    Cmd.Parameters.AddWithValue("@LengthCm", dr["LengthCm"]);
                    Cmd.Parameters.AddWithValue("@GirthFt", dr["GirthFt"]);
                    Cmd.Parameters.AddWithValue("@GirthIn", dr["GirthIn"]);
                    Cmd.Parameters.AddWithValue("@GirthCm", dr["GirthCm"]);
                    Cmd.Parameters.AddWithValue("@ThicknessFt", dr["ThicknessFt"]);
                    Cmd.Parameters.AddWithValue("@ThicknessIn", dr["ThicknessIn"]);
                    Cmd.Parameters.AddWithValue("@ThicknessCm", dr["ThicknessCm"]);
                    Cmd.Parameters.AddWithValue("@TransactionEntryID", dr["TransactionEntryID"]);
                    Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                    Cmd.Parameters.AddWithValue("@Cancel", dr["Cancel"]);
                    Cmd.Parameters.AddWithValue("@MeasuredByID", dr["MeasuredByID"]);
                    Cmd.Parameters.AddWithValue("@RefTransItemID", dr["RefTransItemID"]);
                    Cmd.Parameters.AddWithValue("@FinishDate", dr["FinishDate"]);
                    Cmd.Parameters.AddWithValue("@UpdateDate", dr["UpdateDate"]);
                    Cmd.Parameters.AddWithValue("@IsSameForPcs", dr["IsSameForPcs"]);
                    Cmd.Parameters.AddWithValue("@StockQty", dr["StockQty"]);
                    Cmd.Parameters.AddWithValue("@Margin", dr["Margin"]);
                    Cmd.Parameters.AddWithValue("@InlocID", dr["InlocID"]);
                    Cmd.Parameters.AddWithValue("@OutLocID", dr["OutLocID"]);
                    Cmd.Parameters.AddWithValue("@BatchNo", dr["BatchNo"]);
                    Cmd.Parameters.AddWithValue("@SizeMasterID", dr["SizeMasterID"]);
                    Cmd.Parameters.AddWithValue("@DiscountPerc", dr["DiscountPerc"]);
                    Cmd.Parameters.AddWithValue("@TaxPerc", dr["TaxPerc"]);
                    Cmd.Parameters.AddWithValue("@TaxValue", dr["TaxValue"]);
                    Cmd.Parameters.AddWithValue("@TaxTypeID", dr["TaxTypeID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@CostPerc", dr["CostPerc"]);
                    Cmd.Parameters.AddWithValue("@ManufactureDate", dr["ManufactureDate"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@PriceCategoryID", dr["PriceCategoryID"]);
                    Cmd.Parameters.AddWithValue("@GroupItemID", dr["GroupItemID"]);
                    //Cmd.Parameters.AddWithValue("@RateDsicPerc", dr["RateDsicPerc"]);
                    Cmd.Parameters.AddWithValue("@RateDisc", dr["RateDisc"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@TempQty", dr["TempQty"]);
                    Cmd.Parameters.AddWithValue("@TempRate", dr["TempRate"]);
                    Cmd.Parameters.AddWithValue("@ReplaceQty", dr["ReplaceQty"]);
                    Cmd.Parameters.AddWithValue("@PrintedMRP", dr["PrintedMRP"]);
                    Cmd.Parameters.AddWithValue("@PrintedRate", dr["PrintedRate"]);
                    Cmd.Parameters.AddWithValue("@PTSRate", dr["PTSRate"]);
                    Cmd.Parameters.AddWithValue("@PTRRate", dr["PTRRate"]);
                    Cmd.Parameters.AddWithValue("@TempBatchNo", dr["TempBatchNo"]);
                    Cmd.Parameters.AddWithValue("@StockItemID", dr["StockItemID"]);
                    Cmd.Parameters.AddWithValue("@TaxAccountID", dr["TaxAccountID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvTransItems");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@SerialNo", dr["SerialNo"]);
                    Cmd.Parameters.AddWithValue("@RefTransID1", dr["RefTransID1"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@FOCQty", dr["FOCQty"]);
                    Cmd.Parameters.AddWithValue("@BasicQty", dr["BasicQty"]);
                    Cmd.Parameters.AddWithValue("@Pcs", dr["Pcs"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@AdvanceRate", dr["AdvanceRate"]);
                    Cmd.Parameters.AddWithValue("@OtherRate", dr["OtherRate"]);
                    Cmd.Parameters.AddWithValue("@MasterMiscID1", dr["MasterMiscID1"]);
                    Cmd.Parameters.AddWithValue("@RowType", dr["RowType"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@InvAvgCostID", dr["InvAvgCostID"]);
                    Cmd.Parameters.AddWithValue("@IsReturn", dr["IsReturn"]);
                    Cmd.Parameters.AddWithValue("@Discount", dr["Discount"]);
                    Cmd.Parameters.AddWithValue("@Additional", dr["Additional"]);
                    Cmd.Parameters.AddWithValue("@Factor", dr["Factor"]);
                    Cmd.Parameters.AddWithValue("@CommodityID", dr["CommodityID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@LengthFt", dr["LengthFt"]);
                    Cmd.Parameters.AddWithValue("@LengthIn", dr["LengthIn"]);
                    Cmd.Parameters.AddWithValue("@LengthCm", dr["LengthCm"]);
                    Cmd.Parameters.AddWithValue("@GirthFt", dr["GirthFt"]);
                    Cmd.Parameters.AddWithValue("@GirthIn", dr["GirthIn"]);
                    Cmd.Parameters.AddWithValue("@GirthCm", dr["GirthCm"]);
                    Cmd.Parameters.AddWithValue("@ThicknessFt", dr["ThicknessFt"]);
                    Cmd.Parameters.AddWithValue("@ThicknessIn", dr["ThicknessIn"]);
                    Cmd.Parameters.AddWithValue("@ThicknessCm", dr["ThicknessCm"]);
                    Cmd.Parameters.AddWithValue("@TransactionEntryID", dr["TransactionEntryID"]);
                    Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                    Cmd.Parameters.AddWithValue("@Cancel", dr["Cancel"]);
                    Cmd.Parameters.AddWithValue("@MeasuredByID", dr["MeasuredByID"]);
                    Cmd.Parameters.AddWithValue("@FinishDate", dr["FinishDate"]);
                    Cmd.Parameters.AddWithValue("@UpdateDate", dr["UpdateDate"]);
                    Cmd.Parameters.AddWithValue("@IsSameForPcs", dr["IsSameForPcs"]);
                    Cmd.Parameters.AddWithValue("@RefTransItemID", dr["RefTransItemID"]);
                    Cmd.Parameters.AddWithValue("@StockQty", dr["StockQty"]);
                    Cmd.Parameters.AddWithValue("@Margin", dr["Margin"]);
                    Cmd.Parameters.AddWithValue("@InlocID", dr["InlocID"]);
                    Cmd.Parameters.AddWithValue("@OutLocID", dr["OutLocID"]);
                    Cmd.Parameters.AddWithValue("@BatchNo", dr["BatchNo"]);
                    Cmd.Parameters.AddWithValue("@SizeMasterID", dr["SizeMasterID"]);
                    Cmd.Parameters.AddWithValue("@DiscountPerc", dr["DiscountPerc"]);
                    Cmd.Parameters.AddWithValue("@TaxPerc", dr["TaxPerc"]);
                    Cmd.Parameters.AddWithValue("@TaxValue", dr["TaxValue"]);
                    Cmd.Parameters.AddWithValue("@TaxTypeID", dr["TaxTypeID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@CostPerc", dr["CostPerc"]);
                    Cmd.Parameters.AddWithValue("@ManufactureDate", dr["ManufactureDate"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@PriceCategoryID", dr["PriceCategoryID"]);
                    Cmd.Parameters.AddWithValue("@GroupItemID", dr["GroupItemID"]);
                    //Cmd.Parameters.AddWithValue("@RateDsicPerc", dr["RateDsicPerc"]);
                    Cmd.Parameters.AddWithValue("@RateDisc", dr["RateDisc"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@TempQty", dr["TempQty"]);
                    Cmd.Parameters.AddWithValue("@TempRate", dr["TempRate"]);
                    Cmd.Parameters.AddWithValue("@ReplaceQty", dr["ReplaceQty"]);
                    Cmd.Parameters.AddWithValue("@PrintedMRP", dr["PrintedMRP"]);
                    Cmd.Parameters.AddWithValue("@PrintedRate", dr["PrintedRate"]);
                    Cmd.Parameters.AddWithValue("@PTSRate", dr["PTSRate"]);
                    Cmd.Parameters.AddWithValue("@PTRRate", dr["PTRRate"]);
                    Cmd.Parameters.AddWithValue("@TempBatchNo", dr["TempBatchNo"]);
                    Cmd.Parameters.AddWithValue("@StockItemID", dr["StockItemID"]);
                    Cmd.Parameters.AddWithValue("@TaxAccountID", dr["TaxAccountID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvTransItems");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveInvTransItemDetails(DataTable dtInvTransItemDetails, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtInvTransItemDetails.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertInvTransItemDetails");
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["TransItemID"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@BasicQty", dr["BasicQty"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@LocationID", dr["LocationID"]);
                    Cmd.Parameters.AddWithValue("@ToLocationID", dr["ToLocationID"]);
                    Cmd.Parameters.AddWithValue("@VTypeID", dr["VTypeID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@CreditWeeks", dr["CreditWeeks"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]); ;
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvTransItemDetails");
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["TransItemID"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@BasicQty", dr["BasicQty"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@LocationID", dr["LocationID"]);
                    Cmd.Parameters.AddWithValue("@ToLocationID", dr["ToLocationID"]);
                    Cmd.Parameters.AddWithValue("@VTypeID", dr["VTypeID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@CreditWeeks", dr["CreditWeeks"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]); ;
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvTransItemDetails");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveInvTransSubItems(DataTable dtInvTransSubItems, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtInvTransSubItems.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertInvTransSubItems");
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["TransItemID"]);
                    Cmd.Parameters.AddWithValue("@SubItemID", dr["SubItemID"]);
                    Cmd.Parameters.AddWithValue("@LengthFt", dr["LengthFt"]);
                    Cmd.Parameters.AddWithValue("@LengthIn", dr["LengthIn"]);
                    Cmd.Parameters.AddWithValue("@LengthCm", dr["LengthCm"]);
                    Cmd.Parameters.AddWithValue("@GirthFt", dr["GirthFt"]);
                    Cmd.Parameters.AddWithValue("@GirthIn", dr["GirthIn"]);
                    Cmd.Parameters.AddWithValue("@GirthCm", dr["GirthCm"]);
                    Cmd.Parameters.AddWithValue("@ThicknessFt", dr["ThicknessFt"]);
                    Cmd.Parameters.AddWithValue("@ThicknessIn", dr["ThicknessIn"]);
                    Cmd.Parameters.AddWithValue("@ThicknessCm", dr["ThicknessCm"]);
                    Cmd.Parameters.AddWithValue("@Pcs", dr["Pcs"]);
                    Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                    Cmd.Parameters.AddWithValue("@Cancel", dr["Cancel"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@MeasuredByID", dr["MeasuredByID"]);
                    Cmd.Parameters.AddWithValue("@FinishDate", dr["FinishDate"]);
                    Cmd.Parameters.AddWithValue("@UpdateDate", dr["UpdateDate"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@IsSameForPcs", dr["IsSameForPcs"]);
                    Cmd.Parameters.AddWithValue("@RowType", dr["RowType"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvTransSubItems");
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["TransItemID"]);
                    Cmd.Parameters.AddWithValue("@SubItemID", dr["SubItemID"]);
                    Cmd.Parameters.AddWithValue("@LengthFt", dr["LengthFt"]);
                    Cmd.Parameters.AddWithValue("@LengthIn", dr["LengthIn"]);
                    Cmd.Parameters.AddWithValue("@LengthCm", dr["LengthCm"]);
                    Cmd.Parameters.AddWithValue("@GirthFt", dr["GirthFt"]);
                    Cmd.Parameters.AddWithValue("@GirthIn", dr["GirthIn"]);
                    Cmd.Parameters.AddWithValue("@GirthCm", dr["GirthCm"]);
                    Cmd.Parameters.AddWithValue("@ThicknessFt", dr["ThicknessFt"]);
                    Cmd.Parameters.AddWithValue("@ThicknessIn", dr["ThicknessIn"]);
                    Cmd.Parameters.AddWithValue("@ThicknessCm", dr["ThicknessCm"]);
                    Cmd.Parameters.AddWithValue("@Pcs", dr["Pcs"]);
                    Cmd.Parameters.AddWithValue("@Status", dr["Status"]);
                    Cmd.Parameters.AddWithValue("@Cancel", dr["Cancel"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@MeasuredByID", dr["MeasuredByID"]);
                    Cmd.Parameters.AddWithValue("@FinishDate", dr["FinishDate"]);
                    Cmd.Parameters.AddWithValue("@UpdateDate", dr["UpdateDate"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@IsSameForPcs", dr["IsSameForPcs"]);
                    Cmd.Parameters.AddWithValue("@RowType", dr["RowType"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvTransSubItems");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveDocumentRequests(DataTable dtDocumentRequests, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtDocumentRequests.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertDocumentRequests");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DocTypeID", dr["DocTypeID"]);
                    Cmd.Parameters.AddWithValue("@OriginalCopies", dr["OriginalCopies"]);
                    Cmd.Parameters.AddWithValue("@DuplicateCopies", dr["DuplicateCopies"]);
                    Cmd.Parameters.AddWithValue("@IsReceived", dr["IsReceived"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateDocumentRequests");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DocTypeID", dr["DocTypeID"]);
                    Cmd.Parameters.AddWithValue("@OriginalCopies", dr["OriginalCopies"]);
                    Cmd.Parameters.AddWithValue("@DuplicateCopies", dr["DuplicateCopies"]);
                    Cmd.Parameters.AddWithValue("@IsReceived", dr["IsReceived"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteDocumentRequests");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void SaveDocumentReferences(DataTable dtDocumentReferences, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtDocumentReferences.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertDocumentReferences");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DocID", dr["DocID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateDocumentReferences");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DocID", dr["DocID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteDocumentReferences");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void SaveTransItemExpenses(DataTable dtTransItemExpenses, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransItemExpenses.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactionItemExpenses");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ChargeTypeID", dr["ChargeTypeID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["TransItemID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactionItemExpenses");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@ChargeTypeID", dr["ChargeTypeID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                    Cmd.Parameters.AddWithValue("@TransItemID", dr["TransItemID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactionItemExpenses");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void SaveTransactionExpenses(DataTable dtTransExpenses, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransExpenses.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactionExpenses");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@PreCalculatedAmt", dr["PreCalculatedAmt"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@PayableAccountID", dr["PayableAccountID"]);
                    Cmd.Parameters.AddWithValue("@ChargeTypeID", dr["ChargeTypeID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactionExpenses");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@PreCalculatedAmt", dr["PreCalculatedAmt"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@PayableAccountID", dr["PayableAccountID"]);
                    Cmd.Parameters.AddWithValue("@ChargeTypeID", dr["ChargeTypeID"]);
                    Cmd.Parameters.AddWithValue("@TranType", dr["TranType"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactionExpenses");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void SaveInvSubItemMaster(DataTable dtInvSubItemMaster, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtInvSubItemMaster.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertInvSubItemMaster");
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@LogNo", dr["LogNo"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvSubItemMaster");
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@LogNo", dr["LogNo"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvSubItemMaster");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void SaveTransCriteria(DataTable dtTransCriteria, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransCriteria.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransCriteria");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@CriteriaID", dr["CriteriaID"]);
                    Cmd.Parameters.AddWithValue("@BaseValue", dr["BaseValue"]);
                    Cmd.Parameters.AddWithValue("@Value", dr["Value"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransCriteria");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@CriteriaID", dr["CriteriaID"]);
                    Cmd.Parameters.AddWithValue("@BaseValue", dr["BaseValue"]);
                    Cmd.Parameters.AddWithValue("@Value", dr["Value"]);
                    Cmd.Parameters.AddWithValue("@IsBit", dr["IsBit"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransCriteria");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void saveTransCollections(DataTable dtTransCollections, SqlConnection Con, SqlTransaction tx)
        {
            try
            {
                SqlCommand Cmd;
                foreach (DataRow dr in dtTransCollections.Rows)
                {
                    if (dr.RowState == DataRowState.Added)
                    {
                        Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "InsertTransCollections");
                        Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                        Cmd.Parameters.AddWithValue("@PayTypeID", dr["PayTypeID"]);
                        Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                        Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                        Cmd.Parameters.AddWithValue("@InstrumentTypeID", dr["InstrumentTypeID"]);
                        Cmd.Parameters.AddWithValue("@InstrumentNo", dr["InstrumentNo"]);
                        Cmd.Parameters.AddWithValue("@InstrumentDate", dr["InstrumentDate"]);
                        Cmd.Parameters.AddWithValue("@InstrumentBank", dr["InstrumentBank"]);
                        Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                        //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                        //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                        Cmd.ExecuteNonQuery();
                        dr["ID"] = Cmd.Parameters["@NewID"].Value;
                        dr.EndEdit();
                    }
                    if (dr.RowState == DataRowState.Modified)
                    {
                        Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransCollections");
                        Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                        Cmd.Parameters.AddWithValue("@PayTypeID", dr["PayTypeID"]);
                        Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                        Cmd.Parameters.AddWithValue("@DueDate", dr["DueDate"]);
                        Cmd.Parameters.AddWithValue("@InstrumentTypeID", dr["InstrumentTypeID"]);
                        Cmd.Parameters.AddWithValue("@InstrumentNo", dr["InstrumentNo"]);
                        Cmd.Parameters.AddWithValue("@InstrumentDate", dr["InstrumentDate"]);
                        Cmd.Parameters.AddWithValue("@InstrumentBank", dr["InstrumentBank"]);
                        Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                        Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                        Cmd.ExecuteNonQuery();
                        dr.EndEdit();
                    }
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransCollections");
                        Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                        Cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private void SaveTransLoadSchedules(DataTable dtTransLoadSchedules, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransLoadSchedules.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransLoadSchedules");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@Code", dr["Code"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@LocationID", dr["LocationID"]);
                    Cmd.Parameters.AddWithValue("@VehicleID", dr["VehicleID"]);
                    Cmd.Parameters.AddWithValue("@QtyPerLoad", dr["QtyPerLoad"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransLoadSchedules");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@Code", dr["Code"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@LocationID", dr["LocationID"]);
                    Cmd.Parameters.AddWithValue("@VehicleID", dr["VehicleID"]);
                    Cmd.Parameters.AddWithValue("@QtyPerLoad", dr["QtyPerLoad"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@RefID", dr["RefID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransLoadSchedules");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        private void SaveTransReferences(DataTable dtTransReferences, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransReferences.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransactionReferences");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransactionReferences");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@RefTransID", dr["RefTransID"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactionReferences");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }


        }

        private void SaveAttach(DataTable dtDocImages, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtDocImages.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("DocSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertImage");
                    Cmd.Parameters.AddWithValue("@DocID", dr["DocID"]);
                    Cmd.Parameters.AddWithValue("@Path", dr["Path"]);
                    Cmd.Parameters.AddWithValue("@FileName", dr["FileName"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("DocSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateImage");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                    Cmd.Parameters.AddWithValue("@DocID", dr["DocID"]);
                    Cmd.Parameters.AddWithValue("@Path", dr["Path"]);
                    Cmd.Parameters.AddWithValue("@FileName", dr["FileName"]);
                    Cmd.Parameters.AddWithValue("@Date", dr["Date"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@Remarks", dr["Remarks"]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("DocSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteImages");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveDocuments(DataTable dtDocuments, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtDocuments.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("DocumentsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertDocuments");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DocTypeID", dr["DocTypeID"]);
                    Cmd.Parameters.AddWithValue("@DocNo", dr["DocNo"]);
                    Cmd.Parameters.AddWithValue("@Name", dr["Name"]);
                    Cmd.Parameters.AddWithValue("@IssueDate", dr["IssueDate"]);
                    Cmd.Parameters.AddWithValue("@IssuePlace", dr["IssuePlace"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@Path", dr["Path"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@OriginalCopies", dr["OriginalCopies"]);
                    Cmd.Parameters.AddWithValue("@DuplicateCopies", dr["DuplicateCopies"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@OrginType", dr["OrginType"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ContactInfo", dr["ContactInfo"]);
                    Cmd.Parameters.AddWithValue("@MaMiscID", dr["MaMiscID"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("DocumentsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateDocuments");
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@DocTypeID", dr["DocTypeID"]);
                    Cmd.Parameters.AddWithValue("@DocNo", dr["DocNo"]);
                    Cmd.Parameters.AddWithValue("@Name", dr["Name"]);
                    Cmd.Parameters.AddWithValue("@IssueDate", dr["IssueDate"]);
                    Cmd.Parameters.AddWithValue("@IssuePlace", dr["IssuePlace"]);
                    Cmd.Parameters.AddWithValue("@ExpiryDate", dr["ExpiryDate"]);
                    Cmd.Parameters.AddWithValue("@Path", dr["Path"]);
                    Cmd.Parameters.AddWithValue("@ReferenceNo", dr["ReferenceNo"]);
                    Cmd.Parameters.AddWithValue("@OriginalCopies", dr["OriginalCopies"]);
                    Cmd.Parameters.AddWithValue("@Active", dr["Active"]);
                    Cmd.Parameters.AddWithValue("@DuplicateCopies", dr["DuplicateCopies"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@OrginType", dr["OrginType"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ContactInfo", dr["ContactInfo"]);
                    Cmd.Parameters.AddWithValue("@MaMiscID", dr["MaMiscID"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("DocumentsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteDocuments");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }

            }
        }

        public DataTable FillPDC(int BranchID, int BankID)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection Con = new SqlConnection(ConnectionString);
                //Con.Open();
                SqlCommand Cmd = new SqlCommand("ChequeRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@BankID", BankID);
                Cmd.Parameters.AddWithValue("@Criteria", "FillPDC");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                new SqlDataAdapter(Cmd).Fill(dt);
                //SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //dt.Load(sdr);
                //sdr.Close();
                //Con.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillLocationusingBranch(object BranchID)
        {
            try
            {
                DataTable dt = new DataTable();

                // Use 'using' to ensure connection is properly disposed of after the operation
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("spLocations", Con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    Cmd.Parameters.AddWithValue("@Criteria", "FillLocationusingBranch");
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);

                    // Use SqlDataAdapter to fill the DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public DataSet FillPDCEdit(int BranchID, int BankID)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("ChequeRegisterSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@BankID", BankID);
                Cmd.Parameters.AddWithValue("@Criteria", "FillPDCEdit");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                new SqlDataAdapter(Cmd).Fill(ds);
                Con.Close();
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //For getting lot details in PackinList--Surag

        public DataSet FillLotDetails(int LotID, int PurchaseOrderID, int VoucherID)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("VoucherAdditionalsSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Criteria", "FillLotDetails");
                cmd.Parameters.AddWithValue("@ItemID", LotID);
                cmd.Parameters.AddWithValue("@TransactionID", PurchaseOrderID);
                cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                SqlDataAdapter dAdp = new SqlDataAdapter(cmd);
                dAdp.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataSet FillTemplates()
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("VoucherAdditionalsSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Criteria", "FillTemplates");
                SqlDataAdapter dAdp = new SqlDataAdapter(cmd);
                dAdp.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private void SaveInvAvgCost(DataTable dtInvAvgCost, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtInvAvgCost.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertInvAvgCost");
                    Cmd.Parameters.AddWithValue("@PurchaseDate", dr["PurchaseDate"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["BranchID"]);
                    Cmd.Parameters.AddWithValue("@LastRate", dr["LastRate"]);
                    Cmd.Parameters.AddWithValue("@AvgCost", dr["AvgCost"]);
                    Cmd.Parameters.AddWithValue("@BatchNo", dr["BatchNo"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvAvgCost");
                    Cmd.Parameters.AddWithValue("@PurchaseDate", dr["PurchaseDate"]);
                    Cmd.Parameters.AddWithValue("@ItemID", dr["ItemID"]);
                    Cmd.Parameters.AddWithValue("@BranchID", dr["BranchID"]);
                    Cmd.Parameters.AddWithValue("@LastRate", dr["LastRate"]);
                    Cmd.Parameters.AddWithValue("@AvgCost", dr["AvgCost"]);
                    Cmd.Parameters.AddWithValue("@BatchNo", dr["BatchNo"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("VoucherAdditionalsSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvAvgCost");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveTransCostAllocations(DataTable dtTransCostAllocations, SqlConnection Con, SqlTransaction tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtTransCostAllocations.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("CostCentreSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "InsertTransCostAllocations");
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@CostCentreID", dr["CostCentreID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("CostCentreSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "UpdateTransCostAllocations");
                    Cmd.Parameters.AddWithValue("@VEID", dr["VEID"]);
                    Cmd.Parameters.AddWithValue("@CostCentreID", dr["CostCentreID"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@Description", dr["Description"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("CostCentreSP", Con, tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransCostAllocations");
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }
        }


        public bool IsItemwise(object AccountID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                bool _IsItemwise = false;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "IsItemwise");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                _IsItemwise = Convert.ToBoolean(Cmd.ExecuteScalar());
                Con.Close();
                return _IsItemwise;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable FillItemforBill(object TransactionEntryID)
        {
            SqlConnection Con = null;
            try
            {
                DataTable dt = new DataTable();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillItemforBill");
                Cmd.Parameters.AddWithValue("@TransactionEntryID", TransactionEntryID);
                SqlDataAdapter adr = new SqlDataAdapter(Cmd);
                adr.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Con.Close();
            }
        }

        public void FillInvTransItemsInSalesProj(DataTable dtInvTransItems, Object TransactionID)
        {
            try
            {
                dtInvTransItems.Clear();
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "FillInvTransItemsInSalesProj");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dtInvTransItems.Load(sdr);
                //sdr.Close();
                //Con.Close();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void InsertCuttingVoucherOut(Object TransactionID, Object SubItemID)
        {
            SqlConnection Con = null;
            SqlTransaction Tx = null;
            SqlCommand Cmd = null;

            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Tx = Con.BeginTransaction();
                Cmd = new SqlCommand("CuttingOutSP", Con, Tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                Cmd.Parameters.AddWithValue("@SubItemID", SubItemID);
                Cmd.ExecuteNonQuery();
                Tx.Commit();
            }
            catch (Exception ex)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Tx.Rollback();
                }
                throw ex;
            }
            finally
            {
                Con.Close();
            }
        }

        public void InsertCuttingVoucherOut(Object TransactionID, Object SubItemID, SqlConnection Con, SqlTransaction Tx)
        {

            try
            {
                SqlCommand Cmd = null;
                Cmd = new SqlCommand("CuttingOutSP", Con, Tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                Cmd.Parameters.AddWithValue("@SubItemID", SubItemID);
                Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertStockOut(Object TransactionID, String IDs, SqlConnection Con, SqlTransaction Tx)
        {

            try
            {
                SqlCommand Cmd = null;
                Cmd = new SqlCommand("CuttingOutSP", Con, Tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                //Cmd.Parameters.AddWithValue("@Criteria", "InsertStockOut");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                Cmd.Parameters.AddWithValue("@IDs", IDs);
                Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetUniqueAccountID(Object Keyword)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("FIMaUniqueAccountsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetUniqueAccountID");
                Cmd.Parameters.AddWithValue("@Keyword", Keyword);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Convert.ToInt32(Output);
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public DataTable GetUniqueAccountIDandAccountName(Object Keyword)
        {
            SqlConnection Con = null;
            try
            {
                DataTable dt = new DataTable();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetUniqueAccountIDandAccountName");
                Cmd.Parameters.AddWithValue("@Keyword", Keyword);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public DataTable GetVehicleDetail(Object VehicleID, Object Date)
        {
            SqlConnection Con = null;
            try
            {
                DataTable dt = new DataTable();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetVehicleDetail");
                Cmd.Parameters.AddWithValue("@ID", VehicleID);
                Cmd.Parameters.AddWithValue("@Date", Date);
                SqlDataReader sdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(sdr);
                return dt;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public bool SetSeperateCashBillEntry()
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "SetSeperateCashBillEntry");
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Convert.ToBoolean(Output);
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public bool IsMultiCurrencySupport()
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "IsMultiCurrencySupport");
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Convert.ToBoolean(Output);
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public object SalesLocation()
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "SalesLocation");
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Int32 CashID()
        {
            SqlConnection Con = null;
            try
            {
                Int32 Output;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CashID");
                Output = Convert.ToInt32(Cmd.ExecuteScalar());
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Int32 GetCategoryID(string Category)
        {
            SqlConnection Con = null;
            try
            {
                Int32 Output;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CategoryID");
                Cmd.Parameters.AddWithValue("@Category", Category);
                Output = Convert.ToInt32(Cmd.ExecuteScalar());
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Int32 GetItemID(string ItemCode)
        {
            SqlConnection Con = null;
            try
            {
                Int32 Output;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "ItemID");
                Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                Output = Convert.ToInt32(Cmd.ExecuteScalar());
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetFactor(Object ItemID, Object Unit)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetFactor");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public DataTable GetCartonPieceFactor(Object ItemID, Object Unit, Object Qty, Object RowType, Object BranchID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetCartonPieceFactor");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Cmd.Parameters.AddWithValue("@Qty", Qty);
                Cmd.Parameters.AddWithValue("@RowType", RowType);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                return dt;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetSellingPrice(Object ItemID, Object Unit)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetSellingPrice");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetMarginPrice(Object BranchID, Object ItemID, Object AccountID, Object VoucherID, String Unit)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("select dbo.GetMarginPrice(@BranchID,@ItemID,@AccountID,@VoucherID,@Unit)", Con);
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                if (AccountID != null)
                {
                    Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                }
                Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetCategorySellingPrice(Object BranchID, Object ItemID, Object AccountID, Object VoucherID, String Unit, Object PriceCategoryID)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("select dbo.GetCategorySellingPrice(@BranchID,@ItemID,@AccountID,@VoucherID,@Unit,@PriceCategoryID) ", Con);
                //Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                Cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Cmd.Parameters.AddWithValue("@PriceCategoryID", PriceCategoryID);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetPieceSellingPrice(Object ItemID)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetPieceSellingPrice");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetPurchaseRate(Object ItemID, Object Unit)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetPurchaseRate");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Object GetPiecePurchaseRate(Object ItemID)
        {
            SqlConnection Con = null;
            try
            {
                Object Output = null;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetPiecepurchaseRate");
                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                Output = Cmd.ExecuteScalar();
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Int32 GetCommodityID(string Commodity)
        {
            SqlConnection Con = null;
            try
            {
                Int32 Output;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CommodityID");
                Cmd.Parameters.AddWithValue("@Commodity", Commodity);
                Output = Convert.ToInt32(Cmd.ExecuteScalar());
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public DataTable GetStockCategoryType(Object Category, Object Commodity, Object Quality, Object Unit, Object ItemCode = null)
        {
            SqlConnection Con = null;
            try
            {
                DataTable dt = new DataTable();
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "StockCategoryType");
                Cmd.Parameters.AddWithValue("@Category", Category);
                Cmd.Parameters.AddWithValue("@Commodity", Commodity);
                Cmd.Parameters.AddWithValue("@Quality", Quality);
                Cmd.Parameters.AddWithValue("@Unit", Unit);
                Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                SqlDataAdapter adr = new SqlDataAdapter(Cmd);
                adr.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Con.Close();
            }
        }

        public Int32 GetQualityID(string Quality)
        {
            SqlConnection Con = null;
            try
            {
                Int32 Output;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "QualityID");
                Cmd.Parameters.AddWithValue("@Quality", Quality);
                Output = Convert.ToInt32(Cmd.ExecuteScalar());
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public Int32 CreditID()
        {
            SqlConnection Con = null;
            try
            {
                Int32 Output;
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "CreditID");
                Output = Convert.ToInt32(Cmd.ExecuteScalar());
                Con.Close();
                return Output;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public bool IsCostCentre(object AccountID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                bool _IsCostCentre = false;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "IsCostCentre");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                _IsCostCentre = Convert.ToBoolean(Cmd.ExecuteScalar());
                Con.Close();
                return _IsCostCentre;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public object GetAdvanceType(object ID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Criteria", "GetAdvanceType");
                cmd.Parameters.AddWithValue("@ID", ID);
                Object GetAdvanceType = cmd.ExecuteScalar();
                Con.Close();
                return GetAdvanceType;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }

        public DataTable FillExistingItemIDs(string ExistingItemCode)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("ItemMasterSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Criteria", "FillExistingItems");
                cmd.Parameters.AddWithValue("@ExistingItems", ExistingItemCode);
                SqlDataAdapter dAdp = new SqlDataAdapter(cmd);
                dAdp.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable CancelledVouchers(SqlCommand Cmd)
        {
            try
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = "RecallVoucherSP";
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                Cmd.Connection.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable VouchersToClose(SqlCommand Cmd)
        {
            try
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandText = "CloseVoucherSP";
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                Cmd.Connection.Close();
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void RecallVoucher(DataTable dt, Object BranchID, Object UserID, String Reason = "", String MachineName = "")
        {
            SqlConnection Con = null;
            SqlTransaction Tx = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Tx = Con.BeginTransaction();
                SqlCommand Cmd = new SqlCommand("RecallVoucherSP", Con, Tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "Update");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@Reason", Reason);
                Cmd.Parameters.AddWithValue("@MachineName", MachineName);
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToBoolean(dr["Sel"]))
                    {
                        if (Cmd.Parameters.Contains("@ID"))
                        {
                            Cmd.Parameters["@ID"].Value = dr["ID"];
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                        }
                        Cmd.ExecuteNonQuery();
                    }
                }
                Tx.Commit();
                Con.Close();
            }
            catch (Exception)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Tx.Rollback();
                    Con.Close();
                }
            }
        }

        public void CloseVoucher(DataTable dt, Object BranchID, Object UserID, String Reason = "", String MachineName = "")
        {
            SqlConnection Con = null;
            SqlTransaction Tx = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Tx = Con.BeginTransaction();
                SqlCommand Cmd = new SqlCommand("CloseVoucherSP", Con, Tx);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "Update");
                Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                Cmd.Parameters.AddWithValue("@UserID", UserID);
                Cmd.Parameters.AddWithValue("@Reason", Reason);
                Cmd.Parameters.AddWithValue("@MachineName", MachineName);
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToBoolean(dr["Sel"]))
                    {
                        if (Cmd.Parameters.Contains("@ID"))
                        {
                            Cmd.Parameters["@ID"].Value = dr["ID"];
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@ID", dr["ID"]);
                        }
                        Cmd.ExecuteNonQuery();
                    }
                }
                Tx.Commit();
                Con.Close();
            }
            catch (Exception)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Tx.Rollback();
                    Con.Close();
                }
            }
        }

        public Object GetAllocatedAmount(Object TransactionID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GetAllocatedAmount");
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter dAdp = new SqlDataAdapter(Cmd);
                Object Obj = Cmd.ExecuteScalar();
                return Obj;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #region CommonLogic
        /// <summary>
        /// set Nature field in "FiTransactionEntries" table. 
        /// This is to be called before save of each finance transaction.
        /// </summary>
        /// <param name="FinEntriesTable">"FiTransactionEntries" table</param>
        /// 
        public bool SetNatureInEntries(DataTable FinEntriesTable)
        {
            try
            {
                bool Drflag = false;
                bool Crflag = false;
                foreach (DataRow row in FinEntriesTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        if (row["DrCr"].ToString() == "D")
                        {
                            if (!Drflag)
                            {
                                row["Nature"] = "M";
                                Drflag = true;
                            }
                            else if (row["Nature"].ToString() != string.Empty)
                            {
                                row["Nature"] = "";
                            }
                        }
                        else if (row["DrCr"].ToString() == "C")
                        {
                            if (!Crflag)
                            {
                                row["Nature"] = "M";
                                Crflag = true;
                            }
                            else if (row["Nature"].ToString() != string.Empty)
                            {
                                row["Nature"] = "";
                            }
                        }
                        else
                        {
                            // Error case for entry not D or C.
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// set Nature field in "FiTransactionEntries" table. 
        /// This is to be called before save of each finance transaction.
        /// </summary>
        /// <param name="FinEntriesTable">"FiTransactionEntries" table</param>
        /// 
        public void RemoveUnwantedItemRow(DataTable dtInvTransItem)
        {
            try
            {
                foreach (DataRow row in dtInvTransItem.Copy().Rows)
                {
                    if (row.RowState != DataRowState.Deleted && (row["ItemID"] == DBNull.Value || row["ItemID"] == null || row["ItemID"].ToString() == string.Empty))
                    {
                        dtInvTransItem.Rows.Find(row["ID"]).Delete();
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public Object AccountCode(Object AccountID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = null;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "AccountCode");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }
        public Object AccountName(Object AccountID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionNo = null;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "AccountName");
                Cmd.Parameters.AddWithValue("@AccountID", AccountID);
                TransactionNo = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionNo;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }
        public Object LastChequeTransactionID(Object ChequeID)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                Object TransactionID = null;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "LastChequeTransactionID");
                Cmd.Parameters.AddWithValue("@ChequeID", ChequeID);
                TransactionID = Cmd.ExecuteScalar();
                Con.Close();
                return TransactionID;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }
        public bool GeneralVoucherSettings()
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                Con.Open();
                bool GeneralVoucher = false;
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "GeneralVoucherSettings");
                GeneralVoucher = Convert.ToBoolean(Cmd.ExecuteScalar());
                Con.Close();
                return GeneralVoucher;
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open) Con.Close();
                throw Ex;
            }
        }
        private void SendMailMessage(string from, string Password, string to, string bcc, string cc, string subject, string body, string SMTP)
        {
            // Instantiate a new instance of MailMessage
            MailMessage mMailMessage = new MailMessage();

            // Set the sender address of the mail message
            mMailMessage.From = new MailAddress(from);
            // Set the recepient address of the mail messageg

            if (to.Contains(","))
            {
                string[] ToAddresses = to.Split(',');
                foreach (String S in ToAddresses)
                {
                    mMailMessage.To.Add(new MailAddress(S));
                }
            }
            else
            {
                mMailMessage.To.Add(new MailAddress(to));
            }
            // Check if the bcc value is null or an empty string
            if ((bcc != null) && (bcc != string.Empty))
            {
                // Set the Bcc address of the mail message
                mMailMessage.Bcc.Add(new MailAddress(bcc));
            }      // Check if the cc value is null or an empty value
            if ((cc != null) && (cc != string.Empty))
            {
                // Set the CC address of the mail message
                mMailMessage.CC.Add(new MailAddress(cc));
            }       // Set the subject of the mail message
            mMailMessage.Subject = subject;
            // Set the body of the mail message
            mMailMessage.Body = body;

            // Set the format of the mail message body as HTML
            mMailMessage.IsBodyHtml = true;
            // Set the priority of the mail message to normal
            mMailMessage.Priority = MailPriority.Normal;

            // Instantiate a new instance of SmtpClient            
            SmtpClient mSmtpClient = new SmtpClient();
            mSmtpClient.Port = 587;   // [1] You can try with 465 also, I always used 587 and got success
            mSmtpClient.EnableSsl = true;
            mSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
            mSmtpClient.UseDefaultCredentials = false; // [3] Changed this
            mSmtpClient.Credentials = new NetworkCredential(mMailMessage.From.Address, Password);  // [4] Added this. Note, first parameter is NOT string.
            mSmtpClient.Host = SMTP;

            // Send the mail message
            mSmtpClient.Send(mMailMessage);
        }
        private DataTable SendEmailToApproverDetails(object TransactionID)
        {
            try
            {
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("SendEmailToApproverDetailsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter adr = new SqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                adr.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SendEmailToApprover(Object TransactionID)
        {
            String ToAddress = "";
            DataTable dt = SendEmailToApproverDetails(TransactionID);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["EmailID"].ToString() != "")
                    {
                        if (ToAddress == "")
                        {
                            ToAddress = dr["EmailID"].ToString();
                        }
                        else
                        {
                            ToAddress += "," + dr["EmailID"].ToString();
                        }
                    }
                }
                if (dt.Rows[0]["FromAddress"].ToString() != "")
                {
                    if (dt.Rows[0]["EmailPassword"].ToString() != "")
                    {
                        if (dt.Rows[0]["EmailSMTP"].ToString() != "")
                        {
                            SendMailMessage(dt.Rows[0]["FromAddress"].ToString(), dt.Rows[0]["EmailPassword"].ToString(), ToAddress, "", "", dt.Rows[0]["Subject"].ToString(), dt.Rows[0]["Body"].ToString(), dt.Rows[0]["EmailSMTP"].ToString());
                        }
                    }
                }
            }
        }

        public DataTable FillSalesDaySummary(Object BranchID, Object StartDate, Object EndDate)
        {
            try
            {
                DataTable dt = new DataTable();
                if (ConnectionString != "")
                {
                    SqlConnection Con = new SqlConnection(ConnectionString);
                    SqlCommand Cmd = new SqlCommand("SalesDaySummarySP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@StartDate", StartDate);
                    Cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    new SqlDataAdapter(Cmd).Fill(dt);
                }
                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public void SaveSRVServiceTrans(DataTable dtSRVServiceTrans, SqlConnection Con, SqlTransaction Tx)
        {
            SqlCommand Cmd;
            foreach (DataRow dr in dtSRVServiceTrans.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    Cmd = new SqlCommand("SRVServiceSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", 2);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ServiceID", dr["ServiceID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@DiscountPerc", dr["DiscountPerc"]);
                    Cmd.Parameters.AddWithValue("@Discount", dr["Discount"]);
                    Cmd.Parameters.AddWithValue("@GrossAmount", dr["GrossAmount"]);
                    Cmd.Parameters.AddWithValue("@TaxPerc", dr["TaxPerc"]);
                    Cmd.Parameters.AddWithValue("@TaxValue", dr["TaxValue"]);
                    Cmd.Parameters.AddWithValue("@Total", dr["Total"]);
                    //Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput,4, false, 0, 0, "ID", DataRowVersion.Original, null));
                    Cmd.ExecuteNonQuery();
                    dr["ID"] = Cmd.Parameters["@NewID"].Value;
                    dr.EndEdit();
                }
                if (dr.RowState == DataRowState.Modified)
                {
                    Cmd = new SqlCommand("SRVServiceSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", 3);
                    Cmd.Parameters.AddWithValue("@TransactionID", dr["TransactionID"]);
                    Cmd.Parameters.AddWithValue("@ServiceID", dr["ServiceID"]);
                    Cmd.Parameters.AddWithValue("@AccountID", dr["AccountID"]);
                    Cmd.Parameters.AddWithValue("@Unit", dr["Unit"]);
                    Cmd.Parameters.AddWithValue("@Qty", dr["Qty"]);
                    Cmd.Parameters.AddWithValue("@Rate", dr["Rate"]);
                    Cmd.Parameters.AddWithValue("@Amount", dr["Amount"]);
                    Cmd.Parameters.AddWithValue("@DiscountPerc", dr["DiscountPerc"]);
                    Cmd.Parameters.AddWithValue("@Discount", dr["Discount"]);
                    Cmd.Parameters.AddWithValue("@GrossAmount", dr["GrossAmount"]);
                    Cmd.Parameters.AddWithValue("@TaxPerc", dr["TaxPerc"]);
                    Cmd.Parameters.AddWithValue("@TaxValue", dr["TaxValue"]);
                    Cmd.Parameters.AddWithValue("@Total", dr["Total"]);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
                if (dr.RowState == DataRowState.Deleted)
                {
                    Cmd = new SqlCommand("SRVServiceSP", Con, Tx);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", 4);
                    Cmd.Parameters.AddWithValue("@ID", dr["ID", DataRowVersion.Original]);
                    Cmd.ExecuteNonQuery();
                }
            }

        }
        public DataSet FillVoucher(Object BranchID, Object MaPageMenuID)
        {
            try
            {
                DataSet ds = new DataSet();
                if (string.IsNullOrEmpty(ConnectionString)) return ds;

                // Use 'using' to ensure connection is properly disposed of after the operation
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("LeftGridMasterSP", Con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    Cmd.Parameters.AddWithValue("@Criteria", "FillVoucher");
                    Cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    Cmd.Parameters.AddWithValue("@MaPageMenuID", MaPageMenuID);

                    // Fill the DataTable using the SqlDataAdapter
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Cmd))
                    {
                        adapter.Fill(ds);
                    }
                }
                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion CommonLogic

        public DataTable DataTableFillTransactions(Int64 ID)
        {
            try
            {
                DataTable dtTransactions = new DataTable();

                // Use 'using' to ensure that connection, command, and adapter are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherSPNew", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 1);
                        Cmd.Parameters.AddWithValue("@ID", ID);

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter sda = new SqlDataAdapter(Cmd))
                        {
                            sda.Fill(dtTransactions);
                        }
                    }
                }

                return dtTransactions;
            }
            catch (Exception Ex)
            {
                // Rethrow the exception after closing the resources
                throw Ex;
            }
        }

        public DataTable DataTableFillTransactionEntries(Int64 TransactionID)
        {
            try
            {
                DataTable dtTransactionEntries = new DataTable();
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "FillInvTransItems");
                    Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(dtTransactionEntries);
                }
                return dtTransactionEntries;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public DataTable DataTableFillTransactionAdditionals(Int64 TransactionID)
        {
            try
            {
                DataTable dtFiTransactionAdditionals = new DataTable();
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Criteria", "FillFiTransactionAdditionals");
                    Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                    SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                    sda.Fill(dtFiTransactionAdditionals);
                }
                return dtFiTransactionAdditionals;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public string DeleteTransactionEntries(Int64 ID)
        {
            try
            {
                // Use 'using' to ensure the connection and command are disposed of properly
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con))
                    {
                        // Set command type and parameters
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "DeleteInvTransItems");
                        Cmd.Parameters.AddWithValue("@ID", ID);

                        // Open connection and execute the query
                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        return "true";
                    }
                }
            }
            catch (Exception Ex)
            {
                // Return exception message if an error occurs
                return Ex.Message;
            }
        }

        public string DeleteFillTransactions(Int64 ID)
        {
            try
            {
                // Use 'using' to ensure connection and command are disposed of properly
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherSP", Con))
                    {
                        // Set command type and parameters
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "DeleteTransactions");
                        Cmd.Parameters.AddWithValue("@ID", ID);

                        // Open connection and execute the command
                        Con.Open();
                        int Result = Cmd.ExecuteNonQuery();

                        // Check if the operation was successful
                        if (Result != 0)
                        {
                            return "true";
                        }
                        else
                        {
                            return "Unable to delete Transaction";
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // Return exception message if an error occurs
                return Ex.Message;
            }
        }

        public string DeleteFiTransactionAdditionals(Int64 ID)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                Con.Open();
                SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "DeleteFiTransactionAdditionals");
                Cmd.Parameters.AddWithValue("@TransactionID", ID);
                Cmd.Connection.Open();
                var Result = Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                if (Result == null)
                {
                    return "true";
                }
                else
                {
                    return "Unable to delete Trasaction";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                    return Ex.Message;

                }
                return Ex.Message;
            }
        }

        public string InsertTransactions(Models.FiTransactions dtTransactions)
        {
            try
            {
                string NewID = string.Empty;

                // Use 'using' to ensure connection and command are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherSPNew", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 2);
                        Cmd.Parameters.AddWithValue("@Date", dtTransactions.Date);
                        Cmd.Parameters.AddWithValue("@EffectiveDate", dtTransactions.EffectiveDate);
                        Cmd.Parameters.AddWithValue("@VoucherID", dtTransactions.VoucherID);
                        Cmd.Parameters.AddWithValue("@SerialNo", dtTransactions.SerialNo);
                        Cmd.Parameters.AddWithValue("@TransactionNo", dtTransactions.TransactionNo);
                        Cmd.Parameters.AddWithValue("@IsPostDated", dtTransactions.IsPostDated);
                        Cmd.Parameters.AddWithValue("@CurrencyID", dtTransactions.CurrencyID);
                        Cmd.Parameters.AddWithValue("@PageID", dtTransactions.PageID);
                        Cmd.Parameters.AddWithValue("@ExchangeRate", dtTransactions.ExchangeRate);
                        Cmd.Parameters.AddWithValue("@RefPageTypeID", dtTransactions.RefPageTypeID);
                        Cmd.Parameters.AddWithValue("@RefPageTableID", dtTransactions.RefPageTableID);
                        Cmd.Parameters.AddWithValue("@ReferenceNo", dtTransactions.ReferenceNo);
                        Cmd.Parameters.AddWithValue("@CompanyID", dtTransactions.CompanyID);
                        Cmd.Parameters.AddWithValue("@FinYearID", dtTransactions.FinYearID);
                        Cmd.Parameters.AddWithValue("@InstrumentType", dtTransactions.InstrumentType);
                        Cmd.Parameters.AddWithValue("@InstrumentNo", dtTransactions.InstrumentNo);
                        Cmd.Parameters.AddWithValue("@InstrumentDate", dtTransactions.InstrumentDate);
                        Cmd.Parameters.AddWithValue("@InstrumentBank", dtTransactions.InstrumentBank);
                        Cmd.Parameters.AddWithValue("@CommonNarration", dtTransactions.CommonNarration);
                        Cmd.Parameters.AddWithValue("@AddedBy", dtTransactions.AddedBy);
                        Cmd.Parameters.AddWithValue("@ApprovedBy", dtTransactions.ApprovedBy);
                        Cmd.Parameters.AddWithValue("@AddedDate", dtTransactions.AddedDate);
                        Cmd.Parameters.AddWithValue("@ApprovedDate", dtTransactions.ApprovedDate);
                        Cmd.Parameters.AddWithValue("@ApprovalStatus", dtTransactions.ApprovalStatus);
                        Cmd.Parameters.AddWithValue("@ApproveNote", dtTransactions.ApproveNote);
                        Cmd.Parameters.AddWithValue("@Action", dtTransactions.Action);
                        Cmd.Parameters.AddWithValue("@RefTransID", dtTransactions.RefTransID);
                        Cmd.Parameters.AddWithValue("@StatusID", dtTransactions.StatusID);
                        Cmd.Parameters.AddWithValue("@IsAutoEntry", dtTransactions.IsAutoEntry);
                        Cmd.Parameters.AddWithValue("@Posted", dtTransactions.Posted);
                        Cmd.Parameters.AddWithValue("@Active", dtTransactions.Active);
                        Cmd.Parameters.AddWithValue("@Cancelled", dtTransactions.Cancelled);
                        Cmd.Parameters.AddWithValue("@AccountID", dtTransactions.AccountID);
                        Cmd.Parameters.AddWithValue("@Description", dtTransactions.Description);
                        Cmd.Parameters.AddWithValue("@CostCentreID", dtTransactions.CostCentreID);
                        Cmd.Parameters.AddWithValue("@MachineName", dtTransactions.MachineName);

                        if (dtTransactions.DeliveryCharge != null && dtTransactions.DeliveryCharge.ToString() != "")
                        {
                            Cmd.Parameters.AddWithValue("@DeliveryCharge", dtTransactions.DeliveryCharge);
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@DeliveryCharge", DBNull.Value);
                        }

                        Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt) { Direction = ParameterDirection.InputOutput });
                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close(); //Added By Rafi on 15/11/2025
                        NewID = Convert.ToString(Cmd.Parameters["@NewID"].Value);
                    }
                }

                return NewID;
            }
            catch (Exception Ex)
            {
                // Exception handling
                return Ex.Message;
            }
        }

        public string UpdateTransactions(Models.FiTransactions dtTransactions)
        {
            try
            {
                string result = string.Empty;

                // Use 'using' to ensure connection and command are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherSPNew", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 3);
                        Cmd.Parameters.AddWithValue("@Date", dtTransactions.Date);
                        Cmd.Parameters.AddWithValue("@EffectiveDate", dtTransactions.EffectiveDate);
                        Cmd.Parameters.AddWithValue("@VoucherID", dtTransactions.VoucherID);
                        Cmd.Parameters.AddWithValue("@SerialNo", dtTransactions.SerialNo);
                        Cmd.Parameters.AddWithValue("@TransactionNo", dtTransactions.TransactionNo);
                        Cmd.Parameters.AddWithValue("@IsPostDated", dtTransactions.IsPostDated);
                        Cmd.Parameters.AddWithValue("@CurrencyID", dtTransactions.CurrencyID);
                        Cmd.Parameters.AddWithValue("@ExchangeRate", dtTransactions.ExchangeRate);
                        Cmd.Parameters.AddWithValue("@RefPageTypeID", dtTransactions.RefPageTypeID);
                        Cmd.Parameters.AddWithValue("@RefPageTableID", dtTransactions.RefPageTableID);
                        Cmd.Parameters.AddWithValue("@ReferenceNo", dtTransactions.ReferenceNo);
                        Cmd.Parameters.AddWithValue("@CompanyID", dtTransactions.CompanyID);
                        Cmd.Parameters.AddWithValue("@FinYearID", dtTransactions.FinYearID);
                        Cmd.Parameters.AddWithValue("@InstrumentType", dtTransactions.InstrumentType);
                        Cmd.Parameters.AddWithValue("@InstrumentNo", dtTransactions.InstrumentNo);
                        Cmd.Parameters.AddWithValue("@InstrumentDate", dtTransactions.InstrumentDate);
                        Cmd.Parameters.AddWithValue("@InstrumentBank", dtTransactions.InstrumentBank);
                        Cmd.Parameters.AddWithValue("@CommonNarration", dtTransactions.CommonNarration);
                        Cmd.Parameters.AddWithValue("@AddedBy", dtTransactions.AddedBy);
                        Cmd.Parameters.AddWithValue("@ApprovedBy", dtTransactions.ApprovedBy);
                        Cmd.Parameters.AddWithValue("@AddedDate", dtTransactions.AddedDate);
                        Cmd.Parameters.AddWithValue("@ApprovedDate", dtTransactions.ApprovedDate);
                        Cmd.Parameters.AddWithValue("@ApprovalStatus", dtTransactions.ApprovalStatus);
                        Cmd.Parameters.AddWithValue("@ApproveNote", dtTransactions.ApproveNote);
                        Cmd.Parameters.AddWithValue("@Action", dtTransactions.Action);
                        Cmd.Parameters.AddWithValue("@StatusID", dtTransactions.StatusID);
                        Cmd.Parameters.AddWithValue("@IsAutoEntry", dtTransactions.IsAutoEntry);
                        Cmd.Parameters.AddWithValue("@Posted", dtTransactions.Posted);
                        Cmd.Parameters.AddWithValue("@Active", dtTransactions.Active);
                        Cmd.Parameters.AddWithValue("@Cancelled", dtTransactions.Cancelled);
                        Cmd.Parameters.AddWithValue("@AccountID", dtTransactions.AccountID);
                        Cmd.Parameters.AddWithValue("@Description", dtTransactions.Description);
                        Cmd.Parameters.AddWithValue("@RefTransID", dtTransactions.RefTransID);
                        Cmd.Parameters.AddWithValue("@CostCentreID", dtTransactions.CostCentreID);
                        Cmd.Parameters.AddWithValue("@PageID", dtTransactions.PageID);
                        Cmd.Parameters.AddWithValue("@MachineName", dtTransactions.MachineName);
                        Cmd.Parameters.AddWithValue("@ID", dtTransactions.ID);

                        if (dtTransactions.DeliveryCharge != null && dtTransactions.DeliveryCharge.ToString() != "")
                        {
                            Cmd.Parameters.AddWithValue("@DeliveryCharge", dtTransactions.DeliveryCharge);
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@DeliveryCharge", DBNull.Value);
                        }

                        Con.Open();
                        int resultCount = Cmd.ExecuteNonQuery();
                        Con.Close();// Added by Rafi on 15/11/2025
                        if (resultCount != 0)
                        {
                            result = "true";
                        }
                        else
                        {
                            result = "Unable to update category";
                        }
                    }
                }

                return result;
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        //public DataSet ProductAvailableUnits(Int64 ID)
        //{
        //    try
        //    {
        //        // Use 'using' to ensure connection is properly disposed of after the operation
        //        using (SqlConnection Con = new SqlConnection(ConnectionString))
        //        {
        //            // Use the connection directly in the SqlDataAdapter constructor
        //            SqlDataAdapter General = new SqlDataAdapter("SELECT * FROM InvItemUnits WHERE ItemID = @ID ORDER BY IsDefault DESC;" +
        //                                                        "SELECT * FROM InvItemMaster WHERE ID = @ID;SELECT * FROM InvItemImages WHERE ItemID=@ID;",
        //                                                        Con);

        //            General.SelectCommand.Parameters.AddWithValue("@ID", ID);

        //            DataSet Results = new DataSet();
        //            General.Fill(Results);

        //            return Results;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        // If an exception occurs, it will be handled here
        //        throw Ex;
        //    }
        //}
        public DataSet ProductAvailableUnits(Int64 ID)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("VoucherSPNew", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 8);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds; // Contains 2 tables
                }
            }
        }

        public DataSet ItemAvailableUnitForExcel(int ID)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ItemMasterExcelUploadSP", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 4);
                    cmd.Parameters.AddWithValue("@ItemID1", ID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds; // Contains 2 tables
                }
            }
        }

        public DataTable ProductTaxDetails(Int64 ID)
        {
            try
            {
                // Use 'using' to ensure that connection and data adapter are disposed of properly
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("SELECT * FROM MaTaxType WHERE ID = @ID;", Con))
                    {
                        // Add parameters to the command
                        Cmd.Parameters.AddWithValue("@ID", ID);

                        // Open the connection
                        Con.Open();

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter General = new SqlDataAdapter(Cmd))
                        {
                            DataTable DtDetails = new DataTable();
                            General.Fill(DtDetails);
                            return DtDetails;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // Handle exceptions by throwing the original exception after resources are disposed
                throw Ex;
            }
        }

        public string GetItemUnitPrice(int ItemID, int VoucherID, int AccountID, long BranchID, string Unit)
        {
            try
            {
                // Use 'using' to ensure connection and command are disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT dbo.GetMarginprice(@BranchID, @ItemID, @AccountID, @VoucherID, @Unit)", Con))
                    {
                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@ItemID", ItemID);
                        cmd.Parameters.AddWithValue("@VoucherID", VoucherID);
                        cmd.Parameters.AddWithValue("@AccountID", AccountID);
                        cmd.Parameters.AddWithValue("@Unit", Unit);

                        // Open connection and execute query
                        Con.Open();
                        string Price = cmd.ExecuteScalar()?.ToString();
                        return Price ?? string.Empty;  // Return empty string if null
                    }
                }
            }
            catch (Exception Ex)
            {
                // Handle exceptions
                throw Ex;
            }
        }


        public DataTable UnitDetails(int itemid,string unit)
        {
            try
            {
                // Use 'using' to ensure that connection and data adapter are disposed of properly
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("SELECT * FROM InvItemUnits WHERE ItemID=@ItemID AND Unit = @Unit;", Con))
                    {
                        // Add parameters to the command
                        Cmd.Parameters.AddWithValue("@ItemID", itemid);
                        Cmd.Parameters.AddWithValue("@Unit", unit);

                        // Open the connection
                        Con.Open();

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter General = new SqlDataAdapter(Cmd))
                        {
                            DataTable DtDetails = new DataTable();
                            General.Fill(DtDetails);
                            return DtDetails;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // Handle exceptions by throwing the original exception after resources are disposed
                throw Ex;
            }
        }


        public string InsertInvTransItems(Models.InvTransItems InvTransItems)
        {
            try
            {
                // Use 'using' for automatic disposal of resources
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "InsertInvTransItems");
                        Cmd.Parameters.AddWithValue("@TransactionID", InvTransItems.TransactionID);
                        Cmd.Parameters.AddWithValue("@ItemID", InvTransItems.ItemID);
                        Cmd.Parameters.AddWithValue("@SerialNo", InvTransItems.SerialNo);
                        Cmd.Parameters.AddWithValue("@Unit", InvTransItems.Unit);
                        Cmd.Parameters.AddWithValue("@Qty", InvTransItems.Qty);
                        Cmd.Parameters.AddWithValue("@FOCQty", InvTransItems.FOCQty);
                        Cmd.Parameters.AddWithValue("@BasicQty", InvTransItems.BasicQty);
                        Cmd.Parameters.AddWithValue("@Pcs", InvTransItems.Pcs);
                        Cmd.Parameters.AddWithValue("@Rate", InvTransItems.Rate);
                        Cmd.Parameters.AddWithValue("@AdvanceRate", InvTransItems.AdvanceRate);
                        Cmd.Parameters.AddWithValue("@OtherRate", InvTransItems.OtherRate);
                        Cmd.Parameters.AddWithValue("@MasterMiscID1", InvTransItems.MasterMiscID1);
                        Cmd.Parameters.AddWithValue("@RowType", InvTransItems.RowType);
                        Cmd.Parameters.AddWithValue("@Description", InvTransItems.Description);
                        Cmd.Parameters.AddWithValue("@Remarks", InvTransItems.Remarks);
                        Cmd.Parameters.AddWithValue("@IsBit", InvTransItems.IsBit);
                        Cmd.Parameters.AddWithValue("@InvAvgCostID", InvTransItems.InvAvgCostID);
                        Cmd.Parameters.AddWithValue("@IsReturn", InvTransItems.IsReturn);
                        Cmd.Parameters.AddWithValue("@Discount", InvTransItems.Discount);
                        Cmd.Parameters.AddWithValue("@Additional", InvTransItems.Additional);
                        Cmd.Parameters.AddWithValue("@Factor", InvTransItems.Factor);
                        Cmd.Parameters.AddWithValue("@CommodityID", InvTransItems.CommodityID);
                        Cmd.Parameters.AddWithValue("@AccountID", InvTransItems.AccountID);
                        Cmd.Parameters.AddWithValue("@LengthFt", InvTransItems.LengthFt);
                        Cmd.Parameters.AddWithValue("@LengthIn", InvTransItems.LengthIn);
                        Cmd.Parameters.AddWithValue("@LengthCm", InvTransItems.LengthCm);
                        Cmd.Parameters.AddWithValue("@GirthFt", InvTransItems.GirthFt);
                        Cmd.Parameters.AddWithValue("@GirthIn", InvTransItems.GirthIn);
                        Cmd.Parameters.AddWithValue("@GirthCm", InvTransItems.GirthCm);
                        Cmd.Parameters.AddWithValue("@ThicknessFt", InvTransItems.ThicknessFt);
                        Cmd.Parameters.AddWithValue("@ThicknessIn", InvTransItems.ThicknessIn);
                        Cmd.Parameters.AddWithValue("@ThicknessCm", InvTransItems.ThicknessCm);
                        Cmd.Parameters.AddWithValue("@TransactionEntryID", InvTransItems.TransactionEntryID);
                        Cmd.Parameters.AddWithValue("@Status", InvTransItems.Status);
                        Cmd.Parameters.AddWithValue("@Cancel", InvTransItems.Cancel);
                        Cmd.Parameters.AddWithValue("@MeasuredByID", InvTransItems.MeasuredByID);
                        Cmd.Parameters.AddWithValue("@RefTransItemID", InvTransItems.RefTransItemID);
                        Cmd.Parameters.AddWithValue("@FinishDate", InvTransItems.FinishDate);
                        Cmd.Parameters.AddWithValue("@UpdateDate", InvTransItems.UpdateDate);
                        Cmd.Parameters.AddWithValue("@IsSameForPcs", InvTransItems.IsSameForPcs);
                        Cmd.Parameters.AddWithValue("@StockQty", InvTransItems.StockQty);
                        Cmd.Parameters.AddWithValue("@Margin", InvTransItems.Margin);
                        Cmd.Parameters.AddWithValue("@InlocID", InvTransItems.InLocID);
                        Cmd.Parameters.AddWithValue("@OutLocID", InvTransItems.OutLocID);
                        Cmd.Parameters.AddWithValue("@BatchNo", InvTransItems.BatchNo);
                        Cmd.Parameters.AddWithValue("@SizeMasterID", InvTransItems.SizeMasterID);
                        Cmd.Parameters.AddWithValue("@DiscountPerc", InvTransItems.DiscountPerc);
                        Cmd.Parameters.AddWithValue("@TaxPerc", InvTransItems.TaxPerc);
                        Cmd.Parameters.AddWithValue("@TaxValue", InvTransItems.TaxValue);
                        Cmd.Parameters.AddWithValue("@TaxTypeID", InvTransItems.TaxTypeID);
                        Cmd.Parameters.AddWithValue("@TaxAccountID", InvTransItems.TaxAccountID);
                        Cmd.Parameters.AddWithValue("@TranType", InvTransItems.TranType);
                        Cmd.Parameters.AddWithValue("@CostPerc", InvTransItems.CostPerc);
                        Cmd.Parameters.AddWithValue("@ManufactureDate", InvTransItems.ManufactureDate);
                        Cmd.Parameters.AddWithValue("@ExpiryDate", InvTransItems.ExpiryDate);
                        Cmd.Parameters.AddWithValue("@PriceCategoryID", InvTransItems.PriceCategoryID);
                        Cmd.Parameters.AddWithValue("@GroupItemID", InvTransItems.GroupItemID);
                        Cmd.Parameters.AddWithValue("@RateDisc", InvTransItems.RateDisc);
                        Cmd.Parameters.AddWithValue("@RefID", InvTransItems.RefID);
                        Cmd.Parameters.AddWithValue("@TempQty", InvTransItems.TempQty);
                        Cmd.Parameters.AddWithValue("@TempRate", InvTransItems.Rate);
                        Cmd.Parameters.AddWithValue("@ReplaceQty", InvTransItems.ReplaceQty);
                        Cmd.Parameters.AddWithValue("@PrintedMRP", InvTransItems.PrintedMRP);
                        Cmd.Parameters.AddWithValue("@PrintedRate", InvTransItems.PrintedRate);
                        Cmd.Parameters.AddWithValue("@PTSRate", InvTransItems.PTSRate);
                        Cmd.Parameters.AddWithValue("@PTRRate", InvTransItems.PTRRate);
                        Cmd.Parameters.AddWithValue("@StockItemID", InvTransItems.StockItemID);

                        // Execute Insert
                        Con.Open();
                        Cmd.ExecuteNonQuery();

                        // Fetch the last inserted ID
                        using (SqlCommand cmd = new SqlCommand("Select Top 1 ID FROM InvTransItems ORDER BY ID DESC", Con))
                        {
                            using (SqlDataReader LastInsertedID = cmd.ExecuteReader())
                            {
                                string PanelID = "";
                                if (LastInsertedID.Read())
                                {
                                    PanelID = LastInsertedID["ID"].ToString();
                                }
                                return PanelID;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // Handle connection close in case of an exception
                throw Ex;
            }
        }


        public string UpdateInvTransItems(Models.InvTransItems InvTransItems)
        {
            try
            {
                // Use 'using' for automatic disposal of resources
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "UpdateInvTransItems");
                        Cmd.Parameters.AddWithValue("@ID", InvTransItems.ID);
                        Cmd.Parameters.AddWithValue("@TransactionID", InvTransItems.TransactionID);
                        Cmd.Parameters.AddWithValue("@ItemID", InvTransItems.ItemID);
                        Cmd.Parameters.AddWithValue("@SerialNo", InvTransItems.SerialNo);
                        Cmd.Parameters.AddWithValue("@Unit", InvTransItems.Unit);
                        Cmd.Parameters.AddWithValue("@Qty", InvTransItems.Qty);
                        Cmd.Parameters.AddWithValue("@FOCQty", InvTransItems.FOCQty);
                        Cmd.Parameters.AddWithValue("@BasicQty", InvTransItems.BasicQty);
                        Cmd.Parameters.AddWithValue("@Pcs", InvTransItems.Pcs);
                        Cmd.Parameters.AddWithValue("@Rate", InvTransItems.Rate);
                        Cmd.Parameters.AddWithValue("@AdvanceRate", InvTransItems.AdvanceRate);
                        Cmd.Parameters.AddWithValue("@OtherRate", InvTransItems.OtherRate);
                        Cmd.Parameters.AddWithValue("@MasterMiscID1", InvTransItems.MasterMiscID1);
                        Cmd.Parameters.AddWithValue("@RowType", InvTransItems.RowType);
                        Cmd.Parameters.AddWithValue("@Description", InvTransItems.Description);
                        Cmd.Parameters.AddWithValue("@Remarks", InvTransItems.Remarks);
                        Cmd.Parameters.AddWithValue("@IsBit", InvTransItems.IsBit);
                        Cmd.Parameters.AddWithValue("@InvAvgCostID", InvTransItems.InvAvgCostID);
                        Cmd.Parameters.AddWithValue("@IsReturn", InvTransItems.IsReturn);
                        Cmd.Parameters.AddWithValue("@Discount", InvTransItems.Discount);
                        Cmd.Parameters.AddWithValue("@Additional", InvTransItems.Additional);
                        Cmd.Parameters.AddWithValue("@Factor", InvTransItems.Factor);
                        Cmd.Parameters.AddWithValue("@CommodityID", InvTransItems.CommodityID);
                        Cmd.Parameters.AddWithValue("@AccountID", InvTransItems.AccountID);
                        Cmd.Parameters.AddWithValue("@LengthFt", InvTransItems.LengthFt);
                        Cmd.Parameters.AddWithValue("@LengthIn", InvTransItems.LengthIn);
                        Cmd.Parameters.AddWithValue("@LengthCm", InvTransItems.LengthCm);
                        Cmd.Parameters.AddWithValue("@GirthFt", InvTransItems.GirthFt);
                        Cmd.Parameters.AddWithValue("@GirthIn", InvTransItems.GirthIn);
                        Cmd.Parameters.AddWithValue("@GirthCm", InvTransItems.GirthCm);
                        Cmd.Parameters.AddWithValue("@ThicknessFt", InvTransItems.ThicknessFt);
                        Cmd.Parameters.AddWithValue("@ThicknessIn", InvTransItems.ThicknessIn);
                        Cmd.Parameters.AddWithValue("@ThicknessCm", InvTransItems.ThicknessCm);
                        Cmd.Parameters.AddWithValue("@TransactionEntryID", InvTransItems.TransactionEntryID);
                        Cmd.Parameters.AddWithValue("@Status", InvTransItems.Status);
                        Cmd.Parameters.AddWithValue("@Cancel", InvTransItems.Cancel);
                        Cmd.Parameters.AddWithValue("@MeasuredByID", InvTransItems.MeasuredByID);
                        Cmd.Parameters.AddWithValue("@RefTransItemID", InvTransItems.RefTransItemID);
                        Cmd.Parameters.AddWithValue("@FinishDate", InvTransItems.FinishDate);
                        Cmd.Parameters.AddWithValue("@UpdateDate", InvTransItems.UpdateDate);
                        Cmd.Parameters.AddWithValue("@IsSameForPcs", InvTransItems.IsSameForPcs);
                        Cmd.Parameters.AddWithValue("@StockQty", InvTransItems.StockQty);
                        Cmd.Parameters.AddWithValue("@Margin", InvTransItems.Margin);
                        Cmd.Parameters.AddWithValue("@InlocID", InvTransItems.InLocID);
                        Cmd.Parameters.AddWithValue("@OutLocID", InvTransItems.OutLocID);
                        Cmd.Parameters.AddWithValue("@BatchNo", InvTransItems.BatchNo);
                        Cmd.Parameters.AddWithValue("@SizeMasterID", InvTransItems.SizeMasterID);
                        Cmd.Parameters.AddWithValue("@DiscountPerc", InvTransItems.DiscountPerc);
                        Cmd.Parameters.AddWithValue("@TaxPerc", InvTransItems.TaxPerc);
                        Cmd.Parameters.AddWithValue("@TaxValue", InvTransItems.TaxValue);
                        Cmd.Parameters.AddWithValue("@TaxTypeID", InvTransItems.TaxTypeID);
                        Cmd.Parameters.AddWithValue("@TaxAccountID", InvTransItems.TaxAccountID);
                        Cmd.Parameters.AddWithValue("@TranType", InvTransItems.TranType);
                        Cmd.Parameters.AddWithValue("@CostPerc", InvTransItems.CostPerc);
                        Cmd.Parameters.AddWithValue("@ManufactureDate", InvTransItems.ManufactureDate);
                        Cmd.Parameters.AddWithValue("@ExpiryDate", InvTransItems.ExpiryDate);
                        Cmd.Parameters.AddWithValue("@PriceCategoryID", InvTransItems.PriceCategoryID);
                        Cmd.Parameters.AddWithValue("@GroupItemID", InvTransItems.GroupItemID);
                        Cmd.Parameters.AddWithValue("@RateDisc", InvTransItems.RateDisc);
                        Cmd.Parameters.AddWithValue("@RefID", InvTransItems.RefID);
                        Cmd.Parameters.AddWithValue("@TempQty", InvTransItems.TempQty);
                        Cmd.Parameters.AddWithValue("@TempRate", InvTransItems.Rate);
                        Cmd.Parameters.AddWithValue("@ReplaceQty", InvTransItems.ReplaceQty);
                        Cmd.Parameters.AddWithValue("@PrintedMRP", InvTransItems.PrintedMRP);
                        Cmd.Parameters.AddWithValue("@PrintedRate", InvTransItems.PrintedRate);
                        Cmd.Parameters.AddWithValue("@PTSRate", InvTransItems.PTSRate);
                        Cmd.Parameters.AddWithValue("@PTRRate", InvTransItems.PTRRate);
                        Cmd.Parameters.AddWithValue("@StockItemID", InvTransItems.StockItemID);

                        // Execute Update
                        Con.Open();
                        object Result = Cmd.ExecuteNonQuery();
                        if (Result != null)
                        {
                            return "true";
                        }
                        else
                        {
                            return "Unable to update items";
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                // Handle connection close in case of an exception
                throw Ex;
            }
        }

        public void InvTransItemsMaster(Models.InvTransItems InvTransItems)
        {
            SqlConnection Con = null;
            try
            {
                Con = new SqlConnection(ConnectionString);
                
                using (Con)
                {
                    if (InvTransItems.ID == null || InvTransItems.ID == 0)
                    {
                        InsertInvTransItems(InvTransItems);
                    }
                    else
                    {
                        UpdateInvTransItems(InvTransItems);
                    }
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public string UpdatePanelDetailsWithDealTransaction(Models.FiTransactions FiTransactions)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Con.Open();
                Cmd.CommandText = "SELECT * FROM MaMisc WHERE [Key] = 'EcommercePanelTypes' AND code like '%Deals%' ";
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    int ID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                    dt.Clear();
                    Cmd = new SqlCommand();
                    Cmd.Connection = new SqlConnection(ConnectionString);
                    Cmd.CommandText = "select ID,Active from EcomPanelMaster where PanelTypeID=@ID ";
                    Cmd.Parameters.AddWithValue("@ID", ID);
                    dt = new DataTable();
                    new SqlDataAdapter(Cmd).Fill(dt);
                    if (dt.Rows.Count != 0)
                    {
                        return "true";
                    }
                    else
                    {
                        Con.Open();
                        SqlCommand Cmd2 = new SqlCommand("EcomPanelMasterSP", Con);
                        Cmd2.CommandType = CommandType.StoredProcedure;

                        //Mandatory
                        Cmd2.Parameters.AddWithValue("@Title", FiTransactions.Description);
                        Cmd2.Parameters.AddWithValue("@ArabicTitle", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@Date", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@StartDate", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@EndDate", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@Remarks", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@CategoryID", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@BrandID", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@NDaysBefore", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@OrderNo", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@Active", 1);
                        Cmd2.Parameters.AddWithValue("@UserID", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@PanelTypeID", Convert.ToInt32(dt.Rows[0]["ID"].ToString()));
                        var Check = Cmd2.ExecuteNonQuery();
                        if (Check != null)
                        {
                            return "true";
                        }
                        else
                        {
                            return "Unable to add entities";
                        }
                    }
                }
                else
                {
                    return "Unable to add entities";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public string UpdatePanelDetailsWithDealProducts(Models.InvTransItems TransItem)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Con.Open();
                Cmd.CommandText = "SELECT * FROM MaMisc WHERE [Key] = 'EcommercePanelTypes' AND code like '%Deals%' ";
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    int ID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                    dt.Clear();
                    Cmd = new SqlCommand();
                    Cmd.Connection = new SqlConnection(ConnectionString);
                    Cmd.CommandText = "select ID,Active from EcomPanelMaster where PanelTypeID=@ID ";
                    Cmd.Parameters.AddWithValue("@ID", ID);
                    dt = new DataTable();
                    new SqlDataAdapter(Cmd).Fill(dt);
                    if (dt.Rows.Count != 0)
                    {
                        int PanelID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                        Cmd = new SqlCommand();
                        Cmd.Connection = new SqlConnection(ConnectionString);
                        Cmd.CommandText = "delete from EcomPanelProducts where PanelID=@ID ";
                        Cmd.Parameters.AddWithValue("@ID", PanelID);
                        Cmd.Connection.Open();
                        Cmd.ExecuteNonQuery();
                        Cmd.Connection.Close();
                        SqlCommand Cmd1 = new SqlCommand("EcomPanelMasterSP", Con);
                        Cmd1.CommandType = CommandType.StoredProcedure;
                        Cmd1.Parameters.AddWithValue("@Mode", 22);
                        Cmd1.Parameters.AddWithValue("@PanelID", PanelID);
                        Cmd1.Parameters.AddWithValue("@ItemID", Convert.ToInt64(TransItem.ItemID));
                        Cmd1.Parameters.AddWithValue("@UserID", null);
                        Cmd1.Parameters.AddWithValue("@Active", true);
                        //complete all fields
                        var result = Cmd1.ExecuteNonQuery();
                        Con.Close();
                    }
                    return "true";
                }
                else
                {
                    return "Unable to add entities";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public string UpdatePanelDetailsWithDeals(Models.FiTransactions FiTransactions, List<Models.InvTransItems> InvTransItems)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Con.Open();
                Cmd.CommandText = "SELECT * FROM MaMisc WHERE [Key] = 'EcommercePanelTypes' AND code like '%Deals%' ";
                DataTable dt = new DataTable();
                new SqlDataAdapter(Cmd).Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    int ID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                    dt.Clear();
                    Cmd = new SqlCommand();
                    Cmd.Connection = new SqlConnection(ConnectionString);
                    Cmd.CommandText = "select ID,Active from EcomPanelMaster where PanelTypeID=@ID ";
                    Cmd.Parameters.AddWithValue("@ID", ID);
                    dt = new DataTable();
                    new SqlDataAdapter(Cmd).Fill(dt);
                    if (dt.Rows.Count != 0)
                    {
                        int PanelID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                        Cmd = new SqlCommand();
                        Cmd.Connection = new SqlConnection(ConnectionString);
                        Cmd.CommandText = "delete from EcomPanelProducts where PanelID=@ID ";
                        Cmd.Parameters.AddWithValue("@ID", PanelID);
                        Cmd.Connection.Open();
                        Cmd.ExecuteNonQuery();
                        Cmd.Connection.Close();

                        foreach (Models.InvTransItems TransItem in InvTransItems)
                        {

                            SqlCommand Cmd1 = new SqlCommand("EcomPanelMasterSP", Con);
                            Cmd1.CommandType = CommandType.StoredProcedure;
                            Cmd1.Parameters.AddWithValue("@Mode", 22);
                            Cmd1.Parameters.AddWithValue("@PanelID", PanelID);
                            Cmd1.Parameters.AddWithValue("@ItemID", Convert.ToInt64(TransItem.ItemID));
                            Cmd1.Parameters.AddWithValue("@UserID", null);
                            Cmd1.Parameters.AddWithValue("@Active", true);
                            //complete all fields
                            var result = Cmd1.ExecuteNonQuery();
                        }
                        Con.Close();
                    }
                    else
                    {
                        Con.Open();
                        SqlCommand Cmd2 = new SqlCommand("EcomPanelMasterSP", Con);
                        Cmd2.CommandType = CommandType.StoredProcedure;

                        //Mandatory
                        Cmd2.Parameters.AddWithValue("@Title", FiTransactions.Description);
                        Cmd2.Parameters.AddWithValue("@ArabicTitle", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@Date", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@StartDate", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@EndDate", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@Remarks", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@CategoryID", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@BrandID", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@NDaysBefore", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@OrderNo", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@Active", 1);
                        Cmd2.Parameters.AddWithValue("@UserID", DBNull.Value);
                        Cmd2.Parameters.AddWithValue("@PanelTypeID", Convert.ToInt32(dt.Rows[0]["ID"].ToString()));
                        var Check = Cmd2.ExecuteNonQuery();
                        if (Check != null)
                        {
                            Con.Open();
                            SqlCommand cmd = new SqlCommand("Select Top  1 ID FROM EcomPanelMaster ORDER BY ID DESC", Con);
                            SqlDataReader LastInsertedID = cmd.ExecuteReader();
                            long PanelID = 0;
                            while (LastInsertedID.Read())
                            {
                                PanelID = Convert.ToInt32(LastInsertedID["ID"]);

                            }
                            Con.Close();
                            foreach (Models.InvTransItems TransItem in InvTransItems)
                            {

                                SqlCommand Cmd1 = new SqlCommand("EcomPanelMasterSP", Con);
                                Cmd1.CommandType = CommandType.StoredProcedure;
                                Cmd1.Parameters.AddWithValue("@Mode", 22);
                                Cmd1.Parameters.AddWithValue("@PanelID", PanelID);
                                Cmd1.Parameters.AddWithValue("@ItemID", Convert.ToInt64(TransItem.ItemID));
                                Cmd1.Parameters.AddWithValue("@UserID", null);
                                Cmd1.Parameters.AddWithValue("@Active", true);
                                //complete all fields
                                var result = Cmd1.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            return "Unable to map entities";
                        }
                    }
                    return "true";
                }
                else
                {
                    return "Unable to map entities";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public DataTable DataTableFillTransactionPromotionEntries(Int64 TransactionID)
        {
            try
            {
                DataTable dtTransactionEntries = new DataTable();
                SqlConnection Con = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("SELECT * FROM InvTransItems join InvItemMaster ON InvItemMaster.ID= InvTransItems.ItemID WHERE TransactionID=@TransactionID", Con);
                Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                SqlDataAdapter sda = new SqlDataAdapter(Cmd);
                sda.Fill(dtTransactionEntries);
                return dtTransactionEntries;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public string InsertFiTransactionAdditionals(Models.General.Transactions.FiTransactionAdditionals FiTransactionAdditionals)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlCommand Cmd;
            try
            {
                Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "InsertFiTransactionAdditionals");
                Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionAdditionals.TransactionID);
                if (FiTransactionAdditionals.StartDate != null && FiTransactionAdditionals.StartDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@StartDate", FiTransactionAdditionals.StartDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                }
                if (FiTransactionAdditionals.EndDate != null && FiTransactionAdditionals.EndDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@EndDate", FiTransactionAdditionals.EndDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                }
                Cmd.Connection.Open();
                object Result = Cmd.ExecuteNonQuery();
                if (Result != null)
                {
                    Cmd.Parameters.Clear();
                    Cmd.Connection.Close();
                    return "true";
                }
                else
                {
                    Cmd.Connection.Close();
                    return "Unable to add FiTransactionAdditionals";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public string UpdateFiTransactionAdditionals(Models.General.Transactions.FiTransactionAdditionals FiTransactionAdditionals)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlCommand Cmd;
            try
            {
                Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateFiTransactionAdditionals");
                Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionAdditionals.TransactionID);
                if (FiTransactionAdditionals.StartDate != null && FiTransactionAdditionals.StartDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@StartDate", FiTransactionAdditionals.StartDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
                }
                if (FiTransactionAdditionals.EndDate != null && FiTransactionAdditionals.EndDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@EndDate", FiTransactionAdditionals.EndDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
                }
                Cmd.Connection.Open();
                object Result = Cmd.ExecuteNonQuery();
                if (Result != null)
                {
                    Cmd.Parameters.Clear();
                    Cmd.Connection.Close();
                    return "true";
                }
                else
                {
                    Cmd.Connection.Close();
                    return "Unable to update FiTransactionAdditionals";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public DataSet FillEcommerceStatus(Int64 TransactionID)
        {
            try
            {
                SqlDataAdapter General = new SqlDataAdapter("select EcomOrderStatus.ID,Date As StatusUpdatedOn,Remarks as Status,Name,Mobile  from EcomOrderStatus join MaMisc on MaMisc.ID=EcomOrderStatus.StatusID INNER JOIN EcomUsers ON EcomUsers.ID=EcomOrderStatus.UserID  where MaMisc.[Key]='EcommerceOrderStatus' AND EcomOrderStatus.VID=@TransactionID ORDER By EcomOrderStatus.ID DESC;"
                + "SELECT  * FROM MaMisc WHERE MaMisc.[Key] = 'EcommerceOrderStatus' and [Value] in ('Order Packed','Order Shipped','Order Delivered') and code>(select Max(MM.Code) from EcomOrderStatus ES  inner join MaMisc MM ON MM.ID=ES.StatusID where VID=@TransactionID)  AND Active = 1   union all  SELECT TOP 1 * FROM MaMisc WHERE MaMisc.[Key] = 'EcommerceOrderStatus'  and [Value]='Order Cancelled' and (select top 1 [Value] from EcomOrderStatus ES inner join MaMisc MM ON MM.ID=ES.StatusID  where VID=@TransactionID order by code DESC)='Order Placed' AND Active = 1 Order by Code ;"
                + "SELECT Top 1 EcomOrderStatus.ID,StatusID,MaMisc.Value from EcomOrderStatus INNER JOIN MaMisc ON MaMisc.ID = EcomOrderStatus.StatusID WHERE VID=@TransactionID order by Date DESC;"
                + "SELECT  CONVERT(VARCHAR(10), Date, 103) AS Date,TransactionNo FROM Fitransactions WHERE ID=@TransactionID;"
                , new SqlConnection(ConnectionString));
                General.SelectCommand.Parameters.AddWithValue("@TransactionID", TransactionID);
                DataSet Results = new DataSet();
                General.Fill(Results);
                return Results;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public string AddOrderStatus(Models.Ecommerce.Transactions.EcomOrderStatus EcomOrderStatus)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                Cmd.CommandText = "INSERT INTO EcomOrderStatus(UserID,AddressID,VID,Date,Remarks,StatusID) VALUES(@UserID,@AddressID,@VID,@Date,@Remarks,@StatusID) SELECT SCOPE_IDENTITY()";
                Cmd.Parameters.AddWithValue("@UserID", EcomOrderStatus.UserID);
                Cmd.Parameters.AddWithValue("@AddressID", EcomOrderStatus.AddressID);
                Cmd.Parameters.AddWithValue("@VID", EcomOrderStatus.VID);
                Cmd.Parameters.AddWithValue("@Date", EcomOrderStatus.Date);
                Cmd.Parameters.AddWithValue("@Remarks", EcomOrderStatus.Remarks);
                Cmd.Parameters.AddWithValue("@StatusID", EcomOrderStatus.StatusID);
                object data = Cmd.ExecuteScalar();
                if (data != null)
                {
                    int ID = int.Parse(data.ToString());
                    Cmd.Parameters.Clear();
                    Cmd.Connection.Close();
                    return ID.ToString();

                }
                else
                {
                    return "NULL";
                }
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        public DataSet GetPrevStatusRecord(int StatusID, int VID)
        {
            try
            {
                SqlDataAdapter General = new SqlDataAdapter("SELECT DISTINCT UserID,AddressID,Name,Mobile,TransactionNo,CONVERT(VARCHAR(10), FiTransactions.Date, 103) AS Date FROM EcomOrderStatus INNER JOIN EcomUsers ON EcomUsers.ID=EcomOrderStatus.UserID INNER JOIN FiTransactions ON EcomOrderStatus.VID=FiTransactions.ID WHERE VID=@VID;"
                + "SELECT * FROM MaMisc WHERE MaMisc.[Key]='EcommerceOrderStatus' AND ID=@StatusID;"
                + "SELECT ID FROM EcomOrderStatus WHERE VID=@VID AND StatusID=@StatusID;"
                , new SqlConnection(ConnectionString));
                General.SelectCommand.Parameters.AddWithValue("@StatusID", StatusID);
                General.SelectCommand.Parameters.AddWithValue("@VID", VID);
                DataSet Results = new DataSet();
                General.Fill(Results);
                return Results;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //12-20-2021
        public DataSet FillAvailableBranches(int BranchID, int CompanyID)
        {
            try
            {
                SqlDataAdapter General = new SqlDataAdapter("select ID,Company from MaCompanies where ActiveFlag=1 AND BranchCompanyID=@CompanyID;"
                + "select ID,Company from MaCompanies where ActiveFlag=1 AND ID <> @BranchID AND BranchCompanyID=@CompanyID; "
                , new SqlConnection(ConnectionString));
                General.SelectCommand.Parameters.AddWithValue("@BranchID", BranchID);
                General.SelectCommand.Parameters.AddWithValue("@CompanyID", CompanyID);
                DataSet Results = new DataSet();
                General.Fill(Results);
                return Results;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }



        //===============Adithya K A    26/05/2023 onwards================//

        //===============Newly added functions....===============//

        //===============to get account id for purchase===============//

        public DataSet GetAccountIDPurchase()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT A.ID AccountID,A.[Name] AccountName,* FROM FiMaAccounts A INNER JOIN FIMaUniqueAccounts UA ON A.ID=UA.AccID AND UA.Keyword='PURCHASE ACCOUNT'", new SqlConnection(ConnectionString));
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }


        //===============to get account id for sales================//
        public DataSet GetAccountIDSales()
        {
            try
            {
                // Create the DataSet and SqlDataAdapter
                DataSet ds = new DataSet();

                // Use 'using' to ensure connection is properly disposed of after the operation
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT A.ID AccountID, A.[Name] AccountName, * FROM FiMaAccounts A " +
                        "INNER JOIN FIMaUniqueAccounts UA ON A.ID = UA.AccID AND UA.Keyword = 'SALES ACCOUNT'", Con);

                    // Fill the DataSet
                    da.Fill(ds);
                }

                return ds;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        //===============To get the dropdown list of types================//
        public DataTable GetType()
        {
            try
            {
                DataTable dt = new DataTable();

                // Use 'using' to ensure connection is properly disposed of after the operation
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(
                        "select * from MaMisc where [Key] = 'SalePurchaseMode'", Con);

                    da.Fill(dt);
                }

                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        //===============To get the dropdown list of mode================//
        public DataTable GetMode()
        {
            try
            {
                DataTable dt = new DataTable();

                // Use 'using' to ensure connection is properly disposed of after the operation
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(
                        "select * from MaMisc where [Key] = 'Party Collection Mode'", Con);

                    da.Fill(dt);
                }

                return dt;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        //===============Insert into FiTransactionAddiotionals Table=================//
        public string InsertAdditionals(Models.FiTransactionAdditionals FiTransactionAdditionals)
        {
            try
            {
                // Use 'using' for automatic disposal of resources
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        Cmd.Parameters.AddWithValue("@Criteria", "InsertFiTransactionAdditionals");
                        Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionAdditionals.TransactionID);
                        Cmd.Parameters.AddWithValue("@RefTransID1", FiTransactionAdditionals.RefTransID1);
                        Cmd.Parameters.AddWithValue("@RefTransID2", FiTransactionAdditionals.RefTransID2);
                        Cmd.Parameters.AddWithValue("@TypeID", FiTransactionAdditionals.TypeID);
                        Cmd.Parameters.AddWithValue("@ModeID", FiTransactionAdditionals.ModeID);
                        Cmd.Parameters.AddWithValue("@MeasureTypeID", FiTransactionAdditionals.MeasureTypeID);
                        Cmd.Parameters.AddWithValue("@LoadMeasureTypeID", FiTransactionAdditionals.LoadMeasureTypeID);
                        Cmd.Parameters.AddWithValue("@ConsignTermID", FiTransactionAdditionals.ConsignTermID);
                        Cmd.Parameters.AddWithValue("@FromLocationID", FiTransactionAdditionals.FromLocationID);
                        Cmd.Parameters.AddWithValue("@ToLocationID", FiTransactionAdditionals.ToLocationID);
                        Cmd.Parameters.AddWithValue("@ExchangeRate1", FiTransactionAdditionals.ExchangeRate1);
                        Cmd.Parameters.AddWithValue("@AdvanceExRate", FiTransactionAdditionals.AdvanceExRate);
                        Cmd.Parameters.AddWithValue("@CustomsExRate", FiTransactionAdditionals.CustomsExRate);
                        Cmd.Parameters.AddWithValue("@ApprovalDays", FiTransactionAdditionals.ApprovalDays);
                        Cmd.Parameters.AddWithValue("@WorkflowDays", FiTransactionAdditionals.WorkflowDays);
                        Cmd.Parameters.AddWithValue("@PostedBranchID", FiTransactionAdditionals.PostedBranchID);
                        Cmd.Parameters.AddWithValue("@ShipBerthDate", FiTransactionAdditionals.ShipBerthDate);
                        Cmd.Parameters.AddWithValue("@IsBit", FiTransactionAdditionals.IsBit);
                        Cmd.Parameters.AddWithValue("@Name", FiTransactionAdditionals.Name);
                        Cmd.Parameters.AddWithValue("@Code", FiTransactionAdditionals.Code);
                        Cmd.Parameters.AddWithValue("@Address", FiTransactionAdditionals.Address);
                        Cmd.Parameters.AddWithValue("@Rate", FiTransactionAdditionals.Rate);
                        Cmd.Parameters.AddWithValue("@SystemRate", FiTransactionAdditionals.SystemRate);
                        Cmd.Parameters.AddWithValue("@Period", FiTransactionAdditionals.Period);
                        Cmd.Parameters.AddWithValue("@Days", FiTransactionAdditionals.Days);
                        Cmd.Parameters.AddWithValue("@LCOptionID", FiTransactionAdditionals.LCOptionID);
                        Cmd.Parameters.AddWithValue("@LCNo", FiTransactionAdditionals.LCNo);
                        Cmd.Parameters.AddWithValue("@LCAmt", FiTransactionAdditionals.LCAmt);
                        Cmd.Parameters.AddWithValue("@AvailableLCAmt", FiTransactionAdditionals.AvailableLCAmt);
                        Cmd.Parameters.AddWithValue("@CreditAmt", FiTransactionAdditionals.CreditAmt);
                        Cmd.Parameters.AddWithValue("@MarginAmt", FiTransactionAdditionals.MarginAmt);
                        Cmd.Parameters.AddWithValue("@InterestAmt", FiTransactionAdditionals.InterestAmt);
                        Cmd.Parameters.AddWithValue("@AvailableAmt", FiTransactionAdditionals.AvailableAmt);
                        Cmd.Parameters.AddWithValue("@AllocationPerc", FiTransactionAdditionals.AllocationPerc);
                        Cmd.Parameters.AddWithValue("@InterestPerc", FiTransactionAdditionals.InterestPerc);
                        Cmd.Parameters.AddWithValue("@TolerencePerc", FiTransactionAdditionals.TolerencePerc);
                        Cmd.Parameters.AddWithValue("@CountryID", FiTransactionAdditionals.CountryID);
                        Cmd.Parameters.AddWithValue("@CountryOfOriginID", FiTransactionAdditionals.CountryOfOriginID);
                        Cmd.Parameters.AddWithValue("@MaxDays", FiTransactionAdditionals.MaxDays);
                        Cmd.Parameters.AddWithValue("@DocumentNo", FiTransactionAdditionals.DocumentNo);
                        Cmd.Parameters.AddWithValue("@DocumentDate", FiTransactionAdditionals.DocumentDate);
                        Cmd.Parameters.AddWithValue("@BEMaxDays", FiTransactionAdditionals.BEMaxDays);
                        Cmd.Parameters.AddWithValue("@EntryDate", FiTransactionAdditionals.EntryDate);
                        Cmd.Parameters.AddWithValue("@EntryNo", FiTransactionAdditionals.EntryNo);
                        Cmd.Parameters.AddWithValue("@ApplicationCode", FiTransactionAdditionals.ApplicationCode);
                        Cmd.Parameters.AddWithValue("@BankAddress", FiTransactionAdditionals.BankAddress);
                        Cmd.Parameters.AddWithValue("@Unit", FiTransactionAdditionals.Unit);
                        Cmd.Parameters.AddWithValue("@Amount", FiTransactionAdditionals.Amount);
                        Cmd.Parameters.AddWithValue("@AcceptDate", FiTransactionAdditionals.AcceptDate);
                        Cmd.Parameters.AddWithValue("@ExpiryDate", FiTransactionAdditionals.ExpiryDate);
                        Cmd.Parameters.AddWithValue("@DueDate", FiTransactionAdditionals.DueDate);
                        Cmd.Parameters.AddWithValue("@OpenDate", FiTransactionAdditionals.OpenDate);
                        Cmd.Parameters.AddWithValue("@CloseDate", FiTransactionAdditionals.CloseDate);
                        Cmd.Parameters.AddWithValue("@StartDate", FiTransactionAdditionals.StartDate);
                        Cmd.Parameters.AddWithValue("@EndDate", FiTransactionAdditionals.EndDate);
                        Cmd.Parameters.AddWithValue("@ClearDate", FiTransactionAdditionals.ClearDate);
                        Cmd.Parameters.AddWithValue("@ReceiveDate", FiTransactionAdditionals.ReceiveDate);
                        Cmd.Parameters.AddWithValue("@SubmitDate", FiTransactionAdditionals.SubmitDate);
                        Cmd.Parameters.AddWithValue("@EndTime", FiTransactionAdditionals.EndTime);
                        Cmd.Parameters.AddWithValue("@HandOverTime", FiTransactionAdditionals.HandOverTime);
                        Cmd.Parameters.AddWithValue("@LorryHireRate", FiTransactionAdditionals.LorryHireRate);
                        Cmd.Parameters.AddWithValue("@QtyPerLoad", FiTransactionAdditionals.QtyPerLoad);
                        Cmd.Parameters.AddWithValue("@PassNo", FiTransactionAdditionals.PassNo);
                        Cmd.Parameters.AddWithValue("@ReferenceDate", FiTransactionAdditionals.ReferenceDate);
                        Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransactionAdditionals.ReferenceNo);
                        Cmd.Parameters.AddWithValue("@AuditNote", FiTransactionAdditionals.AuditNote);
                        Cmd.Parameters.AddWithValue("@Terms", FiTransactionAdditionals.Terms);
                        Cmd.Parameters.AddWithValue("@FirmID", FiTransactionAdditionals.FirmID);
                        Cmd.Parameters.AddWithValue("@VehicleID", FiTransactionAdditionals.VehicleID);
                        Cmd.Parameters.AddWithValue("@WeekDays", FiTransactionAdditionals.WeekDays);
                        Cmd.Parameters.AddWithValue("@BankWeekDays", FiTransactionAdditionals.BankWeekDays);
                        Cmd.Parameters.AddWithValue("@RecommendByID", FiTransactionAdditionals.RecommendByID);
                        Cmd.Parameters.AddWithValue("@RecommendDate", FiTransactionAdditionals.RecommendDate);
                        Cmd.Parameters.AddWithValue("@RecommendNote", FiTransactionAdditionals.RecommendNote);
                        Cmd.Parameters.AddWithValue("@RecommendStatus", FiTransactionAdditionals.RecommendStatus);
                        Cmd.Parameters.AddWithValue("@IsHigherApproval", FiTransactionAdditionals.IsHigherApproval);
                        Cmd.Parameters.AddWithValue("@LCApplnTransID", FiTransactionAdditionals.LCApplnTransID);
                        Cmd.Parameters.AddWithValue("@InLocID", FiTransactionAdditionals.InLocID);
                        Cmd.Parameters.AddWithValue("@OutLocID", FiTransactionAdditionals.OutLocID);
                        Cmd.Parameters.AddWithValue("@ExchangeRate2", FiTransactionAdditionals.ExchangeRate2);
                        Cmd.Parameters.AddWithValue("@AccountID", FiTransactionAdditionals.AccountID);
                        Cmd.Parameters.AddWithValue("@RouteID", FiTransactionAdditionals.RouteID);
                        Cmd.Parameters.AddWithValue("@AccountID2", FiTransactionAdditionals.AccountID2);
                        Cmd.Parameters.AddWithValue("@Hours", FiTransactionAdditionals.Hours);
                        Cmd.Parameters.AddWithValue("@Year", FiTransactionAdditionals.Year);
                        Cmd.Parameters.AddWithValue("@AreaID", FiTransactionAdditionals.AreaID);
                        Cmd.Parameters.AddWithValue("@OtherBranchID", FiTransactionAdditionals.OtherBranchID);
                        Cmd.Parameters.AddWithValue("@TaxFormID", FiTransactionAdditionals.TaxFormID);
                        Cmd.Parameters.AddWithValue("@PriceCategoryID", FiTransactionAdditionals.PriceCategoryID);
                        Cmd.Parameters.AddWithValue("@IsClosed", FiTransactionAdditionals.IsClosed);
                        Cmd.Parameters.AddWithValue("@DepartmentID", FiTransactionAdditionals.DepartmentID);

                        // Open the connection and execute the command
                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close();// Added by Rafi on 15/11/2025
                    }
                }

                return "True";
            }
            catch (Exception Ex)
            {
                // Handle exceptions, connection is automatically closed due to 'using' block
                throw Ex;
            }
        }


        //===============Update table FiTransactionAdditionals===============//
        public string UpdateAdditionals(Models.FiTransactionAdditionals FiTransactionAdditionals)
        {
            try
            {
                // Use 'using' to ensure the connection and command are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherAdditionalsSP", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        Cmd.Parameters.AddWithValue("@Criteria", "UpdateFiTransactionAdditionals");
                        Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionAdditionals.TransactionID);
                        Cmd.Parameters.AddWithValue("@RefTransID1", FiTransactionAdditionals.RefTransID1);
                        Cmd.Parameters.AddWithValue("@RefTransID2", FiTransactionAdditionals.RefTransID2);
                        Cmd.Parameters.AddWithValue("@TypeID", FiTransactionAdditionals.TypeID);
                        Cmd.Parameters.AddWithValue("@ModeID", FiTransactionAdditionals.ModeID);
                        Cmd.Parameters.AddWithValue("@MeasureTypeID", FiTransactionAdditionals.MeasureTypeID);
                        Cmd.Parameters.AddWithValue("@LoadMeasureTypeID", FiTransactionAdditionals.LoadMeasureTypeID);
                        Cmd.Parameters.AddWithValue("@ConsignTermID", FiTransactionAdditionals.ConsignTermID);
                        Cmd.Parameters.AddWithValue("@FromLocationID", FiTransactionAdditionals.FromLocationID);
                        Cmd.Parameters.AddWithValue("@ToLocationID", FiTransactionAdditionals.ToLocationID);
                        Cmd.Parameters.AddWithValue("@ExchangeRate1", FiTransactionAdditionals.ExchangeRate1);
                        Cmd.Parameters.AddWithValue("@AdvanceExRate", FiTransactionAdditionals.AdvanceExRate);
                        Cmd.Parameters.AddWithValue("@CustomsExRate", FiTransactionAdditionals.CustomsExRate);
                        Cmd.Parameters.AddWithValue("@ApprovalDays", FiTransactionAdditionals.ApprovalDays);
                        Cmd.Parameters.AddWithValue("@WorkflowDays", FiTransactionAdditionals.WorkflowDays);
                        Cmd.Parameters.AddWithValue("@PostedBranchID", FiTransactionAdditionals.PostedBranchID);
                        Cmd.Parameters.AddWithValue("@ShipBerthDate", FiTransactionAdditionals.ShipBerthDate);
                        Cmd.Parameters.AddWithValue("@IsBit", FiTransactionAdditionals.IsBit);
                        Cmd.Parameters.AddWithValue("@Name", FiTransactionAdditionals.Name);
                        Cmd.Parameters.AddWithValue("@Code", FiTransactionAdditionals.Code);
                        Cmd.Parameters.AddWithValue("@Address", FiTransactionAdditionals.Address);
                        Cmd.Parameters.AddWithValue("@Rate", FiTransactionAdditionals.Rate);
                        Cmd.Parameters.AddWithValue("@SystemRate", FiTransactionAdditionals.SystemRate);
                        Cmd.Parameters.AddWithValue("@Period", FiTransactionAdditionals.Period);
                        Cmd.Parameters.AddWithValue("@Days", FiTransactionAdditionals.Days);
                        Cmd.Parameters.AddWithValue("@LCOptionID", FiTransactionAdditionals.LCOptionID);
                        Cmd.Parameters.AddWithValue("@LCNo", FiTransactionAdditionals.LCNo);
                        Cmd.Parameters.AddWithValue("@LCAmt", FiTransactionAdditionals.LCAmt);
                        Cmd.Parameters.AddWithValue("@AvailableLCAmt", FiTransactionAdditionals.AvailableLCAmt);
                        Cmd.Parameters.AddWithValue("@CreditAmt", FiTransactionAdditionals.CreditAmt);
                        Cmd.Parameters.AddWithValue("@MarginAmt", FiTransactionAdditionals.MarginAmt);
                        Cmd.Parameters.AddWithValue("@InterestAmt", FiTransactionAdditionals.InterestAmt);
                        Cmd.Parameters.AddWithValue("@AvailableAmt", FiTransactionAdditionals.AvailableAmt);
                        Cmd.Parameters.AddWithValue("@AllocationPerc", FiTransactionAdditionals.AllocationPerc);
                        Cmd.Parameters.AddWithValue("@InterestPerc", FiTransactionAdditionals.InterestPerc);
                        Cmd.Parameters.AddWithValue("@TolerencePerc", FiTransactionAdditionals.TolerencePerc);
                        Cmd.Parameters.AddWithValue("@CountryID", FiTransactionAdditionals.CountryID);
                        Cmd.Parameters.AddWithValue("@CountryOfOriginID", FiTransactionAdditionals.CountryOfOriginID);
                        Cmd.Parameters.AddWithValue("@MaxDays", FiTransactionAdditionals.MaxDays);
                        Cmd.Parameters.AddWithValue("@DocumentNo", FiTransactionAdditionals.DocumentNo);
                        Cmd.Parameters.AddWithValue("@DocumentDate", FiTransactionAdditionals.DocumentDate);
                        Cmd.Parameters.AddWithValue("@BEMaxDays", FiTransactionAdditionals.BEMaxDays);
                        Cmd.Parameters.AddWithValue("@EntryDate", FiTransactionAdditionals.EntryDate);
                        Cmd.Parameters.AddWithValue("@EntryNo", FiTransactionAdditionals.EntryNo);
                        Cmd.Parameters.AddWithValue("@ApplicationCode", FiTransactionAdditionals.ApplicationCode);
                        Cmd.Parameters.AddWithValue("@BankAddress", FiTransactionAdditionals.BankAddress);
                        Cmd.Parameters.AddWithValue("@Unit", FiTransactionAdditionals.Unit);
                        Cmd.Parameters.AddWithValue("@Amount", FiTransactionAdditionals.Amount);
                        Cmd.Parameters.AddWithValue("@AcceptDate", FiTransactionAdditionals.AcceptDate);
                        Cmd.Parameters.AddWithValue("@ExpiryDate", FiTransactionAdditionals.ExpiryDate);
                        Cmd.Parameters.AddWithValue("@DueDate", FiTransactionAdditionals.DueDate);
                        Cmd.Parameters.AddWithValue("@OpenDate", FiTransactionAdditionals.OpenDate);
                        Cmd.Parameters.AddWithValue("@CloseDate", FiTransactionAdditionals.CloseDate);
                        Cmd.Parameters.AddWithValue("@StartDate", FiTransactionAdditionals.StartDate);
                        Cmd.Parameters.AddWithValue("@EndDate", FiTransactionAdditionals.EndDate);
                        Cmd.Parameters.AddWithValue("@ClearDate", FiTransactionAdditionals.ClearDate);
                        Cmd.Parameters.AddWithValue("@ReceiveDate", FiTransactionAdditionals.ReceiveDate);
                        Cmd.Parameters.AddWithValue("@SubmitDate", FiTransactionAdditionals.SubmitDate);
                        Cmd.Parameters.AddWithValue("@EndTime", FiTransactionAdditionals.EndTime);
                        Cmd.Parameters.AddWithValue("@HandOverTime", FiTransactionAdditionals.HandOverTime);
                        Cmd.Parameters.AddWithValue("@LorryHireRate", FiTransactionAdditionals.LorryHireRate);
                        Cmd.Parameters.AddWithValue("@QtyPerLoad", FiTransactionAdditionals.QtyPerLoad);
                        Cmd.Parameters.AddWithValue("@PassNo", FiTransactionAdditionals.PassNo);
                        Cmd.Parameters.AddWithValue("@ReferenceDate", FiTransactionAdditionals.ReferenceDate);
                        Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransactionAdditionals.ReferenceNo);
                        Cmd.Parameters.AddWithValue("@AuditNote", FiTransactionAdditionals.AuditNote);
                        Cmd.Parameters.AddWithValue("@Terms", FiTransactionAdditionals.Terms);
                        Cmd.Parameters.AddWithValue("@FirmID", FiTransactionAdditionals.FirmID);
                        Cmd.Parameters.AddWithValue("@VehicleID", FiTransactionAdditionals.VehicleID);
                        Cmd.Parameters.AddWithValue("@WeekDays", FiTransactionAdditionals.WeekDays);
                        Cmd.Parameters.AddWithValue("@BankWeekDays", FiTransactionAdditionals.BankWeekDays);
                        Cmd.Parameters.AddWithValue("@RecommendByID", FiTransactionAdditionals.RecommendByID);
                        Cmd.Parameters.AddWithValue("@RecommendDate", FiTransactionAdditionals.RecommendDate);
                        Cmd.Parameters.AddWithValue("@RecommendNote", FiTransactionAdditionals.RecommendNote);
                        Cmd.Parameters.AddWithValue("@RecommendStatus", FiTransactionAdditionals.RecommendStatus);
                        Cmd.Parameters.AddWithValue("@IsHigherApproval", FiTransactionAdditionals.IsHigherApproval);
                        Cmd.Parameters.AddWithValue("@LCApplnTransID", FiTransactionAdditionals.LCApplnTransID);
                        Cmd.Parameters.AddWithValue("@InLocID", FiTransactionAdditionals.InLocID);
                        Cmd.Parameters.AddWithValue("@OutLocID", FiTransactionAdditionals.OutLocID);
                        Cmd.Parameters.AddWithValue("@ExchangeRate2", FiTransactionAdditionals.ExchangeRate2);
                        Cmd.Parameters.AddWithValue("@AccountID", FiTransactionAdditionals.AccountID);
                        Cmd.Parameters.AddWithValue("@RouteID", FiTransactionAdditionals.RouteID);
                        Cmd.Parameters.AddWithValue("@AccountID2", FiTransactionAdditionals.AccountID2);
                        Cmd.Parameters.AddWithValue("@Hours", FiTransactionAdditionals.Hours);
                        Cmd.Parameters.AddWithValue("@Year", FiTransactionAdditionals.Year);
                        Cmd.Parameters.AddWithValue("@AreaID", FiTransactionAdditionals.AreaID);
                        Cmd.Parameters.AddWithValue("@OtherBranchID", FiTransactionAdditionals.OtherBranchID);
                        Cmd.Parameters.AddWithValue("@TaxFormID", FiTransactionAdditionals.TaxFormID);
                        Cmd.Parameters.AddWithValue("@PriceCategoryID", FiTransactionAdditionals.PriceCategoryID);
                        Cmd.Parameters.AddWithValue("@IsClosed", FiTransactionAdditionals.IsClosed);
                        Cmd.Parameters.AddWithValue("@DepartmentID", FiTransactionAdditionals.DepartmentID);

                        // Open the connection and execute the command
                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close();// Added by Rafi on 15/11/2025
                    }
                }

                return "True";
            }
            catch (Exception Ex)
            {
                // Handle exceptions, connection is automatically closed due to 'using' block
                throw Ex;
            }
        }



        //14-07-2025 FitransactionEntries table save
        public string InsertTransactionEntries(int TransactionID)
        {
            try
            {
                // Use 'using' to ensure the connection and command are disposed of correctly
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherSPNew", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 4);
                        Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);

                        // Open the connection and execute the command
                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close();// Added by Rafi on 15/11/2025
                    }
                }
                return "True";
            }
            catch (Exception Ex)
            {
                // Handle exceptions, connection is automatically closed due to 'using' block
                throw Ex;
            }
        }


        public string UpdateTransactionEntries(int? TransactionID)
        {
            try
            {
                // Use 'using' to ensure the connection and command are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("VoucherSPNew", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        Cmd.Parameters.AddWithValue("@Mode", 5);
                        Cmd.Parameters.AddWithValue("@TransactionID", TransactionID);

                        // Open the connection and execute the command
                        Con.Open();
                        Cmd.ExecuteNonQuery();
                        Con.Close();// Added by Rafi on 15/11/2025
                    }
                }

                return "True";
            }
            catch (Exception Ex)
            {
                // Handle exceptions, connection is automatically closed due to 'using' block
                throw Ex;
            }
        }


        //===============to get account id for sales return================//
        public DataSet GetAccountIDSalesReturn()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT A.ID AccountID,A.[Name] AccountName,* FROM FiMaAccounts A INNER JOIN FIMaUniqueAccounts UA ON A.ID=UA.AccID AND UA.Keyword='Sales Return'", new SqlConnection(ConnectionString));
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        //===============to get account id for purchase return===============//
        public DataSet GetAccountIDPurchaseReturn()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT A.ID AccountID,A.[Name] AccountName,* FROM FiMaAccounts A INNER JOIN FIMaUniqueAccounts UA ON A.ID=UA.AccID AND UA.Keyword='Purchase Return'", new SqlConnection(ConnectionString));
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        //==============To get pdf in Ecommerce Sales  Page==============//
        //For purchase 
        public DataSet TransactionPDF(int id)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select FiTransactions.ID,EffectiveDate,TransactionNo,TelephoneNo,Parties.Name,AddressLineOne,AddressLineTwo,FiTransactions.AccountID,Country,Parties.TelephoneNo,Parties.City from FiTransactions inner join Parties on FiTransactions.AccountID = Parties.AccountID   where FiTransactions.ID=@ID;" +
                "select FiTransactions.ID, EffectiveDate, TransactionNo, TelephoneNo, Parties.Name, AddressLineOne, AddressLineTwo, FiTransactions.AccountID, Country, Parties.TelephoneNo, FiTransactionAdditionals.EntryNo, FiTransactionAdditionals.EntryDate from FiTransactions inner join Parties on FiTransactions.AccountID = Parties.AccountID inner join FiTransactionAdditionals  on FiTransactions.ID = FiTransactionAdditionals.TransactionID   where FiTransactions.ID = @ID; " +
                "SELECT ItemCode,BarCode,ItemName,Qty,InvTransItems.Rate,TaxPerc,TaxValue,InvTransItems.Discount from InvTransItems inner join InvItemMaster on InvTransItems.ItemID = InvItemMaster.ID inner join FiTransactions on InvTransItems.TransactionID = FiTransactions.ID where TransactionID=@ID;", new SqlConnection(ConnectionString));
                da.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        

        //==============To get company details in pdf===============//
        public DataTable CompanyTable(object BranchID)
        {
            SqlDataAdapter da = new SqlDataAdapter("Select Company,POBox,TelephoneNo,MobileNo,EmailAddress,Country,FooterImage from MaCompanies WHERE ID=@ID;", new SqlConnection(ConnectionString));
            da.SelectCommand.Parameters.AddWithValue("@ID", BranchID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //=============To change order cancelled field to 1 ================//
        public string UpdateCancelled(int ID)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                Cmd.CommandText = "UPDATE FiTransactions SET Cancelled=1 where ID=@ID";
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.ExecuteNonQuery();
                Con.Close();
                return "True";
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }       

        //==============Insert addtionals in POS on 31-07-2023============//
        public string InsertAdditionalsPOS(Models.FiTransactionAdditionals FiTransactionAdditionals)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlCommand Cmd;
            try
            {
                Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Con.Open();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "InsertFiTransactionAdditionals");
                Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionAdditionals.TransactionID);
                Cmd.Parameters.AddWithValue("@RefTransID1", FiTransactionAdditionals.RefTransID1);
                Cmd.Parameters.AddWithValue("@RefTransID2", FiTransactionAdditionals.RefTransID2);
                Cmd.Parameters.AddWithValue("@TypeID", FiTransactionAdditionals.TypeID);
                Cmd.Parameters.AddWithValue("@ModeID", FiTransactionAdditionals.ModeID);
                Cmd.Parameters.AddWithValue("@MeasureTypeID", FiTransactionAdditionals.MeasureTypeID);
                Cmd.Parameters.AddWithValue("@LoadMeasureTypeID", FiTransactionAdditionals.LoadMeasureTypeID);
                Cmd.Parameters.AddWithValue("@ConsignTermID", FiTransactionAdditionals.ConsignTermID);
                Cmd.Parameters.AddWithValue("@FromLocationID", FiTransactionAdditionals.FromLocationID);
                Cmd.Parameters.AddWithValue("@ToLocationID", FiTransactionAdditionals.ToLocationID);
                Cmd.Parameters.AddWithValue("@ExchangeRate1", FiTransactionAdditionals.ExchangeRate1);
                Cmd.Parameters.AddWithValue("@AdvanceExRate", FiTransactionAdditionals.AdvanceExRate);
                Cmd.Parameters.AddWithValue("@CustomsExRate", FiTransactionAdditionals.CustomsExRate);
                Cmd.Parameters.AddWithValue("@ApprovalDays", FiTransactionAdditionals.ApprovalDays);
                Cmd.Parameters.AddWithValue("@WorkflowDays", FiTransactionAdditionals.WorkflowDays);
                Cmd.Parameters.AddWithValue("@PostedBranchID", FiTransactionAdditionals.PostedBranchID);
                Cmd.Parameters.AddWithValue("@ShipBerthDate", FiTransactionAdditionals.ShipBerthDate);
                Cmd.Parameters.AddWithValue("@IsBit", FiTransactionAdditionals.IsBit);
                Cmd.Parameters.AddWithValue("@Name", FiTransactionAdditionals.Name);
                Cmd.Parameters.AddWithValue("@Code", FiTransactionAdditionals.Code);
                Cmd.Parameters.AddWithValue("@Address", FiTransactionAdditionals.Address);
                Cmd.Parameters.AddWithValue("@Rate", FiTransactionAdditionals.Rate);
                Cmd.Parameters.AddWithValue("@SystemRate", FiTransactionAdditionals.SystemRate);
                Cmd.Parameters.AddWithValue("@Period", FiTransactionAdditionals.Period);
                Cmd.Parameters.AddWithValue("@Days", FiTransactionAdditionals.Days);
                Cmd.Parameters.AddWithValue("@LCOptionID", FiTransactionAdditionals.LCOptionID);
                Cmd.Parameters.AddWithValue("@LCNo", FiTransactionAdditionals.LCNo);
                Cmd.Parameters.AddWithValue("@LCAmt", FiTransactionAdditionals.LCAmt);
                Cmd.Parameters.AddWithValue("@AvailableLCAmt", FiTransactionAdditionals.AvailableLCAmt);
                Cmd.Parameters.AddWithValue("@CreditAmt", FiTransactionAdditionals.CreditAmt);
                Cmd.Parameters.AddWithValue("@MarginAmt", FiTransactionAdditionals.MarginAmt);
                Cmd.Parameters.AddWithValue("@InterestAmt", FiTransactionAdditionals.InterestAmt);
                Cmd.Parameters.AddWithValue("@AvailableAmt", FiTransactionAdditionals.AvailableAmt);
                Cmd.Parameters.AddWithValue("@AllocationPerc", FiTransactionAdditionals.AllocationPerc);
                Cmd.Parameters.AddWithValue("@InterestPerc", FiTransactionAdditionals.InterestPerc);
                Cmd.Parameters.AddWithValue("@TolerencePerc", FiTransactionAdditionals.TolerencePerc);
                Cmd.Parameters.AddWithValue("@CountryID", FiTransactionAdditionals.CountryID);
                Cmd.Parameters.AddWithValue("@CountryOfOriginID", FiTransactionAdditionals.CountryOfOriginID);
                Cmd.Parameters.AddWithValue("@MaxDays", FiTransactionAdditionals.MaxDays);
                Cmd.Parameters.AddWithValue("@DocumentNo", FiTransactionAdditionals.DocumentNo);
                Cmd.Parameters.AddWithValue("@DocumentDate", FiTransactionAdditionals.DocumentDate);
                Cmd.Parameters.AddWithValue("@BEMaxDays", FiTransactionAdditionals.BEMaxDays);
                Cmd.Parameters.AddWithValue("@EntryDate", FiTransactionAdditionals.EntryDate);
                Cmd.Parameters.AddWithValue("@EntryNo", FiTransactionAdditionals.EntryNo);
                Cmd.Parameters.AddWithValue("@ApplicationCode", FiTransactionAdditionals.ApplicationCode);
                Cmd.Parameters.AddWithValue("@BankAddress", FiTransactionAdditionals.BankAddress);
                Cmd.Parameters.AddWithValue("@Unit", FiTransactionAdditionals.Unit);
                Cmd.Parameters.AddWithValue("@Amount", FiTransactionAdditionals.Amount);
                Cmd.Parameters.AddWithValue("@AcceptDate", FiTransactionAdditionals.AcceptDate);
                Cmd.Parameters.AddWithValue("@ExpiryDate", FiTransactionAdditionals.ExpiryDate);
                Cmd.Parameters.AddWithValue("@DueDate", FiTransactionAdditionals.DueDate);
                Cmd.Parameters.AddWithValue("@OpenDate", FiTransactionAdditionals.OpenDate);
                Cmd.Parameters.AddWithValue("@CloseDate", FiTransactionAdditionals.CloseDate);
                Cmd.Parameters.AddWithValue("@StartDate", FiTransactionAdditionals.StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", FiTransactionAdditionals.EndDate);
                Cmd.Parameters.AddWithValue("@ClearDate", FiTransactionAdditionals.ClearDate);
                Cmd.Parameters.AddWithValue("@ReceiveDate", FiTransactionAdditionals.ReceiveDate);
                Cmd.Parameters.AddWithValue("@SubmitDate", FiTransactionAdditionals.SubmitDate);
                Cmd.Parameters.AddWithValue("@EndTime", FiTransactionAdditionals.EndTime);
                Cmd.Parameters.AddWithValue("@HandOverTime", FiTransactionAdditionals.HandOverTime);
                Cmd.Parameters.AddWithValue("@LorryHireRate", FiTransactionAdditionals.LorryHireRate);
                Cmd.Parameters.AddWithValue("@QtyPerLoad", FiTransactionAdditionals.QtyPerLoad);
                Cmd.Parameters.AddWithValue("@PassNo", FiTransactionAdditionals.PassNo);
                Cmd.Parameters.AddWithValue("@ReferenceDate", FiTransactionAdditionals.ReferenceDate);
                Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransactionAdditionals.ReferenceNo);
                Cmd.Parameters.AddWithValue("@AuditNote", FiTransactionAdditionals.AuditNote);
                Cmd.Parameters.AddWithValue("@Terms", FiTransactionAdditionals.Terms);
                Cmd.Parameters.AddWithValue("@FirmID", FiTransactionAdditionals.FirmID);
                Cmd.Parameters.AddWithValue("@VehicleID", FiTransactionAdditionals.VehicleID);
                Cmd.Parameters.AddWithValue("@WeekDays", FiTransactionAdditionals.WeekDays);
                Cmd.Parameters.AddWithValue("@BankWeekDays", FiTransactionAdditionals.BankWeekDays);
                Cmd.Parameters.AddWithValue("@RecommendByID", FiTransactionAdditionals.RecommendByID);
                Cmd.Parameters.AddWithValue("@RecommendDate", FiTransactionAdditionals.RecommendDate);
                Cmd.Parameters.AddWithValue("@RecommendNote", FiTransactionAdditionals.RecommendNote);
                Cmd.Parameters.AddWithValue("@RecommendStatus", FiTransactionAdditionals.RecommendStatus);
                Cmd.Parameters.AddWithValue("@IsHigherApproval", FiTransactionAdditionals.IsHigherApproval);
                Cmd.Parameters.AddWithValue("@LCApplnTransID", FiTransactionAdditionals.LCApplnTransID);
                Cmd.Parameters.AddWithValue("@InLocID", FiTransactionAdditionals.InLocID);
                Cmd.Parameters.AddWithValue("@OutLocID", FiTransactionAdditionals.OutLocID);
                Cmd.Parameters.AddWithValue("@ExchangeRate2", FiTransactionAdditionals.ExchangeRate2);
                Cmd.Parameters.AddWithValue("@AccountID", FiTransactionAdditionals.AccountID);
                Cmd.Parameters.AddWithValue("@RouteID", FiTransactionAdditionals.RouteID);
                Cmd.Parameters.AddWithValue("@AccountID2", FiTransactionAdditionals.AccountID2);
                Cmd.Parameters.AddWithValue("@Hours", FiTransactionAdditionals.Hours);
                Cmd.Parameters.AddWithValue("@Year", FiTransactionAdditionals.Year);
                Cmd.Parameters.AddWithValue("@AreaID", FiTransactionAdditionals.AreaID);
                Cmd.Parameters.AddWithValue("@OtherBranchID", FiTransactionAdditionals.OtherBranchID);
                Cmd.Parameters.AddWithValue("@TaxFormID", FiTransactionAdditionals.TaxFormID);
                Cmd.Parameters.AddWithValue("@PriceCategoryID", FiTransactionAdditionals.PriceCategoryID);
                Cmd.Parameters.AddWithValue("@IsClosed", FiTransactionAdditionals.IsClosed);
                Cmd.Parameters.AddWithValue("@DepartmentID", FiTransactionAdditionals.DepartmentID);
                Cmd.ExecuteNonQuery();
                Con.Close();
                return "True";
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        //==============Update addtionals in POS on 31-07-2023============//
        public string UpdateAdditionalsPOS(Models.FiTransactionAdditionals FiTransactionAdditionals)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlCommand Cmd;
            try
            {
                Cmd = new SqlCommand("VoucherAdditionalsSP", Con);
                Con.Open();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Criteria", "UpdateFiTransactionAdditionals");
                Cmd.Parameters.AddWithValue("@TransactionID", FiTransactionAdditionals.TransactionID);
                Cmd.Parameters.AddWithValue("@RefTransID1", FiTransactionAdditionals.RefTransID1);
                Cmd.Parameters.AddWithValue("@RefTransID2", FiTransactionAdditionals.RefTransID2);
                Cmd.Parameters.AddWithValue("@TypeID", FiTransactionAdditionals.TypeID);
                Cmd.Parameters.AddWithValue("@ModeID", FiTransactionAdditionals.ModeID);
                Cmd.Parameters.AddWithValue("@MeasureTypeID", FiTransactionAdditionals.MeasureTypeID);
                Cmd.Parameters.AddWithValue("@LoadMeasureTypeID", FiTransactionAdditionals.LoadMeasureTypeID);
                Cmd.Parameters.AddWithValue("@ConsignTermID", FiTransactionAdditionals.ConsignTermID);
                Cmd.Parameters.AddWithValue("@FromLocationID", FiTransactionAdditionals.FromLocationID);
                Cmd.Parameters.AddWithValue("@ToLocationID", FiTransactionAdditionals.ToLocationID);
                Cmd.Parameters.AddWithValue("@ExchangeRate1", FiTransactionAdditionals.ExchangeRate1);
                Cmd.Parameters.AddWithValue("@AdvanceExRate", FiTransactionAdditionals.AdvanceExRate);
                Cmd.Parameters.AddWithValue("@CustomsExRate", FiTransactionAdditionals.CustomsExRate);
                Cmd.Parameters.AddWithValue("@ApprovalDays", FiTransactionAdditionals.ApprovalDays);
                Cmd.Parameters.AddWithValue("@WorkflowDays", FiTransactionAdditionals.WorkflowDays);
                Cmd.Parameters.AddWithValue("@PostedBranchID", FiTransactionAdditionals.PostedBranchID);
                Cmd.Parameters.AddWithValue("@ShipBerthDate", FiTransactionAdditionals.ShipBerthDate);
                Cmd.Parameters.AddWithValue("@IsBit", FiTransactionAdditionals.IsBit);
                Cmd.Parameters.AddWithValue("@Name", FiTransactionAdditionals.Name);
                Cmd.Parameters.AddWithValue("@Code", FiTransactionAdditionals.Code);
                Cmd.Parameters.AddWithValue("@Address", FiTransactionAdditionals.Address);
                Cmd.Parameters.AddWithValue("@Rate", FiTransactionAdditionals.Rate);
                Cmd.Parameters.AddWithValue("@SystemRate", FiTransactionAdditionals.SystemRate);
                Cmd.Parameters.AddWithValue("@Period", FiTransactionAdditionals.Period);
                Cmd.Parameters.AddWithValue("@Days", FiTransactionAdditionals.Days);
                Cmd.Parameters.AddWithValue("@LCOptionID", FiTransactionAdditionals.LCOptionID);
                Cmd.Parameters.AddWithValue("@LCNo", FiTransactionAdditionals.LCNo);
                Cmd.Parameters.AddWithValue("@LCAmt", FiTransactionAdditionals.LCAmt);
                Cmd.Parameters.AddWithValue("@AvailableLCAmt", FiTransactionAdditionals.AvailableLCAmt);
                Cmd.Parameters.AddWithValue("@CreditAmt", FiTransactionAdditionals.CreditAmt);
                Cmd.Parameters.AddWithValue("@MarginAmt", FiTransactionAdditionals.MarginAmt);
                Cmd.Parameters.AddWithValue("@InterestAmt", FiTransactionAdditionals.InterestAmt);
                Cmd.Parameters.AddWithValue("@AvailableAmt", FiTransactionAdditionals.AvailableAmt);
                Cmd.Parameters.AddWithValue("@AllocationPerc", FiTransactionAdditionals.AllocationPerc);
                Cmd.Parameters.AddWithValue("@InterestPerc", FiTransactionAdditionals.InterestPerc);
                Cmd.Parameters.AddWithValue("@TolerencePerc", FiTransactionAdditionals.TolerencePerc);
                Cmd.Parameters.AddWithValue("@CountryID", FiTransactionAdditionals.CountryID);
                Cmd.Parameters.AddWithValue("@CountryOfOriginID", FiTransactionAdditionals.CountryOfOriginID);
                Cmd.Parameters.AddWithValue("@MaxDays", FiTransactionAdditionals.MaxDays);
                Cmd.Parameters.AddWithValue("@DocumentNo", FiTransactionAdditionals.DocumentNo);
                Cmd.Parameters.AddWithValue("@DocumentDate", FiTransactionAdditionals.DocumentDate);
                Cmd.Parameters.AddWithValue("@BEMaxDays", FiTransactionAdditionals.BEMaxDays);
                Cmd.Parameters.AddWithValue("@EntryDate", FiTransactionAdditionals.EntryDate);
                Cmd.Parameters.AddWithValue("@EntryNo", FiTransactionAdditionals.EntryNo);
                Cmd.Parameters.AddWithValue("@ApplicationCode", FiTransactionAdditionals.ApplicationCode);
                Cmd.Parameters.AddWithValue("@BankAddress", FiTransactionAdditionals.BankAddress);
                Cmd.Parameters.AddWithValue("@Unit", FiTransactionAdditionals.Unit);
                Cmd.Parameters.AddWithValue("@Amount", FiTransactionAdditionals.Amount);
                Cmd.Parameters.AddWithValue("@AcceptDate", FiTransactionAdditionals.AcceptDate);
                Cmd.Parameters.AddWithValue("@ExpiryDate", FiTransactionAdditionals.ExpiryDate);
                Cmd.Parameters.AddWithValue("@DueDate", FiTransactionAdditionals.DueDate);
                Cmd.Parameters.AddWithValue("@OpenDate", FiTransactionAdditionals.OpenDate);
                Cmd.Parameters.AddWithValue("@CloseDate", FiTransactionAdditionals.CloseDate);
                Cmd.Parameters.AddWithValue("@StartDate", FiTransactionAdditionals.StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", FiTransactionAdditionals.EndDate);
                Cmd.Parameters.AddWithValue("@ClearDate", FiTransactionAdditionals.ClearDate);
                Cmd.Parameters.AddWithValue("@ReceiveDate", FiTransactionAdditionals.ReceiveDate);
                Cmd.Parameters.AddWithValue("@SubmitDate", FiTransactionAdditionals.SubmitDate);
                Cmd.Parameters.AddWithValue("@EndTime", FiTransactionAdditionals.EndTime);
                Cmd.Parameters.AddWithValue("@HandOverTime", FiTransactionAdditionals.HandOverTime);
                Cmd.Parameters.AddWithValue("@LorryHireRate", FiTransactionAdditionals.LorryHireRate);
                Cmd.Parameters.AddWithValue("@QtyPerLoad", FiTransactionAdditionals.QtyPerLoad);
                Cmd.Parameters.AddWithValue("@PassNo", FiTransactionAdditionals.PassNo);
                Cmd.Parameters.AddWithValue("@ReferenceDate", FiTransactionAdditionals.ReferenceDate);
                Cmd.Parameters.AddWithValue("@ReferenceNo", FiTransactionAdditionals.ReferenceNo);
                Cmd.Parameters.AddWithValue("@AuditNote", FiTransactionAdditionals.AuditNote);
                Cmd.Parameters.AddWithValue("@Terms", FiTransactionAdditionals.Terms);
                Cmd.Parameters.AddWithValue("@FirmID", FiTransactionAdditionals.FirmID);
                Cmd.Parameters.AddWithValue("@VehicleID", FiTransactionAdditionals.VehicleID);
                Cmd.Parameters.AddWithValue("@WeekDays", FiTransactionAdditionals.WeekDays);
                Cmd.Parameters.AddWithValue("@BankWeekDays", FiTransactionAdditionals.BankWeekDays);
                Cmd.Parameters.AddWithValue("@RecommendByID", FiTransactionAdditionals.RecommendByID);
                Cmd.Parameters.AddWithValue("@RecommendDate", FiTransactionAdditionals.RecommendDate);
                Cmd.Parameters.AddWithValue("@RecommendNote", FiTransactionAdditionals.RecommendNote);
                Cmd.Parameters.AddWithValue("@RecommendStatus", FiTransactionAdditionals.RecommendStatus);
                Cmd.Parameters.AddWithValue("@IsHigherApproval", FiTransactionAdditionals.IsHigherApproval);
                Cmd.Parameters.AddWithValue("@LCApplnTransID", FiTransactionAdditionals.LCApplnTransID);
                Cmd.Parameters.AddWithValue("@InLocID", FiTransactionAdditionals.InLocID);
                Cmd.Parameters.AddWithValue("@OutLocID", FiTransactionAdditionals.OutLocID);
                Cmd.Parameters.AddWithValue("@ExchangeRate2", FiTransactionAdditionals.ExchangeRate2);
                Cmd.Parameters.AddWithValue("@AccountID", FiTransactionAdditionals.AccountID);
                Cmd.Parameters.AddWithValue("@RouteID", FiTransactionAdditionals.RouteID);
                Cmd.Parameters.AddWithValue("@AccountID2", FiTransactionAdditionals.AccountID2);
                Cmd.Parameters.AddWithValue("@Hours", FiTransactionAdditionals.Hours);
                Cmd.Parameters.AddWithValue("@Year", FiTransactionAdditionals.Year);
                Cmd.Parameters.AddWithValue("@AreaID", FiTransactionAdditionals.AreaID);
                Cmd.Parameters.AddWithValue("@OtherBranchID", FiTransactionAdditionals.OtherBranchID);
                Cmd.Parameters.AddWithValue("@TaxFormID", FiTransactionAdditionals.TaxFormID);
                Cmd.Parameters.AddWithValue("@PriceCategoryID", FiTransactionAdditionals.PriceCategoryID);
                Cmd.Parameters.AddWithValue("@IsClosed", FiTransactionAdditionals.IsClosed);
                Cmd.Parameters.AddWithValue("@DepartmentID", FiTransactionAdditionals.DepartmentID);
                Cmd.ExecuteNonQuery();
                Con.Close();
                return "True";
            }
            catch (Exception Ex)
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                throw Ex;
            }
        }

        //on 04-10-2023
        public DataSet PartyForPrintInTransaction(int id)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Parties.AddressLineOne,Parties.EmailAddress,Parties.TelephoneNo FROM Parties inner join FiTransactions on FiTransactions.AccountID = Parties.AccountID WHERE FiTransactions.ID = @ID;" +
                    "SELECT Value FROM FiTransactionAdditionals INNER JOIN MaMisc ON MaMisc.ID = FiTransactionAdditionals.ModeID WHERE TransactionID=@ID", new SqlConnection(ConnectionString));
                da.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Added on 17-10-2023 for notfication bar
        public DataTable Notification()
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                Con.Open();
                SqlCommand Cmd = new SqlCommand("EcomNotificationSP", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 4);
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

        //For excel sheet entry adding in Sales invoice on 30-06-2025

        public DataSet GetItemsByProductCodes(List<ProductInformation> productInfos)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ItemMasterExcelUploadSP", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 3);

                    // Table-valued parameter
                    DataTable tvp = new DataTable();
                    tvp.Columns.Add("RowID", typeof(int));
                    tvp.Columns.Add("ProductCode", typeof(string));
                    tvp.Columns.Add("Qty", typeof(int));
                    tvp.Columns.Add("Cost", typeof(decimal));

                    int i = 0;
                    foreach (var p in productInfos)
                    {
                        tvp.Rows.Add(i++, p.ProductCode, p.Qty, p.Cost);
                    }

                    var pInfos = cmd.Parameters.AddWithValue("@ProductInfos", tvp);
                    pInfos.SqlDbType = SqlDbType.Structured;
                    pInfos.TypeName = "ProductListType";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds; // Contains 2 tables
                }
            }
        }

        public DataTable GetItemsByProductCodes1(List<ProductInformation> productInfos1)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                try
                {
                    string valueRows = string.Join(",",
                        productInfos1.Select((p, index) =>
                            $"({index}, '{p.ProductCode.Replace("'", "''")}', {p.Qty}, {p.Cost.ToString(System.Globalization.CultureInfo.InvariantCulture)})")
                    );
                    //string query = $@"
                    //SELECT 
                    //    ed.RowID,
                    //    ed.ItemCode AS ModelNo,
                    //    ed.Qty,
                    //    ed.Cost,
                    //    i.ID,
                    //    i.ItemCode,
                    //    i.ItemName
                    //FROM 
                    //    (SELECT * FROM (VALUES {valueRows}) AS V(RowID, ItemCode, Qty, Cost)) AS ed
                    //INNER JOIN InvItemMaster i ON i.ModelNo = ed.ItemCode
                    //ORDER BY ed.RowID";
                    string query = $@"
                    SELECT 
                        ed.RowID,
                        ed.ItemCode AS ModelNo,
                        ed.Qty,
                        ed.Cost,
                        i.ID,
                        i.ItemCode,
                        i.ItemName,
                        MT.SalesPerc,i.TaxTypeID,(MT.SalesPerc/100.00)*ed.Cost TaxValue
                    FROM 
                        (SELECT * FROM (VALUES {valueRows}) AS V(RowID, ItemCode, Qty, Cost)) AS ed
                    INNER JOIN InvItemMaster i ON i.ModelNo LIKE '%'+ed.ItemCode+'%' LEFT JOIN MaTaxType MT ON i.TaxTypeID=MT.ID 
                    ORDER BY ed.RowID";                 

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        //public DataTable NewBarcode(string barcode)
        //{
        //      try
        //    {
        //        SqlDataAdapter da = new SqlDataAdapter("Select InvItemMaster.ItemName,InvItemBarcodes.Barcode from InvItemMaster inner join InvItemBarcodes on InvItemMaster.ID = InvItemBarcodes.ItemId WHERE InvItemBarcodes.Barcode=@Barcode;", new SqlConnection(ConnectionString));
        //        da.SelectCommand.Parameters.AddWithValue("@Barcode", barcode);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        return dt;
        //    }
        //    catch (Exception) { throw; }
        //}
        public string InsertTransaction(SaveTransactionEntryRequest request)
        {
            // 1️ Build DataTables using helpers
            // ----- 1. Validate Mandatory Objects -----
            if (request?.FiTransactions == null)
                return "Transaction header (FiTransactions) is required";

            if (request?.InvTransItems == null || !request.InvTransItems.Any())
                return "At least one transaction item is required";

            // ----- 2. Convert Mandatory -----
            DataTable transactionDT =
                FiTransactionsHelper.ToDataTable(new[] { request.FiTransactions });

            DataTable itemsDT =
                InvTransItemsHelper.ToDataTable(request.InvTransItems);

            // ----- 3. Convert Optional Safely -----
            DataTable additionalsDT =
                request?.FiTransactionAdditionals != null
                    ? FiTransactionAdditionalsHelper.ToDataTable(new[] { request.FiTransactionAdditionals })
                    : new DataTable();

            DataTable entriesDT =
                request?.FiTransactionEntries != null
                    ? FiTransactionEntriesHelper.ToDataTable(request.FiTransactionEntries)
                    : new DataTable();

            // Execute Stored Procedure
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("VoucherSaveSP", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FiTransactions",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "udtFiTransactions",
                    Value = transactionDT
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FiTransactionAdditionals",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "udtFiTransactionAdditionals",
                    Value = additionalsDT
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FiTransactionEntries",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "udtFiTransactionEntries",
                    Value = entriesDT
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@InvTransItems",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "udtInvTransItems",
                    Value = itemsDT
                });
                //cmd.Parameters.AddWithValue("@Cash", request.FiTransactions.Cash);
                //cmd.Parameters.AddWithValue("@Card", request.FiTransactions.Card);
                //cmd.Parameters.AddWithValue("@IsCredit", request.FiTransactions.Credit);
                //cmd.Parameters.AddWithValue("@Discount", request.FiTransactions.Discount);
                //cmd.Parameters.AddWithValue("@Tax", request.FiTransactions.Tax);

                con.Open();
                cmd.ExecuteNonQuery();
            }
             
            return "true";
        }

        //For getting Sales man lookup on 23-02-2026
        public DataTable GetSalesMan()
        {
            try
            {
                DataTable dtTransactions = new DataTable();

                // Use 'using' to ensure that connection, command, and adapter are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("LookupSP", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "SalesPerson");

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter sda = new SqlDataAdapter(Cmd))
                        {
                            sda.Fill(dtTransactions);
                        }
                    }
                }

                return dtTransactions;
            }
            catch (Exception Ex)
            {
                // Rethrow the exception after closing the resources
                throw Ex;
            }
        }

        public DataTable GetModeID()
        {
            try
            {
                DataTable dtTransactions = new DataTable();

                // Use 'using' to ensure that connection, command, and adapter are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 10);

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter sda = new SqlDataAdapter(Cmd))
                        {
                            sda.Fill(dtTransactions);
                        }
                    }
                }

                return dtTransactions;
            }
            catch (Exception Ex)
            {
                // Rethrow the exception after closing the resources
                throw Ex;
            }
        }

        public DataTable AddedBy(int? UserID)
        {
            try
            {
                DataTable dtTransactions = new DataTable();

                // Use 'using' to ensure that connection, command, and adapter are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 11);
                        Cmd.Parameters.AddWithValue("@UserID", UserID);

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter sda = new SqlDataAdapter(Cmd))
                        {
                            sda.Fill(dtTransactions);
                        }
                    }
                }

                return dtTransactions;
            }
            catch (Exception Ex)
            {
                // Rethrow the exception after closing the resources
                throw Ex;
            }
        }


        //For getting Sales area lookup on 25-04-2026
        public DataTable GetArea()
        {
            try
            {
                DataTable dtTransactions = new DataTable();

                // Use 'using' to ensure that connection, command, and adapter are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("LookupSP", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Criteria", "Area");

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter sda = new SqlDataAdapter(Cmd))
                        {
                            sda.Fill(dtTransactions);
                        }
                    }
                }

                return dtTransactions;
            }
            catch (Exception Ex)
            {
                // Rethrow the exception after closing the resources
                throw Ex;
            }
        }

        //For getting Voucher date on 20-05-2026
        public DataTable GetVoucherDate()
        {
            try
            {
                DataTable dtTransactions = new DataTable();

                // Use 'using' to ensure that connection, command, and adapter are properly disposed of
                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand Cmd = new SqlCommand("MaMiscExtSP1", Con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Mode", 12);

                        // Use SqlDataAdapter to fill the DataTable
                        using (SqlDataAdapter sda = new SqlDataAdapter(Cmd))
                        {
                            sda.Fill(dtTransactions);
                        }
                    }
                }

                return dtTransactions;
            }
            catch (Exception Ex)
            {
                // Rethrow the exception after closing the resources
                throw Ex;
            }
        }

        
    }
}
