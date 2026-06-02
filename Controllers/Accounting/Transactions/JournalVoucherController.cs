using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Accounting.Transactions
{
    public class JournalVoucherController : BaseController
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
                    _MenuRow = DALMenu.LoadWindowsForm(68).Rows[0];

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
                    _VoucherTypeRow = DALVouchers.FillVoucherRow(68, MenuRow["ID"]);

                }
                return _VoucherTypeRow;
            }
        }

        private DAL.Accounting.Transactions.JournalEntries _JournalEntries;
        private DAL.Accounting.Transactions.JournalEntries JournalEntries
        {
            get
            {
                if (_JournalEntries == null)
                {
                    _JournalEntries = new DAL.Accounting.Transactions.JournalEntries(ConnectionString);
                }
                return _JournalEntries;
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
            string JournalVoucher = sb.ToString();
            ViewBag.voucher = VoucherTypeRow;
            ViewBag.DataTable = JournalVoucher;
            return View("~/Views/Accounting/Transactions/JournalVoucher.cshtml");

        }

        [HttpPost]
        public async Task<IActionResult> NewEntryDetails()
        {
            StringBuilder sb = new StringBuilder();
            int Sn = 1;

            sb.Append("<tr id='Row" + Sn + "'>");

            // Serial No
            sb.Append("<td class='serial-no text-center align-middle' style='width:40px;'>" + Sn + "</td>");

            // Account Name — widest column
            sb.Append("<td class='p-1' style='min-width:220px;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm AccountCode excelCells' id='AccountCode" + Sn + "' name='AccountCode" + Sn + "' placeholder='Account Name'");
            sb.Append(" onkeydown=\"ShowLookup(event,'AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='" + Sn + "' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Description — second widest
            sb.Append("<td class='p-1' id='TdAccountDescription" + Sn + "' style='min-width:180px;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='AccountDescription excelCells form-control form-control-sm' type='text' id='AccountDescription" + Sn + "' placeholder='Description' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Due Date — narrow
            sb.Append("<td class='p-1' id='TdDueDate" + Sn + "' style='min-width:140px;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='DueDate form-control form-control-sm excelCells' type='date' id='DueDate" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Debit
            sb.Append("<td class='p-1' id='TdDebit" + Sn + "' style='min-width:120px;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Debit form-control form-control-sm' id='Debit" + Sn + "' placeholder='0.00' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Credit
            sb.Append("<td class='p-1' id='TdCredit" + Sn + "' style='min-width:120px;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Credit form-control form-control-sm' id='Credit" + Sn + "' placeholder='0.00' value='' element-id='" + Sn + "'>");
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

            return Json(new { success = true, innerHTML = sb.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> NewRow(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;

            sb.Append("<tr id='Row" + Sn + "'>");

            // Serial No
            sb.Append("<td class='serial-no text-center align-middle' style='width:40px;'></td>");

            // Account Name — widest column
            sb.Append("<td class='p-1' style='min-width:220px;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' autocomplete='off' class='form-control form-control-sm AccountCode excelCells' id='AccountCode" + Sn + "' name='AccountCode" + Sn + "' placeholder='Account Name'");
            sb.Append(" onkeydown=\"ShowLookup(event,'AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" oninput=\"LookupTextChanged('AccountCode" + Sn + "','lookupDIVAccountCode" + Sn + "')\"");
            sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='" + Sn + "' element-id='" + Sn + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='" + Sn + "' data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVAccountCode" + Sn + "'></div>");
            sb.Append("</td>");

            // Hidden AccountName (kept for compatibility)
            sb.Append("<td id='TdAccountName" + Sn + "' style='display:none;'>");
            sb.Append("<p class='excelCellText' id='AccountName" + Sn + "' element-id='" + Sn + "'></p>");
            sb.Append("</td>");

            // Description — second widest
            sb.Append("<td class='p-1' id='TdAccountDescription" + Sn + "' style='min-width:180px;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='AccountDescription excelCells form-control form-control-sm' type='text' id='AccountDescription" + Sn + "' placeholder='Description' value='' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Due Date — narrow
            sb.Append("<td class='p-1' id='TdDueDate" + Sn + "' style='min-width:140px;'>");
            sb.Append("<input style='width:100%; border-radius:6px;' class='DueDate form-control form-control-sm excelCells' type='date' id='DueDate" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Debit
            sb.Append("<td class='p-1' id='TdDebit" + Sn + "' style='min-width:120px;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Debit form-control form-control-sm' id='Debit" + Sn + "' placeholder='0.00' element-id='" + Sn + "'>");
            sb.Append("</td>");

            // Credit
            sb.Append("<td class='p-1' id='TdCredit" + Sn + "' style='min-width:120px;'>");
            sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='numbersOnly excelCells Credit form-control form-control-sm' id='Credit" + Sn + "' placeholder='0.00' value='' element-id='" + Sn + "'>");
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
                request.FiTransactions.PageID = (int)PageIDs.JournalVoucher;
                request.FiTransactions.ApprovalStatus = 'A';
                foreach (var item in request.FiTransactionEntries)
                {
                    item.CurrencyID = 17;
                }

                List<Models.InvTransItems> InvTransItems = null;
                List<Models.FiTransactionEntries> FiTransactionEntries = request.FiTransactionEntries;
                Models.FiTransactions FiTransactions = request.FiTransactions;
                Models.FiTransactionAdditionals FiTransactionAdditionals = null;
                DALVouchers.InsertTransaction(request);
                return Json(new { success = true });
            }
            catch (Exception ex) { throw; }

        }

        [HttpPost]
        public async Task<IActionResult> SaveVoucherJournalEntries(FiTransactions FiTransactions, List<FiTransactionEntries> FiTransactionEntries)
        {
            try
            {
                FiTransactions.AddedBy = (int)UserID;
                FiTransactions.EditedBy = (int)UserID;
                FiTransactions.CompanyID = (int)BranchID;
                FiTransactions.CurrencyID = 1;
                FiTransactions.PageID =68;
                FiTransactions.IsPostDated = false;
                if (FiTransactions.ID == null || FiTransactions.ID == 0)
                {
                    var NextVNo = DALVouchers.GetTransactionNo(VoucherTypeRow["ID"], BranchID);
                    FiTransactions.TransactionNo = NextVNo.ToString();
                    FiTransactions.SerialNo = Convert.ToInt64(NextVNo);
                }
                if (FiTransactions.ID == 0)
                {
                    string Result = JournalEntries.InsertTransactions(FiTransactions);
                    int ID = Convert.ToInt32(Result);
                    bool isNumeric = int.TryParse(Result, out int n);
                    if (isNumeric)
                    {
                        foreach (FiTransactionEntries entry in FiTransactionEntries)
                        {
                            entry.TransactionID = ID;
                            if (entry.ID == 0)
                            {
                                string Result1 = JournalEntries.InsertTransactionEntries(entry);
                            }
                            else
                            {
                                string Result2 = JournalEntries.UpdateTransactionEntries(entry);
                            }
                        }
                        return Json(new { success = true, transactionNo = Result, message = "Transaction saved successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, transactionNo = "", message = "Unable to process the request" });
                    }
                }
                else
                {
                    string Result = JournalEntries.UpdateTransactions(FiTransactions);
                    int ID = Convert.ToInt32(Result);
                    bool isNumeric = int.TryParse(Result, out int n);
                    if (isNumeric)
                    {
                        foreach (FiTransactionEntries entry in FiTransactionEntries)
                        {
                            entry.TransactionID = ID;
                            if (entry.ID == 0)
                            {
                                string Result1 = JournalEntries.InsertTransactionEntries(entry);
                            }
                            else
                            {
                                string Result2 = JournalEntries.UpdateTransactionEntries(entry);
                            }
                        }
                        return Json(new { success = true, ID = Result, message = "Transaction saved successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, transactionNo = "", message = "Unable to process the request" });
                    }
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    success = false,
                    message = Ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> getVoucherJournalEntries(int ID)
        {
            try
            {
                DataTable Dt = new DataTable();
                Dt = JournalEntries.FillTransactions(ID);
                Dictionary<string, object> row;
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                foreach (DataRow dr1 in Dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col1 in Dt.Columns)
                    {
                        row.Add(col1.ColumnName, dr1[col1]);
                    }
                    rows.Add(row);
                }
                DataTable Dt1 = new DataTable();
                Dt1 = JournalEntries.FillTransactionEntries(ID);
                StringBuilder sb = new StringBuilder();
                int No = 1;
                foreach (DataRow dr in Dt1.Rows)
                {
                    string id = dr["ID"].ToString();

                    sb.Append("<tr id='Row" + id + "'>");

                    // Serial No
                    sb.Append("<td class='serial-no text-center align-middle' style='width:40px;'>"+No+"</td>");

                    // Account Name — widest column
                    sb.Append("<td class='p-1' id='TdAccountCode" + id + "' style='min-width:220px;'>");
                    sb.Append("<input type='text' style='width:100%; border-radius:6px;' class='form-control form-control-sm AccountCode excelCells' placeholder='Account Name'");
                    sb.Append(" value='" + dr["Name"] + "'");
                    sb.Append(" data-id='" + dr["AccountID"] + "'");
                    sb.Append(" id='AccountCode" + id + "'");
                    sb.Append(" element-id='" + id + "'");
                    sb.Append(" onkeydown=\"ShowLookup(event,'AccountCode" + id + "','lookupDIVAccountCode" + id + "')\"");
                    sb.Append(" oninput=\"LookupTextChanged('AccountCode" + id + "','lookupDIVAccountCode" + id + "')\"");
                    sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='" + dr["AccountID"] + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                    sb.Append(" data-intparam1='' data-intparam2='' data-intparam3=''>");
                    sb.Append("<div id='lookupDIVAccountCode" + id + "'></div>");
                    sb.Append("</td>");

                    // Hidden AccountName (kept for compatibility)
                    sb.Append("<td id='TdAccountName" + id + "' style='display:none;'>");
                    sb.Append("<p class='excelCellText AccountName' id='AccountName" + id + "' element-id='" + id + "' journelID='" + id + "'>" + dr["Name"] + "</p>");
                    sb.Append("</td>");

                    // Description — second widest
                    sb.Append("<td class='p-1' id='TdAccountDescription" + id + "' style='min-width:180px;'>");
                    sb.Append("<input type='text' style='width:100%; border-radius:6px;' class='form-control form-control-sm excelCells AccountDescription' placeholder='Description'");
                    sb.Append(" value='" + dr["Description"] + "'");
                    sb.Append(" id='AccountDescription" + id + "' element-id='" + id + "'>");
                    sb.Append("</td>");

                    // Due Date — narrow
                    sb.Append("<td class='p-1' id='TdDueDate" + id + "' style='min-width:140px;'>");
                    sb.Append("<input type='date' style='width:100%; border-radius:6px;' class='form-control form-control-sm excelCells DueDate'");
                    sb.Append(" value='");
                    if (dr["DueDate"] != DBNull.Value && dr["DueDate"] != null)
                        sb.Append(String.Format("{0:yyyy-MM-dd}", dr["DueDate"]));
                    sb.Append("'");
                    sb.Append(" id='DueDate" + id + "' element-id='" + id + "'>");
                    sb.Append("</td>");

                    // Debit
                    sb.Append("<td class='p-1' id='TdDebit" + id + "' style='min-width:120px;'>");
                    sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='form-control form-control-sm excelCells Debit numbersOnly' placeholder='0.00'");
                    sb.Append(" value='" + dr["Debit"] + "'");
                    sb.Append(" id='Debit" + id + "' element-id='" + id + "'>");
                    sb.Append("</td>");

                    // Credit
                    sb.Append("<td class='p-1' id='TdCredit" + id + "' style='min-width:120px;'>");
                    sb.Append("<input type='text' style='width:100%; text-align:right; border-radius:6px;' class='form-control form-control-sm excelCells Credit numbersOnly' placeholder='0.00'");
                    sb.Append(" value='" + dr["Credit"] + "'");
                    sb.Append(" id='Credit" + id + "' element-id='" + id + "'>");
                    sb.Append("</td>");

                    // Add button
                    sb.Append("<td class='p-1 text-center align-middle' style='width:50px;'>");
                    sb.Append("<button type='button' class='btn btn-outline-primary btn-sm rounded-2 addrow' element-id='" + id + "' serialno='" + id + "' title='Add Row'><i class='fa-solid fa-plus'></i></button>");
                    sb.Append("</td>");

                    // Delete button
                    sb.Append("<td class='p-1 text-center align-middle' id='deleteaction" + id + "' style='width:50px;'>");
                    sb.Append("<ul class='action mb-0 ps-0 list-unstyled'><li class='delete action_delete' id='deleteunit" + id + "' element-id='" + id + "'>");
                    sb.Append("<a href='#' class='text-danger' title='Delete Row'><i class='icon-trash'></i></a>");
                    sb.Append("</li></ul>");
                    sb.Append("</td>");

                    // Hidden itemid
                    sb.Append("<td style='display:none;'>");
                    sb.Append("<input type='hidden' class='itemid excelCells form-control' id='itemid" + id + "' value='" + id + "' element-id='" + id + "' autocomplete='off'>");
                    sb.Append("</td>");

                    sb.Append("</tr>");
                    No++;
                }

                return Json(new { success = true, innerHTML = sb.ToString(), header = JsonConvert.SerializeObject(rows), message = "Success" });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteTransactionEntries(int ID)
        {
            try
            {
                string Result = JournalEntries.DeleteTransactionEntries(ID);
                if (Result != "NULL")
                {
                    return Json(new { success = true, ID = Result, message = "Transaction entry deleted successfully" });
                }
                else
                {
                    return Json(new { success = true, ID = "", message = "Unable to process the request" });
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    success = false,
                    message = Ex.Message
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransactions(int ID)
        {
            try
            {
                string Result = JournalEntries.DeleteTransactions(ID);
                if (Result != "NULL")
                {
                    return Json(new { success = true, transactionNo = Result, message = "Transaction deleted successfully" });
                }
                else
                {
                    return Json(new { success = false, transactionNo = "", message = "Unable to process the request" });
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    success = false,
                    message = Ex.Message
                });
            }
        }
    }
}
