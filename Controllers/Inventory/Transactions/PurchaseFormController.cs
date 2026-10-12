using DocumentFormat.OpenXml.Spreadsheet;
using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using static ERPSample.Controllers.BaseController;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class PurchaseFormController : BaseController
    {
        private Lazy<DAL.Inventory.Transactions.Purchase> _DALPurchase;
        private Lazy<DAL.Inventory.Masters.FiMaVouchers> _DALFiMaVouchers;
        private Lazy<DAL.General.Masters.Parties> _DALParties;
        private Lazy<DAL.General.Transactions.FiTransactions> _DALTransactions;
        private Lazy<DAL.General.Masters.Locations> _DALLocations;
        private Lazy<DAL.General.Common.Menu> _DALMenu;
        private Lazy<DAL.General.Common.Vouchers> _DALVouchers;
        private Lazy<DataRow> _MenuRow;
        private Lazy<DataRow> _VoucherTypeRow;

        private DAL.Inventory.Transactions.Purchase DALPurchase => _DALPurchase.Value;
        private DAL.Inventory.Masters.FiMaVouchers DALFiMaVouchers => _DALFiMaVouchers.Value;
        private DAL.General.Masters.Parties DALParties => _DALParties.Value;
        private DAL.General.Transactions.FiTransactions DALTransactions => _DALTransactions.Value;
        private DAL.General.Masters.Locations DALLocations => _DALLocations.Value;
        private DAL.General.Common.Menu DALMenu => _DALMenu.Value;
        private DAL.General.Common.Vouchers DALVouchers => _DALVouchers.Value;
        private DataRow MenuRow => _MenuRow.Value;
        private DataRow VoucherTypeRow => _VoucherTypeRow.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (_DALPurchase != null) return; // skip if already initialized

            var conn = ConnectionString;
            _DALPurchase = new Lazy<DAL.Inventory.Transactions.Purchase>(() => new(conn));
            _DALFiMaVouchers = new Lazy<DAL.Inventory.Masters.FiMaVouchers>(() => new(conn));
            _DALParties = new Lazy<DAL.General.Masters.Parties>(() => new(conn));
            _DALTransactions = new Lazy<DAL.General.Transactions.FiTransactions>(() => new(conn));
            _DALLocations = new Lazy<DAL.General.Masters.Locations>(() => new(conn));
            _DALMenu = new Lazy<DAL.General.Common.Menu>(() => new(conn));
            _DALVouchers = new Lazy<DAL.General.Common.Vouchers>(() => new(conn));
            _MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(264).Rows[0]);
            _VoucherTypeRow = new Lazy<DataRow>(() => DALVouchers.FillVoucherRow(264, MenuRow["ID"]));
        }
        public async Task<IActionResult> Index(long MenuID)
        {
            SetUserPermissions(MenuID);
            //ViewBag.voucher = VoucherTypeRow;
            //ViewBag.DataTable = DALVouchers.FillVoucher(BranchID, MenuRow["ID"]);
            DataSet ds = DALVouchers.FillVoucher(BranchID, MenuRow["ID"]);
            DataTable dt = ds.Tables[0];
            DataTable dt2 = ds.Tables[1];
            DataRow dr2 = dt2.Rows[0];
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                // Adding odd/even class based on the row count
                string rowClass = count % 2 == 0 ? "even" : "odd";

                sb.Append("<tr class='" + rowClass + "'>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["TransactionNo"].ToString() + "</td>");
                sb.Append("<td>" + Convert.ToDateTime(dr["Date"]).ToString("dd/MM/yyyy") + "</td>");
                sb.Append("<td>" + dr["AccountName"].ToString() + "</td>");
                sb.Append("<td>" + dr["Amount"].ToString() + "</td>");
                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"].ToString() + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("</tr>");
                count++;
            }
            string Purchase = sb.ToString();
            ViewBag.voucher = VoucherTypeRow;
            ViewBag.MenuID = MenuID;
            ViewBag.DataTable = Purchase;
            ViewBag.RowType = dr2["RowType"].ToString();
            ViewBag.VoucherCode = dr2["Code"].ToString();
            ViewBag.VoucherID = dr2["VoucherID"].ToString();
            return View("~/Views/Invertory/Transactions/PurchaseForm.cshtml");
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
                request.FiTransactions.PageID = (int)PageIDs.PurchaseForm;
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
            catch (Exception ex)
            {
                // Actually handle it like your other action does
                return Json(new { success = false, message = ex.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryTransaction(int ID)
        {
            try
            {
                DataTable Transaction = DALVouchers.DataTableFillTransactions(ID);
                DataTable Items = DALVouchers.DataTableFillTransactionEntries(ID);
                DataTable Additional = DALVouchers.DataTableFillTransactionAdditionals(ID);
                DataTable Entries = DALVouchers.FillTransactionEntries(ID);

                StringBuilder sb = new StringBuilder();
                string paymentmode = "";
                string SalesArea = "";
                string ListWarehouse = "";

                if (Additional.Rows.Count > 0)
                {
                    DataRow locs = Additional.Rows[0];
                    DataTable warehouses = DALVouchers.FillLocationusingBranch(BranchID);
                    sb.Append("<option value=''> -- Choose Warehouse -- </option>");
                    foreach (DataRow dr in warehouses.Rows)
                    {
                        string selected = dr["ID"].ToString() == locs["InLocID"].ToString() ? " selected" : "";
                        sb.Append($"<option value='{dr["ID"]}'{selected}>{dr["Name"]}</option>");
                    }
                    ListWarehouse = sb.ToString();
                    sb.Clear();

                    DataTable mode = DALVouchers.GetMode();
                    sb.Append("<option value=''> -- Choose Payment Type -- </option>");
                    foreach (DataRow dr in mode.Rows)
                    {
                        string selected = dr["ID"].ToString() == locs["ModeID"].ToString() ? " selected" : "";
                        sb.Append($"<option value='{dr["ID"]}'{selected}>{dr["Value"]}</option>");
                    }
                    paymentmode = sb.ToString();
                    sb.Clear();
                }
                //=============To get the default account ================//
                //DataSet ds = DALVouchers.GetAccountIDPurchase();
                //if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //{
                //    ViewBag.Account = ds.Tables[0].Rows[0]["AccountName"]?.ToString() ?? "";
                //}
                //else
                //{
                //    ViewBag.Account = "";
                //}
                int Sn = 0;
                int No = 1;


                foreach (DataRow dr in Items.Rows)
                {
                    DataSet Details = DALVouchers.ProductAvailableUnits(Convert.ToInt64(dr["ItemID"]));
                    DataTable dataTable = Details.Tables[0];
                    DataTable DtDetails = Details.Tables[1];
                    Sn = Convert.ToInt32(dr["ID"]);

                    sb.Append("<tr>");
                    sb.Append("<td class='serial-no'>" + No + "</td>");
                    //Product Image
                    sb.Append(" <td>");
                    sb.Append(" <img src='" + dr["Image"].ToString() + "' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
                    sb.Append(" </td>");
                    // 1. Product Code
                    sb.Append("<td id='TdproductCode" + Sn + "' >");
                    sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 7cm;'  class='form-control productCode excelCells changedValue' element-id='" + Sn + "' ");
                    sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
                    sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
                    sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + dr["ItemID"] + "' ");
                    sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' ");
                    sb.Append("value='" + dr["ItemName"] + "' />");
                    sb.Append("<div id='lookupDIVproductCode" + Sn + "'></div>");
                    sb.Append("</td>");

                    // 2. Unit
                    sb.Append("<td id='unitTd" + Sn + "'>");
                    sb.Append("<select name='ItemUnit" + Sn + "' style='width: 3cm;' element-id='" + Sn + "' id='ItemUnit" + Sn + "' class='form-select ItemUnit excelCells'>");
                    foreach (DataRow dr1 in dataTable.Rows)
                    {
                        sb.Append("<option value='" + dr1["Unit"] + "'" + (dr["Unit"].ToString() == dr1["Unit"].ToString() ? " selected" : "") + ">" + dr1["Unit"] + "</option>");
                    }
                    sb.Append("</select></td>");

                    // 3. Qty
                    sb.Append("<td id='qtyTd" + Sn + "'>");
                    sb.Append("<input type='text' style=' text-align:center;' class='form-control ItemQty excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Qty"]), 2) + "' element-id='" + Sn + "' id='ItemQty" + Sn + "' /></td>");

                    // 4. Rate
                    sb.Append("<td id='rateTd" + Sn + "' >");
                    sb.Append("<input type='text' style=' text-align:right;' class='form-control ItemRate excelCells' element-factor='" + ToFixedNoRound(Convert.ToDecimal(dr["Factor"]), 2) + "' element-id='" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Rate"]), 2) + "' id='ItemRate" + Sn + "' disabled/></td>");

                    // 5. Gross Amount
                    sb.Append("<td class='ItemGrossAmtTd' >");
                    sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells'style=' text-align:right;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["GrossAmount"]), 2) + "' disabled/></td>");

                    // 6. Discount %
                    sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' style=' text-align:center;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["DiscountPerc"]), 2) + "' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' /></td>");

                    // 7. Discount Amt
                    sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style=' text-align:right;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Discount"]), 2) + "' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' /></td>");

                    // 8. Amount
                    sb.Append("<td class='amtTd' id='amtTd" + Sn + "'>");
                    sb.Append("<input type='text' class='form-control ItemAmt excelCells' style=' text-align:right;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Amount"]), 2) + "' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/></td>");

                    // 9. Tax %
                    sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
                    if (dr["TaxTypeID"].ToString() != "")
                    {
                        object taxTypeValue = DtDetails.Rows[0]["TaxTypeID"];

                        if (taxTypeValue != DBNull.Value && taxTypeValue != null && !string.IsNullOrWhiteSpace(taxTypeValue.ToString()) && taxTypeValue.ToString() != "0")
                        {
                            DataTable TaxDetails = DALVouchers.ProductTaxDetails(Convert.ToInt64(taxTypeValue));
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style=' text-align:center;'  value='" + String.Format("{0:F2}", TaxDetails.Rows[0]["SalesPerc"]) + "' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' taxaccountid='" + TaxDetails.Rows[0]["TaxAccountID"] + "' />");
                        }
                        else
                        {
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style=' text-align: center;'  value='' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");
                        }
                    }
                    else
                    {
                        sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' style=' text-align: center;' />");
                    }
                    sb.Append("</td>");

                    // 10. Tax Amount
                    sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TaxValue"]), 2) + "' style=' text-align:right;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' /></td>");

                    // 11. Total
                    sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTotal excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TotalAmount"]), 2) + "' style=' text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled/></td>");

                    // 12. Add Row Button
                    sb.Append("<td class='col'><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "'><i class='fa-solid fa-plus'></i></button></td>");

                    // 13. Delete Action
                    sb.Append("<td class='col' id='deleteaction" + Sn + "' >");
                    sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");

                    // 14. Hidden Item ID
                    sb.Append("<td style='display:none;'>");
                    sb.Append("<input type='hidden' class='itemid excelCells numbersOnly form-control' id='itemid" + Sn + "' value='" + Sn + "' element-id='" + Sn + "' autocomplete='off'></td>");

                    sb.Append("</tr>");
                    No++;

                }
                string Entities = sb.ToString();
                sb.Clear();

                decimal TotalPaid = 0;
                decimal cash = 0;
                decimal card = 0;
                decimal cheque = 0;
                int EntryID = 0;
                DateTime DueDate = DateTime.MinValue;
                foreach (DataRow dr in Entries.Rows)
                {
                    string tranType = Convert.ToString(dr["TranType"]);

                    decimal amount = dr["Amount"] != DBNull.Value
                        ? Convert.ToDecimal(dr["Amount"])
                        : 0;

                    if (tranType == "Party" || tranType == "Normal")
                    {
                        TotalPaid = amount;
                        DueDate = dr["DueDate"] != DBNull.Value
                            ? Convert.ToDateTime(dr["DueDate"])
                            : DateTime.MinValue;
                        EntryID = dr["ID"] != DBNull.Value
                            ? Convert.ToInt32(dr["ID"])
                            : 0;
                    }
                    else if (tranType == "Cash")
                    {
                        cash = amount;
                    }
                    else if (tranType == "Card")
                    {
                        card = amount;
                    }
                    else if (tranType == "Cheque")
                    {
                        cheque = amount;
                    }
                }

                // Serialize once each — no duplicate loop
                string Trans = SerializeDataTable(Transaction);
                string Additionalentries = SerializeDataTable(Additional);

                //warehouses = ListWarehouses,
                return Json(new
                {
                    success = true,
                    innerHTML = Entities,
                    trans = Trans,
                    fiadditional = Additionalentries,
                    additional = Trans,
                    account = "",
                    warehouses = ListWarehouse,
                    message = "Success",
                    mode = paymentmode,
                    card = card,
                    cash = cash,
                    cheque = cheque,
                    totalpaid = TotalPaid,
                    entryid = EntryID,
                    duedate = DueDate

                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper method to avoid repeating this pattern
        private string SerializeDataTable(DataTable table)
        {
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in table.Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                    row[col.ColumnName] = dr[col];
                rows.Add(row);
            }
            return JsonConvert.SerializeObject(rows);
        }

        public static string ToFixedNoRound(decimal value, int decimals)
        {
            decimal factor = (decimal)Math.Pow(10, decimals);
            decimal truncated = Math.Truncate(value * factor) / factor;
            return truncated.ToString($"F{decimals}");
        }
    }
}
