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
    public partial class SalesReturnForm : Form
    {
        private SalesReturnService _salesReturnService;
        private SalesReturnRepository _salesReturnRepository;
        private SaleRepository _saleRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private InventoryService _inventoryService;
        private BarcodeService _barcodeService;
        
        private List<Sale> _sales;
        private List<Customer> _customers;
        private List<Product> _products;
        private Sale _selectedSale;
        private List<SalesReturnItem> _returnItems;
        
        private bool isEditMode = false;
        private int selectedReturnId = -1;
        
        // Barcode scanner input field
        private TextBox txtBarcodeScanner;
        private Timer _barcodeTimer;
        private PictureBox picBarcode;
        private bool _isShowingBarcodeError = false;

        public SalesReturnForm()
        {
            InitializeComponent();
            _salesReturnService = new SalesReturnService();
            _salesReturnRepository = new SalesReturnRepository();
            _saleRepository = new SaleRepository();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _inventoryService = new InventoryService();
            
            _returnItems = new List<SalesReturnItem>();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadCustomers();
            LoadSales();
            GenerateReturnNumber();
            InitializeReturnReasons();
            CreateBarcodeScanner();
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
            dataGridView1.Columns.Add("Select", "Select");
            dataGridView1.Columns.Add("ProductID", "ProductID"); // Hidden column for data binding
            dataGridView1.Columns.Add("ItemCode", "Item Code");
            dataGridView1.Columns.Add("ItemName", "Item Name");
            dataGridView1.Columns.Add("OrignalQty", "Original Qty");
            dataGridView1.Columns.Add("ReturnQty", "Return Qty");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("Total", "Total");
            
            // Configure column properties
            dataGridView1.Columns["Select"].Width = 80;
            dataGridView1.Columns["ProductID"].Width = 0; // Hidden column
            dataGridView1.Columns["ProductID"].Visible = false; // Hide the ProductID column
            dataGridView1.Columns["ItemCode"].Width = 120;
            dataGridView1.Columns["ItemName"].Width = 200;
            dataGridView1.Columns["OrignalQty"].Width = 100;
            dataGridView1.Columns["ReturnQty"].Width = 100;
            dataGridView1.Columns["Price"].Width = 100;
            dataGridView1.Columns["Total"].Width = 100;
            
            // Format currency columns
            dataGridView1.Columns["Price"].DefaultCellStyle.Format = "F2";
            dataGridView1.Columns["Total"].DefaultCellStyle.Format = "F2";
            
            // Make ReturnQty column editable
            dataGridView1.Columns["ReturnQty"].ReadOnly = false;
            dataGridView1.Columns["ReturnQty"].DefaultCellStyle.BackColor = Color.LightYellow;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnLoadInvoice.Click += BtnLoadInvoice_Click;
            NewItemBtn.Click += NewItemBtn_Click;
            CancelBtn.Click += CancelBtn_Click;
            
            // DataGridView event handlers
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            
            // ComboBox event handlers
            cmbInvoiceNumber.SelectedIndexChanged += CmbInvoiceNumber_SelectedIndexChanged;
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            cmbTax.SelectedIndexChanged += CmbTax_SelectedIndexChanged;
            
            // TextBox event handlers
            txtTaxPercent.TextChanged += TxtTaxPercent_TextChanged;
            
            // Add event handlers for financial field changes
            // Note: txtTaxPercent is used for discount percentage in this form
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                cmbCustomer.DataSource = _customers;
                cmbCustomer.DisplayMember = "CustomerName";
                cmbCustomer.ValueMember = "CustomerID";
                cmbCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSales()
        {
            try
            {
                _sales = _salesReturnService.GetAvailableSalesForReturn();
                SetupSearchableInvoiceComboBox();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sales: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetupSearchableInvoiceComboBox()
        {
            try
            {
                // Make Invoice Number ComboBox searchable using built-in autocomplete
                cmbInvoiceNumber.DropDownStyle = ComboBoxStyle.DropDown;
                cmbInvoiceNumber.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbInvoiceNumber.AutoCompleteSource = AutoCompleteSource.ListItems;
                
                // Set up data binding
                cmbInvoiceNumber.DisplayMember = "InvoiceNumber";
                cmbInvoiceNumber.ValueMember = "SaleID";
                cmbInvoiceNumber.DataSource = _sales;
                cmbInvoiceNumber.SelectedIndex = -1;
                
                // Rely on SelectedIndexChanged and Load button; built-in autocomplete handles typing
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting up invoice combo box: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool _isFilteringInvoiceCombo = false;
        private bool _isNavigatingWithArrows = false;

        // Removed custom Key/DroppedDown handlers; built-in autocomplete avoids text loss issues

        private void GenerateReturnNumber()
        {
            try
            {
                string returnNumber = _salesReturnRepository.GetNextReturnNumber();
                txtReturnNumber.Text = returnNumber;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating return number: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void InitializeReturnReasons()
        {
            try
            {
                cmbreturnreason.Items.Clear();
                var reasons = _salesReturnService.GetCommonReturnReasons();
                cmbreturnreason.Items.AddRange(reasons.ToArray());
                cmbreturnreason.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing return reasons: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
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
            
            // Add to the first panel (green header panel) if it exists
            if (this.Controls.Count > 0 && this.Controls[0] is Panel panel1)
            {
                panel1.Controls.Add(txtBarcodeScanner);
                txtBarcodeScanner.BringToFront();
            }
            else
            {
                // Add to form if no panel found
                this.Controls.Add(txtBarcodeScanner);
                txtBarcodeScanner.BringToFront();
            }
            
            // Add event handler for barcode scanning
            txtBarcodeScanner.KeyPress += TxtBarcodeScanner_KeyPress;
            txtBarcodeScanner.TextChanged += TxtBarcodeScanner_TextChanged;
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
                _barcodeTimer.Interval = 100; // Reduced delay for better responsiveness
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

                // Find product by barcode - need to get all products to check barcodes
                var products = _productRepository.GetAllProducts();
                var product = products?.FirstOrDefault(p => 
                    !string.IsNullOrEmpty(p.Barcode) && 
                    p.Barcode.Trim().Equals(scannedBarcode, StringComparison.OrdinalIgnoreCase));
                
                if (product != null)
                {
                    // Product found - increase return quantity in datagridview
                    IncreaseReturnQuantityForProduct(product);
                    
                    // Clear scanner input and set placeholder
                    txtBarcodeScanner.Text = "Scan or enter product barcode...";
                    txtBarcodeScanner.ForeColor = Color.Gray;
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
                        MessageBox.Show($"Product not found for barcode: {scannedBarcode}", "Scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Clear input
                        txtBarcodeScanner.Text = "";
                        txtBarcodeScanner.Focus();
                        
                        // Reset the error flag after a brief delay
                        Timer resetTimer = new Timer();
                        resetTimer.Interval = 500; // 500ms delay
                        resetTimer.Tick += (s, args) => {
                            resetTimer.Stop();
                            resetTimer.Dispose();
                            _isShowingBarcodeError = false;
                            
                            // Restore placeholder if empty
                            if (string.IsNullOrWhiteSpace(txtBarcodeScanner.Text))
                            {
                                txtBarcodeScanner.Text = "Scan or enter product barcode...";
                                txtBarcodeScanner.ForeColor = Color.Gray;
                            }
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

        private void IncreaseReturnQuantityForProduct(Product product)
        {
            try
            {
                // Find the row in the datagridview that corresponds to this product
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    var row = dataGridView1.Rows[i];
                    var productIdCell = row.Cells["ProductID"];
                    
                    if (productIdCell != null && productIdCell.Value != null &&
                        Convert.ToInt32(productIdCell.Value) == product.ProductID)
                    {
                        // Found the product row, increment the return quantity
                        var returnQtyCell = row.Cells["ReturnQty"];
                        var originalQtyCell = row.Cells["OrignalQty"];
                        
                        if (returnQtyCell != null && originalQtyCell != null &&
                            returnQtyCell.Value != null && originalQtyCell.Value != null)
                        {
                            int currentReturnQty = Convert.ToInt32(returnQtyCell.Value ?? 0);
                            int originalQty = Convert.ToInt32(originalQtyCell.Value ?? 0);
                            
                            // Only increment if we haven't reached the original quantity
                            if (currentReturnQty < originalQty)
                            {
                                int newReturnQty = currentReturnQty + 1;
                                returnQtyCell.Value = newReturnQty;
                                
                                // Calculate and update the total for this row
                                var priceCell = row.Cells["Price"];
                                if (priceCell != null && priceCell.Value != null)
                                {
                                    decimal price = Convert.ToDecimal(priceCell.Value);
                                    decimal newTotal = newReturnQty * price;
                                    row.Cells["Total"].Value = newTotal;
                                }
                                
                                // Update the corresponding item in _returnItems list
                                if (i < _returnItems.Count)
                                {
                                    _returnItems[i].Quantity = newReturnQty;
                                    _returnItems[i].SubTotal = newReturnQty * _returnItems[i].UnitPrice;
                                }
                                
                                CalculateTotals();
                                ShowMessage($"Increased return quantity for {product.ProductName}", "Product Added", MessageBoxIcon.Information);
                                return;
                            }
                            else
                            {
                                ShowMessage($"Cannot return more than original quantity ({originalQty}) for {product.ProductName}", "Limit Reached", MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                }
                
                // If we reach here, the product wasn't found in the current invoice
                ShowMessage($"Product '{product.ProductName}' is not in the selected invoice", "Not In Invoice", MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error increasing return quantity: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnLoadInvoice_Click(object sender, EventArgs e)
        {
            LoadSelectedInvoice();
        }

        private void LoadSelectedInvoice()
        {
            try
            {
                // Check for valid selection more thoroughly
                if (cmbInvoiceNumber.SelectedItem == null || !(cmbInvoiceNumber.SelectedItem is Sale))
                {
                    // Don't show error if user is just navigating
                    if (!_isNavigatingWithArrows)
                    {
                        ShowMessage("Please select an invoice to load.", "Selection Error", MessageBoxIcon.Warning);
                    }
                    return;
                }

                Sale selectedSaleObj = cmbInvoiceNumber.SelectedItem as Sale;
                if (selectedSaleObj == null)
                {
                    if (!_isNavigatingWithArrows)
                    {
                        ShowMessage("Please select an invoice to load.", "Selection Error", MessageBoxIcon.Warning);
                    }
                    return;
                }

                int saleId = selectedSaleObj.SaleID;
                _selectedSale = _salesReturnRepository.GetSaleWithItems(saleId);

                if (_selectedSale == null)
                {
                    ShowMessage("Invoice not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Populate original invoice details
                txtOriginalInvoiceNumber.Text = _selectedSale.InvoiceNumber;
                txtOriginalInvoiceDate.Text = _selectedSale.SaleDate.ToString("dd/MM/yyyy");
                txtOriginalInvoiceTotal.Text = _selectedSale.TotalAmount.ToString("F2");

                // Populate customer details
                try
                {
                    cmbCustomer.SelectedValue = _selectedSale.CustomerID;
                }
                catch
                {
                    // If customer ID not found in combo box, try to find and select by value
                    for (int i = 0; i < cmbCustomer.Items.Count; i++)
                    {
                        if (cmbCustomer.Items[i] is Customer customer && customer.CustomerID == _selectedSale.CustomerID)
                        {
                            cmbCustomer.SelectedIndex = i;
                            break;
                        }
                    }
                }
                LoadCustomerDetails();

                // Populate tax and discount from original invoice
                // Set discount percentage (txtTaxPercent is used for discount in this form)
                if (_selectedSale.DiscountPercent > 0)
                {
                    txtTaxPercent.Text = _selectedSale.DiscountPercent.ToString("F2");
                }
                else
                {
                    txtTaxPercent.Text = "0";
                }

                // Set tax percentage in combo box - only if combo box has items
                if (cmbTax.Items.Count > 0)
                {
                    string taxPercentText = _selectedSale.TaxPercent.ToString("F0") + "%";
                    bool found = false;
                    for (int i = 0; i < cmbTax.Items.Count; i++)
                    {
                        if (cmbTax.Items[i].ToString() == taxPercentText)
                        {
                            cmbTax.SelectedIndex = i;
                            found = true;
                            break;
                        }
                    }
                    // If exact match not found, set to first item (0%) only if items exist
                    if (!found && cmbTax.Items.Count > 0)
                    {
                        cmbTax.SelectedIndex = 0; // Default to 0%
                    }
                }

                // Populate DataGridView with sale items
                PopulateReturnItems();

                // Calculate totals (this will use the tax and discount values we just set)
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading invoice: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void PopulateReturnItems()
        {
            try
            {
                dataGridView1.Rows.Clear();
                _returnItems.Clear();

                if (_selectedSale == null || _selectedSale.SaleItems == null || _selectedSale.SaleItems.Count == 0)
                {
                    return;
                }

                foreach (var saleItem in _selectedSale.SaleItems)
                {
                    // Get product name - if not available in sale item, fetch from repository
                    string productName = saleItem.ProductName;
                    string productCode = saleItem.ProductCode;

                    // If product name is missing, fetch from product repository
                    if (string.IsNullOrWhiteSpace(productName) && saleItem.ProductID > 0)
                    {
                        try
                        {
                            var product = _productRepository.GetProductById(saleItem.ProductID);
                            if (product != null)
                            {
                                productName = product.ProductName ?? "Unknown Product";
                                productCode = product.ProductCode ?? saleItem.ProductCode ?? "";
                            }
                        }
                        catch (Exception prodEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error fetching product name: {prodEx.Message}");
                            productName = $"Product ID: {saleItem.ProductID}";
                        }
                    }

                    // Ensure we have a display name
                    if (string.IsNullOrWhiteSpace(productName))
                    {
                        productName = $"Product ID: {saleItem.ProductID}";
                    }
                    if (string.IsNullOrWhiteSpace(productCode))
                    {
                        productCode = "";
                    }

                    var returnItem = new SalesReturnItem
                    {
                        ProductID = saleItem.ProductID,
                        Quantity = 0, // Start with 0 return quantity
                        UnitPrice = saleItem.UnitPrice,
                        SubTotal = 0,
                        ProductName = productName,
                        ProductCode = productCode
                    };

                    _returnItems.Add(returnItem);

                    // Add row to DataGridView
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Select"].Value = false;
                    dataGridView1.Rows[rowIndex].Cells["ProductID"].Value = saleItem.ProductID;
                    
                    // Ensure cell values are set properly
                    var itemCodeCell = dataGridView1.Rows[rowIndex].Cells["ItemCode"];
                    var itemNameCell = dataGridView1.Rows[rowIndex].Cells["ItemName"];
                    var originalQtyCell = dataGridView1.Rows[rowIndex].Cells["OrignalQty"];
                    var returnQtyCell = dataGridView1.Rows[rowIndex].Cells["ReturnQty"];
                    var priceCell = dataGridView1.Rows[rowIndex].Cells["Price"];
                    var totalCell = dataGridView1.Rows[rowIndex].Cells["Total"];

                    if (itemCodeCell != null)
                        itemCodeCell.Value = productCode ?? "";
                    if (itemNameCell != null)
                        itemNameCell.Value = productName ?? "Unknown Product";
                    if (originalQtyCell != null)
                        originalQtyCell.Value = saleItem.Quantity;
                    if (returnQtyCell != null)
                        returnQtyCell.Value = 0;
                    if (priceCell != null)
                        priceCell.Value = saleItem.UnitPrice;
                    if (totalCell != null)
                        totalCell.Value = 0;

                    // Force refresh of the row
                    dataGridView1.RefreshEdit();
                }

                // Force DataGridView to refresh and display
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error populating return items: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"PopulateReturnItems Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    if (e.ColumnIndex == dataGridView1.Columns["ReturnQty"].Index)
                    {
                        // Update return quantity and total
                        int returnQty = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ReturnQty"].Value ?? 0);
                        decimal price = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Price"].Value ?? 0);
                        decimal total = returnQty * price;
                        
                        dataGridView1.Rows[e.RowIndex].Cells["Total"].Value = total;
                        
                        // Update return items list
                        if (e.RowIndex < _returnItems.Count)
                        {
                            _returnItems[e.RowIndex].Quantity = returnQty;
                            _returnItems[e.RowIndex].SubTotal = total;
                        }
                        
                        CalculateTotals();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating return quantity: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    if (e.ColumnIndex == dataGridView1.Columns["ReturnQty"].Index)
                    {
                        int returnQty = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ReturnQty"].Value ?? 0);
                        int originalQty = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["OrignalQty"].Value ?? 0);
                        int productId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ProductID"].Value);
                        
                        // Validate return quantity
                        if (returnQty < 0)
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["ReturnQty"].Value = 0;
                            ShowMessage("Return quantity cannot be negative.", "Validation Error", MessageBoxIcon.Warning);
                        }
                        else if (returnQty > originalQty)
                        {
                            dataGridView1.Rows[e.RowIndex].Cells["ReturnQty"].Value = originalQty;
                            ShowMessage($"Return quantity cannot exceed original quantity ({originalQty}).", "Validation Error", MessageBoxIcon.Warning);
                        }
                        else if (returnQty > 0 && _selectedSale != null)
                        {
                            // Validate return eligibility using the service
                            if (!_salesReturnService.ValidateReturnEligibility(_selectedSale.SaleID, productId, returnQty))
                            {
                                dataGridView1.Rows[e.RowIndex].Cells["ReturnQty"].Value = 0;
                                ShowMessage($"Cannot return {returnQty} units. Exceeds available quantity for return.", "Validation Error", MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating return quantity: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CalculateTotals()
        {
            try
            {
                decimal subtotal = _returnItems.Sum(item => item.SubTotal);
                
                // Calculate discount amount
                decimal discountAmount = 0;
                
                // Check if discount is entered as percentage
                if (!string.IsNullOrEmpty(txtTaxPercent.Text))
                {
                    decimal discountPercent = 0;
                    if (decimal.TryParse(txtTaxPercent.Text, out discountPercent))
                    {
                        discountAmount = subtotal * (discountPercent / 100);
                    }
                }
                
                // Note: In this form, discount is only handled as percentage through txtTaxPercent
                // No separate discount amount field exists
                
                decimal taxableAmount = subtotal - discountAmount;
                
                // Calculate tax
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
                
                // Populate all financial fields
                txtsubTotal.Text = subtotal.ToString("F2");
                // Note: Discount amount is calculated and stored but not displayed in a separate field
                // The discount percentage is shown in txtTaxPercent
                // If you need to show discount amount, you could add a label or tooltip
                txtTax.Text = taxAmount.ToString("F2");
                txtTotal.Text = total.ToString("F2");
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Sales Return Calculation: Subtotal={subtotal:F2}, Discount={discountAmount:F2}, Taxable={taxableAmount:F2}, Tax%={taxPercent:F2}, Tax={taxAmount:F2}, Total={total:F2}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating totals: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Sales Return CalculateTotals Error: {ex.Message}");
            }
        }

        private void CmbInvoiceNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb == null) return;

                // Update text when item is selected
                if (cb.SelectedItem is Sale selectedSale)
                {
                    // Only update text if we're not filtering (to preserve user's search text)
                    if (!_isFilteringInvoiceCombo)
                    {
                        // Update text to show selected invoice number
                        if (cb.Text != selectedSale.InvoiceNumber)
                        {
                            cb.Text = selectedSale.InvoiceNumber;
                        }
                    }
                }

                // Only load invoice if:
                // 1. NOT currently navigating with arrow keys (let user press Enter or click to confirm)
                // 2. Dropdown is NOT open (user clicked on an item in closed dropdown)
                // 3. Has a valid selection
                // This prevents loading while user is navigating with arrows
                if (!_isNavigatingWithArrows && 
                    !cb.DroppedDown &&
                    cb.SelectedIndex >= 0 && 
                    cb.SelectedItem != null && 
                    cb.SelectedItem is Sale)
                {
                    // User explicitly selected an item (likely by clicking or tabbing)
                    LoadSelectedInvoice();
                }
                // If navigating with arrows, don't load - wait for Enter or click
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CmbInvoiceNumber_SelectedIndexChanged: {ex.Message}");
            }
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomerDetails();
        }

        private void LoadCustomerDetails()
        {
            try
            {
                if (cmbCustomer.SelectedValue != null)
                {
                    int customerId = ((Customer)cmbCustomer.SelectedItem).CustomerID;
                    var customer = _customers.FirstOrDefault(c => c.CustomerID == customerId);
                    
                    if (customer != null)
                    {
                        txtCustomerName.Text = customer.CustomerName;
                        txtCustomerPhone.Text = customer.Phone;
                        txtCustomerAddress.Text = customer.Address;
                    }
                }
                else
                {
                    txtCustomerName.Clear();
                    txtCustomerPhone.Clear();
                    txtCustomerAddress.Clear();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customer details: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CmbTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Sales Return Tax dropdown changed to: {cmbTax.SelectedItem?.ToString()}");
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling tax selection: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Sales Return Tax Selection Error: {ex.Message}");
            }
        }

        private void TxtTaxPercent_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }


        private void NewItemBtn_Click(object sender, EventArgs e)
        {
            SaveSalesReturn();
        }

        private void SaveSalesReturn()
        {
            try
            {
                // Validate form
                if (cmbInvoiceNumber.SelectedValue == null)
                {
                    ShowMessage("Please select an invoice.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (cmbCustomer.SelectedValue == null)
                {
                    ShowMessage("Please select a customer.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Check if any items are selected for return
                var selectedItems = _returnItems.Where(item => item.Quantity > 0).ToList();
                if (selectedItems.Count == 0)
                {
                    ShowMessage("Please select items to return.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Validate return quantities
                foreach (var item in selectedItems)
                {
                    var originalItem = _selectedSale.SaleItems.FirstOrDefault(si => si.ProductID == item.ProductID);
                    if (originalItem != null && item.Quantity > originalItem.Quantity)
                    {
                        ShowMessage($"Return quantity for {item.ProductName} cannot exceed original quantity ({originalItem.Quantity}).", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Create sales return
                var salesReturn = new SalesReturn
                {
                    ReturnNumber = txtReturnNumber.Text.Trim(),
                    SaleID = ((Sale)cmbInvoiceNumber.SelectedItem).SaleID,
                    CustomerID = ((Customer)cmbCustomer.SelectedItem).CustomerID,
                    ReturnDate = dtpReturnDate.Value,
                    ReturnReason = cmbreturnreason.SelectedItem?.ToString(),
                    Description = txtdescription.Text.Trim(),
                    TotalAmount = ParseDecimal(txtTotal.Text),
                    UserID = UserSession.CurrentUser?.UserID ?? 1, // Get from current user session
                    CreatedDate = DateTime.Now,
                    ReturnItems = selectedItems
                };

                // Populate original invoice details
                _salesReturnService.PopulateOriginalInvoiceDetails(salesReturn, _selectedSale);

                // Save to database using service
                bool success = _salesReturnService.ProcessSalesReturn(salesReturn);
                
                if (success)
                {
                    ShowMessage("Sales return processed successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadSales(); // Refresh the sales list to remove returned invoices
                }
                else
                {
                    ShowMessage("Failed to process sales return.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing sales return: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtReturnNumber.Clear();
            dtpReturnDate.Value = DateTime.Now;
            cmbInvoiceNumber.SelectedIndex = -1;
            cmbCustomer.SelectedIndex = -1;
            txtCustomerName.Clear();
            txtCustomerPhone.Clear();
            txtCustomerAddress.Clear();
            txtOriginalInvoiceNumber.Clear();
            txtOriginalInvoiceDate.Clear();
            txtOriginalInvoiceTotal.Clear();
            dataGridView1.Rows.Clear();
            cmbreturnreason.SelectedIndex = 0;
            txtdescription.Clear();
            txtsubTotal.Clear();
            txtTaxPercent.Clear();
            txtTax.Clear();
            txtTotal.Clear();
            cmbTax.SelectedIndex = -1;
            
            _returnItems.Clear();
            _selectedSale = null;
            GenerateReturnNumber();

            // Reload lookups so dropdowns are ready for the next return
            LoadCustomers();
            LoadSales();
            InitializeReturnReasons();
            if (cmbTax.Items.Count > 0)
            {
                cmbTax.SelectedIndex = 0;
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void SalesReturn_Load(object sender, EventArgs e)
        {
            // Set initial state
            dtpReturnDate.Value = DateTime.Now;
            cmbTax.Items.Add("0%");
            cmbTax.Items.Add("5%");
            cmbTax.Items.Add("10%");
            cmbTax.Items.Add("15%");
            cmbTax.Items.Add("20%");
            cmbTax.SelectedIndex = 0;
            
            // Initialize financial fields
            InitializeFinancialFields();
        }

        private void InitializeFinancialFields()
        {
            txtsubTotal.Text = "0.00";
            txtTaxPercent.Text = "0";
            txtTax.Text = "0.00";
            txtTotal.Text = "0.00";
            
            // Ensure tax calculation runs after initialization
            CalculateTotals();
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            
            // Remove any non-numeric characters except decimal point and minus sign
            string cleanValue = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d.-]", "");
            
            if (string.IsNullOrWhiteSpace(cleanValue))
                return 0;
            
            if (decimal.TryParse(cleanValue, out decimal result))
                return result;
            
            return 0;
        }
    }
}
