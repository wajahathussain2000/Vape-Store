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

namespace Vape_Store
{
    public partial class NewSale : Form
    {
        private SalesService _salesService;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private InventoryService _inventoryService;
        private List<Models.SaleItem> saleItems;
        private List<Customer> _customers;
        private List<Product> _products;
        private List<Category> _categories;
        private List<Brand> _brands;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        
        private decimal subtotal = 0;
        private decimal discount = 0;
        private decimal tax = 0;
        private decimal total = 0;
        private decimal paidAmount = 0;
        private decimal changeAmount = 0;
        private string invoiceNumber = "";
        private int currentUserID = 1; // Default user ID, should be from UserSession

        public NewSale()
        {
            InitializeComponent();
            _salesService = new SalesService();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            saleItems = new List<Models.SaleItem>();
            
            InitializeDataGridView();
            SetupEventHandlers();
            LoadCustomers();
            LoadProducts();
            LoadCategories();
            LoadBrands();
            GenerateInvoiceNumber();
            InitializePaymentMethods();
            InitializeTaxOptions();
        }

        private void InitializeDataGridView()
        {
            // Configure DataGridView for cart items
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            
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
            
            // Format currency columns
            Price.DefaultCellStyle.Format = "C2";
            SubTotal.DefaultCellStyle.Format = "C2";
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
            
            // Payment calculation event handlers
            txtPaid.TextChanged += TxtPaid_TextChanged;
            txtTaxPercent.TextChanged += TxtTaxPercent_TextChanged;
            cmbTax.SelectedIndexChanged += CmbTax_SelectedIndexChanged;
            
            // DataGridView event handlers
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            
            // Add Item button event handler
            btnAddItem.Click += BtnAddItem_Click;
            
            // Product search functionality
            txtProductName.TextChanged += TxtProductName_TextChanged;
            txtProductName.Leave += TxtProductName_Leave;
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                cmbCustomer.DataSource = _customers;
                cmbCustomer.DisplayMember = "CustomerName";
                cmbCustomer.ValueMember = "CustomerID";
                cmbCustomer.SelectedIndex = -1; // No default selection
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
                _products = _productRepository.GetAllProducts();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
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
        
        private void GenerateInvoiceNumber()
        {
            try
            {
                invoiceNumber = _salesService.GetNextInvoiceNumber();
                label4.Text = invoiceNumber;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating invoice number: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void InitializePaymentMethods()
        {
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.Items.AddRange(new string[] { "Cash", "Card", "Bank Transfer", "Check", "Digital Wallet" });
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
                    ShowMessage("Please enter a product name.", "Validation Error", MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }

                // Validate quantity
                if (string.IsNullOrWhiteSpace(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int requestedQuantity) || requestedQuantity <= 0)
                {
                    ShowMessage("Please enter a valid quantity (greater than 0).", "Validation Error", MessageBoxIcon.Warning);
                    txtQuantity.Focus();
                    return;
                }

                // Find product by name
                var product = _products.FirstOrDefault(p => 
                    p.ProductName.ToLower().Contains(txtProductName.Text.ToLower()) ||
                    p.ProductCode.ToLower().Contains(txtProductName.Text.ToLower()));

                if (product == null)
                {
                    ShowMessage("Product not found. Please check the name.", "Error", MessageBoxIcon.Warning);
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
                subtotal = saleItems.Sum(item => item.SubTotal);
                txtsubTotal.Text = subtotal.ToString("F2");

                // Calculate discount (if any)
                if (decimal.TryParse(txtTaxPercent.Text, out decimal discountPercent))
                {
                    discount = subtotal * (discountPercent / 100);
                    // Update discount amount field immediately
                    txtTax.Text = discount.ToString("F2");
                }
                else
                {
                    discount = 0;
                    txtTax.Text = "0.00";
                }

                // Calculate tax
                if (cmbTax.SelectedItem != null)
                {
                    string taxText = cmbTax.SelectedItem.ToString().Replace("%", "");
                    if (decimal.TryParse(taxText, out decimal taxPercent))
                    {
                        tax = (subtotal - discount) * (taxPercent / 100);
                        // Update tax field with calculated tax amount
                        txtTax.Text = tax.ToString("F2");
                    }
                }

                // Calculate total
                total = subtotal - discount + tax;
                txtTotal.Text = total.ToString("F2");

                // Calculate change
                CalculateChange();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating totals: {ex.Message}", "Error", MessageBoxIcon.Error);
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

                // Get current paid amount from text field
                if (!decimal.TryParse(txtPaid.Text, out decimal currentPaidAmount))
                {
                    ShowMessage("Please enter a valid paid amount.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (currentPaidAmount < total)
                {
                    ShowMessage($"Paid amount (${currentPaidAmount:F2}) is less than total amount (${total:F2}).", "Payment Error", MessageBoxIcon.Warning);
                    return;
                }

                // Create sale object
                var sale = new Sale
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerID = cmbCustomer.SelectedItem != null ? ((Customer)cmbCustomer.SelectedItem).CustomerID : 0,
                    SaleDate = guna2DateTimePicker1.Value,
                    SubTotal = subtotal,
                    TaxAmount = tax,
                    TaxPercent = decimal.Parse(cmbTax.SelectedItem.ToString().Replace("%", "")),
                    TotalAmount = total,
                    PaymentMethod = cmbPaymentMethod.Text,
                    PaidAmount = currentPaidAmount,
                    ChangeAmount = currentPaidAmount - total,
                    UserID = currentUserID,
                    SaleItems = saleItems
                };

                // Process sale
                bool success = _salesService.ProcessSale(sale);
                
                if (success)
                {
                    ShowMessage("Sale completed successfully!", "Success", MessageBoxIcon.Information);
                    
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
                txtTax.Clear();
                txtTotal.Clear();
                txtPaid.Clear();
                txtChange.Clear();
                txtProductName.Clear();
                txtStockQuantity.Clear();
                txtReorderLevel.Clear();
                
                // Reset selections
                cmbCustomer.SelectedIndex = -1;
                cmbCategory.SelectedIndex = -1;
                cmbBrand.SelectedIndex = -1;
                cmbPaymentMethod.SelectedIndex = 0;
                cmbTax.SelectedIndex = 2;
                
                // Generate new invoice number
                GenerateInvoiceNumber();
                
                // Set current date
                guna2DateTimePicker1.Value = DateTime.Now;
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
                // Create sale object for receipt
                var sale = new Sale
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerID = cmbCustomer.SelectedItem != null ? ((Customer)cmbCustomer.SelectedItem).CustomerID : 0,
                    SaleDate = guna2DateTimePicker1.Value,
                    SubTotal = subtotal,
                    TaxAmount = tax,
                    TaxPercent = cmbTax.SelectedItem != null ? decimal.Parse(cmbTax.SelectedItem.ToString().Replace("%", "")) : 0,
                    TotalAmount = total,
                    PaymentMethod = cmbPaymentMethod.Text,
                    PaidAmount = decimal.Parse(txtPaid.Text),
                    ChangeAmount = decimal.Parse(txtPaid.Text) - total,
                    UserID = currentUserID,
                    SaleItems = saleItems
                };

                // Show receipt preview
                var receiptPreview = new ReceiptPreviewForm(sale);
                receiptPreview.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing receipt: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                var exactMatch = _products.FirstOrDefault(p => 
                    p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase));

                if (exactMatch != null)
                {
                    // Show exact match details
                    txtStockQuantity.Text = exactMatch.StockQuantity.ToString();
                    txtReorderLevel.Text = exactMatch.ReorderLevel.ToString();
                }
                else
                {
                    // Search for products that contain the search text
                    var matchingProducts = _products.Where(p => 
                        p.ProductName.ToLower().Contains(searchText.ToLower()) ||
                        p.ProductCode.ToLower().Contains(searchText.ToLower())
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
                    Category selectedCategory = (Category)cmbCategory.SelectedItem;
                    int categoryID = selectedCategory.CategoryID;
                    var filteredProducts = _products.Where(p => p.CategoryID == categoryID).ToList();
                    // Update product suggestions based on category
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error filtering products by category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Brand selectedBrand = (Brand)cmbBrand.SelectedItem;
                    int brandID = selectedBrand.BrandID;
                    var filteredProducts = _products.Where(p => p.BrandID == brandID).ToList();
                    // Update product suggestions based on brand
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error filtering products by brand: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void CmbTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Remove item on double click
            if (e.RowIndex >= 0 && e.RowIndex < saleItems.Count)
            {
                saleItems.RemoveAt(e.RowIndex);
                RefreshCartDisplay();
                CalculateTotals();
            }
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // Remove item on Delete key
            if (e.KeyCode == Keys.Delete && dataGridView1.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;
                if (selectedIndex >= 0 && selectedIndex < saleItems.Count)
                {
                    saleItems.RemoveAt(selectedIndex);
                    RefreshCartDisplay();
                    CalculateTotals();
                }
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Handle quantity changes in the DataGridView
            if (e.RowIndex >= 0 && e.RowIndex < saleItems.Count && e.ColumnIndex == 2) // Qty column
            {
                try
                {
                    var newQuantityText = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (int.TryParse(newQuantityText, out int newQuantity) && newQuantity > 0)
                    {
                        var item = saleItems[e.RowIndex];
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
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = saleItems[e.RowIndex].Quantity;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error updating quantity: {ex.Message}", "Error", MessageBoxIcon.Error);
                }
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the grid is refreshed after editing
            if (e.RowIndex >= 0 && e.RowIndex < saleItems.Count)
            {
                RefreshCartDisplay();
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            AddItem();
        }



        private void SetupProductAutoComplete()
        {
            try
            {
                // Enable AutoComplete for the ProductName TextBox
                txtProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtProductName.AutoCompleteSource = AutoCompleteSource.CustomSource;

                // Create AutoComplete collection
                AutoCompleteStringCollection productCollection = new AutoCompleteStringCollection();

                // Add all product names to the collection
                foreach (var product in _products)
                {
                    productCollection.Add(product.ProductName);
                }

                // Set the custom source
                txtProductName.AutoCompleteCustomSource = productCollection;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting up product autocomplete: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtProductName_Leave(object sender, EventArgs e)
        {
            try
            {
                string productName = txtProductName.Text.Trim();
                
                if (!string.IsNullOrEmpty(productName))
                {
                    // Find the product by name
                    var product = _products.FirstOrDefault(p => 
                        p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));

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
            CalculateTotals();
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
            cmbTax.SelectedIndex = 0; // Default to first tax rate
            txtQuantity.Text = "1";

            // Initialize sale items list
            saleItems = new List<Models.SaleItem>();

            // Set up AutoComplete for Product Name
            SetupProductAutoComplete();

            // Set up event handlers
            cmbTax.SelectedIndexChanged += CmbTax_SelectedIndexChanged;
            
            // DataGridView event handlers
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            
            // Add Item button event handler
            btnAddItem.Click += BtnAddItem_Click;
            
            // Product search functionality
            txtProductName.TextChanged += TxtProductName_TextChanged;
            txtProductName.Leave += TxtProductName_Leave;
            
            
            // Focus on product name field
            txtProductName.Focus();
        }

        private void label14_Click(object sender, EventArgs e)
        {
            // Total label click - no action needed
        }
    }

}