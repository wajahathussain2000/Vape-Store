using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Services
{
    public class DashboardService
    {
        private BusinessDateService _businessDateService;
        
        public DashboardService()
        {
            _businessDateService = new BusinessDateService();
        }
        public DashboardStats GetDashboardStats()
        {
            return GetDashboardStatsByDateRange(null, null);
        }
        
        public DashboardStats GetDashboardStatsByDateRange(DateTime? fromDate, DateTime? toDate)
        {
            var stats = new DashboardStats();
            
            try
            {
                DateTime businessDate = _businessDateService.GetCurrentBusinessDate();
                
                // If viewing current day and it's closed, return 0 for all today's stats
                bool isCurrentDayClosed = false;
                if (fromDate == null && toDate == null)
                {
                    isCurrentDayClosed = _businessDateService.IsDateClosed(businessDate);
                }
                
                // Get today's sales (or date range sales)
                if (fromDate.HasValue && toDate.HasValue)
                {
                    stats.TodaySales = GetSalesByDateRange(fromDate.Value, toDate.Value);
                }
                else
                {
                    stats.TodaySales = isCurrentDayClosed ? 0 : GetTodaySales();
                }
                
                // Get monthly sales
                stats.MonthlySales = GetMonthlySales();
                
                // Get monthly purchases (or date range purchases)
                if (fromDate.HasValue && toDate.HasValue)
                {
                    stats.MonthlyPurchases = GetPurchasesByDateRange(fromDate.Value, toDate.Value);
                }
                else
                {
                    stats.MonthlyPurchases = isCurrentDayClosed ? 0 : GetMonthlyPurchases();
                }
                
                // Get total products
                stats.TotalProducts = GetTotalProducts();
                
                // Get low stock items
                stats.LowStockItems = GetLowStockItems();
                
                // Get total customers
                stats.TotalCustomers = GetTotalCustomers();
                
                // Get active users
                stats.ActiveUsers = GetActiveUsers();
                
                // Get profit margin
                stats.ProfitMargin = GetProfitMargin();
                
                // Get pending orders (purchases not fully paid)
                stats.PendingOrders = GetPendingOrders();
                
                // Get sales returns (for date range if specified)
                if (fromDate.HasValue && toDate.HasValue)
                {
                    stats.SalesReturns = GetSalesReturnsByDateRange(fromDate.Value, toDate.Value);
                }
                else
                {
                    stats.SalesReturns = isCurrentDayClosed ? 0 : GetSalesReturns();
                }
                
                // Get purchase returns (for date range if specified)
                if (fromDate.HasValue && toDate.HasValue)
                {
                    stats.PurchaseReturns = GetPurchaseReturnsByDateRange(fromDate.Value, toDate.Value);
                }
                else
                {
                    stats.PurchaseReturns = isCurrentDayClosed ? 0 : GetPurchaseReturns();
                }
                
                // Get expenses (for date range if specified)
                if (fromDate.HasValue && toDate.HasValue)
                {
                    stats.Expenses = GetExpensesByDateRange(fromDate.Value, toDate.Value);
                }
                else
                {
                    stats.Expenses = isCurrentDayClosed ? 0 : GetExpenses();
                }
                
                // Get suppliers
                stats.TotalSuppliers = GetTotalSuppliers();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading dashboard statistics: {ex.Message}");
            }
            
            return stats;
        }
        
        public List<RecentActivity> GetRecentActivities(int limit = 10)
        {
            var activities = new List<RecentActivity>();
            
            try
            {
                // Get recent sales
                var recentSales = GetRecentSales(limit / 3);
                activities.AddRange(recentSales);
                
                // Get recent purchases
                var recentPurchases = GetRecentPurchases(limit / 3);
                activities.AddRange(recentPurchases);
                
                // Get recent products
                var recentProducts = GetRecentProducts(limit / 3);
                activities.AddRange(recentProducts);
                
                // Sort by time and take the most recent
                activities.Sort((x, y) => y.Time.CompareTo(x.Time));
                return activities.GetRange(0, Math.Min(limit, activities.Count));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading recent activities: {ex.Message}");
            }
        }
        
        public List<SalesDataPoint> GetSalesData(DateTime startDate, DateTime endDate)
        {
            var salesData = new List<SalesDataPoint>();
            
            try
            {
                string query = @"
                    SELECT CAST(SaleDate AS DATE) as SaleDate, SUM(TotalAmount) as TotalSales
                    FROM Sales 
                    WHERE SaleDate BETWEEN @StartDate AND @EndDate
                    GROUP BY CAST(SaleDate AS DATE)
                    ORDER BY SaleDate";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesData.Add(new SalesDataPoint
                                {
                                    Date = Convert.ToDateTime(reader["SaleDate"]),
                                    Amount = ParseDecimal(reader["TotalSales"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading sales data: {ex.Message}");
            }
            
            return salesData;
        }
        
        public List<PurchaseDataPoint> GetPurchaseData(DateTime startDate, DateTime endDate)
        {
            var purchaseData = new List<PurchaseDataPoint>();
            
            try
            {
                string query = @"
                    SELECT CAST(PurchaseDate AS DATE) as PurchaseDate, SUM(TotalAmount) as TotalPurchases
                    FROM Purchases 
                    WHERE PurchaseDate BETWEEN @StartDate AND @EndDate
                    GROUP BY CAST(PurchaseDate AS DATE)
                    ORDER BY PurchaseDate";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                purchaseData.Add(new PurchaseDataPoint
                                {
                                    Date = Convert.ToDateTime(reader["PurchaseDate"]),
                                    Amount = ParseDecimal(reader["TotalPurchases"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading purchase data: {ex.Message}");
            }
            
            return purchaseData;
        }
        
        public List<TopProduct> GetTopProducts(int limit = 10)
        {
            var topProducts = new List<TopProduct>();
            
            try
            {
                string query = @"
                    SELECT TOP (@Limit) p.ProductName, p.ProductCode, 
                           SUM(si.Quantity) as TotalSold,
                           SUM(si.SubTotal) as TotalRevenue
                    FROM Products p
                    INNER JOIN SaleItems si ON p.ProductID = si.ProductID
                    INNER JOIN Sales s ON si.SaleID = s.SaleID
                    WHERE s.SaleDate >= DATEADD(day, -30, GETDATE())
                    GROUP BY p.ProductID, p.ProductName, p.ProductCode
                    ORDER BY TotalSold DESC";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Limit", limit);
                        
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                topProducts.Add(new TopProduct
                                {
                                    ProductName = reader["ProductName"].ToString(),
                                    ProductCode = reader["ProductCode"].ToString(),
                                    TotalSold = Convert.ToInt32(reader["TotalSold"]),
                                    TotalRevenue = ParseDecimal(reader["TotalRevenue"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading top products: {ex.Message}");
            }
            
            return topProducts;
        }
        
        public List<LowStockProduct> GetLowStockProducts(int threshold = 10)
        {
            var lowStockProducts = new List<LowStockProduct>();
            
            try
            {
                string query = @"
                    SELECT ProductName, ProductCode, StockQuantity, ReorderLevel
                    FROM Products 
                    WHERE StockQuantity <= @Threshold AND IsActive = 1
                    ORDER BY StockQuantity ASC";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Threshold", threshold);
                        
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lowStockProducts.Add(new LowStockProduct
                                {
                                    ProductName = reader["ProductName"].ToString(),
                                    ProductCode = reader["ProductCode"].ToString(),
                                    StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                                    ReorderLevel = Convert.ToInt32(reader["ReorderLevel"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading low stock products: {ex.Message}");
            }
            
            return lowStockProducts;
        }
        
        private decimal GetTodaySales()
        {
            // Use business date instead of calendar date so dashboard resets to 0 after day end
            DateTime businessDate = _businessDateService.GetCurrentBusinessDate();
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE CAST(SaleDate AS DATE) = @BusinessDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BusinessDate", businessDate.Date);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetMonthlySales()
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE MONTH(SaleDate) = MONTH(GETDATE()) AND YEAR(SaleDate) = YEAR(GETDATE())";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private int GetTotalProducts()
        {
            string query = "SELECT COUNT(*) FROM Products WHERE IsActive = 1";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        private int GetLowStockItems()
        {
            string query = "SELECT COUNT(*) FROM Products WHERE StockQuantity <= ReorderLevel AND IsActive = 1";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        private int GetTotalCustomers()
        {
            string query = "SELECT COUNT(*) FROM Customers WHERE IsActive = 1";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        private int GetActiveUsers()
        {
            string query = "SELECT COUNT(*) FROM Users WHERE IsActive = 1";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        private decimal GetProfitMargin()
        {
            try
            {
                decimal totalSales = GetMonthlySales();
                decimal totalPurchases = GetMonthlyPurchases();
                
                if (totalSales == 0) return 0;
                
                return ((totalSales - totalPurchases) / totalSales) * 100;
            }
            catch
            {
                return 0;
            }
        }
        
        private decimal GetMonthlyPurchases()
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Purchases WHERE MONTH(PurchaseDate) = MONTH(GETDATE()) AND YEAR(PurchaseDate) = YEAR(GETDATE())";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private int GetPendingOrders()
        {
            string query = "SELECT COUNT(*) FROM Purchases WHERE PaidAmount < TotalAmount";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        private decimal GetSalesReturns()
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM SalesReturns WHERE MONTH(ReturnDate) = MONTH(GETDATE()) AND YEAR(ReturnDate) = YEAR(GETDATE())";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetPurchaseReturns()
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM PurchaseReturns WHERE MONTH(ReturnDate) = MONTH(GETDATE()) AND YEAR(ReturnDate) = YEAR(GETDATE())";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetExpenses()
        {
            string query = "SELECT ISNULL(SUM(Amount), 0) FROM ExpenseEntries WHERE MONTH(ExpenseDate) = MONTH(GETDATE()) AND YEAR(ExpenseDate) = YEAR(GETDATE())";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private int GetTotalSuppliers()
        {
            string query = "SELECT COUNT(*) FROM Suppliers WHERE IsActive = 1";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        private List<RecentActivity> GetRecentSales(int limit)
        {
            var activities = new List<RecentActivity>();
            
            string query = @"
                SELECT TOP (@Limit) s.SaleDate, s.TotalAmount, c.CustomerName
                FROM Sales s
                LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                ORDER BY s.SaleDate DESC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Limit", limit);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activities.Add(new RecentActivity
                            {
                                Time = Convert.ToDateTime(reader["SaleDate"]),
                                Description = $"Sale completed: ${ParseDecimal(reader["TotalAmount"]):F2} - {reader["CustomerName"]}",
                                Type = "Sale"
                            });
                        }
                    }
                }
            }
            
            return activities;
        }
        
        private List<RecentActivity> GetRecentPurchases(int limit)
        {
            var activities = new List<RecentActivity>();
            
            string query = @"
                SELECT TOP (@Limit) p.PurchaseDate, p.TotalAmount, s.SupplierName
                FROM Purchases p
                LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                ORDER BY p.PurchaseDate DESC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Limit", limit);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activities.Add(new RecentActivity
                            {
                                Time = Convert.ToDateTime(reader["PurchaseDate"]),
                                Description = $"Purchase completed: ${ParseDecimal(reader["TotalAmount"]):F2} - {reader["SupplierName"]}",
                                Type = "Purchase"
                            });
                        }
                    }
                }
            }
            
            return activities;
        }
        
        private List<RecentActivity> GetRecentProducts(int limit)
        {
            var activities = new List<RecentActivity>();
            
            string query = @"
                SELECT TOP (@Limit) ProductName, CreatedDate
                FROM Products
                ORDER BY CreatedDate DESC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Limit", limit);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activities.Add(new RecentActivity
                            {
                                Time = Convert.ToDateTime(reader["CreatedDate"]),
                                Description = $"New product added: {reader["ProductName"]}",
                                Type = "Product"
                            });
                        }
                    }
                }
            }
            
            return activities;
        }
        
        private decimal GetSalesByDateRange(DateTime fromDate, DateTime toDate)
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE CAST(SaleDate AS DATE) >= @FromDate AND CAST(SaleDate AS DATE) <= @ToDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    command.Parameters.AddWithValue("@ToDate", toDate.Date);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetSalesReturnsByDateRange(DateTime fromDate, DateTime toDate)
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM SalesReturns WHERE CAST(ReturnDate AS DATE) >= @FromDate AND CAST(ReturnDate AS DATE) <= @ToDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    command.Parameters.AddWithValue("@ToDate", toDate.Date);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetPurchaseReturnsByDateRange(DateTime fromDate, DateTime toDate)
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM PurchaseReturns WHERE CAST(ReturnDate AS DATE) >= @FromDate AND CAST(ReturnDate AS DATE) <= @ToDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    command.Parameters.AddWithValue("@ToDate", toDate.Date);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetExpensesByDateRange(DateTime fromDate, DateTime toDate)
        {
            string query = "SELECT ISNULL(SUM(Amount), 0) FROM ExpenseEntries WHERE CAST(ExpenseDate AS DATE) >= @FromDate AND CAST(ExpenseDate AS DATE) <= @ToDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    command.Parameters.AddWithValue("@ToDate", toDate.Date);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal GetPurchasesByDateRange(DateTime fromDate, DateTime toDate)
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Purchases WHERE CAST(PurchaseDate AS DATE) >= @FromDate AND CAST(PurchaseDate AS DATE) <= @ToDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    command.Parameters.AddWithValue("@ToDate", toDate.Date);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return ParseDecimal(result);
                }
            }
        }
        
        private decimal ParseDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            
            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;
            
            return 0;
        }
    }
    
    // Data classes for dashboard
    public class SalesDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class PurchaseDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class TopProduct
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    
    public class LowStockProduct
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}
