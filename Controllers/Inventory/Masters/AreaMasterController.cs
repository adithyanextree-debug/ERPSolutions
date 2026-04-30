using ERPSample.Models;
using ERPSample.Models.Inventory.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Masters
{
    public class AreaMasterController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public AreaMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }


        private DAL.Inventory.Masters.InvAreaMaster _InvAreaMaster;

        private DAL.Inventory.Masters.InvAreaMaster InvAreaMaster
        {
            get
            {
                if (_InvAreaMaster == null)
                {
                    _InvAreaMaster = new DAL.Inventory.Masters.InvAreaMaster(ConnectionString);
                }
                return _InvAreaMaster;
            }
        }

     
        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);
            DataTable dt = InvAreaMaster.Fill();
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["Description"].ToString() + "</td>");
                //sb.Append("<td>");
                //sb.Append(dr["ArDescription"].ToString());
                //sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse'>");
                sb.Append("<input class='form-check-input' role='switch'");
                sb.Append("name='Active' type='checkbox' ");
                if (Convert.ToBoolean(dr["Active"]))
                {
                    sb.Append("checked ");
                }
                sb.Append(">");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"].ToString() + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("</tr>");
                count++;
            }
            string brands = sb.ToString();
            ViewBag.AreaMaster = brands;
            ViewBag.MenuID = MenuID;
            return View("~/Views/Invertory/Masters/InvAreaMaster.cshtml");
        }

        public async Task<IActionResult> NewEntry()
        {
            DataTable dt = InvAreaMaster.SelectCode();
            DataRow row = dt.Rows[0];
            string code = row["Code"].ToString();
            if (code != "")
            {
                int nexcode = Convert.ToInt32(code) + 1;
                ViewBag.NextCode = nexcode.ToString("D4");
            }
            else
            {
                ViewBag.NextCode = "0001";

            }
            DataTable Dt2 = InvAreaMaster.GetStateDropdown();

            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>Choose State</option>");  // Default option

            foreach (DataRow dr1 in Dt2.Rows)
            {
                // Get the value for the option tag and the text for the option
                string optionValue = dr1["ID"].ToString();
                string optionText = dr1["Value"].ToString();

                // Append the option to the StringBuilder without 'selected' attribute
                sb.Append($"<option value='{optionValue}'>{optionText}</option>");
            }

            return Json(new { success = true, nextcode = ViewBag.NextCode,state = sb.ToString() });
        }
       

        public async Task<IActionResult> SaveEntry(InvAreaMaster ma)
        {

            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlTransaction Tx = null;
            String Result = "";
            try
            {
                Con.Open();
                Tx = Con.BeginTransaction();
                if (ma.ID == null || ma.ID == 0 )
                {
                    int count = InvAreaMaster.CheckDuplicateEntry(ma.Description);
                    if (count > 0)
                    {
                        return Json(new { success = false, message = "This Area Already Exists!" });

                    }
                    else
                    {
                        // Value is not a duplicate, return JSON response indicating not a duplicate
                        Result = InvAreaMaster.InsertInvAreaMaster(ma, Con, Tx);
                    }
                }
                else
                {
                    Result = InvAreaMaster.InsertInvAreaMaster(ma, Con, Tx);
                }

                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Area added", transactionNo = ma.ID });

            }
            catch (SqlException sqlEx)
            {
                if (Tx != null)
                {
                    Tx.Rollback();
                    Con.Close();
                }

                // Other SQL errors
                return Json(new { success = false, message = "A database error occurred: " + sqlEx.Message });
            }
            catch (Exception Ex)
            {
                if (Tx != null)
                {
                    Tx.Rollback();
                    Con.Close();
                }
                return Json(new { success = false, message = "An error occurred: " + Ex.Message });
            }


        }

        public async Task<IActionResult> RowClick(int ID)
        {

            DataTable Dt = InvAreaMaster.RowClick(ID);
            DataRow dr = Dt.Rows[0];

            // Check for nulls or empty strings before using them
            string Id = dr["ID"]?.ToString() ?? string.Empty;
            string StateID = dr["StateID"]?.ToString() ?? string.Empty;
            string Code = dr["Code"]?.ToString() ?? string.Empty;
            string Description = dr["Description"]?.ToString() ?? string.Empty;
            string ArDescription = dr["ArDescription"]?.ToString() ?? string.Empty;
            string Active = dr["Active"]?.ToString() ?? string.Empty;

            DataTable Dt2 = InvAreaMaster.GetStateDropdown();

            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>Choose State</option>");  // Default option

            foreach (DataRow dr1 in Dt2.Rows)
            {
                // Get the value for the option tag and the text for the option
                string optionValue = dr1["ID"].ToString();
                string optionText = dr1["Value"].ToString();

                // Check if the current StateID matches the ID in Dt2, and set the selected attribute
                string selected = (optionValue == StateID) ? "selected" : "";

                // Append the option to the StringBuilder
                sb.Append($"<option value='{optionValue}' {selected}>{optionText}</option>");
            }

            return Json(new { success = true, id = Id, code = Code, state = sb.ToString(), description = Description,ardescription=ArDescription, active = Active,  message = "Success" });
        }

        //[10-04-2023] Deleting datas from tables MaMisc while clicking on delete button//

        public async Task<IActionResult> Delete(int ID)
        {
            int val = InvAreaMaster.DeleteInvAreaMaster(ID);
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
