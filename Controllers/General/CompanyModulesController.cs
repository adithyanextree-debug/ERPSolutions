using ERPSample.DAL.General.Masters;
using ERPSample.DAL.General.Users;
using ERPSample.Models.General.Masters;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.General
{
    public class CompanyModulesController : BaseController
    {

        private DAL.General.Masters.CompanyModulesDAL _CompanyModulesDAL;

        private DAL.General.Masters.CompanyModulesDAL CompanyModulesDAL
        {
            get
            {
                if (_CompanyModulesDAL == null)
                {
                    _CompanyModulesDAL = new DAL.General.Masters.CompanyModulesDAL(ConnectionString);
                }
                return _CompanyModulesDAL;
            }
        }
        public async Task<IActionResult> Index(int MenuID)
        {
            try
            {
                SetUserPermissions(MenuID);

                DataTable dt = CompanyModulesDAL.Fill();

                StringBuilder sb = new StringBuilder();
                int count = 1;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>" + count + "</td>");
                        sb.Append("<td>");
                        sb.Append(System.Net.WebUtility.HtmlEncode(dr["Company"]?.ToString() ?? ""));
                        sb.Append("</td>");
                        sb.Append("<td>" + System.Net.WebUtility.HtmlEncode(dr["ModuleCount"]?.ToString() ?? "") + "</td>");
                        sb.Append("<td><ul class='action'>");
                        int CompanyID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);

                        sb.Append("<li class='edit' onclick='RowClick(" + CompanyID + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                        sb.Append("</ul>");
                        sb.Append("</td>");
                        sb.Append("</tr>");

                        count++;
                    }
                }

                string cm = sb.ToString();
                ViewBag.CM = cm;
                ViewBag.MenuID = MenuID;

                return View("~/Views/General/CompanyModules.cshtml");
            }
            catch (Exception ex)
            {
                // Log the exception here if ILogger is available.
                ViewBag.ErrorMessage = "An error occurred while loading users.";
                ViewBag.Users = "";
                ViewBag.MenuID = MenuID;

                return View("~/Views/General/CompanyModules.cshtml");
            }
        }

        public async Task<IActionResult> NewEntry()
        {
            try
            {
                DataSet Ds = CompanyModulesDAL.GetCompanyandModule();

                StringBuilder sb = new StringBuilder();
                sb.Append("<option value=''>Choose Company</option>");

                if (Ds.Tables[0] != null && Ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in Ds.Tables[0].Rows)
                    {
                        string optionValue = System.Net.WebUtility.HtmlEncode(dr1["ID"]?.ToString() ?? "");
                        string optionText = System.Net.WebUtility.HtmlEncode(dr1["Company"]?.ToString() ?? "");

                        sb.Append($"<option value='{optionValue}'>{optionText}</option>");
                    }
                }

                string company = sb.ToString();
                sb.Clear();
                foreach (DataRow dr in Ds.Tables[1].Rows)
                {
                    string moduleID = System.Net.WebUtility.HtmlEncode(dr["ID"]?.ToString() ?? "");
                    string moduleName = System.Net.WebUtility.HtmlEncode(dr["Module"]?.ToString() ?? "");

                    sb.Append("<div class='col-md-3 module-item'>");
                    sb.Append("<div class='form-check'>");
                    sb.Append("<input class='form-check-input ModuleCheck' type='checkbox' value='" + moduleID + "' id='Module" + moduleID + "' onchange='UpdateModuleCount()'>");
                    sb.Append("<label class='form-check-label' for='Module" + moduleID + "'>");
                    sb.Append(moduleName);
                    sb.Append("</label>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                }
                string modules = sb.ToString();
                sb.Clear();
                return Json(new
                {
                    success = true,
                    company = company,
                    modules = modules
                });
            }
            catch (Exception ex)
            {
                // Log the exception here if ILogger is available.
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while loading company and modules."
                });
            }
        }

        public async Task<IActionResult> SaveEntry([FromBody] CompanyModulesModel model)
        {
            try
            {
                string result = CompanyModulesDAL.InsertCompanyModules(model);

                return Json(new
                {
                    success = true,
                    message = "Company modules saved successfully.",
                    newID = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> RowClick(int ID)
        {
            try
            {
                DataTable Dt = CompanyModulesDAL.RowClick(ID);
                DataSet Ds = CompanyModulesDAL.GetCompanyandModule();

                int selectedCompanyID = Dt.Rows.Count > 0 && Dt.Rows[0]["ID"] != DBNull.Value
                    ? Convert.ToInt32(Dt.Rows[0]["ID"])
                    : 0;

                // Build Company dropdown
                StringBuilder sb = new StringBuilder();
                sb.Append("<option value=''>Choose Company</option>");

                if (Ds.Tables[0] != null && Ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in Ds.Tables[0].Rows)
                    {
                        string optionValue = System.Net.WebUtility.HtmlEncode(dr1["ID"]?.ToString() ?? "");
                        string optionText = System.Net.WebUtility.HtmlEncode(dr1["Company"]?.ToString() ?? "");

                        bool isSelected = dr1["ID"] != DBNull.Value &&
                                          Convert.ToInt32(dr1["ID"]) == selectedCompanyID;

                        sb.Append("<option value='" + optionValue + "' " + (isSelected ? "selected" : "") + ">");
                        sb.Append(optionText);
                        sb.Append("</option>");
                    }
                }

                string company = sb.ToString();
                sb.Clear();

                // Build all Module checkboxes from Dataset
                if (Ds.Tables[1] != null && Ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in Ds.Tables[1].Rows)
                    {
                        string moduleID = System.Net.WebUtility.HtmlEncode(dr["ID"]?.ToString() ?? "");
                        string moduleName = System.Net.WebUtility.HtmlEncode(dr["Module"]?.ToString() ?? "");

                        bool isChecked = Dt.AsEnumerable().Any(row =>
                            row["ModuleID"] != DBNull.Value &&
                            row["ModuleID"].ToString() == moduleID);

                        sb.Append("<div class='col-md-3 module-item'>");
                        sb.Append("<div class='form-check'>");
                        sb.Append("<input class='form-check-input ModuleCheck' type='checkbox' value='" + moduleID + "' id='Module" + moduleID + "' " + (isChecked ? "checked" : "") + " onchange='UpdateModuleCount()'>");
                        sb.Append("<label class='form-check-label' for='Module" + moduleID + "'>");
                        sb.Append(moduleName);
                        sb.Append("</label>");
                        sb.Append("</div>");
                        sb.Append("</div>");
                    }
                }

                string modules = sb.ToString();

                return Json(new
                {
                    success = true,
                    company = company,
                    modules = modules,id = selectedCompanyID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while loading company and modules."
                });
            }
        }

        public async Task<IActionResult> Delete(int ID)
        {
            int val = CompanyModulesDAL.DeleteCompanyModule(ID);
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
