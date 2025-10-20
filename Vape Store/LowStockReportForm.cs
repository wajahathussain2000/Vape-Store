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
    public partial class LowStockReportForm : Form
    {
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private InventoryService _inventoryService;
        
        private List<Product> _allProducts;
        private List<Category> _categories;
        private List<Brand> _brands;
        private List<LowStockReportItem> _lowStockItems;

        public LowStockReportForm()
        {
            InitializeComponent();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            
            _lowStockItems = new List<LowStockReportItem>();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadCategories();
            LoadBrands();
            LoadLowStockData();
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
            btnCreatePurchaseOrder.Click += BtnCreatePurchaseOrder_Click;
            
            // Filter event handlers
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            cmbBrand.SelectedIndexChanged += CmbBrand_SelectedIndexChanged;
            chkLowStock.CheckedChanged += ChkLowStock_CheckedChanged;
            chkOutOfStock.CheckedChanged += ChkOutOfStock_CheckedChanged;
            chkCriticalStock.CheckedChanged += ChkCriticalStock_CheckedChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += LowStockReportForm_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvLowStockReport.AutoGenerateColumns = false;
                dgvLowStockReport.AllowUserToAddRows = false;
                dgvLowStockReport.AllowUserToDeleteRows = false;
                dgvLowStockReport.ReadOnly = true;
                dgvLowStockReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvLowStockReport.MultiSelect = true;

                // Define columns
                dgvLowStockReport.Columns.Clear();
                
                dgvLowStockReport.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "Select",
                    HeaderText = "Select",
                    Width = 50
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductCode",
                    HeaderText = "Code",
                    DataPropertyName = "ProductCode",
                    Width = 80
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product Name",
                    DataPropertyName = "ProductName",
                    Width = 200
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CategoryName",
                    HeaderText = "Category",
                    DataPropertyName = "CategoryName",
                    Width = 120
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "BrandName",
                    HeaderText = "Brand",
                    DataPropertyName = "BrandName",
                    Width = 120
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CurrentStock",
                    HeaderText = "Current Stock",
                    DataPropertyName = "CurrentStock",
                    Width = 100
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReorderLevel",
                    HeaderText = "Reorder Level",
                    DataPropertyName = "ReorderLevel",
                    Width = 100
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "RecommendedOrder",
                    HeaderText = "Recommended Order",
                    DataPropertyName = "RecommendedOrder",
                    Width = 130
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "EstimatedCost",
                    HeaderText = "Estimated Cost",
                    DataPropertyName = "EstimatedCost",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StockStatus",
                    HeaderText = "Status",
                    DataPropertyName = "StockStatus",
                    Width = 100
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DaysToStockOut",
                    HeaderText = "Days to Stock Out",
                    DataPropertyName = "DaysToStockOut",
                    Width = 120
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LastPurchaseDate",
                    HeaderText = "Last Purchase",
                    DataPropertyName = "LastPurchaseDate",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
                });
                
                dgvLowStockReport.Columns.Add(new DataGridViewCheckBoxColumn
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
                cmbCategory.DataSource = null;
                cmbCategory.Items.Clear();
                
                // Add "All Categories" option
                cmbCategory.Items.Add(new { CategoryID = 0, CategoryName = "All Categories" });
                
                // Add categories
                foreach (var category in _categories)
                {
                    cmbCategory.Items.Add(new { CategoryID = category.CategoryID, CategoryName = category.CategoryName });
                }
                
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
                cmbCategory.SelectedIndex = 0;
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
                cmbBrand.DataSource = null;
                cmbBrand.Items.Clear();
                
                // Add "All Brands" option
                cmbBrand.Items.Add(new { BrandID = 0, BrandName = "All Brands" });
                
                // Add brands
                foreach (var brand in _brands)
                {
                    cmbBrand.Items.Add(new { BrandID = brand.BrandID, BrandName = brand.BrandName });
                }
                
                cmbBrand.DisplayMember = "BrandName";
                cmbBrand.ValueMember = "BrandID";
                cmbBrand.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadLowStockData()
        {
            try
            {
                _allProducts = _productRepository.GetAllProducts();
                GenerateLowStockReport();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading low stock data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            cmbCategory.SelectedIndex = 0;
            cmbBrand.SelectedIndex = 0;
            chkLowStock.Checked = true;
            chkOutOfStock.Checked = true;
            chkCriticalStock.Checked = true;
            txtSearch.Clear();
        }

        private void GenerateLowStockReport()
        {
            try
            {
                _lowStockItems.Clear();
                
                foreach (var product in _allProducts)
                {
                    var stockStatus = GetStockStatus(product.StockQuantity, product.ReorderLevel);
                    var isLowStock = stockStatus == "Low Stock" || stockStatus == "Out of Stock" || stockStatus == "Critical Stock";
                    
                    if (isLowStock)
                    {
                        var recommendedOrder = CalculateRecommendedOrder(product.StockQuantity, product.ReorderLevel);
                        var estimatedCost = recommendedOrder * product.UnitPrice;
                        var daysToStockOut = CalculateDaysToStockOut(product.StockQuantity, product.ReorderLevel);
                        
                        var lowStockItem = new LowStockReportItem
                        {
                            ProductID = product.ProductID,
                            ProductCode = product.ProductCode,
                            ProductName = product.ProductName,
                            CategoryName = product.CategoryName,
                            BrandName = product.BrandName,
                            CurrentStock = product.StockQuantity,
                            ReorderLevel = product.ReorderLevel,
                            RecommendedOrder = recommendedOrder,
                            UnitPrice = product.UnitPrice,
                            EstimatedCost = estimatedCost,
                            StockStatus = stockStatus,
                            DaysToStockOut = daysToStockOut,
                            LastPurchaseDate = product.LastPurchaseDate ?? DateTime.MinValue,
                            IsActive = product.IsActive,
                            Select = false
                        };
                        
                        _lowStockItems.Add(lowStockItem);
                    }
                }
                
                ApplyFilters();
                RefreshDataGridView();
                UpdateSummaryLabels();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating low stock report: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private string GetStockStatus(int stockQuantity, int reorderLevel)
        {
            if (stockQuantity == 0)
                return "Out of Stock";
            else if (stockQuantity <= reorderLevel * 0.5) // Critical: less than 50% of reorder level
                return "Critical Stock";
            else if (stockQuantity <= reorderLevel)
                return "Low Stock";
            else
                return "In Stock";
        }

        private int CalculateRecommendedOrder(int currentStock, int reorderLevel)
        {
            // Recommend ordering 2-3 times the reorder level to avoid frequent reordering
            var recommendedOrder = Math.Max(reorderLevel * 2, 10); // Minimum 10 units
            return recommendedOrder;
        }

        private string CalculateDaysToStockOut(int currentStock, int reorderLevel)
        {
            if (currentStock == 0)
                return "0 days";
            
            // Assume average daily consumption based on reorder level
            // This is a simplified calculation - in real scenario, you'd use historical sales data
            var dailyConsumption = Math.Max(1, reorderLevel / 30); // Assume reorder level covers 30 days
            var daysToStockOut = currentStock / dailyConsumption;
            
            if (daysToStockOut <= 7)
                return $"{daysToStockOut} days (Critical)";
            else if (daysToStockOut <= 14)
                return $"{daysToStockOut} days (Urgent)";
            else
                return $"{daysToStockOut} days";
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredItems = _lowStockItems.AsEnumerable();
                
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
                var statusFilters = new List<string>();
                if (chkLowStock.Checked) statusFilters.Add("Low Stock");
                if (chkOutOfStock.Checked) statusFilters.Add("Out of Stock");
                if (chkCriticalStock.Checked) statusFilters.Add("Critical Stock");
                
                if (statusFilters.Count > 0)
                {
                    filteredItems = filteredItems.Where(item => statusFilters.Contains(item.StockStatus));
                }
                
                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredItems = filteredItems.Where(item => 
                        item.ProductCode.ToLower().Contains(searchTerm) ||
                        item.ProductName.ToLower().Contains(searchTerm) ||
                        item.CategoryName.ToLower().Contains(searchTerm) ||
                        item.BrandName.ToLower().Contains(searchTerm) ||
                        item.StockStatus.ToLower().Contains(searchTerm)
                    );
                }
                
                // Sort by priority: Critical Stock first, then Out of Stock, then Low Stock
                filteredItems = filteredItems.OrderBy(item => 
                    item.StockStatus == "Critical Stock" ? 1 :
                    item.StockStatus == "Out of Stock" ? 2 :
                    item.StockStatus == "Low Stock" ? 3 : 4
                ).ThenBy(item => item.DaysToStockOut);
                
                _lowStockItems = filteredItems.ToList();
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
                dgvLowStockReport.DataSource = null;
                dgvLowStockReport.DataSource = _lowStockItems;
                
                // Apply color coding for stock status
                foreach (DataGridViewRow row in dgvLowStockReport.Rows)
                {
                    if (row.DataBoundItem is LowStockReportItem item)
                    {
                        switch (item.StockStatus)
                        {
                            case "Critical Stock":
                                row.DefaultCellStyle.BackColor = Color.LightCoral;
                                break;
                            case "Out of Stock":
                                row.DefaultCellStyle.BackColor = Color.LightPink;
                                break;
                            case "Low Stock":
                                row.DefaultCellStyle.BackColor = Color.LightYellow;
                                break;
                        }
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
                var totalItems = _lowStockItems.Count;
                var criticalCount = _lowStockItems.Count(item => item.StockStatus == "Critical Stock");
                var outOfStockCount = _lowStockItems.Count(item => item.StockStatus == "Out of Stock");
                var lowStockCount = _lowStockItems.Count(item => item.StockStatus == "Low Stock");
                var totalEstimatedCost = _lowStockItems.Sum(item => item.EstimatedCost);
                var selectedItemsCount = _lowStockItems.Count(item => item.Select);
                var selectedItemsCost = _lowStockItems.Where(item => item.Select).Sum(item => item.EstimatedCost);
                
                lblTotalItems.Text = $"Total Items: {totalItems}";
                lblCriticalStock.Text = $"Critical Stock: {criticalCount}";
                lblOutOfStock.Text = $"Out of Stock: {outOfStockCount}";
                lblLowStock.Text = $"Low Stock: {lowStockCount}";
                lblTotalEstimatedCost.Text = $"Total Estimated Cost: Rs {totalEstimatedCost:F2}";
                lblSelectedItems.Text = $"Selected Items: {selectedItemsCount}";
                lblSelectedCost.Text = $"Selected Cost: Rs {selectedItemsCost:F2}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating summary labels: {ex.Message}");
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        // Event Handlers
        private void LowStockReportForm_Load(object sender, EventArgs e)
        {
            SetInitialState();
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateLowStockReport();
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lowStockItems == null || _lowStockItems.Count == 0)
                {
                    ShowMessage("No data to export. Please generate a report first.", "No Data", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement Excel export functionality
                ShowMessage("Excel export functionality will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
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
                if (_lowStockItems == null || _lowStockItems.Count == 0)
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
                if (_lowStockItems == null || _lowStockItems.Count == 0)
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

        private void BtnCreatePurchaseOrder_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedItems = _lowStockItems.Where(item => item.Select).ToList();
                
                if (selectedItems.Count == 0)
                {
                    ShowMessage("Please select items to create a purchase order.", "No Items Selected", MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement purchase order creation functionality
                ShowMessage($"Purchase order creation for {selectedItems.Count} items will be implemented in the next version.", "Feature Coming Soon", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error creating purchase order: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            SetInitialState();
            _lowStockItems.Clear();
            dgvLowStockReport.DataSource = null;
            UpdateSummaryLabels();
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

        private void ChkCriticalStock_CheckedChanged(object sender, EventArgs e)
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
    }

    // Data class for low stock report items
    public class LowStockReportItem
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public int RecommendedOrder { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal EstimatedCost { get; set; }
        public string StockStatus { get; set; }
        public string DaysToStockOut { get; set; }
        public DateTime LastPurchaseDate { get; set; }
        public bool IsActive { get; set; }
        public bool Select { get; set; }
    }
}