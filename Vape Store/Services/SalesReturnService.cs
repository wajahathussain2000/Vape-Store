using System;
using System.Collections.Generic;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.Repositories;

namespace Vape_Store.Services
{
    public class SalesReturnService
    {
        private readonly SalesReturnRepository _salesReturnRepository;
        private readonly InventoryService _inventoryService;
        private readonly CustomerRepository _customerRepository;

        public SalesReturnService()
        {
            _salesReturnRepository = new SalesReturnRepository();
            _inventoryService = new InventoryService();
            _customerRepository = new CustomerRepository();
        }

        /// <summary>
        /// Populate original invoice details for the sales return
        /// </summary>
        public void PopulateOriginalInvoiceDetails(SalesReturn salesReturn, Sale originalSale)
        {
            if (originalSale != null)
            {
                salesReturn.OriginalInvoiceNumber = originalSale.InvoiceNumber;
                salesReturn.OriginalInvoiceDate = originalSale.SaleDate;
                salesReturn.OriginalInvoiceTotal = originalSale.TotalAmount;
                
                // Determine if this is a full return
                decimal totalReturnAmount = salesReturn.ReturnItems.Sum(item => item.SubTotal);
                salesReturn.IsFullyReturned = (totalReturnAmount >= originalSale.TotalAmount);
                salesReturn.ReturnStatus = salesReturn.IsFullyReturned ? "Full" : "Partial";
            }
        }

