using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class FiTransactionEntries
    {
        [Key]
        public int? ID { get; set; }
        public int? TransactionID { get; set; }
        public string? DrCr { get; set; }
        public char? Nature { get; set; }
        public int? AccountID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? FCAmount { get; set; }
        public DateTime? BankDate { get; set; }
        public int? RefPageTypeID { get; set; }
        public int? RefPageTableID { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Description { get; set; }
        public string? TranType { get; set; }
        public DateTime? DueDate { get; set; }
        public int? RefTransID { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? ExchangeRate { get; set; }
        public int? RefTransactionID { get; set; }
        public decimal? ExchRate { get; set; }
        public decimal? TaxPerc { get; set; }
        public int? RowState { get; set; }
       
        
    }

    public class FiTransactionEntriesDataTable : DataTable
    {
        public FiTransactionEntriesDataTable()
        {
            TableName = "FiTransactionEntries";

            Columns.Add("ID", typeof(int));
            Columns.Add("TransactionID", typeof(int));
            Columns.Add("DrCr", typeof(string));
            Columns.Add("Nature", typeof(char));
            Columns.Add("AccountID", typeof(int));
            Columns.Add("Amount", typeof(decimal));
            Columns.Add("FCAmount", typeof(decimal));
            Columns.Add("BankDate", typeof(DateTime));
            Columns.Add("RefPageTypeID", typeof(int));
            Columns.Add("RefPageTableID", typeof(int));
            Columns.Add("ReferenceNo", typeof(string));
            Columns.Add("Description", typeof(string));
            Columns.Add("TranType", typeof(string));
            Columns.Add("DueDate", typeof(DateTime));
            Columns.Add("RefTransID", typeof(int));
            Columns.Add("CurrencyID", typeof(int));
            Columns.Add("ExchangeRate", typeof(decimal));
            Columns.Add("RefTransactionID", typeof(int));
            Columns.Add("ExchRate", typeof(decimal));
            Columns.Add("TaxPerc", typeof(decimal));
            Columns.Add("RowState", typeof(int));
        }
    }

}
