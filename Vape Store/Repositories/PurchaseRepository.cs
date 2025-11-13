using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;
using Vape_Store.Services;

namespace Vape_Store.Repositories
{
    public class PurchaseRepository
    {
        private BusinessDateService _businessDateService;

        public PurchaseRepository()
        {
            _businessDateService = new BusinessDateService();
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
                        SELECT 
                            pi.PurchaseItemID,
                            pi.PurchaseID,
                            pi.ProductID,
                            pi.Quantity - ISNULL(SUM(pri.Quantity), 0) as Quantity,
                            pi.Bonus,
                            pi.UnitPrice,
                            pi.SubTotal - ISNULL(SUM(pri.SubTotal), 0) as SubTotal,
                            p.ProductName,
                            p.ProductCode
                        FROM PurchaseItems pi
                        LEFT JOIN Products p ON pi.ProductID = p.ProductID
                        LEFT JOIN PurchaseReturns pr ON pi.PurchaseID = pr.PurchaseID
                        LEFT JOIN PurchaseReturnItems pri ON pr.ReturnID = pri.ReturnID 
                            AND pi.ProductID = pri.ProductID
                        WHERE pi.PurchaseID = @PurchaseID
                        GROUP BY pi.PurchaseItemID, pi.PurchaseID, pi.ProductID, pi.Quantity, 
                                 pi.Bonus, pi.UnitPrice, pi.SubTotal, p.ProductName, p.ProductCode
                        HAVING pi.Quantity - ISNULL(SUM(pri.Quantity), 0) > 0";

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

                            // Delete existing purchase items and re-insert
                            // NOTE: This operation is transaction-safe. If any subsequent operation fails,
                            // the entire transaction (including this DELETE) will be rolled back.
                            // This pattern ensures data consistency during updates.
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
                                    INSERT INTO PurchaseItems (PurchaseID, ProductID, ProductName, ProductCode, Quantity, Unit, 
                                                               UnitPrice, SellingPrice, SubTotal, Bonus, BatchNumber, ExpiryDate, 
                                                               DiscountAmount, TaxPercent, Remarks)
                                    VALUES (@PurchaseID, @ProductID, @ProductName, @ProductCode, @Quantity, @Unit, 
                                            @UnitPrice, @SellingPrice, @SubTotal, @Bonus, @BatchNumber, @ExpiryDate, 
                                            @DiscountAmount, @TaxPercent, @Remarks)";

                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@PurchaseID", purchase.PurchaseID);
                                    itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                    itemCommand.Parameters.AddWithValue("@ProductName", item.ProductName ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@ProductCode", item.ProductCode ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    itemCommand.Parameters.AddWithValue("@Unit", item.Unit ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                    itemCommand.Parameters.AddWithValue("@SellingPrice", item.SellingPrice);
                                    itemCommand.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                                    itemCommand.Parameters.AddWithValue("@Bonus", item.Bonus);
                                    itemCommand.Parameters.AddWithValue("@BatchNumber", item.BatchNumber ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@ExpiryDate", item.ExpiryDate);
                                    itemCommand.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                                    itemCommand.Parameters.AddWithValue("@TaxPercent", item.TaxPercent);
                                    itemCommand.Parameters.AddWithValue("@Remarks", item.Remarks ?? (object)DBNull.Value);

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
                    // Use CAST to compare date portion only for more reliable date matching
                    // This ensures purchases are found regardless of time component
                    var query = @"
                        SELECT p.*, s.SupplierName, u.FullName as UserName
                        FROM Purchases p
                        LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
                        LEFT JOIN Users u ON p.UserID = u.UserID
                        WHERE CAST(p.PurchaseDate AS DATE) >= CAST(@FromDate AS DATE) 
                          AND CAST(p.PurchaseDate AS DATE) <= CAST(@ToDate AS DATE)
                        ORDER BY p.PurchaseDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Ensure dates are properly formatted
                        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
                        command.Parameters.AddWithValue("@ToDate", toDate.Date);
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
            // Validate date - check if the purchase date is closed
            if (!_businessDateService.CanCreateTransaction(purchase.PurchaseDate))
            {
                string message = _businessDateService.GetValidationMessage(purchase.PurchaseDate);
                throw new InvalidOperationException(message);
            }

            // Validate purchase items for negative or zero quantities
            foreach (var item in purchaseItems)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException($"Invalid quantity ({item.Quantity}) for product {item.ProductName}. Quantity must be greater than zero.");
                }
                if (item.UnitPrice < 0)
                {
                    throw new ArgumentException($"Invalid unit price ({item.UnitPrice}) for product {item.ProductName}. Price cannot be negative.");
                }
                if (item.Bonus < 0)
                {
                    throw new ArgumentException($"Invalid bonus ({item.Bonus}) for product {item.ProductName}. Bonus cannot be negative.");
                }
            }
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int purchaseId = 0;
                            
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

                                System.Diagnostics.Debug.WriteLine($"Executing INSERT for Purchase - Invoice: {purchase.InvoiceNumber}, Date: {purchase.PurchaseDate:yyyy-MM-dd HH:mm:ss}, UserID: {purchase.UserID}");
                                
                                object result = command.ExecuteScalar();
                                if (result == null || result == DBNull.Value)
                                {
                                    throw new Exception("Failed to insert purchase - no PurchaseID returned from database");
                                }
                                
                                purchaseId = Convert.ToInt32(result);
                                System.Diagnostics.Debug.WriteLine($"Purchase inserted successfully! PurchaseID: {purchaseId}");
                            }

