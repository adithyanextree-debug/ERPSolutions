using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Ecommerce.Transactions
{
    public class EcomOrderStatus
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int? AddressID { get; set; }
        public int VID { get; set; }
        public DateTime Date { get; set; }
        public string? Remarks { get; set; }
        public int? StatusID { get; set; }
    }
}
