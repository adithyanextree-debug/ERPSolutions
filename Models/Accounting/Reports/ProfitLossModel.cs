using System.ComponentModel.DataAnnotations;
using System.Data;
using System;

namespace ERPSample.Models.Accounting.Reports
{
    public class ProfitLossModel
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public bool TwoSided { get; set; }
        public DataSet ReportTable { get; set; }
    }

    public class CorporateTax
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public DataSet ReportTable { get; set; }
    }
}
