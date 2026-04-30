using ERPSample.DAL.General.Companies;
using ERPSample.Models.Accounting.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Accounting.Reports
{
    public class BillwiseStatementController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public BillwiseStatementController(IWebHostEnvironment env)
        {
            _env = env;
        }
        private DAL.Accounting.Reports.DALAccountingReports _billwise;
        private DAL.Accounting.Reports.DALAccountingReports billwise
        {
            get
            {
                if (_billwise == null)
                {
                    _billwise = new DAL.Accounting.Reports.DALAccountingReports(ConnectionString);
                }
                return _billwise;
            }
        }
        public IActionResult Index()
        {
            SetUserPermissions(78);
            BillwiseStatementModel billwiseStatementModel = new BillwiseStatementModel();
            billwiseStatementModel.EndDate = DateTime.Now;
            billwiseStatementModel.StartDate = billwiseStatementModel.EndDate.AddYears(-1);
            return View("~/Views/Accounting/Reports/BillwiseStatement.cshtml", billwiseStatementModel);
        }
        //showreport-table--[21/09/2023]
        public async Task<IActionResult> ShowReport(BillwiseStatementModel billwiseStatementModel)
        {
            SetUserPermissions(78);
            CompaniesOperations companyoperations = new CompaniesOperations(ConnectionString);
            int id = Convert.ToInt32(BranchID);
            DataSet ds = companyoperations.GetCompanyImages(id);
            DataRow dr1 = ds.Tables[0].Rows[0];
            string header = Path.Combine(CompanyLogoPath, dr1["HeaderImage"].ToString());
            string footer = Path.Combine(CompanyLogoPath, dr1["FooterImage"].ToString());
            ViewBag.HeaderImage = header;
            ViewBag.FooterImage = footer;
            //======upto here=====//
            //DataTable Report = new DataTable();
            StringBuilder sb = new StringBuilder();
            billwiseStatementModel.ReportTable = billwise.BillwiseStatement(billwiseStatementModel, BranchID);
            // Detect AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return just the table portion as a Partial View
                return PartialView("~/Views/Accounting/Reports/_BillwiseStatement.cshtml", billwiseStatementModel);
            }

            // Fallback: full view load (e.g., direct link or refresh)
            return View("~/Views/Accounting/Reports/BillwiseStatement.cshtml", billwiseStatementModel);
           
        }

    }
}
