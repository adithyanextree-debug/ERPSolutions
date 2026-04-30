using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class MaMisc
    {
        public int? ID { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        public bool? Active { get; set; }
        public bool? AllowDelete { get; set; }
        public int? DevCode { get; set; }
        public string? code { get; set; }
        public string? ArabicDescription { get; set; }
        public string? ImagePath { get; set; }
        public int? OrderNo { get; set; }
    }
}
