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
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalValue",
                    HeaderText = "Total Value",
                    DataPropertyName = "TotalValue",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
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
                        UnitPrice = product.UnitPrice,
                        TotalValue = product.StockQuantity * product.UnitPrice,
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
                    var selectedCategory = (Category)cmbCategory.SelectedItem;
                    if (selectedCategory != null)
                    {
                        filteredItems = filteredItems.Where(item => item.CategoryName == selectedCategory.CategoryName);
                    }
                }
                
                // Brand filter
                if (cmbBrand.SelectedItem != null)
                {
                    var selectedBrand = (Brand)cmbBrand.SelectedItem;
                    if (selectedBrand != null)
                    {
                        filteredItems = filteredItems.Where(item => item.BrandName == selectedBrand.BrandName);
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
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.ProductName.ToLower().Contains(searchTerm) ||
                        item.ProductCode.ToLower().Contains(searchTerm) ||
                        item.CategoryName.ToLower().Contains(searchTerm) ||
                        item.BrandName.ToLower().Contains(searchTerm));
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
                dgvStockReport.DataSource = null;
                dgvStockReport.DataSource = _stockReportItems;
                
                // Apply color coding for stock status
                foreach (DataGridViewRow row in dgvStockReport.Rows)
                {
                    var stockStatus = row.Cells["StockStatus"].Value?.ToString();
                    if (stockStatus == "Out of Stock")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                    else if (stockStatus == "Low Stock")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                    else if (stockStatus == "In Stock")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
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
                var totalProducts = _stockReportItems.Count;
                var totalValue = _stockReportItems.Sum(item => item.TotalValue);
                var lowStockCount = _stockReportItems.Count(item => item.StockStatus == "Low Stock");
                var outOfStockCount = _stockReportItems.Count(item => item.StockStatus == "Out of Stock");
                var inStockCount = _stockReportItems.Count(item => item.StockStatus == "In Stock");
                
                lblTotalProducts.Text = $"Total Products: {totalProducts}";
                lblTotalValue.Text = $"Total Value: ${totalValue:F2}";
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
                // TODO: Implement PDF export functionality
                ShowMessage("PDF export functionality will be implemented with iTextSharp package.", "Info", MessageBoxIcon.Information);
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
                writer.WriteLine("Product Code,Product Name,Category,Brand,Stock Quantity,Reorder Level,Unit Price,Total Value,Stock Status");
                
                // Write data
                foreach (var item in _stockReportItems)
                {
                    writer.WriteLine($"{item.ProductCode},{item.ProductName},{item.CategoryName},{item.BrandName},{item.StockQuantity},{item.ReorderLevel},{item.UnitPrice:F2},{item.TotalValue:F2},{item.StockStatus}");
                }
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
        public decimal TotalValue { get; set; }
        public string StockStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
