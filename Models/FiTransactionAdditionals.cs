using Microsoft.VisualBasic;
using System;
using System.Data;

namespace ERPSample.Models
{
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
        public decimal? InterestAmt { get; set; }
        public decimal? AvailableAmt { get; set; }
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
        public DateTime? ExpiryDate  { get; set; }
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
        public int? RowState { get; set; }
       
    }

    public class FiTransactionAdditionalsDataTable : DataTable
    {
        public FiTransactionAdditionalsDataTable()
        {
            TableName = "FiTransactionAdditionals";

            Columns.Add("TransactionID", typeof(int));
            Columns.Add("RefTransID1", typeof(int));
            Columns.Add("RefTransID2", typeof(int));
            Columns.Add("TypeID", typeof(int));
            Columns.Add("ModeID", typeof(int));
            Columns.Add("MeasureTypeID", typeof(int));
            Columns.Add("LoadMeasureTypeID", typeof(int));
            Columns.Add("ConsignTermID", typeof(int));
            Columns.Add("FromLocationID", typeof(int));
            Columns.Add("ToLocationID", typeof(int));

            Columns.Add("ExchangeRate1", typeof(decimal));
            Columns.Add("AdvanceExRate", typeof(decimal));
            Columns.Add("CustomsExRate", typeof(decimal));

            Columns.Add("ApprovalDays", typeof(int));
            Columns.Add("WorkflowDays", typeof(int));
            Columns.Add("PostedBranchID", typeof(int));
            Columns.Add("ShipBerthDate", typeof(DateTime));
            Columns.Add("IsBit", typeof(bool));

            Columns.Add("Name", typeof(string));
            Columns.Add("Code", typeof(string));
            Columns.Add("Address", typeof(string));
            Columns.Add("Rate", typeof(decimal));
            Columns.Add("SystemRate", typeof(decimal));

            Columns.Add("Period", typeof(int));
            Columns.Add("Days", typeof(int));
            Columns.Add("LCOptionID", typeof(int));
            Columns.Add("LCNo", typeof(string));
            Columns.Add("LCAmt", typeof(decimal));
            Columns.Add("AvailableLCAmt", typeof(decimal));
            Columns.Add("CreditAmt", typeof(decimal));
            Columns.Add("MarginAmt", typeof(decimal));

            Columns.Add("InterestAmt", typeof(decimal));
            Columns.Add("AvailableAmt", typeof(decimal));
            Columns.Add("AllocationPerc", typeof(decimal));
            Columns.Add("InterestPerc", typeof(decimal));
            Columns.Add("TolerencePerc", typeof(decimal));

            Columns.Add("CountryID", typeof(int));
            Columns.Add("CountryOfOriginID", typeof(int));
            Columns.Add("MaxDays", typeof(int));
            Columns.Add("DocumentNo", typeof(string));
            Columns.Add("DocumentDate", typeof(DateTime));
            Columns.Add("BEMaxDays", typeof(int));

            Columns.Add("EntryDate", typeof(DateTime));
            Columns.Add("EntryNo", typeof(string));
            Columns.Add("ApplicationCode", typeof(string));
            Columns.Add("BankAddress", typeof(string));
            Columns.Add("Unit", typeof(string));
            Columns.Add("Amount", typeof(decimal));

            Columns.Add("AcceptDate", typeof(DateTime));
            Columns.Add("ExpiryDate", typeof(DateTime));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("OpenDate", typeof(DateTime));
            Columns.Add("CloseDate", typeof(DateTime));
            Columns.Add("StartDate", typeof(DateTime));
            Columns.Add("EndDate", typeof(DateTime));
            Columns.Add("ClearDate", typeof(DateTime));
            Columns.Add("ReceiveDate", typeof(DateTime));
            Columns.Add("SubmitDate", typeof(DateTime));
            Columns.Add("EndTime", typeof(DateTime));
            Columns.Add("HandOverTime", typeof(DateTime));

            Columns.Add("LorryHireRate", typeof(decimal));
            Columns.Add("QtyPerLoad", typeof(decimal));
            Columns.Add("PassNo", typeof(string));
            Columns.Add("ReferenceDate", typeof(DateTime));
            Columns.Add("ReferenceNo", typeof(string));
            Columns.Add("AuditNote", typeof(string));
            Columns.Add("Terms", typeof(string));

            Columns.Add("FirmID", typeof(int));
            Columns.Add("VehicleID", typeof(int));
            Columns.Add("WeekDays", typeof(int));
            Columns.Add("BankWeekDays", typeof(int));
            Columns.Add("RecommendByID", typeof(int));
            Columns.Add("RecommendDate", typeof(DateTime));
            Columns.Add("RecommendNote", typeof(string));
            Columns.Add("RecommendStatus", typeof(char));
            Columns.Add("IsHigherApproval", typeof(bool));

            Columns.Add("LCApplnTransID", typeof(int));
            Columns.Add("InLocID", typeof(int));
            Columns.Add("OutLocID", typeof(int));
            Columns.Add("ExchangeRate2", typeof(decimal));
            Columns.Add("AccountID", typeof(int));
            Columns.Add("RouteID", typeof(int));
            Columns.Add("AccountID2", typeof(int));
            Columns.Add("Hours", typeof(decimal));
            Columns.Add("Year", typeof(int));
            Columns.Add("AreaID", typeof(int));
            Columns.Add("OtherBranchID", typeof(int));
            Columns.Add("TaxFormID", typeof(int));
            Columns.Add("PriceCategoryID", typeof(int));
            Columns.Add("IsClosed", typeof(bool));
            Columns.Add("DepartmentID", typeof(int));
            Columns.Add("RowState", typeof(int));

        }


    }
}
