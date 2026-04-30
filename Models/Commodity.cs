using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class Commodity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Code { get; set; }
        public int TypeofWoodID { get; set; }
        public char Category { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte ActiveFlag { get; set; }
        public int CreatedBranchID { get; set; }
        public char StockType { get; set; }
        public decimal MinQty { get; set; }
        public decimal MaxQty { get; set; }

        [Column(TypeName = "money")]
        public decimal FloorRate { get; set; }
    }
}
