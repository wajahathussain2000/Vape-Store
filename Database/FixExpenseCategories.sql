-- Fix ExpenseCategories table to match code expectations
USE VapeStore;
GO

-- Add missing columns to ExpenseCategories
ALTER TABLE ExpenseCategories ADD CategoryCode NVARCHAR(20) NULL;
ALTER TABLE ExpenseCategories ADD UserID INT NULL;
ALTER TABLE ExpenseCategories ADD LastModifiedDate DATETIME NULL;

PRINT 'ExpenseCategories table updated successfully!';
GO
