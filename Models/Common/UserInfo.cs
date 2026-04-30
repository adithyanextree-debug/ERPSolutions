using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample
{
    public class UserInfo
    {
        public UserInfo() { }
        public UserInfo(String ConnectionString,String MainConnectionString, String Username, String Name, int TenantID,
            String TenantName, DateTime LoginTime, String IPAddress,Int64 UserID, Int64 BranchID)
        {
            this.MainConnectionString = MainConnectionString;
            this.ConnectionString = ConnectionString;
            this.Username = Username;
            this.Name = Name;
            this.TenantID = TenantID;
            this.TenantName = TenantName;
            this.LoginTime = LoginTime;
            this.IPAddress = IPAddress;
            this.UserID = UserID;
            this.BranchID = BranchID;
        }
        public Int64 UserID { get; set; }
        public String Username { get; set; }
        public String Name { get; set; }
        public int TenantID { get; set; }
        public String TenantName { get; set; }
        public DateTime LoginTime { get; set; }
        public String Language { get; set; }
        public String ConnectionString { get; set; }
        public String MainConnectionString { get; set; }
        public String IPAddress { get; set; }
        public Int64 BranchID { get; set; } 
    }

}
