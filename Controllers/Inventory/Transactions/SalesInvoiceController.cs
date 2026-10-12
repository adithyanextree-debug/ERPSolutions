using ERPSample.Models;
using ERPSample.Models.Inventory;
using ERPSample.Models.Inventory.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ERPSample.Hubs;

//using OfficeOpenXml;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using ClosedXML.Excel;


namespace ERPSample.Controllers.Inventory.Transactions
{
    public class SalesInvoiceController : BaseController
    {
       // private readonly ILogger<SalesInvoiceController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string baseUrl;
        private readonly IHubContext<ProgressHub> _hub;
        public SalesInvoiceController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHubContext<ProgressHub> hub)
        {
            //_logger = logger;
            _webHostEnvironment = webHostEnvironment;
            baseUrl = configuration["BaseUrl"];
            _hub = hub;
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

        private DataRow _MenuRow;
        private DataRow MenuRow
        {
            get
            {
                if (_MenuRow == null)
                {
                    _MenuRow = DALMenu.LoadWindowsForm(149).Rows[0];

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
                    _VoucherTypeRow = DALVouchers.FillVoucherRow(149, MenuRow["ID"]);
                    //_VoucherTypeRow = DALVouchers.FillVoucherRow(149, MenuRow["ID"]);

                }
                return _VoucherTypeRow;
            }
        }
        
        public async Task<IActionResult> Index(long MenuID)
        {
            SetUserPermissions(MenuID);
            DataSet dataSet = DALVouchers.FillVoucher(BranchID, MenuID);
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

            return View("~/Views/Invertory/Transactions/SalesInvoice.cshtml");// Json(new { itemmaster = itemmaster, success = true});
        }

        public async Task<IActionResult> GetCashDetails()
        {
            StringBuilder sb = new StringBuilder();
            int Sn = 1;
            sb.Append("<tr id='CashRow" + Sn + "' dataentry-id=''>");

            // Account Name
            sb.Append("<td class='p-1' style='width:5cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm CashAccountCode excelCells' id='CashAccountCode" + Sn + "' name='CashAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'CashAccountCode" + Sn + "','lookupDIVCashAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('CashAccountCode" + Sn + "','lookupDIVCashAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='CashAccount' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVCashAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdCashAccountDescription" + Sn + "' style='width:10cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='CashAccountDescription excelCells form-control form-control-sm' type='text' id='CashAccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Amount
            sb.Append("<td class='p-1' id='TdCashAmount" + Sn + "' style='width:3cm; '>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells CashAmount form-control form-control-sm' id='CashAmount" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCash' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='Cashdeleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCash' id='Cashdeleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            //sb.Append("<td style='display:none;'>");
            //sb.Append("<input type='hidden' class='cashitemid excelCells form-control' id='cashitemid" + Sn + "' value='' element-id='" + Sn + "'>");
            //sb.Append("</td>");

            sb.Append("</tr>");
            string htmlString = sb.ToString();
            return Json(new { cashTable = htmlString });
        }

        public async Task<IActionResult> GetCardDetails()
        {
            StringBuilder sb = new StringBuilder();
            int Sn = 1;
            sb.Append("<tr id='CardRow" + Sn + "' dataentry-id=''>");

            // Account Name
            sb.Append("<td class='p-1' style='width:5cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm CardAccountCode excelCells' id='CardAccountCode" + Sn + "' name='CardAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'CardAccountCode" + Sn + "','lookupDIVCardAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('CardAccountCode" + Sn + "','lookupDIVCardAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='CardAccount' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVCardAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdCardAccountDescription" + Sn + "' style='width:10cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='CardAccountDescription excelCells form-control form-control-sm' type='text' id='CardAccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Amount
            sb.Append("<td class='p-1' id='TdCardAmount" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells CardAmount form-control form-control-sm' id='CardAmount" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCard' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='Carddeleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCard' id='Carddeleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            //sb.Append("<td style='display:none;'>");
            //sb.Append("<input type='hidden' class='carditemid excelCells form-control' id='carditemid" + Sn + "' value='' element-id='" + Sn + "'>");
            //sb.Append("</td>");

            sb.Append("</tr>");
            string htmlString = sb.ToString();
            return Json(new { cardTable = htmlString });
        }

        public async Task<IActionResult> GetChequeDetails()
        {
            StringBuilder sb = new StringBuilder();
            int Sn = 1;
            sb.Append("<tr id='ChequeRow" + Sn + "' dataentry-id=''>");

            // Account Name
            sb.Append("<td class='p-1' style='width:5cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm ChequeAccountCode excelCells' id='ChequeAccountCode" + Sn + "' name='ChequeAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'ChequeAccountCode" + Sn + "','lookupDIVChequeAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('ChequeAccountCode" + Sn + "','lookupDIVChequeAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='BankorCheque' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVChequeAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // ChequeNo
            sb.Append("<td class='p-1' id='TdChequeNo" + Sn + "' style='width:3cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='ChequeNo excelCells form-control form-control-sm' type='text' id='ChequeNo" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Cheque Date
            sb.Append("<td class='p-1' id='TdChequeDate" + Sn + "' style='width:3cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' oninput='formatDateInput(this)' class='ChequeDate form-control form-control-sm excelCells' type='text' id='ChequeDate" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Bank Name
            sb.Append("<td class='p-1' style='width:5cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm BankAccountCode excelCells' id='BankAccountCode" + Sn + "' name='BankAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'BankAccountCode" + Sn + "','lookupDIVBankAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('BankAccountCode" + Sn + "','lookupDIVBankAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='BankName' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVBankAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Clearing Days
            sb.Append("<td class='p-1' id='TdChequeClearingDays" + Sn + "' style='width:3cm; '>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells ChequeClearingDays form-control form-control-sm' id='ChequeClearingDays" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdChequeAccountDescription" + Sn + "' style='width:3cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='ChequeAccountDescription excelCells form-control form-control-sm' type='text' id='ChequeAccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Amount
            sb.Append("<td class='p-1' id='TdChequeAmount" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells ChequeAmount form-control form-control-sm' id='ChequeAmount" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCheque' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='Chequedeleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCheque' id='Chequedeleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            sb.Append("<td style='display:none;' chequeitemid=''>");
            sb.Append("</td>");


            //sb.Append("<td style='display:none;'>");
            //sb.Append("<input type='hidden' class='chequeitemid excelCells form-control' id='chequeitemid" + Sn + "' value='' element-id='" + Sn + "'>");
            //sb.Append("</td>");

            sb.Append("</tr>");
            string htmlString = sb.ToString();
            return Json(new { chequeTable = htmlString });
        }

        [HttpGet]
        public async Task<IActionResult> NewRowCash(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int Sn = (no ?? 0) + 1;

            sb.Append("<tr id='CashRow" + Sn + "' dataentry-id=''>");

            // Account Name
            sb.Append("<td class='p-1' style='width:5cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm CashAccountCode excelCells' id='CashAccountCode" + Sn + "' name='CashAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'CashAccountCode" + Sn + "','lookupDIVCashAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('CashAccountCode" + Sn + "','lookupDIVCashAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='CashAccount' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVCashAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdCashAccountDescription" + Sn + "' style='width:10cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='CashAccountDescription excelCells form-control form-control-sm' type='text' id='CashAccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Amount
            sb.Append("<td class='p-1' id='TdCashAmount" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells CashAmount form-control form-control-sm' id='CashAmount" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCash' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='Cashdeleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCash' id='Cashdeleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            //sb.Append("<td style='display:none;'>");
            //sb.Append("<input type='hidden' class='cashitemid excelCells form-control' id='cashitemid" + Sn + "' value='' element-id='" + Sn + "'>");
            //sb.Append("</td>");

            sb.Append("</tr>");
            return Json(new { success = true, newrow = sb.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> NewRowCard(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int Sn = (no ?? 0) + 1;

            sb.Append("<tr id='CardRow" + Sn + "' dataentry-id=''>");

            // Account Name
            sb.Append("<td class='p-1' style='width:5cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm CardAccountCode excelCells' id='CardAccountCode" + Sn + "' name='CardAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'CardAccountCode" + Sn + "','lookupDIVCardAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('CardAccountCode" + Sn + "','lookupDIVCardAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='CardAccount' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVCardAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdCardAccountDescription" + Sn + "' style='width:10cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='CardAccountDescription excelCells form-control form-control-sm' type='text' id='CardAccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Amount
            sb.Append("<td class='p-1' id='TdCardAmount" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells CardAmount form-control form-control-sm' id='CardAmount" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCard' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='Carddeleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCard' id='Carddeleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            //sb.Append("<td style='display:none;'>");
            //sb.Append("<input type='hidden' class='carditemid excelCells form-control' id='carditemid" + Sn + "' value='' element-id='" + Sn + "'>");
            //sb.Append("</td>");

            sb.Append("</tr>");
            return Json(new { success = true, newrow = sb.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> NewRowCheque(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int Sn = (no ?? 0) + 1;

            sb.Append("<tr id='ChequeRow" + Sn + "' dataentry-id=''>");

            // Account Name
            sb.Append("<td class='p-1' style='width:5cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm ChequeAccountCode excelCells' id='ChequeAccountCode" + Sn + "' name='ChequeAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'ChequeAccountCode" + Sn + "','lookupDIVChequeAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('ChequeAccountCode" + Sn + "','lookupDIVChequeAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='BankorCheque' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVChequeAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // ChequeNo
            sb.Append("<td class='p-1' id='TdChequeNo" + Sn + "' style='width:3cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='ChequeNo excelCells form-control form-control-sm' type='text' id='ChequeNo" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Cheque Date
            sb.Append("<td class='p-1' id='TdChequeDate" + Sn + "' style='width:3cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' oninput='formatDateInput(this)' class='ChequeDate form-control form-control-sm excelCells' type='text' id='ChequeDate" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Bank Name
            sb.Append("<td class='p-1' style='width:5cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm BankAccountCode excelCells' id='BankAccountCode" + Sn + "' name='BankAccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'BankAccountCode" + Sn + "','lookupDIVBankAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('BankAccountCode" + Sn + "','lookupDIVBankAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='BankName' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVBankAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Clearing Days
            sb.Append("<td class='p-1' id='TdChequeClearingDays" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells ChequeClearingDays form-control form-control-sm' id='ChequeClearingDays" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdChequeAccountDescription" + Sn + "' style='width:3cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='ChequeAccountDescription excelCells form-control form-control-sm' type='text' id='ChequeAccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Amount
            sb.Append("<td class='p-1' id='TdChequeAmount" + Sn + "' style='width:3cm; min-width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells ChequeAmount form-control form-control-sm' id='ChequeAmount" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCheque' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='Chequedeleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCheque' id='Chequedeleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            sb.Append("<td style='display:none;' chequeitemid=''>");
            sb.Append("</td>");



            //sb.Append("<td style='display:none;'>");
            //sb.Append("<input type='hidden' class='chequeitemid excelCells form-control' id='chequeitemid" + Sn + "' value='' element-id='" + Sn + "'>");
            //sb.Append("</td>");

            sb.Append("</tr>");
            return Json(new { success = true, newrow = sb.ToString() });
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
                request.FiTransactions.PageID = (int)PageIDs.SalesInvoice;
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
                string result = DALVouchers.InsertTransaction(request);
                int transactionId;
                if (int.TryParse(result, out transactionId))
                {
                    return Json(new { success = true, transactionNo = transactionId });
                }

                return Json(new { success = false, message = result });
            }
            catch (Exception ex) { throw; }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile, int AccountID, [FromForm] Models.FiTransactions transaction, [FromForm] Models.FiTransactionAdditionals additionals)
        {
            string progressKey = Guid.NewGuid().ToString();

            await _hub.Clients.All.SendAsync("progress", progressKey, 1, "Uploading file...");

            if (excelFile == null || excelFile.Length == 0)
            {
                return Json(new { success = false, message = "No file uploaded." });
            }

            List<ProductInformation> productInfos = new();
            List<Models.InvTransItems> InvTransItems = new();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0; 

                await _hub.Clients.All.SendAsync("progress", progressKey, 10, "Reading Excel...");

                using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        return Json(new { success = false, message = "Excel file invalid." });

                    var headerRow = worksheet.Row(1);
                    int asinCol = 0, qtyCol = 0, costCol = 0;

                    foreach (var cell in headerRow.CellsUsed())
                    {
                        var header = cell.GetString().Trim();
                        if (header.Equals("ASIN", StringComparison.OrdinalIgnoreCase)) asinCol = cell.Address.ColumnNumber;
                        else if (header.Equals("Item Quantity", StringComparison.OrdinalIgnoreCase)) qtyCol = cell.Address.ColumnNumber;
                        else if (header.Equals("Item Cost Excluding vat", StringComparison.OrdinalIgnoreCase)) costCol = cell.Address.ColumnNumber;
                    }

                    if (asinCol == 0 || qtyCol == 0 || costCol == 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Excel format is invalid. Required columns not found."
                        });
                    }

                    var rowCount = worksheet.LastRowUsed().RowNumber();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var code = worksheet.Cell(row, asinCol).GetString().Trim();
                        if (string.IsNullOrWhiteSpace(code)) continue;

                        //int.TryParse(worksheet.Cell(row, qtyCol).GetString().Trim(), out int qty);

                        //decimal.TryParse(
                        //    ExtractNumericPart(
                        //        worksheet.Cell(row, costCol).GetString().Trim()),
                        //    out decimal cost
                        //);
                        // ---- QTY (safe for numeric + text cells) ----
                        var qtyCell = worksheet.Cell(row, qtyCol);
                        int qty = 0;

                        if (qtyCell.DataType == XLDataType.Number)
                        {
                            qty = (int)qtyCell.GetDouble();
                        }
                        else
                        {
                            int.TryParse(qtyCell.GetString().Trim(), out qty);
                        }

                        // ---- COST (culture + format safe) ----
                        var costCell = worksheet.Cell(row, costCol);
                        decimal cost = 0;

                        if (costCell.DataType == XLDataType.Number)
                        {
                            cost = costCell.GetDouble() != 0
                                ? Convert.ToDecimal(costCell.GetDouble())
                                : 0;
                        }
                        else
                        {
                            decimal.TryParse(
                                ExtractNumericPart(costCell.GetString().Trim()),
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture,
                                out cost
                            );
                        }

                        productInfos.Add(new ProductInformation
                        {
                            ProductCode = code,
                            Qty = qty,
                            Cost = cost
                        });

                        int percent = 10 + (int)((row / (double)rowCount) * 30);
                        await _hub.Clients.All.SendAsync("progress", progressKey, percent, "Reading Excel rows...");
                    }
                }
            }

            await _hub.Clients.All.SendAsync("progress", progressKey, 45, "Fetching items from DB...");

            DataSet ds = DALVouchers.GetItemsByProductCodes(productInfos);

            DataTable foundItems = ds.Tables[0];
            DataTable missingItems = ds.Tables[1];

            // BUILD MISSING MESSAGE FIRST (independent of found items)
            string missingMessage = string.Empty;

            if (missingItems != null && missingItems.Rows.Count > 0)
            {
                var missingCodes = missingItems.Rows
                    .Cast<DataRow>()
                    .Select(dr => dr["MissingProductCode"].ToString());

                missingMessage = "The following items are missing: " +
                                 string.Join(", ", missingCodes);
            }

            int total = foundItems.Rows.Count;
            int counter = 0;

            // IF FOUND ITEMS EXIST  PROCESS & SAVE (SAME FLOW)
            if (foundItems.Rows.Count > 0)
            {
                foreach (DataRow row in foundItems.Rows)
                {
                    counter++;

                    Models.InvTransItems invTransItem = new Models.InvTransItems();
                    int itemID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0;
                    decimal factor = 1;

                    DataSet details = DALVouchers.ItemAvailableUnitForExcel(itemID);
                    DataTable unitList = details.Tables[0];

                    string selectedUnit = "No";
                    if (unitList.Rows.Count > 0)
                    {
                        var u = unitList.Rows[0];
                        selectedUnit = u["Unit"] == DBNull.Value ? "No" : u["Unit"].ToString();
                        factor = u["Factor"] == DBNull.Value ? 1 : Convert.ToDecimal(u["Factor"]);
                    }

                    invTransItem.ItemID = itemID;
                    invTransItem.Rate = row["Cost"] != DBNull.Value ? Convert.ToDecimal(row["Cost"]) : 0;
                    invTransItem.Qty = row["Qty"] != DBNull.Value ? Convert.ToDecimal(row["Qty"]) : 0;
                    invTransItem.TempQty = invTransItem.Qty;
                    invTransItem.BasicQty = factor * invTransItem.Qty;
                    invTransItem.StockQty = factor * invTransItem.Qty;
                    invTransItem.Unit = selectedUnit;
                    invTransItem.Factor = factor;
                    invTransItem.OutLocID = additionals.OutLocID ?? 0;
                    invTransItem.TaxValue = row["TaxValue"] != DBNull.Value ? Convert.ToDecimal(row["TaxValue"]) : 0;
                    invTransItem.TaxTypeID = row["TaxTypeID"] != DBNull.Value ? Convert.ToInt32(row["TaxTypeID"]) : 0;
                   // invTransItem.TaxTypeID = Convert.ToInt32(row["TaxTypeID"]);
                    invTransItem.TaxPerc = row["SalesPerc"] != DBNull.Value ? Convert.ToDecimal(row["SalesPerc"]) : 0;
                    invTransItem.RowState = 1; // Assuming 1 means "new" or "added"
                    InvTransItems.Add(invTransItem);

                    int percent = 45 + (int)((counter / (double)total) * 40);
                    await _hub.Clients.All.SendAsync("progress", progressKey, percent, "Processing items...");
                }
                transaction.CashAmount = InvTransItems.Sum(i => (i.Rate * i.Qty) + i.TaxValue);
                transaction.TaxAmount = InvTransItems.Sum(i => i.TaxValue);
                FiTransactionEntries entries = new FiTransactionEntries();
                entries.AccountID = AccountID;
                entries.Amount = InvTransItems.Sum(i => (i.Rate * i.Qty) + i.TaxValue);
                entries.FCAmount = InvTransItems.Sum(i => (i.Rate * i.Qty) + i.TaxValue);
                entries.RowState = 1;
                entries.DrCr = "D";
                entries.Nature = "M";
                entries.TranType = "Party";
                SaveTransactionEntryRequest request = new()
                {
                    InvTransItems = InvTransItems,
                    FiTransactions = transaction,
                    FiTransactionAdditionals = additionals,
                    FiTransactionEntries = new List<FiTransactionEntries> { entries }
                };

                await _hub.Clients.All.SendAsync("progress", progressKey, 90, "Saving transaction...");
                var result = InsertTransaction(request);

                await _hub.Clients.All.SendAsync("progress", progressKey, 100, "Completed");

                //  FOUND ITEMS → SUCCESS (WITH OPTIONAL MISSING INFO)
                return Json(new
                {
                    success = true,
                    progressKey,
                    message = missingMessage
                });
            }

            // NO FOUND ITEMS → FAIL, SHOW MISSING
            await _hub.Clients.All.SendAsync("progress", progressKey, 100, "Completed");

            return Json(new
            {
                success = false,
                progressKey,
                message = string.IsNullOrEmpty(missingMessage)
                    ? "No valid items found in Excel."
                    : missingMessage
            });
        }

        string ExtractNumericPart(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "0";

            var match = Regex.Match(input, @"[\d,]*\.?\d+");
            return match.Success ? match.Value.Replace(",", "") : "0";
        }
        public async Task<IActionResult> PrintInvoice(int ID)
        {
            // 1. Header details
            var invoiceNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);
            DataTable dt1 = DALVouchers.DataTableFillTransactions(ID);
            DataRow dr1 = dt1.Rows[0];
            string invoiceDate = Convert.ToDateTime(dr1["Date"]).ToString("dd/MM/yyyy");
            string partyName = dr1["AccountName"].ToString();

            // 2. Party details
            DataSet ds = DALVouchers.PartyForPrintInTransaction(ID);
            DataTable dt2 = ds.Tables[0];
            DataRow dr2 = dt2.Rows[0];
            
            string partyAddress = dr2["AddressLineOne"].ToString();
            string partyEmail = dr2["EmailAddress"].ToString();
            string telNo = dr2["TelephoneNo"].ToString();
            DataTable dt3 = ds.Tables[1];
            DataRow dr3 = dt3.Rows[0];
            string PaymentMode = dr3["Value"].ToString();

            // 3. Line items
            DataTable dt4 = DALVouchers.DataTableFillTransactionEntries(ID);
            var imgpath = Url.Content("~/CompanyLogo/SalesInvoice.png");

            // Helper: safely read a column that may or may not exist in dt4
            string GetSafe(DataRow row, string col)
            {
                return row.Table.Columns.Contains(col) ? (row[col]?.ToString() ?? "") : "";
            }

            // TODO: Replace these with your actual company info (from config/session/DB)
            string companyName = "AL SAQER GENERAL TRADING (S.P.S-L.L.C)";
            string companyPOBox = "P.O.Box : 13293 , AJMAN- U.A.E";
            string companyTel = "Tel : 00971-067454069";
            string companyMob = "Mob : 00971-507044566";
            string companyEmail = "Email : alsaqersales@gmail.com";
            string companyWeb = "Web Store : www.go2camps.com";
            string companyTRN = "104428539100003"; // TODO: pull from config

            // TODO: these fields don't currently exist in your data layer — wire them up if needed
            string poNo = "";
            string poDate = "";
            string customerSCode = "";
            string customerTRN = "";

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
        <style type='text/css' media='print'>
            @@page {
                size: A4 portrait;
                margin: 12mm 12mm 22mm 12mm; /* extra bottom margin to leave room for fixed footer */
            }
            body { font-family: Arial, Helvetica, sans-serif; font-size: 12px; color:#000; margin:0; }
            .invoice-wrapper { width:100%; }

            .top-header { width:100%; border-collapse:collapse; margin-bottom:8px; }
            .top-header td { vertical-align: top; padding:0; }
            .company-block { font-size:11px; line-height:1.5; }
            .company-name { font-weight:bold; font-size:13px; margin-bottom:2px; }
            .logo-cell { text-align:left; vertical-align:middle; }
            .logo-cell img { max-height:75px; }
            .tax-invoice-title { text-align:right; font-weight:bold; font-size:16px; letter-spacing:1px; vertical-align:top; }

            /* Meta info section - field tables for proper wrap alignment */
            .meta-table { width:100%; border-collapse:collapse; margin:10px 0 6px 0; }
            .meta-table > tbody > tr > td { vertical-align:top; padding:0 10px 0 0; }
            .field-table { width:100%; border-collapse:collapse; }
            .field-table td { padding:1px 0; font-size:11.5px; vertical-align:top; }
            .field-table td.f-label { font-weight:bold; width:85px; white-space:nowrap; }
            .field-table td.f-colon { width:12px; }
            .field-table td.f-value { width:auto; word-wrap:break-word; }

            .items-table { width:100%; border-collapse:collapse; margin-top:8px; table-layout:fixed; }
            .items-table th, .items-table td {
                border: none;
                padding: 4px 5px;
                font-size: 10.5px;
                word-wrap: break-word;
            }
            .items-table thead th {
                border-top: 1px dotted #000;
                border-bottom: 1px dotted #000;
            }
            .items-table tbody tr:last-child td {
                border-bottom: 1px dotted #000;
            }
            .items-table th { background-color:#fff; font-weight:bold; text-align:center; }
            .items-table td.num { text-align:right; }
            .items-table td.center { text-align:center; }
           .items-table td.nowrap {
                white-space: nowrap;
            }
            .totals-table { width:100%; margin-top:0; border-collapse:collapse; }
            .totals-table td { padding:2px 6px; font-size:11.5px; }
            .totals-table .label { text-align:right; font-weight:bold; width:85%; }
            .totals-table .value { text-align:right; font-weight:bold; border-top:1px solid #000; width:110px; }

            .amount-words { margin-top:8px; font-weight:bold; font-size:11.5px; }
            .pay-note { margin-top:8px; font-size:11.5px; }

            .sign-row { width:100%; margin-top:45px; border-collapse:collapse; }
            .sign-row td { font-size:11.5px; width:50%; color:#000; }

            .computer-note { text-align:center; font-style:italic; font-size:10.5px; margin-top:12px; color:#000; }

            /* Spacer so content never collides with the fixed black footer */
            .bottom-spacer { height:40px; }

            /* Footer pinned to bottom of EVERY printed page - ONLY the category bar */
            .footer-banner {
                position: fixed;
                bottom: 0;
                left: 0;
                width: 100%;
                background-color:#000;
                color:#fff;
                text-align:center;
                font-weight:bold;
                font-size:11px;
                padding:6px 0;
                letter-spacing:0.5px;
            }
            .dotted-line {
                border: none;
                border-top: 1px dotted #000;
                margin: 6px 0;
                height: 0;
            }
        </style>
        <style type='text/css' media='screen'>
            /* On-screen preview (before print) gets the same basic look */
            body { font-family: Arial, Helvetica, sans-serif; font-size: 12px; color:#000; }
            .top-header { width:100%; border-collapse:collapse; margin-bottom:8px; }
            .top-header td { vertical-align: top; padding:0; }
            .company-name { font-weight:bold; font-size:13px; }
            .logo-cell { text-align:left; }
            .logo-cell img { max-height:75px; }
            .tax-invoice-title { text-align:right; font-weight:bold; font-size:16px; }
            .field-table { width:100%; border-collapse:collapse; }
            .field-table td.f-label { font-weight:bold; width:85px; white-space:nowrap; }
            .items-table { width:100%; border-collapse:collapse; margin-top:8px; }
            .items-table th, .items-table td { border:1px solid #000; padding:4px 5px; font-size:11px; }
            .items-table td.num { text-align:right; }
            .items-table td.nowrap {
                white-space: nowrap;
            }
            .totals-table .label { text-align:right; font-weight:bold; }
            .totals-table .value { text-align:right; font-weight:bold; border-top:1px solid #000; }
            .sign-row { width:100%; margin-top:45px; }
            .sign-row td { width:50%; }
            .computer-note { text-align:center; font-style:italic; margin-top:12px; }
            .dotted-line {border: none;border-top: 1px dotted #000;margin: 6px 0;height: 0;}
            .footer-banner { background-color:#000; color:#fff; text-align:center; font-weight:bold; padding:6px 0; margin-top:15px; }
        </style>

        <div class='invoice-wrapper' id='invoicetable'>

            <table class='top-header'>
                <tr>
                    <td style='width:40%;'>
                        <div class='company-block'>
                            <div class='company-name'>" + companyName + @"</div>
                            " + companyPOBox + @"<br/>
                            " + companyTel + @"<br/>
                            " + companyMob + @"<br/>
                            " + companyEmail + @"<br/>
                            " + companyWeb + @"
                        </div>
                    </td>
                    <td style='width:40%;' class='logo-cell'>
                        <img src='" + imgpath + @"' />
                    </td>
                    <td style='width:40%;' class='tax-invoice-title'>
                        TAX INVOICE
                    </td>
                </tr>
            </table>

            <table class='meta-table'>
                <tr>
                    <td style='width:50%;'>
                        <table class='field-table'>
                            <tr><td class='f-label'>TRN No.</td><td class='f-colon'>:</td><td class='f-value'>" + companyTRN + @"</td></tr>
                            <tr><td class='f-label'>Invoice No.</td><td class='f-colon'>:</td><td class='f-value'>" + invoiceNo + @"</td></tr>
                            <tr><td class='f-label'>Invoice Date</td><td class='f-colon'>:</td><td class='f-value'>" + invoiceDate + @"</td></tr>
                            <tr><td class='f-label'>P.O No.</td><td class='f-colon'>:</td><td class='f-value'>" + poNo + @"</td></tr>
                            <tr><td class='f-label'>P.O Date</td><td class='f-colon'>:</td><td class='f-value'>" + poDate + @"</td></tr>
                        </table>
                    </td>
                    <td style='width:50%;'>
                        <table class='field-table'>
                            <tr><td class='f-label'>Customer</td><td class='f-colon'>:</td><td class='f-value'>" + partyName + @"</td></tr>
                            <tr><td class='f-label'>Address</td><td class='f-colon'>:</td><td class='f-value'>" + partyAddress + @"</td></tr>
                            <tr><td class='f-label'>Tel.</td><td class='f-colon'>:</td><td class='f-value'>" + telNo + @"</td></tr>
                            <tr><td class='f-label'>S.Code</td><td class='f-colon'>:</td><td class='f-value'>" + customerSCode + @"</td></tr>
                            <tr><td class='f-label'>TRN No.</td><td class='f-colon'>:</td><td class='f-value'>" + customerTRN + @"</td></tr>
                        </table>
                    </td>
                </tr>
            </table>

            <table class='items-table'>
                <colgroup>
                    <col style='width:5%;'>
                    <col style='width:10%;'>
                    <col style='width:12%;'>
                    <col style='width:28%;'>
                    <col style='width:6%;'>
                    <col style='width:9%;'>
                    <col style='width:9%;'>
                    <col style='width:7%;'>
                    <col style='width:7%;'>
                    <col style='width:9%;'>
                </colgroup>
                <thead>
                    <tr>
                        <th>SN</th>
                        <th>ITEM CODE</th>
                        <th>BARCODE</th>
                        <th>DESCRIPTION</th>
                        <th>QTY</th>
                        <th>RATE</th>
                        <th>TOTAL</th>
                        <th>VAT %</th>
                        <th>VAT</th>
                        <th>AMOUNT</th>
                    </tr>
                </thead>
                <tbody>");

                    decimal grandGross = 0;
                    decimal grandVat = 0;
                    decimal grandTotal = 0;
                    int Sn = 1;

                    foreach (DataRow dr4 in dt4.Rows)
                    {
                        string itemCode = GetSafe(dr4, "ItemCode");
                        string barcode = GetSafe(dr4, "Barcode");
                        string itemName = dr4["ItemName"].ToString();
                        int qty = Convert.ToInt32(dr4["Qty"]);
                        decimal price = Convert.ToDecimal(dr4["Rate"]);
                        decimal discount = Convert.ToDecimal(dr4["Discount"]);
                        decimal tax = Convert.ToDecimal(dr4["TaxValue"]);

                        decimal lineTotal = qty * price;
                        decimal vatPercent = lineTotal > 0 ? Math.Round((tax / lineTotal) * 100, 2) : 0;
                        decimal amount = lineTotal - discount + tax;

                        grandGross += lineTotal;
                        grandVat += tax;
                        grandTotal += amount;

                        sb.Append(@"
                    <tr>
                        <td class='center'>" + Sn + @"</td>
                        <td class='center'>" + itemCode + @"</td>
                        <td class='center nowrap'>" + barcode + @"</td>
                        <td>" + itemName + @"</td>
                        <td class='num'>" + qty + @"</td>
                        <td class='num'>" + price.ToString("0.00") + @"</td>
                        <td class='num'>" + lineTotal.ToString("0.00") + @"</td>
                        <td class='num'>" + vatPercent.ToString("0.00") + @"</td>
                        <td class='num'>" + tax.ToString("0.00") + @"</td>
                        <td class='num'>" + amount.ToString("0.00") + @"</td>
                    </tr>");
                        Sn++;
                    }

                    sb.Append(@"
                </tbody>
            </table>

            <table class='totals-table'>
                <tr>
                    <td class='label'>GROSS AMOUNT :</td>
                    <td class='value'>" + grandGross.ToString("0.00") + @"</td>
                </tr>
                <tr>
                    <td class='label'>TOTAL VAT :</td>
                    <td class='value'>" + grandVat.ToString("0.00") + @"</td>
                </tr>
                <tr>
                    <td class='label'>TOTAL AMOUNT :</td>
                    <td class='value'>" + grandTotal.ToString("0.00") + @"</td>
                </tr>
            </table>

            <div class='amount-words'>" + NumberToWords(grandTotal) + @"</div>

                <hr class='dotted-line' />
                <table class='sign-row'>
                    <tr>
                        <td style='text-align:left;'>Approved By..............................</td>
                        <td style='text-align:right;'>Recieved By..............................</td>
                    </tr>
                </table>

                <div class='computer-note'>
                    *** This is a Computer generated document and does not required stamp ***
                </div>

                <div class='bottom-spacer'></div>

            </div>

            <div class='footer-banner'>
                HOUSEHOLD - CAMPING &amp; BBQ GRIL - KITCHEN ACCESSORIES - PICNIC ITEMS -SPORTS ITEMS
        </div>");

            return Json(new { table = sb.ToString(), success = true });
        }
        private string NumberToWords(decimal amount)
        {
            int rupees = (int)Math.Floor(amount);
            int fils = (int)Math.Round((amount - rupees) * 100);

            string rupeeWords = ConvertWholeNumberToWords(rupees).ToUpper();
            string filsWords = fils > 0 ? ConvertWholeNumberToWords(fils).ToUpper() + " FILS" : "";

            string result = rupeeWords + " DIRHAMS";
            if (fils > 0) result += " AND " + filsWords;
            return result;
        }

        private string ConvertWholeNumberToWords(int number)
        {
            if (number == 0) return "ZERO";

            string[] ones = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE",
        "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN",
        "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
            string[] tens = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

            string ConvertGroup(int n)
            {
                string s = "";
                if (n >= 100)
                {
                    s += ones[n / 100] + " HUNDRED ";
                    n %= 100;
                }
                if (n >= 20)
                {
                    s += tens[n / 10] + " ";
                    n %= 10;
                    if (n > 0) s += ones[n] + " ";
                }
                else if (n > 0)
                {
                    s += ones[n] + " ";
                }
                return s.Trim();
            }

            string result = "";
            int crore = number / 10000000;
            number %= 10000000;
            int lakh = number / 100000;
            number %= 100000;
            int thousand = number / 1000;
            number %= 1000;
            int rest = number;

            if (crore > 0) result += ConvertGroup(crore) + " CRORE ";
            if (lakh > 0) result += ConvertGroup(lakh) + " LAKH ";
            if (thousand > 0) result += ConvertGroup(thousand) + " THOUSAND ";
            if (rest > 0) result += ConvertGroup(rest);

            return result.Trim();
        }

        public async Task<IActionResult> PrintInvoice1(int ID)
        {
            // 1. Header details
            var invoiceNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);
            DataTable dt1 = DALVouchers.DataTableFillTransactions(ID);
            DataRow dr1 = dt1.Rows[0];
            // string invoiceNo = dr1["VoucherNo"].ToString();
            string invoiceDate = Convert.ToDateTime(dr1["Date"]).ToString("dd MMMM, yyyy");
            string accountName = dr1["AccountName"].ToString();
            // 2. Party details
            DataSet ds = DALVouchers.PartyForPrintInTransaction(ID);
            DataTable dt2 = ds.Tables[0];
            DataRow dr2 = dt2.Rows[0];
            string partyName = accountName;
            string partyAddress = dr2["AddressLineOne"].ToString();
            string partyEmail = dr2["EmailAddress"].ToString();
            string telNo = dr2["TelephoneNo"].ToString();
            DataTable dt3 = ds.Tables[1];
            DataRow dr3 = dt3.Rows[0];
            string PaymentMode = dr3["Value"].ToString();
            // 3. Line items
            DataTable dt4 = DALVouchers.DataTableFillTransactionEntries(ID);
            var imgpath = Url.Content("~/CompanyLogo/SalesInvoice.png");
            var footer = Url.Content("~/CompanyLogo/WavyDesign.jpg");
            //var header = Url.Content("~/CompanyLogo/WavyDesign2.jpg");
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
            <style type='text/css' media='print'>
                @@page {
                    size: landscape;
                    margin: 20mm 10mm 20mm 10mm;
                    min-height: 200mm; /* Approx. height for A4 minus margins */
                }
             

                .footer {
                    position: fixed;
                    bottom: 0;
                    left: 0;
                    width: 100%;
                    height: 100px; /* Adjust as needed */
                    background-color: #f0f0f0; /* Optional: Add a background color */
                    z-index: 1; /* Ensure it's above the content */
                    text-align: center; /* Center the image */
                    /* Add any other footer styles */
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }
                .table {
                    width: 100%;
                    border-collapse: collapse;
                }
               .invoice-items td, .invoice-items th {
                    padding: 8px;
                    border: 1px solid #ddd;
                }
                .total {
                    font-weight: bold;
                }
                .heading {
                    background-color: #f2f2f2;
                }
                

            </style>
            <style type='text/css' media='screen'>
             .header, .footer {
                    display: none;
              }
            </style>
            <div class='card'>
                <div class='card-block row'>
                    <div class='col-sm-12 col-lg-12 col-xl-12'>
                        
                        <div class='invoice-box table-responsive' id='invoicetable'>
                            <table class='table'>
                                <tbody>
                                    <tr>
                                        <td><img src='" + imgpath + @"' width='100px' height='100px' /></td>
                                        <td style='text-align:right;'>Invoice No. " + invoiceNo + @"</td>
                                    </tr>
                                </tbody>
                            </table>
                            <h2>INVOICE</h2>
                            <br/>
                            <table class='table'>
                                <tbody>
                                    <tr>
                                        <td>
                                            <b>Date:</b> " + invoiceDate + @"<br/>
                                            <b>Billed to:</b><br/>
                                            " + partyName + @"<br/>
                                            " + partyAddress + @"<br/>
                                            " + telNo + @"<br/>
                                            " + partyEmail + @"
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <br/>
                            <table class='table invoice-items'>
                                <thead>
                                    <tr class='heading'>
                                        <th>Sl.No.</th>
                                        <th>Item</th>
                                        <th>Quantity</th>
                                        <th>Price</th>
                                        <th>Amount</th>
                                        <th>Discount</th>
                                        <th>Tax</th>
                                        <th>Total</th>
                                    </tr>
                                </thead>
                                <tbody>");
            decimal grandTotal = 0;
            int Sn = 1;
            foreach (DataRow dr4 in dt4.Rows)
            {
                string itemName = dr4["ItemName"].ToString();
                int qty = Convert.ToInt32(dr4["Qty"]);
                decimal price = Convert.ToDecimal(dr4["Rate"]);  // Make sure this is Unit Price
                decimal discount = Convert.ToDecimal(dr4["Discount"]); // Discount amount
                decimal tax = Convert.ToDecimal(dr4["TaxValue"]);   // Tax percentage (e.g. 18)

                decimal amount = qty * price;                      // Gross amount
                decimal amountAfterDisc = amount - discount;       // After discount
                decimal total = amountAfterDisc + tax;             // Final total after discount and tax

                grandTotal += total;  // Add to grand total

                sb.Append(@"
                <tr>
                    <td>" + Sn + @"</td>
                    <td>" + itemName + @"</td>
                    <td style='text-align:center'>" + qty + @"</td>
                    <td style='text-align:right'>" + price.ToString("0.00") + @"</td>
                    <td style='text-align:right'>" + amount.ToString("0.00") + @"</td>
                    <td style='text-align:right'>" + discount.ToString("0.00") + @"</td>
                    <td style='text-align:right'>" + tax.ToString("0.00") + @"</td>
                    <td style='text-align:right'>" + total.ToString("0.00") + @"</td>
                </tr>");
                Sn++;
            }

            sb.Append(@"
                        <tr>
                            <td colspan='7' class='total' style='text-align:right;'>Total</td>
                            <td class='total' style='text-align:right'>" + grandTotal.ToString("0.00") + @"</td>
                        </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan='8' style='height:100px;width:100%;'></td>
                            </tr>
                        </tfoot>
                    </table>
                    <br/>
                    <b>Payment method:</b> " + PaymentMode + @"<br/>
                    <b>Note:</b> Thank you for choosing us!
                    </div>
                    <div class='footer'>
                        <!-- Your footer content, including the image -->
                        <img id='footerimage' style='height:100px;width:100%;' src='" + footer + @"' />
                    </div>
                    </div>
                </div>
            </div>");
            // Return as HTML
            return Json(new { table = sb.ToString(), success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SaveTransactionEntry([FromBody] SaveTransactionEntryRequest request)
        {
            List<Models.InvTransItems> InvTransItems = request.InvTransItems;
            List<Models.FiTransactionEntries> FiTransactionEntries = request.FiTransactionEntries;
            Models.FiTransactions FiTransactions = request.FiTransactions;
            Models.FiTransactionAdditionals FiTransactionAdditionals = request.FiTransactionAdditionals;

            if (FiTransactions.ID == null || FiTransactions.ID == 0)
            {
                var NextVNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);
                FiTransactions.TransactionNo = NextVNo.ToString();
                FiTransactions.SerialNo = Convert.ToInt64(NextVNo);
            }

            try
            {
                FiTransactions.AddedBy = (int)UserID;
                FiTransactions.EditedBy = (int)UserID;
                FiTransactions.CurrencyID = 1;
                FiTransactions.IsPostDated = false;
                FiTransactions.CompanyID = (int)BranchID;
                FiTransactions.StatusID = 806;
                FiTransactions.IsAutoEntry = false;
                FiTransactions.Active = true;
                FiTransactions.Cancelled = false;
                FiTransactions.Posted = true;
                FiTransactions.PageID = (int)PageIDs.SalesInvoice;
                FiTransactions.RefPageTypeID = null;
                FiTransactions.RefPageTableID = null;
                FiTransactions.FinYearID = null;
                FiTransactions.InstrumentType = null;
                FiTransactions.InstrumentNo = null;
                FiTransactions.InstrumentDate = null;
                FiTransactions.InstrumentBank = null;
                //FiTransactions.CommonNarration = null;
                FiTransactions.ApprovedBy = null;
                FiTransactions.ApprovedDate = null;
                FiTransactions.ApproveNote = null;
                FiTransactions.Action = null;
                FiTransactions.RefTransID = null;
                FiTransactions.EditedBy = null;
                FiTransactions.EditedDate = null;
                //FiTransactions.CostCentreID = null;
                FiTransactions.MachineName = null;
                FiTransactions.ApprovalStatus = 'A';
                //LCApplnTransID is additional foreign key
                int ID = 0;
                if (FiTransactions.ID == null || FiTransactions.ID == 0)
                {
                    String Result = DALVouchers.InsertTransactions(FiTransactions);
                    ID = Convert.ToInt32(Result);
                    bool isNumeric = int.TryParse(Result, out int n);
                    if (isNumeric)
                    {
                        foreach (Models.InvTransItems item in InvTransItems)
                        {
                            item.TransactionID = ID;
                            item.Pcs = null;
                            item.AdvanceRate = null;
                            item.MasterMiscID1 = null;
                            item.Description = null;
                            item.Remarks = null;
                            item.IsBit = null;
                            item.InvAvgCostID = null;
                            item.IsReturn = null;
                            item.Additional = null;
                            item.CommodityID = null;
                            item.AccountID = null;
                            item.TransactionEntryID = null;
                            item.LengthFt = null;
                            item.LengthIn = null;
                            item.LengthCm = null;
                            item.GirthFt = null;
                            item.GirthIn = null;
                            item.GirthCm = null;
                            item.ThicknessFt = null;
                            item.ThicknessIn = null;
                            item.ThicknessCm = null;
                            item.ShortageQty = null;
                            item.AvgCostID = null;
                            item.RefTransItemID = null;
                            item.Status = null;
                            item.Cancel = null;
                            item.MeasuredByID = null;
                            item.FinishDate = null;
                            item.UpdateDate = null;
                            item.IsSameForPcs = null;
                            item.RefID = null;
                            item.BatchNo = null;
                            item.Margin = null;
                            item.SizeMasterID = null;
                            item.TranType = "Normal";
                            item.CostPerc = null;
                            item.ManufactureDate = null;
                            item.ExpiryDate = null;
                            item.FOCQty = null;
                            item.GroupItemID = null;
                            item.PriceCategoryID = null;
                            item.SerialNo = null;
                            item.ReplaceQty = null;
                            item.PrintedMRP = null;
                            item.PrintedRate = null;
                            item.PTSRate = null;
                            item.PTRRate = null;
                            item.TempRate = null;
                            item.StockItemID = null;
                            item.Visible = true;
                            item.RowType = -1;
                            item.InLocID = null;
                            DALVouchers.InvTransItemsMaster(item);

                        }
                        DALVouchers.InsertTransactionEntries(ID);

                        FiTransactionAdditionals.TransactionID = ID;
                        FiTransactionAdditionals.RefTransID1 = null;
                        FiTransactionAdditionals.RefTransID2 = null;
                        FiTransactionAdditionals.MeasureTypeID = null;
                        FiTransactionAdditionals.LoadMeasureTypeID = null;
                        FiTransactionAdditionals.ConsignTermID = null;
                        FiTransactionAdditionals.ToLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        FiTransactionAdditionals.Code = null;
                        //FiTransactionAdditionals.Address = null;
                        FiTransactionAdditionals.Rate = null;
                        FiTransactionAdditionals.SystemRate = null;
                        FiTransactionAdditionals.Period = null;
                        FiTransactionAdditionals.Days = null;
                        FiTransactionAdditionals.LCOptionID = null;
                        FiTransactionAdditionals.LCNo = null;
                        FiTransactionAdditionals.LCAmt = null;
                        FiTransactionAdditionals.AvailableLCAmt = null;
                        FiTransactionAdditionals.CreditAmt = null;
                        FiTransactionAdditionals.MarginAmt = null;
                        FiTransactionAdditionals.AvailableAmt = null;
                        FiTransactionAdditionals.AllocationPerc = null;
                        FiTransactionAdditionals.TolerencePerc = null;
                        FiTransactionAdditionals.CountryID = null;
                        FiTransactionAdditionals.CountryOfOriginID = null;
                        FiTransactionAdditionals.MaxDays = null;
                        FiTransactionAdditionals.BEMaxDays = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        FiTransactionAdditionals.DueDate = null;
                        FiTransactionAdditionals.OpenDate = null;
                        FiTransactionAdditionals.CloseDate = null;
                        FiTransactionAdditionals.StartDate = null;
                        FiTransactionAdditionals.EndDate = null;
                        FiTransactionAdditionals.ClearDate = null;
                        FiTransactionAdditionals.ReceiveDate = null;
                        FiTransactionAdditionals.EndTime = null;
                        FiTransactionAdditionals.HandOverTime = null;
                        FiTransactionAdditionals.LorryHireRate = null;
                        FiTransactionAdditionals.QtyPerLoad = null;
                        FiTransactionAdditionals.AuditNote = null;
                        //FiTransactionAdditionals.Terms = null;
                        FiTransactionAdditionals.FirmID = null;
                        FiTransactionAdditionals.VehicleID = null;
                        FiTransactionAdditionals.WeekDays = null;
                        FiTransactionAdditionals.BankWeekDays = null;
                        FiTransactionAdditionals.RecommendByID = null;
                        FiTransactionAdditionals.RecommendDate = null;
                        FiTransactionAdditionals.RecommendNote = null;
                        FiTransactionAdditionals.RecommendStatus = null;
                        FiTransactionAdditionals.IsHigherApproval = null;
                        FiTransactionAdditionals.LCApplnTransID = null;
                        FiTransactionAdditionals.InLocID = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AccountID = null;
                        FiTransactionAdditionals.RouteID = null;
                        FiTransactionAdditionals.AccountID2 = null;
                        FiTransactionAdditionals.Hours = null;
                        FiTransactionAdditionals.Year = null;
                        FiTransactionAdditionals.OtherBranchID = null;
                        FiTransactionAdditionals.TaxFormID = null;
                        FiTransactionAdditionals.PriceCategoryID = null;
                        FiTransactionAdditionals.IsClosed = null;
                        FiTransactionAdditionals.DepartmentID = null;
                        DALVouchers.InsertAdditionals(FiTransactionAdditionals);
                        return Json(new { success = true, message = "Transaction added", transactionNo = ID });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Unable to add Transaction....", transactionNo = "" });
                    }
                }
                else
                {
                    String Result = DALVouchers.UpdateTransactions(FiTransactions);
                    if (Result == "true")
                    {
                        foreach (Models.InvTransItems item in InvTransItems)
                        {
                            item.TransactionID = FiTransactions.ID;
                            item.Pcs = null;
                            item.AdvanceRate = null;
                            item.MasterMiscID1 = null;
                            item.Description = null;
                            item.Remarks = null;
                            item.IsBit = null;
                            item.InvAvgCostID = null;
                            item.IsReturn = null;
                            item.Additional = null;
                            item.CommodityID = null;
                            item.AccountID = null;
                            item.TransactionEntryID = null;
                            item.LengthFt = null;
                            item.LengthIn = null;
                            item.LengthCm = null;
                            item.GirthFt = null;
                            item.GirthIn = null;
                            item.GirthCm = null;
                            item.ThicknessFt = null;
                            item.ThicknessIn = null;
                            item.ThicknessCm = null;
                            item.ShortageQty = null;
                            item.AvgCostID = null;
                            item.RefTransItemID = null;
                            item.Status = null;
                            item.Cancel = null;
                            item.MeasuredByID = null;
                            item.FinishDate = null;
                            item.UpdateDate = null;
                            item.IsSameForPcs = null;
                            item.RefID = null;
                            item.BatchNo = null;
                            item.Margin = null;
                            item.SizeMasterID = null;
                            item.TranType = "Normal";
                            item.CostPerc = null;
                            item.ManufactureDate = null;
                            item.ExpiryDate = null;
                            item.FOCQty = null;
                            item.GroupItemID = null;
                            item.PriceCategoryID = null;
                            item.SerialNo = null;
                            item.ReplaceQty = null;
                            item.PrintedMRP = null;
                            item.PrintedRate = null;
                            item.PTSRate = null;
                            item.PTRRate = null;
                            item.TempRate = null;
                            item.StockItemID = null;
                            item.Visible = true;
                            item.RowType = -1;
                            item.InLocID = null;
                            DALVouchers.InvTransItemsMaster(item);
                        }
                        DALVouchers.UpdateTransactionEntries(FiTransactions.ID);

                        FiTransactionAdditionals.TransactionID = FiTransactions.ID;
                        FiTransactionAdditionals.RefTransID1 = null;
                        FiTransactionAdditionals.RefTransID2 = null;
                        FiTransactionAdditionals.MeasureTypeID = null;
                        FiTransactionAdditionals.LoadMeasureTypeID = null;
                        FiTransactionAdditionals.ConsignTermID = null;
                        FiTransactionAdditionals.ToLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        FiTransactionAdditionals.Code = null;
                        // FiTransactionAdditionals.Address = null;
                        FiTransactionAdditionals.Rate = null;
                        FiTransactionAdditionals.SystemRate = null;
                        FiTransactionAdditionals.Period = null;
                        FiTransactionAdditionals.Days = null;
                        FiTransactionAdditionals.LCOptionID = null;
                        FiTransactionAdditionals.LCNo = null;
                        FiTransactionAdditionals.LCAmt = null;
                        FiTransactionAdditionals.AvailableLCAmt = null;
                        FiTransactionAdditionals.CreditAmt = null;
                        FiTransactionAdditionals.MarginAmt = null;
                        FiTransactionAdditionals.AvailableAmt = null;
                        FiTransactionAdditionals.AllocationPerc = null;
                        FiTransactionAdditionals.TolerencePerc = null;
                        FiTransactionAdditionals.CountryID = null;
                        FiTransactionAdditionals.CountryOfOriginID = null;
                        FiTransactionAdditionals.MaxDays = null;
                        FiTransactionAdditionals.BEMaxDays = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        FiTransactionAdditionals.DueDate = null;
                        FiTransactionAdditionals.OpenDate = null;
                        FiTransactionAdditionals.CloseDate = null;
                        FiTransactionAdditionals.StartDate = null;
                        FiTransactionAdditionals.EndDate = null;
                        FiTransactionAdditionals.ClearDate = null;
                        FiTransactionAdditionals.ReceiveDate = null;
                        FiTransactionAdditionals.EndTime = null;
                        FiTransactionAdditionals.HandOverTime = null;
                        FiTransactionAdditionals.LorryHireRate = null;
                        FiTransactionAdditionals.QtyPerLoad = null;
                        FiTransactionAdditionals.AuditNote = null;
                        //FiTransactionAdditionals.Terms = null;
                        FiTransactionAdditionals.FirmID = null;
                        FiTransactionAdditionals.VehicleID = null;
                        FiTransactionAdditionals.WeekDays = null;
                        FiTransactionAdditionals.BankWeekDays = null;
                        FiTransactionAdditionals.RecommendByID = null;
                        FiTransactionAdditionals.RecommendDate = null;
                        FiTransactionAdditionals.RecommendNote = null;
                        FiTransactionAdditionals.RecommendStatus = null;
                        FiTransactionAdditionals.IsHigherApproval = null;
                        FiTransactionAdditionals.LCApplnTransID = null;
                        FiTransactionAdditionals.InLocID = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AccountID = null;
                        FiTransactionAdditionals.RouteID = null;
                        FiTransactionAdditionals.AccountID2 = null;
                        FiTransactionAdditionals.Hours = null;
                        FiTransactionAdditionals.Year = null;
                        FiTransactionAdditionals.OtherBranchID = null;
                        FiTransactionAdditionals.TaxFormID = null;
                        FiTransactionAdditionals.PriceCategoryID = null;
                        FiTransactionAdditionals.IsClosed = null;
                        FiTransactionAdditionals.DepartmentID = null;
                        DALVouchers.UpdateAdditionals(FiTransactionAdditionals);
                        return Json(new { success = true, message = "Transaction updated", transactionNo = FiTransactions.ID });
                    }
                    else
                    {
                        return Json(new { success = false, message = Result, transactionNo = "" });
                    }
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        //foreach (DataRow dr in foundItems.Rows) foreach (Datarow dr in foundItems.Rows)
        //{{
        //    int itemID = 0; int itemID = 0;
        //    if (dr["ID"] != DBNull.Value) if(dr["ID"] != DBNull.Value)
        //        itemID = Convert.ToInt32(dr["ID"]); iteID = Convert.ToInt32(dr["ID"]);

        //    string selectedUnit = ""; tring selectedUnit = "";
        //    decimal? factor = null; decimal? factor = null;
        //    string? Basicunit = ""; string? BasicUnit = "";
        //    DataSet details = DALVouchers.ItemAvailableUnitForExcel(itemID); DatatSet details = DALVoucher.ItemAvailableUnitForExcel(itemID);
        //    DataTable unitList = details.Tables[0]; DataTable unitList = details.Tables[0];
        //    //DataTable unitDetails = null;

        //    if (unitList != null && unitList.Rows.Count > 0)
        //    {
        //        DataRow unittable = unitList.Rows[0];
        //        string unitValue = unitList.Rows[0]["Unit"] == DBNull.Value
        //                           ? ""
        //                           : unitList.Rows[0]["Unit"].ToString().Trim();

        //        selectedUnit = string.IsNullOrWhiteSpace(unitValue) ? "No" : unitValue;
        //        Basicunit = "No";
        //        factor = Convert.ToDecimal(unittable["Factor"]);


        //    }
        //    else
        //    {
        //        selectedUnit = "No";  // No rows in unitList
        //        Basicunit = "No";  // No rows in unitList
        //        factor = 1m;
        //    }

        //    //if (!string.IsNullOrEmpty(selectedUnit))
        //    //    unitDetails = DALVouchers.UnitDetails(selectedUnit);

        //    //string itemUnitPrice = "";
        //    //if (itemID != 0)
        //    //    itemUnitPrice = DALVouchers.GetItemUnitPrice(itemID, 23, accountID, BranchID, selectedUnit);

        //    // Generate units dropdown
        //    StringBuilder unitOptions = new StringBuilder();
        //    if (unitList != null && unitList.Rows.Count > 0)
        //    {
        //        foreach (DataRow unit in unitList.Rows)
        //        {
        //            string unitVal = unit["Unit"] != DBNull.Value ? unit["Unit"].ToString() : "";
        //            unitOptions.Append("<option value='").Append(unitVal).Append("'");

        //            if (unitVal == selectedUnit)
        //                unitOptions.Append(" selected");

        //            unitOptions.Append(">").Append(unitVal).Append("</option>");
        //        }
        //    }

        //    finalHTML.Append("<tr>");
        //    finalHTML.Append("<td class='serial-no'>" + Sn + "</td>");

        //    // 1. Product Code with lookup support
        //    string itemName = dr["ItemName"] != DBNull.Value ? dr["ItemName"].ToString() : "";
        //    string idValue = dr["ID"] != DBNull.Value ? dr["ID"].ToString() : "";

        //    finalHTML.Append("<td id='TdproductCode" + Sn + "' >");
        //    finalHTML.Append("<input type='text' id='productCode" + Sn + "' style='width: 5cm;' class='form-control productCode' element-id='" + Sn + "' ");
        //    finalHTML.Append("value='" + itemName + "' ");
        //    finalHTML.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
        //    finalHTML.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
        //    finalHTML.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + idValue + "' ");
        //    finalHTML.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
        //    finalHTML.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
        //    finalHTML.Append("</td>");

        //    // 2. Unit dropdown
        //    finalHTML.Append("<td id='unitTd" + Sn + "' >");
        //    finalHTML.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;' class='form-select ItemUnit excelCells'>" + unitOptions.ToString() + "</select>");
        //    finalHTML.Append("</td>");

        //    // 3. Quantity
        //    int qtyVal = 0;
        //    if (dr["Qty"] != DBNull.Value)
        //        int.TryParse(dr["Qty"].ToString(), out qtyVal);

        //    finalHTML.Append("<td id='qtyTd" + Sn + "' >");
        //    finalHTML.Append("<input type='text' class='form-control ItemQty' style='width: 2cm;' element-id='" + Sn + "' id='ItemQty" + Sn + "' value='" + qtyVal + "'/>");
        //    finalHTML.Append("</td>");

        //    // 4. Rate
        //    decimal costVal = 0;
        //    if (dr["Cost"] != DBNull.Value)
        //        decimal.TryParse(dr["Cost"].ToString(), out costVal);

        //    finalHTML.Append("<td id='rateTd" + Sn + "' >");
        //    //finalHTML.Append("<input type='text' class='form-control ItemRate excelCells' element-factor='" + String.Format("{0:N2}", dr["Factor"]) + "' style='width: 2cm;' element-id='" + Sn + "' value='" + String.Format("{0:N2}", dr["Rate"]) + "' id='ItemRate" + Sn + "' disabled/></td>");

        //    finalHTML.Append("<input type='text' class='form-control ItemRate excelCells' element-factor='"+factor+"' style='width: 2cm;' element-id='" + Sn + "' id='ItemRate" + Sn + "' value='" + costVal + "' disabled />");
        //    finalHTML.Append("</td>");

        //    // 5. Gross Amount
        //    finalHTML.Append("<td class='ItemGrossAmtTd" + Sn + "'>");
        //    finalHTML.Append("<input type='text' class='form-control ItemGrossAmt excelCells' style='width: 2cm;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' disabled/>");
        //    finalHTML.Append("</td>");

        //    // 6. Discount %
        //    finalHTML.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
        //    finalHTML.Append("<input type='text' class='form-control ItemDiscPer excelCells' style='width: 2cm;' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' />");
        //    finalHTML.Append("</td>");

        //    // 7. Discount Amount
        //    finalHTML.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
        //    finalHTML.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style='width: 2cm;' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' />");
        //    finalHTML.Append("</td>");

        //    // 8. Amount
        //    finalHTML.Append("<td class='amtTd' id='amtTd" + Sn + "' >");
        //    finalHTML.Append("<input type='text' class='form-control ItemAmt excelCells' style='width: 2cm;' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/>");
        //    finalHTML.Append("</td>");
        //    finalHTML.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
        //    int taxTypeID = (dr["TaxTypeID"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["TaxTypeID"].ToString()))
        //        ? Convert.ToInt32(dr["TaxTypeID"])
        //        : 0;

        //    decimal salesPerc = (dr["SalesPerc"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["SalesPerc"].ToString()))
        //        ? Convert.ToDecimal(dr["SalesPerc"])
        //        : 0.00m;

        //    finalHTML.Append(
        //        "<input type='text' class='form-control ItemTaxPer excelCells' " +
        //        "taxTypeID='" + taxTypeID + "' " +
        //        "style='width: 2cm;text-align: right;' " +
        //        "element-id='" + Sn + "' " +
        //        "id='ItemTaxPer" + Sn + "' " +
        //        "value='" + String.Format("{0:N2}", salesPerc) + "' />"
        //    );
        //    finalHTML.Append("</td>");
        //    decimal TaxValue = (dr["TaxValue"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["TaxValue"].ToString()))
        //       ? Convert.ToDecimal(dr["TaxValue"])
        //       : 0.00m;
        //    // 10. Tax Amount
        //    finalHTML.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
        //    finalHTML.Append("<input type='text' class='form-control ItemTaxAmt excelCells' style='width: 2cm;' value='" + String.Format("{0:N2}", TaxValue) + "' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' />");
        //    finalHTML.Append("</td>");

        //    // 11. Total
        //    finalHTML.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' style='width: 2cm;'>");
        //    finalHTML.Append("<input type='text' class='form-control ItemTotal excelCells' style='width: 2cm;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled />");
        //    finalHTML.Append("</td>");

        //    // 12. Add row (+)
        //    finalHTML.Append("<td class='col'><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "'><i class='fa-solid fa-plus'></i></button></td>");

        //    // 13. Delete action
        //    finalHTML.Append("<td class='col' id='deleteaction" + Sn + "'>");
        //    finalHTML.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
        //    finalHTML.Append("</td>");

        //    // 14. Hidden ItemID
        //    finalHTML.Append("<td>");
        //    finalHTML.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
        //    finalHTML.Append("</td>");

        //    finalHTML.Append("</tr>");

        //    Sn++;
        //}

    }
}
