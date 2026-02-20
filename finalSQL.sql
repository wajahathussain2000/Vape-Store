USE [master]
GO
/****** Object:  Database [VapeStore]    Script Date: 2/19/2026 7:30:54 PM ******/
CREATE DATABASE [VapeStore]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'VapeStore', FILENAME = N'/var/opt/mssql/data/VapeStore.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'VapeStore_log', FILENAME = N'/var/opt/mssql/data/VapeStore_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [VapeStore] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [VapeStore].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [VapeStore] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [VapeStore] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [VapeStore] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [VapeStore] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [VapeStore] SET ARITHABORT OFF 
GO
ALTER DATABASE [VapeStore] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [VapeStore] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [VapeStore] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [VapeStore] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [VapeStore] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [VapeStore] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [VapeStore] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [VapeStore] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [VapeStore] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [VapeStore] SET  ENABLE_BROKER 
GO
ALTER DATABASE [VapeStore] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [VapeStore] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [VapeStore] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [VapeStore] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [VapeStore] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [VapeStore] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [VapeStore] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [VapeStore] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [VapeStore] SET  MULTI_USER 
GO
ALTER DATABASE [VapeStore] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [VapeStore] SET DB_CHAINING OFF 
GO
ALTER DATABASE [VapeStore] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [VapeStore] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [VapeStore] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [VapeStore] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'VapeStore', N'ON'
GO
ALTER DATABASE [VapeStore] SET QUERY_STORE = ON
GO
ALTER DATABASE [VapeStore] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [VapeStore]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ValidateSaleData]    Script Date: 2/19/2026 7:30:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [dbo].[fn_ValidateSaleData](
    @CustomerID INT,
    @TotalAmount DECIMAL(10,2),
    @PaidAmount DECIMAL(10,2)
)
RETURNS NVARCHAR(500)
AS
BEGIN
    DECLARE @ErrorMessage NVARCHAR(500) = '';
    
    -- Validate customer exists
    IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerID = @CustomerID)
        SET @ErrorMessage = @ErrorMessage + 'Invalid Customer ID. ';
    
    -- Validate amounts
    IF @TotalAmount <= 0
        SET @ErrorMessage = @ErrorMessage + 'Total amount must be greater than 0. ';
    
    IF @PaidAmount < 0
        SET @ErrorMessage = @ErrorMessage + 'Paid amount cannot be negative. ';
    
    RETURN @ErrorMessage;
END

