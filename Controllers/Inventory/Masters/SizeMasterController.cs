using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Masters
{
    public class SizeMasterController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public SizeMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }


        private DAL.Inventory.Masters.SizeMaster _SizeMaster;

        private DAL.Inventory.Masters.SizeMaster SizeMaster
        {
            get
            {
                if (_SizeMaster == null)
                {
                    _SizeMaster = new DAL.Inventory.Masters.SizeMaster(ConnectionString);
                }
                return _SizeMaster;
            }
        }


        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);
            DataTable dt = SizeMaster.Fill();
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["Value"].ToString() + "</td>");
                sb.Append("<td>" + dr["Description"].ToString() + "</td>");
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
            ViewBag.SizeMaster = brands;
            ViewBag.MenuID = MenuID;
            return View("~/Views/Invertory/Masters/SizeMaster.cshtml");
        }

        public async Task<IActionResult> NewEntry()
        {
            DataTable dt = SizeMaster.SelectCode();
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


            return Json(new { success = true, nextcode = ViewBag.NextCode });
        }


        public async Task<IActionResult> SaveEntry(MaMisc ma)
        {

            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlTransaction Tx = null;
            String Result = "";
            try
            {
                Con.Open();
                Tx = Con.BeginTransaction();
                if (ma.ID == null || ma.ID == 0)
                {
                    int count = SizeMaster.CheckDuplicateEntry(ma.Value);
                    if (count > 0)
                    {
                        return Json(new { success = false, message = "This Size Already Exists!" });

                    }
                    else
                    {
                        // Value is not a duplicate, return JSON response indicating not a duplicate
                        Result = SizeMaster.InsertSizeMaster(ma, Con, Tx);
                    }
                }
                else
                {
                    Result = SizeMaster.InsertSizeMaster(ma, Con, Tx);
                }

                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Size added", transactionNo = ma.ID });

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

            DataTable Dt = SizeMaster.RowClick(ID);
            DataRow dr = Dt.Rows[0];

            // Check for nulls or empty strings before using them
            string Id = dr["ID"]?.ToString() ?? string.Empty;
            string Code = dr["Code"]?.ToString() ?? string.Empty;
            string Value = dr["Value"]?.ToString() ?? string.Empty;
            string Description = dr["Description"]?.ToString() ?? string.Empty;
            string Active = dr["Active"]?.ToString() ?? string.Empty;



            return Json(new { success = true, id = Id, code = Code, description = Description, active = Active, message = "Success", value = Value });
        }

        public async Task<IActionResult> Delete(int ID)
        {
            int val = SizeMaster.DeleteSizeMaster(ID);
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
