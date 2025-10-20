-- Fix All Database Schema Issues
-- This script addresses all identified schema mismatches

USE VapeStore;
GO

-- 1. Add CostPrice column to Products table (if not exists)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'CostPrice')
BEGIN
    ALTER TABLE Products ADD CostPrice decimal(18,2) NOT NULL DEFAULT 0;
    PRINT 'Added CostPrice column to Products table';
END
GO

-- 2. Update existing records to set CostPrice = PurchasePrice
UPDATE Products SET CostPrice = PurchasePrice WHERE CostPrice = 0;
PRINT 'Updated CostPrice values in Products table';
GO

-- 3. Check if all required tables exist and have correct structure
-- Verify Products table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
BEGIN
    PRINT 'Products table exists';
    
    -- Check for required columns
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'ProductCode')
    BEGIN
        PRINT 'ERROR: ProductCode column missing in Products table';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'CostPrice')
    BEGIN
        PRINT 'ERROR: CostPrice column missing in Products table';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: Products table does not exist';
END
GO

-- 4. Verify Sales table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Sales')
BEGIN
    PRINT 'Sales table exists';
    
    -- Check for required columns
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'InvoiceNumber')
    BEGIN
        PRINT 'ERROR: InvoiceNumber column missing in Sales table';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: Sales table does not exist';
END
GO

-- 5. Verify SaleItems table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SaleItems')
BEGIN
    PRINT 'SaleItems table exists';
    
    -- Check for required columns
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'SaleItems' AND COLUMN_NAME = 'ProductID')
    BEGIN
        PRINT 'ERROR: ProductID column missing in SaleItems table';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: SaleItems table does not exist';
END
GO

-- 6. Verify Purchases table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Purchases')
BEGIN
    PRINT 'Purchases table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: Purchases table does not exist';
END
GO

-- 7. Verify PurchaseItems table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PurchaseItems')
BEGIN
    PRINT 'PurchaseItems table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: PurchaseItems table does not exist';
END
GO

-- 8. Verify SalesReturns table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SalesReturns')
BEGIN
    PRINT 'SalesReturns table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: SalesReturns table does not exist';
END
GO

-- 9. Verify SalesReturnItems table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SalesReturnItems')
BEGIN
    PRINT 'SalesReturnItems table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: SalesReturnItems table does not exist';
END
GO

-- 10. Verify PurchaseReturns table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PurchaseReturns')
BEGIN
    PRINT 'PurchaseReturns table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: PurchaseReturns table does not exist';
END
GO

-- 11. Verify PurchaseReturnItems table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PurchaseReturnItems')
BEGIN
    PRINT 'PurchaseReturnItems table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: PurchaseReturnItems table does not exist';
END
GO

-- 12. Verify CustomerPayments table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CustomerPayments')
BEGIN
    PRINT 'CustomerPayments table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: CustomerPayments table does not exist';
END
GO

-- 13. Verify SupplierPayments table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SupplierPayments')
BEGIN
    PRINT 'SupplierPayments table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: SupplierPayments table does not exist';
END
GO

-- 14. Verify ExpenseEntries table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ExpenseEntries')
BEGIN
    PRINT 'ExpenseEntries table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: ExpenseEntries table does not exist';
END
GO

-- 15. Verify ExpenseCategories table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ExpenseCategories')
BEGIN
    PRINT 'ExpenseCategories table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: ExpenseCategories table does not exist';
END
GO

-- 16. Verify CashInHand table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CashInHand')
BEGIN
    PRINT 'CashInHand table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: CashInHand table does not exist';
END
GO

-- 17. Verify Customers table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers')
BEGIN
    PRINT 'Customers table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: Customers table does not exist';
END
GO

-- 18. Verify Suppliers table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Suppliers')
BEGIN
    PRINT 'Suppliers table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: Suppliers table does not exist';
END
GO

-- 19. Verify Categories table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
BEGIN
    PRINT 'Categories table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: Categories table does not exist';
END
GO

-- 20. Verify Brands table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Brands')
BEGIN
    PRINT 'Brands table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: Brands table does not exist';
END
GO

-- 21. Verify Users table structure
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    PRINT 'Users table exists';
END
ELSE
BEGIN
    PRINT 'ERROR: Users table does not exist';
END
GO

PRINT 'Database schema verification completed!';
GO
