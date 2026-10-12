using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class FiChequeEntry
    {
        public int? ID { get; set; }
        public string? CardType { get; set; }
        public string? Commission { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int? ClrDays { get; set; }
        public int? BankID { get; set; }
        public string? BankName { get; set; }
        public string? Status { get; set; }
        public int? PartyID { get; set; }
        public int? RowState { get; set; }
    }
    public class FiChequeEntryDataTable : DataTable
    {
        public FiChequeEntryDataTable()
        {
            TableName = "FiChequeEntries";

            // Added from FiChequeEntry
            Columns.Add("ID", typeof(int));
            Columns.Add("CardType", typeof(string));
            Columns.Add("Commission", typeof(string));
            Columns.Add("ChequeNo", typeof(string));
            Columns.Add("ChequeDate", typeof(DateTime));
            Columns.Add("ClrDays", typeof(int));
            Columns.Add("BankID", typeof(int));
            Columns.Add("BankName", typeof(string));
            Columns.Add("Status", typeof(string));
            Columns.Add("PartyID", typeof(int));

            Columns.Add("RowState", typeof(int));
        }
    }

}
