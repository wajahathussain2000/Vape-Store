using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Services
{
    public class ReportingService
    {
        public class SalesReport
        {
            public DateTime Date { get; set; }
            public decimal TotalSales { get; set; }
            public int TotalTransactions { get; set; }
            public decimal AverageTransaction { get; set; }
        }
        
        public class ProductSalesReport
        {
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public int QuantitySold { get; set; }
            public decimal TotalRevenue { get; set; }
        }
        
        public class CustomerReport
        {
            public string CustomerName { get; set; }
            public int TotalPurchases { get; set; }
            public decimal TotalSpent { get; set; }
            public DateTime LastPurchase { get; set; }
        }
        
        public List<SalesReport> GetDailySalesReport(DateTime startDate, DateTime endDate)
        {
            List<SalesReport> reports = new List<SalesReport>();
            string query = @"SELECT CAST(SaleDate AS DATE) as SaleDate, 
                           SUM(TotalAmount) as TotalSales, 
                           COUNT(*) as TotalTransactions,
                           AVG(TotalAmount) as AverageTransaction
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
                            reports.Add(new SalesReport
                            {
                                Date = Convert.ToDateTime(reader["SaleDate"]),
                                TotalSales = Convert.ToDecimal(reader["TotalSales"]),
                                TotalTransactions = Convert.ToInt32(reader["TotalTransactions"]),
                                AverageTransaction = Convert.ToDecimal(reader["AverageTransaction"])
                            });
                        }
                    }
                }
            }
            
            return reports;
        }
        
        public List<ProductSalesReport> GetTopSellingProducts(DateTime startDate, DateTime endDate, int topCount = 10)
        {
            List<ProductSalesReport> reports = new List<ProductSalesReport>();
            string query = @"SELECT p.ProductName, p.ProductCode, 
                           SUM(si.Quantity) as QuantitySold,
                           SUM(si.SubTotal) as TotalRevenue
                           FROM SaleItems si
                           INNER JOIN Sales s ON si.SaleID = s.SaleID
                           INNER JOIN Products p ON si.ProductID = p.ProductID
                           WHERE s.SaleDate BETWEEN @StartDate AND @EndDate
                           GROUP BY p.ProductID, p.ProductName, p.ProductCode
                           ORDER BY QuantitySold DESC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read() && count < topCount)
                        {
                            reports.Add(new ProductSalesReport
                            {
                                ProductName = reader["ProductName"].ToString(),
                                ProductCode = reader["ProductCode"].ToString(),
                                QuantitySold = Convert.ToInt32(reader["QuantitySold"]),
                                TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                            });
                            count++;
                        }
                    }
                }
            }
            
            return reports;
        }
        
        public List<CustomerReport> GetTopCustomers(DateTime startDate, DateTime endDate, int topCount = 10)
        {
            List<CustomerReport> reports = new List<CustomerReport>();
            string query = @"SELECT c.CustomerName,
                           COUNT(s.SaleID) as TotalPurchases,
                           SUM(s.TotalAmount) as TotalSpent,
                           MAX(s.SaleDate) as LastPurchase
                           FROM Sales s
                           INNER JOIN Customers c ON s.CustomerID = c.CustomerID
                           WHERE s.SaleDate BETWEEN @StartDate AND @EndDate
                           GROUP BY c.CustomerID, c.CustomerName
                           ORDER BY TotalSpent DESC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read() && count < topCount)
                        {
                            reports.Add(new CustomerReport
                            {
                                CustomerName = reader["CustomerName"].ToString(),
                                TotalPurchases = Convert.ToInt32(reader["TotalPurchases"]),
                                TotalSpent = Convert.ToDecimal(reader["TotalSpent"]),
                                LastPurchase = Convert.ToDateTime(reader["LastPurchase"])
                            });
                            count++;
                        }
                    }
                }
            }
            
            return reports;
        }
        
        public decimal GetTotalSales(DateTime startDate, DateTime endDate)
        {
            string query = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE SaleDate BETWEEN @StartDate AND @EndDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToDecimal(result);
                }
            }
        }
        
        public int GetTotalTransactions(DateTime startDate, DateTime endDate)
        {
            string query = "SELECT COUNT(*) FROM Sales WHERE SaleDate BETWEEN @StartDate AND @EndDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }
        
        public decimal GetAverageTransactionValue(DateTime startDate, DateTime endDate)
        {
            string query = "SELECT ISNULL(AVG(TotalAmount), 0) FROM Sales WHERE SaleDate BETWEEN @StartDate AND @EndDate";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToDecimal(result);
                }
            }
        }
        
        public Dictionary<string, decimal> GetSalesByPaymentMethod(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, decimal> salesByMethod = new Dictionary<string, decimal>();
            string query = @"SELECT PaymentMethod, SUM(TotalAmount) as TotalSales
                           FROM Sales 
                           WHERE SaleDate BETWEEN @StartDate AND @EndDate
                           GROUP BY PaymentMethod";
            
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
                            string method = reader["PaymentMethod"].ToString();
                            decimal total = Convert.ToDecimal(reader["TotalSales"]);
                            salesByMethod[method] = total;
                        }
                    }
                }
            }
            
            return salesByMethod;
        }
        
        public List<SalesReport> GetMonthlySalesReport(int year)
        {
            List<SalesReport> reports = new List<SalesReport>();
            string query = @"SELECT MONTH(SaleDate) as Month,
                           SUM(TotalAmount) as TotalSales,
                           COUNT(*) as TotalTransactions,
                           AVG(TotalAmount) as AverageTransaction
                           FROM Sales 
                           WHERE YEAR(SaleDate) = @Year
                           GROUP BY MONTH(SaleDate)
                           ORDER BY Month";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", year);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reports.Add(new SalesReport
                            {
                                Date = new DateTime(year, Convert.ToInt32(reader["Month"]), 1),
                                TotalSales = Convert.ToDecimal(reader["TotalSales"]),
                                TotalTransactions = Convert.ToInt32(reader["TotalTransactions"]),
                                AverageTransaction = Convert.ToDecimal(reader["AverageTransaction"])
                            });
                        }
                    }
                }
            }
            
            return reports;
        }
    }
}
