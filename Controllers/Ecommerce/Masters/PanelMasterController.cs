using ERPSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace ERPSample.Controllers.Ecommerce.Masters
{
    public class PanelMasterController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;

        public PanelMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        private DAL.Ecommerce.Masters.PanelMaster _PanelMasterDAL;
        private DAL.Ecommerce.Masters.PanelMaster PanelMasterDAL
        {
            get
            {
                if (_PanelMasterDAL == null)
                {
                    _PanelMasterDAL = new DAL.Ecommerce.Masters.PanelMaster(ConnectionString);
                }
                return _PanelMasterDAL;
            }
        }
        private String ProductImagePath
        {
            get
            {
                return "Resources\\" + TenantID + "\\Ecommerce\\ItemImage";
            }
        }

        public async Task<IActionResult> Index(int MenuID)
        {
            SetUserPermissions(MenuID);
            DataTable dt = PanelMasterDAL.Fill();
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["Title"].ToString() + "</td>");
                sb.Append("<td>");
                sb.Append(dr["PanelType"].ToString());  
                sb.Append("</td>");
                sb.Append("<td>" + dr["Date"].ToString() + "</td>");
                sb.Append("<td>" + dr["Remarks"].ToString() + "</td>");
                sb.Append("<td>" + dr["OrderNo"].ToString() + "</td>");
                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"].ToString() + ")'> <a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("</tr>");
                count++;
            }
            string panelmaster = sb.ToString();
            ViewBag.PanelMaster = panelmaster;
            ViewBag.MenuID = MenuID;
            return View("~/Views/Ecommerce/Masters/Panels.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> PanelMappingTypes()
        {
            int NextNo = 0;
            DataSet Additional = PanelMasterDAL.FillAdditionals();
            DataTable NextAccountCode = Additional.Tables[0];
            DataTable PanelTypes = Additional.Tables[1];
            DataTable ItemSortby = Additional.Tables[2];
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> row;
            if (NextAccountCode.Rows.Count != 0)
            {
                NextNo = Convert.ToInt32(NextAccountCode.Rows[0]["LastID"].ToString());
            }
            sb.Append("<option value=''>Choose Panel Type</option>");
            foreach (DataRow dr in PanelTypes.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string PanelType = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''> Choose One</option>");
            foreach (DataRow dr in ItemSortby.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string sortBy = sb.ToString();
            return Json(new { success = true, message = "Success", sortBy = sortBy, categories = PanelType, NextNo = NextNo });
        }

        [HttpPost]
        public async Task<IActionResult> PanelMappingEntities(int PanelTypeID, int DestnID)
        {

            DataTable dt = PanelMasterDAL.PanelMappingEntities(PanelTypeID, DestnID);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='table table-bordered key-buttons text-nowrap' id=''>");
            sb.Append("<thead>");
            sb.Append("<tr>");

            foreach (DataColumn dc in dt.Columns)
            {
                sb.Append("<th>");
                sb.Append(dc.ColumnName);
                sb.Append("</th>");
            }
            sb.Append("<th>");
            sb.Append("</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            int No = 1;
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn dc in dt.Columns)
                {

                    sb.Append("<td>");
                    if (dc.DataType == typeof(Boolean))
                    {
                        sb.Append("<div class='material-switch ProductActive'>");
                        sb.Append("<input id='");
                        sb.Append(dc.ColumnName);
                        sb.Append(dr[0]);
                        sb.Append("'");
                        sb.Append(" data-id='");
                        sb.Append(dr[0]);
                        sb.Append("'");
                        sb.Append(" name='");
                        sb.Append(dc.ColumnName);
                        sb.Append("' ");
                        sb.Append("type='checkbox' ");
                        if (Convert.ToBoolean(dr[dc.ColumnName]))
                        {
                            sb.Append(" checked ");
                        }
                        sb.Append("><label for='");
                        sb.Append(dc.ColumnName);
                        sb.Append(dr[0]);
                        sb.Append("' ");
                        sb.Append("class='label-success'></label></div>");
                    }
                    else
                    {
                        sb.Append(dr[dc.ColumnName]);
                    }
                    sb.Append("</td>");
                }
                sb.Append("<td class='badge-danger border-bottom p-0 text-center'><span class='pe-7s-trash action_delete text-white'></span></td>");
                sb.Append("</tr>");
                No++;
            }
            sb.Append("</tbody>");
            sb.Append("</table>");

            return Json(new { success = true, message = "Success", innerHTML = sb.ToString(), ProductImagePath = Path.Combine(_appEnvironment.WebRootPath, ProductImagePath) });
        }

        [HttpGet]
        public async Task<IActionResult> RowClick(int id)
        {
            try
            {
                DataSet Ds = new DataSet();
                //var Results = PanelMasterDAL.Fill(id);
                Ds = PanelMasterDAL.GetPanelMaster(id);
                DataTable Dt = Ds.Tables[0];
                DataRow datarow = Dt.Rows[0];
                DataTable Dt2 = Ds.Tables[1];
                Dictionary<string, object> row;
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                foreach (DataRow dr1 in Dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col1 in Dt.Columns)
                    {
                        row.Add(col1.ColumnName, dr1[col1]);
                    }
                    rows.Add(row);
                }


                DataSet Additional = PanelMasterDAL.FillAdditionals();
                DataTable PanelTypes = Additional.Tables[1];
                DataTable ItemSortby = Additional.Tables[2];

                StringBuilder sb = new StringBuilder();
                
                sb.Append("<option value=''>Choose Panel Type</option>");
                foreach (DataRow dr in PanelTypes.Rows)
                {
                    sb.Append("<option value='");
                    sb.Append(dr["ID"]);
                    sb.Append("'");
                    if (datarow["PanelTypeID"].ToString() == dr["ID"].ToString())
                    {
                        sb.Append(" selected");

                    }
                    sb.Append(">"); 
                    sb.Append(dr["Value"]);
                    sb.Append("</option>");
                }
                string PanelType = sb.ToString();
                sb.Clear();
                sb.Append("<option value=''> Choose One</option>");
                foreach (DataRow dr in ItemSortby.Rows)
                {
                    sb.Append("<option value='");
                    sb.Append(dr["ID"]);
                    sb.Append("'");
                    if (datarow["Sortby"].ToString() == dr["ID"].ToString())
                    {
                        sb.Append(" selected");

                    }
                    sb.Append(">");
                    sb.Append(dr["Value"]);
                    sb.Append("</option>");
                }
                string sortBy = sb.ToString();
                sb.Clear();
                int serialNo = 1; // for S.No column
                foreach (DataRow dr in Dt2.Rows)
                {
                    sb.Append("<tr element-id='");
                    sb.Append(dr[0]);   // assuming first col is Item ID
                    sb.Append("'>");

                    // S.No
                    sb.Append("<td>");
                    sb.Append(serialNo++);
                    sb.Append("</td>");

                    // Item ID (assuming column name is "ItemID")
                    sb.Append("<td>");
                    sb.Append(dr["ID"]);
                    sb.Append("</td>");

                    // Item Name
                    sb.Append("<td>");
                    sb.Append(dr["ItemName"]);
                    sb.Append("</td>");

                    // Selling Price
                    sb.Append("<td>");
                    sb.Append(dr["SellingPrice"]);
                    sb.Append("</td>");

                    // Active (checkbox switch)
                    sb.Append("<td>");
                    sb.Append("  <div class='form-check form-switch form-check-reverse'>");
                    sb.Append("    <input class='form-check-input' ");
                    sb.Append("id='Active" + dr[0] + "' ");
                    sb.Append("data-id='" + dr[0] + "' ");
                    sb.Append("name='Active' type='checkbox' role='switch' ");

                    if (Convert.ToBoolean(dr["Active"]))
                    {
                        sb.Append("checked ");
                    }

                    sb.Append("/>");
                    sb.Append("  </div>");
                    sb.Append("</td>");


                    // Delete button
                    //sb.Append("<td class='badge-danger border-bottom p-0 text-center'>");
                    //sb.Append("<span class='pe-7s-trash action_delete text-white'></span>");
                    //sb.Append("</td>");
                    sb.Append("<td class='col' id='deleteaction" + serialNo + "'><ul class='action'><li class='delete ms-3 action_delete ' id='deletitem" + serialNo + "' element-id='" + serialNo + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");

                    sb.Append("</tr>");
                }

                return Json(new { success = true, sortby = sortBy, paneltype = PanelType, innerHTML = sb.ToString(), header = JsonConvert.SerializeObject(rows), message = "Success" });
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }
        }



    }
}
