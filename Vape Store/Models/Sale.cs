using System;
using System.Collections.Generic;

namespace Vape_Store.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public string InvoiceNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // New audit and enhancement fields
        public DateTime? LastModified { get; set; }
        public int? ModifiedBy { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public byte[] BarcodeImage { get; set; }
        public string BarcodeData { get; set; }
        
        // Navigation properties
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string ModifiedByName { get; set; }
        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
    
    public class SaleItem
    {
        public int SaleItemID { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime? LastModified { get; set; }
        
        // Navigation properties
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
