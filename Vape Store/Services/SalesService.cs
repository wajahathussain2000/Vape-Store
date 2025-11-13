using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;
using Vape_Store.Repositories;

namespace Vape_Store.Services
{
    public class SalesService
    {
        private readonly ProductRepository _productRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly BusinessDateService _businessDateService;
        
        public SalesService()
        {
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository();
            _businessDateService = new BusinessDateService();
        }
        
        public bool ProcessSale(Sale sale)
        {
            // Validate date - check if the sale date is closed
            if (!_businessDateService.CanCreateTransaction(sale.SaleDate))
            {
                string message = _businessDateService.GetValidationMessage(sale.SaleDate);
                throw new InvalidOperationException(message);
            }

            // Validate sale items for negative or zero quantities
            foreach (var item in sale.SaleItems)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException($"Invalid quantity ({item.Quantity}) for product {item.ProductName}. Quantity must be greater than zero.");
                }
                if (item.UnitPrice < 0)
                {
                    throw new ArgumentException($"Invalid unit price ({item.UnitPrice}) for product {item.ProductName}. Price cannot be negative.");
                }
            }
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert sale
                        int saleID = InsertSale(sale, connection, transaction);
                        
                        // Insert sale items and update stock
                        foreach (var item in sale.SaleItems)
                        {
                            item.SaleID = saleID;
                            InsertSaleItem(item, connection, transaction);
                            
                            // Update product stock
                            _productRepository.UpdateStock(item.ProductID, -item.Quantity);
                        }
                        
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Sale processing failed: {ex.Message}");
                    }
                }
            }
        }
        
        private int InsertSale(Sale sale, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"INSERT INTO Sales (InvoiceNumber, CustomerID, SaleDate, SubTotal, TaxAmount, TaxPercent, 
                           TotalAmount, PaymentMethod, PaidAmount, ChangeAmount, UserID, BarcodeImage, BarcodeData, 
                           DiscountAmount, DiscountPercent) 
                           VALUES (@InvoiceNumber, @CustomerID, @SaleDate, @SubTotal, @TaxAmount, @TaxPercent, 
                           @TotalAmount, @PaymentMethod, @PaidAmount, @ChangeAmount, @UserID, @BarcodeImage, @BarcodeData,
                           @DiscountAmount, @DiscountPercent);
                           SELECT SCOPE_IDENTITY();";
            
            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@InvoiceNumber", sale.InvoiceNumber);
                command.Parameters.AddWithValue("@CustomerID", sale.CustomerID);
                command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                command.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
                command.Parameters.AddWithValue("@TaxAmount", sale.TaxAmount);
                command.Parameters.AddWithValue("@TaxPercent", sale.TaxPercent);
                command.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);
                command.Parameters.AddWithValue("@PaymentMethod", sale.PaymentMethod);
                command.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
                command.Parameters.AddWithValue("@ChangeAmount", sale.ChangeAmount);
                command.Parameters.AddWithValue("@UserID", sale.UserID);
                command.Parameters.AddWithValue("@BarcodeImage", sale.BarcodeImage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BarcodeData", sale.BarcodeData ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DiscountAmount", sale.DiscountAmount);
                command.Parameters.AddWithValue("@DiscountPercent", sale.DiscountPercent);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        
        private void InsertSaleItem(SaleItem item, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice, SubTotal) 
                           VALUES (@SaleID, @ProductID, @Quantity, @UnitPrice, @SubTotal)";
            
            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@SaleID", item.SaleID);
                command.Parameters.AddWithValue("@ProductID", item.ProductID);
                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                command.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                
                command.ExecuteNonQuery();
            }
        }
        
        public List<Sale> GetAllSales()
        {
            List<Sale> sales = new List<Sale>();
            string query = @"SELECT s.SaleID, s.InvoiceNumber, s.CustomerID, s.SaleDate, s.SubTotal, s.TaxAmount, 
                           s.TaxPercent, s.TotalAmount, s.PaymentMethod, s.PaidAmount, s.ChangeAmount, s.UserID, s.CreatedDate,
                           c.CustomerName, u.Username
                           FROM Sales s
                           LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                           LEFT JOIN Users u ON s.UserID = u.UserID
                           ORDER BY s.SaleDate DESC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sales.Add(new Sale
                            {
                                SaleID = Convert.ToInt32(reader["SaleID"]),
                                InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                UserName = reader["Username"].ToString()
                            });
                        }
                    }
                }
            }
            
            return sales;
        }
        
        public Sale GetSaleById(int saleID)
        {
            Sale sale = null;
            string query = @"SELECT s.SaleID, s.InvoiceNumber, s.CustomerID, s.SaleDate, s.SubTotal, s.TaxAmount, 
                           s.TaxPercent, s.TotalAmount, s.PaymentMethod, s.PaidAmount, s.ChangeAmount, s.UserID, s.CreatedDate,
                           c.CustomerName, u.Username
                           FROM Sales s
                           LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                           LEFT JOIN Users u ON s.UserID = u.UserID
                           WHERE s.SaleID = @SaleID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SaleID", saleID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sale = new Sale
                            {
                                SaleID = Convert.ToInt32(reader["SaleID"]),
                                InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                CustomerName = reader["CustomerName"].ToString(),
                                UserName = reader["Username"].ToString()
                            };
                        }
                    }
                }
            }
            
            if (sale != null)
            {
                sale.SaleItems = GetSaleItems(saleID);
            }
            
            return sale;
        }
        
        public List<SaleItem> GetSaleItems(int saleID)
        {
            List<SaleItem> items = new List<SaleItem>();
            string query = @"SELECT si.SaleItemID, si.SaleID, si.ProductID, si.Quantity, si.UnitPrice, si.SubTotal,
                           p.ProductName, p.ProductCode
                           FROM SaleItems si
                           LEFT JOIN Products p ON si.ProductID = p.ProductID
                           WHERE si.SaleID = @SaleID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SaleID", saleID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new SaleItem
                            {
                                SaleItemID = Convert.ToInt32(reader["SaleItemID"]),
                                SaleID = Convert.ToInt32(reader["SaleID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                ProductName = reader["ProductName"].ToString(),
                                ProductCode = reader["ProductCode"].ToString()
                            });
                        }
                    }
                }
            }
            
            return items;
        }
        
        public string GetNextInvoiceNumber()
        {
            // Handle the current format: INV-YYYY-XXX
            string query = @"
                SELECT ISNULL(
                    MAX(CAST(
                        SUBSTRING(InvoiceNumber, 
                            CHARINDEX('-', InvoiceNumber, CHARINDEX('-', InvoiceNumber) + 1) + 1, 
                            LEN(InvoiceNumber)
                        ) AS INT
                    )), 0
                ) + 1 
                FROM Sales 
                WHERE InvoiceNumber LIKE 'INV-%-%'";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    string invoiceNumber = $"INV-{DateTime.Now.Year}-{result:D3}";
                    
                    // Debug output to track invoice number generation
                    System.Diagnostics.Debug.WriteLine($"[GetNextInvoiceNumber] Generated: {invoiceNumber} (Result from DB: {result})");
                    
                    return invoiceNumber;
                }
            }
        }
        
        public decimal CalculateTax(decimal subtotal, decimal taxPercent)
        {
            return subtotal * (taxPercent / 100);
        }
        
        public decimal CalculateChange(decimal paidAmount, decimal totalAmount)
        {
            return paidAmount - totalAmount;
        }
    }
}
