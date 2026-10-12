using DocumentFormat.OpenXml.Spreadsheet;
using ERPSample.DAL.Accounting.Transactions;
using ERPSample.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Web;

namespace ERPSample.Controllers.Accounting.Transactions
{
    public class ReceiptVoucherController : BaseController
    {
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
                    _MenuRow = DALMenu.LoadWindowsForm(70).Rows[0];

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
                    _VoucherTypeRow = DALVouchers.FillVoucherRow(70, MenuRow["ID"]);

                }
                return _VoucherTypeRow;
            }
        }

        private DAL.Accounting.Transactions.ReceiptVoucher _ReceiptVoucher;
        private DAL.Accounting.Transactions.ReceiptVoucher ReceiptVoucher
        {
            get
            {
                if (_ReceiptVoucher == null)
                {
                    _ReceiptVoucher = new DAL.Accounting.Transactions.ReceiptVoucher(ConnectionString);
                }
                return _ReceiptVoucher;
            }
        }
        public async Task<IActionResult> Index(int? MenuID)
        {
            SetUserPermissions(68);
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
                sb.Append("<tr>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["TransactionNo"].ToString() + "</td>");
                sb.Append("<td>" + Convert.ToDateTime(dr["Date"]).ToString("dd/MM/yyyy") + "</td>");
                //if (dr["Cancelled"].ToString().ToLower() == "true")
                //{
                //    sb.Append("<td>Cancelled</td>");
                //}
                //else
                //{
                //    sb.Append("<td></td>"); // Empty <td> if Cancelled is false
                //}
                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"].ToString() + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("</tr>");
                count++;
            }
            string ReceiptVoucher = sb.ToString();
            ViewBag.voucher = VoucherTypeRow;
            ViewBag.DataTable = ReceiptVoucher;
            ViewBag.MenuID = MenuID;
            ViewBag.RowType = dr2["RowType"].ToString();
            ViewBag.VoucherCode = dr2["Code"].ToString();
            ViewBag.VoucherID = dr2["VoucherID"].ToString();
            return View("~/Views/Accounting/Transactions/ReceiptVoucher.cshtml");

        }

        [HttpPost]
        public async Task<IActionResult> NewEntryDetails()
        {
            StringBuilder sb = new StringBuilder();
            int Sn = 1;

            sb.Append("<tr id='Row" + Sn + "' element-id='"+Sn+"'>");

            // Serial No
            sb.Append("<td class='serial-no text-center align-middle' style='width:40px;'>" + Sn + "</td>");

            // Account Name
            sb.Append("<td class='p-1' style='width:10cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm AccountCode excelCells' id='AccountCode" + Sn + "' name='AccountCode" + Sn + "' ");
            sb.Append(" onkeydown=\"ShowLookup(event,'AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Description
            sb.Append("<td class='p-1' id='TdAccountDescription" + Sn + "' style='width:20cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='AccountDescription excelCells form-control form-control-sm' type='text' id='AccountDescription" + Sn + "'  value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Due Date
            sb.Append("<td class='p-1' id='TdDueDate" + Sn + "' style='width:3cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' oninput='formatDateInput(this)' class='DueDate form-control form-control-sm excelCells' type='text' id='DueDate" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Credit
            sb.Append("<td class='p-1' id='TdCredit" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Credit form-control form-control-sm' id='Credit" + Sn + "'  value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrow' element-id='" + Sn + "' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='deleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            // Hidden itemid
            sb.Append("<td style='display:none;'>");
            sb.Append("<input type='hidden' class='itemid excelCells form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");

            sb.Append("</tr>");
            string accountdetails = sb.ToString();
            sb.Clear();


            return Json(new { success = true, accountdetails = accountdetails });
        }

        public async Task<IActionResult> GetCashDetails(int AccountID)
        {
            decimal balance = ReceiptVoucher.GetAccountBalance(AccountID, BranchID);
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
            return Json(new { cashTable = htmlString, balance = balance });
        }

        public async Task<IActionResult> GetCardDetails(int AccountID)
        {
            decimal balance = ReceiptVoucher.GetAccountBalance(AccountID, BranchID);
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
            return Json(new { cardTable = htmlString, balance = balance });
        }

        public async Task<IActionResult> GetChequeDetails(int AccountID)
        {
            decimal balance = ReceiptVoucher.GetAccountBalance(AccountID, BranchID);
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
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCheque' serialno='"+Sn+"' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
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
            return Json(new { chequeTable = htmlString, balance = balance });
        }

        [HttpGet]
        public async Task<IActionResult> NewRowAccount(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;

            sb.Append("<tr id='Row" + Sn + "' element-id='"+Sn+ "'>");

            // Serial No
            sb.Append("<td class='serial-no text-center align-middle' style='width:40px;'>" + Sn + "</td>");

            // Account Name — widest column
            sb.Append("<td class='p-1' style='width:10cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm AccountCode excelCells' id='AccountCode" + Sn + "' name='AccountCode" + Sn + "'");
            sb.Append(" onkeydown=\"ShowLookup(event,'AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Hidden AccountName (kept for compatibility)
            sb.Append("<td id='TdAccountName" + Sn + "' style='display:none;'>");
            sb.Append("<p class='excelCellText' id='AccountName" + Sn + "' element-id='" + Sn + "'></p>");
            sb.Append("</td>");

            // Description — second widest
            sb.Append("<td class='p-1' id='TdAccountDescription" + Sn + "' style='width:20cm; '>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='AccountDescription excelCells form-control form-control-sm' type='text' id='AccountDescription" + Sn + "' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Due Date — narrow
            sb.Append("<td class='p-1' id='TdDueDate" + Sn + "' style='width:3cm;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' oninput='formatDateInput(this)' class='DueDate form-control form-control-sm excelCells' type='text' id='DueDate" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Credit
            sb.Append("<td class='p-1' id='TdCredit" + Sn + "' style='width:3cm;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Credit form-control form-control-sm' id='Credit" + Sn + "'  value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Add button
            sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrow' element-id='" + Sn + "' serialno='" + Sn + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
            sb.Append("</td>");

            // Delete button
            sb.Append("<td class='p-1 text-center align-middle' id='deleteaction" + Sn + "' style='width:50px;'>");
            sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
            sb.Append("</li></ul>");
            sb.Append("</td>");

            // Hidden itemid
            sb.Append("<td style='display:none;'>");
            sb.Append("<input type='hidden' class='itemid excelCells form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");

            sb.Append("</tr>");
            string accountdetails = sb.ToString();
            sb.Clear();

            return Json(new { success = true, newrow = accountdetails });
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
            sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCheque' serialno='"+Sn+"' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
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
                request.FiTransactions.IsPostDated = false;
                request.FiTransactions.CompanyID = (int)BranchID;
                request.FiTransactions.StatusID = 806;
                request.FiTransactions.IsAutoEntry = false;
                request.FiTransactions.Active = true;
                request.FiTransactions.Cancelled = false;
                request.FiTransactions.Posted = true;
                request.FiTransactions.PageID = (int)PageIDs.ReceiptVoucher;
                request.FiTransactions.ApprovalStatus = 'A';
                request.FiTransactions.CurrencyID = 1;

                if (request.FiTransactionAdditionals != null)
                {
                    request.FiTransactionAdditionals.TransactionID = request.FiTransactions.ID;
                    if (request.FiTransactionAdditionals.RowState == null || request.FiTransactionAdditionals.RowState == 0)
                    {
                        request.FiTransactionAdditionals.RowState =
                            (request.FiTransactions.ID == null || request.FiTransactions.ID == 0) ? 1 : 2;
                    }
                }

                //foreach (var item in request.FiTransactionEntries)
                //{
                //    item.CurrencyID = 17;
                //}

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

        [HttpGet]
        public async Task<IActionResult> getVoucherReceiptEntries(int ID)
        {
            try
            {
                DataTable Transaction = new DataTable();
                Transaction = ReceiptVoucher.FillTransactions(ID);

                DataTable Additional = new DataTable();
                Additional = ReceiptVoucher.FillTransactionAdditonals(ID);

                DataSet entriesDs = ReceiptVoucher.FillTransactionEntries(ID);
                DataTable Entries = entriesDs.Tables["TransactionEntries"];
                DataTable Cheques = entriesDs.Tables["Cheques"];

                StringBuilder sbAccount = new StringBuilder();
                StringBuilder sbCash = new StringBuilder();
                StringBuilder sbCard = new StringBuilder();
                StringBuilder sbCheque = new StringBuilder();
                StringBuilder sbPayment = new StringBuilder();

                int accountNo = 1;
                int PaccountNo = 1;

                foreach (DataRow dr in Entries.Rows)
                {
                    string id = dr["ID"].ToString();
                    string drCr = dr["DrCr"].ToString().Trim();
                    string tranType = dr["TranType"] == DBNull.Value ? "" : dr["TranType"].ToString().Trim();

                    if (drCr == "C")
                    {
                        sbAccount.Append("<tr id='Row" + accountNo + "' element-id='"+ accountNo + "'>");
                        sbAccount.Append("<td class='serial-no text-center align-middle' style='width:40px;'>" + accountNo + "</td>");

                        // Account Name
                        sbAccount.Append("<td class='p-1' style='width:10cm;'>");
                        sbAccount.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm AccountCode excelCells'");
                        sbAccount.Append(" id='AccountCode" + accountNo + "' name='AccountCode" + accountNo + "'");
                        sbAccount.Append(" value='" + dr["Name"].ToString().Replace("'", "&#39;") + "'");
                        sbAccount.Append(" onkeydown=\"ShowLookup(event,'AccountCode" + accountNo + "','lookupDIVAccountCode" + accountNo + "')\"");
                        sbAccount.Append(" oninput=\"LookupTextChanged('AccountCode" + accountNo + "','lookupDIVAccountCode" + accountNo + "')\"");
                        sbAccount.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='" + dr["AccountID"] + "'");
                        sbAccount.Append(" element-id='" + accountNo + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                        sbAccount.Append(" data-intparam1='" + id + "' data-intparam2='' data-intparam3=''>");
                        sbAccount.Append("<div id='lookupDIVAccountCode" + accountNo + "'></div>");
                        sbAccount.Append("</td>");

                        // Hidden AccountName
                        sbAccount.Append("<td id='TdAccountName" + accountNo + "' style='display:none;'>");
                        sbAccount.Append("<p class='excelCellText AccountName' id='AccountName" + accountNo + "' element-id='" + accountNo + "'>" + dr["Name"] + "</p>");
                        sbAccount.Append("</td>");

                        // Description
                        sbAccount.Append("<td class='p-1' id='TdAccountDescription" + accountNo + "' style='width:20cm;'>");
                        sbAccount.Append("<input style='width:100%; border-radius:6px;' class='AccountDescription excelCells form-control form-control-sm' type='text'");
                        sbAccount.Append(" id='AccountDescription" + accountNo + "' value='" + dr["Description"].ToString().Replace("'", "&#39;") + "' element-id='" + accountNo + "'>");
                        sbAccount.Append("</td>");

                        // Due Date
                        sbAccount.Append("<td class='p-1' id='TdDueDate" + accountNo + "' style='width:3cm;'>");
                        sbAccount.Append("<input style='width:100%; border-radius:6px;' oninput='formatDateInput(this)' class='DueDate form-control form-control-sm excelCells' type='text'");
                        sbAccount.Append(" id='DueDate" + accountNo + "' element-id='" + accountNo + "'");
                        sbAccount.Append(" value='");
                        //if (dr["DueDate"] != DBNull.Value)
                        //    sbAccount.Append(dr["DueDate"].ToString());
                        if (dr["DueDate"] != DBNull.Value && dr["DueDate"] != null)
                            sbAccount.Append(String.Format("{0:dd-MM-yyyy}", dr["DueDate"]));
                        sbAccount.Append("'>");
                        sbAccount.Append("</td>");

                        // Credit
                        sbAccount.Append("<td class='p-1' id='TdCredit" + accountNo + "' style='width:3cm;'>");
                        sbAccount.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Credit form-control form-control-sm'");
                        sbAccount.Append(" id='Credit" + accountNo + "' value='" + dr["Credit"] + "' element-id='" + accountNo + "'>");
                        sbAccount.Append("</td>");

                        // Add button
                        sbAccount.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
                        sbAccount.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrow' element-id='" + accountNo + "' serialno='" + accountNo + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
                        sbAccount.Append("</td>");

                        // Delete button
                        sbAccount.Append("<td class='p-1 text-center align-middle' id='deleteaction" + id + "' style='width:50px;'>");
                        sbAccount.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete action_delete' id='deleteunit" + id + "' element-accountNo='" + id + "'>");
                        sbAccount.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
                        sbAccount.Append("</li></ul>");
                        sbAccount.Append("</td>");

                        // Hidden itemid
                        sbAccount.Append("<td style='display:none;'>");
                        sbAccount.Append("<input type='hidden' class='itemid excelCells form-control' id='itemid" + accountNo + "' value='" + id + "' element-id='" + accountNo + "' autocomplete='off'>");
                        sbAccount.Append("</td>");

                        sbAccount.Append("</tr>");
                        accountNo++;
                    }
                    else if (drCr == "D")
                    {
                        // Payment Details
                        sbPayment.Append("<tr class='PayRow' id='PayRow" + PaccountNo + "' element-id='" + PaccountNo + "'>");
                        sbPayment.Append("<td class='p-1 PaymentAccount' style='width:15cm;' id='PaymentAccount" + PaccountNo + "' accountid='" + dr["AccountID"] + "'>");
                        sbPayment.Append(dr["Name"].ToString().Replace("'", "&#39;"));
                        sbPayment.Append("</td>");

                        sbPayment.Append("<td class='p-1' style='width:20cm; ' id='PAccountDescription"+ PaccountNo +"'>");
                        sbPayment.Append(dr["Description"].ToString().Replace("'", "&#39;"));
                        sbPayment.Append("</td>");

                        sbPayment.Append("<td class='p-1' style='text-align:right; width:5cm; ' id='DebitAmount" + PaccountNo + "'>");
                        sbPayment.Append(dr["Amount"].ToString());
                        sbPayment.Append("</td>");

                        sbPayment.Append("<td class='p-1' style='width:5cm;' id='TranType"+PaccountNo+"'>");
                        sbPayment.Append(tranType);
                        sbPayment.Append("</td>");
                        sbPayment.Append("<td style='display:none;' Pitemid='"+ id + "'>");
                        sbPayment.Append("</td>");
                        sbPayment.Append("</tr>");
                        PaccountNo++;

                        // Cash Modal
                        if (tranType == "Cash")
                        {
                            sbCash.Append("<tr id='CashRow" + id + "' dataentry-id='"+id+"'>");
                            sbCash.Append("<td class='p-1' style='width:5cm; '>");
                            sbCash.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm CashAccountCode excelCells'");
                            sbCash.Append(" id='CashAccountCode" + id + "' name='CashAccountCode" + id + "'");
                            sbCash.Append(" onkeydown=\"ShowLookup(event,'CashAccountCode" + id + "','lookupDIVCashAccountCode" + id + "')\"");
                            sbCash.Append(" oninput=\"LookupTextChanged('CashAccountCode" + id + "','lookupDIVCashAccountCode" + id + "')\"");
                            sbCash.Append(" data-lookupcriteria='CashAccount' data-idcolumn='ID' data-idvalue='" + dr["AccountID"] + "'");
                            sbCash.Append(" element-id='" + id + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                            sbCash.Append(" data-intparam1='" + id + "' data-intparam2='' data-intparam3=''");
                            sbCash.Append(" value='" + dr["Name"].ToString().Replace("'", "&#39;") + "'>");
                            sbCash.Append("<div id='lookupDIVCashAccountCode" + id + "'></div>");
                            sbCash.Append("</td>");
                            sbCash.Append("<td class='p-1' id='TdCashAccountDescription" + id + "' style='width:10cm;'>");
                            sbCash.Append("<input style='width:100%; border-radius:6px;' class='CashAccountDescription excelCells form-control form-control-sm' type='text'");
                            sbCash.Append(" id='CashAccountDescription" + id + "' value='" + dr["Description"].ToString().Replace("'", "&#39;") + "' element-id='" + id + "'>");
                            sbCash.Append("</td>");
                            sbCash.Append("<td class='p-1' id='TdCashAmount" + id + "' style='width:3cm;'>");
                            sbCash.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells CashAmount form-control form-control-sm'");
                            sbCash.Append(" id='CashAmount" + id + "' value='" + dr["Amount"] + "' element-id='" + id + "'>");
                            sbCash.Append("</td>");
                            sbCash.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
                            sbCash.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCash' serialno='" + id + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
                            sbCash.Append("</td>");
                            sbCash.Append("<td class='p-1 text-center align-middle' id='Cashdeleteaction" + id + "' style='width:50px;'>");
                            sbCash.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCash' id='Cashdeleteunit" + id + "' element-id='" + id + "'>");
                            sbCash.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
                            sbCash.Append("</li></ul>");
                            sbCash.Append("</td>");
                            //sbCash.Append("<td style='display:none;'>");
                            //sbCash.Append("<input type='hidden' class='cashitemid excelCells form-control' id='cashitemid" + id + "' value='" + id + "' element-id='" + id + "'>");
                            //sbCash.Append("</td>");
                            sbCash.Append("</tr>");
                        }

                        // Card Modal
                        else if (tranType == "Card")
                        {
                            sbCard.Append("<tr id='CardRow" + id + "' dataentry-id='"+id+"'>");
                            sbCard.Append("<td class='p-1' style='width:5cm; '>");
                            sbCard.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm CardAccountCode excelCells'");
                            sbCard.Append(" id='CardAccountCode" + id + "' name='CardAccountCode" + id + "'");
                            sbCard.Append(" onkeydown=\"ShowLookup(event,'CardAccountCode" + id + "','lookupDIVCardAccountCode" + id + "')\"");
                            sbCard.Append(" oninput=\"LookupTextChanged('CardAccountCode" + id + "','lookupDIVCardAccountCode" + id + "')\"");
                            sbCard.Append(" data-lookupcriteria='CardAccount' data-idcolumn='ID' data-idvalue='" + dr["AccountID"] + "'");
                            sbCard.Append(" element-id='" + id + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                            sbCard.Append(" data-intparam1='" + id + "' data-intparam2='' data-intparam3=''");
                            sbCard.Append(" value='" + dr["Name"].ToString().Replace("'", "&#39;") + "'>");
                            sbCard.Append("<div id='lookupDIVCardAccountCode" + id + "'></div>");
                            sbCard.Append("</td>");
                            sbCard.Append("<td class='p-1' id='TdCardAccountDescription" + id + "' style='width:10cm;'>");
                            sbCard.Append("<input style='width:100%; border-radius:6px;' class='CardAccountDescription excelCells form-control form-control-sm' type='text'");
                            sbCard.Append(" id='CardAccountDescription" + id + "' value='" + dr["Description"].ToString().Replace("'", "&#39;") + "' element-id='" + id + "'>");
                            sbCard.Append("</td>");
                            sbCard.Append("<td class='p-1' id='TdCardAmount" + id + "' style='width:3cm;'>");
                            sbCard.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells CardAmount form-control form-control-sm'");
                            sbCard.Append(" id='CardAmount" + id + "' value='" + dr["Amount"] + "' element-id='" + id + "'>");
                            sbCard.Append("</td>");
                            sbCard.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
                            sbCard.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCard' serialno='" + id + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
                            sbCard.Append("</td>");
                            sbCard.Append("<td class='p-1 text-center align-middle' id='Carddeleteaction" + id + "' style='width:50px;'>");
                            sbCard.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCard' id='Carddeleteunit" + id + "' element-id='" + id + "'>");
                            sbCard.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
                            sbCard.Append("</li></ul>");
                            sbCard.Append("</td>");
                            //sbCard.Append("<td style='display:none;'>");
                            //sbCard.Append("<input type='hidden' class='carditemid excelCells form-control' id='carditemid" + id + "' value='" + id + "' element-id='" + id + "'>");
                            //sbCard.Append("</td>");
                            sbCard.Append("</tr>");
                        }

                        // Cheque Modal
                        // Cheque Modal
                        else if (tranType == "Cheque")
                        {
                            // Find matching cheque row by VEID = entry ID
                            DataRow[] chequeRows = Cheques.Select("VEID = " + id);
                            DataRow chq = chequeRows.Length > 0 ? chequeRows[0] : null;

                            string chequeid = chq != null && chq["ID"] != DBNull.Value ? chq["ID"].ToString() : "";
                            string chequeNo = chq != null && chq["ChequeNo"] != DBNull.Value ? chq["ChequeNo"].ToString() : "";
                            string chequeDate = chq != null && chq["ChequeDate"] != DBNull.Value ? String.Format("{0:dd/MM/yyyy}", chq["ChequeDate"]) : "";
                            string bankName = chq != null && chq["BankName"] != DBNull.Value ? chq["BankName"].ToString().Replace("'", "&#39;") : "";
                            string bankID = chq != null && chq["BankID"] != DBNull.Value ? chq["BankID"].ToString() : "0";
                            string clearingDays = chq != null && chq["ClrDays"] != DBNull.Value ? chq["ClrDays"].ToString() : "";
                            string pdcPayable = chq != null && chq["PDCPayable"] != DBNull.Value ? chq["PDCPayable"].ToString().Replace("'", "&#39;") : "";

                            sbCheque.Append("<tr id='ChequeRow" + id + "' dataentry-id='"+id+"'>");

                            // PDC Payable — Account lookup (ChequeAccountCode)
                            sbCheque.Append("<td class='p-1' style='width:5cm;'>");
                            sbCheque.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm ChequeAccountCode excelCells'");
                            sbCheque.Append(" id='ChequeAccountCode" + id + "' name='ChequeAccountCode" + id + "'");
                            sbCheque.Append(" onkeydown=\"ShowLookup(event,'ChequeAccountCode" + id + "','lookupDIVChequeAccountCode" + id + "')\"");
                            sbCheque.Append(" oninput=\"LookupTextChanged('ChequeAccountCode" + id + "','lookupDIVChequeAccountCode" + id + "')\"");
                            sbCheque.Append(" data-lookupcriteria='BankorCheque' data-idcolumn='ID' data-idvalue='" + dr["AccountID"] + "'");
                            sbCheque.Append(" element-id='" + id + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                            sbCheque.Append(" data-intparam1='" + id + "' data-intparam2='' data-intparam3=''");
                            sbCheque.Append(" value='" + dr["Name"].ToString().Replace("'", "&#39;") + "'>");
                            sbCheque.Append("<div id='lookupDIVChequeAccountCode" + id + "'></div>");
                            sbCheque.Append("</td>");

                            // Cheque No
                            sbCheque.Append("<td class='p-1' id='TdChequeNo" + id + "' style='width:3cm;'>");
                            sbCheque.Append("<input style='width:100%; border-radius:6px;' class='ChequeNo excelCells form-control form-control-sm' type='text'");
                            sbCheque.Append(" id='ChequeNo" + id + "' value='" + chequeNo + "' element-id='" + id + "'>");
                            sbCheque.Append("</td>");

                            // Cheque Date
                            sbCheque.Append("<td class='p-1' id='TdChequeDate" + id + "' style='width:3cm; '>");
                            sbCheque.Append("<input style='width:100%; border-radius:6px;' oninput='formatDateInput(this)' class='ChequeDate form-control form-control-sm excelCells' type='text'");
                            sbCheque.Append(" id='ChequeDate" + id + "' value='" + chequeDate + "' element-id='" + id + "'>");
                            sbCheque.Append("</td>");

                            // Bank Name — lookup (BankAccountCode)
                            sbCheque.Append("<td class='p-1' style='width:5cm;'>");
                            sbCheque.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm BankAccountCode excelCells'");
                            sbCheque.Append(" id='BankAccountCode" + id + "' name='BankAccountCode" + id + "'");
                            sbCheque.Append(" onkeydown=\"ShowLookup(event,'BankAccountCode" + id + "','lookupDIVBankAccountCode" + id + "')\"");
                            sbCheque.Append(" oninput=\"LookupTextChanged('BankAccountCode" + id + "','lookupDIVBankAccountCode" + id + "')\"");
                            sbCheque.Append(" data-lookupcriteria='BankName' data-idcolumn='ID' data-idvalue='" + bankID + "'");
                            sbCheque.Append(" element-id='" + id + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                            sbCheque.Append(" data-intparam1='" + id + "' data-intparam2='' data-intparam3=''");
                            sbCheque.Append(" value='" + bankName + "'>");
                            sbCheque.Append("<div id='lookupDIVBankAccountCode" + id + "'></div>");
                            sbCheque.Append("</td>");

                            // Clearing Days
                            sbCheque.Append("<td class='p-1' id='TdChequeClearingDays" + id + "' style='width:3cm;'>");
                            sbCheque.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells ChequeClearingDays form-control form-control-sm'");
                            sbCheque.Append(" id='ChequeClearingDays" + id + "' value='" + clearingDays + "' element-id='" + id + "'>");
                            sbCheque.Append("</td>");

                            // Description
                            sbCheque.Append("<td class='p-1' id='TdChequeAccountDescription" + id + "' style='width:3cm;'>");
                            sbCheque.Append("<input style='width:100%; border-radius:6px;' class='ChequeAccountDescription excelCells form-control form-control-sm' type='text'");
                            sbCheque.Append(" id='ChequeAccountDescription" + id + "' value='" + dr["Description"].ToString().Replace("'", "&#39;") + "' element-id='" + id + "'>");
                            sbCheque.Append("</td>");

                            // Amount
                            sbCheque.Append("<td class='p-1' id='TdChequeAmount" + id + "' style='width:3cm;'>");
                            sbCheque.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells ChequeAmount form-control form-control-sm'");
                            sbCheque.Append(" id='ChequeAmount" + id + "' value='" + dr["Amount"] + "' element-id='" + id + "'>");
                            sbCheque.Append("</td>");

                            // Add button
                            sbCheque.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
                            sbCheque.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrowCheque' serialno='' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
                            sbCheque.Append("</td>");

                            // Delete button
                            sbCheque.Append("<td class='p-1 text-center align-middle' id='Chequedeleteaction" + id + "' style='width:50px;'>");
                            sbCheque.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete deleterowCheque' id='Chequedeleteunit" + id + "' element-id='" + id + "'>");
                            sbCheque.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
                            sbCheque.Append("</li></ul>");
                            sbCheque.Append("</td>");

                            sbCheque.Append("<td style='display:none;' chequeitemid='" + chequeid + "'>");
                            sbCheque.Append("</td>");

                            //sbCheque.Append("<td style='display:none;'>");
                            //sbCheque.Append("<input type='hidden' class='chequeitemid excelCells form-control' id='chequeitemid" + id + "' value='" + chequeid + "' name='chequeitemid' element-id='" + id + "'>");
                            //sbCheque.Append("</td>");

                            sbCheque.Append("</tr>");
                        }
                    }
                }

                string accountDetails = sbAccount.ToString();
                string paymentDetails = sbPayment.ToString();
                string cash = sbCash.ToString();
                string card = sbCard.ToString();
                string cheque = sbCheque.ToString();


                // Serialize once each — no duplicate loop
                string Trans = SerializeDataTable(Transaction);
                string Additionalentries = SerializeDataTable(Additional);  // you already do this

                return Json(new
                {
                    success = true,
                    header = Trans,
                    additonals = Additionalentries,
                    accountDetails = accountDetails,
                    paymentDetails = paymentDetails,
                    cash = cash,
                    card = card,
                    cheque = cheque,
                    message = "Success"
                });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransactions(int id)
        {
            try
            {
                string Result = DALVouchers.DeleteFillTransactions(id);
                if (Result != "true")
                {
                    return Json(new { success = false, message = Result });
                }
                else
                {
                    return Json(new { success = true, message = "Transaction deleted successfully" });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }
        }

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
    }
}
