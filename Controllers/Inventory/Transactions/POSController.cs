using ERPSample.Hubs;
using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class POSController : BaseController
    {
        private readonly ILogger<POSController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string baseUrl;
        private readonly IHubContext<ProgressHub> _hub;

        public POSController(ILogger<POSController> logger, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            baseUrl = configuration["BaseUrl"];
        }

        private int ThisPageID
        {
            get
            {
                return 230;
            }
        }
        private int ThisVoucherID
        {
            get
            {
                return 89;
            }
        }

        private DAL.General.Masters.Locations _DALLocations;
        private DAL.General.Masters.Locations DALLocations
        {
            get
            {
                if (_DALLocations == null)
                {
                    _DALLocations = new DAL.General.Masters.Locations(ConnectionString);
                }
                return _DALLocations;
            }
        }

        private DAL.General.Common.Menu _DALMenu;
        private DAL.General.Common.Menu DALMenu
        {
            get
            {
                if (_DALMenu == null)
                {
                    _DALMenu = new DAL.General.Common.Menu(ConnectionString);
                }
                return _DALMenu;
            }

        }

        private DAL.General.Common.Vouchers _DALVouchers;
        private DAL.General.Common.Vouchers DALVouchers
        {
            get
            {
                if (_DALVouchers == null)
                {
                    _DALVouchers = new DAL.General.Common.Vouchers(ConnectionString);
                }
                return _DALVouchers;
            }

        }

        private DataRow _MenuRow;
        private DataRow MenuRow
        {
            get
            {
                if (_MenuRow == null)
                {
                    _MenuRow = DALMenu.LoadWindowsForm(ThisPageID).Rows[0];

                }
                return _MenuRow;
            }
        }

        private DataRow _VoucherTypeRow;
        private DataRow VoucherTypeRow
        {
            get
            {
                if (_VoucherTypeRow == null)
                {
                    _VoucherTypeRow = DALVouchers.FillVoucherRow(ThisPageID, MenuRow["ID"]);

                }
                return _VoucherTypeRow;
            }
        }

        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);

            //DataSet ds = DALVouchers.FillVoucher(BranchID, MenuRow["ID"]);
            //DataTable dt2 = ds.Tables[1];
            //DataRow dr2 = dt2.Rows[0];

            var NextVNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);
            string? username = null;

            DataTable addedBy = DALVouchers.AddedBy((int)UserID);

            if (addedBy != null && addedBy.Rows.Count > 0)
            {
                DataRow dr = addedBy.Rows[0];

                if (dr["FirstName"] != DBNull.Value)
                {
                    username = dr["FirstName"].ToString();
                }
            }
            DataTable ModeID = DALVouchers.GetModeID();
            DataRow BankDr = null;
            DataRow CashDr = null;
            string BankID = "";
            string Bank = "";
            string CashID = "";
            string Cash = "";
            if (ModeID != null && ModeID.Rows.Count > 0)
            {
                BankDr = ModeID.Rows[0];
                BankID=BankDr["ID"].ToString();
                Bank = BankDr["Value"].ToString();
            }

            if (ModeID != null && ModeID.Rows.Count > 1)
            {
                CashDr = ModeID.Rows[1];
                CashID = CashDr["ID"].ToString();
                Cash = CashDr["Value"].ToString();
            }

            ViewBag.Bank = BankID ?? null;
            ViewBag.Cash = CashID ?? null;
            //DataRow Credit = ModeID.Rows[2];
            //ViewBag.Credit = Credit;

            ViewBag.VoucherNo = NextVNo;
            ViewBag.voucher = VoucherTypeRow;
            ViewBag.MenuID = MenuID;
            ViewBag.RowType = 1;// dr2["RowType"].ToString();
            ViewBag.VoucherID = ThisVoucherID;

            // First row serial number
            ViewBag.FirstRowSerial = 1;

            // Set today's date formatted for input type="date"
            ViewBag.TodayDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.Salesman = username;
            return View("~/Views/Invertory/Transactions/POS.cshtml");
        }

        public async Task<IActionResult> NewRow(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;
            sb.Append("<tr>");
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            //Product Image
            sb.Append(" <td>");
            sb.Append(" <img src='../Resources/demo2.jpg' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer;' height='50px' width='50px' />");
            sb.Append(" </td>");

            // 1. Product Code (Wider)
            sb.Append("<td id='TdproductCode" + Sn + "' >");
            sb.Append("<input type='text' id='productCode" + Sn + "' class='form-control productCode' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + Sn + "' ");
            sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2. Unit (Wider)
            sb.Append("<td id='unitTd" + Sn + "' >");
            sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "'  class='form-select ItemUnit excelCells'></select>");
            sb.Append("</td>");

            // 3. Qty
            sb.Append("<td id='qtyTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemQty' element-id='" + Sn + "'  id='ItemQty" + Sn + "' />");
            sb.Append("</td>");

            // 4. Rate
            sb.Append("<td id='rateTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemRate excelCells' element-id='" + Sn + "' id='ItemRate" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 5. Gross Amount
            sb.Append("<td class='ItemGrossAmtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' element-id='" + Sn + "'  id='ItemGrossAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 6. Discount %
            sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' taxTypeID='' element-id='" + Sn + "'  id='ItemDiscPer" + Sn + "' />");
            sb.Append("</td>");

            // 7. Discount Amount
            sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' element-id='" + Sn + "'  id='ItemDiscAmt" + Sn + "' />");
            sb.Append("</td>");

            // 8. Amount
            sb.Append("<td class='amtTd' id='amtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemAmt excelCells' element-id='" + Sn + "'  id='ItemAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 9. Tax %
            sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "'  id='ItemTaxPer" + Sn + "' />");
            sb.Append("</td>");

            // 10. Tax Amount
            sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' element-id='" + Sn + "'  id='ItemTaxAmt" + Sn + "' />");
            sb.Append("</td>");

            // 11. Total
            sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTotal excelCells' element-id='" + Sn + "'  id='ItemTotal" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 12. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' ><i class='fa-solid fa-plus'></i></button></td>");

            sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
            sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
            sb.Append("</td>");
            sb.Append("<td style=''>");
            sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");

            string NewEntry = sb.ToString();
            return Json(new { success = true, newrow = NewEntry });
        }

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
                request.FiTransactions.CurrencyID = 1;
                request.FiTransactions.IsPostDated = false;
                request.FiTransactions.CompanyID = (int)BranchID;
                request.FiTransactions.StatusID = 806;
                request.FiTransactions.IsAutoEntry = false;
                request.FiTransactions.Active = true;
                request.FiTransactions.Cancelled = false;
                request.FiTransactions.Posted = true;
                request.FiTransactions.PageID = ThisPageID;
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
