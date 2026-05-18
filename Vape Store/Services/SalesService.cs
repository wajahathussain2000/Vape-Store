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
        private readonly CustomerLedgerRepository _customerLedgerRepository;
        
        public SalesService()
        {
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository();
            _businessDateService = new BusinessDateService();
            _customerLedgerRepository = new CustomerLedgerRepository();
        }

        private void ReconcileBatches(int productId, SqlConnection connection, SqlTransaction transaction = null)
        {
            // 1. Get current stock info from Products
            int stockQty = 0;
            decimal costPrice = 0;
            decimal retailPrice = 0;
            string productName = "";
            string productCode = "";

            string prodQuery = "SELECT StockQuantity, CostPrice, RetailPrice, ProductName, ProductCode FROM Products WHERE ProductID = @ProductID";
            using (var cmd = new SqlCommand(prodQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductID", productId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stockQty = Convert.ToInt32(reader["StockQuantity"]);
                        costPrice = Convert.ToDecimal(reader["CostPrice"]);
                        retailPrice = Convert.ToDecimal(reader["RetailPrice"]);
                        productName = reader["ProductName"]?.ToString() ?? "";
                        productCode = reader["ProductCode"]?.ToString() ?? "";
                    }
                    else
                    {
                        return; // Product doesn't exist
                    }
                }
            }

            if (stockQty <= 0) return;

            // 2. Sum remaining quantities in PurchaseItems
            int sumRemaining = 0;
            string sumQuery = "SELECT ISNULL(SUM(RemainingQuantity), 0) FROM PurchaseItems WHERE ProductID = @ProductID";
            using (var cmd = new SqlCommand(sumQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductID", productId);
                var sumRes = cmd.ExecuteScalar();
                sumRemaining = sumRes != null && sumRes != DBNull.Value ? Convert.ToInt32(sumRes) : 0;
            }

            if (stockQty > sumRemaining)
            {
                int diff = stockQty - sumRemaining;

                // Check if any purchase item exists for this product
                int latestPurchaseItemId = 0;
                string checkQuery = "SELECT TOP 1 PurchaseItemID FROM PurchaseItems WHERE ProductID = @ProductID ORDER BY PurchaseItemID DESC";
                using (var cmd = new SqlCommand(checkQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                    {
                        latestPurchaseItemId = Convert.ToInt32(res);
                    }
                }

                if (latestPurchaseItemId > 0)
                {
                    // Add the difference to the latest purchase item's remaining quantity
                    string updateQuery = "UPDATE PurchaseItems SET RemainingQuantity = RemainingQuantity + @Diff WHERE PurchaseItemID = @PurchaseItemID";
                    using (var cmd = new SqlCommand(updateQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Diff", diff);
                        cmd.Parameters.AddWithValue("@PurchaseItemID", latestPurchaseItemId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // No purchase items exist at all for this product! We must create one.
                    // Get an existing PurchaseID to link to, or create a system restoration purchase.
                    int purchaseId = 0;
                    string purchaseCheck = "SELECT TOP 1 PurchaseID FROM Purchases WHERE InvoiceNumber = 'SYS-AUTO-RESTORE'";
                    using (var cmd = new SqlCommand(purchaseCheck, connection, transaction))
                    {
                        var res = cmd.ExecuteScalar();
                        if (res != null && res != DBNull.Value)
                        {
                            purchaseId = Convert.ToInt32(res);
                        }
                    }

                    if (purchaseId == 0)
                    {
                        int supplierId = 0;
                        string supCheck = "SELECT TOP 1 SupplierID FROM Suppliers ORDER BY SupplierID ASC";
                        using (var cmd = new SqlCommand(supCheck, connection, transaction))
                        {
                            var res = cmd.ExecuteScalar();
                            if (res != null && res != DBNull.Value) supplierId = Convert.ToInt32(res);
                        }

                        int userId = 1;
                        string userCheck = "SELECT TOP 1 UserID FROM Users ORDER BY UserID ASC";
                        using (var cmd = new SqlCommand(userCheck, connection, transaction))
                        {
                            var res = cmd.ExecuteScalar();
                            if (res != null && res != DBNull.Value) userId = Convert.ToInt32(res);
                        }

                        // Create system restoration purchase
                        string createPurchase = @"
                            INSERT INTO Purchases (InvoiceNumber, SupplierID, PurchaseDate, SubTotal, TaxAmount, TaxPercent, TotalAmount, CreatedDate, UserID)
                            VALUES ('SYS-AUTO-RESTORE', @SupplierID, GETDATE(), 0, 0, 0, 0, GETDATE(), @UserID);
                            SELECT SCOPE_IDENTITY();";
                        
                        using (var cmd = new SqlCommand(createPurchase, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@SupplierID", supplierId > 0 ? (object)supplierId : DBNull.Value);
                            cmd.Parameters.AddWithValue("@UserID", userId);
                            purchaseId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }

                    // Insert purchase item representing the missing batch
                    string insertItem = @"
                        INSERT INTO PurchaseItems (PurchaseID, ProductID, ProductName, ProductCode, Quantity, RemainingQuantity, UnitPrice, SellingPrice, SubTotal, Bonus, ExpiryDate, TaxPercent, DiscountAmount)
                        VALUES (@PurchaseID, @ProductID, @ProductName, @ProductCode, @Qty, @Qty, @CostPrice, @RetailPrice, 0, 0, GETDATE(), 0, 0)";
                    
                    using (var cmd = new SqlCommand(insertItem, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@ProductName", productName);
                        cmd.Parameters.AddWithValue("@ProductCode", productCode);
                        cmd.Parameters.AddWithValue("@Qty", diff);
                        cmd.Parameters.AddWithValue("@CostPrice", costPrice);
                        cmd.Parameters.AddWithValue("@RetailPrice", retailPrice);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
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
                        
                        // Process each sale item using FIFO logic
                        foreach (var item in sale.SaleItems)
                        {
                            item.SaleID = saleID;
                            int remainingToSell = item.Quantity;

                            // Reconcile stock batches dynamically on-the-fly
                            ReconcileBatches(item.ProductID, connection, transaction);

                            // 1. Get available batches for this product, ordered by purchase date (FIFO)
                            string batchQuery = @"
                                SELECT pi.PurchaseItemID, pi.RemainingQuantity, pi.UnitPrice as CostPrice, pi.SellingPrice
                                FROM PurchaseItems pi
                                JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                                WHERE pi.ProductID = @ProductID AND pi.RemainingQuantity > 0
                                ORDER BY p.PurchaseDate ASC, pi.PurchaseItemID ASC";

                            using (var batchCmd = new SqlCommand(batchQuery, connection, transaction))
                            {
                                batchCmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                                List<dynamic> batches = new List<dynamic>();
                                using (var reader = batchCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        batches.Add(new {
                                            Id = Convert.ToInt32(reader["PurchaseItemID"]),
                                            Remaining = Convert.ToInt32(reader["RemainingQuantity"]),
                                            Cost = Convert.ToDecimal(reader["CostPrice"]),
                                            Price = Convert.ToDecimal(reader["SellingPrice"])
                                        });
                                    }
                                }

                                foreach (var batch in batches)
                                {
                                    if (remainingToSell <= 0) break;

                                    int consume = Math.Min(remainingToSell, batch.Remaining);
                                    
                                    // Create a sale item for this batch portion
                                    var saleItemPart = new SaleItem
                                    {
                                        SaleID = saleID,
                                        ProductID = item.ProductID,
                                        Quantity = consume,
                                        UnitPrice = batch.Price,
                                        SubTotal = consume * batch.Price,
                                        CostPrice = batch.Cost,
                                        PurchaseItemID = batch.Id
                                    };
                                    
                                    InsertSaleItem(saleItemPart, connection, transaction);

                                    // 2. Update RemainingQuantity in PurchaseItems
                                    string updateBatchQuery = "UPDATE PurchaseItems SET RemainingQuantity = RemainingQuantity - @Consume WHERE PurchaseItemID = @BatchID";
                                    using (var updateBatchCmd = new SqlCommand(updateBatchQuery, connection, transaction))
                                    {
                                        updateBatchCmd.Parameters.AddWithValue("@Consume", consume);
                                        updateBatchCmd.Parameters.AddWithValue("@BatchID", batch.Id);
                                        updateBatchCmd.ExecuteNonQuery();
                                    }

                                    remainingToSell -= consume;
                                }
                            }

                            // If there's still quantity to sell but no batches (over-selling scenario)
                            if (remainingToSell > 0)
                            {
                                throw new InvalidOperationException($"Insufficient stock for product. Required extra: {remainingToSell}");
                            }

                            // 3. Update global Product stock quantity
                            _productRepository.UpdateStock(item.ProductID, -item.Quantity);
                        }
                        
                        if (sale.CustomerID > 0)
                        {
                            InsertCustomerLedgerEntries(sale, saleID, connection, transaction);
                        }
                        
                        transaction.Commit();
                        // Trigger product update event to refresh sales form
                        ProductRepository.OnProductsUpdated();
                        
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

        private void InsertCustomerLedgerEntries(Sale sale, int saleId, SqlConnection connection, SqlTransaction transaction)
        {
            if (sale.CustomerID <= 0)
                return;

            var saleEntry = new CustomerLedgerEntry
            {
                CustomerID = sale.CustomerID,
                EntryDate = sale.SaleDate,
                ReferenceType = "Sale",
                ReferenceID = saleId,
                InvoiceNumber = sale.InvoiceNumber,
                Description = $"Sale Invoice {sale.InvoiceNumber}",
                Debit = sale.TotalAmount,
                Credit = 0,
                CreatedDate = DateTime.Now
            };

            _customerLedgerRepository.InsertEntry(connection, transaction, saleEntry);

            if (sale.PaidAmount > 0)
            {
                var paymentEntry = new CustomerLedgerEntry
                {
                    CustomerID = sale.CustomerID,
                    EntryDate = sale.SaleDate,
                    ReferenceType = "SalePayment",
                    ReferenceID = saleId,
                    InvoiceNumber = sale.InvoiceNumber,
                    Description = $"Payment Received ({sale.PaymentMethod})",
                    Debit = 0,
                    Credit = sale.PaidAmount,
                    CreatedDate = DateTime.Now
                };

                _customerLedgerRepository.InsertEntry(connection, transaction, paymentEntry);
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
                
                // Explicitly set type to VarBinary for image and data to avoid implicit conversion errors
                var imgParam = command.Parameters.Add("@BarcodeImage", System.Data.SqlDbType.VarBinary);
                imgParam.Value = sale.BarcodeImage ?? (object)DBNull.Value;
                
                var dataParam = command.Parameters.Add("@BarcodeData", System.Data.SqlDbType.VarBinary);
                if (!string.IsNullOrEmpty(sale.BarcodeData))
                {
                    dataParam.Value = System.Text.Encoding.UTF8.GetBytes(sale.BarcodeData);
                }
                else
                {
                    dataParam.Value = DBNull.Value;
                }

                command.Parameters.AddWithValue("@DiscountAmount", sale.DiscountAmount);
                command.Parameters.AddWithValue("@DiscountPercent", sale.DiscountPercent);
                
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        
        private void InsertSaleItem(SaleItem item, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice, Discount, DiscountPercent, TaxPercent, TaxAmount, SubTotal, CostPrice, PurchaseItemID) 
                           VALUES (@SaleID, @ProductID, @Quantity, @UnitPrice, @Discount, @DiscountPercent, @TaxPercent, @TaxAmount, @SubTotal, @CostPrice, @PurchaseItemID)";
            
            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@SaleID", item.SaleID);
                command.Parameters.AddWithValue("@ProductID", item.ProductID);
                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                command.Parameters.AddWithValue("@Discount", item.Discount);
                command.Parameters.AddWithValue("@DiscountPercent", item.DiscountPercent);
                command.Parameters.AddWithValue("@TaxPercent", item.TaxPercent);
                command.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                command.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                command.Parameters.AddWithValue("@CostPrice", item.CostPrice);
                command.Parameters.AddWithValue("@PurchaseItemID", (object)item.PurchaseItemID ?? DBNull.Value);
                
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
            string query = @"SELECT si.SaleItemID, si.SaleID, si.ProductID, si.Quantity, si.UnitPrice,
                           ISNULL(si.Discount, 0) AS Discount, ISNULL(si.DiscountPercent, 0) AS DiscountPercent,
                           ISNULL(si.TaxPercent, 0) AS TaxPercent, ISNULL(si.TaxAmount, 0) AS TaxAmount,
                           si.SubTotal,
                           ISNULL(si.CostPrice, 0) AS CostPrice, si.PurchaseItemID,
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
                                Discount = Convert.ToDecimal(reader["Discount"]),
                                DiscountPercent = Convert.ToDecimal(reader["DiscountPercent"]),
                                TaxPercent = Convert.ToDecimal(reader["TaxPercent"]),
                                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                                CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                                PurchaseItemID = reader["PurchaseItemID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["PurchaseItemID"]) : null,
                                ProductName = reader["ProductName"].ToString(),
                                ProductCode = reader["ProductCode"].ToString()
                            });
                        }
                    }
                }
            }
            
            return items;
        }
        
        public List<SaleItem> GetFIFOBreakdown(Product product, int quantity)
        {
            var breakdowns = new List<SaleItem>();
            int remainingToSell = quantity;

            string batchQuery = @"
                SELECT pi.PurchaseItemID, pi.RemainingQuantity, pi.UnitPrice as CostPrice, pi.SellingPrice
                FROM PurchaseItems pi
                JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
                WHERE pi.ProductID = @ProductID AND pi.RemainingQuantity > 0
                ORDER BY p.PurchaseDate ASC, pi.PurchaseItemID ASC";

            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                ReconcileBatches(product.ProductID, connection);

                using (var batchCmd = new SqlCommand(batchQuery, connection))
                {
                    batchCmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    using (var reader = batchCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (remainingToSell <= 0) break;

                            int batchRemaining = Convert.ToInt32(reader["RemainingQuantity"]);
                            decimal batchCost = Convert.ToDecimal(reader["CostPrice"]);
                            decimal batchPrice = Convert.ToDecimal(reader["SellingPrice"]);
                            int batchId = Convert.ToInt32(reader["PurchaseItemID"]);

                            int consume = Math.Min(remainingToSell, batchRemaining);

                            breakdowns.Add(new SaleItem
                            {
                                ProductID = product.ProductID,
                                ProductName = product.ProductName,
                                ProductCode = product.ProductCode,
                                Quantity = consume,
                                UnitPrice = batchPrice,
                                SubTotal = consume * batchPrice,
                                CostPrice = batchCost,
                                PurchaseItemID = batchId
                            });

                            remainingToSell -= consume;
                        }
                    }
                }
            }

            if (remainingToSell > 0)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.ProductName}. Required extra: {remainingToSell}");
            }

            return breakdowns;
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
