using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class FiMaVouchers
    {
        [Key]
        public int ID { get; set; }
        public int PrimaryVoucherID { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool Active { get; set; }
    }
}
