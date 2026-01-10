using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Models;
using Vape_Store.Helpers;

namespace Vape_Store
{
    public partial class NewSale : Form
    {
        private SalesService _salesService;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private InventoryService _inventoryService;
        private BarcodeService _barcodeService;
        private BusinessDateService _businessDateService;
        private PictureBox picBarcode;
        private TextBox txtBarcodeScanner;
        private Timer _barcodeTimer;
        private bool _isShowingBarcodeError = false;
        private List<Models.SaleItem> saleItems;
        private List<Customer> _customers;
        private List<Product> _products;
        private List<Product> _filteredProducts; // For category/brand filtering
        private List<Category> _categories;
        private List<Brand> _brands;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        // New tax UI controls (runtime)
        private TextBox taxPercentTextBox; // user enters percent
        private TextBox taxAmountTextBox;  // readonly amount
        
        private decimal subtotal = 0;
        private decimal discount = 0;
        private decimal tax = 0;
        private decimal total = 0;
        private decimal paidAmount = 0;
        private decimal changeAmount = 0;
        private string invoiceNumber = "";
        private int currentUserID = 1; // Default user ID, should be from UserSession
        private bool _isUpdatingDiscount = false; // Flag to prevent recursive updates
        private bool _isUpdatingTax = false; // Flag to prevent recursive tax updates

        public NewSale()
        {
            InitializeComponent();
            _salesService = new SalesService();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            _barcodeService = new BarcodeService();
            _businessDateService = new BusinessDateService();
            saleItems = new List<Models.SaleItem>();
            
            InitializeDataGridView();
            SetupEventHandlers();
            CreateBarcodePictureBox();
            CreateBarcodeScanner();
            LoadCustomers();
            LoadProducts();
            LoadCategories();
            LoadBrands();
            GenerateInvoiceNumber();
            InitializePaymentMethods();
            
            // Set date to current business date (not calendar date)
            guna2DateTimePicker1.Value = _businessDateService.GetCurrentBusinessDate();
            
            // Ensure barcode is displayed after form is fully loaded
            this.Load += (s, e) => {
                if (picBarcode != null && !string.IsNullOrEmpty(invoiceNumber))
                {
                    GenerateAndDisplayBarcode();
                }
            };
            
            // Subscribe to product update events
            ProductRepository.ProductsUpdated += OnProductsUpdated;
            
            // Handle form closing to unsubscribe from events
            this.FormClosing += NewSale_FormClosing;
        }

        private void InitializeDataGridView()
        {
            // Configure DataGridView for cart items
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = false; // Allow editing
            
            // Configure columns
            SrNo.Width = 80;
            ItemName.Width = 400;
            Qty.Width = 100;
            Price.Width = 120;
            SubTotal.Width = 120;
            
            // Set column headers
            SrNo.HeaderText = "Sr.No";
            ItemName.HeaderText = "Product Name";
            Qty.HeaderText = "Quantity";
            Price.HeaderText = "Unit Price";
            SubTotal.HeaderText = "Sub Total";
            
            // Make specific columns editable/readonly
            SrNo.ReadOnly = true;        // Serial number - not editable
            ItemName.ReadOnly = false;    // Product name - editable
            Qty.ReadOnly = false;         // Quantity - editable
            Price.ReadOnly = true;        // Unit price - not editable
            SubTotal.ReadOnly = true;     // Sub total - not editable (calculated)
            
            // Format currency columns
            Price.DefaultCellStyle.Format = "F2";
            SubTotal.DefaultCellStyle.Format = "F2";
            
            // Set up cell editing
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            NewItemBtn.Click += NewItemBtn_Click;
            saveBtn.Click += SaveBtn_Click;
            ClearBtn.Click += ClearBtn_Click;
            CancelBtn.Click += CancelBtn_Click;
            RefreshBtn.Click += RefreshBtn_Click;
            
            // Product search event handlers
            txtProductName.TextChanged += TxtProductName_TextChanged;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            cmbBrand.SelectedIndexChanged += CmbBrand_SelectedIndexChanged;

            // Handle Enter key for manual adding via Quantity box
            txtQuantity.KeyDown += TxtQuantity_KeyDown;
            // Also handle Enter key in product name textbox to add item
            txtProductName.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    AddItem();
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Prevent beep
                }
            };
            // Hide Add Item button as requested (Auto-add on scan / manual add via Enter)
            btnAddItem.Visible = false;
            
            // Payment calculation event handlers
            txtPaid.TextChanged += TxtPaid_TextChanged;
            txtTaxPercent.TextChanged += TxtTaxPercent_TextChanged; // Discount percentage
            if (txtDiscountAmount != null)
            {
                txtDiscountAmount.TextChanged += TxtDiscountAmount_TextChanged; // Discount amount
            }
            
            // Tax calculation event handlers
            if (txtTaxPercentInput != null)
            {
                txtTaxPercentInput.TextChanged += TxtTaxPercentInput_TextChanged; // Tax percentage
            }
            if (txtTaxAmountInput != null)
            {
                txtTaxAmountInput.TextChanged += TxtTaxAmountInput_TextChanged; // Tax amount
            }
            
            // DataGridView event handlers
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.CellClick += DataGridView1_CellClick;
            
            // Add Item button event handler - REMOVED (already handled in SetupEventHandlers)
            
            // Product search functionality - REMOVED (already handled in SetupEventHandlers)
            txtProductName.Leave += TxtProductName_Leave;
            
            // Form events
            this.Activated += NewSale_Activated;

            // Tax controls are now in Designer - no need to create at runtime
            // Hide the old dropdown if it exists
                if (cmbTax != null)
                {
                    cmbTax.Visible = false;
                    cmbTax.Enabled = false;
                }
            
            // Initialize tax textboxes
            if (txtTaxPercentInput != null)
            {
                txtTaxPercentInput.Text = "0";
            }
            if (txtTaxAmountInput != null)
            {
                txtTaxAmountInput.Text = "0.00";
            }
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                
                // Make ComboBox searchable
                SearchableComboBoxHelper.MakeSearchable(cmbCustomer, _customers, "CustomerName", "CustomerID", "CustomerName");
                
