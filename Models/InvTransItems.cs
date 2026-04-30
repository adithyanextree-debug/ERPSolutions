using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class InvTransItems
    {
        [Key]
        public int? ID { get; set; }
        public int? TransactionID { get; set; }
        public int? ItemID { get; set; }
        public string? Unit { get; set; }
        public decimal? Qty { get; set; }
        public decimal? BasicQty { get; set; }
        public int? Pcs { get; set; }

        [Column(TypeName = "money")]
        public decimal? Rate { get; set; }
        [Column(TypeName = "money")]
        public decimal? AdvanceRate { get; set; }
        [Column(TypeName = "money")]
        public decimal? OtherRate { get; set; }
        public int? MasterMiscID1 { get; set; }
        public int? RowType { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool? IsBit { get; set; }
        public int? InvAvgCostID { get; set; }
        public bool? IsReturn { get; set; }
        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }
        public decimal? Additional { get; set; }
        public decimal? Factor { get; set; }
        public int? CommodityID { get; set; }
        public int? AccountID { get; set; }
        public int? TransactionEntryID { get; set; }
        public decimal? LengthFt { get; set; }
        public decimal? LengthIn { get; set; }
        public decimal? LengthCm { get; set; }
        public decimal? GirthFt { get; set; }
        public decimal? GirthIn { get; set; }
        public decimal? GirthCm { get; set; }
        public decimal? ThicknessFt { get; set; }
        public decimal? ThicknessIn { get; set; }
        public decimal? ThicknessCm { get; set; }
        public decimal? ShortageQty { get; set; }
        public int? AvgCostID { get; set; }
        public int? RefTransItemID { get; set; }
        public int? Status { get; set; }
        public bool? Cancel { get; set; }
        public int? MeasuredByID { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsSameForPcs { get; set; }
        public decimal? StockQty { get; set; }
        public int? RefID { get; set; }
        public int? InLocID { get; set; }
        public int? OutLocID { get; set; }
        public string? BatchNo { get; set; }
        public decimal? Margin { get; set; }
        public decimal? DiscountPerc { get; set; }
        public decimal? TaxPerc { get; set; }

        [Column(TypeName = "money")]
        public decimal? TaxValue { get; set; }
        public int? TaxTypeID { get; set; }
        public int? TaxAccountID { get; set; }
        public int? SizeMasterID { get; set; }
        public string? TranType { get; set; }
        public decimal? CostPerc { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? FOCQty { get; set; }
        public int? GroupItemID { get; set; }
        public int? PriceCategoryID { get; set; }
        public decimal? RateDiscPerc { get; set; }

        [Column(TypeName = "money")]
        public decimal? RateDisc { get; set; }
        public int? SerialNo { get; set; }
        public decimal? TempQty { get; set; }
        public decimal? ReplaceQty { get; set; }
        [Column(TypeName = "money")]
        public decimal? PrintedMRP { get; set; }
        [Column(TypeName = "money")]
        public decimal? PrintedRate { get; set; }
        [Column(TypeName = "money")]
        public decimal? PTSRate { get; set; }
        [Column(TypeName = "money")]
        public decimal? PTRRate { get; set; }
        [Column(TypeName = "money")]
        public decimal? TempRate { get; set; }
        public int? StockItemID { get; set; }
        public bool? Visible { get; set; }
        public int? RowState { get; set; }
        public int? RefTransID1 { get; set; }

    }

    public class InvTransItemsDataTable : DataTable
    {
        public InvTransItemsDataTable()
        {
            TableName = "InvTransItems";

            Columns.Add("ID", typeof(int));
            Columns.Add("TransactionID", typeof(int));
            Columns.Add("ItemID", typeof(int));

            // ---- MISSING COLUMN ADDED ----
            Columns.Add("RefTransID1", typeof(int));

            Columns.Add("Unit", typeof(string));
            Columns.Add("Qty", typeof(decimal));
            Columns.Add("BasicQty", typeof(decimal));
            Columns.Add("Pcs", typeof(int));

            Columns.Add("Rate", typeof(decimal));
            Columns.Add("AdvanceRate", typeof(decimal));
            Columns.Add("OtherRate", typeof(decimal));

            Columns.Add("MasterMiscID1", typeof(int));
            Columns.Add("RowType", typeof(int));
            Columns.Add("Description", typeof(string));
            Columns.Add("Remarks", typeof(string));
            Columns.Add("IsBit", typeof(bool));
            Columns.Add("InvAvgCostID", typeof(int));
            Columns.Add("IsReturn", typeof(bool));

            Columns.Add("Discount", typeof(decimal));
            Columns.Add("Additional", typeof(decimal));
            Columns.Add("Factor", typeof(decimal));

            Columns.Add("CommodityID", typeof(int));
            Columns.Add("AccountID", typeof(int));
            Columns.Add("TransactionEntryID", typeof(int));

            Columns.Add("LengthFt", typeof(decimal));
            Columns.Add("LengthIn", typeof(decimal));
            Columns.Add("LengthCm", typeof(decimal));

            Columns.Add("GirthFt", typeof(decimal));
            Columns.Add("GirthIn", typeof(decimal));
            Columns.Add("GirthCm", typeof(decimal));

            Columns.Add("ThicknessFt", typeof(decimal));
            Columns.Add("ThicknessIn", typeof(decimal));
            Columns.Add("ThicknessCm", typeof(decimal));

            Columns.Add("ShortageQty", typeof(decimal));
            Columns.Add("AvgCostID", typeof(int));
            Columns.Add("RefTransItemID", typeof(int));
            Columns.Add("Status", typeof(int));
            Columns.Add("Cancel", typeof(bool));
            Columns.Add("MeasuredByID", typeof(int));

            Columns.Add("FinishDate", typeof(DateTime));
            Columns.Add("UpdateDate", typeof(DateTime));
            Columns.Add("IsSameForPcs", typeof(bool));
            Columns.Add("StockQty", typeof(decimal));

            Columns.Add("RefID", typeof(int));
            Columns.Add("InLocID", typeof(int));
            Columns.Add("OutLocID", typeof(int));
            Columns.Add("BatchNo", typeof(string));

            Columns.Add("Margin", typeof(decimal));
            Columns.Add("DiscountPerc", typeof(decimal));
            Columns.Add("TaxPerc", typeof(decimal));
            Columns.Add("TaxValue", typeof(decimal));

            Columns.Add("TaxTypeID", typeof(int));
            Columns.Add("SizeMasterID", typeof(int));
            Columns.Add("TranType", typeof(string));

            Columns.Add("CostPerc", typeof(decimal));
            Columns.Add("ManufactureDate", typeof(DateTime));
            Columns.Add("ExpiryDate", typeof(DateTime));
            Columns.Add("FOCQty", typeof(decimal));

            Columns.Add("GroupItemID", typeof(int));
            Columns.Add("PriceCategoryID", typeof(int));
            Columns.Add("RateDiscPerc", typeof(decimal));
            Columns.Add("RateDisc", typeof(decimal));

            Columns.Add("SerialNo", typeof(int));
            Columns.Add("TempQty", typeof(decimal));
            Columns.Add("TempRate", typeof(decimal));
            Columns.Add("ReplaceQty", typeof(decimal));

            Columns.Add("PrintedMRP", typeof(decimal));
            Columns.Add("PrintedRate", typeof(decimal));
            Columns.Add("PTSRate", typeof(decimal));
            Columns.Add("PTRRate", typeof(decimal));

            Columns.Add("StockItemID", typeof(int));
            Columns.Add("Visible", typeof(bool));
            Columns.Add("TaxAccountID", typeof(int));

            // ---- MISSING COLUMN ----
            Columns.Add("RowState", typeof(int));
        }
    }


    public class ProductInformation
    {
        public string ProductCode { get; set; }
        public int Qty { get; set; }
        public decimal Cost { get; set; }

    }

    public class SaveTransactionEntryRequest
    {
        public List<InvTransItems> InvTransItems { get; set; }
        public FiTransactions FiTransactions { get; set; }
        public FiTransactionAdditionals FiTransactionAdditionals { get; set; }
        public List<FiTransactionEntries> FiTransactionEntries { get; set; }
       // public FiTransactionEntries FiTransactionEntries { get; set; }
    }
}
