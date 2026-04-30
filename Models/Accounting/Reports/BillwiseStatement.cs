using System.ComponentModel.DataAnnotations;
using System.Data;
using System;
using Microsoft.Identity.Client;

namespace ERPSample.Models.Accounting.Reports
{
    public class BillwiseStatementModel
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public DataTable ReportTable { get; set; }
        public int? AccountID { get; set; }
        public bool? Receivables { get; set; }
        public bool? Payables { get; set; }
        public bool? EffectiveDate { get; set; }
        public bool? VDate { get; set; }

        public bool? Detailed { get; set; }
        public string? DueDaysFrom { get; set; }
        public string? DueDaysUpto { get; set; }
        public bool? Pending { get; set; }
        public int? AccCategoryID { get; set; }
        public int? AccGroup { get; set; }
       
    }
}
