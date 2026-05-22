using ERPSample.DAL.Inventory.Reports.Purchase;
using ERPSample.DAL.Inventory.Reports.Stock;
using ERPSample.Models.Inventory.Reports.Purchase;
using ERPSample.Models.Inventory.Reports.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Reports.Stock
{
    public class StockItemRegisterController : BaseController
    {
        private Lazy<DAL.General.Common.Vouchers> _DALVouchers;
        private Lazy<DAL.General.Common.Menu> _DALMenu;

        //private Lazy<DataRow> _MenuRow;
        //private Lazy<DataRow> _VoucherTypeRow;
        private DAL.General.Common.Menu DALMenu => _DALMenu.Value;
        private DAL.General.Common.Vouchers DALVouchers => _DALVouchers.Value;
        //private DataRow VoucherTypeRow => _VoucherTypeRow.Value;
        //private DataRow MenuRow => _MenuRow.Value;



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var conn = ConnectionString;  // from BaseController

            _DALVouchers = new Lazy<DAL.General.Common.Vouchers>(() => new(conn));
            _DALMenu = new Lazy<DAL.General.Common.Menu>(() => new(conn));
            //_MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(157).Rows[0]);
            //_VoucherTypeRow = new Lazy<DataRow>(() => DALVouchers.FillVoucherRow(157, MenuRow["ID"]));

            // Re-create DAL with the connection string from BaseController
            _stockeregisterDAL = new StockItemRegisterDAL(conn);
        }

        private StockItemRegisterDAL _stockeregisterDAL;   //  non-readonly, set in OnActionExecuting

        // GET - load page only, no data yet
        public IActionResult Index(int menuid)
        {
            var model = new StockItemRegisterModel();  // create model first

            model.Date = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            DataTable warehouses = DALVouchers.FillLocationusingBranch(BranchID);
            sb.Append("<option value=''> -- Choose Warehouse-- </option>");
            foreach (DataRow dr in warehouses.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Name"]);
                sb.Append("</option>");
            }
            ViewBag.Warehouse = sb.ToString();
            sb.Clear();
            return View("~/Views/Invertory/Reports/Stock/StockItemRegister.cshtml", model);
        }

        [HttpPost]
        public IActionResult GetData([FromForm] StockItemRegisterModel filter,
                              [FromForm] int draw = 1,
                              [FromForm] int start = 0,
                              [FromForm] int length = 50)
        {
            filter.BranchID = BranchID;

            var (pagedData, totalRecords, summary) = _stockeregisterDAL.GetData(filter, start, length);

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = pagedData,
                isItemwise = filter.IsItemwise   // pass this so JS knows which columns to show
            });
        }

        // POST - Ajax call, returns partial HTML (table + summary)
        //[HttpPost]
        //public IActionResult GetData([FromForm] StockItemRegisterModel filter)
        //{
        //    filter.BranchID = BranchID;
        //    var (dt, summary) = _stockeregisterDAL.GetData(filter);

        //    // Pass DataTable via ViewBag, summary via model
        //    ViewBag.ReportData = dt;
        //    return PartialView("~/Views/Invertory/Reports/Stock/_StockItemRegisterGrid.cshtml",
        //                       summary);
        //}
    }
}
