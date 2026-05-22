namespace ERPSample.Models.Inventory.Reports.Purchase
{
    // Models/PurchaseRegisterModel.cs
    public class PurchaseRegisterModel
    {
        // Required filter fields
        public long BranchID { get; set; }
        public DateTime FromDate { get; set; } 
        public DateTime ToDate { get; set; }
        public int? BasicVTypeID { get; set; }

        // Optional filter fieldss
        public int? VTypeID { get; set; }   // -1 = all
        public int? AccountID { get; set; }   // Party
        public string? PaymentTypeID { get; set; }
        public int? ItemID { get; set; }
        public int? CounterID { get; set; }
        public bool Criteria { get; set; }

        // Checkboxes
        public bool? IsDetailed { get; set; }
        public bool? IsInventory { get; set; }
        public bool? IsColumnar { get; set; }
        public bool? IsGroupItem { get; set; }

        // Summary totals (filled after query)
        public decimal? CashDebit { get; set; }
        public decimal? CashCredit { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? Taxable { get; set; }
        public decimal? NonTaxable { get; set; }
        public decimal? TotalDebit { get; set; }
        public decimal? TotalCredit { get; set; }
    }
}
