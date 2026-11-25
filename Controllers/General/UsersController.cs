using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ERPSample.Models.General.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.General
{
    public class UsersController : BaseController
    {

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public UsersController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }


        private DAL.General.Users.UsersDAL _UsersDAL;

        private DAL.General.Users.UsersDAL UsersDAL
        {
            get
            {
                if (_UsersDAL == null)
                {
                    _UsersDAL = new DAL.General.Users.UsersDAL (ConnectionString);
                }
                return _UsersDAL;
            }
        }

        public async Task<IActionResult> Index(int MenuID)
        {
            try
            {
                SetUserPermissions(MenuID);

                DataTable dt = UsersDAL.Fill(BranchID);

                StringBuilder sb = new StringBuilder();
                int count = 1;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>" + count + "</td>");
                        sb.Append("<td>");
                        sb.Append(System.Net.WebUtility.HtmlEncode(dr["UserName"]?.ToString() ?? ""));
                        sb.Append("</td>");
                        sb.Append("<td>" + System.Net.WebUtility.HtmlEncode(dr["FullName"]?.ToString() ?? "") + "</td>");
                        //sb.Append("<td>");
                        //sb.Append("<div class='form-check form-switch form-check-reverse'>");
                        //sb.Append("<input class='form-check-input' role='switch'");
                        //sb.Append("name='Active' type='checkbox' ");
                        //if (Convert.ToBoolean(dr["Active"]))
                        //{
                        //    sb.Append("checked ");
                        //}
                        //sb.Append(">");
                        //sb.Append("</div>");
                        //sb.Append("</td>");
                        sb.Append("<td><ul class='action'>");
                        sb.Append("<li class='edit' onclick='RowClick(" + Convert.ToInt32(dr["ID"]) + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                        sb.Append("</ul>");
                        sb.Append("</td>");
                        sb.Append("</tr>");

                        count++;
                    }
                }

                string users = sb.ToString();
                ViewBag.Users = users;
                ViewBag.MenuID = MenuID;

                return View("~/Views/General/Users/Users.cshtml");
            }
            catch (Exception ex)
            {
                // Log the exception here if ILogger is available.
                ViewBag.ErrorMessage = "An error occurred while loading users.";
                ViewBag.Users = "";
                ViewBag.MenuID = MenuID;

                return View("~/Views/General/Users/Users.cshtml");
            }
        }

        public async Task<IActionResult> NewEntry()
        {
            try
            {
                DataTable Dt1 = UsersDAL.GetDesignations();

                StringBuilder sb = new StringBuilder();
                sb.Append("<option value=''>Choose Designation</option>");

                if (Dt1 != null && Dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in Dt1.Rows)
                    {
                        string optionValue = System.Net.WebUtility.HtmlEncode(
                            dr1["ID"]?.ToString() ?? "");

                        string optionText = System.Net.WebUtility.HtmlEncode(
                            dr1["Name"]?.ToString() ?? "");

                        sb.Append($"<option value='{optionValue}'>{optionText}</option>");
                    }
                }
                string designation =  sb.ToString();
                sb.Clear();

                int Sn = 1;
                sb.Append("<tr data-rowkey='" + Sn + "' class='detail-row'>");
                //SNo
                sb.Append("<td class='serial-no'>" + Sn + "</td>");
                sb.Append(" <td>");
                sb.Append(" <input type='checkbox' id='IsActive"+Sn+"' checked />");
                sb.Append(" </td>");
                sb.Append(" <td>");
                sb.Append(" <input type='checkbox' id='IsMainBranch"+Sn+ "' class='IsMainBranch'/>");
                sb.Append(" </td>");

                // 1. Department Name
                sb.Append("<td id='TdDepartment" + Sn + "' >");
                sb.Append("<input type='text' id='Department" + Sn + "' style='width: 7cm;' class='form-control Department' element-id='" + Sn + "' ");
                sb.Append("onkeydown=\"ShowLookup(event,'Department" + Sn + "','lookupDIVDepartment" + Sn + "')\" ");
                sb.Append("oninput=\"LookupTextChanged('Department" + Sn + "','lookupDIVDepartment" + Sn + "')\" ");
                sb.Append("data-lookupcriteria='Department' data-idcolumn='ID' data-idvalue='' ");
                sb.Append("data-assigncolumnname='Department' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
                sb.Append("<div id='lookupDIVDepartment" + Sn + "' ></div>");
                sb.Append("</td>");

                // 2.Branch Name
                sb.Append("<td id='TdBranch" + Sn + "' >");
                sb.Append("<input type='text' id='Branch" + Sn + "' style='width: 7cm;' class='form-control Branch' element-id='" + Sn + "' ");
                sb.Append("onkeydown=\"ShowLookup(event,'Branch" + Sn + "','lookupDIVBranch" + Sn + "')\" ");
                sb.Append("oninput=\"LookupTextChanged('Branch" + Sn + "','lookupDIVBranch" + Sn + "')\" ");
                sb.Append("data-lookupcriteria='Branch' data-idcolumn='ID' data-idvalue='' ");
                sb.Append("data-assigncolumnname='Company' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
                sb.Append("<div id='lookupDIVBranch" + Sn + "' ></div>");
                sb.Append("</td>");

                // 3.Supervisor
                sb.Append("<td id='TdSupervisor" + Sn + "' >");
                sb.Append("<input type='text' id='Supervisor" + Sn + "' style='width: 7cm;' class='form-control Supervisor' element-id='" + Sn + "' ");
                sb.Append("onkeydown=\"ShowLookup(event,'Supervisor" + Sn + "','lookupDIVSupervisor" + Sn + "')\" ");
                sb.Append("oninput=\"LookupTextChanged('Supervisor" + Sn + "','lookupDIVSupervisor" + Sn + "')\" ");
                sb.Append("data-lookupcriteria='Supervisor' data-idcolumn='ID' data-idvalue='' ");
                sb.Append("data-assigncolumnname='Name' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
                sb.Append("<div id='lookupDIVSupervisor" + Sn + "' ></div>");
                sb.Append("</td>");

                // 4. Roles button 
                //sb.Append("<td><button type='button' id='ClickRoles"+Sn+"' onclick='ClickRoles("+Sn+ ")' class='btn btn-primary' >Roles</button></td>");
                sb.Append("<td><button type='button' id='ClickRoles" + Sn + "' onclick='ClickRoles(" + Sn + ")' class='btn btn-primary'>Roles <i class='fa fa-chevron-down ms-1'></i></button></td>");                // 5. Action
                
                //5. Action
                sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
                sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
                sb.Append("</td>");
                sb.Append("<td style='display:none;'>");
                sb.Append("<input type='hidden' class='detailsid excelCells numbersOnly  form-control' id='detailsid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");

                // Roles table Start
                sb.Append("<tr data-rowkey='"+Sn+"' id='RoleRow" + Sn + "' style='display:none;' class='rights-row'>");
                sb.Append("<td colspan='10'>");
                sb.Append("<div class='col-sm-12' id='roletablediv" + Sn + "'>");
                sb.Append("<div class='card rounded-custom'>");

                // Role Selection and Permissions
                sb.Append("<div class='row mb-3'>");
                sb.Append("<div class='col-md-5'>");
                sb.Append("<fieldset class='border rounded p-2'>");
                sb.Append("<legend class='float-none w-auto px-2 fs-6 mb-0'>Roles</legend>");
                sb.Append("<select id='Roles" + Sn + "' class='form-select form-select-sm branch-role' element-id='"+Sn+"'>");
                sb.Append("<option value=''>Select Role</option>");
                sb.Append("</select>");
                sb.Append("</fieldset>");
                sb.Append("</div>");

                // Roles Table
                sb.Append("<div class='card rounded-custom overflow-hidden'>");
                sb.Append("<div class='table-responsive roles-table-scroll'>");
                sb.Append("<table class='table table-light role-table' id='roletable" + Sn + "'>");
                sb.Append("</table>");
                sb.Append("</div>");
                sb.Append("</div>");

                sb.Append("</div>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("</tr>");
                // Roles table End

                string NewEntry = sb.ToString();
                sb.Clear();

                return Json(new
                {
                    success = true,
                    designation = designation,
                    branchtable = NewEntry
                });
            }
            catch (Exception ex)
            {
                // Log the exception here if ILogger is available.
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while loading designations."
                });
            }
        }
        public async Task<IActionResult> NewRow(int Sn)
        {
            string NewEntry = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("<tr data-rowkey='" + Sn + "' class='detail-row'>");
            //SNo
            sb.Append("<td class='serial-no'>" + Sn + "</td>");
            sb.Append(" <td>");
            sb.Append(" <input type='checkbox' id='IsActive"+Sn+"' checked />");
            sb.Append(" </td>");
            sb.Append(" <td>");
            sb.Append(" <input type='checkbox' id='IsMainBranch"+Sn+ "' class='IsMainBranch'/>");
            sb.Append(" </td>");

            // 1. Department Name
            sb.Append("<td id='TdDepartment" + Sn + "' >");
            sb.Append("<input type='text' id='Department" + Sn + "' style='width: 7cm;' class='form-control Department' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'Department" + Sn + "','lookupDIVDepartment" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('Department" + Sn + "','lookupDIVDepartment" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Department' data-idcolumn='ID' data-idvalue='' ");
            sb.Append("data-assigncolumnname='Department' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVDepartment" + Sn + "' ></div>");
            sb.Append("</td>");

            // 2.Branch Name
            sb.Append("<td id='TdBranch" + Sn + "' >");
            sb.Append("<input type='text' id='Branch" + Sn + "' style='width: 7cm;' class='form-control Branch' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'Branch" + Sn + "','lookupDIVBranch" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('Branch" + Sn + "','lookupDIVBranch" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Branch' data-idcolumn='ID' data-idvalue='' ");
            sb.Append("data-assigncolumnname='Company' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVBranch" + Sn + "' ></div>");
            sb.Append("</td>");

            // 3.Supervisor
            sb.Append("<td id='TdSupervisor" + Sn + "' >");
            sb.Append("<input type='text' id='Supervisor" + Sn + "' style='width: 7cm;' class='form-control Supervisor' element-id='" + Sn + "' ");
            sb.Append("onkeydown=\"ShowLookup(event,'Supervisor" + Sn + "','lookupDIVSupervisor" + Sn + "')\" ");
            sb.Append("oninput=\"LookupTextChanged('Supervisor" + Sn + "','lookupDIVSupervisor" + Sn + "')\" ");
            sb.Append("data-lookupcriteria='Supervisor' data-idcolumn='ID' data-idvalue='' ");
            sb.Append("data-assigncolumnname='Name' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' />");
            sb.Append("<div id='lookupDIVSupervisor" + Sn + "' ></div>");
            sb.Append("</td>");

            // 4.   Roles button 
            //sb.Append("<td><button type='button' id='ClickRoles" + Sn + "' onclick='ClickRoles(" + Sn + ")' class='btn btn-primary' >Roles</button></td>");
            sb.Append("<td><button type='button' id='ClickRoles" + Sn + "' onclick='ClickRoles(" + Sn + ")' class='btn btn-primary'>Roles <i class='fa fa-chevron-down ms-1'></i></button></td>");                // 5. Action

            // 5. Action
            sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
            sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
            sb.Append("</td>");
            sb.Append("<td style='display:none;'>");
            sb.Append("<input type='hidden' class='detailsid excelCells numbersOnly  form-control' id='detailsid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");

            // Roles table Start
            sb.Append("<tr data-rowkey='"+Sn+"' id='RoleRow" + Sn + "' style='display:none;' class='rights-row'>");
            sb.Append("<td colspan='10'>");
            sb.Append("<div class='col-sm-12' id='roletablediv" + Sn + "'>");
            sb.Append("<div class='card rounded-custom'>");

            // Role Selection and Permissions
            sb.Append("<div class='row mb-3'>");
            sb.Append("<div class='col-md-5'>");
            sb.Append("<fieldset class='border rounded p-2'>");
            sb.Append("<legend class='float-none w-auto px-2 fs-6 mb-0'>Roles</legend>");
            sb.Append("<select id='Roles" + Sn + "' class='form-select form-select-sm branch-role' element-id='"+Sn+"'>");
            sb.Append("<option value=''>Select Role</option>");
            sb.Append("</select>");
            sb.Append("</fieldset>");
            sb.Append("</div>");

            // Roles Table
            sb.Append("<div class='card rounded-custom overflow-hidden'>");
            sb.Append("<div class='table-responsive roles-table-scroll'>");
            sb.Append("<table class='table table-light role-table' id='roletable" + Sn + "'>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("</tr>");
            // Roles table End

            NewEntry = sb.ToString();
            sb.Clear();


            return Json(new
            {
                success = true,
                newrow = NewEntry
            });
        }

        public async Task<IActionResult> GetRoles(int branchid)
        {
            try
            {
                DataTable Dt1 = UsersDAL.GetRoles(branchid);

                StringBuilder sb = new StringBuilder();
                sb.Append("<option value=''>Choose Role</option>");

                if (Dt1 != null && Dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in Dt1.Rows)
                    {
                        string optionValue = System.Net.WebUtility.HtmlEncode(
                            dr1["ID"]?.ToString() ?? "");

                        string optionText = System.Net.WebUtility.HtmlEncode(
                            dr1["Role"]?.ToString() ?? "");

                        sb.Append($"<option value='{optionValue}'>{optionText}</option>");
                    }
                }
                string roles = sb.ToString();
                sb.Clear();

                return Json(new
                {
                    success = true,
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                // Log the exception here if ILogger is available.
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while loading designations."
                });
            }
        }

        public async Task<IActionResult> Roles(int id, int rowKey)
        {
            try
            {
                DataTable dt = UsersDAL.UserRoles(id);
                StringBuilder sb = new StringBuilder();
                int Sn = 1;
                sb.Append("<thead><tr>");
                sb.Append("<th style='display:none'>ID</th><th>Page Name</th><th>Module Type</th><th>Is View</th><th>Is Create</th><th>Is Edit</th>");
                sb.Append("<th>Is Cancel</th><th>Is Delete</th><th>Is Approve</th><th>IsEditApproved</th>");
                sb.Append("<th>Is HigherApproved</th><th>Is Print</th><th>Is Email</th></tr></thead><tbody>");

                foreach (DataRow dr in dt.Rows)
                {
                    int pageMenuID = Convert.ToInt32(dr["PageMenuID"]);
                    string moduleType = Convert.ToString(dr["ModuleType"]);

                    bool isView = dr["IsView"] != DBNull.Value && Convert.ToBoolean(dr["IsView"]);
                    bool isCreate = dr["IsCreate"] != DBNull.Value && Convert.ToBoolean(dr["IsCreate"]);
                    bool isEdit = dr["IsEdit"] != DBNull.Value && Convert.ToBoolean(dr["IsEdit"]);
                    bool isCancel = dr["IsCancel"] != DBNull.Value && Convert.ToBoolean(dr["IsCancel"]);
                    bool isDelete = dr["IsDelete"] != DBNull.Value && Convert.ToBoolean(dr["IsDelete"]);
                    bool isApprove = dr["IsApprove"] != DBNull.Value && Convert.ToBoolean(dr["IsApprove"]);
                    bool isEditApproved = dr["IsEditApproved"] != DBNull.Value && Convert.ToBoolean(dr["IsEditApproved"]);
                    bool isHigherApprove = dr["IsHigherApprove"] != DBNull.Value && Convert.ToBoolean(dr["IsHigherApprove"]);
                    bool isPrint = dr["IsPrint"] != DBNull.Value && Convert.ToBoolean(dr["IsPrint"]);
                    bool isEmail = dr["IsEMail"] != DBNull.Value && Convert.ToBoolean(dr["IsEMail"]);

                    sb.Append("<tr data-pagemenuid='" + pageMenuID + "' >");

                    sb.Append("<td style='display:none'></td>");
                    //sb.Append("<td>" + Sn + "</td>");
                    sb.Append("<td>" + System.Net.WebUtility.HtmlEncode(Convert.ToString(dr["PageName"]) ?? "") + "</td>");
                    sb.Append("<td>" + System.Net.WebUtility.HtmlEncode(moduleType ?? "") + "</td>");
                    sb.Append("<td><input id='IsView" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isView ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsCreate" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isCreate ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsEdit" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isEdit ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsCancel" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isCancel ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsDelete" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isDelete ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsApprove" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isApprove ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsEditApproved" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isEditApproved ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsHigherApprove" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isHigherApprove ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsPrint" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isPrint ? "checked" : "") + "></td>");
                    sb.Append("<td><input id='IsEmail" + rowKey + "_" + pageMenuID + "' type='checkbox' " + (isEmail ? "checked" : "") + "></td>");
                   
                    sb.Append("</tr>");
                    Sn++;
                }

                sb.Append("</tbody>");
                string table = sb.ToString();
                return Json(new { success = true, table = table });
            }

            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //public async Task<IActionResult> ChangePermissiomStatus(int id, int active)
        //{
        //    try
        //    {
        //        int val = UsersDAL.ChangePermissiomStatus(id, active);
        //        return Json(new { success = true, });
        //    }
        //    catch (Exception Ex)
        //    {

        //        return Json(new { success = false, ex = Ex.Message });
        //    }
        //}

        public async Task<IActionResult> ChangeRolesStatus(Models.General.Users.MaUserRights mauserrights)
        {
            try
            {
                int val = UsersDAL.ChangeRolesStatus(mauserrights);
                return Json(new { success = true, message = "roles permission updated successfully" });
            }
            catch (Exception Ex)
            {

                return Json(new { success = false, ex = Ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveEntry([FromBody] UsersModel model)
        {
            SqlTransaction Tx = null;

            try
            {
                if (model?.MaEmployees == null)
                    return Json(new { success = false, message = "Employee details are required." });

                if (model.MaEmployeeDetails == null || !model.MaEmployeeDetails.Any())
                    return Json(new { success = false, message = "At least one branch detail is required." });

                model.MaUserRights ??= new List<MaUserRights>();

                using SqlConnection Con = new SqlConnection(ConnectionString);
                await Con.OpenAsync();

                Tx = Con.BeginTransaction();

                int count = UsersDAL.CheckUserNameIsUnique(model, Con, Tx);

                if (count < 0)
                    throw new Exception("Error checking username.");

                if (count > 0)
                {
                    Tx.Rollback();
                    return Json(new
                    {
                        success = false,
                        message = "Username already exists. Please choose another username."
                    });
                }

                model.MaEmployees.CreatedBy = (int)UserID;
                model.MaEmployees.CreatedBranchID = (int)BranchID;
                model.MaEmployees.CreatedOn = DateTime.Now;

                foreach (var detail in model.MaEmployeeDetails)
                {
                    detail.CreatedBy = (int)UserID;
                    detail.CreatedOn = DateTime.Now;
                }

                int newID = UsersDAL.InsertUsers(model, Con, Tx);

                Tx.Commit();

                return Json(new
                {
                    success = true,
                    message = "User details saved successfully.",
                    transactionNo = newID
                });
            }
            catch (Exception ex)
            {
                try { Tx?.Rollback(); } catch { }

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            finally
            {
                Tx?.Dispose();
            }
        }


        public async Task<IActionResult> RowClick(int ID)
        {
            try
            {
                DataSet Dt = UsersDAL.RowClick(ID);

                // Check DataSet
                if (Dt == null || Dt.Tables.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No data found."
                    });
                }

                // Safely access each table
                DataTable table0 = Dt.Tables.Count > 0 ? Dt.Tables[0] : null;
                DataTable table1 = Dt.Tables.Count > 1 ? Dt.Tables[1] : null;
                DataTable table2 = Dt.Tables.Count > 2 ? Dt.Tables[2] : null;

                if (table0 == null || table0.Rows.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User details not found."
                    });
                }

                DataRow dr = table0.Rows[0];

                // Safely get string values, including missing columns and DBNull
                string GetValue(DataRow row, string column)
                {
                    if (row == null ||
                        row.Table == null ||
                        !row.Table.Columns.Contains(column) ||
                        row[column] == DBNull.Value)
                    {
                        return string.Empty;
                    }

                    return Convert.ToString(row[column]) ?? string.Empty;
                }

                // Safely get integer values
                int GetInt(DataRow row, string column)
                {
                    string value = GetValue(row, column);

                    return int.TryParse(value, out int result)
                        ? result
                        : 0;
                }

                // Safely encode text inserted into HTML
                string HtmlValue(DataRow row, string column)
                {
                    return System.Net.WebUtility.HtmlEncode(
                        GetValue(row, column)
                    );
                }

                // First table: user details
                string Id = GetValue(dr, "ID");
                string Active = GetValue(dr, "Active");
                string DesignationID = GetValue(dr, "DesignationID");
                string FirstName = GetValue(dr, "FirstName");
                string MiddleName = GetValue(dr, "MiddleName");
                string LastName = GetValue(dr, "LastName");
                string Address = GetValue(dr, "Address");
                string EmailID = GetValue(dr, "EmailID");
                string ResidenceNumber = GetValue(dr, "ResidenceNumber");
                string OfficeNumber = GetValue(dr, "OfficeNumber");
                string MobileNumber = GetValue(dr, "MobileNumber");
                string EmployeeType = GetValue(dr, "EmployeeType");
                string Username = GetValue(dr, "UserName");
                string Password = GetValue(dr, "Password");
                string GmailID = GetValue(dr, "GmailID");
                string IsLocationRestrictedUser =GetValue(dr, "IsLocationRestrictedUser");
                string PhotoID = GetValue(dr, "PhotoID");
                string ImagePath = GetValue(dr, "ImagePath");
                string AccountID = GetValue(dr, "AccountID");
                string AccountName = GetValue(dr, "AccountName");
                StringBuilder sb = new StringBuilder();
                int Sn = 1;
                if (table1 == null || table1.Rows.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Employee branch details not found."
                    });
                }
                // Second table: branch details
                foreach (DataRow dr4 in table1.Rows)
                {
                    bool active = dr4["ActiveFlag"] != DBNull.Value && Convert.ToBoolean(dr4["ActiveFlag"]);
                    bool mainBranch = dr4["IsMainBranch"] != DBNull.Value && Convert.ToBoolean(dr4["IsMainBranch"]);
                    string activeChecked = active ? "checked" : "";
                    string mainBranchChecked = mainBranch ? "checked" : "";
                    string departmentID = dr4["DepartmentID"] == DBNull.Value ? "" : Convert.ToString(dr4["DepartmentID"]);
                    string departmentName = dr4["DepartmentName"] == DBNull.Value ? "" : Convert.ToString(dr4["DepartmentName"]);

                    int branchID = dr4["BranchID"] == DBNull.Value ? 0 : Convert.ToInt32(dr4["BranchID"]);
                    string branchName = dr4["BranchName"] == DBNull.Value ? "" : Convert.ToString(dr4["BranchName"]);

                    string supervisorID = dr4["SuperVisorID"] == DBNull.Value ? "" : Convert.ToString(dr4["SuperVisorID"]);
                    string supervisorName = dr4["EmployeeName"] == DBNull.Value ? "" : Convert.ToString(dr4["EmployeeName"]);

                    string DetailsID = dr4["ID"] == DBNull.Value ? "" : Convert.ToString(dr4["ID"]);
                    int userDetailsID = dr4["ID"] == DBNull.Value ? 0: Convert.ToInt32(dr4["ID"]);
                    DataRow[] branchRights = table2 == null ? Array.Empty<DataRow>() : table2.Select("UserDetailsID = " + userDetailsID);

                    string RoleID = dr4["MaRoleID"] == DBNull.Value ? "" : Convert.ToString(dr4["MaRoleID"]);
                    sb.Append("<tr data-id='" + DetailsID + "' data-rowkey='" + Sn + "' class='detail-row'>");
                    //SNo
                    sb.Append("<td class='serial-no'>" + Sn + "</td>");
                    // Active and Main Branch checkboxes
                    sb.Append(" <td>");     
                    sb.Append(" <input type='checkbox' id='IsActive"+Sn+"' " + activeChecked + " />");
                    sb.Append(" </td>");
                    sb.Append(" <td>");
                    sb.Append(" <input type='checkbox' id='IsMainBranch"+Sn+"' " + mainBranchChecked + " class='IsMainBranch'/>");
                    sb.Append(" </td>");

                    // 1. Department Name
                    sb.Append("<td id='TdDepartment" + Sn + "' >");
                    sb.Append("<input type='text' id='Department" + Sn + "' style='width: 7cm;' class='form-control Department' element-id='" + Sn + "' ");
                    sb.Append("onkeydown=\"ShowLookup(event,'Department" + Sn + "','lookupDIVDepartment" + Sn + "')\" ");
                    sb.Append("oninput=\"LookupTextChanged('Department" + Sn + "','lookupDIVDepartment" + Sn + "')\" ");
                    sb.Append("data-lookupcriteria='Department' data-idcolumn='ID' data-idvalue='" + departmentID + "' ");
                    sb.Append("data-assigncolumnname='Department' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' value='" + System.Net.WebUtility.HtmlEncode(departmentName) + "'/>");
                    sb.Append("<div id='lookupDIVDepartment" + Sn + "' ></div>");
                    sb.Append("</td>");

                    // 2.Branch Name
                    sb.Append("<td id='TdBranch" + Sn + "' >");
                    sb.Append("<input type='text' id='Branch" + Sn + "' style='width: 7cm;' class='form-control Branch' element-id='" + Sn + "' ");
                    sb.Append("onkeydown=\"ShowLookup(event,'Branch" + Sn + "','lookupDIVBranch" + Sn + "')\" ");
                    sb.Append("oninput=\"LookupTextChanged('Branch" + Sn + "','lookupDIVBranch" + Sn + "')\" ");
                    sb.Append("data-lookupcriteria='Branch' data-idcolumn='ID' data-idvalue='" + branchID + "' ");
                    sb.Append("data-assigncolumnname='Company' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' value='" + System.Net.WebUtility.HtmlEncode(branchName) + "'/>");
                    sb.Append("<div id='lookupDIVBranch" + Sn + "' ></div>");
                    sb.Append("</td>");

                    // 3.Supervisor
                    sb.Append("<td id='TdSupervisor" + Sn + "' >");
                    sb.Append("<input type='text' id='Supervisor" + Sn + "' style='width: 7cm;' class='form-control Supervisor' element-id='" + Sn + "' ");
                    sb.Append("onkeydown=\"ShowLookup(event,'Supervisor" + Sn + "','lookupDIVSupervisor" + Sn + "')\" ");
                    sb.Append("oninput=\"LookupTextChanged('Supervisor" + Sn + "','lookupDIVSupervisor" + Sn + "')\" ");
                    sb.Append("data-lookupcriteria='Supervisor' data-idcolumn='ID' data-idvalue='" + supervisorID + "' ");
                    sb.Append("data-assigncolumnname='Name' data-ismandatory='false' data-intparam1='' data-intparam2='' data-intparam3='' value='" + System.Net.WebUtility.HtmlEncode(supervisorName) + "'/>");
                    sb.Append("<div id='lookupDIVSupervisor" + Sn + "' ></div>");
                    sb.Append("</td>");
                    // 4.   Roles button 
                    //sb.Append("<td><button type='button' id='ClickRoles" + Sn + "' onclick='ClickRoles(" + Sn + ")' class='btn btn-primary' >Roles</button></td>");
                    sb.Append("<td><button type='button' id='ClickRoles" + Sn + "' onclick='ClickRoles(" + Sn + ")' class='btn btn-primary'>Roles <i class='fa fa-chevron-down ms-1'></i></button></td>");                // 5. Action

                    // 5. Action
                    sb.Append("<td class='col' style=''><button type='button' class='btn btn-outline-primary rounded-1 addrow' element-id='" + Sn + "' serialno='" + Sn + "' style=''><i class='fa-solid fa-plus'></i></button></td>");
                    sb.Append("<td class='col' id='deleteaction" + Sn + "' style=''>");
                    sb.Append("<ul class='action'><li class='delete ms-3 action_delete' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul>");
                    sb.Append("</td>");
                    sb.Append("<td style='display:none;'>");
                    sb.Append("<input type='hidden' class='detailsid excelCells numbersOnly  form-control' id='detailsid" + Sn + "' value='" + DetailsID + "' element-id='" + Sn + "' autocomplete='off'>");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                    // Roles table Start
                    if (table2 != null)
                    {
                        sb.Append("<tr  data-rowkey='" + Sn + "' id='RoleRow" + Sn + "'  class='rights-row'>");
                        sb.Append("<td colspan='10'>");
                        sb.Append("<div class='col-sm-12' id='roletablediv" + Sn + "'>");
                        sb.Append("<div class='card rounded-custom'>");

                        // Role Selection and Permissions
                        sb.Append("<div class='row mb-3'>");
                        sb.Append("<div class='col-md-5'>");
                        sb.Append("<fieldset class='border rounded p-2'>");
                        sb.Append("<legend class='float-none w-auto px-2 fs-6 mb-0'>Roles</legend>");

                        sb.Append("<select id='Roles" + Sn + "' class='form-select form-select-sm branch-role' element-id='" + Sn + "'>");
                        //Roles dropdown
                        DataTable Dt3 = UsersDAL.GetRoles(branchID);
                        sb.Append("<option value=''>Choose Role</option>");

                        if (Dt3 != null && Dt3.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in Dt3.Rows)
                            {
                                string optionValue = GetValue(dr1, "ID");
                                string optionText = GetValue(dr1, "Role");

                                string selected = optionValue == RoleID
                                    ? "selected"
                                    : "";

                                sb.Append(
                                    "<option value='" +
                                    System.Net.WebUtility.HtmlEncode(optionValue) +
                                    "' " + selected + ">" +
                                    System.Net.WebUtility.HtmlEncode(optionText) +
                                    "</option>"
                                );
                            }
                        }
                        sb.Append("</select>");
                        sb.Append("</fieldset>");
                        sb.Append("</div>");

                        // Roles Table
                        sb.Append("<div class='card rounded-custom overflow-hidden'>");
                        sb.Append("<div class='table-responsive roles-table-scroll'>");
                        sb.Append("<table class='table table-light role-table' id='roletable" + Sn + "'>");
                        sb.Append("<thead><tr>");
                        sb.Append("<th style='display:none'>ID</th><th>Page Name</th><th>Module Type</th><th>Is View</th><th>Is Create</th><th>Is Edit</th>");
                        sb.Append("<th>Is Cancel</th><th>Is Delete</th><th>Is Approve</th><th>IsEditApproved</th>");
                        sb.Append("<th>Is HigherApproved</th><th>Is Print</th><th>Is Email</th></tr></thead><tbody>");
                        //if (table2 != null)
                        //{
                            
                            foreach (DataRow dr3 in branchRights)
                            {
                                string rowID = dr3["ID"] == DBNull.Value ? "" : Convert.ToString(dr3["ID"]);
                                string pageMenuID = dr3["PageMenuID"] == DBNull.Value ? "" : Convert.ToString(dr3["PageMenuID"]);
                                string safeID = System.Net.WebUtility.HtmlEncode(rowID);

                                bool isView = dr3["IsView"] != DBNull.Value && Convert.ToBoolean(dr3["IsView"]);
                                bool isCreate = dr3["IsCreate"] != DBNull.Value && Convert.ToBoolean(dr3["IsCreate"]);
                                bool isEdit = dr3["IsEdit"] != DBNull.Value && Convert.ToBoolean(dr3["IsEdit"]);
                                bool isCancel = dr3["IsCancel"] != DBNull.Value && Convert.ToBoolean(dr3["IsCancel"]);
                                bool isDelete = dr3["IsDelete"] != DBNull.Value && Convert.ToBoolean(dr3["IsDelete"]);
                                bool isApprove = dr3["IsApprove"] != DBNull.Value && Convert.ToBoolean(dr3["IsApprove"]);
                                bool isEditApproved = dr3["IsEditApproved"] != DBNull.Value && Convert.ToBoolean(dr3["IsEditApproved"]);
                                bool isHigherApprove = dr3["IsHigherApprove"] != DBNull.Value && Convert.ToBoolean(dr3["IsHigherApprove"]);
                                bool isPrint = dr3["IsPrint"] != DBNull.Value && Convert.ToBoolean(dr3["IsPrint"]);
                                bool isEmail = dr3["IsEMail"] != DBNull.Value && Convert.ToBoolean(dr3["IsEMail"]);

                                sb.Append("<tr data-pagemenuid='" + pageMenuID + "' data-id='" + rowID + "'>");
                                sb.Append("<td style='display:none;'>" + safeID + "</td>");
                                sb.Append("<td>" + HtmlValue(dr3, "PageName") + "</td>");
                                sb.Append("<td>" + HtmlValue(dr3, "ModuleType") + "</td>");

                                sb.Append("<td><input id='IsView" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isView ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsCreate" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isCreate ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsEdit" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isEdit ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsCancel" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isCancel ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsDelete" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isDelete ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsApprove" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isApprove ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsEditApproved" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isEditApproved ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsHigherApprove" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isHigherApprove ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsPrint" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isPrint ? "checked" : "") + "></td>");
                                sb.Append("<td><input id='IsEmail" + Sn + "_" + pageMenuID + "' type='checkbox' " + (isEmail ? "checked" : "") + "></td>");

                                sb.Append("</tr>");
                            }
                        //}

                        sb.Append("</tbody>");
                        sb.Append("</table>");
                        sb.Append("</div>");
                        sb.Append("</div>");

                        sb.Append("</div>");
                        sb.Append("</div>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // Roles table End
                    }

                    Sn++;
                }
                string branchtable = sb.ToString();
                sb.Clear();
                
                // Designation dropdown
                DataTable Dt2 = UsersDAL.GetDesignations();

                sb.Append("<option value=''>Choose Designation</option>");

                if (Dt2 != null)
                {
                    foreach (DataRow dr1 in Dt2.Rows)
                    {
                        string optionValue = GetValue(dr1, "ID");
                        string optionText = GetValue(dr1, "Name");

                        string selected = optionValue == DesignationID
                            ? "selected"
                            : "";

                        sb.Append(
                            "<option value='" +
                            System.Net.WebUtility.HtmlEncode(optionValue) +
                            "' " + selected + ">" +
                            System.Net.WebUtility.HtmlEncode(optionText) +
                            "</option>"
                        );
                    }
                }

                string designationOptions = sb.ToString();
                sb.Clear();
               
                // Return all required values in JSON
                return Json(new
                {
                    success = true,
                    message = "Success",

                    // User details
                    Id = Id,
                    Active = Active,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
                    Address = Address,
                    EmailID = EmailID,
                    ResidenceNumber = ResidenceNumber,
                    OfficeNumber = OfficeNumber,
                    MobileNumber = MobileNumber,
                    //EmployeeType = EmployeeType,
                    Username = Username,
                    Password = Password,
                    GmailID = GmailID,
                    IsLocationRestrictedUser = IsLocationRestrictedUser,
                    PhotoID = PhotoID,
                    ImagePath = ImagePath,
                    AccountID = AccountID,
                    AccountName = AccountName,
                    // HTML and dropdown options
                    //userrightstable = userrightstable,
                    designationOptions = designationOptions,
                    //role = role,
                    branch = branchtable
                });
            }
            catch (Exception ex)
            {
                // Log the exception using your application's ILogger in production.
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving user details."
                });
            }
        }

        public async Task<IActionResult> DeleteEmployeeDetails(int ID)
        {
            int val = UsersDAL.DeleteEmployeeDetails(ID);
            if (val != 0)
            {
                return Json(new { success = true, message = "Entry deleted successfully" });

            }
            else
            {
                return Json(new { success = false, message = "Unable to delete!" });
            }
        }


        public async Task<IActionResult> Delete(int ID)
        {
            int val = UsersDAL.DeleteUsers(ID);
            if (val != 0)
            {
                return Json(new { success = true, message = "Entry deleted successfully" });

            }
            else
            {
                return Json(new { success = false, message = "Unable to delete!" });
            }
        }

    }
}
