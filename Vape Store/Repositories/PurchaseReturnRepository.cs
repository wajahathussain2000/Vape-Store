using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class PurchaseReturnRepository
    {
        public PurchaseReturnRepository()
        {
        }

        public List<PurchaseReturn> GetAllPurchaseReturns()
        {
            var purchaseReturns = new List<PurchaseReturn>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT pr.*, s.SupplierName, u.FullName as UserName, p.InvoiceNumber
                        FROM PurchaseReturns pr
                        LEFT JOIN Suppliers s ON pr.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON pr.UserID = u.UserID
                        LEFT JOIN Purchases p ON pr.PurchaseID = p.PurchaseID
                        ORDER BY pr.ReturnDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                purchaseReturns.Add(new PurchaseReturn
                                {
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
                                    ReturnNumber = reader["ReturnNumber"].ToString(),
                                    PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                                    ReturnReason = reader["ReturnReason"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
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
                throw new Exception($"Error retrieving purchase returns: {ex.Message}", ex);
            }

            return purchaseReturns;
        }

        public PurchaseReturn GetPurchaseReturnById(int returnId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT pr.*, s.SupplierName, u.FullName as UserName, p.InvoiceNumber
                        FROM PurchaseReturns pr
                        LEFT JOIN Suppliers s ON pr.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON pr.UserID = u.UserID
                        LEFT JOIN Purchases p ON pr.PurchaseID = p.PurchaseID
                        WHERE pr.ReturnID = @ReturnID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReturnID", returnId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new PurchaseReturn
                                {
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
                                    ReturnNumber = reader["ReturnNumber"].ToString(),
                                    PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                                    ReturnReason = reader["ReturnReason"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
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
                throw new Exception($"Error retrieving purchase return: {ex.Message}", ex);
            }

            return null;
        }

        public List<PurchaseReturnItem> GetPurchaseReturnItems(int returnId)
        {
            var returnItems = new List<PurchaseReturnItem>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT pri.*, p.ProductName, p.ProductCode
                        FROM PurchaseReturnItems pri
                        LEFT JOIN Products p ON pri.ProductID = p.ProductID
                        WHERE pri.ReturnID = @ReturnID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReturnID", returnId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                returnItems.Add(new PurchaseReturnItem
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
                throw new Exception($"Error retrieving purchase return items: {ex.Message}", ex);
            }

            return returnItems;
        }

        public bool AddPurchaseReturn(PurchaseReturn purchaseReturn)
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
                            // Insert purchase return
                            var returnQuery = @"
                                INSERT INTO PurchaseReturns (ReturnNumber, PurchaseID, SupplierID, ReturnDate, ReturnReason, Description, TotalAmount, UserID, CreatedDate)
                                VALUES (@ReturnNumber, @PurchaseID, @SupplierID, @ReturnDate, @ReturnReason, @Description, @TotalAmount, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var returnCommand = new SqlCommand(returnQuery, connection, transaction))
                            {
                                returnCommand.Parameters.AddWithValue("@ReturnNumber", purchaseReturn.ReturnNumber);
                                returnCommand.Parameters.AddWithValue("@PurchaseID", purchaseReturn.PurchaseID);
                                returnCommand.Parameters.AddWithValue("@SupplierID", purchaseReturn.SupplierID);
                                returnCommand.Parameters.AddWithValue("@ReturnDate", purchaseReturn.ReturnDate);
                                returnCommand.Parameters.AddWithValue("@ReturnReason", purchaseReturn.ReturnReason ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@Description", purchaseReturn.Description ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@TotalAmount", purchaseReturn.TotalAmount);
                                returnCommand.Parameters.AddWithValue("@UserID", purchaseReturn.UserID);
                                returnCommand.Parameters.AddWithValue("@CreatedDate", purchaseReturn.CreatedDate);

                                var returnId = Convert.ToInt32(returnCommand.ExecuteScalar());

                                // Insert return items
                                foreach (var item in purchaseReturn.ReturnItems)
                                {
                                    var itemQuery = @"
                                        INSERT INTO PurchaseReturnItems (ReturnID, ProductID, Quantity, UnitPrice, SubTotal)
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
                throw new Exception($"Error adding purchase return: {ex.Message}", ex);
            }
        }

        public bool UpdatePurchaseReturn(PurchaseReturn purchaseReturn)
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
                            // Update purchase return
                            var returnQuery = @"
                                UPDATE PurchaseReturns 
                                SET ReturnNumber = @ReturnNumber, PurchaseID = @PurchaseID, SupplierID = @SupplierID, 
                                    ReturnDate = @ReturnDate, ReturnReason = @ReturnReason, Description = @Description, 
                                    TotalAmount = @TotalAmount, UserID = @UserID
                                WHERE ReturnID = @ReturnID";

                            using (var returnCommand = new SqlCommand(returnQuery, connection, transaction))
                            {
                                returnCommand.Parameters.AddWithValue("@ReturnID", purchaseReturn.ReturnID);
                                returnCommand.Parameters.AddWithValue("@ReturnNumber", purchaseReturn.ReturnNumber);
                                returnCommand.Parameters.AddWithValue("@PurchaseID", purchaseReturn.PurchaseID);
                                returnCommand.Parameters.AddWithValue("@SupplierID", purchaseReturn.SupplierID);
                                returnCommand.Parameters.AddWithValue("@ReturnDate", purchaseReturn.ReturnDate);
                                returnCommand.Parameters.AddWithValue("@ReturnReason", purchaseReturn.ReturnReason ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@Description", purchaseReturn.Description ?? (object)DBNull.Value);
                                returnCommand.Parameters.AddWithValue("@TotalAmount", purchaseReturn.TotalAmount);
                                returnCommand.Parameters.AddWithValue("@UserID", purchaseReturn.UserID);

                                returnCommand.ExecuteNonQuery();
                            }

                            // Delete existing return items
                            var deleteItemsQuery = "DELETE FROM PurchaseReturnItems WHERE ReturnID = @ReturnID";
                            using (var deleteCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@ReturnID", purchaseReturn.ReturnID);
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Insert updated return items
                            foreach (var item in purchaseReturn.ReturnItems)
                            {
                                var itemQuery = @"
                                    INSERT INTO PurchaseReturnItems (ReturnID, ProductID, Quantity, UnitPrice, SubTotal)
                                    VALUES (@ReturnID, @ProductID, @Quantity, @UnitPrice, @SubTotal)";

                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@ReturnID", purchaseReturn.ReturnID);
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
                throw new Exception($"Error updating purchase return: {ex.Message}", ex);
            }
        }

        public bool DeletePurchaseReturn(int returnId)
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
                            var deleteItemsQuery = "DELETE FROM PurchaseReturnItems WHERE ReturnID = @ReturnID";
                            using (var deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteItemsCommand.Parameters.AddWithValue("@ReturnID", returnId);
                                deleteItemsCommand.ExecuteNonQuery();
                            }

                            // Delete purchase return
                            var deleteReturnQuery = "DELETE FROM PurchaseReturns WHERE ReturnID = @ReturnID";
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
                throw new Exception($"Error deleting purchase return: {ex.Message}", ex);
            }
        }

        public string GetNextReturnNumber()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = "SELECT ISNULL(MAX(CAST(SUBSTRING(ReturnNumber, 4, LEN(ReturnNumber)) AS INT)), 0) + 1 FROM PurchaseReturns WHERE ReturnNumber LIKE 'PRT%'";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"PRT{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating return number: {ex.Message}", ex);
            }
        }

        public List<Purchase> GetPurchasesForReturn()
        {
            var purchases = new List<Purchase>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT p.*, s.SupplierName, u.FullName as UserName
                        FROM Purchases p
                        LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON p.UserID = u.UserID
                        ORDER BY p.PurchaseDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                purchases.Add(new Purchase
                                {
                                    PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchases: {ex.Message}", ex);
            }

            return purchases;
        }

        public Purchase GetPurchaseWithItems(int purchaseId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Get purchase details
                    var purchaseQuery = @"
                        SELECT p.*, s.SupplierName, u.FullName as UserName
                        FROM Purchases p
                        LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON p.UserID = u.UserID
                        WHERE p.PurchaseID = @PurchaseID";

                    using (var command = new SqlCommand(purchaseQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var purchase = new Purchase
                                {
                                    PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"]),
                                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                    TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                    TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                                    ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };

                                reader.Close();

                                // Get purchase items
                                var itemsQuery = @"
                                    SELECT pi.*, p.ProductName, p.ProductCode
                                    FROM PurchaseItems pi
                                    LEFT JOIN Products p ON pi.ProductID = p.ProductID
                                    WHERE pi.PurchaseID = @PurchaseID";

                                using (var itemsCommand = new SqlCommand(itemsQuery, connection))
                                {
                                    itemsCommand.Parameters.AddWithValue("@PurchaseID", purchaseId);
                                    
                                    using (var itemsReader = itemsCommand.ExecuteReader())
                                    {
                                        while (itemsReader.Read())
                                        {
                                            purchase.PurchaseItems.Add(new PurchaseItem
                                            {
                                                PurchaseItemID = Convert.ToInt32(itemsReader["PurchaseItemID"]),
                                                PurchaseID = Convert.ToInt32(itemsReader["PurchaseID"]),
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

                                return purchase;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchase with items: {ex.Message}", ex);
            }

            return null;
        }

        public List<PurchaseReturn> GetPurchaseReturnsByDateRange(DateTime fromDate, DateTime toDate)
        {
            var purchaseReturns = new List<PurchaseReturn>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT pr.*, s.SupplierName, u.FullName as UserName
                        FROM PurchaseReturns pr
                        LEFT JOIN Suppliers s ON pr.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON pr.UserID = u.UserID
                        WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                        ORDER BY pr.ReturnDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var purchaseReturn = new PurchaseReturn
                                {
                                    ReturnID = Convert.ToInt32(reader["ReturnID"]),
                                    ReturnNumber = reader["ReturnNumber"].ToString(),
                                    PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                                    SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                    ReturnDate = Convert.ToDateTime(reader["ReturnDate"]),
                                    ReturnReason = reader["ReturnReason"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    SupplierName = reader["SupplierName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                                
                                purchaseReturns.Add(purchaseReturn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchase returns by date range: {ex.Message}", ex);
            }

            return purchaseReturns;
        }
    }
}
