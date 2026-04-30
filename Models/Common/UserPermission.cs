using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Common
{
    //public class UserPermission
    //{
    //    public String PageName { get; set; }
    //    public Boolean IsView { get; set; } = false;

    //    public Boolean IsCreate { get; set; } = false;
    //    public Boolean IsEdit { get; set; } = false;
    //    public Boolean IsDelete { get; set; } = false;
    //    public Boolean IsCancel { get; set; } = false;
    //    public Boolean IsPrint { get; set; } = false;
    //    public Boolean IsCopy { get; set; } = false;
    //    public Boolean IsApprove { get; set; } = false;
    //    public Boolean IsEditApprove { get; set; } = false;
    //    public Boolean IsHigherApprove { get; set; } = false;
    //    public Boolean IsEmail { get; set; } = false;
    //    public Boolean IsAttach { get; set; } = false;
    //    public Boolean IsBills { get; set; } = false;
    //    public Boolean IsSettings { get; set; } = false;
    //    public Boolean IsPDF { get; set; } = false;
    //    public Boolean IsExcel { get; set; } = false;
    //    public void SetPermissions(DataTable dtUserPermissions)
    //    {
    //        if (dtUserPermissions.Rows.Count > 0)
    //        {
    //            DataRow dr = dtUserPermissions.Rows[0];
    //            if (dr["IsView"] == null || dr["IsView"] == DBNull.Value)
    //            {
    //                IsView = false;
    //            }
    //            else
    //            {
    //                IsView = Convert.ToBoolean(dr["IsView"]);
    //            }
    //            if (dr["IsCreate"] == null || dr["IsCreate"] == DBNull.Value)
    //            {
    //                IsCreate = false;
    //            }
    //            else
    //            {
    //                IsCreate = Convert.ToBoolean(dr["IsCreate"]);
    //            }
    //            if (dr["IsEdit"] == null || dr["IsEdit"] == DBNull.Value)
    //            {
    //                IsEdit = false;
    //            }
    //            else
    //            {
    //                IsEdit = Convert.ToBoolean(dr["IsEdit"]);
    //            }
    //            if (dr["IsCancel"] == null || dr["IsCancel"] == DBNull.Value)
    //            {
    //                IsCancel = false;
    //            }
    //            else
    //            {
    //                IsCancel = Convert.ToBoolean(dr["IsCancel"]);
    //            }
    //            if (dr["IsDelete"] == null || dr["IsDelete"] == DBNull.Value)
    //            {
    //                IsDelete = false;
    //            }
    //            else
    //            {
    //                IsDelete = Convert.ToBoolean(dr["IsDelete"]);
    //            }
    //            if (dr["IsApprove"] == null || dr["IsApprove"] == DBNull.Value)
    //            {
    //                IsApprove = false;
    //            }
    //            else
    //            {
    //                IsApprove = Convert.ToBoolean(dr["IsApprove"]);
    //            }
    //            if (dr["IsEditApproved"] == null || dr["IsEditApproved"] == DBNull.Value)
    //            {
    //                IsEditApprove = false;
    //            }
    //            else
    //            {
    //                IsEditApprove = Convert.ToBoolean(dr["IsEditApproved"]);
    //            }
    //            if (dr["IsHigherApprove"] == null || dr["IsHigherApprove"] == DBNull.Value)
    //            {
    //                IsHigherApprove = false;
    //            }
    //            else
    //            {
    //                IsHigherApprove = Convert.ToBoolean(dr["IsHigherApprove"]);
    //            }
    //            if (dr["IsPrint"] == null || dr["IsPrint"] == DBNull.Value)
    //            {
    //                IsPrint = false;
    //            }
    //            else
    //            {
    //                IsPrint = Convert.ToBoolean(dr["IsPrint"]);
    //            }
    //        }
    //    }
    //}
}
