using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class FiMaSubGroup
    {
        [Key]
        public int ID { get; set; }
        public int OrderNo { get; set; }
        public int DivisionNo { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }
        public string MajorGroup { get; set; }
    }
}
