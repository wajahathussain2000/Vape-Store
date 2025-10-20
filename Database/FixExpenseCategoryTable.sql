-- Fix ExpenseCategory Table Issues
-- This script fixes the mismatches between code and database

USE VapeStore;
GO

-- Add missing columns to ExpenseCategories table
ALTER TABLE ExpenseCategories 
ADD CategoryCode NVARCHAR(20) NULL,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    LastModifiedDate DATETIME NULL;

-- Create index on CategoryCode for better performance
CREATE INDEX IX_ExpenseCategories_CategoryCode ON ExpenseCategories(CategoryCode);

-- Update existing records to have default values
UPDATE ExpenseCategories 
SET CategoryCode = 'EC' + RIGHT('00000' + CAST(CategoryID AS VARCHAR), 5),
    UserID = 1  -- Default to first user
WHERE CategoryCode IS NULL;

-- Make CategoryCode NOT NULL after updating existing records
ALTER TABLE ExpenseCategories 
ALTER COLUMN CategoryCode NVARCHAR(20) NOT NULL;

-- Create unique constraint on CategoryCode
ALTER TABLE ExpenseCategories 
ADD CONSTRAINT UQ_ExpenseCategories_CategoryCode UNIQUE (CategoryCode);

PRINT 'ExpenseCategories table updated successfully!';
GO
