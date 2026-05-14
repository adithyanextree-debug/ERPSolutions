using DocumentFormat.OpenXml.Spreadsheet;
using ERPSample.Hubs;
using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Text;
using static ERPSample.Controllers.BaseController;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class SalesQuotationController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _baseUrl;
        private readonly IHubContext<ProgressHub> _hub;

        // Lazy-initialized DAL and data members
        private readonly Lazy<DAL.General.Common.Vouchers> _dalVouchers;
        private readonly Lazy<DAL.General.Common.Menu> _dalMenu;
        private readonly Lazy<DataRow> _menuRow;
        private readonly Lazy<DataRow> _voucherTypeRow;

        public SalesQuotationController(
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            IHubContext<ProgressHub> hub)
        {
            _webHostEnvironment = webHostEnvironment;
            _baseUrl = configuration["BaseUrl"];
            _hub = hub;

            _dalVouchers = new Lazy<DAL.General.Common.Vouchers>(() => new DAL.General.Common.Vouchers(ConnectionString));
            _dalMenu = new Lazy<DAL.General.Common.Menu>(() => new DAL.General.Common.Menu(ConnectionString));
            _menuRow = new Lazy<DataRow>(() => _dalMenu.Value.LoadWindowsForm(233).Rows[0]);
            _voucherTypeRow = new Lazy<DataRow>(() => _dalVouchers.Value.FillVoucherRow(233, _menuRow.Value["ID"]));
        }

        // Clean accessors — no null checks needed
        private DAL.General.Common.Vouchers DALVouchers => _dalVouchers.Value;
        private DAL.General.Common.Menu DALMenu => _dalMenu.Value;
        private DataRow MenuRow => _menuRow.Value;
        private DataRow VoucherTypeRow => _voucherTypeRow.Value;

        public async Task<IActionResult> Index(long MenuID)
        {
            SetUserPermissions(MenuID);
            DataSet dataSet = DALVouchers.FillVoucher(BranchID, MenuRow["ID"]);
            DataTable vouchersTable = dataSet.Tables[0];
            DataRow voucherMeta = dataSet.Tables[1].Rows[0];

            StringBuilder htmlBuilder = new StringBuilder();
            for (int index = 0; index < vouchersTable.Rows.Count; index++)
            {
                DataRow row = vouchersTable.Rows[index];
                int rowNumber = index + 1;
                string rowClass = rowNumber % 2 == 0 ? "even" : "odd";

                htmlBuilder.Append(
                    $"<tr class='{rowClass}'>" +
                    $"<td>{rowNumber}</td>" +
                    $"<td>{row["TransactionNo"]}</td>" +
                    $"<td>{Convert.ToDateTime(row["Date"]):dd/MM/yyyy}</td>" +
                    $"<td>{row["AccountName"]}</td>" +
                    $"<td>{row["Amount"]}</td>" +
                    $"<td><ul class='action'><li class='edit' onclick='RowClick({row["ID"]})'> <a href='#'><i class='icon-pencil-alt'></i></a></li></ul></td>" +
                    "</tr>");
            }

            ViewBag.voucher = VoucherTypeRow;
            ViewBag.MenuID = MenuID;
            ViewBag.DataTable = htmlBuilder.ToString();
            ViewBag.RowType = voucherMeta["RowType"].ToString();
            ViewBag.VoucherCode = voucherMeta["Code"].ToString();
            ViewBag.VoucherID = voucherMeta["VoucherID"].ToString();

            return View("~/Views/Invertory/Transactions/SalesQuotation.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> InsertTransaction([FromBody] SaveTransactionEntryRequest request)
        {
            try
            {

                if (request.FiTransactions.ID == null || request.FiTransactions.ID == 0)
                {
                    var NextVNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);
                    request.FiTransactions.TransactionNo = NextVNo.ToString();
                    request.FiTransactions.SerialNo = Convert.ToInt64(NextVNo);
                }
                request.FiTransactions.AddedBy = (int)UserID;
                request.FiTransactions.EditedBy = (int)UserID;
                request.FiTransactions.CurrencyID = 17;
                request.FiTransactions.IsPostDated = false;
                request.FiTransactions.CompanyID = (int)BranchID;
                request.FiTransactions.StatusID = 806;
                request.FiTransactions.IsAutoEntry = false;
                request.FiTransactions.Active = true;
                request.FiTransactions.Cancelled = false;
                request.FiTransactions.Posted = true;
                request.FiTransactions.PageID = (int)PageIDs.SalesQuotation;
                request.FiTransactions.ApprovalStatus = 'A';
                foreach (var item in request.InvTransItems)
                {
                    item.TranType = "Normal";
                    item.Visible = true;
                }

                List<Models.InvTransItems> InvTransItems = request.InvTransItems;
                List<Models.FiTransactionEntries> FiTransactionEntries = request.FiTransactionEntries;
                Models.FiTransactions FiTransactions = request.FiTransactions;
                Models.FiTransactionAdditionals FiTransactionAdditionals = request.FiTransactionAdditionals;
                DALVouchers.InsertTransaction(request);
                //additional 
                return Json(new { success = true });
            }
            catch (Exception ex) { throw; }

        }
    }
}
