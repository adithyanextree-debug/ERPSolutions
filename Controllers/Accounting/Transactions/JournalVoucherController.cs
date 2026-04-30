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
                if (dr["Cancelled"].ToString().ToLower() == "true")
                {
                    sb.Append("<td>Cancelled</td>");
                }
                else
                {
                    sb.Append("<td></td>"); // Empty <td> if Cancelled is false
                }
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
            sb.Append("<tr id='Row"+Sn+"'>");
            sb.Append("<td class='p-0'><input autocomplete='off' class='form-control AccountCode excelCells' id='AccountCode"+Sn+"' name='AccountCode"+Sn+"' ");
            sb.Append("onkeydown=");
            sb.Append('"');
            sb.Append("ShowLookup(");
            sb.Append("event,'AccountCode"+Sn+"','lookupDIVAccountCode"+Sn+"')");
            sb.Append('"');
            sb.Append(" oninput=");
            sb.Append('"');
            sb.Append("LookupTextChanged('AccountCode"+Sn+"','lookupDIVAccountCode"+Sn+"')");
            sb.Append('"');
            sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='"+Sn+"' element-id='"+Sn+"' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='"+Sn+"'");
            sb.Append(" data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVAccountCode"+Sn+"' ></div> </td>");
            sb.Append("<td class='p-0' id='TdAccountName"+Sn+"' style='display:none'><p class='excelCellText' id='AccountName"+Sn+"'  element-id='"+Sn+"'></p></td>");
            sb.Append("<td class='p-0' id='TdAccountDescription"+Sn+"' ><input class='AccountDescription excelCells form-control' type='text' id='AccountDescription"+Sn+"' value='' element-id='"+Sn+"'></td>");
            sb.Append("<td class='p-0' id='TdDueDate"+Sn+"' ><input class='DueDate form-control excelCells' type='date' id='DueDate"+Sn+"'  element-id='"+Sn+"'></td>");
            sb.Append("<td class='p-0' id='TdDebit"+Sn+"' ><input type='text' class='numbersOnly excelCells Debit form-control' id='Debit"+Sn+"'  element-id='"+Sn+"' style='text-align: right;'></td>");
            sb.Append("<td class='p-0' id='TdCredit"+Sn+"' ><input type='text' class='numbersOnly excelCells Credit form-control' id='Credit"+Sn+"' value='' element-id='"+Sn+"' style='text-align: right;'></td>");
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

            sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
            sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
            sb.Append("</td>");
            sb.Append("<td style=''>");
            sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");

            sb.Append("</tr>");
           
            return Json(new { success = true, innerHTML = sb.ToString() });
        }

        public async Task<IActionResult> NewRow(int? no)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;
            sb.Append("<tr id='Row"+Sn+"'>");
            sb.Append("<td class='p-0'><input autocomplete='off' class='form-control AccountCode excelCells' id='AccountCode"+Sn+"' name='AccountCode"+Sn+"' ");
            sb.Append("onkeydown=");
            sb.Append('"');
            sb.Append("ShowLookup(");
            sb.Append("event,'AccountCode"+Sn+"','lookupDIVAccountCode"+Sn+"')");
            sb.Append('"');
            sb.Append(" oninput=");
            sb.Append('"');
            sb.Append("LookupTextChanged('AccountCode"+Sn+"','lookupDIVAccountCode"+Sn+"')");
            sb.Append('"');
            sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='"+Sn+"' element-id='"+Sn+"' data-assigncolumnname='AccountName' data-ismandatory='false'");
            sb.Append(" data-intparam1='"+Sn+"'");
            sb.Append(" data-intparam2='' data-intparam3=''>");
            sb.Append("<div id='lookupDIVAccountCode"+Sn+"' ></div> </td>");
            sb.Append("<td class='p-0' id='TdAccountName"+Sn+"' style='display:none'><p class='excelCellText' id='AccountName"+Sn+"'  element-id='"+Sn+"'></p></td>");
            sb.Append("<td class='p-0' id='TdAccountDescription"+Sn+"' ><input class='AccountDescription excelCells form-control' type='text' id='AccountDescription"+Sn+"' value='' element-id='"+Sn+"'></td>");
            sb.Append("<td class='p-0' id='TdDueDate"+Sn+"' ><input class='DueDate form-control excelCells' type='date' id='DueDate"+Sn+"'  element-id='"+Sn+"'></td>");
            sb.Append("<td class='p-0' id='TdDebit"+Sn+"' ><input type='text' class='numbersOnly excelCells Debit form-control' id='Debit"+Sn+"'  element-id='"+Sn+"' style='text-align: right;'></td>");
            sb.Append("<td class='p-0' id='TdCredit"+Sn+"' ><input type='text' class='numbersOnly excelCells Credit form-control' id='Credit"+Sn+"' value='' element-id='"+Sn+"' style='text-align: right;'></td>");
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
                int No = 0;
                foreach (DataRow dr in Dt1.Rows)
                {
                    sb.Append("<tr");
                    sb.Append(" id='Row");
                    sb.Append(dr["ID"]);
                    sb.Append("'>");

                    sb.Append("<td class='p-0' id='TdAccountCode");
                    sb.Append(dr["ID"]);
                    sb.Append("'><input type='text' class='form-control AccountCode excelCells ' value='");
                    sb.Append(dr["Name"]);
                    sb.Append("' data-id='");
                    sb.Append(dr["AccountID"]);
                    sb.Append("' id='AccountCode");
                    sb.Append(dr["ID"]);
                    sb.Append("' element-id='");
                    sb.Append(dr["ID"]);
                    sb.Append("' onkeydown=");
                    sb.Append('"');
                    sb.Append("ShowLookup(");
                    sb.Append("event,'AccountCode" + dr["ID"] + "','lookupDIVAccountCode" + dr["ID"] + "')");
                    sb.Append('"');
                    sb.Append(" oninput=");
                    sb.Append('"');
                    sb.Append("LookupTextChanged('AccountCode" + dr["ID"] + "','lookupDIVAccountCode" + dr["ID"] + "')");
                    sb.Append('"');
                    sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='" + dr["AccountID"] + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                    sb.Append(" data-intparam1=''");
                    sb.Append(" data-intparam2='' data-intparam3=''>");
                    sb.Append("<div id='lookupDIVAccountCode" + dr["ID"] + "' ></div> </td>");
                    sb.Append("</td>");

                    sb.Append("<td class='p-0' id='TdAccountName ");
                    sb.Append(dr["ID"]);
                    sb.Append("' style='display:none'><p class='excelCellText AccountName' id='AccountName");
                    sb.Append(dr["ID"]);
                    sb.Append("' element-id='");
                    sb.Append(dr["ID"]);
                    sb.Append("' journelID='");
                    sb.Append(dr["ID"]);
                    sb.Append("'>");
                    sb.Append(dr["Name"]);
                    sb.Append("</p>");
                    sb.Append("</td>");


                    sb.Append("<td class='p-0' id='TdAccountDescription");
                    sb.Append(dr["ID"]);
                    sb.Append("'><input type='text' class='form-control excelCells AccountDescription' value='");
                    sb.Append(dr["Description"]);
                    sb.Append("' id='AccountDescription");
                    sb.Append(dr["ID"]);
                    sb.Append("' element-id='");
                    sb.Append(dr["ID"]);
                    sb.Append("'>");
                    sb.Append("</td>");
                    sb.Append("<td class='p-0' id='TdDueDate");
                    sb.Append(dr["ID"]);
                    sb.Append("'><input type='date' class='form-control excelCells DueDate' value='");
                    if (dr["DueDate"] != null)
                    {
                        sb.Append(String.Format("{0:yyyy-MM-dd}", dr["DueDate"]));
                    }
                    else
                    {
                        sb.Append(dr["DueDate"]);
                    }
                    sb.Append("' id='DueDate");
                    sb.Append(dr["ID"]);
                    sb.Append("' element-id='");
                    sb.Append(dr["ID"]);
                    sb.Append("'>");
                    sb.Append("</td>");

                    sb.Append("<td class='p-0' id='TdDebit");
                    sb.Append(dr["ID"]);
                    sb.Append("'><input type='text' class='form-control excelCells Debit numbersOnly' value='");
                    sb.Append(dr["Debit"]);
                    sb.Append("' id='Debit");
                    sb.Append(dr["ID"]);
                    sb.Append("' element-id='");
                    sb.Append(dr["ID"]);
                    sb.Append("'>");
                    sb.Append("</td>");

                    sb.Append("<td class='p-0' id='TdCredit");
                    sb.Append(dr["ID"]);
                    sb.Append("'><input type='text' class='form-control excelCells Credit numbersOnly' value='");
                    sb.Append(dr["Credit"]);
                    sb.Append("' id='Credit");
                    sb.Append(dr["ID"]);
                    sb.Append("' element-id='");
                    sb.Append(dr["ID"]);
                    sb.Append("'>");
                    sb.Append("</td>");
                    sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + dr["ID"] + "' serialno='" + dr["ID"] + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

                    sb.Append("<td class='col' id='deleteaction" + dr["ID"] + "' style=''>");
                    sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + dr["ID"] + "' element-id='" + dr["ID"] + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
                    sb.Append("</td>");
                    sb.Append("<td style=''>");
                    sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + dr["ID"] + "' value='"+ dr["ID"] + "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                    sb.Append("</td>");
                    //sb.Append("<td class='badge-danger border-bottom p-0 text-center'><span class='pe-7s-trash action_delete text-white'");
                    //sb.Append(" element-id='");
                    //sb.Append(dr["ID"]);
                    //sb.Append("'>");
                    //sb.Append("</span></td>");

                    sb.Append("</tr>");
                }
                No = ID + 1;
                sb.Append("<tr id='Row" + No + "'>");
                sb.Append("<td class='p-0'><input autocomplete='off' class='form-control AccountCode excelCells' id='AccountCode" + No + "' name='AccountCode" + No + "' ");
                sb.Append("onkeydown=");
                sb.Append('"');
                sb.Append("ShowLookup(");
                sb.Append("event,'AccountCode" + No + "','lookupDIVAccountCode" + No + "')");
                sb.Append('"');
                sb.Append(" oninput=");
                sb.Append('"');
                sb.Append("LookupTextChanged('AccountCode" + No + "','lookupDIVAccountCode" + No + "')");
                sb.Append('"');
                sb.Append(" data-lookupcriteria='Accounts' data-idcolumn='ID' data-idvalue='1' element-id='" + No + "' data-assigncolumnname='AccountName' data-ismandatory='false'");
                sb.Append(" data-intparam1=''");
                sb.Append(" data-intparam2='' data-intparam3=''>");
                sb.Append("<div id='lookupDIVAccountCode" + No + "' ></div> </td>");
                sb.Append("<td class='p-0' style='display:none' id='TdAccountName" + No + "' ><p class='excelCellText AccountName' id='AccountName" + No + "'  element-id='" + No + "'></p></td>");
                sb.Append("<td class='p-0' id='TdAccountDescription" + No + "' ><input class='AccountDescription excelCells form-control' type='text' id='AccountDescription" + No + "' value='' element-id='" + No + "'></td>");
                sb.Append("<td class='p-0' id='TdDueDate" + No + "' ><input class='DueDate form-control excelCells' type='date' id='DueDate" + No + "'  element-id='" + No + "'></td>");
                sb.Append("<td class='p-0' id='TdDebit" + No + "' ><input type='text' class='numbersOnly excelCells Debit form-control' id='Debit" + No + "'  element-id='" + No + "' style='text-align: right;'></td>");
                sb.Append("<td class='p-0' id='TdCredit" + No + "' ><input type='text' class='numbersOnly excelCells Credit form-control' id='Credit" + No + "' value='' element-id='" + No + "'></td>");
                sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + No + "' serialno='" + No + "' style=''><i class='fa-solid fa-plus'></i></button></td>");

                sb.Append("<td class='col' id='deleteaction" + No + "' style=''>");
                sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + No + "' element-id='" + No + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
                sb.Append("</td>");
                sb.Append("<td style=''>");
                sb.Append("<input type='hidden' class='itemid excelCells numbersOnly  form-control' id='itemid" + No + "' value='' element-id='" + No + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");
               
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
