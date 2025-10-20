-- Vape Store Database Creation Script (Simplified)
-- Server: DESKTOP-KPKLR5V\SQLEXPRESS
-- Database: VapeStore

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'VapeStore')
BEGIN
    ALTER DATABASE VapeStore SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE VapeStore;
END
GO

-- Create VapeStore Database (simplified)
CREATE DATABASE VapeStore;
GO

-- Use the new database
USE VapeStore;
GO

-- Create Users Table
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(20) DEFAULT 'User',
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME
);
GO

-- Create Categories Table
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create Brands Table
CREATE TABLE Brands (
    BrandID INT IDENTITY(1,1) PRIMARY KEY,
    BrandName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create Suppliers Table
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode NVARCHAR(20) NOT NULL UNIQUE,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    City NVARCHAR(50),
    PostalCode NVARCHAR(10),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create Customers Table
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerCode NVARCHAR(20) NOT NULL UNIQUE,
    CustomerName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    City NVARCHAR(50),
    PostalCode NVARCHAR(10),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create Products Table
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(20) NOT NULL UNIQUE,
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
    BrandID INT FOREIGN KEY REFERENCES Brands(BrandID),
    PurchasePrice DECIMAL(10,2) NOT NULL,
    RetailPrice DECIMAL(10,2) NOT NULL,
    StockQuantity INT DEFAULT 0,
    ReorderLevel INT DEFAULT 0,
    Barcode NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create Sales Table
CREATE TABLE Sales (
    SaleID INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(20) NOT NULL UNIQUE,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    SaleDate DATETIME DEFAULT GETDATE(),
    SubTotal DECIMAL(10,2) NOT NULL,
    TaxAmount DECIMAL(10,2) DEFAULT 0,
    TaxPercent DECIMAL(5,2) DEFAULT 0,
    TotalAmount DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(20),
    PaidAmount DECIMAL(10,2) DEFAULT 0,
    ChangeAmount DECIMAL(10,2) DEFAULT 0,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create SaleItems Table
CREATE TABLE SaleItems (
    SaleItemID INT IDENTITY(1,1) PRIMARY KEY,
    SaleID INT FOREIGN KEY REFERENCES Sales(SaleID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    SubTotal DECIMAL(10,2) NOT NULL
);
GO

-- Create Purchases Table
CREATE TABLE Purchases (
    PurchaseID INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(20) NOT NULL UNIQUE,
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    PurchaseDate DATETIME DEFAULT GETDATE(),
    SubTotal DECIMAL(10,2) NOT NULL,
    TaxAmount DECIMAL(10,2) DEFAULT 0,
    TaxPercent DECIMAL(5,2) DEFAULT 0,
    TotalAmount DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(20),
    PaidAmount DECIMAL(10,2) DEFAULT 0,
    ChangeAmount DECIMAL(10,2) DEFAULT 0,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create PurchaseItems Table
CREATE TABLE PurchaseItems (
    PurchaseItemID INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseID INT FOREIGN KEY REFERENCES Purchases(PurchaseID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    SubTotal DECIMAL(10,2) NOT NULL
);
GO

-- Create SalesReturns Table
CREATE TABLE SalesReturns (
    ReturnID INT IDENTITY(1,1) PRIMARY KEY,
    ReturnNumber NVARCHAR(20) NOT NULL UNIQUE,
    SaleID INT FOREIGN KEY REFERENCES Sales(SaleID),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    ReturnDate DATETIME DEFAULT GETDATE(),
    ReturnReason NVARCHAR(255),
    Description NVARCHAR(255),
    TotalAmount DECIMAL(10,2) NOT NULL,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create SalesReturnItems Table
CREATE TABLE SalesReturnItems (
    ReturnItemID INT IDENTITY(1,1) PRIMARY KEY,
    ReturnID INT FOREIGN KEY REFERENCES SalesReturns(ReturnID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    SubTotal DECIMAL(10,2) NOT NULL
);
GO

-- Create PurchaseReturns Table
CREATE TABLE PurchaseReturns (
    ReturnID INT IDENTITY(1,1) PRIMARY KEY,
    ReturnNumber NVARCHAR(20) NOT NULL UNIQUE,
    PurchaseID INT FOREIGN KEY REFERENCES Purchases(PurchaseID),
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    ReturnDate DATETIME DEFAULT GETDATE(),
    ReturnReason NVARCHAR(255),
    Description NVARCHAR(255),
    TotalAmount DECIMAL(10,2) NOT NULL,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create PurchaseReturnItems Table
CREATE TABLE PurchaseReturnItems (
    ReturnItemID INT IDENTITY(1,1) PRIMARY KEY,
    ReturnID INT FOREIGN KEY REFERENCES PurchaseReturns(ReturnID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    SubTotal DECIMAL(10,2) NOT NULL
);
GO

-- Create CustomerPayments Table
CREATE TABLE CustomerPayments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    PaymentDate DATETIME DEFAULT GETDATE(),
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(20),
    Description NVARCHAR(255),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create SupplierPayments Table
CREATE TABLE SupplierPayments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    PaymentDate DATETIME DEFAULT GETDATE(),
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(20),
    Description NVARCHAR(255),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create ExpenseCategories Table
CREATE TABLE ExpenseCategories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create ExpenseEntries Table
CREATE TABLE ExpenseEntries (
    ExpenseID INT IDENTITY(1,1) PRIMARY KEY,
    ExpenseDate DATETIME DEFAULT GETDATE(),
    CategoryID INT FOREIGN KEY REFERENCES ExpenseCategories(CategoryID),
    Amount DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(255),
    PaymentMethod NVARCHAR(20),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Create CashInHand Table
CREATE TABLE CashInHand (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    TransactionDate DATETIME DEFAULT GETDATE(),
    TransactionType NVARCHAR(20) NOT NULL, -- 'Income' or 'Expense'
    Amount DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(255),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

PRINT 'VapeStore database and all tables created successfully!';
GO
