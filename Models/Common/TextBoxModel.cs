using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Common
{
    public class TextBoxModel
    { 
        public String Type { get; set; }
        public String ID { get; set; }
        public String IDColumn { get; set; }
        public String cssClass { get; set; }
        public String DisplayText { get; set; }        
        public Object IDValue { get; set; }
        public Object Value { get; set; }
        public String AssignColumn { get; set; }
        public String LookupCriteria { get; set; }
        public Boolean IsMandatory { get; set; } = false;
        public Object IntParam1 { get; set; }
        public Object IntParam2 { get; set; }
        public Object IntParam3 { get; set; }
    }
    public class LookupModel
    {
        public String LookupID { get; set; }
        public String LookupDIV { get; set; }
        public String SearchText { get; set; }
        public String Criteria { get; set; }
        public String IDColumnName { get; set; }
        public String AssignColumnName { get; set; }
        public Int64 IntParam1 { get; set; } = 0;
        public Int64 IntParam2 { get; set; } = 0;
        public Int64 IntParam3 { get; set; } = 0;

    }
}