        /// <summary>
        /// Process a comprehensive sales return with validation and stock management
        /// </summary>
        public bool ProcessSalesReturn(SalesReturn salesReturn)
        {
            try
            {
                // Validate return eligibility for each item
                foreach (var item in salesReturn.ReturnItems)
                {
                    if (!ValidateReturnEligibility(salesReturn.SaleID, item.ProductID, item.Quantity))
                    {
                        throw new Exception($"Cannot return {item.Quantity} units of {item.ProductName}. Exceeds available quantity for return.");
                    }
                }

                // Validate customer exists
                var customer = _customerRepository.GetCustomerById(salesReturn.CustomerID);
                if (customer == null)
                {
                    throw new Exception("Customer not found.");
                }

                // Generate return number if not provided
                if (string.IsNullOrEmpty(salesReturn.ReturnNumber))
                {
                    salesReturn.ReturnNumber = _salesReturnRepository.GetNextReturnNumber();
                }

                // Set creation date if not provided
                if (salesReturn.CreatedDate == DateTime.MinValue)
                {
                    salesReturn.CreatedDate = DateTime.Now;
                }

                // Calculate total amount if not set
                if (salesReturn.TotalAmount == 0)
                {
                    salesReturn.TotalAmount = salesReturn.ReturnItems.Sum(item => item.SubTotal);
                }

                // Process the return with stock management
                bool success = _salesReturnRepository.AddSalesReturn(salesReturn);

                if (success)
                {
                    // Log return activity (you can extend this to add audit trail)
                    LogReturnActivity(salesReturn, "RETURN_PROCESSED");
                }

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing sales return: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate if a return is eligible based on original sale and previous returns
        /// </summary>
        public bool ValidateReturnEligibility(int saleId, int productId, int requestedQuantity)
        {
            try
            {
                return _salesReturnRepository.ValidateReturnEligibility(saleId, productId, requestedQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validating return eligibility: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all sales returns with comprehensive filtering
        /// </summary>
        public List<SalesReturn> GetAllSalesReturns()
        {
            try
            {
                return _salesReturnRepository.GetAllSalesReturns();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales returns: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get sales returns by customer
        /// </summary>
        public List<SalesReturn> GetReturnsByCustomer(int customerId)
        {
            try
            {
                return _salesReturnRepository.GetReturnsByCustomer(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving returns by customer: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get sales returns by date range
        /// </summary>
        public List<SalesReturn> GetReturnsByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return _salesReturnRepository.GetSalesReturnsByDateRange(fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving returns by date range: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get recent returns for dashboard
        /// </summary>
        public List<SalesReturn> GetRecentReturns(int count = 10)
        {
            try
            {
                return _salesReturnRepository.GetRecentReturns(count);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent returns: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get return statistics
        /// </summary>
        public ReturnStatistics GetReturnStatistics(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var returns = _salesReturnRepository.GetSalesReturnsByDateRange(fromDate, toDate);
                
                return new ReturnStatistics
                {
                    TotalReturns = returns.Count,
                    TotalAmount = returns.Sum(r => r.TotalAmount),
                    AverageReturnAmount = returns.Count > 0 ? returns.Average(r => r.TotalAmount) : 0,
                    TopReturnReasons = returns
                        .Where(r => !string.IsNullOrEmpty(r.ReturnReason))
                        .GroupBy(r => r.ReturnReason)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key, g => g.Count())
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating return statistics: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update a sales return with proper stock management
        /// </summary>
        public bool UpdateSalesReturn(SalesReturn salesReturn)
        {
            try
            {
                // Validate return eligibility for each item
                foreach (var item in salesReturn.ReturnItems)
                {
                    if (!ValidateReturnEligibility(salesReturn.SaleID, item.ProductID, item.Quantity))
                    {
                        throw new Exception($"Cannot return {item.Quantity} units of {item.ProductName}. Exceeds available quantity for return.");
                    }
                }

                bool success = _salesReturnRepository.UpdateSalesReturn(salesReturn);

                if (success)
                {
                    LogReturnActivity(salesReturn, "RETURN_UPDATED");
                }

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating sales return: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete a sales return with proper stock reversal
        /// </summary>
        public bool DeleteSalesReturn(int returnId)
        {
            try
            {
                bool success = _salesReturnRepository.DeleteSalesReturn(returnId);

                if (success)
                {
                    LogReturnActivity(new SalesReturn { ReturnID = returnId }, "RETURN_DELETED");
                }

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting sales return: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Get return reasons for dropdown/selection
        /// </summary>
        public List<string> GetCommonReturnReasons()
        {
            return new List<string>
            {
                "Defective Product",
                "Wrong Item",
                "Size/Color Mismatch",
                "Customer Changed Mind",
                "Quality Issue",
                "Damaged in Transit",
                "Not as Described",
                "Duplicate Order",
                "Late Delivery",
                "Other"
            };
        }

        /// <summary>
        /// Check if a sale is eligible for return (within return period, etc.)
        /// </summary>
        public bool IsSaleEligibleForReturn(int saleId, int returnPeriodDays = 30)
        {
            try
            {
                var sales = _salesReturnRepository.GetSalesForReturn();
                var sale = sales.FirstOrDefault(s => s.SaleID == saleId);
                
                if (sale == null)
                    return false;

                // Check if sale is within return period
                var daysSinceSale = (DateTime.Now - sale.SaleDate).Days;
                return daysSinceSale <= returnPeriodDays;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking sale eligibility: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Log return activity for audit trail
        /// </summary>
        private void LogReturnActivity(SalesReturn salesReturn, string action)
        {
            try
            {
                // This can be extended to log to an audit table
                // For now, we'll just ensure the action is tracked
                // You can implement a proper audit logging system here
                Console.WriteLine($"Return Activity: {action} - Return ID: {salesReturn.ReturnID} - Date: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                // Don't throw exception for logging failures
                Console.WriteLine($"Error logging return activity: {ex.Message}");
            }
        }

        /// <summary>
        /// Get available sales that can be returned (excluding already fully returned invoices)
        /// </summary>
        public List<Sale> GetAvailableSalesForReturn()
        {
            return _salesReturnRepository.GetAvailableSalesForReturn();
        }

        /// <summary>
        /// Check if an invoice is fully returned
        /// </summary>
        public bool IsInvoiceFullyReturned(int saleId)
        {
            return _salesReturnRepository.IsInvoiceFullyReturned(saleId);
        }
    }

    /// <summary>
    /// Return statistics model
    /// </summary>
    public class ReturnStatistics
    {
        public int TotalReturns { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageReturnAmount { get; set; }
        public Dictionary<string, int> TopReturnReasons { get; set; } = new Dictionary<string, int>();
    }
}
