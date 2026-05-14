using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Text;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class EcommerceSalesController : BaseController

    {
        private readonly IWebHostEnvironment _env;

        public EcommerceSalesController(IWebHostEnvironment env)
        {
            _env = env;
        }
        private int ThisPageID
        {
            get
            {
                return 445;
            }
        }
        private int ThisVoucherID
        {
            get
            {
                return 9;
            }
        }

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
            //ViewBag.voucher = VoucherTypeRow;
            //ViewBag.DataTable = DALVouchers.FillVoucher(BranchID, MenuRow["ID"]);
            DataSet ds = DALVouchers.FillVoucher(BranchID, MenuRow["ID"]);
            DataTable dt = ds.Tables[0];
            //DataTable dt2 = ds.Tables[1];
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["TransactionNo"].ToString() + "</td>");
                sb.Append("<td>" + Convert.ToDateTime(dr["Date"]).ToString("dd/MM/yyyy") + "</td>");
                sb.Append("<td>" + dr["CustomerName"].ToString() + "</td>");
                sb.Append("<td>" + dr["Mobile"].ToString() + "</td>");
                sb.Append("<td>" + dr["Email"].ToString() + "</td>");
                sb.Append("<td>" + dr["Address"].ToString() + "</td>");
                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"].ToString() + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("</tr>");
                count++;
            }
            string Purchase = sb.ToString();
            ViewBag.voucher = VoucherTypeRow;
            ViewBag.DataTable = Purchase;
            return View("~/Views/Invertory/Transactions/EcommerceSales.cshtml");// Json(new { itemmaster = itemmaster, success = true});
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

                    foreach (DataRow dr in warehouses.Rows)
                    {
                        sb.Append("<option value='");
                        sb.Append(dr["ID"]);
                        sb.Append("'");
                        if (dr["ID"].ToString() == locs["OutLocID"].ToString())
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
                DataSet ds = DALVouchers.GetAccountIDSales();
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
                    sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 7cm;' class='form-control productCode' element-id='" + Sn + "' ");
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

                    // 5. Gross Amount
                    sb.Append("<td class='ItemGrossAmtTd' >");
                    sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' style='width: 2cm;text-align:right;' value='" + ToFixedNoRound(Convert.ToDecimal(dr["GrossAmount"]), 2) + "' disabled/></td>");

                    // 6. Discount %
                    sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["DiscountPerc"]), 2) + "' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' /></td>");

                    // 7. Discount Amt
                    sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["Discount"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' /></td>");

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
                            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='" + dr["TaxTypeID"] + "' style='width: 2cm;text-align:center;'  value='" + String.Format("{0:F2}", TaxDetails.Rows[0]["SalesPerc"]) + "' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");

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

                    // 11. Total
                    sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
                    sb.Append("<input type='text' class='form-control ItemTotal excelCells' value='" + ToFixedNoRound(Convert.ToDecimal(dr["TotalAmount"]), 2) + "' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled/></td>");

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

                DataSet Statuses = DALVouchers.FillEcommerceStatus(ID);//=====error in tables[0],tables[2]//                
                DataTable SDataTable = Statuses.Tables[0];
                DataTable AllStatus = Statuses.Tables[1];
                DataTable LastData = Statuses.Tables[2];
                DataTable FiTransaction = Statuses.Tables[3];
                string currentStatusID = LastData.Rows[0]["StatusID"].ToString();
                string currentStatus = LastData.Rows[0]["Value"].ToString();
                DataRow fitransactions = FiTransaction.Rows[0];
                string OrderDate = fitransactions["Date"].ToString();
                string OrderNo = fitransactions["TransactionNo"].ToString();
                string AllStatusString = "";
                string Status = "";
               // if (SDataTable.Rows.Count > 0 && AllStatus.Rows.Count > 0 && LastData.Rows.Count > 0)
                if ( AllStatus.Rows.Count > 0 )
                {
                  
                    foreach (DataRow dr in AllStatus.Rows)
                    {
                        string statusValue = dr["Value"].ToString(); // Get the status name
                        string statusID = dr["ID"].ToString(); // Get the status ID (for comparison)
                        //bool isDisabled = false;

                        //// Disable the current status in the dropdown
                        //if (currentStatusID == statusID)
                        //{
                        //    isDisabled = true; // Disable the option if it matches the current status
                        //}

                        //// Only include relevant statuses based on the current status
                        //if (currentStatus == "Order Placed")
                        //{
                        //    if (statusValue != "Order Placed" && statusValue != "Order Cancelled" && statusValue != "Order Packed")
                        //    {
                        //        continue; // Skip statuses that are not relevant to "Order Placed"
                        //    }
                        //}
                        //else if (currentStatus == "Order Cancelled")
                        //{
                        //    if (statusValue != "Order Cancelled")
                        //    {
                        //        continue; // Skip statuses that are not relevant to "Order Cancelled"
                        //    }
                        //}
                        //else if (currentStatus == "Order Packed")
                        //{
                        //    if (statusValue != "Order Packed" && statusValue != "Order Shipped")
                        //    {
                        //        continue; // Skip statuses that are not relevant to "Order Packed"
                        //    }
                        //}
                        //else if (currentStatus == "Order Shipped")
                        //{
                        //    if (statusValue != "Order Shipped" && statusValue != "Order Delivered")
                        //    {
                        //        continue; // Skip statuses that are not relevant to "Order Shipped"
                        //    }
                        //}
                        //else if (currentStatus == "Order Delivered")
                        //{
                        //    if (statusValue != "Order Delivered")
                        //    {
                        //        continue; // Skip statuses that are not relevant to "Order Delivered"
                        //    }
                        //}

                        // Append the option to the dropdown
                        sb.Append("<option value='");
                        sb.Append(statusID); // Use statusID in the value attribute
                        sb.Append("'");

                        // If the status is disabled (current status), mark it with disabled and selected
                        //if (isDisabled)
                        //{
                        //    sb.Append(" disabled selected");
                        //}

                        sb.Append(">");
                        sb.Append(statusValue); // Display the status name in the dropdown
                        sb.Append("</option>");
                    }

                    AllStatusString = sb.ToString();
                    sb.Clear();

                    //sb.Append("<thead>");
                    //sb.Append("<tr>");

                    //foreach (DataColumn dc in SDataTable.Columns)
                    //{
                    //    if (dc.ColumnName == "ID") // Skip the ID column
                    //    {
                    //        continue;
                    //    }
                    //    sb.Append("<th>");
                    //    sb.Append(dc.ColumnName);
                    //    sb.Append("</th>");
                    //}
                    //sb.Append("</tr>");
                    //sb.Append("</thead>");
                  
                }
                foreach (DataRow dr in SDataTable.Rows)
                {
                    sb.Append("<tr ");
                    sb.Append(">");
                    //foreach (DataColumn dc in SDataTable.Columns)
                    //{
                    //    // Skip the ID column
                    //    if (dc.ColumnName == "ID")
                    //    {
                    //        continue;
                    //    }
                    sb.Append("<td>");
                    sb.Append(OrderNo);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(OrderDate);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(dr["Name"].ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(dr["Mobile"].ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(dr["StatusUpdatedOn"].ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(dr["Status"].ToString());
                    sb.Append("</td>");
                    //}
                    sb.Append("</tr>");
                }
                Status = sb.ToString();
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
                return Json(new { success = true, innerHTML = Entities, trans = Trans, fiadditional = Additionalentries, additional = Add, account = ViewBag.Account, warehouses = ListWarehouse, message = "Success", 
                    mode = paymentmode,status= Status,statusdropdown= AllStatusString,remarks= currentStatus
                });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }
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
                FiTransactions.PageID = ThisPageID;
                FiTransactions.RefPageTypeID = null;
                FiTransactions.RefPageTableID = null;
              //  FiTransactions.ReferenceNo = null;
                FiTransactions.FinYearID = null;
                FiTransactions.InstrumentType = null;
                FiTransactions.InstrumentNo = null;
                FiTransactions.InstrumentDate = null;
                FiTransactions.InstrumentBank = null;
                FiTransactions.CommonNarration = null;
                FiTransactions.ApprovedBy = null;
                FiTransactions.ApprovedDate = null;
                FiTransactions.ApproveNote = null;
                FiTransactions.Action = null;
                FiTransactions.RefTransID = null;
                FiTransactions.EditedBy = null;
                FiTransactions.EditedDate = null;
                FiTransactions.CostCentreID = null;
                FiTransactions.MachineName = null;
                FiTransactions.ApprovalStatus = 'A';
                //LCApplnTransID is additional foreign key
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
                            DALVouchers.InvTransItemsMaster(item);

                        }

                        FiTransactionAdditionals.TransactionID = ID;
                        FiTransactionAdditionals.RefTransID1 = null;
                        FiTransactionAdditionals.RefTransID2 = null;
                        FiTransactionAdditionals.MeasureTypeID = null;
                        FiTransactionAdditionals.LoadMeasureTypeID = null;
                        FiTransactionAdditionals.ConsignTermID = null;
                    //    FiTransactionAdditionals.FromLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        //FiTransactionAdditionals.Name = null;
                        FiTransactionAdditionals.Code = null;
                      //  FiTransactionAdditionals.Address = null;
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
                        // FiTransactionAdditionals.DocumentNo = null;
                        //  FiTransactionAdditionals.DocumentDate = null;
                        FiTransactionAdditionals.BEMaxDays = null;
                        //  FiTransactionAdditionals.EntryDate = null;
                        // FiTransactionAdditionals.EntryNo = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        // FiTransactionAdditionals.BankAddress = null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        // FiTransactionAdditionals.ExpiryDate = null;
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
                        //   FiTransactionAdditionals.PassNo = null;
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
                        FiTransactionAdditionals.InLocID = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AccountID = null;
                        FiTransactionAdditionals.RouteID = null;
                        FiTransactionAdditionals.AccountID2 = null;
                        FiTransactionAdditionals.Hours = null;
                        FiTransactionAdditionals.Year = null;
                        // FiTransactionAdditionals.AreaID = null;
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
                            DALVouchers.InvTransItemsMaster(item);
                        }

                        FiTransactionAdditionals.TransactionID = FiTransactions.ID;
                        FiTransactionAdditionals.RefTransID1 = null;
                        FiTransactionAdditionals.RefTransID2 = null;
                        FiTransactionAdditionals.MeasureTypeID = null;
                        FiTransactionAdditionals.LoadMeasureTypeID = null;
                        FiTransactionAdditionals.ConsignTermID = null;
                      //  FiTransactionAdditionals.FromLocationID = null;
                        FiTransactionAdditionals.ExchangeRate1 = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AdvanceExRate = null;
                        FiTransactionAdditionals.CustomsExRate = null;
                        FiTransactionAdditionals.ApprovalDays = null;
                        FiTransactionAdditionals.WorkflowDays = null;
                        FiTransactionAdditionals.PostedBranchID = null;
                        FiTransactionAdditionals.ShipBerthDate = null;
                        FiTransactionAdditionals.IsBit = null;
                        //   FiTransactionAdditionals.Name = null;
                        FiTransactionAdditionals.Code = null;
                      //  FiTransactionAdditionals.Address = null;
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
                        //  FiTransactionAdditionals.InterestAmt = null;
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
                        // FiTransactionAdditionals.EntryDate = null;
                        // FiTransactionAdditionals.EntryNo = null;
                        FiTransactionAdditionals.ApplicationCode = null;
                        //  FiTransactionAdditionals.BankAddress = null;
                        FiTransactionAdditionals.Unit = null;
                        FiTransactionAdditionals.Amount = null;
                        FiTransactionAdditionals.AcceptDate = null;
                        // FiTransactionAdditionals.ExpiryDate = null;
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
                        FiTransactionAdditionals.InLocID = null;
                        FiTransactionAdditionals.ExchangeRate2 = null;
                        FiTransactionAdditionals.AccountID = null;
                        FiTransactionAdditionals.RouteID = null;
                        FiTransactionAdditionals.AccountID2 = null;
                        FiTransactionAdditionals.Hours = null;
                        FiTransactionAdditionals.Year = null;
                        //    FiTransactionAdditionals.AreaID = null;
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

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int StatusID, int VID)
        {
            try
            {
                Models.Ecommerce.Transactions.EcomOrderStatus EcomOrderStatus = new Models.Ecommerce.Transactions.EcomOrderStatus();
                DataSet Results1 = DALVouchers.GetPrevStatusRecord(StatusID, VID);
                DataTable PrevStatusDet = Results1.Tables[0];
                DataTable PrevMiscDet = Results1.Tables[1];
                DataTable PrevMiscDetExist = Results1.Tables[2];
                DataRow dr1 = PrevStatusDet.Rows[0];
                string OrderNo = dr1["TransactionNo"].ToString();
                string OrderDate = dr1["Date"].ToString();
                string Name = dr1["Name"].ToString();
                string Mobile = dr1["Mobile"].ToString();
                if (PrevMiscDetExist.Rows.Count != 0)
                {
                    return Json(new { success = false, message = "Status has been already updated" });
                }
                if (PrevMiscDet.Rows[0]["Description"].ToString() != "")
                {
                    EcomOrderStatus.Remarks = PrevMiscDet.Rows[0]["Description"].ToString();
                }
                else
                {
                    EcomOrderStatus.Remarks = null;
                }
                if (PrevStatusDet.Rows[0]["UserID"].ToString() != "")
                {
                    EcomOrderStatus.UserID = Convert.ToInt32(PrevStatusDet.Rows[0]["UserID"].ToString());
                }
                else
                {
                    EcomOrderStatus.UserID = 0;
                }
                if (PrevStatusDet.Rows[0]["AddressID"].ToString() != "")
                {
                    EcomOrderStatus.AddressID = Convert.ToInt32(PrevStatusDet.Rows[0]["AddressID"].ToString());
                }
                else
                {
                    EcomOrderStatus.AddressID = 0;
                }
                EcomOrderStatus.Date = DateTime.Now;
                EcomOrderStatus.StatusID = StatusID;
                EcomOrderStatus.VID = VID;
                string Result = DALVouchers.AddOrderStatus(EcomOrderStatus);
                int ID = Convert.ToInt32(Result);
                bool isNumeric = int.TryParse(Result, out int n);
                if (isNumeric)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<tr>");
                  //  sb.Append("<td>" + ID + "</td>");
                    sb.Append("<td>" + OrderNo + "</td>");
                    sb.Append("<td>" + OrderDate + "</td>");
                    sb.Append("<td>" + Name + "</td>");
                    sb.Append("<td>" + Mobile + "</td>");
                    sb.Append("<td>" + EcomOrderStatus.Date + "</td>");
                    sb.Append("<td>" + EcomOrderStatus.Remarks + "</td>");
                    sb.Append("</tr>");
                    string LastRow = sb.ToString();
                    sb.Clear();

                    if (EcomOrderStatus.Remarks == "Order Cancelled")
                    {
                        var cancelled = DALVouchers.UpdateCancelled(ID);
                    }

                    DataSet Statuses = DALVouchers.FillEcommerceStatus(VID);//=====error in tables[0],tables[2]//                
                    DataTable SDataTable = Statuses.Tables[0];
                    DataTable AllStatus = Statuses.Tables[1];
                    DataTable LastData = Statuses.Tables[2];
                    string currentStatusID = LastData.Rows[0]["StatusID"].ToString();
                    string currentStatus = LastData.Rows[0]["Value"].ToString();

                    string AllStatusString = "";
                    string Status = "";
                    if (SDataTable.Rows.Count > 0 && AllStatus.Rows.Count > 0 && LastData.Rows.Count > 0)
                    {

                        foreach (DataRow dr in AllStatus.Rows)
                        {
                            string statusValue = dr["Value"].ToString(); // Get the status name
                            string statusID = dr["ID"].ToString(); // Get the status ID (for comparison)
                            //bool isDisabled = false;

                            //// Disable the current status in the dropdown
                            //if (currentStatusID == statusID)
                            //{
                            //    isDisabled = true; // Disable the option if it matches the current status
                            //}

                            //// Only include relevant statuses based on the current status
                            //if (currentStatus == "Order Placed")
                            //{
                            //    if (statusValue != "Order Placed" && statusValue != "Order Cancelled" && statusValue != "Order Packed")
                            //    {
                            //        continue; // Skip statuses that are not relevant to "Order Placed"
                            //    }
                            //}
                            //else if (currentStatus == "Order Cancelled")
                            //{
                            //    if (statusValue != "Order Cancelled")
                            //    {
                            //        continue; // Skip statuses that are not relevant to "Order Cancelled"
                            //    }
                            //}
                            //else if (currentStatus == "Order Packed")
                            //{
                            //    if (statusValue != "Order Packed" && statusValue != "Order Shipped")
                            //    {
                            //        continue; // Skip statuses that are not relevant to "Order Packed"
                            //    }
                            //}
                            //else if (currentStatus == "Order Shipped")
                            //{
                            //    if (statusValue != "Order Shipped" && statusValue != "Order Delivered")
                            //    {
                            //        continue; // Skip statuses that are not relevant to "Order Shipped"
                            //    }
                            //}
                            //else if (currentStatus == "Order Delivered")
                            //{
                            //    if (statusValue != "Order Delivered")
                            //    {
                            //        continue; // Skip statuses that are not relevant to "Order Delivered"
                            //    }
                            //}

                            // Append the option to the dropdown
                            sb.Append("<option value='");
                            sb.Append(statusID); // Use statusID in the value attribute
                            sb.Append("'");

                            // If the status is disabled (current status), mark it with disabled and selected
                            //if (isDisabled)
                            //{
                            //    sb.Append(" disabled selected");
                            //}

                            sb.Append(">");
                            sb.Append(statusValue); // Display the status name in the dropdown
                            sb.Append("</option>");
                        }

                        AllStatusString = sb.ToString();
                        sb.Clear();

                        //String Email = new DAL.Ecommerce.Transactions.General(ConnectionString).GetCustomerEmail(VID);
                        //String Message = "";
                        //String Body = "";
                        //if (EcomOrderStatus.Remarks == "Order Packed")
                        //{
                        //    Message = "Your order has been packed";
                        //    Body = "Your order has been packed";
                        //}
                        //else if (EcomOrderStatus.Remarks == "Order Shipped")
                        //{
                        //    Message = "Your order has been Shipped";
                        //    Body = "Your order has been Shipped";
                        //}
                        //else if (EcomOrderStatus.Remarks == "Order Delivered")
                        //{
                        //    Message = "Your order has been Delivered";
                        //    Body = "Your order has been Delivered";
                        //}
                        //if (Email != "")
                        //{
                        //    await SendMail(Email, Message, Body);
                        //}
                    }
                        return Json(new { success = true, lastrow = LastRow, remarks = EcomOrderStatus.Remarks,status= AllStatusString,vid= VID });
                }
                else
                {
                    return Json(new { success = false, message = Result });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }


    }
}
