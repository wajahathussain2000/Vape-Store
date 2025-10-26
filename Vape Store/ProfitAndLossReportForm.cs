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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Profit & Loss Report: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
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
                dgvProfitLoss.EnableHeadersVisualStyles = false;
                dgvProfitLoss.GridColor = Color.FromArgb(236, 240, 241);
                
                // Set header styling
                dgvProfitLoss.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvProfitLoss.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvProfitLoss.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvProfitLoss.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvProfitLoss.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvProfitLoss.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvProfitLoss.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvProfitLoss.DefaultCellStyle.SelectionForeColor = Color.White;

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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
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
                    lblNetProfit.Text = "Net Profit: 0.00";
                    lblProfitMargin.Text = "Profit Margin: 0.00%";
                    return;
                }

                lblNetProfit.Text = $"Net Profit: {_summary.NetProfit:F2}";
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
                
                // Profit & Loss report generated successfully
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
                            row.DefaultCellStyle.Font = new System.Drawing.Font(dgvProfitLoss.DefaultCellStyle.Font, FontStyle.Bold);
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

        private void ExportToExcel(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("Category,Description,Amount");
                    
                    // Write data
                    foreach (var item in _profitLossItems)
                    {
                        writer.WriteLine($"{item.Category},{item.Description},{item.Amount:F2}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating Excel file: {ex.Message}");
            }
        }

        private string GenerateHTMLReport()
        {
            try
            {
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html><head>");
                html.AppendLine("<title>Profit & Loss Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { color: #2c3e50; text-align: center; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                html.AppendLine(".summary { background-color: #e8f4f8; padding: 15px; margin: 20px 0; border-radius: 5px; }");
                html.AppendLine(".total { font-weight: bold; background-color: #e8f4f8; }");
                html.AppendLine("</style>");
                html.AppendLine("</head><body>");
                
                html.AppendLine("<h1>VAPE STORE - PROFIT & LOSS REPORT</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Table
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Category</th><th>Description</th><th>Amount</th></tr>");
                
                foreach (var item in _profitLossItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.Category}</td>");
                    html.AppendLine($"<td>{item.Description}</td>");
                    html.AppendLine($"<td>{item.Amount:F2}</td>");
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
                Paragraph title = new Paragraph("VAPE STORE - PROFIT & LOSS REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Create table for report data
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 3f, 1.5f, 1f, 1f });

                // Table headers
                string[] headers = { "Category", "Description", "Amount", "Type", "Date" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var item in _profitLossItems)
                {
                    // Category
                    PdfPCell categoryCell = new PdfPCell(new Phrase(item.Category, normalFont));
                    categoryCell.Padding = 3f;
                    table.AddCell(categoryCell);

                    // Description
                    PdfPCell descCell = new PdfPCell(new Phrase(item.Description, normalFont));
                    descCell.Padding = 3f;
                    table.AddCell(descCell);

                    // Amount
                    PdfPCell amountCell = new PdfPCell(new Phrase(item.Amount.ToString("F2"), normalFont));
                    amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    amountCell.Padding = 3f;
                    table.AddCell(amountCell);

                    // Type
                    PdfPCell typeCell = new PdfPCell(new Phrase(item.Type, normalFont));
                    typeCell.Padding = 3f;
                    table.AddCell(typeCell);

                    // Date
                    PdfPCell dateCell = new PdfPCell(new Phrase(item.Date.ToString("MM/dd/yyyy"), normalFont));
                    dateCell.Padding = 3f;
                    table.AddCell(dateCell);
                }

                document.Add(table);

                // Add summary section
                document.Add(new Paragraph("\n", normalFont));
                
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Summary details
                if (_summary != null)
                {
                    document.Add(new Paragraph($"Total Revenue: {_summary.TotalRevenue:F2}", normalFont));
                    document.Add(new Paragraph($"Total COGS: {_summary.TotalCostOfGoods:F2}", normalFont));
                    document.Add(new Paragraph($"Gross Profit: {_summary.GrossProfit:F2}", normalFont));
                    document.Add(new Paragraph($"Total Expenses: {_summary.TotalExpenses:F2}", normalFont));
                    document.Add(new Paragraph($"Net Profit: {_summary.NetProfit:F2}", normalFont));
                    document.Add(new Paragraph($"Profit Margin: {_summary.ProfitMargin:F2}%", normalFont));
                }

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}");
            }
        }

        // Event Handlers
        private void ProfitAndLossReportForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetInitialState();
                GenerateProfitLossReport(); // Automatically load data when form opens
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Profit & Loss Report: {ex.Message}", "Load Error", MessageBoxIcon.Error);
            }
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

                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveFileDialog.FileName = $"ProfitLossReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportToExcel(saveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error exporting to Excel: {ex.Message}", "Error", MessageBoxIcon.Error);
                }
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

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"ProfitLossReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveFileDialog.FileName);
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
                if (_profitLossItems == null || _profitLossItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    if (_profitLossItems == null || _profitLossItems.Count == 0)
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