-- FIFO Inventory Update: Adding RemainingQuantity to track stock per batch
-- Target: PurchaseItems table

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PurchaseItems') AND name = 'RemainingQuantity')
BEGIN
    ALTER TABLE PurchaseItems ADD RemainingQuantity INT NOT NULL DEFAULT 0;
    
    -- Initialize RemainingQuantity for existing items
    -- We assume current stock is the untracked remaining quantity for all historical items
    -- A better approach might be needed for very large databases, but here we sync with Quantity + Bonus
    EXEC('UPDATE PurchaseItems SET RemainingQuantity = Quantity + Bonus');
    
    PRINT 'RemainingQuantity column added and initialized.';
END
ELSE
BEGIN
    PRINT 'RemainingQuantity column already exists.';
END
