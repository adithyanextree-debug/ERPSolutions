using ERPSample.Models.General.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Inventory.Transactions
{
    public class SalesInvoice
    {
        private FiTransactions _FiTransactions;
        public FiTransactions FiTransactions
        {
            get
            {
                if (_FiTransactions == null)
                {
                    _FiTransactions = new FiTransactions();
                }
                return _FiTransactions;
            }
            set
            {
                _FiTransactions = value;
            }
        }
        private FiTransactionAdditionals _FiTransactionAdditionals;
        public FiTransactionAdditionals FiTransactionAdditionals
        {
            get
            {
                if (_FiTransactionAdditionals == null)
                {
                    _FiTransactionAdditionals = new FiTransactionAdditionals();
                }
                return _FiTransactionAdditionals;
            }
            set
            {
                _FiTransactionAdditionals = value;
            }
        }
        private List<InvTransItems> _InvTransItems;
        public List<InvTransItems> InvTransItems
        {
            get
            {
                if (_InvTransItems == null)
                {
                    _InvTransItems = new List<InvTransItems>();
                }
                return _InvTransItems;
            }
            set
            {
                _InvTransItems = value;
            }
        }
        private InvTransItems _FiTransactionsEntry;
        public InvTransItems FiTransactionsEntry
        {
            get
            {
                if (_FiTransactionsEntry == null)
                {
                    _FiTransactionsEntry = new InvTransItems();
                }
                return _FiTransactionsEntry;
            }
            set
            {
                _FiTransactionsEntry = value;
            }
        }
    }
    
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
    }

    public class FiTransactions
    {
        [Key]
        public int? ID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? VoucherID { get; set; }
        public string? TransactionNo { get; set; }
        public long? SerialNo { get; set; }
        public bool? IsPostDated { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? ExchangeRate { get; set; }
        public int? RefPageTypeID { get; set; }
        public int? RefPageTableID { get; set; }
        public string? ReferenceNo { get; set; }
        public int? CompanyID { get; set; }
        public int? FinYearID { get; set; }
        public char? InstrumentType { get; set; }
        public string? InstrumentNo { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public string? InstrumentBank { get; set; }
        public string? CommonNarration { get; set; }
        public int AddedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public char? ApprovalStatus { get; set; }
        public string? ApproveNote { get; set; }
        public char? Action { get; set; }
        public int? StatusID { get; set; }
        public bool? IsAutoEntry { get; set; }
        public bool? Posted { get; set; }
        public bool? Active { get; set; }
        public bool? Cancelled { get; set; }
        public int? AccountID { get; set; }
        public string? Description { get; set; }
        public int? RefTransID { get; set; }
        public int? EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public int? CostCentreID { get; set; }
        public int? PageID { get; set; }
        public string? MachineName { get; set; }
        public int? RowState { get; set; }
        public decimal? DeliveryCharge { get; set; }
    }

    public class FiTransactionAdditionals
    {
        public int? TransactionID { get; set; }
        public int? RefTransID1 { get; set; }
        public int? RefTransID2 { get; set; }
        public int? TypeID { get; set; }
        public int? ModeID { get; set; }
        public int? MeasureTypeID { get; set; }
        public int? LoadMeasureTypeID { get; set; }
        public int? ConsignTermID { get; set; }
        public int? FromLocationID { get; set; }
        public int? ToLocationID { get; set; }
        public string? ExchangeRate1 { get; set; }
        public string? AdvanceExRate { get; set; }
        public string? CustomsExRate { get; set; }
        public int? ApprovalDays { get; set; }
        public int? WorkflowDays { get; set; }
        public int? PostedBranchID { get; set; }
        public DateTime? ShipBerthDate { get; set; }
        public bool? IsBit { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public string? Rate { get; set; }
        public string? SystemRate { get; set; }
        public int? Period { get; set; }
        public int? Days { get; set; }
        public int? LCOptionID { get; set; }
        public string? LCNo { get; set; }
        public string? LCAmt { get; set; }
        public string? AvailableLCAmt { get; set; }
        public string? CreditAmt { get; set; }
        public string? MarginAmt { get; set; }
        public string? InterestAmt { get; set; }
        public string? AvailableAmt { get; set; }
        public Decimal? AllocationPerc { get; set; }
        public Decimal? InterestPerc { get; set; }
        public Decimal? TolerencePerc { get; set; }
        public int? CountryID { get; set; }
        public int? CountryOfOriginID { get; set; }
        public int? MaxDays { get; set; }
        public string? DocumentNo { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? BEMaxDays { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? EntryNo { get; set; }
        public string? ApplicationCode { get; set; }
        public string? BankAddress { get; set; }
        public string? Unit { get; set; }
        public string? Amount { get; set; }
        public DateTime? AcceptDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ClearDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? HandOverTime { get; set; }
        public string? LorryHireRate { get; set; }

        public Decimal? QtyPerLoad { get; set; }
        public string? PassNo { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string? ReferenceNo { get; set; }
        public string? AuditNote { get; set; }
        public string? Terms { get; set; }
        public int? FirmID { get; set; }
        public int? VehicleID { get; set; }
        public int? WeekDays { get; set; }
        public int? BankWeekDays { get; set; }
        public int? RecommendByID { get; set; }
        public DateTime? RecommendDate { get; set; }
        public string? RecommendNote { get; set; }
        public char? RecommendStatus { get; set; }
        public bool? IsHigherApproval { get; set; }
        public int? LCApplnTransID { get; set; }
        public int? InLocID { get; set; }
        public int? OutLocID { get; set; }
        public Decimal? ExchangeRate2 { get; set; }
        public int? AccountID { get; set; }
        public int? RouteID { get; set; }
        public int? AccountID2 { get; set; }
        public Decimal? Hours { get; set; }
        public int? Year { get; set; }
        public int? AreaID { get; set; }
        public int? OtherBranchID { get; set; }
        public int? TaxFormID { get; set; }
        public int? PriceCategoryID { get; set; }
        public bool? IsClosed { get; set; }
        public int? DepartmentID { get; set; }

    }

    //public class FiTransactions
    //{
    //    [Key]
    //    public int ID { get; set; }
    //    public DateTime Date { get; set; }
    //    public DateTime EffectiveDate { get; set; }
    //    public int VoucherID { get; set; }
    //    public string? TransactionNo { get; set; }
    //    public int SerialNo { get; set; }
    //    public bool IsPostDated { get; set; }
    //    public int? CurrencyID { get; set; }
    //    public decimal ExchangeRate { get; set; }
    //    public int? RefPageTypeID { get; set; }
    //    public int? RefPageTableID { get; set; }
    //    public string? ReferenceNo { get; set; }
    //    public int CompanyID { get; set; }
    //    public int? FinYearID { get; set; }
    //    public char? InstrumentType { get; set; }
    //    public string? InstrumentNo { get; set; }
    //    public DateTime? InstrumentDate { get; set; }
    //    public string? InstrumentBank { get; set; }
    //    public string? CommonNarration { get; set; }
    //    public int AddedBy { get; set; }
    //    public int? ApprovedBy { get; set; }
    //    public DateTime AddedDate { get; set; }
    //    public DateTime? ApprovedDate { get; set; }
    //    public char ApprovalStatus { get; set; }
    //    public string? ApproveNote { get; set; }
    //    public char? Action { get; set; }
    //    public int? StatusID { get; set; }
    //    public bool? IsAutoEntry { get; set; }
    //    public bool? Posted { get; set; }
    //    public bool? Active { get; set; }
    //    public bool? Cancelled { get; set; }
    //    public int? AccountID { get; set; }
    //    public string? Description { get; set; }
    //    public int? RefTransID { get; set; }
    //    public int? EditedBy { get; set; }
    //    public DateTime? EditedDate { get; set; }
    //    public int? CostCentreID { get; set; }
    //    public int? PageID { get; set; }
    //    public string? MachineName { get; set; }
    //    public int? RowState { get; set; }
    //}
    //public class InvTransItems
    //{
    //    [Key]
    //    public int ID { get; set; }
    //    public int TransactionID { get; set; }
    //    public int? ItemID { get; set; }
    //    public string? Unit { get; set; }
    //    public decimal? Qty { get; set; }
    //    public decimal? BasicQty { get; set; }
    //    public int? Pcs { get; set; }

    //    [Column(TypeName = "money")]
    //    public decimal? Rate { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? AdvanceRate { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? OtherRate { get; set; }
    //    public int? MasterMiscID1 { get; set; }
    //    public int? RowType { get; set; }
    //    public string? Description { get; set; }
    //    public string? Remarks { get; set; }
    //    public bool? IsBit { get; set; }
    //    public int? InvAvgCostID { get; set; }
    //    public bool? IsReturn { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? Discount { get; set; }
    //    public decimal? Additional { get; set; }
    //    public decimal? Factor { get; set; }
    //    public int? CommodityID { get; set; }
    //    public int? AccountID { get; set; }
    //    public int? TransactionEntryID { get; set; }
    //    public decimal? LengthFt { get; set; }
    //    public decimal? LengthIn { get; set; }
    //    public decimal? LengthCm { get; set; }
    //    public decimal? GirthFt { get; set; }
    //    public decimal? GirthIn { get; set; }
    //    public decimal? GirthCm { get; set; }
    //    public decimal? ThicknessFt { get; set; }
    //    public decimal? ThicknessIn { get; set; }
    //    public decimal? ThicknessCm { get; set; }
    //    public decimal? ShortageQty { get; set; }
    //    public int? AvgCostID { get; set; }
    //    public int? RefTransItemID { get; set; }
    //    public int? Status { get; set; }
    //    public bool? Cancel { get; set; }
    //    public int? MeasuredByID { get; set; }
    //    public DateTime? FinishDate { get; set; }
    //    public DateTime? UpdateDate { get; set; }
    //    public bool? IsSameForPcs { get; set; }
    //    public decimal? StockQty { get; set; }
    //    public int? RefID { get; set; }
    //    public int? InLocID { get; set; }
    //    public int? OutLocID { get; set; }
    //    public string? BatchNo { get; set; }
    //    public decimal? Margin { get; set; }
    //    public decimal? DiscountPerc { get; set; }
    //    public decimal? TaxPerc { get; set; }

    //    [Column(TypeName = "money")]
    //    public decimal? TaxValue { get; set; }
    //    public int? TaxTypeID { get; set; }
    //    public int? SizeMasterID { get; set; }
    //    public string? TranType { get; set; }
    //    public decimal? CostPerc { get; set; }
    //    public DateTime? ManufactureDate { get; set; }
    //    public DateTime? ExpiryDate { get; set; }
    //    public decimal? FOCQty { get; set; }
    //    public int? GroupItemID { get; set; }
    //    public int? PriceCategoryID { get; set; }
    //    public decimal? RateDiscPerc { get; set; }

    //    [Column(TypeName = "money")]
    //    public decimal? RateDisc { get; set; }
    //    public int? SerialNo { get; set; }
    //    public decimal? TempQty { get; set; }
    //    public decimal? ReplaceQty { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? PrintedMRP { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? PrintedRate { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? PTSRate { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? PTRRate { get; set; }
    //    [Column(TypeName = "money")]
    //    public decimal? TempRate { get; set; }
    //    public int? StockItemID { get; set; }
    //    public bool? Visible { get; set; }
    //}
}
