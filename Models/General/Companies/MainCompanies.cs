namespace ERPSample.Models.General.Companies
{
    public class MainCompanies
    {
        public int ID { get; set; }
        public string Company { get; set; }
        public int ContactPersonID { get; set; }
        public string Nature { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string POBox { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string FaxNo { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool ActiveFlag { get; set; }
        public long BranchCompanyID { get; set; }
        public string SalesTaxNo { get; set; }
        public string CentralSalesTaxNo { get; set; }
        public string UniqueID { get; set; }
        public string Reference { get; set; }
        public string BankCode { get; set; }
        public string DL1 { get; set; }
        public string DL2 { get; set; }
        public bool LockSystem { get; set; }
        public string HeaderImage { get; set; }
        public string FooterImage { get; set; }
    }
}
