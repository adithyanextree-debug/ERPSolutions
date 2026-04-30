using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class PurchaseController : BaseController
    {
        private DAL.Inventory.Transactions.Purchase _DALPurchase;

        private DAL.Inventory.Transactions.Purchase DALPurchase
        {
            get
            {
                if (_DALPurchase == null)
                {
                    _DALPurchase = new DAL.Inventory.Transactions.Purchase(ConnectionString);
                }
                return _DALPurchase;
            }
        }

        private DAL.Inventory.Masters.FiMaVouchers _DALFiMaVouchers;

        private DAL.Inventory.Masters.FiMaVouchers DALFiMaVouchers
        {
            get
            {
                if (_DALFiMaVouchers == null)
                {
                    _DALFiMaVouchers = new DAL.Inventory.Masters.FiMaVouchers(ConnectionString);
                }
                return _DALFiMaVouchers;
            }
        }

        private DAL.General.Masters.Parties _DALParties;

        private DAL.General.Masters.Parties DALParties
        {
            get
            {
                if (_DALParties == null)
                {
                    _DALParties = new DAL.General.Masters.Parties(ConnectionString);
                }
                return _DALParties;
            }
        }

        private DAL.General.Transactions.FiTransactions _DALTransactions;

        private DAL.General.Transactions.FiTransactions DALTransactions
        {
            get
            {
                if (_DALTransactions == null)
                {
                    _DALTransactions = new DAL.General.Transactions.FiTransactions(ConnectionString);
                }
                return _DALTransactions;
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
                    _MenuRow = DALMenu.LoadWindowsForm(15).Rows[0];

                }
                return _MenuRow;
            }
        }

        private DataRow _VoucherTypeRow = null;

        private DataRow VoucherTypeRow
        {
            get
            {
                if (_VoucherTypeRow == null)
                {
                    _VoucherTypeRow = DALVouchers.FillVoucherRow(15, MenuRow["ID"]);

                }
                return _VoucherTypeRow;
            }
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
            return View("~/Views/Invertory/Transactions/Purchase.cshtml");
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
                request.FiTransactions.PageID = (int)PageIDs.Purchase;
                request.FiTransactions.ApprovalStatus = 'A';
                foreach (var item in request.InvTransItems)
                {
                    item.TranType = "Normal";
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

        [HttpGet]
        public async Task<IActionResult> GetInventoryTransaction(int ID)
        {
            try
            {
                DataTable Transaction = DALVouchers.DataTableFillTransactions(ID);
                DataTable Entries = DALVouchers.DataTableFillTransactionEntries(ID);
                DataTable Additional = DALVouchers.DataTableFillTransactionAdditionals(ID);
                string paymentmode = "";
                StringBuilder sb = new StringBuilder();
                string ListWarehouse = "";

                if (Additional.Rows.Count > 0)
                {
                    DataRow locs = Additional.Rows[0];
                    DataTable warehouses = DALVouchers.FillLocationusingBranch(BranchID);
                    sb.Append("<option value=''> -- Choose Warehouse -- </option>");
                    foreach (DataRow dr in warehouses.Rows)
                    {
                        sb.Append("<option value='");
                        sb.Append(dr["ID"]);
                        sb.Append("'");
                        if (dr["ID"].ToString() == locs["InLocID"].ToString())
                        {
                            sb.Append(" selected");
                        }
                        sb.Append(">");
                        sb.Append(dr["Name"]);
                        sb.Append("</option>");
                    }
                    ListWarehouse = sb.ToString();
                    sb.Clear();

                    DataTable mode = DALVouchers.GetMode();
                    sb.Append("<option value=''> -- Choose Payment Type -- </option>");
                    foreach (DataRow datarow in mode.Rows)
                    {
                        sb.Append("<option value='");
                        sb.Append(datarow["ID"]);
                        sb.Append("'");
                        if (datarow["ID"].ToString() == locs["ModeID"].ToString())
                        {
                            sb.Append(" selected");
                        }
                        sb.Append(">");
                        sb.Append(datarow["Value"]);
                        sb.Append("</option>");
                    }
                    paymentmode = sb.ToString();
                    sb.Clear();
                }
                //=============To get the default account ================//
                DataSet ds = DALVouchers.GetAccountIDPurchase();
                DataRow dr2 = ds.Tables[0].Rows[0];
                string Account = dr2["AccountName"].ToString();
                ViewBag.Account = Account.ToString();
                //if (Account != "")
                //{
                //    string accountname = dr2["AccountName"].ToString();
                //    ViewBag.Account = accountname.ToString();
                //}

                int Sn = 0;
                int No = 1;

                foreach (DataRow dr in Entries.Rows)
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
                    sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 5cm;'  class='form-control productCode excelCells changedValue' element-id='" + Sn + "' ");
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
                    sb.Append("<input type='text' style='width: 2cm;' class='form-control ItemQty excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Qty"]), 2) + "' element-id='" + Sn + "' id='ItemQty" + Sn + "' /></td>");

                    // 4. Rate
                    sb.Append("<td id='rateTd" + Sn + "' >");
                    sb.Append("<input type='text' style='width: 2cm;' class='form-control ItemRate excelCells' element-factor='" + ToFixedNoRound(Convert.ToDecimal(dr["Factor"]), 2) + "' element-id='" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Rate"]), 2) + "' id='ItemRate" + Sn + "' disabled/></td>");

                    // 5. Gross Amount
                    sb.Append("<td class='ItemGrossAmtTd' >");
                    sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells'style='width: 2cm;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["GrossAmount"]), 2) + "' disabled/></td>");

                    // 6. Discount %
                    sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' style='width: 2cm;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["DiscountPerc"]), 2) + "' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' /></td>");

                    // 7. Discount Amt
                    sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style='width: 2cm;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Discount"]), 2) + "' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' /></td>");

                    // 8. Amount
                    sb.Append("<td class='amtTd' id='amtTd" + Sn + "'>");
                    sb.Append("<input type='text' class='form-control ItemAmt excelCells' style='width: 2cm;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Amount"]), 2) + "' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/></td>");

                    // 9. Tax %
                    sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
                    if (dr["TaxTypeID"].ToString() != "")
                    {
                        object taxTypeValue = DtDetails.Rows[0]["TaxTypeID"];

                        if (taxTypeValue != DBNull.Value && taxTypeValue != null && !string.IsNullOrWhiteSpace(taxTypeValue.ToString()) && taxTypeValue.ToString() != "0")
                        {
                            DataTable TaxDetails = DALVouchers.ProductTaxDetails(Convert.ToInt64(taxTypeValue));
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align: right;'  value='" + String.Format("{0:F2}", TaxDetails.Rows[0]["SalesPerc"]) + "' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' taxaccountid='" + TaxDetails.Rows[0]["TaxAccountID"] + "' />");
                        }
                        else
                        {
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align: right;'  value='' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");
                        }
                    }
                    else
                    {
                        sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' style='width: 2cm;text-align: right;' />");
                    }
                    sb.Append("</td>");

                    // 10. Tax Amount
                    sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TaxValue"]), 2) + "' style='width: 2cm;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' /></td>");

                    // 11. Total
                    sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTotal excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TotalAmount"]), 2) + "' style='width: 2cm;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled/></td>");

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

                Dictionary<string, object> row;
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                foreach (DataRow dr1 in Transaction.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col1 in Transaction.Columns)
                    {
                        row.Add(col1.ColumnName, dr1[col1]);
                    }
                    rows.Add(row);
                }
                string Trans = JsonConvert.SerializeObject(rows);
                rows.Clear();
                Dictionary<string, object> row1;
                foreach (DataRow dr1 in Transaction.Rows)
                {
                    row1 = new Dictionary<string, object>();
                    foreach (DataColumn col1 in Transaction.Columns)
                    {
                        row1.Add(col1.ColumnName, dr1[col1]);
                    }
                    rows.Add(row1);
                }
                string Add = JsonConvert.SerializeObject(rows);
                rows.Clear();
                Dictionary<string, object> row2;
                foreach (DataRow dr1 in Additional.Rows)
                {
                    row2 = new Dictionary<string, object>();
                    foreach (DataColumn col1 in Additional.Columns)
                    {
                        row2.Add(col1.ColumnName, dr1[col1]);
                    }
                    rows.Add(row2);
                }
                string Additionalentries = JsonConvert.SerializeObject(rows);
                //warehouses = ListWarehouses,
                return Json(new { success = true, innerHTML = Entities, trans = Trans, 
                    fiadditional = Additionalentries, additional = Add, account = ViewBag.Account,
                    warehouses = ListWarehouse, message = "Success",mode=paymentmode });
               
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public static string ToFixedNoRound(decimal value, int decimals)
        {
            decimal factor = (decimal)Math.Pow(10, decimals);
            decimal truncated = Math.Truncate(value * factor) / factor;
            return truncated.ToString($"F{decimals}");
        }

        public async Task<IActionResult> PrintInvoice(int ID)
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

        private List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row[col.ColumnName] = dr[col];
                }
                rows.Add(row);
            }
            return rows;
        }

        [HttpPost]
        public async Task<IActionResult> SaveTransactionEntry(List<InvTransItems> InvTransItems, FiTransactions FiTransactions, FiTransactionAdditionals FiTransactionAdditionals)
        {
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
                FiTransactions.IsPostDated = false;
                FiTransactions.CurrencyID = 1;
                FiTransactions.RefPageTypeID = null;
                FiTransactions.RefPageTableID = null;
                // FiTransactions.ReferenceNo = null;
                FiTransactions.CompanyID = 1;
                FiTransactions.FinYearID = null;
                FiTransactions.InstrumentType = null;
                FiTransactions.InstrumentNo = null;
                FiTransactions.InstrumentDate = null;
                FiTransactions.InstrumentBank = null;
                FiTransactions.ApprovedBy = null;
                FiTransactions.ApprovedDate = null;
                FiTransactions.ApprovalStatus = 'A';
                FiTransactions.ApproveNote = null;
                FiTransactions.Action = null;
                FiTransactions.StatusID = 806;
                FiTransactions.IsAutoEntry = false;
                FiTransactions.Posted = true;
                FiTransactions.Active = true;
                FiTransactions.Cancelled = false;
                FiTransactions.RefTransID = null;
                FiTransactions.EditedBy = null;
                FiTransactions.EditedDate = null;
                FiTransactions.PageID = (int)PageIDs.Purchase;

                FiTransactions.MachineName = null;
                if (FiTransactions.ID == 0)
                {
                    String Result = DALVouchers.InsertTransactions(FiTransactions);

                    int ID = Convert.ToInt32(Result);
                    bool isNumeric = int.TryParse(Result, out int n);
                    if (isNumeric)
                    {
                        foreach (InvTransItems item in InvTransItems)
                        {
                            item.TransactionID = ID;
                            item.RowType = 1;
                            item.Pcs = null;
                            item.AdvanceRate = null;
                            item.OtherRate = null;
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
                            //  item.SerialNo = null;
                            item.ReplaceQty = null;
                            item.PrintedMRP = null;
                            item.PrintedRate = null;
                            item.PTSRate = null;
                            item.PTRRate = null;
                            item.TempRate = null;
                            item.StockItemID = null;
                            item.OutLocID = null;
                            item.Visible = true;
                            DALVouchers.InvTransItemsMaster(item);

                        }
                        FiTransactionAdditionals.TransactionID = ID;
                        FiTransactionAdditionals.RefTransID1 = null;
                        FiTransactionAdditionals.RefTransID2 = null;
                        FiTransactionAdditionals.MeasureTypeID = null;
                        FiTransactionAdditionals.LoadMeasureTypeID = null;
                        FiTransactionAdditionals.ConsignTermID = null;
                        FiTransactionAdditionals.FromLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        //  FiTransactionAdditionals.Name = null;
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
                        //    FiTransactionAdditionals.InterestAmt = null;
                        FiTransactionAdditionals.AvailableAmt = null;
                        FiTransactionAdditionals.AllocationPerc = null;
                        //     FiTransactionAdditionals.InterestPerc= null;
                        FiTransactionAdditionals.TolerencePerc = null;
                        FiTransactionAdditionals.CountryID = null;
                        FiTransactionAdditionals.CountryOfOriginID = null;
                        FiTransactionAdditionals.MaxDays = null;
                        //  FiTransactionAdditionals.DocumentNo= null;
                        //  FiTransactionAdditionals.DocumentDate= null;
                        FiTransactionAdditionals.BEMaxDays = null;
                        //  FiTransactionAdditionals.EntryDate  = null;
                        //  FiTransactionAdditionals.EntryNo = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        //  FiTransactionAdditionals.BankAddress= null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        // FiTransactionAdditionals.ExpiryDate= null;
                        FiTransactionAdditionals.DueDate = null;
                        FiTransactionAdditionals.OpenDate = null;
                        FiTransactionAdditionals.CloseDate = null;
                        FiTransactionAdditionals.StartDate = null;
                        FiTransactionAdditionals.EndDate = null;
                        FiTransactionAdditionals.ClearDate = null;
                        FiTransactionAdditionals.ReceiveDate = null;
                        //   FiTransactionAdditionals.SubmitDate= null;
                        FiTransactionAdditionals.EndTime = null;
                        FiTransactionAdditionals.HandOverTime = null;
                        FiTransactionAdditionals.LorryHireRate = null;
                        FiTransactionAdditionals.QtyPerLoad = null;
                        //  FiTransactionAdditionals.PassNo= null;
                        //  FiTransactionAdditionals.ReferenceDate= null;
                        // FiTransactionAdditionals.ReferenceNo= null;
                        FiTransactionAdditionals.AuditNote = null;
                        // FiTransactionAdditionals.Terms = null;
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
                        FiTransactionAdditionals.OutLocID = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AccountID = null;
                        FiTransactionAdditionals.RouteID = null;
                        FiTransactionAdditionals.AccountID2 = null;
                        FiTransactionAdditionals.Hours = null;
                        FiTransactionAdditionals.Year = null;
                        //  FiTransactionAdditionals.AreaID = null;
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
                        foreach (InvTransItems item in InvTransItems)
                        {
                            item.TransactionID = FiTransactions.ID;
                            item.RowType = 1;
                            item.Pcs = null;
                            item.AdvanceRate = null;
                            item.OtherRate = null;
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
                            // item.SerialNo = null;
                            item.ReplaceQty = null;
                            item.PrintedMRP = null;
                            item.PrintedRate = null;
                            item.PTSRate = null;
                            item.PTRRate = null;
                            item.TempRate = null;
                            item.StockItemID = null;
                            item.Visible = true;
                            DALVouchers.InvTransItemsMaster(item);
                        }
                        FiTransactionAdditionals.TransactionID = FiTransactions.ID;
                        FiTransactionAdditionals.RefTransID1 = null;
                        FiTransactionAdditionals.RefTransID2 = null;
                        FiTransactionAdditionals.MeasureTypeID = null;
                        FiTransactionAdditionals.LoadMeasureTypeID = null;
                        FiTransactionAdditionals.ConsignTermID = null;
                        FiTransactionAdditionals.FromLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        //  FiTransactionAdditionals.Name = null;
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
                        //   FiTransactionAdditionals.InterestAmt = null;
                        FiTransactionAdditionals.AvailableAmt = null;
                        FiTransactionAdditionals.AllocationPerc = null;
                        //    FiTransactionAdditionals.InterestPerc = null;
                        FiTransactionAdditionals.TolerencePerc = null;
                        FiTransactionAdditionals.CountryID = null;
                        FiTransactionAdditionals.CountryOfOriginID = null;
                        FiTransactionAdditionals.MaxDays = null;
                        //  FiTransactionAdditionals.DocumentNo = null;
                        //  FiTransactionAdditionals.DocumentDate = null;
                        FiTransactionAdditionals.BEMaxDays = null;
                        //  FiTransactionAdditionals.EntryDate = null;
                        //  FiTransactionAdditionals.EntryNo = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        //  FiTransactionAdditionals.BankAddress = null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        //   FiTransactionAdditionals.ExpiryDate = null;
                        FiTransactionAdditionals.DueDate = null;
                        FiTransactionAdditionals.OpenDate = null;
                        FiTransactionAdditionals.CloseDate = null;
                        FiTransactionAdditionals.StartDate = null;
                        FiTransactionAdditionals.EndDate = null;
                        FiTransactionAdditionals.ClearDate = null;
                        FiTransactionAdditionals.ReceiveDate = null;
                        //   FiTransactionAdditionals.SubmitDate = null;
                        FiTransactionAdditionals.EndTime = null;
                        FiTransactionAdditionals.HandOverTime = null;
                        FiTransactionAdditionals.LorryHireRate = null;
                        FiTransactionAdditionals.QtyPerLoad = null;
                        //  FiTransactionAdditionals.PassNo = null;
                        //  FiTransactionAdditionals.ReferenceDate = null;
                        //  FiTransactionAdditionals.ReferenceNo = null;
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
                        FiTransactionAdditionals.OutLocID = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AccountID = null;
                        FiTransactionAdditionals.RouteID = null;
                        FiTransactionAdditionals.AccountID2 = null;
                        FiTransactionAdditionals.Hours = null;
                        FiTransactionAdditionals.Year = null;
                        //   FiTransactionAdditionals.AreaID = null;
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

    }
}
