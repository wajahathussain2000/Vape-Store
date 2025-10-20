-- Fix SupplierPayments table to match code expectations
USE VapeStore;
GO

-- Add missing columns to SupplierPayments
ALTER TABLE SupplierPayments ADD VoucherNumber NVARCHAR(20) NULL;
ALTER TABLE SupplierPayments ADD PreviousBalance DECIMAL(10,2) DEFAULT 0;
ALTER TABLE SupplierPayments ADD TotalPayable DECIMAL(10,2) DEFAULT 0;
ALTER TABLE SupplierPayments ADD PaidAmount DECIMAL(10,2) DEFAULT 0;
ALTER TABLE SupplierPayments ADD RemainingAmount DECIMAL(10,2) DEFAULT 0;
ALTER TABLE SupplierPayments ADD RemainingBalance DECIMAL(10,2) DEFAULT 0;

PRINT 'SupplierPayments table updated successfully!';
GO
