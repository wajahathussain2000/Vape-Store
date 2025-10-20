-- Fix ExpenseEntries table to match code expectations
USE VapeStore;
GO

-- Add missing columns to ExpenseEntries
ALTER TABLE ExpenseEntries ADD ExpenseCode NVARCHAR(20) NULL;
ALTER TABLE ExpenseEntries ADD ReferenceNumber NVARCHAR(50) NULL;
ALTER TABLE ExpenseEntries ADD Remarks NVARCHAR(255) NULL;
ALTER TABLE ExpenseEntries ADD Status NVARCHAR(20) DEFAULT 'Draft';
ALTER TABLE ExpenseEntries ADD LastModifiedDate DATETIME NULL;

PRINT 'ExpenseEntries table updated successfully!';
GO
