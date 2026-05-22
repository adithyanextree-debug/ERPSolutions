namespace ERPSample.Models.Inventory.Reports.Stock
{
    public class StockItemRegisterModel
    {
        public string? Criteria { get; set; }
        public long BranchID { get; set; }
        public int? LocationID { get; set; }
        public int? CommodityID { get; set; }
        public int? ItemID { get; set; }
        public bool? IsItemwise { get; set; }
        public string? Barcode { get; set; }
        public int? OriginID { get; set; }
        public int? ColorID { get; set; }
        public int? BrandID { get; set; }
        public string? BatchNo { get; set; }
        public int? SupplierID { get; set; }
        public int? CustomerID { get; set; }
        public int? AccountID { get; set; }
        public DateTime Date { get; set; }

    }
}
