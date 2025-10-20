@echo off
echo Setting up Vape Store Database...
echo.

echo Step 1: Creating Database and Tables...
sqlcmd -S "DESKTOP-KPKLR5V\SQLEXPRESS" -i "CreateDatabase.sql"
if %errorlevel% neq 0 (
    echo Error creating database and tables!
    pause
    exit /b 1
)

echo.
echo Step 2: Inserting Sample Data...
sqlcmd -S "DESKTOP-KPKLR5V\SQLEXPRESS" -d "VapeStore" -i "InsertSampleData.sql"
if %errorlevel% neq 0 (
    echo Error inserting sample data!
    pause
    exit /b 1
)

echo.
echo Database setup completed successfully!
echo.
echo Database: VapeStore
echo Server: DESKTOP-KPKLR5V\SQLEXPRESS
echo.
echo You can now run your Vape Store application.
pause
