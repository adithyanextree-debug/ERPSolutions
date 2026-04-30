using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSample
{
    public static class SharedClass
    {
        public static String MasterConnectionString
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var Configuration = builder.Build();
                StringBuilder sb = new StringBuilder();
                sb.Append("Data Source=");
                sb.Append(MasterDataSource);
                sb.Append(";");
                sb.Append("Initial Catalog=");
                sb.Append(MasterDatabase);
                sb.Append(";");
                sb.Append("User ID=nextree;Password=Nextree@4313$;TrustServerCertificate=True;");
                return sb.ToString();
            }
        }
        //public static String ConnectionString
        //{
        //    get
        //    {
        //        UserInfo objUserInfo = HttpContext.Session.GetComplexData<UserInfo>("UserInfo");
        //        return objUserInfo.ConnectionString;
        //    }
        //}
        private static String MasterDataSource
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var Configuration = builder.Build();
                return Configuration.GetConnectionString("Master Data Source");
            }
        }
        private static String MasterDatabase
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var Configuration = builder.Build();
                return Configuration.GetConnectionString("Master Database");
            }
        }
        public static DataTable GetMenu(HttpContext httpContext)
        {
            UserInfo objUserInfo = httpContext.Session.GetComplexData<UserInfo>("UserInfo");
            DataTable dt = null;
            if (objUserInfo != null)
            {
                dt = new DAL.General.Common.Menu(objUserInfo.ConnectionString).FillMenu(objUserInfo.BranchID, objUserInfo.UserID, objUserInfo.Language);
            }
            return dt;
        }

        public static String MainConnectionString
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var Configuration = builder.Build();
                StringBuilder sb = new StringBuilder();
                sb.Append("Data Source=");
                sb.Append(MasterDataSource);
                sb.Append(";");
                sb.Append("Initial Catalog=");
                sb.Append(MainDatabase);
                sb.Append(";");
                sb.Append("User ID=nextree;Password=Nextree@4313$;TrustServerCertificate=True;");
                return sb.ToString();
            }
        }

        private static String MainDatabase
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var Configuration = builder.Build();
                return Configuration.GetConnectionString("Main Database");
            }
        }
       
    }
}