                // Set "Walk-in Customer" as default (case-insensitive, tolerant to spacing)
                var walkInCustomer = _customers.FirstOrDefault(c =>
                    (c.CustomerName ?? string.Empty).Trim().ToLower().Contains("walk-in") ||
                    (c.CustomerName ?? string.Empty).Trim().ToLower().Contains("walk in") ||
                    (c.CustomerName ?? string.Empty).Trim().ToLower().Contains("walkin"));
                if (walkInCustomer != null)
                {
                    cmbCustomer.SelectedValue = walkInCustomer.CustomerID;
                }
                else if (_customers.Count > 0)
                {
                    cmbCustomer.SelectedIndex = 0; // Select first customer if Walk-in not found
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                // Load ALL products from Products table (not just those with stock > 0)
                _products = _productRepository.GetAllProducts();
                _filteredProducts = _products; // Initialize filtered products with all products
                
                System.Diagnostics.Debug.WriteLine($"Loaded {_products?.Count ?? 0} products from Products table");
                
                // Update stock quantities from purchase/inventory data
                UpdateStockQuantitiesFromInventory();
                
                if (_products != null)
                {
                    foreach (var product in _products)
                    {
                        System.Diagnostics.Debug.WriteLine($"Product: {product.ProductName} - Stock: {product.StockQuantity}");
                    }
                }
                
                // Setup autocomplete after loading products
                UpdateProductAutoComplete();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        
        private void UpdateStockQuantitiesFromInventory()
        {
            try
            {
                if (_products == null || _products.Count == 0)
                    return;

                using (var connection = DataAccess.DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    
                    // Get current stock quantities from Products table (which gets updated from purchases)
                    // This ensures we get the most up-to-date stock information from SQL Express
                    string query = @"
                        SELECT ProductID, StockQuantity 
                        FROM Products 
                        WHERE IsActive = 1";
                    
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 30; // Set timeout for SQL Express
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = Convert.ToInt32(reader["ProductID"]);
                                int stockQuantity = Convert.ToInt32(reader["StockQuantity"]);
                                
                                // Update the stock quantity in our local product list
                                var product = _products.FirstOrDefault(p => p.ProductID == productId);
                                if (product != null)
                                {
                                    product.StockQuantity = stockQuantity;
                                }
                            }
                        }
                    }
                }
                
