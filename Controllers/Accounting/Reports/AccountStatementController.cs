using ERPSample.DAL.General.Companies;
using ERPSample.Models.Accounting.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERPSample.Controllers.Accounting.Reports
{
    public class AccountStatementController : BaseController
    {
        const int PageMenuID = 77;
        public IActionResult Index()
        {
            SetUserPermissions(PageMenuID);
            AccountStatementModel accountStatementModel = new AccountStatementModel();
            accountStatementModel.EndDate = DateTime.Now;
            accountStatementModel.StartDate = accountStatementModel.EndDate.AddYears(-1);
          //  LoadAccounts(accountStatementModel);
            return View("~/Views/Accounting/Reports/AccountStatement.cshtml", accountStatementModel);
        }

        public IActionResult ShowReport(AccountStatementModel accountStatementModel)
        {
            SetUserPermissions(PageMenuID);

            accountStatementModel.ReportTable = new DAL.Accounting.Reports.DALAccountingReports(ConnectionString)
                .AccountSatetement(accountStatementModel.AccountID, accountStatementModel.StartDate, accountStatementModel.EndDate, BranchID);

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
                return PartialView("~/Views/Accounting/Reports/_AccountStatementReport.cshtml", accountStatementModel);
            }

            // Fallback: full view load (e.g., direct link or refresh)
            return View("~/Views/Accounting/Reports/AccountStatement.cshtml", accountStatementModel);
        }


        //private void LoadAccounts(AccountStatementModel accountStatementModel)
        //{
        //    DataTable dt = new DAL.General.Common.CommandText(ConnectionString).GetTable(new DAL.General.Common.CommandText(ConnectionString).CommandTexts("BranchAccounts", null, BranchID));
        //    AccountModel accountModel;
        //    accountStatementModel.Accounts = new List<AccountModel>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        accountModel = new AccountModel();
        //        accountModel.ID = Convert.ToInt64(dr["ID"]);
        //        accountModel.AccountName = dr["AccountName"].ToString();
        //        accountStatementModel.Accounts.Add(accountModel);
        //    }
        //}
    }
}
