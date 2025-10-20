using System;

namespace Vape_Store.Models
{
    public class SupplierPayment
    {
        public int PaymentID { get; set; }
        public string VoucherNumber { get; set; }
        public int SupplierID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public string SupplierName { get; set; }
        public string SupplierContact { get; set; }
        public string UserName { get; set; }
    }
}
