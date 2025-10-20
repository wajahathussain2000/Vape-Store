using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.DataAccess;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class DatabaseStatisticsReport : Form
    {
        private SaleRepository _saleRepository;
        private PurchaseRepository _purchaseRepository;
        private ProductRepository _productRepository;
        private CustomerRepository _customerRepository;
        private SupplierRepository _supplierRepository;
        private ExpenseRepository _expenseRepository;
        private ReportingService _reportingService;

        public DatabaseStatisticsReport()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _purchaseRepository = new PurchaseRepository();
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository();
            _supplierRepository = new SupplierRepository();
            _expenseRepository = new ExpenseRepository();
            _reportingService = new ReportingService();
            
            SetupEventHandlers();
            InitializeDataGridView();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            this.Load += DatabaseStatisticsReport_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvStatistics.AutoGenerateColumns = false;
                dgvStatistics.AllowUserToAddRows = false;
                dgvStatistics.AllowUserToDeleteRows = false;
                dgvStatistics.ReadOnly = true;
                dgvStatistics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvStatistics.MultiSelect = false;

                // Define columns
                dgvStatistics.Columns.Clear();
                
                dgvStatistics.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Category",
                    HeaderText = "Category",
                    DataPropertyName = "Category",
                    Width = 200
                });
                
                dgvStatistics.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Metric",
                    HeaderText = "Metric",
                    DataPropertyName = "Metric",
                    Width = 300
                });
                
                dgvStatistics.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Value",
                    HeaderText = "Value",
                    DataPropertyName = "Value",
                    Width = 150
                });
                
                dgvStatistics.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastUpdated",
                    HeaderText = "Last Updated",
                    DataPropertyName = "LastUpdated",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy HH:mm" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            // Set default date range to last 30 days
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateDatabaseStatistics();
        }

        private void GenerateDatabaseStatistics()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1);
                
                var statistics = new List<DatabaseStatistic>();
                
                // Get basic counts
                statistics.AddRange(GetBasicCounts());
                
                // Get sales statistics
                statistics.AddRange(GetSalesStatistics(fromDate, toDate));
                
                // Get purchase statistics
                statistics.AddRange(GetPurchaseStatistics(fromDate, toDate));
                
                // Get inventory statistics
                statistics.AddRange(GetInventoryStatistics());
                
                // Get financial statistics
                statistics.AddRange(GetFinancialStatistics(fromDate, toDate));
                
                // Get customer/supplier statistics
                statistics.AddRange(GetCustomerSupplierStatistics());
                
                // Get expense statistics
                statistics.AddRange(GetExpenseStatistics(fromDate, toDate));
                
                // Bind to DataGridView
                dgvStatistics.DataSource = statistics;
                UpdateSummaryLabels(statistics);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating database statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private List<DatabaseStatistic> GetBasicCounts()
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Total sales count
                    var salesCount = GetCount(connection, "SELECT COUNT(*) FROM Sales");
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Sales",
                        Metric = "Total Sales Transactions",
                        Value = salesCount.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Total purchases count
                    var purchasesCount = GetCount(connection, "SELECT COUNT(*) FROM Purchases");
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Purchases",
                        Metric = "Total Purchase Transactions",
                        Value = purchasesCount.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Total products count
                    var productsCount = GetCount(connection, "SELECT COUNT(*) FROM Products WHERE IsActive = 1");
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Inventory",
                        Metric = "Total Active Products",
                        Value = productsCount.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Total customers count
                    var customersCount = GetCount(connection, "SELECT COUNT(*) FROM Customers WHERE IsActive = 1");
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Customers",
                        Metric = "Total Active Customers",
                        Value = customersCount.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Total suppliers count
                    var suppliersCount = GetCount(connection, "SELECT COUNT(*) FROM Suppliers WHERE IsActive = 1");
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Suppliers",
                        Metric = "Total Active Suppliers",
                        Value = suppliersCount.ToString(),
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting basic counts: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private List<DatabaseStatistic> GetSalesStatistics(DateTime fromDate, DateTime toDate)
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Sales in date range
                    var salesInRange = GetCount(connection, 
                        "SELECT COUNT(*) FROM Sales WHERE SaleDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Sales",
                        Metric = $"Sales in Period ({fromDate:MM/dd/yyyy} - {toDate:MM/dd/yyyy})",
                        Value = salesInRange.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Total sales amount in date range
                    var totalSalesAmount = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE SaleDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Sales",
                        Metric = $"Total Sales Amount in Period",
                        Value = $"${totalSalesAmount:F2}",
                        LastUpdated = DateTime.Now
                    });

                    // Average sale amount
                    var avgSaleAmount = GetDecimal(connection, 
                        "SELECT ISNULL(AVG(TotalAmount), 0) FROM Sales WHERE SaleDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Sales",
                        Metric = "Average Sale Amount",
                        Value = $"${avgSaleAmount:F2}",
                        LastUpdated = DateTime.Now
                    });

                    // Top selling product
                    var topProduct = GetString(connection, 
                        @"SELECT TOP 1 p.ProductName 
                          FROM SaleItems si 
                          INNER JOIN Products p ON si.ProductID = p.ProductID 
                          INNER JOIN Sales s ON si.SaleID = s.SaleID 
                          WHERE s.SaleDate BETWEEN @FromDate AND @ToDate 
                          GROUP BY p.ProductName 
                          ORDER BY SUM(si.Quantity) DESC", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Sales",
                        Metric = "Top Selling Product",
                        Value = topProduct ?? "No sales in period",
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting sales statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private List<DatabaseStatistic> GetPurchaseStatistics(DateTime fromDate, DateTime toDate)
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Purchases in date range
                    var purchasesInRange = GetCount(connection, 
                        "SELECT COUNT(*) FROM Purchases WHERE PurchaseDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Purchases",
                        Metric = $"Purchases in Period ({fromDate:MM/dd/yyyy} - {toDate:MM/dd/yyyy})",
                        Value = purchasesInRange.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Total purchase amount
                    var totalPurchaseAmount = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(TotalAmount), 0) FROM Purchases WHERE PurchaseDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Purchases",
                        Metric = "Total Purchase Amount in Period",
                        Value = $"${totalPurchaseAmount:F2}",
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting purchase statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private List<DatabaseStatistic> GetInventoryStatistics()
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Total inventory value
                    var totalInventoryValue = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(StockQuantity * UnitPrice), 0) FROM Products WHERE IsActive = 1");
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Inventory",
                        Metric = "Total Inventory Value",
                        Value = $"${totalInventoryValue:F2}",
                        LastUpdated = DateTime.Now
                    });

                    // Low stock items
                    var lowStockCount = GetCount(connection, 
                        "SELECT COUNT(*) FROM Products WHERE StockQuantity <= ReorderLevel AND IsActive = 1");
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Inventory",
                        Metric = "Low Stock Items",
                        Value = lowStockCount.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Out of stock items
                    var outOfStockCount = GetCount(connection, 
                        "SELECT COUNT(*) FROM Products WHERE StockQuantity = 0 AND IsActive = 1");
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Inventory",
                        Metric = "Out of Stock Items",
                        Value = outOfStockCount.ToString(),
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting inventory statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private List<DatabaseStatistic> GetFinancialStatistics(DateTime fromDate, DateTime toDate)
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Total sales revenue
                    var totalRevenue = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE SaleDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    // Total purchase costs
                    var totalCosts = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(TotalAmount), 0) FROM Purchases WHERE PurchaseDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    // Total expenses
                    var totalExpenses = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(Amount), 0) FROM ExpenseEntries WHERE ExpenseDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    // Gross profit
                    var grossProfit = totalRevenue - totalCosts;
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Financial",
                        Metric = "Total Revenue",
                        Value = $"${totalRevenue:F2}",
                        LastUpdated = DateTime.Now
                    });
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Financial",
                        Metric = "Total Purchase Costs",
                        Value = $"${totalCosts:F2}",
                        LastUpdated = DateTime.Now
                    });
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Financial",
                        Metric = "Total Expenses",
                        Value = $"${totalExpenses:F2}",
                        LastUpdated = DateTime.Now
                    });
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Financial",
                        Metric = "Gross Profit",
                        Value = $"${grossProfit:F2}",
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting financial statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private List<DatabaseStatistic> GetCustomerSupplierStatistics()
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Customers with outstanding balances
                    var customersWithBalance = GetCount(connection, 
                        @"SELECT COUNT(DISTINCT c.CustomerID) 
                          FROM Customers c 
                          INNER JOIN Sales s ON c.CustomerID = s.CustomerID 
                          WHERE s.TotalAmount > s.PaidAmount");
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Customers",
                        Metric = "Customers with Outstanding Balances",
                        Value = customersWithBalance.ToString(),
                        LastUpdated = DateTime.Now
                    });

                    // Suppliers with outstanding balances
                    var suppliersWithBalance = GetCount(connection, 
                        @"SELECT COUNT(DISTINCT s.SupplierID) 
                          FROM Suppliers s 
                          INNER JOIN Purchases p ON s.SupplierID = p.SupplierID 
                          WHERE p.TotalAmount > p.PaidAmount");
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Suppliers",
                        Metric = "Suppliers with Outstanding Balances",
                        Value = suppliersWithBalance.ToString(),
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting customer/supplier statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private List<DatabaseStatistic> GetExpenseStatistics(DateTime fromDate, DateTime toDate)
        {
            var statistics = new List<DatabaseStatistic>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Total expenses in period
                    var totalExpenses = GetDecimal(connection, 
                        "SELECT ISNULL(SUM(Amount), 0) FROM ExpenseEntries WHERE ExpenseDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Expenses",
                        Metric = "Total Expenses in Period",
                        Value = $"${totalExpenses:F2}",
                        LastUpdated = DateTime.Now
                    });

                    // Expense categories count
                    var expenseCategoriesCount = GetCount(connection, 
                        "SELECT COUNT(DISTINCT CategoryID) FROM ExpenseEntries WHERE ExpenseDate BETWEEN @FromDate AND @ToDate", 
                        new SqlParameter("@FromDate", fromDate), 
                        new SqlParameter("@ToDate", toDate));
                    
                    statistics.Add(new DatabaseStatistic
                    {
                        Category = "Expenses",
                        Metric = "Active Expense Categories",
                        Value = expenseCategoriesCount.ToString(),
                        LastUpdated = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error getting expense statistics: {ex.Message}", "Error", MessageBoxIcon.Error);
            }

            return statistics;
        }

        private int GetCount(SqlConnection connection, string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                    
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private decimal GetDecimal(SqlConnection connection, string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                    
                return Convert.ToDecimal(command.ExecuteScalar());
            }
        }

        private string GetString(SqlConnection connection, string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
                
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                    
                var result = command.ExecuteScalar();
                return result?.ToString();
            }
        }

        private void UpdateSummaryLabels(List<DatabaseStatistic> statistics)
        {
            try
            {
                var totalCategories = statistics.Select(s => s.Category).Distinct().Count();
                var totalMetrics = statistics.Count;
                var lastUpdated = statistics.Max(s => s.LastUpdated);
                
                lblTotalCategories.Text = $"Categories: {totalCategories}";
                lblTotalMetrics.Text = $"Metrics: {totalMetrics}";
                lblLastUpdated.Text = $"Last Updated: {lastUpdated:MM/dd/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStatistics.DataSource == null)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"DatabaseStatistics_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    ShowMessage("Report exported successfully!", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting report: {ex.Message}", "Export Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStatistics.DataSource == null)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement PDF export functionality
                ShowMessage("PDF export functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStatistics.DataSource == null)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement print functionality
                ShowMessage("Print functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            dgvStatistics.DataSource = null;
            lblTotalCategories.Text = "Categories: 0";
            lblTotalMetrics.Text = "Metrics: 0";
            lblLastUpdated.Text = "Last Updated: Never";
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Category,Metric,Value,Last Updated");
                
                // Write data
                var dataSource = (List<DatabaseStatistic>)dgvStatistics.DataSource;
                foreach (var item in dataSource)
                {
                    writer.WriteLine($"{item.Category},{item.Metric},{item.Value},{item.LastUpdated:yyyy-MM-dd HH:mm}");
                }
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void DatabaseStatisticsReport_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }
    }

    public class DatabaseStatistic
    {
        public string Category { get; set; }
        public string Metric { get; set; }
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

