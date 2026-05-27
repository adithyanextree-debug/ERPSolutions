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
    public class InventoryProfitController : BaseController
    {
        private Lazy<DAL.General.Common.Vouchers> _DALVouchers;
        private Lazy<DAL.General.Common.Menu> _DALMenu;

        private Lazy<DataRow> _MenuRow;
        private Lazy<DataRow> _VoucherTypeRow;
        private DAL.General.Common.Menu DALMenu => _DALMenu.Value;
        private DAL.General.Common.Vouchers DALVouchers => _DALVouchers.Value;


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var conn = ConnectionString;  // from BaseController

            _DALVouchers = new Lazy<DAL.General.Common.Vouchers>(() => new(conn));
            _DALMenu = new Lazy<DAL.General.Common.Menu>(() => new(conn));
            _MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(278).Rows[0]);

            // Re-create DAL with the connection string from BaseController
            _inventoryprofitDAL = new InventoryProfitDAL(conn);
        }

        private InventoryProfitDAL _inventoryprofitDAL;   //  non-readonly, set in OnActionExecuting

        // GET - load page only, no data yet
        public IActionResult Index()
        {
            var model = new InventoryProfitModel();  // create model first

            DataTable ds = DALVouchers.GetVoucherDate();

            if (ds.Rows.Count > 0 && ds.Rows[0]["StartDate"] != DBNull.Value && ds.Rows[0]["EndDate"] != DBNull.Value)
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
           
            return View("~/Views/Invertory/Reports/InventoryProfit.cshtml", model);
        }

        // POST - Ajax call, returns partial HTML (table + summary)
        [HttpPost]
        public IActionResult GetData([FromForm] InventoryProfitModel filter)
        {
            filter.BranchID = BranchID;
            var (dt, summary) = _inventoryprofitDAL.GetData(filter);

            // Pass DataTable via ViewBag, summary via model
            ViewBag.ReportData = dt;
            return PartialView("~/Views/Invertory/Reports/_InventoryProfitGrid.cshtml",
                               summary);
        }
    }
}
