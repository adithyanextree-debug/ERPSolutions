using ERPSample.DAL.Inventory.Reports.Purchase;
using ERPSample.Models.Inventory.Reports.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Reports.Purchase
{
    // Controllers/PurchaseRegisterController.cs
    public class PurchaseRegisterController : BaseController
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
            _MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(15).Rows[0]);
            _VoucherTypeRow = new Lazy<DataRow>(() => DALVouchers.FillVoucherRow(15, MenuRow["ID"]));

            // Re-create DAL with the connection string from BaseController
            _purchaseDAL = new PurchaseRegisterDAL(conn);
        }

        private PurchaseRegisterDAL _purchaseDAL;   //  non-readonly, set in OnActionExecuting

        // GET - load page only, no data yet
        public IActionResult Index()
        {
            ViewBag.voucher = VoucherTypeRow;
            var model = new PurchaseRegisterModel();  // create model first

            DataTable ds = DALVouchers.GetVoucherDate();

            if (ds.Rows.Count > 0 &&ds.Rows[0]["StartDate"] != DBNull.Value && ds.Rows[0]["EndDate"] != DBNull.Value)
            {
                DataRow dr1 = ds.Rows[0];
                model.FromDate = Convert.ToDateTime(dr1["StartDate"]);
                model.ToDate = Convert.ToDateTime(dr1["EndDate"]);
            }
            else
            {
                model.FromDate = DateTime.Today.AddMonths(-1);
                model.ToDate = DateTime.Today;
            }

            StringBuilder sb = new StringBuilder();
            DataTable mode = DALVouchers.GetMode();
            sb.Append("<option value=''> -- Choose Payment Type -- </option>");
            foreach (DataRow dr in mode.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            ViewBag.PaymentMode = sb.ToString();
            sb.Clear();
            return View("~/Views/Invertory/Reports/Purchase/PurchaseRegister.cshtml", model);
        }

        // POST - Ajax call, returns partial HTML (table + summary)
        [HttpPost]
        public IActionResult GetData([FromForm] PurchaseRegisterModel filter)
        {
            filter.BranchID = BranchID;
            var (dt, summary) = _purchaseDAL.GetData(filter);

            // Pass DataTable via ViewBag, summary via model
            ViewBag.ReportData = dt;
            return PartialView("~/Views/Invertory/Reports/Purchase/_PurchaseGrid.cshtml",
                               summary);
        }
    }
}