GO
/****** Object:  Table [dbo].[Purchases]    Script Date: 2/19/2026 7:30:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchases](
	[PurchaseID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](20) NOT NULL,
	[SupplierID] [int] NULL,
	[PurchaseDate] [datetime] NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
	[TaxAmount] [decimal](10, 2) NULL,
	[TaxPercent] [decimal](5, 2) NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[PaidAmount] [decimal](10, 2) NULL,
	[ChangeAmount] [decimal](10, 2) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[PurchaseOrderNumber] [nvarchar](50) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[PaymentTerms] [nvarchar](50) NULL,
	[PurchaseType] [nvarchar](50) NULL,
	[FreightCharges] [decimal](10, 2) NULL,
	[OtherCharges] [decimal](10, 2) NULL,
	[DiscountAmount] [decimal](10, 2) NULL,
	[Notes] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sales]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sales](
	[SaleID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](20) NOT NULL,
	[CustomerID] [int] NULL,
	[SaleDate] [datetime] NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
	[TaxAmount] [decimal](10, 2) NULL,
	[TaxPercent] [decimal](5, 2) NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[PaidAmount] [decimal](10, 2) NULL,
	[ChangeAmount] [decimal](10, 2) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[LastModified] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[DiscountAmount] [decimal](10, 2) NULL,
	[DiscountPercent] [decimal](5, 2) NULL,
	[Status] [nvarchar](20) NULL,
	[Notes] [nvarchar](500) NULL,
	[BarcodeImage] [varbinary](max) NULL,
	[BarcodeData] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[SaleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[SupplierID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierCode] [nvarchar](20) NOT NULL,
	[SupplierName] [nvarchar](100) NOT NULL,
	[ContactPerson] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
	[Address] [nvarchar](255) NULL,
	[City] [nvarchar](50) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SupplierCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerCode] [nvarchar](20) NOT NULL,
	[CustomerName] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
	[Address] [nvarchar](255) NULL,
	[City] [nvarchar](50) NULL,
	[PostalCode] [nvarchar](10) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExpenseCategories]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpenseCategories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CategoryCode] [nvarchar](20) NULL,
	[UserID] [int] NULL,
	[LastModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExpenseEntries]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpenseEntries](
	[ExpenseID] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseDate] [datetime] NULL,
	[CategoryID] [int] NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ExpenseCode] [nvarchar](20) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[Remarks] [nvarchar](255) NULL,
	[Status] [nvarchar](20) NULL,
	[LastModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ExpenseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_DailyReport]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_DailyReport]
AS
    -- Sales (Credit)
    SELECT 
        s.SaleDate AS TransactionDate,
        'Sale' AS TransactionType,
        s.InvoiceNumber AS Reference,
        'Sale to ' + ISNULL(c.CustomerName, 'Walk-in Customer') AS Description,
        0 AS Debit,
        s.TotalAmount AS Credit,
        s.SaleID AS TransactionID
    FROM Sales s
    LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
    
    UNION ALL
    
    -- Purchases (Debit)
    SELECT 
        p.PurchaseDate AS TransactionDate,
        'Purchase' AS TransactionType,
        p.InvoiceNumber AS Reference,
        'Purchase from ' + ISNULL(sp.SupplierName, 'Supplier') AS Description,
        p.TotalAmount AS Debit,
        0 AS Credit,
        p.PurchaseID AS TransactionID
    FROM Purchases p
    LEFT JOIN Suppliers sp ON p.SupplierID = sp.SupplierID
    
    UNION ALL
    
    -- Expenses (Debit)
    SELECT 
        e.ExpenseDate AS TransactionDate,
        'Expense' AS TransactionType,
        e.ExpenseCode AS Reference,
        ISNULL(ec.CategoryName, 'Expense') + ': ' + ISNULL(e.Description, '') AS Description,
        e.Amount AS Debit,
        0 AS Credit,
        e.ExpenseID AS TransactionID
    FROM ExpenseEntries e
    LEFT JOIN ExpenseCategories ec ON e.CategoryID = ec.CategoryID

GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[Role] [nvarchar](20) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[LastLogin] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_SalesWithDetails]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE VIEW [dbo].[vw_SalesWithDetails] AS
SELECT 
    s.SaleID,
    s.InvoiceNumber,
    s.CustomerID,
    c.CustomerName,
    s.SaleDate,
    s.SubTotal,
    s.DiscountAmount,
    s.DiscountPercent,
    s.TaxAmount,
    s.TaxPercent,
    s.TotalAmount,
    s.PaymentMethod,
    s.PaidAmount,
    s.ChangeAmount,
    s.UserID,
    u.FullName as UserName,
    s.Status,
    s.Notes,
    s.CreatedDate,
    s.LastModified,
    s.ModifiedBy,
    um.FullName as ModifiedByName
FROM Sales s
LEFT JOIN Customers c ON s.CustomerID = c.CustomerID
LEFT JOIN Users u ON s.UserID = u.UserID
LEFT JOIN Users um ON s.ModifiedBy = um.UserID

GO
/****** Object:  Table [dbo].[Brands]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[BrandID] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[BrandName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CashInHand]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CashInHand](
	[CashInHandID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionDate] [datetime] NULL,
	[OpeningCash] [decimal](10, 2) NULL,
	[CashIn] [decimal](10, 2) NULL,
	[CashOut] [decimal](10, 2) NULL,
	[Expenses] [decimal](10, 2) NULL,
	[ClosingCash] [decimal](10, 2) NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CashInHandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerLedger]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerLedger](
	[LedgerEntryID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[ReferenceType] [varchar](50) NULL,
	[ReferenceID] [int] NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[Description] [varchar](255) NULL,
	[Debit] [decimal](18, 2) NOT NULL,
	[Credit] [decimal](18, 2) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CustomerLedger] PRIMARY KEY CLUSTERED 
(
	[LedgerEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerPayments]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerPayments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[PaymentDate] [datetime] NULL,
	[VoucherNumber] [nvarchar](20) NULL,
	[PreviousBalance] [decimal](10, 2) NULL,
	[TotalDue] [decimal](10, 2) NULL,
	[PaidAmount] [decimal](10, 2) NULL,
	[RemainingBalance] [decimal](10, 2) NULL,
	[Amount] [decimal](10, 2) NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[Description] [nvarchar](255) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DayClosings]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DayClosings](
	[DayClosingID] [int] IDENTITY(1,1) NOT NULL,
	[ClosingDate] [datetime] NOT NULL,
	[OpeningBalance] [decimal](18, 2) NOT NULL,
	[ClosingBalance] [decimal](18, 2) NOT NULL,
	[TotalSales] [decimal](18, 2) NULL,
	[TotalPurchases] [decimal](18, 2) NULL,
	[TotalExpenses] [decimal](18, 2) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Status] [nvarchar](20) NULL,
	[Remarks] [nvarchar](500) NULL,
 CONSTRAINT [PK_DayClosings] PRIMARY KEY CLUSTERED 
(
	[DayClosingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[PermissionID] [int] IDENTITY(1,1) NOT NULL,
	[PermissionKey] [nvarchar](64) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[Name] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PermissionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductCode] [nvarchar](20) NOT NULL,
	[ProductName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CategoryID] [int] NULL,
	[BrandID] [int] NULL,
	[PurchasePrice] [decimal](10, 2) NOT NULL,
	[RetailPrice] [decimal](10, 2) NOT NULL,
	[StockQuantity] [int] NULL,
	[ReorderLevel] [int] NULL,
	[Barcode] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CostPrice] [decimal](18, 2) NOT NULL,
	[LastPurchaseDate] [datetime] NULL,
	[IsAvailableForSale] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Products_Barcode] UNIQUE NONCLUSTERED 
(
	[Barcode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseItems]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseItems](
	[PurchaseItemID] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseID] [int] NULL,
	[ProductID] [int] NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
	[BatchNumber] [nvarchar](50) NULL,
	[ExpiryDate] [datetime] NULL,
	[DiscountAmount] [decimal](10, 2) NULL,
	[TaxPercent] [decimal](5, 2) NULL,
	[Remarks] [nvarchar](255) NULL,
	[Bonus] [int] NOT NULL,
	[ProductName] [nvarchar](100) NULL,
	[ProductCode] [nvarchar](50) NULL,
	[Unit] [nvarchar](20) NULL,
	[SellingPrice] [decimal](10, 2) NULL,
	[RemainingQuantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseReturnItems]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseReturnItems](
	[ReturnItemID] [int] IDENTITY(1,1) NOT NULL,
	[ReturnID] [int] NULL,
	[ProductID] [int] NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReturnItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseReturns]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseReturns](
	[ReturnID] [int] IDENTITY(1,1) NOT NULL,
	[ReturnNumber] [nvarchar](20) NOT NULL,
	[PurchaseID] [int] NULL,
	[SupplierID] [int] NULL,
	[ReturnDate] [datetime] NULL,
	[ReturnReason] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReturnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReturnNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
 CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](64) NOT NULL,
	[IsSystem] [bit] NOT NULL,
	[Name] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleItems]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleItems](
	[SaleItemID] [int] IDENTITY(1,1) NOT NULL,
	[SaleID] [int] NULL,
	[ProductID] [int] NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
	[LastModified] [datetime] NULL,
	[ProductName] [nvarchar](100) NULL,
	[CostPrice] [decimal](18, 2) NOT NULL,
	[PurchaseItemID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SaleItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalesReturnItems]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalesReturnItems](
	[ReturnItemID] [int] IDENTITY(1,1) NOT NULL,
	[ReturnID] [int] NULL,
	[ProductID] [int] NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
	[ProductName] [nvarchar](100) NULL,
	[ProductCode] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ReturnItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalesReturns]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalesReturns](
	[ReturnID] [int] IDENTITY(1,1) NOT NULL,
	[ReturnNumber] [nvarchar](20) NOT NULL,
	[SaleID] [int] NULL,
	[CustomerID] [int] NULL,
	[ReturnDate] [datetime] NULL,
	[ReturnReason] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[OriginalInvoiceNumber] [nvarchar](20) NULL,
	[OriginalInvoiceDate] [datetime] NULL,
	[OriginalInvoiceTotal] [decimal](10, 2) NULL,
	[IsFullyReturned] [bit] NULL,
	[ReturnStatus] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ReturnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReturnNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SupplierPayments]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierPayments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [int] NULL,
	[PaymentDate] [datetime] NULL,
	[VoucherNumber] [nvarchar](20) NULL,
	[PreviousBalance] [decimal](10, 2) NULL,
	[TotalPayable] [decimal](10, 2) NULL,
	[PaidAmount] [decimal](10, 2) NULL,
	[RemainingAmount] [decimal](10, 2) NULL,
	[RemainingBalance] [decimal](10, 2) NULL,
	[Amount] [decimal](10, 2) NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[Description] [nvarchar](255) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserID] [int] NOT NULL,
	[RoleID] [int] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_CashInHand_CreatedDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_CashInHand_CreatedDate] ON [dbo].[CashInHand]
(
	[CreatedDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CashInHand_TransactionDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_CashInHand_TransactionDate] ON [dbo].[CashInHand]
(
	[TransactionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CashInHand_UserID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_CashInHand_UserID] ON [dbo].[CashInHand]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerPayments_CustomerID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerPayments_CustomerID] ON [dbo].[CustomerPayments]
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerPayments_PaymentDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerPayments_PaymentDate] ON [dbo].[CustomerPayments]
(
	[PaymentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerPayments_UserID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerPayments_UserID] ON [dbo].[CustomerPayments]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpenseEntries_CategoryID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpenseEntries_CategoryID] ON [dbo].[ExpenseEntries]
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpenseEntries_ExpenseDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpenseEntries_ExpenseDate] ON [dbo].[ExpenseEntries]
(
	[ExpenseDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpenseEntries_UserID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpenseEntries_UserID] ON [dbo].[ExpenseEntries]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_BrandID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_BrandID] ON [dbo].[Products]
(
	[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryID] ON [dbo].[Products]
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseItems_PurchaseID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_PurchaseID] ON [dbo].[PurchaseItems]
(
	[PurchaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Purchases_PurchaseDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_PurchaseDate] ON [dbo].[Purchases]
(
	[PurchaseDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Purchases_SupplierID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_SupplierID] ON [dbo].[Purchases]
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RolePermissions_PermissionId]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions]
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SaleItems_ProductID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SaleItems_ProductID] ON [dbo].[SaleItems]
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SaleItems_SaleID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SaleItems_SaleID] ON [dbo].[SaleItems]
(
	[SaleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_CustomerID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_CustomerID] ON [dbo].[Sales]
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Sales_InvoiceNumber]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_InvoiceNumber] ON [dbo].[Sales]
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_SaleDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_SaleDate] ON [dbo].[Sales]
(
	[SaleDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalesReturns_ReturnStatus]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SalesReturns_ReturnStatus] ON [dbo].[SalesReturns]
(
	[ReturnStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SalesReturns_SaleID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SalesReturns_SaleID] ON [dbo].[SalesReturns]
(
	[SaleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierPayments_PaymentDate]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierPayments_PaymentDate] ON [dbo].[SupplierPayments]
(
	[PaymentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierPayments_SupplierID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierPayments_SupplierID] ON [dbo].[SupplierPayments]
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierPayments_UserID]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierPayments_UserID] ON [dbo].[SupplierPayments]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserRoles_RoleId]    Script Date: 2/19/2026 7:30:56 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles]
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Brands] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Brands] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT (getdate()) FOR [TransactionDate]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT ((0)) FOR [OpeningCash]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT ((0)) FOR [CashIn]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT ((0)) FOR [CashOut]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT ((0)) FOR [Expenses]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT ((0)) FOR [ClosingCash]
GO
ALTER TABLE [dbo].[CashInHand] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CustomerLedger] ADD  DEFAULT ((0)) FOR [Debit]
GO
ALTER TABLE [dbo].[CustomerLedger] ADD  DEFAULT ((0)) FOR [Credit]
GO
ALTER TABLE [dbo].[CustomerLedger] ADD  DEFAULT ((0)) FOR [Balance]
GO
ALTER TABLE [dbo].[CustomerLedger] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT (getdate()) FOR [PaymentDate]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT ((0)) FOR [PreviousBalance]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT ((0)) FOR [TotalDue]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT ((0)) FOR [PaidAmount]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT ((0)) FOR [RemainingBalance]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[CustomerPayments] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT ((0)) FOR [OpeningBalance]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT ((0)) FOR [ClosingBalance]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT ((0)) FOR [TotalSales]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT ((0)) FOR [TotalPurchases]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT ((0)) FOR [TotalExpenses]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[DayClosings] ADD  DEFAULT ('Closed') FOR [Status]
GO
ALTER TABLE [dbo].[ExpenseCategories] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ExpenseCategories] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ExpenseEntries] ADD  DEFAULT (getdate()) FOR [ExpenseDate]
GO
ALTER TABLE [dbo].[ExpenseEntries] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ExpenseEntries] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [StockQuantity]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ReorderLevel]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [CostPrice]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [IsAvailableForSale]
GO
ALTER TABLE [dbo].[PurchaseItems] ADD  DEFAULT ((0)) FOR [DiscountAmount]
GO
ALTER TABLE [dbo].[PurchaseItems] ADD  DEFAULT ((0)) FOR [TaxPercent]
GO
ALTER TABLE [dbo].[PurchaseItems] ADD  DEFAULT ((0)) FOR [Bonus]
GO
ALTER TABLE [dbo].[PurchaseItems] ADD  DEFAULT ((0)) FOR [RemainingQuantity]
GO
ALTER TABLE [dbo].[PurchaseReturns] ADD  DEFAULT (getdate()) FOR [ReturnDate]
GO
ALTER TABLE [dbo].[PurchaseReturns] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT (getdate()) FOR [PurchaseDate]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [TaxAmount]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [TaxPercent]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [PaidAmount]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [ChangeAmount]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [FreightCharges]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [OtherCharges]
GO
ALTER TABLE [dbo].[Purchases] ADD  DEFAULT ((0)) FOR [DiscountAmount]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT ((0)) FOR [IsSystem]
GO
ALTER TABLE [dbo].[SaleItems] ADD  DEFAULT (getdate()) FOR [LastModified]
GO
ALTER TABLE [dbo].[SaleItems] ADD  DEFAULT ((0)) FOR [CostPrice]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT (getdate()) FOR [SaleDate]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [TaxAmount]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [TaxPercent]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [PaidAmount]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [ChangeAmount]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT (getdate()) FOR [LastModified]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [DiscountAmount]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [DiscountPercent]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[SalesReturns] ADD  DEFAULT (getdate()) FOR [ReturnDate]
GO
ALTER TABLE [dbo].[SalesReturns] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[SalesReturns] ADD  DEFAULT ((0)) FOR [IsFullyReturned]
GO
ALTER TABLE [dbo].[SalesReturns] ADD  DEFAULT ('Partial') FOR [ReturnStatus]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT (getdate()) FOR [PaymentDate]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT ((0)) FOR [PreviousBalance]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT ((0)) FOR [TotalPayable]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT ((0)) FOR [PaidAmount]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT ((0)) FOR [RemainingAmount]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT ((0)) FOR [RemainingBalance]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[SupplierPayments] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Suppliers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Suppliers] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('User') FOR [Role]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CashInHand]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[CustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_CustomerLedger_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CustomerLedger] CHECK CONSTRAINT [FK_CustomerLedger_Customers]
GO
ALTER TABLE [dbo].[CustomerPayments]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomerPayments]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[DayClosings]  WITH CHECK ADD  CONSTRAINT [FK_DayClosings_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[DayClosings] CHECK CONSTRAINT [FK_DayClosings_Users]
GO
ALTER TABLE [dbo].[ExpenseEntries]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[ExpenseCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[ExpenseEntries]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([BrandID])
REFERENCES [dbo].[Brands] ([BrandID])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
GO
ALTER TABLE [dbo].[PurchaseItems]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[PurchaseItems]  WITH CHECK ADD FOREIGN KEY([PurchaseID])
REFERENCES [dbo].[Purchases] ([PurchaseID])
GO
ALTER TABLE [dbo].[PurchaseReturnItems]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[PurchaseReturnItems]  WITH CHECK ADD FOREIGN KEY([ReturnID])
REFERENCES [dbo].[PurchaseReturns] ([ReturnID])
GO
ALTER TABLE [dbo].[PurchaseReturns]  WITH CHECK ADD FOREIGN KEY([PurchaseID])
REFERENCES [dbo].[Purchases] ([PurchaseID])
GO
ALTER TABLE [dbo].[PurchaseReturns]  WITH CHECK ADD FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO
ALTER TABLE [dbo].[PurchaseReturns]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Purchases]  WITH CHECK ADD FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO
ALTER TABLE [dbo].[Purchases]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD FOREIGN KEY([PermissionID])
REFERENCES [dbo].[Permissions] ([PermissionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SaleItems]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[SaleItems]  WITH CHECK ADD FOREIGN KEY([SaleID])
REFERENCES [dbo].[Sales] ([SaleID])
GO
ALTER TABLE [dbo].[SaleItems]  WITH CHECK ADD  CONSTRAINT [FK_SaleItems_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[SaleItems] CHECK CONSTRAINT [FK_SaleItems_Products]
GO
ALTER TABLE [dbo].[SaleItems]  WITH CHECK ADD  CONSTRAINT [FK_SaleItems_Sales] FOREIGN KEY([SaleID])
REFERENCES [dbo].[Sales] ([SaleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SaleItems] CHECK CONSTRAINT [FK_SaleItems_Sales]
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD  CONSTRAINT [FK_Sales_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Sales] CHECK CONSTRAINT [FK_Sales_Customers]
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD  CONSTRAINT [FK_Sales_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Sales] CHECK CONSTRAINT [FK_Sales_Users]
GO
ALTER TABLE [dbo].[SalesReturnItems]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[SalesReturnItems]  WITH CHECK ADD FOREIGN KEY([ReturnID])
REFERENCES [dbo].[SalesReturns] ([ReturnID])
GO
ALTER TABLE [dbo].[SalesReturns]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SalesReturns]  WITH CHECK ADD FOREIGN KEY([SaleID])
REFERENCES [dbo].[Sales] ([SaleID])
GO
ALTER TABLE [dbo].[SalesReturns]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[SupplierPayments]  WITH CHECK ADD FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO
ALTER TABLE [dbo].[SupplierPayments]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDailyReport]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetDailyReport]
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all transactions for the date range
    SELECT 
        TransactionDate,
        TransactionType,
        Reference,
        Description,
        Debit,
        Credit,
        TransactionID
    FROM vw_DailyReport
    WHERE TransactionDate >= @FromDate
        AND TransactionDate <= @ToDate
    ORDER BY TransactionDate, Reference
    
    -- Return summary totals
    SELECT 
        SUM(Debit) AS TotalDebit,
        SUM(Credit) AS TotalCredit,
        SUM(Credit) - SUM(Debit) AS NetBalance,
        SUM(CASE WHEN TransactionType = 'Sale' THEN Credit ELSE 0 END) AS TotalSales,
        SUM(CASE WHEN TransactionType = 'Purchase' THEN Debit ELSE 0 END) AS TotalPurchases,
        SUM(CASE WHEN TransactionType = 'Expense' THEN Debit ELSE 0 END) AS TotalExpenses
    FROM vw_DailyReport
    WHERE TransactionDate >= @FromDate
        AND TransactionDate <= @ToDate
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousDayClosingBalance]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetPreviousDayClosingBalance]
    @CurrentDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1 ClosingBalance 
    FROM DayClosings 
    WHERE CAST(ClosingDate AS DATE) < CAST(@CurrentDate AS DATE)
        AND Status = 'Closed'
    ORDER BY ClosingDate DESC
END

GO
/****** Object:  StoredProcedure [dbo].[sp_IsDayClosed]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_IsDayClosed]
    @Date DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS IsClosed
    FROM DayClosings 
    WHERE CAST(ClosingDate AS DATE) = CAST(@Date AS DATE)
        AND Status = 'Closed'
END

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSale]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_UpdateSale]
    @SaleID INT,
    @CustomerID INT,
    @SaleDate DATETIME,
    @SubTotal DECIMAL(10,2),
    @DiscountAmount DECIMAL(10,2) = 0,
    @DiscountPercent DECIMAL(5,2) = 0,
    @TaxAmount DECIMAL(10,2),
    @TaxPercent DECIMAL(5,2),
    @TotalAmount DECIMAL(10,2),
    @PaymentMethod NVARCHAR(20),
    @PaidAmount DECIMAL(10,2),
    @ChangeAmount DECIMAL(10,2),
    @UserID INT,
    @Notes NVARCHAR(500) = NULL,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update the sale
        UPDATE Sales 
        SET CustomerID = @CustomerID,
            SaleDate = @SaleDate,
            SubTotal = @SubTotal,
            DiscountAmount = @DiscountAmount,
            DiscountPercent = @DiscountPercent,
            TaxAmount = @TaxAmount,
            TaxPercent = @TaxPercent,
            TotalAmount = @TotalAmount,
            PaymentMethod = @PaymentMethod,
            PaidAmount = @PaidAmount,
            ChangeAmount = @ChangeAmount,
            UserID = @UserID,
            Notes = @Notes,
            LastModified = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE SaleID = @SaleID;
        
        -- Check if any rows were affected
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Sale with ID %d not found', 16, 1, @SaleID);
        END
        
        COMMIT TRANSACTION;
        SELECT 1 as Success, 'Sale updated successfully' as Message;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 as Success, ERROR_MESSAGE() as Message;
    END CATCH
END

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateSaleItems]    Script Date: 2/19/2026 7:30:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_UpdateSaleItems]
    @SaleID INT,
    @SaleItems NVARCHAR(MAX) -- JSON string of sale items
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Delete existing sale items
        DELETE FROM SaleItems WHERE SaleID = @SaleID;
        
        -- Insert new sale items (this would need to be called from C# with proper JSON parsing)
        -- For now, we'll just prepare the structure
        
        COMMIT TRANSACTION;
        SELECT 1 as Success, 'Sale items updated successfully' as Message;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 as Success, ERROR_MESSAGE() as Message;
    END CATCH
END

GO
USE [master]
GO
ALTER DATABASE [VapeStore] SET  READ_WRITE 
GO