                // Also update filtered products if they exist
                if (_filteredProducts != null)
                {
                    foreach (var filteredProduct in _filteredProducts)
                    {
                        var mainProduct = _products.FirstOrDefault(p => p.ProductID == filteredProduct.ProductID);
                        if (mainProduct != null)
                        {
                            filteredProduct.StockQuantity = mainProduct.StockQuantity;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating stock quantities: {ex.Message}");
                ShowMessage($"Warning: Could not update stock quantities: {ex.Message}", "Warning", MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Refreshes stock quantities from the database
        /// Call this method when you need to get the latest stock information
        /// </summary>
        public void RefreshStockQuantities()
        {
            try
            {
                UpdateStockQuantitiesFromInventory();
                System.Diagnostics.Debug.WriteLine("Stock quantities refreshed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing stock quantities: {ex.Message}");
                ShowMessage($"Error refreshing stock quantities: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryRepository.GetAllCategories();
                
                // Create a list with "All" option at the beginning
                var categoryList = new List<Category>();
                
                // Add "All" option as a special category
                categoryList.Add(new Category 
                { 
                    CategoryID = -1, 
                    CategoryName = "All" 
                });
                
                // Add actual categories
                categoryList.AddRange(_categories);
                
                // Make ComboBox searchable
                SearchableComboBoxHelper.MakeSearchable(cmbCategory, categoryList, "CategoryName", "CategoryID", "CategoryName");
                cmbCategory.SelectedIndex = 0; // Select "All" by default
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
                
                // Create a list with "All" option at the beginning
                var brandList = new List<Brand>();
                
                // Add "All" option as a special brand
                brandList.Add(new Brand 
                { 
                    BrandID = -1, 
                    BrandName = "All" 
                });
                
                // Add actual brands
                brandList.AddRange(_brands);
                
                // Make ComboBox searchable
                SearchableComboBoxHelper.MakeSearchable(cmbBrand, brandList, "BrandName", "BrandID", "BrandName");
                cmbBrand.SelectedIndex = 0; // Select "All" by default
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        
        private void GenerateInvoiceNumber()
        {
            try
            {
                string oldInvoiceNumber = invoiceNumber;
                invoiceNumber = _salesService.GetNextInvoiceNumber();
                label4.Text = invoiceNumber;
                
                // Debug output to verify invoice number is changing
                System.Diagnostics.Debug.WriteLine($"[Invoice] Old: {oldInvoiceNumber}, New: {invoiceNumber}");
                
                // Generate and display barcode with new invoice number
                GenerateAndDisplayBarcode();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating invoice number: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[Invoice] Error: {ex.Message}");
            }
        }

        private void CreateBarcodePictureBox()
        {
            picBarcode = new PictureBox
            {
                Name = "picBarcode",
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(250, 470), // Position below the Barcode label
                Size = new Size(150, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabIndex = 0,
                TabStop = false,
                Visible = true
            };
            
            // Add to the main form (not the header panel)
            this.Controls.Add(picBarcode);
            picBarcode.BringToFront();
        }

        private void CreateBarcodeScanner()
        {
            // Create barcode scanner input field
            txtBarcodeScanner = new TextBox
            {
                Name = "txtBarcodeScanner",
                Text = "Scan or enter product barcode...",
                Location = new Point(20, 100),
                Size = new Size(200, 25),
                TabIndex = 0,
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray
            };
            
            // Add placeholder functionality
            txtBarcodeScanner.Enter += (s, e) => {
                if (txtBarcodeScanner.Text == "Scan or enter product barcode...")
                {
                    txtBarcodeScanner.Text = "";
                    txtBarcodeScanner.ForeColor = Color.Black;
                }
            };
            
            txtBarcodeScanner.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtBarcodeScanner.Text))
                {
                    txtBarcodeScanner.Text = "Scan or enter product barcode...";
                    txtBarcodeScanner.ForeColor = Color.Gray;
                }
            };
            
            // Handle text changed event to clear placeholder when user types
            txtBarcodeScanner.TextChanged += (s, e) => {
                if (txtBarcodeScanner.Text != "Scan or enter product barcode...")
                {
                    txtBarcodeScanner.ForeColor = Color.Black;
                }
            };
            
            // Add to the first panel (green header panel)
            if (this.Controls.Count > 0 && this.Controls[0] is Panel panel1)
            {
                panel1.Controls.Add(txtBarcodeScanner);
                txtBarcodeScanner.BringToFront();
            }
            
            // Add event handler for barcode scanning
            txtBarcodeScanner.KeyPress += TxtBarcodeScanner_KeyPress;
            txtBarcodeScanner.TextChanged += TxtBarcodeScanner_TextChanged;
        }

        private void GenerateAndDisplayBarcode()
        {
            try
            {
                if (!string.IsNullOrEmpty(invoiceNumber) && picBarcode != null)
                {
                    // Dispose of old image to prevent memory leaks and ensure new barcode is displayed
                    if (picBarcode.Image != null)
                    {
                        var oldImage = picBarcode.Image;
                        picBarcode.Image = null;
                        oldImage.Dispose();
                    }
                    
                    // Generate barcode image with current invoice number
                    var barcodeImage = _barcodeService.GenerateBarcodeImageObject(invoiceNumber, 150, 50);
                    
                    if (barcodeImage != null)
                    {
                        // Display barcode in PictureBox
                        picBarcode.Image = barcodeImage;
                        picBarcode.SizeMode = PictureBoxSizeMode.Zoom;
                        picBarcode.Visible = true;
                        picBarcode.BringToFront();
                        
                        // Force refresh
                        picBarcode.Invalidate();
                        picBarcode.Refresh();
                        
                        // Debug output to verify invoice number is changing
                        System.Diagnostics.Debug.WriteLine($"[Barcode] Generated barcode for invoice: {invoiceNumber}");
                    }
                    else
                    {
                        // Failed to generate barcode image
                        System.Diagnostics.Debug.WriteLine($"[Barcode] Failed to generate barcode for invoice: {invoiceNumber}");
                    }
                }
                else
                {
                    // Silently handle missing components
                    System.Diagnostics.Debug.WriteLine($"[Barcode] Cannot generate barcode - InvoiceNumber: {invoiceNumber ?? "null"}, picBarcode: {picBarcode != null}");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating barcode: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[Barcode] Error: {ex.Message}");
            }
        }

        private void TxtPaid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void TxtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddItem();
                e.Handled = true;
                e.SuppressKeyPress = true; // Prevent beep
            }
        }
        private void TxtBarcodeScanner_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Handle Enter key to process barcode
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcessBarcodeScan();
            }
        }

        private void TxtBarcodeScanner_TextChanged(object sender, EventArgs e)
        {
            // Auto-process barcode when text is entered (for barcode scanners that don't send Enter)
            if (txtBarcodeScanner.Text.Length >= 3 && txtBarcodeScanner.Text != "Scan or enter product barcode...") // Minimum barcode length
            {
                // Use a timer to avoid processing while user is still typing
                // Stop any existing timer to prevent multiple executions
                if (_barcodeTimer != null)
                {
                    _barcodeTimer.Stop();
                    _barcodeTimer.Dispose();
                }
                
                _barcodeTimer = new Timer();
                _barcodeTimer.Interval = 150; // Slightly increased delay to ensure complete barcode scan
                _barcodeTimer.Tick += (s, args) => {
                    _barcodeTimer.Stop();
                    _barcodeTimer.Dispose();
                    ProcessBarcodeScan();
                };
                _barcodeTimer.Start();
            }
        }

        private void ProcessBarcodeScan()
        {
            try
            {
                string scannedBarcode = txtBarcodeScanner.Text.Trim();
                
                // Ignore placeholder text
                if (string.IsNullOrEmpty(scannedBarcode) || scannedBarcode == "Scan or enter product barcode...")
                    return;

                // Find product by barcode - enhanced search with multiple comparison methods
                var product = _products?.FirstOrDefault(p => 
                    !string.IsNullOrEmpty(p.Barcode) && 
                    (p.Barcode.Trim().Equals(scannedBarcode, StringComparison.OrdinalIgnoreCase) ||
                     p.Barcode.Trim().Equals(scannedBarcode.Trim(), StringComparison.Ordinal) ||
                     p.Barcode.Trim().Contains(scannedBarcode)));
                
                if (product != null)
                {
                    // Product found - add to cart automatically (Immediate Action)
                    AddProductToCart(product);
                    
                    // Clear scanner input completely
                    txtBarcodeScanner.Clear();
                    txtBarcodeScanner.Focus();
                    
                    // Reset error flag
                    _isShowingBarcodeError = false;
                }
                else
                {
                    // Only show error if not already showing one
                    if (!_isShowingBarcodeError)
                    {
                        _isShowingBarcodeError = true;
                        ShowMessage($"Product not found for barcode: {scannedBarcode}", "Product Not Found", MessageBoxIcon.Warning);
                        txtBarcodeScanner.Clear();
                        txtBarcodeScanner.Focus();
                        
                        // Reset the error flag after a brief delay to allow UI to update
                        Timer resetTimer = new Timer();
                        resetTimer.Interval = 500; // 500ms delay
                        resetTimer.Tick += (s, args) => {
                            resetTimer.Stop();
                            resetTimer.Dispose();
                            _isShowingBarcodeError = false;
                        };
                        resetTimer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing barcode: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void AddProductToCart(Product product)
        {
            try
            {
                // Check if product is already in cart
                var existingItem = saleItems.FirstOrDefault(item => item.ProductID == product.ProductID);
                
                if (existingItem != null)
                {
                    // Increase quantity by 1
                    existingItem.Quantity += 1;
                    existingItem.SubTotal = existingItem.Quantity * existingItem.UnitPrice;
                }
                else
                {
                    // Add new item to cart
                    var saleItem = new SaleItem
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Quantity = 1,
                        UnitPrice = product.RetailPrice,
                        SubTotal = product.RetailPrice
                    };
                    
                    saleItems.Add(saleItem);
                }
                
                // Refresh the data grid
                RefreshCartDisplay();
                CalculateTotals();
                
                ShowMessage($"Added {product.ProductName} to cart", "Product Added", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding product to cart: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void InitializePaymentMethods()
        {
            var paymentMethods = new List<string>
            {
                "Cash",
                "Card",
                "Credit",
                "Bank Transfer",
                "Check",
                "Digital Wallet"
            };
            SearchableComboBoxHelper.MakeSearchable(cmbPaymentMethod, paymentMethods);
            cmbPaymentMethod.SelectedIndex = 0; // Default to Cash
        }

        private void InitializeTaxOptions()
        {
            cmbTax.Items.Clear();
            cmbTax.Items.AddRange(new string[] { "0%", "5%", "10%", "15%", "20%" });
            cmbTax.SelectedIndex = 2; // Default to 10%
        }

        private void NewItemBtn_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        private void AddItem()
        {
            try
            {
                // Validate product name
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    ShowMessage("Please enter a product name to add to the cart.", "Add Product", MessageBoxIcon.Information);
                    txtProductName.Focus();
                    return;
                }

                // Validate quantity
                if (string.IsNullOrWhiteSpace(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int requestedQuantity) || requestedQuantity <= 0)
                {
                    ShowMessage("Please enter a valid quantity (must be greater than 0).", "Add Product", MessageBoxIcon.Information);
                    txtQuantity.Focus();
                    return;
                }

                // Find product by name, code, or barcode - enhanced search with case-insensitive matching
                var productsToSearch = _filteredProducts ?? _products ?? new List<Product>();
                string searchText = txtProductName.Text.Trim();
                var product = productsToSearch.FirstOrDefault(p => 
                    p != null && (
                        (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                    );

                if (product == null)
                {
                    ShowMessage("Product not found. Please check the product name or try selecting a different category/brand.", "Product Not Found", MessageBoxIcon.Information);
                    txtProductName.Focus();
                    return;
                }

                // Check stock availability
                if (product.StockQuantity <= 0)
                {
                    ShowMessage($"Product '{product.ProductName}' is out of stock.", "Stock Error", MessageBoxIcon.Warning);
                    return;
                }

                // Check if requested quantity exceeds available stock
                if (requestedQuantity > product.StockQuantity)
                {
                    ShowMessage($"Only {product.StockQuantity} units available in stock. You requested {requestedQuantity}.", "Stock Error", MessageBoxIcon.Warning);
                    return;
                }

                // Check if product already exists in cart
                var existingItem = saleItems.FirstOrDefault(item => item.ProductID == product.ProductID);
                if (existingItem != null)
                {
                    // Check if adding this quantity would exceed stock
                    if (existingItem.Quantity + requestedQuantity > product.StockQuantity)
                    {
                        ShowMessage($"Only {product.StockQuantity} units available in stock. You already have {existingItem.Quantity} in cart, cannot add {requestedQuantity} more.", "Stock Error", MessageBoxIcon.Warning);
                        return;
                    }
                    existingItem.Quantity += requestedQuantity;
                    existingItem.SubTotal = existingItem.Quantity * existingItem.UnitPrice;
                }
                else
                {
                    // Add new item to cart
                    var saleItem = new Models.SaleItem
                    {
                        ProductID = product.ProductID,
                        Quantity = requestedQuantity,
                        UnitPrice = product.RetailPrice,
                        SubTotal = requestedQuantity * product.RetailPrice,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode
                    };
                    saleItems.Add(saleItem);
                }

                // Update stock display
                txtStockQuantity.Text = product.StockQuantity.ToString();
                txtReorderLevel.Text = product.ReorderLevel.ToString();

                // Refresh cart display
                RefreshCartDisplay();
                CalculateTotals();
                
                // Show success message
                ShowMessage($"'{product.ProductName}' added to cart successfully!", "Product Added", MessageBoxIcon.Information);
                
                // Clear product search and quantity
                txtProductName.Clear();
                txtQuantity.Text = "1";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding item: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshCartDisplay()
        {
            dataGridView1.Rows.Clear();
            
            for (int i = 0; i < saleItems.Count; i++)
            {
                var item = saleItems[i];
                dataGridView1.Rows.Add(
                    i + 1,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.SubTotal
                );
            }
        }

        private void CalculateTotals()
        {
            try
            {
                // Calculate subtotal
                decimal newSubtotal = saleItems.Sum(item => item.SubTotal);
                
                // If subtotal changed and user had entered a percentage, update the amount
                if (newSubtotal != subtotal && subtotal > 0 && !_isUpdatingDiscount)
                {
                    if (!string.IsNullOrWhiteSpace(txtTaxPercent.Text) && 
                        decimal.TryParse(txtTaxPercent.Text, out decimal discountPercent))
                    {
                        // Recalculate discount amount based on new subtotal
                        decimal newDiscountAmount = newSubtotal * (discountPercent / 100);
                        if (txtDiscountAmount != null)
                        {
                            _isUpdatingDiscount = true;
                            txtDiscountAmount.TextChanged -= TxtDiscountAmount_TextChanged;
                            txtDiscountAmount.Text = newDiscountAmount.ToString("F2");
                            txtDiscountAmount.TextChanged += TxtDiscountAmount_TextChanged;
                            _isUpdatingDiscount = false;
                        }
                    }
                }
                
                // If subtotal/discount changed and user had entered a tax percentage, update the tax amount
                decimal newTaxableAmount = newSubtotal - discount;
                decimal oldTaxableAmount = subtotal - discount;
                if ((newSubtotal != subtotal || discount != 0) && oldTaxableAmount > 0 && !_isUpdatingTax)
                {
                    if (txtTaxPercentInput != null && !string.IsNullOrWhiteSpace(txtTaxPercentInput.Text) && 
                        decimal.TryParse(txtTaxPercentInput.Text, out decimal taxPercent))
                    {
                        // Recalculate tax amount based on new taxable amount
                        decimal newTaxAmount = newTaxableAmount * (taxPercent / 100);
                        if (txtTaxAmountInput != null)
                        {
                            _isUpdatingTax = true;
                            txtTaxAmountInput.TextChanged -= TxtTaxAmountInput_TextChanged;
                            txtTaxAmountInput.Text = newTaxAmount.ToString("F2");
                            txtTaxAmountInput.TextChanged += TxtTaxAmountInput_TextChanged;
                            _isUpdatingTax = false;
                        }
                    }
                }
                
                subtotal = newSubtotal;
                txtsubTotal.Text = subtotal.ToString("F2");

            // Calculate discount - prioritize amount over percentage if both are entered
            // If discount amount is entered, use it directly
            if (txtDiscountAmount != null && !string.IsNullOrWhiteSpace(txtDiscountAmount.Text) && 
                decimal.TryParse(txtDiscountAmount.Text, out decimal discountAmount))
            {
                discount = discountAmount;
            }
            // Otherwise, calculate from percentage
            else if (decimal.TryParse(txtTaxPercent.Text, out decimal discountPercent))
            {
                discount = subtotal * (discountPercent / 100);
            }
            else
            {
                discount = 0;
            }

            // Calculate tax - prioritize amount over percentage if both are entered
            decimal taxableAmount = subtotal - discount;
            
            // If tax amount is entered, use it directly
            if (txtTaxAmountInput != null && !string.IsNullOrWhiteSpace(txtTaxAmountInput.Text) && 
                decimal.TryParse(txtTaxAmountInput.Text, out decimal taxAmount))
            {
                tax = taxAmount;
            }
            // Otherwise, calculate from percentage
            else if (txtTaxPercentInput != null && decimal.TryParse(txtTaxPercentInput.Text, out decimal taxPercent))
            {
                tax = taxableAmount * (taxPercent / 100);
            }
            else
            {
                tax = 0;
                }

                // Calculate total
                total = subtotal - discount + tax;
                txtTotal.Text = total.ToString("F2");

                // Calculate change
                CalculateChange();
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Totals Updated: Subtotal={subtotal}, Discount={discount}, Tax={tax}, Total={total}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating totals: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"CalculateTotals Error: {ex.Message}");
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveSale();
        }

        private void SaveSale()
        {
            try
            {
                // Ensure totals are calculated before saving
                CalculateTotals();
                
                // Validate cart
                if (saleItems.Count == 0)
                {
                    ShowMessage("Please add at least one item to the cart.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Validate stock availability before saving
                foreach (var item in saleItems)
                {
                    var product = _products.FirstOrDefault(p => p.ProductID == item.ProductID);
                    if (product != null)
                    {
                        if (product.StockQuantity < item.Quantity)
                        {
                            ShowMessage($"Insufficient stock for '{product.ProductName}'. Available: {product.StockQuantity}, Required: {item.Quantity}", "Stock Error", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                // Validate payment
                if (string.IsNullOrEmpty(cmbPaymentMethod.Text))
                {
                    ShowMessage("Please select a payment method.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Handle payment amount based on payment method
                decimal currentPaidAmount;
                bool isCardPayment = cmbPaymentMethod.Text.Equals("Card", StringComparison.OrdinalIgnoreCase);
                bool isCreditSale = cmbPaymentMethod.Text.Equals("Credit", StringComparison.OrdinalIgnoreCase);

                if (isCardPayment)
                {
                    // For card payments, paid amount equals total amount
                    currentPaidAmount = total;
                    txtPaid.Text = currentPaidAmount.ToString("F2");
                }
                else if (isCreditSale)
                {
                    // For credit sales, allow partial or zero payment
                    if (!decimal.TryParse(txtPaid.Text, out currentPaidAmount))
                    {
                        currentPaidAmount = 0;
                        txtPaid.Text = "0.00";
                    }

                    if (currentPaidAmount > total)
                    {
                        ShowMessage("Paid amount cannot exceed total amount for credit sales.", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    // For cash and other payment methods, validate paid amount input (must cover total)
                    if (!decimal.TryParse(txtPaid.Text, out currentPaidAmount))
                    {
                        ShowMessage("Please enter a valid paid amount.", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }

                    if (currentPaidAmount < total)
                    {
                        ShowMessage($"Paid amount ({currentPaidAmount:F2}) is less than total amount ({total:F2}).", "Payment Error", MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Validate date before saving
                DateTime saleDate = guna2DateTimePicker1.Value;
                if (!_businessDateService.CanCreateTransaction(saleDate))
                {
                    string message = _businessDateService.GetValidationMessage(saleDate);
                    ShowMessage(message, "Date Closed", MessageBoxIcon.Warning);
                    return;
                }

                // Generate barcode data for the sale
                byte[] barcodeImageBytes = null;
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    barcodeImageBytes = _barcodeService.GenerateBarcodeImage(invoiceNumber, 300, 100);
                }

                // Create sale object with current calculated values
                decimal saleTaxableAmount = subtotal - discount;
                var sale = new Sale
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerID = cmbCustomer.SelectedItem != null ? ((Customer)cmbCustomer.SelectedItem).CustomerID : 0,
                    SaleDate = saleDate,
                    SubTotal = subtotal,
                TaxAmount = tax,
                    TaxPercent = saleTaxableAmount > 0 ? (tax / saleTaxableAmount) * 100 : 0,
                    TotalAmount = total,
                    PaymentMethod = cmbPaymentMethod.Text,
                    PaidAmount = currentPaidAmount,
                    ChangeAmount = currentPaidAmount - total,
                    UserID = currentUserID,
                    DiscountAmount = discount,
                    DiscountPercent = subtotal > 0 ? (discount / subtotal) * 100 : 0,
                    BarcodeImage = barcodeImageBytes,
                    BarcodeData = invoiceNumber,
                    SaleItems = saleItems
                };

                // Debug output
                System.Diagnostics.Debug.WriteLine($"[SaveSale] Invoice Number: {invoiceNumber}");
                System.Diagnostics.Debug.WriteLine($"Save Sale Data: Subtotal={sale.SubTotal}, TaxAmount={sale.TaxAmount}, TaxPercent={sale.TaxPercent}, Total={sale.TotalAmount}");

                // Process sale
                bool success = _salesService.ProcessSale(sale);
                
                if (success)
                {
                    var outstandingAmount = total - currentPaidAmount;
                    if (isCreditSale && outstandingAmount > 0)
                    {
                        ShowMessage($"Sale recorded successfully on credit.\nOutstanding balance for customer: {outstandingAmount:F2}", "Credit Sale", MessageBoxIcon.Information);
                    }
                    else
                {
                    ShowMessage("Sale completed successfully!", "Success", MessageBoxIcon.Information);
                    }
                    
                    // Print receipt
                    PrintReceipt();
                    
                    // Clear form for next sale
                    ClearForm();
                }
                else
                {
                    ShowMessage("Failed to process sale. Please try again.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving sale: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"SaveSale Error: {ex.Message}");
            }
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            try
            {
                // Clear cart
                saleItems.Clear();
                dataGridView1.Rows.Clear();
                
                // Clear calculations
                subtotal = 0;
                discount = 0;
                tax = 0;
                total = 0;
                paidAmount = 0;
                changeAmount = 0;
                
                // Reset form controls
                txtsubTotal.Clear();
                txtTaxPercent.Clear();
                txtTotal.Clear();
                txtPaid.Clear();
                txtChange.Clear();
                txtProductName.Clear();
                txtStockQuantity.Clear();
                txtReorderLevel.Clear();

                // Immediately prepare next invoice/barcode so the UI updates even if later steps fail
                GenerateInvoiceNumber();

                // Reload lookups and set sensible defaults so dropdowns aren't blank
                LoadCustomers();
                LoadCategories();
                LoadBrands();
                UpdateProductAutoComplete();

                // Reset selections with defaults
                cmbPaymentMethod.SelectedIndex = 0;
                if (cmbTax != null && cmbTax.Visible && cmbTax.Items != null && cmbTax.Items.Count > 2)
                {
                    cmbTax.SelectedIndex = 2;
                }
                if (txtTaxPercentInput != null) txtTaxPercentInput.Text = "0";
                if (txtTaxAmountInput != null) txtTaxAmountInput.Text = "0.00";
                
                // Set current business date (not calendar date)
                guna2DateTimePicker1.Value = _businessDateService.GetCurrentBusinessDate();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error clearing form: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadProducts();
            LoadCustomers();
            LoadCategories();
            LoadBrands();
            ShowMessage("Data refreshed successfully.", "Info", MessageBoxIcon.Information);
        }

        private void PrintReceipt()
        {
            try
            {
                // Ensure totals are calculated before creating receipt
                CalculateTotals();
                
                // Generate barcode for receipt
                byte[] barcodeImageBytes = null;
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    barcodeImageBytes = _barcodeService.GenerateBarcodeImage(invoiceNumber, 200, 80);
                }

                // Create sale object for receipt with current calculated values
                decimal receiptTaxableAmount = subtotal - discount;
                var sale = new Sale
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerID = cmbCustomer.SelectedItem != null ? ((Customer)cmbCustomer.SelectedItem).CustomerID : 0,
                    SaleDate = guna2DateTimePicker1.Value,
                    SubTotal = subtotal,
                    TaxAmount = tax,
                    TaxPercent = receiptTaxableAmount > 0 ? (tax / receiptTaxableAmount) * 100 : 0,
                    TotalAmount = total,
                    PaymentMethod = cmbPaymentMethod.Text,
                    PaidAmount = decimal.TryParse(txtPaid.Text, out decimal paid) ? paid : 0,
                    ChangeAmount = (decimal.TryParse(txtPaid.Text, out decimal paidAmount) ? paidAmount : 0) - total,
                    UserID = currentUserID,
                    DiscountAmount = discount,
                    DiscountPercent = subtotal > 0 ? (discount / subtotal) * 100 : 0,
                    BarcodeImage = barcodeImageBytes,
                    BarcodeData = invoiceNumber,
                    SaleItems = saleItems
                };

                // Debug output
                System.Diagnostics.Debug.WriteLine($"Receipt Data: Subtotal={sale.SubTotal}, TaxAmount={sale.TaxAmount}, TaxPercent={sale.TaxPercent}, Total={sale.TotalAmount}");

                // Show receipt preview
                var receiptPreview = new ReceiptPreviewForm(sale);
                receiptPreview.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing receipt: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"PrintReceipt Error: {ex.Message}");
            }
        }

        // Event Handlers
        private void TxtProductName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtProductName.Text.Trim();
                
                if (string.IsNullOrEmpty(searchText))
                {
                    // Clear stock display when search is empty
                    txtStockQuantity.Clear();
                    txtReorderLevel.Clear();
                    return;
                }

                // Find exact match first (for AutoComplete selection)
                var productsToSearch = _filteredProducts ?? _products ?? new List<Product>();
                var exactMatch = productsToSearch.FirstOrDefault(p => 
                    p != null && (
                        (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.Equals(searchText, StringComparison.OrdinalIgnoreCase))
                    ));

                if (exactMatch != null)
                {
                    // Show exact match details
                    txtStockQuantity.Text = exactMatch.StockQuantity.ToString();
                    txtReorderLevel.Text = exactMatch.ReorderLevel.ToString();
                }
                else
                {
                    // Search for products that contain the search text
                    var matchingProducts = productsToSearch.Where(p => 
                        p.ProductName.ToLower().Contains(searchText.ToLower()) ||
                        p.ProductCode.ToLower().Contains(searchText.ToLower()) ||
                        (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.ToLower().Contains(searchText.ToLower()))
                    ).ToList();

                    if (matchingProducts.Count > 0)
                    {
                        // Show the first matching product's stock info
                        var firstProduct = matchingProducts.First();
                        txtStockQuantity.Text = firstProduct.StockQuantity.ToString();
                        txtReorderLevel.Text = firstProduct.ReorderLevel.ToString();
                    }
                    else
                    {
                        // No matches found
                        txtStockQuantity.Clear();
                        txtReorderLevel.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching products: {ex.Message}", "Search Error", MessageBoxIcon.Error);
            }
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filter products by category
            if (cmbCategory.SelectedItem != null && cmbCategory.SelectedIndex >= 0)
            {
                try
                {
                    // Get the selected category object
                    Category selectedCategory = (Category)cmbCategory.SelectedItem;
                    int categoryID = selectedCategory.CategoryID;
                    
                    if (categoryID == -1) // "All" selected
                    {
                        // Show all products
                        FilterProductsByBrand();
                    }
                    else
                    {
                        // Filter by category (include products with CategoryID = 0 for NULL values)
                        var filteredProducts = _products.Where(p => p.CategoryID == categoryID || p.CategoryID == 0).ToList();
                        FilterProductsByBrand(filteredProducts);
                    }
                    
                    // Update product autocomplete
                    UpdateProductAutoComplete();
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error filtering products by category: {ex.Message}", "Error", MessageBoxIcon.Error);
                }
            }
        }

        private void CmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filter products by brand
            if (cmbBrand.SelectedItem != null && cmbBrand.SelectedIndex >= 0)
            {
                try
                {
                    // Get the selected brand object
                    Brand selectedBrand = (Brand)cmbBrand.SelectedItem;
                    int brandID = selectedBrand.BrandID;
                    
                    if (brandID == -1) // "All" selected
                    {
                        // Show all products
                        FilterProductsByCategory();
                    }
                    else
                    {
                        // Filter by brand (include products with BrandID = 0 for NULL values)
                        var filteredProducts = _products.Where(p => p.BrandID == brandID || p.BrandID == 0).ToList();
                        FilterProductsByCategory(filteredProducts);
                    }
                    
                    // Update product autocomplete
                    UpdateProductAutoComplete();
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error filtering products by brand: {ex.Message}", "Error", MessageBoxIcon.Error);
                }
            }
        }

        private void CmbTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Tax dropdown changed to: {cmbTax.SelectedItem}");
                CalculateTotals();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in tax dropdown change: {ex.Message}");
                ShowMessage($"Error updating tax calculation: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void FilterProductsByCategory(List<Product> productsToFilter = null)
        {
            try
            {
                var products = productsToFilter ?? _products;
                
                // Get current category selection
                if (cmbCategory.SelectedItem != null)
                {
                    Category selectedCategory = (Category)cmbCategory.SelectedItem;
                    int categoryID = selectedCategory.CategoryID;
                    
                    if (categoryID != -1) // Not "All"
                    {
                        products = products.Where(p => p.CategoryID == categoryID || p.CategoryID == 0).ToList();
                    }
                }
                
                // Apply brand filter if needed
                FilterProductsByBrand(products);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering products by category: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void FilterProductsByBrand(List<Product> productsToFilter = null)
        {
            try
            {
                var products = productsToFilter ?? _products;
                
                // Get current brand selection
                if (cmbBrand.SelectedItem != null)
                {
                    Brand selectedBrand = (Brand)cmbBrand.SelectedItem;
                    int brandID = selectedBrand.BrandID;
                    
                    if (brandID != -1) // Not "All"
                    {
                        products = products.Where(p => p.BrandID == brandID || p.BrandID == 0).ToList();
                    }
                }
                
                // Update the filtered products list for autocomplete
                _filteredProducts = products;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering products by brand: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateProductAutoComplete()
        {
            try
            {
                var productsToUse = _filteredProducts ?? _products;
                
                // Enable AutoComplete for the ProductName TextBox (supports name and barcode)
                txtProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtProductName.AutoCompleteSource = AutoCompleteSource.CustomSource;

                // Create AutoComplete collection
                AutoCompleteStringCollection productCollection = new AutoCompleteStringCollection();

                // Add filtered product names and barcodes to the collection
                if (productsToUse != null)
                {
                    foreach (var product in productsToUse)
                    {
                        if (!string.IsNullOrWhiteSpace(product.ProductName))
                            productCollection.Add(product.ProductName);
                        if (!string.IsNullOrWhiteSpace(product.Barcode))
                            productCollection.Add(product.Barcode);
                    }
                }

                // Set the custom source
                txtProductName.AutoCompleteCustomSource = productCollection;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating product autocomplete: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dataGridView1.BeginEdit(true);
            }
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // Remove entire row on Delete key
            if (e.KeyCode == Keys.Delete)
            {
                int rowIndex = -1;
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    rowIndex = dataGridView1.SelectedRows[0].Index;
                }
                else if (dataGridView1.CurrentCell != null)
                {
                    rowIndex = dataGridView1.CurrentCell.RowIndex;
                }

                if (rowIndex >= 0 && rowIndex < saleItems.Count)
                {
                    saleItems.RemoveAt(rowIndex);
                    RefreshCartDisplay();
                    CalculateTotals();
                    e.Handled = true;
                }
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < saleItems.Count)
            {
                try
                {
                    var item = saleItems[e.RowIndex];
                    
                    // Handle Product Name changes (column index 1)
                    if (e.ColumnIndex == 1) // Product Name column
                    {
                        var newProductName = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(newProductName))
                        {
                            // Find the product by name
                            var productsToSearch = _filteredProducts ?? _products ?? new List<Product>();
                            var product = productsToSearch.FirstOrDefault(p => 
                                p != null &&
                                (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Equals(newProductName, StringComparison.OrdinalIgnoreCase)));
                            
                            if (product != null)
                            {
                                // Update the item with new product details
                                item.ProductID = product.ProductID;
                                item.ProductName = product.ProductName;
                                item.ProductCode = product.ProductCode;
                                item.UnitPrice = product.RetailPrice;
                                item.SubTotal = item.Quantity * item.UnitPrice;
                                
                                // Update the grid display
                                dataGridView1.Rows[e.RowIndex].Cells[3].Value = item.UnitPrice; // Price column
                                dataGridView1.Rows[e.RowIndex].Cells[4].Value = item.SubTotal; // SubTotal column
                                
                                CalculateTotals();
                            }
                            else
                            {
                                ShowMessage($"Product '{newProductName}' not found. Please check the name.", "Product Not Found", MessageBoxIcon.Warning);
                                // Revert to original product name
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = item.ProductName;
                            }
                        }
                    }
                    // Handle Quantity changes (column index 2)
                    else if (e.ColumnIndex == 2) // Quantity column
                    {
                        var newQuantityText = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                        if (int.TryParse(newQuantityText, out int newQuantity) && newQuantity > 0)
                        {
                            var product = _products.FirstOrDefault(p => p.ProductID == item.ProductID);
                            
                            if (product != null)
                            {
                                // Check stock availability
                                if (newQuantity > product.StockQuantity)
                                {
                                    ShowMessage($"Only {product.StockQuantity} units available in stock. Cannot set quantity to {newQuantity}.", "Stock Error", MessageBoxIcon.Warning);
                                    // Revert to original quantity
                                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = item.Quantity;
                                    return;
                                }
                                
                                // Update quantity and subtotal
                                item.Quantity = newQuantity;
                                item.SubTotal = newQuantity * item.UnitPrice;
                                
                                // Update subtotal in grid
                                dataGridView1.Rows[e.RowIndex].Cells[4].Value = item.SubTotal; // SubTotal column
                                
                                // Recalculate totals
                                CalculateTotals();
                            }
                        }
                        else
                        {
                            ShowMessage("Please enter a valid quantity (greater than 0).", "Validation Error", MessageBoxIcon.Warning);
                            // Revert to original quantity
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = item.Quantity;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error updating item: {ex.Message}", "Error", MessageBoxIcon.Error);
                }
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the grid is refreshed after editing
            if (e.RowIndex >= 0 && e.RowIndex < saleItems.Count)
            {
                // Avoid re-entrant grid refresh during edit end to prevent SetCurrentCellAddressCore errors
                // Just recalculate totals; the grid will reflect edits without a full redraw
                CalculateTotals();
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Provide visual feedback for editable cells
            if (e.RowIndex >= 0 && e.RowIndex < saleItems.Count)
            {
                // Highlight editable columns (Product Name and Quantity)
                if (e.ColumnIndex == 1 || e.ColumnIndex == 2) // Product Name or Quantity columns
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView1.BeginEdit(true);
                }
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            AddItem();
        }



        // Event handler for form activation - refreshes stock quantities
        private void NewSale_Activated(object sender, EventArgs e)
        {
            try
            {
                // Refresh stock quantities when form becomes active
                // This ensures we have the latest stock information from SQL Express
                RefreshStockQuantities();
                System.Diagnostics.Debug.WriteLine("Sales form activated - stock quantities refreshed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing stock on form activation: {ex.Message}");
            }
        }

        // Event handler for product updates
        private void OnProductsUpdated(object sender, EventArgs e)
        {
            try
            {
                // Refresh products and autocomplete on UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => {
                        LoadProducts();
                        System.Diagnostics.Debug.WriteLine("Products automatically refreshed due to update");
                    }));
                }
                else
                {
                    LoadProducts();
                    System.Diagnostics.Debug.WriteLine("Products automatically refreshed due to update");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing products: {ex.Message}");
            }
        }

        // Form closing event handler
        private void NewSale_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Cleanup searchable ComboBox helpers
                SearchableComboBoxHelper.Cleanup(cmbCustomer);
                SearchableComboBoxHelper.Cleanup(cmbCategory);
                SearchableComboBoxHelper.Cleanup(cmbBrand);
                SearchableComboBoxHelper.Cleanup(cmbPaymentMethod);
                
                // Unsubscribe from product update events
                ProductRepository.ProductsUpdated -= OnProductsUpdated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FormClosing: {ex.Message}");
            }
        }
        
        private void TxtProductName_Leave(object sender, EventArgs e)
        {
            try
            {
                string productName = txtProductName.Text.Trim();
                
                if (!string.IsNullOrEmpty(productName))
                {
                    // Find the product by name, code, or barcode
                    var productsToSearch = _filteredProducts ?? _products ?? new List<Product>();
                    string searchText = productName;
                    var product = productsToSearch.FirstOrDefault(p => 
                        p != null && (
                            (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.Equals(searchText, StringComparison.OrdinalIgnoreCase))
                        ));

                    if (product != null)
                    {
                        // Populate product details
                        txtStockQuantity.Text = product.StockQuantity.ToString();
                        txtReorderLevel.Text = product.ReorderLevel.ToString();
                    }
                    else
                    {
                        // Product not found, clear fields
                        txtStockQuantity.Clear();
                        txtReorderLevel.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading product details: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtTaxPercent_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingDiscount) return;
            
            try
            {
                _isUpdatingDiscount = true;
                
                // Calculate discount amount from percentage
                if (decimal.TryParse(txtTaxPercent.Text, out decimal discountPercent) && subtotal > 0)
                {
                    decimal calculatedAmount = subtotal * (discountPercent / 100);
                    
                    // Update discount amount textbox without triggering its event
                    if (txtDiscountAmount != null)
                    {
                        txtDiscountAmount.TextChanged -= TxtDiscountAmount_TextChanged;
                        txtDiscountAmount.Text = calculatedAmount.ToString("F2");
                        txtDiscountAmount.TextChanged += TxtDiscountAmount_TextChanged;
                    }
                }
                else if (string.IsNullOrWhiteSpace(txtTaxPercent.Text))
                {
                    // Clear discount amount if percentage is cleared
                    if (txtDiscountAmount != null)
                    {
                        txtDiscountAmount.TextChanged -= TxtDiscountAmount_TextChanged;
                        txtDiscountAmount.Text = "";
                        txtDiscountAmount.TextChanged += TxtDiscountAmount_TextChanged;
                    }
                }
                
            CalculateTotals();
            }
            finally
            {
                _isUpdatingDiscount = false;
            }
        }
        
        private void TxtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingDiscount) return;
            
            try
            {
                _isUpdatingDiscount = true;
                
                // Calculate discount percentage from amount
                if (decimal.TryParse(txtDiscountAmount.Text, out decimal discountAmount) && subtotal > 0)
                {
                    decimal calculatedPercent = (discountAmount / subtotal) * 100;
                    
                    // Update discount percentage textbox without triggering its event
                    if (txtTaxPercent != null)
                    {
                        txtTaxPercent.TextChanged -= TxtTaxPercent_TextChanged;
                        txtTaxPercent.Text = calculatedPercent.ToString("F2");
                        txtTaxPercent.TextChanged += TxtTaxPercent_TextChanged;
                    }
                }
                else if (string.IsNullOrWhiteSpace(txtDiscountAmount.Text))
                {
                    // Clear discount percentage if amount is cleared
                    if (txtTaxPercent != null)
                    {
                        txtTaxPercent.TextChanged -= TxtTaxPercent_TextChanged;
                        txtTaxPercent.Text = "";
                        txtTaxPercent.TextChanged += TxtTaxPercent_TextChanged;
                    }
                }
                
                CalculateTotals();
            }
            finally
            {
                _isUpdatingDiscount = false;
            }
        }

        private void TxtTaxPercentInput_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingTax) return;
            
            try
            {
                _isUpdatingTax = true;
                
                // Calculate tax amount from percentage
                decimal taxableAmount = subtotal - discount;
                if (decimal.TryParse(txtTaxPercentInput.Text, out decimal taxPercent) && taxableAmount > 0)
                {
                    decimal calculatedAmount = taxableAmount * (taxPercent / 100);
                    
                    // Update tax amount textbox without triggering its event
                    if (txtTaxAmountInput != null)
                    {
                        txtTaxAmountInput.TextChanged -= TxtTaxAmountInput_TextChanged;
                        txtTaxAmountInput.Text = calculatedAmount.ToString("F2");
                        txtTaxAmountInput.TextChanged += TxtTaxAmountInput_TextChanged;
                    }
                }
                else if (string.IsNullOrWhiteSpace(txtTaxPercentInput.Text))
                {
                    // Clear tax amount if percentage is cleared
                    if (txtTaxAmountInput != null)
                    {
                        txtTaxAmountInput.TextChanged -= TxtTaxAmountInput_TextChanged;
                        txtTaxAmountInput.Text = "";
                        txtTaxAmountInput.TextChanged += TxtTaxAmountInput_TextChanged;
                    }
                }
                
                CalculateTotals();
            }
            finally
            {
                _isUpdatingTax = false;
            }
        }
        
        private void TxtTaxAmountInput_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingTax) return;
            
            try
            {
                _isUpdatingTax = true;
                
                // Calculate tax percentage from amount
                decimal taxableAmount = subtotal - discount;
                if (decimal.TryParse(txtTaxAmountInput.Text, out decimal taxAmount) && taxableAmount > 0)
                {
                    decimal calculatedPercent = (taxAmount / taxableAmount) * 100;
                    
                    // Update tax percentage textbox without triggering its event
                    if (txtTaxPercentInput != null)
                    {
                        txtTaxPercentInput.TextChanged -= TxtTaxPercentInput_TextChanged;
                        txtTaxPercentInput.Text = calculatedPercent.ToString("F2");
                        txtTaxPercentInput.TextChanged += TxtTaxPercentInput_TextChanged;
                    }
                }
                else if (string.IsNullOrWhiteSpace(txtTaxAmountInput.Text))
                {
                    // Clear tax percentage if amount is cleared
                    if (txtTaxPercentInput != null)
                    {
                        txtTaxPercentInput.TextChanged -= TxtTaxPercentInput_TextChanged;
                        txtTaxPercentInput.Text = "";
                        txtTaxPercentInput.TextChanged += TxtTaxPercentInput_TextChanged;
                    }
                }
                
                CalculateTotals();
            }
            finally
            {
                _isUpdatingTax = false;
            }
        }

        private void TxtPaid_TextChanged(object sender, EventArgs e)
        {
            CalculateChange();
        }

        private void CalculateChange()
        {
            try
            {
                if (decimal.TryParse(txtPaid.Text, out decimal paidAmount))
                {
                    decimal totalAmount = decimal.Parse(txtTotal.Text);
                    decimal change = paidAmount - totalAmount;
                    txtChange.Text = change.ToString("F2");
                }
                else
                {
                    txtChange.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating change: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void NewSale_Load(object sender, EventArgs e)
        {
            // Set current date
            guna2DateTimePicker1.Value = DateTime.Now;
            
            // Load data
            LoadCustomers();
            LoadCategories();
            LoadBrands();
            LoadProducts();

            // Set default values
            if (cmbTax != null && cmbTax.Items != null && cmbTax.Items.Count > 0)
            {
                cmbTax.SelectedIndex = 0; // Backward-compat if dropdown still exists with items
            }
            txtQuantity.Text = "1";

            // Initialize sale items list
            saleItems = new List<Models.SaleItem>();

            // Set up AutoComplete for Product Name
            UpdateProductAutoComplete();

            // Payment method change event handler (not in SetupEventHandlers)
            cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;
            
            // Focus on product name field
            txtProductName.Focus();
        }

        private void label14_Click(object sender, EventArgs e)
        {
            // Total label click - no action needed
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbPaymentMethod.SelectedItem != null)
                {
                    string selectedPaymentMethod = cmbPaymentMethod.SelectedItem.ToString();
                    
                    if (selectedPaymentMethod.Equals("Card", StringComparison.OrdinalIgnoreCase))
                    {
                        // For card payments, automatically set paid amount to total
                        if (decimal.TryParse(txtTotal.Text, out decimal totalAmount))
                        {
                            txtPaid.Text = totalAmount.ToString("F2");
                            CalculateChange(); // Update change calculation
                        }
                    }
                    else if (selectedPaymentMethod.Equals("Credit", StringComparison.OrdinalIgnoreCase))
                    {
                        // For credit sales, default paid amount to 0 (user can enter partial payments)
                        if (string.IsNullOrWhiteSpace(txtPaid.Text))
                        {
                            txtPaid.Text = "0.00";
                        }
                        CalculateChange();
                    }
                    else
                    {
                        // For cash and other payment methods, clear paid amount for manual entry
                        if (string.IsNullOrEmpty(txtPaid.Text) || txtPaid.Text == "0.00")
                        {
                            txtPaid.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling payment method change: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
    }

}