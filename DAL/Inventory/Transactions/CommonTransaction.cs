using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.DAL.Inventory.Transactions
{
    public class CommonTransaction
    {
        String ConnectionString;
        public CommonTransaction(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        public string DeleteTransaction(Int64 ID)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlDataAdapter General = new SqlDataAdapter("SELECT ID FROM FiTransactions WHERE  ID = @ID;"
                + "select * from InvTransItems where TransactionNo = @ID;", new SqlConnection(ConnectionString));
                General.SelectCommand.Parameters.AddWithValue("@ID", ID);
                DataSet Results = new DataSet();
                General.Fill(Results);
                DataTable DtDetails = Results.Tables[0];
                DataTable DtitemImages = Results.Tables[1];
                if (DtDetails.Rows.Count == 0)
                {
                    return "Unable to track this record";
                }
                SqlCommand Cmd1 = new SqlCommand();
                Cmd1.Connection = new SqlConnection(ConnectionString);
                Cmd1.CommandText = "DELETE FROM FiTransactions WHERE ID= @ID;";
                Cmd1.Parameters.AddWithValue("@ID", ID);
                Cmd1.Connection.Open();
                object data = Cmd1.ExecuteNonQuery();
                Cmd1.Connection.Close();
                if (data != null)
                {
                    SqlCommand Cmd2 = new SqlCommand();
                    Cmd2.Connection = new SqlConnection(ConnectionString);
                    Cmd2.CommandText = "DELETE FROM InvTransItems WHERE TransactionNo= @ID";
                    Cmd2.Parameters.AddWithValue("@ID", ID);
                    Cmd2.Connection.Open();
                    Cmd2.ExecuteNonQuery();
                    Cmd2.Parameters.Clear();
                    Cmd2.Connection.Close();
                    return "true";
                }
                else
                {
                    return "Unable to delete this record";
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
        public string InsertEntry(Models.Inventory.Transactions.SalesInvoice SalesInvoice)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                Cmd.CommandText = "INSERT INTO FiTransactions(Date,EffectiveDate,VoucherID,TransactionNo,SerialNo,IsPostDated,CurrencyID,ExchangeRate,RefPageTypeID,RefPageTableID,ReferenceNo,CompanyID,FinYearID,InstrumentType,InstrumentNo,InstrumentDate,InstrumentBank,CommonNarration,AddedBy,ApprovedBy,AddedDate,ApprovedDate,ApprovalStatus,ApproveNote,Action,StatusID,IsAutoEntry,Posted,Active,Cancelled,AccountID,Description,RefTransID,EditedBy,EditedDate,CostCentreID,PageID,MachineName) VALUES(@Date,@EffectiveDate,@VoucherID,@TransactionNo,@SerialNo,@IsPostDated,@CurrencyID,@ExchangeRate,@RefPageTypeID,@RefPageTableID,@ReferenceNo,@CompanyID,@FinYearID,@InstrumentType,@InstrumentNo,@InstrumentDate,@InstrumentBank,@CommonNarration,@AddedBy,@ApprovedBy,@AddedDate,@ApprovedDate,@ApprovalStatus,@ApproveNote,@Action,@StatusID,@IsAutoEntry,@Posted,@Active,@Cancelled,@AccountID,@Description,@RefTransID,@EditedBy,@EditedDate,@CostCentreID,@PageID,@MachineName); SELECT SCOPE_IDENTITY()";
                Cmd.Parameters.AddWithValue("@Date", SalesInvoice.FiTransactions.Date);
                Cmd.Parameters.AddWithValue("@EffectiveDate", SalesInvoice.FiTransactions.EffectiveDate);
                Cmd.Parameters.AddWithValue("@VoucherID", SalesInvoice.FiTransactions.VoucherID);
                Cmd.Parameters.AddWithValue("@SerialNo", SalesInvoice.FiTransactions.SerialNo);
                Cmd.Parameters.AddWithValue("@IsPostDated", SalesInvoice.FiTransactions.IsPostDated);
                Cmd.Parameters.AddWithValue("@CompanyID", SalesInvoice.FiTransactions.CompanyID);
                Cmd.Parameters.AddWithValue("@AddedBy", SalesInvoice.FiTransactions.AddedBy);
                Cmd.Parameters.AddWithValue("@AddedDate", SalesInvoice.FiTransactions.AddedDate);
                Cmd.Parameters.AddWithValue("@ApprovalStatus", SalesInvoice.FiTransactions.ApprovalStatus);
                if (SalesInvoice.FiTransactions.TransactionNo != null && SalesInvoice.FiTransactions.TransactionNo.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@TransactionNo", SalesInvoice.FiTransactions.TransactionNo);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TransactionNo", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.CurrencyID != null && SalesInvoice.FiTransactions.CurrencyID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@CurrencyID", SalesInvoice.FiTransactions.CurrencyID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@CurrencyID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.ExchangeRate != null && SalesInvoice.FiTransactions.ExchangeRate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ExchangeRate", SalesInvoice.FiTransactions.ExchangeRate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ExchangeRate", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.RefPageTypeID != null && SalesInvoice.FiTransactions.RefPageTypeID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@RefPageTypeID", SalesInvoice.FiTransactions.RefPageTypeID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@RefPageTypeID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.RefPageTableID != null && SalesInvoice.FiTransactions.RefPageTableID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@RefPageTableID", SalesInvoice.FiTransactions.RefPageTableID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@RefPageTableID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.ReferenceNo != null && SalesInvoice.FiTransactions.ReferenceNo.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ReferenceNo", SalesInvoice.FiTransactions.ReferenceNo);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ReferenceNo", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.FinYearID != null && SalesInvoice.FiTransactions.FinYearID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@FinYearID", SalesInvoice.FiTransactions.FinYearID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@FinYearID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.InstrumentType != null && SalesInvoice.FiTransactions.InstrumentType.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@InstrumentType", SalesInvoice.FiTransactions.InstrumentType);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@InstrumentType", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.InstrumentNo != null && SalesInvoice.FiTransactions.InstrumentNo.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@InstrumentNo", SalesInvoice.FiTransactions.InstrumentNo);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@InstrumentNo", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.InstrumentDate != null && SalesInvoice.FiTransactions.InstrumentDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@InstrumentDate", SalesInvoice.FiTransactions.InstrumentDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@InstrumentDate", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.InstrumentBank != null && SalesInvoice.FiTransactions.InstrumentBank.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@InstrumentBank", SalesInvoice.FiTransactions.InstrumentBank);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@InstrumentBank", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.CommonNarration != null && SalesInvoice.FiTransactions.CommonNarration.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@CommonNarration", SalesInvoice.FiTransactions.CommonNarration);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@CommonNarration", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.ApprovedBy != null && SalesInvoice.FiTransactions.ApprovedBy.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ApprovedBy", SalesInvoice.FiTransactions.ApprovedBy);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ApprovedBy", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.ApprovedDate != null && SalesInvoice.FiTransactions.ApprovedDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ApprovedDate", SalesInvoice.FiTransactions.ApprovedDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ApprovedDate", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.ApproveNote != null && SalesInvoice.FiTransactions.ApproveNote.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@ApproveNote", SalesInvoice.FiTransactions.ApproveNote);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ApproveNote", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.Action != null && SalesInvoice.FiTransactions.Action.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Action", SalesInvoice.FiTransactions.Action);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Action", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.StatusID != null && SalesInvoice.FiTransactions.StatusID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@StatusID", SalesInvoice.FiTransactions.StatusID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@StatusID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.IsAutoEntry != null && SalesInvoice.FiTransactions.IsAutoEntry.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@IsAutoEntry", SalesInvoice.FiTransactions.IsAutoEntry);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@IsAutoEntry", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.Posted != null && SalesInvoice.FiTransactions.Posted.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Posted", SalesInvoice.FiTransactions.Posted);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Posted", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.Active != null && SalesInvoice.FiTransactions.Active.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Active", SalesInvoice.FiTransactions.Active);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Active", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.Cancelled != null && SalesInvoice.FiTransactions.Cancelled.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Cancelled", SalesInvoice.FiTransactions.Cancelled);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Cancelled", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.AccountID != null && SalesInvoice.FiTransactions.AccountID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@AccountID", SalesInvoice.FiTransactions.AccountID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.Description != null && SalesInvoice.FiTransactions.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", SalesInvoice.FiTransactions.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.RefTransID != null && SalesInvoice.FiTransactions.RefTransID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@RefTransID", SalesInvoice.FiTransactions.RefTransID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@RefTransID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.EditedBy != null && SalesInvoice.FiTransactions.EditedBy.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@EditedBy", SalesInvoice.FiTransactions.EditedBy);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@EditedBy", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.EditedDate != null && SalesInvoice.FiTransactions.EditedDate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@EditedDate", SalesInvoice.FiTransactions.EditedDate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@EditedDate", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.CostCentreID != null && SalesInvoice.FiTransactions.CostCentreID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@CostCentreID", SalesInvoice.FiTransactions.CostCentreID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@CostCentreID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.PageID != null && SalesInvoice.FiTransactions.PageID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@PageID", SalesInvoice.FiTransactions.PageID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@PageID", DBNull.Value);
                }
                if (SalesInvoice.FiTransactions.MachineName != null && SalesInvoice.FiTransactions.MachineName.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@MachineName", SalesInvoice.FiTransactions.MachineName);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@MachineName", DBNull.Value);
                }
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
                    return "Unable to insert item";
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
        public string UpdateEntry(Models.Inventory.Transactions.SalesInvoice SalesInvoice)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                Cmd.CommandText = "UPDATE SET Date = @Date,EffectiveDate = @EffectiveDate,VoucherID = @VoucherID,TransactionNo = @TransactionNo,SerialNo = @SerialNo,AccountID = @AccountID,Description = @Description,WHERE ID=@ID";
                Cmd.Parameters.AddWithValue("@Date", SalesInvoice.FiTransactions.Date);
                Cmd.Parameters.AddWithValue("@EffectiveDate", SalesInvoice.FiTransactions.EffectiveDate);
                Cmd.Parameters.AddWithValue("@VoucherID", SalesInvoice.FiTransactions.VoucherID);
                Cmd.Parameters.AddWithValue("@SerialNo", SalesInvoice.FiTransactions.SerialNo);
                Cmd.Parameters.AddWithValue("@ApprovalStatus", SalesInvoice.FiTransactions.ApprovalStatus);
                if (SalesInvoice.FiTransactions.Description != null && SalesInvoice.FiTransactions.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", SalesInvoice.FiTransactions.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                var Check = Cmd.ExecuteNonQuery();
                Con.Close();
                if (Check != null)
                {
                    return "true";
                }
                else
                {
                    return "Unable to process the request";
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
        public string SaveInvTransItems(Models.Inventory.Transactions.SalesInvoice SalesInvoices)
        {
            var entry = SalesInvoices.FiTransactionsEntry;
            SqlConnection Con = new SqlConnection(ConnectionString);
            //try
            //{
                SqlDataAdapter General = new SqlDataAdapter("SELECT ID FROM InvTransItems WHERE ItemID= @ItemID AND TransactionID = @TransactionID",
                new SqlConnection(ConnectionString));
                General.SelectCommand.Parameters.AddWithValue("@TransactionID", entry.TransactionID);
                General.SelectCommand.Parameters.AddWithValue("@ItemID", entry.ItemID);
                DataSet Results = new DataSet();
                General.Fill(Results);
                DataTable DtDetails = Results.Tables[0];
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.Connection.Open();
                int ID = 0;
                if (DtDetails.Rows.Count == 0)
                {
                    Cmd.CommandText = "INSERT INTO InvTransItems(ItemID,Unit,Qty,ID,RowType,Rate,BasicQty,TaxPerc,TaxValue,RateDiscPerc,RateDisc,DiscountPerc,Pcs,AdvanceRate,OtherRate," +
                        "MasterMiscID1,Description,Remarks,IsBit,InvAvgCostID,IsReturn,Discount,Additional,Factor,CommodityID,AccountID,TransactionEntryID,LengthFt,LengthIn," +
                        "LengthCm,GirthFt,GirthIn,GirthCm,ThicknessFt,ThicknessIn,ThicknessCm,ShortageQty,AvgCostID,RefTransItemID,Status,Cancel,MeasuredByID,FinishDate,UpdateDate," +
                        "IsSameForPcs,RefID,BatchNo,Margin,TaxTypeID,SizeMasterID,TranType,CostPerc,ManufactureDate,ExpiryDate,FOCQty,GroupItemID,PriceCategoryID,SerialNo," +
                        "TempQty,ReplaceQty,PrintedMRP,PrintedRate,PTSRate,PTRRate,TempRate,ExchangeRate,StockItemID,Visible,StockQty,OutLocID,Description) VALUES(@ItemID,@Unit," +
                        "@Qty,@ID,@RowType,@Rate,@BasicQty,@TaxPerc,@TaxValue,@RateDiscPerc,@RateDisc,@DiscountPerc,@Pcs,@AdvanceRate,@OtherRate,@MasterMiscID1,@Description,@Remarks," +
                        "@IsBit,@InvAvgCostID,@IsReturn,@Discount,@Additional,@Factor,@CommodityID,@AccountID,@TransactionEntryID,@LengthFt,@LengthIn,@LengthCm,@GirthFt,@GirthIn,@GirthCm," +
                        "@ThicknessFt,@ThicknessIn,@ThicknessCm,@ShortageQty,@AvgCostID,@RefTransItemID,@Status,@Cancel,@MeasuredByID,@FinishDate,@UpdateDate,@IsSameForPcs,@RefID,@BatchNo," +
                        "@Margin,@TaxTypeID,@SizeMasterID,@TranType,@CostPerc,@ManufactureDate,@ExpiryDate,@FOCQty,@GroupItemID,@PriceCategoryID,@SerialNo,@TempQty,@ReplaceQty,@PrintedMRP,@PrintedRate," +
                        "@PTSRate,@PTRRate,@TempRate,@ExchangeRate,@StockItemID,@Visible,@StockQty,@OutLocID,@Description); SELECT SCOPE_IDENTITY()";
                    if (entry.RowType != null && entry.RowType.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@RowType", entry.RowType);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@RowType", DBNull.Value);
                    }
                    if (entry.Pcs != null && entry.Pcs.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Pcs", entry.Pcs);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Pcs", DBNull.Value);
                    }
                    if (entry.AdvanceRate != null && entry.AdvanceRate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@AdvanceRate", entry.AdvanceRate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@AdvanceRate", DBNull.Value);
                    }
                    if (entry.OtherRate != null && entry.OtherRate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@OtherRate", entry.OtherRate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@OtherRate", DBNull.Value);
                    }
                    if (entry.MasterMiscID1 != null && entry.MasterMiscID1.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@MasterMiscID1", entry.MasterMiscID1);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@MasterMiscID1", DBNull.Value);
                    }
                    if (entry.Description != null && entry.Description.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Description", entry.Description);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                    }
                    if (entry.Remarks != null && entry.Remarks.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Remarks", entry.Remarks);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Remarks", DBNull.Value);
                    }
                    if (entry.IsBit != null && entry.IsBit.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@IsBit", entry.IsBit);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@IsBit", DBNull.Value);
                    }
                    if (entry.InvAvgCostID != null && entry.InvAvgCostID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@InvAvgCostID", entry.InvAvgCostID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@InvAvgCostID", DBNull.Value);
                    }
                    if (entry.IsReturn != null && entry.IsReturn.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@IsReturn", entry.IsReturn);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@IsReturn", DBNull.Value);
                    }
                    if (entry.Additional != null && entry.Additional.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Additional", entry.Additional);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Additional", DBNull.Value);
                    }
                    if (entry.CommodityID != null && entry.CommodityID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@CommodityID", entry.CommodityID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@CommodityID", DBNull.Value);
                    }
                    if (entry.AccountID != null && entry.AccountID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@AccountID", entry.AccountID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                    }
                    if (entry.TransactionEntryID != null && entry.TransactionEntryID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@TransactionEntryID", entry.TransactionEntryID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@TransactionEntryID", DBNull.Value);
                    }
                    if (entry.LengthFt != null && entry.LengthFt.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@LengthFt", entry.LengthFt);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@LengthFt", DBNull.Value);
                    }
                    if (entry.LengthIn != null && entry.LengthIn.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@LengthIn", entry.LengthIn);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@LengthIn", DBNull.Value);
                    }
                    if (entry.LengthCm != null && entry.LengthCm.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@LengthCm", entry.LengthCm);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@LengthCm", DBNull.Value);
                    }
                    if (entry.GirthFt != null && entry.GirthFt.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@GirthFt", entry.GirthFt);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@GirthFt", DBNull.Value);
                    }
                    if (entry.GirthIn != null && entry.GirthIn.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@GirthIn", entry.GirthIn);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@GirthIn", DBNull.Value);
                    }
                    if (entry.GirthCm != null && entry.GirthCm.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@GirthCm", entry.GirthCm);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@GirthCm", DBNull.Value);
                    }
                    if (entry.ThicknessFt != null && entry.ThicknessFt.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ThicknessFt", entry.ThicknessFt);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ThicknessFt", DBNull.Value);
                    }
                    if (entry.ThicknessIn != null && entry.ThicknessIn.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ThicknessIn", entry.ThicknessIn);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ThicknessIn", DBNull.Value);
                    }
                    if (entry.ThicknessCm != null && entry.ThicknessCm.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ThicknessCm", entry.ThicknessCm);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ThicknessCm", DBNull.Value);
                    }
                    if (entry.ShortageQty != null && entry.ShortageQty.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ShortageQty", entry.ShortageQty);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ShortageQty", DBNull.Value);
                    }
                    if (entry.AvgCostID != null && entry.AvgCostID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@AvgCostID", entry.AvgCostID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@AvgCostID", DBNull.Value);
                    }
                    if (entry.RefTransItemID != null && entry.RefTransItemID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@RefTransItemID", entry.RefTransItemID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@RefTransItemID", DBNull.Value);
                    }
                    if (entry.Status != null && entry.Status.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Status", entry.Status);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Status", DBNull.Value);
                    }
                    if (entry.Cancel != null && entry.Cancel.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Cancel", entry.Cancel);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Cancel", DBNull.Value);
                    }
                    if (entry.MeasuredByID != null && entry.MeasuredByID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@MeasuredByID", entry.MeasuredByID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@MeasuredByID", DBNull.Value);
                    }
                    if (entry.FinishDate != null && entry.FinishDate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@FinishDate", entry.FinishDate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@FinishDate", DBNull.Value);
                    }
                    if (entry.UpdateDate != null && entry.UpdateDate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@UpdateDate", entry.UpdateDate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@UpdateDate", DBNull.Value);
                    }
                    if (entry.IsSameForPcs != null && entry.IsSameForPcs.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@IsSameForPcs", entry.IsSameForPcs);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@IsSameForPcs", DBNull.Value);
                    }
                    if (entry.RefID != null && entry.RefID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@RefID", entry.RefID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@RefID", DBNull.Value);
                    }
                    if (entry.BatchNo != null && entry.BatchNo.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@BatchNo", entry.BatchNo);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@BatchNo", DBNull.Value);
                    }
                    if (entry.Margin != null && entry.Margin.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Margin", entry.Margin);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Margin", DBNull.Value);
                    }
                    if (entry.SizeMasterID != null && entry.SizeMasterID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@SizeMasterID", entry.SizeMasterID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@SizeMasterID", DBNull.Value);
                    }
                    if (entry.TranType != null && entry.TranType.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@TranType", entry.TranType);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@TranType", DBNull.Value);
                    }
                    if (entry.CostPerc != null && entry.CostPerc.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@CostPerc", entry.CostPerc);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@CostPerc", DBNull.Value);
                    }
                    if (entry.ManufactureDate != null && entry.ManufactureDate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ManufactureDate", entry.ManufactureDate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ManufactureDate", DBNull.Value);
                    }
                    if (entry.ExpiryDate != null && entry.ExpiryDate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ExpiryDate", entry.ExpiryDate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ExpiryDate", DBNull.Value);
                    }
                    if (entry.FOCQty != null && entry.FOCQty.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@FOCQty", entry.FOCQty);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@FOCQty", DBNull.Value);
                    }
                    if (entry.GroupItemID != null && entry.GroupItemID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@GroupItemID", entry.GroupItemID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@GroupItemID", DBNull.Value);
                    }
                    if (entry.PriceCategoryID != null && entry.PriceCategoryID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@PriceCategoryID", entry.PriceCategoryID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@PriceCategoryID", DBNull.Value);
                    }
                    if (entry.SerialNo != null && entry.SerialNo.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@SerialNo", entry.SerialNo);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@SerialNo", DBNull.Value);
                    }
                    if (entry.ReplaceQty != null && entry.ReplaceQty.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@ReplaceQty", entry.ReplaceQty);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@ReplaceQty", DBNull.Value);
                    }
                    if (entry.PrintedMRP != null && entry.PrintedMRP.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@PrintedMRP", entry.PrintedMRP);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@PrintedMRP", DBNull.Value);
                    }
                    if (entry.PrintedRate != null && entry.PrintedRate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@PrintedRate", entry.PrintedRate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@PrintedRate", DBNull.Value);
                    }
                    if (entry.PTSRate != null && entry.PTSRate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@PTSRate", entry.PTSRate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@PTSRate", DBNull.Value);
                    }
                    if (entry.PTRRate != null && entry.PTRRate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@PTRRate", entry.PTRRate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@PTRRate", DBNull.Value);
                    }
                    if (entry.TempRate != null && entry.TempRate.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@TempRate", entry.TempRate);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@TempRate", DBNull.Value);
                    }
                    if (entry.StockItemID != null && entry.StockItemID.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@StockItemID", entry.StockItemID);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@StockItemID", DBNull.Value);
                    }
                    if (entry.Visible != null && entry.Visible.ToString() != "")
                    {
                        Cmd.Parameters.AddWithValue("@Visible", entry.Visible);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Visible", DBNull.Value);
                    }
                } else
                {
                    Cmd.CommandText = "UPDATE  InvTransItems SET ItemID = @ItemID,Unit = @Unit,Qty = @Qty,Rate = @Rate,BasicQty = @BasicQty,TaxPerc = @TaxPerc,TaxValue = @TaxValue,RateDiscPerc = @RateDiscPerc," +
                        "RateDisc = @RateDisc,DiscountPerc = @DiscountPerc,Discount = @Discount,Factor = @Factor,TaxTypeID = @TaxTypeID,TempQty = @TempQty,StockQty = @StockQty,OutLocID = @OutLocID,Description = @Description WHERE ID = @ID";
                    Cmd.Parameters.AddWithValue("@ID", DtDetails.Rows[0]["ID"]);

                }
                Cmd.Parameters.AddWithValue("@TransactionID", entry.TransactionID);
                Cmd.Parameters.AddWithValue("@ItemID", entry.ItemID);
                if (entry.Unit != null && entry.Unit.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Unit", entry.Unit);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Unit", DBNull.Value);
                }
                if (entry.Qty != null && entry.Qty.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Qty", entry.Qty);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Qty", DBNull.Value);
                }
                if (entry.Rate != null && entry.Rate.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Rate", entry.Rate);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Rate", DBNull.Value);
                }
                if (entry.BasicQty != null && entry.BasicQty.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@BasicQty", entry.BasicQty);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@BasicQty", DBNull.Value);
                }
                if (entry.TaxPerc != null && entry.TaxPerc.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@TaxPerc", entry.TaxPerc);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TaxPerc", DBNull.Value);
                }
                if (entry.TaxValue != null && entry.TaxValue.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@TaxValue", entry.TaxValue);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TaxValue", DBNull.Value);
                }
                if (entry.RateDiscPerc != null && entry.RateDiscPerc.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@RateDiscPerc", entry.RateDiscPerc);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@RateDiscPerc", DBNull.Value);
                }
                if (entry.RateDisc != null && entry.RateDisc.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@RateDisc", entry.RateDisc);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@RateDisc", DBNull.Value);
                }
                if (entry.DiscountPerc != null && entry.DiscountPerc.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@DiscountPerc", entry.DiscountPerc);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@DiscountPerc", DBNull.Value);
                }
                if (entry.Discount != null && entry.Discount.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Discount", entry.Discount);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Discount", DBNull.Value);
                }
                if (entry.Factor != null && entry.Factor.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Factor", entry.Factor);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Factor", DBNull.Value);
                }
                if (entry.TaxTypeID != null && entry.TaxTypeID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@TaxTypeID", entry.TaxTypeID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TaxTypeID", DBNull.Value);
                }
                if (entry.TempQty != null && entry.TempQty.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@TempQty", entry.TempQty);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TempQty", DBNull.Value);
                }
                if (entry.StockQty != null && entry.StockQty.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@StockQty", entry.StockQty);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@StockQty", DBNull.Value);
                }
                if (entry.OutLocID != null && entry.OutLocID.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@OutLocID", entry.OutLocID);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@OutLocID", DBNull.Value);
                }
                if (entry.Description != null && entry.Description.ToString() != "")
                {
                    Cmd.Parameters.AddWithValue("@Description", entry.Description);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                var Check = Cmd.ExecuteNonQuery();
                Cmd.Parameters.Clear();
                Cmd.Connection.Close();
                if (DtDetails.Rows.Count == 0)
                {
                    int LAST = int.Parse(Check.ToString());
                    return LAST.ToString();
                }
                else
                {
                    return entry.ID.ToString();
                }
            //}
            //catch (Exception Ex)
            //{
            //    if (Con != null && Con.State == ConnectionState.Open)
            //    {
            //        Con.Close();
            //        return Ex.Message;

            //    }
            //    return Ex.Message;
            //}
        }
    }
}
