using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vape_Store.Helpers;
using Vape_Store.Models;
using Vape_Store.Repositories;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class SalesForm : Form
    {

        string cs = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
        private List<string> allProducts = new List<string>();
        private bool suppressTextChanged = false;

        private SalesService _salesService;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private BarcodeService _barcodeService;
        private BusinessDateService _businessDateService;

        private List<SaleItem> _saleItems = new List<SaleItem>();
        private List<Product> _products;

        private List<Customer> _customers;
        private string _invoiceNumber;
        private bool _isUpdating = false;
        private System.Windows.Forms.Timer _barcodeTimer;

        // FIELDS FROM THE EXACT SNIPPET PROVIDED BY USER
        


        public SalesForm()
        {
            InitializeComponent();
            InitializeDependencies();
            SetupInitialState();
            AttachEventHandlers(); // Wire ALL event handlers including barcode scanner
            //cmbProductName.TextUpdate += cmbProductName_TextUpdate;

            cmbProductName.AutoCompleteMode = AutoCompleteMode.None;
            cmbProductName.AutoCompleteSource = AutoCompleteSource.None;
        }

        private void InitializeDependencies()
        {
            //LoadProducts();
            _salesService = new SalesService();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _categoryRepository = new CategoryRepository();
            _brandRepository = new BrandRepository();
            _barcodeService = new BarcodeService();
            _businessDateService = new BusinessDateService();
            _saleItems = new List<SaleItem>();

            // Debounce timer for barcode scanner (auto-process after 300ms without typing)
            _barcodeTimer = new System.Windows.Forms.Timer();
            _barcodeTimer.Interval = 300;
            _barcodeTimer.Tick += (s, e) =>
            {
                _barcodeTimer.Stop();
                string code = txtBarcodeScanner.Text.Trim();
                if (!string.IsNullOrEmpty(code))
                    ProcessBarcode(code);
            };

            // CRITICAL: Disable all built-in auto-complete features that fight the custom search
            //cmbProductName.AutoCompleteMode = AutoCompleteMode.None;
            //cmbProductName.AutoCompleteSource = AutoCompleteSource.None;
            //this.cmbProductName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            //cmbProductName.IntegralHeight = false;
            //cmbProductName.MaxDropDownItems = 10;

            // MANUALLY FORCE EVENT CONNECTION AND DISPLAY MEMBER
            //cmbProductName.DisplayMember = "ProductName";
            //cmbProductName.ValueMember = "ProductID";
        }

        private void SetupInitialState()
        {
            ThemeManager.ApplyTheme(this);
            dtpSaleDate.Value = _businessDateService.GetCurrentBusinessDate();
            GenerateInvoiceNumber();
            LoadData();
            cmbPaymentMethod.SelectedIndex = 0;
            
            // Force white backgrounds for standard comboboxes to fix the black render bug
            var combos = new[] { cmbCategory, cmbBrand, cmbProductName, cmbCustomer, cmbPaymentMethod };
            foreach (var combo in combos)
            {
                combo.BackColor = Color.White;
                combo.ForeColor = Color.Black;
                // Use DropDown for product selection to allow searching; others can be DropDownList
                combo.DropDownStyle = (combo == cmbProductName) ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
                combo.FlatStyle = FlatStyle.Standard;
            }
        }

        private void AttachEventHandlers()
        {
            btnNew.Click += (s, e) => ClearForm();
            btnClear.Click += (s, e) => ClearForm();
            btnCancel.Click += (s, e) => this.Close();
            btnRefresh.Click += (s, e) => LoadData();
            btnAddItem.Click += (s, e) => AddItem();
            btnSave.Click += (s, e) => SaveSale();

            cmbCategory.SelectedIndexChanged += (s, e) => FilterProducts();
            cmbBrand.SelectedIndexChanged += (s, e) => FilterProducts();
            
            txtDiscountPercent.TextChanged += (s, e) => CalculateTotals();
            txtDiscountAmount.TextChanged += (s, e) => CalculateTotals();
            txtTaxPercent.TextChanged += (s, e) => CalculateTotals();
            txtTaxAmount.TextChanged += (s, e) => CalculateTotals();
            txtPaid.TextChanged += (s, e) => CalculateChange();

            cmbProductName.TextUpdate += cmbProductName_TextUpdate;
            cmbProductName.KeyDown += cmbProductName_KeyDown;

            txtBarcodeScanner.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    _barcodeTimer.Stop();
                    ProcessBarcode(txtBarcodeScanner.Text.Trim());
                }
            };
            // Auto-process when scanner sends characters without Enter
            txtBarcodeScanner.TextChanged += (s, e) =>
            {
                if (txtBarcodeScanner.Text.Length >= 3)
                {
                    _barcodeTimer.Stop();
                    _barcodeTimer.Start();
                }
                else
                {
                    _barcodeTimer.Stop();
                }
            };

            dgvCart.CellClick += DgvCart_CellClick;
            dgvCart.CellValueChanged += DgvCart_CellValueChanged;
            dgvCart.KeyDown += DgvCart_KeyDown;
        }

        private void LoadData()
        {
            LoadCategories();
            LoadBrands();
            LoadCustomers();
            LoadProducts();
        }

        private void LoadCategories()
        {
            var categories = _categoryRepository.GetAllCategories();
            categories.Insert(0, new Category { CategoryID = -1, CategoryName = "All Categories" });
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
        }

        private void LoadBrands()
        {
            var brands = _brandRepository.GetAllBrands();
            brands.Insert(0, new Brand { BrandID = -1, BrandName = "All Brands" });
            cmbBrand.DataSource = brands;
            cmbBrand.DisplayMember = "BrandName";
            cmbBrand.ValueMember = "BrandID";
        }

        private void LoadCustomers()
        {
            _customers = _customerRepository.GetAllCustomers();
            cmbCustomer.DataSource = _customers;
            cmbCustomer.DisplayMember = "CustomerName";
            cmbCustomer.ValueMember = "CustomerID";

            // Default to Walk-in
            var walkIn = _customers.FirstOrDefault(c => c.CustomerName.ToLower().Contains("walk in customer"));
            if (walkIn != null) cmbCustomer.SelectedValue = walkIn.CustomerID;
        }

        private void LoadProducts()
        {
            //_products = _productRepository.GetAllProducts();

            //// Populate the EXACT List<string> from user snippet
            //allProducts = _products.Select(p => p.ProductName).ToList();

            //FilterProducts();

            try
            {
                // ✅ LOAD FULL PRODUCT LIST (IMPORTANT)
                _products = _productRepository.GetAllProducts() ?? new List<Product>();

                using (SqlConnection conn = new SqlConnection(cs))
                {
                    conn.Open();
                    string query = "SELECT ProductName FROM Products";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    allProducts.Clear();
                    while (reader.Read())
                        allProducts.Add(reader["ProductName"].ToString());

                    reader.Close();
                }

                cmbProductName.DataSource = new List<string>(allProducts);
                cmbProductName.SelectedIndex = -1;
                cmbProductName.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }






        }

        private void FilterProducts()
        {
            //_filteredProducts = _products ?? new List<Product>();

            //if (cmbCategory.SelectedValue is int catId && catId != -1)
            //    _filteredProducts = _filteredProducts.Where(p => p.CategoryID == catId).ToList();

            //if (cmbBrand.SelectedValue is int brandId && brandId != -1)
            //    _filteredProducts = _filteredProducts.Where(p => p.BrandID == brandId).ToList();

            //allProducts = _filteredProducts.Select(p => p.ProductName).ToList();

            //suppressTextChanged = true;
            //try
            //{
            //    cmbProductName.DataSource = null; // 🔥 VERY IMPORTANT
            //    cmbProductName.Items.Clear();
            //    cmbProductName.Items.AddRange(allProducts.ToArray());

            //    cmbProductName.SelectedIndex = -1;
            //    cmbProductName.Text = "";
            //}
            //finally
            //{
            //    suppressTextChanged = false;
            //}
        }

        private void cmbProductName_TextUpdate(object sender, EventArgs e)
        {
            if (suppressTextChanged) return;

            string typed = cmbProductName.Text;

            var filtered = allProducts
                .Where(p => p.IndexOf(typed, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            suppressTextChanged = true;

            if (filtered.Count > 0)
            {
                cmbProductName.DataSource = filtered;
                cmbProductName.DroppedDown = true;
            }
            else
            {
                cmbProductName.DroppedDown = false;
            }

            cmbProductName.Text = typed;
            cmbProductName.SelectionStart = typed.Length; // 🔥 always at end
            cmbProductName.SelectionLength = 0;

            suppressTextChanged = false;
        }

        private void cmbProductName_KeyDown(object sender, KeyEventArgs e)
        {
            // THE EXACT LOGIC PROVIDED BY USER (LINKED TO SALES ADDITEM)
            if (e.KeyCode == Keys.Enter)
            {
                if (cmbProductName.SelectedItem != null)
                {
                    cmbProductName.Text = cmbProductName.SelectedItem.ToString();
                    cmbProductName.SelectionStart = cmbProductName.Text.Length;
                    cmbProductName.DroppedDown = false;
                    e.Handled = true;
                    
                    // Sales Integration: Add selected item to cart
                    AddItem();
                }
            }
        }
        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //private void cmbProductName_TextChanged(object sender, EventArgs e)
        //{
        //    // Using TextUpdate as per user snippet
        //}


        private void GenerateInvoiceNumber()
        {
            _invoiceNumber = _salesService.GetNextInvoiceNumber();
            lblInvoiceValue.Text = _invoiceNumber;
            UpdateBarcode();
        }

        private void UpdateBarcode()
        {
            if (!string.IsNullOrEmpty(_invoiceNumber))
            {
                var img = _barcodeService.GenerateBarcodeImageObject(_invoiceNumber, 250, 50);
                picBarcode.Image = img;
                picBarcode.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void AddItem()
        {
            if (_products == null || !_products.Any())
            {
                MessageBox.Show("Products not loaded.");
                return;
            }

            // The exact search snippet uses List<string>, so SelectedItem is now a string
            string selectedName = cmbProductName.Text?.Trim();
            Product product = _products.FirstOrDefault(p => p.ProductName == selectedName);

            if (product != null)
            {
                if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity.");
                    return;
                }

                int currentQty = _saleItems.Where(i => i.ProductID == product.ProductID).Sum(i => i.Quantity);
                if (product.StockQuantity < currentQty + qty)
                {
                    MessageBox.Show($"Insufficient stock. Available: {product.StockQuantity}");
                    return;
                }

                try
                {
                    var breakdowns = _salesService.GetFIFOBreakdown(product, currentQty + qty);
                    
                    // Remove existing items for this product
                    _saleItems.RemoveAll(i => i.ProductID == product.ProductID);
                    
                    // Add the new broken-down FIFO items
                    _saleItems.AddRange(breakdowns);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                RefreshGrid();
                CalculateTotals();
                
                _isUpdating = true;
                suppressTextChanged = true;
                
                // Reset search box completely
                cmbProductName.DataSource = new List<string>(allProducts);
                cmbProductName.SelectedIndex = -1;
                cmbProductName.Text = "";
                
                _isUpdating = false;
                suppressTextChanged = false;
                
                txtBarcodeScanner.Clear();
                cmbProductName.Focus();
            }
        }

        private void ProcessBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) return;

            try
            {
                var product = _productRepository.GetProductByBarcode(barcode);
                if (product != null)
                {
                    // DataSource is List<string> — bypass ComboBox, add directly to cart
                    var match = _products?.FirstOrDefault(p => p.ProductID == product.ProductID);
                    AddSpecificItem(match ?? product);
                }
                else
                {
                    MessageBox.Show($"Product with barcode '{barcode}' not found.", "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning barcode: {ex.Message}");
            }
            finally
            {
                txtBarcodeScanner.Clear();
                txtBarcodeScanner.Focus();
            }
        }

        private void AddSpecificItem(Product product)
        {
            if (product == null) return;

            int currentQty = _saleItems.Where(i => i.ProductID == product.ProductID).Sum(i => i.Quantity);
            if (product.StockQuantity < currentQty + 1)
            {
                MessageBox.Show($"Insufficient stock. Available: {product.StockQuantity}");
                return;
            }

            try
            {
                var breakdowns = _salesService.GetFIFOBreakdown(product, currentQty + 1);
                
                _saleItems.RemoveAll(i => i.ProductID == product.ProductID);
                _saleItems.AddRange(breakdowns);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            RefreshGrid();
            CalculateTotals();
        }

        private void RefreshGrid()
        {
            dgvCart.Rows.Clear();
            for (int i = 0; i < _saleItems.Count; i++)
            {
                var item = _saleItems[i];
                dgvCart.Rows.Add(i + 1, item.ProductName, item.Quantity, item.UnitPrice, 
                    item.Discount, 
                    item.TaxAmount, 
                    item.SubTotal);
            }
        }

        private void CalculateTotals()
        {
            if (_isUpdating) return;
            _isUpdating = true;

            // Calculate all totals from the sale items
            decimal grossSubtotal = _saleItems.Sum(i => i.Quantity * i.UnitPrice);
            decimal lineDiscountTotal = _saleItems.Sum(i => i.Discount);
            decimal lineTaxTotal = _saleItems.Sum(i => i.TaxAmount);

            // Display subtotal
            txtSubtotal.Text = grossSubtotal.ToString("N2");

            // Determine which discount control triggered the calculation
            bool isPercentChanged = txtDiscountPercent.Focused;
            bool isAmountChanged = txtDiscountAmount.Focused;

            // Get values from textboxes
            decimal displayPercent = ParseDecimal(txtDiscountPercent.Text);
            decimal displayAmount = ParseDecimal(txtDiscountAmount.Text);

            decimal totalDiscountAmount = 0;
            decimal totalDiscountPercent = 0;

            if (isPercentChanged)
            {
                // User is editing percentage - THIS OVERRIDES LINE DISCOUNTS
                totalDiscountPercent = displayPercent;

                // Validate percentage range
                if (totalDiscountPercent < 0) totalDiscountPercent = 0;
                if (totalDiscountPercent > 100) totalDiscountPercent = 100;

                // Calculate discount amount based on subtotal
                totalDiscountAmount = grossSubtotal * (totalDiscountPercent / 100);

                // Update amount field
                txtDiscountAmount.Text = totalDiscountAmount.ToString("N2");

                // Update the line items with proportional discount
                ApplyProportionalDiscountToItems(totalDiscountAmount, grossSubtotal);
            }
            else if (isAmountChanged)
            {
                // User is editing amount - THIS OVERRIDES LINE DISCOUNTS
                totalDiscountAmount = displayAmount;

                // Validate amount doesn't exceed subtotal
                if (totalDiscountAmount > grossSubtotal)
                {
                    totalDiscountAmount = grossSubtotal;
                    txtDiscountAmount.Text = totalDiscountAmount.ToString("N2");
                    MessageBox.Show("Discount cannot exceed subtotal.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (totalDiscountAmount < 0)
                {
                    totalDiscountAmount = 0;
                    txtDiscountAmount.Text = totalDiscountAmount.ToString("N2");
                }

                // Calculate discount percentage based on subtotal
                if (grossSubtotal > 0)
                {
                    totalDiscountPercent = (totalDiscountAmount / grossSubtotal) * 100;
                    txtDiscountPercent.Text = totalDiscountPercent.ToString("N2");
                }

                // Update the line items with proportional discount
                ApplyProportionalDiscountToItems(totalDiscountAmount, grossSubtotal);
            }
            else
            {
                // Recalculate from line items (DataGridView edits)
                // Display current state with line discounts
                totalDiscountAmount = lineDiscountTotal;
                txtDiscountAmount.Text = totalDiscountAmount.ToString("N2");

                // Calculate and display effective percentage based on subtotal
                if (grossSubtotal > 0)
                {
                    totalDiscountPercent = (totalDiscountAmount / grossSubtotal) * 100;
                    txtDiscountPercent.Text = totalDiscountPercent.ToString("N2");
                }
                else
                {
                    txtDiscountPercent.Text = "0";
                }
            }

            // Calculate final total after discounts
            decimal amountAfterDiscount = grossSubtotal - totalDiscountAmount;
            if (amountAfterDiscount < 0) amountAfterDiscount = 0;

            // --- Tax Calculation ---
            decimal globalTaxPercent = ParseDecimal(txtTaxPercent.Text);
            decimal globalTaxAmount = ParseDecimal(txtTaxAmount.Text);
            decimal totalTaxWithLine = lineTaxTotal + globalTaxAmount;

            if (txtTaxAmount.Focused)
            {
                globalTaxAmount = totalTaxWithLine - lineTaxTotal;
                globalTaxPercent = amountAfterDiscount > 0 ? (globalTaxAmount / amountAfterDiscount) * 100 : 0;
                txtTaxPercent.Text = globalTaxPercent.ToString("N2");
            }
            else if (txtTaxPercent.Focused)
            {
                globalTaxAmount = amountAfterDiscount * (globalTaxPercent / 100);
                totalTaxWithLine = lineTaxTotal + globalTaxAmount;
                txtTaxAmount.Text = totalTaxWithLine.ToString("N2");
            }
            else
            {
                globalTaxAmount = amountAfterDiscount * (globalTaxPercent / 100);
                totalTaxWithLine = lineTaxTotal + globalTaxAmount;
                txtTaxAmount.Text = totalTaxWithLine.ToString("N2");

                if (string.IsNullOrWhiteSpace(txtTaxPercent.Text) || txtTaxPercent.Text == "0")
                {
                    txtTaxPercent.Text = globalTaxPercent.ToString("N2");
                }
            }

            decimal total = amountAfterDiscount + globalTaxAmount;
            txtTotal.Text = total.ToString("N2");

            _isUpdating = false;
            CalculateChange();
        }

        // New helper method to apply discount proportionally to all items
        private void ApplyProportionalDiscountToItems(decimal totalDiscountAmount, decimal grossSubtotal)
        {
            if (_saleItems.Count == 0) return;
            if (grossSubtotal <= 0) return;

            _isUpdating = true;

            // Calculate the discount ratio
            decimal discountRatio = totalDiscountAmount / grossSubtotal;

            // Apply discount to each item proportionally
            foreach (var item in _saleItems)
            {
                decimal itemTotal = item.Quantity * item.UnitPrice;
                decimal itemDiscount = itemTotal * discountRatio;

                item.Discount = Math.Round(itemDiscount, 2);
                item.DiscountPercent = discountRatio * 100;
                item.SubTotal = itemTotal - item.Discount + item.TaxAmount;
            }

            // Refresh the grid to show updated discounts
            RefreshGrid();

            _isUpdating = false;
        }

        // Update RecalculateFromLineDiscounts to not add global discounts
        private void RecalculateFromLineDiscounts()
        {
            if (_isUpdating) return;

            decimal grossSubtotal = _saleItems.Sum(i => i.Quantity * i.UnitPrice);
            decimal lineDiscountTotal = _saleItems.Sum(i => i.Discount);

            _isUpdating = true;
            txtDiscountAmount.Text = lineDiscountTotal.ToString("N2");

            // Calculate and display effective percentage based on subtotal
            if (grossSubtotal > 0)
            {
                decimal effectivePercent = (lineDiscountTotal / grossSubtotal) * 100;
                txtDiscountPercent.Text = effectivePercent.ToString("N2");
            }
            else
            {
                txtDiscountPercent.Text = "0";
            }

            _isUpdating = false;

            CalculateTotals();
        }

       

        private void txtDiscountAmount_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            CalculateTotals();
        }

        private void txtDiscountPercent_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            CalculateTotals();
        }

        private void CalculateChange()
        {
            decimal total = decimal.TryParse(txtTotal.Text, out decimal t) ? t : 0;
            decimal paid = decimal.TryParse(txtPaid.Text, out decimal p) ? p : 0;
            txtChange.Text = (paid - total).ToString("N2");
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            // Remove currency symbols, commas, and other non-numeric characters except decimal point
            string cleanValue = new string(value.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
            return decimal.TryParse(cleanValue, out decimal result) ? result : 0;
        }

        private void SaveSale()
        {
            if (_saleItems.Count == 0)
            {
                MessageBox.Show("Cart is empty.");
                return;
            }

            if (cmbPaymentMethod.Text.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                if (ParseDecimal(txtPaid.Text) <= 0)
                {
                    MessageBox.Show("Paid amount is mandatory for cash transactions.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var sale = new Sale
            {
                InvoiceNumber = _invoiceNumber,
                CustomerID = cmbCustomer.SelectedValue != null ? (int)cmbCustomer.SelectedValue : 0,
                SaleDate = dtpSaleDate.Value,
                SubTotal = ParseDecimal(txtSubtotal.Text),
                DiscountAmount = ParseDecimal(txtDiscountAmount.Text),
                DiscountPercent = ParseDecimal(txtDiscountPercent.Text),
                TaxAmount = ParseDecimal(txtTaxAmount.Text),
                TaxPercent = ParseDecimal(txtTaxPercent.Text),
                TotalAmount = ParseDecimal(txtTotal.Text),
                PaymentMethod = cmbPaymentMethod.Text,
                PaidAmount = ParseDecimal(txtPaid.Text),
                ChangeAmount = ParseDecimal(txtChange.Text),
                UserID = UserSession.CurrentUser?.UserID ?? 0,
                BarcodeData = _invoiceNumber,
                SaleItems = _saleItems
            };

            if (_salesService.ProcessSale(sale))
            {
                // Show thermal invoice immediately
                var thermalInvoice = new ThermalInvoiceForm(sale);
                thermalInvoice.ShowDialog();
                
                MessageBox.Show("Sale saved successfully.");
                ClearForm();
            }
            else
            {
                MessageBox.Show("Error processing sale.");
            }
        }

        private void ClearForm()
        {
            _saleItems.Clear();
            RefreshGrid();
            txtDiscountPercent.Clear();
            txtDiscountAmount.Clear();
            txtTaxPercent.Text = "0";
            txtTaxAmount.Clear();
            txtPaid.Clear();
            txtChange.Clear();
            txtSubtotal.Clear();
            txtTotal.Clear();
            GenerateInvoiceNumber();
        }

        private void DgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Simple implementation: double click or specific column could be used
                // For now, let's just allow removal on cell click if we added a delete button (which we haven't yet, but can)
            }
        }

        private void DgvCart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvCart.CurrentRow != null)
            {
                int index = dgvCart.CurrentRow.Index;
                if (index >= 0 && index < _saleItems.Count)
                {
                    _saleItems.RemoveAt(index);
                    RefreshGrid();
                    CalculateTotals();
                }
            }
        }

        private void DgvCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdating || e.RowIndex < 0) return;

            // Column 4 is Discount Amount
            if (e.ColumnIndex == 4)
            {
                if (e.RowIndex < 0 || e.RowIndex >= _saleItems.Count) return;

                var row = dgvCart.Rows[e.RowIndex];
                var item = _saleItems[e.RowIndex];
                decimal discountAmount = ParseDecimal(row.Cells[4].Value?.ToString());

                if (discountAmount < 0)
                {
                    _isUpdating = true;
                    row.Cells[4].Value = item.Discount;
                    _isUpdating = false;
                    MessageBox.Show("Discount cannot be negative.");
                    return;
                }

                decimal grossTotal = item.Quantity * item.UnitPrice;
                if (discountAmount > grossTotal)
                {
                    _isUpdating = true;
                    row.Cells[4].Value = item.Discount;
                    _isUpdating = false;
                    MessageBox.Show($"Discount cannot exceed line total ({grossTotal:N2}).");
                    return;
                }

                // Update the item discount
                item.Discount = discountAmount;
                item.DiscountPercent = grossTotal > 0 ? (discountAmount / grossTotal) * 100 : 0;
                item.SubTotal = grossTotal - item.Discount + item.TaxAmount;

                _isUpdating = true;
                row.Cells[6].Value = item.SubTotal.ToString("N2");
                _isUpdating = false;

                // Recalculate totals and update discount fields
                RecalculateFromLineDiscounts();
            }
            // Column 2 is Quantity
            else if (e.ColumnIndex == 2)
            {
                var row = dgvCart.Rows[e.RowIndex];
                if (int.TryParse(row.Cells[2].Value?.ToString(), out int qty) && qty > 0)
                {
                    var item = _saleItems[e.RowIndex];

                    int currentQtyForOtherRows = _saleItems
                        .Where((v, idx) => v.ProductID == item.ProductID && idx != e.RowIndex)
                        .Sum(v => v.Quantity);

                    int newTotal = currentQtyForOtherRows + qty;

                    var product = _productRepository.GetAllProducts().FirstOrDefault(p => p.ProductID == item.ProductID);

                    if (product != null && product.StockQuantity < newTotal)
                    {
                        MessageBox.Show($"Insufficient stock. Available: {product.StockQuantity}");
                        _isUpdating = true;
                        row.Cells[2].Value = item.Quantity;
                        _isUpdating = false;
                        return;
                    }

                    try
                    {
                        if (product != null)
                        {
                            var breakdowns = _salesService.GetFIFOBreakdown(product, newTotal);
                            _saleItems.RemoveAll(i => i.ProductID == item.ProductID);
                            _saleItems.AddRange(breakdowns);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message);
                        _isUpdating = true;
                        row.Cells[2].Value = item.Quantity;
                        _isUpdating = false;
                        return;
                    }

                    _isUpdating = true;
                    RefreshGrid();
                    _isUpdating = false;

                    RecalculateFromLineDiscounts();
                }
                else
                {
                    _isUpdating = true;
                    row.Cells[2].Value = _saleItems[e.RowIndex].Quantity;
                    _isUpdating = false;
                }
            }
            // Column 5 is Tax Amount
            else if (e.ColumnIndex == 5)
            {
                if (e.RowIndex < 0 || e.RowIndex >= _saleItems.Count) return;

                var row = dgvCart.Rows[e.RowIndex];
                var item = _saleItems[e.RowIndex];
                decimal taxAmount = ParseDecimal(row.Cells[5].Value?.ToString());

                if (taxAmount < 0)
                {
                    _isUpdating = true;
                    row.Cells[5].Value = item.TaxAmount;
                    _isUpdating = false;
                    MessageBox.Show("Tax amount cannot be negative.");
                    return;
                }

                decimal taxableAmount = (item.Quantity * item.UnitPrice) - item.Discount;
                decimal taxPercent = taxableAmount > 0 ? (taxAmount / taxableAmount) * 100 : 0;

                item.TaxAmount = taxAmount;
                item.TaxPercent = taxPercent;
                item.SubTotal = taxableAmount + taxAmount;

                _isUpdating = true;
                row.Cells[6].Value = item.SubTotal.ToString("N2");
                _isUpdating = false;

                CalculateTotals();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void SalesForm_Load(object sender, EventArgs e)
        {
           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

      
    }
}
