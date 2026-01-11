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
    public partial class StockReportForm : Form
    {
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private InventoryService _inventoryService;
        
        private List<Product> _allProducts;
        private List<Category> _categories;
        private List<Brand> _brands;
        private List<StockReportItem> _stockReportItems;

        public StockReportForm()
        {
            InitializeComponent();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            
            _stockReportItems = new List<StockReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadCategories();
            LoadBrands();
            LoadStockData();
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
            
            // Filter event handlers
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            cmbBrand.SelectedIndexChanged += CmbBrand_SelectedIndexChanged;
            chkLowStock.CheckedChanged += ChkLowStock_CheckedChanged;
            chkOutOfStock.CheckedChanged += ChkOutOfStock_CheckedChanged;
            chkInStock.CheckedChanged += ChkInStock_CheckedChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += StockReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvStockReport.AutoGenerateColumns = false;
                dgvStockReport.AllowUserToAddRows = false;
                dgvStockReport.AllowUserToDeleteRows = false;
                dgvStockReport.ReadOnly = true;
                dgvStockReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvStockReport.MultiSelect = false;

                // Define columns
                dgvStockReport.Columns.Clear();
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductCode",
                    HeaderText = "Product Code",
                    DataPropertyName = "ProductCode",
                    Width = 100
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product Name",
                    DataPropertyName = "ProductName",
                    Width = 200
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Category",
                    DataPropertyName = "CategoryName",
                    Width = 120
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "BrandName",
                    HeaderText = "Brand",
                    DataPropertyName = "BrandName",
                    Width = 120
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StockQuantity",
                    HeaderText = "Stock Qty",
                    DataPropertyName = "StockQuantity",
                    Width = 80
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReorderLevel",
                    HeaderText = "Reorder Level",
                    DataPropertyName = "ReorderLevel",
                    Width = 100
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SellingPrice",
                    HeaderText = "Selling Price",
                    DataPropertyName = "SellingPrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalValue",
                    HeaderText = "Total Value",
                    DataPropertyName = "TotalValue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StockValue",
                    HeaderText = "Stock Value",
                    DataPropertyName = "StockValue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "F2" }
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StockStatus",
                    HeaderText = "Status",
                    DataPropertyName = "StockStatus",
                    Width = 100
                });
                
                dgvStockReport.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsActive",
                    HeaderText = "Active",
                    DataPropertyName = "IsActive",
                    Width = 60
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryRepository.GetAllCategories();
                cmbCategory.DataSource = new List<Category> { new Category { CategoryID = 0, CategoryName = "All Categories" } }.Concat(_categories).ToList();
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading categories: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadBrands()
        {
            try
            {
                _brands = _brandRepository.GetAllBrands();
                cmbBrand.DataSource = new List<Brand> { new Brand { BrandID = 0, BrandName = "All Brands" } }.Concat(_brands).ToList();
                cmbBrand.DisplayMember = "BrandName";
                cmbBrand.ValueMember = "BrandID";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadStockData()
        {
            try
            {
                _allProducts = _productRepository.GetAllProducts();
                GenerateStockReport();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading stock data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateStockReport()
        {
            try
            {
                _stockReportItems.Clear();
                
                foreach (var product in _allProducts)
                {
                    var stockItem = new StockReportItem
                    {
                        ProductID = product.ProductID,
                        ProductCode = product.ProductCode,
                        ProductName = product.ProductName,
                        CategoryName = product.CategoryName,
                        BrandName = product.BrandName,
                        StockQuantity = product.StockQuantity,
                        ReorderLevel = product.ReorderLevel,
                        UnitPrice = product.PurchasePrice > 0 ? product.PurchasePrice : product.CostPrice, // Cost price (what store paid)
                        SellingPrice = product.RetailPrice, // Retail price (selling price to customer)
                        TotalValue = product.StockQuantity * (product.PurchasePrice > 0 ? product.PurchasePrice : product.CostPrice), // Total value at cost
                        StockValue = product.StockQuantity * product.RetailPrice, // Stock value at selling price (quantity * selling price)
                        StockStatus = GetStockStatus(product.StockQuantity, product.ReorderLevel),
                        IsActive = product.IsActive
                    };
                    
                    _stockReportItems.Add(stockItem);
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating stock report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private string GetStockStatus(int stockQuantity, int reorderLevel)
        {
            if (stockQuantity == 0)
                return "Out of Stock";
            else if (stockQuantity <= reorderLevel)
                return "Low Stock";
            else
                return "In Stock";
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _stockReportItems.AsEnumerable();
                
                // Category filter
                if (cmbCategory.SelectedItem != null)
                {
                    var selectedItem = cmbCategory.SelectedItem;
                    if (selectedItem != null)
                    {
                        // Use reflection to get the CategoryName property from anonymous object
                        var categoryNameProperty = selectedItem.GetType().GetProperty("CategoryName");
                        if (categoryNameProperty != null)
                        {
                            var categoryName = categoryNameProperty.GetValue(selectedItem)?.ToString();
                            if (!string.IsNullOrEmpty(categoryName) && categoryName != "All Categories")
                            {
                                filteredItems = filteredItems.Where(item => item.CategoryName == categoryName);
                            }
                        }
                    }
                }
                
                // Brand filter
                if (cmbBrand.SelectedItem != null)
                {
                    var selectedItem = cmbBrand.SelectedItem;
                    if (selectedItem != null)
                    {
                        // Use reflection to get the BrandName property from anonymous object
                        var brandNameProperty = selectedItem.GetType().GetProperty("BrandName");
                        if (brandNameProperty != null)
                        {
                            var brandName = brandNameProperty.GetValue(selectedItem)?.ToString();
                            if (!string.IsNullOrEmpty(brandName) && brandName != "All Brands")
                            {
                                filteredItems = filteredItems.Where(item => item.BrandName == brandName);
                            }
                        }
                    }
                }
                
                // Stock status filters
                if (chkLowStock.Checked && !chkOutOfStock.Checked && !chkInStock.Checked)
                {
                    filteredItems = filteredItems.Where(item => item.StockStatus == "Low Stock");
                }
                else if (chkOutOfStock.Checked && !chkLowStock.Checked && !chkInStock.Checked)
                {
                    filteredItems = filteredItems.Where(item => item.StockStatus == "Out of Stock");
                }
                else if (chkInStock.Checked && !chkLowStock.Checked && !chkOutOfStock.Checked)
                {
                    filteredItems = filteredItems.Where(item => item.StockStatus == "In Stock");
                }
                else if (chkLowStock.Checked || chkOutOfStock.Checked || chkInStock.Checked)
                {
                    var selectedStatuses = new List<string>();
                    if (chkLowStock.Checked) selectedStatuses.Add("Low Stock");
                    if (chkOutOfStock.Checked) selectedStatuses.Add("Out of Stock");
                    if (chkInStock.Checked) selectedStatuses.Add("In Stock");
                    
                    filteredItems = filteredItems.Where(item => selectedStatuses.Contains(item.StockStatus));
                }
                
                // Search filter - search through multiple fields
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        (item.ProductName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.ProductCode?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.CategoryName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.BrandName?.ToLower().Contains(searchTerm) ?? false) ||
                        (item.StockQuantity.ToString().Contains(searchTerm)) ||
                        (item.ReorderLevel.ToString().Contains(searchTerm)) ||
                        (item.UnitPrice.ToString("F2").Contains(searchTerm)) ||
                        (item.SellingPrice.ToString("F2").Contains(searchTerm)) ||
                        (item.TotalValue.ToString("F2").Contains(searchTerm)) ||
                        (item.StockValue.ToString("F2").Contains(searchTerm)) ||
                        (item.StockStatus?.ToLower().Contains(searchTerm) ?? false));
                }
                
                _stockReportItems = filteredItems.ToList();
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
                // Clear the grid and manually populate rows (including totals)
                AddTotalsRow(); // This method now handles everything: clears grid, adds data rows, and adds totals row
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
                if (_stockReportItems == null || _stockReportItems.Count == 0)
                    return;

                // Calculate totals for all numeric columns
                int totalStockQty = _stockReportItems.Sum(item => item.StockQuantity);
                decimal totalUnitPrice = _stockReportItems.Sum(item => item.UnitPrice);
                decimal totalSellingPrice = _stockReportItems.Sum(item => item.SellingPrice);
                decimal totalValue = _stockReportItems.Sum(item => item.TotalValue);
                decimal totalStockValue = _stockReportItems.Sum(item => item.StockValue);

                // Remove any existing totals rows first
                for (int i = dgvStockReport.Rows.Count - 1; i >= 0; i--)
                {
                    if (dgvStockReport.Rows[i].Tag != null && dgvStockReport.Rows[i].Tag.ToString() == "TOTAL")
                    {
                        dgvStockReport.Rows.RemoveAt(i);
                    }
                }

                // Temporarily remove DataSource to allow adding unbound row
                var savedDataSource = dgvStockReport.DataSource;
                dgvStockReport.DataSource = null;
                dgvStockReport.Rows.Clear(); // Clear all existing rows

                // IMPORTANT: Don't restore DataSource - keep rows unbound to preserve totals row
                // Instead, manually populate all rows from the data source
                foreach (var item in _stockReportItems)
                {
                    int newRowIndex = dgvStockReport.Rows.Add();
                    DataGridViewRow row = dgvStockReport.Rows[newRowIndex];
                    row.Cells["ProductCode"].Value = item.ProductCode;
                    row.Cells["ProductName"].Value = item.ProductName;
                    row.Cells["CategoryName"].Value = item.CategoryName;
                    row.Cells["BrandName"].Value = item.BrandName;
                    row.Cells["StockQuantity"].Value = item.StockQuantity;
                    row.Cells["ReorderLevel"].Value = item.ReorderLevel;
                    row.Cells["UnitPrice"].Value = item.UnitPrice.ToString("F2");
                    row.Cells["SellingPrice"].Value = item.SellingPrice.ToString("F2");
                    row.Cells["TotalValue"].Value = item.TotalValue.ToString("F2");
                    row.Cells["StockValue"].Value = item.StockValue.ToString("F2");
                    row.Cells["StockStatus"].Value = item.StockStatus;
                    row.Cells["IsActive"].Value = item.IsActive;
                    row.ReadOnly = true;

                    // Apply color coding
                    if (item.StockStatus == "Out of Stock")
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    else if (item.StockStatus == "Low Stock")
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    else if (item.StockStatus == "In Stock")
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                }

                // Now add totals row at the bottom AFTER all data rows
                int totalsRowIndex = dgvStockReport.Rows.Add();
                DataGridViewRow totalsRow = dgvStockReport.Rows[totalsRowIndex];
                totalsRow.Tag = "TOTAL";
                totalsRow.DefaultCellStyle.Font = new System.Drawing.Font(dgvStockReport.DefaultCellStyle.Font, FontStyle.Bold);
                totalsRow.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                totalsRow.DefaultCellStyle.ForeColor = Color.FromArgb(41, 128, 185);
                
                // Set alignment for numeric columns
                if (totalsRow.Cells["UnitPrice"] != null)
                    totalsRow.Cells["UnitPrice"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (totalsRow.Cells["SellingPrice"] != null)
                    totalsRow.Cells["SellingPrice"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (totalsRow.Cells["TotalValue"] != null)
                    totalsRow.Cells["TotalValue"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (totalsRow.Cells["StockValue"] != null)
                    totalsRow.Cells["StockValue"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Set values for totals row
                totalsRow.Cells["ProductCode"].Value = "TOTAL";
                totalsRow.Cells["ProductName"].Value = "";
                totalsRow.Cells["CategoryName"].Value = "";
                totalsRow.Cells["BrandName"].Value = "";
                totalsRow.Cells["StockQuantity"].Value = totalStockQty;
                totalsRow.Cells["ReorderLevel"].Value = "";
                totalsRow.Cells["UnitPrice"].Value = totalUnitPrice.ToString("F2");
                totalsRow.Cells["SellingPrice"].Value = totalSellingPrice.ToString("F2");
                totalsRow.Cells["TotalValue"].Value = totalValue.ToString("F2");
                totalsRow.Cells["StockValue"].Value = totalStockValue.ToString("F2");
                totalsRow.Cells["StockStatus"].Value = "";
                totalsRow.Cells["IsActive"].Value = false;

                // Make the totals row read-only
                totalsRow.ReadOnly = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding totals row: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error adding totals row: {ex.Message}");
            }
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                var totalProducts = _stockReportItems.Count;
                var totalValue = _stockReportItems.Sum(item => item.TotalValue);
                var totalStockValue = _stockReportItems.Sum(item => item.StockValue);
                var lowStockCount = _stockReportItems.Count(item => item.StockStatus == "Low Stock");
                var outOfStockCount = _stockReportItems.Count(item => item.StockStatus == "Out of Stock");
                var inStockCount = _stockReportItems.Count(item => item.StockStatus == "In Stock");
                
                lblTotalProducts.Text = $"Total Products: {totalProducts}";
                lblTotalValue.Text = $"Total Value: {totalValue:F2}";
                lblStockValue.Text = $"Stock Value: {totalStockValue:F2}";
                lblLowStock.Text = $"Low Stock: {lowStockCount}";
                lblOutOfStock.Text = $"Out of Stock: {outOfStockCount}";
                lblInStock.Text = $"In Stock: {inStockCount}";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating summary labels: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            cmbCategory.SelectedIndex = 0;
            cmbBrand.SelectedIndex = 0;
            chkLowStock.Checked = false;
            chkOutOfStock.Checked = false;
            chkInStock.Checked = false;
            txtSearch.Clear();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateStockReport();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
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
                if (_stockReportItems == null || _stockReportItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveDialog.FileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPDF(saveDialog.FileName);
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
            GenerateStockReport();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void CmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ChkLowStock_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ChkOutOfStock_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilters();
            RefreshDataGridView();
            UpdateSummaryLabels();
        }

        private void ChkInStock_CheckedChanged(object sender, EventArgs e)
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

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
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
                Paragraph title = new Paragraph("Attock Mobiles Rwp - STOCK REPORT", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Report info
                Paragraph reportInfo = new Paragraph($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normalFont);
                reportInfo.SpacingAfter = 15f;
                document.Add(reportInfo);

                // Summary section
                var totalItems = _stockReportItems.Count;
                var inStock = _stockReportItems.Count(item => item.StockStatus == "In Stock");
                var lowStock = _stockReportItems.Count(item => item.StockStatus == "Low Stock");
                var outOfStock = _stockReportItems.Count(item => item.StockStatus == "Out of Stock");
                var totalValue = _stockReportItems.Sum(item => item.StockQuantity * item.UnitPrice);

                // Summary table
                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 1, 1 });

                summaryTable.AddCell(new PdfPCell(new Phrase("Total Items:", headerFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(totalItems.ToString(), normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase("In Stock:", headerFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(inStock.ToString(), normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase("Low Stock:", headerFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(lowStock.ToString(), normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase("Out of Stock:", headerFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase(outOfStock.ToString(), normalFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase("Total Value:", headerFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase($"{totalValue:F2}", normalFont)) { Border = 0 });
                var totalStockValue = _stockReportItems.Sum(item => item.StockValue);
                summaryTable.AddCell(new PdfPCell(new Phrase("Stock Value:", headerFont)) { Border = 0 });
                summaryTable.AddCell(new PdfPCell(new Phrase($"{totalStockValue:F2}", normalFont)) { Border = 0 });

                document.Add(summaryTable);
                document.Add(new Paragraph(" "));

                // Data table
                PdfPTable dataTable = new PdfPTable(10);
                dataTable.WidthPercentage = 100;
                dataTable.SetWidths(new float[] { 1, 1, 2, 1, 1, 1, 1, 1, 1, 1 });

                // Headers
                dataTable.AddCell(new PdfPCell(new Phrase("Code", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Product", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Category", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Brand", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Stock", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Unit Price", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Selling Price", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Total Value", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Stock Value", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("Status", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                // Data rows
                foreach (var item in _stockReportItems)
                {
                    dataTable.AddCell(new PdfPCell(new Phrase(item.ProductCode ?? "", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase(item.ProductName ?? "", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase(item.CategoryName ?? "", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase(item.BrandName ?? "", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase(item.StockQuantity.ToString(), smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase($"{item.UnitPrice:F2}", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase($"{item.SellingPrice:F2}", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase($"{(item.StockQuantity * item.UnitPrice):F2}", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase($"{item.StockValue:F2}", smallFont)));
                    dataTable.AddCell(new PdfPCell(new Phrase(item.StockStatus ?? "", smallFont)));
                }

                // Add totals row
                var totalStockQty = _stockReportItems.Sum(item => item.StockQuantity);
                var totalUnitPrice = _stockReportItems.Sum(item => item.UnitPrice);
                var totalSellingPrice = _stockReportItems.Sum(item => item.SellingPrice);
                // Reuse totalValue and totalStockValue already calculated above
                
                dataTable.AddCell(new PdfPCell(new Phrase("TOTAL", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("", smallFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("", smallFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("", smallFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase(totalStockQty.ToString(), headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase($"{totalUnitPrice:F2}", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase($"{totalSellingPrice:F2}", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase($"{totalValue:F2}", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase($"{totalStockValue:F2}", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                dataTable.AddCell(new PdfPCell(new Phrase("", smallFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                document.Add(dataTable);

                document.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating PDF: {ex.Message}");
            }
        }

        private void StockReportForm_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Product Code,Product Name,Category,Brand,Stock Quantity,Reorder Level,Unit Price,Selling Price,Total Value,Stock Value,Stock Status");
                
                // Write data
                foreach (var item in _stockReportItems)
                {
                    writer.WriteLine($"{item.ProductCode},{item.ProductName},{item.CategoryName},{item.BrandName},{item.StockQuantity},{item.ReorderLevel},{item.UnitPrice:F2},{item.SellingPrice:F2},{item.TotalValue:F2},{item.StockValue:F2},{item.StockStatus}");
                }
                
                // Add totals row
                var totalStockQty = _stockReportItems.Sum(item => item.StockQuantity);
                var totalUnitPrice = _stockReportItems.Sum(item => item.UnitPrice);
                var totalSellingPrice = _stockReportItems.Sum(item => item.SellingPrice);
                var totalValue = _stockReportItems.Sum(item => item.TotalValue);
                var totalStockValue = _stockReportItems.Sum(item => item.StockValue);
                writer.WriteLine($"TOTAL,,,,{totalStockQty},,{totalUnitPrice:F2},{totalSellingPrice:F2},{totalValue:F2},{totalStockValue:F2},");
            }
        }
    }

    // Data class for stock report items
    public class StockReportItem
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TotalValue { get; set; }
        public decimal StockValue { get; set; } // Quantity * Selling Price
        public string StockStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
