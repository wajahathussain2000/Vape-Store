# Vape Store Database Setup

This directory contains all the necessary files to set up the Vape Store database on your SQL Server instance.

## Prerequisites

- SQL Server Express (DESKTOP-KPKLR5V\SQLEXPRESS)
- SQL Server Management Studio (SSMS) or sqlcmd utility
- Windows Authentication enabled

## Database Structure

The VapeStore database includes the following tables:

### Core Tables
- **Users** - Application users and authentication
- **Categories** - Product categories
- **Brands** - Product brands
- **Suppliers** - Supplier information
- **Customers** - Customer information
- **Products** - Product catalog

### Transaction Tables
- **Sales** - Sales transactions
- **SaleItems** - Individual items in sales
- **Purchases** - Purchase transactions
- **PurchaseItems** - Individual items in purchases
- **SalesReturns** - Sales return transactions
- **SalesReturnItems** - Items in sales returns
- **PurchaseReturns** - Purchase return transactions
- **PurchaseReturnItems** - Items in purchase returns

### Financial Tables
- **CustomerPayments** - Customer payment records
- **SupplierPayments** - Supplier payment records
- **ExpenseCategories** - Expense categories
- **ExpenseEntries** - Expense transactions
- **CashInHand** - Cash flow tracking

## Setup Instructions

### Method 1: Using the Batch File (Recommended)
1. Open Command Prompt as Administrator
2. Navigate to the Database folder
3. Run: `SetupDatabase.bat`
4. Follow the prompts

### Method 2: Manual Setup
1. Open SQL Server Management Studio
2. Connect to `DESKTOP-KPKLR5V\SQLEXPRESS`
3. Open and execute `CreateDatabase.sql`
4. Open and execute `InsertSampleData.sql`

### Method 3: Using sqlcmd
```bash
# Create database and tables
sqlcmd -S "DESKTOP-KPKLR5V\SQLEXPRESS" -i "CreateDatabase.sql"

# Insert sample data
sqlcmd -S "DESKTOP-KPKLR5V\SQLEXPRESS" -d "VapeStore" -i "InsertSampleData.sql"
```

## Sample Data

The database includes sample data for:
- 3 Users (admin, cashier, manager)
- 5 Categories (E-Liquids, Vape Devices, Accessories, Coils, Batteries)
- 5 Brands (SMOK, Vaporesso, GeekVape, Uwell, Aspire)
- 3 Suppliers
- 5 Customers
- 8 Products
- Sample sales, purchases, and expense transactions

## Connection String

The application is configured to use:
```
Server=DESKTOP-KPKLR5V\SQLEXPRESS;Database=VapeStore;Integrated Security=true;TrustServerCertificate=true;
```

## Default Login Credentials

- **Username:** admin
- **Password:** admin123
- **Role:** Admin

- **Username:** cashier
- **Password:** cashier123
- **Role:** Cashier

- **Username:** manager
- **Password:** manager123
- **Role:** Manager

## Troubleshooting

### Common Issues:

1. **SQL Server not running:**
   - Start SQL Server service from Services.msc
   - Or use SQL Server Configuration Manager

2. **Permission denied:**
   - Run Command Prompt as Administrator
   - Ensure your Windows account has SQL Server access

3. **Database already exists:**
   - The script will drop and recreate the database
   - Backup any important data before running

4. **Connection issues:**
   - Verify SQL Server is running
   - Check Windows Authentication is enabled
   - Verify the server name is correct

## File Descriptions

- `CreateDatabase.sql` - Creates database and all tables
- `InsertSampleData.sql` - Inserts sample data for testing
- `SetupDatabase.bat` - Automated setup script
- `ConnectionString.txt` - Connection string information
- `README.md` - This documentation file

## Next Steps

After setting up the database:
1. Run your Vape Store application
2. Login with admin credentials
3. Test all functionality
4. Customize data as needed

## Support

If you encounter any issues:
1. Check SQL Server is running
2. Verify connection string in App.config
3. Ensure all SQL scripts executed successfully
4. Check Windows Event Viewer for errors
