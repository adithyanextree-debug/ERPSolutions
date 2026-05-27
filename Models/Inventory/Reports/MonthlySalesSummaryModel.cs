namespace ERPSample.Models.Inventory.Reports
{
    public class MonthlySalesSummaryModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long BranchID { get; set; }
        public int? MonthYear { get; set; }
        public decimal Amount { get; set; }
    }
}
