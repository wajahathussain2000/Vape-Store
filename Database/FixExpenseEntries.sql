-- Fix ExpenseEntries table to match code expectations
USE VapeStore;
GO

-- Add missing columns to ExpenseEntries
ALTER TABLE ExpenseEntries ADD ExpenseCode NVARCHAR(20) NULL;
ALTER TABLE ExpenseEntries ADD ReferenceNumber NVARCHAR(50) NULL;
ALTER TABLE ExpenseEntries ADD Remarks NVARCHAR(255) NULL;
ALTER TABLE ExpenseEntries ADD Status NVARCHAR(20) DEFAULT 'Draft';
ALTER TABLE ExpenseEntries ADD LastModifiedDate DATETIME NULL;

-- Update existing records with default values
UPDATE ExpenseEntries 
SET Status = 'Draft',
    ExpenseCode = 'EXP' + RIGHT('00000' + CAST(ExpenseID AS VARCHAR), 5)
WHERE ExpenseCode IS NULL;

-- Make ExpenseCode NOT NULL after updating
ALTER TABLE ExpenseEntries ALTER COLUMN ExpenseCode NVARCHAR(20) NOT NULL;

-- Add unique constraint
ALTER TABLE ExpenseEntries ADD CONSTRAINT UQ_ExpenseEntries_ExpenseCode UNIQUE (ExpenseCode);

PRINT 'ExpenseEntries table updated successfully!';
GO
