-- Simple Database Fix Script
USE VapeStore;
GO

-- 1. Fix ExpenseEntries table
PRINT 'Fixing ExpenseEntries table...';

-- Add missing columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseEntries') AND name = 'ExpenseCode')
BEGIN
    ALTER TABLE ExpenseEntries ADD ExpenseCode NVARCHAR(20) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseEntries') AND name = 'ReferenceNumber')
BEGIN
    ALTER TABLE ExpenseEntries ADD ReferenceNumber NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseEntries') AND name = 'Remarks')
BEGIN
    ALTER TABLE ExpenseEntries ADD Remarks NVARCHAR(255) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseEntries') AND name = 'Status')
BEGIN
    ALTER TABLE ExpenseEntries ADD Status NVARCHAR(20) DEFAULT 'Draft';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseEntries') AND name = 'LastModifiedDate')
BEGIN
    ALTER TABLE ExpenseEntries ADD LastModifiedDate DATETIME NULL;
END

-- Update existing records
UPDATE ExpenseEntries 
SET Status = 'Draft',
    ExpenseCode = 'EXP' + RIGHT('00000' + CAST(ExpenseID AS VARCHAR), 5)
WHERE ExpenseCode IS NULL;

-- 2. Fix ExpenseCategories table
PRINT 'Fixing ExpenseCategories table...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseCategories') AND name = 'CategoryCode')
BEGIN
    ALTER TABLE ExpenseCategories ADD CategoryCode NVARCHAR(20) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseCategories') AND name = 'UserID')
BEGIN
    ALTER TABLE ExpenseCategories ADD UserID INT NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExpenseCategories') AND name = 'LastModifiedDate')
BEGIN
    ALTER TABLE ExpenseCategories ADD LastModifiedDate DATETIME NULL;
END

-- Update existing records
UPDATE ExpenseCategories 
SET CategoryCode = 'EC' + RIGHT('00000' + CAST(CategoryID AS VARCHAR), 5),
    UserID = 1
WHERE CategoryCode IS NULL;

-- 3. Fix CashInHand table
PRINT 'Fixing CashInHand table...';

-- Drop and recreate CashInHand table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CashInHand')
BEGIN
    DROP TABLE CashInHand;
END

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
    UserID INT,
    CreatedDate DATETIME DEFAULT GETDATE()
);

PRINT 'Database fixes completed successfully!';
GO
