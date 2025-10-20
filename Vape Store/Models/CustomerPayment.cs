using System;

namespace Vape_Store.Models
{
    public class CustomerPayment
    {
        public int PaymentID { get; set; }
        public string VoucherNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal TotalDue { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string UserName { get; set; }
    }
}
