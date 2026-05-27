namespace ERPSample.Models.Inventory.Reports
{
    public class InventoryProfitModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long BranchID { get; set; }
        public int? ItemID { get; set; }
        public bool? ViewBy { get; set; }
        public bool? IsDetailed { get; set; }
        public int? AccountID { get; set; }
        public string? Criteria { get; set; }
    }
}
