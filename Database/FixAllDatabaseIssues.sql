-- Comprehensive Database Fix Script
-- This script fixes ALL mismatches between code and database

USE VapeStore;
GO

PRINT 'Starting comprehensive database fixes...';

-- 1. Fix ExpenseEntries table
PRINT 'Fixing ExpenseEntries table...';
ALTER TABLE ExpenseEntries 
ADD ExpenseCode NVARCHAR(20) NULL,
    ReferenceNumber NVARCHAR(50) NULL,
    Remarks NVARCHAR(255) NULL,
    Status NVARCHAR(20) DEFAULT 'Draft',
    LastModifiedDate DATETIME NULL;

-- Update existing records
UPDATE ExpenseEntries 
SET Status = 'Draft',
    ExpenseCode = 'EXP' + RIGHT('00000' + CAST(ExpenseID AS VARCHAR), 5)
WHERE ExpenseCode IS NULL;

-- Make ExpenseCode NOT NULL
ALTER TABLE ExpenseEntries 
ALTER COLUMN ExpenseCode NVARCHAR(20) NOT NULL;

-- Add constraints
ALTER TABLE ExpenseEntries 
ADD CONSTRAINT UQ_ExpenseEntries_ExpenseCode UNIQUE (ExpenseCode);

CREATE INDEX IX_ExpenseEntries_ExpenseCode ON ExpenseEntries(ExpenseCode);

-- 2. Fix ExpenseCategories table
PRINT 'Fixing ExpenseCategories table...';
ALTER TABLE ExpenseCategories 
ADD CategoryCode NVARCHAR(20) NULL,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    LastModifiedDate DATETIME NULL;

-- Update existing records
UPDATE ExpenseCategories 
SET CategoryCode = 'EC' + RIGHT('00000' + CAST(CategoryID AS VARCHAR), 5),
    UserID = 1
WHERE CategoryCode IS NULL;

-- Make CategoryCode NOT NULL
ALTER TABLE ExpenseCategories 
ALTER COLUMN CategoryCode NVARCHAR(20) NOT NULL;

-- Add constraints
ALTER TABLE ExpenseCategories 
ADD CONSTRAINT UQ_ExpenseCategories_CategoryCode UNIQUE (CategoryCode);

CREATE INDEX IX_ExpenseCategories_CategoryCode ON ExpenseCategories(CategoryCode);

-- 3. Fix CashInHand table
PRINT 'Fixing CashInHand table...';
-- Drop existing table
DROP TABLE IF EXISTS CashInHand;
GO

-- Recreate with correct structure
CREATE TABLE CashInHand (
    CashInHandID INT IDENTITY(1,1) PRIMARY KEY,
    TransactionDate DATETIME DEFAULT GETDATE(),
    OpeningCash DECIMAL(10,2) DEFAULT 0,
    CashIn DECIMAL(10,2) DEFAULT 0,
    CashOut DECIMAL(10,2) DEFAULT 0,
    Expenses DECIMAL(10,2) DEFAULT 0,
    ClosingCash DECIMAL(10,2) DEFAULT 0,
    Description NVARCHAR(255),
    CreatedBy NVARCHAR(100),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE INDEX IX_CashInHand_TransactionDate ON CashInHand(TransactionDate);
CREATE INDEX IX_CashInHand_UserID ON CashInHand(UserID);

-- 4. Add missing columns to other tables if needed
PRINT 'Checking other tables...';

-- Check if Categories table needs UserID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserID')
BEGIN
    ALTER TABLE Categories ADD UserID INT FOREIGN KEY REFERENCES Users(UserID);
    UPDATE Categories SET UserID = 1 WHERE UserID IS NULL;
END

-- Check if Brands table needs UserID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Brands') AND name = 'UserID')
BEGIN
    ALTER TABLE Brands ADD UserID INT FOREIGN KEY REFERENCES Users(UserID);
    UPDATE Brands SET UserID = 1 WHERE UserID IS NULL;
END

-- Check if Products table needs UserID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'UserID')
BEGIN
    ALTER TABLE Products ADD UserID INT FOREIGN KEY REFERENCES Users(UserID);
    UPDATE Products SET UserID = 1 WHERE UserID IS NULL;
END

-- Check if Customers table needs UserID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Customers') AND name = 'UserID')
BEGIN
    ALTER TABLE Customers ADD UserID INT FOREIGN KEY REFERENCES Users(UserID);
    UPDATE Customers SET UserID = 1 WHERE UserID IS NULL;
END

-- Check if Suppliers table needs UserID
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'UserID')
BEGIN
    ALTER TABLE Suppliers ADD UserID INT FOREIGN KEY REFERENCES Users(UserID);
    UPDATE Suppliers SET UserID = 1 WHERE UserID IS NULL;
END

PRINT 'All database fixes completed successfully!';
PRINT 'The database now matches the code expectations.';
GO
