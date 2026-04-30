using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class InvItemMaster
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string ItemName { get; set; }

        public string? OEMNo { get; set; }

        public string? PartNo { get; set; }

        public int? CategoryID { get; set; }

        public string? Manufacturer { get; set; }

        public string? ArabicName { get; set; }

        public string? ModelNo { get; set; }


        public string? Remarks { get; set; }

        public int? BrandID { get; set; }

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

    }
}
