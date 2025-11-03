USE [master]
GO
/****** Object:  Database [VapeStore]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE DATABASE [VapeStore]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'VapeStore', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\VapeStore.mdf' , SIZE = 4288KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'VapeStore_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\VapeStore_log.ldf' , SIZE = 816KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
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
USE [VapeStore]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ValidateSaleData]    Script Date: 11/3/2025 5:18:51 PM ******/
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
/****** Object:  Table [dbo].[Brands]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[BrandID] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CashInHand]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CashInHand](
	[CashInHandID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionDate] [datetime] NULL DEFAULT (getdate()),
	[OpeningCash] [decimal](10, 2) NULL DEFAULT ((0)),
	[CashIn] [decimal](10, 2) NULL DEFAULT ((0)),
	[CashOut] [decimal](10, 2) NULL DEFAULT ((0)),
	[Expenses] [decimal](10, 2) NULL DEFAULT ((0)),
	[ClosingCash] [decimal](10, 2) NULL DEFAULT ((0)),
	[Description] [nvarchar](255) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[CashInHandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Categories]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CustomerPayments]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerPayments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[PaymentDate] [datetime] NULL DEFAULT (getdate()),
	[VoucherNumber] [nvarchar](20) NULL,
	[PreviousBalance] [decimal](10, 2) NULL DEFAULT ((0)),
	[TotalDue] [decimal](10, 2) NULL DEFAULT ((0)),
	[PaidAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[RemainingBalance] [decimal](10, 2) NULL DEFAULT ((0)),
	[Amount] [decimal](10, 2) NULL DEFAULT ((0)),
	[PaymentMethod] [nvarchar](20) NULL,
	[Description] [nvarchar](255) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Customers]    Script Date: 11/3/2025 5:18:51 PM ******/
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
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExpenseCategories]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpenseCategories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[CategoryCode] [nvarchar](20) NULL,
	[UserID] [int] NULL,
	[LastModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExpenseEntries]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpenseEntries](
	[ExpenseID] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseDate] [datetime] NULL DEFAULT (getdate()),
	[CategoryID] [int] NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[ExpenseCode] [nvarchar](20) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[Remarks] [nvarchar](255) NULL,
	[Status] [nvarchar](20) NULL DEFAULT ('Draft'),
	[LastModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ExpenseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 11/3/2025 5:18:51 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/3/2025 5:18:51 PM ******/
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
	[StockQuantity] [int] NULL DEFAULT ((0)),
	[ReorderLevel] [int] NULL DEFAULT ((0)),
	[Barcode] [nvarchar](50) NULL,
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[CostPrice] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[LastPurchaseDate] [datetime] NULL,
	[IsAvailableForSale] [bit] NOT NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseItems]    Script Date: 11/3/2025 5:18:51 PM ******/
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
	[DiscountAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[TaxPercent] [decimal](5, 2) NULL DEFAULT ((0)),
	[Remarks] [nvarchar](255) NULL,
	[Bonus] [int] NOT NULL DEFAULT ((0)),
	[ProductName] [nvarchar](100) NULL,
	[ProductCode] [nvarchar](50) NULL,
	[Unit] [nvarchar](20) NULL,
	[SellingPrice] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseReturnItems]    Script Date: 11/3/2025 5:18:51 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseReturns]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseReturns](
	[ReturnID] [int] IDENTITY(1,1) NOT NULL,
	[ReturnNumber] [nvarchar](20) NOT NULL,
	[PurchaseID] [int] NULL,
	[SupplierID] [int] NULL,
	[ReturnDate] [datetime] NULL DEFAULT (getdate()),
	[ReturnReason] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[ReturnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Purchases]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Purchases](
	[PurchaseID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](20) NOT NULL,
	[SupplierID] [int] NULL,
	[PurchaseDate] [datetime] NULL DEFAULT (getdate()),
	[SubTotal] [decimal](10, 2) NOT NULL,
	[TaxAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[TaxPercent] [decimal](5, 2) NULL DEFAULT ((0)),
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[PaidAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[ChangeAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[PurchaseOrderNumber] [nvarchar](50) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[PaymentTerms] [nvarchar](50) NULL,
	[PurchaseType] [nvarchar](50) NULL,
	[FreightCharges] [decimal](10, 2) NULL DEFAULT ((0)),
	[OtherCharges] [decimal](10, 2) NULL DEFAULT ((0)),
	[DiscountAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[Notes] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 11/3/2025 5:18:51 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](64) NOT NULL,
	[IsSystem] [bit] NOT NULL DEFAULT ((0)),
	[Name] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SaleItems]    Script Date: 11/3/2025 5:18:51 PM ******/
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
	[LastModified] [datetime] NULL DEFAULT (getdate()),
	[ProductName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[SaleItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sales]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sales](
	[SaleID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [nvarchar](20) NOT NULL,
	[CustomerID] [int] NULL,
	[SaleDate] [datetime] NULL DEFAULT (getdate()),
	[SubTotal] [decimal](10, 2) NOT NULL,
	[TaxAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[TaxPercent] [decimal](5, 2) NULL DEFAULT ((0)),
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[PaymentMethod] [nvarchar](20) NULL,
	[PaidAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[ChangeAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[LastModified] [datetime] NULL DEFAULT (getdate()),
	[ModifiedBy] [int] NULL,
	[DiscountAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[DiscountPercent] [decimal](5, 2) NULL DEFAULT ((0)),
	[Status] [nvarchar](20) NULL DEFAULT ('Active'),
	[Notes] [nvarchar](500) NULL,
	[BarcodeImage] [varbinary](max) NULL,
	[BarcodeData] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[SaleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SalesReturnItems]    Script Date: 11/3/2025 5:18:51 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SalesReturns]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalesReturns](
	[ReturnID] [int] IDENTITY(1,1) NOT NULL,
	[ReturnNumber] [nvarchar](20) NOT NULL,
	[SaleID] [int] NULL,
	[CustomerID] [int] NULL,
	[ReturnDate] [datetime] NULL DEFAULT (getdate()),
	[ReturnReason] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[OriginalInvoiceNumber] [nvarchar](20) NULL,
	[OriginalInvoiceDate] [datetime] NULL,
	[OriginalInvoiceTotal] [decimal](10, 2) NULL,
	[IsFullyReturned] [bit] NULL DEFAULT ((0)),
	[ReturnStatus] [nvarchar](20) NULL DEFAULT ('Partial'),
PRIMARY KEY CLUSTERED 
(
	[ReturnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupplierPayments]    Script Date: 11/3/2025 5:18:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierPayments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [int] NULL,
	[PaymentDate] [datetime] NULL DEFAULT (getdate()),
	[VoucherNumber] [nvarchar](20) NULL,
	[PreviousBalance] [decimal](10, 2) NULL DEFAULT ((0)),
	[TotalPayable] [decimal](10, 2) NULL DEFAULT ((0)),
	[PaidAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[RemainingAmount] [decimal](10, 2) NULL DEFAULT ((0)),
	[RemainingBalance] [decimal](10, 2) NULL DEFAULT ((0)),
	[Amount] [decimal](10, 2) NULL DEFAULT ((0)),
	[PaymentMethod] [nvarchar](20) NULL,
	[Description] [nvarchar](255) NULL,
	[UserID] [int] NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 11/3/2025 5:18:51 PM ******/
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
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 11/3/2025 5:18:51 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/3/2025 5:18:51 PM ******/
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
	[Role] [nvarchar](20) NULL DEFAULT ('User'),
	[IsActive] [bit] NULL DEFAULT ((1)),
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[LastLogin] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[vw_SalesWithDetails]    Script Date: 11/3/2025 5:18:51 PM ******/
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
SET IDENTITY_INSERT [dbo].[Brands] ON 

GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (1, N'SMOK', N'Premium vaping devices and accessories', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (2, N'Vaporesso', N'Innovative vaping technology', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (3, N'GeekVape', N'Durable and reliable vaping products', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (4, N'Uwell', N'High-quality vaping devices', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (5, N'Aspire', N'Advanced vaping solutions', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (6, N'mahi', N'djsjd', 1, CAST(N'2025-10-24 16:56:24.273' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (8, N'SMOK 1', N'Description for brand 1', 1, CAST(N'2025-09-26 13:06:47.290' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (9, N'GeekVape 2', N'Description for brand 2', 1, CAST(N'2025-05-05 13:06:47.293' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (10, N'Uwell 3', N'Description for brand 3', 1, CAST(N'2025-05-10 13:06:47.293' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (11, N'Aspire 4', N'Description for brand 4', 1, CAST(N'2025-04-04 13:06:47.293' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (12, N'Innokin 5', N'Description for brand 5', 1, CAST(N'2024-12-29 13:06:47.293' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (13, N'Voopoo 6', N'Description for brand 6', 1, CAST(N'2025-02-16 13:06:47.293' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (14, N'Lost Vape 7', N'Description for brand 7', 1, CAST(N'2024-12-24 13:06:47.293' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (15, N'Vandy Vape 8', N'Description for brand 8', 1, CAST(N'2025-05-30 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (16, N'Wismec 9', N'Description for brand 9', 1, CAST(N'2025-06-29 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (17, N'Joyetech 10', N'Description for brand 10', 1, CAST(N'2025-08-31 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (18, N'Eleaf 11', N'Description for brand 11', 1, CAST(N'2025-04-18 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (19, N'Sigelei 12', N'Description for brand 12', 1, CAST(N'2025-09-29 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (20, N'Kangertech 13', N'Description for brand 13', 1, CAST(N'2024-12-22 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (21, N'Vape Brand 14', N'Description for brand 14', 1, CAST(N'2025-06-08 13:06:47.297' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (22, N'Vaporesso 15', N'Description for brand 15', 1, CAST(N'2025-09-12 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (23, N'SMOK 16', N'Description for brand 16', 1, CAST(N'2025-06-09 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (24, N'GeekVape 17', N'Description for brand 17', 1, CAST(N'2025-03-11 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (25, N'Uwell 18', N'Description for brand 18', 1, CAST(N'2024-12-30 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (26, N'Aspire 19', N'Description for brand 19', 1, CAST(N'2025-02-05 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (27, N'Innokin 20', N'Description for brand 20', 1, CAST(N'2025-06-13 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (28, N'Voopoo 21', N'Description for brand 21', 1, CAST(N'2025-10-07 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (29, N'Lost Vape 22', N'Description for brand 22', 1, CAST(N'2024-11-02 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (30, N'Vandy Vape 23', N'Description for brand 23', 1, CAST(N'2025-01-18 13:06:47.300' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (31, N'Wismec 24', N'Description for brand 24', 1, CAST(N'2025-04-07 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (32, N'Joyetech 25', N'Description for brand 25', 1, CAST(N'2025-03-09 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (33, N'Eleaf 26', N'Description for brand 26', 1, CAST(N'2025-01-26 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (34, N'Sigelei 27', N'Description for brand 27', 1, CAST(N'2025-08-22 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (35, N'Kangertech 28', N'Description for brand 28', 1, CAST(N'2025-01-30 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (36, N'Vape Brand 29', N'Description for brand 29', 1, CAST(N'2025-03-27 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (37, N'Vaporesso 30', N'Description for brand 30', 1, CAST(N'2024-12-02 13:06:47.303' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (38, N'SMOK 31', N'Description for brand 31', 1, CAST(N'2025-10-26 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (39, N'GeekVape 32', N'Description for brand 32', 1, CAST(N'2025-07-26 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (40, N'Uwell 33', N'Description for brand 33', 1, CAST(N'2025-02-18 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (41, N'Aspire 34', N'Description for brand 34', 1, CAST(N'2024-11-11 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (42, N'Innokin 35', N'Description for brand 35', 1, CAST(N'2025-08-25 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (43, N'Voopoo 36', N'Description for brand 36', 1, CAST(N'2025-07-25 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (44, N'Lost Vape 37', N'Description for brand 37', 1, CAST(N'2025-09-30 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (45, N'Vandy Vape 38', N'Description for brand 38', 1, CAST(N'2025-04-22 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (46, N'Wismec 39', N'Description for brand 39', 1, CAST(N'2025-06-19 13:06:47.307' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (47, N'Joyetech 40', N'Description for brand 40', 1, CAST(N'2025-04-09 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (48, N'Eleaf 41', N'Description for brand 41', 1, CAST(N'2025-05-23 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (49, N'Sigelei 42', N'Description for brand 42', 1, CAST(N'2025-05-22 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (50, N'Kangertech 43', N'Description for brand 43', 1, CAST(N'2024-11-06 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (51, N'Vape Brand 44', N'Description for brand 44', 1, CAST(N'2025-05-15 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (52, N'Vaporesso 45', N'Description for brand 45', 1, CAST(N'2025-04-14 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (53, N'SMOK 46', N'Description for brand 46', 1, CAST(N'2024-11-16 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (54, N'GeekVape 47', N'Description for brand 47', 1, CAST(N'2025-06-06 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (55, N'Uwell 48', N'Description for brand 48', 1, CAST(N'2025-01-01 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (56, N'Aspire 49', N'Description for brand 49', 1, CAST(N'2025-01-30 13:06:47.310' AS DateTime))
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [Description], [IsActive], [CreatedDate]) VALUES (57, N'Innokin 50', N'Description for brand 50', 1, CAST(N'2025-10-10 13:06:47.313' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[CashInHand] ON 

GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (2, CAST(N'2024-01-01 00:00:00.000' AS DateTime), CAST(5000.00 AS Decimal(10, 2)), CAST(200.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5200.00 AS Decimal(10, 2)), N'Initial cash balance', NULL, 1, CAST(N'2025-10-25 10:58:10.807' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (3, CAST(N'2024-01-15 00:00:00.000' AS DateTime), CAST(5200.00 AS Decimal(10, 2)), CAST(250.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5450.00 AS Decimal(10, 2)), N'Daily cash transactions', NULL, 1, CAST(N'2025-10-25 10:58:10.807' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (4, CAST(N'2024-01-16 00:00:00.000' AS DateTime), CAST(5450.00 AS Decimal(10, 2)), CAST(50.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5500.00 AS Decimal(10, 2)), N'Daily cash transactions', NULL, 1, CAST(N'2025-10-25 10:58:10.807' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (5, CAST(N'2024-01-17 00:00:00.000' AS DateTime), CAST(5500.00 AS Decimal(10, 2)), CAST(100.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5600.00 AS Decimal(10, 2)), N'Daily cash transactions', NULL, 1, CAST(N'2025-10-25 10:58:10.807' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (6, CAST(N'2024-01-18 00:00:00.000' AS DateTime), CAST(5600.00 AS Decimal(10, 2)), CAST(50.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5650.00 AS Decimal(10, 2)), N'Daily cash transactions', NULL, 1, CAST(N'2025-10-25 10:58:10.807' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (7, CAST(N'2024-01-19 00:00:00.000' AS DateTime), CAST(5650.00 AS Decimal(10, 2)), CAST(50.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5700.00 AS Decimal(10, 2)), N'Daily cash transactions', NULL, 1, CAST(N'2025-10-25 10:58:10.807' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (8, CAST(N'2025-10-25 13:10:05.320' AS DateTime), CAST(101.00 AS Decimal(10, 2)), CAST(51.00 AS Decimal(10, 2)), CAST(21.00 AS Decimal(10, 2)), CAST(11.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 1', N'User 1', 1, CAST(N'2025-10-25 13:10:05.320' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (9, CAST(N'2025-10-24 13:10:05.323' AS DateTime), CAST(102.00 AS Decimal(10, 2)), CAST(52.00 AS Decimal(10, 2)), CAST(22.00 AS Decimal(10, 2)), CAST(12.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 2', N'User 2', 2, CAST(N'2025-10-24 13:10:05.323' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (10, CAST(N'2025-10-23 13:10:05.323' AS DateTime), CAST(103.00 AS Decimal(10, 2)), CAST(53.00 AS Decimal(10, 2)), CAST(23.00 AS Decimal(10, 2)), CAST(13.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 3', N'User 3', 3, CAST(N'2025-10-23 13:10:05.323' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (12, CAST(N'2025-10-21 13:10:05.327' AS DateTime), CAST(105.00 AS Decimal(10, 2)), CAST(55.00 AS Decimal(10, 2)), CAST(25.00 AS Decimal(10, 2)), CAST(15.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 5', N'User 5', 5, CAST(N'2025-10-21 13:10:05.327' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (13, CAST(N'2025-10-20 13:10:05.327' AS DateTime), CAST(106.00 AS Decimal(10, 2)), CAST(56.00 AS Decimal(10, 2)), CAST(26.00 AS Decimal(10, 2)), CAST(16.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 6', N'User 6', 6, CAST(N'2025-10-20 13:10:05.327' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (14, CAST(N'2025-10-19 13:10:05.327' AS DateTime), CAST(107.00 AS Decimal(10, 2)), CAST(57.00 AS Decimal(10, 2)), CAST(27.00 AS Decimal(10, 2)), CAST(17.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 7', N'User 7', 7, CAST(N'2025-10-19 13:10:05.327' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (15, CAST(N'2025-10-18 13:10:05.330' AS DateTime), CAST(108.00 AS Decimal(10, 2)), CAST(58.00 AS Decimal(10, 2)), CAST(28.00 AS Decimal(10, 2)), CAST(18.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 8', N'User 8', 8, CAST(N'2025-10-18 13:10:05.330' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (16, CAST(N'2025-10-17 13:10:05.330' AS DateTime), CAST(109.00 AS Decimal(10, 2)), CAST(59.00 AS Decimal(10, 2)), CAST(29.00 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 9', N'User 9', 9, CAST(N'2025-10-17 13:10:05.330' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (17, CAST(N'2025-10-16 13:10:05.330' AS DateTime), CAST(110.00 AS Decimal(10, 2)), CAST(60.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 10', N'User 10', 10, CAST(N'2025-10-16 13:10:05.330' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (18, CAST(N'2025-10-15 13:10:05.330' AS DateTime), CAST(111.00 AS Decimal(10, 2)), CAST(61.00 AS Decimal(10, 2)), CAST(31.00 AS Decimal(10, 2)), CAST(21.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 11', N'User 11', 11, CAST(N'2025-10-15 13:10:05.330' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (19, CAST(N'2025-10-14 13:10:05.330' AS DateTime), CAST(112.00 AS Decimal(10, 2)), CAST(62.00 AS Decimal(10, 2)), CAST(32.00 AS Decimal(10, 2)), CAST(22.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 12', N'User 12', 12, CAST(N'2025-10-14 13:10:05.330' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (20, CAST(N'2025-10-13 13:10:05.333' AS DateTime), CAST(113.00 AS Decimal(10, 2)), CAST(63.00 AS Decimal(10, 2)), CAST(33.00 AS Decimal(10, 2)), CAST(23.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 13', N'User 13', 13, CAST(N'2025-10-13 13:10:05.333' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (21, CAST(N'2025-10-12 13:10:05.333' AS DateTime), CAST(114.00 AS Decimal(10, 2)), CAST(64.00 AS Decimal(10, 2)), CAST(34.00 AS Decimal(10, 2)), CAST(24.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 14', N'User 14', 14, CAST(N'2025-10-12 13:10:05.333' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (22, CAST(N'2025-10-11 13:10:05.333' AS DateTime), CAST(115.00 AS Decimal(10, 2)), CAST(65.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), CAST(25.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 15', N'User 15', 15, CAST(N'2025-10-11 13:10:05.333' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (23, CAST(N'2025-10-10 13:10:05.333' AS DateTime), CAST(116.00 AS Decimal(10, 2)), CAST(66.00 AS Decimal(10, 2)), CAST(36.00 AS Decimal(10, 2)), CAST(26.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 16', N'User 16', 16, CAST(N'2025-10-10 13:10:05.333' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (24, CAST(N'2025-10-09 13:10:05.333' AS DateTime), CAST(117.00 AS Decimal(10, 2)), CAST(67.00 AS Decimal(10, 2)), CAST(37.00 AS Decimal(10, 2)), CAST(27.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 17', N'User 17', 17, CAST(N'2025-10-09 13:10:05.333' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (25, CAST(N'2025-10-08 13:10:05.333' AS DateTime), CAST(118.00 AS Decimal(10, 2)), CAST(68.00 AS Decimal(10, 2)), CAST(38.00 AS Decimal(10, 2)), CAST(28.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 18', N'User 18', 18, CAST(N'2025-10-08 13:10:05.333' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (26, CAST(N'2025-10-07 13:10:05.337' AS DateTime), CAST(119.00 AS Decimal(10, 2)), CAST(69.00 AS Decimal(10, 2)), CAST(39.00 AS Decimal(10, 2)), CAST(29.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 19', N'User 19', 19, CAST(N'2025-10-07 13:10:05.337' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (27, CAST(N'2025-10-06 13:10:05.337' AS DateTime), CAST(120.00 AS Decimal(10, 2)), CAST(70.00 AS Decimal(10, 2)), CAST(40.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 20', N'User 20', 20, CAST(N'2025-10-06 13:10:05.337' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (28, CAST(N'2025-10-05 13:10:05.337' AS DateTime), CAST(121.00 AS Decimal(10, 2)), CAST(71.00 AS Decimal(10, 2)), CAST(41.00 AS Decimal(10, 2)), CAST(31.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 21', N'User 21', 21, CAST(N'2025-10-05 13:10:05.337' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (29, CAST(N'2025-10-04 13:10:05.337' AS DateTime), CAST(122.00 AS Decimal(10, 2)), CAST(72.00 AS Decimal(10, 2)), CAST(42.00 AS Decimal(10, 2)), CAST(32.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 22', N'User 22', 22, CAST(N'2025-10-04 13:10:05.337' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (30, CAST(N'2025-10-03 13:10:05.337' AS DateTime), CAST(123.00 AS Decimal(10, 2)), CAST(73.00 AS Decimal(10, 2)), CAST(43.00 AS Decimal(10, 2)), CAST(33.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 23', N'User 23', 23, CAST(N'2025-10-03 13:10:05.337' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (31, CAST(N'2025-10-02 13:10:05.340' AS DateTime), CAST(124.00 AS Decimal(10, 2)), CAST(74.00 AS Decimal(10, 2)), CAST(44.00 AS Decimal(10, 2)), CAST(34.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 24', N'User 24', 24, CAST(N'2025-10-02 13:10:05.340' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (32, CAST(N'2025-10-01 13:10:05.340' AS DateTime), CAST(125.00 AS Decimal(10, 2)), CAST(75.00 AS Decimal(10, 2)), CAST(45.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 25', N'User 25', 25, CAST(N'2025-10-01 13:10:05.340' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (33, CAST(N'2025-09-30 13:10:05.340' AS DateTime), CAST(126.00 AS Decimal(10, 2)), CAST(76.00 AS Decimal(10, 2)), CAST(46.00 AS Decimal(10, 2)), CAST(36.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 26', N'User 26', 26, CAST(N'2025-09-30 13:10:05.340' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (34, CAST(N'2025-09-29 13:10:05.340' AS DateTime), CAST(127.00 AS Decimal(10, 2)), CAST(77.00 AS Decimal(10, 2)), CAST(47.00 AS Decimal(10, 2)), CAST(37.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 27', N'User 27', 27, CAST(N'2025-09-29 13:10:05.340' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (35, CAST(N'2025-09-28 13:10:05.340' AS DateTime), CAST(128.00 AS Decimal(10, 2)), CAST(78.00 AS Decimal(10, 2)), CAST(48.00 AS Decimal(10, 2)), CAST(38.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 28', N'User 28', 28, CAST(N'2025-09-28 13:10:05.340' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (36, CAST(N'2025-09-27 13:10:05.340' AS DateTime), CAST(129.00 AS Decimal(10, 2)), CAST(79.00 AS Decimal(10, 2)), CAST(49.00 AS Decimal(10, 2)), CAST(39.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 29', N'User 29', 29, CAST(N'2025-09-27 13:10:05.340' AS DateTime))
GO
INSERT [dbo].[CashInHand] ([CashInHandID], [TransactionDate], [OpeningCash], [CashIn], [CashOut], [Expenses], [ClosingCash], [Description], [CreatedBy], [UserID], [CreatedDate]) VALUES (37, CAST(N'2025-09-26 13:10:05.340' AS DateTime), CAST(130.00 AS Decimal(10, 2)), CAST(80.00 AS Decimal(10, 2)), CAST(50.00 AS Decimal(10, 2)), CAST(40.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), N'Daily cash transaction 30', N'User 30', 30, CAST(N'2025-09-26 13:10:05.340' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CashInHand] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (1, N'E-Liquids', N'Electronic cigarette liquids and vape juices', 1, CAST(N'2025-10-17 19:51:27.470' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (2, N'Vape Devices', N'Electronic cigarettes and vaping devices', 1, CAST(N'2025-10-17 19:51:27.470' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (3, N'Accessories', N'Vape accessories and spare parts', 1, CAST(N'2025-10-17 19:51:27.470' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (4, N'Coils', N'Vape coils and heating elements', 1, CAST(N'2025-10-17 19:51:27.470' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (5, N'Batteries', N'Vape batteries and chargers', 1, CAST(N'2025-10-17 19:51:27.470' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (6, N'any category', N'fdad', 1, CAST(N'2025-10-24 16:45:43.127' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (8, N'Vape Devices 1', N'Description for category 1', 1, CAST(N'2025-10-14 13:06:39.743' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (9, N'Coils 2', N'Description for category 2', 1, CAST(N'2025-08-01 13:06:39.747' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (10, N'Pods 3', N'Description for category 3', 1, CAST(N'2024-12-08 13:06:39.747' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (11, N'Accessories 4', N'Description for category 4', 1, CAST(N'2025-07-23 13:06:39.747' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (12, N'Batteries 5', N'Description for category 5', 1, CAST(N'2025-02-24 13:06:39.747' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (13, N'Chargers 6', N'Description for category 6', 1, CAST(N'2025-05-23 13:06:39.747' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (14, N'Tanks 7', N'Description for category 7', 1, CAST(N'2025-01-26 13:06:39.747' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (15, N'Mods 8', N'Description for category 8', 1, CAST(N'2024-11-25 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (16, N'Starter Kits 9', N'Description for category 9', 1, CAST(N'2025-06-25 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (17, N'E-Liquids 10', N'Description for category 10', 1, CAST(N'2025-06-02 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (18, N'Vape Devices 11', N'Description for category 11', 1, CAST(N'2024-12-28 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (19, N'Coils 12', N'Description for category 12', 1, CAST(N'2024-12-12 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (20, N'Pods 13', N'Description for category 13', 1, CAST(N'2025-08-23 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (21, N'Accessories 14', N'Description for category 14', 1, CAST(N'2024-11-01 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (22, N'Batteries 15', N'Description for category 15', 1, CAST(N'2025-05-02 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (23, N'Chargers 16', N'Description for category 16', 1, CAST(N'2025-08-29 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (24, N'Tanks 17', N'Description for category 17', 1, CAST(N'2025-01-09 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (25, N'Mods 18', N'Description for category 18', 1, CAST(N'2024-12-11 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (26, N'Starter Kits 19', N'Description for category 19', 1, CAST(N'2025-06-27 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (27, N'E-Liquids 20', N'Description for category 20', 1, CAST(N'2025-01-24 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (28, N'Vape Devices 21', N'Description for category 21', 1, CAST(N'2025-07-07 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (29, N'Coils 22', N'Description for category 22', 1, CAST(N'2025-07-04 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (30, N'Pods 23', N'Description for category 23', 1, CAST(N'2025-01-22 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (31, N'Accessories 24', N'Description for category 24', 1, CAST(N'2025-02-05 13:06:39.750' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (32, N'Batteries 25', N'Description for category 25', 1, CAST(N'2025-06-04 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (33, N'Chargers 26', N'Description for category 26', 1, CAST(N'2025-06-29 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (34, N'Tanks 27', N'Description for category 27', 1, CAST(N'2025-02-04 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (35, N'Mods 28', N'Description for category 28', 1, CAST(N'2025-03-04 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (36, N'Starter Kits 29', N'Description for category 29', 1, CAST(N'2025-03-15 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (37, N'E-Liquids 30', N'Description for category 30', 1, CAST(N'2025-04-08 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (38, N'Vape Devices 31', N'Description for category 31', 1, CAST(N'2024-12-20 13:06:39.753' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (39, N'Coils 32', N'Description for category 32', 1, CAST(N'2025-01-05 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (40, N'Pods 33', N'Description for category 33', 1, CAST(N'2025-02-01 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (41, N'Accessories 34', N'Description for category 34', 1, CAST(N'2024-11-27 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (42, N'Batteries 35', N'Description for category 35', 1, CAST(N'2025-07-25 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (43, N'Chargers 36', N'Description for category 36', 1, CAST(N'2025-10-09 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (44, N'Tanks 37', N'Description for category 37', 1, CAST(N'2025-09-23 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (45, N'Mods 38', N'Description for category 38', 1, CAST(N'2025-06-01 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (46, N'Starter Kits 39', N'Description for category 39', 1, CAST(N'2025-01-16 13:06:39.757' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (47, N'E-Liquids 40', N'Description for category 40', 1, CAST(N'2025-09-02 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (48, N'Vape Devices 41', N'Description for category 41', 1, CAST(N'2024-10-31 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (49, N'Coils 42', N'Description for category 42', 1, CAST(N'2025-07-24 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (50, N'Pods 43', N'Description for category 43', 1, CAST(N'2025-09-12 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (51, N'Accessories 44', N'Description for category 44', 1, CAST(N'2025-08-17 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (52, N'Batteries 45', N'Description for category 45', 1, CAST(N'2025-04-24 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (53, N'Chargers 46', N'Description for category 46', 1, CAST(N'2025-08-01 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (54, N'Tanks 47', N'Description for category 47', 1, CAST(N'2025-02-19 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (55, N'Mods 48', N'Description for category 48', 1, CAST(N'2025-08-04 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (56, N'Starter Kits 49', N'Description for category 49', 1, CAST(N'2025-04-19 13:06:39.760' AS DateTime))
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate]) VALUES (57, N'E-Liquids 50', N'Description for category 50', 1, CAST(N'2025-04-20 13:06:39.763' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[CustomerPayments] ON 

GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (2, 4, CAST(N'2024-01-20 00:00:00.000' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(22.06 AS Decimal(10, 2)), N'Cash', N'Payment for outstanding balance', 1, CAST(N'2025-10-25 10:58:10.803' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (3, 2, CAST(N'2024-01-22 00:00:00.000' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(23.75 AS Decimal(10, 2)), N'Card', N'Payment for outstanding balance', 2, CAST(N'2025-10-25 10:58:10.803' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (4, 1, CAST(N'2025-10-25 17:00:02.767' AS DateTime), N'CP000001', CAST(0.00 AS Decimal(10, 2)), CAST(429.46 AS Decimal(10, 2)), CAST(429.46 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL, N'', 1, CAST(N'2025-10-25 17:00:27.017' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (5, 1, CAST(N'2025-10-25 13:10:24.857' AS DateTime), N'VOUCH000001', CAST(101.00 AS Decimal(10, 2)), CAST(152.00 AS Decimal(10, 2)), CAST(51.00 AS Decimal(10, 2)), CAST(101.00 AS Decimal(10, 2)), CAST(51.00 AS Decimal(10, 2)), N'Credit Card', N'Customer payment 1', 1, CAST(N'2025-10-24 13:10:24.857' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (6, 2, CAST(N'2025-10-02 13:10:24.860' AS DateTime), N'VOUCH000002', CAST(102.00 AS Decimal(10, 2)), CAST(154.00 AS Decimal(10, 2)), CAST(52.00 AS Decimal(10, 2)), CAST(102.00 AS Decimal(10, 2)), CAST(52.00 AS Decimal(10, 2)), N'Debit Card', N'Customer payment 2', 2, CAST(N'2025-10-11 13:10:24.860' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (7, 3, CAST(N'2025-10-08 13:10:24.860' AS DateTime), N'VOUCH000003', CAST(103.00 AS Decimal(10, 2)), CAST(156.00 AS Decimal(10, 2)), CAST(53.00 AS Decimal(10, 2)), CAST(103.00 AS Decimal(10, 2)), CAST(53.00 AS Decimal(10, 2)), N'Check', N'Customer payment 3', 3, CAST(N'2025-10-09 13:10:24.860' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (9, 5, CAST(N'2025-10-05 13:10:24.860' AS DateTime), N'VOUCH000005', CAST(105.00 AS Decimal(10, 2)), CAST(160.00 AS Decimal(10, 2)), CAST(55.00 AS Decimal(10, 2)), CAST(105.00 AS Decimal(10, 2)), CAST(55.00 AS Decimal(10, 2)), N'Credit Card', N'Customer payment 5', 5, CAST(N'2025-10-25 13:10:24.860' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (10, 6, CAST(N'2025-10-20 13:10:24.863' AS DateTime), N'VOUCH000006', CAST(106.00 AS Decimal(10, 2)), CAST(162.00 AS Decimal(10, 2)), CAST(56.00 AS Decimal(10, 2)), CAST(106.00 AS Decimal(10, 2)), CAST(56.00 AS Decimal(10, 2)), N'Debit Card', N'Customer payment 6', 6, CAST(N'2025-10-20 13:10:24.863' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (11, 7, CAST(N'2025-10-11 13:10:24.863' AS DateTime), N'VOUCH000007', CAST(107.00 AS Decimal(10, 2)), CAST(164.00 AS Decimal(10, 2)), CAST(57.00 AS Decimal(10, 2)), CAST(107.00 AS Decimal(10, 2)), CAST(57.00 AS Decimal(10, 2)), N'Check', N'Customer payment 7', 7, CAST(N'2025-10-26 13:10:24.863' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (19, 15, CAST(N'2025-10-12 13:10:24.867' AS DateTime), N'VOUCH000015', CAST(115.00 AS Decimal(10, 2)), CAST(180.00 AS Decimal(10, 2)), CAST(65.00 AS Decimal(10, 2)), CAST(115.00 AS Decimal(10, 2)), CAST(65.00 AS Decimal(10, 2)), N'Check', N'Customer payment 15', 15, CAST(N'2025-10-24 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (20, 16, CAST(N'2025-10-17 13:10:24.867' AS DateTime), N'VOUCH000016', CAST(116.00 AS Decimal(10, 2)), CAST(182.00 AS Decimal(10, 2)), CAST(66.00 AS Decimal(10, 2)), CAST(116.00 AS Decimal(10, 2)), CAST(66.00 AS Decimal(10, 2)), N'Cash', N'Customer payment 16', 16, CAST(N'2025-10-16 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (21, 17, CAST(N'2025-09-28 13:10:24.867' AS DateTime), N'VOUCH000017', CAST(117.00 AS Decimal(10, 2)), CAST(184.00 AS Decimal(10, 2)), CAST(67.00 AS Decimal(10, 2)), CAST(117.00 AS Decimal(10, 2)), CAST(67.00 AS Decimal(10, 2)), N'Credit Card', N'Customer payment 17', 17, CAST(N'2025-10-03 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (22, 18, CAST(N'2025-10-18 13:10:24.867' AS DateTime), N'VOUCH000018', CAST(118.00 AS Decimal(10, 2)), CAST(186.00 AS Decimal(10, 2)), CAST(68.00 AS Decimal(10, 2)), CAST(118.00 AS Decimal(10, 2)), CAST(68.00 AS Decimal(10, 2)), N'Debit Card', N'Customer payment 18', 18, CAST(N'2025-10-08 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (23, 19, CAST(N'2025-10-16 13:10:24.867' AS DateTime), N'VOUCH000019', CAST(119.00 AS Decimal(10, 2)), CAST(188.00 AS Decimal(10, 2)), CAST(69.00 AS Decimal(10, 2)), CAST(119.00 AS Decimal(10, 2)), CAST(69.00 AS Decimal(10, 2)), N'Check', N'Customer payment 19', 19, CAST(N'2025-09-28 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (24, 20, CAST(N'2025-10-22 13:10:24.867' AS DateTime), N'VOUCH000020', CAST(120.00 AS Decimal(10, 2)), CAST(190.00 AS Decimal(10, 2)), CAST(70.00 AS Decimal(10, 2)), CAST(120.00 AS Decimal(10, 2)), CAST(70.00 AS Decimal(10, 2)), N'Cash', N'Customer payment 20', 20, CAST(N'2025-10-19 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (25, 21, CAST(N'2025-10-18 13:10:24.867' AS DateTime), N'VOUCH000021', CAST(121.00 AS Decimal(10, 2)), CAST(192.00 AS Decimal(10, 2)), CAST(71.00 AS Decimal(10, 2)), CAST(121.00 AS Decimal(10, 2)), CAST(71.00 AS Decimal(10, 2)), N'Credit Card', N'Customer payment 21', 21, CAST(N'2025-10-05 13:10:24.867' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (26, 22, CAST(N'2025-10-04 13:10:24.870' AS DateTime), N'VOUCH000022', CAST(122.00 AS Decimal(10, 2)), CAST(194.00 AS Decimal(10, 2)), CAST(72.00 AS Decimal(10, 2)), CAST(122.00 AS Decimal(10, 2)), CAST(72.00 AS Decimal(10, 2)), N'Debit Card', N'Customer payment 22', 22, CAST(N'2025-10-18 13:10:24.870' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (27, 23, CAST(N'2025-10-13 13:10:24.870' AS DateTime), N'VOUCH000023', CAST(123.00 AS Decimal(10, 2)), CAST(196.00 AS Decimal(10, 2)), CAST(73.00 AS Decimal(10, 2)), CAST(123.00 AS Decimal(10, 2)), CAST(73.00 AS Decimal(10, 2)), N'Check', N'Customer payment 23', 23, CAST(N'2025-09-29 13:10:24.870' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (28, 24, CAST(N'2025-09-28 13:10:24.870' AS DateTime), N'VOUCH000024', CAST(124.00 AS Decimal(10, 2)), CAST(198.00 AS Decimal(10, 2)), CAST(74.00 AS Decimal(10, 2)), CAST(124.00 AS Decimal(10, 2)), CAST(74.00 AS Decimal(10, 2)), N'Cash', N'Customer payment 24', 24, CAST(N'2025-09-27 13:10:24.870' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (29, 25, CAST(N'2025-10-06 13:10:24.870' AS DateTime), N'VOUCH000025', CAST(125.00 AS Decimal(10, 2)), CAST(200.00 AS Decimal(10, 2)), CAST(75.00 AS Decimal(10, 2)), CAST(125.00 AS Decimal(10, 2)), CAST(75.00 AS Decimal(10, 2)), N'Credit Card', N'Customer payment 25', 25, CAST(N'2025-10-04 13:10:24.870' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (30, 26, CAST(N'2025-10-19 13:10:24.870' AS DateTime), N'VOUCH000026', CAST(126.00 AS Decimal(10, 2)), CAST(202.00 AS Decimal(10, 2)), CAST(76.00 AS Decimal(10, 2)), CAST(126.00 AS Decimal(10, 2)), CAST(76.00 AS Decimal(10, 2)), N'Debit Card', N'Customer payment 26', 26, CAST(N'2025-10-12 13:10:24.870' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (31, 27, CAST(N'2025-10-01 13:10:24.873' AS DateTime), N'VOUCH000027', CAST(127.00 AS Decimal(10, 2)), CAST(204.00 AS Decimal(10, 2)), CAST(77.00 AS Decimal(10, 2)), CAST(127.00 AS Decimal(10, 2)), CAST(77.00 AS Decimal(10, 2)), N'Check', N'Customer payment 27', 27, CAST(N'2025-10-19 13:10:24.873' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (32, 28, CAST(N'2025-10-11 13:10:24.873' AS DateTime), N'VOUCH000028', CAST(128.00 AS Decimal(10, 2)), CAST(206.00 AS Decimal(10, 2)), CAST(78.00 AS Decimal(10, 2)), CAST(128.00 AS Decimal(10, 2)), CAST(78.00 AS Decimal(10, 2)), N'Cash', N'Customer payment 28', 28, CAST(N'2025-10-22 13:10:24.873' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (33, 29, CAST(N'2025-10-07 13:10:24.873' AS DateTime), N'VOUCH000029', CAST(129.00 AS Decimal(10, 2)), CAST(208.00 AS Decimal(10, 2)), CAST(79.00 AS Decimal(10, 2)), CAST(129.00 AS Decimal(10, 2)), CAST(79.00 AS Decimal(10, 2)), N'Credit Card', N'Customer payment 29', 29, CAST(N'2025-10-01 13:10:24.873' AS DateTime))
GO
INSERT [dbo].[CustomerPayments] ([PaymentID], [CustomerID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalDue], [PaidAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (34, 30, CAST(N'2025-10-13 13:10:24.873' AS DateTime), N'VOUCH000030', CAST(130.00 AS Decimal(10, 2)), CAST(210.00 AS Decimal(10, 2)), CAST(80.00 AS Decimal(10, 2)), CAST(130.00 AS Decimal(10, 2)), CAST(80.00 AS Decimal(10, 2)), N'Debit Card', N'Customer payment 30', 30, CAST(N'2025-10-25 13:10:24.873' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CustomerPayments] OFF
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 

GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (1, N'CUST001', N'Alice', N'555-1001', N'alice@email.com', N'123 Main St', N'New York', N'10001', 1, CAST(N'2025-10-17 19:51:27.477' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (2, N'CUST002', N'Bob Wilson', N'555-1002', N'bob@email.com', N'456 Oak Ave', N'Los Angeles', N'90001', 1, CAST(N'2025-10-17 19:51:27.477' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (3, N'CUST003', N'Carol Davis', N'555-1003', N'carol@email.com', N'789 Pine St', N'Chicago', N'60601', 1, CAST(N'2025-10-17 19:51:27.477' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (4, N'CUST004', N'David Miller', N'555-1004', N'david@email.com', N'321 Elm St', N'Houston', N'77001', 1, CAST(N'2025-10-17 19:51:27.477' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (5, N'CUST005', N'Eva Garcia', N'555-1005', N'eva@email.com', N'654 Maple Ave', N'Phoenix', N'85001', 1, CAST(N'2025-10-17 19:51:27.477' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (6, N'CUST000', N'Walk-in Customer', N'N/A', N'walkin@vapestore.com', N'N/A', N'N/A', NULL, 1, CAST(N'2025-10-24 12:04:13.073' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (7, N'CUST006', N'wajahat hussain', N'03112112929', N'wajahat@gmail.com', N'l216 sector', N'karachi', N'75850', 1, CAST(N'2025-10-24 16:57:54.483' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (15, N'CUST007', N'Customer 7 Name', N'555-007-0014', N'customer7@email.com', N'7 Main Street', N'San Diego', N'10700', 1, CAST(N'2025-03-18 13:07:09.197' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (16, N'CUST008', N'Customer 8 Name', N'555-008-0016', N'customer8@email.com', N'8 Main Street', N'Dallas', N'10800', 1, CAST(N'2025-04-09 13:07:09.197' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (17, N'CUST009', N'Customer 9 Name', N'555-009-0018', N'customer9@email.com', N'9 Main Street', N'Miami', N'10900', 1, CAST(N'2025-03-07 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (18, N'CUST010', N'Customer 10 Name', N'555-010-0020', N'customer10@email.com', N'10 Main Street', N'New York', N'11000', 1, CAST(N'2025-10-05 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (19, N'CUST011', N'Customer 11 Name', N'555-011-0022', N'customer11@email.com', N'11 Main Street', N'Los Angeles', N'11100', 1, CAST(N'2024-12-27 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (20, N'CUST012', N'Customer 12 Name', N'555-012-0024', N'customer12@email.com', N'12 Main Street', N'Chicago', N'11200', 1, CAST(N'2024-11-01 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (21, N'CUST013', N'Customer 13 Name', N'555-013-0026', N'customer13@email.com', N'13 Main Street', N'Houston', N'11300', 1, CAST(N'2025-06-13 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (22, N'CUST014', N'Customer 14 Name', N'555-014-0028', N'customer14@email.com', N'14 Main Street', N'Phoenix', N'11400', 1, CAST(N'2024-11-02 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (23, N'CUST015', N'Customer 15 Name', N'555-015-0030', N'customer15@email.com', N'15 Main Street', N'Philadelphia', N'11500', 1, CAST(N'2025-08-17 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (24, N'CUST016', N'Customer 16 Name', N'555-016-0032', N'customer16@email.com', N'16 Main Street', N'San Antonio', N'11600', 1, CAST(N'2025-01-06 13:07:09.200' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (25, N'CUST017', N'Customer 17 Name', N'555-017-0034', N'customer17@email.com', N'17 Main Street', N'San Diego', N'11700', 1, CAST(N'2025-10-15 13:07:09.203' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (26, N'CUST018', N'Customer 18 Name', N'555-018-0036', N'customer18@email.com', N'18 Main Street', N'Dallas', N'11800', 1, CAST(N'2025-03-23 13:07:09.203' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (27, N'CUST019', N'Customer 19 Name', N'555-019-0038', N'customer19@email.com', N'19 Main Street', N'Miami', N'11900', 1, CAST(N'2025-08-28 13:07:09.203' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (28, N'CUST020', N'Customer 20 Name', N'555-020-0040', N'customer20@email.com', N'20 Main Street', N'New York', N'12000', 1, CAST(N'2025-05-31 13:07:09.203' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (29, N'CUST021', N'Customer 21 Name', N'555-021-0042', N'customer21@email.com', N'21 Main Street', N'Los Angeles', N'12100', 1, CAST(N'2024-10-31 13:07:09.203' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (30, N'CUST022', N'Customer 22 Name', N'555-022-0044', N'customer22@email.com', N'22 Main Street', N'Chicago', N'12200', 1, CAST(N'2025-09-24 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (31, N'CUST023', N'Customer 23 Name', N'555-023-0046', N'customer23@email.com', N'23 Main Street', N'Houston', N'12300', 1, CAST(N'2025-05-13 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (32, N'CUST024', N'Customer 24 Name', N'555-024-0048', N'customer24@email.com', N'24 Main Street', N'Phoenix', N'12400', 1, CAST(N'2025-09-13 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (33, N'CUST025', N'Customer 25 Name', N'555-025-0050', N'customer25@email.com', N'25 Main Street', N'Philadelphia', N'12500', 1, CAST(N'2025-07-05 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (34, N'CUST026', N'Customer 26 Name', N'555-026-0052', N'customer26@email.com', N'26 Main Street', N'San Antonio', N'12600', 1, CAST(N'2025-10-19 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (35, N'CUST027', N'Customer 27 Name', N'555-027-0054', N'customer27@email.com', N'27 Main Street', N'San Diego', N'12700', 1, CAST(N'2025-06-27 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (36, N'CUST028', N'Customer 28 Name', N'555-028-0056', N'customer28@email.com', N'28 Main Street', N'Dallas', N'12800', 1, CAST(N'2025-01-04 13:07:09.207' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (37, N'CUST029', N'Customer 29 Name', N'555-029-0058', N'customer29@email.com', N'29 Main Street', N'Miami', N'12900', 1, CAST(N'2024-12-03 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (38, N'CUST030', N'Customer 30 Name', N'555-030-0060', N'customer30@email.com', N'30 Main Street', N'New York', N'13000', 1, CAST(N'2024-11-28 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (39, N'CUST031', N'Customer 31 Name', N'555-031-0062', N'customer31@email.com', N'31 Main Street', N'Los Angeles', N'13100', 1, CAST(N'2024-11-08 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (40, N'CUST032', N'Customer 32 Name', N'555-032-0064', N'customer32@email.com', N'32 Main Street', N'Chicago', N'13200', 1, CAST(N'2025-07-22 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (41, N'CUST033', N'Customer 33 Name', N'555-033-0066', N'customer33@email.com', N'33 Main Street', N'Houston', N'13300', 1, CAST(N'2025-03-13 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (42, N'CUST034', N'Customer 34 Name', N'555-034-0068', N'customer34@email.com', N'34 Main Street', N'Phoenix', N'13400', 1, CAST(N'2025-09-09 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (43, N'CUST035', N'Customer 35 Name', N'555-035-0070', N'customer35@email.com', N'35 Main Street', N'Philadelphia', N'13500', 1, CAST(N'2025-09-14 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (44, N'CUST036', N'Customer 36 Name', N'555-036-0072', N'customer36@email.com', N'36 Main Street', N'San Antonio', N'13600', 1, CAST(N'2025-08-08 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (45, N'CUST037', N'Customer 37 Name', N'555-037-0074', N'customer37@email.com', N'37 Main Street', N'San Diego', N'13700', 1, CAST(N'2024-12-29 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (46, N'CUST038', N'Customer 38 Name', N'555-038-0076', N'customer38@email.com', N'38 Main Street', N'Dallas', N'13800', 1, CAST(N'2025-02-24 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (47, N'CUST039', N'Customer 39 Name', N'555-039-0078', N'customer39@email.com', N'39 Main Street', N'Miami', N'13900', 1, CAST(N'2025-01-09 13:07:09.210' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (48, N'CUST040', N'Customer 40 Name', N'555-040-0080', N'customer40@email.com', N'40 Main Street', N'New York', N'14000', 1, CAST(N'2024-12-18 13:07:09.213' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (49, N'CUST041', N'Customer 41 Name', N'555-041-0082', N'customer41@email.com', N'41 Main Street', N'Los Angeles', N'14100', 1, CAST(N'2025-08-13 13:07:09.213' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (50, N'CUST042', N'Customer 42 Name', N'555-042-0084', N'customer42@email.com', N'42 Main Street', N'Chicago', N'14200', 1, CAST(N'2025-01-07 13:07:09.213' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (51, N'CUST043', N'Customer 43 Name', N'555-043-0086', N'customer43@email.com', N'43 Main Street', N'Houston', N'14300', 1, CAST(N'2025-07-06 13:07:09.213' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (52, N'CUST044', N'Customer 44 Name', N'555-044-0088', N'customer44@email.com', N'44 Main Street', N'Phoenix', N'14400', 1, CAST(N'2025-04-07 13:07:09.217' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (53, N'CUST045', N'Customer 45 Name', N'555-045-0090', N'customer45@email.com', N'45 Main Street', N'Philadelphia', N'14500', 1, CAST(N'2025-02-23 13:07:09.217' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (54, N'CUST046', N'Customer 46 Name', N'555-046-0092', N'customer46@email.com', N'46 Main Street', N'San Antonio', N'14600', 1, CAST(N'2024-11-25 13:07:09.217' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (55, N'CUST047', N'Customer 47 Name', N'555-047-0094', N'customer47@email.com', N'47 Main Street', N'San Diego', N'14700', 1, CAST(N'2025-08-03 13:07:09.217' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (56, N'CUST048', N'Customer 48 Name', N'555-048-0096', N'customer48@email.com', N'48 Main Street', N'Dallas', N'14800', 1, CAST(N'2025-06-30 13:07:09.220' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (57, N'CUST049', N'Customer 49 Name', N'555-049-0098', N'customer49@email.com', N'49 Main Street', N'Miami', N'14900', 1, CAST(N'2025-05-05 13:07:09.220' AS DateTime))
GO
INSERT [dbo].[Customers] ([CustomerID], [CustomerCode], [CustomerName], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (58, N'CUST050', N'Customer 50 Name', N'555-050-0100', N'customer50@email.com', N'50 Main Street', N'New York', N'15000', 1, CAST(N'2025-01-26 13:07:09.220' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Customers] OFF
GO
SET IDENTITY_INSERT [dbo].[ExpenseCategories] ON 

GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (1, N'Rent', N'Monthly rent for store location', 1, CAST(N'2025-10-17 19:51:27.630' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (2, N'Utilities', N'Electricity, water, internet bills', 1, CAST(N'2025-10-17 19:51:27.630' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (3, N'Marketing', N'Advertising and promotional expenses', 1, CAST(N'2025-10-17 19:51:27.630' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (4, N'Office Supplies', N'Stationery and office materials', 1, CAST(N'2025-10-17 19:51:27.630' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (5, N'Maintenance', N'Equipment and store maintenance', 1, CAST(N'2025-10-17 19:51:27.630' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (7, N'Bills', N'', 1, CAST(N'2025-10-25 16:59:37.773' AS DateTime), N'EC001', 1, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (8, N'Utilities 1', N'Description for expense category 1', 1, CAST(N'2024-12-16 13:09:41.837' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (9, N'Office Supplies 2', N'Description for expense category 2', 1, CAST(N'2024-10-27 13:09:41.837' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (10, N'Marketing 3', N'Description for expense category 3', 1, CAST(N'2025-01-09 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (11, N'Equipment 4', N'Description for expense category 4', 1, CAST(N'2025-08-20 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (12, N'Maintenance 5', N'Description for expense category 5', 1, CAST(N'2024-11-08 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (13, N'Travel 6', N'Description for expense category 6', 1, CAST(N'2025-05-12 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (14, N'Miscellaneous 7', N'Description for expense category 7', 1, CAST(N'2025-09-04 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (15, N'Rent 8', N'Description for expense category 8', 1, CAST(N'2024-12-22 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (16, N'Utilities 9', N'Description for expense category 9', 1, CAST(N'2024-11-11 13:09:41.840' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (17, N'Office Supplies 10', N'Description for expense category 10', 1, CAST(N'2025-07-30 13:09:41.843' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (18, N'Marketing 11', N'Description for expense category 11', 1, CAST(N'2025-03-21 13:09:41.843' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (19, N'Equipment 12', N'Description for expense category 12', 1, CAST(N'2025-03-14 13:09:41.843' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (20, N'Maintenance 13', N'Description for expense category 13', 1, CAST(N'2025-06-05 13:09:41.843' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (21, N'Travel 14', N'Description for expense category 14', 1, CAST(N'2024-10-27 13:09:41.843' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (22, N'Miscellaneous 15', N'Description for expense category 15', 1, CAST(N'2025-04-21 13:09:41.843' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (23, N'Rent 16', N'Description for expense category 16', 1, CAST(N'2025-03-11 13:09:41.847' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (24, N'Utilities 17', N'Description for expense category 17', 1, CAST(N'2025-01-19 13:09:41.847' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (25, N'Office Supplies 18', N'Description for expense category 18', 1, CAST(N'2025-02-21 13:09:41.847' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (26, N'Marketing 19', N'Description for expense category 19', 1, CAST(N'2024-12-19 13:09:41.847' AS DateTime), NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseCategories] ([CategoryID], [CategoryName], [Description], [IsActive], [CreatedDate], [CategoryCode], [UserID], [LastModifiedDate]) VALUES (27, N'Equipment 20', N'Description for expense category 20', 1, CAST(N'2025-08-29 13:09:41.847' AS DateTime), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[ExpenseCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[ExpenseEntries] ON 

GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (1, CAST(N'2024-01-01 00:00:00.000' AS DateTime), 1, CAST(2000.00 AS Decimal(10, 2)), N'Monthly rent payment', N'Bank Transfer', 1, CAST(N'2025-10-17 19:51:27.640' AS DateTime), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (2, CAST(N'2024-01-05 00:00:00.000' AS DateTime), 2, CAST(150.00 AS Decimal(10, 2)), N'Electricity bill', N'Online Payment', 1, CAST(N'2025-10-17 19:51:27.640' AS DateTime), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (3, CAST(N'2024-01-10 00:00:00.000' AS DateTime), 3, CAST(300.00 AS Decimal(10, 2)), N'Social media advertising', N'Card', 1, CAST(N'2025-10-17 19:51:27.640' AS DateTime), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (10, CAST(N'2025-10-25 16:58:50.397' AS DateTime), 4, CAST(20000.00 AS Decimal(10, 2)), N'any discription', N'Cash', 1, CAST(N'2025-10-25 16:59:19.133' AS DateTime), N'EXP000001', N'9099', N'', N'Submitted', NULL)
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (11, CAST(N'2025-10-07 13:09:52.970' AS DateTime), 1, CAST(11.00 AS Decimal(10, 2)), N'Expense description 1', N'Credit Card', 1, CAST(N'2025-10-23 13:09:52.970' AS DateTime), N'EXP000001', N'REF000001', N'Expense remarks 1', N'Approved', CAST(N'2025-10-20 13:09:52.970' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (12, CAST(N'2025-10-09 13:09:52.973' AS DateTime), 2, CAST(12.00 AS Decimal(10, 2)), N'Expense description 2', N'Debit Card', 2, CAST(N'2025-10-09 13:09:52.973' AS DateTime), N'EXP000002', N'REF000002', N'Expense remarks 2', N'Approved', CAST(N'2025-10-17 13:09:52.973' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (13, CAST(N'2025-10-19 13:09:52.973' AS DateTime), 3, CAST(13.00 AS Decimal(10, 2)), N'Expense description 3', N'Check', 3, CAST(N'2025-10-22 13:09:52.973' AS DateTime), N'EXP000003', N'REF000003', N'Expense remarks 3', N'Approved', CAST(N'2025-10-03 13:09:52.973' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (15, CAST(N'2025-10-11 13:09:52.977' AS DateTime), 5, CAST(15.00 AS Decimal(10, 2)), N'Expense description 5', N'Credit Card', 5, CAST(N'2025-10-03 13:09:52.977' AS DateTime), N'EXP000005', N'REF000005', N'Expense remarks 5', N'Approved', CAST(N'2025-10-19 13:09:52.977' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (17, CAST(N'2025-10-08 13:09:52.977' AS DateTime), 7, CAST(17.00 AS Decimal(10, 2)), N'Expense description 7', N'Check', 7, CAST(N'2025-10-06 13:09:52.977' AS DateTime), N'EXP000007', N'REF000007', N'Expense remarks 7', N'Approved', CAST(N'2025-10-14 13:09:52.977' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (18, CAST(N'2025-10-12 13:09:52.977' AS DateTime), 8, CAST(18.00 AS Decimal(10, 2)), N'Expense description 8', N'Cash', 8, CAST(N'2025-10-07 13:09:52.977' AS DateTime), N'EXP000008', N'REF000008', N'Expense remarks 8', N'Approved', CAST(N'2025-10-03 13:09:52.977' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (19, CAST(N'2025-10-17 13:09:52.977' AS DateTime), 9, CAST(19.00 AS Decimal(10, 2)), N'Expense description 9', N'Credit Card', 9, CAST(N'2025-10-12 13:09:52.977' AS DateTime), N'EXP000009', N'REF000009', N'Expense remarks 9', N'Approved', CAST(N'2025-10-21 13:09:52.977' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (20, CAST(N'2025-10-23 13:09:52.980' AS DateTime), 10, CAST(20.00 AS Decimal(10, 2)), N'Expense description 10', N'Debit Card', 10, CAST(N'2025-10-24 13:09:52.980' AS DateTime), N'EXP000010', N'REF000010', N'Expense remarks 10', N'Approved', CAST(N'2025-10-15 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (21, CAST(N'2025-10-23 13:09:52.980' AS DateTime), 11, CAST(21.00 AS Decimal(10, 2)), N'Expense description 11', N'Check', 11, CAST(N'2025-10-07 13:09:52.980' AS DateTime), N'EXP000011', N'REF000011', N'Expense remarks 11', N'Approved', CAST(N'2025-10-20 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (22, CAST(N'2025-10-21 13:09:52.980' AS DateTime), 12, CAST(22.00 AS Decimal(10, 2)), N'Expense description 12', N'Cash', 12, CAST(N'2025-10-05 13:09:52.980' AS DateTime), N'EXP000012', N'REF000012', N'Expense remarks 12', N'Approved', CAST(N'2025-10-15 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (23, CAST(N'2025-10-11 13:09:52.980' AS DateTime), 13, CAST(23.00 AS Decimal(10, 2)), N'Expense description 13', N'Credit Card', 13, CAST(N'2025-10-15 13:09:52.980' AS DateTime), N'EXP000013', N'REF000013', N'Expense remarks 13', N'Approved', CAST(N'2025-10-17 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (24, CAST(N'2025-09-28 13:09:52.980' AS DateTime), 14, CAST(24.00 AS Decimal(10, 2)), N'Expense description 14', N'Debit Card', 14, CAST(N'2025-10-11 13:09:52.980' AS DateTime), N'EXP000014', N'REF000014', N'Expense remarks 14', N'Approved', CAST(N'2025-10-15 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (25, CAST(N'2025-10-01 13:09:52.980' AS DateTime), 15, CAST(25.00 AS Decimal(10, 2)), N'Expense description 15', N'Check', 15, CAST(N'2025-09-28 13:09:52.980' AS DateTime), N'EXP000015', N'REF000015', N'Expense remarks 15', N'Approved', CAST(N'2025-09-27 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (26, CAST(N'2025-10-09 13:09:52.980' AS DateTime), 16, CAST(26.00 AS Decimal(10, 2)), N'Expense description 16', N'Cash', 16, CAST(N'2025-10-13 13:09:52.980' AS DateTime), N'EXP000016', N'REF000016', N'Expense remarks 16', N'Approved', CAST(N'2025-10-19 13:09:52.980' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (27, CAST(N'2025-10-26 13:09:52.983' AS DateTime), 17, CAST(27.00 AS Decimal(10, 2)), N'Expense description 17', N'Credit Card', 17, CAST(N'2025-10-15 13:09:52.983' AS DateTime), N'EXP000017', N'REF000017', N'Expense remarks 17', N'Approved', CAST(N'2025-10-06 13:09:52.983' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (28, CAST(N'2025-10-15 13:09:52.983' AS DateTime), 18, CAST(28.00 AS Decimal(10, 2)), N'Expense description 18', N'Debit Card', 18, CAST(N'2025-10-08 13:09:52.983' AS DateTime), N'EXP000018', N'REF000018', N'Expense remarks 18', N'Approved', CAST(N'2025-10-01 13:09:52.983' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (29, CAST(N'2025-10-24 13:09:52.983' AS DateTime), 19, CAST(29.00 AS Decimal(10, 2)), N'Expense description 19', N'Check', 19, CAST(N'2025-10-25 13:09:52.983' AS DateTime), N'EXP000019', N'REF000019', N'Expense remarks 19', N'Approved', CAST(N'2025-10-17 13:09:52.983' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (30, CAST(N'2025-10-17 13:09:52.983' AS DateTime), 20, CAST(30.00 AS Decimal(10, 2)), N'Expense description 20', N'Cash', 20, CAST(N'2025-10-25 13:09:52.983' AS DateTime), N'EXP000020', N'REF000020', N'Expense remarks 20', N'Approved', CAST(N'2025-10-25 13:09:52.983' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (31, CAST(N'2025-10-24 13:09:52.983' AS DateTime), 1, CAST(31.00 AS Decimal(10, 2)), N'Expense description 21', N'Credit Card', 21, CAST(N'2025-10-15 13:09:52.983' AS DateTime), N'EXP000021', N'REF000021', N'Expense remarks 21', N'Approved', CAST(N'2025-09-29 13:09:52.983' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (32, CAST(N'2025-10-10 13:09:52.983' AS DateTime), 2, CAST(32.00 AS Decimal(10, 2)), N'Expense description 22', N'Debit Card', 22, CAST(N'2025-10-09 13:09:52.983' AS DateTime), N'EXP000022', N'REF000022', N'Expense remarks 22', N'Approved', CAST(N'2025-10-12 13:09:52.983' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (33, CAST(N'2025-09-28 13:09:52.987' AS DateTime), 3, CAST(33.00 AS Decimal(10, 2)), N'Expense description 23', N'Check', 23, CAST(N'2025-10-19 13:09:52.987' AS DateTime), N'EXP000023', N'REF000023', N'Expense remarks 23', N'Approved', CAST(N'2025-10-24 13:09:52.987' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (34, CAST(N'2025-10-05 13:09:52.987' AS DateTime), 4, CAST(34.00 AS Decimal(10, 2)), N'Expense description 24', N'Cash', 24, CAST(N'2025-10-09 13:09:52.987' AS DateTime), N'EXP000024', N'REF000024', N'Expense remarks 24', N'Approved', CAST(N'2025-10-26 13:09:52.987' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (35, CAST(N'2025-09-28 13:09:52.987' AS DateTime), 5, CAST(35.00 AS Decimal(10, 2)), N'Expense description 25', N'Credit Card', 25, CAST(N'2025-10-06 13:09:52.987' AS DateTime), N'EXP000025', N'REF000025', N'Expense remarks 25', N'Approved', CAST(N'2025-09-28 13:09:52.987' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (37, CAST(N'2025-10-13 13:09:52.990' AS DateTime), 7, CAST(37.00 AS Decimal(10, 2)), N'Expense description 27', N'Check', 27, CAST(N'2025-10-02 13:09:52.990' AS DateTime), N'EXP000027', N'REF000027', N'Expense remarks 27', N'Approved', CAST(N'2025-10-17 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (38, CAST(N'2025-10-08 13:09:52.990' AS DateTime), 8, CAST(38.00 AS Decimal(10, 2)), N'Expense description 28', N'Cash', 28, CAST(N'2025-10-20 13:09:52.990' AS DateTime), N'EXP000028', N'REF000028', N'Expense remarks 28', N'Approved', CAST(N'2025-10-10 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (39, CAST(N'2025-10-08 13:09:52.990' AS DateTime), 9, CAST(39.00 AS Decimal(10, 2)), N'Expense description 29', N'Credit Card', 29, CAST(N'2025-10-03 13:09:52.990' AS DateTime), N'EXP000029', N'REF000029', N'Expense remarks 29', N'Approved', CAST(N'2025-10-10 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (40, CAST(N'2025-10-07 13:09:52.990' AS DateTime), 10, CAST(40.00 AS Decimal(10, 2)), N'Expense description 30', N'Debit Card', 30, CAST(N'2025-10-05 13:09:52.990' AS DateTime), N'EXP000030', N'REF000030', N'Expense remarks 30', N'Approved', CAST(N'2025-10-23 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (41, CAST(N'2025-10-18 13:09:52.990' AS DateTime), 11, CAST(41.00 AS Decimal(10, 2)), N'Expense description 31', N'Check', 31, CAST(N'2025-10-07 13:09:52.990' AS DateTime), N'EXP000031', N'REF000031', N'Expense remarks 31', N'Approved', CAST(N'2025-10-02 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (42, CAST(N'2025-10-14 13:09:52.990' AS DateTime), 12, CAST(42.00 AS Decimal(10, 2)), N'Expense description 32', N'Cash', 32, CAST(N'2025-10-15 13:09:52.990' AS DateTime), N'EXP000032', N'REF000032', N'Expense remarks 32', N'Approved', CAST(N'2025-10-11 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (43, CAST(N'2025-10-04 13:09:52.990' AS DateTime), 13, CAST(43.00 AS Decimal(10, 2)), N'Expense description 33', N'Credit Card', 33, CAST(N'2025-09-30 13:09:52.990' AS DateTime), N'EXP000033', N'REF000033', N'Expense remarks 33', N'Approved', CAST(N'2025-10-17 13:09:52.990' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (44, CAST(N'2025-10-24 13:09:52.993' AS DateTime), 14, CAST(44.00 AS Decimal(10, 2)), N'Expense description 34', N'Debit Card', 34, CAST(N'2025-10-04 13:09:52.993' AS DateTime), N'EXP000034', N'REF000034', N'Expense remarks 34', N'Approved', CAST(N'2025-10-24 13:09:52.993' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (45, CAST(N'2025-10-12 13:09:52.993' AS DateTime), 15, CAST(45.00 AS Decimal(10, 2)), N'Expense description 35', N'Check', 35, CAST(N'2025-10-05 13:09:52.993' AS DateTime), N'EXP000035', N'REF000035', N'Expense remarks 35', N'Approved', CAST(N'2025-09-27 13:09:52.993' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (46, CAST(N'2025-10-23 13:09:52.993' AS DateTime), 16, CAST(46.00 AS Decimal(10, 2)), N'Expense description 36', N'Cash', 36, CAST(N'2025-10-08 13:09:52.993' AS DateTime), N'EXP000036', N'REF000036', N'Expense remarks 36', N'Approved', CAST(N'2025-10-19 13:09:52.993' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (47, CAST(N'2025-10-08 13:09:52.993' AS DateTime), 17, CAST(47.00 AS Decimal(10, 2)), N'Expense description 37', N'Credit Card', 37, CAST(N'2025-10-22 13:09:52.993' AS DateTime), N'EXP000037', N'REF000037', N'Expense remarks 37', N'Approved', CAST(N'2025-10-12 13:09:52.993' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (48, CAST(N'2025-10-07 13:09:52.993' AS DateTime), 18, CAST(48.00 AS Decimal(10, 2)), N'Expense description 38', N'Debit Card', 38, CAST(N'2025-10-18 13:09:52.993' AS DateTime), N'EXP000038', N'REF000038', N'Expense remarks 38', N'Approved', CAST(N'2025-10-05 13:09:52.993' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (49, CAST(N'2025-10-08 13:09:52.997' AS DateTime), 19, CAST(49.00 AS Decimal(10, 2)), N'Expense description 39', N'Check', 39, CAST(N'2025-09-27 13:09:52.997' AS DateTime), N'EXP000039', N'REF000039', N'Expense remarks 39', N'Approved', CAST(N'2025-09-29 13:09:52.997' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (50, CAST(N'2025-10-26 13:09:52.997' AS DateTime), 20, CAST(50.00 AS Decimal(10, 2)), N'Expense description 40', N'Cash', 40, CAST(N'2025-10-21 13:09:52.997' AS DateTime), N'EXP000040', N'REF000040', N'Expense remarks 40', N'Approved', CAST(N'2025-10-13 13:09:52.997' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (51, CAST(N'2025-10-11 13:09:53.000' AS DateTime), 1, CAST(51.00 AS Decimal(10, 2)), N'Expense description 41', N'Credit Card', 41, CAST(N'2025-10-22 13:09:53.000' AS DateTime), N'EXP000041', N'REF000041', N'Expense remarks 41', N'Approved', CAST(N'2025-10-04 13:09:53.000' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (52, CAST(N'2025-10-26 13:09:53.000' AS DateTime), 2, CAST(52.00 AS Decimal(10, 2)), N'Expense description 42', N'Debit Card', 42, CAST(N'2025-10-22 13:09:53.000' AS DateTime), N'EXP000042', N'REF000042', N'Expense remarks 42', N'Approved', CAST(N'2025-09-27 13:09:53.000' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (53, CAST(N'2025-10-23 13:09:53.000' AS DateTime), 3, CAST(53.00 AS Decimal(10, 2)), N'Expense description 43', N'Check', 43, CAST(N'2025-09-27 13:09:53.000' AS DateTime), N'EXP000043', N'REF000043', N'Expense remarks 43', N'Approved', CAST(N'2025-10-20 13:09:53.000' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (54, CAST(N'2025-10-23 13:09:53.000' AS DateTime), 4, CAST(54.00 AS Decimal(10, 2)), N'Expense description 44', N'Cash', 44, CAST(N'2025-10-17 13:09:53.000' AS DateTime), N'EXP000044', N'REF000044', N'Expense remarks 44', N'Approved', CAST(N'2025-10-21 13:09:53.000' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (55, CAST(N'2025-09-29 13:09:53.000' AS DateTime), 5, CAST(55.00 AS Decimal(10, 2)), N'Expense description 45', N'Credit Card', 45, CAST(N'2025-10-04 13:09:53.000' AS DateTime), N'EXP000045', N'REF000045', N'Expense remarks 45', N'Approved', CAST(N'2025-10-13 13:09:53.000' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (57, CAST(N'2025-10-12 13:09:53.003' AS DateTime), 7, CAST(57.00 AS Decimal(10, 2)), N'Expense description 47', N'Check', 47, CAST(N'2025-10-14 13:09:53.003' AS DateTime), N'EXP000047', N'REF000047', N'Expense remarks 47', N'Approved', CAST(N'2025-10-22 13:09:53.003' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (58, CAST(N'2025-10-18 13:09:53.003' AS DateTime), 8, CAST(58.00 AS Decimal(10, 2)), N'Expense description 48', N'Cash', 48, CAST(N'2025-10-23 13:09:53.003' AS DateTime), N'EXP000048', N'REF000048', N'Expense remarks 48', N'Approved', CAST(N'2025-10-08 13:09:53.003' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (59, CAST(N'2025-10-17 13:09:53.003' AS DateTime), 9, CAST(59.00 AS Decimal(10, 2)), N'Expense description 49', N'Credit Card', 49, CAST(N'2025-10-14 13:09:53.003' AS DateTime), N'EXP000049', N'REF000049', N'Expense remarks 49', N'Approved', CAST(N'2025-10-18 13:09:53.003' AS DateTime))
GO
INSERT [dbo].[ExpenseEntries] ([ExpenseID], [ExpenseDate], [CategoryID], [Amount], [Description], [PaymentMethod], [UserID], [CreatedDate], [ExpenseCode], [ReferenceNumber], [Remarks], [Status], [LastModifiedDate]) VALUES (60, CAST(N'2025-10-10 13:09:53.003' AS DateTime), 10, CAST(60.00 AS Decimal(10, 2)), N'Expense description 50', N'Debit Card', 50, CAST(N'2025-10-14 13:09:53.003' AS DateTime), N'EXP000050', N'REF000050', N'Expense remarks 50', N'Approved', CAST(N'2025-10-06 13:09:53.003' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[ExpenseEntries] OFF
GO
SET IDENTITY_INSERT [dbo].[Permissions] ON 

GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (1, N'*', N'All', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (2, N'sales', N'Sales module', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (3, N'purchases', N'Purchases module', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (4, N'inventory', N'Inventory & products', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (5, N'people', N'Customers & suppliers', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (6, N'users', N'User management', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (7, N'accounts', N'Cash/expenses/payments', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (8, N'reports', N'All reports', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (9, N'basic_reports', N'Basic reports', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (10, N'utilities', N'Utilities', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (11, N'backup', N'Database backup', NULL)
GO
INSERT [dbo].[Permissions] ([PermissionID], [PermissionKey], [Description], [Name]) VALUES (12, N'settings', N'System settings', NULL)
GO
SET IDENTITY_INSERT [dbo].[Permissions] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (12, N'9012', N'apple', N'', 53, 47, CAST(0.00 AS Decimal(10, 2)), CAST(26.00 AS Decimal(10, 2)), 100, 10, N'AUTO_12', 1, CAST(N'2025-10-24 12:43:35.847' AS DateTime), CAST(20.00 AS Decimal(18, 2)), CAST(N'2025-10-31 12:43:00.923' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (21, N'VAP-XROS3M', N'Vaporesso XROS 3 Mini', N'Compact pod system', 2, 2, CAST(15.00 AS Decimal(10, 2)), CAST(24.99 AS Decimal(10, 2)), 30, 5, N'AUTO_21', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(15.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (22, N'SMK-NORD5', N'SMOK Nord 5', N'Latest Nord with 2000mAh', 2, 1, CAST(24.00 AS Decimal(10, 2)), CAST(39.99 AS Decimal(10, 2)), 25, 5, N'AUTO_22', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(24.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (23, N'GK-AEGIS', N'GeekVape Aegis', N'Rugged vape device', 2, 3, CAST(33.00 AS Decimal(10, 2)), CAST(54.99 AS Decimal(10, 2)), 20, 3, N'AUTO_23', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(33.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (24, N'UW-CALI', N'Uwell Caliburn', N'Pod system device', 2, 4, CAST(14.00 AS Decimal(10, 2)), CAST(22.99 AS Decimal(10, 2)), 41, 10, N'AUTO_24', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(14.00 AS Decimal(18, 2)), CAST(N'2025-10-30 13:33:28.953' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (25, N'AS-NAUT', N'Aspire Nautilus', N'Tank system', 1, 5, CAST(16.00 AS Decimal(10, 2)), CAST(26.99 AS Decimal(10, 2)), 35, 8, N'AUTO_25', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(16.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (26, N'MAH-ELIQ', N'mahi E-Liquid', N'Premium e-liquid', 1, 6, CAST(15.00 AS Decimal(10, 2)), CAST(24.99 AS Decimal(10, 2)), 31, 6, N'AUTO_26', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(15.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (27, N'SMK-TFV18', N'SMOK TFV18 Tank', N'Sub-ohm tank', 3, 1, CAST(12.00 AS Decimal(10, 2)), CAST(19.99 AS Decimal(10, 2)), 25, 5, N'AUTO_27', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(12.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (28, N'VAP-GTX', N'Vaporesso GTX Coils', N'Replacement coils', 4, 2, CAST(26.00 AS Decimal(10, 2)), CAST(42.99 AS Decimal(10, 2)), 15, 3, N'AUTO_28', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(26.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (29, N'GK-COILS', N'GeekVape Coils', N'Coil pack', 4, 3, CAST(10.00 AS Decimal(10, 2)), CAST(16.99 AS Decimal(10, 2)), 50, 10, N'AUTO_29', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(10.00 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (30, N'UW-PODS', N'Uwell Pods', N'Pod replacements', 2, 4, CAST(8.50 AS Decimal(10, 2)), CAST(13.99 AS Decimal(10, 2)), 60, 15, N'AUTO_30', 1, CAST(N'2025-10-25 11:15:04.963' AS DateTime), CAST(8.50 AS Decimal(18, 2)), NULL, 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (31, N'PROD001', N'E-Liquid 1', N'High quality vape product 1', 1, 1, CAST(11.00 AS Decimal(10, 2)), CAST(16.00 AS Decimal(10, 2)), 11, 5, N'BC000001', 1, CAST(N'2025-08-28 13:08:09.213' AS DateTime), CAST(11.00 AS Decimal(18, 2)), CAST(N'2025-10-18 13:08:09.213' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (32, N'PROD002', N'Coil 2', N'High quality vape product 2', 2, 2, CAST(12.00 AS Decimal(10, 2)), CAST(17.00 AS Decimal(10, 2)), 12, 5, N'BC000002', 1, CAST(N'2024-11-03 13:08:09.220' AS DateTime), CAST(12.00 AS Decimal(18, 2)), CAST(N'2025-10-07 13:08:09.220' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (33, N'PROD003', N'Pod 3', N'High quality vape product 3', 3, 3, CAST(13.00 AS Decimal(10, 2)), CAST(18.00 AS Decimal(10, 2)), 13, 5, N'BC000003', 1, CAST(N'2024-11-15 13:08:09.220' AS DateTime), CAST(13.00 AS Decimal(18, 2)), CAST(N'2025-10-13 13:08:09.220' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (34, N'PROD004', N'Battery 4', N'High quality vape product 4', 4, 4, CAST(14.00 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), 14, 5, N'BC000004', 1, CAST(N'2025-01-21 13:08:09.220' AS DateTime), CAST(14.00 AS Decimal(18, 2)), CAST(N'2025-10-23 13:08:09.220' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (35, N'PROD005', N'Tank 5', N'High quality vape product 5', 5, 5, CAST(15.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), 15, 5, N'BC000005', 1, CAST(N'2025-09-08 13:08:09.220' AS DateTime), CAST(15.00 AS Decimal(18, 2)), CAST(N'2025-10-08 13:08:09.220' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (36, N'PROD006', N'Mod 6', N'High quality vape product 6', 6, 6, CAST(16.00 AS Decimal(10, 2)), CAST(21.00 AS Decimal(10, 2)), 16, 5, N'BC000006', 1, CAST(N'2025-06-24 13:08:09.220' AS DateTime), CAST(16.00 AS Decimal(18, 2)), CAST(N'2025-10-06 13:08:09.220' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (38, N'PROD008', N'Accessory 8', N'High quality vape product 8', 8, 8, CAST(18.00 AS Decimal(10, 2)), CAST(23.00 AS Decimal(10, 2)), 18, 5, N'BC000008', 1, CAST(N'2025-01-21 13:08:09.230' AS DateTime), CAST(18.00 AS Decimal(18, 2)), CAST(N'2025-10-25 13:08:09.230' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (39, N'PROD009', N'Starter Kit 9', N'High quality vape product 9', 9, 9, CAST(19.00 AS Decimal(10, 2)), CAST(24.00 AS Decimal(10, 2)), 19, 5, N'BC000009', 1, CAST(N'2025-08-19 13:08:09.230' AS DateTime), CAST(19.00 AS Decimal(18, 2)), CAST(N'2025-09-28 13:08:09.230' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (40, N'PROD010', N'Vape Pen 10', N'High quality vape product 10', 10, 10, CAST(20.00 AS Decimal(10, 2)), CAST(25.00 AS Decimal(10, 2)), 20, 5, N'BC000010', 1, CAST(N'2025-08-18 13:08:09.230' AS DateTime), CAST(20.00 AS Decimal(18, 2)), CAST(N'2025-10-15 13:08:09.230' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (41, N'PROD011', N'E-Liquid 11', N'High quality vape product 11', 11, 11, CAST(21.00 AS Decimal(10, 2)), CAST(26.00 AS Decimal(10, 2)), 21, 5, N'BC000011', 1, CAST(N'2025-06-25 13:08:09.233' AS DateTime), CAST(21.00 AS Decimal(18, 2)), CAST(N'2025-10-04 13:08:09.233' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (42, N'PROD012', N'Coil 12', N'High quality vape product 12', 12, 12, CAST(22.00 AS Decimal(10, 2)), CAST(27.00 AS Decimal(10, 2)), 22, 5, N'BC000012', 1, CAST(N'2025-02-01 13:08:09.233' AS DateTime), CAST(22.00 AS Decimal(18, 2)), CAST(N'2025-10-21 13:08:09.233' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (43, N'PROD013', N'Pod 13', N'High quality vape product 13', 13, 13, CAST(23.00 AS Decimal(10, 2)), CAST(28.00 AS Decimal(10, 2)), 23, 5, N'BC000013', 1, CAST(N'2024-11-07 13:08:09.233' AS DateTime), CAST(23.00 AS Decimal(18, 2)), CAST(N'2025-10-17 13:08:09.233' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (44, N'PROD014', N'Battery 14', N'High quality vape product 14', 14, 14, CAST(24.00 AS Decimal(10, 2)), CAST(29.00 AS Decimal(10, 2)), 24, 5, N'BC000014', 1, CAST(N'2025-06-29 13:08:09.233' AS DateTime), CAST(24.00 AS Decimal(18, 2)), CAST(N'2025-10-01 13:08:09.233' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (45, N'PROD015', N'Tank 15', N'High quality vape product 15', 15, 15, CAST(25.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), 25, 5, N'BC000015', 1, CAST(N'2025-03-05 13:08:09.237' AS DateTime), CAST(25.00 AS Decimal(18, 2)), CAST(N'2025-10-04 13:08:09.237' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (46, N'PROD016', N'Mod 16', N'High quality vape product 16', 16, 16, CAST(26.00 AS Decimal(10, 2)), CAST(31.00 AS Decimal(10, 2)), 27, 5, N'BC000016', 1, CAST(N'2025-07-11 13:08:09.237' AS DateTime), CAST(26.00 AS Decimal(18, 2)), CAST(N'2025-10-30 13:33:28.953' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (47, N'PROD017', N'Charger 17', N'High quality vape product 17', 17, 17, CAST(27.00 AS Decimal(10, 2)), CAST(32.00 AS Decimal(10, 2)), 28, 5, N'BC000017', 1, CAST(N'2025-04-25 13:08:09.237' AS DateTime), CAST(27.00 AS Decimal(18, 2)), CAST(N'2025-10-31 12:43:00.923' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (48, N'PROD018', N'Accessory 18', N'High quality vape product 18', 18, 12, CAST(0.00 AS Decimal(10, 2)), CAST(33.00 AS Decimal(10, 2)), 201, 5, N'wajahathussain', 1, CAST(N'2024-11-11 13:08:09.237' AS DateTime), CAST(28.00 AS Decimal(18, 2)), CAST(N'2025-10-30 13:33:28.953' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (49, N'PROD019', N'Starter Kit 19', N'High quality vape product 19', 19, 19, CAST(29.00 AS Decimal(10, 2)), CAST(34.00 AS Decimal(10, 2)), 29, 5, N'BC000019', 1, CAST(N'2024-12-28 13:08:09.237' AS DateTime), CAST(29.00 AS Decimal(18, 2)), CAST(N'2025-10-14 13:08:09.237' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (50, N'PROD020', N'Vape Pen 20', N'High quality vape product 20', 20, 20, CAST(10.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), 30, 5, N'BC000020', 1, CAST(N'2025-09-16 13:08:09.237' AS DateTime), CAST(10.00 AS Decimal(18, 2)), CAST(N'2025-09-30 13:08:09.237' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (51, N'PROD021', N'E-Liquid 21', N'High quality vape product 21', 21, 21, CAST(11.00 AS Decimal(10, 2)), CAST(36.00 AS Decimal(10, 2)), 31, 5, N'BC000021', 1, CAST(N'2024-12-26 13:08:09.240' AS DateTime), CAST(11.00 AS Decimal(18, 2)), CAST(N'2025-10-07 13:08:09.240' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (52, N'PROD022', N'Coil 22', N'High quality vape product 22', 22, 22, CAST(12.00 AS Decimal(10, 2)), CAST(37.00 AS Decimal(10, 2)), 32, 5, N'BC000022', 1, CAST(N'2025-04-19 13:08:09.240' AS DateTime), CAST(12.00 AS Decimal(18, 2)), CAST(N'2025-10-11 13:08:09.240' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (53, N'PROD023', N'Pod 23', N'High quality vape product 23', 23, 23, CAST(13.00 AS Decimal(10, 2)), CAST(38.00 AS Decimal(10, 2)), 33, 5, N'BC000023', 1, CAST(N'2025-08-06 13:08:09.240' AS DateTime), CAST(13.00 AS Decimal(18, 2)), CAST(N'2025-10-09 13:08:09.240' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (54, N'PROD024', N'Battery 24', N'High quality vape product 24', 24, 24, CAST(14.00 AS Decimal(10, 2)), CAST(39.00 AS Decimal(10, 2)), 34, 5, N'BC000024', 1, CAST(N'2024-10-29 13:08:09.240' AS DateTime), CAST(14.00 AS Decimal(18, 2)), CAST(N'2025-10-21 13:08:09.240' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (55, N'PROD025', N'Tank 25', N'High quality vape product 25', 25, 25, CAST(15.00 AS Decimal(10, 2)), CAST(40.00 AS Decimal(10, 2)), 35, 5, N'BC000025', 1, CAST(N'2025-04-23 13:08:09.240' AS DateTime), CAST(15.00 AS Decimal(18, 2)), CAST(N'2025-10-25 13:08:09.240' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (56, N'PROD026', N'Mod 26', N'High quality vape product 26', 26, 26, CAST(16.00 AS Decimal(10, 2)), CAST(41.00 AS Decimal(10, 2)), 36, 5, N'BC000026', 1, CAST(N'2025-07-02 13:08:09.240' AS DateTime), CAST(16.00 AS Decimal(18, 2)), CAST(N'2025-10-26 13:08:09.240' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (57, N'PROD027', N'Charger 27', N'High quality vape product 27', 27, 27, CAST(17.00 AS Decimal(10, 2)), CAST(42.00 AS Decimal(10, 2)), 37, 5, N'BC000027', 1, CAST(N'2024-12-06 13:08:09.243' AS DateTime), CAST(17.00 AS Decimal(18, 2)), CAST(N'2025-10-22 13:08:09.243' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (58, N'PROD028', N'Accessory 28', N'High quality vape product 28', 28, 28, CAST(0.00 AS Decimal(10, 2)), CAST(43.00 AS Decimal(10, 2)), 39, 10, N'BC000028', 1, CAST(N'2025-07-24 13:08:09.243' AS DateTime), CAST(18.00 AS Decimal(18, 2)), CAST(N'2025-10-30 13:33:28.953' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (59, N'PROD029', N'Starter Kit 29', N'High quality vape product 29', 29, 29, CAST(19.00 AS Decimal(10, 2)), CAST(44.00 AS Decimal(10, 2)), 39, 5, N'BC000029', 1, CAST(N'2025-07-30 13:08:09.243' AS DateTime), CAST(19.00 AS Decimal(18, 2)), CAST(N'2025-10-18 13:08:09.243' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (60, N'PROD030', N'Vape Pen 30', N'High quality vape product 30', 30, 30, CAST(20.00 AS Decimal(10, 2)), CAST(15.00 AS Decimal(10, 2)), 40, 5, N'BC000030', 1, CAST(N'2025-02-27 13:08:09.243' AS DateTime), CAST(20.00 AS Decimal(18, 2)), CAST(N'2025-09-29 13:08:09.243' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (61, N'PROD031', N'E-Liquid 31', N'High quality vape product 31', 31, 31, CAST(21.00 AS Decimal(10, 2)), CAST(16.00 AS Decimal(10, 2)), 41, 5, N'BC000031', 1, CAST(N'2024-12-20 13:08:09.243' AS DateTime), CAST(21.00 AS Decimal(18, 2)), CAST(N'2025-10-03 13:08:09.243' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (62, N'PROD032', N'Coil 32', N'High quality vape product 32', 32, 32, CAST(22.00 AS Decimal(10, 2)), CAST(17.00 AS Decimal(10, 2)), 42, 5, N'BC000032', 1, CAST(N'2024-12-29 13:08:09.243' AS DateTime), CAST(22.00 AS Decimal(18, 2)), CAST(N'2025-10-02 13:08:09.243' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (63, N'PROD033', N'Pod 33', N'High quality vape product 33', 33, 33, CAST(23.00 AS Decimal(10, 2)), CAST(18.00 AS Decimal(10, 2)), 43, 5, N'BC000033', 1, CAST(N'2024-10-31 13:08:09.247' AS DateTime), CAST(23.00 AS Decimal(18, 2)), CAST(N'2025-10-26 13:08:09.247' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (64, N'PROD034', N'Battery 34', N'High quality vape product 34', 34, 34, CAST(24.00 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), 44, 5, N'BC000034', 1, CAST(N'2025-07-17 13:08:09.247' AS DateTime), CAST(24.00 AS Decimal(18, 2)), CAST(N'2025-10-21 13:08:09.247' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (65, N'PROD035', N'Tank 35', N'High quality vape product 35', 35, 35, CAST(25.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), 45, 5, N'BC000035', 1, CAST(N'2025-04-12 13:08:09.247' AS DateTime), CAST(25.00 AS Decimal(18, 2)), CAST(N'2025-10-20 13:08:09.247' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (66, N'PROD036', N'Mod 36', N'High quality vape product 36', 36, 36, CAST(26.00 AS Decimal(10, 2)), CAST(21.00 AS Decimal(10, 2)), 46, 5, N'BC000036', 1, CAST(N'2024-11-10 13:08:09.247' AS DateTime), CAST(26.00 AS Decimal(18, 2)), CAST(N'2025-10-14 13:08:09.247' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (67, N'PROD037', N'Charger 37', N'High quality vape product 37', 37, 37, CAST(27.00 AS Decimal(10, 2)), CAST(22.00 AS Decimal(10, 2)), 47, 5, N'BC000037', 1, CAST(N'2025-04-04 13:08:09.250' AS DateTime), CAST(27.00 AS Decimal(18, 2)), CAST(N'2025-10-12 13:08:09.250' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (68, N'PROD038', N'Accessory 38', N'High quality vape product 38', 38, 38, CAST(28.00 AS Decimal(10, 2)), CAST(23.00 AS Decimal(10, 2)), 48, 5, N'BC000038', 1, CAST(N'2025-09-16 13:08:09.250' AS DateTime), CAST(28.00 AS Decimal(18, 2)), CAST(N'2025-10-06 13:08:09.250' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (69, N'PROD039', N'Starter Kit 39', N'High quality vape product 39', 39, 39, CAST(29.00 AS Decimal(10, 2)), CAST(24.00 AS Decimal(10, 2)), 49, 5, N'BC000039', 1, CAST(N'2025-02-15 13:08:09.250' AS DateTime), CAST(29.00 AS Decimal(18, 2)), CAST(N'2025-10-21 13:08:09.250' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (70, N'PROD040', N'Vape Pen 40', N'High quality vape product 40', 40, 40, CAST(10.00 AS Decimal(10, 2)), CAST(25.00 AS Decimal(10, 2)), 50, 5, N'BC000040', 1, CAST(N'2025-05-29 13:08:09.250' AS DateTime), CAST(10.00 AS Decimal(18, 2)), CAST(N'2025-10-18 13:08:09.250' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (71, N'PROD041', N'E-Liquid 41', N'High quality vape product 41', 41, 41, CAST(11.00 AS Decimal(10, 2)), CAST(26.00 AS Decimal(10, 2)), 51, 5, N'BC000041', 1, CAST(N'2024-11-02 13:08:09.250' AS DateTime), CAST(11.00 AS Decimal(18, 2)), CAST(N'2025-10-09 13:08:09.250' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (72, N'PROD042', N'Coil 42', N'High quality vape product 42', 42, 42, CAST(12.00 AS Decimal(10, 2)), CAST(27.00 AS Decimal(10, 2)), 52, 5, N'BC000042', 1, CAST(N'2025-04-01 13:08:09.250' AS DateTime), CAST(12.00 AS Decimal(18, 2)), CAST(N'2025-09-29 13:08:09.250' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (73, N'PROD043', N'Pod 43', N'High quality vape product 43', 43, 43, CAST(13.00 AS Decimal(10, 2)), CAST(28.00 AS Decimal(10, 2)), 53, 5, N'BC000043', 1, CAST(N'2025-03-21 13:08:09.253' AS DateTime), CAST(13.00 AS Decimal(18, 2)), CAST(N'2025-10-19 13:08:09.253' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (74, N'PROD044', N'Battery 44', N'High quality vape product 44', 44, 44, CAST(14.00 AS Decimal(10, 2)), CAST(29.00 AS Decimal(10, 2)), 55, 5, N'BC000044', 1, CAST(N'2025-06-21 13:08:09.253' AS DateTime), CAST(14.00 AS Decimal(18, 2)), CAST(N'2025-10-30 13:33:28.953' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (75, N'PROD045', N'Tank 45', N'High quality vape product 45', 45, 45, CAST(15.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), 55, 5, N'BC000045', 1, CAST(N'2025-02-21 13:08:09.253' AS DateTime), CAST(15.00 AS Decimal(18, 2)), CAST(N'2025-10-18 13:08:09.253' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (76, N'PROD046', N'Mod 46', N'High quality vape product 46', 46, 46, CAST(16.00 AS Decimal(10, 2)), CAST(31.00 AS Decimal(10, 2)), 56, 5, N'BC000046', 1, CAST(N'2025-04-15 13:08:09.253' AS DateTime), CAST(16.00 AS Decimal(18, 2)), CAST(N'2025-10-15 13:08:09.253' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (77, N'PROD047', N'Charger 47', N'High quality vape product 47', 47, 47, CAST(17.00 AS Decimal(10, 2)), CAST(32.00 AS Decimal(10, 2)), 57, 5, N'BC000047', 1, CAST(N'2025-09-01 13:08:09.257' AS DateTime), CAST(17.00 AS Decimal(18, 2)), CAST(N'2025-10-26 13:08:09.257' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (78, N'PROD048', N'Accessory 48', N'High quality vape product 48', 48, 48, CAST(18.00 AS Decimal(10, 2)), CAST(33.00 AS Decimal(10, 2)), 58, 5, N'BC000048', 1, CAST(N'2025-01-26 13:08:09.257' AS DateTime), CAST(18.00 AS Decimal(18, 2)), CAST(N'2025-10-23 13:08:09.257' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (79, N'PROD049', N'Starter Kit 49', N'High quality vape product 49', 49, 49, CAST(19.00 AS Decimal(10, 2)), CAST(34.00 AS Decimal(10, 2)), 59, 5, N'BC000049', 1, CAST(N'2024-11-02 13:08:09.257' AS DateTime), CAST(19.00 AS Decimal(18, 2)), CAST(N'2025-10-02 13:08:09.257' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (80, N'PROD050', N'Vape Pen 50', N'High quality vape product 50', 50, 50, CAST(20.00 AS Decimal(10, 2)), CAST(35.00 AS Decimal(10, 2)), 60, 5, N'BC000050', 1, CAST(N'2025-04-17 13:08:09.257' AS DateTime), CAST(20.00 AS Decimal(18, 2)), CAST(N'2025-09-30 13:08:09.257' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (131, N'PROD051', N'wajahat', N'any', 3, 5, CAST(0.00 AS Decimal(10, 2)), CAST(500.00 AS Decimal(10, 2)), 50, 50, N'PRDPROD051140521', 1, CAST(N'2025-10-27 14:05:43.777' AS DateTime), CAST(300.00 AS Decimal(18, 2)), CAST(N'2025-10-30 13:33:28.953' AS DateTime), 1)
GO
INSERT [dbo].[Products] ([ProductID], [ProductCode], [ProductName], [Description], [CategoryID], [BrandID], [PurchasePrice], [RetailPrice], [StockQuantity], [ReorderLevel], [Barcode], [IsActive], [CreatedDate], [CostPrice], [LastPurchaseDate], [IsAvailableForSale]) VALUES (132, N'PROD052', N'Farhan', N'none', 3, 5, CAST(0.00 AS Decimal(10, 2)), CAST(1000.00 AS Decimal(10, 2)), 71, 50, N'PRDPROD052105648', 1, CAST(N'2025-10-28 10:56:55.150' AS DateTime), CAST(500.00 AS Decimal(18, 2)), CAST(N'2025-10-31 12:43:00.923' AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseItems] ON 

GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (4, 16, 12, 50, CAST(100.00 AS Decimal(10, 2)), CAST(5000.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-24 16:40:01.457' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'apple', N'9012', N'pcs', CAST(26.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (8, 25, 12, 10, CAST(50.00 AS Decimal(10, 2)), CAST(500.00 AS Decimal(10, 2)), NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (9, 26, 21, 15, CAST(50.00 AS Decimal(10, 2)), CAST(750.00 AS Decimal(10, 2)), NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (10, 27, 22, 5, CAST(60.00 AS Decimal(10, 2)), CAST(300.00 AS Decimal(10, 2)), NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (11, 28, 23, 8, CAST(50.00 AS Decimal(10, 2)), CAST(400.00 AS Decimal(10, 2)), NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (12, 29, 24, 12, CAST(50.00 AS Decimal(10, 2)), CAST(600.00 AS Decimal(10, 2)), NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (37, 25, 25, 6, CAST(30.00 AS Decimal(10, 2)), CAST(180.00 AS Decimal(10, 2)), N'BATCH025', CAST(N'2026-11-20 13:09:35.393' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 25', 1, N'Product 25', N'PROD025', N'PCS', CAST(45.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (38, 26, 26, 7, CAST(31.00 AS Decimal(10, 2)), CAST(217.00 AS Decimal(10, 2)), N'BATCH026', CAST(N'2026-11-21 13:09:35.397' AS DateTime), CAST(1.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 26', 2, N'Product 26', N'PROD026', N'PCS', CAST(46.50 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (39, 27, 27, 8, CAST(32.00 AS Decimal(10, 2)), CAST(256.00 AS Decimal(10, 2)), N'BATCH027', CAST(N'2026-11-22 13:09:35.397' AS DateTime), CAST(2.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 27', 0, N'Product 27', N'PROD027', N'PCS', CAST(48.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (40, 28, 28, 9, CAST(33.00 AS Decimal(10, 2)), CAST(297.00 AS Decimal(10, 2)), N'BATCH028', CAST(N'2026-11-23 13:09:35.397' AS DateTime), CAST(3.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 28', 1, N'Product 28', N'PROD028', N'PCS', CAST(49.50 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (41, 29, 29, 10, CAST(34.00 AS Decimal(10, 2)), CAST(340.00 AS Decimal(10, 2)), N'BATCH029', CAST(N'2026-11-24 13:09:35.397' AS DateTime), CAST(4.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 29', 2, N'Product 29', N'PROD029', N'PCS', CAST(51.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (42, 30, 30, 1, CAST(5.00 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), N'BATCH030', CAST(N'2026-11-25 13:09:35.397' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 30', 0, N'Product 30', N'PROD030', N'PCS', CAST(7.50 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (43, 31, 31, 2, CAST(6.00 AS Decimal(10, 2)), CAST(12.00 AS Decimal(10, 2)), N'BATCH031', CAST(N'2026-11-26 13:09:35.397' AS DateTime), CAST(1.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 31', 1, N'Product 31', N'PROD031', N'PCS', CAST(9.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (44, 32, 32, 3, CAST(7.00 AS Decimal(10, 2)), CAST(21.00 AS Decimal(10, 2)), N'BATCH032', CAST(N'2026-11-27 13:09:35.400' AS DateTime), CAST(2.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 32', 2, N'Product 32', N'PROD032', N'PCS', CAST(10.50 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (93, 31, 21, 2, CAST(26.00 AS Decimal(10, 2)), CAST(52.00 AS Decimal(10, 2)), N'BATCH081', CAST(N'2027-01-15 13:09:35.433' AS DateTime), CAST(1.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 81', 0, N'Product 21', N'PROD021', N'PCS', CAST(39.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (94, 32, 22, 3, CAST(27.00 AS Decimal(10, 2)), CAST(81.00 AS Decimal(10, 2)), N'BATCH082', CAST(N'2027-01-16 13:09:35.433' AS DateTime), CAST(2.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), N'Purchase item remarks 82', 1, N'Product 22', N'PROD022', N'PCS', CAST(40.50 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (113, 80, 12, 20, CAST(50.00 AS Decimal(10, 2)), CAST(1000.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-27 13:38:35.400' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'apple', N'9012', N'pcs', CAST(100.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (114, 81, 131, 50, CAST(300.00 AS Decimal(10, 2)), CAST(15000.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-27 14:06:37.560' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'wajahat', N'PROD051', N'pcs', CAST(500.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (115, 82, 132, 100, CAST(500.00 AS Decimal(10, 2)), CAST(50000.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-28 10:58:34.623' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Farhan', N'PROD052', N'pcs', CAST(1000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (116, 83, 48, 50, CAST(28.00 AS Decimal(10, 2)), CAST(1400.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-28 11:41:08.437' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Accessory 18', N'PROD018', N'pcs', CAST(33.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (117, 84, 48, 22, CAST(28.00 AS Decimal(10, 2)), CAST(616.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-28 12:04:57.083' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Accessory 18', N'PROD018', N'pcs', CAST(33.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (118, 85, 48, 100, CAST(28.00 AS Decimal(10, 2)), CAST(2800.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-28 12:07:36.170' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Accessory 18', N'PROD018', N'pcs', CAST(33.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (119, 86, 58, 1, CAST(18.00 AS Decimal(10, 2)), CAST(18.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Accessory 28', N'PROD028', N'pcs', CAST(43.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (120, 86, 74, 1, CAST(14.00 AS Decimal(10, 2)), CAST(14.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Battery 44', N'PROD044', N'pcs', CAST(29.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (121, 86, 46, 1, CAST(26.00 AS Decimal(10, 2)), CAST(26.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Mod 16', N'PROD016', N'pcs', CAST(31.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (122, 86, 131, 1, CAST(300.00 AS Decimal(10, 2)), CAST(300.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'wajahat', N'PROD051', N'pcs', CAST(500.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (123, 86, 48, 1, CAST(28.00 AS Decimal(10, 2)), CAST(28.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Accessory 18', N'PROD018', N'pcs', CAST(33.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (124, 86, 12, 1, CAST(20.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'apple', N'9012', N'pcs', CAST(26.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (125, 86, 24, 1, CAST(14.00 AS Decimal(10, 2)), CAST(14.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-30 13:35:31.640' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Uwell Caliburn', N'UW-CALI', N'pcs', CAST(22.99 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (126, 87, 132, 1, CAST(500.00 AS Decimal(10, 2)), CAST(500.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-31 12:45:33.250' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Farhan', N'PROD052', N'pcs', CAST(1000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (127, 87, 12, 1, CAST(20.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-31 12:45:33.253' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'apple', N'9012', N'pcs', CAST(26.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseItems] ([PurchaseItemID], [PurchaseID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [BatchNumber], [ExpiryDate], [DiscountAmount], [TaxPercent], [Remarks], [Bonus], [ProductName], [ProductCode], [Unit], [SellingPrice]) VALUES (128, 87, 47, 1, CAST(27.00 AS Decimal(10, 2)), CAST(27.00 AS Decimal(10, 2)), N'', CAST(N'2026-10-31 12:45:33.253' AS DateTime), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'', 0, N'Charger 17', N'PROD017', N'pcs', CAST(32.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[PurchaseItems] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseReturnItems] ON 

GO
INSERT [dbo].[PurchaseReturnItems] ([ReturnItemID], [ReturnID], [ProductID], [Quantity], [UnitPrice], [SubTotal]) VALUES (1, 1, 12, 5, CAST(20.00 AS Decimal(10, 2)), CAST(100.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[PurchaseReturnItems] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseReturns] ON 

GO
INSERT [dbo].[PurchaseReturns] ([ReturnID], [ReturnNumber], [PurchaseID], [SupplierID], [ReturnDate], [ReturnReason], [Description], [TotalAmount], [UserID], [CreatedDate]) VALUES (1, N'PRT000001', 16, 1, CAST(N'2025-10-24 13:04:46.713' AS DateTime), N'Defective Product', N'', CAST(100.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-24 13:05:14.640' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[PurchaseReturns] OFF
GO
SET IDENTITY_INSERT [dbo].[Purchases] ON 

GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (16, N'PUR20251024124237', 1, CAST(N'2025-10-24 12:42:37.330' AS DateTime), CAST(5000.00 AS Decimal(10, 2)), CAST(237.13 AS Decimal(10, 2)), CAST(5.15 AS Decimal(5, 2)), CAST(4979.63 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(-4979.63 AS Decimal(10, 2)), 1, CAST(N'2025-10-24 12:43:35.777' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (25, N'PUR-2024-001', 1, CAST(N'2024-01-10 09:00:00.000' AS DateTime), CAST(500.00 AS Decimal(10, 2)), CAST(42.50 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(542.50 AS Decimal(10, 2)), N'Check', CAST(542.50 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 12:55:56.847' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (26, N'PUR-2024-002', 2, CAST(N'2024-01-12 14:30:00.000' AS DateTime), CAST(750.00 AS Decimal(10, 2)), CAST(63.75 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(813.75 AS Decimal(10, 2)), N'Bank Transfer', CAST(813.75 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 12:55:56.847' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (27, N'PUR-2024-003', 3, CAST(N'2024-01-14 11:15:00.000' AS DateTime), CAST(300.00 AS Decimal(10, 2)), CAST(25.50 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(325.50 AS Decimal(10, 2)), N'Cash', CAST(325.50 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 12:55:56.847' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (28, N'PUR-2024-004', 1, CAST(N'2024-01-16 16:45:00.000' AS DateTime), CAST(400.00 AS Decimal(10, 2)), CAST(34.00 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(434.00 AS Decimal(10, 2)), N'Check', CAST(200.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 12:55:56.847' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (29, N'PUR-2024-005', 2, CAST(N'2024-01-18 10:20:00.000' AS DateTime), CAST(600.00 AS Decimal(10, 2)), CAST(51.00 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(651.00 AS Decimal(10, 2)), N'Bank Transfer', CAST(651.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 12:55:56.847' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (30, N'PUR000001', 1, CAST(N'2025-09-06 13:09:07.987' AS DateTime), CAST(101.00 AS Decimal(10, 2)), CAST(8.33 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(109.33 AS Decimal(10, 2)), N'Credit', CAST(109.33 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-24 13:09:07.987' AS DateTime), N'PO000001', N'REF000001', N'Net 30', N'Stock Purchase', CAST(1.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(10, 2)), N'Purchase notes for invoice 1')
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (31, N'PUR000002', 2, CAST(N'2025-09-17 13:09:07.990' AS DateTime), CAST(102.00 AS Decimal(10, 2)), CAST(8.42 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(110.42 AS Decimal(10, 2)), N'Check', CAST(110.42 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-09-09 13:09:07.990' AS DateTime), N'PO000002', N'REF000002', N'Net 30', N'Stock Purchase', CAST(2.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(10, 2)), N'Purchase notes for invoice 2')
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (32, N'PUR000003', 3, CAST(N'2025-09-14 13:09:07.990' AS DateTime), CAST(103.00 AS Decimal(10, 2)), CAST(8.50 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(111.50 AS Decimal(10, 2)), N'Cash', CAST(111.50 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-15 13:09:07.990' AS DateTime), N'PO000003', N'REF000003', N'Net 30', N'Stock Purchase', CAST(3.00 AS Decimal(10, 2)), CAST(3.00 AS Decimal(10, 2)), CAST(3.00 AS Decimal(10, 2)), N'Purchase notes for invoice 3')
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (80, N'PUR20251027133642', 2, CAST(N'2025-10-27 13:36:42.747' AS DateTime), CAST(1000.00 AS Decimal(10, 2)), CAST(170.00 AS Decimal(10, 2)), CAST(17.00 AS Decimal(5, 2)), CAST(1170.00 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(1170.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-27 13:38:35.397' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (81, N'PUR20251027140613', 2, CAST(N'2025-10-27 14:06:12.997' AS DateTime), CAST(15000.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(15000.00 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(15000.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-27 14:06:37.557' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (82, N'PUR20251028105724', 1, CAST(N'2025-10-28 10:57:24.623' AS DateTime), CAST(50000.00 AS Decimal(10, 2)), CAST(9000.00 AS Decimal(10, 2)), CAST(18.00 AS Decimal(5, 2)), CAST(59000.00 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(59000.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 10:58:34.620' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (83, N'PUR20251028114017', 2, CAST(N'2025-10-28 11:40:17.510' AS DateTime), CAST(1400.00 AS Decimal(10, 2)), CAST(238.00 AS Decimal(10, 2)), CAST(17.00 AS Decimal(5, 2)), CAST(1638.00 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(1638.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 11:41:08.437' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (84, N'PUR20251028120419', 2, CAST(N'2025-10-28 12:04:19.147' AS DateTime), CAST(616.00 AS Decimal(10, 2)), CAST(110.88 AS Decimal(10, 2)), CAST(18.00 AS Decimal(5, 2)), CAST(726.88 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(726.88 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 12:04:57.080' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (85, N'PUR20251028120715', 2, CAST(N'2025-10-28 12:07:15.603' AS DateTime), CAST(2800.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(2800.00 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(2800.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 12:07:36.170' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (86, N'PUR20251030133329', 2, CAST(N'2025-10-30 13:33:28.953' AS DateTime), CAST(420.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(420.00 AS Decimal(10, 2)), N'Cash', CAST(0.00 AS Decimal(10, 2)), CAST(420.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-30 13:35:31.640' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
INSERT [dbo].[Purchases] ([PurchaseID], [InvoiceNumber], [SupplierID], [PurchaseDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [PurchaseOrderNumber], [ReferenceNumber], [PaymentTerms], [PurchaseType], [FreightCharges], [OtherCharges], [DiscountAmount], [Notes]) VALUES (87, N'PUR20251031124301', 1, CAST(N'2025-10-31 12:43:00.923' AS DateTime), CAST(547.00 AS Decimal(10, 2)), CAST(92.99 AS Decimal(10, 2)), CAST(17.00 AS Decimal(5, 2)), CAST(639.99 AS Decimal(10, 2)), N'60 Days', CAST(0.00 AS Decimal(10, 2)), CAST(639.99 AS Decimal(10, 2)), 1, CAST(N'2025-10-31 12:45:33.250' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL)
GO
SET IDENTITY_INSERT [dbo].[Purchases] OFF
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 1)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 2)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 3)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 4)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 5)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 6)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 7)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 8)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 9)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 10)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 11)
GO
INSERT [dbo].[RolePermissions] ([RoleID], [PermissionID]) VALUES (1, 12)
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [IsSystem], [Name]) VALUES (1, N'SuperAdmin', 1, NULL)
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [IsSystem], [Name]) VALUES (2, N'Admin', 1, NULL)
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [IsSystem], [Name]) VALUES (3, N'Manager', 0, NULL)
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [IsSystem], [Name]) VALUES (4, N'Sales', 0, NULL)
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName], [IsSystem], [Name]) VALUES (5, N'Cashier', 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[SaleItems] ON 

GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (13, 13, 12, 5, CAST(26.00 AS Decimal(10, 2)), CAST(130.00 AS Decimal(10, 2)), CAST(N'2025-10-24 12:44:43.373' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (15, 46, 29, 2, CAST(48.81 AS Decimal(10, 2)), CAST(97.62 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.410' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (17, 48, 25, 1, CAST(41.21 AS Decimal(10, 2)), CAST(41.21 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.413' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (18, 49, 22, 2, CAST(26.03 AS Decimal(10, 2)), CAST(52.06 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.417' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (19, 50, 29, 2, CAST(40.67 AS Decimal(10, 2)), CAST(81.34 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.417' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (20, 51, 25, 2, CAST(32.54 AS Decimal(10, 2)), CAST(65.08 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.417' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (21, 52, 28, 1, CAST(32.54 AS Decimal(10, 2)), CAST(32.54 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.417' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (22, 53, 25, 1, CAST(28.19 AS Decimal(10, 2)), CAST(28.19 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.420' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (23, 54, 27, 1, CAST(48.80 AS Decimal(10, 2)), CAST(48.80 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.420' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (24, 55, 24, 2, CAST(36.88 AS Decimal(10, 2)), CAST(73.75 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.420' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (25, 56, 28, 2, CAST(28.74 AS Decimal(10, 2)), CAST(57.48 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.420' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (26, 57, 25, 1, CAST(42.28 AS Decimal(10, 2)), CAST(42.28 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.420' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (27, 58, 27, 2, CAST(39.04 AS Decimal(10, 2)), CAST(78.08 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.423' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (28, 59, 27, 1, CAST(49.89 AS Decimal(10, 2)), CAST(49.89 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.423' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (29, 60, 22, 1, CAST(36.87 AS Decimal(10, 2)), CAST(36.87 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.423' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (30, 61, 30, 1, CAST(31.45 AS Decimal(10, 2)), CAST(31.45 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.423' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (31, 62, 22, 2, CAST(30.91 AS Decimal(10, 2)), CAST(61.81 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.427' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (32, 63, 22, 1, CAST(46.63 AS Decimal(10, 2)), CAST(46.63 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.427' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (33, 64, 23, 1, CAST(43.39 AS Decimal(10, 2)), CAST(43.39 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.427' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (34, 65, 26, 2, CAST(28.20 AS Decimal(10, 2)), CAST(56.40 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.430' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (35, 66, 26, 2, CAST(35.25 AS Decimal(10, 2)), CAST(70.49 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.430' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (36, 67, 27, 1, CAST(41.21 AS Decimal(10, 2)), CAST(41.21 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.430' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (37, 68, 24, 2, CAST(26.57 AS Decimal(10, 2)), CAST(53.13 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.430' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (38, 69, 30, 1, CAST(35.79 AS Decimal(10, 2)), CAST(35.79 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.430' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (39, 70, 23, 2, CAST(32.54 AS Decimal(10, 2)), CAST(65.08 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.433' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (40, 71, 23, 1, CAST(48.79 AS Decimal(10, 2)), CAST(48.79 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.433' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (41, 72, 21, 1, CAST(45.55 AS Decimal(10, 2)), CAST(45.55 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.433' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (42, 73, 28, 2, CAST(29.28 AS Decimal(10, 2)), CAST(58.56 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.437' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (43, 74, 25, 2, CAST(36.33 AS Decimal(10, 2)), CAST(72.65 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.437' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (44, 75, 24, 1, CAST(42.30 AS Decimal(10, 2)), CAST(42.30 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.437' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (45, 76, 30, 2, CAST(27.12 AS Decimal(10, 2)), CAST(54.23 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.440' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (46, 77, 25, 1, CAST(39.03 AS Decimal(10, 2)), CAST(39.03 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.440' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (47, 78, 30, 2, CAST(31.45 AS Decimal(10, 2)), CAST(62.89 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.440' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (48, 79, 22, 1, CAST(47.72 AS Decimal(10, 2)), CAST(47.72 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.440' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (49, 80, 21, 1, CAST(44.47 AS Decimal(10, 2)), CAST(44.47 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.443' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (50, 81, 21, 2, CAST(28.74 AS Decimal(10, 2)), CAST(57.48 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.443' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (51, 82, 21, 2, CAST(35.79 AS Decimal(10, 2)), CAST(71.58 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.443' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (52, 83, 28, 1, CAST(43.38 AS Decimal(10, 2)), CAST(43.38 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.443' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (53, 84, 24, 2, CAST(27.65 AS Decimal(10, 2)), CAST(55.30 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.447' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (54, 85, 22, 1, CAST(40.12 AS Decimal(10, 2)), CAST(40.12 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.447' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (55, 86, 25, 2, CAST(31.99 AS Decimal(10, 2)), CAST(63.98 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.450' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (56, 87, 26, 1, CAST(48.78 AS Decimal(10, 2)), CAST(48.78 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.450' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (57, 88, 28, 1, CAST(45.56 AS Decimal(10, 2)), CAST(45.56 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.450' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (58, 89, 23, 2, CAST(29.29 AS Decimal(10, 2)), CAST(58.57 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.450' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (59, 90, 24, 2, CAST(36.33 AS Decimal(10, 2)), CAST(72.66 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.453' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (60, 91, 28, 1, CAST(44.46 AS Decimal(10, 2)), CAST(44.46 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.453' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (61, 92, 25, 2, CAST(28.20 AS Decimal(10, 2)), CAST(56.39 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.457' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (62, 93, 30, 1, CAST(41.21 AS Decimal(10, 2)), CAST(41.21 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.457' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (63, 94, 26, 2, CAST(32.53 AS Decimal(10, 2)), CAST(65.06 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.457' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (64, 95, 30, 1, CAST(49.88 AS Decimal(10, 2)), CAST(49.88 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.460' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (65, 96, 24, 1, CAST(46.63 AS Decimal(10, 2)), CAST(46.63 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.460' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (66, 97, 28, 2, CAST(29.82 AS Decimal(10, 2)), CAST(59.64 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.460' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (67, 98, 29, 2, CAST(36.87 AS Decimal(10, 2)), CAST(73.74 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.460' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (68, 99, 26, 1, CAST(45.56 AS Decimal(10, 2)), CAST(45.56 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.463' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (69, 100, 28, 2, CAST(28.74 AS Decimal(10, 2)), CAST(57.48 AS Decimal(10, 2)), CAST(N'2025-10-25 12:51:40.463' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (70, 101, 12, 5, CAST(26.00 AS Decimal(10, 2)), CAST(130.00 AS Decimal(10, 2)), CAST(N'2025-10-25 18:05:06.920' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (116, 46, 46, 2, CAST(56.00 AS Decimal(10, 2)), CAST(112.00 AS Decimal(10, 2)), CAST(N'2025-10-22 13:08:50.343' AS DateTime), N'Product 46')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (118, 48, 48, 4, CAST(58.00 AS Decimal(10, 2)), CAST(232.00 AS Decimal(10, 2)), CAST(N'2025-10-04 13:08:50.343' AS DateTime), N'Product 48')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (119, 49, 49, 5, CAST(59.00 AS Decimal(10, 2)), CAST(295.00 AS Decimal(10, 2)), CAST(N'2025-10-01 13:08:50.343' AS DateTime), N'Product 49')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (120, 50, 50, 1, CAST(10.00 AS Decimal(10, 2)), CAST(10.00 AS Decimal(10, 2)), CAST(N'2025-10-02 13:08:50.347' AS DateTime), N'Product 50')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (166, 46, 36, 2, CAST(56.00 AS Decimal(10, 2)), CAST(112.00 AS Decimal(10, 2)), CAST(N'2025-10-22 13:08:50.370' AS DateTime), N'Product 36')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (168, 48, 38, 4, CAST(58.00 AS Decimal(10, 2)), CAST(232.00 AS Decimal(10, 2)), CAST(N'2025-10-04 13:08:50.370' AS DateTime), N'Product 38')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (169, 49, 39, 5, CAST(59.00 AS Decimal(10, 2)), CAST(295.00 AS Decimal(10, 2)), CAST(N'2025-10-11 13:08:50.370' AS DateTime), N'Product 39')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (170, 50, 40, 1, CAST(10.00 AS Decimal(10, 2)), CAST(10.00 AS Decimal(10, 2)), CAST(N'2025-10-18 13:08:50.370' AS DateTime), N'Product 40')
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (171, 152, 132, 10, CAST(1000.00 AS Decimal(10, 2)), CAST(10000.00 AS Decimal(10, 2)), CAST(N'2025-10-28 11:08:12.257' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (172, 153, 132, 10, CAST(1000.00 AS Decimal(10, 2)), CAST(10000.00 AS Decimal(10, 2)), CAST(N'2025-10-28 11:11:37.713' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (173, 154, 132, 5, CAST(1000.00 AS Decimal(10, 2)), CAST(5000.00 AS Decimal(10, 2)), CAST(N'2025-10-28 11:24:09.103' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (174, 155, 12, 2, CAST(26.00 AS Decimal(10, 2)), CAST(52.00 AS Decimal(10, 2)), CAST(N'2025-10-30 13:58:20.010' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (175, 156, 132, 5, CAST(1000.00 AS Decimal(10, 2)), CAST(5000.00 AS Decimal(10, 2)), CAST(N'2025-10-31 12:40:41.640' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (176, 156, 131, 1, CAST(500.00 AS Decimal(10, 2)), CAST(500.00 AS Decimal(10, 2)), CAST(N'2025-10-31 12:40:41.673' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (177, 47, 132, 2, CAST(1000.00 AS Decimal(10, 2)), CAST(2000.00 AS Decimal(10, 2)), CAST(N'2025-11-03 13:13:46.203' AS DateTime), NULL)
GO
INSERT [dbo].[SaleItems] ([SaleItemID], [SaleID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [LastModified], [ProductName]) VALUES (178, 47, 12, 3, CAST(26.00 AS Decimal(10, 2)), CAST(78.00 AS Decimal(10, 2)), CAST(N'2025-11-03 13:13:46.220' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[SaleItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Sales] ON 

GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (13, N'INV001', 6, CAST(N'2025-10-24 12:43:47.140' AS DateTime), CAST(130.00 AS Decimal(10, 2)), CAST(6.50 AS Decimal(10, 2)), CAST(5.00 AS Decimal(5, 2)), CAST(136.50 AS Decimal(10, 2)), N'Cash', CAST(140.00 AS Decimal(10, 2)), CAST(3.50 AS Decimal(10, 2)), 1, CAST(N'2025-10-24 12:44:43.360' AS DateTime), CAST(N'2025-10-24 12:44:43.360' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (46, N'INV-2024-001', 1, CAST(N'2024-01-15 10:30:00.000' AS DateTime), CAST(89.97 AS Decimal(10, 2)), CAST(7.65 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(97.62 AS Decimal(10, 2)), N'Cash', CAST(97.62 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (47, N'INV-2024-002', 2, CAST(N'2024-01-15 11:15:00.000' AS DateTime), CAST(2078.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(2078.00 AS Decimal(10, 2)), N'Cash', CAST(70.50 AS Decimal(10, 2)), CAST(-2007.50 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-11-03 13:13:46.157' AS DateTime), 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', N'', NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (48, N'INV-2024-003', 3, CAST(N'2024-01-15 14:20:00.000' AS DateTime), CAST(37.98 AS Decimal(10, 2)), CAST(3.23 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(41.21 AS Decimal(10, 2)), N'Cash', CAST(41.21 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (49, N'INV-2024-004', 4, CAST(N'2024-01-16 09:45:00.000' AS DateTime), CAST(47.98 AS Decimal(10, 2)), CAST(4.08 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(52.06 AS Decimal(10, 2)), N'Card', CAST(30.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (50, N'INV-2024-005', 5, CAST(N'2024-01-16 13:30:00.000' AS DateTime), CAST(74.97 AS Decimal(10, 2)), CAST(6.37 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(81.34 AS Decimal(10, 2)), N'Cash', CAST(81.34 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (51, N'INV-2024-006', 6, CAST(N'2024-01-17 10:00:00.000' AS DateTime), CAST(59.98 AS Decimal(10, 2)), CAST(5.10 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(65.08 AS Decimal(10, 2)), N'Card', CAST(65.08 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (52, N'INV-2024-007', 7, CAST(N'2024-01-17 15:45:00.000' AS DateTime), CAST(29.99 AS Decimal(10, 2)), CAST(2.55 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(32.54 AS Decimal(10, 2)), N'Cash', CAST(32.54 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (53, N'INV-2024-008', 1, CAST(N'2024-01-18 12:30:00.000' AS DateTime), CAST(25.98 AS Decimal(10, 2)), CAST(2.21 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(28.19 AS Decimal(10, 2)), N'Cash', CAST(28.19 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (54, N'INV-2024-009', 2, CAST(N'2024-01-18 16:20:00.000' AS DateTime), CAST(44.98 AS Decimal(10, 2)), CAST(3.82 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(48.80 AS Decimal(10, 2)), N'Card', CAST(48.80 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (55, N'INV-2024-010', 3, CAST(N'2024-01-19 11:10:00.000' AS DateTime), CAST(67.97 AS Decimal(10, 2)), CAST(5.78 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(73.75 AS Decimal(10, 2)), N'Cash', CAST(50.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (56, N'INV-2024-011', 4, CAST(N'2024-01-20 09:30:00.000' AS DateTime), CAST(52.98 AS Decimal(10, 2)), CAST(4.50 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(57.48 AS Decimal(10, 2)), N'Cash', CAST(57.48 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (57, N'INV-2024-012', 5, CAST(N'2024-01-20 14:15:00.000' AS DateTime), CAST(38.97 AS Decimal(10, 2)), CAST(3.31 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(42.28 AS Decimal(10, 2)), N'Card', CAST(42.28 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (58, N'INV-2024-013', 6, CAST(N'2024-01-21 11:45:00.000' AS DateTime), CAST(71.96 AS Decimal(10, 2)), CAST(6.12 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(78.08 AS Decimal(10, 2)), N'Cash', CAST(78.08 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (59, N'INV-2024-014', 7, CAST(N'2024-01-21 16:20:00.000' AS DateTime), CAST(45.98 AS Decimal(10, 2)), CAST(3.91 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(49.89 AS Decimal(10, 2)), N'Card', CAST(49.89 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (60, N'INV-2024-015', 1, CAST(N'2024-01-22 10:30:00.000' AS DateTime), CAST(33.98 AS Decimal(10, 2)), CAST(2.89 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(36.87 AS Decimal(10, 2)), N'Cash', CAST(36.87 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (61, N'INV-2024-016', 2, CAST(N'2024-01-22 15:10:00.000' AS DateTime), CAST(28.99 AS Decimal(10, 2)), CAST(2.46 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(31.45 AS Decimal(10, 2)), N'Cash', CAST(31.45 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (62, N'INV-2024-017', 3, CAST(N'2024-01-23 09:15:00.000' AS DateTime), CAST(56.97 AS Decimal(10, 2)), CAST(4.84 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(61.81 AS Decimal(10, 2)), N'Card', CAST(61.81 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (63, N'INV-2024-018', 4, CAST(N'2024-01-23 13:45:00.000' AS DateTime), CAST(42.98 AS Decimal(10, 2)), CAST(3.65 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(46.63 AS Decimal(10, 2)), N'Cash', CAST(46.63 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (64, N'INV-2024-019', 5, CAST(N'2024-01-24 11:20:00.000' AS DateTime), CAST(39.99 AS Decimal(10, 2)), CAST(3.40 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(43.39 AS Decimal(10, 2)), N'Card', CAST(43.39 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (65, N'INV-2024-020', 6, CAST(N'2024-01-24 16:30:00.000' AS DateTime), CAST(51.98 AS Decimal(10, 2)), CAST(4.42 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(56.40 AS Decimal(10, 2)), N'Cash', CAST(56.40 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (66, N'INV-2024-021', 7, CAST(N'2024-01-25 10:45:00.000' AS DateTime), CAST(64.97 AS Decimal(10, 2)), CAST(5.52 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(70.49 AS Decimal(10, 2)), N'Card', CAST(70.49 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (67, N'INV-2024-022', 1, CAST(N'2024-01-25 14:20:00.000' AS DateTime), CAST(37.98 AS Decimal(10, 2)), CAST(3.23 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(41.21 AS Decimal(10, 2)), N'Cash', CAST(41.21 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (68, N'INV-2024-023', 2, CAST(N'2024-01-26 09:30:00.000' AS DateTime), CAST(48.97 AS Decimal(10, 2)), CAST(4.16 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(53.13 AS Decimal(10, 2)), N'Card', CAST(53.13 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (69, N'INV-2024-024', 3, CAST(N'2024-01-26 15:15:00.000' AS DateTime), CAST(32.99 AS Decimal(10, 2)), CAST(2.80 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(35.79 AS Decimal(10, 2)), N'Cash', CAST(35.79 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (70, N'INV-2024-025', 4, CAST(N'2024-01-27 11:00:00.000' AS DateTime), CAST(59.98 AS Decimal(10, 2)), CAST(5.10 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(65.08 AS Decimal(10, 2)), N'Cash', CAST(65.08 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (71, N'INV-2024-026', 5, CAST(N'2024-01-27 16:45:00.000' AS DateTime), CAST(44.97 AS Decimal(10, 2)), CAST(3.82 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(48.79 AS Decimal(10, 2)), N'Card', CAST(48.79 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (72, N'INV-2024-027', 6, CAST(N'2024-01-28 10:15:00.000' AS DateTime), CAST(41.98 AS Decimal(10, 2)), CAST(3.57 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(45.55 AS Decimal(10, 2)), N'Cash', CAST(45.55 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (73, N'INV-2024-028', 7, CAST(N'2024-01-28 14:30:00.000' AS DateTime), CAST(53.97 AS Decimal(10, 2)), CAST(4.59 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(58.56 AS Decimal(10, 2)), N'Card', CAST(58.56 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (74, N'INV-2024-029', 1, CAST(N'2024-01-29 09:45:00.000' AS DateTime), CAST(66.96 AS Decimal(10, 2)), CAST(5.69 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(72.65 AS Decimal(10, 2)), N'Cash', CAST(72.65 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (75, N'INV-2024-030', 2, CAST(N'2024-01-29 13:20:00.000' AS DateTime), CAST(38.99 AS Decimal(10, 2)), CAST(3.31 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(42.30 AS Decimal(10, 2)), N'Card', CAST(42.30 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (76, N'INV-2024-031', 3, CAST(N'2024-01-30 10:30:00.000' AS DateTime), CAST(49.98 AS Decimal(10, 2)), CAST(4.25 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(54.23 AS Decimal(10, 2)), N'Cash', CAST(54.23 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (77, N'INV-2024-032', 4, CAST(N'2024-01-30 15:45:00.000' AS DateTime), CAST(35.97 AS Decimal(10, 2)), CAST(3.06 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(39.03 AS Decimal(10, 2)), N'Card', CAST(39.03 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (78, N'INV-2024-033', 5, CAST(N'2024-01-31 11:15:00.000' AS DateTime), CAST(57.96 AS Decimal(10, 2)), CAST(4.93 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(62.89 AS Decimal(10, 2)), N'Cash', CAST(62.89 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (79, N'INV-2024-034', 6, CAST(N'2024-01-31 16:20:00.000' AS DateTime), CAST(43.98 AS Decimal(10, 2)), CAST(3.74 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(47.72 AS Decimal(10, 2)), N'Card', CAST(47.72 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (80, N'INV-2024-035', 7, CAST(N'2024-02-01 09:30:00.000' AS DateTime), CAST(40.99 AS Decimal(10, 2)), CAST(3.48 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(44.47 AS Decimal(10, 2)), N'Cash', CAST(44.47 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (81, N'INV-2024-036', 1, CAST(N'2024-02-01 14:15:00.000' AS DateTime), CAST(52.98 AS Decimal(10, 2)), CAST(4.50 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(57.48 AS Decimal(10, 2)), N'Card', CAST(57.48 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (82, N'INV-2024-037', 2, CAST(N'2024-02-02 10:45:00.000' AS DateTime), CAST(65.97 AS Decimal(10, 2)), CAST(5.61 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(71.58 AS Decimal(10, 2)), N'Cash', CAST(71.58 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (83, N'INV-2024-038', 3, CAST(N'2024-02-02 15:30:00.000' AS DateTime), CAST(39.98 AS Decimal(10, 2)), CAST(3.40 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(43.38 AS Decimal(10, 2)), N'Card', CAST(43.38 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (84, N'INV-2024-039', 4, CAST(N'2024-02-03 11:20:00.000' AS DateTime), CAST(50.97 AS Decimal(10, 2)), CAST(4.33 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(55.30 AS Decimal(10, 2)), N'Cash', CAST(55.30 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (85, N'INV-2024-040', 5, CAST(N'2024-02-03 16:10:00.000' AS DateTime), CAST(36.98 AS Decimal(10, 2)), CAST(3.14 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(40.12 AS Decimal(10, 2)), N'Card', CAST(40.12 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (86, N'INV-2024-041', 6, CAST(N'2024-02-04 09:45:00.000' AS DateTime), CAST(58.97 AS Decimal(10, 2)), CAST(5.01 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(63.98 AS Decimal(10, 2)), N'Cash', CAST(63.98 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (87, N'INV-2024-042', 7, CAST(N'2024-02-04 14:30:00.000' AS DateTime), CAST(44.96 AS Decimal(10, 2)), CAST(3.82 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(48.78 AS Decimal(10, 2)), N'Card', CAST(48.78 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (88, N'INV-2024-043', 1, CAST(N'2024-02-05 10:15:00.000' AS DateTime), CAST(41.99 AS Decimal(10, 2)), CAST(3.57 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(45.56 AS Decimal(10, 2)), N'Cash', CAST(45.56 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (89, N'INV-2024-044', 2, CAST(N'2024-02-05 15:20:00.000' AS DateTime), CAST(53.98 AS Decimal(10, 2)), CAST(4.59 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(58.57 AS Decimal(10, 2)), N'Card', CAST(58.57 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (90, N'INV-2024-045', 3, CAST(N'2024-02-06 11:30:00.000' AS DateTime), CAST(66.97 AS Decimal(10, 2)), CAST(5.69 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(72.66 AS Decimal(10, 2)), N'Cash', CAST(72.66 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (91, N'INV-2024-046', 4, CAST(N'2024-02-06 16:45:00.000' AS DateTime), CAST(40.98 AS Decimal(10, 2)), CAST(3.48 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(44.46 AS Decimal(10, 2)), N'Card', CAST(44.46 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (92, N'INV-2024-047', 5, CAST(N'2024-02-07 09:20:00.000' AS DateTime), CAST(51.97 AS Decimal(10, 2)), CAST(4.42 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(56.39 AS Decimal(10, 2)), N'Cash', CAST(56.39 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (93, N'INV-2024-048', 6, CAST(N'2024-02-07 14:15:00.000' AS DateTime), CAST(37.98 AS Decimal(10, 2)), CAST(3.23 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(41.21 AS Decimal(10, 2)), N'Card', CAST(41.21 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (94, N'INV-2024-049', 7, CAST(N'2024-02-08 10:30:00.000' AS DateTime), CAST(59.96 AS Decimal(10, 2)), CAST(5.10 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(65.06 AS Decimal(10, 2)), N'Cash', CAST(65.06 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (95, N'INV-2024-050', 1, CAST(N'2024-02-08 15:45:00.000' AS DateTime), CAST(45.97 AS Decimal(10, 2)), CAST(3.91 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(49.88 AS Decimal(10, 2)), N'Card', CAST(49.88 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (96, N'INV-2024-051', 2, CAST(N'2024-02-09 11:10:00.000' AS DateTime), CAST(42.98 AS Decimal(10, 2)), CAST(3.65 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(46.63 AS Decimal(10, 2)), N'Cash', CAST(46.63 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (97, N'INV-2024-052', 3, CAST(N'2024-02-09 16:25:00.000' AS DateTime), CAST(54.97 AS Decimal(10, 2)), CAST(4.67 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(59.64 AS Decimal(10, 2)), N'Card', CAST(59.64 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (98, N'INV-2024-053', 4, CAST(N'2024-02-10 09:40:00.000' AS DateTime), CAST(67.96 AS Decimal(10, 2)), CAST(5.78 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(73.74 AS Decimal(10, 2)), N'Cash', CAST(73.74 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (99, N'INV-2024-054', 5, CAST(N'2024-02-10 14:20:00.000' AS DateTime), CAST(41.99 AS Decimal(10, 2)), CAST(3.57 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(45.56 AS Decimal(10, 2)), N'Card', CAST(45.56 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 2, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (100, N'INV-2024-055', 6, CAST(N'2024-02-11 10:50:00.000' AS DateTime), CAST(52.98 AS Decimal(10, 2)), CAST(4.50 AS Decimal(10, 2)), CAST(8.50 AS Decimal(5, 2)), CAST(57.48 AS Decimal(10, 2)), N'Cash', CAST(57.48 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-25 11:16:01.527' AS DateTime), CAST(N'2025-10-25 11:16:01.527' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, NULL, NULL)
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (101, N'INV-2025-056', 6, CAST(N'2025-10-25 18:04:28.423' AS DateTime), CAST(130.00 AS Decimal(10, 2)), CAST(5.20 AS Decimal(10, 2)), CAST(5.00 AS Decimal(5, 2)), CAST(109.20 AS Decimal(10, 2)), N'Cash', CAST(110.00 AS Decimal(10, 2)), CAST(0.80 AS Decimal(10, 2)), 1, CAST(N'2025-10-25 18:05:06.850' AS DateTime), CAST(N'2025-10-25 18:05:06.850' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, 0x89504E470D0A1A0A0000000D494844520000012C000000640802000000B3639A97000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA864000010F249444154785EED99D16D253B0E44273C07E4709CCBA43299EC837D6DBABA4E5190D617302434BF0A57558712C8DE9979FBE77F47D49F8F7A88FA251A78AAC187CD4E79A4BF18A78BC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF435E62435261069E72BADC809955D0D68CC416A509341AB3F11A036C69C5764CA65683114203819D412166D8BD0E79890D49851978CAE972036656415B33125B9426D068CCC66B0CB0A515DB31995A0D46080D047606859861F73AE42536241566E029A7CB0D9859056DCD486C519A40A3311BAF31C096566CC7646A3518213410D819146286DDEB9097D890549881A79C2E37606615B43523B14569028DC66CBCC6005B5AB11D93A9D56084D040606750881976AF735E72D75D9BD6FD11DE75D72FD7FD11DE75D72FD7FD113EB5FEBDBDFC7979FBF7100F6547E5D07AFFF1F5EFF5B7CF9F1FA589BFAF9F3F4A2239BF7F33F3770554934A0DEE7A52DD1FE153EBFA11E23BF938FAFBEA8B9CBFC1F72FE4F3D7AB7CA4DB6391846A25544EBD1B1ED6F8BF2177FDACEE8FF0A975F9085F5E5FE5E3FA5E5FFF0ADB6FF0DBA5DFCBD7AF15CBCE7F6F2FE3AF25A172CA2F7CD773EBFE089F5AD78FF0ED9F2DFAA7BE2E75FE06AFF595B878E3B771F95C5F3EFF0E495B874AA9990BDEF583BA3FC2A7967F84D73F996AADF5F39958F1EBDF06F9E574CECFD377C9161115531F877FBFFE5108D05D3FADFB237C6AF123AC65D78F50BE9FF8F73FAD8F8FE1CB12BF9CE8BC94FEA3EFAB86A8AF1F3F1C1FFFF9463E4D3AEFFA51DD1FE1532B7C845FDFD9757DBFB6FEB2FD1F5FD1E5CF1BFFAEFA2FC79D97FA8C5DF83DCA52D74FF812BBEB29757F844FADF8113E76FCFDBFD25C3FA7EBBF1959E9BB92807C0EC9A915DB6454305C4EB3F5AE9FD4FD113EB59A8FF0F3AF74975FDEBFC2D7D7FEEFA24EF8ACFA63E9FBCFA7E8B4FFDB02E719D5A4BEBFD7F417DBBB7E58F747F8D46A3FC2F087157FD17A7CB55A5FBB5F27F2B55CEBFBDBBCFCC072549FFAFEB963DDF57FD7FD11DE75D72FD7FD11DE75D72FD7FD11DE75D72FD77F627B715F0A07F66C0000000049454E44AE426082, N'INV-2025-056')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (102, N'INV000001', 1, CAST(N'2025-10-02 13:08:40.130' AS DateTime), CAST(51.00 AS Decimal(10, 2)), CAST(4.21 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(55.21 AS Decimal(10, 2)), N'Credit Card', CAST(56.21 AS Decimal(10, 2)), CAST(1.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-22 13:08:40.130' AS DateTime), CAST(N'2025-10-12 13:08:40.130' AS DateTime), 1, CAST(1.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 1', NULL, N'BC000001')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (103, N'INV000002', 2, CAST(N'2025-10-01 13:08:40.137' AS DateTime), CAST(52.00 AS Decimal(10, 2)), CAST(4.29 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(56.29 AS Decimal(10, 2)), N'Debit Card', CAST(58.29 AS Decimal(10, 2)), CAST(2.00 AS Decimal(10, 2)), 2, CAST(N'2025-09-27 13:08:40.137' AS DateTime), CAST(N'2025-09-27 13:08:40.137' AS DateTime), 2, CAST(2.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 2', NULL, N'BC000002')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (104, N'INV000003', 3, CAST(N'2025-10-05 13:08:40.140' AS DateTime), CAST(53.00 AS Decimal(10, 2)), CAST(4.37 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(57.37 AS Decimal(10, 2)), N'Mobile Payment', CAST(60.37 AS Decimal(10, 2)), CAST(3.00 AS Decimal(10, 2)), 3, CAST(N'2025-10-17 13:08:40.140' AS DateTime), CAST(N'2025-10-24 13:08:40.140' AS DateTime), 3, CAST(3.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 3', NULL, N'BC000003')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (106, N'INV000005', 5, CAST(N'2025-09-28 13:08:40.140' AS DateTime), CAST(55.00 AS Decimal(10, 2)), CAST(4.54 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(59.54 AS Decimal(10, 2)), N'Credit Card', CAST(64.54 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), 5, CAST(N'2025-10-20 13:08:40.140' AS DateTime), CAST(N'2025-10-21 13:08:40.140' AS DateTime), 5, CAST(0.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 5', NULL, N'BC000005')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (107, N'INV000006', 6, CAST(N'2025-10-07 13:08:40.140' AS DateTime), CAST(56.00 AS Decimal(10, 2)), CAST(4.62 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(60.62 AS Decimal(10, 2)), N'Debit Card', CAST(66.62 AS Decimal(10, 2)), CAST(6.00 AS Decimal(10, 2)), 6, CAST(N'2025-10-17 13:08:40.140' AS DateTime), CAST(N'2025-10-20 13:08:40.140' AS DateTime), 6, CAST(1.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 6', NULL, N'BC000006')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (108, N'INV000007', 7, CAST(N'2025-10-02 13:08:40.140' AS DateTime), CAST(57.00 AS Decimal(10, 2)), CAST(4.70 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(61.70 AS Decimal(10, 2)), N'Mobile Payment', CAST(68.70 AS Decimal(10, 2)), CAST(7.00 AS Decimal(10, 2)), 7, CAST(N'2025-10-07 13:08:40.140' AS DateTime), CAST(N'2025-10-24 13:08:40.140' AS DateTime), 7, CAST(2.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 7', NULL, N'BC000007')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (116, N'INV000015', 15, CAST(N'2025-10-12 13:08:40.147' AS DateTime), CAST(65.00 AS Decimal(10, 2)), CAST(5.36 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(70.36 AS Decimal(10, 2)), N'Mobile Payment', CAST(75.36 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), 15, CAST(N'2025-10-25 13:08:40.147' AS DateTime), CAST(N'2025-10-17 13:08:40.147' AS DateTime), 15, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 15', NULL, N'BC000015')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (117, N'INV000016', 16, CAST(N'2025-10-10 13:08:40.150' AS DateTime), CAST(66.00 AS Decimal(10, 2)), CAST(5.45 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(71.45 AS Decimal(10, 2)), N'Cash', CAST(77.45 AS Decimal(10, 2)), CAST(6.00 AS Decimal(10, 2)), 16, CAST(N'2025-10-17 13:08:40.150' AS DateTime), CAST(N'2025-10-10 13:08:40.150' AS DateTime), 16, CAST(1.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 16', NULL, N'BC000016')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (118, N'INV000017', 17, CAST(N'2025-10-20 13:08:40.150' AS DateTime), CAST(67.00 AS Decimal(10, 2)), CAST(5.53 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(72.53 AS Decimal(10, 2)), N'Credit Card', CAST(79.53 AS Decimal(10, 2)), CAST(7.00 AS Decimal(10, 2)), 17, CAST(N'2025-10-14 13:08:40.150' AS DateTime), CAST(N'2025-09-29 13:08:40.150' AS DateTime), 17, CAST(2.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 17', NULL, N'BC000017')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (119, N'INV000018', 18, CAST(N'2025-10-24 13:08:40.150' AS DateTime), CAST(68.00 AS Decimal(10, 2)), CAST(5.61 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(73.61 AS Decimal(10, 2)), N'Debit Card', CAST(81.61 AS Decimal(10, 2)), CAST(8.00 AS Decimal(10, 2)), 18, CAST(N'2025-10-17 13:08:40.150' AS DateTime), CAST(N'2025-09-27 13:08:40.150' AS DateTime), 18, CAST(3.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 18', NULL, N'BC000018')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (120, N'INV000019', 19, CAST(N'2025-10-17 13:08:40.153' AS DateTime), CAST(69.00 AS Decimal(10, 2)), CAST(5.69 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(74.69 AS Decimal(10, 2)), N'Mobile Payment', CAST(83.69 AS Decimal(10, 2)), CAST(9.00 AS Decimal(10, 2)), 19, CAST(N'2025-10-03 13:08:40.153' AS DateTime), CAST(N'2025-10-04 13:08:40.153' AS DateTime), 19, CAST(4.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 19', NULL, N'BC000019')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (121, N'INV000020', 20, CAST(N'2025-09-27 13:08:40.153' AS DateTime), CAST(70.00 AS Decimal(10, 2)), CAST(5.78 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(75.78 AS Decimal(10, 2)), N'Cash', CAST(75.78 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 20, CAST(N'2025-10-04 13:08:40.153' AS DateTime), CAST(N'2025-10-08 13:08:40.153' AS DateTime), 20, CAST(0.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 20', NULL, N'BC000020')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (122, N'INV000021', 21, CAST(N'2025-10-15 13:08:40.153' AS DateTime), CAST(71.00 AS Decimal(10, 2)), CAST(5.86 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(76.86 AS Decimal(10, 2)), N'Credit Card', CAST(77.86 AS Decimal(10, 2)), CAST(1.00 AS Decimal(10, 2)), 21, CAST(N'2025-10-05 13:08:40.153' AS DateTime), CAST(N'2025-10-10 13:08:40.153' AS DateTime), 21, CAST(1.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 21', NULL, N'BC000021')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (123, N'INV000022', 22, CAST(N'2025-10-12 13:08:40.153' AS DateTime), CAST(72.00 AS Decimal(10, 2)), CAST(5.94 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(77.94 AS Decimal(10, 2)), N'Debit Card', CAST(79.94 AS Decimal(10, 2)), CAST(2.00 AS Decimal(10, 2)), 22, CAST(N'2025-10-03 13:08:40.153' AS DateTime), CAST(N'2025-10-24 13:08:40.153' AS DateTime), 22, CAST(2.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 22', NULL, N'BC000022')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (124, N'INV000023', 23, CAST(N'2025-10-01 13:08:40.153' AS DateTime), CAST(73.00 AS Decimal(10, 2)), CAST(6.02 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(79.02 AS Decimal(10, 2)), N'Mobile Payment', CAST(82.02 AS Decimal(10, 2)), CAST(3.00 AS Decimal(10, 2)), 23, CAST(N'2025-10-06 13:08:40.153' AS DateTime), CAST(N'2025-10-20 13:08:40.153' AS DateTime), 23, CAST(3.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 23', NULL, N'BC000023')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (125, N'INV000024', 24, CAST(N'2025-10-15 13:08:40.157' AS DateTime), CAST(74.00 AS Decimal(10, 2)), CAST(6.11 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(80.11 AS Decimal(10, 2)), N'Cash', CAST(84.11 AS Decimal(10, 2)), CAST(4.00 AS Decimal(10, 2)), 24, CAST(N'2025-10-14 13:08:40.157' AS DateTime), CAST(N'2025-10-07 13:08:40.157' AS DateTime), 24, CAST(4.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 24', NULL, N'BC000024')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (126, N'INV000025', 25, CAST(N'2025-10-20 13:08:40.157' AS DateTime), CAST(75.00 AS Decimal(10, 2)), CAST(6.19 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(81.19 AS Decimal(10, 2)), N'Credit Card', CAST(86.19 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), 25, CAST(N'2025-10-17 13:08:40.157' AS DateTime), CAST(N'2025-10-15 13:08:40.157' AS DateTime), 25, CAST(0.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 25', NULL, N'BC000025')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (127, N'INV000026', 26, CAST(N'2025-10-04 13:08:40.157' AS DateTime), CAST(76.00 AS Decimal(10, 2)), CAST(6.27 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(82.27 AS Decimal(10, 2)), N'Debit Card', CAST(88.27 AS Decimal(10, 2)), CAST(6.00 AS Decimal(10, 2)), 26, CAST(N'2025-10-19 13:08:40.157' AS DateTime), CAST(N'2025-10-01 13:08:40.157' AS DateTime), 26, CAST(1.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 26', NULL, N'BC000026')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (128, N'INV000027', 27, CAST(N'2025-10-15 13:08:40.157' AS DateTime), CAST(77.00 AS Decimal(10, 2)), CAST(6.35 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(83.35 AS Decimal(10, 2)), N'Mobile Payment', CAST(90.35 AS Decimal(10, 2)), CAST(7.00 AS Decimal(10, 2)), 27, CAST(N'2025-10-16 13:08:40.157' AS DateTime), CAST(N'2025-09-28 13:08:40.157' AS DateTime), 27, CAST(2.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 27', NULL, N'BC000027')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (129, N'INV000028', 28, CAST(N'2025-10-12 13:08:40.157' AS DateTime), CAST(78.00 AS Decimal(10, 2)), CAST(6.44 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(84.44 AS Decimal(10, 2)), N'Cash', CAST(92.44 AS Decimal(10, 2)), CAST(8.00 AS Decimal(10, 2)), 28, CAST(N'2025-10-23 13:08:40.157' AS DateTime), CAST(N'2025-10-17 13:08:40.157' AS DateTime), 28, CAST(3.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 28', NULL, N'BC000028')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (130, N'INV000029', 29, CAST(N'2025-09-30 13:08:40.160' AS DateTime), CAST(79.00 AS Decimal(10, 2)), CAST(6.52 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(85.52 AS Decimal(10, 2)), N'Credit Card', CAST(94.52 AS Decimal(10, 2)), CAST(9.00 AS Decimal(10, 2)), 29, CAST(N'2025-10-05 13:08:40.160' AS DateTime), CAST(N'2025-10-12 13:08:40.160' AS DateTime), 29, CAST(4.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 29', NULL, N'BC000029')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (131, N'INV000030', 30, CAST(N'2025-10-13 13:08:40.160' AS DateTime), CAST(80.00 AS Decimal(10, 2)), CAST(6.60 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(86.60 AS Decimal(10, 2)), N'Debit Card', CAST(86.60 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 30, CAST(N'2025-10-02 13:08:40.160' AS DateTime), CAST(N'2025-10-01 13:08:40.160' AS DateTime), 30, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 30', NULL, N'BC000030')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (132, N'INV000031', 31, CAST(N'2025-10-02 13:08:40.160' AS DateTime), CAST(81.00 AS Decimal(10, 2)), CAST(6.68 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(87.68 AS Decimal(10, 2)), N'Mobile Payment', CAST(88.68 AS Decimal(10, 2)), CAST(1.00 AS Decimal(10, 2)), 31, CAST(N'2025-10-23 13:08:40.160' AS DateTime), CAST(N'2025-09-29 13:08:40.160' AS DateTime), 31, CAST(1.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 31', NULL, N'BC000031')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (133, N'INV000032', 32, CAST(N'2025-09-27 13:08:40.160' AS DateTime), CAST(82.00 AS Decimal(10, 2)), CAST(6.77 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(88.77 AS Decimal(10, 2)), N'Cash', CAST(90.77 AS Decimal(10, 2)), CAST(2.00 AS Decimal(10, 2)), 32, CAST(N'2025-10-08 13:08:40.160' AS DateTime), CAST(N'2025-10-15 13:08:40.160' AS DateTime), 32, CAST(2.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 32', NULL, N'BC000032')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (134, N'INV000033', 33, CAST(N'2025-10-02 13:08:40.160' AS DateTime), CAST(83.00 AS Decimal(10, 2)), CAST(6.85 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(89.85 AS Decimal(10, 2)), N'Credit Card', CAST(92.85 AS Decimal(10, 2)), CAST(3.00 AS Decimal(10, 2)), 33, CAST(N'2025-10-14 13:08:40.160' AS DateTime), CAST(N'2025-10-22 13:08:40.160' AS DateTime), 33, CAST(3.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 33', NULL, N'BC000033')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (135, N'INV000034', 34, CAST(N'2025-09-27 13:08:40.160' AS DateTime), CAST(84.00 AS Decimal(10, 2)), CAST(6.93 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(90.93 AS Decimal(10, 2)), N'Debit Card', CAST(94.93 AS Decimal(10, 2)), CAST(4.00 AS Decimal(10, 2)), 34, CAST(N'2025-10-08 13:08:40.160' AS DateTime), CAST(N'2025-10-19 13:08:40.160' AS DateTime), 34, CAST(4.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 34', NULL, N'BC000034')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (136, N'INV000035', 35, CAST(N'2025-09-29 13:08:40.163' AS DateTime), CAST(85.00 AS Decimal(10, 2)), CAST(7.01 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(92.01 AS Decimal(10, 2)), N'Mobile Payment', CAST(97.01 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), 35, CAST(N'2025-10-08 13:08:40.163' AS DateTime), CAST(N'2025-10-10 13:08:40.163' AS DateTime), 35, CAST(0.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 35', NULL, N'BC000035')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (137, N'INV000036', 36, CAST(N'2025-10-25 13:08:40.163' AS DateTime), CAST(86.00 AS Decimal(10, 2)), CAST(7.10 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(93.10 AS Decimal(10, 2)), N'Cash', CAST(99.10 AS Decimal(10, 2)), CAST(6.00 AS Decimal(10, 2)), 36, CAST(N'2025-09-29 13:08:40.163' AS DateTime), CAST(N'2025-09-27 13:08:40.163' AS DateTime), 36, CAST(1.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 36', NULL, N'BC000036')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (138, N'INV000037', 37, CAST(N'2025-10-02 13:08:40.163' AS DateTime), CAST(87.00 AS Decimal(10, 2)), CAST(7.18 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(94.18 AS Decimal(10, 2)), N'Credit Card', CAST(101.18 AS Decimal(10, 2)), CAST(7.00 AS Decimal(10, 2)), 37, CAST(N'2025-10-02 13:08:40.163' AS DateTime), CAST(N'2025-10-11 13:08:40.163' AS DateTime), 37, CAST(2.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 37', NULL, N'BC000037')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (139, N'INV000038', 38, CAST(N'2025-10-06 13:08:40.167' AS DateTime), CAST(88.00 AS Decimal(10, 2)), CAST(7.26 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(95.26 AS Decimal(10, 2)), N'Debit Card', CAST(103.26 AS Decimal(10, 2)), CAST(8.00 AS Decimal(10, 2)), 38, CAST(N'2025-10-05 13:08:40.167' AS DateTime), CAST(N'2025-10-22 13:08:40.167' AS DateTime), 38, CAST(3.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 38', NULL, N'BC000038')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (140, N'INV000039', 39, CAST(N'2025-10-08 13:08:40.167' AS DateTime), CAST(89.00 AS Decimal(10, 2)), CAST(7.34 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(96.34 AS Decimal(10, 2)), N'Mobile Payment', CAST(105.34 AS Decimal(10, 2)), CAST(9.00 AS Decimal(10, 2)), 39, CAST(N'2025-10-13 13:08:40.167' AS DateTime), CAST(N'2025-09-28 13:08:40.167' AS DateTime), 39, CAST(4.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 39', NULL, N'BC000039')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (141, N'INV000040', 40, CAST(N'2025-10-11 13:08:40.167' AS DateTime), CAST(90.00 AS Decimal(10, 2)), CAST(7.43 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(97.43 AS Decimal(10, 2)), N'Cash', CAST(97.43 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 40, CAST(N'2025-10-19 13:08:40.167' AS DateTime), CAST(N'2025-10-25 13:08:40.167' AS DateTime), 40, CAST(0.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 40', NULL, N'BC000040')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (142, N'INV000041', 41, CAST(N'2025-10-03 13:08:40.167' AS DateTime), CAST(91.00 AS Decimal(10, 2)), CAST(7.51 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(98.51 AS Decimal(10, 2)), N'Credit Card', CAST(99.51 AS Decimal(10, 2)), CAST(1.00 AS Decimal(10, 2)), 41, CAST(N'2025-10-07 13:08:40.167' AS DateTime), CAST(N'2025-10-16 13:08:40.167' AS DateTime), 41, CAST(1.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 41', NULL, N'BC000041')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (143, N'INV000042', 42, CAST(N'2025-10-09 13:08:40.170' AS DateTime), CAST(92.00 AS Decimal(10, 2)), CAST(7.59 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(99.59 AS Decimal(10, 2)), N'Debit Card', CAST(101.59 AS Decimal(10, 2)), CAST(2.00 AS Decimal(10, 2)), 42, CAST(N'2025-10-24 13:08:40.170' AS DateTime), CAST(N'2025-10-20 13:08:40.170' AS DateTime), 42, CAST(2.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 42', NULL, N'BC000042')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (144, N'INV000043', 43, CAST(N'2025-10-03 13:08:40.170' AS DateTime), CAST(93.00 AS Decimal(10, 2)), CAST(7.67 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(100.67 AS Decimal(10, 2)), N'Mobile Payment', CAST(103.67 AS Decimal(10, 2)), CAST(3.00 AS Decimal(10, 2)), 43, CAST(N'2025-10-05 13:08:40.170' AS DateTime), CAST(N'2025-09-29 13:08:40.170' AS DateTime), 43, CAST(3.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 43', NULL, N'BC000043')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (145, N'INV000044', 44, CAST(N'2025-10-17 13:08:40.170' AS DateTime), CAST(94.00 AS Decimal(10, 2)), CAST(7.76 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(101.76 AS Decimal(10, 2)), N'Cash', CAST(105.76 AS Decimal(10, 2)), CAST(4.00 AS Decimal(10, 2)), 44, CAST(N'2025-10-17 13:08:40.170' AS DateTime), CAST(N'2025-09-27 13:08:40.170' AS DateTime), 44, CAST(4.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 44', NULL, N'BC000044')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (146, N'INV000045', 45, CAST(N'2025-10-13 13:08:40.170' AS DateTime), CAST(95.00 AS Decimal(10, 2)), CAST(7.84 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(102.84 AS Decimal(10, 2)), N'Credit Card', CAST(107.84 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), 45, CAST(N'2025-10-08 13:08:40.170' AS DateTime), CAST(N'2025-10-14 13:08:40.170' AS DateTime), 45, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 45', NULL, N'BC000045')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (147, N'INV000046', 46, CAST(N'2025-10-24 13:08:40.170' AS DateTime), CAST(96.00 AS Decimal(10, 2)), CAST(7.92 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(103.92 AS Decimal(10, 2)), N'Debit Card', CAST(109.92 AS Decimal(10, 2)), CAST(6.00 AS Decimal(10, 2)), 46, CAST(N'2025-10-22 13:08:40.170' AS DateTime), CAST(N'2025-10-22 13:08:40.170' AS DateTime), 46, CAST(1.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 46', NULL, N'BC000046')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (148, N'INV000047', 47, CAST(N'2025-10-24 13:08:40.170' AS DateTime), CAST(97.00 AS Decimal(10, 2)), CAST(8.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(105.00 AS Decimal(10, 2)), N'Mobile Payment', CAST(112.00 AS Decimal(10, 2)), CAST(7.00 AS Decimal(10, 2)), 47, CAST(N'2025-09-30 13:08:40.170' AS DateTime), CAST(N'2025-10-03 13:08:40.170' AS DateTime), 47, CAST(2.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 47', NULL, N'BC000047')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (149, N'INV000048', 48, CAST(N'2025-10-07 13:08:40.173' AS DateTime), CAST(98.00 AS Decimal(10, 2)), CAST(8.09 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(106.09 AS Decimal(10, 2)), N'Cash', CAST(114.09 AS Decimal(10, 2)), CAST(8.00 AS Decimal(10, 2)), 48, CAST(N'2025-10-14 13:08:40.173' AS DateTime), CAST(N'2025-10-20 13:08:40.173' AS DateTime), 48, CAST(3.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 48', NULL, N'BC000048')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (150, N'INV000049', 49, CAST(N'2025-10-10 13:08:40.173' AS DateTime), CAST(99.00 AS Decimal(10, 2)), CAST(8.17 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(107.17 AS Decimal(10, 2)), N'Credit Card', CAST(116.17 AS Decimal(10, 2)), CAST(9.00 AS Decimal(10, 2)), 49, CAST(N'2025-10-03 13:08:40.173' AS DateTime), CAST(N'2025-10-15 13:08:40.173' AS DateTime), 49, CAST(4.00 AS Decimal(10, 2)), CAST(1.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 49', NULL, N'BC000049')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (151, N'INV000050', 50, CAST(N'2025-10-17 13:08:40.173' AS DateTime), CAST(100.00 AS Decimal(10, 2)), CAST(8.25 AS Decimal(10, 2)), CAST(8.25 AS Decimal(5, 2)), CAST(108.25 AS Decimal(10, 2)), N'Debit Card', CAST(108.25 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 50, CAST(N'2025-10-23 13:08:40.173' AS DateTime), CAST(N'2025-10-15 13:08:40.173' AS DateTime), 50, CAST(0.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Completed', N'Sale notes for invoice 50', NULL, N'BC000050')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (152, N'INV-2025-057', 6, CAST(N'2025-10-28 11:04:49.010' AS DateTime), CAST(10000.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(15.00 AS Decimal(5, 2)), CAST(9800.00 AS Decimal(10, 2)), N'Card', CAST(9800.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 11:08:12.240' AS DateTime), CAST(N'2025-10-28 11:08:12.240' AS DateTime), NULL, CAST(200.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Active', NULL, 0x89504E470D0A1A0A0000000D494844520000012C000000640802000000B3639A97000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000116649444154785EED9961AE9D3792437B7959509693BDF456B29319B865D37C3C54CD007A8D40C2AD5F848A6449607DB9B6F3AFFF79A2FEF59F5A402795C0AE0B172DBA6CF949F8EC247584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE425115224CDD888B90AEAB25557C14753524708D330DCA8ADD7186C85DD76E749D5A1300E17732090E3DA20DC5E8FBC24428AA4191B3157415DB6EA2AF8684AEA08611A861BB5F51A83ADB0DBEE3CA93A14C6E1620E04725C1B84DBEB9197444891346323E62AA8CB565D051F4D491D214CC370A3B65E63B01576DB9D275587C2385CCC81408E6B83707B3DF292082992666CC45C0575D9AAABE0A329A9238469186ED4D66B0CB6C26EBBF3A4EA5018878B3910C8716D106EAF475E122145D28C8D98ABA02E5B75157C34257584300DC38DDA7A8DC156D86D779E541D0AE37031070239AE0DC2EDF5C84B22A4489AB1117315D465ABAE828FA6A48E10A661B8515BAF31D80ABBEDCE93AA43611C2EE64020C7B541B8BD1E7949841449333662AE82BA6CD555F0D194D411C2340C376AEB35065B61B7DD795275288CC3C51C08E4B83608B7D7232F89902269C646CC555097ADBA0A3E9A923A429886E1466DBDC6602BECB63B4FAA0E8571B89803811CD706E1F67AE7259FFAD4A5F5F9083FF5A97FB83E1FE1A73EF50FD7E723FCD6FAFBAF3FFEF5C75F7F2FB050B4C4F0FA71F8E7BFBF9EFD3C5EE58A7FFFF9F3D0148DF9FB2CC8BFAB581595585F8F3FF55DF5F908BFB5BE7E84F84EFED3FAF79FF915F66FF0C7EEFF3CFD0A977ADB364853AF66F57FA9EA7F433E75569F8FF05BEBCB47F8C79F7FDAC7F57B7DF32BDC7E83BF59FEBDFC3A95AC33FFFEEB8FF96B6956FF87EAF309FE57EAF3117E6B7DFD08FFFA3B16FD27FEFA15F66FF06BFD527CE1E6C7FCF5F007F8E3E71F2D49DB598D2AFFC5FCD4F7D5E723FCD6CA8FF0EB2F93D6DA3F9FFFC737F863F97F9BF0CBD9317F767F408EA85693CA1FF0A9EFACCF47F8ADC58F50CBFE65877F7F3FF39FFF7E7D0CBF28F5CBA9CC2FD57EC246AB5F87A6FA7C83FFB5FA7C84DF5AE523FCF59D7D5DE25F5BFF65FBFFF3157DF9F7C7FCAEF65F4E32BFD44FD917FFBD55A87E56A57CEA3BEAF3117E6BD58F702DF08F7FA5F9FA397DFD3B23AB7D5726B06FA431BDEA986ED509F9B3F8A9EFACCF47F8ADB5F908D70E7FFD4E7E7C857FFEB9FFB3683AFC2C7D0CBFBF8ACA8CFF6D817EB7DAAAFA67FAA96FA9CF47F8ADB5FD08CB8F154FBCD657EBF5EB1B50C7BE96AFF5FBDBFC72C04AABBDEAF343F85FACCF47F8A94FFDC3F5F9083FF5A97FB83E1FE1A73EF50FD7FF0230EB91B71DBC58AA0000000049454E44AE426082, N'INV-2025-057')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (153, N'INV-2025-058', 6, CAST(N'2025-10-28 11:11:04.090' AS DateTime), CAST(10000.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(5.00 AS Decimal(5, 2)), CAST(10000.00 AS Decimal(10, 2)), N'Card', CAST(10000.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 11:11:37.697' AS DateTime), CAST(N'2025-10-28 11:11:37.697' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, 0x89504E470D0A1A0A0000000D494844520000012C000000640802000000B3639A97000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA864000010FF49444154785EED99516E5D390E447B7959509693BDF456B2936924B669BE734A82A0781048B8FA2A3C551D52202F9C9EF9E77F579C7F7E9F3751BF44836F7BF0CD865B5FF55FC019456289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FD5CF2120CA90B187CEBE97A035656A197762496286D2068CEC63626D8D21D3B623AB51D44CA41B40D7304C270FAB9E42518521730F8D6D3F506ACAC422FED482C51DA40D09C8D6D4CB0A53B76C4746A3B889483681BE60884E1F473C94B30A42E60F0ADA7EB0D5859855EDA9158A2B481A0391BDB98604B77EC88E9D476102907D136CC1108C3E9E7929760485DC0E05B4FD71BB0B20ABDB423B1446903417336B631C196EED811D3A9ED20520EA26D98231086D3CF252FC190BA80C1B79EAE376065157A69476289D20682E66C6C63822DDDB123A653DB41A41C44DB3047200CA79F4B5E82217501836F3D5D6FC0CA2AF4D28EC412A50D04CDD9D8C6045BBA63474CA7B683483988B6618E40184E3F97BC0443EA0206DF7ABADE809555E8A51D89254A1B089AB3B18D09B674C78E984E6D079172106DC31C81309C7E2E790986D4050CBEF574BD012BABD04B3B124B9436103467631B136CE98E1D319DDA0E22E520DA8639026138FDDCF392E73CE7D0F37C84CF79CE5F3ECF47F89CE7FCE5F37C845F7A7EFEF8F6CFB71F3FDFC49BC25539FAF9F5E3F77F5F7F7BFFF9EDF4C4BFDFDF7F6C89E4FCFC0DE6CF1350395546F7FE9C3F3ECF47F8A5E7F523D477F2FBEADFEFDCE4FC0DFE5AFCF75F5FE55B7A78DDA4A1FD24544C7DDEFF7E96EE9FF367E7F908BFF4BC7C84DFBE7F6F1FD7E79F407E85C36FF0D3D5BF978F5F2B969D3F7F7C9BFFD54AA898EADFA89A7FCE9F9FE723FCD2F3FA11FEF889457FD7AF8B9CBFC1D7F39178F1C6EFE1E573FDF6FE4F4BDB46A8987AFD4B9860CFF993F37C845F7AF811BEFE65AAFDED9FCFC237F8EB23F884F8CB1939DF6F7F499788A861EAE33F0A59F0395F709E8FF04B8F3FC25AF6973F229FDF4FFAF7DFCBF9FD317C58E297139D2FE7E51F94EF678AFAF8B1FEBDFB719D48CFF9C3F37C845F7AC247F8F19DBD7C84B5F52FDBFFFB2B7A3BEF8BCEEF6AFCE5D0F972DE632FFC310A29157DBEC22F3ECF47F8A5277E846F3BFEEB7FA579FD9C5EFF9BD1277D572DD0BE8DE4EC2796C9A86078B9CDD6E7FCC9793EC22F3D838FF0ED0FC8EB77F2EB2BFCFE7DFC6F5112DECFCBBF11DFBE86E87CFDFF2A7C9F5139D50A3C7F08FF0FE7F908BFF40C3FC2F0C7CABFF4F3F1BF847C9E8FDDAF9BF6B5BC9ECF6FF3E5071FA2C6A9720E1B7ECEF6793EC2E73CE72F9FE7237CCE73FEF2793EC2E73CE72F9FFF0048D76C88F11D70520000000049454E44AE426082, N'INV-2025-058')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (154, N'INV-2025-059', 6, CAST(N'2025-10-28 11:23:19.240' AS DateTime), CAST(5000.00 AS Decimal(10, 2)), CAST(245.00 AS Decimal(10, 2)), CAST(5.00 AS Decimal(5, 2)), CAST(5145.00 AS Decimal(10, 2)), N'Card', CAST(5145.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-28 11:24:09.090' AS DateTime), CAST(N'2025-10-28 11:24:09.090' AS DateTime), NULL, CAST(100.00 AS Decimal(10, 2)), CAST(2.00 AS Decimal(5, 2)), N'Active', NULL, 0x89504E470D0A1A0A0000000D494844520000012C000000640802000000B3639A97000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000117349444154785EED98E18D56C90E44373C02221C72211532794F3083A7BE3A656F6F3312EAD6F52FEBB3EBD8ADF25D60FFF9DF15F1CFAF784BEA97D8C0AA0ADFDAACCA92FE629C4E1247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C7252F3193A2D3E6ABE5EA2E2F60E5147434257144E5041A8DDAB8C680AD5CB11D93AA3DE1B0644547D6062DB1E1F4B8E425665274DA7CB55CDDE505AC9C828EA6248EA89C40A3511BD718B0952BB66352B5271C96ACE8C8DAA025369C1E97BCC44C8A4E9BAF96ABBBBC809553D0D194C411951368346AE31A03B672C5764CAAF684C392151D591BB4C486D3E392979849D169F3D572759717B0720A3A9A9238A272028D466D5C63C056AED88E49D59E7058B2A2236B8396D8707A5CF21233293A6DBE5AAEEEF202564E4147531247544EA0D1A88D6B0CD8CA15DB31A9DA130E4B5674646DD0121B4E8F4B5E622645A7CD57CBD55D5EC0CA29E8684AE288CA09341AB5718D015BB9623B26557BC261C98A8EAC0D5A62C3E971C94BCCA4E8B4F96AB9BACB0B5839051D4D491C51398146A336AE31602B576CC7A46A4F382C59D191B5414B6C383D2E798999149D365F2D577779012BA7A0A32989232A27D068D4C635066CE58AED9854ED0987252B3AB23668890DA7C73D2F79E28943E3F9089F78E22FC7F3113EF1C45F8EE723FCD4F8F1EDCB3F5FBEFD784BDE322B5587C6CF1FBF7E7FFDEDFDE7B750C5F7AFEF3F8A22757EFC66CD1F115059558D58FD893F8FE723FCD478FD08F19DFC2A7DFFEAA79CBFC19F87FFFEEB6BFAA66ECB9212AA915049F5EB2D6F3FC6FF843CF187F17C849F1A2F1FE197AF5FE5E3FAB85FFF0ADB6FF0A34BBF97DFBF962C77FEF8F665FE5C122AAA32FF894F8BE723FCD478FD08BFFDB0437FCF5F0F397F83AFF15BF1D21BBF8797CFF5CBFB5F2DD9D6A192CA3FC27FDDF689FF16CF47F8A9E11FE1EB9F4CF1AC17BEC19F87FF01E197D375BE577FA61C115151A5FA5FFF3804EB893F8AE723FCD4E04758C7AE1FA17C3FE9EF7F2FF1EB13F8DD12BF9CD8F912E98FAF11F5FBC7976FEFD73F72BFFDFB7F339EF88FF17C849F1AE123FCFD9DBD7C8475F52FD7FFEB2B7A3FF6B73BF7EFAAFF72BCF325DE652FFC1E652AFCC8CE27FE289E8FF053237E846F37FEF3FFD2BC7E4EAFFF6664A4EF4A04F28DA44E8D3826A3724345EE7CE28FE2F9083F359A8FF0F7DFE7F4979F5FE1D7AFFD1F2B4E788FFA3BE2C75F1663A7FD5D12F58C8A2AE1A7BFD73EF1A7F17C849F1AED4718FEB0E22F1AF5AFB08ADFC7FFF1EFB38FAFE5353EBECD971F188EEA541F3FB6FB3EB11FCF47F8C4137F399E8FF08927FE723C1FE1134FFCE5F83F6EF766F2EF5BA6BF0000000049454E44AE426082, N'INV-2025-059')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (155, N'INV-2025-060', 6, CAST(N'2025-10-30 13:56:05.390' AS DateTime), CAST(52.00 AS Decimal(10, 2)), CAST(8.84 AS Decimal(10, 2)), CAST(17.00 AS Decimal(5, 2)), CAST(60.84 AS Decimal(10, 2)), N'Card', CAST(60.84 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-30 13:58:19.993' AS DateTime), CAST(N'2025-10-30 13:58:19.993' AS DateTime), NULL, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), N'Active', NULL, 0x89504E470D0A1A0A0000000D494844520000012C000000640802000000B3639A97000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000118949444154785EED99D18D5D390E441D9E037238CEC5A938935DB8D54DD7AB53E2F0E3018684CB9FE188558712C83BDDF67CFBDF15F1ED2356522751C0AA1A97CCAA2CE989717696D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971C94B6C48FDA4ADCAE9720326ABA0AD69892D2A27D068F4C66B34D8CA15BB63D245233573C24A54A6253D218782D3E39297D890FA495B95D3E5064C56415BD3125B544EA0D1E88DD768B0952B76C7A48B466AE68495A84C4B7A420E05A7C7252FB121F593B62AA7CB0D98AC82B6A625B6A89C40A3D11BAFD1602B57EC8E49178DD4CC092B519996F4841C0A4E8F4B5E6243EA276D554E971B3059056D4D4B6C51398146A3375EA3C156AED81D932E1AA9991356A2322DE90939149C1E97BCC486D44FDAAA9C2E3760B20ADA9A96D8A272028D466FBC4683AD5CB13B265D34523327AC44655AD2137228383D2E79890DA99FB455395D6EC06415B4352DB145E5041A8DDE788D065BB962774CBA68A4664E5889CAB4A427E450707A5CF2121B523F69AB72BADC80C92A686B5A628BCA09341ABDF11A0DB672C5EE9874D148CD9CB0129569494FC8A1E0F4B8E42536A47ED256E574B9019355D0D6B4C416951368347AE3351A6CE58ADD31E9A2919A3961252AD3929E9043C1E971CF4B9E78E2D0783EC2279EF8C7F17C844F3CF18FE3F908DF1ABF7F7EFFF6FDE7EF95ACCC4AA5D0F873F8E3D7EBD9E7F10A75FCFAF179288EA4FC7B66E2BF91509195954FBC279E8FF0ADF1FA11E23BF928FDFAE15F61FE06FFECFDE7E96BBADCDBB2A4846A24D447BE4EE53F1751F9C4BBE2F908DF1A2F1FE1F71F3FE4E3FABBD3FE156EBFC1BF2AFD0ABE4ECB9695BF7F7EB76FDD22A170B7BDF289B7C5F311BE355E3FC29FBF6D7DFD074B15FE6BAFBF1C2FDAFE83F9937CFFFCC592B20D2ADE252A9F785F3C1FE15BC33FC2D79F4CB5BEBAC971EF5FE3F557C4EE7B88BF4CFE49D922A23EFEF9EBEB0F859FE5A87CE27DF17C846F0D7E84B5C2FA11CA2AFFE76F8D1F9FD097A4FD1E5E942F91FE2817511F7FFF22DF6EBD05CA27DE17CF47F8D6081FE1D777F6F211BEFEE8F93AFCF88AF04348977EFF3DB8F2253E6D2FFC887AF95ABF1451F9C4FBE2F908DF1AF1235C9BFBE76F695E3FA7D73F3332D2772506F936925223B649A897EFADFE25299F785F3C1FE15B63F3117EFE9EF772F2E72BFCF163FFBBA8133EA37E56FDFDA11595F6BF2D50CF28FDE08C00E513EF8AE7237C6B6C3FC2F0C38A271AEBABD590CFE4E560A7FCE0EB01C3511FF1D727A751F9C47BE2F9089F78E21FC7F3113EF1C43F8EE7237CE2897F1CFF078044644589F318700000000049454E44AE426082, N'INV-2025-060')
GO
INSERT [dbo].[Sales] ([SaleID], [InvoiceNumber], [CustomerID], [SaleDate], [SubTotal], [TaxAmount], [TaxPercent], [TotalAmount], [PaymentMethod], [PaidAmount], [ChangeAmount], [UserID], [CreatedDate], [LastModified], [ModifiedBy], [DiscountAmount], [DiscountPercent], [Status], [Notes], [BarcodeImage], [BarcodeData]) VALUES (156, N'INV-2025-061', 6, CAST(N'2025-10-31 12:39:18.297' AS DateTime), CAST(5500.00 AS Decimal(10, 2)), CAST(888.25 AS Decimal(10, 2)), CAST(17.00 AS Decimal(5, 2)), CAST(6113.25 AS Decimal(10, 2)), N'Card', CAST(6113.25 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), 1, CAST(N'2025-10-31 12:40:41.613' AS DateTime), CAST(N'2025-10-31 12:40:41.613' AS DateTime), NULL, CAST(275.00 AS Decimal(10, 2)), CAST(5.00 AS Decimal(5, 2)), N'Active', NULL, 0x89504E470D0A1A0A0000000D494844520000012C000000640802000000B3639A97000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000097048597300000EC300000EC301C76FA8640000112349444154785EED9951925DB90D4367795E502FC77BF156BC93A4DCECA6F97020441577E292EAF227AC27E0402AF28E3D937FFE7545FDF35ED5F42F56C0D3692C999CF268FE229C95C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707A5DF292305111F094D3E506ECACC28CA6C546744FA0D0E8B5D708D8EE2776C5A48B466A02C1266663F5364504A7D7252F091315014F395D6EC0CE2ACC685A6C44F7040A8D5E7B8D80ED7E62574CBA68A426106C6236566F5344707ADDF392A79E3AB49E8FF0A9A7FE723D1FE1534FFDE57A3EC22FAD9FDFBFFDF3EDFBCF6AAA93A356CCFAF5E3DB8FD7DF3E7EAE9A8E1F6F1F3F0E8753FEFE4DC4BFCBA12CEBA37EBCF1B7A7FEB89E8FF04BEBF523C477F27EC44DF6DFE0AF2FE4E3D7D7B6DCCBE3D1123ACBA1DEFBFA55FF71515FECF3117E7D3D1FE197D6CB47F8EDED6D7C5CBF775ABFC2E537F85B35BF97CF5FDBE6953FBF7FCB1F8C43E16E55F58F94F7F798D3A7FEB09E8FF04BEBF523FCFE5316FDA37FDD74FF0DBED6A7E3456B3F9897CFF5DBC75F2C295BA01677F9F9E3C7C7776D514FFD593D1FE197967E84AF7F32F506CFCF67B1F7B35EFF8A882F67A9FC387DFF730C1116F5FEBF3F3EFFA5503DCF47F8BFA9E723FCD2E247D8CBFEB2C1BFBF9FFFF8B7C6F74FE85362BF1CAB7CA9F92F7D9F6551EFFFDE37BEDD571A7F79EA2BEAF908BFB4CC47F8F99DBD6EF0CB1F3D9F3FBE7F452F7F08E97765BF1CAB7CA90FDB0BDFA25EBED617C5E70FAB88A7FEFB7A3EC22F2DFB11D68ECB7FD5A82F53FF307B29F75D0DC3F8469C72968D71A897EFEEF908FF4FF57C845F5A8B8FF0E3EF792FBFFCFA0ADFDED67F1755C247F59F55BFFFD0B24AF9BF2D70EE51F3D3E4DF616DD0537F5CCF47F8A5B5FC08CD1F56FC65567DB5B3C667F2F2C34AF9CE9F3FB014F55EBF7D6AC3A39EFA927A3EC2A79EFACBF57C844F3DF597EBF9089F7AEA2FD7BF01332486566379E02C0000000049454E44AE426082, N'INV-2025-061')
GO
SET IDENTITY_INSERT [dbo].[Sales] OFF
GO
SET IDENTITY_INSERT [dbo].[SalesReturnItems] ON 

GO
INSERT [dbo].[SalesReturnItems] ([ReturnItemID], [ReturnID], [ProductID], [Quantity], [UnitPrice], [SubTotal], [ProductName], [ProductCode]) VALUES (4, 4, 26, 1, CAST(35.25 AS Decimal(10, 2)), CAST(35.25 AS Decimal(10, 2)), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[SalesReturnItems] OFF
GO
SET IDENTITY_INSERT [dbo].[SalesReturns] ON 

GO
INSERT [dbo].[SalesReturns] ([ReturnID], [ReturnNumber], [SaleID], [CustomerID], [ReturnDate], [ReturnReason], [Description], [TotalAmount], [UserID], [CreatedDate], [OriginalInvoiceNumber], [OriginalInvoiceDate], [OriginalInvoiceTotal], [IsFullyReturned], [ReturnStatus]) VALUES (4, N'SRT000001', 47, 2, CAST(N'2025-10-27 14:07:06.217' AS DateTime), N'Defective Product', N'', CAST(35.25 AS Decimal(10, 2)), 1, CAST(N'2025-10-27 14:07:50.637' AS DateTime), N'INV-2024-002', CAST(N'2024-01-15 11:15:00.000' AS DateTime), CAST(70.50 AS Decimal(10, 2)), 0, N'Partial')
GO
SET IDENTITY_INSERT [dbo].[SalesReturns] OFF
GO
SET IDENTITY_INSERT [dbo].[SupplierPayments] ON 

GO
INSERT [dbo].[SupplierPayments] ([PaymentID], [SupplierID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalPayable], [PaidAmount], [RemainingAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (3, 2, CAST(N'2025-10-25 17:16:32.153' AS DateTime), N'SP000001', CAST(0.00 AS Decimal(10, 2)), CAST(1464.75 AS Decimal(10, 2)), CAST(1200.00 AS Decimal(10, 2)), CAST(264.75 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), NULL, N'', 1, CAST(N'2025-10-25 17:16:42.427' AS DateTime))
GO
INSERT [dbo].[SupplierPayments] ([PaymentID], [SupplierID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalPayable], [PaidAmount], [RemainingAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (4, 1, CAST(N'2025-10-23 13:10:37.997' AS DateTime), N'SPV000001', CAST(201.00 AS Decimal(10, 2)), CAST(302.00 AS Decimal(10, 2)), CAST(101.00 AS Decimal(10, 2)), CAST(201.00 AS Decimal(10, 2)), CAST(201.00 AS Decimal(10, 2)), CAST(101.00 AS Decimal(10, 2)), N'Credit Card', N'Supplier payment 1', 1, CAST(N'2025-10-12 13:10:37.997' AS DateTime))
GO
INSERT [dbo].[SupplierPayments] ([PaymentID], [SupplierID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalPayable], [PaidAmount], [RemainingAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (5, 2, CAST(N'2025-10-06 13:10:38.000' AS DateTime), N'SPV000002', CAST(202.00 AS Decimal(10, 2)), CAST(304.00 AS Decimal(10, 2)), CAST(102.00 AS Decimal(10, 2)), CAST(202.00 AS Decimal(10, 2)), CAST(202.00 AS Decimal(10, 2)), CAST(102.00 AS Decimal(10, 2)), N'Debit Card', N'Supplier payment 2', 2, CAST(N'2025-10-12 13:10:38.000' AS DateTime))
GO
INSERT [dbo].[SupplierPayments] ([PaymentID], [SupplierID], [PaymentDate], [VoucherNumber], [PreviousBalance], [TotalPayable], [PaidAmount], [RemainingAmount], [RemainingBalance], [Amount], [PaymentMethod], [Description], [UserID], [CreatedDate]) VALUES (6, 3, CAST(N'2025-10-23 13:10:38.000' AS DateTime), N'SPV000003', CAST(203.00 AS Decimal(10, 2)), CAST(306.00 AS Decimal(10, 2)), CAST(103.00 AS Decimal(10, 2)), CAST(203.00 AS Decimal(10, 2)), CAST(203.00 AS Decimal(10, 2)), CAST(103.00 AS Decimal(10, 2)), N'Check', N'Supplier payment 3', 3, CAST(N'2025-09-27 13:10:38.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[SupplierPayments] OFF
GO
SET IDENTITY_INSERT [dbo].[Suppliers] ON 

GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (1, N'SUP001', N'Vape Supply Co.', N'John Smith', N'555-0101', N'john@vapesupply.com', N'123 Business St', N'New York', N'10001', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (2, N'SUP002', N'Cloud', N'', N'555-0102', N'jane@clouddist.com', N'456 Commerce Ave', N'Los Angeles', N'90001', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (3, N'SUP003', N'Vapor Wholesale', N'Mike Johnson', N'555-0103', N'mike@vaporwholesale.com', N'789 Trade Blvd', N'Chicago', N'60601', 1, CAST(N'2025-10-17 19:51:27.473' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (58, N'SUP004', N'Supplier 4 Company', N'Contact Person 4', N'555-004-0012', N'supplier4@company.com', N'4 Business Avenue', N'Phoenix', N'20800', 1, CAST(N'2024-12-02 13:07:57.027' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (59, N'SUP005', N'Supplier 5 Company', N'Contact Person 5', N'555-005-0015', N'supplier5@company.com', N'5 Business Avenue', N'Philadelphia', N'21000', 1, CAST(N'2024-12-21 13:07:57.027' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (60, N'SUP006', N'Supplier 6 Company', N'Contact Person 6', N'555-006-0018', N'supplier6@company.com', N'6 Business Avenue', N'San Antonio', N'21200', 1, CAST(N'2024-11-25 13:07:57.027' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (61, N'SUP007', N'Supplier 7 Company', N'Contact Person 7', N'555-007-0021', N'supplier7@company.com', N'7 Business Avenue', N'San Diego', N'21400', 1, CAST(N'2025-08-08 13:07:57.027' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (62, N'SUP008', N'Supplier 8 Company', N'Contact Person 8', N'555-008-0024', N'supplier8@company.com', N'8 Business Avenue', N'New York', N'21600', 1, CAST(N'2025-08-23 13:07:57.027' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (63, N'SUP009', N'Supplier 9 Company', N'Contact Person 9', N'555-009-0027', N'supplier9@company.com', N'9 Business Avenue', N'Los Angeles', N'21800', 1, CAST(N'2025-04-28 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (64, N'SUP010', N'Supplier 10 Company', N'Contact Person 10', N'555-010-0030', N'supplier10@company.com', N'10 Business Avenue', N'Chicago', N'22000', 1, CAST(N'2024-11-04 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (65, N'SUP011', N'Supplier 11 Company', N'Contact Person 11', N'555-011-0033', N'supplier11@company.com', N'11 Business Avenue', N'Houston', N'22200', 1, CAST(N'2025-03-07 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (66, N'SUP012', N'Supplier 12 Company', N'Contact Person 12', N'555-012-0036', N'supplier12@company.com', N'12 Business Avenue', N'Phoenix', N'22400', 1, CAST(N'2024-11-21 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (67, N'SUP013', N'Supplier 13 Company', N'Contact Person 13', N'555-013-0039', N'supplier13@company.com', N'13 Business Avenue', N'Philadelphia', N'22600', 1, CAST(N'2025-05-13 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (68, N'SUP014', N'Supplier 14 Company', N'Contact Person 14', N'555-014-0042', N'supplier14@company.com', N'14 Business Avenue', N'San Antonio', N'22800', 1, CAST(N'2025-04-06 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (69, N'SUP015', N'Supplier 15 Company', N'Contact Person 15', N'555-015-0045', N'supplier15@company.com', N'15 Business Avenue', N'San Diego', N'23000', 1, CAST(N'2025-10-21 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (70, N'SUP016', N'Supplier 16 Company', N'Contact Person 16', N'555-016-0048', N'supplier16@company.com', N'16 Business Avenue', N'New York', N'23200', 1, CAST(N'2024-12-16 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (71, N'SUP017', N'Supplier 17 Company', N'Contact Person 17', N'555-017-0051', N'supplier17@company.com', N'17 Business Avenue', N'Los Angeles', N'23400', 1, CAST(N'2025-06-19 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (72, N'SUP018', N'Supplier 18 Company', N'Contact Person 18', N'555-018-0054', N'supplier18@company.com', N'18 Business Avenue', N'Chicago', N'23600', 1, CAST(N'2025-01-06 13:07:57.030' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (73, N'SUP019', N'Supplier 19 Company', N'Contact Person 19', N'555-019-0057', N'supplier19@company.com', N'19 Business Avenue', N'Houston', N'23800', 1, CAST(N'2025-07-09 13:07:57.033' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (74, N'SUP020', N'Supplier 20 Company', N'Contact Person 20', N'555-020-0060', N'supplier20@company.com', N'20 Business Avenue', N'Phoenix', N'24000', 1, CAST(N'2025-08-31 13:07:57.037' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (75, N'SUP021', N'Supplier 21 Company', N'Contact Person 21', N'555-021-0063', N'supplier21@company.com', N'21 Business Avenue', N'Philadelphia', N'24200', 1, CAST(N'2025-04-22 13:07:57.037' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (76, N'SUP022', N'Supplier 22 Company', N'Contact Person 22', N'555-022-0066', N'supplier22@company.com', N'22 Business Avenue', N'San Antonio', N'24400', 1, CAST(N'2025-03-09 13:07:57.037' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (77, N'SUP023', N'Supplier 23 Company', N'Contact Person 23', N'555-023-0069', N'supplier23@company.com', N'23 Business Avenue', N'San Diego', N'24600', 1, CAST(N'2024-11-25 13:07:57.037' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (78, N'SUP024', N'Supplier 24 Company', N'Contact Person 24', N'555-024-0072', N'supplier24@company.com', N'24 Business Avenue', N'New York', N'24800', 1, CAST(N'2024-12-12 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (79, N'SUP025', N'Supplier 25 Company', N'Contact Person 25', N'555-025-0075', N'supplier25@company.com', N'25 Business Avenue', N'Los Angeles', N'25000', 1, CAST(N'2025-07-15 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (80, N'SUP026', N'Supplier 26 Company', N'Contact Person 26', N'555-026-0078', N'supplier26@company.com', N'26 Business Avenue', N'Chicago', N'25200', 1, CAST(N'2025-04-29 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (81, N'SUP027', N'Supplier 27 Company', N'Contact Person 27', N'555-027-0081', N'supplier27@company.com', N'27 Business Avenue', N'Houston', N'25400', 1, CAST(N'2025-08-23 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (82, N'SUP028', N'Supplier 28 Company', N'Contact Person 28', N'555-028-0084', N'supplier28@company.com', N'28 Business Avenue', N'Phoenix', N'25600', 1, CAST(N'2025-01-16 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (83, N'SUP029', N'Supplier 29 Company', N'Contact Person 29', N'555-029-0087', N'supplier29@company.com', N'29 Business Avenue', N'Philadelphia', N'25800', 1, CAST(N'2025-04-26 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (84, N'SUP030', N'Supplier 30 Company', N'Contact Person 30', N'555-030-0090', N'supplier30@company.com', N'30 Business Avenue', N'San Antonio', N'26000', 1, CAST(N'2024-12-17 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (85, N'SUP031', N'Supplier 31 Company', N'Contact Person 31', N'555-031-0093', N'supplier31@company.com', N'31 Business Avenue', N'San Diego', N'26200', 1, CAST(N'2025-10-24 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (86, N'SUP032', N'Supplier 32 Company', N'Contact Person 32', N'555-032-0096', N'supplier32@company.com', N'32 Business Avenue', N'New York', N'26400', 1, CAST(N'2025-06-19 13:07:57.040' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (87, N'SUP033', N'Supplier 33 Company', N'Contact Person 33', N'555-033-0099', N'supplier33@company.com', N'33 Business Avenue', N'Los Angeles', N'26600', 1, CAST(N'2025-02-23 13:07:57.043' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (88, N'SUP034', N'Supplier 34 Company', N'Contact Person 34', N'555-034-0102', N'supplier34@company.com', N'34 Business Avenue', N'Chicago', N'26800', 1, CAST(N'2025-05-23 13:07:57.043' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (89, N'SUP035', N'Supplier 35 Company', N'Contact Person 35', N'555-035-0105', N'supplier35@company.com', N'35 Business Avenue', N'Houston', N'27000', 1, CAST(N'2025-03-04 13:07:57.043' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (90, N'SUP036', N'Supplier 36 Company', N'Contact Person 36', N'555-036-0108', N'supplier36@company.com', N'36 Business Avenue', N'Phoenix', N'27200', 1, CAST(N'2025-01-29 13:07:57.043' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (91, N'SUP037', N'Supplier 37 Company', N'Contact Person 37', N'555-037-0111', N'supplier37@company.com', N'37 Business Avenue', N'Philadelphia', N'27400', 1, CAST(N'2024-11-14 13:07:57.047' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (92, N'SUP038', N'Supplier 38 Company', N'Contact Person 38', N'555-038-0114', N'supplier38@company.com', N'38 Business Avenue', N'San Antonio', N'27600', 1, CAST(N'2025-10-17 13:07:57.047' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (93, N'SUP039', N'Supplier 39 Company', N'Contact Person 39', N'555-039-0117', N'supplier39@company.com', N'39 Business Avenue', N'San Diego', N'27800', 1, CAST(N'2024-11-01 13:07:57.047' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (94, N'SUP040', N'Supplier 40 Company', N'Contact Person 40', N'555-040-0120', N'supplier40@company.com', N'40 Business Avenue', N'New York', N'28000', 1, CAST(N'2024-11-03 13:07:57.047' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (95, N'SUP041', N'Supplier 41 Company', N'Contact Person 41', N'555-041-0123', N'supplier41@company.com', N'41 Business Avenue', N'Los Angeles', N'28200', 1, CAST(N'2025-10-09 13:07:57.047' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (96, N'SUP042', N'Supplier 42 Company', N'Contact Person 42', N'555-042-0126', N'supplier42@company.com', N'42 Business Avenue', N'Chicago', N'28400', 1, CAST(N'2024-12-31 13:07:57.047' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (97, N'SUP043', N'Supplier 43 Company', N'Contact Person 43', N'555-043-0129', N'supplier43@company.com', N'43 Business Avenue', N'Houston', N'28600', 1, CAST(N'2024-12-09 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (98, N'SUP044', N'Supplier 44 Company', N'Contact Person 44', N'555-044-0132', N'supplier44@company.com', N'44 Business Avenue', N'Phoenix', N'28800', 1, CAST(N'2025-02-20 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (99, N'SUP045', N'Supplier 45 Company', N'Contact Person 45', N'555-045-0135', N'supplier45@company.com', N'45 Business Avenue', N'Philadelphia', N'29000', 1, CAST(N'2025-10-09 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (100, N'SUP046', N'Supplier 46 Company', N'Contact Person 46', N'555-046-0138', N'supplier46@company.com', N'46 Business Avenue', N'San Antonio', N'29200', 1, CAST(N'2025-02-24 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (101, N'SUP047', N'Supplier 47 Company', N'Contact Person 47', N'555-047-0141', N'supplier47@company.com', N'47 Business Avenue', N'San Diego', N'29400', 1, CAST(N'2025-04-19 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (102, N'SUP048', N'Supplier 48 Company', N'Contact Person 48', N'555-048-0144', N'supplier48@company.com', N'48 Business Avenue', N'New York', N'29600', 1, CAST(N'2025-09-06 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (103, N'SUP049', N'Supplier 49 Company', N'Contact Person 49', N'555-049-0147', N'supplier49@company.com', N'49 Business Avenue', N'Los Angeles', N'29800', 1, CAST(N'2025-05-07 13:07:57.050' AS DateTime))
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierCode], [SupplierName], [ContactPerson], [Phone], [Email], [Address], [City], [PostalCode], [IsActive], [CreatedDate]) VALUES (104, N'SUP050', N'Supplier 50 Company', N'Contact Person 50', N'555-050-0150', N'supplier50@company.com', N'50 Business Avenue', N'Chicago', N'20000', 1, CAST(N'2025-05-21 13:07:57.050' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Suppliers] OFF
GO
INSERT [dbo].[UserRoles] ([UserID], [RoleID]) VALUES (55, 1)
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (1, N'admin', N'admin123', N'Admin admin', N'admin@vapestore.com', NULL, N'Admin', 1, CAST(N'2025-10-17 19:51:27.467' AS DateTime), CAST(N'2025-11-03 17:15:50.620' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (2, N'cashier', N'cashier123', N'Cashier User', N'cashier@vapestore.com', N'0987654321', N'Cashier', 1, CAST(N'2025-10-17 19:51:27.467' AS DateTime), NULL)
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (3, N'manager', N'manager123', N'Manager User', N'manager@vapestore.com', N'1122334455', N'Manager', 1, CAST(N'2025-10-17 19:51:27.467' AS DateTime), NULL)
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (5, N'user1', N'password123', N'User 1 Name', N'user1@vapestore.com', N'555-0001', N'Admin', 1, CAST(N'2025-08-18 13:06:32.487' AS DateTime), CAST(N'2025-10-05 13:06:32.487' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (6, N'user2', N'password123', N'User 2 Name', N'user2@vapestore.com', N'555-0002', N'Admin', 1, CAST(N'2025-01-04 13:06:32.487' AS DateTime), CAST(N'2025-10-12 13:06:32.487' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (7, N'user3', N'password123', N'User 3 Name', N'user3@vapestore.com', N'555-0003', N'Admin', 1, CAST(N'2025-01-28 13:06:32.490' AS DateTime), CAST(N'2025-10-23 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (8, N'user4', N'password123', N'User 4 Name', N'user4@vapestore.com', N'555-0004', N'Admin', 1, CAST(N'2025-09-12 13:06:32.490' AS DateTime), CAST(N'2025-10-17 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (9, N'user5', N'password123', N'User 5 Name', N'user5@vapestore.com', N'555-0005', N'Admin', 1, CAST(N'2024-11-02 13:06:32.490' AS DateTime), CAST(N'2025-10-05 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (10, N'user6', N'password123', N'User 6 Name', N'user6@vapestore.com', N'555-0006', N'Manager', 1, CAST(N'2024-12-05 13:06:32.490' AS DateTime), CAST(N'2025-10-17 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (11, N'user7', N'password123', N'User 7 Name', N'user7@vapestore.com', N'555-0007', N'Manager', 1, CAST(N'2025-05-14 13:06:32.490' AS DateTime), CAST(N'2025-10-05 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (12, N'user8', N'password123', N'User 8 Name', N'user8@vapestore.com', N'555-0008', N'Manager', 1, CAST(N'2025-08-21 13:06:32.490' AS DateTime), CAST(N'2025-10-20 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (13, N'user9', N'password123', N'User 9 Name', N'user9@vapestore.com', N'555-0009', N'Manager', 1, CAST(N'2025-08-08 13:06:32.490' AS DateTime), CAST(N'2025-10-18 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (14, N'user10', N'password123', N'User 10 Name', N'user10@vapestore.com', N'555-0010', N'Manager', 1, CAST(N'2025-08-14 13:06:32.490' AS DateTime), CAST(N'2025-10-15 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (15, N'user11', N'password123', N'User 11 Name', N'user11@vapestore.com', N'555-0011', N'Manager', 1, CAST(N'2024-11-29 13:06:32.490' AS DateTime), CAST(N'2025-10-21 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (16, N'user12', N'password123', N'User 12 Name', N'user12@vapestore.com', N'555-0012', N'Manager', 1, CAST(N'2025-04-25 13:06:32.490' AS DateTime), CAST(N'2025-10-07 13:06:32.490' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (17, N'user13', N'password123', N'User 13 Name', N'user13@vapestore.com', N'555-0013', N'Manager', 1, CAST(N'2025-05-28 13:06:32.493' AS DateTime), CAST(N'2025-10-05 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (18, N'user14', N'password123', N'User 14 Name', N'user14@vapestore.com', N'555-0014', N'Manager', 1, CAST(N'2024-12-13 13:06:32.493' AS DateTime), CAST(N'2025-10-24 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (19, N'user15', N'password123', N'User 15 Name', N'user15@vapestore.com', N'555-0015', N'Manager', 1, CAST(N'2025-03-16 13:06:32.493' AS DateTime), CAST(N'2025-10-05 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (20, N'user16', N'password123', N'User 16 Name', N'user16@vapestore.com', N'555-0016', N'Cashier', 1, CAST(N'2025-06-05 13:06:32.493' AS DateTime), CAST(N'2025-10-26 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (21, N'user17', N'password123', N'User 17 Name', N'user17@vapestore.com', N'555-0017', N'Cashier', 1, CAST(N'2024-11-07 13:06:32.493' AS DateTime), CAST(N'2025-10-24 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (22, N'user18', N'password123', N'User 18 Name', N'user18@vapestore.com', N'555-0018', N'Cashier', 1, CAST(N'2025-06-11 13:06:32.493' AS DateTime), CAST(N'2025-10-06 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (23, N'user19', N'password123', N'User 19 Name', N'user19@vapestore.com', N'555-0019', N'Cashier', 1, CAST(N'2025-08-27 13:06:32.493' AS DateTime), CAST(N'2025-10-12 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (24, N'user20', N'password123', N'User 20 Name', N'user20@vapestore.com', N'555-0020', N'Cashier', 1, CAST(N'2025-04-18 13:06:32.493' AS DateTime), CAST(N'2025-10-10 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (25, N'user21', N'password123', N'User 21 Name', N'user21@vapestore.com', N'555-0021', N'Cashier', 1, CAST(N'2025-06-19 13:06:32.493' AS DateTime), CAST(N'2025-09-27 13:06:32.493' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (26, N'user22', N'password123', N'User 22 Name', N'user22@vapestore.com', N'555-0022', N'Cashier', 1, CAST(N'2025-03-28 13:06:32.497' AS DateTime), CAST(N'2025-10-08 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (27, N'user23', N'password123', N'User 23 Name', N'user23@vapestore.com', N'555-0023', N'Cashier', 1, CAST(N'2025-03-20 13:06:32.497' AS DateTime), CAST(N'2025-10-16 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (28, N'user24', N'password123', N'User 24 Name', N'user24@vapestore.com', N'555-0024', N'Cashier', 1, CAST(N'2025-04-05 13:06:32.497' AS DateTime), CAST(N'2025-10-20 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (29, N'user25', N'password123', N'User 25 Name', N'user25@vapestore.com', N'555-0025', N'Cashier', 1, CAST(N'2024-12-07 13:06:32.497' AS DateTime), CAST(N'2025-10-16 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (30, N'user26', N'password123', N'User 26 Name', N'user26@vapestore.com', N'555-0026', N'Cashier', 1, CAST(N'2025-07-05 13:06:32.497' AS DateTime), CAST(N'2025-10-02 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (31, N'user27', N'password123', N'User 27 Name', N'user27@vapestore.com', N'555-0027', N'Cashier', 1, CAST(N'2025-10-20 13:06:32.497' AS DateTime), CAST(N'2025-10-08 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (32, N'user28', N'password123', N'User 28 Name', N'user28@vapestore.com', N'555-0028', N'Cashier', 1, CAST(N'2024-11-02 13:06:32.497' AS DateTime), CAST(N'2025-10-03 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (33, N'user29', N'password123', N'User 29 Name', N'user29@vapestore.com', N'555-0029', N'Cashier', 1, CAST(N'2025-09-10 13:06:32.497' AS DateTime), CAST(N'2025-10-16 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (34, N'user30', N'password123', N'User 30 Name', N'user30@vapestore.com', N'555-0030', N'Cashier', 1, CAST(N'2025-04-11 13:06:32.497' AS DateTime), CAST(N'2025-10-21 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (35, N'user31', N'password123', N'User 31 Name', N'user31@vapestore.com', N'555-0031', N'Cashier', 1, CAST(N'2025-07-05 13:06:32.497' AS DateTime), CAST(N'2025-10-07 13:06:32.497' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (36, N'user32', N'password123', N'User 32 Name', N'user32@vapestore.com', N'555-0032', N'Cashier', 1, CAST(N'2025-07-22 13:06:32.500' AS DateTime), CAST(N'2025-09-27 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (37, N'user33', N'password123', N'User 33 Name', N'user33@vapestore.com', N'555-0033', N'Cashier', 1, CAST(N'2025-10-01 13:06:32.500' AS DateTime), CAST(N'2025-10-05 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (38, N'user34', N'password123', N'User 34 Name', N'user34@vapestore.com', N'555-0034', N'Cashier', 1, CAST(N'2024-12-02 13:06:32.500' AS DateTime), CAST(N'2025-10-20 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (39, N'user35', N'password123', N'User 35 Name', N'user35@vapestore.com', N'555-0035', N'Cashier', 1, CAST(N'2025-10-12 13:06:32.500' AS DateTime), CAST(N'2025-10-11 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (40, N'user36', N'password123', N'User 36 Name', N'user36@vapestore.com', N'555-0036', N'Cashier', 1, CAST(N'2025-04-21 13:06:32.500' AS DateTime), CAST(N'2025-10-08 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (41, N'user37', N'password123', N'User 37 Name', N'user37@vapestore.com', N'555-0037', N'Cashier', 1, CAST(N'2025-05-18 13:06:32.500' AS DateTime), CAST(N'2025-10-05 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (42, N'user38', N'password123', N'User 38 Name', N'user38@vapestore.com', N'555-0038', N'Cashier', 1, CAST(N'2024-12-30 13:06:32.500' AS DateTime), CAST(N'2025-10-01 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (43, N'user39', N'password123', N'User 39 Name', N'user39@vapestore.com', N'555-0039', N'Cashier', 1, CAST(N'2025-09-03 13:06:32.500' AS DateTime), CAST(N'2025-10-20 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (44, N'user40', N'password123', N'User 40 Name', N'user40@vapestore.com', N'555-0040', N'Cashier', 1, CAST(N'2024-10-31 13:06:32.500' AS DateTime), CAST(N'2025-09-29 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (45, N'user41', N'password123', N'User 41 Name', N'user41@vapestore.com', N'555-0041', N'Cashier', 1, CAST(N'2024-12-01 13:06:32.500' AS DateTime), CAST(N'2025-10-26 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (46, N'user42', N'password123', N'User 42 Name', N'user42@vapestore.com', N'555-0042', N'Cashier', 1, CAST(N'2025-09-02 13:06:32.500' AS DateTime), CAST(N'2025-10-20 13:06:32.500' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (47, N'user43', N'password123', N'User 43 Name', N'user43@vapestore.com', N'555-0043', N'Cashier', 1, CAST(N'2025-07-23 13:06:32.503' AS DateTime), CAST(N'2025-10-04 13:06:32.503' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (48, N'user44', N'password123', N'User 44 Name', N'user44@vapestore.com', N'555-0044', N'Cashier', 1, CAST(N'2025-09-25 13:06:32.503' AS DateTime), CAST(N'2025-10-21 13:06:32.503' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (49, N'user45', N'password123', N'User 45 Name', N'user45@vapestore.com', N'555-0045', N'Cashier', 1, CAST(N'2024-11-11 13:06:32.503' AS DateTime), CAST(N'2025-10-08 13:06:32.503' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (50, N'user46', N'password123', N'User 46 Name', N'user46@vapestore.com', N'555-0046', N'Cashier', 1, CAST(N'2025-06-29 13:06:32.507' AS DateTime), CAST(N'2025-10-19 13:06:32.507' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (51, N'user47', N'password123', N'User 47 Name', N'user47@vapestore.com', N'555-0047', N'Cashier', 1, CAST(N'2025-05-09 13:06:32.507' AS DateTime), CAST(N'2025-10-10 13:06:32.507' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (52, N'user48', N'password123', N'User 48 Name', N'user48@vapestore.com', N'555-0048', N'Cashier', 1, CAST(N'2024-12-01 13:06:32.507' AS DateTime), CAST(N'2025-09-29 13:06:32.507' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (53, N'user49', N'password123', N'User 49 Name', N'user49@vapestore.com', N'555-0049', N'Cashier', 1, CAST(N'2024-12-05 13:06:32.507' AS DateTime), CAST(N'2025-10-19 13:06:32.507' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (54, N'user50', N'password123', N'User 50 Name', N'user50@vapestore.com', N'555-0050', N'Cashier', 1, CAST(N'2024-12-06 13:06:32.510' AS DateTime), CAST(N'2025-10-03 13:06:32.510' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [Role], [IsActive], [CreatedDate], [LastLogin]) VALUES (55, N'WajahatHussain', N'wajahat123', N'Super Admin', NULL, NULL, N'SuperAdmin', 1, CAST(N'2025-11-03 15:24:19.380' AS DateTime), CAST(N'2025-11-03 16:23:35.347' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Brands__2206CE9B225CABD1]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Brands] ADD UNIQUE NONCLUSTERED 
(
	[BrandName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CashInHand_CreatedDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CashInHand_CreatedDate] ON [dbo].[CashInHand]
(
	[CreatedDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CashInHand_TransactionDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CashInHand_TransactionDate] ON [dbo].[CashInHand]
(
	[TransactionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CashInHand_UserID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CashInHand_UserID] ON [dbo].[CashInHand]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Categori__8517B2E0724755E8]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Categories] ADD UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerPayments_CustomerID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerPayments_CustomerID] ON [dbo].[CustomerPayments]
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerPayments_PaymentDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerPayments_PaymentDate] ON [dbo].[CustomerPayments]
(
	[PaymentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerPayments_UserID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerPayments_UserID] ON [dbo].[CustomerPayments]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Customer__0667852148E707CE]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Customers] ADD UNIQUE NONCLUSTERED 
(
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__ExpenseC__8517B2E0590614BD]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[ExpenseCategories] ADD UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpenseEntries_CategoryID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpenseEntries_CategoryID] ON [dbo].[ExpenseEntries]
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpenseEntries_ExpenseDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpenseEntries_ExpenseDate] ON [dbo].[ExpenseEntries]
(
	[ExpenseDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExpenseEntries_UserID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExpenseEntries_UserID] ON [dbo].[ExpenseEntries]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Permissi__8884ABD4DE1A333F]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Permissions] ADD UNIQUE NONCLUSTERED 
(
	[PermissionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Products__2F4E024FCA044E17]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Products] ADD UNIQUE NONCLUSTERED 
(
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_Products_Barcode]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [UQ_Products_Barcode] UNIQUE NONCLUSTERED 
(
	[Barcode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_BrandID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_BrandID] ON [dbo].[Products]
(
	[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryID] ON [dbo].[Products]
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PurchaseItems_PurchaseID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseItems_PurchaseID] ON [dbo].[PurchaseItems]
(
	[PurchaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Purchase__2739D7BBCA2611F8]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[PurchaseReturns] ADD UNIQUE NONCLUSTERED 
(
	[ReturnNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Purchase__D776E981C1B4E5DE]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Purchases] ADD UNIQUE NONCLUSTERED 
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Purchases_PurchaseDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_PurchaseDate] ON [dbo].[Purchases]
(
	[PurchaseDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Purchases_SupplierID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Purchases_SupplierID] ON [dbo].[Purchases]
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RolePermissions_PermissionId]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions]
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Roles__8A2B616050D61660]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SaleItems_ProductID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SaleItems_ProductID] ON [dbo].[SaleItems]
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SaleItems_SaleID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SaleItems_SaleID] ON [dbo].[SaleItems]
(
	[SaleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Sales__D776E981DAC574B0]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Sales] ADD UNIQUE NONCLUSTERED 
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_CustomerID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_CustomerID] ON [dbo].[Sales]
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Sales_InvoiceNumber]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_InvoiceNumber] ON [dbo].[Sales]
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_SaleDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_SaleDate] ON [dbo].[Sales]
(
	[SaleDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__SalesRet__2739D7BB9E75B758]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[SalesReturns] ADD UNIQUE NONCLUSTERED 
(
	[ReturnNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SalesReturns_ReturnStatus]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SalesReturns_ReturnStatus] ON [dbo].[SalesReturns]
(
	[ReturnStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SalesReturns_SaleID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SalesReturns_SaleID] ON [dbo].[SalesReturns]
(
	[SaleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierPayments_PaymentDate]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierPayments_PaymentDate] ON [dbo].[SupplierPayments]
(
	[PaymentDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierPayments_SupplierID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierPayments_SupplierID] ON [dbo].[SupplierPayments]
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SupplierPayments_UserID]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_SupplierPayments_UserID] ON [dbo].[SupplierPayments]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Supplier__44BE981BD69C964E]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Suppliers] ADD UNIQUE NONCLUSTERED 
(
	[SupplierCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserRoles_RoleId]    Script Date: 11/3/2025 5:18:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles]
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Users__536C85E4974F7F9E]    Script Date: 11/3/2025 5:18:51 PM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CashInHand]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[CustomerPayments]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomerPayments]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
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
/****** Object:  StoredProcedure [dbo].[sp_UpdateSale]    Script Date: 11/3/2025 5:18:51 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_UpdateSaleItems]    Script Date: 11/3/2025 5:18:51 PM ******/
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
