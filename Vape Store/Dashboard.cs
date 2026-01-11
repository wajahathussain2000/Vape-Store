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

        // Date range controls (declared in Designer)
        private bool isDateRangeActive = false;

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
            ReportsDropdown.Items.Add("Customer Outstanding Report", null, (s, e) => OpenCustomerDueReportForm());
            ReportsDropdown.Items.Add("Supplier Outstanding Report", null, (s, e) => OpenSupplierDueReportForm());

            ReportsDropdown.Items.Add("-"); // Separator
            ReportsDropdown.Items.Add("Expense Report", null, (s, e) => OpenExpenseReportForm());
            
            ReportsDropdown.Items.Add("-"); // Separator
            ReportsDropdown.Items.Add("Profit Margin Report", null, (s, e) => OpenProfitMarginReportForm());
            ReportsDropdown.Items.Add("Profit And Loss Report", null, (s, e) => OpenProfitAndLossForm());
            ReportsDropdown.Items.Add("Daily Report", null, (s, e) => OpenDailyReportForm());
            ReportsDropdown.Items.Add("Daily Sale Report", null, (s, e) => OpenDailySaleReportForm());
            
            ReportsDropdown.Items.Add("-"); // Separator
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
            if (!RequirePermission("sales")) return;
            NewSale newsale = new NewSale();
            newsale.FormClosed += (s, e) => RefreshDashboardData();
            newsale.Show();
        }
       
        private void OpenSalesLedgerForm()
        {
            try
            {
                if (!RequirePermission("sales")) return;
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
            if (!RequirePermission("Edit Sales")) return;
            EditSale editsale = new EditSale();
            editsale.FormClosed += (s, e) => RefreshDashboardData();
            editsale.Show();
        }
        private void OpenSalesReturnForm()
        {
            if (!RequirePermission("Sales Return")) return;
            SalesReturnForm salesreturn = new SalesReturnForm();
            salesreturn.FormClosed += (s, e) => RefreshDashboardData();
            salesreturn.Show();
        }
        private void OpenAddPurchaseForm()
        { 
           if (!RequirePermission("purchases")) return;
           NewPurchase newPurchase= new NewPurchase();  
            newPurchase.FormClosed += (s, e) => RefreshDashboardData();
            newPurchase.Show(); 

        }
        private void OpenPurchaseLedgerForm()
        {
            if (!RequirePermission("purchases")) return;
            PurchaseReportForm purchaseReport = new PurchaseReportForm();
            purchaseReport.Show();
        }

        private void OpenPurchaseReturnForm()
        {
            if (!RequirePermission("Purchase Return")) return;
            PurchaseReturnForm purchaseReturn= new PurchaseReturnForm(); 
            purchaseReturn.FormClosed += (s, e) => RefreshDashboardData();
            purchaseReturn.Show();  

        }

        private void OpenProductsForm()
        {
            if (!RequirePermission("inventory")) return;
            Products products = new Products(); 
            products.Show();            

        }
        private void OpenCategoriesForm()
        {
            if (!RequirePermission("inventory")) return;
            Categories categories = new Categories();
            categories.Show();

        }
        private void OpenBrandsForm()
        {
            if (!RequirePermission("inventory")) return;
            Brands brands = new Brands();   
            brands.Show();

        }
        private void OpenPrintBarCodeForm()
        {
            try
            {
                if (!RequirePermission("inventory")) return;
                var printBarcodeForm = new PrintBarcodeForm();
                printBarcodeForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening barcode printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenStockInHandForm()
        {
            if (!RequirePermission("inventory") && !RequirePermission("reports")) return;
            StockReportForm stockReport = new StockReportForm();
            stockReport.Show();
        }
        private void OpenCustomersForm()
        {
            if (!RequirePermission("people")) return;
            Customers customers = new Customers();  
            customers.Show();   

        }
        private void OpenSuppliersForm()
        {
            if (!RequirePermission("people")) return;
            Supplier_Master suppliers = new Supplier_Master();  
            suppliers.Show();

        }
        private void OpenUsersForm()
        {
            if (!RequirePermission("users")) return;
            Users users = new Users();
            users.Show();
        }
        private void OpenCashinHandForm()
        {
            if (!RequirePermission("accounts")) return;
            Cash_in_Hand cash_In_Hand = new Cash_in_Hand(); 
            cash_In_Hand.Show();    

        }
        private void OpenExpenseEntryForm()
        {
          if (!RequirePermission("accounts")) return;
          ExpenseEntry expense_entry = new ExpenseEntry();  
            expense_entry.Show();   
        }
        private void OpenCustomerPaymentForm()
        {
            if (!RequirePermission("people")) return;
            CustomerPaymentForm customerPayment = new CustomerPaymentForm();    
            customerPayment.Show(); 
        }
        private void OpenSupplierPaymentForm()
        {
            if (!RequirePermission("people")) return;
            SupplierPaymentForm supplierPayment = new SupplierPaymentForm();
            supplierPayment.Show(); 
        }

        private void OpenProfitAndLossForm()
        {
            try
            {
                if (!RequirePermission("reports")) return;
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
            if (!RequirePermission("reports") && !RequirePermission("basic_reports")) return;
            StockReportForm stockReport = new StockReportForm();
            stockReport.Show();
        }

        private void OpenLowStockReportForm()
        {
            if (!RequirePermission("reports")) return;
            LowStockReportForm lowStockReport = new LowStockReportForm();
            lowStockReport.Show();
        }

        private void OpenSummaryReportForm()
        {
            if (!RequirePermission("reports")) return;
            ProfitAndLossReportForm profitLossReport = new ProfitAndLossReportForm();
            profitLossReport.Show();
        }

        private void OpenSalesReportForm()
        {
            if (!RequirePermission("reports") && !RequirePermission("View Sales")) return;
            SalesReportForm salesReport = new SalesReportForm();
            salesReport.Show();
        }

        private void OpenItemsSalesReportForm()
        {
            try
            {
                if (!RequirePermission("reports")) return;
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
                if (!RequirePermission("reports") && !RequirePermission("Sales Return")) return;
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
            if (!RequirePermission("reports") && !RequirePermission("View Purchases")) return;
            PurchaseReportForm purchaseReport = new PurchaseReportForm();
            purchaseReport.Show();
        }

        private void OpenPurchaseReturnReportForm()
        {
            try
            {
                if (!RequirePermission("reports") && !RequirePermission("Purchase Return")) return;
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
            if (!RequirePermission("reports")) return;
            ProfitAndLossReportForm profitLossReport = new ProfitAndLossReportForm();
            profitLossReport.Show();
        }

        private void OpenExpenseReportForm()
        {
            if (!RequirePermission("reports") && !RequirePermission("accounts")) return;
            ExpenseReportForm expenseReport = new ExpenseReportForm();
            expenseReport.Show();
        }

        private void OpenDailyReportForm()
        {
            if (!RequirePermission("reports")) return;
            DailyReportForm dailyReport = new DailyReportForm();
            dailyReport.Show();
        }

        private void OpenDailySaleReportForm()
        {
            if (!RequirePermission("reports")) return;
            DailySaleReportForm dailySaleReport = new DailySaleReportForm();
            dailySaleReport.Show();
        }

        private void OpenSupplierDueReportForm()
        {
            if (!RequirePermission("reports") && !RequirePermission("people")) return;
            SupplierDueReportForm supplierDueReport = new SupplierDueReportForm();
            supplierDueReport.Show();
        }

        private void OpenCustomerDueReportForm()
        {
            if (!RequirePermission("reports") && !RequirePermission("people")) return;
            CustomerDueReportForm customerDueReport = new CustomerDueReportForm();
            customerDueReport.Show();
        }

        private void OpenDataBaseBackupForm()
        {
            try
            {
                if (!RequirePermission("backup") && !RequirePermission("utilities")) return;
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
            // Check permission before opening Users form
            if (!RequirePermission("users")) return;
            Users users = new Users();
            users.Show();
        }

        private void OpenUserAccessForm()
        {
            try
            {
                // All roles now have access to manage roles & permissions
                var rp = new RolesPermissionsForm();
                rp.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening user access management: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenThermalInvoiceForm()
        {
            if (!RequirePermission("utilities")) return;
            ThermalInvoiceForm thermalInvoice = new ThermalInvoiceForm();
            thermalInvoice.Show();
        }

        private void OpenDatabaseStatisticsForm()
        {
            if (!RequirePermission("reports")) return;
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
            
            // Prevent closing the application when Dashboard is closed - redirect to logout
            this.FormClosing += Dashboard_FormClosing;
        }
        
        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent closing the application - instead, trigger logout process
            // If user cancels logout, form stays open (e.Cancel remains true)
            // If user confirms logout, logout handler will hide this form and show login
            e.Cancel = true;
            
            // Trigger logout (will show confirmation dialog)
            // If user confirms, logout handler hides dashboard and shows login
            // If user cancels, form stays open (which is what we want)
            logoutBtn_Click(sender, e);
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
            
            // Initialize date range controls - attach event handlers
            InitializeDateRangeControls();
        }
        
        private void InitializeDateRangeControls()
        {
            // Set initial values
            dtpFromDate.Value = DateTime.Now.Date;
            dtpToDate.Value = DateTime.Now.Date;
            lblDateRange.Text = "Viewing Stats:";
            
            // Attach event handlers
            btnApplyDateRange.Click += BtnApplyDateRange_Click;
            btnResetDateRange.Click += BtnResetDateRange_Click;
            
            // Handle form resize to keep controls on right
            this.Resize += Dashboard_Resize;
        }
        
        private void Dashboard_Resize(object sender, EventArgs e)
        {
            if (dtpFromDate != null && dtpToDate != null && btnApplyDateRange != null && btnResetDateRange != null && lblDateRange != null)
            {
                int formWidth = this.ClientSize.Width;
                int rightMargin = 20;
                int controlSpacing = 8;
                int viewingStatsLabelWidth = 250;
                
                // Update row 2 positions
                int newResetBtnX = formWidth - rightMargin - 70;
                int newApplyBtnX = newResetBtnX - controlSpacing - 70;
                int newToDateX = newApplyBtnX - controlSpacing - 130;
                int newToLabelX = newToDateX - controlSpacing - 25;
                int newFromDateX = newToLabelX - controlSpacing - 130;
                
                // Update row 1 position
                int newViewingStatsX = formWidth - rightMargin - viewingStatsLabelWidth;
                
                // Apply new positions
                lblDateRange.Location = new Point(newViewingStatsX, lblDateRange.Location.Y);
                dtpFromDate.Location = new Point(newFromDateX, dtpFromDate.Location.Y);
                lblTo.Location = new Point(newToLabelX, lblTo.Location.Y);
                dtpToDate.Location = new Point(newToDateX, dtpToDate.Location.Y);
                btnApplyDateRange.Location = new Point(newApplyBtnX, btnApplyDateRange.Location.Y);
                btnResetDateRange.Location = new Point(newResetBtnX, btnResetDateRange.Location.Y);
            }
        }
        
        private void BtnApplyDateRange_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpFromDate.Value > dtpToDate.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                isDateRangeActive = true;
                LoadDashboardDataWithDateRange(dtpFromDate.Value.Date, dtpToDate.Value.Date);
                
                // Update label to show active filter
                if (dtpFromDate.Value.Date == dtpToDate.Value.Date)
                {
                    lblDateRange.Text = $"Viewing Stats: {dtpFromDate.Value:MM/dd/yyyy}";
                }
                else
                {
                    lblDateRange.Text = $"Viewing Stats: {dtpFromDate.Value:MM/dd/yyyy} to {dtpToDate.Value:MM/dd/yyyy}";
                }
                lblDateRange.ForeColor = Color.FromArgb(46, 204, 113);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying date range: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtnResetDateRange_Click(object sender, EventArgs e)
        {
            try
            {
                isDateRangeActive = false;
                dtpFromDate.Value = DateTime.Now.Date;
                dtpToDate.Value = DateTime.Now.Date;
                lblDateRange.Text = "Viewing Stats:";
                lblDateRange.ForeColor = Color.FromArgb(52, 73, 94);
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting date range: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadDashboardDataWithDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Load dashboard data for the specified date range
                currentStats = _dashboardService.GetDashboardStatsByDateRange(fromDate, toDate);
                recentActivities = _dashboardService.GetRecentActivities(10);
                
                // Update UI with real data
                UpdateKPIDisplay();
                UpdateRecentActivityDisplay();
                LoadSalesData();
                LoadInventoryData();
                
                // Force UI update
                this.Invalidate();
                this.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                // If date range is active, use it; otherwise load current stats
                if (isDateRangeActive && dtpFromDate != null && dtpToDate != null)
                {
                    LoadDashboardDataWithDateRange(dtpFromDate.Value.Date, dtpToDate.Value.Date);
                }
                else
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
        
        
        public void RefreshDashboardData()
        {
            try
            {
                // Force immediate refresh of dashboard data
                LoadDashboardData();
                
                // Force UI update to show changes immediately
                this.Invalidate();
                this.Update();
                Application.DoEvents();
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
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                try
                {
                    // Clear the user session
                    UserSession.Logout();
                    
                    // Find or create the login form first
                    Form1 loginForm = null;
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form is Form1)
                        {
                            loginForm = form as Form1;
                            break;
                        }
                    }
                    
                    // Close all open child forms (but not the login form)
                    var formsToClose = new List<Form>();
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form != null && form != this && !(form is Form1) && form != loginForm)
                        {
                            formsToClose.Add(form);
                        }
                    }
                    
                    // Dispose child forms
                    foreach (var form in formsToClose)
                    {
                        form.Hide();
                        form.Dispose();
                    }
                    
                    // If login form doesn't exist, create a new one
                    if (loginForm == null)
                    {
                        loginForm = new Form1();
                    }
                    
                    // Clear login form fields
                    loginForm.ClearLoginFields();
                    
                    // Show login form and hide dashboard
                    loginForm.Show();
                    loginForm.BringToFront();
                    loginForm.WindowState = FormWindowState.Normal;
                    loginForm.Activate();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during logout: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        


        private void Dashboard_Load(object sender, EventArgs e)
        {
            LoadDashboardData();
            ApplyRolePermissions();
        }

        private bool RequirePermission(string permission)
        {
            try
            {
                if (UserSession.HasPermission(permission)) return true;
            }
            catch { }
            MessageBox.Show("You do not have permission to perform this action.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void ApplyRolePermissions()
        {
            try
            {
                // Sales menu: visible to sales/manager/admin/superadmin
                BtnSales.Enabled = UserSession.HasPermission("sales");

                // Match actual field names from Designer
                purchaseBtn.Enabled = UserSession.HasPermission("purchases");
                inventoryBtn.Enabled = UserSession.HasPermission("inventory");
                // People button should only be enabled if user has "people" permission
                // Users button handles "users" permission separately
                peopleBtn.Enabled = UserSession.HasPermission("people");
                accountsBtn.Enabled = UserSession.HasPermission("accounts");
                reportsBtn.Enabled = UserSession.HasPermission("reports") || UserSession.HasPermission("basic_reports");
                utilitiesBtn.Enabled = UserSession.HasPermission("utilities") || UserSession.HasPermission("backup");
                usersBtn.Enabled = UserSession.HasPermission("users");
            }
            catch { }
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

        private void lblDateRange_Click(object sender, EventArgs e)
        {

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
