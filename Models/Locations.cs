using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class Locations
    {
        [Key]
        public int ID { get; set; }
        public int LocationTypeID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
        public bool Active { get; set; }
        public int CreatedBranchID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class LocationBranchList
    {
        [Key]
        public int ID { get; set; }
        public int LocationID { get; set; }
        public int BranchID { get; set; }
        public bool IsDefault { get; set; }
        public bool Active { get; set; }
    }
}
