using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Primitives;
using NextreeWebControls.Models;
using ERPSample;
using ERPSample.Models.Common;

namespace ERPSample.Controllers
{
    public class BaseController : Controller
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    base.OnActionExecuting(context);
        //    Controller = RouteData.Values.Values.ToList()[0].ToString();
        //    UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
        //    if (objUserInfo == null)
        //    {
        //       // String _ConnectionString = "Data Source=HP\\SQLEXPRESS;Initial Catalog=NextreeSystemMAIN;User ID=nextree;Password=Nextree@4313$;TrustServerCertificate=True;";
        //        objUserInfo = new UserInfo(SharedClass.MainConnectionString, SharedClass.MasterConnectionString,"user", "user", 1, "", DateTime.Now, "", 93, 1);
        //        HttpContext.Session.SetComplexData("UserInfo", objUserInfo);
        //      //  context.Result = new RedirectResult(Url.Action("Index", "Login"));

        //    }
        //    //else
        //    //{
        //        if (Language == "Arabic")
        //        {
        //            ViewBag.Language = "smart-rtl";
        //        }
        //        ViewBag.LoggedIn = String.Format("{0:dd-MM-yyyy hh:mm tt}", objUserInfo.LoginTime);
        //        ViewBag.ServerDateTime = String.Format("{0:dd-MM-yyyy hh:mm tt}", DateTime.Now);// GetServerTime());
        //        ViewBag.Username = objUserInfo.Username;
        //        ViewBag.Name = objUserInfo.Name;
        //        ViewBag.Connectivity = objUserInfo.TenantName;
        //        ViewBag.Controller = Controller;
        //        //ViewBag.DecimalPoint = Settings.DefaultDecimal;
        //        //UserLog
        //        if (RouteData.Values.Values.ToList()[1].ToString() == "Index")
        //        {
        //            //if (Controller != "MiscMaster" && Controller != "index" && Controller != "FavouriteMenu" && Controller != "Voucher" && Controller != "Notes")
        //            if (DoUpdateLogForController())
        //            {
        //                //new SmartSuiteDAL.BaseDAL(ConnectionString).UpdateLog(Controller, objUserInfo.Username, 10);
        //            }
        //        }
        //    //}
        //}

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Controller = RouteData.Values.Values.ToList()[0].ToString();
            UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
            if (objUserInfo == null)
            {
                context.Result = new RedirectResult(Url.Action("Index", "Login"));
            }
            else
            {
                if (Language == "Arabic")
                {
                    ViewBag.Language = "smart-rtl";
                }
                ViewBag.LoggedIn = String.Format("{0:dd-MM-yyyy hh:mm tt}", objUserInfo.LoginTime);
                ViewBag.ServerDateTime = String.Format("{0:dd-MM-yyyy hh:mm tt}", DateTime.Now);// GetServerTime());
                ViewBag.Username = objUserInfo.Username;
                ViewBag.Name = objUserInfo.Name;
                ViewBag.Connectivity = objUserInfo.TenantName;
                ViewBag.Controller = Controller;
                //ViewBag.DecimalPoint = Settings.DefaultDecimal;
                //UserLog
                if (RouteData.Values.Values.ToList()[1].ToString() == "Index")
                {
                    //if (Controller != "MiscMaster" && Controller != "index" && Controller != "FavouriteMenu" && Controller != "Voucher" && Controller != "Notes")
                    if (DoUpdateLogForController())
                    {
                        //new SmartSuiteDAL.BaseDAL(ConnectionString).UpdateLog(Controller, objUserInfo.Username, 10);
                    }
                }
            }
        }
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    base.OnActionExecuting(context);
        //    HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //    HttpContext.Response.Headers["Pragma"] = "no-cache";
        //    HttpContext.Response.Headers["Expires"] = "0";
        //    HttpContext.Response.Headers["Referrer-Policy"] = "no-referrer";
        //  //  HttpContext.Response.Headers["X-Frame-Options"] = "SAMEORIGIN"; // Optional: block iframe access
        //    HttpContext.Response.Headers["X-Content-Type-Options"] = "nosniff";


        //    Controller = RouteData.Values.Values.ToList()[0].ToString();
        //    UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
        //    if (objUserInfo == null)
        //    {
        //        context.Result = new RedirectResult(Url.Action("Index", "Login"));
        //    }
        //    else
        //    {
        //        if (Language == "Arabic")
        //        {
        //            ViewBag.Language = "smart-rtl";
        //        }
        //        ViewBag.LoggedIn = String.Format("{0:dd-MM-yyyy hh:mm tt}", objUserInfo.LoginTime);
        //        ViewBag.ServerDateTime = String.Format("{0:dd-MM-yyyy hh:mm tt}", DateTime.Now);// GetServerTime());
        //        ViewBag.Username = objUserInfo.Username;
        //        ViewBag.Name = objUserInfo.Name;
        //        ViewBag.Connectivity = objUserInfo.TenantName;
        //        ViewBag.Controller = Controller;
        //        //ViewBag.DecimalPoint = Settings.DefaultDecimal;
        //        //UserLog
        //        if (RouteData.Values.Values.ToList()[1].ToString() == "Index")
        //        {
        //            //if (Controller != "MiscMaster" && Controller != "index" && Controller != "FavouriteMenu" && Controller != "Voucher" && Controller != "Notes")
        //            if (DoUpdateLogForController())
        //            {
        //                //new SmartSuiteDAL.BaseDAL(ConnectionString).UpdateLog(Controller, objUserInfo.Username, 10);
        //            }
        //        }
        //    }
        //}

        public void SetUserPermissions(Int64 PageID)
        {
            this.PageID = PageID;
            UserPermission _UserPermissions = new UserPermission();
            _UserPermissions.SetPermissions(new DAL.General.Common.Permission(ConnectionString).GetPermissions(UserID, PageID, BranchID));
            UserPermissions = _UserPermissions;
        }
        private UserPermission _UserPermissions;
        public UserPermission UserPermissions
        {
            get
            {
                return ViewBag.UserPermissions;
            }
            set
            {
                ViewBag.UserPermissions = value;
            }
        }


        private Boolean DoUpdateLogForController()
        {
            switch (Controller)
            {
                case "CompanySwitch":
                    return false;
                case "MiscMaster":
                    return false;
                case "index":
                    return false;
                case "FavouriteMenu":
                    return false;
                case "Voucher":
                    return false;
                case "Notes":
                    return false;
                default:
                    return true;
            }
        }

        private String _Controller;
        /// <summary>
        /// Returns Controller Name from Base class
        /// </summary>        /// 
        public String Controller
        {
            get
            {
                //return RouteData.Values.Values.ToList()[0].ToString();
                //return this.ControllerContext.RouteData.Values["controller"].ToString();
                return _Controller;
            }
            set
            {
                _Controller = value;
            }

        }

        public String ControllerName(ControllerContext controllerContext)
        {

            //return RouteData.Values.Values.ToList()[0].ToString();
            return controllerContext.RouteData.Values["controller"].ToString();

        }
        public void ChangeToEnglish()
        {
            UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
            objUserInfo.Language = "English";
            @ViewBag.Language = "";
            HttpContext.Session.SetComplexData("UserInfo", objUserInfo);
        }
        public void ChangeToArabic()
        {
            UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
            objUserInfo.Language = "Arabic";
            @ViewBag.Language = "smart-rtl";
            HttpContext.Session.SetComplexData("UserInfo", objUserInfo);
        }
        public void SetMenuCompanyID(int MenuID, int CompanyID)
        {
            _MenuID = MenuID;
            //_CompanyID = CompanyID;
        }
        public int _MenuID
        {
            get
            {
                return ViewBag.MenuID;
            }
            set
            {
                if (ViewBag.MenuID == null)
                {
                    ViewBag.MenuID = value;
                }
            }
        }
        private Object _PageID;
        public Object PageID
        {
            get
            {
                return _PageID;
            }
            set
            {
                _PageID = value;
                ViewBag.PageID = value;
            }
        }
        //private Object _CompanyID;
        public Object CompanyID
        {
            get
            {
                //return _CompanyID;
                return ViewBag.CompanyID;
            }
            set
            {
                //_CompanyID = value;
                ViewBag.CompanyID = value;
            }
        }
        public Object IsNull(Object source1, Object source2 = null)
        {
            if (source1 == null || source1 == DBNull.Value || source1.ToString() == "")
            {
                if (source2 != null && source2 != DBNull.Value)
                {
                    return source2;
                }
            }
            return source1;
        }
        public Decimal IsNullToDecimal(Object source1)
        {
            if (source1 == null || source1 == DBNull.Value || source1.ToString() == "")
            {
                return 0;
            }
            return Convert.ToDecimal(source1);
        }
        public Boolean IsNull(Object source)
        {
            if (source == null || source == DBNull.Value || source.ToString() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public String Username
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.Username;
            }
        }
        public String TenantName
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.TenantName;
            }
        }
        public int TenantID
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.TenantID;
            }
        }
        public String Language
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.Language;
            }
        }
        public String ConnectionString
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.ConnectionString;
            }
        }
        public String MainConnectionString
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.MainConnectionString;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLookupLists(Models.Common.LookupModel lookupModel)
        {
            DataTable dt = new DAL.General.Common.CommandText(ConnectionString).GetTable(lookupModel);
            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                sb.Append("<table class='lookup table table-bordered table-responsive'>");

                sb.Append("<thead>");
                sb.Append("<tr>");
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName != lookupModel.IDColumnName) // Skip ID column
                    {
                        sb.Append("<th>");
                        sb.Append(dc.ColumnName);
                        sb.Append("</th>");
                    }
                }
                sb.Append("</tr>");
                sb.Append("</thead>");

                sb.Append("<tbody>");
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr ondblclick='LookupDoubleClick(");
                    sb.Append('"'); sb.Append(lookupModel.LookupID); sb.Append('"'); sb.Append(",");
                    sb.Append('"'); sb.Append(lookupModel.LookupDIV); sb.Append('"'); sb.Append(",");
                    sb.Append(dr[lookupModel.IDColumnName]); sb.Append(",");
                    sb.Append('"'); sb.Append(dr[lookupModel.AssignColumnName]); sb.Append('"');
                    sb.Append(")'>");

                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName != lookupModel.IDColumnName) // Skip ID column
                        {
                            sb.Append("<td>");
                            sb.Append(dr[dc.ColumnName]);
                            sb.Append("</td>");
                        }
                    }

                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
            }
            else
            {
                sb.Append("Sorry, no record(s) found!!");
            }
                return Json(new { success = true, lookupHTML = sb.ToString() });
        }

        //public SmartSuiteDAL.Settings Settings
        //{
        //    get
        //    {
        //        UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
        //        if (objUserInfo.Settings == null)
        //        {
        //            objUserInfo.Settings = new SmartSuiteDAL.Settings(objUserInfo.ConnectionString, objUserInfo.Username);
        //        }
        //        return objUserInfo.Settings;
        //    }
        //}        
        public String IPAddress
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.IPAddress;
            }
            set
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                objUserInfo.IPAddress = value;
                HttpContext.Session.SetComplexData("UserInfo", objUserInfo);
            }
        }
        public Int64 BranchID
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.BranchID;
            }
        }
        public Int64 UserID
        {
            get
            {
                UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
                return objUserInfo.UserID;
            }
        }

        //private PermissionClass PermissionClass;
        //public PermissionClass Permissions
        //{
        //    get
        //    {
        //        if (PermissionClass == null)
        //        {
        //            PermissionClass = new PermissionClass(Username, Controller, CompanyID, ConnectionString, PageID);
        //        }
        //        return PermissionClass;
        //    }
        //}
        //public void SetPermissions()
        //{
        //    ViewBag.ViewPermission = Permissions.View;
        //    ViewBag.NewPermission = Permissions.New;
        //    ViewBag.EditPermission = Permissions.Edit;
        //    ViewBag.DeletePermission = Permissions.Delete;
        //    ViewBag.CancelPermission = Permissions.Cancel;
        //    ViewBag.PrintPermission = Permissions.Print;
        //    ViewBag.EmailPermission = Permissions.Email;
        //    ViewBag.ExcelPermission = Permissions.Excel;
        //    ViewBag.PdfPermission = Permissions.Pdf;
        //    ViewBag.ApprovalPermission = Permissions.Approval;

        //    ViewBag.SmartUser = SmartUser;
        //}

        //public void LoadDisplayTexts()
        //{
        //    DataTable dataTable = new SmartSuiteDAL.Options.clsFields(ConnectionString).GetDisplayTexts(Language, Controller, Username, PageID);
        //    Hashtable DisplayTextsTable = new Hashtable();
        //    foreach (DataRow dr in dataTable.Rows)
        //    {
        //        var values = new HashTableFields() { FieldText = dr["FieldText"].ToString(), Visible = Convert.ToBoolean(dr["Visible"]) };
        //        if (!DisplayTextsTable.ContainsKey(dr["FieldName"]))
        //        {
        //            DisplayTextsTable.Add(dr["FieldName"].ToString(), values);
        //        }
        //    }
        //    ViewBag.DisplayTextsTable = DisplayTextsTable;
        //}

        public IActionResult New(String ID, String MenuID, String CompanyID)
        {
            return Json(Url.Action("Create", Controller, new { ID, MenuID, CompanyID }));
        }
        public IActionResult Back(String ID, String MenuID)
        {
            return Json(Url.Action("Index", Controller, new { ID, MenuID, CompanyID }));
        }

        //public String DisplayText(String FieldName)
        //{
        //    try
        //    {
        //        if (ViewBag.DisplayTextsTable == null)
        //        {
        //            LoadDisplayTexts();
        //        }
        //        foreach (DictionaryEntry de in ViewBag.DisplayTextsTable)
        //        {
        //            if (de.Key.ToString() == FieldName)
        //            {
        //                return (de.Value as HashTableFields).FieldText;
        //            }
        //        }
        //        return FieldName;

        //    }
        //    catch (Exception)
        //    {
        //        return FieldName;
        //    }
        //}

        //public String Format(Object Value)
        //{
        //    return SmartSuiteBLL.BuisinessLogics.NumericFormat(Value);
        //}

        //public Boolean FieldVisible(String FieldName)
        //{
        //    try
        //    {
        //        if (ViewBag.DisplayTextsTable == null)
        //        {
        //            LoadDisplayTexts();
        //        }
        //        foreach (DictionaryEntry de in ViewBag.DisplayTextsTable)
        //        {
        //            if (de.Key.ToString() == FieldName)
        //            {
        //                return (de.Value as HashTableFields).Visible;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return true;
        //    }
        //}

        /*
         public JsonResult GetValidationFields(String ControllerName)
         {
             List<SelectListItem> lst = new List<SelectListItem>();
             DataTable dataTable = new SmartSuiteDAL.BaseDAL(ConnectionString).GetValidationFields(ControllerName, Language);
             foreach (DataRow dr in dataTable.Rows)
             {
                 lst.Add(new SelectListItem { Text = dr["FieldName"].ToString(), Value = dr["ValidationMsg"].ToString() });
             }
             return Json(new SelectList(lst, "Value", "Text"));
         }
         /// <summary>
         /// 14-01-2019
         /// </summary>
         /// <param name="ex"></param>
         public void LoadDisplayValidationTexts()
         {
             DataTable dataTable = new SmartSuiteDAL.Options.clsFields(ConnectionString).GetDisplayValidationTexts(Language, Controller, Username);
             Hashtable DisplayValidationTextsTable = new Hashtable();
             foreach (DataRow dr in dataTable.Rows)
             {
                 var values = new HashTableFields() { FieldText = dr["ValidationMsg"].ToString(), Visible = Convert.ToBoolean(dr["Visible"]) };
                 if (!DisplayValidationTextsTable.ContainsKey(dr["FieldName"]))
                 {
                     DisplayValidationTextsTable.Add(dr["FieldName"].ToString().Trim(), values);
                 }
             }
             ViewBag.DisplayValidationTextsTable = DisplayValidationTextsTable;
         }
         public String DisplayValidationText(String FieldName)
         {
             try
             {
                 if (ViewBag.DisplayValidationTextsTable == null)
                 {
                     LoadDisplayValidationTexts();
                 }
                 foreach (DictionaryEntry de in ViewBag.DisplayValidationTextsTable)
                 {
                     if (de.Key.ToString() == FieldName)
                     {
                         return (de.Value as HashTableFields).FieldText;
                     }
                 }
                 return FieldName;

             }
             catch (Exception)
             {
                 return FieldName;
             }
         }
         /// <summary>
         /// 17-01-2019
         /// </summary>
         /// <param name="ex"></param>
         ///  
         public JsonResult GethelpFields(String ControllerName)
         {
             List<SelectListItem> lst = new List<SelectListItem>();
             DataTable dataTable = new SmartSuiteDAL.BaseDAL(ConnectionString).GetHelpFields(Language, ControllerName);
             foreach (DataRow dr in dataTable.Rows)
             {
                 lst.Add(new SelectListItem { Text = dr["FieldName"].ToString(), Value = dr["FieldValue"].ToString() });
             }
             return Json(new SelectList(lst, "Text", "Value"));
         }
         public void HandleHRException(Exception ex)
         {
             ViewBag.Message = new SmartSuiteDAL.BaseDAL(ConnectionString).HandleException(ex, Language, Username, PageID, Controller);
         }
         // to get print/excel/mail into update useractivity log
         public IActionResult UpdateActivityLog([FromBody]SmartSuiteModels.Options.UserLog userLog)
         {
             //UserLog
             //new SmartSuiteDAL.BaseDAL(ConnectionString).UpdateLog(userLog.Username, userLog.ActionID, userLog.CompanyID, userLog.PageID, "", "", "", userLog.Remarks);
             return Json(new { value = true });
         }
         // get Controller name and action for ajax call.
         public JsonResult GetAjaxRedirect(Int32 MenuID)
         {
             DataTable dataTable = new SmartSuiteDAL.BaseDAL(ConnectionString).GetRedirect(Language, MenuID);
             return Json(new { message = "success", result = JsonConvert.SerializeObject(dataTable, Formatting.Indented) });
             //return Json(new { message = "success", result = dataTable.Rows[0] });
         }

         //public IActionResult SmartTextBoxController(String QryString, String IdKey, String IdValue, String Text, String AssignText, String SearchString, String UniqueID, String InputList, String ExcludeColumns = "")
         //{
         //    if (Text == null) Text = "";

         //    return ViewComponent("SmartTextBox", new { QryString = QryString, IdKey = IdKey, IdValue = IdValue, Text = Text, AssignText = AssignText, SearchString = SearchString, UniqueID = UniqueID, InputList = InputList, ExcludeColumns = ExcludeColumns });
         //}

         public IActionResult SmartTextBoxController(String Criteria = "", String SearchString = "", String UniqueID = "", String ExcludeColumns = "", String CompanyID = "", String[] CreteriaFields = null, String InputList = null, String IdKey = "", Boolean ReadOnly = false)
         {
             return ViewComponent("SmartTextBox", new { Criteria = Criteria, IdKey = IdKey, IdValue = "", Text = "", AssignText = "", SearchString = SearchString, UniqueID = UniqueID, InputList = InputList, ExcludeColumns = ExcludeColumns, CreteriaFields = CreteriaFields, ReadOnly = ReadOnly });
         }
         public IActionResult GridViewExtendedController(VcGridviewExtendedModel VcModel)
         {
             return ViewComponent("GridViewExtended", new { CompanyID = VcModel.CompanyID, DatasTable = VcModel.DatasTable, ExcludeColumns = VcModel.ExcludeColumns });
         }
         /// <summary>
         /// LoadCompanyDetails
         /// To get company details by CompanyID
         /// </summary>
         /// <param name="CompanyID"></param>
         public JsonResult LoadCompanyDetails(String CompanyID)
         {

             DataTable dataTable = new SmartSuiteDAL.General.Masters.CompanyMaster(ConnectionString).Fill(CompanyID);
             return Json(new { result = JsonConvert.SerializeObject(dataTable, Formatting.Indented), username = Username });
         }

         public IActionResult PrintPage(PrintModel printTpl)
         {
             CompanyMaster objCompanyModel = new CompanyMaster();
             if(printTpl.IsHeader == true)
             {

                 DataTable dt = new SmartSuiteDAL.General.Masters.CompanyMaster(ConnectionString).Fill(printTpl.CompanyID);
                 objCompanyModel.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                 objCompanyModel.Address1 = dt.Rows[0]["Address1"].ToString();
                 objCompanyModel.Address2 = dt.Rows[0]["Address2"].ToString();
                 objCompanyModel.City = dt.Rows[0]["City"].ToString();
                 objCompanyModel.State = dt.Rows[0]["State"].ToString();
                 objCompanyModel.Telephone = dt.Rows[0]["Telephone"].ToString();
                 objCompanyModel.Mobile = dt.Rows[0]["Mobile"].ToString();
                 objCompanyModel.Email = dt.Rows[0]["Email"].ToString();
                 printTpl.CompanyDetails = objCompanyModel;
                 printTpl.PrintLogo = dt.Rows[0]["Logo"].ToString();
             }
             printTpl.PrintBy = this.Username;
             printTpl.PrintDate = DateTime.Now.ToString("dd/MM/yyyy h:mm tt");

             return View("~/Views/printTpl.cshtml", printTpl);
         }


         // set MaxLength for smart-input-box -- Jiyad -- 04-04-2019
         public JsonResult GetMaxLengthFields(String ControllerName)
         {
             List<SelectListItem> lst = new List<SelectListItem>();
             DataTable dataTable = new SmartSuiteDAL.BaseDAL(ConnectionString).GetMaxLengthFields(ControllerName, Language);
             foreach (DataRow dr in dataTable.Rows)
             {
                 lst.Add(new SelectListItem { Text = dr["FieldName"].ToString(), Value = dr["MaxLength"].ToString() });
             }
             return Json(new SelectList(lst, "Value", "Text"));
         }
         public JsonResult GetBarcode(String Content)
         {
             BarCodeAndQrCode objCode = new BarCodeAndQrCode();
             String retString = objCode.GenerateBarCode(Content);
             return Json(new { message = "success", result = retString });
         }

         public void LoadSettings(Object PageID,Object CompanyID,String Username)
         {
             DataTable dataTable = new SmartSuiteDAL.BaseDAL(ConnectionString).LoadSettings(PageID, CompanyID, Username);
             Hashtable SettingsTable = new Hashtable();
             foreach (DataRow dr in dataTable.Rows)
             {
                 if (!SettingsTable.ContainsKey(dr["Key"]))
                 {
                     SettingsTable.Add(dr["Key"].ToString(), dr["Value"]);
                 }
             }
             ViewBag.SettingsTable = SettingsTable;
         }
         public void LoadEcomSettings(Object CompanyID)
         {
             DataTable dataTable = new SmartSuiteDAL.BaseDAL(ConnectionString).LoadEcomSettings(CompanyID);
             Hashtable EcomSettingsTable = new Hashtable();
             foreach (DataRow dr in dataTable.Rows)
             {
                 if (!EcomSettingsTable.ContainsKey(dr["Key"]))
                 {
                     EcomSettingsTable.Add(dr["Key"].ToString(), dr["Value"]);
                 }
             }
             ViewBag.EcomSettingsTable = EcomSettingsTable;
         }
         // get server time

         public DateTime GetServerTime()
         {
             DateTime ServerDateTime = new SmartSuiteDAL.BaseDAL(ConnectionString).GetServerTime();
             return ServerDateTime;
         }
        */
        //Newly added for pageid Common
        public enum PageIDs
        {
            Purchase = 15,
            SalesInvoice = 149,
            DeliveryIn = 234,
            DeliveryOut = 235,
            PurchaseEnquiry = 268,
            PurchaseForm = 264,
            PurchaseOrder = 231,
            PurchaseReturn = 178,
            SalesEnquiry = 232,
            SalesEstimate = 261,
            SalesOrder = 262,
            SalesQuotation = 233,
            SalesReturn = 179,
            SalesRegister = 201,
            DaySummary = 378,
            AreaWiseSales = 310,
            Voucher = 91,
            EmployeeMaster = 142,
            SalaryTimesheet = 128,
            Salary = 129,
            Leave = 161,
            OpeningStock = 135,
            StockAdjustment = 24
        }

        //ImagePath for  Company header and footer on 27-09-2023
        public String CompanyLogoPath
        {
            get
            {
                return "/Resources/46/MaCompany/CompanyLogo/";
               // return "/Resources/" + TenantID + "/MaCompany/CompanyLogo/";
            }
        }
    }
}
