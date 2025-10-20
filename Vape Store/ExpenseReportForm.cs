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
    public partial class ExpenseReportForm : Form
    {
        private ExpenseRepository _expenseRepository;
        private ExpenseCategoryRepository _expenseCategoryRepository;
        private ReportingService _reportingService;
        
        private List<ExpenseReportItem> _expenseReportItems;
        private List<ExpenseCategory> _expenseCategories;

        public ExpenseReportForm()
        {
            InitializeComponent();
            _expenseRepository = new ExpenseRepository();
            _expenseCategoryRepository = new ExpenseCategoryRepository();
            _reportingService = new ReportingService();
            
            _expenseReportItems = new List<ExpenseReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadExpenseCategories();
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
            
            // Filter event handlers
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += ExpenseReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvExpenseReport.AutoGenerateColumns = false;
                dgvExpenseReport.AllowUserToAddRows = false;
                dgvExpenseReport.AllowUserToDeleteRows = false;
                dgvExpenseReport.ReadOnly = true;
                dgvExpenseReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvExpenseReport.MultiSelect = false;

                // Define columns
                dgvExpenseReport.Columns.Clear();
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ExpenseCode",
                    HeaderText = "Code",
                    DataPropertyName = "ExpenseCode",
                    Width = 100
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ExpenseDate",
                    HeaderText = "Date",
                    DataPropertyName = "ExpenseDate",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Category",
                    DataPropertyName = "CategoryName",
                    Width = 150
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 200
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Amount",
                    HeaderText = "Amount",
                    DataPropertyName = "Amount",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentMethod",
                    HeaderText = "Payment Method",
                    DataPropertyName = "PaymentMethod",
                    Width = 120
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReferenceNumber",
                    HeaderText = "Reference",
                    DataPropertyName = "ReferenceNumber",
                    Width = 100
                });
                
                dgvExpenseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Status",
                    DataPropertyName = "Status",
                    Width = 80
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadExpenseCategories()
        {
            try
            {
                _expenseCategories = _expenseCategoryRepository.GetAllExpenseCategories();
                cmbCategory.DataSource = new List<ExpenseCategory> { new ExpenseCategory { CategoryID = 0, CategoryName = "All Categories" } }.Concat(_expenseCategories).ToList();
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expense categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadExpenseData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                var expenses = _expenseRepository.GetExpensesByDateRange(fromDate, toDate);
                _expenseReportItems.Clear();
                
                foreach (var expense in expenses)
                {
                    var reportItem = new ExpenseReportItem
                    {
                        ExpenseID = expense.ExpenseID,
                        ExpenseCode = expense.ExpenseCode,
                        ExpenseDate = expense.ExpenseDate,
                        CategoryName = expense.CategoryName,
                        Description = expense.Description,
                        Amount = expense.Amount,
                        PaymentMethod = expense.PaymentMethod,
                        ReferenceNumber = expense.ReferenceNumber,
                        Status = expense.Status,
                        Remarks = expense.Remarks
                    };
                    
                    _expenseReportItems.Add(reportItem);
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading expense data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _expenseReportItems.AsEnumerable();
                
                // Category filter
                if (cmbCategory.SelectedItem != null)
                {
                    filteredItems = filteredItems.Where(item => item.CategoryName == cmbCategory.Text);
                }
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.Description.ToLower().Contains(searchTerm) ||
                        item.ExpenseCode.ToLower().Contains(searchTerm) ||
                        item.CategoryName.ToLower().Contains(searchTerm) ||
                        item.PaymentMethod.ToLower().Contains(searchTerm));
                }
                
                dgvExpenseReport.DataSource = filteredItems.ToList();
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
                dgvExpenseReport.Refresh();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                var totalExpenses = _expenseReportItems.Sum(item => item.Amount);
                var totalCount = _expenseReportItems.Count;
                var averageExpense = totalCount > 0 ? totalExpenses / totalCount : 0;
                var uniqueCategories = _expenseReportItems.Select(item => item.CategoryName).Distinct().Count();
                
                lblTotalExpenses.Text = $"Total Expenses: ${totalExpenses:F2}";
                lblTotalCount.Text = $"Total Records: {totalCount}";
                lblAverageExpense.Text = $"Average Expense: ${averageExpense:F2}";
                lblUniqueCategories.Text = $"Categories: {uniqueCategories}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            cmbCategory.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadExpenseData();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_expenseReportItems == null || _expenseReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"ExpenseReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
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
                if (_expenseReportItems == null || _expenseReportItems.Count == 0)
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
                if (_expenseReportItems == null || _expenseReportItems.Count == 0)
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
            if (dtpFromDate.Value > dtpToDate.Value)
                dtpToDate.Value = dtpFromDate.Value;
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpToDate.Value < dtpFromDate.Value)
                dtpFromDate.Value = dtpToDate.Value;
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Expense Code,Date,Category,Description,Amount,Payment Method,Reference,Status");
                
                // Write data
                foreach (var item in _expenseReportItems)
                {
                    writer.WriteLine($"{item.ExpenseCode},{item.ExpenseDate:yyyy-MM-dd},{item.CategoryName},{item.Description},{item.Amount:F2},{item.PaymentMethod},{item.ReferenceNumber},{item.Status}");
                }
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void ExpenseReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }
    }

    public class ExpenseReportItem
    {
        public int ExpenseID { get; set; }
        public string ExpenseCode { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}

