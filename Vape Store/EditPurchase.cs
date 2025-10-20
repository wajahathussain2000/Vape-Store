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
    public partial class EditPurchase : Form
    {
        private PurchaseRepository _purchaseRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private InventoryService _inventoryService;
        
        private List<Supplier> _suppliers;
        private List<Product> _products;
        private List<Category> _categories;
        private List<Brand> _brands;
        private Purchase _currentPurchase;
        private List<PurchaseItem> _purchaseItems;
        
        private bool isEditMode = false;
        private int selectedPurchaseId = -1;

        public EditPurchase()
        {
            InitializeComponent();
            _purchaseRepository = new PurchaseRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _inventoryService = new InventoryService();
            
            _purchaseItems = new List<PurchaseItem>();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadSuppliers();
            LoadProducts();
            LoadCategories();
            LoadBrands();
            InitializePaymentMethods();
            InitializeTaxOptions();
        }

        private void InitializeDataGridView()
        {
            // Configure DataGridView columns
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = false;
            
            // Clear existing columns
            dataGridView1.Columns.Clear();
            
            // Add columns
            dataGridView1.Columns.Add("SrNo", "Sr.No");
            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("Qty", "Qty");
            dataGridView1.Columns.Add("Bonus", "Bonus");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("SubTotal", "SubTotal");
            
            // Configure column properties
            dataGridView1.Columns["SrNo"].Width = 80;
            dataGridView1.Columns["ItemName"].Width = 300;
            dataGridView1.Columns["Qty"].Width = 100;
            dataGridView1.Columns["Bonus"].Width = 100;
            dataGridView1.Columns["Price"].Width = 120;
            dataGridView1.Columns["SubTotal"].Width = 120;
            
            // Format currency columns
            dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["SubTotal"].DefaultCellStyle.Format = "C2";
            
            // Make Qty, Bonus, and Price columns editable
            dataGridView1.Columns["Qty"].ReadOnly = false;
            dataGridView1.Columns["Bonus"].ReadOnly = false;
            dataGridView1.Columns["Price"].ReadOnly = false;
            dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView1.Columns["Bonus"].DefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView1.Columns["Price"].DefaultCellStyle.BackColor = Color.LightYellow;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnGetData.Click += BtnGetData_Click;
            NewItemBtn.Click += NewItemBtn_Click;
            saveBtn.Click += SaveBtn_Click;
            CancelBtn.Click += CancelBtn_Click;
            ClearBtn.Click += ClearBtn_Click;
            RefreshBtn.Click += RefreshBtn_Click;
            
            // DataGridView event handlers
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            
            // ComboBox event handlers
            cmbCustomer.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            cmbBrand.SelectedIndexChanged += CmbBrand_SelectedIndexChanged;
            cmbTax.SelectedIndexChanged += CmbTax_SelectedIndexChanged;
            
            // TextBox event handlers
            txtProductName.TextChanged += TxtProductName_TextChanged;
            txtTaxPercent.TextChanged += TxtTaxPercent_TextChanged;
            txtPaid.TextChanged += TxtPaid_TextChanged;
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                cmbSupplier.DataSource = _suppliers;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
                cmbSupplier.SelectedIndex = -1;
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
                cmbCategory.SelectedIndex = -1;
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
                cmbBrand.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading brands: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void InitializePaymentMethods()
        {
            try
            {
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("Credit Card");
                cmbPaymentMethod.Items.Add("Bank Transfer");
                cmbPaymentMethod.Items.Add("Check");
                cmbPaymentMethod.Items.Add("Other");
                cmbPaymentMethod.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing payment methods: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void InitializeTaxOptions()
        {
            try
            {
                cmbTax.Items.Clear();
                cmbTax.Items.Add("0%");
                cmbTax.Items.Add("5%");
                cmbTax.Items.Add("10%");
                cmbTax.Items.Add("15%");
                cmbTax.Items.Add("20%");
                cmbTax.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing tax options: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnGetData_Click(object sender, EventArgs e)
        {
            LoadPurchaseData();
        }

        private void LoadPurchaseData()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtinvoiceNo.Text))
                {
                    ShowMessage("Please enter a purchase invoice number.", "Validation Error", MessageBoxIcon.Warning);
                    txtinvoiceNo.Focus();
                    return;
                }

                string invoiceNumber = txtinvoiceNo.Text.Trim();
                _currentPurchase = _purchaseRepository.GetPurchaseWithItemsByInvoice(invoiceNumber);

                if (_currentPurchase == null)
                {
                    ShowMessage("Purchase not found with the given invoice number.", "Not Found", MessageBoxIcon.Warning);
                    return;
                }

                // Populate form with purchase data
                PopulatePurchaseData();
                ShowMessage("Purchase data loaded successfully!", "Success", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading purchase data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void PopulatePurchaseData()
        {
            try
            {
                if (_currentPurchase == null) return;

                // Populate basic purchase information
                guna2DateTimePicker1.Value = _currentPurchase.PurchaseDate;
                cmbCustomer.SelectedValue = _currentPurchase.SupplierID;
                cmbPaymentMethod.Text = _currentPurchase.PaymentMethod ?? "Cash";
                
                // Populate purchase items
                _purchaseItems.Clear();
                dataGridView1.Rows.Clear();
                
                for (int i = 0; i < _currentPurchase.PurchaseItems.Count; i++)
                {
                    var item = _currentPurchase.PurchaseItems[i];
                    _purchaseItems.Add(new PurchaseItem
                    {
                        PurchaseItemID = item.PurchaseItemID,
                        PurchaseID = item.PurchaseID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Bonus = item.Bonus,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.SubTotal,
                        ProductName = item.ProductName,
                        ProductCode = item.ProductCode
                    });

                    // Add row to DataGridView
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["SrNo"].Value = i + 1;
                    dataGridView1.Rows[rowIndex].Cells["ItemName"].Value = item.ProductName;
                    dataGridView1.Rows[rowIndex].Cells["Qty"].Value = item.Quantity;
                    dataGridView1.Rows[rowIndex].Cells["Bonus"].Value = item.Bonus;
                    dataGridView1.Rows[rowIndex].Cells["Price"].Value = item.UnitPrice;
                    dataGridView1.Rows[rowIndex].Cells["SubTotal"].Value = item.SubTotal;
                }

                // Populate totals
                txtsubTotal.Text = _currentPurchase.SubTotal.ToString("F2");
                txtTaxPercent.Text = _currentPurchase.TaxPercent.ToString("F2");
                txtTax.Text = _currentPurchase.TaxAmount.ToString("F2");
                txtTotal.Text = _currentPurchase.TotalAmount.ToString("F2");
                txtPaid.Text = _currentPurchase.PaidAmount.ToString("F2");
                txtChange.Text = _currentPurchase.ChangeAmount.ToString("F2");

                // Set tax combo box
                string taxText = _currentPurchase.TaxPercent.ToString("F0") + "%";
                for (int i = 0; i < cmbTax.Items.Count; i++)
                {
                    if (cmbTax.Items[i].ToString() == taxText)
                    {
                        cmbTax.SelectedIndex = i;
                        break;
                    }
                }

                selectedPurchaseId = _currentPurchase.PurchaseID;
                isEditMode = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error populating purchase data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void NewItemBtn_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void AddNewItem()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    ShowMessage("Please enter a product name.", "Validation Error", MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }

                // Find product by name
                var product = _products.FirstOrDefault(p => 
                    p.ProductName.ToLower().Contains(txtProductName.Text.ToLower()));

                if (product == null)
                {
                    ShowMessage("Product not found.", "Not Found", MessageBoxIcon.Warning);
                    return;
                }

                // Check if product already exists in the purchase
                var existingItem = _purchaseItems.FirstOrDefault(item => item.ProductID == product.ProductID);
                if (existingItem != null)
                {
                    ShowMessage("Product already exists in the purchase. Please update quantity instead.", "Duplicate", MessageBoxIcon.Warning);
                    return;
                }

                // Add new item
                var purchaseItem = new PurchaseItem
                {
                    PurchaseID = selectedPurchaseId,
                    ProductID = product.ProductID,
                    Quantity = 1, // Default quantity
                    Bonus = 0, // Default bonus
                    UnitPrice = product.PurchasePrice, // Use purchase price for purchases
                    SubTotal = product.PurchasePrice,
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode
                };

                _purchaseItems.Add(purchaseItem);

                // Add row to DataGridView
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells["SrNo"].Value = _purchaseItems.Count;
                dataGridView1.Rows[rowIndex].Cells["ItemName"].Value = product.ProductName;
                dataGridView1.Rows[rowIndex].Cells["Qty"].Value = 1;
                dataGridView1.Rows[rowIndex].Cells["Bonus"].Value = 0;
                dataGridView1.Rows[rowIndex].Cells["Price"].Value = product.PurchasePrice;
                dataGridView1.Rows[rowIndex].Cells["SubTotal"].Value = product.PurchasePrice;

                // Update stock display
                txtStockQuantity.Text = product.StockQuantity.ToString();

                // Clear product search
                txtProductName.Clear();
                cmbCategory.SelectedIndex = -1;
                cmbBrand.SelectedIndex = -1;

                // Calculate totals
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding item: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    if (e.ColumnIndex == dataGridView1.Columns["Qty"].Index || 
                        e.ColumnIndex == dataGridView1.Columns["Bonus"].Index ||
                        e.ColumnIndex == dataGridView1.Columns["Price"].Index)
                    {
                        // Update quantity, bonus, and price
                        int qty = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Qty"].Value ?? 0);
                        int bonus = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Bonus"].Value ?? 0);
                        decimal price = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Price"].Value ?? 0);
                        decimal subtotal = qty * price;
                        
                        dataGridView1.Rows[e.RowIndex].Cells["SubTotal"].Value = subtotal;
                        
                        // Update purchase items list
                        if (e.RowIndex < _purchaseItems.Count)
                        {
                            _purchaseItems[e.RowIndex].Quantity = qty;
                            _purchaseItems[e.RowIndex].Bonus = bonus;
                            _purchaseItems[e.RowIndex].UnitPrice = price;
                            _purchaseItems[e.RowIndex].SubTotal = subtotal;
                        }
                        
                        CalculateTotals();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating item: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    if (e.ColumnIndex == dataGridView1.Columns["Qty"].Index)
                    {
                        int qty = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Qty"].Value ?? 0);
                        
                        // Validate quantity
                        if (qty <= 0)
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["Qty"].Value = 1;
                            ShowMessage("Quantity must be greater than 0.", "Validation Error", MessageBoxIcon.Warning);
                        }
                    }
                    else if (e.ColumnIndex == dataGridView1.Columns["Bonus"].Index)
                    {
                        int bonus = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Bonus"].Value ?? 0);
                        
                        // Validate bonus (can be 0 or positive)
                        if (bonus < 0)
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["Bonus"].Value = 0;
                            ShowMessage("Bonus cannot be negative.", "Validation Error", MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating input: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RemoveSelectedItem();
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedItem();
            }
        }

        private void RemoveSelectedItem()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    if (rowIndex >= 0 && rowIndex < _purchaseItems.Count)
                    {
                        _purchaseItems.RemoveAt(rowIndex);
                        dataGridView1.Rows.RemoveAt(rowIndex);
                        
                        // Update serial numbers
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            dataGridView1.Rows[i].Cells["SrNo"].Value = i + 1;
                        }
                        
                        CalculateTotals();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error removing item: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals()
        {
            try
            {
                decimal subtotal = _purchaseItems.Sum(item => item.SubTotal);
                decimal discount = Convert.ToDecimal(txtTaxPercent.Text ?? "0");
                decimal discountAmount = subtotal * (discount / 100);
                decimal taxableAmount = subtotal - discountAmount;
                
                decimal taxPercent = 0;
                if (cmbTax.SelectedItem != null)
                {
                    string taxText = cmbTax.SelectedItem.ToString();
                    if (taxText.Contains("%"))
                    {
                        taxText = taxText.Replace("%", "").Trim();
                        decimal.TryParse(taxText, out taxPercent);
                    }
                }
                
                decimal taxAmount = taxableAmount * (taxPercent / 100);
                decimal total = taxableAmount + taxAmount;
                
                decimal paid = Convert.ToDecimal(txtPaid.Text ?? "0");
                decimal change = paid - total;
                
                txtsubTotal.Text = subtotal.ToString("F2");
                txtTax.Text = taxAmount.ToString("F2");
                txtTotal.Text = total.ToString("F2");
                txtChange.Text = change.ToString("F2");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating totals: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Supplier selection changed - no specific action needed for editing
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private void CmbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private void FilterProducts()
        {
            try
            {
                var filteredProducts = _products.AsEnumerable();
                
                if (cmbCategory.SelectedValue != null)
                {
                    int categoryId = ((Category)cmbCategory.SelectedItem).CategoryID;
                    filteredProducts = filteredProducts.Where(p => p.CategoryID == categoryId);
                }
                
                if (cmbBrand.SelectedValue != null)
                {
                    int brandId = ((Brand)cmbBrand.SelectedItem).BrandID;
                    filteredProducts = filteredProducts.Where(p => p.BrandID == brandId);
                }
                
                // Update product search suggestions (if implementing autocomplete)
                // This would require additional UI components
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering products: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtProductName_TextChanged(object sender, EventArgs e)
        {
            // Product name search - could implement autocomplete here
        }

        private void CmbTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void TxtTaxPercent_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void TxtPaid_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SavePurchase();
        }

        private void SavePurchase()
        {
            try
            {
                if (!isEditMode)
                {
                    ShowMessage("Please load a purchase first.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (_purchaseItems.Count == 0)
                {
                    ShowMessage("Please add at least one item to the purchase.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (cmbCustomer.SelectedValue == null)
                {
                    ShowMessage("Please select a supplier.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Validate payment
                decimal total = Convert.ToDecimal(txtTotal.Text ?? "0");
                decimal paid = Convert.ToDecimal(txtPaid.Text ?? "0");
                
                if (paid < total)
                {
                    var result = MessageBox.Show(
                        $"Paid amount (${paid:F2}) is less than total amount (${total:F2}). Do you want to continue?",
                        "Payment Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                // Update purchase object
                _currentPurchase.SupplierID = ((Supplier)cmbCustomer.SelectedItem).SupplierID;
                _currentPurchase.PurchaseDate = guna2DateTimePicker1.Value;
                _currentPurchase.SubTotal = Convert.ToDecimal(txtsubTotal.Text ?? "0");
                _currentPurchase.TaxPercent = Convert.ToDecimal(txtTaxPercent.Text ?? "0");
                _currentPurchase.TaxAmount = Convert.ToDecimal(txtTax.Text ?? "0");
                _currentPurchase.TotalAmount = Convert.ToDecimal(txtTotal.Text ?? "0");
                _currentPurchase.PaymentMethod = cmbPaymentMethod.SelectedItem?.ToString();
                _currentPurchase.PaidAmount = paid;
                _currentPurchase.ChangeAmount = Convert.ToDecimal(txtChange.Text ?? "0");
                _currentPurchase.PurchaseItems = _purchaseItems;

                // Save to database
                bool success = _purchaseRepository.UpdatePurchase(_currentPurchase);
                
                if (success)
                {
                    ShowMessage("Purchase updated successfully!", "Success", MessageBoxIcon.Information);
                    // Optionally print receipt
                    PrintReceipt();
                }
                else
                {
                    ShowMessage("Failed to update purchase.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating purchase: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void PrintReceipt()
        {
            try
            {
                // TODO: Implement receipt printing
                ShowMessage("Receipt printing functionality will be implemented.", "Info", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error printing receipt: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            if (isEditMode)
            {
                LoadPurchaseData();
            }
        }

        private void ClearForm()
        {
            txtinvoiceNo.Clear();
            guna2DateTimePicker1.Value = DateTime.Now;
            cmbCustomer.SelectedIndex = -1;
            cmbPaymentMethod.SelectedIndex = 0;
            txtProductName.Clear();
            txtStockQuantity.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbBrand.SelectedIndex = -1;
            dataGridView1.Rows.Clear();
            txtsubTotal.Clear();
            txtTaxPercent.Clear();
            txtTax.Clear();
            txtTotal.Clear();
            txtPaid.Clear();
            txtChange.Clear();
            cmbTax.SelectedIndex = 0;
            
            _purchaseItems.Clear();
            _currentPurchase = null;
            selectedPurchaseId = -1;
            isEditMode = false;
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void EditPurchase_Load(object sender, EventArgs e)
        {
            // Set initial state
            guna2DateTimePicker1.Value = DateTime.Now;
            txtinvoiceNo.Focus();
        }
    }
}
