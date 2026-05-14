using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class PurchaseEnquiryController : BaseController
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
            _MenuRow = new Lazy<DataRow>(() => DALMenu.LoadWindowsForm(268).Rows[0]);
            _VoucherTypeRow = new Lazy<DataRow>(() => DALVouchers.FillVoucherRow(268, MenuRow["ID"]));
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
                string rowClass = count % 2 == 0 ? "even" : "odd";
                sb.Append($"<tr class='{rowClass}'>");
                sb.Append($"<td>{count}</td>");
                sb.Append($"<td>{dr["TransactionNo"]}</td>");
                sb.Append($"<td>{Convert.ToDateTime(dr["Date"]):dd/MM/yyyy}</td>");
                sb.Append($"<td>{dr["AccountName"]}</td>");
                sb.Append($"<td>{dr["Amount"]}</td>");
                sb.Append($"<td><ul class='action'><li class='edit' onclick='RowClick({dr["ID"]})'><a href='#'><i class='icon-pencil-alt'></i></a></li></ul></td>");
                sb.Append("</tr>");
                count++;
            }

            ViewBag.voucher = VoucherTypeRow;
            ViewBag.MenuID = MenuID;
            ViewBag.DataTable = sb.ToString();
            ViewBag.RowType = dr2["RowType"].ToString();
            ViewBag.VoucherCode = dr2["Code"].ToString();
            ViewBag.VoucherID = dr2["VoucherID"].ToString();

            return View("~/Views/Invertory/Transactions/PurchaseEnquiry.cshtml");
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
                request.FiTransactions.PageID = (int)PageIDs.PurchaseEnquiry;
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
                    sb.Append("<input type='text' style='width: 2cm;text-align:center;' class='form-control ItemQty excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Qty"]), 2) + "' element-id='" + Sn + "' id='ItemQty" + Sn + "' /></td>");

                    // 4. Rate
                    sb.Append("<td id='rateTd" + Sn + "' >");
                    sb.Append("<input type='text' style='width: 2cm;text-align:right;' class='form-control ItemRate excelCells' element-factor='" + ToFixedNoRound(Convert.ToDecimal(dr["Factor"]), 2) + "' element-id='" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Rate"]), 2) + "' id='ItemRate" + Sn + "' disabled/></td>");

                    // 5. Gross Amount
                    sb.Append("<td class='ItemGrossAmtTd' >");
                    sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells'style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' value='" + ToFixedNoRound(Convert.ToDecimal(dr["GrossAmount"]), 2) + "' disabled/></td>");

                    // 6. Discount %
                    sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' style='width: 2cm;text-align:center;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["DiscountPerc"]), 2) + "' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' /></td>");

                    // 7. Discount Amt
                    sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style='width: 2cm;text-align:right;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Discount"]), 2) + "' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' /></td>");

                    // 8. Amount
                    sb.Append("<td class='amtTd' id='amtTd" + Sn + "'>");
                    sb.Append("<input type='text' class='form-control ItemAmt excelCells' style='width: 2cm;text-align:right;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Amount"]), 2) + "' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/></td>");

                    // 9. Tax %
                    sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
                    if (dr["TaxTypeID"].ToString() != "")
                    {
                        object taxTypeValue = DtDetails.Rows[0]["TaxTypeID"];

                        if (taxTypeValue != DBNull.Value && taxTypeValue != null && !string.IsNullOrWhiteSpace(taxTypeValue.ToString()) && taxTypeValue.ToString() != "0")
                        {
                            DataTable TaxDetails = DALVouchers.ProductTaxDetails(Convert.ToInt64(taxTypeValue));
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align:center;'  value='" + String.Format("{0:F2}", TaxDetails.Rows[0]["SalesPerc"]) + "' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' taxaccountid='" + TaxDetails.Rows[0]["TaxAccountID"] + "' />");
                        }
                        else
                        {
                            // Always display two decimal places (50.00 instead of 50.0)
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align: center;'  value='' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");
                        }
                    }
                    else
                    {
                        sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' style='width: 2cm;text-align: center;' />");
                    }
                    sb.Append("</td>");

                    // 10. Tax Amount
                    sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TaxValue"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' /></td>");

                    // 11. Total
                    sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTotal excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TotalAmount"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled/></td>");

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
                    message = "Success"
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
                FiTransactions.CommonNarration = null;
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
                FiTransactions.CostCentreID = null;
                FiTransactions.PageID = (int)PageIDs.PurchaseOrder;

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
                            item.RowType = null;
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
                            item.SerialNo = null;
                            item.ReplaceQty = null;
                            item.PrintedMRP = null;
                            item.PrintedRate = null;
                            item.PTSRate = null;
                            item.PTRRate = null;
                            item.TempRate = null;
                            item.StockItemID = null;
                            // item.InLocID = null;
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
                        //   FiTransactionAdditionals.ToLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        // FiTransactionAdditionals.Name = null;
                        FiTransactionAdditionals.Code = null;
                        FiTransactionAdditionals.Address = null;
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
                        // FiTransactionAdditionals.EntryDate = null;
                        // FiTransactionAdditionals.EntryNo = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        //  FiTransactionAdditionals.BankAddress = null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        //  FiTransactionAdditionals.ExpiryDate = null;
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
                        FiTransactionAdditionals.Terms = null;
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
                        //  FiTransactionAdditionals.InLocID = null;
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
                            item.RowType = null;
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
                            item.SerialNo = null;
                            item.ReplaceQty = null;
                            item.PrintedMRP = null;
                            item.PrintedRate = null;
                            item.PTSRate = null;
                            item.PTRRate = null;
                            item.TempRate = null;
                            item.StockItemID = null;
                            //    item.InLocID = null;
                            item.OutLocID = null;
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
                        //  FiTransactionAdditionals.ToLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        // FiTransactionAdditionals.Name = null;
                        FiTransactionAdditionals.Code = null;
                        FiTransactionAdditionals.Address = null;
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
                        //   FiTransactionAdditionals.InterestPerc = null;
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
                        //    FiTransactionAdditionals.SubmitDate = null;
                        FiTransactionAdditionals.EndTime = null;
                        FiTransactionAdditionals.HandOverTime = null;
                        FiTransactionAdditionals.LorryHireRate = null;
                        FiTransactionAdditionals.QtyPerLoad = null;
                        //  FiTransactionAdditionals.PassNo = null;
                        //  FiTransactionAdditionals.ReferenceDate = null;
                        //  FiTransactionAdditionals.ReferenceNo = null;
                        FiTransactionAdditionals.AuditNote = null;
                        FiTransactionAdditionals.Terms = null;
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
                        //  FiTransactionAdditionals.InLocID = null;
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
                        return Json(new { success = false, transactionNo = "" });
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
