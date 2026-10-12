namespace ERPSample.Models.Inventory.Reports
{
    public class eReturnsModel
    {
        // Required filter fields
        public long BranchID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int? AccountID { get; set; }   // Party
        public string? Emirate { get; set; }
        public string? TaxRegNo { get; set; }
        public decimal? InvoiceAmountIP { get; set; }
        public decimal? InvoiceAmountOP { get; set; }
    }
}
