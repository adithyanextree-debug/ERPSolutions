using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Inventory.Masters
{
    public class InvItemImages
    {
        [Key]
        public int ID { get; set; }
        public int? ItemID { get; set; }
        public string? Title { get; set; }
        public string? ArabicTitle { get; set; }
        public string? ImageSize { get; set; }
        public string? ImagePath { get; set; }
        public int? OrderNo { get; set; }
        public bool IsDefault { get; set; }
        public bool Active { get; set; }
    }
}
