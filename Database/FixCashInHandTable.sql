-- Fix CashInHand Table Issues
-- This script fixes the mismatches between code and database

USE VapeStore;
GO

-- Drop the existing CashInHand table and recreate it with the correct structure
DROP TABLE IF EXISTS CashInHand;
GO

-- Create CashInHand table with the structure expected by the code
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
GO

-- Create index for better performance
CREATE INDEX IX_CashInHand_TransactionDate ON CashInHand(TransactionDate);
CREATE INDEX IX_CashInHand_UserID ON CashInHand(UserID);

PRINT 'CashInHand table recreated successfully!';
GO
