using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Common
{
    public class LoginModel
    {
        public Int64 ID { get; set; }
        public String Name { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Location { get; set; }
        public String IPAddress { get; set; }
        public String Language { get; set; }
        public String Token { get; set; }
        public String ConnectionString { get; set; }
        public String OS { get; set; }
        public String Browser { get; set; }
        public String BrowserVersion { get; set; }
        public List<LoginCompaniesModel> loginCompaniesModels { get; set; }
    }
    public class LoginCompaniesModel
    {
        public Int64 ID { get; set; }
        public String Name { get; set; }
        public String DatabaseName { get; set; }
        public String ServerName { get; set; }
        public String ServerIP { get; set; }
        public Boolean IsRemote { get; set; }
    }
    public class LoginShortModel
    {
        public Int64 ID { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public Int64 CompanyID { get; set; }
        public Int64 BranchID { get; set; }

    }
}
