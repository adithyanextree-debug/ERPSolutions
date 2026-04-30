using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class MaTaxType
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int SalePurchaseModeID { get; set; }
        public int TaxAccountID { get; set; }
        public decimal? SalesPerc { get; set; }
        public decimal? PurchasePerc { get; set; }
        public bool? Active { get; set; }
        public string? Note { get; set; }
        public int? TaxMiscID { get; set; }
        public int? ReceivableAccountID { get; set; }
        public int? PayableAccountID { get; set; }
    }
}
