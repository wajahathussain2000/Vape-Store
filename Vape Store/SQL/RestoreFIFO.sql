-- FIFO Data Restoration Script
-- This script resets RemainingQuantity and re-links SaleItems to correct FIFO batches

BEGIN TRANSACTION;
BEGIN TRY
    -- 1. Reset all RemainingQuantity to original purchase totals
    UPDATE PurchaseItems SET RemainingQuantity = Quantity + Bonus;

    -- 2. Clear PurchaseItemID from SaleItems temporarily (or we can just keep them and re-assign)
    -- Actually, it's better to re-assign them to ensure FIFO integrity
    
    DECLARE @SaleItemID INT, @ProductID INT, @QuantityToSell INT;
    
    -- Cursor to iterate through all SaleItems in chronological order
    DECLARE SaleCursor CURSOR FOR 
    SELECT si.SaleItemID, si.ProductID, si.Quantity 
    FROM SaleItems si
    JOIN Sales s ON si.SaleID = s.SaleID
    ORDER BY s.SaleDate ASC, si.SaleItemID ASC;

    OPEN SaleCursor;
    FETCH NEXT FROM SaleCursor INTO @SaleItemID, @ProductID, @QuantityToSell;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- For each SaleItem, find and consume batches in FIFO order
        DECLARE @RemainingToProcess INT = @QuantityToSell;
        DECLARE @FirstBatchID INT = NULL;

        -- Sub-cursor for available batches
        DECLARE BatchCursor CURSOR FOR
        SELECT pi.PurchaseItemID, pi.RemainingQuantity
        FROM PurchaseItems pi
        JOIN Purchases p ON pi.PurchaseID = p.PurchaseID
        WHERE pi.ProductID = @ProductID AND pi.RemainingQuantity > 0
        ORDER BY p.PurchaseDate ASC, pi.PurchaseItemID ASC;

        OPEN BatchCursor;
        DECLARE @BatchID INT, @BatchRemaining INT;
        FETCH NEXT FROM BatchCursor INTO @BatchID, @BatchRemaining;

        WHILE @@FETCH_STATUS = 0 AND @RemainingToProcess > 0
        BEGIN
            IF @FirstBatchID IS NULL SET @FirstBatchID = @BatchID;

            DECLARE @Consume INT = CASE WHEN @RemainingToProcess < @BatchRemaining THEN @RemainingToProcess ELSE @BatchRemaining END;
            
            UPDATE PurchaseItems SET RemainingQuantity = RemainingQuantity - @Consume WHERE PurchaseItemID = @BatchID;
            
            SET @RemainingToProcess = @RemainingToProcess - @Consume;
            FETCH NEXT FROM BatchCursor INTO @BatchID, @BatchRemaining;
        END

        CLOSE BatchCursor;
        DEALLOCATE BatchCursor;

        -- Update the SaleItem with the first batch it used (simplification for reporting)
        -- In a perfect world, we'd split SaleItems if they span batches, 
        -- but for restoration, we'll link it to the primary batch used.
        UPDATE SaleItems SET PurchaseItemID = @FirstBatchID WHERE SaleItemID = @SaleItemID;

        FETCH NEXT FROM SaleCursor INTO @SaleItemID, @ProductID, @QuantityToSell;
    END

    CLOSE SaleCursor;
    DEALLOCATE SaleCursor;

    -- 3. Synchronize global Product stock counts
    UPDATE Products SET StockQuantity = (SELECT ISNULL(SUM(RemainingQuantity), 0) FROM PurchaseItems WHERE ProductID = Products.ProductID);

    COMMIT TRANSACTION;
    PRINT 'FIFO Data Restoration Successful';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error during Restoration: ' + ERROR_MESSAGE();
END CATCH
