-- Vape Store Sample Data Insertion Script
-- Run this after creating the database and tables

USE VapeStore;
GO

-- Insert Sample Users
INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, IsActive)
VALUES 
('admin', 'admin123', 'Administrator', 'admin@vapestore.com', '1234567890', 'Admin', 1),
('cashier', 'cashier123', 'Cashier User', 'cashier@vapestore.com', '0987654321', 'Cashier', 1),
('manager', 'manager123', 'Manager User', 'manager@vapestore.com', '1122334455', 'Manager', 1);
GO

-- Insert Sample Categories
INSERT INTO Categories (CategoryName, Description, IsActive)
VALUES 
('E-Liquids', 'Electronic cigarette liquids and vape juices', 1),
('Vape Devices', 'Electronic cigarettes and vaping devices', 1),
('Accessories', 'Vape accessories and spare parts', 1),
('Coils', 'Vape coils and heating elements', 1),
('Batteries', 'Vape batteries and chargers', 1);
GO

-- Insert Sample Brands
INSERT INTO Brands (BrandName, Description, IsActive)
VALUES 
('SMOK', 'Premium vaping devices and accessories', 1),
('Vaporesso', 'Innovative vaping technology', 1),
('GeekVape', 'Durable and reliable vaping products', 1),
('Uwell', 'High-quality vaping devices', 1),
('Aspire', 'Advanced vaping solutions', 1);
GO

-- Insert Sample Suppliers
INSERT INTO Suppliers (SupplierCode, SupplierName, ContactPerson, Phone, Email, Address, City, PostalCode, IsActive)
VALUES 
('SUP001', 'Vape Supply Co.', 'John Smith', '555-0101', 'john@vapesupply.com', '123 Business St', 'New York', '10001', 1),
('SUP002', 'Cloud Distributors', 'Jane Doe', '555-0102', 'jane@clouddist.com', '456 Commerce Ave', 'Los Angeles', '90001', 1),
('SUP003', 'Vapor Wholesale', 'Mike Johnson', '555-0103', 'mike@vaporwholesale.com', '789 Trade Blvd', 'Chicago', '60601', 1);
GO

-- Insert Sample Customers
INSERT INTO Customers (CustomerCode, CustomerName, Phone, Email, Address, City, PostalCode, IsActive)
VALUES 
('CUST001', 'Alice Brown', '555-1001', 'alice@email.com', '123 Main St', 'New York', '10001', 1),
('CUST002', 'Bob Wilson', '555-1002', 'bob@email.com', '456 Oak Ave', 'Los Angeles', '90001', 1),
('CUST003', 'Carol Davis', '555-1003', 'carol@email.com', '789 Pine St', 'Chicago', '60601', 1),
('CUST004', 'David Miller', '555-1004', 'david@email.com', '321 Elm St', 'Houston', '77001', 1),
('CUST005', 'Eva Garcia', '555-1005', 'eva@email.com', '654 Maple Ave', 'Phoenix', '85001', 1);
GO

-- Insert Sample Products
INSERT INTO Products (ProductCode, ProductName, Description, CategoryID, BrandID, PurchasePrice, RetailPrice, StockQuantity, ReorderLevel, Barcode, IsActive)
VALUES 
('PROD001', 'SMOK Nord 4 Kit', 'SMOK Nord 4 Pod System Kit', 2, 1, 25.00, 45.00, 50, 10, '1234567890123', 1),
('PROD002', 'Vaporesso Luxe PM40', 'Vaporesso Luxe PM40 Pod Kit', 2, 2, 30.00, 55.00, 30, 5, '1234567890124', 1),
('PROD003', 'Strawberry E-Liquid 30ml', 'Premium strawberry flavored e-liquid', 1, 3, 8.00, 15.00, 100, 20, '1234567890125', 1),
('PROD004', 'Mint E-Liquid 30ml', 'Refreshing mint flavored e-liquid', 1, 3, 8.00, 15.00, 80, 15, '1234567890126', 1),
('PROD005', 'GeekVape Aegis X', 'GeekVape Aegis X 200W Mod', 2, 3, 60.00, 95.00, 20, 5, '1234567890127', 1),
('PROD006', 'Uwell Caliburn G', 'Uwell Caliburn G Pod System', 2, 4, 20.00, 35.00, 40, 8, '1234567890128', 1),
('PROD007', 'Aspire Nautilus Coils', 'Aspire Nautilus Replacement Coils (5-pack)', 4, 5, 12.00, 20.00, 200, 50, '1234567890129', 1),
('PROD008', 'Vape Charging Cable', 'USB-C charging cable for vape devices', 5, 2, 3.00, 8.00, 150, 30, '1234567890130', 1);
GO

