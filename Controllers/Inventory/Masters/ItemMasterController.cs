using ClosedXML.Excel;
using ERPSample.DAL.Inventory.Masters;
using ERPSample.Models;
using ERPSample.Models.Inventory;
using ERPSample.Models.Inventory.Masters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ItemMaster = ERPSample.DAL.Inventory.Masters.ItemMaster;

namespace ERPSample.Controllers.Inventory.Masters
{
    public class ItemMasterController : BaseController
    {

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _appEnvironment;
        //private readonly string baseUrl;

        public ItemMasterController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment appEnvironment)
        {
            // baseUrl = configuration["BaseUrl:BaseUrl"];
            _appEnvironment = appEnvironment;
        }

        private DAL.Inventory.Masters.ItemMaster _ItemMaster;

        private DAL.Inventory.Masters.ItemMaster ItemMaster
        {
            get
            {
                if (_ItemMaster == null)
                {
                    _ItemMaster = new ItemMaster(ConnectionString);
                }
                return _ItemMaster;
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    SetUserPermissions(55);
        //    //ViewBag.DataTable = itemmaster;
        //    //ViewBag.BaseUrl = baseUrl;
        //    ViewData["ViewName"] = "ItemMaster";
        //    return View("~/Views/Invertory/Masters/ItemMaster.cshtml");
        //}

        public async Task<IActionResult> Index( string? item, string? category, string? brand,string? modelno, string? barcode)
        {
            SetUserPermissions(55);
            DataTable dt = ItemMaster.Fill(item, category, brand, modelno, barcode);
            StringBuilder sb = new StringBuilder();
            int count = 1;

            foreach (DataRow dr in dt.Rows)
            {
                string rowClass = (count % 2 == 0) ? "even" : "odd";

                sb.Append("<tr class='" + rowClass + "'>");
                sb.Append("<td>" + count + "</td>");
                sb.Append("<td>" + dr["ItemCode"] + "</td>");

                sb.Append("<td>");
                if (!string.IsNullOrEmpty(dr["ImagePath"]?.ToString()))
                {
                    sb.Append("<img src='/" + ItemImagePath + "/" + dr["ImagePath"] +
                              "' path='" + dr["ImagePath"] +
                              "' alt='product image' width='30' height='30' style='margin-right: 15px;' />");
                }
                sb.Append(dr["ItemName"]);
                sb.Append("</td>");

                sb.Append("<td>" + dr["Brand"] + "</td>");
                sb.Append("<td>" + dr["Category"] + "</td>");
                sb.Append("<td>" + dr["Stock"] + "</td>");

                sb.Append("<td><ul class='action'>");
                sb.Append("<li class='edit' onclick='RowClick(" + dr["ID"] + ")'>");
                sb.Append("<a href='#'><i class='icon-pencil-alt'></i></a></li>");
                sb.Append("</ul></td>");

                sb.Append("</tr>");

                count++;
            }

            string itemmaster = sb.ToString();
            ViewBag.ItemMaster = itemmaster;
            ViewBag.MenuID = 55;
            return  View("~/Views/Invertory/Masters/ItemMaster.cshtml");// Json(new { itemmaster = itemmaster, success = true});
        }

        [HttpGet]
        public async Task<IActionResult> NewEntryDetails()
        {
            SetUserPermissions(55);
            DataSet ProuctDetails = ItemMaster.NewProductDetails();
            DataTable DtItemCategory = ProuctDetails.Tables[0];
            DataTable DtBrand = ProuctDetails.Tables[1];
            DataTable DtColor = ProuctDetails.Tables[2];
            DataTable DtArticle = ProuctDetails.Tables[3];
            DataTable DtSize = ProuctDetails.Tables[4];
            DataTable DtUnitMaster = ProuctDetails.Tables[5];
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>Choose Category</option>");
            foreach (DataRow dr in DtItemCategory.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string categories = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Brand</option>");
            foreach (DataRow dr in DtBrand.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'>");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string brands = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Colour</option>");
            foreach (DataRow dr in DtColor.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string color = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Article</option>");
            foreach (DataRow dr in DtArticle.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string article = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Size</option>");
            foreach (DataRow dr in DtSize.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string size = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Unit</option>");
            foreach (DataRow dr in DtUnitMaster.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["Unit"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Unit"]);
                sb.Append("</option>");
            }
            string units = sb.ToString();
          
            sb.Clear();
           
            int Sn = 1;
            sb.Append("<tr id='Row");
            sb.Append(Sn);
            sb.Append("' class='border-bottom-dark'><th scope='row'>"+ Sn + "</th>");
            sb.Append("<td class='' id='unitTd" + Sn + "'>");
            sb.Append("<select name='itemUnit" + Sn + "' element-id='" + Sn + "' style='width:5cm' id='itemUnit" + Sn + "' class='form-select itemUnit excelCells '> " + units + "</select>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemFactor excelCells numbersOnly  form-control' id='itemFactor" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='barcode excelCells numbersOnly form-control'   id='barcode" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemPurchase numbersOnly excelCells   form-control'   id='itemPurchase" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemSelling numbersOnly excelCells   form-control'   id='itemSelling" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemOnline numbersOnly excelCells   form-control'   id='itemOnline" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemPromotion numbersOnly excelCells   form-control'  id='itemPromotion" + Sn + "' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<div class='form-check form-switch form-check-reverse'>");
            sb.Append("<input id = 'productISDefault" + Sn + "' name = 'productISDefault" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
            sb.Append("class='form-check-input productISDefault' role='switch' checked=''>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<div class='form-check form-switch form-check-reverse'>");
            sb.Append("<input id = 'productunitActive" + Sn + "' name = 'productunitActive" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
            sb.Append("class='form-check-input' role='switch' checked=''>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addunit' element-id='" + Sn + "' serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("<td class='col' id='deleteaction"+Sn+ "'><ul class='action'><li class='delete ms-3 action_deleteunit ' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");
            sb.Append("<td>");
            sb.Append("<input type='hidden' class='itemunitid excelCells numbersOnly  form-control' id='itemunitid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");
            string ItemUnits = sb.ToString();
            sb.Clear();

            sb.Append("<tr id='Row");
            sb.Append(Sn);
            sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='unitbarcode excelCells numbersOnly  form-control' id='unitbarcode" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<div class='form-check form-switch form-check-reverse d-flex justify-content-center align-items-center'>");
            sb.Append("<input id = 'productbarcodeActive" + Sn + "' name = 'productbarcodeActive" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
            sb.Append("class='form-check-input productbarcodeActive' role='switch' checked=''>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addbarcode' style='' id='addbarcode"+Sn+ "'serialno='"+Sn+"' element-id='" + Sn+"'><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("<td class='col' id='deletebarcodeaction" + Sn + "'><ul class='action'><li class='delete ms-3 action_deletebarcode' id='deletebarcode" + Sn + "' element-id='" + Sn + "'>" +
                "<a href='#'><i class='icon-trash'></i></a></li></ul></td>");
            sb.Append("<td>");
            sb.Append("<input type='hidden' class='itembarcodeid excelCells numbersOnly  form-control' id='itembarcodeid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");
            string Barcode = sb.ToString();
            sb.Clear();
            sb.Append("<tr class='border-bottom-dark productImageRow'  id='productImageRow" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("    <th id='productimageNo" + Sn+ "' scope='row' element-id='" + Sn + "'  class='productimageNo'>" + Sn + "</th>");
            //sb.Append("<td id='tdproductimageNo" + Sn + "' >");
            //sb.Append("<span id='productimageNo" + Sn + "' imageid='" + Sn + "' element-id='" + Sn + "' data-value=" + Sn + " class='productimageNo'>" + Sn2 + "</span>");
            //sb.Append("</td>");
            sb.Append("    <td>");
            sb.Append("    <form action='/ItemMaster/AddImage' method='post' enctype='multipart/form-data' onsubmit='AJAXSubmit(this);return false;'>");
            sb.Append("            <input type='file' id='FileUpload_FormFile" + Sn + "' name='FileUpload.FormFile' accept='image/png, image/jpeg, image/gif' style='display:none;' />");
            sb.Append("            <input type='text' id='productImageItemId" + Sn + "' name='ImageItemId' style='display:none;' value='" + Sn + "' />");
            sb.Append("            <label for='FileUpload_FormFile" + Sn + "'>");
            sb.Append("                <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 70px; height: 70px;' />");
            sb.Append("            </label>");
            sb.Append("            <input type='submit' style='display:none;' id='ProductImageUploadBtn" + Sn + "' />");
            sb.Append("        </form>");
            sb.Append("    </td>");
            sb.Append("    <td><input class='form-control productimageTitle'  type='text' id='productimageTitle" + Sn + "'  value='' element-id='" + Sn + "' disabled></td>");
            sb.Append("    <td><input class='form-control productimageArabicTitle' type='text'  id='productimageArabicTitle" + Sn + "' value='' element-id='" + Sn + "'></td>");
            sb.Append("    <td><input class='form-control productimageSize' type='text' style='width:2cm;' id='productimageSize" + Sn + "' value='' element-id='" + Sn + "' disabled></td>");
            sb.Append("    <td id='tdproductimageSetPrimary" + Sn + "' class='col isDefault'><button type='button' id='productimageSetPrimary" + Sn + "'  class='btn btn-success rounded-1 productimageSetPrimary'  element-id='" + Sn + "' imageid='true' style=''>Primary</button></td>");
            sb.Append("    <td>");
            sb.Append("        <div class='form-check form-switch form-check-reverse'>");
            sb.Append("            <input class='form-check-input' id='productimageActive" + Sn + "' type='checkbox' role='switch' checked>");
            sb.Append("        </div>");
            sb.Append("    </td>");
            sb.Append("    <td class='col'><button type='button' class='btn btn-outline-primary rounded-1 addimage' id='addimage"+Sn+"' element-id='"+Sn+ "' serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("    <td class='col'  id='deleteimageaction" + Sn + "'>");
            sb.Append("        <ul class='action'>");
            sb.Append("            <li class='delete ms-3 action_deleteimage' id='deleteimage" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("                <a href='#'>");
            sb.Append("                    <i class='icon-trash'></i>");
            sb.Append("                </a>");
            sb.Append("            </li>");
            sb.Append("        </ul>");
            sb.Append("    </td>");
            sb.Append("<td>");
            sb.Append("<input type='hidden' class='itemimageid excelCells numbersOnly  form-control' id='itemimageid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");

            string Image = sb.ToString();
            return Json(new { success = true, units = units, categories = categories, brands = brands, itemUnits = ItemUnits, barcode = Barcode,image= Image,
            color = color,article=article,size=size
            });
        }

        public async Task<IActionResult> NewRow(int? no,string? Text)
        {
            StringBuilder sb = new StringBuilder();
            int? Sn = no + 1;
            if (Text == "Unit")
            {
                DataTable DtUnitMaster = ItemMaster.UnitNewRow();
               // DataTable DtUnitMaster = ProuctDetails.Tables[0];

                sb.Append("<option value=''>Choose Unit</option>");
                foreach (DataRow dr in DtUnitMaster.Rows)
                {
                    sb.Append("<option value='");
                    sb.Append(dr["Unit"]);
                    sb.Append("'");
                    sb.Append(">");
                    sb.Append(dr["Unit"]);
                    sb.Append("</option>");
                }
                string units = sb.ToString();
                sb.Clear();

                sb.Append("<tr id='Row");
                sb.Append(Sn);
                sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
                sb.Append("<td class='' id='unitTd" + Sn + "'>");
                sb.Append("<select name='itemUnit" + Sn + "' element-id='" + Sn + "' id='itemUnit" + Sn + "' style='width:5cm' class='form-select itemUnit excelCells '> " + units + "</select>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='itemFactor excelCells numbersOnly  form-control' id='itemFactor" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='barcode excelCells   form-control'   id='barcode" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='itemPurchase numbersOnly excelCells   form-control'   id='itemPurchase" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;''>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='itemSelling numbersOnly excelCells   form-control'   id='itemSelling" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;''>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='itemOnline numbersOnly excelCells   form-control'   id='itemOnline" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;''>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='itemPromotion numbersOnly excelCells   form-control'  id='itemPromotion" + Sn + "' element-id='" + Sn + "' autocomplete='off' style='text-align: right;''>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse'>");
                sb.Append("<input id = 'productISDefault" + Sn + "' name = 'productISDefault" + Sn + "' type = 'checkbox'   element-id='" + Sn + "'");
                sb.Append("class='form-check-input productISDefault' role='switch'>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse'>");
                sb.Append("<input id = 'productunitActive" + Sn + "' name = 'productunitActive" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
                sb.Append("class='form-check-input' role='switch' checked=''>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addunit' element-id='" + Sn + "' serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("<td class='col' id='deleteaction" + Sn + "'><ul class='action'><li class='delete ms-3 action_deleteunit ' id='deleteunit" + Sn + "' element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");
                sb.Append("<td>");
                sb.Append("<input type='hidden' class='itemunitid excelCells numbersOnly  form-control' id='itemunitid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");
                string ItemUnits = sb.ToString();
                sb.Clear();
                return Json(new { success = true, itemUnits = ItemUnits });
            }
            else if (Text == "Barcode")
            {
                sb.Append("<tr id='Row");
                sb.Append(Sn);
                sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='unitbarcode excelCells numbersOnly  form-control' id='unitbarcode" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse d-flex justify-content-center align-items-center'>");
                sb.Append("<input id = 'productbarcodeActive" + Sn + "' name = 'productbarcodeActive" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
                sb.Append("class='form-check-input productbarcodeActive' role='switch' checked=''>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addbarcode' style='' id='addbarcode" + Sn + "' serialno='"+Sn+"' element-id='" + Sn + "'><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("<td class='col' id='deletebarcodeaction" + Sn + "'><ul class='action'><li class='delete ms-3 action_deletebarcode' id='deletebarcode" + Sn + "' element-id='" + Sn + "'>" +
                    "<a href='#'><i class='icon-trash'></i></a></li></ul></td>");
                sb.Append("<td>");
                sb.Append("<input type='hidden' class='itembarcodeid excelCells numbersOnly  form-control' id='itembarcodeid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");
                string Barcode = sb.ToString();
                sb.Clear();
                return Json(new { success = true, itemBarcodes = Barcode });

            }
            else
            {
                sb.Append("<tr class='border-bottom-dark productImageRow'  id='productImageRow" + Sn + "' element-id='" + Sn + "'>");
                sb.Append("    <th id='productimageNo" + Sn + "' scope='row' element-id='" + Sn + "'  class='productimageNo'>" + Sn + "</th>");
                //sb.Append("<td id='tdproductimageNo" + Sn + "' >");
                //sb.Append("<span id='productimageNo" + Sn + "' imageid='" + Sn + "' element-id='" + Sn + "' data-value=" + Sn + " class='productimageNo'>" + Sn2 + "</span>");
                //sb.Append("</td>");
                sb.Append("    <td>");
                sb.Append("    <form action='/ItemMaster/AddImage' method='post' enctype='multipart/form-data' onsubmit='AJAXSubmit(this);return false;'>");
                sb.Append("            <input type='file' id='FileUpload_FormFile" + Sn + "' name='FileUpload.FormFile' accept='image/png, image/jpeg, image/gif' style='display:none;' />");
                sb.Append("            <input type='text' id='productImageItemId" + Sn + "' name='ImageItemId' style='display:none;' value='" + Sn + "' />");
                sb.Append("            <label for='FileUpload_FormFile" + Sn + "'>");
                sb.Append("                <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 70px; height: 70px;' />");
                sb.Append("            </label>");
                sb.Append("            <input type='submit' style='display:none;' id='ProductImageUploadBtn" + Sn + "' />");
                sb.Append("        </form>");
                sb.Append("    </td>");
                sb.Append("    <td><input class='form-control productimageTitle' type='text' id='productimageTitle" + Sn + "' value='' element-id='" + Sn + "' disabled></td>");
                sb.Append("    <td><input class='form-control productimageArabicTitle' type='text' id='productimageArabicTitle" + Sn + "' value='' element-id='" + Sn + "'></td>");
                sb.Append("    <td><input class='form-control productimageSize' type='text' id='productimageSize" + Sn + "' value='' element-id='" + Sn + "' disabled></td>");
                sb.Append("    <td id='tdproductimageSetPrimary" + Sn + "' class='col isDefault'><button type='button' id='productimageSetPrimary" + Sn + "'  class='btn btn-danger rounded-1 productimageSetPrimary'  element-id='"+Sn+"' imageid='false' style=''>Primary</button></td>");
                sb.Append("    <td>");
                sb.Append("        <div class='form-check form-switch form-check-reverse'>");
                sb.Append("            <input class='form-check-input' id='productimageActive" + Sn + "' type='checkbox' role='switch' checked>");
                sb.Append("        </div>");
                sb.Append("    </td>");
                sb.Append("    <td class='col'><button type='button' class='btn btn-outline-primary rounded-1 addimage' id='addimage" + Sn + "' element-id='" + Sn + "' serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("    <td class='col'  id='deleteimageaction" + Sn + "'>");
                sb.Append("        <ul class='action'>");
                sb.Append("            <li class='delete ms-3 action_deleteimage' id='deleteimage" + Sn + "' element-id='" + Sn + "'>");
                sb.Append("                <a href='#'>");
                sb.Append("                    <i class='icon-trash'></i>");
                sb.Append("                </a>");
                sb.Append("            </li>");
                sb.Append("        </ul>");
                sb.Append("    </td>");
                sb.Append("<td>");
                sb.Append("<input type='hidden' class='itemimageid excelCells numbersOnly  form-control' id='itemimageid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");

                string Image = sb.ToString();
                return Json(new { success = true, image = Image });

            }
        }
        [HttpPost]
        public async Task<IActionResult> addImage()
        {
            try
            {
                var DefaultValue = 0;
                var files = HttpContext.Request.Form.Files[0];
                string itemcode = (HttpContext.Request.Form["ItemCode"].ToString());
                string NAME = itemcode.ToString();

                if (files != null && files.Length > 0)
                {
                    // Get the path where the images are stored
                    var uploads = Path.Combine(_appEnvironment.WebRootPath, ItemImagePath);

                    // Count how many images already exist for the given itemcode (i.e., how many files start with itemcode)
                    var existingFiles = Directory.GetFiles(uploads, $"{NAME}_*.webp");
                    int imageCount = existingFiles.Length;

                    // Create the image name (itemcode_1, itemcode_2, etc.)
                    string ImageName = $"{NAME}_{imageCount + 1}";  // +1 to start from itemcode_1
                    var fileName = ImageName + ".webp";  // use .webp extension

                    // Save the image to the file system
                    if (files.Length > 0)
                    {
                        string filePath = Path.Combine(uploads, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await files.CopyToAsync(fileStream);
                        }

                        return Json(new { success = true, message = "Image Saved", imagepath = fileName, imagename = fileName, ID = "", imagesize = string.Format("{0:n1} KB", files.Length / 1024f) });
                    }
                    else
                    {
                        return Json(new { success = false, message = "An error occurred", DefaultValue = DefaultValue, ID = "", ImageSize = "" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No files uploaded", DefaultValue = DefaultValue, ID = "", ImageSize = "" });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveImageDetails(int ID, string Column, string Value)
        //{
        //    try
        //    {
        //        string Result = ItemMaster.SaveImageDetails(ID, Column, Value);
        //        if (Result == "true")
        //        {
        //            return Json(new { success = true, message = "Entry saved successfully", ID = ID });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = Result });
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        return Json(new { success = false, message = Ex.Message });
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> SaveProductEntry(Models.Inventory.Masters.ItemMaster itemMaster)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlTransaction Tx = null;
            String Result = "";
            try
            {
                Con.Open();
                Tx = Con.BeginTransaction();
                if (itemMaster.InvItemMaster.ID == 0)
                {
                    Result = ItemMaster.InsertEntry(itemMaster, Con, Tx);
                }
                else
                {
                    Result = ItemMaster.InsertEntry(itemMaster, Con, Tx);
                }
                bool isNumeric = int.TryParse(Result, out int n);
                int ID = 0;
                if (isNumeric)
                {
                    ID = Convert.ToInt32(Result);

                    foreach (var Item in itemMaster.InvItemUnits)
                    {
                        Item.ItemID = ID;
                        ItemMaster.SaveUnitDetails(Item, Con, Tx);

                    }
                    foreach (var Item in itemMaster.InvItemImages)
                    {
                        Item.ItemID = ID;
                        ItemMaster.SaveItemImages(Item, Con, Tx);

                    }
                    //Newly added for barcode insertion on 27-07-2023
                    foreach (var barcode in itemMaster.InvItemBarcodes)
                    {
                        barcode.ItemID = ID;
                        ItemMaster.SaveBarcodeDetails(barcode, Con, Tx);
                    }
                }
                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Product added", transactionNo = ID });

            }
            catch (SqlException sqlEx)
            {
                if (Tx != null)
                {
                    Tx.Rollback();
                    Con.Close();
                }

                // Duplicate key violation
                if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                {
                    if (sqlEx.ToString().Contains("InvItemBarcode"))
                    {
                        return Json(new { success = false, message = "This barcode already exists. Please use a different one." ,text="Barcode"});

                    }
                    else if (sqlEx.ToString().Contains("InvItemMaster"))
                    {
                        return Json(new { success = false, message = "This item code already exists. Please use a different one.",text = "Item Code" });

                    }
                }

                // Other SQL errors
                return Json(new { success = false, message = "A database error occurred: " + sqlEx.Message });
            }
            catch (Exception Ex)
            {
                if (Tx != null)
                {
                    Tx.Rollback();
                    Con.Close();
                }
                return Json(new { success = false, message = "An error occurred: " + Ex.Message });
            }
        }

        //Row Click
        [HttpPost]
        public async Task<IActionResult> ItemMasterDetails(int ID)
        {
            DataSet ProductDetails = ItemMaster.ProuctDetails(ID);
            DataTable DtDetails = ProductDetails.Tables[0];
            DataTable DtitemImages = ProductDetails.Tables[1];
            DataTable DtItemUnits = ProductDetails.Tables[2];
            DataTable DtItemCategory = ProductDetails.Tables[3];
            DataTable DtBrand = ProductDetails.Tables[4];
            DataTable DtColor = ProductDetails.Tables[5];
            DataTable DtArticle = ProductDetails.Tables[6];
            DataTable DtSize = ProductDetails.Tables[7];
            DataTable DtUnitMaster = ProductDetails.Tables[8];
            DataTable DtBarcode = ProductDetails.Tables[9];//Added for barcodes rowclick on 11-08-2023
            DataTable DtRootPath = ProductDetails.Tables[11];
            DataRow rootpath = DtRootPath.Rows[0];
            string? link = rootpath["Value"].ToString();
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> row;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            int Sn2 = 1;
            foreach (DataRow dr1 in DtDetails.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col1 in DtDetails.Columns)
                {
                    row.Add(col1.ColumnName, dr1[col1]);
                }
                rows.Add(row);
            }

            sb.Append("<option value=''>Choose Category</option>");
            foreach (DataRow dr in DtItemCategory.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (dr["ID"].ToString() == DtDetails.Rows[0]["CategoryID"].ToString())
                {
                    sb.Append(" selected");
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string categories = sb.ToString();
            sb.Clear();

            sb.Append("<option value=''>Choose Purchase Unit</option>");
            foreach (DataRow dr in DtUnitMaster.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["Unit"]);
                sb.Append("'");
                if (dr["Unit"].ToString() == DtDetails.Rows[0]["PurchaseUnit"].ToString())
                {
                    sb.Append(" selected");
                }
                sb.Append(">");
                sb.Append(dr["Unit"]);
                sb.Append("</option>");
            }
            string PurchaseUnits = sb.ToString();
            sb.Clear();

            sb.Append("<option value=''>Choose Selling Unit</option>");
            foreach (DataRow dr in DtUnitMaster.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["Unit"]);
                sb.Append("'");
                if (dr["Unit"].ToString() == DtDetails.Rows[0]["SellingUnit"].ToString())
                {
                    sb.Append(" selected");
                }
                sb.Append(">");
                sb.Append(dr["Unit"]);
                sb.Append("</option>");
            }
            string SalesUnits = sb.ToString();
            sb.Clear();

            sb.Append("<option value=''>Choose Brand</option>");
            foreach (DataRow dr in DtBrand.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (DtDetails.Rows[0]["BrandID"] != null && DtDetails.Rows[0]["BrandID"] != DBNull.Value)
                {
                    if (dr["ID"].ToString() == DtDetails.Rows[0]["BrandID"].ToString())
                    {
                        sb.Append(" selected");
                    }
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string brands = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Colour</option>");
            foreach (DataRow dr in DtColor.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (DtDetails.Rows[0]["ColorID"] != null && DtDetails.Rows[0]["ColorID"] != DBNull.Value)
                {
                    if (dr["ID"].ToString() == DtDetails.Rows[0]["ColorID"].ToString())
                    {
                        sb.Append(" selected");
                    }
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string color = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Article</option>");
            foreach (DataRow dr in DtArticle.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (DtDetails.Rows[0]["ArticleID"] != null && DtDetails.Rows[0]["ArticleID"] != DBNull.Value)
                {
                    if (dr["ID"].ToString() == DtDetails.Rows[0]["ArticleID"].ToString())
                    {
                        sb.Append(" selected");
                    }
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string article = sb.ToString();
            sb.Clear();
            sb.Append("<option value=''>Choose Size</option>");
            foreach (DataRow dr in DtSize.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["ID"]);
                sb.Append("'");
                if (DtDetails.Rows[0]["SizeID"] != null && DtDetails.Rows[0]["SizeID"] != DBNull.Value)
                {
                    if (dr["ID"].ToString() == DtDetails.Rows[0]["SizeID"].ToString())
                    {
                        sb.Append(" selected");
                    }
                }
                sb.Append(">");
                sb.Append(dr["Value"]);
                sb.Append("</option>");
            }
            string size = sb.ToString();
            sb.Clear();
            //Added for InvItemUnits unit dropdown list on 11-08-2023
            sb.Append("<option value=''>Choose Unit</option>");
            foreach (DataRow dr in DtUnitMaster.Rows)
            {
                sb.Append("<option value='");
                sb.Append(dr["Unit"]);
                sb.Append("'");
                sb.Append(">");
                sb.Append(dr["Unit"]);
                sb.Append("</option>");
            }
            string unitnewrow = sb.ToString();
            sb.Clear();
           
            int Sn = 1;
            foreach (DataRow dr in DtItemUnits.Rows)
            {
                sb.Append("<tr id='Row");
                sb.Append(dr["ID"]);
                sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
                sb.Append("<td class='' id='unitTd"+dr["ID"]+"'>");
                sb.Append("<select name='itemUnit" + dr["ID"] + "' element-id='" + dr["ID"] + "' id='itemUnit" + dr["ID"] + "' style='width:5cm' class='form-select itemUnit excelCells '>");
                sb.Append("<option value=''>Choose Unit</option>");
                foreach (DataRow datarow in DtUnitMaster.Rows)
                {
                    sb.Append("<option value='");
                    sb.Append(datarow["Unit"]);
                    sb.Append("'");
                    if (datarow["Unit"].ToString() == dr["Unit"].ToString())
                    {
                        sb.Append("selected");
                    }
                    sb.Append(">");
                    sb.Append(datarow["Unit"]);
                    sb.Append("</option>");
                }
                sb.Append("</select>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='itemFactor excelCells numbersOnly form-control' id='itemFactor" + dr["ID"] +
                    "' value='" + ((dr["Factor"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["Factor"].ToString())) ? "0.00" : Convert.ToDecimal(dr["Factor"]).ToString("F2")) +
                    "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<input type='text' class='barcode excelCells numbersOnly form-control' id='barcode" + dr["ID"] +
                    "' value='" + (dr["BarCode"] == DBNull.Value ? "" : dr["BarCode"].ToString()) +
                    "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<input type='text' class='itemPurchase numbersOnly excelCells form-control' id='itemPurchase" + dr["ID"] +
                    "' value='" + ((dr["PurchaseRate"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["PurchaseRate"].ToString())) ? "0.00" : Convert.ToDecimal(dr["PurchaseRate"]).ToString("F2")) +
                    "' element-id='" + dr["ID"] + "' autocomplete='off' style='text-align: right;'>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<input type='text' class='itemSelling numbersOnly excelCells form-control' id='itemSelling" + dr["ID"] +
                    "' value='" + ((dr["SellingPrice"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["SellingPrice"].ToString())) ? "0.00" : Convert.ToDecimal(dr["SellingPrice"]).ToString("F2")) +
                    "' element-id='" + dr["ID"] + "' autocomplete='off' style='text-align: right;'>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<input type='text' class='itemOnline numbersOnly excelCells form-control' id='itemOnline" + dr["ID"] +
                    "' value='" + ((dr["OnlinePrice"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["OnlinePrice"].ToString())) ? "0.00" : Convert.ToDecimal(dr["OnlinePrice"]).ToString("F2")) +
                    "' element-id='" + dr["ID"] + "' autocomplete='off' style='text-align: right;'>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<input type='text' class='itemPromotion numbersOnly excelCells form-control' id='itemPromotion" + dr["ID"] +
                    "' value='" + ((dr["PromotionPrice"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["PromotionPrice"].ToString())) ? "0.00" : Convert.ToDecimal(dr["PromotionPrice"]).ToString("F2")) +
                    "' element-id='" + dr["ID"] + "' autocomplete='off' style='text-align: right;'>");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse'>");
                sb.Append("<input id = 'productISDefault" + dr["ID"] + "' name = 'productISDefault" + dr["ID"] + "' type = 'checkbox'   element-id='" + dr["ID"] + "'");
                if (Convert.ToBoolean(dr["IsDefault"]))
                {
                    sb.Append(" checked ");
                }
                sb.Append("element-id='" + dr["ID"] + "'");
                sb.Append("class='form-check-input productISDefault' role='switch'>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse'>");
                sb.Append("<input id = 'productunitActive" + dr["ID"] + "' name = 'productunitActive" + dr["ID"] + "' type = 'checkbox'  element-id='" + dr["ID"] + "'");
                if (Convert.ToBoolean(dr["Active"]))
                {
                    sb.Append(" checked ");
                }
                sb.Append("element-id='" + dr["ID"] + "'");
                sb.Append("class='form-check-input' role='switch'>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addunit' element-id='" + dr["ID"] + "' serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("<td class='col' id='deleteaction" + dr["ID"] + "'><ul class='action'><li class='delete ms-3 action_deleteunit ' id='deleteunit" + dr["ID"] + "' element-id='" + dr["ID"] + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");
                sb.Append("<td>");
                sb.Append("<input type='hidden' class='itemunitid excelCells numbersOnly  form-control' id='itemunitid" + dr["ID"] + "' value='"+ dr["ID"] + "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                sb.Append("</td>");
                Sn = Sn + 1;
            }
            sb.Append("<tr id='Row");
            sb.Append(Sn);
            sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
            sb.Append("<td class='' id='unitTd" + Sn + "'>");
            sb.Append("<select name='itemUnit" + Sn + "' element-id='" + Sn + "' style='width:5cm' id='itemUnit" + Sn + "' class='form-select itemUnit excelCells '> " + unitnewrow + "</select>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemFactor excelCells numbersOnly  form-control' id='itemFactor" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='barcode excelCells   form-control'   id='barcode" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemPurchase numbersOnly excelCells   form-control'   id='itemPurchase" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemSelling numbersOnly excelCells   form-control'   id='itemSelling" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemOnline numbersOnly excelCells   form-control'   id='itemOnline" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='itemPromotion numbersOnly excelCells   form-control'  id='itemPromotion" + Sn + "' element-id='" + Sn + "' autocomplete='off' style='text-align: right;'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<div class='form-check form-switch form-check-reverse'>");
            sb.Append("<input id = 'productISDefault" + Sn + "' name = 'productISDefault" + Sn + "' type = 'checkbox'   element-id='" + Sn + "'");
            sb.Append("class='form-check-input productISDefault' role='switch'>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<div class='form-check form-switch form-check-reverse'>");
            sb.Append("<input id = 'productunitActive" + Sn + "' name = 'productunitActive" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
            sb.Append("class='form-check-input' role='switch' checked=''>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addunit' element-id='" + Sn + "' style='' serialno='"+Sn+"'><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("<td class='col' id='deleteaction" + Sn + "'><ul class='action'><li class='delete ms-3 action_deleteunit ' id='deleteunit" + Sn + "'  element-id='" + Sn + "'><a href='#'><i class='icon-trash'></i></a></li></ul></td>");
            sb.Append("<td>");
            sb.Append("<input type='hidden' class='itemunitid excelCells numbersOnly  form-control' id='itemunitid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");
            string ItemUnits = sb.ToString();
            sb.Clear();

            //Newly added for barcode  26-07-2023 [Adithya]
            Sn = 1;
            foreach (DataRow dr in DtBarcode.Rows)
            {
                sb.Append("<tr id='Row");
                sb.Append(dr["ID"]);
                sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
                sb.Append("<td>");
                sb.Append("<input type='text' class='unitbarcode excelCells numbersOnly  form-control' id='unitbarcode" + dr["ID"] + "' value='" + dr["BarCode"] + "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<div class='form-check form-switch form-check-reverse d-flex justify-content-center align-items-center'>");
                sb.Append("<input id = 'productbarcodeActive" + dr["ID"] + "' name = 'productbarcodeActive" + dr["ID"] + "' type = 'checkbox'  element-id='" + dr["ID"] + "'");
                if (Convert.ToBoolean(dr["Active"]))
                {
                    sb.Append(" checked ");
                }
                sb.Append(" element-id='" + dr["ID"] + "'");
                sb.Append("class='form-check-input productbarcodeActive' role='switch'>");
                sb.Append("</div>");
                sb.Append("</td>");
                sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addbarcode' style='' id='addbarcode" + dr["ID"] + "' serialno='"+Sn+"' element-id='" + dr["ID"] + "'><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("<td class='col' id='deletebarcodeaction" + dr["ID"] + "'><ul class='action'><li class='delete ms-3 action_deletebarcode' id='deletebarcode" + dr["ID"] + "' element-id='" + dr["ID"] + "'>" +
                    "<a href='#'><i class='icon-trash'></i></a></li></ul></td>");
                sb.Append("<td>");
                sb.Append("<input type='hidden' class='itembarcodeid excelCells numbersOnly  form-control' id='itembarcodeid" + dr["ID"] + "' value='"+ dr["ID"] + "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");
                Sn = Sn + 1;
            }

            sb.Append("<tr id='Row");
            sb.Append(Sn);
            sb.Append("' class='border-bottom-dark'><th scope='row'>" + Sn + "</th>");
            sb.Append("<td>");
            sb.Append("<input type='text' class='unitbarcode excelCells numbersOnly  form-control' id='unitbarcode" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<div class='form-check form-switch form-check-reverse d-flex justify-content-center align-items-center'>");
            sb.Append("<input id = 'productbarcodeActive" + Sn + "' name = 'productbarcodeActive" + Sn + "' type = 'checkbox'  checked element-id='" + Sn + "'");
            sb.Append("class='form-check-input productbarcodeActive' role='switch' checked=''>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("<td class='col' ><button type='button' class='btn btn-outline-primary rounded-1 addbarcode' style='' id='addbarcode" + Sn + "' serialno='"+Sn+"' element-id='" + Sn + "'><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("<td class='col' id='deletebarcodeaction" + Sn + "'><ul class='action'><li class='delete ms-3 action_deletebarcode' id='deletebarcode" + Sn + "' element-id='" + Sn + "'>" +
                "<a href='#'><i class='icon-trash'></i></a></li></ul></td>");
            sb.Append("<td>");
            sb.Append("<input type='hidden' class='itembarcodeid excelCells numbersOnly  form-control' id='itembarcodeid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");
            string Barcode = sb.ToString();
            sb.Clear();

            Sn = 1;
            foreach (DataRow dr in DtitemImages.Rows)
            {
                sb.Append("<tr class='border-bottom-dark productImageRow'  id='productImageRow" + dr["ID"] + "' element-id='" + dr["ID"] + "'>");
                sb.Append("    <th id='productimageNo" + dr["ID"] + "' scope='row' element-id='" + dr["ID"] + "'  class='productimageNo'>" + Sn + "</th>");
                sb.Append("    <td>");
                sb.Append("    <form action='/ItemMaster/AddImage' method='post' enctype='multipart/form-data' onsubmit='AJAXSubmit(this);return false;'>");
                sb.Append("            <input type='file' id='FileUpload_FormFile" + dr["ID"] + "' name='FileUpload.FormFile' accept='image/png, image/jpeg, image/gif' style='display:none;' />");
                sb.Append("            <input type='text' id='productImageItemId" + dr["ID"] + "' name='ImageItemId' style='display:none;' value='" + dr["ID"] + "' />");
                sb.Append("            <label for='FileUpload_FormFile" + dr["ID"] + "'>");
                sb.Append("                <img src='/" + ItemImagePath + "/" + (dr["ImagePath"].ToString()) + "' path='" + dr["ImagePath"].ToString() +"' alt='product image' id='productimagepreview" + dr["ID"] + "' class='productimagepreview' element-id='" + dr["ID"] + "' style='cursor:pointer; width: 70px; height: 70px;' />");
                sb.Append("            </label>");
                sb.Append("            <input type='submit' style='display:none;' id='ProductImageUploadBtn" + dr["ID"] + "' />");
                sb.Append("        </form>");
                sb.Append("    </td>");
                sb.Append("    <td><input class='form-control productimageTitle' type='text'  id='productimageTitle" + dr["ID"] + "' value='" + dr["Title"].ToString() + "' element-id='" + dr["ID"] + "' disabled></td>");
                sb.Append("    <td><input class='form-control productimageArabicTitle' type='text'  id='productimageArabicTitle" + dr["ID"] + "' value='" + dr["ArabicTitle"].ToString() + "' element-id='" + dr["ID"] + "'></td>");
                sb.Append("    <td><input class='form-control productimageSize' type='text' style='width:2cm;' id='productimageSize" + dr["ID"] + "' value='" + dr["ImageSize"].ToString() + "' element-id='" + dr["ID"] + "' disabled></td>");
                if (Convert.ToBoolean(dr["IsDefault"]))
                {
                    sb.Append("    <td id='tdproductimageSetPrimary" + dr["ID"] + "' class='col isDefault'><button type='button' id='productimageSetPrimary" + dr["ID"] + "'  class='btn btn-success rounded-1 productimageSetPrimary'  " +
                   "element-id='" + dr["ID"] + "' imageid='true' style=''>Primary</button></td>");
                }
                else
                {
                    sb.Append("    <td id='tdproductimageSetPrimary" + dr["ID"] + "' class='col isDefault'><button type='button' id='productimageSetPrimary" + dr["ID"] + "'  class='btn btn-danger rounded-1 productimageSetPrimary'  " +
                   "element-id='" + dr["ID"] + "' imageid='false' style=''>Primary</button></td>");
                }
                sb.Append("    <td>");
                sb.Append("        <div class='form-check form-switch form-check-reverse'>");
                sb.Append("            <input class='form-check-input' id='productimageActive" + dr["ID"] + "' type='checkbox' role='switch'");
                if (Convert.ToBoolean(dr["Active"]))
                {
                    sb.Append(" checked >");
                }
                sb.Append("        </div>");
                sb.Append("    </td>");
                sb.Append("    <td class='col'><button type='button' class='btn btn-outline-primary rounded-1 addimage' id='addimage" + dr["ID"] + "' element-id='" + dr["ID"] + "' serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
                sb.Append("    <td class='col'  id='deleteimageaction" + dr["ID"] + "'>");
                sb.Append("        <ul class='action'>");
                sb.Append("            <li class='delete ms-3 action_deleteimage' id='deleteimage" + dr["ID"] + "' element-id='" + dr["ID"] + "'>");
                sb.Append("                <a href='#'>");
                sb.Append("                    <i class='icon-trash'></i>");
                sb.Append("                </a>");
                sb.Append("            </li>");
                sb.Append("        </ul>");
                sb.Append("    </td>");
                sb.Append("<td>");
                sb.Append("<input type='hidden' class='itemimageid excelCells numbersOnly  form-control' id='itemimageid" + dr["ID"] + "' value='" + dr["ID"] + "' element-id='" + dr["ID"] + "' autocomplete='off'>");
                sb.Append("</td>");
                sb.Append("</tr>");
                Sn = Sn + 1;
            }
            sb.Append("<tr class='border-bottom-dark productImageRow'  id='productImageRow" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("    <th id='productimageNo" + Sn + "' scope='row' element-id='" + Sn + "'  class='productimageNo'>" + Sn + "</th>");
            sb.Append("    <td>");
            sb.Append("    <form action='/ItemMaster/AddImage' method='post' enctype='multipart/form-data' onsubmit='AJAXSubmit(this);return false;'>");
            sb.Append("            <input type='file' id='FileUpload_FormFile" + Sn + "' name='FileUpload.FormFile' accept='image/png, image/jpeg, image/gif' style='display:none;' />");
            sb.Append("            <input type='text' id='productImageItemId" + Sn + "' name='ImageItemId' style='display:none;' value='" + Sn + "' />");
            sb.Append("            <label for='FileUpload_FormFile" + Sn + "'>");
            sb.Append("                <img src='../assets/images/profile.png' alt='product image' id='productimagepreview" + Sn + "' class='productimagepreview' element-id='" + Sn + "' style='cursor:pointer; width: 70px; height: 70px;' />");
            sb.Append("            </label>");
            sb.Append("            <input type='submit' style='display:none;' id='ProductImageUploadBtn" + Sn + "' />");
            sb.Append("        </form>");
            sb.Append("    </td>");
            sb.Append("    <td><input class='form-control productimageTitle' type='text'  id='productimageTitle" + Sn + "' value='' element-id='" + Sn + "' disabled></td>");
            sb.Append("    <td><input class='form-control productimageArabicTitle' type='text'  id='productimageArabicTitle" + Sn + "' value='' element-id='" + Sn + "'></td>");
            sb.Append("    <td><input class='form-control productimageSize' type='text' style='width:2cm;' id='productimageSize" + Sn + "' value='' element-id='" + Sn + "' disabled></td>");
            sb.Append("    <td id='tdproductimageSetPrimary" + Sn + "' class='col isDefault'><button type='button' id='productimageSetPrimary" + Sn + "'  class='btn btn-danger rounded-1 productimageSetPrimary'  element-id='" + Sn + "' imageid='false' style=''>Primary</button></td>");
            sb.Append("    <td>");
            sb.Append("        <div class='form-check form-switch form-check-reverse'>");
            sb.Append("            <input class='form-check-input' id='productimageActive" + Sn + "' type='checkbox' role='switch' checked>");
            sb.Append("        </div>");
            sb.Append("    </td>");
            sb.Append("    <td class='col'><button type='button' class='btn btn-outline-primary rounded-1 addimage' id='addimage" + Sn + "' element-id='" + Sn + "'serialno='"+Sn+"' style=''><i class='fa-solid fa-plus'></i></button></td>");
            sb.Append("    <td class='col'  id='deleteimageaction" + Sn + "'>");
            sb.Append("        <ul class='action'>");
            sb.Append("            <li class='delete ms-3 action_deleteimage' id='deleteimage" + Sn + "' element-id='" + Sn + "'>");
            sb.Append("                <a href='#'>");
            sb.Append("                    <i class='icon-trash'></i>");
            sb.Append("                </a>");
            sb.Append("            </li>");
            sb.Append("        </ul>");
            sb.Append("    </td>");
            sb.Append("<td>");
            sb.Append("<input type='hidden' class='itemimageid excelCells numbersOnly  form-control' id='itemimageid" + Sn + "' value='' element-id='" + Sn + "' autocomplete='off'>");
            sb.Append("</td>");
            sb.Append("</tr>");
            string ItemImages = sb.ToString();
            sb.Clear();

            
            return Json(new { success = true, message = "Success", link= link,tenantid = TenantID, header = JsonConvert.SerializeObject(rows), categories = categories, brands = brands,
                bannerimagepath = ItemImagePath, purchaseunits = PurchaseUnits, salesunit = SalesUnits, itemunits = ItemUnits, itemimages = ItemImages, barcode = Barcode,
                color=color,article=article,size=size
            });
        }

        //Newly added for InvItemUnits  delete on 16-08-2023 
        public async Task<IActionResult> DeleteUnits(int ID)
        {
            try
            {
                string Result = ItemMaster.DeletInvItemUnits(ID);
                if (Result == "true")
                {
                    return Json(new { success = true, message = "Entry deleted successfully", ID = ID });
                }
                else
                {
                    return Json(new { success = false, message = Result });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }
        //Newly added for InvItemBarcodes delete on 16-08-2023 

        public async Task<IActionResult> DeleteBarcodes(int ID)
        {
            try
            {
                string Result = ItemMaster.DeletInvItemBarcodes(ID);
                if (Result == "true")
                {
                    return Json(new { success = true, message = "Entry deleted successfully", ID = ID });
                }
                else
                {
                    return Json(new { success = false, message = Result });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImageDetails(int ID)
        {
            try
            {
                string Result = ItemMaster.DeleteImageDetails(ID, Path.Combine(_appEnvironment.WebRootPath, ItemImagePath));
                if (Result == "true")
                {
                    return Json(new { success = true, message = "Entry saved successfully", ID = ID });
                }
                else
                {
                    return Json(new { success = false, message = Result });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                var result = ItemMaster.DeleteItemMaster(ID, Path.Combine(_appEnvironment.WebRootPath, ItemImagePath));
                if (result == "true")
                {
                    return Json(new { success = true, message = "Entry deleted successfully", transactionNo = ID });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });

            }
        }

        [HttpPost]        
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return Json(new { success = false, message = "No file uploaded." });
            }

            var logs = new List<string>();   // collect all debug info

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { success = false, message = "Excel file is empty or invalid." });
                        }

                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header
                        var httpClient = new HttpClient();
                        int successCount = 0, failCount = 0;

                        foreach (var row in rows)
                        {
                            try
                            {
                                string itemCode = row.Cell("A").GetString().Trim();
                                logs.Add($"Processing SKU: {itemCode}");

                                decimal price = 0;
                                string priceCellValue = row.Cell("R").GetValue<string>().Trim();
                                if (!decimal.TryParse(priceCellValue, out price))
                                {
                                    logs.Add($"⚠ Warning: Invalid price for SKU {itemCode}, setting price to 0.");
                                }

                                var longDescriptionLines = new List<string>
                                {
                                    row.Cell("F").GetString().Trim(),
                                    row.Cell("H").GetString().Trim(),
                                    row.Cell("I").GetString().Trim(),
                                    row.Cell("J").GetString().Trim(),
                                    row.Cell("K").GetString().Trim()
                                };
                                string combinedLongDescription = string.Join("\n", longDescriptionLines.Where(line => !string.IsNullOrWhiteSpace(line)));

                                var arabicLongDescription = new List<string>
                                {
                                    row.Cell("G").GetString().Trim(),
                                    row.Cell("M").GetString().Trim(),
                                    row.Cell("N").GetString().Trim(),
                                    row.Cell("O").GetString().Trim(),
                                    row.Cell("P").GetString().Trim()
                                };
                                string combinedArabicLongDescription = string.Join("\n", arabicLongDescription.Where(line => !string.IsNullOrWhiteSpace(line)));

                                var itemMaster = new Models.Inventory.Masters.ItemMaster
                                {
                                    InvItemMasterExcel = new Models.Inventory.Masters.InvItemMasterExcel
                                    {
                                        SKU = itemCode,
                                        BarCode = row.Cell("B").GetString().Trim(),
                                        ProductName = row.Cell("E").GetString().Trim(),
                                        Category = row.Cell("D").GetString().Trim(),
                                        Brand = row.Cell("C").GetString().Trim(),
                                        ENGLISHProductDescription = combinedLongDescription,
                                        ARABICProductDescription = combinedArabicLongDescription,
                                        //Active = true,
                                        PRICE = price,
                                    },
                                    //InvItemUnits = new List<Models.Inventory.Masters.InvItemUnits>
                                    //{
                                    //    new Models.Inventory.Masters.InvItemUnits
                                    //    {
                                    //        Unit = "No",
                                    //        BasicUnit = "No",
                                    //        OnlinePrice = price,
                                    //        SellingPrice = price,
                                    //        Active = true,
                                    //        Barcode = row.Cell("B").GetString().Trim()
                                    //    }
                                    //},
                                    //InvItemBarcodes = new List<Models.Inventory.Masters.InvItemBarcodes>
                                    //{
                                    //    new Models.Inventory.Masters.InvItemBarcodes
                                    //    {
                                    //        Active = true,
                                    //        Barcode = row.Cell("B").GetString().Trim()
                                    //    }
                                    //},
                                    InvItemImages = new List<Models.Inventory.Masters.InvItemImages>()
                                };

                                // handle images
                                var uploadRoot = Path.Combine(_appEnvironment.WebRootPath, ItemImagePath);
                                if (!Directory.Exists(uploadRoot))
                                    Directory.CreateDirectory(uploadRoot);

                                for (int i = 0; i < 5; i++)
                                {
                                    string imageUrl = row.Cell(19 + i).GetString().Trim();
                                    if (!string.IsNullOrEmpty(imageUrl))
                                    {
                                        try
                                        {
                                            var fileExtension = Path.GetExtension(imageUrl).ToLower();
                                            if (!fileExtension.EndsWith(".jpg") && !fileExtension.EndsWith(".jpeg") &&
                                                !fileExtension.EndsWith(".png") && !fileExtension.EndsWith(".webp") &&
                                                !fileExtension.EndsWith(".gif"))
                                            {
                                                logs.Add($"⚠ Skipped non-image file: {imageUrl}");
                                                continue;
                                            }

                                            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                                            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                            //var imageName = $"{itemCode}_{timestamp}.webp";
                                            var imageName = $"{itemCode}_{i + 1}.webp";
                                            var filePath = Path.Combine(uploadRoot, imageName);
                                            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                                            itemMaster.InvItemImages.Add(new Models.Inventory.Masters.InvItemImages
                                            {
                                                ImagePath = imageName,
                                                Active = true,
                                                IsDefault = false,
                                                //IsDefault = (i == 0),
                                                OrderNo = i + 1,
                                                Title = imageName,
                                                ImageSize = $"{(imageBytes.Length / 1024f):0.0} KB"
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            logs.Add($"❌ Failed to download image for SKU {itemCode} from {imageUrl}. Error: {ex.Message}");
                                        }
                                    }
                                }

                                // Save product
                                var saveValue = await SaveExcelProductEntry(itemMaster) as JsonResult;
                                dynamic result = saveValue.Value;
                              
                                if (result.success == false)
                                {
                                    failCount++;
                                    logs.Add($"❌ Import failed for SKU {itemCode}: {result.message}");
                                }
                                else
                                {
                                    successCount++;
                                    logs.Add($"✅ Successfully saved SKU {itemCode}");
                                }
                            }
                            catch (Exception exRow)
                            {
                                failCount++;
                                logs.Add($"❌ Error processing row: {exRow.Message}");
                            }
                        }

                        return Json(new
                        {
                            success = (failCount == 0),
                            message = (failCount == 0)
                                ? $"Excel import completed. Success: {successCount}, Failed: {failCount}"
                                : $"Excel import completed with errors. Success: {successCount}, Failed: {failCount}. " +
                                  $"Errors: {string.Join(" | ", logs.Where(l => l.StartsWith("❌")))}",
                            logs = logs
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Import failed: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveExcelProductEntry(Models.Inventory.Masters.ItemMaster itemMaster)
        {
            SqlConnection Con = new SqlConnection(ConnectionString);
            SqlTransaction Tx = null;
            String Result = "";
            try
            {
                Con.Open();
                Tx = Con.BeginTransaction();
                if (itemMaster.InvItemMaster.ID == 0)
                {
                    Result = ItemMaster.InsertExcelEntry(itemMaster, Con, Tx);
                }
                else
                {
                    Result = ItemMaster.InsertExcelEntry(itemMaster, Con, Tx);
                }
                bool isNumeric = int.TryParse(Result, out int n);
                int ID = 0;
                if (isNumeric)
                {
                    ID = Convert.ToInt32(Result);
                    foreach (var Item in itemMaster.InvItemImages)
                    {
                        Item.ItemID = ID;
                        ItemMaster.SaveExcelItemImages(Item, Con, Tx);

                    }
                }
                Tx.Commit();
                Con.Close();
                return Json(new { success = true, message = "Product added", transactionNo = ID });

            }
            catch (SqlException sqlEx)
            {
                if (Tx != null)
                {
                    Tx.Rollback();
                    Con.Close();
                }

                // Duplicate key violation
                if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                {
                    if (sqlEx.ToString().Contains("InvItemBarcode"))
                    {
                        return Json(new { success = false, message = "This barcode already exists. Please use a different one.", text = "Barcode" });

                    }
                    else if (sqlEx.ToString().Contains("InvItemMaster"))
                    {
                        return Json(new { success = false, message = "This item code already exists. Please use a different one.", text = "Item Code" });

                    }
                }

                // Other SQL errors
                return Json(new { success = false, message = "A database error occurred: " + sqlEx.Message });
            }
            catch (Exception Ex)
            {
                if (Tx != null)
                {
                    Tx.Rollback();
                    Con.Close();
                }
                return Json(new { success = false, message = "An error occurred: " + Ex.Message });
            }
        }

        public int GetMiscIdByName(string key, string Text)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                // Step 1: Try to get the existing ID
                var cmdSelect = new SqlCommand("SELECT ID FROM MaMisc WHERE [Key] = @key AND Value = @Text AND Active = 1", con);
                cmdSelect.Parameters.AddWithValue("@key", key);
                cmdSelect.Parameters.AddWithValue("@Text", Text);

                var result = cmdSelect.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }

                // Step 2: No row found — insert the new record
                var cmdInsert = new SqlCommand(
                    "INSERT INTO MaMisc ([Key], Value,Description, Active) OUTPUT INSERTED.ID VALUES (@key, @Text,@Text, 1)", con);

                cmdInsert.Parameters.AddWithValue("@key", key);
                cmdInsert.Parameters.AddWithValue("@Text", Text);

                var insertedId = cmdInsert.ExecuteScalar();
                return Convert.ToInt32(insertedId);
            }
        }



        private String ItemImagePath
        {
            get
            {
               // return "Resources/" + 1 + "/Ecommerce/ItemImage";
                return "Resources\\" + TenantID + "\\Ecommerce\\ItemImage";
            }
        }
    }

}
