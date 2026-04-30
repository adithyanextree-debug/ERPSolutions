using ERPSample.DAL.Inventory.Masters;
using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Ecommerce.Masters
{
    public class CategoryMasterController : BaseController
    {
        private DAL.Ecommerce.Masters.CategoryMaster _CategoryMaster;

        private DAL.Ecommerce.Masters.CategoryMaster CategoryMaster
        {
            get
            {
                if (_CategoryMaster == null)
                {
                    _CategoryMaster = new DAL.Ecommerce.Masters.CategoryMaster(ConnectionString);
                }
                return _CategoryMaster;
            }
        }
        private String CategoryImagePath
        {
            get
            {
                return "Resources\\" + TenantID + "\\Ecommerce\\CategoryImage";
            }
        }
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public CategoryMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);
            DataTable dt = CategoryMaster.Fill();
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["Value"].ToString() + "</td>");
                sb.Append("<td>");
                sb.Append(dr["Description"].ToString());
                sb.Append("</td>");
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
            string category = sb.ToString();
            ViewBag.CategoryMaster = category;
            ViewBag.MenuID = MenuID;
            return View("~/Views/Ecommerce/Masters/CategoryMaster.cshtml");
        }

        public async Task<IActionResult> NewEntry()
        {
            DataTable dt = CategoryMaster.SelectCode();
            DataRow row = dt.Rows[0];
            string code = row["code"].ToString();
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

        [HttpPost]
        public async Task<IActionResult> addImage()
        {
            try
            {
                var DefaultValue = 0;
                var files = HttpContext.Request.Form.Files[0];
                string category = (HttpContext.Request.Form["Value"].ToString());

                // Sanitize the category string by replacing invalid characters
                category = category.Replace("/", "_").Replace("\\", "_").Replace(" ", "_");

                if (files != null && files.Length > 0)
                {
                    // Get the path where the images are stored
                    var uploads = Path.Combine(_appEnvironment.WebRootPath, CategoryImagePath);

                    // Count how many images already exist for the given category (i.e., how many files start with category)
                    var existingFiles = Directory.GetFiles(uploads, $"{category}_*.webp");
                    int imageCount = existingFiles.Length;

                    // Create the image name (category_1, category_2, etc.)
                    string ImageName = $"{category}_{imageCount + 1}";  // +1 to start from category_1
                    var fileName = ImageName + ".webp";  // use .webp extension

                    // Save the image to the file system
                    if (files.Length > 0)
                    {
                        string filePath = Path.Combine(uploads, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await files.CopyToAsync(fileStream);
                        }

                        return Json(new { success = true, message = "Image Saved", imagepath = fileName, imagename = fileName });
                    }
                    else
                    {
                        return Json(new { success = false, message = "An error occurred" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No files uploaded" });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
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
                if (ma.ID == 0)
                {
                    int count = CategoryMaster.CheckDuplicateEntry(ma.Value);
                    if (count > 0)
                    {
                        return Json(new { success = false, message = "This Category Already Exists!" });

                    }
                    else
                    {
                        // Value is not a duplicate, return JSON response indicating not a duplicate
                        Result = CategoryMaster.InsertCategory(ma, Con, Tx);
                    }
                }
                else
                {
                    Result = CategoryMaster.InsertCategory(ma, Con, Tx);
                }

                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Category added", transactionNo = ma.ID });

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

            DataTable Dt = CategoryMaster.RowClick(ID, TenantID);
            DataRow dr = Dt.Rows[0];
            // Check for nulls or empty strings before using them
            string Id = dr["ID"]?.ToString() ?? string.Empty;
            string Code = dr["code"]?.ToString() ?? string.Empty;
            string Value = dr["Value"]?.ToString() ?? string.Empty;
            string Description = dr["Description"]?.ToString() ?? string.Empty;
            string ArabicDescription = dr["ArabicDescription"]?.ToString() ?? string.Empty;
            string Active = dr["Active"]?.ToString() ?? string.Empty;
            string imagePath = dr["Image"]?.ToString() ?? string.Empty;

            // Initialize imageName to empty by default
            string imageName = string.Empty;

            // If imagePath is not null or empty, split and extract filename
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                // Split at '?' and take the first part
                string cleanImagePath = imagePath.Split('?')[0];

                // Now extract the file name with extension
                imageName = System.IO.Path.GetFileName(cleanImagePath);
            }
            return Json(new { success = true, id = Id, code = Code, value = Value, description = Description, active = Active, imagePath = imagePath, imageName = imageName,arabicdescription=ArabicDescription, message = "Success" });
        }

        //[10-04-2023] Deleting datas from tables MaMisc while clicking on delete button//

        public async Task<IActionResult> Delete(int ID)
        {
            int val = CategoryMaster.DeleteCategory(ID);
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
