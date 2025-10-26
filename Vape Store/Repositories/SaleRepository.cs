using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class SaleRepository
    {
        public SaleRepository()
        {
        }

        public List<Sale> GetAllSales()
        {
            var sales = new List<Sale>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT s.*, c.CustomerName, u.FullName as UserName
                        FROM Sales s
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON s.UserID = u.UserID
                        ORDER BY s.SaleDate DESC";

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
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales: {ex.Message}", ex);
            }

            return sales;
        }

        public Sale GetSaleById(int saleId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT s.*, c.CustomerName, u.FullName as UserName
                        FROM Sales s
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON s.UserID = u.UserID
                        WHERE s.SaleID = @SaleID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SaleID", saleId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Sale
                                {
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale: {ex.Message}", ex);
            }

            return null;
        }

        public Sale GetSaleByInvoiceNumber(string invoiceNumber)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT s.*, c.CustomerName, u.FullName as UserName
                        FROM Sales s
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON s.UserID = u.UserID
                        WHERE s.InvoiceNumber = @InvoiceNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Sale
                                {
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale by invoice number: {ex.Message}", ex);
            }

            return null;
        }

        public List<SaleItem> GetSaleItems(int saleId)
        {
            var saleItems = new List<SaleItem>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT si.*, p.ProductName, p.ProductCode
                        FROM SaleItems si
                        LEFT JOIN Products p ON si.ProductID = p.ProductID
                        WHERE si.SaleID = @SaleID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SaleID", saleId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                saleItems.Add(new SaleItem
                                {
                                    SaleItemID = Convert.ToInt32(reader["SaleItemID"]),
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    ProductName = reader["ProductName"]?.ToString(),
                                    ProductCode = reader["ProductCode"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale items: {ex.Message}", ex);
            }

            return saleItems;
        }

        public Sale GetSaleWithItems(int saleId)
        {
            try
            {
                var sale = GetSaleById(saleId);
                if (sale != null)
                {
                    sale.SaleItems = GetSaleItems(saleId);
                }
                return sale;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale with items: {ex.Message}", ex);
            }
        }

        public Sale GetSaleWithItemsByInvoice(string invoiceNumber)
        {
            try
            {
                var sale = GetSaleByInvoiceNumber(invoiceNumber);
                if (sale != null)
                {
                    sale.SaleItems = GetSaleItems(sale.SaleID);
                }
                return sale;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale with items by invoice: {ex.Message}", ex);
            }
        }

        public bool UpdateSale(Sale sale)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Update sale with new fields
                            var saleQuery = @"
                                UPDATE Sales 
                                SET CustomerID = @CustomerID, SaleDate = @SaleDate, SubTotal = @SubTotal, 
                                    DiscountAmount = @DiscountAmount, DiscountPercent = @DiscountPercent,
                                    TaxAmount = @TaxAmount, TaxPercent = @TaxPercent, TotalAmount = @TotalAmount, 
                                    PaymentMethod = @PaymentMethod, PaidAmount = @PaidAmount, ChangeAmount = @ChangeAmount, 
                                    UserID = @UserID, Status = @Status, Notes = @Notes,
                                    LastModified = GETDATE(), ModifiedBy = @ModifiedBy
                                WHERE SaleID = @SaleID";

                            using (var saleCommand = new SqlCommand(saleQuery, connection, transaction))
                            {
                                saleCommand.Parameters.AddWithValue("@SaleID", sale.SaleID);
                                saleCommand.Parameters.AddWithValue("@CustomerID", sale.CustomerID);
                                saleCommand.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                                saleCommand.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
                                saleCommand.Parameters.AddWithValue("@DiscountAmount", sale.DiscountAmount);
                                saleCommand.Parameters.AddWithValue("@DiscountPercent", sale.DiscountPercent);
                                saleCommand.Parameters.AddWithValue("@TaxAmount", sale.TaxAmount);
                                saleCommand.Parameters.AddWithValue("@TaxPercent", sale.TaxPercent);
                                saleCommand.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);
                                saleCommand.Parameters.AddWithValue("@PaymentMethod", sale.PaymentMethod ?? (object)DBNull.Value);
                                saleCommand.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
                                saleCommand.Parameters.AddWithValue("@ChangeAmount", sale.ChangeAmount);
                                saleCommand.Parameters.AddWithValue("@UserID", sale.UserID);
                                saleCommand.Parameters.AddWithValue("@Status", sale.Status ?? "Active");
                                saleCommand.Parameters.AddWithValue("@Notes", sale.Notes ?? (object)DBNull.Value);
                                saleCommand.Parameters.AddWithValue("@ModifiedBy", sale.ModifiedBy ?? sale.UserID);

                                saleCommand.ExecuteNonQuery();
                            }

                            // Delete existing sale items
                            var deleteItemsQuery = "DELETE FROM SaleItems WHERE SaleID = @SaleID";
                            using (var deleteCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@SaleID", sale.SaleID);
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Insert updated sale items
                            foreach (var item in sale.SaleItems)
                            {
                                var itemQuery = @"
                                    INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice, SubTotal)
                                    VALUES (@SaleID, @ProductID, @Quantity, @UnitPrice, @SubTotal)";

                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@SaleID", sale.SaleID);
                                    itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                    itemCommand.Parameters.AddWithValue("@SubTotal", item.SubTotal);

                                    itemCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating sale: {ex.Message}", ex);
            }
        }

        public bool DeleteSale(int saleId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Delete sale items first
                            var deleteItemsQuery = "DELETE FROM SaleItems WHERE SaleID = @SaleID";
                            using (var deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteItemsCommand.Parameters.AddWithValue("@SaleID", saleId);
                                deleteItemsCommand.ExecuteNonQuery();
                            }

                            // Delete sale
                            var deleteSaleQuery = "DELETE FROM Sales WHERE SaleID = @SaleID";
                            using (var deleteSaleCommand = new SqlCommand(deleteSaleQuery, connection, transaction))
                            {
                                deleteSaleCommand.Parameters.AddWithValue("@SaleID", saleId);
                                deleteSaleCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting sale: {ex.Message}", ex);
            }
        }

        public List<Sale> GetSalesByDateRange(DateTime fromDate, DateTime toDate)
        {
            var sales = new List<Sale>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT s.*, c.CustomerName, u.FullName as UserName
                        FROM Sales s
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON s.UserID = u.UserID
                        WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                        ORDER BY s.SaleDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var sale = new Sale
                                {
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                                
                                // Load sale items for each sale
                                sale.SaleItems = GetSaleItems(sale.SaleID);
                                sales.Add(sale);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales by date range: {ex.Message}", ex);
            }
            
            return sales;
        }

        public List<Sale> GetSalesByCustomerAndDateRange(int customerId, DateTime fromDate, DateTime toDate)
        {
            var sales = new List<Sale>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT s.*, c.CustomerName, u.FullName as UserName
                        FROM Sales s
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON s.UserID = u.UserID
                        WHERE s.CustomerID = @CustomerID AND s.SaleDate BETWEEN @FromDate AND @ToDate
                        ORDER BY s.SaleDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", customerId);
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var sale = new Sale
                                {
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                                
                                sales.Add(sale);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales by customer and date range: {ex.Message}", ex);
            }

            return sales;
        }

        public List<SalesReportItem> GetSalesReportData(DateTime fromDate, DateTime toDate)
        {
            var salesReportItems = new List<SalesReportItem>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT 
                            s.SaleID,
                            s.InvoiceNumber,
                            s.SaleDate,
                            c.CustomerName,
                            p.ProductName,
                            si.Quantity,
                            si.UnitPrice,
                            si.SubTotal,
                            s.TaxAmount,
                            s.TotalAmount,
                            s.PaymentMethod,
                            s.PaidAmount,
                            (s.TotalAmount - s.PaidAmount) as BalanceAmount
                        FROM Sales s
                        INNER JOIN SaleItems si ON s.SaleID = si.SaleID
                        INNER JOIN Products p ON si.ProductID = p.ProductID
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        WHERE s.SaleDate >= @FromDate AND s.SaleDate <= @ToDate
                        ORDER BY s.SaleDate DESC, s.InvoiceNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                        command.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddTicks(-1));
                        
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesReportItems.Add(new SalesReportItem
                                {
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString() ?? "Walk-in Customer",
                                    ProductName = reader["ProductName"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    BalanceAmount = Convert.ToDecimal(reader["BalanceAmount"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales report data: {ex.Message}", ex);
            }

            return salesReportItems;
        }
    }
}
