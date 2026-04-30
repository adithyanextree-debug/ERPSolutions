namespace ERPSample.Models.Inventory.Transactions
{
    public class TransactionHelperClass
    {
        public static class FiTransactionsHelper
        {
            public static FiTransactionsDataTable ToDataTable(
            IEnumerable<ERPSample.Models.FiTransactions> transactions)
            {
                var dt = new FiTransactionsDataTable();

                foreach (var t in transactions)
                {
                    dt.Rows.Add(

                        // ---- EXACT ORDER FROM SQL ----

                        t.ID ?? (object)DBNull.Value,                     // 1
                        t.Date ?? (object)DBNull.Value,                   // 2
                        t.EffectiveDate ?? (object)DBNull.Value,          // 3
                        t.VoucherID ?? (object)DBNull.Value,              // 4

                        t.SerialNo ?? (object)DBNull.Value,               // 5
                        t.TransactionNo ?? (object)DBNull.Value,          // 6

                        t.IsPostDated ?? (object)DBNull.Value,            // 7
                        t.CurrencyID ?? (object)DBNull.Value,             // 8
                        t.ExchangeRate ?? (object)DBNull.Value,           // 9

                        t.RefPageTypeID ?? (object)DBNull.Value,          // 10
                        t.RefPageTableID ?? (object)DBNull.Value,         // 11
                        t.ReferenceNo ?? (object)DBNull.Value,            // 12

                        t.CompanyID ?? (object)DBNull.Value,              // 13
                        t.FinYearID ?? (object)DBNull.Value,              // 14

                        t.InstrumentType ?? (object)DBNull.Value,         // 15
                        t.InstrumentNo ?? (object)DBNull.Value,           // 16
                        t.InstrumentDate ?? (object)DBNull.Value,         // 17
                        t.InstrumentBank ?? (object)DBNull.Value,         // 18

                        t.CommonNarration ?? (object)DBNull.Value,        // 19

                        t.AddedBy ?? (object)DBNull.Value,                // 20
                        t.ApprovedBy ?? (object)DBNull.Value,             // 21

                        t.AddedDate ?? (object)DBNull.Value,              // 22
                        t.ApprovedDate ?? (object)DBNull.Value,           // 23

                        t.ApprovalStatus ?? (object)DBNull.Value,         // 24
                        t.ApproveNote ?? (object)DBNull.Value,            // 25

                        t.Action ?? (object)DBNull.Value,                 // 26
                        t.StatusID ?? (object)DBNull.Value,               // 27

                        t.IsAutoEntry ?? (object)DBNull.Value,            // 28
                        t.Posted ?? (object)DBNull.Value,                 // 29
                        t.Active ?? (object)DBNull.Value,                 // 30
                        t.Cancelled ?? (object)DBNull.Value,              // 31

                        t.AccountID ?? (object)DBNull.Value,              // 32

                        t.Description ?? (object)DBNull.Value,            // 33
                        t.RefTransID ?? (object)DBNull.Value,             // 34

                        t.EditedBy ?? (object)DBNull.Value,               // 35
                        t.EditedDate ?? (object)DBNull.Value,             // 36

                        t.CostCentreID ?? (object)DBNull.Value,           // 37
                        t.PageID ?? (object)DBNull.Value,                 // 38

                        t.MachineName ?? (object)DBNull.Value,            // 39

                        // ---- MISSING ECOM FIELDS ADDED ----
                        t.EcomCustomerID ?? (object)DBNull.Value,         // 40
                        t.PaymentTypeID ?? (object)DBNull.Value,          // 41

                        t.DeliveryCharge ?? (object)DBNull.Value,         // 42
                        t.PaymentCompleted ?? (object)DBNull.Value,       // 43
                        t.Discount ?? (object)DBNull.Value,               // 44

                        t.Language ?? (object)DBNull.Value,               // 45
                        t.Processed ?? (object)DBNull.Value,              // 46

                        t.DeliveryTimeSlotDate ?? (object)DBNull.Value,   // 47
                        t.DeliveryTimeSlotID ?? (object)DBNull.Value,     // 48

                        t.RowState ?? (object)DBNull.Value                // 49
                    );
                }

                return dt;
            }

        }

        public static class FiTransactionAdditionalsHelper
        {
            public static FiTransactionAdditionalsDataTable ToDataTable(
                IEnumerable<ERPSample.Models.FiTransactionAdditionals> additionals)
            {
                var dt = new FiTransactionAdditionalsDataTable();

                foreach (var a in additionals)
                {
                    dt.Rows.Add(
                        a.TransactionID ?? (object)DBNull.Value,
                        a.RefTransID1 ?? (object)DBNull.Value,
                        a.RefTransID2 ?? (object)DBNull.Value,
                        a.TypeID ?? (object)DBNull.Value,
                        a.ModeID ?? (object)DBNull.Value,
                        a.MeasureTypeID ?? (object)DBNull.Value,
                        a.LoadMeasureTypeID ?? (object)DBNull.Value,
                        a.ConsignTermID ?? (object)DBNull.Value,
                        a.FromLocationID ?? (object)DBNull.Value,
                        a.ToLocationID ?? (object)DBNull.Value,

                        a.ExchangeRate1 ?? (object)DBNull.Value,
                        a.AdvanceExRate ?? (object)DBNull.Value,
                        a.CustomsExRate ?? (object)DBNull.Value,

                        a.ApprovalDays ?? (object)DBNull.Value,
                        a.WorkflowDays ?? (object)DBNull.Value,
                        a.PostedBranchID ?? (object)DBNull.Value,
                        a.ShipBerthDate ?? (object)DBNull.Value,
                        a.IsBit ?? (object)DBNull.Value,

                        a.Name ?? (object)DBNull.Value,
                        a.Code ?? (object)DBNull.Value,
                        a.Address ?? (object)DBNull.Value,
                        a.Rate ?? (object)DBNull.Value,
                        a.SystemRate ?? (object)DBNull.Value,

                        a.Period ?? (object)DBNull.Value,
                        a.Days ?? (object)DBNull.Value,
                        a.LCOptionID ?? (object)DBNull.Value,
                        a.LCNo ?? (object)DBNull.Value,
                        a.LCAmt ?? (object)DBNull.Value,
                        a.AvailableLCAmt ?? (object)DBNull.Value,
                        a.CreditAmt ?? (object)DBNull.Value,
                        a.MarginAmt ?? (object)DBNull.Value,

                        a.InterestAmt ?? (object)DBNull.Value,
                        a.AvailableAmt ?? (object)DBNull.Value,
                        a.AllocationPerc ?? (object)DBNull.Value,
                        a.InterestPerc ?? (object)DBNull.Value,
                        a.TolerencePerc ?? (object)DBNull.Value,

                        a.CountryID ?? (object)DBNull.Value,
                        a.CountryOfOriginID ?? (object)DBNull.Value,
                        a.MaxDays ?? (object)DBNull.Value,
                        a.DocumentNo ?? (object)DBNull.Value,
                        a.DocumentDate ?? (object)DBNull.Value,
                        a.BEMaxDays ?? (object)DBNull.Value,

                        a.EntryDate ?? (object)DBNull.Value,
                        a.EntryNo ?? (object)DBNull.Value,
                        a.ApplicationCode ?? (object)DBNull.Value,
                        a.BankAddress ?? (object)DBNull.Value,
                        a.Unit ?? (object)DBNull.Value,
                        a.Amount ?? (object)DBNull.Value,

                        a.AcceptDate ?? (object)DBNull.Value,
                        a.ExpiryDate ?? (object)DBNull.Value,
                        a.DueDate ?? (object)DBNull.Value,
                        a.OpenDate ?? (object)DBNull.Value,
                        a.CloseDate ?? (object)DBNull.Value,
                        a.StartDate ?? (object)DBNull.Value,
                        a.EndDate ?? (object)DBNull.Value,
                        a.ClearDate ?? (object)DBNull.Value,
                        a.ReceiveDate ?? (object)DBNull.Value,
                        a.SubmitDate ?? (object)DBNull.Value,
                        a.EndTime ?? (object)DBNull.Value,
                        a.HandOverTime ?? (object)DBNull.Value,

                        a.LorryHireRate ?? (object)DBNull.Value,
                        a.QtyPerLoad ?? (object)DBNull.Value,
                        a.PassNo ?? (object)DBNull.Value,
                        a.ReferenceDate ?? (object)DBNull.Value,
                        a.ReferenceNo ?? (object)DBNull.Value,
                        a.AuditNote ?? (object)DBNull.Value,
                        a.Terms ?? (object)DBNull.Value,

                        a.FirmID ?? (object)DBNull.Value,
                        a.VehicleID ?? (object)DBNull.Value,
                        a.WeekDays ?? (object)DBNull.Value,
                        a.BankWeekDays ?? (object)DBNull.Value,
                        a.RecommendByID ?? (object)DBNull.Value,
                        a.RecommendDate ?? (object)DBNull.Value,
                        a.RecommendNote ?? (object)DBNull.Value,
                        a.RecommendStatus ?? (object)DBNull.Value,
                        a.IsHigherApproval ?? (object)DBNull.Value,

                        a.LCApplnTransID ?? (object)DBNull.Value,
                        a.InLocID ?? (object)DBNull.Value,
                        a.OutLocID ?? (object)DBNull.Value,
                        a.ExchangeRate2 ?? (object)DBNull.Value,
                        a.AccountID ?? (object)DBNull.Value,
                        a.RouteID ?? (object)DBNull.Value,
                        a.AccountID2 ?? (object)DBNull.Value,
                        a.Hours ?? (object)DBNull.Value,
                        a.Year ?? (object)DBNull.Value,
                        a.AreaID ?? (object)DBNull.Value,
                        a.OtherBranchID ?? (object)DBNull.Value,
                        a.TaxFormID ?? (object)DBNull.Value,
                        a.PriceCategoryID ?? (object)DBNull.Value,
                        a.IsClosed ?? (object)DBNull.Value,
                        a.DepartmentID ?? (object)DBNull.Value,
                        a.RowState ?? (object)DBNull.Value

                    );
                }

                return dt;
            }
        }

        public static class FiTransactionEntriesHelper
        {
            public static FiTransactionEntriesDataTable ToDataTable(
                IEnumerable<ERPSample.Models.FiTransactionEntries> entries)
            {
                var dt = new FiTransactionEntriesDataTable();

                foreach (var e in entries)
                {
                    dt.Rows.Add(
                        e.ID ?? (object)DBNull.Value,
                        e.TransactionID,
                        e.DrCr ?? (object)DBNull.Value,
                        e.Nature ?? (object)DBNull.Value,
                        e.AccountID,
                        e.Amount != null ? e.Amount : 0m,
                        e.FCAmount ?? (object)DBNull.Value,
                        e.BankDate ?? (object)DBNull.Value,
                        e.RefPageTypeID ?? (object)DBNull.Value,
                        e.RefPageTableID ?? (object)DBNull.Value,
                        e.ReferenceNo ?? (object)DBNull.Value,
                        e.Description ?? (object)DBNull.Value,
                        e.TranType ?? (object)DBNull.Value,
                        e.DueDate ?? (object)DBNull.Value,
                        e.RefTransID ?? (object)DBNull.Value,
                        e.CurrencyID ?? (object)DBNull.Value,
                        e.ExchangeRate ?? (object)DBNull.Value,
                        e.RefTransactionID ?? (object)DBNull.Value,
                        e.ExchRate ?? (object)DBNull.Value,
                        e.TaxPerc ?? (object)DBNull.Value,
                        e.RowState ?? (object)DBNull.Value

                    );
                }

                return dt;
            }
        }

        public static class InvTransItemsHelper
        {
            public static InvTransItemsDataTable ToDataTable(
                IEnumerable<ERPSample.Models.InvTransItems> items)
            {
                var dt = new InvTransItemsDataTable();

                foreach (var i in items)
                {
                    dt.Rows.Add(
                        i.ID ?? (object)DBNull.Value,
                        i.TransactionID ?? (object)DBNull.Value,
                        i.ItemID ?? (object)DBNull.Value,
                        i.RefTransID1 ?? (object)DBNull.Value,
                        i.Unit ?? (object)DBNull.Value,
                        i.Qty ?? (object)DBNull.Value,
                        i.BasicQty ?? (object)DBNull.Value,
                        i.Pcs ?? (object)DBNull.Value,

                        i.Rate ?? (object)DBNull.Value,
                        i.AdvanceRate ?? (object)DBNull.Value,
                        i.OtherRate ?? (object)DBNull.Value,

                        i.MasterMiscID1 ?? (object)DBNull.Value,
                        i.RowType ?? (object)DBNull.Value,
                        i.Description ?? (object)DBNull.Value,
                        i.Remarks ?? (object)DBNull.Value,
                        i.IsBit ?? (object)DBNull.Value,
                        i.InvAvgCostID ?? (object)DBNull.Value,
                        i.IsReturn ?? (object)DBNull.Value,

                        i.Discount ?? (object)DBNull.Value,
                        i.Additional ?? (object)DBNull.Value,
                        i.Factor ?? (object)DBNull.Value,

                        i.CommodityID ?? (object)DBNull.Value,
                        i.AccountID ?? (object)DBNull.Value,
                        i.TransactionEntryID ?? (object)DBNull.Value,

                        i.LengthFt ?? (object)DBNull.Value,
                        i.LengthIn ?? (object)DBNull.Value,
                        i.LengthCm ?? (object)DBNull.Value,

                        i.GirthFt ?? (object)DBNull.Value,
                        i.GirthIn ?? (object)DBNull.Value,
                        i.GirthCm ?? (object)DBNull.Value,

                        i.ThicknessFt ?? (object)DBNull.Value,
                        i.ThicknessIn ?? (object)DBNull.Value,
                        i.ThicknessCm ?? (object)DBNull.Value,

                        i.ShortageQty ?? (object)DBNull.Value,
                        i.AvgCostID ?? (object)DBNull.Value,
                        i.RefTransItemID ?? (object)DBNull.Value,
                        i.Status ?? (object)DBNull.Value,
                        i.Cancel ?? (object)DBNull.Value,
                        i.MeasuredByID ?? (object)DBNull.Value,

                        i.FinishDate ?? (object)DBNull.Value,
                        i.UpdateDate ?? (object)DBNull.Value,
                        i.IsSameForPcs ?? (object)DBNull.Value,
                        i.StockQty ?? (object)DBNull.Value,

                        i.RefID ?? (object)DBNull.Value,
                        i.InLocID ?? (object)DBNull.Value,
                        i.OutLocID ?? (object)DBNull.Value,
                        i.BatchNo ?? (object)DBNull.Value,

                        i.Margin ?? (object)DBNull.Value,
                        i.DiscountPerc ?? (object)DBNull.Value,
                        i.TaxPerc ?? (object)DBNull.Value,
                        i.TaxValue ?? (object)DBNull.Value,

                        i.TaxTypeID ?? (object)DBNull.Value,
                        i.SizeMasterID ?? (object)DBNull.Value,
                        i.TranType ?? (object)DBNull.Value,

                        i.CostPerc ?? (object)DBNull.Value,
                        i.ManufactureDate ?? (object)DBNull.Value,
                        i.ExpiryDate ?? (object)DBNull.Value,
                        i.FOCQty ?? (object)DBNull.Value,

                        i.GroupItemID ?? (object)DBNull.Value,
                        i.PriceCategoryID ?? (object)DBNull.Value,
                        i.RateDiscPerc ?? (object)DBNull.Value,
                        i.RateDisc ?? (object)DBNull.Value,

                        i.SerialNo ?? (object)DBNull.Value,
                        i.TempQty ?? (object)DBNull.Value,
                        i.ReplaceQty ?? (object)DBNull.Value,

                        i.PrintedMRP ?? (object)DBNull.Value,
                        i.PrintedRate ?? (object)DBNull.Value,
                        i.PTSRate ?? (object)DBNull.Value,
                        i.PTRRate ?? (object)DBNull.Value,
                        i.TempRate ?? (object)DBNull.Value,

                        i.StockItemID ?? (object)DBNull.Value,
                        i.Visible ?? (object)DBNull.Value,
                        i.TaxAccountID ?? (object)DBNull.Value,
                        i.RowState ?? (object)DBNull.Value

                    );
                }

                return dt;
            }
        }


    }
}
