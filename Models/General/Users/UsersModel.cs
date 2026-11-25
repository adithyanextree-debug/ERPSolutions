namespace ERPSample.Models.General.Users
{
    using System.Data;

    public class UsersModel
    {
        public List<MaEmployeeDetails> MaEmployeeDetails { get; set; }
        public MaEmployees MaEmployees { get; set; }
        public List<MaUserRights> MaUserRights { get; set; }
    }
    public class MaEmployeeDetails
    {
        public int? ID { get; set; }
        public int? EmployeeID { get; set; }
        public int DetailRowKey { get; set; }
        public int? BranchID { get; set; }
        public int? DepartmentID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? ActiveFlag { get; set; }
        public bool? IsMainBranch { get; set; }
        public int? SuperVisorID { get; set; }
        public int? MaRoleID { get; set; }

    }
    public class MaEmployees
    {
        public int? ID { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? EmailID { get; set; }
        public string? ResidenceNumber { get; set; }
        public string? OfficeNumber { get; set; }
        public string? MobileNumber { get; set; }
        public int? DesignationID { get; set; }
        public bool? Active { get; set; }
        public int? EmployeeType { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? GmailID { get; set; }
        public bool? IsLocationRestrictedUser { get; set; }
        public int? PhotoID { get; set; }
        public int? CreatedBranchID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? AccountID { get; set; }
        public string? ImagePath { get; set; }
    }
    public class MaUserRights
    {
        public int? ID { get; set; }
        public int? UserDetailsID { get; set; }
        public int DetailRowKey { get; set; }
        public int? PageMenuID { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsCancel { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsApprove { get; set; }
        public bool? IsEditApproved { get; set; }
        public bool? IsHigherApprove { get; set; }
        public bool? IsPrint { get; set; }
        public bool? IsEmail { get; set; }
        public bool? IsExcelUpload { get; set; }
    }

    public class MaEmployeesDatatable : DataTable
    {
        public MaEmployeesDatatable()
        {
            Columns.Add("ID", typeof(int));
            Columns.Add("FirstName", typeof(string));
            Columns.Add("MiddleName", typeof(string));
            Columns.Add("LastName", typeof(string));
            Columns.Add("Address", typeof(string));
            Columns.Add("EmailID", typeof(string));
            Columns.Add("ResidenceNumber", typeof(string));
            Columns.Add("OfficeNumber", typeof(string));
            Columns.Add("MobileNumber", typeof(string));
            Columns.Add("DesignationID", typeof(int));
            Columns.Add("Active", typeof(bool));
            Columns.Add("EmployeeType", typeof(int));
            Columns.Add("UserName", typeof(string));
            Columns.Add("Password", typeof(string));
            Columns.Add("GmailID", typeof(string));
            Columns.Add("IsLocationRestrictedUser", typeof(bool));
            Columns.Add("PhotoID", typeof(int));
            Columns.Add("CreatedBranchID", typeof(int));
            Columns.Add("CreatedBy", typeof(int));
            Columns.Add("CreatedOn", typeof(DateTime));
            Columns.Add("AccountID", typeof(int));
            Columns.Add("ImagePath", typeof(string));
        }
    }
    public class MaEmployeeDetailsDataTable : DataTable
    {
        public MaEmployeeDetailsDataTable()
        {
            Columns.Add("ID", typeof(int));
            Columns.Add("EmployeeID", typeof(int));
            Columns.Add("DetailRowKey", typeof(int));
            Columns.Add("BranchID", typeof(int));
            Columns.Add("DepartmentID", typeof(int));
            Columns.Add("CreatedBy", typeof(int));
            Columns.Add("CreatedOn", typeof(DateTime));
            Columns.Add("ActiveFlag", typeof(bool));
            Columns.Add("IsMainBranch", typeof(bool));
            Columns.Add("SupervisorID", typeof(int));
            Columns.Add("MaRoleID", typeof(int));
        }
    }
    public class MaUserRightsDataTable : DataTable
    {
        public MaUserRightsDataTable()
        {
            Columns.Add("ID", typeof(int));
            Columns.Add("UserDetailsID", typeof(int));
            Columns.Add("DetailRowKey", typeof(int));
            Columns.Add("PageMenuID", typeof(int));
            Columns.Add("IsView", typeof(bool));
            Columns.Add("IsCreate", typeof(bool));
            Columns.Add("IsEdit", typeof(bool));
            Columns.Add("IsCancel", typeof(bool));
            Columns.Add("IsDelete", typeof(bool));
            Columns.Add("IsApprove", typeof(bool));
            Columns.Add("IsEditApproved", typeof(bool));
            Columns.Add("IsHigherApprove", typeof(bool));
            Columns.Add("IsPrint", typeof(bool));
            Columns.Add("IsEMail", typeof(bool));
            Columns.Add("IsExcelUpload", typeof(bool));
        }
    }

}