-- Test Connection and Data Verification
USE VapeStore;
GO

-- Test 1: Check all tables exist
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO

-- Test 2: Check sample data
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Brands', COUNT(*) FROM Brands
UNION ALL
SELECT 'Suppliers', COUNT(*) FROM Suppliers
UNION ALL
SELECT 'Customers', COUNT(*) FROM Customers
UNION ALL
SELECT 'Products', COUNT(*) FROM Products
UNION ALL
SELECT 'Sales', COUNT(*) FROM Sales
UNION ALL
SELECT 'Purchases', COUNT(*) FROM Purchases
UNION ALL
SELECT 'ExpenseCategories', COUNT(*) FROM ExpenseCategories
UNION ALL
SELECT 'CashInHand', COUNT(*) FROM CashInHand;
GO

-- Test 3: Check login credentials
SELECT Username, FullName, Role, IsActive 
FROM Users 
WHERE IsActive = 1;
GO

-- Test 4: Check products with stock
SELECT 
    p.ProductCode,
    p.ProductName,
    p.StockQuantity,
    c.CategoryName,
    b.BrandName
FROM Products p
LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
LEFT JOIN Brands b ON p.BrandID = b.BrandID
WHERE p.IsActive = 1
ORDER BY p.ProductName;
GO

PRINT 'Database connection and data verification completed successfully!';
GO
