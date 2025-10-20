-- Fix CustomerPayments table to match code expectations
USE VapeStore;
GO

-- Add missing columns to CustomerPayments
ALTER TABLE CustomerPayments ADD VoucherNumber NVARCHAR(20) NULL;
ALTER TABLE CustomerPayments ADD PreviousBalance DECIMAL(10,2) DEFAULT 0;
ALTER TABLE CustomerPayments ADD TotalDue DECIMAL(10,2) DEFAULT 0;
ALTER TABLE CustomerPayments ADD PaidAmount DECIMAL(10,2) DEFAULT 0;
ALTER TABLE CustomerPayments ADD RemainingBalance DECIMAL(10,2) DEFAULT 0;

-- Rename Amount column to match the model
EXEC sp_rename 'CustomerPayments.Amount', 'Amount_Old', 'COLUMN';
ALTER TABLE CustomerPayments ADD Amount DECIMAL(10,2) DEFAULT 0;

PRINT 'CustomerPayments table updated successfully!';
GO
