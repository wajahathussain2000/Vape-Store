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
    public partial class SalesReturnReportForm : Form
    {
        private SalesReturnRepository _salesReturnRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private ReportingService _reportingService;
        
        private List<SalesReturnReportItem> _salesReturnReportItems;
        private List<Customer> _customers;
        private List<Product> _products;

        public SalesReturnReportForm()
        {
            InitializeComponent();
            _salesReturnRepository = new SalesReturnRepository();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _reportingService = new ReportingService();
            
            _salesReturnReportItems = new List<SalesReturnReportItem>();
            
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
            this.Load += SalesReturnReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvSalesReturnReport.AutoGenerateColumns = false;
                dgvSalesReturnReport.AllowUserToAddRows = false;
                dgvSalesReturnReport.AllowUserToDeleteRows = false;
                dgvSalesReturnReport.ReadOnly = true;
                dgvSalesReturnReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSalesReturnReport.MultiSelect = false;
                dgvSalesReturnReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvSalesReturnReport.AllowUserToResizeColumns = true;
                dgvSalesReturnReport.AllowUserToResizeRows = false;
                dgvSalesReturnReport.RowHeadersVisible = false;
                dgvSalesReturnReport.EnableHeadersVisualStyles = false;
                dgvSalesReturnReport.GridColor = Color.FromArgb(236, 240, 241);
                dgvSalesReturnReport.BorderStyle = BorderStyle.None;
                
                // Set header styling
                dgvSalesReturnReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                dgvSalesReturnReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvSalesReturnReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                dgvSalesReturnReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                // Set row styling
                dgvSalesReturnReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dgvSalesReturnReport.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
                dgvSalesReturnReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
                dgvSalesReturnReport.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvSalesReturnReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvSalesReturnReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvSalesReturnReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvSalesReturnReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvSalesReturnReport.Columns.Clear();
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReturnNumber",
                    HeaderText = "Return #",
                    DataPropertyName = "ReturnNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReturnDate",
                    HeaderText = "Return Date",
                    DataPropertyName = "ReturnDate",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "OriginalInvoiceNumber",
                    HeaderText = "Original Invoice",
                    DataPropertyName = "OriginalInvoiceNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    HeaderText = "Customer",
                    DataPropertyName = "CustomerName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    MinimumWidth = 60
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Total",
                    DataPropertyName = "TotalAmount",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvSalesReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReturnReason",
                    HeaderText = "Reason",
                    DataPropertyName = "ReturnReason",
                    Width = 200,
                    MinimumWidth = 150
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
                // Set date range to include all sales return data
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

        private void LoadSalesReturnData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                var salesReturns = _salesReturnRepository.GetSalesReturnsByDateRange(fromDate, toDate);
                _salesReturnReportItems.Clear();
                
                if (salesReturns.Count == 0)
                {
                    ShowMessage($"No sales returns found for the date range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Please check your date range or ensure there is data in the database.", "No Data Found", MessageBoxIcon.Information);
                    return;
                }
                
                foreach (var salesReturn in salesReturns)
                {
                    // Load the return items for this sales return
                    var returnItems = _salesReturnRepository.GetSalesReturnItems(salesReturn.ReturnID);
                    
                    if (returnItems != null && returnItems.Count > 0)
                    {
                        foreach (var returnItem in returnItems)
                        {
                            // Get product name - if not in return item, look it up by ProductID
                            string productName = returnItem.ProductName;
                            if (string.IsNullOrWhiteSpace(productName) && returnItem.ProductID > 0)
                            {
                                // Try to find product in loaded products list
                                var product = _products?.FirstOrDefault(p => p.ProductID == returnItem.ProductID);
                                if (product != null && !string.IsNullOrWhiteSpace(product.ProductName))
                                {
                                    productName = product.ProductName;
                                }
                                else
                                {
                                    // If not in list, try direct database lookup
                                    try
                                    {
                                        var dbProduct = _productRepository.GetProductById(returnItem.ProductID);
                                        productName = dbProduct?.ProductName ?? $"Product ID: {returnItem.ProductID}";
                                    }
                                    catch
                                    {
                                        productName = $"Product ID: {returnItem.ProductID}";
                                    }
                                }
                            }
                            else if (string.IsNullOrWhiteSpace(productName))
                            {
                                productName = returnItem.ProductID > 0 ? $"Product ID: {returnItem.ProductID}" : "Unknown Product";
                            }
                            
                            var reportItem = new SalesReturnReportItem
                            {
                                ReturnID = salesReturn.ReturnID,
                                ReturnNumber = salesReturn.ReturnNumber,
                                ReturnDate = salesReturn.ReturnDate,
                                OriginalInvoiceNumber = salesReturn.OriginalInvoiceNumber,
                                CustomerName = salesReturn.CustomerName ?? "Unknown",
                                ProductName = productName,
                                Quantity = returnItem.Quantity,
                                UnitPrice = returnItem.UnitPrice,
                                SubTotal = returnItem.SubTotal,
                                TotalAmount = salesReturn.TotalAmount,
                                ReturnReason = salesReturn.ReturnReason
                            };
                            
                            _salesReturnReportItems.Add(reportItem);
                        }
                    }
                    else
                    {
                        // Only show a row if there are actually no items, but don't show incorrect totals
                        var reportItem = new SalesReturnReportItem
                        {
                            ReturnID = salesReturn.ReturnID,
                            ReturnNumber = salesReturn.ReturnNumber,
                            ReturnDate = salesReturn.ReturnDate,
                            OriginalInvoiceNumber = salesReturn.OriginalInvoiceNumber,
                            CustomerName = salesReturn.CustomerName ?? "Unknown",
                            ProductName = "No Items",
                            Quantity = 0,
                            UnitPrice = 0,
                            SubTotal = 0,
                            TotalAmount = 0, // Set to 0 instead of the return total
                            ReturnReason = salesReturn.ReturnReason
                        };
                        
                        _salesReturnReportItems.Add(reportItem);
                    }
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sales return data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _salesReturnReportItems.AsEnumerable();
                
                // Customer filter
                if (cmbCustomer.SelectedItem != null)
                {
                    var selectedCustomer = (Customer)cmbCustomer.SelectedItem;
                    if (selectedCustomer != null && selectedCustomer.CustomerName != "All Customers")
                    {
                        filteredItems = filteredItems.Where(item => item.CustomerName == selectedCustomer.CustomerName);
                    }
                }
                
                // Product filter
                if (cmbProduct.SelectedItem != null)
                {
                    var selectedProduct = (Product)cmbProduct.SelectedItem;
                    if (selectedProduct != null && selectedProduct.ProductName != "All Products")
                    {
                        filteredItems = filteredItems.Where(item => item.ProductName == selectedProduct.ProductName);
                    }
                }
                
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.ReturnNumber?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.OriginalInvoiceNumber?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.CustomerName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ReturnReason?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ReturnDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.ReturnDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.TotalAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.UnitPrice.ToString("F2").Contains(searchTerm)) ||
                        (item.Quantity.ToString().Contains(searchTerm)));
                }
                
                _salesReturnReportItems = filteredItems.ToList();
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
                dgvSalesReturnReport.DataSource = null;
                dgvSalesReturnReport.DataSource = _salesReturnReportItems;
                
                // Format decimal columns
                if (dgvSalesReturnReport.Columns["UnitPrice"] != null)
                    dgvSalesReturnReport.Columns["UnitPrice"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReturnReport.Columns["SubTotal"] != null)
                    dgvSalesReturnReport.Columns["SubTotal"].DefaultCellStyle.Format = "F2";
                if (dgvSalesReturnReport.Columns["TotalAmount"] != null)
                    dgvSalesReturnReport.Columns["TotalAmount"].DefaultCellStyle.Format = "F2";
                
                // Format date column
                if (dgvSalesReturnReport.Columns["ReturnDate"] != null)
                    dgvSalesReturnReport.Columns["ReturnDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
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
                var totalReturns = _salesReturnReportItems.Sum(item => item.TotalAmount);
                var totalQuantity = _salesReturnReportItems.Sum(item => item.Quantity);
                var uniqueCustomers = _salesReturnReportItems.Select(item => item.CustomerName).Distinct().Count();
                var uniqueProducts = _salesReturnReportItems.Select(item => item.ProductName).Distinct().Count();
                var totalReturnsCount = _salesReturnReportItems.Select(item => item.ReturnID).Distinct().Count();
                
                lblTotalReturns.Text = $"Total Returns: {totalReturns:F2}";
                lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                lblTotalReturnsCount.Text = $"Total Returns Count: {totalReturnsCount}";
                lblUniqueCustomers.Text = $"Unique Customers: {uniqueCustomers}";
                lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadSalesReturnData();
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog.FileName = $"SalesReturnReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
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
                if (_salesReturnReportItems.Count == 0)
                {
                    LoadSalesReturnData();
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
            LoadSalesReturnData();
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
                html.AppendLine("<title>Sales Return Report</title>");
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
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                
                // Header
                html.AppendLine("<h1>SALES RETURN REPORT</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<h2>Summary</h2>");
                html.AppendLine("<div class='summary'>");
                html.AppendLine($"<div class='summary-item'>Total Returns: {_salesReturnReportItems.Sum(x => x.TotalAmount):F2}</div>");
                html.AppendLine($"<div class='summary-item'>Total Quantity: {_salesReturnReportItems.Sum(x => x.Quantity)}</div>");
                html.AppendLine($"<div class='summary-item'>Total Returns Count: {_salesReturnReportItems.Select(x => x.ReturnID).Distinct().Count()}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Customers: {_salesReturnReportItems.Select(x => x.CustomerName).Distinct().Count()}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Products: {_salesReturnReportItems.Select(x => x.ProductName).Distinct().Count()}</div>");
                html.AppendLine("</div>");
                
                // Data table
                html.AppendLine("<h2>Sales Return Details</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>Return #</th>");
                html.AppendLine("<th>Date</th>");
                html.AppendLine("<th>Original Invoice</th>");
                html.AppendLine("<th>Customer</th>");
                html.AppendLine("<th>Product</th>");
                html.AppendLine("<th>Qty</th>");
                html.AppendLine("<th>Unit Price</th>");
                html.AppendLine("<th>Sub Total</th>");
                html.AppendLine("<th>Total</th>");
                html.AppendLine("<th>Reason</th>");
                html.AppendLine("</tr>");
                
                foreach (var item in _salesReturnReportItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.ReturnNumber}</td>");
                    html.AppendLine($"<td>{item.ReturnDate:yyyy-MM-dd}</td>");
                    html.AppendLine($"<td>{item.OriginalInvoiceNumber}</td>");
                    html.AppendLine($"<td>{item.CustomerName}</td>");
                    html.AppendLine($"<td>{item.ProductName}</td>");
                    html.AppendLine($"<td>{item.Quantity}</td>");
                    html.AppendLine($"<td>{item.UnitPrice:F2}</td>");
                    html.AppendLine($"<td>{item.SubTotal:F2}</td>");
                    html.AppendLine($"<td>{item.TotalAmount:F2}</td>");
                    html.AppendLine($"<td>{item.ReturnReason}</td>");
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
                Paragraph title = new Paragraph("VAPE STORE - SALES RETURN REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Summary section
                var totalReturns = _salesReturnReportItems.Sum(item => item.TotalAmount);
                var totalQuantity = _salesReturnReportItems.Sum(item => item.Quantity);
                var totalReturnsCount = _salesReturnReportItems.Select(item => item.ReturnID).Distinct().Count();
                var uniqueCustomers = _salesReturnReportItems.Select(item => item.CustomerName).Distinct().Count();
                var uniqueProducts = _salesReturnReportItems.Select(item => item.ProductName).Distinct().Count();

                Paragraph summaryTitle = new Paragraph("SUMMARY", headerFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                // Create summary table
                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 50;
                summaryTable.SetWidths(new float[] { 1, 1 });

                summaryTable.AddCell(new PdfPCell(new Phrase("Total Returns:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase($"{totalReturns:F2}", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Total Quantity:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(totalQuantity.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Total Returns Count:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(totalReturnsCount.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Unique Customers:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(uniqueCustomers.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Unique Products:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(uniqueProducts.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                // Sales return details section
                Paragraph detailsTitle = new Paragraph("SALES RETURN DETAILS", headerFont);
                detailsTitle.SpacingAfter = 10f;
                document.Add(detailsTitle);

                // Create sales return table
                PdfPTable salesReturnTable = new PdfPTable(8);
                salesReturnTable.WidthPercentage = 100;
                salesReturnTable.SetWidths(new float[] { 1.5f, 1.2f, 1.5f, 2f, 2f, 0.8f, 1f, 1f });

                // Add headers
                string[] headers = { "Return #", "Date", "Original Invoice", "Customer", "Product", "Qty", "Unit Price", "Total" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 5f;
                    salesReturnTable.AddCell(headerCell);
                }

                // Add data rows
                foreach (var item in _salesReturnReportItems)
                {
                    salesReturnTable.AddCell(new PdfPCell(new Phrase(item.ReturnNumber, smallFont)) { Padding = 3f });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase(item.ReturnDate.ToString("yyyy-MM-dd"), smallFont)) { Padding = 3f });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase(item.OriginalInvoiceNumber, smallFont)) { Padding = 3f });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase(item.CustomerName.Length > 20 ? item.CustomerName.Substring(0, 20) + "..." : item.CustomerName, smallFont)) { Padding = 3f });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase(item.ProductName.Length > 20 ? item.ProductName.Substring(0, 20) + "..." : item.ProductName, smallFont)) { Padding = 3f });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase($"{item.UnitPrice:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    salesReturnTable.AddCell(new PdfPCell(new Phrase($"{item.TotalAmount:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(salesReturnTable);

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

        private void SalesReturnReportForm_Load(object sender, EventArgs e)
        {
            // Maximize the form
            this.WindowState = FormWindowState.Maximized;
            
            // Set initial state
            SetInitialState();
            
            // Load data automatically
            LoadSalesReturnData();
        }
    }

    // Data class for sales return report items
    public class SalesReturnReportItem
    {
        public int ReturnID { get; set; }
        public string ReturnNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public string OriginalInvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReturnReason { get; set; }
    }
}
