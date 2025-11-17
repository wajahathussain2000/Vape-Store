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
using Vape_Store.Helpers;
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
        private BarcodeService _barcodeService;
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
            _barcodeService = new BarcodeService();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadCategories();
            LoadBrands();
            GenerateProductCode();
            LoadProducts();

            // Configure buttons: ADD_button acts as dedicated Save; Save_button is Update
            ADD_button.Text = "Save";
            Save_button.Text = "Update";
            
            // Show barcode section - make it visible and enabled
            if (barcodeGroup != null)
            {
                barcodeGroup.Visible = true;
                barcodeGroup.Enabled = true;
            }
            
            // Ensure barcode panel is visible
            if (pnlBarcode != null)
            {
                pnlBarcode.Visible = true;
                pnlBarcode.Enabled = true;
            }
            
            // Ensure barcode textbox is editable
            if (txtBarcode != null)
            {
                txtBarcode.ReadOnly = false;
                txtBarcode.Enabled = true;
            }
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
            dgvProducts.Columns["CostPrice"].DefaultCellStyle.Format = "F2";
            dgvProducts.Columns["RetailPrice"].DefaultCellStyle.Format = "F2";
            
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
            
            // DataGridView event handlers
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Barcode textbox event handler
            if (txtBarcode != null)
            {
                txtBarcode.TextChanged += TxtBarcode_TextChanged;
            }
            
            // Generate barcode button event handler
            if (generateBtn != null)
            {
                generateBtn.Click += GenerateBarcode_Click;
            }
            
            // Form closing handler to ensure cleanup
            this.FormClosing += Products_FormClosing;
        }
        
        private void Products_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Allow form to close even if operations are pending
            // Cancel close only if user explicitly cancels
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false;
            }
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryRepository.GetAllCategories();
                SearchableComboBoxHelper.MakeSearchable(cmbCategory, _categories, "CategoryName", "CategoryID", "CategoryName");
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
                SearchableComboBoxHelper.MakeSearchable(cmbBrand, _brands, "BrandName", "BrandID", "BrandName");
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
                Application.DoEvents(); // Keep UI responsive
                _products = _productRepository.GetAllProducts();
                Application.DoEvents();
                
                // Check if products were loaded
                if (_products == null)
                {
                    _products = new List<Product>();
                }
                
                RefreshDataGridView();
                Application.DoEvents();
                UpdateSearchAutoComplete();
                
                // Optional: Show info if no products found (only on initial load, not on every refresh)
                // Uncomment the line below if you want to be notified when there are no products
                // if (_products.Count == 0)
                // {
                //     System.Diagnostics.Debug.WriteLine("No active products found in database.");
                // }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
                // Ensure _products is initialized even on error
                if (_products == null)
                {
                    _products = new List<Product>();
                }
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                // Check if form and controls are still valid
                if (this.IsDisposed)
                    return;
                    
                if (dgvProducts == null || dgvProducts.IsDisposed)
                    return;

                // Temporarily disable SelectionChanged to prevent errors during clear
                dgvProducts.SelectionChanged -= DgvProducts_SelectionChanged;
                try
                {
                    dgvProducts.Rows.Clear();
                    selectedProductId = -1; // Reset selection
                    
                    if (_products == null || _products.Count == 0)
                        return;
                    
                    foreach (var product in _products)
                    {
                        // Check again in loop in case control gets disposed
                        if (dgvProducts.IsDisposed)
                            break;
                            
                        string categoryName = "";
                        if (_categories != null && _categories.Count > 0)
                        {
                            var cat = _categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
                            categoryName = cat?.CategoryName ?? "";
                        }
                        
                        string brandName = "";
                        if (_brands != null && _brands.Count > 0)
                        {
                            var brand = _brands.FirstOrDefault(b => b.BrandID == product.BrandID);
                            brandName = brand?.BrandName ?? "";
                        }
                        
                        dgvProducts.Rows.Add(
                            product.ProductID,
                            product.ProductCode,
                            product.ProductName,
                            categoryName,
                            brandName,
                            product.CostPrice,
                            product.RetailPrice,
                            product.StockQuantity,
                            product.ReorderLevel,
                            product.Barcode,
                            product.IsActive ? "Active" : "Inactive"
                        );
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Control was disposed during operation, silently exit
                    return;
                }
                catch (AccessViolationException)
                {
                    // Memory access violation, silently exit
                    return;
                }
                finally
                {
                    // Re-enable SelectionChanged after refresh (only if control is still valid)
                    if (!dgvProducts.IsDisposed)
                    {
                        try
                        {
                            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
                        }
                        catch
                        {
                            // Silently handle if event handler can't be reattached
                        }
                    }
                }
                
                UpdateSearchAutoComplete();
            }
            catch (ObjectDisposedException)
            {
                // Form or control was disposed, silently exit
                return;
            }
            catch (AccessViolationException)
            {
                // Memory access violation, silently exit
                return;
            }
            catch (Exception ex)
            {
                // Only show message for unexpected errors, not disposal/access violations
                if (!this.IsDisposed)
                {
                    try
                    {
                        ShowMessage($"Error refreshing data: {ex.Message}", "Error", MessageBoxIcon.Error);
                    }
                    catch
                    {
                        // Silently handle if we can't show message
                    }
                }
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

        private string GenerateUniqueBarcode()
        {
            try
            {
                // Generate a unique barcode using product code + timestamp
                string productCode = txtProductCode.Text.Trim();
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string uniqueBarcode = $"PRD{productCode}{timestamp.Substring(timestamp.Length - 6)}";
                
                // Ensure uniqueness by checking against existing barcodes (safe check for null/empty)
                if (_products != null && _products.Count > 0)
                {
                    int attempts = 0;
                    while (_products.Any(p => p.Barcode == uniqueBarcode) && attempts < 10)
                    {
                        uniqueBarcode = $"PRD{productCode}{DateTime.Now.Ticks.ToString().Substring(Math.Max(0, DateTime.Now.Ticks.ToString().Length - 6))}";
                        attempts++;
                    }
                }
                
                return uniqueBarcode;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating barcode: {ex.Message}", "Error", MessageBoxIcon.Error);
                return $"PRD{DateTime.Now.Ticks}";
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // Dedicated Save: always add a new product
            SaveNewProduct();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Dedicated Update
            UpdateExistingProduct();
        }

        private void SaveNewProduct()
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

                // Auto-generate barcode (UI section removed, always auto-generate)
                string barcodeText = GenerateUniqueBarcode();

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
                    Barcode = barcodeText,
                    IsActive = checkBox1.Checked,
                    CreatedDate = DateTime.Now
                };

                // Disable buttons to prevent double-click
                ADD_button.Enabled = false;
                ADD_button.Text = "Saving...";
                try
                {
                    Application.DoEvents(); // Keep UI responsive
                    bool success = _productRepository.AddProduct(product);
                    
                    if (success)
                    {
                        Application.DoEvents();
                        // Refresh data first
                        LoadProducts();
                        Application.DoEvents();
                        ClearForm();
                        GenerateProductCode();
                        Application.DoEvents();
                        
                        ShowMessage("Product added successfully!", "Success", MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShowMessage("Failed to add product.", "Error", MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    ADD_button.Enabled = true;
                    ADD_button.Text = "Save";
                }
            }
            catch (Exception ex)
            {
                ADD_button.Enabled = true;
                ADD_button.Text = "Save";
                ShowMessage($"Error saving product: {ex.Message}\n\nPlease check:\n1. Database connection\n2. Required fields are filled\n3. Product code is unique", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateExistingProduct()
        {
            try
            {
                if (selectedProductId == -1)
                {
                    ShowMessage("Please select a product to update.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

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

                // Get existing product
                var existing = _products.FirstOrDefault(p => p.ProductID == selectedProductId);
                if (existing == null)
                {
                    ShowMessage("Product not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Determine barcode value: use txtBarcode if not empty, otherwise keep existing or generate new
                string barcodeValue;
                if (!string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    // User has entered/edited barcode - use it
                    barcodeValue = txtBarcode.Text.Trim();
                    
                    // Validate the barcode
                    if (!_barcodeService.ValidateBarcode(barcodeValue))
                    {
                        ShowMessage("Invalid barcode format! Only letters, numbers, hyphens, underscores, and dots are allowed.", "Validation Error", MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                    
                    // Check for duplicates (excluding current product)
                    bool isDuplicate = _productRepository.IsBarcodeExists(barcodeValue, selectedProductId);
                    if (isDuplicate)
                    {
                        ShowMessage($"Barcode '{barcodeValue}' already exists! Please use a different barcode.", "Duplicate Barcode", MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        txtBarcode.SelectAll();
                        return;
                    }
                }
                else
                {
                    // No barcode entered - keep existing or generate new if none exists
                    barcodeValue = existing.Barcode ?? GenerateUniqueBarcode();
                }

                var productToUpdate = new Product
                {
                    ProductID = selectedProductId,
                    ProductCode = txtProductCode.Text.Trim(),
                    ProductName = txtProductName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    CategoryID = ((Category)cmbCategory.SelectedItem).CategoryID,
                    BrandID = ((Brand)cmbBrand.SelectedItem).BrandID,
                    CostPrice = costPrice,
                    RetailPrice = retailPrice,
                    StockQuantity = existing.StockQuantity,
                    ReorderLevel = reorderLevel,
                    Barcode = barcodeValue, // Use the determined barcode value
                    IsActive = checkBox1.Checked,
                    CreatedDate = existing.CreatedDate
                };

                bool updated = _productRepository.UpdateProduct(productToUpdate);
                if (updated)
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
            // Clear barcode display panel
            if (pnlBarcode != null)
            {
                pnlBarcode.Controls.Clear();
            }
            checkBox1.Checked = true;
            selectedProductId = -1;
            SetEditMode(false);
            GenerateProductCode();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Cancel any pending operations
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch
            {
                // Force close if normal close fails
                try { Application.ExitThread(); } catch { }
            }
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
                // Check if user has entered custom barcode text
                if (!string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    // User wants to use custom barcode
                    string customBarcode = txtBarcode.Text.Trim();
                    
                    // Validate custom barcode
                    if (!_barcodeService.ValidateBarcode(customBarcode))
                    {
                        ShowMessage("Invalid barcode format! Only letters, numbers, hyphens, underscores, and dots are allowed.", "Validation Error", MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        return;
                    }
                    
                    // Check for duplicates
                    bool isDuplicate = _productRepository.IsBarcodeExists(customBarcode, isEditMode ? selectedProductId : (int?)null);
                    if (isDuplicate)
                    {
                        ShowMessage($"Barcode '{customBarcode}' already exists! Please use a different barcode.", "Duplicate Barcode", MessageBoxIcon.Warning);
                        txtBarcode.Focus();
                        txtBarcode.SelectAll();
                        return;
                    }
                    
                    // Test barcode generation
                    if (!_barcodeService.TestBarcodeGeneration(customBarcode))
                    {
                        ShowMessage("Invalid barcode format! Cannot generate barcode with this text.", "Barcode Error", MessageBoxIcon.Error);
                        txtBarcode.Focus();
                        return;
                    }
                    
                    // Display the custom barcode
                    DisplayBarcodeImage(customBarcode);
                    ShowMessage($"Custom barcode '{customBarcode}' generated successfully!", "Success", MessageBoxIcon.Information);
                }
                else
                {
                    // Generate automatic barcode
                    // If product code is empty, generate it first
                    if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                    {
                        GenerateProductCode();
                    }

                    // Ensure we have a product code now
                    if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                    {
                        ShowMessage("Unable to generate product code. Please enter a product code manually.", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }

                    // Generate barcode text
                    string barcodeText = GenerateBarcodeFromCode(txtProductCode.Text);
                    txtBarcode.Text = barcodeText;
                    
                    // Generate and display barcode image
                    DisplayBarcodeImage(barcodeText);
                    
                    ShowMessage("Barcode generated successfully!", "Success", MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating barcode: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DisplayBarcodeImage(string barcodeText)
        {
            try
            {
                if (pnlBarcode == null)
                {
                    System.Diagnostics.Debug.WriteLine("pnlBarcode is null in DisplayBarcodeImage");
                    return;
                }
                
                if (string.IsNullOrEmpty(barcodeText))
                {
                    pnlBarcode.Controls.Clear();
                    return;
                }

                // Ensure barcode group and panel are visible
                if (barcodeGroup != null)
                {
                    barcodeGroup.Visible = true;
                    barcodeGroup.Enabled = true;
                }
                pnlBarcode.Visible = true;
                pnlBarcode.BringToFront();

                // Generate barcode image using BarcodeService
                var barcodeImage = _barcodeService.GenerateBarcodeImageObject(barcodeText, 440, 80);
                
                if (barcodeImage != null)
                {
                    // Clear the panel and add the barcode image
                    pnlBarcode.Controls.Clear();
                    
                    var pictureBox = new PictureBox
                    {
                        Image = barcodeImage,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Dock = DockStyle.Fill,
                        BackColor = Color.White
                    };
                    
                    pnlBarcode.Controls.Add(pictureBox);
                    
                    // Add barcode text below the image
                    var label = new Label
                    {
                        Text = barcodeText,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Bottom,
                        Height = 25,
                        BackColor = Color.White,
                        ForeColor = Color.Black
                    };
                    
                    pnlBarcode.Controls.Add(label);
                    
                    // Force refresh
                    pnlBarcode.Invalidate();
                    pnlBarcode.Refresh();
                    pnlBarcode.Update();
                    
                    System.Diagnostics.Debug.WriteLine($"Barcode image displayed: {barcodeText}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to generate barcode image");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying barcode image: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowMessage($"Error displaying barcode image: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private string GenerateBarcodeFromCode(string productCode)
        {
            try
            {
                // Generate a unique barcode using product code + timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string uniqueBarcode = $"PRD{productCode}{timestamp.Substring(timestamp.Length - 6)}";
                
                // Ensure uniqueness by checking against existing barcodes
                while (_products.Any(p => p.Barcode == uniqueBarcode))
                {
                    uniqueBarcode = $"PRD{productCode}{DateTime.Now.Ticks.ToString().Substring(DateTime.Now.Ticks.ToString().Length - 6)}";
                }
                
                return uniqueBarcode;
            }
            catch (Exception ex)
            {
                // Fallback to simple barcode if generation fails
                return $"PRD{productCode}{DateTime.Now.Ticks}";
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
                // Get ProductID directly from the clicked row
                DataGridViewRow clickedRow = dgvProducts.Rows[e.RowIndex];
                if (clickedRow.Cells["ProductID"].Value != null)
                {
                    if (int.TryParse(clickedRow.Cells["ProductID"].Value.ToString(), out int productId))
                    {
                        selectedProductId = productId;
                    }
                }
                EditSelectedProduct();
            }
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvProducts == null || dgvProducts.Rows.Count == 0)
                {
                    selectedProductId = -1;
                    return;
                }

                if (dgvProducts.SelectedRows.Count > 0)
                {
                    // Get ProductID directly from the DataGridView row instead of using row index
                    // This works correctly even when products are filtered
                    DataGridViewRow selectedRow = dgvProducts.SelectedRows[0];
                    if (selectedRow != null && selectedRow.Cells["ProductID"] != null && selectedRow.Cells["ProductID"].Value != null)
                    {
                        if (int.TryParse(selectedRow.Cells["ProductID"].Value.ToString(), out int productId))
                        {
                            selectedProductId = productId;
                        }
                        else
                        {
                            selectedProductId = -1;
                        }
                    }
                    else
                    {
                        selectedProductId = -1;
                    }
                }
                else
                {
                    selectedProductId = -1;
                }
            }
            catch
            {
                // Silently handle errors during selection change (e.g., when grid is being cleared)
                selectedProductId = -1;
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

                // Ensure products are loaded
                if (_products == null || _products.Count == 0)
                {
                    LoadProducts();
                }

                var product = _products?.FirstOrDefault(p => p.ProductID == selectedProductId);
                if (product == null)
                {
                    ShowMessage("Product not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Ensure categories and brands are loaded
                if (_categories == null || _categories.Count == 0)
                {
                    LoadCategories();
                }
                if (_brands == null || _brands.Count == 0)
                {
                    LoadBrands();
                }

                // Populate form with product data
                txtProductCode.Text = product.ProductCode ?? "";
                txtProductName.Text = product.ProductName ?? "";
                txtDescription.Text = product.Description ?? "";
                
                // Set category and brand safely
                try
                {
                    if (cmbCategory.Items.Count > 0 && product.CategoryID > 0)
                    {
                        cmbCategory.SelectedValue = product.CategoryID;
                    }
                }
                catch { cmbCategory.SelectedIndex = -1; }
                
                try
                {
                    if (cmbBrand.Items.Count > 0 && product.BrandID > 0)
                    {
                        cmbBrand.SelectedValue = product.BrandID;
                    }
                }
                catch { cmbBrand.SelectedIndex = -1; }
                
                txtPrice.Text = product.CostPrice.ToString("F2");
                txtretailprice.Text = product.RetailPrice.ToString("F2");
                txtReorderLevel.Text = product.ReorderLevel.ToString();
                
                // Load and display barcode
                if (!string.IsNullOrEmpty(product.Barcode))
                {
                    txtBarcode.Text = product.Barcode;
                    DisplayBarcodeImage(product.Barcode);
                }
                else
                {
                    // Generate new barcode if product doesn't have one
                    string barcodeText = GenerateBarcodeFromCode(product.ProductCode ?? "PROD");
                    txtBarcode.Text = barcodeText;
                    DisplayBarcodeImage(barcodeText);
                }
                
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
            // Prevent recursive calls and ensure thread safety
            if (this.IsDisposed)
                return;
                
            // Use BeginInvoke to ensure we're on the UI thread
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => FilterProducts()));
                return;
            }
            
            FilterProducts();
        }

        private void FilterProducts()
        {
            try
            {
                // Check if form and controls are still valid
                if (this.IsDisposed)
                    return;
                    
                if (txtSearch == null || txtSearch.IsDisposed)
                    return;
                    
                if (dgvProducts == null || dgvProducts.IsDisposed)
                    return;

                if (_products == null || _products.Count == 0)
                    return;

                // Safely get search text
                string searchText = "";
                try
                {
                    searchText = txtSearch.Text?.ToLower() ?? "";
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (AccessViolationException)
                {
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    RefreshDataGridView();
                    return;
                }

                var filteredProducts = _products.Where(p => 
                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.ToLower().Contains(searchText)) ||
                    (_categories != null && _categories.Count > 0 && (_categories.FirstOrDefault(c => c.CategoryID == p.CategoryID)?.CategoryName ?? "").ToLower().Contains(searchText)) ||
                    (_brands != null && _brands.Count > 0 && (_brands.FirstOrDefault(b => b.BrandID == p.BrandID)?.BrandName ?? "").ToLower().Contains(searchText))).ToList();

                // Check again before manipulating DataGridView
                if (dgvProducts.IsDisposed)
                    return;

                // Temporarily disable SelectionChanged to prevent errors during clear
                dgvProducts.SelectionChanged -= DgvProducts_SelectionChanged;
                try
                {
                    dgvProducts.Rows.Clear();
                    selectedProductId = -1; // Reset selection
                    
                    foreach (var product in filteredProducts)
                    {
                        // Check again in loop in case control gets disposed
                        if (dgvProducts.IsDisposed)
                            break;
                            
                        string categoryName = "";
                        if (_categories != null && _categories.Count > 0)
                        {
                            var cat = _categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
                            categoryName = cat?.CategoryName ?? "";
                        }
                        
                        string brandName = "";
                        if (_brands != null && _brands.Count > 0)
                        {
                            var brand = _brands.FirstOrDefault(b => b.BrandID == product.BrandID);
                            brandName = brand?.BrandName ?? "";
                        }
                        
                        dgvProducts.Rows.Add(
                            product.ProductID,
                            product.ProductCode,
                            product.ProductName,
                            categoryName,
                            brandName,
                            product.CostPrice,
                            product.RetailPrice,
                            product.StockQuantity,
                            product.ReorderLevel,
                            product.Barcode,
                            product.IsActive ? "Active" : "Inactive"
                        );
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Control was disposed during operation, silently exit
                    return;
                }
                catch (AccessViolationException)
                {
                    // Memory access violation, silently exit
                    return;
                }
                finally
                {
                    // Re-enable SelectionChanged after filtering (only if control is still valid)
                    if (!dgvProducts.IsDisposed)
                    {
                        try
                        {
                            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
                        }
                        catch
                        {
                            // Silently handle if event handler can't be reattached
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Form or control was disposed, silently exit
                return;
            }
            catch (AccessViolationException)
            {
                // Memory access violation, silently exit
                return;
            }
            catch (Exception ex)
            {
                // Only show message for unexpected errors, not disposal/access violations
                if (!this.IsDisposed)
                {
                    try
                    {
                        ShowMessage($"Error filtering products: {ex.Message}", "Error", MessageBoxIcon.Error);
                    }
                    catch
                    {
                        // Silently handle if we can't show message
                    }
                }
            }
        }

        private void UpdateSearchAutoComplete()
        {
            try
            {
                // Check if search textbox is available
                if (txtSearch == null || txtSearch.IsDisposed)
                    return;

                var collection = new AutoCompleteStringCollection();
                if (_products != null && _products.Count > 0)
                {
                    foreach (var p in _products)
                    {
                        if (p == null) continue;
                        
                        if (!string.IsNullOrWhiteSpace(p.ProductName)) 
                            collection.Add(p.ProductName);
                        if (!string.IsNullOrWhiteSpace(p.ProductCode)) 
                            collection.Add(p.ProductCode);
                        if (!string.IsNullOrWhiteSpace(p.Barcode)) 
                            collection.Add(p.Barcode);
                    }
                }
                
                // Only update autocomplete if textbox is still valid
                if (!txtSearch.IsDisposed)
                {
                    txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    txtSearch.AutoCompleteCustomSource = collection;
                }
            }
            catch (Exception ex)
            {
                // Silently handle autocomplete errors to prevent "code blast"
                System.Diagnostics.Debug.WriteLine($"Error updating autocomplete: {ex.Message}");
            }
        }

        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;
            
            // Keep fixed labels: ADD_button = Save (add new), Save_button = Update
            Del_button.Enabled = editMode;
            // Enable only the relevant action
            ADD_button.Enabled = !editMode;   // Save enabled only in add mode
            Save_button.Enabled = editMode;   // Update enabled only in edit mode
            
            // Ensure barcode textbox is always editable
            if (txtBarcode != null)
            {
                txtBarcode.ReadOnly = false;
                txtBarcode.Enabled = true;
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

        private void TxtBarcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string barcodeText = txtBarcode.Text.Trim();
                
                if (string.IsNullOrEmpty(barcodeText))
                {
                    // Clear barcode display if text is empty
                    pnlBarcode.Controls.Clear();
                    return;
                }

                // Validate barcode format in real-time
                if (!_barcodeService.ValidateBarcode(barcodeText))
                {
                    // Don't show error message on every keystroke, just clear the display
                    pnlBarcode.Controls.Clear();
                    return;
                }

                // Check for duplicates (but don't show error on every keystroke)
                bool isDuplicate = _productRepository.IsBarcodeExists(barcodeText, isEditMode ? selectedProductId : (int?)null);
                if (isDuplicate)
                {
                    // Clear display for duplicate barcodes
                    pnlBarcode.Controls.Clear();
                    return;
                }

                // If validation passes, generate and display the barcode
                if (_barcodeService.TestBarcodeGeneration(barcodeText))
                {
                    DisplayBarcodeImage(barcodeText);
                }
            }
            catch (Exception ex)
            {
                // Silently handle errors during real-time validation
                System.Diagnostics.Debug.WriteLine($"Barcode validation error: {ex.Message}");
            }
        }
    }
}
