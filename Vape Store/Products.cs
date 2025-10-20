using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing.Printing;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Models;
using ZXing;
using ZXing.Common;

namespace Vape_Store
{
    public partial class Products : Form
    {
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private InventoryService _inventoryService;
        private List<Product> _products;
        private List<Category> _categories;
        private List<Brand> _brands;
        private bool isEditMode = false;
        private int selectedProductId = -1;

        public Products()
        {
            InitializeComponent();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadCategories();
            LoadBrands();
            GenerateProductCode();
            LoadProducts();
        }

        private void InitializeDataGridView()
        {
            // Configure DataGridView columns
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.ReadOnly = true;
            
            // Clear existing columns
            dgvProducts.Columns.Clear();
            
            // Add columns
            dgvProducts.Columns.Add("ProductID", "ID");
            dgvProducts.Columns.Add("ProductCode", "Product Code");
            dgvProducts.Columns.Add("ProductName", "Product Name");
            dgvProducts.Columns.Add("CategoryName", "Category");
            dgvProducts.Columns.Add("BrandName", "Brand");
            dgvProducts.Columns.Add("CostPrice", "Cost Price");
            dgvProducts.Columns.Add("RetailPrice", "Retail Price");
            dgvProducts.Columns.Add("StockQuantity", "Stock");
            dgvProducts.Columns.Add("ReorderLevel", "Reorder Level");
            dgvProducts.Columns.Add("Barcode", "Barcode");
            dgvProducts.Columns.Add("IsActive", "Status");
            
            // Configure column properties
            dgvProducts.Columns["ProductID"].Width = 50;
            dgvProducts.Columns["ProductCode"].Width = 100;
            dgvProducts.Columns["ProductName"].Width = 200;
            dgvProducts.Columns["CategoryName"].Width = 120;
            dgvProducts.Columns["BrandName"].Width = 120;
            dgvProducts.Columns["CostPrice"].Width = 100;
            dgvProducts.Columns["RetailPrice"].Width = 100;
            dgvProducts.Columns["StockQuantity"].Width = 80;
            dgvProducts.Columns["ReorderLevel"].Width = 100;
            dgvProducts.Columns["Barcode"].Width = 120;
            dgvProducts.Columns["IsActive"].Width = 80;
            
            // Format currency columns
            dgvProducts.Columns["CostPrice"].DefaultCellStyle.Format = "C2";
            dgvProducts.Columns["RetailPrice"].DefaultCellStyle.Format = "C2";
            
            // Format status column
            dgvProducts.Columns["IsActive"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            ADD_button.Click += AddButton_Click;
            Save_button.Click += SaveButton_Click;
            Del_button.Click += DeleteButton_Click;
            Clear_button.Click += ClearButton_Click;
            Exit_button.Click += ExitButton_Click;
            Print_button.Click += PrintButton_Click;
            generateBtn.Click += GenerateBarcode_Click;
            
            // DataGridView event handlers
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryRepository.GetAllCategories();
                cmbCategory.DataSource = _categories;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";
                cmbCategory.SelectedIndex = -1; // No default selection
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
                cmbBrand.DataSource = _brands;
                cmbBrand.DisplayMember = "BrandName";
                cmbBrand.ValueMember = "BrandID";
                cmbBrand.SelectedIndex = -1; // No default selection
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvProducts.Rows.Clear();
                
                foreach (var product in _products)
                {
                    dgvProducts.Rows.Add(
                        product.ProductID,
                        product.ProductCode,
                        product.ProductName,
                        _categories.FirstOrDefault(c => c.CategoryID == product.CategoryID)?.CategoryName ?? "",
                        _brands.FirstOrDefault(b => b.BrandID == product.BrandID)?.BrandName ?? "",
                        product.CostPrice,
                        product.RetailPrice,
                        product.StockQuantity,
                        product.ReorderLevel,
                        product.Barcode,
                        product.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateProductCode()
        {
            try
            {
                string productCode = _productRepository.GetNextProductCode();
                txtProductCode.Text = productCode;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating product code: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ClearForm();
            SetEditMode(false);
            txtProductName.Focus();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveProduct();
        }

        private void SaveProduct()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    ShowMessage("Please enter a product name.", "Validation Error", MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }

                if (cmbCategory.SelectedValue == null)
                {
                    ShowMessage("Please select a category.", "Validation Error", MessageBoxIcon.Warning);
                    cmbCategory.Focus();
                    return;
                }

                if (cmbBrand.SelectedValue == null)
                {
                    ShowMessage("Please select a brand.", "Validation Error", MessageBoxIcon.Warning);
                    cmbBrand.Focus();
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal costPrice) || costPrice < 0)
                {
                    ShowMessage("Please enter a valid cost price.", "Validation Error", MessageBoxIcon.Warning);
                    txtPrice.Focus();
                    return;
                }

                if (!decimal.TryParse(txtretailprice.Text, out decimal retailPrice) || retailPrice < 0)
                {
                    ShowMessage("Please enter a valid retail price.", "Validation Error", MessageBoxIcon.Warning);
                    txtretailprice.Focus();
                    return;
                }

                if (!int.TryParse(txtReorderLevel.Text, out int reorderLevel) || reorderLevel < 0)
                {
                    ShowMessage("Please enter a valid reorder level.", "Validation Error", MessageBoxIcon.Warning);
                    txtReorderLevel.Focus();
                    return;
                }

                // Check for duplicate product name
                var existingProduct = _products.FirstOrDefault(p => 
                    p.ProductName.ToLower() == txtProductName.Text.ToLower() && 
                    p.ProductID != selectedProductId);

                if (existingProduct != null)
                {
                    ShowMessage("A product with this name already exists.", "Duplicate Error", MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }

                if (isEditMode)
                {
                    // Update existing product
                    var product = new Product
                    {
                        ProductID = selectedProductId,
                        ProductCode = txtProductCode.Text.Trim(),
                        ProductName = txtProductName.Text.Trim(),
                        Description = txtDescription.Text.Trim(),
                        CategoryID = ((Category)cmbCategory.SelectedItem).CategoryID,
                        BrandID = ((Brand)cmbBrand.SelectedItem).BrandID,
                        CostPrice = costPrice,
                        RetailPrice = retailPrice,
                        StockQuantity = _products.First(p => p.ProductID == selectedProductId).StockQuantity,
                        ReorderLevel = reorderLevel,
                        Barcode = txtBarcode.Text.Trim(),
                        IsActive = checkBox1.Checked,
                        CreatedDate = _products.First(p => p.ProductID == selectedProductId).CreatedDate
                    };

                    bool success = _productRepository.UpdateProduct(product);
                    
                    if (success)
                    {
                        ShowMessage("Product updated successfully!", "Success", MessageBoxIcon.Information);
                        LoadProducts();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to update product.", "Error", MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Add new product
                    var product = new Product
                    {
                        ProductCode = txtProductCode.Text.Trim(),
                        ProductName = txtProductName.Text.Trim(),
                        Description = txtDescription.Text.Trim(),
                        CategoryID = ((Category)cmbCategory.SelectedItem).CategoryID,
                        BrandID = ((Brand)cmbBrand.SelectedItem).BrandID,
                        CostPrice = costPrice,
                        RetailPrice = retailPrice,
                        StockQuantity = 0, // New products start with 0 stock
                        ReorderLevel = reorderLevel,
                        Barcode = txtBarcode.Text.Trim(),
                        IsActive = checkBox1.Checked,
                        CreatedDate = DateTime.Now
                    };

                    bool success = _productRepository.AddProduct(product);
                    
                    if (success)
                    {
                        ShowMessage("Product added successfully!", "Success", MessageBoxIcon.Information);
                        LoadProducts();
                        ClearForm();
                        GenerateProductCode();
                    }
                    else
                    {
                        ShowMessage("Failed to add product.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving product: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteProduct();
        }

        private void DeleteProduct()
        {
            try
            {
                if (selectedProductId == -1)
                {
                    ShowMessage("Please select a product to delete.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var product = _products.FirstOrDefault(p => p.ProductID == selectedProductId);
                if (product == null)
                {
                    ShowMessage("Product not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Check if product has stock
                if (product.StockQuantity > 0)
                {
                    ShowMessage($"Cannot delete product '{product.ProductName}' because it has {product.StockQuantity} units in stock.", "Stock Error", MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the product '{product.ProductName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _productRepository.DeleteProduct(selectedProductId);
                    
                    if (success)
                    {
                        ShowMessage("Product deleted successfully!", "Success", MessageBoxIcon.Information);
                        LoadProducts();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to delete product.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting product: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtProductCode.Clear();
            txtProductName.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbBrand.SelectedIndex = -1;
            txtPrice.Clear();
            txtretailprice.Clear();
            txtReorderLevel.Clear();
            txtBarcode.Clear();
            checkBox1.Checked = true;
            selectedProductId = -1;
            SetEditMode(false);
            GenerateProductCode();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            PrintProductList();
        }

        private void PrintProductList()
        {
            try
            {
                // TODO: Implement product list printing
                ShowMessage("Product list printing functionality will be implemented.", "Info", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing product list: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateBarcode_Click(object sender, EventArgs e)
        {
            GenerateBarcode();
        }

        private void GenerateBarcode()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    ShowMessage("Please generate a product code first.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Generate barcode based on product code
                string barcode = GenerateBarcodeFromCode(txtProductCode.Text);
                txtBarcode.Text = barcode;
                
                ShowMessage("Barcode generated successfully!", "Success", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating barcode: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private string GenerateBarcodeFromCode(string productCode)
        {
            // Generate EAN-13 barcode using ZXing
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.EAN_13,
                    Options = new EncodingOptions
                    {
                        Height = 100,
                        Width = 300,
                        Margin = 2
                    }
                };
                
                // Create a 13-digit barcode from product code
                string barcodeData = productCode.PadLeft(12, '0');
                if (barcodeData.Length > 12)
                    barcodeData = barcodeData.Substring(0, 12);
                
                // Add check digit for EAN-13
                barcodeData += CalculateEAN13CheckDigit(barcodeData);
                
                return barcodeData;
            }
            catch (Exception ex)
            {
                // Fallback to simple barcode if ZXing fails
                return $"BAR{productCode.PadLeft(10, '0')}";
            }
        }
        
        private string CalculateEAN13CheckDigit(string barcode)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(barcode[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }
            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit.ToString();
        }

        private void DgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvProducts.Rows.Count)
            {
                EditSelectedProduct();
            }
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                int rowIndex = dgvProducts.SelectedRows[0].Index;
                if (rowIndex >= 0 && rowIndex < _products.Count)
                {
                    selectedProductId = _products[rowIndex].ProductID;
                }
            }
        }

        private void EditSelectedProduct()
        {
            try
            {
                if (selectedProductId == -1)
                {
                    ShowMessage("Please select a product to edit.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var product = _products.FirstOrDefault(p => p.ProductID == selectedProductId);
                if (product == null)
                {
                    ShowMessage("Product not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Populate form with product data
                txtProductCode.Text = product.ProductCode;
                txtProductName.Text = product.ProductName;
                txtDescription.Text = product.Description;
                cmbCategory.SelectedValue = product.CategoryID;
                cmbBrand.SelectedValue = product.BrandID;
                txtPrice.Text = product.CostPrice.ToString("F2");
                txtretailprice.Text = product.RetailPrice.ToString("F2");
                txtReorderLevel.Text = product.ReorderLevel.ToString();
                txtBarcode.Text = product.Barcode;
                checkBox1.Checked = product.IsActive;
                
                SetEditMode(true);
                txtProductName.Focus();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error editing product: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private void FilterProducts()
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    RefreshDataGridView();
                    return;
                }

                var filteredProducts = _products.Where(p => 
                    p.ProductName.ToLower().Contains(searchText) ||
                    p.ProductCode.ToLower().Contains(searchText) ||
                    p.Barcode.ToLower().Contains(searchText) ||
                    (_categories.FirstOrDefault(c => c.CategoryID == p.CategoryID)?.CategoryName ?? "").ToLower().Contains(searchText) ||
                    (_brands.FirstOrDefault(b => b.BrandID == p.BrandID)?.BrandName ?? "").ToLower().Contains(searchText)).ToList();

                dgvProducts.Rows.Clear();
                
                foreach (var product in filteredProducts)
                {
                    dgvProducts.Rows.Add(
                        product.ProductID,
                        product.ProductCode,
                        product.ProductName,
                        _categories.FirstOrDefault(c => c.CategoryID == product.CategoryID)?.CategoryName ?? "",
                        _brands.FirstOrDefault(b => b.BrandID == product.BrandID)?.BrandName ?? "",
                        product.CostPrice,
                        product.RetailPrice,
                        product.StockQuantity,
                        product.ReorderLevel,
                        product.Barcode,
                        product.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;
            
            if (editMode)
            {
                ADD_button.Text = "New Product";
                Save_button.Text = "Update";
                Del_button.Enabled = true;
            }
            else
            {
                ADD_button.Text = "Add Product";
                Save_button.Text = "Save";
                Del_button.Enabled = false;
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void Products_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetEditMode(false);
            txtSearch.Focus();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Handle checkbox state change
            // Implementation depends on what this checkbox controls
        }

        private void productGroup_Enter(object sender, EventArgs e)
        {
            // Handle product group enter event
            // Implementation depends on the specific functionality needed
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Handle text box change event
            // Implementation depends on what this text box controls
        }
    }
}
