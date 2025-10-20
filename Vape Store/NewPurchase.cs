using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Models;

namespace Vape_Store
{
    public partial class NewPurchase : Form
    {
        private PurchaseRepository _purchaseRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private InventoryService _inventoryService;
        
        private List<Models.PurchaseItem> purchaseItems;
        private List<Supplier> _suppliers;
        private List<Product> _products;
        private List<Category> _categories;
        private List<Brand> _brands;
        
        private decimal subtotal = 0;
        private decimal discount = 0;
        private decimal tax = 0;
        private decimal total = 0;
        private decimal paidAmount = 0;
        private decimal changeAmount = 0;
        private string invoiceNumber = "";
        private int currentUserID = 1; // Default user ID, should be from UserSession

        public NewPurchase()
        {
            InitializeComponent();
            _purchaseRepository = new PurchaseRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            purchaseItems = new List<Models.PurchaseItem>();
            
            InitializeDataGridView();
            SetupEventHandlers();
            LoadSuppliers();
            LoadProducts();
            LoadCategories();
            LoadBrands();
            GenerateInvoiceNumber();
            InitializePaymentMethods();
            InitializeTaxOptions();
        }

        private void InitializeDataGridView()
        {
            // Configure DataGridView for purchase items
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            
            // Configure columns
            SrNo.Width = 80;
            ItemName.Width = 300;
            Qty.Width = 100;
            Bonus.Width = 100;
            Price.Width = 120;
            SubTotal.Width = 120;
            
            // Set column headers
            SrNo.HeaderText = "Sr.No";
            ItemName.HeaderText = "Product Name";
            Qty.HeaderText = "Quantity";
            Bonus.HeaderText = "Bonus";
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
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                cmbSupplier.DataSource = _suppliers;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
                cmbSupplier.SelectedIndex = -1; // No default selection
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                invoiceNumber = _purchaseRepository.GetNextInvoiceNumber();
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
            cmbPaymentMethod.Items.AddRange(new string[] { "Cash", "Bank Transfer", "Check", "Credit", "Other" });
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

                // Check if product already exists in cart
                var existingItem = purchaseItems.FirstOrDefault(item => item.ProductID == product.ProductID);
                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += 1;
                    existingItem.SubTotal = existingItem.Quantity * existingItem.UnitPrice;
                }
                else
                {
                    // Add new item to cart
                    var purchaseItem = new Models.PurchaseItem
                    {
                        ProductID = product.ProductID,
                        Quantity = 1,
                        UnitPrice = product.CostPrice, // Use cost price for purchases
                        SubTotal = product.CostPrice,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        Bonus = 0 // Default bonus
                    };
                    purchaseItems.Add(purchaseItem);
                }

                // Update stock display
                txtStockQuantity.Text = product.StockQuantity.ToString();

                // Refresh cart display
                RefreshCartDisplay();
                CalculateTotals();
                
