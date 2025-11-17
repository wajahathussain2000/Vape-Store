using System;

namespace Vape_Store.Models
{
    public class SupplierLedgerEntry
    {
        public int LedgerEntryID { get; set; }
        public int SupplierID { get; set; }
        public DateTime EntryDate { get; set; }
        public string ReferenceType { get; set; }
        public int? ReferenceID { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierPhone { get; set; }
    }

    public class SupplierLedgerSummary
    {
        public int SupplierID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Phone { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }
}

