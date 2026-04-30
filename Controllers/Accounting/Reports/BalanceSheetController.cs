using ERPSample.DAL.General.Companies;
using ERPSample.Models.Accounting.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERPSample.Controllers.Accounting.Reports
{
    public class BalanceSheetController : BaseController
    {
        public IActionResult Index()
        {
            SetUserPermissions(82);
            BalanceSheet balancesheet = new BalanceSheet();
            //balancesheet.EndDate = DateTime.Now;
            //balancesheet.StartDate = balancesheet.EndDate.AddYears(-1);
            DateTime dateTime = DateTime.Now;
            var edate = dateTime.ToShortDateString();
            var sdate = dateTime.AddYears(-1).ToShortDateString();
            string enddate = balancesheet.EndDate.ToString();
            enddate = edate;
            string startdate = balancesheet.StartDate.ToString();
            startdate = sdate;

            return View("~/Views/Accounting/Reports/BalanceSheet.cshtml", balancesheet);
        }

        public IActionResult ShowReport(BalanceSheet balancesheet)
        {
            SetUserPermissions(82);
            string selectedOption = Request.Form["Viewby"];
            var viewby = Request.Form["Viewby"];
            bool Twosided = false;
            if (viewby == "1")
            {
                Twosided = true;
            }
            balancesheet.TwoSided = Twosided;
            balancesheet.ReportTable = new DAL.Accounting.Reports.DALAccountingReports(ConnectionString).BalanceSheet(balancesheet.StartDate, balancesheet.EndDate, BranchID, ConnectionString, balancesheet.TwoSided);
            //======to get header and footer in print 12/07/2023 Adithya K A======//
            CompaniesOperations companyoperations = new CompaniesOperations(ConnectionString);
            int id = Convert.ToInt32(BranchID);
            DataSet ds = companyoperations.GetCompanyImages(id);
            DataRow dr = ds.Tables[0].Rows[0];
            string header = Path.Combine(CompanyLogoPath, dr["HeaderImage"].ToString());
            string footer = Path.Combine(CompanyLogoPath, dr["FooterImage"].ToString());
            ViewBag.HeaderImage = header;
            ViewBag.FooterImage = footer;
            //======upto here=====//
            // Detect AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return just the table portion as a Partial View
                return PartialView("~/Views/Accounting/Reports/_BalanceSheet.cshtml", balancesheet);
            }
            return View("~/Views/Accounting/Reports/BalanceSheet.cshtml", balancesheet);
        }
    }
}
