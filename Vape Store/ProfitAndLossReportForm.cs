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
                InitializeEnterpriseUI();
                SetInitialState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Profit & Loss Report: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        // Enterprise UI: Tabbed details
        private TabControl tabControl;
        private TabPage tabSummary;
        private TabPage tabRevenue;
        private TabPage tabCOGS;
        private TabPage tabExpenses;
        private DataGridView dgvRevenueBreakdown;
        private DataGridView dgvCogsBreakdown;
        private DataGridView dgvExpenseBreakdown;

        private void InitializeEnterpriseUI()
        {
            // Tabs
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.None; // manual layout for coexisting controls

            tabSummary = new TabPage("Summary");
            tabRevenue = new TabPage("Revenue Breakdown");
            tabCOGS = new TabPage("COGS Breakdown");
            tabExpenses = new TabPage("Expenses Breakdown");

            // Move existing main grid into Summary tab
            dgvProfitLoss.Parent = tabSummary;
            dgvProfitLoss.Dock = DockStyle.Fill;

            // Revenue grid
            dgvRevenueBreakdown = CreateReadOnlyGrid();
            dgvRevenueBreakdown.Parent = tabRevenue;
            dgvRevenueBreakdown.Dock = DockStyle.Fill;

            // COGS grid
            dgvCogsBreakdown = CreateReadOnlyGrid();
            dgvCogsBreakdown.Parent = tabCOGS;
            dgvCogsBreakdown.Dock = DockStyle.Fill;

            // Expenses grid
            dgvExpenseBreakdown = CreateReadOnlyGrid();
            dgvExpenseBreakdown.Parent = tabExpenses;
            dgvExpenseBreakdown.Dock = DockStyle.Fill;

            tabControl.TabPages.Add(tabSummary);
            tabControl.TabPages.Add(tabRevenue);
            tabControl.TabPages.Add(tabCOGS);
            tabControl.TabPages.Add(tabExpenses);

            // Insert into form
            this.Controls.Add(tabControl);
            LayoutEnterpriseUI();
        }


        private DataGridView CreateReadOnlyGrid()
        {
            var grid = new DataGridView();
            grid.AutoGenerateColumns = true;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.DataError += (s, e) => { e.ThrowException = false; };
            return grid;
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
            this.Resize += (s, e) => LayoutEnterpriseUI();
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
                // Prevent default DataGridView data error dialog from popping up
                dgvProfitLoss.DataError += (s, e) => { e.ThrowException = false; };
                
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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
                });
                
                dgvProfitLoss.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Percentage",
                    HeaderText = "% of Net Sales",
                    DataPropertyName = "Percentage",
                    Width = 140,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "P2", Alignment = DataGridViewContentAlignment.MiddleRight }
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
                
                // Build a fresh list each time to avoid CurrencyManager index mismatches
                var items = new List<ProfitLossItem>();
                _summary = new ProfitLossSummary();
                
                // Get sales data
                var sales = _saleRepository.GetSalesByDateRange(fromDate, toDate);
                var totalSales = sales.Sum(s => s.TotalAmount);
                var totalSalesTax = sales.Sum(s => s.TaxAmount);
                var salesReturns = 0m; // placeholder if returns are modeled elsewhere
                var netRevenue = Math.Max(0m, totalSales - totalSalesTax - salesReturns);
                
                // Add revenue items
                items.Add(new ProfitLossItem
                {
                    Category = "REVENUE",
                    Description = "Total Sales",
                    Amount = totalSales,
                    Type = "Revenue",
                    Date = fromDate,
                    IsHeader = true,
                    Percentage = netRevenue > 0 ? totalSales / netRevenue : 0
                });
                
                items.Add(new ProfitLossItem
                {
                    Category = "REVENUE",
                    Description = "Sales Tax",
                    Amount = totalSalesTax,
                    Type = "Revenue",
                    Date = fromDate,
                    IsHeader = false,
                    Percentage = netRevenue > 0 ? totalSalesTax / netRevenue : 0
                });

                items.Add(new ProfitLossItem
                {
                    Category = "REVENUE",
                    Description = "Net Revenue",
                    Amount = netRevenue,
                    Type = "Revenue",
                    Date = fromDate,
                    IsHeader = true,
                    Percentage = netRevenue > 0 ? 1m : 0
                });
                
                _summary.TotalRevenue = netRevenue;
                
                // Get purchase data (Cost of Goods Sold)
                var purchases = _purchaseRepository.GetPurchasesByDateRange(fromDate, toDate);
                var totalPurchases = purchases.Sum(p => p.TotalAmount);
                var totalPurchaseTax = purchases.Sum(p => p.TaxAmount);
                var cogs = Math.Max(0m, totalPurchases - totalPurchaseTax);
                
                // Add COGS items
                items.Add(new ProfitLossItem
                {
                    Category = "COST OF GOODS SOLD",
                    Description = "Purchases (Net of Tax)",
                    Amount = cogs,
                    Type = "Expense",
                    Date = fromDate,
                    IsHeader = true,
                    Percentage = netRevenue > 0 ? cogs / netRevenue : 0
                });
                
                items.Add(new ProfitLossItem
                {
                    Category = "COST OF GOODS SOLD",
                    Description = "Purchase Tax",
                    Amount = totalPurchaseTax,
                    Type = "Expense",
                    Date = fromDate,
                    IsHeader = false,
                    Percentage = netRevenue > 0 ? totalPurchaseTax / netRevenue : 0
                });
                
                _summary.TotalCostOfGoods = cogs;
                _summary.GrossProfit = _summary.TotalRevenue - _summary.TotalCostOfGoods;
                
                // Get expense data
                var expenses = _expenseRepository.GetAllExpenses()
                    .Where(e => e.ExpenseDate >= fromDate && e.ExpenseDate <= toDate && e.Status == "Submitted")
                    .ToList();
                
                var totalExpenses = expenses.Sum(e => e.Amount);
                
                // Add expense items
                items.Add(new ProfitLossItem
                {
                    Category = "OPERATING EXPENSES",
                    Description = "Total Business Expenses",
                    Amount = totalExpenses,
                    Type = "Expense",
                    Date = fromDate,
                    IsHeader = true,
                    Percentage = netRevenue > 0 ? totalExpenses / netRevenue : 0
                });
                
                // Add individual expense categories
                var expenseCategories = expenses.GroupBy(e => e.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(e => e.Amount) })
                    .OrderByDescending(x => x.Amount);
                
                foreach (var category in expenseCategories)
                {
                    items.Add(new ProfitLossItem
                    {
                        Category = "OPERATING EXPENSES",
                        Description = category.Category,
                        Amount = category.Amount,
                        Type = "Expense",
                        Date = fromDate,
                        IsHeader = false,
                        Percentage = netRevenue > 0 ? category.Amount / netRevenue : 0
                    });
                }
                
                _summary.TotalExpenses = totalExpenses;
                var operatingProfit = _summary.GrossProfit - _summary.TotalExpenses;
                _summary.NetProfit = operatingProfit;
                _summary.ProfitMargin = _summary.TotalRevenue > 0 ? (_summary.NetProfit / _summary.TotalRevenue) * 100 : 0;
                
                // Add summary items
                items.Add(new ProfitLossItem
                {
                    Category = "SUMMARY",
                    Description = "Gross Profit",
                    Amount = _summary.GrossProfit,
                    Type = "Summary",
                    Date = fromDate,
                    IsHeader = true,
                    Percentage = netRevenue > 0 ? _summary.GrossProfit / netRevenue : 0
                });
                
                items.Add(new ProfitLossItem
                {
                    Category = "SUMMARY",
                    Description = "Operating/Net Profit",
                    Amount = _summary.NetProfit,
                    Type = "Summary",
                    Date = fromDate,
                    IsHeader = true,
                    Percentage = netRevenue > 0 ? _summary.NetProfit / netRevenue : 0
                });
                
                // Bind to DataGridView
                dgvProfitLoss.DataSource = null; // reset binding to avoid stale CurrencyManager state
                _profitLossItems = items;
                dgvProfitLoss.DataSource = _profitLossItems;
                
                // Apply formatting
                ApplyDataGridViewFormatting();
                
                // Update summary
                UpdateSummaryLabels();
                
                // Build breakdowns
                BuildAndBindBreakdowns(sales, purchases, expenses, netRevenue, cogs, totalExpenses);
                // Profit & Loss report generated successfully
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating profit & loss report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BuildAndBindBreakdowns(List<Sale> sales, List<Purchase> purchases, List<Expense> expenses, decimal netRevenue, decimal cogs, decimal totalExpenses)
        {
            // Revenue by payment method
            var revenueByPayment = sales
                .GroupBy(s => s.PaymentMethod ?? "Unknown")
                .Select(g => new RevenueBreakdownRow
                {
                    PaymentMethod = g.Key,
                    Invoices = g.Count(),
                    Subtotal = g.Sum(x => x.TotalAmount - x.TaxAmount),
                    Tax = g.Sum(x => x.TaxAmount),
                    Total = g.Sum(x => x.TotalAmount),
                    PercentageOfNet = netRevenue > 0 ? g.Sum(x => x.TotalAmount - x.TaxAmount) / netRevenue : 0
                })
                .OrderByDescending(r => r.Total)
                .ToList();
            dgvRevenueBreakdown.DataSource = revenueByPayment;

            // COGS by supplier
            var cogsBySupplier = purchases
                .GroupBy(p => p.SupplierName ?? "Unknown")
                .Select(g => new CogsBreakdownRow
                {
                    Supplier = g.Key,
                    Bills = g.Count(),
                    Subtotal = g.Sum(x => x.TotalAmount - x.TaxAmount),
                    Tax = g.Sum(x => x.TaxAmount),
                    Total = g.Sum(x => x.TotalAmount),
                    PercentageOfNet = netRevenue > 0 ? (g.Sum(x => x.TotalAmount - x.TaxAmount)) / netRevenue : 0
                })
                .OrderByDescending(r => r.Total)
                .ToList();
            dgvCogsBreakdown.DataSource = cogsBySupplier;

            // Expenses by category
            var expenseByCategory = expenses
                .GroupBy(e => e.Category ?? "Other")
                .Select(g => new ExpenseBreakdownRow
                {
                    Category = g.Key,
                    Amount = g.Sum(x => x.Amount),
                    PercentageOfNet = netRevenue > 0 ? g.Sum(x => x.Amount) / netRevenue : 0
                })
                .OrderByDescending(r => r.Amount)
                .ToList();
            dgvExpenseBreakdown.DataSource = expenseByCategory;
        }


        private void LayoutEnterpriseUI()
        {
            try
            {
                // Calculate top anchor based on buttons panel or use default
                int topAnchor = 12;
                if (pnlButtons != null)
                {
                    topAnchor = pnlButtons.Bottom + 12;
                }
                else if (btnClose != null)
                {
                    topAnchor = btnClose.Bottom + 12;
                }

                // Position tab control below buttons panel, above summary panel
                if (tabControl != null)
                {
                    tabControl.Left = 12;
                    tabControl.Top = topAnchor;
                    tabControl.Width = this.ClientSize.Width - 24;
                    
                    // Calculate height - leave space for summary panel at bottom
                    int bottomMargin = pnlSummary != null ? pnlSummary.Height + 12 : 16;
                    tabControl.Height = this.ClientSize.Height - topAnchor - bottomMargin;
                    tabControl.BringToFront();
                }
            }
            catch { }
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
                            row.DefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
                        }
                        
                        // Professional color rules: negatives red, others default
                        if (item.Amount < 0)
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                        else
                            row.DefaultCellStyle.ForeColor = dgvProfitLoss.DefaultCellStyle.ForeColor;
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
                
                html.AppendLine("<h1>MADNI MOBILE & PHOTOSTATE - PROFIT & LOSS REPORT</h1>");
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
                Paragraph title = new Paragraph("MADNI MOBILE & PHOTOSTATE - PROFIT & LOSS REPORT", titleFont);
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
                LayoutEnterpriseUI();
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
        public decimal Percentage { get; set; }
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

    // Breakdown row models for enterprise view
    public class RevenueBreakdownRow
    {
        public string PaymentMethod { get; set; }
        public int Invoices { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal PercentageOfNet { get; set; }
    }

    public class CogsBreakdownRow
    {
        public string Supplier { get; set; }
        public int Bills { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal PercentageOfNet { get; set; }
    }

    public class ExpenseBreakdownRow
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public decimal PercentageOfNet { get; set; }
    }
}