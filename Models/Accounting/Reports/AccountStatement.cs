using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ERPSample.Models.Accounting.Reports
{
    public class AccountStatementModel
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Account is mandatory!!")]
        public int? AccountID { get; set; }
        public String AccountName { get; set; }
        public DataTable ReportTable { get; set; }
      //  public List<AccountModel> Accounts { get; set; }
    }
    public class AccountModel
    {
        public Int64 ID { get; set; }
        public String AccountName { get; set; }
    }
}
