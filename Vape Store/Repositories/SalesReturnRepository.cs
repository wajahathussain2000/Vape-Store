using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class SalesReturnRepository
    {
        public SalesReturnRepository()
        {
        }

        public List<SalesReturn> GetAllSalesReturns()
        {
            var salesReturns = new List<SalesReturn>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sr.*, c.CustomerName, u.FullName as UserName, s.InvoiceNumber
                        FROM SalesReturns sr
                        LEFT JOIN Customers c ON sr.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON sr.UserID = u.UserID
                        LEFT JOIN Sales s ON sr.SaleID = s.SaleID
                        ORDER BY sr.ReturnDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesReturns.Add(new SalesReturn
                                {
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
                                    ReturnNumber = reader["ReturnNumber"].ToString(),
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                                    ReturnReason = reader["ReturnReason"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString(),
                                    InvoiceNumber = reader["InvoiceNumber"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales returns: {ex.Message}", ex);
            }

            return salesReturns;
        }

        public SalesReturn GetSalesReturnById(int returnId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sr.*, c.CustomerName, u.FullName as UserName, s.InvoiceNumber
                        FROM SalesReturns sr
                        LEFT JOIN Customers c ON sr.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON sr.UserID = u.UserID
                        LEFT JOIN Sales s ON sr.SaleID = s.SaleID
                        WHERE sr.ReturnID = @ReturnID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReturnID", returnId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SalesReturn
                                {
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
                                    ReturnNumber = reader["ReturnNumber"].ToString(),
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                                    ReturnReason = reader["ReturnReason"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString(),
                                    InvoiceNumber = reader["InvoiceNumber"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales return: {ex.Message}", ex);
            }

            return null;
        }

        public List<SalesReturnItem> GetSalesReturnItems(int returnId)
        {
            var returnItems = new List<SalesReturnItem>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sri.*, p.ProductName, p.ProductCode
                        FROM SalesReturnItems sri
                        LEFT JOIN Products p ON sri.ProductID = p.ProductID
                        WHERE sri.ReturnID = @ReturnID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReturnID", returnId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                returnItems.Add(new SalesReturnItem
                                {
                                    ReturnItemID = Convert.ToInt32(reader["ReturnItemID"]),
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
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
                throw new Exception($"Error retrieving sales return items: {ex.Message}", ex);
            }

            return returnItems;
        }

        public bool AddSalesReturn(SalesReturn salesReturn)
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
                            // Insert sales return
                            var returnQuery = @"
                                INSERT INTO SalesReturns (ReturnNumber, SaleID, CustomerID, ReturnDate, ReturnReason, Description, TotalAmount, UserID, CreatedDate)
                                VALUES (@ReturnNumber, @SaleID, @CustomerID, @ReturnDate, @ReturnReason, @Description, @TotalAmount, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var returnCommand = new SqlCommand(returnQuery, connection, transaction))
                            {
                                returnCommand.Parameters.AddWithValue("@ReturnNumber", salesReturn.ReturnNumber);
                                returnCommand.Parameters.AddWithValue("@SaleID", salesReturn.SaleID);
                                returnCommand.Parameters.AddWithValue("@CustomerID", salesReturn.CustomerID);
                                returnCommand.Parameters.AddWithValue("@ReturnDate", salesReturn.ReturnDate);
                                returnCommand.Parameters.AddWithValue("@ReturnReason", salesReturn.ReturnReason ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@Description", salesReturn.Description ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@TotalAmount", salesReturn.TotalAmount);
                                returnCommand.Parameters.AddWithValue("@UserID", salesReturn.UserID);
                                returnCommand.Parameters.AddWithValue("@CreatedDate", salesReturn.CreatedDate);

                                var returnId = Convert.ToInt32(returnCommand.ExecuteScalar());

                                // Insert return items
                                foreach (var item in salesReturn.ReturnItems)
                                {
                                    var itemQuery = @"
                                        INSERT INTO SalesReturnItems (ReturnID, ProductID, Quantity, UnitPrice, SubTotal)
                                        VALUES (@ReturnID, @ProductID, @Quantity, @UnitPrice, @SubTotal)";

                                    using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                    {
                                        itemCommand.Parameters.AddWithValue("@ReturnID", returnId);
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
                throw new Exception($"Error adding sales return: {ex.Message}", ex);
            }
        }

        public bool UpdateSalesReturn(SalesReturn salesReturn)
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
                            // Update sales return
                            var returnQuery = @"
                                UPDATE SalesReturns 
                                SET ReturnNumber = @ReturnNumber, SaleID = @SaleID, CustomerID = @CustomerID, 
                                    ReturnDate = @ReturnDate, ReturnReason = @ReturnReason, Description = @Description, 
                                    TotalAmount = @TotalAmount, UserID = @UserID
                                WHERE ReturnID = @ReturnID";

                            using (var returnCommand = new SqlCommand(returnQuery, connection, transaction))
                            {
                                returnCommand.Parameters.AddWithValue("@ReturnID", salesReturn.ReturnID);
                                returnCommand.Parameters.AddWithValue("@ReturnNumber", salesReturn.ReturnNumber);
                                returnCommand.Parameters.AddWithValue("@SaleID", salesReturn.SaleID);
                                returnCommand.Parameters.AddWithValue("@CustomerID", salesReturn.CustomerID);
                                returnCommand.Parameters.AddWithValue("@ReturnDate", salesReturn.ReturnDate);
                                returnCommand.Parameters.AddWithValue("@ReturnReason", salesReturn.ReturnReason ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@Description", salesReturn.Description ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@TotalAmount", salesReturn.TotalAmount);
                                returnCommand.Parameters.AddWithValue("@UserID", salesReturn.UserID);

                                returnCommand.ExecuteNonQuery();
                            }

                            // Delete existing return items
                            var deleteItemsQuery = "DELETE FROM SalesReturnItems WHERE ReturnID = @ReturnID";
                            using (var deleteCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@ReturnID", salesReturn.ReturnID);
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Insert updated return items
                            foreach (var item in salesReturn.ReturnItems)
                            {
                                var itemQuery = @"
                                    INSERT INTO SalesReturnItems (ReturnID, ProductID, Quantity, UnitPrice, SubTotal)
                                    VALUES (@ReturnID, @ProductID, @Quantity, @UnitPrice, @SubTotal)";

                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@ReturnID", salesReturn.ReturnID);
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
                throw new Exception($"Error updating sales return: {ex.Message}", ex);
            }
        }

        public bool DeleteSalesReturn(int returnId)
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
                            // Delete return items first
                            var deleteItemsQuery = "DELETE FROM SalesReturnItems WHERE ReturnID = @ReturnID";
                            using (var deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteItemsCommand.Parameters.AddWithValue("@ReturnID", returnId);
                                deleteItemsCommand.ExecuteNonQuery();
                            }

                            // Delete sales return
                            var deleteReturnQuery = "DELETE FROM SalesReturns WHERE ReturnID = @ReturnID";
                            using (var deleteReturnCommand = new SqlCommand(deleteReturnQuery, connection, transaction))
                            {
                                deleteReturnCommand.Parameters.AddWithValue("@ReturnID", returnId);
                                deleteReturnCommand.ExecuteNonQuery();
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
                throw new Exception($"Error deleting sales return: {ex.Message}", ex);
            }
        }

        public string GetNextReturnNumber()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = "SELECT ISNULL(MAX(CAST(SUBSTRING(ReturnNumber, 4, LEN(ReturnNumber)) AS INT)), 0) + 1 FROM SalesReturns WHERE ReturnNumber LIKE 'SRT%'";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"SRT{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating return number: {ex.Message}", ex);
            }
        }

        public List<Sale> GetSalesForReturn()
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

        public Sale GetSaleWithItems(int saleId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Get sale details
                    var saleQuery = @"
                        SELECT s.*, c.CustomerName, u.FullName as UserName
                        FROM Sales s
                        LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON s.UserID = u.UserID
                        WHERE s.SaleID = @SaleID";

                    using (var command = new SqlCommand(saleQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SaleID", saleId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
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

                                reader.Close();

                                // Get sale items
                                var itemsQuery = @"
                                    SELECT si.*, p.ProductName, p.ProductCode
                                    FROM SaleItems si
                                    LEFT JOIN Products p ON si.ProductID = p.ProductID
                                    WHERE si.SaleID = @SaleID";

                                using (var itemsCommand = new SqlCommand(itemsQuery, connection))
                                {
                                    itemsCommand.Parameters.AddWithValue("@SaleID", saleId);
                                    
                                    using (var itemsReader = itemsCommand.ExecuteReader())
                                    {
                                        while (itemsReader.Read())
                                        {
                                            sale.SaleItems.Add(new SaleItem
                                            {
                                                SaleItemID = Convert.ToInt32(itemsReader["SaleItemID"]),
                                                SaleID = Convert.ToInt32(itemsReader["SaleID"]),
                                                ProductID = Convert.ToInt32(itemsReader["ProductID"]),
                                                Quantity = Convert.ToInt32(itemsReader["Quantity"]),
                                                UnitPrice = Convert.ToDecimal(itemsReader["UnitPrice"]),
                                                SubTotal = Convert.ToDecimal(itemsReader["SubTotal"]),
                                                ProductName = itemsReader["ProductName"]?.ToString(),
                                                ProductCode = itemsReader["ProductCode"]?.ToString()
                                            });
                                        }
                                    }
                                }

                                return sale;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale with items: {ex.Message}", ex);
            }

            return null;
        }

        public List<SalesReturn> GetSalesReturnsByDateRange(DateTime fromDate, DateTime toDate)
        {
            var salesReturns = new List<SalesReturn>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT sr.*, c.CustomerName, u.FullName as UserName
                        FROM SalesReturns sr
                        LEFT JOIN Customers c ON sr.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON sr.UserID = u.UserID
                        WHERE sr.ReturnDate BETWEEN @FromDate AND @ToDate
                        ORDER BY sr.ReturnDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var salesReturn = new SalesReturn
                                {
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
                                    ReturnNumber = reader["ReturnNumber"].ToString(),
                                    SaleID = Convert.ToInt32(reader["SaleID"]),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                                    ReturnReason = reader["ReturnReason"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CustomerName = reader["CustomerName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                                
                                salesReturns.Add(salesReturn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales returns by date range: {ex.Message}", ex);
            }

            return salesReturns;
        }
    }
}
