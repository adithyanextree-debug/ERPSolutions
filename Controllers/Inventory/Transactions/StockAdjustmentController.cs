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
    public class StockAdjustmentController : BaseController
    {
        private Lazy<DAL.General.Common.Menu> _DALMenu;
        private Lazy<DAL.General.Common.Vouchers> _DALVouchers;
        private Lazy<DataRow> _MenuRow;
        private Lazy<DataRow> _VoucherTypeRow;


        private DAL.General.Common.Menu DALMenu => _DALMenu.Value;
        private DAL.General.Common.Vouchers DALVouchers => _DALVouchers.Value;
        private DataRow MenuRow => _MenuRow.Value;
        private DataRow VoucherTypeRow => _VoucherTypeRow.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var conn = ConnectionString;

            _DALMenu = new Lazy<DAL.General.Common.Menu>(() => new(conn));
            _DALVouchers = new Lazy<DAL.General.Common.Vouchers>(() => new(conn));
            _MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(24).Rows[0]);
            _VoucherTypeRow = new Lazy<DataRow>(() => DALVouchers.FillVoucherRow(24, MenuRow["ID"]));
        }
        // ── Your actions — just use DAL directly, nothing extra needed ──

        public async Task<IActionResult> Index(long MenuID)
        {
            SetUserPermissions(MenuID);

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

                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"].ToString() + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("</tr>");
                count++;
            }
            string SalesReturn = sb.ToString();
            ViewBag.voucher = VoucherTypeRow;
            ViewBag.MenuID = MenuID;
            ViewBag.DataTable = SalesReturn;
            ViewBag.RowType = dr2["RowType"].ToString();
            ViewBag.VoucherCode = dr2["Code"].ToString();
            ViewBag.VoucherID = dr2["VoucherID"].ToString();
            return View("~/Views/Invertory/Transactions/StockAdjustment.cshtml");// Json(new { itemmaster = itemmaster, success = true});
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
                request.FiTransactions.PageID = (int)PageIDs.StockAdjustment;
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
                DataTable Entries = DALVouchers.DataTableFillTransactionEntries(ID);
                DataTable Additional = DALVouchers.DataTableFillTransactionAdditionals(ID);
                StringBuilder sb = new StringBuilder();
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
                }

                int Sn = 0;
                int No = 1;
                sb.Clear();

                foreach (DataRow dr in Entries.Rows)
                {
                    DataSet Details = DALVouchers.ProductAvailableUnits(Convert.ToInt64(dr["ItemID"].ToString()));
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
                    sb.Append("<td id='TdproductCode" + Sn + "'>");
                    sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 15cm;' class='form-control productCode' element-id='" + Sn + "' ");
                    sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
                    sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
                    sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + dr["ItemID"] + "' ");
                    sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' ");
                    sb.Append("value='" + dr["ItemName"] + "' />");
                    sb.Append("<div id='lookupDIVproductCode" + Sn + "'></div>");
                    sb.Append("</td>");

                    // 2. Unit
                    sb.Append("<td id='unitTd" + Sn + "'>");
                    sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;' class='form-select ItemUnit excelCells'>");
                    foreach (DataRow dr1 in dataTable.Rows)
                    {
                        sb.Append("<option value='" + dr1["Unit"] + "'" + (dr["Unit"].ToString() == dr1["Unit"].ToString() ? " selected" : "") + ">" + dr1["Unit"] + "</option>");
                    }
                    sb.Append("</select></td>");

                    // 3. Qty
                    sb.Append("<td id='qtyTd" + Sn + "'>");
                    sb.Append("<input type='text' class='form-control ItemQty' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Qty"]), 2) + "' element-id='" + Sn + "' style='width: 2cm;text-align:center;' id='ItemQty" + Sn + "' /></td>");

                    // 4. Rate
                    sb.Append("<td id='rateTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemRate excelCells' element-factor='" + ToFixedNoRound(Convert.ToDecimal(dr["Factor"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Rate"]), 2) + "' id='ItemRate" + Sn + "' disabled/></td>");

                    //// 5. Gross Amount
                    //sb.Append("<td class='ItemGrossAmtTd' >");
                    //sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' style='width: 2cm;text-align:right;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["GrossAmount"]), 2) + "' disabled/></td>");

                    //// 6. Discount %
                    //sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
                    //sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["DiscountPerc"]), 2) + "' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' /></td>");

                    //// 7. Discount Amt
                    //sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
                    //sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Discount"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' /></td>");

                    // 8. Amount
                    sb.Append("<td class='amtTd' id='amtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Amount"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/></td>");

                    // 9. Tax %
                    sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
                    if (dr["TaxTypeID"].ToString() != "")
                    {
                        object taxTypeValue = DtDetails.Rows[0]["TaxTypeID"];

                        if (taxTypeValue != DBNull.Value &&
                            taxTypeValue != null &&
                            !string.IsNullOrWhiteSpace(taxTypeValue.ToString()) &&
                            taxTypeValue.ToString() != "0")
                        {
                            DataTable TaxDetails = DALVouchers.ProductTaxDetails(Convert.ToInt64(taxTypeValue));
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align:center;'  value='" + String.Format("{0:F2}", TaxDetails.Rows[0]["SalesPerc"]) + "' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' taxaccountid='" + TaxDetails.Rows[0]["TaxAccountID"] + "'/>");

                        }
                        else
                        {
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align:center;'  value='' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");

                        }
                    }
                    else
                    {
                        sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' style='width: 2cm;text-align:center;' />");
                    }
                    sb.Append("</td>");


                    // 10. Tax Amt
                    sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TaxValue"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' /></td>");

                    //// 11. Total
                    //sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
                    //sb.Append("<input type='text' class='form-control ItemTotal excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TotalAmount"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled/></td>");

                    // 12. Add button
                    sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "'><i class='fa-solid fa-plus'></i></button></td>");

                    // 13. Delete action
                    sb.Append("<td class='col' id='deleteaction" + Sn + "' >");
                    sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");

                    // 14. Hidden ItemID
                    sb.Append("<td >");
                    sb.Append("<input type='hidden' class='itemid excelCells numbersOnly form-control' id='itemid" + Sn + "' value='" + Sn + "' element-id='" + Sn + "' autocomplete='off'></td>");

                    sb.Append("</tr>");
                    No++;
                }
                string Entities = sb.ToString();
                sb.Clear();

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
                    warehouses = ListWarehouse,
                    message = "Success",

                });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

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
    }
}
