using System;

namespace Vape_Store.Models
{
    public class CustomerLedgerEntry
    {
        public int LedgerEntryID { get; set; }
        public int CustomerID { get; set; }
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
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerPhone { get; set; }
    }

    public class CustomerLedgerSummary
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public DateTime? LastSaleDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }
}

