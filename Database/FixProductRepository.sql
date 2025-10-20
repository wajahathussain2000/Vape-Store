-- Fix ProductRepository CostPrice Issues
-- This script ensures the database schema supports the CostPrice column properly

USE VapeStore;
GO

-- Verify CostPrice column exists and has data
SELECT 
    ProductID, 
    ProductName, 
    PurchasePrice, 
    CostPrice, 
    RetailPrice 
FROM Products 
WHERE ProductID <= 3;
GO

-- Update any remaining NULL or 0 CostPrice values
UPDATE Products 
SET CostPrice = PurchasePrice 
WHERE CostPrice IS NULL OR CostPrice = 0;
GO

-- Verify the update
SELECT 
    COUNT(*) as TotalProducts,
    COUNT(CASE WHEN CostPrice > 0 THEN 1 END) as ProductsWithCostPrice,
    COUNT(CASE WHEN CostPrice = PurchasePrice THEN 1 END) as ProductsWithMatchingCostPrice
FROM Products;
GO

PRINT 'ProductRepository CostPrice issues resolved!';
GO
