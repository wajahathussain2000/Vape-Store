using System;

namespace Vape_Store.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public string Barcode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
    }
}
