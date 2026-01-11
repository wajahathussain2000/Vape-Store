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
using Vape_Store.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Vape_Store
{
    public partial class PurchaseReportForm : Form
    {
        private PurchaseRepository _purchaseRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private ReportingService _reportingService;
        
        private List<PurchaseReportItem> _purchaseReportItems;
        private List<PurchaseReportItem> _originalPurchaseReportItems; // Store original unfiltered data
        private List<Supplier> _suppliers;
        private List<Product> _products;

        public PurchaseReportForm()
        {
            InitializeComponent();
            _purchaseRepository = new PurchaseRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _reportingService = new ReportingService();
            
            _purchaseReportItems = new List<PurchaseReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadSuppliers();
            LoadProducts();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnViewHTML.Click += BtnViewHTML_Click;
            btnExportPDF.Click += BtnExportPDF_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            // Filter event handlers
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += PurchaseReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvPurchaseReport.AutoGenerateColumns = false;
                dgvPurchaseReport.AllowUserToAddRows = false;
                dgvPurchaseReport.AllowUserToDeleteRows = false;
                dgvPurchaseReport.ReadOnly = true;
                dgvPurchaseReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPurchaseReport.MultiSelect = false;
                dgvPurchaseReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvPurchaseReport.AllowUserToResizeColumns = true;
                dgvPurchaseReport.AllowUserToResizeRows = false;
                dgvPurchaseReport.RowHeadersVisible = false;
                dgvPurchaseReport.EnableHeadersVisualStyles = false;
                dgvPurchaseReport.GridColor = Color.FromArgb(236, 240, 241);
                // Suppress default DataGridView data error dialogs
                dgvPurchaseReport.DataError += (s, e) => { e.ThrowException = false; };
                
                // Set header styling
                dgvPurchaseReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvPurchaseReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvPurchaseReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvPurchaseReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvPurchaseReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvPurchaseReport.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvPurchaseReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvPurchaseReport.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvPurchaseReport.BorderStyle = BorderStyle.Fixed3D;
                dgvPurchaseReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvPurchaseReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvPurchaseReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvPurchaseReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvPurchaseReport.Columns.Clear();
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceNumber",
                    HeaderText = "Invoice #",
                    DataPropertyName = "InvoiceNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PurchaseDate",
                    HeaderText = "Purchase Date",
                    DataPropertyName = "PurchaseDate",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Supplier",
                    DataPropertyName = "SupplierName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    MinimumWidth = 60
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Bonus",
                    HeaderText = "Bonus",
                    DataPropertyName = "Bonus",
                    Width = 80,
                    MinimumWidth = 60
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TaxAmount",
                    HeaderText = "Tax",
                    DataPropertyName = "TaxAmount",
                    Width = 100,
                    MinimumWidth = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Total",
                    DataPropertyName = "TotalAmount",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentMethod",
                    HeaderText = "Payment",
                    DataPropertyName = "PaymentMethod",
                    Width = 120,
                    MinimumWidth = 100
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaidAmount",
                    HeaderText = "Paid",
                    DataPropertyName = "PaidAmount",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Balance",
                    HeaderText = "Balance",
                    DataPropertyName = "Balance",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                var supplierList = new List<Supplier> { new Supplier { SupplierID = 0, SupplierName = "All Suppliers" } };
                if (_suppliers != null && _suppliers.Count > 0)
                {
                    supplierList.AddRange(_suppliers);
                }
                SearchableComboBoxHelper.MakeSearchable(cmbSupplier, supplierList, "SupplierName", "SupplierID", "SupplierName");
                cmbSupplier.SelectedIndex = 0; // Select "All Suppliers"
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
                // Ensure ComboBox has at least one item
                var fallbackList = new List<Supplier> { new Supplier { SupplierID = 0, SupplierName = "All Suppliers" } };
                SearchableComboBoxHelper.MakeSearchable(cmbSupplier, fallbackList, "SupplierName", "SupplierID", "SupplierName");
                cmbSupplier.SelectedIndex = 0;
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
            // Set default date range (last 30 days)
            dtpToDate.Value = DateTime.Now;
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            
            // Initialize payment method filter
            var paymentMethods = new List<string> { "All Payment Methods", "Cash", "Credit Card", "Bank Transfer", "Cheque" };
            SearchableComboBoxHelper.MakeSearchable(cmbPaymentMethod, paymentMethods);
            cmbPaymentMethod.SelectedIndex = 0;
            
            // Clear search
            txtSearch.Clear();
            
            // Clear report data
            _purchaseReportItems.Clear();
            dgvPurchaseReport.DataSource = null;
            
            // Clear summary labels
            UpdateSummaryLabels();
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _originalPurchaseReportItems != null && 
                                          _originalPurchaseReportItems.Count > 0 && 
                                          (_purchaseReportItems == null || _purchaseReportItems.Count == 0);
                
                if (hasDataButNoResults)
                {
                    // Show "No results" message in summary
                    lblTotalPurchases.Text = $"Total Purchases: No results found for '{txtSearch.Text}'";
                    lblTotalQuantity.Text = "Total Quantity: No results found";
                    lblTotalTax.Text = "Total Tax: No results found";
                    lblTotalPaid.Text = "Total Paid: No results found";
                    lblTotalBalance.Text = "Total Balance: No results found";
                    lblUniqueSuppliers.Text = "Unique Suppliers: 0";
                    lblUniqueProducts.Text = "Unique Products: 0";
                    return;
                }
                
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    lblTotalPurchases.Text = "Total Purchases: 0.00";
                    lblTotalQuantity.Text = "Total Quantity: 0";
                    lblTotalTax.Text = "Total Tax: 0.00";
                    lblTotalPaid.Text = "Total Paid: 0.00";
                    lblTotalBalance.Text = "Total Balance: 0.00";
                    lblUniqueSuppliers.Text = "Unique Suppliers: 0";
                    lblUniqueProducts.Text = "Unique Products: 0";
                    return;
                }

                var totalPurchases = _purchaseReportItems.Sum(x => x.TotalAmount);
                var totalQuantity = _purchaseReportItems.Sum(x => x.Quantity);
                var totalTax = _purchaseReportItems.Sum(x => x.TaxAmount);
                var totalPaid = _purchaseReportItems.Sum(x => x.PaidAmount);
                var totalBalance = _purchaseReportItems.Sum(x => x.Balance);
                var uniqueSuppliers = _purchaseReportItems.Select(x => x.SupplierName).Distinct().Count();
                var uniqueProducts = _purchaseReportItems.Select(x => x.ProductName).Distinct().Count();

                lblTotalPurchases.Text = $"Total Purchases: {totalPurchases:F2}";
                lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                lblTotalTax.Text = $"Total Tax: {totalTax:F2}";
                lblTotalPaid.Text = $"Total Paid: {totalPaid:F2}";
                lblTotalBalance.Text = $"Total Balance: {totalBalance:F2}";
                lblUniqueSuppliers.Text = $"Unique Suppliers: {uniqueSuppliers}";
                lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating summary labels: {ex.Message}");
            }
        }

        private void GenerateReport()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                // Get purchases by date range
                var purchases = _purchaseRepository.GetPurchasesByDateRange(fromDate, toDate);
                
                // Filter by supplier if selected
                if (cmbSupplier.SelectedItem != null)
                {
                    var selectedSupplier = (dynamic)cmbSupplier.SelectedItem;
                    if (selectedSupplier.SupplierID != 0)
                    {
                        purchases = purchases.Where(p => p.SupplierID == selectedSupplier.SupplierID).ToList();
                    }
                }
                
                // Filter by payment method if selected
                if (cmbPaymentMethod.SelectedItem != null && cmbPaymentMethod.SelectedItem.ToString() != "All Payment Methods")
                {
                    var selectedPaymentMethod = cmbPaymentMethod.SelectedItem.ToString();
                    purchases = purchases.Where(p => p.PaymentMethod == selectedPaymentMethod).ToList();
                }
                
                // Convert to report items
                _purchaseReportItems.Clear();
                
                foreach (var purchase in purchases)
                {
                    if (purchase.PurchaseItems != null && purchase.PurchaseItems.Count > 0)
                    {
                        foreach (var item in purchase.PurchaseItems)
                        {
                            // Filter by product if selected
                            if (cmbProduct.SelectedItem != null)
                            {
                                var selectedProduct = (dynamic)cmbProduct.SelectedItem;
                                if (selectedProduct.ProductID != 0 && item.ProductID != selectedProduct.ProductID)
                                {
                                    continue;
                                }
                            }
                            
                            var reportItem = new PurchaseReportItem
                            {
                                InvoiceNumber = purchase.InvoiceNumber,
                                PurchaseDate = purchase.PurchaseDate,
                                SupplierName = purchase.SupplierName ?? "Unknown",
                                ProductName = item.ProductName ?? "Unknown",
                                Quantity = item.Quantity,
                                Bonus = item.Bonus,
                                UnitPrice = item.UnitPrice,
                                SubTotal = item.SubTotal,
                                TaxAmount = purchase.TaxAmount,
                                TotalAmount = purchase.TotalAmount,
                                PaymentMethod = purchase.PaymentMethod ?? "Unknown",
                                PaidAmount = purchase.PaidAmount,
                                Balance = purchase.TotalAmount - purchase.PaidAmount
                            };
                            
                            _purchaseReportItems.Add(reportItem);
                        }
                    }
                    else
                    {
                        // Handle purchases without items
                        var reportItem = new PurchaseReportItem
                        {
                            InvoiceNumber = purchase.InvoiceNumber,
                            PurchaseDate = purchase.PurchaseDate,
                            SupplierName = purchase.SupplierName ?? "Unknown",
                            ProductName = "No Items",
                            Quantity = 0,
                            Bonus = 0,
                            UnitPrice = 0,
                            SubTotal = purchase.SubTotal,
                            TaxAmount = purchase.TaxAmount,
                            TotalAmount = purchase.TotalAmount,
                            PaymentMethod = purchase.PaymentMethod ?? "Unknown",
                            PaidAmount = purchase.PaidAmount,
                            Balance = purchase.TotalAmount - purchase.PaidAmount
                        };
                        
                        _purchaseReportItems.Add(reportItem);
                    }
                }
                
                // Store original unfiltered data before applying search
                _originalPurchaseReportItems = new List<PurchaseReportItem>(_purchaseReportItems);
                
                // Apply search filter
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    _purchaseReportItems = _purchaseReportItems.Where(x => 
                        (x.InvoiceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                        (x.SupplierName?.ToLower().Contains(searchTerm) ?? false) ||
                        (x.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (x.PaymentMethod?.ToLower().Contains(searchTerm) ?? false) ||
                        (x.PurchaseDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (x.PurchaseDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (x.TotalAmount.ToString("F2").Contains(searchTerm)) ||
                        (x.Quantity.ToString().Contains(searchTerm)) ||
                        (x.Bonus.ToString().Contains(searchTerm)) ||
                        (x.UnitPrice.ToString("F2").Contains(searchTerm))
                    ).ToList();
                }
                
                // Bind to DataGridView (reset first to avoid CurrencyManager index mismatches)
                dgvPurchaseReport.DataSource = null;
                bool hasSearchFilter = !string.IsNullOrWhiteSpace(txtSearch.Text);
                bool hasDataButNoResults = hasSearchFilter && _originalPurchaseReportItems != null && 
                                          _originalPurchaseReportItems.Count > 0 && _purchaseReportItems.Count == 0;
                
                if (hasDataButNoResults)
                {
                    // Show empty grid - message will be in summary labels
                    dgvPurchaseReport.DataSource = new List<PurchaseReportItem>();
                }
                else
                {
                dgvPurchaseReport.DataSource = _purchaseReportItems;
                }
                
                // Update summary
                UpdateSummaryLabels();
                
                // Report generated successfully
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating report: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                Paragraph title = new Paragraph("Attock Mobiles Rwp - PURCHASE REPORT", titleFont);
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
                table.SetWidths(new float[] { 1f, 1.5f, 2f, 1.5f, 1f, 1f, 1f, 1f });

                // Table headers
                string[] headers = { "Invoice", "Date", "Supplier", "Product", "Quantity", "Unit Price", "Total", "Payment" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var item in _purchaseReportItems)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.InvoiceNumber, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.PurchaseDate.ToString("MM/dd/yyyy"), normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.SupplierName, normalFont)) { Padding = 3f });
                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, normalFont)) { Padding = 3f });
                    
                    PdfPCell qtyCell = new PdfPCell(new Phrase(item.Quantity.ToString(), normalFont));
                    qtyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    qtyCell.Padding = 3f;
                    table.AddCell(qtyCell);
                    
                    PdfPCell priceCell = new PdfPCell(new Phrase(item.UnitPrice.ToString("F2"), normalFont));
                    priceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    priceCell.Padding = 3f;
                    table.AddCell(priceCell);
                    
                    PdfPCell totalCell = new PdfPCell(new Phrase(item.TotalAmount.ToString("F2"), normalFont));
                    totalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    totalCell.Padding = 3f;
                    table.AddCell(totalCell);
                    
                    table.AddCell(new PdfPCell(new Phrase(item.PaymentMethod, normalFont)) { Padding = 3f });
                }

                document.Add(table);

                // Add summary section
                document.Add(new Paragraph("\n", normalFont));
                
                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Calculate totals
                var totalAmount = _purchaseReportItems.Sum(x => x.TotalAmount);
                var supplierTotals = _purchaseReportItems.GroupBy(x => x.SupplierName)
                    .Select(g => new { Supplier = g.Key, Total = g.Sum(x => x.TotalAmount) })
                    .OrderByDescending(x => x.Total);

                document.Add(new Paragraph($"Total Purchases: {totalAmount:F2}", normalFont));
                document.Add(new Paragraph("\nBy Supplier:", headerFont));
                
                foreach (var supplier in supplierTotals)
                {
                    document.Add(new Paragraph($"  {supplier.Supplier}: {supplier.Total:F2}", normalFont));
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

        private void ExportToExcel(string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("Invoice Number,Purchase Date,Supplier,Product,Quantity,Unit Price,Sub Total,Tax Amount,Total Amount,Payment Method,Paid Amount,Balance");
                    
                    // Write data
                    foreach (var item in _purchaseReportItems)
                    {
                        writer.WriteLine($"{item.InvoiceNumber},{item.PurchaseDate:yyyy-MM-dd},{item.SupplierName},{item.ProductName},{item.Quantity},{item.UnitPrice:F2},{item.SubTotal:F2},{item.TaxAmount:F2},{item.TotalAmount:F2},{item.PaymentMethod},{item.PaidAmount:F2},{item.Balance:F2}");
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
                html.AppendLine("<title>Purchase Report</title>");
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
                
                html.AppendLine("<h1>Attock Mobiles Rwp - PURCHASE REPORT</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<div class='summary'>");
                html.AppendLine("<h2>Summary</h2>");
                var totalPurchases = _purchaseReportItems.Sum(x => x.TotalAmount);
                var totalPaid = _purchaseReportItems.Sum(x => x.PaidAmount);
                var totalBalance = _purchaseReportItems.Sum(x => x.Balance);
                html.AppendLine($"<p><strong>Total Purchases:</strong> {totalPurchases:F2}</p>");
                html.AppendLine($"<p><strong>Total Paid:</strong> {totalPaid:F2}</p>");
                html.AppendLine($"<p><strong>Total Balance:</strong> {totalBalance:F2}</p>");
                html.AppendLine("</div>");
                
                // Table
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Invoice #</th><th>Date</th><th>Supplier</th><th>Product</th><th>Qty</th><th>Unit Price</th><th>Sub Total</th><th>Tax</th><th>Total</th><th>Payment</th><th>Paid</th><th>Balance</th></tr>");
                
                foreach (var item in _purchaseReportItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.InvoiceNumber}</td>");
                    html.AppendLine($"<td>{item.PurchaseDate:yyyy-MM-dd}</td>");
                    html.AppendLine($"<td>{item.SupplierName}</td>");
                    html.AppendLine($"<td>{item.ProductName}</td>");
                    html.AppendLine($"<td>{item.Quantity}</td>");
                    html.AppendLine($"<td>{item.UnitPrice:F2}</td>");
                    html.AppendLine($"<td>{item.SubTotal:F2}</td>");
                    html.AppendLine($"<td>{item.TaxAmount:F2}</td>");
                    html.AppendLine($"<td>{item.TotalAmount:F2}</td>");
                    html.AppendLine($"<td>{item.PaymentMethod}</td>");
                    html.AppendLine($"<td>{item.PaidAmount:F2}</td>");
                    html.AppendLine($"<td>{item.Balance:F2}</td>");
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

        // Event Handlers
        private void PurchaseReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
            GenerateReport(); // Automatically load data when form opens
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void BtnViewHTML_Click(object sender, EventArgs e)
        {
            try
            {
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveFileDialog.FileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    
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
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
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
                if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
                {
                    ShowMessage("No data to print. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    if (_purchaseReportItems == null || _purchaseReportItems.Count == 0)
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

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-generate report when filter changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
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

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Auto-filter when search text changes
            if (_purchaseReportItems != null && _purchaseReportItems.Count > 0)
            {
                GenerateReport();
            }
        }
    }

    // Report item class for purchase report
    public class PurchaseReportItem
    {
        public string InvoiceNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Bonus { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
    }
}