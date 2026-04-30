using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Inventory.Masters
{
    public class BrandMasterController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public BrandMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }


        private DAL.Inventory.Masters.BrandMaster _BrandMaster;

        private DAL.Inventory.Masters.BrandMaster BrandMaster
        {
            get
            {
                if (_BrandMaster == null)
                {
                    _BrandMaster = new DAL.Inventory.Masters.BrandMaster(ConnectionString);
                }
                return _BrandMaster;
            }
        }

        private String BrandImagePath
        {
            get
            {
                return "Resources\\" + TenantID + "\\Ecommerce\\BrandImage";
            }
        }

        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);
            DataTable dt = BrandMaster.Fill();
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
            string brands = sb.ToString();
            ViewBag.BrandMaster = brands;
            ViewBag.MenuID = MenuID;
            return View("~/Views/Invertory/Masters/BrandMaster.cshtml");
        }

        public async Task<IActionResult> NewEntry()
        {
            DataTable dt = BrandMaster.SelectCode();
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
                string brand = (HttpContext.Request.Form["Value"].ToString());

                // Sanitize the brand string by replacing invalid characters
                string brandnname = brand.Replace("/", "_").Replace("\\", "_").Replace(" ", "_").Replace("&", "_").Replace("?", "_");

                if (files != null && files.Length > 0)
                {
                    // Get the path where the images are stored
                    var uploads = Path.Combine(_appEnvironment.WebRootPath, BrandImagePath);

                    // Ensure the directory exists
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads); // Create the directory if it doesn't exist
                    }

                    // Count how many images already exist for the given brand (i.e., how many files start with brand)
                    var existingFiles = Directory.GetFiles(uploads, $"{brandnname}_*.webp");
                    int imageCount = existingFiles.Length;

                    // Create the image name (brand_1, brand_2, etc.)
                    string ImageName = $"{brandnname}_{imageCount + 1}";  // +1 to start from brand_1
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
            //int id = BrandMaster.InsertBrand(ma);
            //return Json(new { success = true });


            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlTransaction Tx = null;
            String Result = "";
            try
            {
                Con.Open();
                Tx = Con.BeginTransaction();
                if (ma.ID == 0)
                {
                    int count = BrandMaster.CheckDuplicateEntry(ma.Value);
                    if (count > 0)
                    {
                        return Json(new { success = false ,message = "This Brand Already Exists!" });

                    }
                    else
                    {
                        // Value is not a duplicate, return JSON response indicating not a duplicate
                        Result = BrandMaster.InsertBrand(ma, Con, Tx);
                    }
                }
                else
                {
                    Result = BrandMaster.InsertBrand(ma, Con, Tx);
                }
                
                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Brand added", transactionNo = ma.ID });

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

            DataTable Dt = BrandMaster.RowClick(ID,TenantID);
            DataRow dr = Dt.Rows[0];
            // Check for nulls or empty strings before using them
            string Id = dr["ID"]?.ToString() ?? string.Empty;
            string Code = dr["code"]?.ToString() ?? string.Empty;
            string Value = dr["Value"]?.ToString() ?? string.Empty;
            string Description = dr["Description"]?.ToString() ?? string.Empty;
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
            return Json(new { success = true, id = Id, code = Code, value = Value, description = Description, active = Active, imagePath = imagePath,imageName= imageName, message = "Success" });
        }

        //[10-04-2023] Deleting datas from tables MaMisc while clicking on delete button//

        public async Task<IActionResult> Delete(int ID)
        {
            int val = BrandMaster.DeleteBrand(ID);
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
