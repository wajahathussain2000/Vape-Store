using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.DataAccess;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class DailyReportForm : Form
    {
        private SaleRepository _saleRepository;
        private PurchaseRepository _purchaseRepository;
        private ExpenseRepository _expenseRepository;
        private ReportingService _reportingService;
        private DayClosingRepository _dayClosingRepository;
        
        private List<DailyReportItem> _dailyReportItems;
        private List<DailyReportItem> _filteredItems = new List<DailyReportItem>();
        private decimal _openingBalance = 0;
        private decimal _closingBalance = 0;

        public DailyReportForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _purchaseRepository = new PurchaseRepository();
            _expenseRepository = new ExpenseRepository();
            _reportingService = new ReportingService();
            _dayClosingRepository = new DayClosingRepository();
            
            _dailyReportItems = new List<DailyReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            btnEndDay.Click += BtnEndDay_Click;
            
            // Date event handler
            dtpDate.ValueChanged += DtpDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += DailyReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvDailyReport.AutoGenerateColumns = false;
                dgvDailyReport.AllowUserToAddRows = false;
                dgvDailyReport.AllowUserToDeleteRows = false;
                dgvDailyReport.ReadOnly = true;
                dgvDailyReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvDailyReport.MultiSelect = false;
                dgvDailyReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvDailyReport.AllowUserToResizeColumns = true;
                dgvDailyReport.AllowUserToResizeRows = false;
                dgvDailyReport.RowHeadersVisible = false;
                dgvDailyReport.EnableHeadersVisualStyles = false;
                dgvDailyReport.GridColor = Color.FromArgb(236, 240, 241);
                dgvDailyReport.BorderStyle = BorderStyle.None;
                
                // Set header styling
                dgvDailyReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvDailyReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvDailyReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvDailyReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvDailyReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvDailyReport.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvDailyReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvDailyReport.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvDailyReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvDailyReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvDailyReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvDailyReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvDailyReport.Columns.Clear();
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Date",
                    HeaderText = "Date",
                    DataPropertyName = "Date",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Type",
                    HeaderText = "Type",
                    DataPropertyName = "Type",
                    Width = 120,
                    MinimumWidth = 100
                });
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Reference",
                    HeaderText = "Reference #",
                    DataPropertyName = "Reference",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 250,
                    MinimumWidth = 200
                });
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Debit",
                    HeaderText = "Debit (DR)",
                    DataPropertyName = "Debit",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Format = "F2",
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        ForeColor = Color.FromArgb(192, 57, 43) // Red for debits
                    }
                });
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Credit",
                    HeaderText = "Credit (CR)",
                    DataPropertyName = "Credit",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Format = "F2",
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        ForeColor = Color.FromArgb(39, 174, 96) // Green for credits
                    }
                });
                
                dgvDailyReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Balance",
                    HeaderText = "Balance",
                    DataPropertyName = "Balance",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Format = "F2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadDailyData()
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;
                // Use proper date range: from start of day (00:00:00) to end of day (23:59:59.999)
                var fromDate = selectedDate; // Start of day
                var toDate = selectedDate.AddDays(1).AddTicks(-1); // End of day (23:59:59.9999999)
                
                // Always start with 0 balance (fresh start each day)
                _openingBalance = 0;
                
                _dailyReportItems.Clear();
                
                // Load Sales (CREDIT)
                var sales = _saleRepository.GetSalesByDateRange(fromDate, toDate);
                foreach (var sale in sales)
                {
                    _dailyReportItems.Add(new DailyReportItem
                    {
                        Date = sale.SaleDate,
                        Type = "Sale",
                        Reference = sale.InvoiceNumber,
                        Description = $"Sale to {(sale.CustomerName ?? "Walk-in Customer")}",
                        Debit = 0,
                        Credit = sale.TotalAmount,
                        Balance = 0 // Will be calculated after sorting
                    });
                }
                
                // Load Purchases (DEBIT) - CRITICAL: Ensure purchases are loaded correctly
                var purchases = _purchaseRepository.GetPurchasesByDateRange(fromDate, toDate);
                
                // Debug: Log purchase count
                System.Diagnostics.Debug.WriteLine($"Daily Report - Date: {selectedDate:yyyy-MM-dd}, Purchases found: {purchases?.Count ?? 0}");
                
                if (purchases != null && purchases.Count > 0)
                {
                    foreach (var purchase in purchases)
                    {
                        System.Diagnostics.Debug.WriteLine($"Purchase found - Invoice: {purchase.InvoiceNumber}, Date: {purchase.PurchaseDate:yyyy-MM-dd HH:mm:ss}, Amount: {purchase.TotalAmount}");
                        
                        _dailyReportItems.Add(new DailyReportItem
                        {
                            Date = purchase.PurchaseDate,
                            Type = "Purchase",
                            Reference = purchase.InvoiceNumber,
                            Description = $"Purchase from {(purchase.SupplierName ?? "Supplier")}",
                            Debit = purchase.TotalAmount,
                            Credit = 0,
                            Balance = 0 // Will be calculated after sorting
                        });
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No purchases found for date range: {fromDate:yyyy-MM-dd HH:mm:ss} to {toDate:yyyy-MM-dd HH:mm:ss}");
                }
                
                // Load Expenses (DEBIT)
                var expenses = _expenseRepository.GetExpensesByDateRange(fromDate, toDate);
                foreach (var expense in expenses)
                {
                    _dailyReportItems.Add(new DailyReportItem
                    {
                        Date = expense.ExpenseDate,
                        Type = "Expense",
                        Reference = expense.ExpenseCode,
                        Description = $"{expense.CategoryName ?? "Expense"}: {expense.Description ?? ""}",
                        Debit = expense.Amount,
                        Credit = 0,
                        Balance = 0 // Will be calculated after sorting
                    });
                }
                
                // Sort by date and then by reference
                _dailyReportItems = _dailyReportItems
                    .OrderBy(x => x.Date)
                    .ThenBy(x => x.Reference)
                    .ToList();
                
                // Calculate running balance in chronological order
                // Balance = Previous Balance + Credit - Debit
                decimal runningBalance = _openingBalance; // Start with opening balance
                foreach (var item in _dailyReportItems)
                {
                    runningBalance = runningBalance + item.Credit - item.Debit;
                    item.Balance = runningBalance;
                }
                
                // Calculate closing balance
                _closingBalance = runningBalance;
                
                ApplyFilters();
                AddTotalsRow(); // Add totals row at the end (includes RefreshDataGridView)
                UpdateSummaryLabels();
                
                // Check if day is already closed
                CheckDayClosingStatus();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading daily data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (_dailyReportItems == null || _dailyReportItems.Count == 0)
                {
                    _filteredItems = new List<DailyReportItem>();
                    return;
                }
                
                var filteredItems = _dailyReportItems.AsEnumerable();
                
                // Search filter - search through multiple fields including extracted names from description
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.Reference?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.Description?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.Type?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.Date.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.Date.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.Debit.ToString("F2").Contains(searchTerm)) ||
                        (item.Credit.ToString("F2").Contains(searchTerm)) ||
                        (item.Balance.ToString("F2").Contains(searchTerm)) ||
                        // Extract customer/vendor names from description (e.g., "Sale to John" or "Purchase from ABC")
                        (item.Description?.ToLower().Contains("to " + searchTerm) ?? false) ||
                        (item.Description?.ToLower().Contains("from " + searchTerm) ?? false));
                }
                
                // Don't set DataSource here - will be set manually in AddTotalsRow
                // This allows us to add a totals row without binding issues
                _filteredItems = filteredItems.ToList();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error applying filters: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvDailyReport.Refresh();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void AddTotalsRow()
        {
            try
            {
                if (_dailyReportItems == null || _dailyReportItems.Count == 0)
                    return;

                // Use filtered items for totals if available
                var itemsForTotals = _filteredItems != null && _filteredItems.Count > 0 ? _filteredItems : _dailyReportItems;
                
                // Calculate totals
                decimal totalDebit = itemsForTotals.Sum(item => item.Debit);
                decimal totalCredit = itemsForTotals.Sum(item => item.Credit);
                
                // Calculate final balance: Opening Balance + Total Credit - Total Debit
                // Always calculate from totals to ensure accuracy
                decimal finalBalance = _openingBalance + totalCredit - totalDebit;
                
                // For full dataset (no filter), verify it matches closing balance
                // If filtered, use calculated balance from filtered totals
                bool isFiltered = _filteredItems != null && _filteredItems.Count > 0 && 
                                 _filteredItems.Count < _dailyReportItems.Count;
                
                if (!isFiltered)
                {
                    // Full dataset: use the actual closing balance (more accurate)
                    finalBalance = _closingBalance;
                }
                // else: use calculated balance from filtered totals

                // Remove any existing totals rows first
                for (int i = dgvDailyReport.Rows.Count - 1; i >= 0; i--)
                {
                    if (dgvDailyReport.Rows[i].Tag != null && dgvDailyReport.Rows[i].Tag.ToString() == "TOTAL")
                    {
                        dgvDailyReport.Rows.RemoveAt(i);
                    }
                }

                // Temporarily remove DataSource to allow adding unbound row
                var savedDataSource = dgvDailyReport.DataSource;
                var dataList = savedDataSource as List<DailyReportItem>;
                dgvDailyReport.DataSource = null;
                dgvDailyReport.Rows.Clear();

                // Use filtered items if available, otherwise use all items
                var itemsToDisplay = _filteredItems != null && _filteredItems.Count > 0 ? _filteredItems : _dailyReportItems;
                
                // Check if search returned no results
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _dailyReportItems.Count > 0 && itemsToDisplay.Count == 0;
                
                // Manually populate all rows from the data
                if (hasDataButNoResults)
                {
                    // Show "No results found" message
                    int noResultsRowIndex = dgvDailyReport.Rows.Add();
                    DataGridViewRow noResultsRow = dgvDailyReport.Rows[noResultsRowIndex];
                    noResultsRow.Tag = "NO_RESULTS";
                    noResultsRow.DefaultCellStyle.Font = new System.Drawing.Font(dgvDailyReport.DefaultCellStyle.Font, FontStyle.Italic);
                    noResultsRow.DefaultCellStyle.ForeColor = Color.Gray;
                    noResultsRow.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                    
                    noResultsRow.Cells["Date"].Value = "";
                    noResultsRow.Cells["Type"].Value = "";
                    noResultsRow.Cells["Reference"].Value = "";
                    noResultsRow.Cells["Description"].Value = $"No results found for '{txtSearch.Text}'. Clear search to see all data.";
                    noResultsRow.Cells["Description"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    noResultsRow.Cells["Debit"].Value = "";
                    noResultsRow.Cells["Credit"].Value = "";
                    noResultsRow.Cells["Balance"].Value = "";
                    noResultsRow.ReadOnly = true;
                    
                    // Merge cells for the message (if possible)
                    if (dgvDailyReport.Columns["Description"] != null)
                    {
                        noResultsRow.Cells["Description"].Style.WrapMode = DataGridViewTriState.True;
                    }
                }
                else
                {
                foreach (var item in (itemsToDisplay ?? new List<DailyReportItem>()))
                {
                    int newRowIndex = dgvDailyReport.Rows.Add();
                    DataGridViewRow row = dgvDailyReport.Rows[newRowIndex];
                    row.Cells["Date"].Value = item.Date.ToString("MM/dd/yyyy");
                    row.Cells["Type"].Value = item.Type;
                    row.Cells["Reference"].Value = item.Reference;
                    row.Cells["Description"].Value = item.Description;
                    row.Cells["Debit"].Value = item.Debit.ToString("F2");
                    row.Cells["Credit"].Value = item.Credit.ToString("F2");
                    row.Cells["Balance"].Value = item.Balance.ToString("F2");
                    row.ReadOnly = true;
                    }
                }

                // Add totals row at the bottom (only if we have results)
                if (!hasDataButNoResults && itemsToDisplay.Count > 0)
                {
                int totalsRowIndex = dgvDailyReport.Rows.Add();
                DataGridViewRow totalsRow = dgvDailyReport.Rows[totalsRowIndex];
                totalsRow.Tag = "TOTAL";
                totalsRow.DefaultCellStyle.Font = new System.Drawing.Font(dgvDailyReport.DefaultCellStyle.Font, FontStyle.Bold);
                totalsRow.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                totalsRow.DefaultCellStyle.ForeColor = Color.FromArgb(41, 128, 185);
                
                // Set alignment for numeric columns
                if (totalsRow.Cells["Debit"] != null)
                    totalsRow.Cells["Debit"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (totalsRow.Cells["Credit"] != null)
                    totalsRow.Cells["Credit"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (totalsRow.Cells["Balance"] != null)
                    totalsRow.Cells["Balance"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Set values for totals row
                totalsRow.Cells["Date"].Value = "";
                totalsRow.Cells["Type"].Value = "TOTAL";
                totalsRow.Cells["Reference"].Value = "";
                totalsRow.Cells["Description"].Value = "TOTAL";
                totalsRow.Cells["Debit"].Value = totalDebit.ToString("F2");
                totalsRow.Cells["Credit"].Value = totalCredit.ToString("F2");
                totalsRow.Cells["Balance"].Value = finalBalance.ToString("F2");

                // Make the totals row read-only
                totalsRow.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding totals row: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                // Use filtered items if search is active, otherwise use all items
                var itemsForSummary = _filteredItems != null && _filteredItems.Count > 0 ? _filteredItems : _dailyReportItems;
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _dailyReportItems.Count > 0 && itemsForSummary.Count == 0;
                
                if (hasDataButNoResults)
                {
                    // Show "No results" in summary when search returns nothing
                    lblTotalDebit.Text = "Total Debit: No results";
                    lblTotalCredit.Text = "Total Credit: No results";
                    lblOpeningBalance.Text = $"Opening Balance: {_openingBalance:F2}";
                    lblClosingBalance.Text = $"Closing Balance: {_closingBalance:F2}";
                    lblNetBalance.Text = "Net Balance: No results";
                    lblTotalSales.Text = "Total Sales: No results";
                    lblTotalPurchases.Text = "Total Purchases: No results";
                    lblTotalExpenses.Text = "Total Expenses: No results";
                }
                else
                {
                    var totalDebit = itemsForSummary.Sum(item => item.Debit);
                    var totalCredit = itemsForSummary.Sum(item => item.Credit);
                var netBalance = totalCredit - totalDebit;
                    var totalSales = itemsForSummary.Where(x => x.Type == "Sale").Sum(x => x.Credit);
                    var totalPurchases = itemsForSummary.Where(x => x.Type == "Purchase").Sum(x => x.Debit);
                    var totalExpenses = itemsForSummary.Where(x => x.Type == "Expense").Sum(x => x.Debit);
                
                lblTotalDebit.Text = $"Total Debit: {totalDebit:F2}";
                lblTotalCredit.Text = $"Total Credit: {totalCredit:F2}";
                lblOpeningBalance.Text = $"Opening Balance: {_openingBalance:F2}";
                    // For closing balance, use actual closing balance if showing all data, otherwise calculate from filtered
                    if (!hasSearchFilter || itemsForSummary.Count == _dailyReportItems.Count)
                    {
                lblClosingBalance.Text = $"Closing Balance: {_closingBalance:F2}";
                    }
                    else
                    {
                        decimal filteredBalance = _openingBalance + totalCredit - totalDebit;
                        lblClosingBalance.Text = $"Closing Balance: {filteredBalance:F2} (filtered)";
                    }
                lblNetBalance.Text = $"Net Balance: {netBalance:F2}";
                lblTotalSales.Text = $"Total Sales: {totalSales:F2}";
                lblTotalPurchases.Text = $"Total Purchases: {totalPurchases:F2}";
                lblTotalExpenses.Text = $"Total Expenses: {totalExpenses:F2}";
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        
        private void CheckDayClosingStatus()
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;
                    bool isClosed = _dayClosingRepository.IsDayClosed(selectedDate);
                    if (isClosed)
                    {
                        lblDayStatus.Text = "Day Status: CLOSED";
                        lblDayStatus.ForeColor = Color.FromArgb(231, 76, 60); // Red
                        btnEndDay.Enabled = false;
                        btnEndDay.Text = "Day Already Closed";
                    }
                    else
                    {
                        lblDayStatus.Text = "Day Status: OPEN";
                        lblDayStatus.ForeColor = Color.FromArgb(46, 204, 113); // Green
                        btnEndDay.Enabled = true;
                        btnEndDay.Text = "End Day";
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error checking day status: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        
        private void BtnEndDay_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;
                
                // Check if already closed
                if (_dayClosingRepository.IsDayClosed(selectedDate))
                {
                    ShowMessage("This day has already been closed.", "Day Already Closed", MessageBoxIcon.Information);
                    return;
                }
                
                // Confirm with user
                var result = MessageBox.Show(
                    $"Are you sure you want to close the day for {selectedDate:yyyy-MM-dd}?\n\n" +
                    $"Opening Balance: {_openingBalance:F2}\n" +
                    $"Total Sales: {_dailyReportItems.Where(x => x.Type == "Sale").Sum(x => x.Credit):F2}\n" +
                    $"Total Purchases: {_dailyReportItems.Where(x => x.Type == "Purchase").Sum(x => x.Debit):F2}\n" +
                    $"Total Expenses: {_dailyReportItems.Where(x => x.Type == "Expense").Sum(x => x.Debit):F2}\n" +
                    $"Closing Balance: {_closingBalance:F2}\n\n" +
                    "This action cannot be undone.",
                    "End Day Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Get current user
                    int userId = 1; // Default
                    if (UserSession.CurrentUser != null)
                    {
                        userId = UserSession.CurrentUser.UserID;
                    }
                    
                    // Save day closing
                    bool success = _dayClosingRepository.CloseDay(selectedDate, _openingBalance, _closingBalance, userId);
                    
                    if (success)
                    {
                        // Notify Dashboard to clear/refresh data IMMEDIATELY before showing message
                        // This ensures dashboard resets to 0 right away
                        NotifyDashboardDayEnd();
                        
                        ShowMessage($"Day closed successfully!\nClosing Balance: {_closingBalance:F2}\nNext day will start with this balance.", "Day Closed", MessageBoxIcon.Information);
                        CheckDayClosingStatus();
                        
                        // Refresh data to show updated status
                        LoadDailyData();
                    }
                    else
                    {
                        ShowMessage("Error closing the day. Please try again.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error ending the day: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            dtpDate.Value = DateTime.Now.Date;
            txtSearch.Clear();
            lblDayStatus.Text = "Day Status: -";
            lblDayStatus.ForeColor = Color.Black;
            _openingBalance = 0;
            _closingBalance = 0;
        }

        /// <summary>
        /// Notifies the Dashboard to clear/refresh data after day end
        /// </summary>
        private void NotifyDashboardDayEnd()
        {
            try
            {
                // Find and refresh all open Dashboard instances
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Dashboard dashboard)
                    {
                        // Clear dashboard data by refreshing
                        dashboard.RefreshDashboardData();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error notifying dashboard: {ex.Message}");
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadDailyData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dailyReportItems == null || _dailyReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"DailyReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
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
                if (_dailyReportItems == null || _dailyReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"DailyReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveFileDialog.FileName);
                    ShowMessage("Report exported to PDF successfully!", "Export Complete", MessageBoxIcon.Information);
                }
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
                if (_dailyReportItems == null || _dailyReportItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                var htmlContent = GenerateHTMLReport();
                var htmlViewer = new HTMLReportViewerForm();
                htmlViewer.LoadReport(htmlContent);
                htmlViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
            _dailyReportItems.Clear();
            _filteredItems.Clear();
            dgvDailyReport.DataSource = null;
            dgvDailyReport.Rows.Clear();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DtpDate_ValueChanged(object sender, EventArgs e)
        {
            // Refresh day status when date changes
                CheckDayClosingStatus();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            AddTotalsRow(); // Refresh with totals row
            UpdateSummaryLabels();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Date,Type,Reference,Description,Debit (DR),Credit (CR),Balance");
                
                // Write data
                foreach (var item in _dailyReportItems)
                {
                    writer.WriteLine($"{item.Date:yyyy-MM-dd},{item.Type},{item.Reference},{item.Description},{item.Debit:F2},{item.Credit:F2},{item.Balance:F2}");
                }
            }
        }

        private void ExportToPDF(string filePath)
        {
            try
            {
                // Create PDF document
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Set up fonts
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font smallFont = new iTextSharp.text.Font(baseFont, 8, iTextSharp.text.Font.NORMAL);

                // Title
                Paragraph title = new Paragraph("Attock Mobiles Rwp - DAILY REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate: {dtpDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Create table for report data
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 1.2f, 1.5f, 2.5f, 1.2f, 1.2f, 1.2f });

                // Table headers
                string[] headers = { "Date", "Type", "Reference", "Description", "Debit (DR)", "Credit (CR)", "Balance" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var item in _dailyReportItems)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.Date.ToString("MM/dd/yyyy"), normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Type, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Reference, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Description, normalFont)) { Padding = 3f });
                    
                    PdfPCell debitCell = new PdfPCell(new Phrase(item.Debit.ToString("F2"), normalFont));
                    debitCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    debitCell.Padding = 3f;
                    table.AddCell(debitCell);
                    
                    PdfPCell creditCell = new PdfPCell(new Phrase(item.Credit.ToString("F2"), normalFont));
                    creditCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    creditCell.Padding = 3f;
                    table.AddCell(creditCell);
                    
                    PdfPCell balanceCell = new PdfPCell(new Phrase(item.Balance.ToString("F2"), normalFont));
                    balanceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    balanceCell.Padding = 3f;
                    table.AddCell(balanceCell);
                }

                document.Add(table);

                // Add summary section
                document.Add(new Paragraph("\n", normalFont));
                
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Calculate totals
                var totalDebit = _dailyReportItems.Sum(x => x.Debit);
                var totalCredit = _dailyReportItems.Sum(x => x.Credit);
                var netBalance = totalCredit - totalDebit;
                var totalSales = _dailyReportItems.Where(x => x.Type == "Sale").Sum(x => x.Credit);
                var totalPurchases = _dailyReportItems.Where(x => x.Type == "Purchase").Sum(x => x.Debit);
                var totalExpenses = _dailyReportItems.Where(x => x.Type == "Expense").Sum(x => x.Debit);

                document.Add(new Paragraph($"Opening Balance: {_openingBalance:F2}", normalFont));
                document.Add(new Paragraph($"Total Debit (DR): {totalDebit:F2}", normalFont));
                document.Add(new Paragraph($"Total Credit (CR): {totalCredit:F2}", normalFont));
                document.Add(new Paragraph($"Net Balance: {netBalance:F2}", normalFont));
                document.Add(new Paragraph($"Closing Balance: {_closingBalance:F2}", headerFont));
                document.Add(new Paragraph("\nBreakdown:", headerFont));
                document.Add(new Paragraph($"Total Sales: {totalSales:F2}", normalFont));
                document.Add(new Paragraph($"Total Purchases: {totalPurchases:F2}", normalFont));
                document.Add(new Paragraph($"Total Expenses: {totalExpenses:F2}", normalFont));

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}");
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private string GenerateHTMLReport()
        {
            try
            {
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html><head>");
                html.AppendLine("<title>Daily Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { color: #2c3e50; text-align: center; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                html.AppendLine(".debit { color: #c0392b; text-align: right; }");
                html.AppendLine(".credit { color: #27ae60; text-align: right; }");
                html.AppendLine(".balance { text-align: right; }");
                html.AppendLine(".summary { background-color: #e8f4f8; padding: 15px; margin: 20px 0; border-radius: 5px; }");
                html.AppendLine("</style>");
                html.AppendLine("</head><body>");
                
                html.AppendLine("<h1>Attock Mobiles Rwp - DAILY REPORT</h1>");
                html.AppendLine($"<p><strong>Date:</strong> {dtpDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<div class='summary'>");
                html.AppendLine("<h2>Summary</h2>");
                var totalDebit = _dailyReportItems.Sum(x => x.Debit);
                var totalCredit = _dailyReportItems.Sum(x => x.Credit);
                var netBalance = totalCredit - totalDebit;
                var totalSales = _dailyReportItems.Where(x => x.Type == "Sale").Sum(x => x.Credit);
                var totalPurchases = _dailyReportItems.Where(x => x.Type == "Purchase").Sum(x => x.Debit);
                var totalExpenses = _dailyReportItems.Where(x => x.Type == "Expense").Sum(x => x.Debit);
                
                html.AppendLine($"<p><strong>Opening Balance:</strong> {_openingBalance:F2}</p>");
                html.AppendLine($"<p><strong>Total Debit (DR):</strong> {totalDebit:F2}</p>");
                html.AppendLine($"<p><strong>Total Credit (CR):</strong> {totalCredit:F2}</p>");
                html.AppendLine($"<p><strong>Net Balance:</strong> {netBalance:F2}</p>");
                html.AppendLine($"<p><strong>Closing Balance:</strong> {_closingBalance:F2}</p>");
                html.AppendLine($"<p><strong>Total Sales:</strong> {totalSales:F2}</p>");
                html.AppendLine($"<p><strong>Total Purchases:</strong> {totalPurchases:F2}</p>");
                html.AppendLine($"<p><strong>Total Expenses:</strong> {totalExpenses:F2}</p>");
                html.AppendLine("</div>");
                
                // Table
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Date</th><th>Type</th><th>Reference</th><th>Description</th><th>Debit (DR)</th><th>Credit (CR)</th><th>Balance</th></tr>");
                
                foreach (var item in _dailyReportItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.Date:yyyy-MM-dd}</td>");
                    html.AppendLine($"<td>{item.Type}</td>");
                    html.AppendLine($"<td>{item.Reference}</td>");
                    html.AppendLine($"<td>{item.Description}</td>");
                    html.AppendLine($"<td class='debit'>{item.Debit:F2}</td>");
                    html.AppendLine($"<td class='credit'>{item.Credit:F2}</td>");
                    html.AppendLine($"<td class='balance'>{item.Balance:F2}</td>");
                    html.AppendLine("</tr>");
                }
                
                html.AppendLine("</table>");
                html.AppendLine("</body></html>");
                
                return html.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating HTML report: {ex.Message}");
            }
        }

        private void DailyReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
            LoadDailyData(); // Automatically load data when form opens
        }
    }

    public class DailyReportItem
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}

