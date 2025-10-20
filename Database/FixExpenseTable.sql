-- Fix Expense Table Issues
-- This script fixes the mismatches between code and database

USE VapeStore;
GO

-- Add missing columns to ExpenseEntries table
ALTER TABLE ExpenseEntries 
ADD ExpenseCode NVARCHAR(20) NULL,
    ReferenceNumber NVARCHAR(50) NULL,
    Remarks NVARCHAR(255) NULL,
    Status NVARCHAR(20) DEFAULT 'Draft',
    LastModifiedDate DATETIME NULL;

-- Create index on ExpenseCode for better performance
CREATE INDEX IX_ExpenseEntries_ExpenseCode ON ExpenseEntries(ExpenseCode);

-- Update existing records to have default values
UPDATE ExpenseEntries 
SET Status = 'Draft',
    ExpenseCode = 'EXP' + RIGHT('00000' + CAST(ExpenseID AS VARCHAR), 5)
WHERE ExpenseCode IS NULL;

-- Make ExpenseCode NOT NULL after updating existing records
ALTER TABLE ExpenseEntries 
ALTER COLUMN ExpenseCode NVARCHAR(20) NOT NULL;

-- Create unique constraint on ExpenseCode
ALTER TABLE ExpenseEntries 
ADD CONSTRAINT UQ_ExpenseEntries_ExpenseCode UNIQUE (ExpenseCode);

PRINT 'ExpenseEntries table updated successfully!';
GO
