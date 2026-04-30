using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Inventory.Masters
{
    public class ItemMaster
    {
        private InvItemMaster _InvItemMaster;
        public InvItemMaster InvItemMaster
        {
            get
            {
                if (_InvItemMaster == null)
                {
                    _InvItemMaster = new InvItemMaster();
                }
                return _InvItemMaster;
            }
            set
            {
                _InvItemMaster = value;
            }
        }
        private List<InvItemUnits> _InvItemUnits;
        public List<InvItemUnits> InvItemUnits
        {
            get
            {
                if (_InvItemUnits == null)
                {
                    _InvItemUnits = new List<InvItemUnits>();
                }
                return _InvItemUnits;
            }
            set
            {
                _InvItemUnits = value;
            }
        }
        private List<InvItemImages> _InvItemImages;
        public List<InvItemImages> InvItemImages
        {
            get
            {
                if (_InvItemImages == null)
                {
                    _InvItemImages = new List<InvItemImages>();
                }
                return _InvItemImages;
            }
            set
            {
                _InvItemImages = value;
            }
        }
        private List<InvItemBarcodes> _InvItemBarcodes;
        public List<InvItemBarcodes> InvItemBarcodes
        {
            get
            {
                if (_InvItemBarcodes == null)
                {
                    _InvItemBarcodes = new List<InvItemBarcodes>();
                }
                return _InvItemBarcodes;
            }
            set
            {
                _InvItemBarcodes = value;
            }
        }
        private InvItemMasterExcel _InvItemMasterExcel;
        public InvItemMasterExcel InvItemMasterExcel
        {
            get
            {
                if (_InvItemMasterExcel == null)
                {
                    _InvItemMasterExcel = new InvItemMasterExcel();
                }
                return _InvItemMasterExcel;
            }
            set
            {
                _InvItemMasterExcel = value;
            }
        }
    }
    public class InvItemMaster
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string ItemName { get; set; }
        public string? ArabicName { get; set; }

        public string ShortDescription { get; set; }
        public string? ShortDescriptionArabic { get; set; }

        public string? OEMNo { get; set; }

        public string? PartNo { get; set; }

        public int? CategoryID { get; set; }

        public string? Manufacturer { get; set; }
        

        public string? ModelNo { get; set; }


        public string? Remarks { get; set; }

        public int? BrandID { get; set; }
        public int? ColorID { get; set; }
        public int? ArticleID { get; set; }
        public int? SizeID { get; set; }

        public bool? IsExpiry { get; set; }

        public string? PurchaseUnit { get; set; }

        public int? ExpiryPeriod { get; set; }

        [Required]
        public bool StockItem { get; set; }

        public string? SellingUnit { get; set; }

        [Required]
        public bool Active { get; set; }

        public decimal? Weight { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? CreatedUserID { get; set; }

        public int? ModifiedUserID { get; set; }

        public string Unit { get; set; }
        public int? TaxTypeID { get; set; }
        public string LongDescription { get; set; }
        public string ArabicLongDescription { get; set; }

        public bool SellOnEcommerce { get; set; }
        public string UrlName { get; set; }
        public decimal? Margin { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? BarCode { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
    }
    public class InvItemUnits
    {
        [Key]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string Unit { get; set; }
        public string BasicUnit { get; set; }
        public decimal Factor { get; set; }
        public bool Active { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Barcode { get; set; }
        public decimal? PurchaseRate { get; set; }
        public bool IsDefault { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal? OnlinePrice { get; set; }
    }

    public class InvItemBarcodes
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int UnitID { get; set; }
        public string Barcode { get; set; }
        public bool Active { get; set; }
    }
    public class InvItemMasterExcel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string SKU { get; set; }

        [Required]
        public string ProductName { get; set; }
        public string? ArabicName { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? ENGLISHProductDescription { get; set; }
        public string? ARABICProductDescription { get; set; }
        public string? BarCode { get; set; }
        //public bool? Active { get; set; }
        public decimal PRICE { get; set; }

       
    }
}
