-- Fix CashInHand table to match code expectations
USE VapeStore;
GO

-- Drop existing CashInHand table
DROP TABLE IF EXISTS CashInHand;

-- Create new CashInHand table with correct structure
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

PRINT 'CashInHand table recreated successfully!';
GO
