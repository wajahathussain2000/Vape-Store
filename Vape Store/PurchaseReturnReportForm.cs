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
    public partial class PurchaseReturnReportForm : Form
    {
        private PurchaseReturnRepository _purchaseReturnRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private ReportingService _reportingService;
        
        private List<PurchaseReturnReportItem> _purchaseReturnReportItems;
        private List<Supplier> _suppliers;
        private List<Product> _products;

        public PurchaseReturnReportForm()
        {
            InitializeComponent();
            _purchaseReturnRepository = new PurchaseReturnRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _reportingService = new ReportingService();
            
            _purchaseReturnReportItems = new List<PurchaseReturnReportItem>();
            
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
            btnExportPDF.Click += BtnExportPDF_Click;
            btnViewHTML.Click += BtnViewHTML_Click;
            btnPrint.Click += BtnPrint_Click;
            btnClear.Click += BtnClear_Click;
            btnClose.Click += BtnClose_Click;
            
            // Filter event handlers
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            
            // Date range event handlers
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpToDate.ValueChanged += DtpToDate_ValueChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += PurchaseReturnReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvPurchaseReturnReport.AutoGenerateColumns = false;
                dgvPurchaseReturnReport.AllowUserToAddRows = false;
                dgvPurchaseReturnReport.AllowUserToDeleteRows = false;
                dgvPurchaseReturnReport.ReadOnly = true;
                dgvPurchaseReturnReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPurchaseReturnReport.MultiSelect = false;
                dgvPurchaseReturnReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvPurchaseReturnReport.AllowUserToResizeColumns = true;
                dgvPurchaseReturnReport.AllowUserToResizeRows = false;
                dgvPurchaseReturnReport.RowHeadersVisible = false;
                dgvPurchaseReturnReport.EnableHeadersVisualStyles = true;
                dgvPurchaseReturnReport.GridColor = Color.LightGray;
                dgvPurchaseReturnReport.BorderStyle = BorderStyle.Fixed3D;
                dgvPurchaseReturnReport.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvPurchaseReturnReport.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvPurchaseReturnReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgvPurchaseReturnReport.ColumnHeadersHeight = 35;

                // Define columns
                dgvPurchaseReturnReport.Columns.Clear();
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReturnNumber",
                    HeaderText = "Return #",
                    DataPropertyName = "ReturnNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReturnDate",
                    HeaderText = "Return Date",
                    DataPropertyName = "ReturnDate",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "OriginalInvoiceNumber",
                    HeaderText = "Original Invoice",
                    DataPropertyName = "OriginalInvoiceNumber",
                    Width = 150,
                    MinimumWidth = 120
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Supplier",
                    DataPropertyName = "SupplierName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName",
                    Width = 200,
                    MinimumWidth = 150
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "Quantity",
                    Width = 80,
                    MinimumWidth = 60
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubTotal",
                    HeaderText = "Sub Total",
                    DataPropertyName = "SubTotal",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalAmount",
                    HeaderText = "Total",
                    DataPropertyName = "TotalAmount",
                    Width = 120,
                    MinimumWidth = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvPurchaseReturnReport.Columns.Add(new DataGridViewTextBoxColumn
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
            try
            {
                // Set date range to include all purchase return data
                dtpFromDate.Value = new DateTime(2024, 1, 1);
                dtpToDate.Value = DateTime.Now.AddDays(1);
                
                // Only set SelectedIndex if ComboBoxes have items
                if (cmbSupplier.Items.Count > 0)
                    cmbSupplier.SelectedIndex = 0;
                
                if (cmbProduct.Items.Count > 0)
                    cmbProduct.SelectedIndex = 0;
                
                txtSearch.Clear();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting initial state: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseReturnData()
        {
            try
            {
                var fromDate = dtpFromDate.Value.Date;
                var toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                
                var purchaseReturns = _purchaseReturnRepository.GetPurchaseReturnsByDateRange(fromDate, toDate);
                _purchaseReturnReportItems.Clear();
                
                if (purchaseReturns.Count == 0)
                {
                    ShowMessage($"No purchase returns found for the date range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Please check your date range or ensure there is data in the database.", "No Data Found", MessageBoxIcon.Information);
                    return;
                }
                
                foreach (var purchaseReturn in purchaseReturns)
                {
                    // Load the return items for this purchase return
                    var returnItems = _purchaseReturnRepository.GetPurchaseReturnItems(purchaseReturn.ReturnID);
                    
                    if (returnItems != null && returnItems.Count > 0)
                    {
                        foreach (var returnItem in returnItems)
                        {
                            var reportItem = new PurchaseReturnReportItem
                            {
                                ReturnID = purchaseReturn.ReturnID,
                                ReturnNumber = purchaseReturn.ReturnNumber,
                                ReturnDate = purchaseReturn.ReturnDate,
                                OriginalInvoiceNumber = purchaseReturn.PurchaseID.ToString(),
                                SupplierName = purchaseReturn.SupplierName ?? "Unknown",
                                ProductName = returnItem.ProductName,
                                Quantity = returnItem.Quantity,
                                UnitPrice = returnItem.UnitPrice,
                                SubTotal = returnItem.SubTotal,
                                TotalAmount = purchaseReturn.TotalAmount,
                                ReturnReason = purchaseReturn.ReturnReason
                            };
                            
                            _purchaseReturnReportItems.Add(reportItem);
                        }
                    }
                    else
                    {
                        // Only show a row if there are actually no items, but don't show incorrect totals
                        var reportItem = new PurchaseReturnReportItem
                        {
                            ReturnID = purchaseReturn.ReturnID,
                            ReturnNumber = purchaseReturn.ReturnNumber,
                            ReturnDate = purchaseReturn.ReturnDate,
                            OriginalInvoiceNumber = purchaseReturn.PurchaseID.ToString(),
                            SupplierName = purchaseReturn.SupplierName ?? "Unknown",
                            ProductName = "No Items",
                            Quantity = 0,
                            UnitPrice = 0,
                            SubTotal = 0,
                            TotalAmount = 0, // Set to 0 instead of the return total
                            ReturnReason = purchaseReturn.ReturnReason
                        };
                        
                        _purchaseReturnReportItems.Add(reportItem);
                    }
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading purchase return data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _purchaseReturnReportItems.AsEnumerable();
                
                // Supplier filter
                if (cmbSupplier.SelectedItem != null)
                {
                    var selectedSupplier = (Supplier)cmbSupplier.SelectedItem;
                    if (selectedSupplier != null && selectedSupplier.SupplierName != "All Suppliers")
                    {
                        filteredItems = filteredItems.Where(item => item.SupplierName == selectedSupplier.SupplierName);
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
                        (item.SupplierName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ReturnReason?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ReturnDate.ToString("yyyy-MM-dd").Contains(searchTerm)) ||
                        (item.ReturnDate.ToString("MM/dd/yyyy").Contains(searchTerm)) ||
                        (item.TotalAmount.ToString("F2").Contains(searchTerm)) ||
                        (item.UnitPrice.ToString("F2").Contains(searchTerm)) ||
                        (item.Quantity.ToString().Contains(searchTerm)));
                }
                
                _purchaseReturnReportItems = filteredItems.ToList();
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
                dgvPurchaseReturnReport.DataSource = null;
                dgvPurchaseReturnReport.DataSource = _purchaseReturnReportItems;
                
                // Format decimal columns
                if (dgvPurchaseReturnReport.Columns["UnitPrice"] != null)
                    dgvPurchaseReturnReport.Columns["UnitPrice"].DefaultCellStyle.Format = "F2";
                if (dgvPurchaseReturnReport.Columns["SubTotal"] != null)
                    dgvPurchaseReturnReport.Columns["SubTotal"].DefaultCellStyle.Format = "F2";
                if (dgvPurchaseReturnReport.Columns["TotalAmount"] != null)
                    dgvPurchaseReturnReport.Columns["TotalAmount"].DefaultCellStyle.Format = "F2";
                
                // Format date column
                if (dgvPurchaseReturnReport.Columns["ReturnDate"] != null)
                    dgvPurchaseReturnReport.Columns["ReturnDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
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
                var totalReturns = _purchaseReturnReportItems.Sum(item => item.TotalAmount);
                var totalQuantity = _purchaseReturnReportItems.Sum(item => item.Quantity);
                var uniqueSuppliers = _purchaseReturnReportItems.Select(item => item.SupplierName).Distinct().Count();
                var uniqueProducts = _purchaseReturnReportItems.Select(item => item.ProductName).Distinct().Count();
                var totalReturnsCount = _purchaseReturnReportItems.Select(item => item.ReturnID).Distinct().Count();
                
                lblTotalReturns.Text = $"Total Returns: {totalReturns:F2}";
                lblTotalQuantity.Text = $"Total Quantity: {totalQuantity}";
                lblTotalReturnsCount.Text = $"Total Returns Count: {totalReturnsCount}";
                lblUniqueSuppliers.Text = $"Unique Suppliers: {uniqueSuppliers}";
                lblUniqueProducts.Text = $"Unique Products: {uniqueProducts}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            LoadPurchaseReturnData();
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog.FileName = $"PurchaseReturnReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
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
                if (_purchaseReturnReportItems.Count == 0)
                {
                    LoadPurchaseReturnData();
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
            LoadPurchaseReturnData();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
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
                html.AppendLine("<title>Purchase Return Report</title>");
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
                html.AppendLine("<h1>PURCHASE RETURN REPORT</h1>");
                html.AppendLine($"<p><strong>Report Period:</strong> {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}</p>");
                html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                
                // Summary
                html.AppendLine("<h2>Summary</h2>");
                html.AppendLine("<div class='summary'>");
                html.AppendLine($"<div class='summary-item'>Total Returns: {_purchaseReturnReportItems.Sum(x => x.TotalAmount):F2}</div>");
                html.AppendLine($"<div class='summary-item'>Total Quantity: {_purchaseReturnReportItems.Sum(x => x.Quantity)}</div>");
                html.AppendLine($"<div class='summary-item'>Total Returns Count: {_purchaseReturnReportItems.Select(x => x.ReturnID).Distinct().Count()}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Suppliers: {_purchaseReturnReportItems.Select(x => x.SupplierName).Distinct().Count()}</div>");
                html.AppendLine($"<div class='summary-item'>Unique Products: {_purchaseReturnReportItems.Select(x => x.ProductName).Distinct().Count()}</div>");
                html.AppendLine("</div>");
                
                // Data table
                html.AppendLine("<h2>Purchase Return Details</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>Return #</th>");
                html.AppendLine("<th>Date</th>");
                html.AppendLine("<th>Original Invoice</th>");
                html.AppendLine("<th>Supplier</th>");
                html.AppendLine("<th>Product</th>");
                html.AppendLine("<th>Qty</th>");
                html.AppendLine("<th>Unit Price</th>");
                html.AppendLine("<th>Sub Total</th>");
                html.AppendLine("<th>Total</th>");
                html.AppendLine("<th>Reason</th>");
                html.AppendLine("</tr>");
                
                foreach (var item in _purchaseReturnReportItems)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{item.ReturnNumber}</td>");
                    html.AppendLine($"<td>{item.ReturnDate:yyyy-MM-dd}</td>");
                    html.AppendLine($"<td>{item.OriginalInvoiceNumber}</td>");
                    html.AppendLine($"<td>{item.SupplierName}</td>");
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
                Paragraph title = new Paragraph("VAPE STORE - PURCHASE RETURN REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\nDate Range: {dtpFromDate.Value:yyyy-MM-dd} to {dtpToDate.Value:yyyy-MM-dd}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Summary section
                var totalReturns = _purchaseReturnReportItems.Sum(item => item.TotalAmount);
                var totalQuantity = _purchaseReturnReportItems.Sum(item => item.Quantity);
                var totalReturnsCount = _purchaseReturnReportItems.Select(item => item.ReturnID).Distinct().Count();
                var uniqueSuppliers = _purchaseReturnReportItems.Select(item => item.SupplierName).Distinct().Count();
                var uniqueProducts = _purchaseReturnReportItems.Select(item => item.ProductName).Distinct().Count();

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
                summaryTable.AddCell(new PdfPCell(new Phrase("Unique Suppliers:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(uniqueSuppliers.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                summaryTable.AddCell(new PdfPCell(new Phrase("Unique Products:", normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(uniqueProducts.ToString(), normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                summaryTable.SpacingAfter = 20f;
                document.Add(summaryTable);

                // Purchase return details section
                Paragraph detailsTitle = new Paragraph("PURCHASE RETURN DETAILS", headerFont);
                detailsTitle.SpacingAfter = 10f;
                document.Add(detailsTitle);

                // Create purchase return table
                PdfPTable purchaseReturnTable = new PdfPTable(8);
                purchaseReturnTable.WidthPercentage = 100;
                purchaseReturnTable.SetWidths(new float[] { 1.5f, 1.2f, 1.5f, 2f, 2f, 0.8f, 1f, 1f });

                // Add headers
                string[] headers = { "Return #", "Date", "Original Invoice", "Supplier", "Product", "Qty", "Unit Price", "Total" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 5f;
                    purchaseReturnTable.AddCell(headerCell);
                }

                // Add data rows
                foreach (var item in _purchaseReturnReportItems)
                {
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase(item.ReturnNumber, smallFont)) { Padding = 3f });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase(item.ReturnDate.ToString("yyyy-MM-dd"), smallFont)) { Padding = 3f });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase(item.OriginalInvoiceNumber, smallFont)) { Padding = 3f });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase(item.SupplierName.Length > 20 ? item.SupplierName.Substring(0, 20) + "..." : item.SupplierName, smallFont)) { Padding = 3f });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase(item.ProductName.Length > 20 ? item.ProductName.Substring(0, 20) + "..." : item.ProductName, smallFont)) { Padding = 3f });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_CENTER });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase($"{item.UnitPrice:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    purchaseReturnTable.AddCell(new PdfPCell(new Phrase($"{item.TotalAmount:F2}", smallFont)) { Padding = 3f, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(purchaseReturnTable);

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

        private void PurchaseReturnReportForm_Load(object sender, EventArgs e)
        {
            // Maximize the form
            this.WindowState = FormWindowState.Maximized;
            
            // Set initial state
            SetInitialState();
            
            // Load data automatically
            LoadPurchaseReturnData();
        }
    }

    // Data class for purchase return report items
    public class PurchaseReturnReportItem
    {
        public int ReturnID { get; set; }
        public string ReturnNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public string OriginalInvoiceNumber { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReturnReason { get; set; }
    }
}
