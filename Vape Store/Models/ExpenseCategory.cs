using System;

namespace Vape_Store.Models
{
    public class ExpenseCategory
    {
        public int CategoryID { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int UserID { get; set; }

        // Navigation properties
        public string UserName { get; set; }
    }
}
