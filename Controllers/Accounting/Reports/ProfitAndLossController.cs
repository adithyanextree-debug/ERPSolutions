using ERPSample.DAL.General.Companies;
using ERPSample.Models.Accounting.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERPSample.Controllers.Accounting.Reports
{
    public class ProfitAndLossController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public ProfitAndLossController(IWebHostEnvironment env)
        {
            _env = env;
        }
        const int PageMenuID = 81;
        public IActionResult Index()
        {
            SetUserPermissions(80);
            ProfitLossModel profitLossModel = new ProfitLossModel();
            DateTime dateTime = DateTime.Now;
            var edate = dateTime.ToShortDateString();
            var sdate = dateTime.AddYears(-1).ToShortDateString();
            string enddate = profitLossModel.EndDate.ToString();
            enddate = edate;
            string startdate = profitLossModel.StartDate.ToString();
            startdate = sdate;
            return View("~/Views/Accounting/Reports/ProfitLoss.cshtml", profitLossModel);
        }

        public IActionResult Index2()
        {
            SetUserPermissions(80);
            CorporateTax corporatetax = new CorporateTax();
            DateTime dateTime = DateTime.Now;
            var edate = dateTime.ToShortDateString();
            var sdate = dateTime.AddYears(-1).ToShortDateString();
            string enddate = corporatetax.EndDate.ToString();
            enddate = edate;
            string startdate = corporatetax.StartDate.ToString();
            startdate = sdate;
            return View("~/Views/Accounting/Reports/CorporateTax.cshtml", corporatetax);
        }

        public IActionResult ShowReport(ProfitLossModel profitLossModel)
        {
            SetUserPermissions(PageMenuID);
            var viewby = Request.Form["Viewby"];
            bool Twosided = false;
            if (viewby == "1")
            {
                Twosided = true;
            }
            profitLossModel.ReportTable = new DAL.Accounting.Reports.DALAccountingReports(ConnectionString).ProfitAndLoss(profitLossModel.StartDate, profitLossModel.EndDate, BranchID, ConnectionString, Twosided);
            //======to get header and footer in print 20/09/2023 Aiswarya K S======//
            CompaniesOperations companyoperations = new CompaniesOperations(ConnectionString);
            int id = Convert.ToInt32(BranchID);
            DataSet ds = companyoperations.GetCompanyImages(id);
            DataRow dr = ds.Tables[0].Rows[0];
            string header = Path.Combine(CompanyLogoPath, dr["HeaderImage"].ToString());
            string footer = Path.Combine(CompanyLogoPath, dr["FooterImage"].ToString());
            ViewBag.HeaderImage = header;
            ViewBag.FooterImage = footer;
            //======Upto here======//
            // Detect AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return just the table portion as a Partial View
                return PartialView("~/Views/Accounting/Reports/_ProfitandLossReport.cshtml", profitLossModel);
            }
            return View("~/Views/Accounting/Reports/ProfitLoss.cshtml", profitLossModel);
        }

        public IActionResult ShowCorporateTaxReport(CorporateTax corporatetax)
        {
            SetUserPermissions(PageMenuID);

            corporatetax.ReportTable = new DAL.Accounting.Reports.DALAccountingReports(ConnectionString).CorporateTax(corporatetax.StartDate, corporatetax.EndDate, BranchID, ConnectionString);
            //======to get header and footer in print 20/09/2023 Aiswarya K S======//
            CompaniesOperations companyoperations = new CompaniesOperations(ConnectionString);
            int id = Convert.ToInt32(BranchID);
            DataSet ds = companyoperations.GetCompanyImages(id);
            DataRow dr = ds.Tables[0].Rows[0];
            string header = Path.Combine(CompanyLogoPath, dr["HeaderImage"].ToString());
            string footer = Path.Combine(CompanyLogoPath, dr["FooterImage"].ToString());
            ViewBag.HeaderImage = header;
            ViewBag.FooterImage = footer;
            //======Upto here======//
            // Detect AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return just the table portion as a Partial View
                return PartialView("~/Views/Accounting/Reports/_CorporateTax.cshtml", corporatetax);
            }
            return View("~/Views/Accounting/Reports/CorporateTax.cshtml", corporatetax);
        }
    }
}
