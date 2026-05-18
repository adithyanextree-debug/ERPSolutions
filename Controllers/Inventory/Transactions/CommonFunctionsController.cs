using DocumentFormat.OpenXml.Office.PowerPoint.Y2022.M08.Main;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Transactions
{
    public class CommonFunctionsController : BaseController
    {

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
       
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductAvailableUnits(int ID, int AccountID, int VoucherID, string Unit = "")
        {
            StringBuilder sb = new StringBuilder();
            DataSet Details = DALVouchers.ProductAvailableUnits(ID);
            DataTable dataTable = Details.Tables[0];
            DataTable DtDetails = Details.Tables[1];
            string imagesrcc = (Details.Tables.Count > 2 && Details.Tables[2].Rows.Count > 0
             && Details.Tables[2].Rows[0]["ImagePath"] != DBNull.Value)
             ? Details.Tables[2].Rows[0]["ImagePath"].ToString()
             : "";

            DataTable TaxDetails = null;

            foreach (DataRow dr in dataTable.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["Unit"]);
                sb.Append("'");
                if (Unit == dr["Unit"].ToString())
                {
                    sb.Append(" selected ");
                }
                sb.Append(">");
                sb.Append(dr["Unit"]);
                sb.Append("</option>");
            }
            DataTable UnitDetailss = null;
            string ItemUnitPricee = "";
            if (Unit != "")
            {
                ItemUnitPricee = DALVouchers.GetItemUnitPrice(ID, VoucherID, AccountID, BranchID, Unit);

                UnitDetailss = DALVouchers.UnitDetails(ID,Unit);
            }
            else
            {
                if (dataTable.Rows[0]["Unit"].ToString() != "")
                {
                    ItemUnitPricee = DALVouchers.GetItemUnitPrice(ID, VoucherID, AccountID, BranchID, dataTable.Rows[0]["Unit"].ToString());
                    //UnitDetailss = DALVouchers.UnitDetails(dataTable.Rows[0]["Unit"].ToString());
                    UnitDetailss = DALVouchers.UnitDetails(ID, dataTable.Rows[0]["Unit"].ToString());
                }
            }

            string SalesUnits = sb.ToString();
            Dictionary<string, object> row;
            //List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> rows1 = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> rows2 = new List<Dictionary<string, object>>();
            //foreach (DataRow dr1 in DtDetails.Rows)
            //{
            //    row = new Dictionary<string, object>();
            //    foreach (DataColumn col1 in DtDetails.Columns)
            //    {
            //        row.Add(col1.ColumnName, dr1[col1]);
            //    }
            //    rows.Add(row);
            //}
            if (DtDetails.Rows[0]["TaxTypeID"].ToString() != "")
            {
                TaxDetails = DALVouchers.ProductTaxDetails(Convert.ToInt64(DtDetails.Rows[0]["TaxTypeID"].ToString()));
                foreach (DataRow dr1 in TaxDetails.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col1 in TaxDetails.Columns)
                    {
                        row.Add(col1.ColumnName, dr1[col1]);
                    }
                    rows1.Add(row);
                }
            }

            foreach (DataRow dr1 in UnitDetailss.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col1 in UnitDetailss.Columns)
                {
                    row.Add(col1.ColumnName, dr1[col1]);
                }
                rows2.Add(row);
            }
            return Json(new { success = true, message = "Success", units = SalesUnits,//header = JsonConvert.SerializeObject(rows),
                TaxDetails = JsonConvert.SerializeObject(rows1),UnitDetails = JsonConvert.SerializeObject(rows2), ItemUnitPrice = ItemUnitPricee ,imagesrc = imagesrcc});
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
                        string selected = dr["ID"].ToString() == locs["OutLocID"].ToString() ? " selected" : "";
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
                    DataTable area = DALVouchers.GetArea();
                    sb.Append("<option value=''> -- Choose Area -- </option>");
                    foreach (DataRow dr in area.Rows)
                    {
                        string selected = dr["ID"].ToString() == locs["AreaID"].ToString() ? " selected" : "";
                        sb.Append($"<option value='{dr["ID"]}'{selected}>{dr["Name"]}</option>");
                    }
                    SalesArea = sb.ToString();
                    sb.Clear();
                }
                //=============To get the default account ================//
                DataSet ds = DALVouchers.GetAccountIDSales();
                if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.Account = ds.Tables[0].Rows[0]["AccountName"]?.ToString() ?? "";
                }
                else
                {
                    ViewBag.Account = "";
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
                    sb.Append(" <img src='" + dr["Image"].ToString() +"' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
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
                    sb.Append("<input type='hidden' class='itemid excelCells numbersOnly form-control' id='itemid" + Sn + "' value='"+ Sn + "' element-id='" + Sn + "' autocomplete='off'></td>");

                    sb.Append("</tr>");
                    No++;
                }
                string Entities = sb.ToString();
                sb.Clear();

                // Serialize once each — no duplicate loop
                string Trans = SerializeDataTable(Transaction);
                string Additionalentries = SerializeDataTable(Additional);
                //warehouses = ListWarehouses,
                return Json(new { success = true, innerHTML = Entities, trans = Trans, fiadditional = Additionalentries, additional = Trans, account = ViewBag.Account, 
                    warehouses = ListWarehouse, message = "Success", mode = paymentmode ,area= SalesArea
                });
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

        [HttpPost]
        public async Task<IActionResult> NewEntryDetailsSales()
        {
            //=============To get the defaut account ================//
            DataSet ds = DALVouchers.GetAccountIDSales();
            DataRow dr1 = ds.Tables[0].Rows[0];
            string Account = dr1["AccountName"].ToString();
            if (Account != "")
            {
                string accountname = dr1["AccountName"].ToString();
                ViewBag.Account = accountname.ToString();
            }
           
            StringBuilder sb = new StringBuilder();
            int Sn = 1;
            sb.Append("<tr>");
            //Product Image
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            sb.Append(" <td>");
            sb.Append(" <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
            sb.Append(" </td>");
            // 1. Product Code
            sb.Append("<td id='TdproductCode" + Sn + "' >");
            sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 7cm;' class='form-control productCode' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + Sn + "' ");
            sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2. Unit (Wider)
            sb.Append("<td id='unitTd" + Sn + "' >");
            sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;' class='form-select ItemUnit excelCells'></select>");
            sb.Append("</td>");

            // 3. Qty
            sb.Append("<td id='qtyTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemQty' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemQty" + Sn + "' />");
            sb.Append("</td>");

            // 4. Rate
            sb.Append("<td id='rateTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemRate excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemRate" + Sn + "' disabled />");
            sb.Append("</td>");

            // 5. Gross Amount
            sb.Append("<td class='ItemGrossAmtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 6. Discount %
            sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' />");
            sb.Append("</td>");

            // 7. Discount Amount
            sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' />");
            sb.Append("</td>");

            // 8. Amount
            sb.Append("<td class='amtTd' id='amtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 9. Tax %
            sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");
            sb.Append("</td>");

            // 10. Tax Amount
            sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' />");
            sb.Append("</td>");

            // 11. Total
            sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' style='width: 2cm;'>");
            sb.Append("<input type='text' class='form-control ItemTotal excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled />");
            sb.Append("</td>");

            // 12. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

            sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
            sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
            sb.Append("</td>");
            sb.Append("<td style=''>");
            sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");

            string NewEntry = sb.ToString();
            sb.Clear();
            DataTable warehouses = DALVouchers.FillLocationusingBranch(BranchID);
            sb.Append("<option value=''> -- Choose Warehouse-- </option>");
            foreach (DataRow dr in warehouses.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Name"]);
                sb.Append("</option>");
            }
            string ListWarehouse = sb.ToString();
            sb.Clear();

            DataTable mode = DALVouchers.GetMode();
            sb.Append("<option value=''> -- Choose Payment Type -- </option>");
            foreach (DataRow dr in mode.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (dr["Value"].ToString() == "Cash")
                {
                    sb.Append(" selected");
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string paymentmode = sb.ToString();
            sb.Clear();

            DataTable area = DALVouchers.GetArea();
            sb.Append("<option value=''> -- Choose Area -- </option>");
            foreach (DataRow dr in area.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Name"]);
                sb.Append("</option>");
            }
            string SalesArea = sb.ToString();
            sb.Clear();

            return Json(new { success = true, NewEntry = NewEntry, warehouses = ListWarehouse, account = ViewBag.Account, mode = paymentmode,area= SalesArea });
        }

        [HttpPost]
        public async Task<IActionResult> NewEntryDetailsPurchase()
        {
            //=============To get the defaut account ================//
            DataSet ds = DALVouchers.GetAccountIDPurchase();
            DataRow dr1 = ds.Tables[0].Rows[0];
            string Account = dr1["AccountName"].ToString();
            if (Account != "")
            {
                string accountname = dr1["AccountName"].ToString();
                ViewBag.Account = accountname.ToString();
            }
            StringBuilder sb = new StringBuilder();
            int Sn = 1;
            sb.Append("<tr>");
            //Product Image
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            sb.Append(" <td>");
            sb.Append(" <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
            sb.Append(" </td>");
            // 1. Product Code
            sb.Append("<td id='TdproductCode" + Sn + "' >");
            sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 7cm;' class='form-control productCode' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + Sn + "' ");
            sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2. Unit (Wider)
            sb.Append("<td id='unitTd" + Sn + "' >");
            sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;' class='form-select ItemUnit excelCells'></select>");
            sb.Append("</td>");

            // 3. Qty
            sb.Append("<td id='qtyTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemQty' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemQty" + Sn + "' />");
            sb.Append("</td>");

            // 4. Rate
            sb.Append("<td id='rateTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemRate excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemRate" + Sn + "' disabled />");
            sb.Append("</td>");

            // 5. Gross Amount
            sb.Append("<td class='ItemGrossAmtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 6. Discount %
            sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' />");
            sb.Append("</td>");

            // 7. Discount Amount
            sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' />");
            sb.Append("</td>");

            // 8. Amount
            sb.Append("<td class='amtTd' id='amtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 9. Tax %
            sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");
            sb.Append("</td>");

            // 10. Tax Amount
            sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' />");
            sb.Append("</td>");

            // 11. Total
            sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' style='width: 2cm;'>");
            sb.Append("<input type='text' class='form-control ItemTotal excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled />");
            sb.Append("</td>");

            // 12. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

            sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
            sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
            sb.Append("</td>");
            sb.Append("<td style=''>");
            sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");

            string NewEntry = sb.ToString();
            sb.Clear(); 

            DataTable warehouses = DALVouchers.FillLocationusingBranch(BranchID);
            sb.Append("<option value=''> -- Choose Warehouse -- </option>");
            foreach (DataRow dr in warehouses.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Name"]);
                sb.Append("</option>");
            }
            string ListWarehouse = sb.ToString();
            sb.Clear();

            DataTable mode = DALVouchers.GetMode();
            sb.Append("<option value=''> -- Choose Payment Type -- </option>");
            foreach (DataRow dr in mode.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (dr["Value"].ToString() == "Cash")
                {
                    sb.Append(" selected");
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string paymentmode = sb.ToString();
            sb.Clear();

            return Json(new { success = true, NewEntry = NewEntry, warehouses = ListWarehouse, account = ViewBag.Account, mode = paymentmode });
        }


        [HttpPost]
        public async Task<IActionResult> NewEntryDetailsStock()
        {
            DataTable ds = DALVouchers.GetVoucherDate();
            string StartDate = "";
            if (ds.Rows.Count > 0 && ds.Rows[0]["StartDate"] != DBNull.Value)
            {
                DataRow dr1 = ds.Rows[0];

                DateTime startDate = Convert.ToDateTime(dr1["StartDate"]);

                DateTime previousDate = startDate.AddDays(-1);

                StartDate = previousDate.ToString("dd-MM-yyyy");
            }
            else
            {
                StartDate = "";
            }

            //=============To get the defaut account ================//
            StringBuilder sb = new StringBuilder();
            int Sn = 1;
            sb.Append("<tr>");
            //Product Image
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            sb.Append(" <td>");
            sb.Append(" <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
            sb.Append(" </td>");
            // 1. Product Code
            sb.Append("<td id='TdproductCode" + Sn + "' >");
            sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 15cm;' class='form-control productCode' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + Sn + "' ");
            sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2. Unit (Wider)
            sb.Append("<td id='unitTd" + Sn + "' >");
            sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;' class='form-select ItemUnit excelCells'></select>");
            sb.Append("</td>");

            // 3. Qty
            sb.Append("<td id='qtyTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemQty' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemQty" + Sn + "' />");
            sb.Append("</td>");

            // 4. Rate
            sb.Append("<td id='rateTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemRate excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemRate" + Sn + "' disabled />");
            sb.Append("</td>");

            //// 5. Gross Amount
            //sb.Append("<td class='ItemGrossAmtTd" + Sn + "'>");
            //sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemGrossAmt" + Sn + "' disabled/>");
            //sb.Append("</td>");

            //// 6. Discount %
            //sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
            //sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemDiscPer" + Sn + "' />");
            //sb.Append("</td>");

            //// 7. Discount Amount
            //sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' >");
            //sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemDiscAmt" + Sn + "' />");
            //sb.Append("</td>");

            // 8. Amount
            sb.Append("<td class='amtTd' id='amtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 9. Tax %
            sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' taxTypeID='' style='width: 2cm;text-align:center;' element-id='" + Sn + "' id='ItemTaxPer" + Sn + "' />");
            sb.Append("</td>");

            // 10. Tax Amount
            sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTaxAmt" + Sn + "' />");
            sb.Append("</td>");

            //// 11. Total
            //sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' style='width: 2cm;'>");
            //sb.Append("<input type='text' class='form-control ItemTotal excelCells' style='width: 2cm;text-align:right;' element-id='" + Sn + "' id='ItemTotal" + Sn + "' disabled />");
            //sb.Append("</td>");

            // 12. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

            sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
            sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
            sb.Append("</td>");
            sb.Append("<td style=''>");
            sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");

            string NewEntry = sb.ToString();
            sb.Clear();
            DataTable warehouses = DALVouchers.FillLocationusingBranch(BranchID);
            sb.Append("<option value=''> -- Choose Warehouse-- </option>");
            foreach (DataRow dr in warehouses.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Name"]);
                sb.Append("</option>");
            }
            string ListWarehouse = sb.ToString();
            sb.Clear();
          
            return Json(new { success = true, NewEntry = NewEntry, warehouses = ListWarehouse,voucherDate=StartDate });
        }

        public async Task<IActionResult> NewRowStockItems(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;
            sb.Append("<tr>");
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            //Product Image
            sb.Append(" <td>");
            sb.Append(" <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
            sb.Append(" </td>");

            // 1. Product Code (Wider)
            sb.Append("<td id='TdproductCode" + Sn + "' >");
            sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 15cm;' class='form-control productCode' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + Sn + "' ");
            sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2. Unit (Wider)
            sb.Append("<td id='unitTd" + Sn + "' >");
            sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;'  class='form-select ItemUnit excelCells'></select>");
            sb.Append("</td>");

            // 3. Qty
            sb.Append("<td id='qtyTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemQty' element-id='" + Sn + "' style='width: 2cm;text-align:center;' id='ItemQty" + Sn + "' />");
            sb.Append("</td>");

            // 4. Rate
            sb.Append("<td id='rateTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemRate excelCells' element-id='" + Sn + "'style='width: 2cm;text-align:right;' id='ItemRate" + Sn + "' disabled/>");
            sb.Append("</td>");
           
            // 8. Amount
            sb.Append("<td class='amtTd' id='amtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemAmt excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 9. Tax %
            sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:center;' id='ItemTaxPer" + Sn + "' />");
            sb.Append("</td>");

            // 10. Tax Amount
            sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemTaxAmt" + Sn + "' />");
            sb.Append("</td>");

            // 12. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

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

        public async Task<IActionResult> NewRow(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;
            sb.Append("<tr>");
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            //Product Image
            sb.Append(" <td>");
            sb.Append(" <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 50px; height: 40px;' />");
            sb.Append(" </td>");

            // 1. Product Code (Wider)
            sb.Append("<td id='TdproductCode" + Sn + "' >");
            sb.Append("<input type='text' id='productCode" + Sn + "' style='width: 7cm;' class='form-control productCode' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('productCode" + Sn + "','lookupDIVproductCode" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Items' data-idcolumn='ID' data-idvalue='" + Sn + "' ");
            sb.Append("data-assigncolumnname='ItemName' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVproductCode" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2. Unit (Wider)
            sb.Append("<td id='unitTd" + Sn + "' >");
            sb.Append("<select name='ItemUnit" + Sn + "' element-id='" + Sn + "' id='ItemUnit" + Sn + "' style='width: 3cm;'  class='form-select ItemUnit excelCells'></select>");
            sb.Append("</td>");

            // 3. Qty
            sb.Append("<td id='qtyTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemQty' element-id='" + Sn + "' style='width: 2cm;text-align:center;' id='ItemQty" + Sn + "' />");
            sb.Append("</td>");

            // 4. Rate
            sb.Append("<td id='rateTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemRate excelCells' element-id='" + Sn + "'style='width: 2cm;text-align:right;' id='ItemRate" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 5. Gross Amount
            sb.Append("<td class='ItemGrossAmtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemGrossAmt excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemGrossAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 6. Discount %
            sb.Append("<td class='discsTd' id='dicsTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemDiscPer excelCells' taxTypeID='' element-id='" + Sn + "' style='width: 2cm;text-align:center;' id='ItemDiscPer" + Sn + "' />");
            sb.Append("</td>");

            // 7. Discount Amount
            sb.Append("<td class='dicsAmtTd' id='dicsAmtTd" + Sn + "' style='width: 2cm;'>");
            sb.Append("<input type='text' class='form-control ItemDiscAmt excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemDiscAmt" + Sn + "' />");
            sb.Append("</td>");

            // 8. Amount
            sb.Append("<td class='amtTd' id='amtTd" + Sn + "'>");
            sb.Append("<input type='text' class='form-control ItemAmt excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemAmt" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 9. Tax %
            sb.Append("<td class='taxPerTd' id='taxPerTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxPer excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:center;' id='ItemTaxPer" + Sn + "' />");
            sb.Append("</td>");

            // 10. Tax Amount
            sb.Append("<td class='taxAmtTd' id='taxAmtTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTaxAmt excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemTaxAmt" + Sn + "' />");
            sb.Append("</td>");

            // 11. Total
            sb.Append("<td class='itemTotalTd' id='itemTotalTd" + Sn + "' >");
            sb.Append("<input type='text' class='form-control ItemTotal excelCells' element-id='" + Sn + "' style='width: 2cm;text-align:right;' id='ItemTotal" + Sn + "' disabled/>");
            sb.Append("</td>");

            // 12. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

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

        public static string ToFixedNoRound(decimal value, int decimals)
        {
            decimal factor = (decimal)Math.Pow(10, decimals);
            decimal truncated = Math.Truncate(value * factor) / factor;
            return truncated.ToString($"F{decimals}");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransactionEntries(int id)
        {
            try
            {
                string Result = DALVouchers.DeleteTransactionEntries(id);
                if (Result != "true")
                {
                    return Json(new { success = false, message = Result });
                }
                else
                {
                    return Json(new { success = true, message = "Transaction entry deleted successfully" });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }
        }

        [HttpDelete]
        public async Task<IActionResult> deleteTransactions(int id)
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
     

    }
}
