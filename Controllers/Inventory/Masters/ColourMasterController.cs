using ERPSample.Models;
using ERPSample.Models.Inventory.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Masters
{
    public class ColourMasterController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public ColourMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }


        private DAL.Inventory.Masters.ColorMaster _ColorMaster;

        private DAL.Inventory.Masters.ColorMaster ColorMaster
        {
            get
            {
                if (_ColorMaster == null)
                {
                    _ColorMaster = new DAL.Inventory.Masters.ColorMaster(ConnectionString);
                }
                return _ColorMaster;
            }
        }


        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);
            DataTable dt = ColorMaster.Fill();
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
            ViewBag.ColorMaster = brands;
            ViewBag.MenuID = MenuID;
            return View("~/Views/Invertory/Masters/ColorMaster.cshtml");
        }

        //public async Task<IActionResult> NewEntry()
        //{
        //    DataTable dt = ColorMaster.SelectCode();
        //    DataRow row = dt.Rows[0];
        //    string code = row["Code"].ToString();
        //    if (code != "")
        //    {
        //        int nexcode = Convert.ToInt32(code) + 1;
        //        ViewBag.NextCode = nexcode.ToString("D4");
        //    }
        //    else
        //    {
        //        ViewBag.NextCode = "0001";

        //    }


        //    return Json(new { success = true, nextcode = ViewBag.NextCode });
        //}


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
                    int count = ColorMaster.CheckDuplicateEntry(ma.Value);
                    if (count > 0)
                    {
                        return Json(new { success = false, message = "This Color Already Exists!" });

                    }
                    else
                    {
                        // Value is not a duplicate, return JSON response indicating not a duplicate
                        Result = ColorMaster.InsertColorMaster(ma, Con, Tx);
                    }
                }
                else
                {
                    Result = ColorMaster.InsertColorMaster(ma, Con, Tx);
                }

                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Color added", transactionNo = ma.ID });

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

            DataTable Dt = ColorMaster.RowClick(ID);
            DataRow dr = Dt.Rows[0];

            // Check for nulls or empty strings before using them
            string Id = dr["ID"]?.ToString() ?? string.Empty;
            string Code = dr["Code"]?.ToString() ?? string.Empty;
            string Value = dr["Value"]?.ToString() ?? string.Empty;
            string Description = dr["Description"]?.ToString() ?? string.Empty;
            string Active = dr["Active"]?.ToString() ?? string.Empty;

           

            return Json(new { success = true, id = Id, code = Code,description = Description, active = Active, message = "Success",value= Value });
        }

        public async Task<IActionResult> Delete(int ID)
        {
            int val = ColorMaster.DeleteColorMaster(ID);
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
