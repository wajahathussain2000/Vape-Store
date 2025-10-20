using System;

namespace Vape_Store.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemID { get; set; }
        public int PurchaseID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public int Bonus { get; set; }
        
        // Navigation properties
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
