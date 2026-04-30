using System;

namespace ERPSample.Models
{
    public class MaArea
    {
       public int ID { get; set; }
       public string Code { get; set; }
       public string Name { get; set; }
       public string Note { get; set; }
       public int ParentID { get; set; }
       public bool IsGroup { get; set; }
       public int CreatedBy { get; set; }
       public DateTime CreatedOn { get; set; }
       public int CreatedBranchID { get; set; }
       public bool Active { get; set; }
    }
}
