using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class MaMiscKeys
    {
        public int ID { get; set; }

        [Key]
        public string Name { get; set; }

    }
}
