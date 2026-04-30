using ERPSample.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERPSample.Controllers
{
    public class LoginController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public LoginController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }
        public IActionResult Index()
        {
            LoginModel loginModel = new LoginModel();
            List<LoginCompaniesModel> modelList = new List<LoginCompaniesModel>();
            LoginCompaniesModel loginCompaniesModel;
            DataTable dtCompanies = new DAL.General.Common.Permission().GetCompaniesFromMaster();
            foreach (DataRow dr in dtCompanies.Rows)
            {
                loginCompaniesModel = new LoginCompaniesModel();
                loginCompaniesModel.ID = Convert.ToInt64(dr["ID"]);
                loginCompaniesModel.Name = dr["Name"].ToString();
                modelList.Add(loginCompaniesModel);
            }
            loginModel.loginCompaniesModels = modelList;
            //ViewBag.BaseUrl = baseUrl; // Assign the base URL to ViewBag

            return View("~/Views/Login.cshtml", loginModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches(Int64 CompanyID)
        {
            List<LoginCompaniesModel> modelList = new List<LoginCompaniesModel>();
            DataTable dtMaster = new DAL.General.Common.Permission().GetCompanyConnectionDetails(CompanyID);
            if (dtMaster.Rows.Count > 0)
            {
                String ConnectionString;
                DataRow dr = dtMaster.Rows[0];
                ConnectionString = "Data Source=" + dr["ServerName"].ToString() + ";Initial Catalog=" + dr["DatabaseName"].ToString() + ";User ID=nextree;Password=Nextree@4313$;TrustServerCertificate=True;";

                DataTable dtCompanies = new DAL.General.Common.Permission(ConnectionString).GetBranches();
                LoginModel loginModel = new LoginModel();
                LoginCompaniesModel loginCompaniesModel;
                foreach (DataRow drBranch in dtCompanies.Rows)
                {
                    loginCompaniesModel = new LoginCompaniesModel();
                    loginCompaniesModel.ID = Convert.ToInt64(drBranch["ID"]);
                    loginCompaniesModel.Name = drBranch["Company"].ToString();
                    modelList.Add(loginCompaniesModel);
                }
                loginModel.loginCompaniesModels = modelList;
            }
            return Json(new { success = true, branchlist = modelList });
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate(LoginShortModel loginShortModel)
        {
            Boolean IsOk = false;
            if (loginShortModel.Username != "")
            {
                DataTable dtMaster = new DAL.General.Common.Permission().GetCompanyConnectionDetails(loginShortModel.CompanyID);
                if (dtMaster.Rows.Count > 0)
                {
                    String ConnectionString;
                    DataRow dr = dtMaster.Rows[0];
                    ConnectionString = "Data Source=" + dr["ServerName"].ToString() + ";Initial Catalog=" + dr["DatabaseName"].ToString() + ";User ID=nextree;Password=Nextree@4313$;TrustServerCertificate=True;";
                    DataTable dtUserAuthentication = new DAL.General.Common.Permission(ConnectionString).GetUserAuthentication(loginShortModel.Username, BLL.Utilities.Configuration.Encrypt(loginShortModel.Password), loginShortModel.BranchID);
                    if (dtUserAuthentication.Rows.Count > 0)
                    {
                        UserInfo objUserInfo = new UserInfo(ConnectionString, SharedClass.MasterConnectionString, loginShortModel.Username, loginShortModel.Username, Convert.ToInt32(loginShortModel.CompanyID), "", DateTime.Now, "", Convert.ToInt64(dtUserAuthentication.Rows[0]["EmployeeID"]), loginShortModel.BranchID);
                        HttpContext.Session.SetComplexData("UserInfo", objUserInfo);
                        IsOk = true;
                    }
                    CreateResourceDirectory(loginShortModel.CompanyID);
                }
            }
            return Json(new { success = IsOk });
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all session and cookies
            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index", "Login");
        }


        public void CreateResourceDirectory(Object TenantID)
        {
            try
            {
                String webpath = Path.Combine(_appEnvironment.WebRootPath, "Resources", TenantID.ToString());
                DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(webpath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                    directoryInfo = new DirectoryInfo(Path.Combine(webpath, "Ecommerce"));
                    directoryInfo.Create();
                    directoryInfo.CreateSubdirectory("BannerImage");
                    directoryInfo.CreateSubdirectory("CategoryDesc");
                    directoryInfo.CreateSubdirectory("CategoryImage");
                    directoryInfo.CreateSubdirectory("ItemImage");
                    directoryInfo.CreateSubdirectory("ItemDesc");
                    directoryInfo = new DirectoryInfo(Path.Combine(webpath, "VoucherDocuments"));
                    directoryInfo.Create();
                }
            }
            catch
            {
            }
        }
    }
}
