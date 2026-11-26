namespace ERPSample.Models.General.Masters
{
    public class CompanyModulesModel
    {
        public int CompanyID { get; set; }
        public List<int> ModuleIDs { get; set; } = new List<int>();
    }
}
