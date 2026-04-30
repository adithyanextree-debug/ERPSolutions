using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Inventory
{
    public class UnitMaster
    {
        [Key]
        public string Unit { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Factor { get; set; }
        [Required]
        public bool IsComplex { get; set; }
        public string? BasicUnit { get; set; }
        [Required]
        public bool AllowDelete { get; set; }
        public int? Precision { get; set; }

        public decimal? Factor1 { get; set; }
        public bool? Active { get; set; }
        public string? ArabicName { get; set; }
    }
}
