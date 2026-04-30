using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models
{
    public class FiTransactions
    {
        [Key]
        public int? ID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? VoucherID { get; set; }

        public string? TransactionNo { get; set; }
        public long? SerialNo { get; set; }

        public bool? IsPostDated { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? ExchangeRate { get; set; }

        public int? RefPageTypeID { get; set; }
        public int? RefPageTableID { get; set; }
        public string? ReferenceNo { get; set; }

        public int? CompanyID { get; set; }
        public int? FinYearID { get; set; }

        public char? InstrumentType { get; set; }
        public string? InstrumentNo { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public string? InstrumentBank { get; set; }

        public string? CommonNarration { get; set; }

        public int? AddedBy { get; set; }
        public int? ApprovedBy { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public char? ApprovalStatus { get; set; }
        public string? ApproveNote { get; set; }

        public char? Action { get; set; }
        public int? StatusID { get; set; }

        public bool? IsAutoEntry { get; set; }
        public bool? Posted { get; set; }
        public bool? Active { get; set; }
        public bool? Cancelled { get; set; }

        public int? AccountID { get; set; }
        public string? Description { get; set; }
        public int? RefTransID { get; set; }

        public int? EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }

        public int? CostCentreID { get; set; }
        public int? PageID { get; set; }

        public string? MachineName { get; set; }

        public int? RowState { get; set; }

        public decimal? DeliveryCharge { get; set; }
        public decimal? Amount { get; set; }


        // -------- ONLY MISSING FIELDS ADDED BELOW --------

        public int? EcomCustomerID { get; set; }

        public int? PaymentTypeID { get; set; }

        public bool? PaymentCompleted { get; set; }

        public decimal? Discount { get; set; }

        public string? Language { get; set; }

        public bool? Processed { get; set; }

        public DateTime? DeliveryTimeSlotDate { get; set; }

        public int? DeliveryTimeSlotID { get; set; }
    }


    public class FiTransactionsDataTable : DataTable
    {
        public FiTransactionsDataTable()
        {
            Columns.Add("ID", typeof(int));
            Columns.Add("Date", typeof(DateTime));
            Columns.Add("EffectiveDate", typeof(DateTime));
            Columns.Add("VoucherID", typeof(int));

            Columns.Add("SerialNo", typeof(long));
            Columns.Add("TransactionNo", typeof(string));

            Columns.Add("IsPostDated", typeof(bool));
            Columns.Add("CurrencyID", typeof(int));
            Columns.Add("ExchangeRate", typeof(decimal));

            Columns.Add("RefPageTypeID", typeof(int));
            Columns.Add("RefPageTableID", typeof(int));
            Columns.Add("ReferenceNo", typeof(string));

            Columns.Add("CompanyID", typeof(int));
            Columns.Add("FinYearID", typeof(int));

            Columns.Add("InstrumentType", typeof(string));
            Columns.Add("InstrumentNo", typeof(string));
            Columns.Add("InstrumentDate", typeof(DateTime));
            Columns.Add("InstrumentBank", typeof(string));

            Columns.Add("CommonNarration", typeof(string));

            Columns.Add("AddedBy", typeof(int));
            Columns.Add("ApprovedBy", typeof(int));

            Columns.Add("AddedDate", typeof(DateTime));
            Columns.Add("ApprovedDate", typeof(DateTime));

            Columns.Add("ApprovalStatus", typeof(string));
            Columns.Add("ApproveNote", typeof(string));

            Columns.Add("Action", typeof(string));
            Columns.Add("StatusID", typeof(int));

            Columns.Add("IsAutoEntry", typeof(bool));
            Columns.Add("Posted", typeof(bool));
            Columns.Add("Active", typeof(bool));
            Columns.Add("Cancelled", typeof(bool));

            Columns.Add("AccountID", typeof(int));

            Columns.Add("Description", typeof(string));
            Columns.Add("RefTransID", typeof(int));

            Columns.Add("EditedBy", typeof(int));
            Columns.Add("EditedDate", typeof(DateTime));

            Columns.Add("CostCentreID", typeof(int));
            Columns.Add("PageID", typeof(int));

            Columns.Add("MachineName", typeof(string));

            Columns.Add("EcomCustomerID", typeof(int));
            Columns.Add("PaymentTypeID", typeof(int));

            Columns.Add("DeliveryCharge", typeof(decimal));
            Columns.Add("PaymentCompleted", typeof(bool));
            Columns.Add("Discount", typeof(decimal));

            Columns.Add("Language", typeof(string));
            Columns.Add("Processed", typeof(bool));

            Columns.Add("DeliveryTimeSlotDate", typeof(DateTime));
            Columns.Add("DeliveryTimeSlotID", typeof(int));

            Columns.Add("RowState", typeof(int));
        }
    }

}
