
namespace ERPSample.Models.Inventory.Masters
{
    public class MainUnitMaster
    {
        public int ID { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public decimal Factor { get; set; }
        public bool IsComplex { get; set; }
        public string BasicUnit { get; set; }
        public bool AllowDelete { get; set; }= true;
        public int Precision { get; set; }
        public decimal Factor1 { get; set; }
        public bool Active { get; set; }
        public string ArabicName { get; set; }

    }
}
