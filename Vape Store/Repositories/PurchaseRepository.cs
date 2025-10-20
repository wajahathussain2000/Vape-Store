using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class PurchaseRepository
    {
        public PurchaseRepository()
        {
        }

        public List<Purchase> GetAllPurchases()
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

        public Purchase GetPurchaseById(int purchaseId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT p.*, s.SupplierName, u.FullName as UserName
                        FROM Purchases p
                        LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON p.UserID = u.UserID
                        WHERE p.PurchaseID = @PurchaseID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Purchase
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchase: {ex.Message}", ex);
            }

            return null;
        }

        public Purchase GetPurchaseByInvoiceNumber(string invoiceNumber)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT p.*, s.SupplierName, u.FullName as UserName
                        FROM Purchases p
                        LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON p.UserID = u.UserID
                        WHERE p.InvoiceNumber = @InvoiceNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Purchase
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchase by invoice number: {ex.Message}", ex);
            }

            return null;
        }

        public List<PurchaseItem> GetPurchaseItems(int purchaseId)
        {
            var purchaseItems = new List<PurchaseItem>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT pi.*, p.ProductName, p.ProductCode
                        FROM PurchaseItems pi
                        LEFT JOIN Products p ON pi.ProductID = p.ProductID
                        WHERE pi.PurchaseID = @PurchaseID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                purchaseItems.Add(new PurchaseItem
                                {
                                    PurchaseItemID = Convert.ToInt32(reader["PurchaseItemID"]),
                                    PurchaseID = Convert.ToInt32(reader["PurchaseID"]),
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    Bonus = Convert.ToInt32(reader["Bonus"]),
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
                throw new Exception($"Error retrieving purchase items: {ex.Message}", ex);
            }

            return purchaseItems;
        }

        public Purchase GetPurchaseWithItems(int purchaseId)
        {
            try
            {
                var purchase = GetPurchaseById(purchaseId);
                if (purchase != null)
                {
                    purchase.PurchaseItems = GetPurchaseItems(purchaseId);
                }
                return purchase;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchase with items: {ex.Message}", ex);
            }
        }

        public Purchase GetPurchaseWithItemsByInvoice(string invoiceNumber)
        {
            try
            {
                var purchase = GetPurchaseByInvoiceNumber(invoiceNumber);
                if (purchase != null)
                {
                    purchase.PurchaseItems = GetPurchaseItems(purchase.PurchaseID);
                }
                return purchase;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchase with items by invoice: {ex.Message}", ex);
            }
        }

        public bool UpdatePurchase(Purchase purchase)
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
                            // Update purchase
                            var purchaseQuery = @"
                                UPDATE Purchases 
                                SET SupplierID = @SupplierID, PurchaseDate = @PurchaseDate, SubTotal = @SubTotal, 
                                    TaxAmount = @TaxAmount, TaxPercent = @TaxPercent, TotalAmount = @TotalAmount, 
                                    PaymentMethod = @PaymentMethod, PaidAmount = @PaidAmount, ChangeAmount = @ChangeAmount, 
                                    UserID = @UserID
                                WHERE PurchaseID = @PurchaseID";

                            using (var purchaseCommand = new SqlCommand(purchaseQuery, connection, transaction))
                            {
                                purchaseCommand.Parameters.AddWithValue("@PurchaseID", purchase.PurchaseID);
                                purchaseCommand.Parameters.AddWithValue("@SupplierID", purchase.SupplierID);
                                purchaseCommand.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                                purchaseCommand.Parameters.AddWithValue("@SubTotal", purchase.SubTotal);
                                purchaseCommand.Parameters.AddWithValue("@TaxAmount", purchase.TaxAmount);
                                purchaseCommand.Parameters.AddWithValue("@TaxPercent", purchase.TaxPercent);
                                purchaseCommand.Parameters.AddWithValue("@TotalAmount", purchase.TotalAmount);
                                purchaseCommand.Parameters.AddWithValue("@PaymentMethod", purchase.PaymentMethod ?? (object)DBNull.Value);
                                purchaseCommand.Parameters.AddWithValue("@PaidAmount", purchase.PaidAmount);
                                purchaseCommand.Parameters.AddWithValue("@ChangeAmount", purchase.ChangeAmount);
                                purchaseCommand.Parameters.AddWithValue("@UserID", purchase.UserID);

                                purchaseCommand.ExecuteNonQuery();
                            }

                            // Delete existing purchase items
                            var deleteItemsQuery = "DELETE FROM PurchaseItems WHERE PurchaseID = @PurchaseID";
                            using (var deleteCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@PurchaseID", purchase.PurchaseID);
                                deleteCommand.ExecuteNonQuery();
                            }

                            // Insert updated purchase items
                            foreach (var item in purchase.PurchaseItems)
                            {
                                var itemQuery = @"
                                    INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, Bonus, UnitPrice, SubTotal)
                                    VALUES (@PurchaseID, @ProductID, @Quantity, @Bonus, @UnitPrice, @SubTotal)";

                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@PurchaseID", purchase.PurchaseID);
                                    itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    itemCommand.Parameters.AddWithValue("@Bonus", item.Bonus);
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
                throw new Exception($"Error updating purchase: {ex.Message}", ex);
            }
        }

        public List<Purchase> GetPurchasesByDateRange(DateTime fromDate, DateTime toDate)
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
                        WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                        ORDER BY p.PurchaseDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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
                                
                                // Load purchase items for each purchase
                                purchase.PurchaseItems = GetPurchaseItems(purchase.PurchaseID);
                                purchases.Add(purchase);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchases by date range: {ex.Message}", ex);
            }

            return purchases;
        }

        public bool DeletePurchase(int purchaseId)
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
                            // Delete purchase items first
                            var deleteItemsQuery = "DELETE FROM PurchaseItems WHERE PurchaseID = @PurchaseID";
                            using (var deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                            {
                                deleteItemsCommand.Parameters.AddWithValue("@PurchaseID", purchaseId);
                                deleteItemsCommand.ExecuteNonQuery();
                            }

                            // Delete purchase
                            var deletePurchaseQuery = "DELETE FROM Purchases WHERE PurchaseID = @PurchaseID";
                            using (var deletePurchaseCommand = new SqlCommand(deletePurchaseQuery, connection, transaction))
                            {
                                deletePurchaseCommand.Parameters.AddWithValue("@PurchaseID", purchaseId);
                                deletePurchaseCommand.ExecuteNonQuery();
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
                throw new Exception($"Error deleting purchase: {ex.Message}", ex);
            }
        }

        public string GetNextInvoiceNumber()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = "SELECT ISNULL(MAX(CAST(SUBSTRING(InvoiceNumber, 4, LEN(InvoiceNumber)) AS INT)), 0) + 1 FROM Purchases WHERE InvoiceNumber LIKE 'PUR%'";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"PUR{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating invoice number: {ex.Message}", ex);
            }
        }

        public List<Purchase> GetPurchasesBySupplierAndDateRange(int supplierId, DateTime fromDate, DateTime toDate)
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
                        WHERE p.SupplierID = @SupplierID AND p.PurchaseDate BETWEEN @FromDate AND @ToDate
                        ORDER BY p.PurchaseDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", supplierId);
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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
                                
                                purchases.Add(purchase);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving purchases by supplier and date range: {ex.Message}", ex);
            }

            return purchases;
        }

        public bool ProcessPurchase(Purchase purchase, List<PurchaseItem> purchaseItems)
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
                            // Insert purchase
                            string purchaseQuery = @"
                                INSERT INTO Purchases (InvoiceNumber, SupplierID, PurchaseDate, SubTotal, TaxAmount, TaxPercent, 
                                                      TotalAmount, PaymentMethod, PaidAmount, ChangeAmount, UserID, CreatedDate)
                                VALUES (@InvoiceNumber, @SupplierID, @PurchaseDate, @SubTotal, @TaxAmount, @TaxPercent, 
                                        @TotalAmount, @PaymentMethod, @PaidAmount, @ChangeAmount, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(purchaseQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@InvoiceNumber", purchase.InvoiceNumber);
                                command.Parameters.AddWithValue("@SupplierID", purchase.SupplierID);
                                command.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                                command.Parameters.AddWithValue("@SubTotal", purchase.SubTotal);
                                command.Parameters.AddWithValue("@TaxAmount", purchase.TaxAmount);
                                command.Parameters.AddWithValue("@TaxPercent", purchase.TaxPercent);
                                command.Parameters.AddWithValue("@TotalAmount", purchase.TotalAmount);
                                command.Parameters.AddWithValue("@PaymentMethod", purchase.PaymentMethod ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@PaidAmount", purchase.PaidAmount);
                                command.Parameters.AddWithValue("@ChangeAmount", purchase.ChangeAmount);
                                command.Parameters.AddWithValue("@UserID", purchase.UserID);
                                command.Parameters.AddWithValue("@CreatedDate", purchase.CreatedDate);

                                int purchaseId = Convert.ToInt32(command.ExecuteScalar());

                                // Insert purchase items
                                foreach (var item in purchaseItems)
                                {
                                    string itemQuery = @"
                                        INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, Bonus, UnitPrice, SubTotal)
                                        VALUES (@PurchaseID, @ProductID, @Quantity, @Bonus, @UnitPrice, @SubTotal)";

                                    using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                    {
                                        itemCommand.Parameters.AddWithValue("@PurchaseID", purchaseId);
                                        itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                        itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                        itemCommand.Parameters.AddWithValue("@Bonus", item.Bonus);
                                        itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                        itemCommand.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                                        itemCommand.ExecuteNonQuery();
                                    }

                                    // Update stock
                                    string stockQuery = @"
                                        UPDATE Products 
                                        SET StockQuantity = StockQuantity + @Quantity + @Bonus,
                                            LastPurchaseDate = @PurchaseDate
                                        WHERE ProductID = @ProductID";

                                    using (var stockCommand = new SqlCommand(stockQuery, connection, transaction))
                                    {
                                        stockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                        stockCommand.Parameters.AddWithValue("@Bonus", item.Bonus);
                                        stockCommand.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                                        stockCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                        stockCommand.ExecuteNonQuery();
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
                throw new Exception($"Error processing purchase: {ex.Message}", ex);
            }
        }
    }
}
