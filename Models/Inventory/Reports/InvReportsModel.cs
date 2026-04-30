using System.ComponentModel.DataAnnotations;
using System.Data;
using System;

using System.Collections.Generic;


namespace ERPSample.Models.Inventory.Reports
{
    public class InvReportsModel
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }//Used in SalesCommission,DaySummary,AreaWiseSale,Sales Register
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }//Used in SalesCommission,DaySummary,AreaWiseSale,Sales Register
        public DataTable ReportTable { get; set; } // --used in Daysummary[29/09/23]
        public int CounterID { get; set; } //--used in SalesRegister --[04/10/2023]
        public int PaymentTypeID { get; set; }//--used in SalesRegister --[04/10/2023]
        public int BasicVTypeID { get; set; }//--used in SalesRegister --[04/10/2023]

        public int VTypeID { get; set; }//--used in SalesRegister --[04/10/2023]
        public bool Criteria { get; set; }//used in SalesRegister --[04/10/2023]
        public bool Detailed { get; set; }//used in SalesRegister --[04/10/2023]
        //public bool InventoryView { get; set; }//used in SalesRegister --[04/10/2023]
        public bool Inventory { get; set; }//used in SalesRegister --[04/10/2023]
        public bool Finance { get; set; }//used in SalesRegister --[04/10/2023]
        public bool Columnar { get; set; }//used in SalesRegister --[04/10/2023]
        public bool GroupItem { get; set; }//used in SalesRegister --[04/10/2023]


        //public Object BranchID { get; set; }
        public int AccountID { get; set; } //Used in SalesCommission,Sales Register
         
        public int AreaID { get; set; } //used in area-wise-sales [17/10/23]
        public DateTime Date { get; set; }
        //  public string Warehouse { get; set; }
        public int ItemID { get; set; }//Used in Sales Register
        public string Unit { get; set; }
        //public string Size { get; set; }
        public bool ItemWise { get; set; }
        public int BarCode { get; set; }//Used in StockRegItemwise
        //public string BarCode1 { get; set; }
        public int Origin { get; set; }
        public int Brand { get; set; }
        public int Category { get; set; }
        public int Color { get; set; }
        public string BatchNo { get; set; }
        public int Supplier { get; set; }
        public int Customer { get; set; }
        public int WarehouseID { get; set; }
        public string Value { get; set; }//Added for ItemsSearch on 13-10-2023 Adithya

        //Added BarCode1,Orgin1, PurchaseRate - AvgCost fileds for StockRegItemwise on 16-10-2023 Aiswarya

        public string Item { get;set; }
        public string BarCode1 { get;set; }
        public string Origin1 { get;set; }
        public string PurchaseRate { get;set; }
        public string Stock { get;set; }
        public string SellingPrice { get;set; }
        public string OEMNo { get;set; }
        public string Manufacturer { get;set; }
        public string ModelNo { get;set; }
        public string Category1 { get;set; }
        public string Quality { get;set; }
        public string Commodity { get;set; }
        public string Margin { get;set; }
        public string CashPrice { get;set; }
        public string CreditPrice { get;set; }
        public string PartNo { get;set; }
        public string Location { get;set; }
        public string Weight { get;set; }
        public string AvgCost { get;set; }

    }

}