                // Clear product search
                txtProductName.Clear();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding item: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshCartDisplay()
        {
            dataGridView1.Rows.Clear();
            
            for (int i = 0; i < purchaseItems.Count; i++)
            {
                var item = purchaseItems[i];
                dataGridView1.Rows.Add(
                    i + 1,
                    item.ProductName,
                    item.Quantity,
                    item.Bonus,
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
                subtotal = purchaseItems.Sum(item => item.SubTotal);
                txtsubTotal.Text = subtotal.ToString("F2");

                // Calculate discount (if any)
                if (decimal.TryParse(txtTaxPercent.Text, out decimal discountPercent))
                {
                    discount = subtotal * (discountPercent / 100);
                }
                else
                {
                    discount = 0;
                }

                // Calculate tax
                if (cmbTax.SelectedItem != null)
                {
                    string taxText = cmbTax.SelectedItem.ToString().Replace("%", "");
                    if (decimal.TryParse(taxText, out decimal taxPercent))
                    {
                        tax = (subtotal - discount) * (taxPercent / 100);
                        txtTax.Text = tax.ToString("F2");
                    }
                }

                // Calculate total
                total = subtotal - discount + tax;
                txtTotal.Text = total.ToString("F2");

                // Calculate change if paid amount is entered
                if (decimal.TryParse(txtPaid.Text, out paidAmount))
                {
                    changeAmount = paidAmount - total;
                    txtChange.Text = changeAmount.ToString("F2");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating totals: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SavePurchase();
        }

        private void SavePurchase()
        {
            try
            {
                // Validate cart
                if (purchaseItems.Count == 0)
                {
                    ShowMessage("Please add at least one item to the cart.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Validate supplier
                if (cmbCustomer.SelectedValue == null)
                {
                    ShowMessage("Please select a supplier.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Validate payment
                if (string.IsNullOrEmpty(cmbPaymentMethod.Text))
                {
                    ShowMessage("Please select a payment method.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (paidAmount < total)
                {
                    ShowMessage($"Paid amount (${paidAmount:F2}) is less than total amount (${total:F2}).", "Payment Error", MessageBoxIcon.Warning);
                    return;
                }

                // Create purchase object
                var purchase = new Purchase
                {
                    InvoiceNumber = invoiceNumber,
                    SupplierID = ((Supplier)cmbCustomer.SelectedItem).SupplierID,
                    PurchaseDate = guna2DateTimePicker1.Value,
                    SubTotal = subtotal,
                    TaxAmount = tax,
                    TaxPercent = decimal.Parse(cmbTax.SelectedItem.ToString().Replace("%", "")),
                    TotalAmount = total,
                    PaymentMethod = cmbPaymentMethod.Text,
                    PaidAmount = paidAmount,
                    ChangeAmount = changeAmount,
                    UserID = currentUserID,
                    PurchaseItems = purchaseItems
                };

                // Process purchase
                bool success = _purchaseRepository.ProcessPurchase(purchase, purchaseItems);
                
                if (success)
                {
                    ShowMessage("Purchase completed successfully!", "Success", MessageBoxIcon.Information);
                    
                    // Clear form for next purchase
                    ClearForm();
                }
                else
                {
                    ShowMessage("Failed to process purchase. Please try again.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving purchase: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                purchaseItems.Clear();
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
            LoadSuppliers();
            LoadCategories();
            LoadBrands();
            ShowMessage("Data refreshed successfully.", "Info", MessageBoxIcon.Information);
        }

        // Event Handlers
        private void TxtProductName_TextChanged(object sender, EventArgs e)
        {
            // Auto-suggest products as user types
            if (!string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                var filteredProducts = _products.Where(p => 
                    p.ProductName.ToLower().Contains(txtProductName.Text.ToLower()) ||
                    p.ProductCode.ToLower().Contains(txtProductName.Text.ToLower())).ToList();

                if (filteredProducts.Count > 0)
                {
                    var product = filteredProducts.First();
                    txtStockQuantity.Text = product.StockQuantity.ToString();
                }
            }
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filter products by category
            if (cmbCategory.SelectedValue != null)
            {
                int categoryID = ((Category)cmbCategory.SelectedItem).CategoryID;
                var filteredProducts = _products.Where(p => p.CategoryID == categoryID).ToList();
                // Update product suggestions based on category
            }
        }

        private void CmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filter products by brand
            if (cmbBrand.SelectedValue != null)
            {
                int brandID = ((Brand)cmbBrand.SelectedItem).BrandID;
                var filteredProducts = _products.Where(p => p.BrandID == brandID).ToList();
                // Update product suggestions based on brand
            }
        }

        private void TxtPaid_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void TxtTaxPercent_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void CmbTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Remove item on double click
            if (e.RowIndex >= 0 && e.RowIndex < purchaseItems.Count)
            {
                purchaseItems.RemoveAt(e.RowIndex);
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
                if (selectedIndex >= 0 && selectedIndex < purchaseItems.Count)
                {
                    purchaseItems.RemoveAt(selectedIndex);
                    RefreshCartDisplay();
                    CalculateTotals();
                }
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void NewPurchase_Load(object sender, EventArgs e)
        {
            // Set current date
            guna2DateTimePicker1.Value = DateTime.Now;
            
            // Focus on product name field
            txtProductName.Focus();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Panel paint event - no action needed
        }
    }
}
