using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class Parties
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool Active { get; set; }
        public int AccountID { get; set; }
        public char Nature { get; set; }
    }
}
