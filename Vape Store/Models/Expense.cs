using System;

namespace Vape_Store.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; }
        public string ExpenseCode { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; } // Draft, Submitted, Approved, Rejected
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        // Navigation properties
        public string CategoryName { get; set; }
        public string Category { get; set; }
        public string UserName { get; set; }
    }
}
