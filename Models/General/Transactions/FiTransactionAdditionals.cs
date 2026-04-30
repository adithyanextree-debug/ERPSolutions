using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.General.Transactions
{
    public class FiTransactionAdditionals
    {
            [Key]
            public int TransactionID { get; set; }
            public int? RefTransID1 { get; set; }
            public int? RefTransID2 { get; set; }
            public int? TypeID { get; set; }
            public int? ModeID { get; set; }
            public int? MeasureTypeID { get; set; }
            public int? LoadMeasureTypeID { get; set; }
            public int? ConsignTermID { get; set; }
            public int? FromLocationID { get; set; }
            public int? ToLocationID { get; set; }
            public decimal? ExchangeRate1 { get; set; }
            public decimal? AdvanceExRate { get; set; }
            public decimal? CustomsExRate { get; set; }
            public int? ApprovalDays { get; set; }
            public int? WorkflowDays { get; set; }
            public int? PostedBranchID { get; set; }

            public DateTime? ShipBerthDate { get; set; }
            public bool? IsBit { get; set; }
            public string? Name { get; set; }
            public string? Code { get; set; }
            public string? Address { get; set; }
            public decimal? Rate { get; set; }
            public decimal? SystemRate { get; set; }
            public int? Period { get; set; }
            public int? Days { get; set; }
            public int? LCOptionID { get; set; }
            public string? LCNo { get; set; }
            public decimal? LCAmt { get; set; }
            public decimal? AvailableLCAmt { get; set; }
            public decimal? CreditAmt { get; set; }
            public decimal? MarginAmt { get; set; }
            public decimal? InterestAmt { get; set; }
            public decimal? AvailableAmt { get; set; }
            public decimal? AllocationPerc { get; set; }
            public decimal? InterestPerc { get; set; }
            public decimal? TolerencePerc { get; set; }
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
            public decimal? Amount { get; set; }
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
            public DateTime? LorryHireRate { get; set; }
            public decimal? QtyPerLoad { get; set; }
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
            public decimal? ExchangeRate2 { get; set; }
            public int? AccountID { get; set; }
            public int? RouteID { get; set; }
            public int? AccountID2 { get; set; }
            public decimal? Hours { get; set; }
            public int? Year { get; set; }
            public int? AreaID { get; set; }
            public int? OtherBranchID { get; set; }
            public int? TaxFormID { get; set; }
            public int? PriceCategoryID { get; set; }
            public bool? IsClosed { get; set; }
            public int? DepartmentID { get; set; }
    }
}
