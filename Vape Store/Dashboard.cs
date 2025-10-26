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
using System.Configuration;
using Vape_Store.Services;
using Vape_Store.Models;
using Vape_Store.Repositories;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vape_Store
{
    public partial class Dashboard : Form
    {
        private ContextMenuStrip SalesDropdown; //creating ContextMenuStrip
        private ContextMenuStrip PurchaseDropdown;
        private ContextMenuStrip InventoryDropdown;
        private ContextMenuStrip PeopleDropdown;
        private ContextMenuStrip AccountsDropdown;
        private ContextMenuStrip ReportsDropdown;
        private ContextMenuStrip UtilitiesDropdown;
        private ContextMenuStrip UsersDropdown;

        // Dashboard data properties
        private string connectionString;
        private Timer refreshTimer;
        private List<RecentActivity> recentActivities;
        private DashboardStats currentStats;
        private DashboardService _dashboardService;

        private void InitializeDropdownMenus() //method for adding list in the all Menu dropdown.
        {
            SalesDropdown = new ContextMenuStrip();
            SalesDropdown.Items.Add("New Sale",null, (s,e) => OpenNewSaleForm());
            SalesDropdown.Items.Add("Sales Return", null, (s, e) => OpenSalesReturnForm());
            SalesDropdown.Items.Add("Edit Sales",null, (s,e) => OpenEditSalesForm());
            SalesDropdown.Items.Add("Sales Ledger",null, (s,e) => OpenSalesLedgerForm());

            PurchaseDropdown= new ContextMenuStrip();  
            PurchaseDropdown.Items.Add("Add Purchase",null, (s,e) => OpenAddPurchaseForm());
            PurchaseDropdown.Items.Add("Purchase Returns", null, (s, e) => OpenPurchaseReturnForm());
            PurchaseDropdown.Items.Add("Purchase Ledger",null, (s,e) => OpenPurchaseLedgerForm());

            InventoryDropdown = new ContextMenuStrip();
            InventoryDropdown.Items.Add("Products", null, (s, e) => OpenProductsForm());
            InventoryDropdown.Items.Add("Categories", null, (s, e) => OpenCategoriesForm());
            InventoryDropdown.Items.Add("Brands", null, (s, e) => OpenBrandsForm());
            InventoryDropdown.Items.Add("Print Barcode", null, (s, e) => OpenPrintBarCodeForm());
            InventoryDropdown.Items.Add("Stock in Hand", null, (s, e) => OpenStockInHandForm());

            PeopleDropdown = new ContextMenuStrip();
            PeopleDropdown.Items.Add("Customers", null, (s, e) => OpenCustomersForm());
            PeopleDropdown.Items.Add("Suppliers", null, (s, e) => OpenSuppliersForm());
            PeopleDropdown.Items.Add("Users", null, (s, e) => OpenUsersForm());

            AccountsDropdown = new ContextMenuStrip();
            AccountsDropdown.Items.Add("Cash in Hand", null, (s, e) => OpenCashinHandForm());
            AccountsDropdown.Items.Add("Expense Entry", null, (s, e) => OpenExpenseEntryForm());
            AccountsDropdown.Items.Add("Customer Payments", null, (s, e) => OpenCustomerPaymentForm());
            AccountsDropdown.Items.Add("Supplier Payments", null, (s, e) => OpenSupplierPaymentForm());

            ReportsDropdown = new ContextMenuStrip();
            ReportsDropdown.Items.Add("Stock Report", null, (s, e) => OpenStockReportForm());
            ReportsDropdown.Items.Add("Low Stock Report", null, (s, e) => OpenLowStockReportForm());
            ReportsDropdown.Items.Add("Stock In Hand Report", null, (s, e) => OpenStockInHandForm());

            ReportsDropdown.Items.Add("Summary Report", null, (s, e) => OpenSummaryReportForm());
            ReportsDropdown.Items.Add("Sales Report", null, (s, e) => OpenSalesReportForm());
            ReportsDropdown.Items.Add("Items-wise Sales Report", null, (s, e) => OpenItemsSalesReportForm());
            ReportsDropdown.Items.Add("Sales Return Report", null, (s, e) => OpenSalesReturnReportForm());
            ReportsDropdown.Items.Add("Sales Register/Ledger", null, (s, e) => OpenSalesLedgerForm());

            ReportsDropdown.Items.Add("Purchase Report", null, (s, e) => OpenPurchaseReportForm());
            ReportsDropdown.Items.Add("Purchase Return Report", null, (s, e) => OpenPurchaseReturnReportForm());

            ReportsDropdown.Items.Add("Profit Margin Report", null, (s, e) => OpenProfitMarginReportForm());
            ReportsDropdown.Items.Add("Profit And Loss Report", null, (s, e) => OpenProfitAndLossForm());
            ReportsDropdown.Items.Add("Database Statistics", null, (s, e) => OpenDatabaseStatisticsForm());



            UtilitiesDropdown = new ContextMenuStrip();
            UtilitiesDropdown.Items.Add("Thermal Invoice", null, (s, e) => OpenThermalInvoiceForm());
            UtilitiesDropdown.Items.Add("DataBase Backup", null, (s, e) => OpenDataBaseBackupForm());

            UsersDropdown = new ContextMenuStrip();
            UsersDropdown.Items.Add("User Management", null, (s, e) => OpenUserManagementForm());
            UsersDropdown.Items.Add("User Access", null, (s, e) => OpenUserAccessForm());

        }
        private void OpenNewSaleForm() //method for opening the product form 
        {
            NewSale newsale = new NewSale();
            newsale.FormClosed += (s, e) => RefreshDashboardData();
            newsale.Show();
        }
       
        private void OpenSalesLedgerForm()
        {
            try
            {
                var salesLedgerForm = new SalesLedgerForm();
                salesLedgerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening sales ledger: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenEditSalesForm()
        {
            EditSale editsale = new EditSale();
            editsale.FormClosed += (s, e) => RefreshDashboardData();
            editsale.Show();
        }
        private void OpenSalesReturnForm()
        {
            SalesReturnForm salesreturn = new SalesReturnForm();
            salesreturn.FormClosed += (s, e) => RefreshDashboardData();
            salesreturn.Show();
        }
        private void OpenAddPurchaseForm()
        { 
           NewPurchase newPurchase= new NewPurchase();  
            newPurchase.FormClosed += (s, e) => RefreshDashboardData();
            newPurchase.Show(); 

        }
        private void OpenPurchaseLedgerForm()
        {
            PurchaseReportForm purchaseReport = new PurchaseReportForm();
            purchaseReport.Show();
        }

        private void OpenPurchaseReturnForm()
        {
           PurchaseReturnForm purchaseReturn= new PurchaseReturnForm(); 
            purchaseReturn.FormClosed += (s, e) => RefreshDashboardData();
            purchaseReturn.Show();  

        }

        private void OpenProductsForm()
        {
            Products products = new Products(); 
            products.Show();            

        }
        private void OpenCategoriesForm()
        {
            Categories categories = new Categories();
            categories.Show();

        }
        private void OpenBrandsForm()
        {
            Brands brands = new Brands();   
            brands.Show();

        }
        private void OpenPrintBarCodeForm()
        {
            try
            {
                // Open Products form to allow barcode printing
                var productsForm = new Products();
                productsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening barcode printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenStockInHandForm()
        {
            StockReportForm stockReport = new StockReportForm();
            stockReport.Show();
        }
        private void OpenCustomersForm()
        {
            Customers customers = new Customers();  
            customers.Show();   

        }
        private void OpenSuppliersForm()
        {
            Supplier_Master suppliers = new Supplier_Master();  
            suppliers.Show();

        }
        private void OpenUsersForm()
        {
            Users users = new Users();
            users.Show();
        }
        private void OpenCashinHandForm()
        {
            Cash_in_Hand cash_In_Hand = new Cash_in_Hand(); 
            cash_In_Hand.Show();    

        }
        private void OpenExpenseEntryForm()
        {
          ExpenseEntry expense_entry = new ExpenseEntry();  
            expense_entry.Show();   
        }
        private void OpenCustomerPaymentForm()
        {
            CustomerPaymentForm customerPayment = new CustomerPaymentForm();    
            customerPayment.Show(); 
        }
        private void OpenSupplierPaymentForm()
        {
            SupplierPaymentForm supplierPayment = new SupplierPaymentForm();
            supplierPayment.Show(); 
        }

        private void OpenProfitAndLossForm()
        {
            try
            {
                var profitLossReport = new ProfitAndLossReportForm();
                profitLossReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening profit and loss report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenStockReportForm()
        {
            StockReportForm stockReport = new StockReportForm();
            stockReport.Show();
        }

        private void OpenLowStockReportForm()
        {
            LowStockReportForm lowStockReport = new LowStockReportForm();
            lowStockReport.Show();
        }

        private void OpenSummaryReportForm()
        {
            ProfitAndLossReportForm profitLossReport = new ProfitAndLossReportForm();
            profitLossReport.Show();
        }

        private void OpenSalesReportForm()
        {
            SalesReportForm salesReport = new SalesReportForm();
            salesReport.Show();
        }

        private void OpenItemsSalesReportForm()
        {
            try
            {
                // Create a custom item-wise sales report
                var itemwiseReport = new SalesReportForm();
                // Set to show item-wise data by default
                itemwiseReport.SetItemWiseMode();
                itemwiseReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening item-wise sales report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenSalesReturnReportForm()
        {
            try
            {
                // Create a specialized sales return report
                var salesReturnReport = new SalesReturnReportForm();
                salesReturnReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening sales return report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenPurchaseReportForm()
        {
            PurchaseReportForm purchaseReport = new PurchaseReportForm();
            purchaseReport.Show();
        }

        private void OpenPurchaseReturnReportForm()
        {
            try
            {
                // Create a specialized purchase return report
                var purchaseReturnReport = new PurchaseReturnReportForm();
                purchaseReturnReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening purchase return report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenProfitMarginReportForm()
        {
            ProfitAndLossReportForm profitLossReport = new ProfitAndLossReportForm();
            profitLossReport.Show();
        }

        private void OpenExpenseReportForm()
        {
            ExpenseReportForm expenseReport = new ExpenseReportForm();
            expenseReport.Show();
        }

        private void OpenSupplierDueReportForm()
        {
            SupplierDueReportForm supplierDueReport = new SupplierDueReportForm();
            supplierDueReport.Show();
        }

        private void OpenCustomerDueReportForm()
        {
            CustomerDueReportForm customerDueReport = new CustomerDueReportForm();
            customerDueReport.Show();
        }

        private void OpenDataBaseBackupForm()
        {
            try
            {
                // Open Database Backup Manager form
                var databaseBackupForm = new DatabaseBackupForm();
                databaseBackupForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening database backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenUserManagementForm()
        {
            Users users = new Users();
            users.Show();
        }

        private void OpenUserAccessForm()
        {
            try
            {
                // Open Users form for user access management
                var usersForm = new Users();
                usersForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening user access management: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenThermalInvoiceForm()
        {
            ThermalInvoiceForm thermalInvoice = new ThermalInvoiceForm();
            thermalInvoice.Show();
        }

        private void OpenDatabaseStatisticsForm()
        {
            DatabaseStatisticsReport databaseStats = new DatabaseStatisticsReport();
            databaseStats.Show();
        }


        public Dashboard()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
            recentActivities = new List<RecentActivity>();
            currentStats = new DashboardStats();
            _dashboardService = new DashboardService();

            InitializeDropdownMenus();
            InitializeDashboard();
            SetupRefreshTimer();
            LoadDashboardData();
            SetupThemeSwitching();
        }

        private void SetupThemeSwitching()
        {
            // Theme switching functionality temporarily disabled
            // Will be re-enabled when properly configured
            
            // Subscribe to theme changes
            ThemeManager.OnThemeChanged += () => ThemeManager.ApplyTheme(this);
        }

        private void ToggleTheme()
        {
            ThemeManager.ToggleTheme();
            ThemeManager.ApplyTheme(this);
        }

        private void InitializeDashboard()
        {
            // Initialize charts
            InitializeSalesChart();
            InitializeInventoryChart();
            InitializeTopProductsChart();
            
            // Initialize KPI cards
            InitializeKPICards();
            
            // Initialize recent activity
            InitializeRecentActivity();
            
            // Initialize breadcrumb navigation
            InitializeBreadcrumb();
            
            // Initialize recent forms panel
            InitializeRecentFormsPanel();
        }

        private void InitializeRecentFormsPanel()
        {
            // RecentFormsPanel functionality temporarily disabled
            // Will be re-enabled when properly configured
        }

        private void InitializeBreadcrumb()
        {
            // Breadcrumb functionality temporarily disabled
            // Will be re-enabled when properly configured
        }

        private void InitializeSalesChart()
        {
            // Chart functionality temporarily disabled
            // Will be re-enabled when chart panels are properly configured
        }

        private void InitializeInventoryChart()
        {
            // Chart functionality temporarily disabled
            // Will be re-enabled when chart panels are properly configured
        }

        private void InitializeTopProductsChart()
        {
            // Chart functionality temporarily disabled
            // Will be re-enabled when chart panels are properly configured
        }

        private void InitializeKPICards()
        {
            // Add KPI cards to the dashboard
            // These would be added to the designer or programmatically
            // For now, we'll update the existing labels with KPI data
        }

        private void InitializeRecentActivity()
        {
            // Initialize recent activity list
            recentActivities.Add(new RecentActivity { Time = DateTime.Now, Description = "New product added: Vaporesso XROS", Type = "Product" });
            recentActivities.Add(new RecentActivity { Time = DateTime.Now.AddMinutes(-15), Description = "Sale completed: 125.50", Type = "Sale" });
            recentActivities.Add(new RecentActivity { Time = DateTime.Now.AddMinutes(-30), Description = "Low stock alert: SMOK Nord 4", Type = "Alert" });
            recentActivities.Add(new RecentActivity { Time = DateTime.Now.AddHours(-1), Description = "New customer registered: John Smith", Type = "Customer" });
        }

        private void SetupRefreshTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 30000; // Refresh every 30 seconds
            refreshTimer.Tick += (s, e) => LoadDashboardData();
            refreshTimer.Start();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load all dashboard data using the service
                currentStats = _dashboardService.GetDashboardStats();
                recentActivities = _dashboardService.GetRecentActivities(10);
                
                // Update UI with real data
                UpdateKPIDisplay();
                UpdateRecentActivityDisplay();
                LoadSalesData();
                LoadInventoryData();
            }
            catch (Exception ex)
            {
                // Handle error silently for now
                System.Diagnostics.Debug.WriteLine($"Dashboard data load error: {ex.Message}");
            }
        }

        private void LoadSalesData()
        {
            try
            {
                // Load sales and purchase data for charts
                var startDate = DateTime.Now.AddDays(-30);
                var endDate = DateTime.Now;
                
                var salesData = _dashboardService.GetSalesData(startDate, endDate);
                var purchaseData = _dashboardService.GetPurchaseData(startDate, endDate);
                
                // Charts temporarily disabled - will be available after package installation
                // var salesChart = purchaseVsSalesChartPanel.Controls.OfType<Chart>().FirstOrDefault();
                // if (salesChart != null)
                // {
                //     salesChart.Series["Sales"].Points.Clear();
                //     salesChart.Series["Purchases"].Points.Clear();
                //     
                //     // Add real data from database
                //     foreach (var dataPoint in salesData)
                //     {
                //         salesChart.Series["Sales"].Points.AddXY(dataPoint.Date.ToString("MM/dd"), dataPoint.Amount);
                //     }
                //     
                //     foreach (var dataPoint in purchaseData)
                //     {
                //         salesChart.Series["Purchases"].Points.AddXY(dataPoint.Date.ToString("MM/dd"), dataPoint.Amount);
                //     }
                // }
                
                // Chart panel functionality temporarily disabled
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading sales data: {ex.Message}");
            }
        }

        private void LoadInventoryData()
        {
            try
            {
                // Load top products and low stock data
                var topProducts = _dashboardService.GetTopProducts(10);
                var lowStockProducts = _dashboardService.GetLowStockProducts(10);
                
                // Charts temporarily disabled - will be available after package installation
                // var inventoryChart = topProductsChartPanel.Controls.OfType<Chart>().FirstOrDefault();
                // if (inventoryChart != null)
                // {
                //     inventoryChart.Series["Stock Levels"].Points.Clear();
                //     
                //     // Add real stock data from database
                //     foreach (var product in topProducts)
                //     {
                //         inventoryChart.Series["Stock Levels"].Points.AddXY(product.ProductName, product.TotalSold);
                //     }
                // }
                
                // Chart panel functionality temporarily disabled
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading inventory data: {ex.Message}");
            }
        }

        private void UpdateKPIDisplay()
        {
            try
            {
                // Update Sales KPI
                label9.Text = $"{currentStats.TodaySales:F2}";
                
                // Update Sales Return KPI
                label10.Text = $"{currentStats.SalesReturns:F2}";
                
                // Update Purchase KPI - Fixed: Use MonthlyPurchases instead of MonthlySales
                label11.Text = $"{currentStats.MonthlyPurchases:F2}";
                
                // Update Purchase Return KPI
                label12.Text = $"{currentStats.PurchaseReturns:F2}";
                
                // Update Expenses KPI
                label13.Text = $"{currentStats.Expenses:F2}";
                
                // Update Users KPI
                label14.Text = $"{currentStats.ActiveUsers}-All Time";
                
                // Update Customers KPI
                label15.Text = $"{currentStats.TotalCustomers}-All Time";
                
                // Update Suppliers KPI
                label16.Text = $"{currentStats.TotalSuppliers}-All Time";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating KPI display: {ex.Message}");
            }
        }

        private void UpdateRecentActivityDisplay()
        {
            try
            {
                // Update recent activity display
                // This would update a list control or panel with recent activities
                // For now, we'll just log the activities
                foreach (var activity in recentActivities)
                {
                    System.Diagnostics.Debug.WriteLine($"{activity.Time}: {activity.Description} ({activity.Type})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating recent activity display: {ex.Message}");
            }
        }
        
        
        private void RefreshDashboardData()
        {
            try
            {
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing dashboard data: {ex.Message}");
            }
        }

        private void usersBtn_Click(object sender, EventArgs e)
        {
            if (UsersDropdown.Visible)
                UsersDropdown.Hide();
            else
                UsersDropdown.Show(usersBtn, new Point(0, usersBtn.Height));
        }

        private void purchaseBtn_Click(object sender, EventArgs e)
        {
            if (PurchaseDropdown.Visible)
                PurchaseDropdown.Hide();
            else
                PurchaseDropdown.Show(purchaseBtn, new Point(0, purchaseBtn.Height));
        }

        private void inventoryBtn_Click(object sender, EventArgs e)
        {
            if (InventoryDropdown.Visible)
                InventoryDropdown.Hide();
            else
                InventoryDropdown.Show(inventoryBtn, new Point(0, inventoryBtn.Height));
        }

        private void peopleBtn_Click(object sender, EventArgs e)
        {
            if (PeopleDropdown.Visible)
                PeopleDropdown.Hide();
            else
                PeopleDropdown.Show(peopleBtn, new Point(0, peopleBtn.Height));
        }

        private void accountsBtn_Click(object sender, EventArgs e)
        {
            if (AccountsDropdown.Visible)
                AccountsDropdown.Hide();
            else
                AccountsDropdown.Show(accountsBtn, new Point(0, accountsBtn.Height));
        }

        private void reportsBtn_Click(object sender, EventArgs e)
        {
            if (ReportsDropdown.Visible)
                ReportsDropdown.Hide();
            else
                ReportsDropdown.Show(reportsBtn, new Point(0, reportsBtn.Height));
        }

        private void utilitiesBtn_Click(object sender, EventArgs e)
        {
            if (UtilitiesDropdown.Visible)
                UtilitiesDropdown.Hide();
            else
                UtilitiesDropdown.Show(utilitiesBtn, new Point(0, utilitiesBtn.Height));
        }


        private void backupBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var backupService = new Services.DatabaseBackupService();
                
                // Show backup options
                var result = MessageBox.Show(
                    $"This will create a backup of your database.\n\nCurrent location: {backupService.GetBackupLocationInfo()}\n\nContinue with backup?",
                    "Confirm Backup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Show progress
                    var progressForm = new Form
                    {
                        Text = "Creating Backup...",
                        Size = new Size(300, 100),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var progressLabel = new Label
                    {
                        Text = "Creating database backup, please wait...",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    progressForm.Controls.Add(progressLabel);
                    progressForm.Show();

                    Application.DoEvents();

                    // Create backup
                    string backupPath = backupService.BackupDatabase();
                    
                    progressForm.Close();

                    var openManager = MessageBox.Show(
                        $"Backup created successfully!\n\nFile: {System.IO.Path.GetFileName(backupPath)}\n\n{backupService.GetBackupLocationInfo()}\n\nOpen Backup Manager for more options?",
                        "Backup Complete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (openManager == DialogResult.Yes)
                    {
                        var backupManager = new DatabaseBackupForm();
                        backupManager.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}\n\nTry opening Backup Manager to change the backup location.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure You want to Logout" , "Confirmation", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Application.Exit();
            }
            else 
            {
             
            }

        }


        private void Dashboard_Load(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
            }
            base.OnFormClosed(e);
        }

        private void BtnSales_Click(object sender, EventArgs e)
        {
            if (SalesDropdown.Visible)
                SalesDropdown.Hide();
            else
                SalesDropdown.Show(BtnSales, new Point(0, BtnSales.Height));
        }

    }

    // Data classes for dashboard
    public class RecentActivity
    {
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class DashboardStats
    {
        public decimal TodaySales { get; set; }
        public decimal MonthlySales { get; set; }
        public decimal MonthlyPurchases { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockItems { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal ProfitMargin { get; set; }
        public int PendingOrders { get; set; }
        public decimal SalesReturns { get; set; }
        public decimal PurchaseReturns { get; set; }
        public decimal Expenses { get; set; }
        public int TotalSuppliers { get; set; }
    }
}
