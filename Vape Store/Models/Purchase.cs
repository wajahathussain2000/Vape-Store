using System;
using System.Collections.Generic;

namespace Vape_Store.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }
        public string InvoiceNumber { get; set; }
        public int SupplierID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public string SupplierName { get; set; }
        public string UserName { get; set; }
        public List<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}

