-- FIFO Inventory Update: Adding CostPrice and PurchaseItemID to SaleItems
-- Target: SaleItems table

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SaleItems') AND name = 'CostPrice')
BEGIN
    ALTER TABLE SaleItems ADD CostPrice DECIMAL(18, 2) NOT NULL DEFAULT 0;
    PRINT 'CostPrice column added to SaleItems.';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SaleItems') AND name = 'PurchaseItemID')
BEGIN
    ALTER TABLE SaleItems ADD PurchaseItemID INT NULL;
    
    -- Optional: Add Foreign Key constraint
    -- ALTER TABLE SaleItems ADD CONSTRAINT FK_SaleItems_PurchaseItems FOREIGN KEY (PurchaseItemID) REFERENCES PurchaseItems(PurchaseItemID);
    
    PRINT 'PurchaseItemID column added to SaleItems.';
END
