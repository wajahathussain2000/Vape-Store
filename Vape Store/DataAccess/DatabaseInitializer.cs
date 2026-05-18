using System;
using System.Data.SqlClient;

namespace Vape_Store.DataAccess
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    EnsureCustomerLedgerTable(connection);
                    EnsureStoreSettingsTable(connection);
                    EnsureSalesReturnItemsMigration(connection);
                    HealDatabaseStockBatches(connection);
                }
            }
            catch (Exception ex)
            {
                // In a real app we might want to log this or show a message, 
                // but we don't want to crash start up if it's just a minor connection issue that might resolve later.
                // However, missing tables are critical.
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        private static void EnsureSalesReturnItemsMigration(SqlConnection connection)
        {
            try
            {
                string checkColumnQuery = @"
                    IF NOT EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'SalesReturnItems' AND COLUMN_NAME = 'IsResellable'
                    )
                    BEGIN
                        ALTER TABLE SalesReturnItems ADD IsResellable BIT NOT NULL DEFAULT 1;
                    END";
                
                using (var command = new SqlCommand(checkColumnQuery, connection))
                {
                    command.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Ensured SalesReturnItems has IsResellable column.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to run SalesReturnItems migration: {ex.Message}");
            }
        }

        private static void EnsureCustomerLedgerTable(SqlConnection connection)
        {
            string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CustomerLedger'";
            
            using (var command = new SqlCommand(checkTableQuery, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    // Table missing, create it
                    string createTableQuery = @"
                        CREATE TABLE [dbo].[CustomerLedger](
                            [LedgerEntryID] [int] IDENTITY(1,1) NOT NULL,
                            [CustomerID] [int] NOT NULL,
                            [EntryDate] [datetime] NOT NULL,
                            [ReferenceType] [varchar](50) NULL,
                            [ReferenceID] [int] NULL,
                            [InvoiceNumber] [varchar](50) NULL,
                            [Description] [varchar](255) NULL,
                            [Debit] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [Credit] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [Balance] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
                            CONSTRAINT [PK_CustomerLedger] PRIMARY KEY CLUSTERED ([LedgerEntryID] ASC)
                        );
                        
                        -- Add Foreign Key if Customers table exists
                        IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers')
                        BEGIN
                            ALTER TABLE [dbo].[CustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_CustomerLedger_Customers] FOREIGN KEY([CustomerID])
                            REFERENCES [dbo].[Customers] ([CustomerID])
                            ON DELETE CASCADE;

                            ALTER TABLE [dbo].[CustomerLedger] CHECK CONSTRAINT [FK_CustomerLedger_Customers];
                        END
                    ";

                    using (var createCommand = new SqlCommand(createTableQuery, connection))
                    {
                        createCommand.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("Created table: CustomerLedger");
                    }
                }
            }
        }
        private static void EnsureStoreSettingsTable(SqlConnection connection)
        {
            string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'StoreSettings'";
            
            using (var command = new SqlCommand(checkTableQuery, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    string createTableQuery = @"
                        CREATE TABLE [dbo].[StoreSettings](
                            [SettingID] [int] IDENTITY(1,1) NOT NULL,
                            [StoreName] [nvarchar](100) NOT NULL,
                            [StoreContact] [nvarchar](100) NULL,
                            [StoreAddress] [nvarchar](255) NULL,
                            [StoreEmail] [nvarchar](100) NULL,
                            [ReceiptFooter] [nvarchar](MAX) NULL,
                            [UpdatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
                            [BarcodeDefaultLabel] [nvarchar](100) NULL,
                            [BarcodeWidth] [int] NOT NULL DEFAULT 130,
                            [BarcodeHeight] [int] NOT NULL DEFAULT 90,
                            [BarcodeGap] [decimal](18, 2) NOT NULL DEFAULT 3,
                            [BarcodeMarginLeft] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [BarcodeMarginRight] [decimal](18, 2) NOT NULL DEFAULT 12,
                            [BarcodeMarginTop] [decimal](18, 2) NOT NULL DEFAULT 4,
                            [BarcodeMarginBottom] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [BarcodeIsThermal] [bit] NOT NULL DEFAULT 1,
                            [ThermalPaperWidth] [int] NOT NULL DEFAULT 300,
                            CONSTRAINT [PK_StoreSettings] PRIMARY KEY CLUSTERED ([SettingID] ASC)
                        );
                        
                        INSERT INTO [dbo].[StoreSettings] (StoreName, StoreContact, StoreAddress, ReceiptFooter, BarcodeDefaultLabel)
                        VALUES ('MADNI MOBILE', '0345:5518744', 'Shop#3, opp Save Mart, main Tulsa road, lalazar,Rwp', '- GOODS PURCHASED ARE NOT RETURNABLE\n- GOODS ONCE PURCHASED ARE ONLY EXCHANGEABLE NOT RETURNABLE\n- MADNI MOBILE SHOP IS NOT RESPONSIBLE FOR ANY WARRANTY CLAIMS', 'MADNI MOBILE');
                    ";

                    using (var createCommand = new SqlCommand(createTableQuery, connection))
                    {
                        createCommand.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("Created table: StoreSettings with printing defaults");
                    }
                }
                else
                {
                    // Migration: Add missing columns if table already exists
                    string migrationQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeDefaultLabel')
                            ALTER TABLE StoreSettings ADD BarcodeDefaultLabel [nvarchar](100) NULL;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeWidth')
                            ALTER TABLE StoreSettings ADD BarcodeWidth [int] NOT NULL DEFAULT 130;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeHeight')
                            ALTER TABLE StoreSettings ADD BarcodeHeight [int] NOT NULL DEFAULT 90;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeGap')
                            ALTER TABLE StoreSettings ADD BarcodeGap [decimal](18, 2) NOT NULL DEFAULT 3;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeMarginLeft')
                            ALTER TABLE StoreSettings ADD BarcodeMarginLeft [decimal](18, 2) NOT NULL DEFAULT 0;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeMarginRight')
                            ALTER TABLE StoreSettings ADD BarcodeMarginRight [decimal](18, 2) NOT NULL DEFAULT 12;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeMarginTop')
                            ALTER TABLE StoreSettings ADD BarcodeMarginTop [decimal](18, 2) NOT NULL DEFAULT 4;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeMarginBottom')
                            ALTER TABLE StoreSettings ADD BarcodeMarginBottom [decimal](18, 2) NOT NULL DEFAULT 0;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodeIsThermal')
                            ALTER TABLE StoreSettings ADD BarcodeIsThermal [bit] NOT NULL DEFAULT 1;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'ThermalPaperWidth')
                            ALTER TABLE StoreSettings ADD ThermalPaperWidth [int] NOT NULL DEFAULT 300;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'ThermalPrinterName')
                            ALTER TABLE StoreSettings ADD ThermalPrinterName [nvarchar](255) NULL;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'BarcodePrinterName')
                            ALTER TABLE StoreSettings ADD BarcodePrinterName [nvarchar](255) NULL;
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StoreSettings' AND COLUMN_NAME = 'DirectPrintReceipt')
                            ALTER TABLE StoreSettings ADD DirectPrintReceipt [bit] NOT NULL DEFAULT 0;
                    ";
                    using (var migrateCommand = new SqlCommand(migrationQuery, connection))
                    {
                        migrateCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private class ProductStockInfo
        {
            public int ProductID { get; set; }
            public int StockQuantity { get; set; }
            public decimal CostPrice { get; set; }
            public decimal RetailPrice { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
        }

        private static void HealDatabaseStockBatches(SqlConnection connection)
        {
            try
            {
                // 1. Get all products
                string getProductsQuery = @"
                    SELECT ProductID, StockQuantity, CostPrice, RetailPrice, ProductName, ProductCode 
                    FROM Products 
                    WHERE StockQuantity > 0";

                var products = new System.Collections.Generic.List<ProductStockInfo>();
                using (var cmd = new SqlCommand(getProductsQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductStockInfo
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                                CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                                RetailPrice = Convert.ToDecimal(reader["RetailPrice"]),
                                ProductName = reader["ProductName"]?.ToString() ?? "",
                                ProductCode = reader["ProductCode"]?.ToString() ?? ""
                            });
                        }
                    }
                }

                foreach (var product in products)
                {
                    int productId = product.ProductID;
                    int stockQty = product.StockQuantity;

                    // Sum remaining batch quantities
                    int sumRemaining = 0;
                    string sumQuery = "SELECT ISNULL(SUM(RemainingQuantity), 0) FROM PurchaseItems WHERE ProductID = @ProductID";
                    using (var cmd = new SqlCommand(sumQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        sumRemaining = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (stockQty > sumRemaining)
                    {
                        int diff = stockQty - sumRemaining;

                        // Check if any purchase item exists
                        int latestPurchaseItemId = 0;
                        string checkQuery = "SELECT TOP 1 PurchaseItemID FROM PurchaseItems WHERE ProductID = @ProductID ORDER BY PurchaseItemID DESC";
                        using (var cmd = new SqlCommand(checkQuery, connection))
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
                            // Reconcile by adding the missing quantity to the latest purchase item
                            string updateQuery = "UPDATE PurchaseItems SET RemainingQuantity = RemainingQuantity + @Diff WHERE PurchaseItemID = @PurchaseItemID";
                            using (var cmd = new SqlCommand(updateQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@Diff", diff);
                                cmd.Parameters.AddWithValue("@PurchaseItemID", latestPurchaseItemId);
                                cmd.ExecuteNonQuery();
                            }
                            System.Diagnostics.Debug.WriteLine($"Healed product {product.ProductName}: Added {diff} to existing purchase item {latestPurchaseItemId}.");
                        }
                        else
                        {
                            // No purchase items exist. Create restoration batch.
                            int purchaseId = 0;
                            string purchaseCheck = "SELECT TOP 1 PurchaseID FROM Purchases WHERE InvoiceNumber = 'SYS-AUTO-RESTORE'";
                            using (var cmd = new SqlCommand(purchaseCheck, connection))
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
                                using (var cmd = new SqlCommand(supCheck, connection))
                                {
                                    var res = cmd.ExecuteScalar();
                                    if (res != null && res != DBNull.Value) supplierId = Convert.ToInt32(res);
                                }

                                int userId = 1;
                                string userCheck = "SELECT TOP 1 UserID FROM Users ORDER BY UserID ASC";
                                using (var cmd = new SqlCommand(userCheck, connection))
                                {
                                    var res = cmd.ExecuteScalar();
                                    if (res != null && res != DBNull.Value) userId = Convert.ToInt32(res);
                                }

                                string createPurchase = @"
                                    INSERT INTO Purchases (InvoiceNumber, SupplierID, PurchaseDate, SubTotal, TaxAmount, TaxPercent, TotalAmount, CreatedDate, UserID)
                                    VALUES ('SYS-AUTO-RESTORE', @SupplierID, GETDATE(), 0, 0, 0, 0, GETDATE(), @UserID);
                                    SELECT SCOPE_IDENTITY();";
                                
                                using (var cmd = new SqlCommand(createPurchase, connection))
                                {
                                    cmd.Parameters.AddWithValue("@SupplierID", supplierId > 0 ? (object)supplierId : DBNull.Value);
                                    cmd.Parameters.AddWithValue("@UserID", userId);
                                    purchaseId = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                            }

                            string insertItem = @"
                                INSERT INTO PurchaseItems (PurchaseID, ProductID, ProductName, ProductCode, Quantity, RemainingQuantity, UnitPrice, SellingPrice, SubTotal, Bonus, ExpiryDate, TaxPercent, DiscountAmount)
                                VALUES (@PurchaseID, @ProductID, @ProductName, @ProductCode, @Qty, @Qty, @CostPrice, @RetailPrice, 0, 0, GETDATE(), 0, 0)";
                            
                            using (var cmd = new SqlCommand(insertItem, connection))
                            {
                                cmd.Parameters.AddWithValue("@PurchaseID", purchaseId);
                                cmd.Parameters.AddWithValue("@ProductID", productId);
                                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                                cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                                cmd.Parameters.AddWithValue("@Qty", diff);
                                cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                                cmd.Parameters.AddWithValue("@RetailPrice", product.RetailPrice);
                                cmd.ExecuteNonQuery();
                            }
                            System.Diagnostics.Debug.WriteLine($"Healed product {product.ProductName}: Created restoration purchase item of quantity {diff}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to run database stock self-healing: {ex.Message}");
            }
        }
    }
}
