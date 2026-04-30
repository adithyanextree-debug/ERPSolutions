using ERPSample.DAL.General.Companies;
using ERPSample.Models.Accounting.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERPSample.Controllers.Accounting.Reports
{
    public class TrialBalanceController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public TrialBalanceController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            SetUserPermissions(80);
            TrialBalanceModel trialBalanceModel = new TrialBalanceModel();
            trialBalanceModel.EndDate = DateTime.Now;
            trialBalanceModel.StartDate = trialBalanceModel.EndDate.AddYears(-1);
            return View("~/Views/Accounting/Reports/TrialBalance.cshtml", trialBalanceModel);
        }
        public IActionResult ShowReport(TrialBalanceModel trialBalanceModel)
        {
            SetUserPermissions(80);

            trialBalanceModel.ReportTable = new DAL.Accounting.Reports.DALAccountingReports(ConnectionString).TrialBalance(trialBalanceModel.StartDate, trialBalanceModel.EndDate, BranchID, true, true, true, Convert.ToInt32(1));
            //======to get header and footer in print 12/07/2023 Adithya K A======//
            CompaniesOperations companyoperations = new CompaniesOperations(ConnectionString);
            int id = Convert.ToInt32(BranchID);
            DataSet ds = companyoperations.GetCompanyImages(id);
            DataRow dr = ds.Tables[0].Rows[0];
            string header = Path.Combine(CompanyLogoPath, dr["HeaderImage"].ToString());
            string footer = Path.Combine(CompanyLogoPath, dr["FooterImage"].ToString());
            ViewBag.HeaderImage = header;
            ViewBag.FooterImage = footer;

            // Detect AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return just the table portion as a Partial View
                return PartialView("~/Views/Accounting/Reports/_TrialBalance.cshtml", trialBalanceModel);
            }



            //======upto here=====//
            return View("~/Views/Accounting/Reports/TrialBalance.cshtml", trialBalanceModel);
        }

    }
}
