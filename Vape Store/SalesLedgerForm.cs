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
using Vape_Store.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class SalesLedgerForm : Form
    {
        private SaleRepository _saleRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private CustomerLedgerRepository _customerLedgerRepository;
        private ReportingService _reportingService;
        
        private List<SalesLedgerItem> _salesLedgerItems;
        private List<Customer> _customers;
        private List<Product> _products;

        public SalesLedgerForm()
        {
            InitializeComponent();
            _saleRepository = new SaleRepository();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _customerLedgerRepository = new CustomerLedgerRepository();
            _reportingService = new ReportingService();
            
            _salesLedgerItems = new List<SalesLedgerItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadCustomers();
            LoadProducts();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnViewHTML.Click += BtnViewHTML_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            // Filter event handlers
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += SalesLedgerForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvSalesLedger.AutoGenerateColumns = false;
                dgvSalesLedger.AllowUserToAddRows = false;
                dgvSalesLedger.AllowUserToDeleteRows = false;
                dgvSalesLedger.ReadOnly = true;
                dgvSalesLedger.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSalesLedger.MultiSelect = false;
                dgvSalesLedger.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvSalesLedger.AllowUserToResizeColumns = true;
                dgvSalesLedger.AllowUserToResizeRows = false;
                dgvSalesLedger.RowHeadersVisible = false;
                dgvSalesLedger.EnableHeadersVisualStyles = false;
                dgvSalesLedger.GridColor = Color.FromArgb(236, 240, 241);
                
                // Set header styling
                dgvSalesLedger.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvSalesLedger.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvSalesLedger.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvSalesLedger.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvSalesLedger.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvSalesLedger.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvSalesLedger.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvSalesLedger.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvSalesLedger.BorderStyle = BorderStyle.Fixed3D;
                dgvSalesLedger.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvSalesLedger.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvSalesLedger.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvSalesLedger.ColumnHeadersHeight = 35;

                // Define columns for ledger view
                dgvSalesLedger.Columns.Clear();
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Date",
                    HeaderText = "Date",
                    DataPropertyName = "Date",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice #",
                    DataPropertyName = "InvoiceNumber",
                    Width = 120,
                    MinimumWidth = 100
                });
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    HeaderText = "Customer",
                    DataPropertyName = "CustomerName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 250,
                    MinimumWidth = 200
                });
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Debit",
                    HeaderText = "Debit",
                    DataPropertyName = "Debit",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
                });
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Credit",
                    HeaderText = "Credit",
                    DataPropertyName = "Credit",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
                });
                
                dgvSalesLedger.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Balance",
                    HeaderText = "Balance",
                    DataPropertyName = "Balance",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2", Alignment = DataGridViewContentAlignment.MiddleRight }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                var customerList = new List<Customer> { new Customer { CustomerID = 0, CustomerName = "All Customers" } };
                if (_customers != null && _customers.Count > 0)
                {
                    customerList.AddRange(_customers);
                }
                SearchableComboBoxHelper.MakeSearchable(cmbCustomer, customerList, "CustomerName", "CustomerID", "CustomerName");
                cmbCustomer.SelectedIndex = 0; // Select "All Customers"
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
                // Ensure ComboBox has at least one item
                var fallbackList = new List<Customer> { new Customer { CustomerID = 0, CustomerName = "All Customers" } };
                SearchableComboBoxHelper.MakeSearchable(cmbCustomer, fallbackList, "CustomerName", "CustomerID", "CustomerName");
                cmbCustomer.SelectedIndex = 0;
            }
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                var productList = new List<Product> { new Product { ProductID = 0, ProductName = "All Products" } };
                if (_products != null && _products.Count > 0)
                {
                    productList.AddRange(_products);
                }
                SearchableComboBoxHelper.MakeSearchable(cmbProduct, productList, "ProductName", "ProductID", "ProductName");
                cmbProduct.SelectedIndex = 0; // Select "All Products"
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
                // Ensure ComboBox has at least one item
                var fallbackList = new List<Product> { new Product { ProductID = 0, ProductName = "All Products" } };
                SearchableComboBoxHelper.MakeSearchable(cmbProduct, fallbackList, "ProductName", "ProductID", "ProductName");
                cmbProduct.SelectedIndex = 0;
            }
        }

        private void SetInitialState()
        {
            try
            {
                // Set date range to include all sales data
                dtpFromDate.Value = new DateTime(2024, 1, 1);
                dtpToDate.Value = DateTime.Now.AddDays(1);
                
                // Only set SelectedIndex if ComboBoxes have items
                if (cmbCustomer.Items.Count > 0)
                    cmbCustomer.SelectedIndex = 0;
                
                if (cmbProduct.Items.Count > 0)
                    cmbProduct.SelectedIndex = 0;
                
                txtSearch.Clear();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting initial state: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSalesLedgerData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day

                int? selectedCustomerId = null;
                if (cmbCustomer.SelectedItem is Customer selectedCustomer && selectedCustomer.CustomerID != 0)
                {
                    selectedCustomerId = selectedCustomer.CustomerID;
                }

                var ledgerEntries = _customerLedgerRepository.GetEntries(fromDate, toDate, selectedCustomerId);
                _salesLedgerItems = new List<SalesLedgerItem>();

                foreach (var entry in ledgerEntries)
                {
                    _salesLedgerItems.Add(new SalesLedgerItem
                    {
                        CustomerID = entry.CustomerID,
                        Date = entry.EntryDate,
                        InvoiceNumber = entry.InvoiceNumber,
                        CustomerName = entry.CustomerName ?? "Walk-in Customer",
                        Description = entry.Description,
                        Debit = entry.Debit,
                        Credit = entry.Credit,
                        Balance = entry.Balance
                    });
                }

                if (_salesLedgerItems.Count == 0)
                {
                    ShowMessage($"No ledger entries found for the date range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}.", "No Data Found", MessageBoxIcon.Information);
                }

                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sales ledger data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _salesLedgerItems.AsEnumerable();
                
                // Customer filter
                if (cmbCustomer.SelectedItem is Customer selectedCustomer && selectedCustomer.CustomerID != 0)
                {
                    filteredItems = filteredItems.Where(item => item.CustomerID == selectedCustomer.CustomerID);
                }
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.InvoiceNumber.ToLower().Contains(searchTerm) ||
                        item.CustomerName.ToLower().Contains(searchTerm) ||
                        item.Description.ToLower().Contains(searchTerm));
                }
                
                _salesLedgerItems = filteredItems.ToList();
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
                dgvSalesLedger.DataSource = null;
                dgvSalesLedger.DataSource = _salesLedgerItems;
                
                // Format decimal columns
                if (dgvSalesLedger.Columns["Debit"] != null)
                    dgvSalesLedger.Columns["Debit"].DefaultCellStyle.Format = "F2";
                if (dgvSalesLedger.Columns["Credit"] != null)
                    dgvSalesLedger.Columns["Credit"].DefaultCellStyle.Format = "F2";
                if (dgvSalesLedger.Columns["Balance"] != null)
                    dgvSalesLedger.Columns["Balance"].DefaultCellStyle.Format = "F2";
                
                // Format date column
                if (dgvSalesLedger.Columns["Date"] != null)
                    dgvSalesLedger.Columns["Date"].DefaultCellStyle.Format = "yyyy-MM-dd";
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
                var totalDebit = _salesLedgerItems.Sum(item => item.Debit);
                var totalCredit = _salesLedgerItems.Sum(item => item.Credit);
                var currentBalance = _salesLedgerItems.LastOrDefault()?.Balance ?? 0;
                var totalTransactions = _salesLedgerItems.Count;
                var uniqueCustomers = _salesLedgerItems.Select(item => item.CustomerName).Distinct().Count();
                
                lblTotalDebit.Text = $"Total Debit: {totalDebit:F2}";
                lblTotalCredit.Text = $"Total Credit: {totalCredit:F2}";
                lblBalance.Text = $"Current Balance: {currentBalance:F2}";
                lblTotalTransactions.Text = $"Total Transactions: {totalTransactions}";
                lblUniqueCustomers.Text = $"Unique Customers: {uniqueCustomers}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadSalesLedgerData();
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SalesLedger_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveFileDialog.FileName);
                    ShowMessage($"PDF report exported successfully to: {saveFileDialog.FileName}", "Export Complete", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting PDF: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnViewHTML_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure data is loaded before generating HTML report
                if (_salesLedgerItems.Count == 0)
                {
                    LoadSalesLedgerData();
                }
                
                var htmlContent = GenerateHTMLReport();
                var htmlViewer = new HTMLReportViewerForm();
                htmlViewer.LoadReport(htmlContent);
                htmlViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error opening HTML report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement print functionality
                ShowMessage("Print functionality will be implemented with PrintDocument class.", "Info", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
            LoadSalesLedgerData();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                dtpToDate.Value = dtpFromDate.Value;
            }
        }

        private void DtpToDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpToDate.Value < dtpFromDate.Value)
            {
                dtpFromDate.Value = dtpToDate.Value;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
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
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<title>Sales Ledger Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; line-height: 1.4; }");
                html.AppendLine("h1 { color: #2c3e50; text-align: center; margin-bottom: 20px; }");
                html.AppendLine("h2 { color: #34495e; border-bottom: 2px solid #3498db; margin-top: 30px; margin-bottom: 15px; }");
                html.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; font-size: 12px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; vertical-align: top; }");
                html.AppendLine("th { background-color: #2c3e50; color: white; font-weight: bold; text-align: center; }");
                html.AppendLine("tr:nth-child(even) { background-color: #f8f9fa; }");
                html.AppendLine(".summary { background-color: #ecf0f1; padding: 15px; border-radius: 5px; margin: 20px 0; }");
                html.AppendLine(".summary-item { margin: 8px 0; font-weight: bold; }");
                html.AppendLine("td { word-wrap: break-word; max-width: 150px; }");
                html.AppendLine(".debit { color: #e74c3c; font-weight: bold; }");
                html.AppendLine(".credit { color: #27ae60; font-weight: bold; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                
                // Header
                html.AppendLine("<h1>SALES LEDGER REPORT</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<h2>Summary</h2>");
                html.AppendLine("<div class='summary'>");
                html.AppendLine($"<div class='summary-item'>Total Debit: {_salesLedgerItems.Sum(x => x.Debit):F2}</div>");
                html.AppendLine($"<div class='summary-item'>Total Credit: {_salesLedgerItems.Sum(x => x.Credit):F2}</div>");
                html.AppendLine($"<div class='summary-item'>Current Balance: {_salesLedgerItems.LastOrDefault()?.Balance ?? 0:F2}</div>");
                html.AppendLine($"<div class='summary-item'>Total Transactions: {_salesLedgerItems.Count}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Customers: {_salesLedgerItems.Select(x => x.CustomerName).Distinct().Count()}</div>");
                html.AppendLine("</div>");
                
                // Data table
                html.AppendLine("<h2>Sales Ledger Details</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>Date</th>");
                html.AppendLine("<th>Invoice #</th>");
                html.AppendLine("<th>Customer</th>");
                html.AppendLine("<th>Description</th>");
                html.AppendLine("<th>Debit</th>");
                html.AppendLine("<th>Credit</th>");
                html.AppendLine("<th>Balance</th>");
                html.AppendLine("</tr>");
                
                foreach (var item in _salesLedgerItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.Date:yyyy-MM-dd}</td>");
                    html.AppendLine($"<td>{item.InvoiceNumber}</td>");
                    html.AppendLine($"<td>{item.CustomerName}</td>");
                    html.AppendLine($"<td>{item.Description}</td>");
                    html.AppendLine($"<td class='debit'>{item.Debit:F2}</td>");
                    html.AppendLine($"<td class='credit'>{item.Credit:F2}</td>");
                    html.AppendLine($"<td>{item.Balance:F2}</td>");
                    html.AppendLine("</tr>");
                }
                
                html.AppendLine("</table>");
                html.AppendLine("</body>");
                html.AppendLine("</html>");
                
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
                Paragraph title = new Paragraph("VAPE STORE - SALES LEDGER REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Summary section
                var totalDebit = _salesLedgerItems.Sum(item => item.Debit);
                var totalCredit = _salesLedgerItems.Sum(item => item.Credit);
                var currentBalance = _salesLedgerItems.LastOrDefault()?.Balance ?? 0;
                var totalTransactions = _salesLedgerItems.Count;
                var uniqueCustomers = _salesLedgerItems.Select(item => item.CustomerName).Distinct().Count();

                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Create summary table
                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 50;
                summaryTable.SetWidths(new float[] { 1, 1 });

                summaryTable.AddCell(new PdfPCell(new Phrase("Total Debit:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase($"{totalDebit:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Total Credit:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase($"{totalCredit:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Current Balance:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase($"{currentBalance:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Total Transactions:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(totalTransactions.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Unique Customers:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(uniqueCustomers.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                // Sales ledger details section
                Paragraph detailsTitle = new Paragraph("SALES LEDGER DETAILS", headerFont);
                detailsTitle.SpacingAfter = 10f;
                document.Add(detailsTitle);

                // Create sales ledger table
                PdfPTable ledgerTable = new PdfPTable(7);
                ledgerTable.WidthPercentage = 100;
                ledgerTable.SetWidths(new float[] { 1f, 1.2f, 2f, 2.5f, 1f, 1f, 1f });

                // Add headers
                string[] headers = { "Date", "Invoice #", "Customer", "Description", "Debit", "Credit", "Balance" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 5f;
                    ledgerTable.AddCell(headerCell);
                }

                // Add data rows
                foreach (var item in _salesLedgerItems)
                {
                    ledgerTable.AddCell(new PdfPCell(new Phrase(item.Date.ToString("yyyy-MM-dd"), smallFont)) { Padding = 3f });
                    ledgerTable.AddCell(new PdfPCell(new Phrase(item.InvoiceNumber, smallFont)) { Padding = 3f });
                    ledgerTable.AddCell(new PdfPCell(new Phrase(item.CustomerName.Length > 20 ? item.CustomerName.Substring(0, 20) + "..." : item.CustomerName, smallFont)) { Padding = 3f });
                    ledgerTable.AddCell(new PdfPCell(new Phrase(item.Description.Length > 30 ? item.Description.Substring(0, 30) + "..." : item.Description, smallFont)) { Padding = 3f });
                    ledgerTable.AddCell(new PdfPCell(new Phrase($"{item.Debit:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    ledgerTable.AddCell(new PdfPCell(new Phrase($"{item.Credit:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    ledgerTable.AddCell(new PdfPCell(new Phrase($"{item.Balance:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(ledgerTable);

                // Footer
                Paragraph footer = new Paragraph($"\n\nReport generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}", smallFont);
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}");
            }
        }

        private void SalesLedgerForm_Load(object sender, EventArgs e)
        {
            // Maximize the form
            this.WindowState = FormWindowState.Maximized;
            
            // Set initial state
            SetInitialState();
            
            // Load data automatically
            LoadSalesLedgerData();
        }
    }

    // Data class for sales ledger items
    public class SalesLedgerItem
    {
        public int CustomerID { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