                            // Insert purchase items
                            System.Diagnostics.Debug.WriteLine($"Inserting {purchaseItems.Count} purchase items...");
                            int itemIndex = 0;
                            foreach (var item in purchaseItems)
                            {
                                itemIndex++;
                                System.Diagnostics.Debug.WriteLine($"Inserting item {itemIndex}/{purchaseItems.Count}: ProductID={item.ProductID}, ProductName={item.ProductName}, Quantity={item.Quantity}");
                                
                                // AUTO-CREATE PRODUCT IF IT DOESN'T EXIST
                                // This is the key fix: When purchasing a new product, automatically add it to Products table
                                if (item.ProductID == 0 || !ProductExists(item.ProductID, connection, transaction))
                                {
                                    // Create new product in Products table
                                    string createProductQuery = @"
                                        INSERT INTO Products (ProductCode, ProductName, PurchasePrice, CostPrice, RetailPrice, 
                                                              StockQuantity, ReorderLevel, IsActive, IsAvailableForSale, LastPurchaseDate)
                                        VALUES (@ProductCode, @ProductName, @PurchasePrice, @CostPrice, @RetailPrice, 
                                                0, 10, 1, 1, @PurchaseDate);
                                        SELECT SCOPE_IDENTITY();";
                                    
                                    using (var createProductCmd = new SqlCommand(createProductQuery, connection, transaction))
                                    {
                                        createProductCmd.Parameters.AddWithValue("@ProductCode", item.ProductCode ?? "AUTO-" + Guid.NewGuid().ToString().Substring(0, 8));
                                        createProductCmd.Parameters.AddWithValue("@ProductName", item.ProductName ?? "Unknown Product");
                                        createProductCmd.Parameters.AddWithValue("@PurchasePrice", item.UnitPrice);
                                        createProductCmd.Parameters.AddWithValue("@CostPrice", item.UnitPrice);
                                        createProductCmd.Parameters.AddWithValue("@RetailPrice", item.SellingPrice > 0 ? item.SellingPrice : item.UnitPrice * 1.3m);
                                        createProductCmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                                        
                                        int newProductId = Convert.ToInt32(createProductCmd.ExecuteScalar());
                                        item.ProductID = newProductId; // Update the item with the new ProductID
                                    }
                                }
                                
                                string itemQuery = @"
                                    INSERT INTO PurchaseItems (PurchaseID, ProductID, ProductName, ProductCode, Quantity, Unit, 
                                                               UnitPrice, SellingPrice, SubTotal, Bonus, BatchNumber, ExpiryDate, 
                                                               DiscountAmount, TaxPercent, Remarks)
                                    VALUES (@PurchaseID, @ProductID, @ProductName, @ProductCode, @Quantity, @Unit, 
                                            @UnitPrice, @SellingPrice, @SubTotal, @Bonus, @BatchNumber, @ExpiryDate, 
                                            @DiscountAmount, @TaxPercent, @Remarks)";

                                using (var itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@PurchaseID", purchaseId);
                                    itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                    itemCommand.Parameters.AddWithValue("@ProductName", item.ProductName ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@ProductCode", item.ProductCode ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    itemCommand.Parameters.AddWithValue("@Unit", item.Unit ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                    itemCommand.Parameters.AddWithValue("@SellingPrice", item.SellingPrice);
                                    itemCommand.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                                    itemCommand.Parameters.AddWithValue("@Bonus", item.Bonus);
                                    itemCommand.Parameters.AddWithValue("@BatchNumber", item.BatchNumber ?? (object)DBNull.Value);
                                    itemCommand.Parameters.AddWithValue("@ExpiryDate", item.ExpiryDate);
                                    itemCommand.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                                    itemCommand.Parameters.AddWithValue("@TaxPercent", item.TaxPercent);
                                    itemCommand.Parameters.AddWithValue("@Remarks", item.Remarks ?? (object)DBNull.Value);
                                    itemCommand.ExecuteNonQuery();
                                }

                                // Update stock with improved error handling and logging
                                // Check if optional columns exist before updating
                                string enhancedStockQuery = @"
                                    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'LastPurchaseDate')
                                    BEGIN
                                        UPDATE Products 
                                        SET StockQuantity = StockQuantity + @Quantity + @Bonus,
                                            LastPurchaseDate = @PurchaseDate,
                                            IsAvailableForSale = 1
                                        WHERE ProductID = @ProductID
                                    END
                                    ELSE
                                    BEGIN
                                        UPDATE Products 
                                        SET StockQuantity = StockQuantity + @Quantity + @Bonus
                                        WHERE ProductID = @ProductID
                                    END";

                                using (var stockCommand = new SqlCommand(enhancedStockQuery, connection, transaction))
                                {
                                    stockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    stockCommand.Parameters.AddWithValue("@Bonus", item.Bonus);
                                    stockCommand.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                                    stockCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                    stockCommand.ExecuteNonQuery();
                                }
                                
                                System.Diagnostics.Debug.WriteLine($"Item {itemIndex} inserted successfully");
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"All {purchaseItems.Count} purchase items inserted successfully");

                            // Commit transaction after all operations succeed
                            transaction.Commit();
                            
                            System.Diagnostics.Debug.WriteLine($"Purchase saved successfully! PurchaseID: {purchaseId}, Invoice: {purchase.InvoiceNumber}");
                            
                            // Trigger product update event to refresh sales form
                            ProductRepository.OnProductsUpdated();
                            
                            return true;
                        }
                        catch (Exception innerEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Transaction error: {innerEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack Trace: {innerEx.StackTrace}");
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProcessPurchase Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw new Exception($"Error processing purchase: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to check if a product exists in the Products table
        /// </summary>
        private bool ProductExists(int productId, SqlConnection connection, SqlTransaction transaction)
        {
            if (productId <= 0) return false;
            
            string query = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ProductID", productId);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }
}
