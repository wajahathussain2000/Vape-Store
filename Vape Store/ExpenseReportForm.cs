using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class ExpenseReportForm : Form
    {
        private ExpenseRepository _expenseRepository;
        private ExpenseCategoryRepository _expenseCategoryRepository;
        private ReportingService _reportingService;
        
        private List<ExpenseReportItem> _expenseReportItems;
        private List<ExpenseReportItem> _originalExpenseReportItems; // Store original unfiltered data
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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
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
                
                // Store original unfiltered data
                _originalExpenseReportItems = new List<ExpenseReportItem>(_expenseReportItems);
                
                ApplyFilters();
                RefreshDataGridView();
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
                // Use original unfiltered data as base
                var filteredItems = _originalExpenseReportItems?.AsEnumerable() ?? _expenseReportItems.AsEnumerable();
                
                // Category filter
                if (cmbCategory.SelectedItem != null)
                {
                    var selectedCategory = (ExpenseCategory)cmbCategory.SelectedItem;
                    if (selectedCategory != null && selectedCategory.CategoryName != "All Categories")
                    {
                        filteredItems = filteredItems.Where(item => item.CategoryName == selectedCategory.CategoryName);
                    }
                }
                
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.Description?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ExpenseCode?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.CategoryName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.PaymentMethod?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ExpenseDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.ExpenseDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.Amount.ToString("F2").Contains(searchTerm)));
                }
                
                // Update filtered list and DataGridView
                var filteredList = filteredItems.ToList();
                dgvExpenseReport.DataSource = filteredList;
                
                // Update summary labels based on filtered data
                UpdateSummaryLabels(filteredList);
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

        private void UpdateSummaryLabels(List<ExpenseReportItem> items = null)
        {
            try
            {
                // Use provided filtered items or current items
                var itemsToUse = items ?? _expenseReportItems;
                
                var totalExpenses = itemsToUse.Sum(item => item.Amount);
                var totalCount = itemsToUse.Count;
                var averageExpense = totalCount > 0 ? totalExpenses / totalCount : 0;
                var uniqueCategories = itemsToUse.Select(item => item.CategoryName).Distinct().Count();
                
                lblTotalExpenses.Text = $"Total Expenses: {totalExpenses:F2}";
                lblTotalCount.Text = $"Total Records: {totalCount}";
                lblAverageExpense.Text = $"Average Expense: {averageExpense:F2}";
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

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"ExpenseReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
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
                if (_expenseReportItems == null || _expenseReportItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    if (_expenseReportItems == null || _expenseReportItems.Count == 0)
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
                Paragraph title = new Paragraph("MADNI MOBILE & PHOTOSTATE - EXPENSE REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Create table for report data
                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 1f, 1.5f, 2f, 1f, 1.5f, 1f, 1f });

                // Table headers
                string[] headers = { "Code", "Date", "Category", "Description", "Amount", "Payment", "Reference", "Status" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var item in _expenseReportItems)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.ExpenseCode, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.ExpenseDate.ToString("MM/dd/yyyy"), normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.CategoryName, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Description, normalFont)) { Padding = 3f });
                    
                    PdfPCell amountCell = new PdfPCell(new Phrase(item.Amount.ToString("F2"), normalFont));
                    amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    amountCell.Padding = 3f;
                    table.AddCell(amountCell);
                    
                    table.AddCell(new PdfPCell(new Phrase(item.PaymentMethod, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.ReferenceNumber, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.Status, normalFont)) { Padding = 3f });
                }

                document.Add(table);

                // Add summary section
                document.Add(new Paragraph("\n", normalFont));
                
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Calculate totals
                var totalAmount = _expenseReportItems.Sum(x => x.Amount);
                var categoryTotals = _expenseReportItems.GroupBy(x => x.CategoryName)
                    .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                    .OrderByDescending(x => x.Total);

                document.Add(new Paragraph($"Total Expenses: {totalAmount:F2}", normalFont));
                document.Add(new Paragraph("\nBy Category:", headerFont));
                
                foreach (var category in categoryTotals)
                {
                    document.Add(new Paragraph($"  {category.Category}: {category.Total:F2}", normalFont));
                }

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
                html.AppendLine("<title>Expense Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { color: #2c3e50; text-align: center; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                html.AppendLine(".summary { background-color: #e8f4f8; padding: 15px; margin: 20px 0; border-radius: 5px; }");
                html.AppendLine("</style>");
                html.AppendLine("</head><body>");
                
                html.AppendLine("<h1>MADNI MOBILE & PHOTOSTATE - EXPENSE REPORT</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<div class='summary'>");
                html.AppendLine("<h2>Summary</h2>");
                var totalExpenses = _expenseReportItems.Sum(x => x.Amount);
                html.AppendLine($"<p><strong>Total Expenses:</strong> {_expenseReportItems.Count}</p>");
                html.AppendLine($"<p><strong>Total Amount:</strong> {totalExpenses:F2}</p>");
                html.AppendLine("</div>");
                
                // Table
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Expense Code</th><th>Date</th><th>Category</th><th>Amount</th><th>Payment Method</th><th>Description</th></tr>");
                
                foreach (var item in _expenseReportItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.ExpenseCode}</td>");
                    html.AppendLine($"<td>{item.ExpenseDate:yyyy-MM-dd}</td>");
                    html.AppendLine($"<td>{item.CategoryName}</td>");
                    html.AppendLine($"<td>{item.Amount:F2}</td>");
                    html.AppendLine($"<td>{item.PaymentMethod}</td>");
                    html.AppendLine($"<td>{item.Description}</td>");
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

        private void ExpenseReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
            LoadExpenseData(); // Automatically load data when form opens
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

