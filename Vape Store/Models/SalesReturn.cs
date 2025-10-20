using System;
using System.Collections.Generic;

namespace Vape_Store.Models
{
    public class SalesReturn
    {
        public int ReturnID { get; set; }
        public string ReturnNumber { get; set; }
        public int SaleID { get; set; }
        public int CustomerID { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReturnReason { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string InvoiceNumber { get; set; }
        public List<SalesReturnItem> ReturnItems { get; set; } = new List<SalesReturnItem>();
    }
    
    public class SalesReturnItem
    {
        public int ReturnItemID { get; set; }
        public int ReturnID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        
        // Navigation properties
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
