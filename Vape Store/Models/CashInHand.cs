using System;

namespace Vape_Store.Models
{
    public class CashInHand
    {
        public int CashInHandID { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal OpeningCash { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal Expenses { get; set; }
        public decimal ClosingCash { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public string UserName { get; set; }
    }
}
