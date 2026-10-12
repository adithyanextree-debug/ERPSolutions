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
                request.FiTransactions.CurrencyID = 17;
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
                string resultID = DALVouchers.InsertTransaction(request);

                if (int.TryParse(resultID, out int transactionId))
                {
                    return Json(new
                    {
                        success = true,
                        message = "Transaction inserted successfully.",
                        transactionNo = transactionId
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "failed to insert transaction.",
                    transactionNo = resultID
                });

            }
            catch (Exception ex) { throw; }

        }

        public async Task<IActionResult> PrintInvoice(int ID)
        {
            // 1. Header details
            //var invoiceNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);

            DataTable dt1 = DALVouchers.DataTableFillTransactions(ID);
            DataRow dr1 = dt1.Rows[0];
            //Helper: safely read a column that may or may not exist in a DataRow
            string GetSafe(DataRow row, string col, string fallback = "")
            {
                return row.Table.Columns.Contains(col) ? (row[col]?.ToString() ?? fallback) : fallback;
            }

            decimal GetSafeDecimal(DataRow row, string col)
            {
                return row.Table.Columns.Contains(col) && decimal.TryParse(row[col]?.ToString(), out var v) ? v : 0m;
            }

            // TODO: confirm these column names against your POS header table
            decimal paidAmount = GetSafeDecimal(dr1, "PaidAmount");
            decimal balanceAmount = GetSafeDecimal(dr1, "BalanceAmount");
            decimal headerDiscount = GetSafeDecimal(dr1, "Discount");
            string  invoiceNo = dr1["TransactionNo"].ToString();
            DateTime billDateTime = DateTime.Now;
            string billDate = billDateTime.ToString("dd/MM/yyyy");
            string billTime = billDateTime.ToString("h:mm:sstt").ToUpper(); // e.g. 1:24:46PM

            // 2. Line items
            DataTable dt4 = DALVouchers.DataTableFillTransactionEntries(ID);
           
            // TODO: Replace these with your actual company info (from config/session/DB)
            string companyName = "AL SAQER HOUSEHOLD TRADING";
            string companyAddressLine1 = "AL RUMAILA-1";
            string companyAddressLine2 = "AJMAN,U.A.E";
            string companyMob = "MOB : 050-7044566";
            string companyTRN = "TRN :100392636500003";
            string companyWeb = "www.go2camps.com";

            // TODO: Replace with the actual static QR code image path you use
            var qrImgPath = Url.Content("~/CompanyLogo/SalesInvoice.png");

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
<style type='text/css' media='print'>
    @@page {
        size: A4 portrait;
        margin: 12mm 12mm 22mm 12mm;
    }
    #posinvoicetable {
        font-family: Georgia, 'Times New Roman', serif;
        font-size: 11px;
        color:#000;
        margin: 0 auto;
        width: 76mm;
    }
    .receipt-wrapper { width:100%; padding: 4px 6px; }
    .center { text-align:center; }
    .bold { font-weight:bold; }
    .company-name { font-weight:bold; font-size:13px; }
    .company-line { font-size:10.5px; }
    .tax-invoice-title { font-weight:bold; font-size:12px; margin-top:4px; }
    .dotted-line { border:none; border-top:1px dotted #000; margin:4px 0; height:0; }
    .meta-table { width:100%; border-collapse:collapse; font-size:10.5px; margin:4px 0; }
    .meta-table td { padding:0; vertical-align:top; }
    .items-table { width:100%; border-collapse:collapse; margin-top:2px; table-layout:fixed; }
    .items-table th, .items-table td { padding:1px 2px; font-size:10px; vertical-align:top; word-wrap:break-word; }
    .items-table th { text-align:left; font-weight:bold; border-bottom:1px dotted #000; }
    .items-table td.num, .items-table th.num { text-align:right; }
    .items-table td.desc { text-align:left; }
    .totals-table { width:100%; border-collapse:collapse; margin-top:4px; font-size:10.5px; }
    .totals-table td { padding:1px 0; }
    .totals-table td.label { text-align:left; }
    .totals-table td.value { text-align:right; }
    .footer-block { margin-top:6px; font-size:10.5px; }
    .footer-block img { display:block; margin:4px auto; max-width:90px; }
    .thank-you { font-weight:bold; margin-top:2px; }
</style>
<style type='text/css' media='screen'>
    #posinvoicetable {
        font-family: Georgia, 'Times New Roman', serif;
        font-size:11px;
        color:#000;
        width:76mm;
        margin: 0 auto;
    }
    .receipt-wrapper { width:100%; padding:4px 6px; }
    .center { text-align:center; }
    .bold { font-weight:bold; }
    .company-name { font-weight:bold; font-size:13px; }
    .tax-invoice-title { font-weight:bold; font-size:12px; margin-top:4px; }
    .dotted-line { border:none; border-top:1px dotted #000; margin:4px 0; height:0; }
    .meta-table { width:100%; border-collapse:collapse; font-size:10.5px; margin:4px 0; }
    .items-table { width:100%; border-collapse:collapse; margin-top:2px; }
    .items-table th, .items-table td { padding:1px 2px; font-size:10px; }
    .items-table td.num, .items-table th.num { text-align:right; }
    .totals-table { width:100%; border-collapse:collapse; margin-top:4px; font-size:10.5px; }
    .totals-table td.value { text-align:right; }
    .footer-block { margin-top:6px; font-size:10.5px; text-align:center; }
    .footer-block img { display:block; margin:4px auto; max-width:90px; }
</style>

            <div class='receipt-wrapper' id='posinvoicetable'>

                <div class='center company-name'>" + companyName + @"</div>
                <div class='center company-line'>" + companyAddressLine1 + @"</div>
                <div class='center company-line'>" + companyAddressLine2 + @"</div>
                <div class='center company-line'>" + companyMob + @"</div>
                <div class='center company-line'>" + companyTRN + @"</div>
                <div class='center tax-invoice-title'>TAX INVOICE</div>

                <hr class='dotted-line' />

                <table class='meta-table'>
                    <tr>
                        <td style='width:33%;'><span class='bold'>BILL NO:#</span> " + invoiceNo + @"</td>
                        <td style='width:34%;' class='center'>" + billDate + @"</td>
                        <td style='width:33%;' class='center'>" + billTime + @"</td>
                    </tr>
                </table>

                <hr class='dotted-line' />

                <table class='items-table'>
                    <colgroup>
                        <col style='width:46%;'>
                        <col style='width:12%;'>
                        <col style='width:16%;'>
                        <col style='width:12%;'>
                        <col style='width:14%;'>
                    </colgroup>
                    <thead>
                        <tr>
                            <th>DESCRIPTION</th>
                            <th class='num'>QTY</th>
                            <th class='num'>RATE</th>
                            <th class='num'>VAT</th>
                            <th class='num'>AMOUNT</th>
                        </tr>
                    </thead>
            <tbody>");

            decimal grandGross = 0;
            decimal grandVat = 0;
            decimal grandTotal = 0;
            decimal RateTotal = 0;

            foreach (DataRow dr4 in dt4.Rows)
            {
                string itemName = dr4["ItemName"].ToString();
                int qty = Convert.ToInt32(dr4["Qty"]);
                decimal rate = Convert.ToDecimal(dr4["Rate"]);
                decimal tax = Convert.ToDecimal(dr4["TaxValue"]);
                decimal discount = Convert.ToDecimal(dr4["Discount"]);

                decimal lineTotal = qty * rate;
                decimal amount = lineTotal - discount + tax;

                RateTotal += rate;
                grandGross += lineTotal;
                grandVat += tax;
                grandTotal += amount;

                sb.Append(@"
            <tr>
                <td class='desc'>" + itemName + @"</td>
                <td class='num'>" + qty + @"</td>
                <td class='num'>" + rate.ToString("0.00") + @"</td>
                <td class='num'>" + tax.ToString("0.00") + @"</td>
                <td class='num'>" + amount.ToString("0.00") + @"</td>
            </tr>");
            }

            decimal vatPercent = grandGross > 0 ? Math.Round((grandVat / grandGross) * 100, 0) : 0;

            sb.Append(@"
                </tbody>
            </table>

                <hr class='dotted-line' />

                <table class='totals-table'>
                    <tr>
                        <td class='label'>TOTAL</td>
                        <td class='value'>" + RateTotal.ToString("0.00") + @" AED</td>
                    </tr>
                    <tr>
                        <td class='label'>DISCOUNT</td>
                        <td class='value'>" + headerDiscount.ToString("0.00") + @" AED</td>
                    </tr>
                    <tr>
                        <td class='label'>VAT " + vatPercent.ToString("0") + @"%</td>
                        <td class='value'>" + grandVat.ToString("0.00") + @" AED</td>
                    </tr>
                    <tr>
                        <td class='label bold'>TOTAL AMOUNT</td>
                        <td class='value bold'>" + grandTotal.ToString("0.00") + @" AED</td>
                    </tr>
                </table>

                <hr class='dotted-line' />

                <table class='totals-table'>
                    <tr>
                        <td class='label'>PAID AMOUNT</td>
                        <td class='value'>" + paidAmount.ToString("0.00") + @" AED</td>
                    </tr>
                    <tr>
                        <td class='label'>BALANCE</td>
                        <td class='value'>" + balanceAmount.ToString("0.00") + @" AED</td>
                    </tr>
                </table>

                <hr class='dotted-line' />

                <div class='footer-block'>
                    <img src='" + qrImgPath + @"' />
                    <div class='bold center'>SCAN ME</div>
                    <div class='bold center'>" + companyWeb + @"</div>
                    <div class='center thank-you'>*** THANK YOU FOR SHOPPING ***</div>
                </div>

                <hr class='dotted-line' />

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
    }
}
