using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
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
        public bool? IsDefault { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal? OnlinePrice { get; set; }
    }
}