-- Insert Sample Sales
INSERT INTO Sales (InvoiceNumber, CustomerID, SaleDate, SubTotal, TaxAmount, TaxPercent, TotalAmount, PaymentMethod, PaidAmount, ChangeAmount, UserID)
VALUES 
('INV001', 1, '2024-01-15 10:30:00', 60.00, 6.00, 10.00, 66.00, 'Cash', 70.00, 4.00, 1),
('INV002', 2, '2024-01-15 11:15:00', 50.00, 5.00, 10.00, 55.00, 'Card', 55.00, 0.00, 2),
('INV003', 3, '2024-01-15 14:20:00', 75.00, 7.50, 10.00, 82.50, 'Cash', 85.00, 2.50, 1);
GO

-- Insert Sample Sale Items
INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice, SubTotal)
VALUES 
(1, 1, 1, 45.00, 45.00),
(1, 3, 1, 15.00, 15.00),
(2, 2, 1, 55.00, 55.00),
(3, 5, 1, 95.00, 75.00),
(3, 4, 1, 15.00, 15.00);
GO

-- Insert Sample Purchases
INSERT INTO Purchases (InvoiceNumber, SupplierID, PurchaseDate, SubTotal, TaxAmount, TaxPercent, TotalAmount, PaymentMethod, PaidAmount, ChangeAmount, UserID)
VALUES 
('PUR001', 1, '2024-01-10 09:00:00', 500.00, 50.00, 10.00, 550.00, 'Bank Transfer', 550.00, 0.00, 1),
('PUR002', 2, '2024-01-12 14:30:00', 300.00, 30.00, 10.00, 330.00, 'Check', 330.00, 0.00, 1);
GO

-- Insert Sample Purchase Items
INSERT INTO PurchaseItems (PurchaseID, ProductID, Quantity, UnitPrice, SubTotal)
VALUES 
(1, 1, 20, 25.00, 500.00),
(2, 2, 10, 30.00, 300.00);
GO

-- Insert Sample Expense Categories
INSERT INTO ExpenseCategories (CategoryName, Description, IsActive)
VALUES 
('Rent', 'Monthly rent for store location', 1),
('Utilities', 'Electricity, water, internet bills', 1),
('Marketing', 'Advertising and promotional expenses', 1),
('Office Supplies', 'Stationery and office materials', 1),
('Maintenance', 'Equipment and store maintenance', 1);
GO

-- Insert Sample Expense Entries
INSERT INTO ExpenseEntries (ExpenseDate, CategoryID, Amount, Description, PaymentMethod, UserID)
VALUES 
('2024-01-01', 1, 2000.00, 'Monthly rent payment', 'Bank Transfer', 1),
('2024-01-05', 2, 150.00, 'Electricity bill', 'Online Payment', 1),
('2024-01-10', 3, 300.00, 'Social media advertising', 'Card', 1);
GO

-- Insert Sample Cash in Hand Transactions
INSERT INTO CashInHand (TransactionDate, TransactionType, Amount, Description, UserID)
VALUES 
('2024-01-15', 'Income', 66.00, 'Sale - Invoice INV001', 1),
('2024-01-15', 'Income', 55.00, 'Sale - Invoice INV002', 2),
('2024-01-15', 'Income', 82.50, 'Sale - Invoice INV003', 1),
('2024-01-15', 'Expense', 50.00, 'Daily expenses', 1);
GO

PRINT 'Sample data inserted successfully!';
GO
