-- Customer ledger schema update script
-- Run this against the VapeStore database to add comprehensive customer ledger tracking

IF OBJECT_ID('dbo.CustomerLedger', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CustomerLedger](
        [LedgerEntryID] [int] IDENTITY(1,1) NOT NULL,
        [CustomerID] [int] NOT NULL,
        [EntryDate] [datetime] NOT NULL DEFAULT (getdate()),
        [ReferenceType] [nvarchar](50) NULL,
        [ReferenceID] [int] NULL,
        [InvoiceNumber] [nvarchar](50) NULL,
        [Description] [nvarchar](255) NULL,
        [Debit] [decimal](18, 2) NOT NULL DEFAULT ((0)),
        [Credit] [decimal](18, 2) NOT NULL DEFAULT ((0)),
        [Balance] [decimal](18, 2) NOT NULL DEFAULT ((0)),
        [CreatedDate] [datetime] NOT NULL DEFAULT (getdate()),
    PRIMARY KEY CLUSTERED 
    (
        [LedgerEntryID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[CustomerLedger]  WITH CHECK
        ADD FOREIGN KEY([CustomerID]) REFERENCES [dbo].[Customers] ([CustomerID]);

    CREATE NONCLUSTERED INDEX [IX_CustomerLedger_CustomerID_EntryDate]
    ON [dbo].[CustomerLedger]([CustomerID] ASC, [EntryDate] ASC, [LedgerEntryID] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF,
          DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

    CREATE NONCLUSTERED INDEX [IX_CustomerLedger_Reference]
    ON [dbo].[CustomerLedger]([ReferenceType] ASC, [ReferenceID] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF,
          DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
GO

IF OBJECT_ID('dbo.SupplierLedger', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SupplierLedger](
        [LedgerEntryID] [int] IDENTITY(1,1) NOT NULL,
        [SupplierID] [int] NOT NULL,
        [EntryDate] [datetime] NOT NULL DEFAULT (getdate()),
        [ReferenceType] [nvarchar](50) NULL,
        [ReferenceID] [int] NULL,
        [InvoiceNumber] [nvarchar](50) NULL,
        [Description] [nvarchar](255) NULL,
        [Debit] [decimal](18, 2) NOT NULL DEFAULT ((0)),
        [Credit] [decimal](18, 2) NOT NULL DEFAULT ((0)),
        [Balance] [decimal](18, 2) NOT NULL DEFAULT ((0)),
        [CreatedDate] [datetime] NOT NULL DEFAULT (getdate()),
    PRIMARY KEY CLUSTERED 
    (
        [LedgerEntryID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[SupplierLedger]  WITH CHECK
        ADD FOREIGN KEY([SupplierID]) REFERENCES [dbo].[Suppliers] ([SupplierID]);

    CREATE NONCLUSTERED INDEX [IX_SupplierLedger_SupplierID_EntryDate]
    ON [dbo].[SupplierLedger]([SupplierID] ASC, [EntryDate] ASC, [LedgerEntryID] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF,
          DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

    CREATE NONCLUSTERED INDEX [IX_SupplierLedger_Reference]
    ON [dbo].[SupplierLedger]([ReferenceType] ASC, [ReferenceID] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF,
          DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
GO

-- Optional: future spot to copy data from historical sales/payments into ledger if needed
-- INSERT logic would go here if you want to backfill existing transactions.

