using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class FiMaAccountCategory
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
    }
}
