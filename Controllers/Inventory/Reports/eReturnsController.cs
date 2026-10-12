using ERPSample.DAL.Inventory.Reports;
using ERPSample.DAL.Inventory.Reports.Purchase;
using ERPSample.Models.Inventory.Reports;
using ERPSample.Models.Inventory.Reports.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Reports
{
    public class eReturnsController : BaseController
    {
        private Lazy<DAL.General.Common.Vouchers> _DALVouchers;
        private Lazy<DAL.General.Common.Menu> _DALMenu;

        private Lazy<DataRow> _MenuRow;
        private Lazy<DataRow> _VoucherTypeRow;
        private DAL.General.Common.Menu DALMenu => _DALMenu.Value;
        private DAL.General.Common.Vouchers DALVouchers => _DALVouchers.Value;
        private DataRow VoucherTypeRow => _VoucherTypeRow.Value;
        private DataRow MenuRow => _MenuRow.Value;



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var conn = ConnectionString;  // from BaseController

            _DALVouchers = new Lazy<DAL.General.Common.Vouchers>(() => new(conn));
            _DALMenu = new Lazy<DAL.General.Common.Menu>(() => new(conn));
            _MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(284).Rows[0]);
            _VoucherTypeRow = new Lazy<DataRow>(() => DALVouchers.FillVoucherRow(284, MenuRow["ID"]));

            // Re-create DAL with the connection string from BaseController
            _eReturnsDAL = new eReturnsDAL(conn);
        }

        private eReturnsDAL _eReturnsDAL;   //  non-readonly, set in OnActionExecuting

        // GET - load page only, no data yet
        public IActionResult Index()
        {
            var model = new eReturnsModel();  // create model first

            DataTable ds = DALVouchers.GetVoucherDate();

            if (ds.Rows.Count > 0 && ds.Rows[0]["StartDate"] != DBNull.Value && ds.Rows[0]["EndDate"] != DBNull.Value)
            {
                DataRow dr1 = ds.Rows[0];
                model.DateFrom = Convert.ToDateTime(dr1["StartDate"]);
                model.DateTo = Convert.ToDateTime(dr1["EndDate"]);
            }
            else
            {
                model.DateFrom = DateTime.Today.AddMonths(-1);
                model.DateTo = DateTime.Today;
            }

            StringBuilder sb = new StringBuilder();
            DataTable mode = DALVouchers.GetEmirates();
            sb.Append("<option value=''>All</option>");
            foreach (DataRow dr in mode.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["Value"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            ViewBag.Emirates = sb.ToString();
            sb.Clear();
            return View("~/Views/Invertory/Reports/eReturns.cshtml", model);
        }

        // POST - Ajax call, returns partial HTML (table + summary)
        [HttpPost]
        public IActionResult GetData([FromForm] eReturnsModel filter)
        {
            filter.BranchID = BranchID;
            var (dt, summary) = _eReturnsDAL.GetData(filter);

            // Pass DataTable via ViewBag, summary via model
            ViewBag.ReportData = dt;
            return PartialView("~/Views/Invertory/Reports/_eReturns.cshtml",
                               summary);
        }
    }
}
