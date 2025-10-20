using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class ProfitAndLossReportForm : Form
    {
        private SaleRepository _saleRepository;
        private PurchaseRepository _purchaseRepository;
        private ExpenseRepository _expenseRepository;
        private ReportingService _reportingService;
        
        private List<ProfitLossItem> _profitLossItems;
        private ProfitLossSummary _summary;

        public ProfitAndLossReportForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _purchaseRepository = new PurchaseRepository();
            _expenseRepository = new ExpenseRepository();
            _reportingService = new ReportingService();
            
            _profitLossItems = new List<ProfitLossItem>();
            _summary = new ProfitLossSummary();
            
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
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Form event handlers
            this.Load += ProfitAndLossReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvProfitLoss.AutoGenerateColumns = false;
                dgvProfitLoss.AllowUserToAddRows = false;
                dgvProfitLoss.AllowUserToDeleteRows = false;
                dgvProfitLoss.ReadOnly = true;
                dgvProfitLoss.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvProfitLoss.MultiSelect = false;

                // Define columns
                dgvProfitLoss.Columns.Clear();
                
                dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Category",
                    HeaderText = "Category",
                    DataPropertyName = "Category",
                    Width = 200
                });
                
                dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 300
                });
                
                dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Amount",
                    HeaderText = "Amount",
                    DataPropertyName = "Amount",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Type",
                    HeaderText = "Type",
                    DataPropertyName = "Type",
                    Width = 100
                });
                
                dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Date",
                    HeaderText = "Date",
                    DataPropertyName = "Date",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            // Set default date range (current month)
            dtpToDate.Value = DateTime.Now;
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            
            // Clear report data
            _profitLossItems.Clear();
            dgvProfitLoss.DataSource = null;
            
            // Clear summary labels
            UpdateSummaryLabels();
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                if (_summary == null)
                {
                    lblTotalRevenue.Text = "Total Revenue: Rs 0.00";
                    lblTotalCostOfGoods.Text = "Total COGS: Rs 0.00";
                    lblGrossProfit.Text = "Gross Profit: Rs 0.00";
                    lblTotalExpenses.Text = "Total Expenses: Rs 0.00";
                    lblNetProfit.Text = "Net Profit: Rs 0.00";
                    lblProfitMargin.Text = "Profit Margin: 0.00%";
                    return;
                }

                lblTotalRevenue.Text = $"Total Revenue: Rs {_summary.TotalRevenue:F2}";
                lblTotalCostOfGoods.Text = $"Total COGS: Rs {_summary.TotalCostOfGoods:F2}";
                lblGrossProfit.Text = $"Gross Profit: Rs {_summary.GrossProfit:F2}";
                lblTotalExpenses.Text = $"Total Expenses: Rs {_summary.TotalExpenses:F2}";
                lblNetProfit.Text = $"Net Profit: Rs {_summary.NetProfit:F2}";
                lblProfitMargin.Text = $"Profit Margin: {_summary.ProfitMargin:F2}%";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating summary labels: {ex.Message}");
            }
        }

        private void GenerateProfitLossReport()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                _profitLossItems.Clear();
                _summary = new ProfitLossSummary();
                
                // Get sales data
                var sales = _saleRepository.GetSalesByDateRange(fromDate, toDate);
                var totalSales = sales.Sum(s => s.TotalAmount);
                var totalSalesTax = sales.Sum(s => s.TaxAmount);
                
                // Add revenue items
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "REVENUE",
                    Description = "Total Sales",
                    Amount = totalSales,
                    Type = "Revenue",
                    Date = fromDate,
                    IsHeader = true
                });
                
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "REVENUE",
                    Description = "Sales Tax",
                    Amount = totalSalesTax,
                    Type = "Revenue",
                    Date = fromDate,
                    IsHeader = false
                });
                
                _summary.TotalRevenue = totalSales;
                
                // Get purchase data (Cost of Goods Sold)
                var purchases = _purchaseRepository.GetPurchasesByDateRange(fromDate, toDate);
                var totalPurchases = purchases.Sum(p => p.TotalAmount);
                var totalPurchaseTax = purchases.Sum(p => p.TaxAmount);
                
                // Add COGS items
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "COST OF GOODS SOLD",
                    Description = "Total Purchases",
                    Amount = totalPurchases,
                    Type = "Expense",
                    Date = fromDate,
                    IsHeader = true
                });
                
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "COST OF GOODS SOLD",
                    Description = "Purchase Tax",
                    Amount = totalPurchaseTax,
                    Type = "Expense",
                    Date = fromDate,
                    IsHeader = false
                });
                
                _summary.TotalCostOfGoods = totalPurchases;
                _summary.GrossProfit = _summary.TotalRevenue - _summary.TotalCostOfGoods;
                
                // Get expense data
                var expenses = _expenseRepository.GetAllExpenses()
                    .Where(e => e.ExpenseDate >= fromDate && e.ExpenseDate <= toDate && e.Status == "Submitted")
                    .ToList();
                
                var totalExpenses = expenses.Sum(e => e.Amount);
                
                // Add expense items
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "OPERATING EXPENSES",
                    Description = "Total Business Expenses",
                    Amount = totalExpenses,
                    Type = "Expense",
                    Date = fromDate,
                    IsHeader = true
                });
                
                // Add individual expense categories
                var expenseCategories = expenses.GroupBy(e => e.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(e => e.Amount) })
                    .OrderByDescending(x => x.Amount);
                
                foreach (var category in expenseCategories)
                {
                    _profitLossItems.Add(new ProfitLossItem
                    {
                        Category = "OPERATING EXPENSES",
                        Description = category.Category,
                        Amount = category.Amount,
                        Type = "Expense",
                        Date = fromDate,
                        IsHeader = false
                    });
                }
                
                _summary.TotalExpenses = totalExpenses;
                _summary.NetProfit = _summary.GrossProfit - _summary.TotalExpenses;
                _summary.ProfitMargin = _summary.TotalRevenue > 0 ? (_summary.NetProfit / _summary.TotalRevenue) * 100 : 0;
                
                // Add summary items
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "SUMMARY",
                    Description = "Gross Profit",
                    Amount = _summary.GrossProfit,
                    Type = "Summary",
                    Date = fromDate,
                    IsHeader = true
                });
                
                _profitLossItems.Add(new ProfitLossItem
                {
                    Category = "SUMMARY",
                    Description = "Net Profit",
                    Amount = _summary.NetProfit,
                    Type = "Summary",
                    Date = fromDate,
                    IsHeader = true
                });
                
                // Bind to DataGridView
                dgvProfitLoss.DataSource = _profitLossItems;
                
                // Apply formatting
                ApplyDataGridViewFormatting();
                
                // Update summary
                UpdateSummaryLabels();
                
                ShowMessage($"Profit & Loss report generated successfully for the period {fromDate:MM/dd/yyyy} to {toDate:MM/dd/yyyy}.", "Success", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating profit & loss report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyDataGridViewFormatting()
        {
            try
            {
                foreach (DataGridViewRow row in dgvProfitLoss.Rows)
                {
                    if (row.DataBoundItem is ProfitLossItem item)
                    {
                        if (item.IsHeader)
                        {
                            row.DefaultCellStyle.Font = new Font(dgvProfitLoss.DefaultCellStyle.Font, FontStyle.Bold);
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        
                        if (item.Category == "REVENUE")
                        {
                            row.DefaultCellStyle.ForeColor = Color.Green;
                        }
                        else if (item.Category == "COST OF GOODS SOLD" || item.Category == "OPERATING EXPENSES")
                        {
                            row.DefaultCellStyle.ForeColor = Color.Red;
                        }
                        else if (item.Category == "SUMMARY")
                        {
                            row.DefaultCellStyle.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying DataGridView formatting: {ex.Message}");
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        // Event Handlers
        private void ProfitAndLossReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateProfitLossReport();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_profitLossItems == null || _profitLossItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement Excel export functionality
                ShowMessage("Excel export functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to Excel: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_profitLossItems == null || _profitLossItems.Count == 0)
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
                if (_profitLossItems == null || _profitLossItems.Count == 0)
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
            SetInitialState();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            // Validate date range
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                dtpToDate.Value = dtpFromDate.Value;
            }
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            // Validate date range
            if (dtpToDate.Value < dtpFromDate.Value)
            {
                dtpFromDate.Value = dtpToDate.Value;
            }
        }
    }

    // Data classes for profit & loss report
    public class ProfitLossItem
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public bool IsHeader { get; set; }
    }

    public class ProfitLossSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCostOfGoods { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}