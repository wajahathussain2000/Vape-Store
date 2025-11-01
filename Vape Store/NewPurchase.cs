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
using Vape_Store.Repositories;
using Vape_Store.Services;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store
{
    public partial class NewPurchase : Form
    {
        #region Private Fields
        private PurchaseRepository _purchaseRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private BrandRepository _brandRepository;
        private InventoryService _inventoryService;
        
        private List<PurchaseItem> _purchaseItems;
        private List<Supplier> _suppliers;
        private List<Product> _products;
        private List<Category> _categories;
        private List<Brand> _brands;
        private Product _selectedProduct;
        
        private decimal _subtotal = 0;
        private decimal _overallDiscount = 0;
        private decimal _tax = 0;
        private decimal _freightCharges = 0;
        private decimal _otherCharges = 0;
        private decimal _grandTotal = 0;
        private decimal _paidAmount = 0;
        private decimal _balanceAmount = 0;
        private string _invoiceNumber = "";
        private int _currentUserID; // Will be set from UserSession
        
        private bool _isCalculating = false;
        private bool _isFormDirty = false;
        // Holds the current editing ComboBox for incremental search
        private ComboBox _productNameEditingControl;
        // Guard flag to avoid recursive TextChanged during filtering
        private bool _isFilteringProductCombo = false;
        private bool _isFilteringVendorCombo = false;
        private object _filterLock = new object();
        #endregion

        #region Constructor
        public NewPurchase()
        {
            try
            {
                InitializeComponent();
                InitializeRepositories();
                InitializeForm();
                SetupEventHandlers();
                LoadInitialData();
                GenerateInvoiceNumber();
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing the form: {ex.Message}");
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"NewPurchase Constructor Error: {ex}");
            }
        }
        #endregion

        #region Initialization
        private void InitializeRepositories()
        {
            try
            {
                _purchaseRepository = new PurchaseRepository();
                _supplierRepository = new SupplierRepository();
                _productRepository = new ProductRepository();
                _categoryRepository = new CategoryRepository();
                _brandRepository = new BrandRepository();
                _inventoryService = new InventoryService();
                
                _purchaseItems = new List<PurchaseItem>();
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing repositories: {ex.Message}");
                throw; // Re-throw to be caught by constructor
            }
        }

        private void InitializeForm()
        {
            try
            {
                // Set default values
                dtpInvoiceDate.Value = DateTime.Now;
                
                // Set default selections
                cmbPaymentTerms.SelectedIndex = 0; // Cash
                
                // Set current user from UserSession
                if (UserSession.CurrentUser != null)
                {
                    _currentUserID = UserSession.CurrentUser.UserID;
                }
                else
                {
                    _currentUserID = 1; // Fallback to default if session not available
                }
                
                // Make all ComboBoxes searchable
                SetupSearchableComboBoxes();
                
                // Initialize DataGridView
                InitializeDataGridView();
                
                // Set form state
                _isFormDirty = false;
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing form: {ex.Message}");
                throw; // Re-throw to be caught by constructor
            }
        }

        private void SetupSearchableComboBoxes()
        {
            try
            {
                // Make Vendor (Supplier) ComboBox searchable with same functionality as product dropdown
                cmbVendorName.DropDownStyle = ComboBoxStyle.DropDown;
                cmbVendorName.AutoCompleteMode = AutoCompleteMode.None; // Disable to prevent conflicts
                cmbVendorName.AutoCompleteSource = AutoCompleteSource.None;
                
                // Unsubscribe from old events if any
                cmbVendorName.TextChanged -= CmbVendorName_TextChanged;
                cmbVendorName.KeyUp -= CmbVendorName_KeyUp;
                cmbVendorName.KeyDown -= CmbVendorName_KeyDown;
                cmbVendorName.KeyPress -= CmbVendorName_KeyPress;
                cmbVendorName.PreviewKeyDown -= CmbVendorName_PreviewKeyDown;
                
                // Subscribe to new events for search functionality
                cmbVendorName.KeyUp += CmbVendorName_KeyUp;
                cmbVendorName.KeyDown += CmbVendorName_KeyDown;
                cmbVendorName.KeyPress += CmbVendorName_KeyPress;
                cmbVendorName.PreviewKeyDown += CmbVendorName_PreviewKeyDown;
                
                // Make Payment Terms ComboBox searchable
                cmbPaymentTerms.DropDownStyle = ComboBoxStyle.DropDown;
                cmbPaymentTerms.AutoCompleteMode = AutoCompleteMode.None; // Disable auto-complete to prevent conflicts
                cmbPaymentTerms.AutoCompleteSource = AutoCompleteSource.None;
                
                // Product search: keep dropdown and enable auto-complete suggestions
                cmbProductName.DropDownStyle = ComboBoxStyle.DropDown;
                cmbProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbProductName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                
                // Add selection changed event
                cmbProductName.SelectedIndexChanged += CmbProductName_SelectedIndexChanged;

                // Always show full product list when opening the dropdown
                cmbProductName.DropDown += (s, e2) => { PopulateProductNameComboBox(); };
            }
            catch (Exception ex)
            {
                ShowError($"Error setting up searchable combo boxes: {ex.Message}");
            }
        }

        // DISABLED: TextChanged handler - using KeyUp instead (same pattern as product dropdown)
        private void CmbVendorName_TextChanged(object sender, EventArgs e)
        {
            // DISABLED: This handler conflicts with KeyUp filtering
            // KeyUp is now the primary filtering mechanism
            return;
        }
        
        private void CmbVendorName_KeyUp(object sender, KeyEventArgs e)
        {
            // Prevent re-entrant calls
            if (_isFilteringVendorCombo) return;
            
            try
            {
                var cb = sender as ComboBox;
                if (cb == null || cb.IsDisposed) return;
                
                // Skip filtering for control keys (arrows, enter, etc.)
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                    e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape ||
                    e.KeyCode == Keys.Tab || e.KeyCode == Keys.Left ||
                    e.KeyCode == Keys.Right)
                {
                    return;
                }
                
                _isFilteringVendorCombo = true;
                
                try
                {
                    // Get the current filter text (what user has typed)
                    string filter = (cb.Text ?? string.Empty).ToLower().Trim();
                    
                    // If filter is empty, show all vendors
                    if (string.IsNullOrWhiteSpace(filter))
                    {
                        RefreshAllVendorsInComboBox(cb);
                        if (cb != null && !cb.IsDisposed && cb.Items.Count > 0 && !cb.DroppedDown)
                        {
                            cb.DroppedDown = true;
                        }
                        return;
                    }
                    
                    // Filter vendors based on search text (contains match - works for "ap" -> "Apple Suppliers")
                    if (_suppliers == null || _suppliers.Count == 0) return;
                    
                    // Filter items that contain the search text (case-insensitive substring match)
                    var filteredItems = _suppliers
                        .Where(s => !string.IsNullOrEmpty(s.SupplierName) && 
                                    s.SupplierName.ToLower().Contains(filter))
                        .Select(s => s.SupplierName)
                        .ToList();
                    
                    // Preserve current text and caret position BEFORE modifying
                    string currentText = cb.Text ?? string.Empty;
                    int selectionStart = Math.Max(0, Math.Min(cb.SelectionStart, currentText.Length));
                    
                    // Update items safely
                    if (cb != null && !cb.IsDisposed)
                    {
                        // Close dropdown before modifying items to prevent AccessViolationException
                        bool wasDroppedDown = cb.DroppedDown;
                        if (wasDroppedDown)
                        {
                            cb.DroppedDown = false;
                        }
                        
                        try
                        {
                            cb.BeginUpdate();
                            cb.Items.Clear();
                            if (filteredItems.Count > 0)
                            {
                                cb.Items.AddRange(filteredItems.ToArray());
                            }
                            cb.EndUpdate();
                            
                            // Restore text and caret position AFTER updating items
                            _isFilteringVendorCombo = true;
                            try
                            {
                                cb.Text = currentText;
                                if (selectionStart >= 0 && selectionStart <= currentText.Length)
                                {
                                    cb.SelectionStart = selectionStart;
                                    cb.SelectionLength = 0;
                                }
                            }
                            finally
                            {
                                _isFilteringVendorCombo = false;
                            }
                            
                            // Reopen dropdown if it was open and we have filtered items
                            if (filteredItems.Count > 0 && wasDroppedDown)
                            {
                                cb.DroppedDown = true;
                            }
                        }
                        catch (ObjectDisposedException) { }
                        catch (AccessViolationException) { }
                        catch (InvalidOperationException) { }
                        catch { }
                    }
                }
                finally
                {
                    _isFilteringVendorCombo = false;
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch { }
        }
        
        private void CmbVendorName_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb == null || cb.IsDisposed) return;
                
                // If user types a printable character and dropdown is closed, open it
                if (!char.IsControl(e.KeyChar) && !cb.DroppedDown)
                {
                    if (cb.Items.Count == 0 && _suppliers != null && _suppliers.Count > 0)
                    {
                        RefreshAllVendorsInComboBox(cb);
                    }
                    if (cb.Items.Count > 0)
                    {
                        cb.DroppedDown = true;
                    }
                }
            }
            catch { }
        }
        
        private void CmbVendorName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb == null || cb.IsDisposed) return;
                
                if (e.KeyCode == Keys.Enter)
                {
                    // Get selected item or first item in filtered list
                    string selectedVendorName = null;
                    
                    // First priority: If user navigated with arrow keys, use selected item
                    if (cb.SelectedIndex >= 0 && cb.SelectedItem != null)
                    {
                        selectedVendorName = cb.SelectedItem.ToString();
                    }
                    // Second priority: If no selection but filtered items exist, use first filtered item
                    else if (cb.Items.Count > 0)
                    {
                        // Use first item in filtered list - this is the best match for what user typed
                        selectedVendorName = cb.Items[0].ToString();
                        // Also update the ComboBox to show this selection
                        try
                        {
                            cb.SelectedIndex = 0;
                            cb.Text = selectedVendorName;
                        }
                        catch { }
                    }
                    // Third priority: If no items but user typed something, try to find exact match
                    else if (!string.IsNullOrEmpty(cb.Text))
                    {
                        // Try to find vendor that matches the typed text
                        string searchText = cb.Text.Trim();
                        var matchedSupplier = _suppliers?.FirstOrDefault(s => 
                            (!string.IsNullOrEmpty(s.SupplierName) && 
                             s.SupplierName.Equals(searchText, StringComparison.OrdinalIgnoreCase)));
                        
                        if (matchedSupplier != null)
                        {
                            selectedVendorName = matchedSupplier.SupplierName;
                        }
                    }
                    
                    // If we found a vendor, commit the selection
                    if (!string.IsNullOrEmpty(selectedVendorName))
                    {
                        cb.Text = selectedVendorName;
                        cb.SelectedItem = selectedVendorName;
                        // This will trigger SelectedIndexChanged which populates vendor details
                    }
                    
                    e.Handled = true;
                }
            }
            catch { }
        }
        
        private void CmbVendorName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb == null || cb.IsDisposed) return;
                
                // Mark arrow keys as input keys so they work for navigation
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    e.IsInputKey = true;
                    
                    // If dropdown is closed and user presses arrow key, open it
                    if (!cb.DroppedDown && cb.Items.Count > 0)
                    {
                        cb.DroppedDown = true;
                    }
                }
            }
            catch { }
        }
        
        private void RefreshAllVendorsInComboBox(ComboBox cb)
        {
            if (cb == null || cb.IsDisposed || _suppliers == null || _suppliers.Count == 0)
                return;
            
            try
            {
                _isFilteringVendorCombo = true;
                
                // Close dropdown before modifying items to prevent AccessViolationException
                bool wasDroppedDown = cb.DroppedDown;
                if (wasDroppedDown)
                {
                    cb.DroppedDown = false;
                }
                
                // Temporarily disable AutoComplete during updates
                AutoCompleteMode originalAutoCompleteMode = cb.AutoCompleteMode;
                AutoCompleteSource originalAutoCompleteSource = cb.AutoCompleteSource;
                try
                {
                    cb.AutoCompleteMode = AutoCompleteMode.None;
                    cb.AutoCompleteSource = AutoCompleteSource.None;
                }
                catch { }
                
                bool updateStarted = false;
                bool layoutSuspended = false;
                
                try
                {
                    cb.SuspendLayout();
                    layoutSuspended = true;
                    
                    cb.BeginUpdate();
                    updateStarted = true;
                    
                    cb.Items.Clear();
                    
                    // Add all vendor names
                    foreach (var supplier in _suppliers)
                    {
                        if (!string.IsNullOrEmpty(supplier.SupplierName))
                        {
                            cb.Items.Add(supplier.SupplierName);
                        }
                    }
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                catch (AccessViolationException) { }
                catch { }
                finally
                {
                    // End update safely and resume layout
                    try
                    {
                        if (updateStarted && !cb.IsDisposed)
                        {
                            cb.EndUpdate();
                        }
                    }
                    catch { }
                    try
                    {
                        if (layoutSuspended && !cb.IsDisposed)
                        {
                            cb.ResumeLayout(false);
                        }
                    }
                    catch { }
                    
                    // Restore AutoComplete settings
                    try
                    {
                        if (!cb.IsDisposed)
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                }
                
                _isFilteringVendorCombo = false;
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (AccessViolationException) { }
            catch { }
            finally
            {
                _isFilteringVendorCombo = false;
            }
        }

        private void CmbProductName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Prevent recursive calls and memory issues
                if (_isCalculating)
                    return;

                _isCalculating = true;

                // Get current text
                string searchText = cmbProductName.Text?.Trim() ?? "";
                
                // Don't filter if text is too short (less than 2 characters)
                if (searchText.Length < 2)
                {
                    _isCalculating = false;
                    return;
                }

                // Use BeginUpdate/EndUpdate to prevent flickering and memory issues
                cmbProductName.BeginUpdate();
                
                try
                {
                    // Store current selection
                    int currentSelection = cmbProductName.SelectedIndex;
                    string currentText = cmbProductName.Text;
                    
                    // Clear items safely
                    cmbProductName.Items.Clear();
                    
                    // Filter products safely
                    if (_products != null && _products.Count > 0)
                    {
                        var filteredProducts = _products.Where(p => 
                            !string.IsNullOrEmpty(p.ProductName) && 
                            p.ProductName.ToLower().Contains(searchText.ToLower())).ToList();
                        
                        // Add filtered items
                        foreach (var product in filteredProducts)
                        {
                            if (!string.IsNullOrEmpty(product.ProductName))
                            {
                                cmbProductName.Items.Add(product.ProductName);
                            }
                        }
                    }
                    
                    // Restore text if no exact match
                    if (cmbProductName.Items.Count > 0)
                    {
                        cmbProductName.Text = currentText;
                        cmbProductName.SelectionStart = currentText.Length;
                        cmbProductName.SelectionLength = 0;
                    }
                }
                finally
                {
                    cmbProductName.EndUpdate();
                    _isCalculating = false;
                }
            }
            catch (Exception ex)
            {
                _isCalculating = false;
                ShowError($"Error filtering products: {ex.Message}");
            }
        }

        private void CmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbProductName.SelectedItem != null)
                {
                    string selectedProductName = cmbProductName.SelectedItem.ToString();
                    var selectedProduct = _products.FirstOrDefault(p => p.ProductName.Equals(selectedProductName, StringComparison.OrdinalIgnoreCase));
                    
                    if (selectedProduct != null)
                    {
                        // Display existing stock quantity
                        txtExistingStock.Text = selectedProduct.StockQuantity.ToString();
                        
                        // Store selected product for later use
                        _selectedProduct = selectedProduct;
                        
                        MarkFormDirty();
                    }
                }
                else
                {
                    txtExistingStock.Clear();
                    _selectedProduct = null;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading product details: {ex.Message}");
            }
        }


        private void InitializeDataGridView()
        {
            try
        {
            // Configure DataGridView for purchase items
                dgvPurchaseItems.AutoGenerateColumns = false;
                dgvPurchaseItems.AllowUserToAddRows = false;
                dgvPurchaseItems.AllowUserToDeleteRows = false;
                dgvPurchaseItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPurchaseItems.MultiSelect = false;
                dgvPurchaseItems.ReadOnly = false;
                dgvPurchaseItems.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                
                // Configure columns with proper widths
                colSrNo.Width = 50;
                colProductName.Width = 200;
                colProductCode.Width = 150;
                colQty.Width = 80;
                colPurchasePrice.Width = 150;
                colSalePrice.Width = 150;
                colTotal.Width = 150;
                colDelete.Width = 80;
            
            // Format currency columns
                colPurchasePrice.DefaultCellStyle.Format = "F2";
                colSalePrice.DefaultCellStyle.Format = "F2";
                colTotal.DefaultCellStyle.Format = "F2";
                
                // CRITICAL: Ensure ProductCode column is NOT formatted as numeric
                // It's a text column, so no numeric formatting should apply
                colProductCode.DefaultCellStyle.Format = string.Empty;
                colProductCode.DefaultCellStyle.NullValue = string.Empty;
                
                // Set up product name dropdown with searchable functionality
                SetupProductNameComboBox();
                
                // Set column properties
                colSrNo.ReadOnly = true;
                colProductCode.ReadOnly = true;
                colTotal.ReadOnly = true;
                
                // Set default values for new rows
                colQty.DefaultCellStyle.NullValue = "1";
                colPurchasePrice.DefaultCellStyle.NullValue = "0.00";
                colSalePrice.DefaultCellStyle.NullValue = "0.00";
                colTotal.DefaultCellStyle.NullValue = "0.00";
                
                // Ensure DataGridView starts empty
                dgvPurchaseItems.DataSource = null;
                dgvPurchaseItems.Rows.Clear();
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing DataGridView: {ex.Message}");
            }
        }

        private void SetupProductNameComboBox()
        {
            try
            {
                // Set DataSource on the COLUMN for initial population
                // This ensures the ComboBox has items when the grid is displayed
                if (_products == null || _products.Count == 0)
                {
                    colProductName.DataSource = null;
                    return;
                }

                colProductName.DataSource = _products;
                colProductName.DisplayMember = nameof(Product.ProductName);
                colProductName.ValueMember = nameof(Product.ProductID);
                colProductName.ValueType = typeof(int);
                colProductName.FlatStyle = FlatStyle.Flat;
            }
            catch (Exception ex)
            {
                ShowError($"Error setting up product name combo box: {ex.Message}");
            }
        }


        private void SetupEventHandlers()
        {
            // Form events
            this.Load += NewPurchase_Load;
            
            // Button event handlers
            btnAddItem.Click += BtnAddItem_Click;
            btnSavePurchase.Click += BtnSavePurchase_Click;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            btnClearForm.Click += BtnClearForm_Click;
            btnCancel.Click += BtnCancel_Click;
            
            // Vendor selection event
            cmbVendorName.SelectedIndexChanged += CmbVendorName_SelectedIndexChanged;
            
            // Calculation events
            txtDiscountPercent.TextChanged += CalculateTotals;
            txtTaxPercent.TextChanged += CalculateTotals;
            txtPaid.TextChanged += CalculateBalance;
            
            // DataGridView events
            dgvPurchaseItems.CellValueChanged += DgvPurchaseItems_CellValueChanged;
            dgvPurchaseItems.CellEndEdit += DgvPurchaseItems_CellEndEdit;
            dgvPurchaseItems.CellBeginEdit += DgvPurchaseItems_CellBeginEdit;
            dgvPurchaseItems.EditingControlShowing += DgvPurchaseItems_EditingControlShowing;
            dgvPurchaseItems.CurrentCellDirtyStateChanged += DgvPurchaseItems_CurrentCellDirtyStateChanged;
            dgvPurchaseItems.KeyDown += DgvPurchaseItems_KeyDown;
            dgvPurchaseItems.SelectionChanged += DgvPurchaseItems_SelectionChanged;
            dgvPurchaseItems.DataError += DgvPurchaseItems_DataError;
            dgvPurchaseItems.CellClick += DgvPurchaseItems_CellClick;
            dgvPurchaseItems.DataError += (s, e) => { e.ThrowException = false; };
            
            // Form events
            this.FormClosing += NewPurchase_FormClosing;
        }

        private void DgvPurchaseItems_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // Show purchase items summary when selection changes
                if (dgvPurchaseItems.SelectedRows.Count > 0)
                {
                    ShowPurchaseItemsDisplay();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error showing purchase items: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                // Handle DataGridView data errors gracefully
                e.ThrowException = false;
                
                // Log the error for debugging (silently)
                string errorMessage = $"DataGridView Error in row {e.RowIndex}, column {e.ColumnIndex}: {e.Exception.Message}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                
                // Handle error silently - no annoying popup messages
            }
            catch (Exception ex)
            {
                // Handle silently to avoid more popups
                System.Diagnostics.Debug.WriteLine($"Error handling DataGridView error: {ex.Message}");
            }
        }

        private void LoadInitialData()
        {
            try
            {
                LoadSuppliers();
                LoadProducts();
                LoadCategories();
                LoadBrands();
                DisplayPurchaseSummary();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading initial data: {ex.Message}");
            }
        }

        private void DisplayPurchaseSummary()
        {
            try
            {
                // Display purchase details
                DisplayPurchaseDetails();
            }
            catch (Exception ex)
            {
                // Don't show error for display update
            }
        }

        private void DisplayPurchaseDetails()
        {
            try
            {
                // Display detailed purchase information
                StringBuilder details = new StringBuilder();
                details.AppendLine("PURCHASE DETAILS:");
                details.AppendLine("=================");
                
                if (dgvPurchaseItems.Rows.Count > 0)
                {
                    details.AppendLine($"Invoice: {txtInvoiceNo.Text}");
                    details.AppendLine($"Date: {dtpInvoiceDate.Value:yyyy-MM-dd}");
                    details.AppendLine($"Vendor: {cmbVendorName.Text}");
                    details.AppendLine();
                    details.AppendLine("ITEMS PURCHASED:");
                    details.AppendLine("================");
                    
                    for (int i = 0; i < dgvPurchaseItems.Rows.Count; i++)
                    {
                        var row = dgvPurchaseItems.Rows[i];
                        if (row.Cells["colProductCode"].Value != null && 
                            !string.IsNullOrEmpty(row.Cells["colProductCode"].Value.ToString()))
                        {
                            string itemCode = row.Cells["colProductCode"].Value.ToString();
                            string itemName = row.Cells["colProductName"].Value?.ToString() ?? "";
                            string quantity = row.Cells["colQty"].Value?.ToString() ?? "0";
                            string price = row.Cells["colPurchasePrice"].Value?.ToString() ?? "0.00";
                            string total = row.Cells["colTotal"].Value?.ToString() ?? "0.00";
                            
                            details.AppendLine($"{i + 1}. {itemName} ({itemCode})");
                            details.AppendLine($"   Qty: {quantity} @ ${price} = ${total}");
                            details.AppendLine();
                        }
                    }
                }
                else
                {
                    details.AppendLine("No items added yet");
                }
                
                // Update the details display (if you have a textbox for this)
                // txtPurchaseDetails.Text = details.ToString();
            }
            catch (Exception ex)
            {
                // Don't show error for display update
            }
        }
        #endregion

        #region Data Loading
        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                
                // Use the refresh method to populate vendor dropdown (same pattern as products)
                RefreshAllVendorsInComboBox(cmbVendorName);
                
                cmbVendorName.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading suppliers: {ex.Message}");
            }
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productRepository.GetAllProducts();
                
                // Refresh the product ComboBox in DataGridView
                SetupProductNameComboBox();
                
                // Populate the main Product Name ComboBox
                PopulateProductNameComboBox();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading products: {ex.Message}");
            }
        }

        private void PopulateProductNameComboBox()
        {
            try
            {
                // Clear existing items
                cmbProductName.Items.Clear();
                
                // Add all product names to the dropdown
                if (_products != null && _products.Count > 0)
                {
                    foreach (var product in _products)
                    {
                        cmbProductName.Items.Add(product.ProductName);
                    }
                }

                // Update auto-complete custom source for better search
                var ac = new AutoCompleteStringCollection();
                if (_products != null && _products.Count > 0)
                {
                    foreach (var product in _products)
                    {
                        ac.Add(product.ProductName);
                    }
                }
                cmbProductName.AutoCompleteCustomSource = ac;
            }
            catch (Exception ex)
            {
                ShowError($"Error populating product name combo box: {ex.Message}");
            }
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryRepository.GetAllCategories();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading categories: {ex.Message}");
            }
        }

        private void LoadBrands()
        {
            try
            {
                _brands = _brandRepository.GetAllBrands();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading brands: {ex.Message}");
            }
        }
        #endregion
        
        #region Invoice Management
        private void GenerateInvoiceNumber()
        {
            try
            {
                // Generate unique invoice number with timestamp
                _invoiceNumber = $"PUR{DateTime.Now:yyyyMMdd}{DateTime.Now:HHmmss}";
                txtInvoiceNo.Text = _invoiceNumber;
                
                // Auto-generate invoice number on form load
                txtInvoiceNo.ReadOnly = true;
                txtInvoiceNo.BackColor = System.Drawing.Color.LightGray;
            }
            catch (Exception ex)
            {
                ShowError($"Error generating invoice number: {ex.Message}");
            }
        }
        #endregion

        #region Vendor Management
        private void CmbVendorName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbVendorName.SelectedItem != null)
                {
                    string selectedVendorName = cmbVendorName.SelectedItem.ToString();
                    var selectedSupplier = _suppliers.FirstOrDefault(s => s.SupplierName.Equals(selectedVendorName, StringComparison.OrdinalIgnoreCase));
                    
                    if (selectedSupplier != null)
                    {
                    txtVendorCode.Text = selectedSupplier.SupplierCode;
                    
                    // Set default payment terms based on supplier
                    SetDefaultPaymentTerms(selectedSupplier);
                    
                    MarkFormDirty();
                    }
                }
                else
                {
                    txtVendorCode.Clear();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading vendor details: {ex.Message}");
            }
        }

        private void SetDefaultPaymentTerms(Supplier supplier)
        {
            // Set default payment terms based on supplier preferences
            // This could be extended to read from supplier settings
            if (supplier != null)
            {
                // For now, just ensure a default is selected
                if (cmbPaymentTerms.SelectedIndex == -1)
                {
                    cmbPaymentTerms.SelectedIndex = 0; // Cash
                }
            }
        }
        #endregion

        #region Product Entry Management
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                // This method is no longer needed - products are added directly to the grid
                ShowInfo("Use 'Add Item' button to add products to the grid.");
            }
            catch (Exception ex)
            {
                ShowError($"Error adding product: {ex.Message}");
            }
        }

        private void BtnClearProduct_Click(object sender, EventArgs e)
        {
            try
            {
                // This method is no longer needed - products are managed in the grid
                ShowInfo("Use the Delete button in the grid to remove products.");
            }
            catch (Exception ex)
            {
                ShowError($"Error clearing product entry: {ex.Message}");
            }
        }
        #endregion

        #region Product Management
        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                // If an empty row already exists, just focus it instead of creating more
                int emptyRowIndex = GetFirstEmptyRowIndex();
                if (emptyRowIndex >= 0)
                {
                    FocusProductCell(emptyRowIndex);
                }
                else
                {
                    AddBlankRow();
                }
                MarkFormDirty();
            }
            catch (Exception ex)
            {
                ShowError($"Error adding item: {ex.Message}");
            }
        }

        private void AddBlankRow()
        {
            // Prevent duplicate empty rows - check if one already exists
            int emptyRowIndex = GetFirstEmptyRowIndex();
            if (emptyRowIndex >= 0)
            {
                // Empty row exists, just focus it instead of creating another
                FocusProductCell(emptyRowIndex);
                return;
            }

            // No empty row exists, create exactly one
            int rowIndex = dgvPurchaseItems.Rows.Add();
            var row = dgvPurchaseItems.Rows[rowIndex];
            
            // CRITICAL: Set values explicitly to prevent formatting issues
            row.Cells["colSrNo"].Value = rowIndex + 1;
            row.Cells["colProductName"].Value = null; // ComboBox expects null/DBNULL until selection
            
            // CRITICAL: Ensure ProductCode is empty string (NOT numeric, NOT formatted)
            // Clear any formatting that might apply numeric display
            row.Cells["colProductCode"].Value = DBNull.Value; // Use DBNull first, then set to empty
            row.Cells["colProductCode"].Value = string.Empty; // Explicitly empty string
            
            row.Cells["colQty"].Value = 1;
            row.Cells["colPurchasePrice"].Value = 0.00m;
            row.Cells["colSalePrice"].Value = 0.00m;
            row.Cells["colTotal"].Value = 0.00m;
            
            // Ensure row is editable now; product name becomes read-only only after selection
            row.Cells["colProductName"].ReadOnly = false;
            FocusProductCell(rowIndex);
        }

        private bool HasPendingBlankRow()
        {
            return GetFirstEmptyRowIndex() >= 0;
        }

        private int GetFirstEmptyRowIndex()
        {
            for (int i = 0; i < dgvPurchaseItems.Rows.Count; i++)
            {
                var val = dgvPurchaseItems.Rows[i].Cells["colProductName"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(val))
                {
                    return i;
                }
            }
            return -1;
        }

        private void FocusProductCell(int rowIndex)
        {
            try
            {
                dgvPurchaseItems.CurrentCell = dgvPurchaseItems.Rows[rowIndex].Cells["colProductName"];
                dgvPurchaseItems.BeginEdit(true);
                if (dgvPurchaseItems.EditingControl is ComboBox ec)
                {
                    ec.DroppedDown = true;
                }
            }
            catch { }
        }

        private void ShowAddItemInstructions()
        {
            try
            {
                StringBuilder instructions = new StringBuilder();
                instructions.AppendLine("📝 HOW TO ADD PRODUCTS TO PURCHASE:");
                instructions.AppendLine("===================================");
                instructions.AppendLine("1. Click 'Add Item' button");
                instructions.AppendLine("2. Enter Product Code (e.g., APP001)");
                instructions.AppendLine("3. Product Name will auto-fill (e.g., Apple)");
                instructions.AppendLine("4. Enter Quantity (e.g., 50)");
                instructions.AppendLine("5. Enter Purchase Price (e.g., 90)");
                instructions.AppendLine("6. Total will calculate automatically");
                instructions.AppendLine();
                instructions.AppendLine("📊 EXAMPLE:");
                instructions.AppendLine("Product: Apple");
                instructions.AppendLine("Quantity: 50 pcs");
                instructions.AppendLine("Purchase Price: 90.00");
                instructions.AppendLine("Total: 4,500.00");
                instructions.AppendLine();
                instructions.AppendLine("💡 TIPS:");
                instructions.AppendLine("- Use Tab to move between cells");
                instructions.AppendLine("- Press Enter to move to next row");
                instructions.AppendLine("- Press Delete to remove selected row");
                
                // Instructions removed to avoid annoying popups
            }
            catch (Exception ex)
            {
                ShowError($"Error showing instructions: {ex.Message}");
            }
        }

        private void ShowPurchaseItemsDisplay()
        {
            try
            {
                // Display current purchase items in a user-friendly format
                StringBuilder itemsDisplay = new StringBuilder();
                itemsDisplay.AppendLine("🛒 CURRENT PURCHASE ITEMS:");
                itemsDisplay.AppendLine("==========================");
                
                if (dgvPurchaseItems.Rows.Count > 0)
                {
                    decimal grandTotal = 0;
                    int validItems = 0;
                    
                    for (int i = 0; i < dgvPurchaseItems.Rows.Count; i++)
                    {
                        var row = dgvPurchaseItems.Rows[i];
                        if (row.Cells["colProductCode"].Value != null && 
                            !string.IsNullOrEmpty(row.Cells["colProductCode"].Value.ToString()))
                        {
                            string itemCode = row.Cells["colProductCode"].Value.ToString();
                            string itemName = row.Cells["colProductName"].Value?.ToString() ?? "";
                            string quantity = row.Cells["colQty"].Value?.ToString() ?? "0";
                            string price = row.Cells["colPurchasePrice"].Value?.ToString() ?? "0.00";
                            string total = row.Cells["colTotal"].Value?.ToString() ?? "0.00";
                            
                            itemsDisplay.AppendLine($"📦 {i + 1}. {itemName} ({itemCode})");
                            itemsDisplay.AppendLine($"   📊 Quantity: {quantity}");
                            itemsDisplay.AppendLine($"   💰 Purchase Price: ${price}");
                            itemsDisplay.AppendLine($"   💵 Total Amount: ${total}");
                            itemsDisplay.AppendLine();
                            
                            grandTotal += ParseDecimal(total);
                            validItems++;
                        }
                    }
                    
                    itemsDisplay.AppendLine("==========================");
                    itemsDisplay.AppendLine($"📈 Total Items: {validItems}");
                    itemsDisplay.AppendLine($"💰 Grand Total: ${grandTotal:F2}");
                }
                else
                {
                    itemsDisplay.AppendLine("❌ No items added yet");
                    itemsDisplay.AppendLine("Click 'Add Item' to start adding products");
                }
                
                // Show this in a message box so you can see what's being purchased
                // Purchase items summary removed to avoid annoying popups
            }
            catch (Exception ex)
            {
                ShowError($"Error displaying purchase items: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == dgvPurchaseItems.Columns["colProductName"].Index)
                {
                    // Ensure cell is editable (in case it was set to ReadOnly previously)
                    try
                    {
                        dgvPurchaseItems.Rows[e.RowIndex].Cells["colProductName"].ReadOnly = false;
                    }
                    catch { }
                    
                    // Open the dropdown right after entering edit mode
                    // Use BeginInvoke with shorter delay to open dropdown faster
                    if (this != null && !this.IsDisposed && this.IsHandleCreated)
                    {
                        // Try immediately first
                        try
                        {
                            if (dgvPurchaseItems.EditingControl is ComboBox ec && !ec.IsDisposed && IsComboBoxValid(ec))
                            {
                                // Ensure items are populated
                                if (ec.Items.Count == 0 && _products != null && _products.Count > 0)
                                {
                                    RefreshAllProductsInComboBox(ec);
                                }
                                if (ec.Items.Count > 0 && !ec.DroppedDown)
                                {
                                    ec.DroppedDown = true;
                                }
                            }
                        }
                        catch { }
                        
                        // Also use BeginInvoke as backup
                        this.BeginInvoke(new Action(() => {
                            try
                            {
                                if (this != null && !this.IsDisposed && 
                                    dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed &&
                                    dgvPurchaseItems.EditingControl is ComboBox ec && !ec.IsDisposed && IsComboBoxValid(ec))
                                {
                                    // Ensure items are populated
                                    if (ec.Items.Count == 0 && _products != null && _products.Count > 0)
                                    {
                                        RefreshAllProductsInComboBox(ec);
                                    }
                                    if (ec.Items.Count > 0 && !ec.DroppedDown)
                                    {
                                        ec.DroppedDown = true;
                                    }
                                }
                            }
                            catch (ObjectDisposedException) { }
                            catch (InvalidOperationException) { }
                            catch { }
                        }), null);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error setting up cell edit: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                // Validate DataGridView first
                if (dgvPurchaseItems == null || dgvPurchaseItems.IsDisposed) return;
                if (e.Control == null) return;
                
                if (dgvPurchaseItems.CurrentCell != null && dgvPurchaseItems.CurrentCell.ColumnIndex == dgvPurchaseItems.Columns["colProductName"].Index)
                {
                    var cb = e.Control as ComboBox;
                    if (cb == null) return;
                    
                    // Validate ComboBox before using it
                    if (cb.IsDisposed) return;
                    
                    _productNameEditingControl = cb;
                    
                    // Set properties safely
                    // Enable AutoComplete with DropDown style for better search experience
                    try
                    {
                        cb.DropDownStyle = ComboBoxStyle.DropDown;
                        cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        cb.AutoCompleteSource = AutoCompleteSource.ListItems;
                    }
                    catch (ObjectDisposedException) { return; }
                    catch (InvalidOperationException) { return; }
                    catch { return; }

                    // Unsubscribe from all events first to prevent duplicate handlers
                    // Do this safely to avoid exceptions
                    try
                    {
                        cb.SelectionChangeCommitted -= ProductCombo_SelectionChangeCommitted;
                        cb.SelectedIndexChanged -= ProductCombo_SelectionChangeCommitted;
                        cb.KeyDown -= ProductCombo_KeyDown;
                        cb.KeyUp -= ProductCombo_KeyUp;
                        cb.KeyPress -= ProductCombo_KeyPress;
                        cb.PreviewKeyDown -= ProductCombo_PreviewKeyDown;
                        cb.TextChanged -= ProductCombo_TextChanged;
                        cb.Leave -= ProductCombo_Leave;
                        cb.DropDown -= ProductCombo_DropDown;
                    }
                    catch { }

                    // Subscribe to events - Only commit on Enter or explicit selection
                    // Only subscribe if control is still valid
                    if (!cb.IsDisposed)
                    {
                        try
                        {
                            cb.SelectionChangeCommitted += ProductCombo_SelectionChangeCommitted; // Mouse click commits
                            cb.KeyDown += ProductCombo_KeyDown; // Enter key commits, arrows just navigate
                            cb.KeyUp += ProductCombo_KeyUp; // Dynamic filtering as user types (PRIMARY FILTERING)
                            cb.KeyPress += ProductCombo_KeyPress; // Detect typing to open dropdown
                            cb.PreviewKeyDown += ProductCombo_PreviewKeyDown;
                            // TextChanged disabled - using KeyUp instead to avoid conflicts
                            // cb.TextChanged += ProductCombo_TextChanged; // DISABLED - conflicts with KeyUp
                            cb.Leave += ProductCombo_Leave;
                            cb.DropDown += ProductCombo_DropDown; // Ensure all products shown when dropdown opens
                        }
                        catch (ObjectDisposedException) { return; }
                        catch (InvalidOperationException) { return; }
                        catch { return; }
                    }
                    
                    // Load ALL products initially - simple approach
                    // Clear DataSource first if exists
                    if (!cb.IsDisposed)
                    {
                        try
                        {
                            cb.DataSource = null;
                            cb.DisplayMember = null;
                            cb.ValueMember = null;
                        }
                        catch { }
                    }
                    
                    // Determine if this is a new/empty row - if so, always show all products
                    bool isEmptyRow = false;
                    int currentRowIndex = -1;
                    if (dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed && dgvPurchaseItems.CurrentCell != null)
                    {
                        try
                        {
                            currentRowIndex = dgvPurchaseItems.CurrentCell.RowIndex;
                            var currentValue = dgvPurchaseItems.CurrentCell.Value;
                            isEmptyRow = (currentValue == null || currentValue == DBNull.Value || 
                                        (currentValue is int && (int)currentValue == 0) ||
                                        string.IsNullOrWhiteSpace(currentValue.ToString()));
                        }
                        catch { isEmptyRow = true; }
                    }
                    else
                    {
                        isEmptyRow = true;
                    }
                    
                    // CRITICAL: Always populate with ALL products when editing starts
                    // This ensures dropdown is never empty, especially for new rows
                    if (IsComboBoxValid(cb) && _products != null && _products.Count > 0)
                    {
                        _isFilteringProductCombo = true;
                        try
                        {
                            // CRITICAL: Close dropdown BEFORE modifying items to prevent AccessViolationException
                            try
                            {
                                if (IsComboBoxValid(cb) && cb.DroppedDown)
                                {
                                    cb.DroppedDown = false;
                                }
                            }
                            catch (ObjectDisposedException) 
                            { 
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            catch (AccessViolationException) 
                            { 
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            catch { }
                            
                            // Re-validate after closing dropdown
                            if (!IsComboBoxValid(cb))
                            {
                                _isFilteringProductCombo = false;
                                return;
                            }
                            
                            // CRITICAL: Disable AutoComplete during updates (already disabled, but ensure it)
                            AutoCompleteMode originalAutoCompleteMode = cb.AutoCompleteMode;
                            AutoCompleteSource originalAutoCompleteSource = cb.AutoCompleteSource;
                            try
                            {
                                if (IsComboBoxValid(cb))
                                {
                                    cb.AutoCompleteMode = AutoCompleteMode.None;
                                    cb.AutoCompleteSource = AutoCompleteSource.None;
                                }
                            }
                            catch { }
                            
                            // Use both SuspendLayout and BeginUpdate for maximum safety
                            bool layoutSuspended = false;
                            bool updateStarted = false;
                            
                            try
                            {
                                if (IsComboBoxValid(cb))
                                {
                                    cb.SuspendLayout();
                                    layoutSuspended = true;
                                }
                            }
                            catch { }
                            
                            // Begin update safely
                            try
                            {
                                if (IsComboBoxValid(cb))
                                {
                                    cb.BeginUpdate();
                                    updateStarted = true;
                                }
                            }
                            catch (ObjectDisposedException) 
                            { 
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            catch (AccessViolationException) 
                            { 
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            catch 
                            { 
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            
                            // Re-validate after BeginUpdate
                            if (!IsComboBoxValid(cb))
                            {
                                if (updateStarted && IsComboBoxValid(cb))
                                {
                                    try { cb.EndUpdate(); } catch { }
                                }
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return;
                            }
                            
                            // Clear items safely
                            try
                            {
                                if (IsComboBoxValid(cb))
                                {
                                    cb.Items.Clear();
                                }
                            }
                            catch (ObjectDisposedException) 
                            { 
                                if (updateStarted && IsComboBoxValid(cb))
                                {
                                    try { cb.EndUpdate(); } catch { }
                                }
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            catch (AccessViolationException) 
                            { 
                                if (updateStarted && IsComboBoxValid(cb))
                                {
                                    try { cb.EndUpdate(); } catch { }
                                }
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            catch 
                            { 
                                if (updateStarted && IsComboBoxValid(cb))
                                {
                                    try { cb.EndUpdate(); } catch { }
                                }
                                if (layoutSuspended && IsComboBoxValid(cb))
                                {
                                    try { cb.ResumeLayout(false); } catch { }
                                }
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        cb.AutoCompleteMode = originalAutoCompleteMode;
                                        cb.AutoCompleteSource = originalAutoCompleteSource;
                                    }
                                }
                                catch { }
                                _isFilteringProductCombo = false;
                                return; 
                            }
                            
                            // ALWAYS populate with all products - TextChanged will filter if needed
                            foreach (var product in _products)
                            {
                                if (!IsComboBoxValid(cb)) break;
                                if (!string.IsNullOrEmpty(product.ProductName) && !string.IsNullOrEmpty(product.ProductCode))
                                {
                                    try
                                    {
                                        cb.Items.Add($"{product.ProductName} | {product.ProductCode}");
                                    }
                                    catch (ObjectDisposedException) { break; }
                                    catch (AccessViolationException) { break; }
                                    catch (InvalidOperationException) { break; }
                                    catch { break; }
                                }
                            }
                            
                            // End update safely and resume layout
                            if (IsComboBoxValid(cb))
                            {
                                try
                                {
                                    if (updateStarted)
                                    {
                                        cb.EndUpdate();
                                    }
                                }
                                catch { }
                                try
                                {
                                    if (layoutSuspended)
                                    {
                                        cb.ResumeLayout(false);
                                    }
                                }
                                catch { }
                            }
                            
                            // Restore AutoComplete settings (but it should remain None since we disabled it)
                            try
                            {
                                if (IsComboBoxValid(cb))
                                {
                                    // Keep it disabled - we handle filtering manually
                                    cb.AutoCompleteMode = AutoCompleteMode.None;
                                    cb.AutoCompleteSource = AutoCompleteSource.None;
                                }
                            }
                            catch { }
                            
                            // For empty rows, ensure text is cleared safely
                            if (isEmptyRow && IsComboBoxValid(cb))
                            {
                                try
                                {
                                    cb.SelectedIndex = -1;
                                    cb.Text = "";
                                }
                                catch { }
                            }
                        }
                        catch (ObjectDisposedException) { }
                        catch (AccessViolationException) { }
                        catch { }
                        finally
                        {
                            _isFilteringProductCombo = false;
                        }
                    }
                    
                    // Restore selection for existing rows (not empty rows)
                    if (!isEmptyRow && IsComboBoxValid(cb) && dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed && dgvPurchaseItems.CurrentCell != null)
                    {
                        try
                        {
                            var currentValue = dgvPurchaseItems.CurrentCell.Value;
                            if (currentValue != null)
                            {
                                Product currentProduct = null;
                                
                                // Check if currentValue is ProductID (int) or product name (string)
                                if (currentValue is int productID)
                                {
                                    currentProduct = _products?.FirstOrDefault(p => p.ProductID == productID);
                                }
                                else
                                {
                                    string currentText = currentValue.ToString();
                                    if (!string.IsNullOrWhiteSpace(currentText))
                                    {
                                        // Try to find by name or code
                                        currentProduct = _products?.FirstOrDefault(p => 
                                            p.ProductName.Equals(currentText, StringComparison.OrdinalIgnoreCase) ||
                                            p.ProductCode.Equals(currentText, StringComparison.OrdinalIgnoreCase));
                                    }
                                }
                                
                                if (currentProduct != null && IsComboBoxValid(cb))
                                {
                                    // Find and select matching item
                                    string searchText = $"{currentProduct.ProductName} | {currentProduct.ProductCode}";
                                    for (int i = 0; i < cb.Items.Count; i++)
                                    {
                                        if (!IsComboBoxValid(cb)) break;
                                        if (cb.Items[i].ToString().Equals(searchText, StringComparison.OrdinalIgnoreCase))
                                        {
                                            cb.SelectedIndex = i;
                                            // Also set text to show the selected product
                                            cb.Text = searchText;
                                            break;
                                        }
                                    }
                                }
                                else if (IsComboBoxValid(cb))
                                {
                                    cb.SelectedIndex = -1;
                                    cb.Text = "";
                                }
                            }
                            else if (IsComboBoxValid(cb))
                            {
                                cb.SelectedIndex = -1;
                                cb.Text = "";
                            }
                        }
                        catch (ObjectDisposedException) { }
                        catch (InvalidOperationException) { }
                        catch { }
                    }
                    else if (isEmptyRow && IsComboBoxValid(cb))
                    {
                        // For empty rows, clear text and ensure no selection
                        cb.SelectedIndex = -1;
                        cb.Text = "";
                    }
                    
                    _isFilteringProductCombo = false;
                    
                    // IMPORTANT: Don't auto-select on SelectedIndexChanged - only navigate
                    // Arrow keys should just navigate, not select
                }
            }
            catch (ObjectDisposedException)
            {
                // Safe ignore during rapid editor teardown
            }
            catch (Exception ex)
            {
                ShowError($"Error enabling product search: {ex.Message}");
            }
        }

        private void ProductCombo_Leave(object sender, EventArgs e)
        {
            try
            {
                // When user leaves the combo box without selecting, check if we should commit
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb) || dgvPurchaseItems == null || dgvPurchaseItems.IsDisposed || dgvPurchaseItems.CurrentCell == null) return;
                
                // If text matches a product, commit it
                if (!string.IsNullOrEmpty(cb.Text) && cb.SelectedIndex < 0)
                {
                    // Try to find matching product
                    string searchText = cb.Text.Trim();
                    if (searchText.Contains(" | "))
                    {
                        searchText = searchText.Split('|')[0].Trim();
                    }
                    
                    var matchingProduct = _products?.FirstOrDefault(p => 
                        p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase) ||
                        p.ProductCode.Equals(searchText, StringComparison.OrdinalIgnoreCase));
                    
                    if (matchingProduct != null)
                    {
                        int currentRow = dgvPurchaseItems.CurrentCell.RowIndex;
                        // Store ProductID in cell
                        dgvPurchaseItems.CurrentCell.Value = matchingProduct.ProductID;
                        dgvPurchaseItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
                        // Use BeginInvoke safely
                        if (this != null && !this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() => 
                            { 
                                if (this != null && !this.IsDisposed)
                                    CommitAndAddNextRow(currentRow); 
                            }));
                        }
                    }
                }
            }
            catch { }
        }

        private void DgvPurchaseItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                // DON'T auto-commit on arrow key navigation - only commit on explicit selection (Enter key or mouse)
                // When user presses arrow keys, SelectedIndex changes but we don't want to commit yet
                // Only commit when user explicitly confirms with Enter or mouse click
                // So we'll let SelectionChangeCommitted or Enter key handle the commit, not here
                // This prevents auto-selection when using arrow keys
            }
            catch { }
        }

        private void ProductCombo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb)) return;

                if (e.KeyCode == Keys.Enter)
                {
                    try
                    {
                        if (dgvPurchaseItems.CurrentCell != null &&
                            dgvPurchaseItems.CurrentCell.ColumnIndex == dgvPurchaseItems.Columns["colProductName"].Index)
                        {
                            int currentRow = dgvPurchaseItems.CurrentCell.RowIndex;
                        
                            // Get selected item or first item in filtered list
                            Product selectedProduct = null;
                            string selectedText = null;
                            
                            // First priority: If user navigated with arrow keys, use selected item
                            if (cb.SelectedIndex >= 0 && cb.SelectedItem != null)
                            {
                                selectedText = cb.SelectedItem.ToString();
                            }
                            // Second priority: If no selection but filtered items exist, use first filtered item
                            // CRITICAL: This ensures Enter key works when user types and presses Enter
                            else if (cb.Items.Count > 0)
                            {
                                // Use first item in filtered list - this is the best match for what user typed
                                selectedText = cb.Items[0].ToString();
                            }
                            // Third priority: If no items but user typed something, try to find exact match
                            else if (!string.IsNullOrEmpty(cb.Text))
                            {
                                // Try to find product that matches the typed text
                                string searchText = cb.Text.Trim();
                                var matchedProduct = _products?.FirstOrDefault(p => 
                                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                                    (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.Equals(searchText, StringComparison.OrdinalIgnoreCase)));
                                
                                if (matchedProduct != null)
                                {
                                    selectedText = $"{matchedProduct.ProductName} | {matchedProduct.ProductCode}";
                                }
                                else
                                {
                                    selectedText = searchText;
                                }
                            }
                            
                            if (!string.IsNullOrEmpty(selectedText))
                            {
                                // Extract product name and code from format "ProductName | ProductCode"
                                string productName = "";
                                string productCode = "";
                                
                                // Parse the selected text - it should be in format "ProductName | ProductCode"
                                if (selectedText.Contains(" | "))
                                {
                                    var parts = selectedText.Split(new[] { " | " }, StringSplitOptions.None);
                                    if (parts.Length >= 2)
                                    {
                                        productName = parts[0].Trim();
                                        productCode = parts[1].Trim();
                                    }
                                    else if (parts.Length == 1)
                                    {
                                        productName = parts[0].Trim();
                                    }
                                }
                                else if (selectedText.Contains("|"))
                                {
                                    var parts = selectedText.Split('|');
                                    if (parts.Length >= 2)
                                    {
                                        productName = parts[0].Trim();
                                        productCode = parts[1].Trim();
                                    }
                                    else if (parts.Length == 1)
                                    {
                                        productName = parts[0].Trim();
                                    }
                                }
                                else
                                {
                                    // No separator - treat as product name
                                    productName = selectedText.Trim();
                                }
                                
                                // CRITICAL: Match by ProductCode FIRST (should be unique)
                                // This ensures we get the exact product the user selected
                                if (!string.IsNullOrEmpty(productCode))
                                {
                                    selectedProduct = _products?.FirstOrDefault(p => 
                                        !string.IsNullOrEmpty(p.ProductCode) && 
                                        p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                                }
                                
                                // If not found by code, try matching by both name AND code together
                                if (selectedProduct == null && !string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productCode))
                                {
                                    selectedProduct = _products?.FirstOrDefault(p => 
                                        !string.IsNullOrEmpty(p.ProductName) && 
                                        !string.IsNullOrEmpty(p.ProductCode) &&
                                        p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                                        p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                                }
                                
                                // Last resort: match by name only (may have duplicates, but best we can do)
                                if (selectedProduct == null && !string.IsNullOrEmpty(productName))
                                {
                                    selectedProduct = _products?.FirstOrDefault(p => 
                                        !string.IsNullOrEmpty(p.ProductName) && 
                                        p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
                                }
                            }
                            
                            // Set the product in the cell
                            // CRITICAL: Column expects ProductID (int), and DisplayMember will show ProductName
                            if (selectedProduct != null)
                            {
                                // Set ProductID - this will trigger CellValueChanged which populates product details
                                dgvPurchaseItems.CurrentCell.Value = selectedProduct.ProductID;
                                
                                // Ensure the cell displays correctly by forcing a refresh
                                // The column's DisplayMember will show the ProductName automatically
                                dgvPurchaseItems.InvalidateCell(dgvPurchaseItems.CurrentCell);
                            }
                            else if (!string.IsNullOrEmpty(cb.Text))
                            {
                                // Try to find product one more time with the exact text
                                string searchText = cb.Text.Trim();
                                var fallbackProduct = _products?.FirstOrDefault(p => 
                                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                                    (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.Equals(searchText, StringComparison.OrdinalIgnoreCase)) ||
                                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0));
                                
                                if (fallbackProduct != null)
                                {
                                    dgvPurchaseItems.CurrentCell.Value = fallbackProduct.ProductID;
                                    dgvPurchaseItems.InvalidateCell(dgvPurchaseItems.CurrentCell);
                                }
                                else
                                {
                                    // Last resort: store as text if no match found
                                    dgvPurchaseItems.CurrentCell.Value = cb.Text.Trim();
                                }
                            }
                            
                            // CRITICAL: Store current row index before commit to ensure we don't access invalid row
                            int rowIndexBeforeCommit = currentRow;
                            
                            // Commit the edit - this triggers CellValueChanged which populates product details
                            // Use try-catch to handle any index issues during commit
                            try
                            {
                                dgvPurchaseItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
                                
                                // Force a refresh to ensure display updates (only if row still exists)
                                if (selectedProduct != null && dgvPurchaseItems.CurrentCell != null &&
                                    rowIndexBeforeCommit >= 0 && rowIndexBeforeCommit < dgvPurchaseItems.Rows.Count)
                                {
                                    dgvPurchaseItems.RefreshEdit();
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // Row was deleted/modified during commit, ignore
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Row index invalid, ignore
                            }
                            
                            // Automatically add new row and move to it
                            e.Handled = true;
                            
                            // Use BeginInvoke to ensure commit completes first
                            if (this != null && !this.IsDisposed && this.IsHandleCreated)
                            {
                                this.BeginInvoke(new Action(() => 
                                { 
                                    try
                                    {
                                        if (this != null && !this.IsDisposed && 
                                            dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed)
                                        {
                                            // Check if we need to add a new row (check if any row has empty/null product)
                                            // CRITICAL: Check for null, DBNull, 0, or empty string
                                            int emptyRowCount = dgvPurchaseItems.Rows
                                                .Cast<DataGridViewRow>()
                                                .Count(r => {
                                                    var val = r.Cells["colProductName"].Value;
                                                    if (val == null || val == DBNull.Value) return true;
                                                    if (val is int productId) return productId == 0;
                                                    return string.IsNullOrWhiteSpace(val.ToString());
                                                });
                                            
                                            // If last row was just filled, add a new empty row
                                            if (currentRow == dgvPurchaseItems.Rows.Count - 1 || emptyRowCount == 0)
                                            {
                                                AddBlankRow();
                                            }
                                            
                                            // Move to next row's product name cell
                                            int nextRow = currentRow + 1;
                                            if (nextRow < dgvPurchaseItems.Rows.Count)
                                            {
                                                // Move to next row - use BeginInvoke again to ensure current edit is fully committed
                                                this.BeginInvoke(new Action(() =>
                                                {
                                                    try
                                                    {
                                                        if (this != null && !this.IsDisposed && 
                                                            dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed &&
                                                            nextRow < dgvPurchaseItems.Rows.Count)
                                                        {
                                                            dgvPurchaseItems.CurrentCell = dgvPurchaseItems.Rows[nextRow].Cells["colProductName"];
                                                            dgvPurchaseItems.BeginEdit(true);
                                                            
                                                            // Auto-open dropdown for next row
                                                            if (dgvPurchaseItems.EditingControl is ComboBox nextCb && !nextCb.IsDisposed)
                                                            {
                                                                if (nextCb.Items.Count == 0 && _products != null && _products.Count > 0)
                                                                {
                                                                    RefreshAllProductsInComboBox(nextCb);
                                                                }
                                                                if (nextCb.Items.Count > 0 && !nextCb.DroppedDown)
                                                                {
                                                                    nextCb.DroppedDown = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch { }
                                                }), null);
                                            }
                                        }
                                    }
                                    catch { }
                                }));
                            }
                        }
                    }
                    catch { }
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    // Arrow keys - let ComboBox handle navigation naturally
                    // Don't handle it here - PreviewKeyDown already marked it as input key
                    // This allows the native ComboBox arrow key navigation to work
                    e.Handled = false;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    // Cancel editing
                    if (dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed)
                    {
                        dgvPurchaseItems.CancelEdit();
                    }
                    e.Handled = true;
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch { }
        }

        private void ProductCombo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb)) return;
                
                // If user types a printable character (not control keys), open dropdown if closed
                if (!char.IsControl(e.KeyChar) && !cb.DroppedDown)
                {
                    // Ensure items are populated first
                    if (cb.Items.Count == 0 && _products != null && _products.Count > 0)
                    {
                        RefreshAllProductsInComboBox(cb);
                    }
                    
                    // Open dropdown to show filtered results as user types
                    if (IsComboBoxValid(cb) && cb.Items.Count > 0)
                    {
                        cb.DroppedDown = true;
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch { }
        }

        private void ProductCombo_KeyUp(object sender, KeyEventArgs e)
        {
            // Prevent re-entrant calls
            if (_isFilteringProductCombo) return;
            
            try
            {
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb)) return;
                
                // Skip filtering for control keys (arrows, enter, etc.)
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || 
                    e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape || 
                    e.KeyCode == Keys.Tab || e.KeyCode == Keys.Left || 
                    e.KeyCode == Keys.Right)
                {
                    return;
                }
                
                // Handle Delete and Backspace - they should trigger filtering
                // (Enter is handled separately in KeyDown)
                
                _isFilteringProductCombo = true;
                
                try
                {
                    // Get the current filter text (what user has typed)
                    string filter = (cb.Text ?? string.Empty).ToLower().Trim();
                    
                    // If filter is empty, show all products
                    if (string.IsNullOrWhiteSpace(filter))
                    {
                        RefreshAllProductsInComboBox(cb);
                        if (IsComboBoxValid(cb) && cb.Items.Count > 0 && !cb.DroppedDown)
                        {
                            cb.DroppedDown = true;
                        }
                        return;
                    }
                    
                    // Filter products based on search text (contains match - works for "ap" -> "apple")
                    if (_products == null || _products.Count == 0) return;
                    
                    // Create list of all product strings
                    var allItems = _products
                        .Where(p => !string.IsNullOrEmpty(p.ProductName) && !string.IsNullOrEmpty(p.ProductCode))
                        .Select(p => $"{p.ProductName} | {p.ProductCode}")
                        .ToList();
                    
                    // Filter items that contain the search text (case-insensitive substring match)
                    // This works for "ap" -> "apple", "pod" -> "Pod 13", etc.
                    var filteredItems = allItems
                        .Where(x => !string.IsNullOrEmpty(x) && x.ToLower().Contains(filter))
                        .ToList();
                    
                    // Preserve current text and caret position BEFORE modifying
                    string currentText = cb.Text ?? string.Empty;
                    int selectionStart = Math.Max(0, Math.Min(cb.SelectionStart, currentText.Length));
                    
                    // Update items safely
                    if (IsComboBoxValid(cb))
                    {
                        // Close dropdown before modifying items to prevent AccessViolationException
                        bool wasDroppedDown = cb.DroppedDown;
                        if (wasDroppedDown)
                        {
                            cb.DroppedDown = false;
                        }
                        
                        try
                        {
                            cb.BeginUpdate();
                            cb.Items.Clear();
                            if (filteredItems.Count > 0)
                            {
                                cb.Items.AddRange(filteredItems.ToArray());
                            }
                            cb.EndUpdate();
                            
                            // Restore text and caret position AFTER updating items
                            // Use _isFilteringProductCombo flag to prevent TextChanged from interfering
                            _isFilteringProductCombo = true;
                            try
                            {
                                // Keep the user's typed text visible
                                cb.Text = currentText;
                                if (selectionStart >= 0 && selectionStart <= currentText.Length)
                                {
                                    cb.SelectionStart = selectionStart;
                                    cb.SelectionLength = 0;
                                }
                                
                                // CRITICAL: Don't auto-select first item here - it would overwrite user's text
                                // Instead, Enter handler will use Items[0] if nothing is selected
                                // This allows user to see their typed text while filtering happens
                            }
                            finally
                            {
                                _isFilteringProductCombo = false;
                            }
                            
                            // Reopen dropdown if it was open and we have filtered items
                            if (filteredItems.Count > 0 && wasDroppedDown)
                            {
                                cb.DroppedDown = true;
                            }
                        }
                        catch (ObjectDisposedException) { }
                        catch (AccessViolationException) { }
                        catch (InvalidOperationException) { }
                        catch { }
                    }
                }
                finally
                {
                    _isFilteringProductCombo = false;
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch { }
        }

        private void ProductCombo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb)) return;
                
                // Handle arrow keys to enable navigation in dropdown
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    // If dropdown is closed, open it and show all products
                    if (!cb.DroppedDown)
                    {
                        // Refresh items to show all products before opening
                        RefreshAllProductsInComboBox(cb);
                        if (IsComboBoxValid(cb) && cb.Items.Count > 0)
                        {
                            cb.DroppedDown = true;
                            // Select first item for down arrow, last for up arrow
                            if (e.KeyCode == Keys.Down && cb.Items.Count > 0)
                            {
                                cb.SelectedIndex = 0;
                            }
                            else if (e.KeyCode == Keys.Up && cb.Items.Count > 0)
                            {
                                cb.SelectedIndex = cb.Items.Count - 1;
                            }
                        }
                    }
                    // Mark as input key so DataGridView doesn't interfere
                    e.IsInputKey = true;
                }
                // Handle Escape to close dropdown
                else if (e.KeyCode == Keys.Escape)
                {
                    if (IsComboBoxValid(cb) && cb.DroppedDown)
                    {
                        cb.DroppedDown = false;
                        e.IsInputKey = true;
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch { }
        }

        private void ProductCombo_DropDown(object sender, EventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb) || _products == null) return;

                // Check if this is an empty row - if so, always show all products
                bool isEmptyRow = false;
                if (dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed && dgvPurchaseItems.CurrentCell != null)
                {
                    try
                    {
                        var currentValue = dgvPurchaseItems.CurrentCell.Value;
                        isEmptyRow = (currentValue == null || currentValue == DBNull.Value || 
                                    (currentValue is int && (int)currentValue == 0) ||
                                    string.IsNullOrWhiteSpace(currentValue.ToString()));
                    }
                    catch { isEmptyRow = true; }
                }
                else
                {
                    isEmptyRow = true;
                }

                // CRITICAL: When dropdown opens for empty row OR if text is empty, show ALL products
                // This ensures dropdown is never empty, especially for new rows
                if (isEmptyRow || string.IsNullOrWhiteSpace(cb.Text))
                {
                    RefreshAllProductsInComboBox(cb);
                }
                // If there's text and it's not an empty row, TextChanged has already filtered - just ensure dropdown is showing
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch { }
        }

        private bool IsComboBoxValid(ComboBox cb)
        {
            try
            {
                if (cb == null) return false;
                
                // First check IsDisposed - this is the safest check
                bool disposed = false;
                try
                {
                    disposed = cb.IsDisposed;
                }
                catch
                {
                    return false; // If we can't even check IsDisposed, control is invalid
                }
                
                if (disposed) return false;
                
                // Check if handle is created
                bool handleCreated = false;
                try
                {
                    handleCreated = cb.IsHandleCreated;
                }
                catch
                {
                    return false;
                }
                
                // Try to access a property to verify it's still valid (without side effects)
                try
                {
                    var test = cb.DropDownStyle;
                    // Check if Items collection is accessible without modifying it
                    var count = cb.Items.Count;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
                catch (AccessViolationException)
                {
                    return false;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch
                {
                    return false;
                }
                
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (AccessViolationException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void RefreshAllProductsInComboBox(ComboBox cb)
        {
            try
            {
                // CRITICAL: Validate before ANY operation
                if (_isFilteringProductCombo || !IsComboBoxValid(cb) || _products == null) return;

                _isFilteringProductCombo = true;

                // CRITICAL: Close dropdown BEFORE modifying items to prevent AccessViolationException
                try
                {
                    if (IsComboBoxValid(cb) && cb.DroppedDown)
                    {
                        cb.DroppedDown = false;
                    }
                }
                catch (ObjectDisposedException) 
                { 
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch (AccessViolationException) 
                { 
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch { }
                
                // Re-validate after closing dropdown
                if (!IsComboBoxValid(cb)) 
                { 
                    _isFilteringProductCombo = false;
                    return; 
                }

                // CRITICAL: Disable AutoComplete during updates
                AutoCompleteMode originalAutoCompleteMode = cb.AutoCompleteMode;
                AutoCompleteSource originalAutoCompleteSource = cb.AutoCompleteSource;
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.AutoCompleteMode = AutoCompleteMode.None;
                        cb.AutoCompleteSource = AutoCompleteSource.None;
                    }
                }
                catch { }

                // Clear DataSource first if it exists - do it safely
                try
                {
                    if (cb.DataSource != null && IsComboBoxValid(cb))
                    {
                        cb.DataSource = null;
                        cb.DisplayMember = null;
                        cb.ValueMember = null;
                    }
                }
                catch (ObjectDisposedException) 
                { 
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch (AccessViolationException) 
                { 
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch 
                { 
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }

                // Re-validate after clearing DataSource
                if (!IsComboBoxValid(cb)) 
                { 
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }

                // Use both SuspendLayout and BeginUpdate for maximum safety
                bool layoutSuspended = false;
                bool updateStarted = false;
                
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.SuspendLayout();
                        layoutSuspended = true;
                    }
                }
                catch { }

                // Begin update safely
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.BeginUpdate();
                        updateStarted = true;
                    }
                }
                catch (ObjectDisposedException) 
                { 
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch (AccessViolationException) 
                { 
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch 
                { 
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                
                // Clear items safely
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.Items.Clear();
                    }
                }
                catch (ObjectDisposedException) 
                { 
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch (AccessViolationException) 
                { 
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                catch 
                { 
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    _isFilteringProductCombo = false;
                    return; 
                }
                
                // Add all products with validation
                foreach (var product in _products)
                {
                    if (!IsComboBoxValid(cb)) break; // Check again during loop
                    if (!string.IsNullOrEmpty(product.ProductName) && !string.IsNullOrEmpty(product.ProductCode))
                    {
                        try
                        {
                            cb.Items.Add($"{product.ProductName} | {product.ProductCode}");
                        }
                        catch (ObjectDisposedException) { break; }
                        catch (AccessViolationException) { break; }
                        catch (InvalidOperationException) { break; }
                        catch { break; }
                    }
                }
                
                // End update safely and resume layout
                if (IsComboBoxValid(cb))
                {
                    try
                    {
                        if (updateStarted)
                        {
                            cb.EndUpdate();
                        }
                    }
                    catch { }
                    try
                    {
                        if (layoutSuspended)
                        {
                            cb.ResumeLayout(false);
                        }
                    }
                    catch { }
                }
                
                // Restore AutoComplete settings
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.AutoCompleteMode = originalAutoCompleteMode;
                        cb.AutoCompleteSource = originalAutoCompleteSource;
                    }
                }
                catch { }
                
                // Don't auto-select - let arrow keys handle navigation
                // When user presses down arrow, ComboBox will naturally select the first item
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (AccessViolationException) { }
            catch { }
            finally
            {
                _isFilteringProductCombo = false;
            }
        }

        private void ProductCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                // This is triggered only when user explicitly selects (mouse click on dropdown item)
                // NOT triggered by arrow key navigation
                if (dgvPurchaseItems == null || dgvPurchaseItems.IsDisposed || dgvPurchaseItems.CurrentCell == null) return;
                
                var cb = sender as ComboBox;
                if (!IsComboBoxValid(cb)) return;
                
                int currentRow = dgvPurchaseItems.CurrentCell.RowIndex;

                if (dgvPurchaseItems.CurrentCell.ColumnIndex == dgvPurchaseItems.Columns["colProductName"].Index)
                {
                    // Handle selection (mouse click on dropdown item)
                    Product selectedProduct = null;
                    string selectedText = null;
                    
                    // First priority: Use SelectedItem if available (most reliable)
                    if (cb.SelectedItem != null && cb.SelectedIndex >= 0)
                    {
                        selectedText = cb.SelectedItem.ToString();
                    }
                    // Second priority: Use Text if it looks like a product selection
                    else if (!string.IsNullOrEmpty(cb.Text) && cb.Text.Contains("|"))
                    {
                        selectedText = cb.Text.Trim();
                    }
                    // Third priority: Use Text as search term
                    else if (!string.IsNullOrEmpty(cb.Text))
                    {
                        selectedText = cb.Text.Trim();
                    }
                    
                    if (!string.IsNullOrEmpty(selectedText))
                    {
                        // Extract product name and code from format "ProductName | ProductCode"
                        string productName = "";
                        string productCode = "";
                        
                        // Parse the selected text - it should be in format "ProductName | ProductCode"
                        if (selectedText.Contains(" | "))
                        {
                            var parts = selectedText.Split(new[] { " | " }, StringSplitOptions.None);
                            if (parts.Length >= 2)
                            {
                                productName = parts[0].Trim();
                                productCode = parts[1].Trim();
                            }
                            else if (parts.Length == 1)
                            {
                                productName = parts[0].Trim();
                            }
                        }
                        else if (selectedText.Contains("|"))
                        {
                            var parts = selectedText.Split('|');
                            if (parts.Length >= 2)
                            {
                                productName = parts[0].Trim();
                                productCode = parts[1].Trim();
                            }
                            else if (parts.Length == 1)
                            {
                                productName = parts[0].Trim();
                            }
                        }
                        else
                        {
                            // No separator - treat as product name
                            productName = selectedText.Trim();
                        }
                        
                        // CRITICAL: Match by ProductCode FIRST (should be unique)
                        // This ensures we get the exact product the user selected
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            selectedProduct = _products?.FirstOrDefault(p => 
                                !string.IsNullOrEmpty(p.ProductCode) && 
                                p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                        }
                        
                        // If not found by code, try matching by both name AND code together
                        if (selectedProduct == null && !string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productCode))
                        {
                            selectedProduct = _products?.FirstOrDefault(p => 
                                !string.IsNullOrEmpty(p.ProductName) && 
                                !string.IsNullOrEmpty(p.ProductCode) &&
                                p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                                p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                        }
                        
                        // Last resort: match by name only (may have duplicates, but best we can do)
                        if (selectedProduct == null && !string.IsNullOrEmpty(productName))
                        {
                            selectedProduct = _products?.FirstOrDefault(p => 
                                !string.IsNullOrEmpty(p.ProductName) && 
                                p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
                        }
                        
                        if (selectedProduct != null)
                        {
                            // Store ProductID in cell - this will trigger CellValueChanged
                            dgvPurchaseItems.CurrentCell.Value = selectedProduct.ProductID;
                        }
                        else
                        {
                            // Product not found - store as text (shouldn't happen, but handle gracefully)
                            dgvPurchaseItems.CurrentCell.Value = productName;
                        }
                    }

                    // Commit safely; do not touch the editing control after this
                    dgvPurchaseItems.CommitEdit(DataGridViewDataErrorContexts.Commit);

                    // Defer row-advance and new-row logic until editor is disposed
                    this.BeginInvoke(new Action(() =>
                    {
                        CommitAndAddNextRow(currentRow);
                    }));
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (Exception ex)
            {
                // Only show error if control is still valid
                try
                {
                    if (dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed)
                    {
                        ShowError($"Error committing product selection: {ex.Message}");
                    }
                }
                catch { }
            }
        }

        private void CommitAndAddNextRow(int currentRow)
        {
            try
            {
                // Check if form and DataGridView are still valid
                if (this.IsDisposed || dgvPurchaseItems == null || dgvPurchaseItems.IsDisposed)
                    return;

                // Validate row index
                if (currentRow < 0 || currentRow >= dgvPurchaseItems.Rows.Count)
                    return;

                // DON'T lock product cell - allow re-editing if user wants to change product
                // Keep it editable so dropdown can open again when clicked
                // try { dgvPurchaseItems.Rows[currentRow].Cells["colProductName"].ReadOnly = true; } catch { }

                // Move to Qty on the same row
                try
                {
                    if (currentRow < dgvPurchaseItems.Rows.Count)
                    {
                        var qtyCell = dgvPurchaseItems.Rows[currentRow].Cells["colQty"];
                        dgvPurchaseItems.CurrentCell = qtyCell;
                        dgvPurchaseItems.BeginEdit(true);
                        if (dgvPurchaseItems.EditingControl is TextBox tb && !tb.IsDisposed)
                        {
                            tb.SelectAll();
                        }
                    }
                }
                catch { }

                // Ensure exactly ONE empty row exists - add only if none exists
                if (dgvPurchaseItems != null && !dgvPurchaseItems.IsDisposed)
                {
                    int emptyRowCount = dgvPurchaseItems.Rows
                        .Cast<DataGridViewRow>()
                        .Count(r => string.IsNullOrWhiteSpace(Convert.ToString(r.Cells["colProductName"].Value)));
                    
                    // If no empty row exists, add exactly one
                    if (emptyRowCount == 0)
                    {
                        AddBlankRow();
                    }
                    // If more than one empty row exists, remove extras to keep only one
                    else if (emptyRowCount > 1)
                    {
                        RemoveExtraEmptyRows();
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (Exception ex)
            {
                // Only show error if form is still valid
                try
                {
                    if (this != null && !this.IsDisposed)
                    {
                        ShowError($"Error committing and advancing: {ex.Message}");
                    }
                }
                catch { }
            }
        }

        private void ProductCombo_TextChanged(object sender, EventArgs e)
        {
            // DISABLED: This handler conflicts with KeyUp filtering
            // KeyUp is now the primary filtering mechanism
            // Return immediately to prevent conflicts
            return;
            
            // OLD CODE BELOW - KEPT FOR REFERENCE BUT NOT EXECUTED
            /*
            // Prevent re-entrant calls
            if (_isFilteringProductCombo) return;
            
            lock (_filterLock)
            {
                if (_isFilteringProductCombo) return;
                
                try
                {
                    _isFilteringProductCombo = true;
                    
                    var cb = sender as ComboBox;
                    if (cb == null) return;
                    
                    // CRITICAL: Check if control is disposed before any access
                    try
                    {
                        if (cb.IsDisposed) return;
                    }
                    catch
                    {
                        return; // Can't even check if disposed - control is invalid
                    }
                    
                    // Double-check validity
                    if (!IsComboBoxValid(cb)) return;
                    
                    if (_products == null || _products.Count == 0) return;

                    // Get text safely and preserve dropdown state
                    string text = string.Empty;
                    bool wasDroppedDown = false;
                    try
                    {
                        text = cb.Text ?? string.Empty;
                        wasDroppedDown = cb.DroppedDown; // Remember if dropdown was open
                        
                        // CRITICAL: If user is typing (text not empty) and dropdown is closed, open it
                        // This ensures dropdown opens automatically when user starts typing
                        if (!string.IsNullOrWhiteSpace(text) && !wasDroppedDown)
                        {
                            // Open dropdown to show filtered results
                            wasDroppedDown = true; // Mark as should be open
                        }
                    }
                    catch (ObjectDisposedException) { return; }
                    catch (AccessViolationException) { return; }
                    catch { return; }
                    
                    // Filter products based on text input with priority for starts-with matches
                    // CRITICAL: Don't trim the search text - preserve it exactly as typed for display
                    // Only trim when doing the actual search comparison
                    List<string> filtered = new List<string>();
                    try
                    {
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            // If text is empty, show all products
                            filtered = _products
                                .Where(p => !string.IsNullOrEmpty(p.ProductName))
                                .Select(p => $"{p.ProductName} | {p.ProductCode}")
                                .ToList();
                        }
                        else
                        {
                            // Trim only for search comparison, but preserve original text for display
                            string searchTerm = text.Trim();
                            
                            // Priority 1: Products where name starts with the search text
                            var startsWithMatches = _products
                                .Where(p => 
                                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                                    (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)))
                                .Select(p => $"{p.ProductName} | {p.ProductCode}")
                                .ToList();
                            
                            // Priority 2: Products where name or code contains the search text (but doesn't start with it)
                            var containsMatches = _products
                                .Where(p => 
                                    (!string.IsNullOrEmpty(p.ProductName) && !p.ProductName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) && p.ProductName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                    (!string.IsNullOrEmpty(p.ProductCode) && !p.ProductCode.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) && p.ProductCode.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0))
                                .Select(p => $"{p.ProductName} | {p.ProductCode}")
                                .ToList();
                            
                            // Combine: starts-with matches first, then contains matches
                            filtered = startsWithMatches.Concat(containsMatches).Distinct().ToList();
                        }
                    }
                    catch { return; }

                    // Check validity again before modifying Items
                    if (!IsComboBoxValid(cb)) return;
                    
                    // Preserve caret position safely
                    int selStart = 0;
                    try
                    {
                        selStart = cb.SelectionStart;
                    }
                    catch { }

                    // Update items collection safely - use form's Invoke instead of combobox
                    // CRITICAL: Don't use cb.BeginInvoke as combobox may be disposed
                    if (this != null && !this.IsDisposed && this.IsHandleCreated && cb.InvokeRequired)
                    {
                        try
                        {
                            this.BeginInvoke(new Action(() => 
                            {
                                try
                                {
                                    if (IsComboBoxValid(cb))
                                    {
                                        UpdateComboBoxItems(cb, filtered, selStart, wasDroppedDown, text);
                                    }
                                }
                                catch { }
                            }));
                            return;
                        }
                        catch (ObjectDisposedException) { return; }
                        catch (InvalidOperationException) { return; }
                        catch { return; }
                    }
                    
                    // Already on UI thread
                    UpdateComboBoxItems(cb, filtered, selStart, wasDroppedDown, text);
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                catch (AccessViolationException) { }
                catch { }
                finally
                {
                    _isFilteringProductCombo = false;
                }
            }
            */
        }

        private void UpdateComboBoxItems(ComboBox cb, List<string> filtered, int selStart, bool keepDropdownOpen = true, string searchText = "")
        {
            try
            {
                // CRITICAL: Validate before ANY access
                if (!IsComboBoxValid(cb)) return;
                
                // CRITICAL: Use the searchText parameter if provided, otherwise get from ComboBox
                // This ensures we use the exact text from TextChanged event, not what might be in the ComboBox
                string preservedText = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        // Use the text passed from TextChanged event - this is the actual typed text
                        preservedText = searchText;
                    }
                    else if (IsComboBoxValid(cb))
                    {
                        // Fallback: get from ComboBox if no parameter provided
                        preservedText = cb.Text ?? string.Empty;
                    }
                }
                catch { }
                
                // CRITICAL: Close dropdown BEFORE modifying items to prevent AccessViolationException
                bool wasDroppedDown = false;
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        wasDroppedDown = cb.DroppedDown;
                        if (wasDroppedDown)
                        {
                            // Close dropdown safely before modifying items
                            // This prevents AccessViolationException when modifying Items collection
                            cb.DroppedDown = false;
                        }
                    }
                }
                catch (ObjectDisposedException) { return; }
                catch (AccessViolationException) { return; }
                catch (InvalidOperationException) { return; }
                catch { }
                
                // Re-validate after closing dropdown
                if (!IsComboBoxValid(cb)) return;
                
                // CRITICAL: Temporarily disable AutoComplete to prevent conflicts during item updates
                AutoCompleteMode originalAutoCompleteMode = cb.AutoCompleteMode;
                AutoCompleteSource originalAutoCompleteSource = cb.AutoCompleteSource;
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.AutoCompleteMode = AutoCompleteMode.None;
                        cb.AutoCompleteSource = AutoCompleteSource.None;
                    }
                }
                catch { }
                
                // Use both SuspendLayout and BeginUpdate for maximum safety
                bool layoutSuspended = false;
                bool updateStarted = false;
                
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.SuspendLayout();
                        layoutSuspended = true;
                    }
                }
                catch { }
                
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.BeginUpdate();
                        updateStarted = true;
                    }
                }
                catch (ObjectDisposedException) 
                { 
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return; 
                }
                catch (AccessViolationException) 
                { 
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return; 
                }
                catch (InvalidOperationException) 
                { 
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return; 
                }
                catch { return; }
                
                // Check again after BeginUpdate
                if (!IsComboBoxValid(cb))
                {
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return;
                }
                
                // Clear items safely
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.Items.Clear();
                    }
                }
                catch (ObjectDisposedException) 
                { 
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return; 
                }
                catch (AccessViolationException) 
                { 
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return; 
                }
                catch 
                { 
                    if (updateStarted && IsComboBoxValid(cb))
                    {
                        try { cb.EndUpdate(); } catch { }
                    }
                    if (layoutSuspended && IsComboBoxValid(cb))
                    {
                        try { cb.ResumeLayout(false); } catch { }
                    }
                    try
                    {
                        if (IsComboBoxValid(cb))
                        {
                            cb.AutoCompleteMode = originalAutoCompleteMode;
                            cb.AutoCompleteSource = originalAutoCompleteSource;
                        }
                    }
                    catch { }
                    return; 
                }
                
                // Add items with validation
                foreach (var name in filtered)
                {
                    if (!IsComboBoxValid(cb)) break;
                    try
                    {
                        cb.Items.Add(name);
                    }
                    catch (ObjectDisposedException) { break; }
                    catch (InvalidOperationException) { break; }
                    catch (AccessViolationException) { break; }
                    catch { break; }
                }
                
                // Safely end update and resume layout
                if (IsComboBoxValid(cb))
                {
                    try
                    {
                        if (updateStarted)
                        {
                            cb.EndUpdate();
                        }
                    }
                    catch { }
                    try
                    {
                        if (layoutSuspended)
                        {
                            cb.ResumeLayout(false);
                        }
                    }
                    catch { }
                }
                
                // Restore AutoComplete settings
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.AutoCompleteMode = originalAutoCompleteMode;
                        cb.AutoCompleteSource = originalAutoCompleteSource;
                    }
                }
                catch { }
                
                // CRITICAL: Re-validate before ANY property access
                if (!IsComboBoxValid(cb)) return;
                
                // CRITICAL: Restore the preserved text (user's typed input) after updating items
                // Only set text if it's different to avoid triggering another TextChanged event
                try
                {
                    if (IsComboBoxValid(cb) && !string.IsNullOrEmpty(preservedText))
                    {
                        string currentText = cb.Text ?? string.Empty;
                        
                        // Only update text if it's different to prevent recursive TextChanged events
                        if (!currentText.Equals(preservedText, StringComparison.Ordinal))
                        {
                            // Temporarily disable filtering flag to prevent recursive TextChanged
                            bool wasFiltering = _isFilteringProductCombo;
                            _isFilteringProductCombo = true;
                            
                            try
                            {
                                // Restore the text the user typed (the full search string)
                                cb.Text = preservedText;
                                
                                // Restore caret position to end of text so user can continue typing
                                if (preservedText.Length > 0)
                                {
                                    cb.SelectionStart = preservedText.Length;
                                    cb.SelectionLength = 0;
                                }
                            }
                            finally
                            {
                                // Restore filtering flag
                                _isFilteringProductCombo = wasFiltering;
                            }
                        }
                        else
                        {
                            // Text is already correct, just restore caret position
                            if (preservedText.Length > 0)
                            {
                                try
                                {
                                    cb.SelectionStart = Math.Min(preservedText.Length, cb.SelectionStart);
                                    cb.SelectionLength = 0;
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }

                // Don't auto-select - let user navigate with arrow keys
                try
                {
                    if (IsComboBoxValid(cb))
                    {
                        cb.SelectedIndex = -1;
                    }
                }
                catch { }

                // CRITICAL: Always re-open dropdown AFTER all items are updated if we have items
                // This ensures the dropdown stays open for navigation and selection during search
                // OR if user is typing (keepDropdownOpen is true)
                try
                {
                    if (IsComboBoxValid(cb) && filtered.Count > 0 && keepDropdownOpen)
                    {
                        // Always reopen dropdown after filtering to allow navigation
                        // Use BeginInvoke to ensure it happens after current TextChanged completes
                        if (this != null && !this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    if (IsComboBoxValid(cb) && cb.Items.Count > 0)
                                    {
                                        // Always reopen to keep dropdown visible during search
                                        // This ensures dropdown opens automatically when user starts typing
                                        cb.DroppedDown = true;
                                    }
                                }
                                catch (ObjectDisposedException) { }
                                catch (AccessViolationException) { }
                                catch (InvalidOperationException) { }
                                catch { }
                            }), null);
                        }
                        else
                        {
                            // Fallback: open directly if BeginInvoke not available
                            try
                            {
                                if (IsComboBoxValid(cb) && cb.Items.Count > 0)
                                {
                                    // Open dropdown to show search results
                                    cb.DroppedDown = true;
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch (ObjectDisposedException) { }
                catch (AccessViolationException) { }
                catch (InvalidOperationException) { }
                catch { }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (AccessViolationException) { }
            catch { }
        }

        private void SearchProductsRealTime(string searchText, int rowIndex)
        {
            try
            {
                if (string.IsNullOrEmpty(searchText) || searchText.Length < 2)
                    return;
                
                // Search products by code or name
                var matchingProducts = _products.Where(p => 
                    p.ProductCode.ToLower().Contains(searchText.ToLower()) ||
                    p.ProductName.ToLower().Contains(searchText.ToLower())
                ).Take(5).ToList();
                
                if (matchingProducts.Any())
                {
                    // Show product suggestions
                    ShowProductSuggestions(matchingProducts, rowIndex);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error searching products: {ex.Message}");
            }
        }

        private void ShowProductSuggestions(List<Product> products, int rowIndex)
        {
            try
            {
                StringBuilder suggestions = new StringBuilder();
                suggestions.AppendLine("🔍 PRODUCT SUGGESTIONS:");
                suggestions.AppendLine("======================");
                
                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    suggestions.AppendLine($"{i + 1}. {product.ProductName} ({product.ProductCode})");
                    suggestions.AppendLine($"   Stock: {product.StockQuantity} | Cost: ${product.CostPrice:F2}");
                }
                
                suggestions.AppendLine();
                suggestions.AppendLine("💡 Tip: Type the product code or name to auto-fill");
                
                // You can show this in a tooltip or status bar
                // For now, we'll just log it
                System.Diagnostics.Debug.WriteLine(suggestions.ToString());
            }
            catch (Exception ex)
            {
                ShowError($"Error showing product suggestions: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // CRITICAL: Prevent re-entrant calls and validate immediately
            if (_isCalculating)
            {
                return; // Already processing, prevent re-entry
            }
            
            try
            {
                // CRITICAL: Validate DataGridView is valid first
                if (dgvPurchaseItems == null || dgvPurchaseItems.IsDisposed)
                {
                    return;
                }
                
                // CRITICAL: Validate row index before accessing rows
                if (e.RowIndex < 0 || e.RowIndex >= dgvPurchaseItems.Rows.Count)
                {
                    return; // Invalid row index, ignore silently
                }
                
                if (e.ColumnIndex >= 0)
                {
                    // Set flag to prevent re-entrant calls
                    _isCalculating = true;
                    // Auto-fill product information when product name is selected
                    if (e.ColumnIndex == 1) // Product Name column
                    {
                        // CRITICAL: Double-check row index is still valid before accessing
                        if (e.RowIndex < 0 || e.RowIndex >= dgvPurchaseItems.Rows.Count)
                        {
                            return;
                        }
                        
                        // CRITICAL: Safely access the row with additional try-catch for race conditions
                        DataGridViewRow row = null;
                        try
                        {
                            row = dgvPurchaseItems.Rows[e.RowIndex];
                            if (row == null || row.IsNewRow)
                            {
                                return; // Row doesn't exist or is a placeholder
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return; // Row index invalid
                        }
                        catch (IndexOutOfRangeException)
                        {
                            return; // Row index out of range
                        }
                        
                        // Value is ProductID due to column binding; fall back to string parsing if needed
                        var rawVal = row?.Cells["colProductName"]?.Value;
                        if (rawVal == null)
                        {
                            return; // No value to process
                        }
                        Product product = null;
                        if (rawVal is int id)
                        {
                            product = _products.FirstOrDefault(p => p.ProductID == id);
                        }
                        else
                        {
                            string productNameRaw = rawVal?.ToString();
                            string selectedName = productNameRaw;
                            string selectedCode = null;
                            if (!string.IsNullOrEmpty(productNameRaw) && productNameRaw.Contains("|"))
                            {
                                var parts = productNameRaw.Split('|');
                                selectedName = parts[0].Trim();
                                if (parts.Length > 1) selectedCode = parts[1].Trim();
                            }
                            if (!string.IsNullOrEmpty(selectedName) || !string.IsNullOrEmpty(selectedCode))
                            {
                                product = _products.FirstOrDefault(p => p.ProductName.Equals(selectedName, StringComparison.OrdinalIgnoreCase)
                                    || (!string.IsNullOrEmpty(selectedCode) && p.ProductCode.Equals(selectedCode, StringComparison.OrdinalIgnoreCase)));
                            }
                        }
                        if (product != null && row != null)
                        {
                            // Auto-fill product information (with validation)
                            // Use the cached row reference instead of accessing by index again
                            try
                            {
                                row.Cells["colProductCode"].Value = product.ProductCode;
                                row.Cells["colPurchasePrice"].Value = product.CostPrice;
                                row.Cells["colSalePrice"].Value = product.RetailPrice;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // Row was deleted during operation, ignore
                                return;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Row index invalid, ignore
                                return;
                            }
                            
                            // CRITICAL: Ensure the product name cell displays correctly
                            // The column's DisplayMember should show ProductName, but refresh to be sure
                            // Double-check row index before invalidating cell
                            if (e.RowIndex >= 0 && e.RowIndex < dgvPurchaseItems.Rows.Count && 
                                e.ColumnIndex >= 0 && e.ColumnIndex < dgvPurchaseItems.Columns.Count)
                            {
                                dgvPurchaseItems.InvalidateCell(e.RowIndex, e.ColumnIndex);
                            }
                            
                            // DON'T lock product name cell - allow re-editing if user wants to change product
                            // This allows dropdown to open again when user clicks on the cell

                            // Update existing stock display
                            UpdateExistingStockDisplay(product);

                            // Calculate total for this row (with validation)
                            if (e.RowIndex >= 0 && e.RowIndex < dgvPurchaseItems.Rows.Count)
                            {
                                CalculateRowTotal(e.RowIndex);
                            }
                        }
                        else if (row != null)
                        {
                            // Product not found - clear the fields (with validation)
                            // Use the cached row reference instead of accessing by index again
                            try
                            {
                                // CRITICAL: Ensure ProductCode is explicitly empty string (not numeric formatted)
                                row.Cells["colProductCode"].Value = DBNull.Value;
                                row.Cells["colProductCode"].Value = string.Empty;
                                row.Cells["colPurchasePrice"].Value = 0.00m;
                                row.Cells["colSalePrice"].Value = 0.00m;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // Row was deleted during operation, ignore
                                return;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Row index invalid, ignore
                                return;
                            }
                        }
                    }
                    
                    // Calculate total amount for the row when quantity or price changes
                    if ((e.ColumnIndex == 3 || e.ColumnIndex == 4) && // Qty or Purchase Price column
                        e.RowIndex >= 0 && e.RowIndex < dgvPurchaseItems.Rows.Count)
                    {
                        CalculateRowTotal(e.RowIndex);
                    }
                    
                    // Update product retail price in database when sale price changes
                    if (e.ColumnIndex == 5 && // Sale Price column
                        e.RowIndex >= 0 && e.RowIndex < dgvPurchaseItems.Rows.Count)
                    {
                        UpdateProductRetailPrice(e.RowIndex);
                    }
                    
                    // Recalculate all totals (only if row index is still valid)
                    if (e.RowIndex >= 0 && e.RowIndex < dgvPurchaseItems.Rows.Count)
                    {
                        CalculateTotals();
                        MarkFormDirty();
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Silently ignore - row index was invalid, likely row was deleted
                // Don't show error to user as this is expected in some edge cases
            }
            catch (IndexOutOfRangeException)
            {
                // Silently ignore - row index was out of range
            }
            catch (Exception ex)
            {
                // Only show error if it's not a row index issue
                // Use IndexOf instead of Contains with StringComparison (not available in older .NET)
                string messageLower = ex.Message.ToLower();
                if (!messageLower.Contains("rowindex") &&
                    !messageLower.Contains("out of range") &&
                    !messageLower.Contains("index"))
                {
                    ShowError($"Error updating item: {ex.Message}");
                }
            }
            finally
            {
                // Always reset the flag
                _isCalculating = false;
            }
        }

        private void UpdateExistingStockDisplay(Product product)
        {
            try
            {
                if (product != null)
                {
                    txtExistingStock.Text = product.StockQuantity.ToString();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating existing stock display: {ex.Message}");
            }
        }

        private void UpdateProductRetailPrice(int rowIndex)
        {
            try
            {
                // Validate row index
                if (rowIndex < 0 || rowIndex >= dgvPurchaseItems.Rows.Count)
                {
                    return;
                }

                var row = dgvPurchaseItems.Rows[rowIndex];
                if (row == null)
                {
                    return;
                }

                string productCode = row.Cells["colProductCode"].Value?.ToString();
                string salePriceText = row.Cells["colSalePrice"].Value?.ToString();
                
                if (!string.IsNullOrEmpty(productCode) && !string.IsNullOrEmpty(salePriceText))
                {
                    // Parse the sale price with validation
                    if (decimal.TryParse(salePriceText, out decimal newRetailPrice) && newRetailPrice >= 0)
                    {
                        // Find the product in our local list
                        var product = _products?.FirstOrDefault(p => p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                        if (product != null && product.ProductID > 0)
                        {
                            // Update only the retail price using a targeted method
                            UpdateProductRetailPriceOnly(product.ProductID, newRetailPrice);
                            
                            // Update the retail price in the local product object
                            product.RetailPrice = newRetailPrice;
                            
                            // Success message removed - no popup
                        }
                        else
                        {
                            ShowWarning($"Product with code '{productCode}' not found or invalid");
                        }
                    }
                    else
                    {
                        ShowWarning("Invalid sale price. Please enter a valid positive number.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating product retail price: {ex.Message}");
            }
        }

        private void UpdateProductRetailPriceOnly(int productID, decimal newRetailPrice)
        {
            try
            {
                // Validate inputs
                if (productID <= 0)
                {
                    throw new ArgumentException("Invalid product ID");
                }
                
                if (newRetailPrice < 0)
                {
                    throw new ArgumentException("Retail price cannot be negative");
                }

                string query = "UPDATE Products SET RetailPrice = @RetailPrice WHERE ProductID = @ProductID";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 30; // Set timeout to prevent hanging
                        command.Parameters.AddWithValue("@ProductID", productID);
                        command.Parameters.AddWithValue("@RetailPrice", newRetailPrice);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        
                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException($"No product found with ID {productID}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error updating product retail price: {ex.Message}", ex);
            }
        }

        private void UpdateProductRetailPricesFromPurchase()
        {
            try
            {
                foreach (DataGridViewRow row in dgvPurchaseItems.Rows)
                {
                    // Check if row has product data
                    if (row.Cells["colProductCode"].Value != null && 
                        !string.IsNullOrEmpty(row.Cells["colProductCode"].Value.ToString()))
                    {
                        string productCode = row.Cells["colProductCode"].Value.ToString();
                        string salePriceText = row.Cells["colSalePrice"].Value?.ToString();
                        
                        if (!string.IsNullOrEmpty(salePriceText) && decimal.TryParse(salePriceText, out decimal newRetailPrice))
                        {
                            // Find the product
                            var product = _products.FirstOrDefault(p => p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                            if (product != null)
                            {
                                // Update only the retail price using targeted method
                                UpdateProductRetailPriceOnly(product.ProductID, newRetailPrice);
                                
                                // Update the retail price in the local product object
                                product.RetailPrice = newRetailPrice;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating product retail prices: {ex.Message}");
            }
        }

        private void ShowProductDetails(Product product, int rowIndex)
        {
            try
            {
                // Display product details in a user-friendly format
                StringBuilder productInfo = new StringBuilder();
                productInfo.AppendLine("PRODUCT SELECTED:");
                productInfo.AppendLine("==================");
                productInfo.AppendLine($"Name: {product.ProductName}");
                productInfo.AppendLine($"Code: {product.ProductCode}");
                productInfo.AppendLine($"Category: {product.CategoryName}");
                productInfo.AppendLine($"Brand: {product.BrandName}");
                productInfo.AppendLine($"Cost Price: ${product.CostPrice:F2}");
                productInfo.AppendLine($"Retail Price: ${product.RetailPrice:F2}");
                productInfo.AppendLine($"Stock: {product.StockQuantity} units");
                productInfo.AppendLine();
                productInfo.AppendLine("Please enter quantity and purchase rate:");
                
                // You can display this in a message box or a text control
                // MessageBox.Show(productInfo.ToString(), "Product Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Error displaying product details: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    CalculateRowTotal(e.RowIndex);
                CalculateTotals();
                
                    // Show updated purchase items
                    ShowPurchaseItemsDisplay();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error calculating totals: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Handle Enter key to move to next cell
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    SendKeys.Send("{TAB}");
                }
                // Handle Delete key to remove row
                else if (e.KeyCode == Keys.Delete && dgvPurchaseItems.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("Are you sure you want to delete the selected item?", "Confirm Delete", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvPurchaseItems.Rows.RemoveAt(dgvPurchaseItems.SelectedRows[0].Index);
                        RefreshRowNumbers();
                        CalculateTotals();
                        
                        MarkFormDirty();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error handling key press: {ex.Message}");
            }
        }

        private void DgvPurchaseItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Handle Delete button click
                if (e.ColumnIndex == 7 && e.RowIndex >= 0) // Delete column
                {
                    if (MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvPurchaseItems.Rows.RemoveAt(e.RowIndex);
                        RefreshRowNumbers();
                        CalculateTotals();
                        
                        MarkFormDirty();

                        // Ensure there's always exactly ONE empty row available
                        int emptyRowCount = dgvPurchaseItems.Rows
                            .Cast<DataGridViewRow>()
                            .Count(r => string.IsNullOrWhiteSpace(Convert.ToString(r.Cells["colProductName"].Value)));
                        
                        if (emptyRowCount == 0)
                        {
                            AddBlankRow();
                        }
                        else if (emptyRowCount > 1)
                        {
                            RemoveExtraEmptyRows();
                        }
                    }
                }
                // Handle Product Name column click - ensure it's editable and opens dropdown
                else if (e.RowIndex >= 0 && e.ColumnIndex == dgvPurchaseItems.Columns["colProductName"].Index)
                {
                    try
                    {
                        // Ensure cell is editable
                        dgvPurchaseItems.Rows[e.RowIndex].Cells["colProductName"].ReadOnly = false;
                        // Begin editing to open dropdown
                        dgvPurchaseItems.CurrentCell = dgvPurchaseItems.Rows[e.RowIndex].Cells["colProductName"];
                        dgvPurchaseItems.BeginEdit(true);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error handling cell click: {ex.Message}");
            }
        }

        private void RefreshRowNumbers()
        {
            try
            {
                for (int i = 0; i < dgvPurchaseItems.Rows.Count; i++)
                {
                    dgvPurchaseItems.Rows[i].Cells["colSrNo"].Value = i + 1;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error refreshing row numbers: {ex.Message}");
            }
        }
        #endregion

        #region Calculations
        private void CalculateRowTotal(int rowIndex)
        {
            try
            {
                if (rowIndex >= 0 && rowIndex < dgvPurchaseItems.Rows.Count)
                {
                    var row = dgvPurchaseItems.Rows[rowIndex];
                    
                    // Get values with proper parsing
                    decimal quantity = ParseDecimal(row.Cells["colQty"].Value);
                    decimal purchasePrice = ParseDecimal(row.Cells["colPurchasePrice"].Value);
                    
                    // Calculate total
                    decimal total = quantity * purchasePrice;
                    
                    // Update the row
                    row.Cells["colTotal"].Value = total;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error calculating row total: {ex.Message}");
            }
        }

        private void ShowRowCalculationDetails(int rowIndex, decimal quantity, decimal price, decimal discount, decimal taxPercent, decimal total)
        {
            try
            {
                // Display row calculation details
                StringBuilder calculationInfo = new StringBuilder();
                calculationInfo.AppendLine($"ROW {rowIndex + 1} CALCULATION:");
                calculationInfo.AppendLine("=====================");
                calculationInfo.AppendLine($"Quantity: {quantity}");
                calculationInfo.AppendLine($"Price: ${price:F2}");
                calculationInfo.AppendLine($"Subtotal: {quantity} × ${price:F2} = ${(quantity * price):F2}");
                
                if (discount > 0)
                {
                    calculationInfo.AppendLine($"Discount: ${discount:F2}");
                    calculationInfo.AppendLine($"After Discount: ${(quantity * price - discount):F2}");
                }
                
                calculationInfo.AppendLine($"Tax ({taxPercent}%): ${((quantity * price - discount) * taxPercent / 100):F2}");
                calculationInfo.AppendLine($"TOTAL: ${total:F2}");
                calculationInfo.AppendLine();
                
                // You can display this in a message box or a text control
                // MessageBox.Show(calculationInfo.ToString(), "Row Calculation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Error displaying calculation details: {ex.Message}");
            }
        }

        private decimal ParseDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            
            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;
            
            return 0;
        }

        private void CalculateTotals(object sender = null, EventArgs e = null)
        {
            try
            {
                if (_isCalculating) return;
                
                _isCalculating = true;
                
                // Calculate subtotal from all rows
                _subtotal = 0;
                int totalItems = 0;
                decimal totalQuantity = 0;
                
                foreach (DataGridViewRow row in dgvPurchaseItems.Rows)
                {
                    if (row.Cells["colTotal"].Value != null && 
                        !string.IsNullOrEmpty(row.Cells["colProductName"].Value?.ToString()))
                    {
                        decimal rowTotal = ParseDecimal(row.Cells["colTotal"].Value);
                        decimal quantity = ParseDecimal(row.Cells["colQty"].Value);
                        _subtotal += rowTotal;
                        totalQuantity += quantity;
                        totalItems++;
                    }
                }
                
                // Get discount and tax percentages
                decimal discountPercent = ParseDecimal(txtDiscountPercent.Text);
                decimal taxPercent = ParseDecimal(txtTaxPercent.Text);
                
                // Calculate discount amount
                decimal discountAmount = _subtotal * (discountPercent / 100);
                
                // Calculate tax amount
                decimal taxAmount = (_subtotal - discountAmount) * (taxPercent / 100);
                
                // Calculate net amount
                decimal netAmount = _subtotal - discountAmount + taxAmount;
                
                // Get paid amount
                decimal paidAmount = ParseDecimal(txtPaid.Text);
                
                // Calculate balance
                decimal balance = netAmount - paidAmount;
                
                // Update display with proper formatting
                txtSubtotal.Text = _subtotal.ToString("F2");
                txtNetAmount.Text = netAmount.ToString("F2");
                txtBalance.Text = balance.ToString("F2");
                
                // Update form title with purchase summary
                UpdateFormTitle(totalItems, totalQuantity);
                
                // Calculate balance
                CalculateBalance();
            }
            catch (Exception ex)
            {
                ShowError($"Error calculating totals: {ex.Message}");
            }
            finally
            {
                _isCalculating = false;
            }
        }

        private void UpdateFormTitle(int itemCount, decimal totalQuantity)
        {
            try
            {
                string title = "Vape Store - Purchase from Vendor";
                if (itemCount > 0)
                {
                    title += $" | Items: {itemCount} | Qty: {totalQuantity} | Total: ${_grandTotal:F2}";
                }
                this.Text = title;
            }
            catch (Exception ex)
            {
                // Don't show error for title update
            }
        }

        private void UpdatePurchaseSummary(int itemCount, decimal totalQuantity)
        {
            try
            {
                // Update form title with purchase summary
                string title = "Vape Store - Purchase from Vendor";
                if (itemCount > 0)
                {
                    title += $" | Items: {itemCount} | Qty: {totalQuantity} | Total: ${_grandTotal:F2}";
                }
                this.Text = title;
            }
            catch (Exception ex)
            {
                // Don't show error for display update
            }
        }

        private void CalculateBalance(object sender = null, EventArgs e = null)
        {
            try
            {
                decimal paidAmount = ParseDecimal(txtPaid.Text);
                decimal netAmount = ParseDecimal(txtNetAmount.Text);
                decimal balance = netAmount - paidAmount;
                
                txtBalance.Text = balance.ToString("F2");
                
                // Color code the balance
                if (balance > 0)
                {
                    txtBalance.BackColor = Color.LightCoral; // Amount due
                }
                else if (balance < 0)
                {
                    txtBalance.BackColor = Color.LightGreen; // Overpaid
                }
                else
                {
                    txtBalance.BackColor = Color.White; // Exact payment
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error calculating balance: {ex.Message}");
            }
        }
        #endregion

        #region Purchase Processing
        private void BtnSavePurchase_Click(object sender, EventArgs e)
        {
            try
            {
                // Drop any trailing blank row before validation/save
                RemoveBlankRows();
                // Validate form
                if (!ValidatePurchase())
                    return;
                
                // Purchase confirmation removed to avoid annoying popups
                
                // Confirmation removed to avoid annoying popups
                
                // Show progress
                this.Cursor = Cursors.WaitCursor;

                // Create purchase object
                var purchase = CreatePurchaseObject();
                
                // Create purchase items
                var purchaseItems = CreatePurchaseItems();
                
                // Debug: Check created items
                System.Diagnostics.Debug.WriteLine($"Created {purchaseItems.Count} purchase items");
                foreach (var item in purchaseItems)
                {
                    System.Diagnostics.Debug.WriteLine($"Created Item: {item.ProductName} - Qty: {item.Quantity} - Price: {item.UnitPrice}");
                }
                
                // Validate inventory and business rules
                if (!ValidateBusinessRules(purchase, purchaseItems))
                {
                    this.Cursor = Cursors.Default;
                    return;
                }
                
                // Process purchase with inventory updates
                bool success = _purchaseRepository.ProcessPurchase(purchase, purchaseItems);
                
                if (success)
                {
                    // Update product retail prices in database for any changes made
                    UpdateProductRetailPricesFromPurchase();
                    
                    // Stock is already updated in ProcessPurchase method
                    // No need to update again here
                    
                    // Generate thermal invoice with current data (like sales form does)
                    GenerateThermalInvoice(purchase, purchaseItems);
                    
                    // Success message removed to avoid annoying popups
                    
                    // Clear form
                    ClearForm();
                }
                else
                {
                    ShowError("Failed to process purchase. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error saving purchase: {ex.Message}");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private bool ValidatePurchase()
        {
            // Validate vendor selection
            if (cmbVendorName.SelectedItem == null)
            {
                // Validation removed to avoid annoying popups
                cmbVendorName.Focus();
                return false;
            }
            
            // Validate invoice number
            if (string.IsNullOrEmpty(txtInvoiceNo.Text.Trim()))
            {
                // Validation removed to avoid annoying popups
                txtInvoiceNo.Focus();
                return false;
            }
            
            // Validate items
            bool hasValidItems = false;
            foreach (DataGridViewRow row in dgvPurchaseItems.Rows)
            {
                if (row.Cells["colProductCode"].Value != null && 
                    !string.IsNullOrEmpty(row.Cells["colProductCode"].Value.ToString()) &&
                    ParseDecimal(row.Cells["colQty"].Value) > 0 &&
                    ParseDecimal(row.Cells["colPurchasePrice"].Value) > 0)
                {
                    hasValidItems = true;
                    break;
                }
            }
            
            if (!hasValidItems)
            {
                // Validation removed to avoid annoying popups
                return false;
            }
            
            // Validate payment
            if (ParseDecimal(txtPaid.Text) < 0)
            {
                // Validation removed to avoid annoying popups
                txtPaid.Focus();
                return false;
            }
            
            return true;
        }

        private Purchase CreatePurchaseObject()
        {
            // Get selected supplier
            Supplier selectedSupplier = null;
            if (cmbVendorName.SelectedItem != null)
            {
                string selectedVendorName = cmbVendorName.SelectedItem.ToString();
                selectedSupplier = _suppliers.FirstOrDefault(s => s.SupplierName.Equals(selectedVendorName, StringComparison.OrdinalIgnoreCase));
            }

            return new Purchase
            {
                InvoiceNumber = _invoiceNumber,
                SupplierID = selectedSupplier?.SupplierID ?? 0,
                PurchaseDate = dtpInvoiceDate.Value,
                SubTotal = _subtotal,
                TaxAmount = ParseDecimal(txtTaxPercent.Text) > 0 ? (_subtotal * ParseDecimal(txtTaxPercent.Text) / 100) : 0,
                TaxPercent = ParseDecimal(txtTaxPercent.Text),
                TotalAmount = ParseDecimal(txtNetAmount.Text),
                PaymentMethod = cmbPaymentTerms.SelectedItem?.ToString() ?? "Cash",
                PaidAmount = ParseDecimal(txtPaid.Text),
                ChangeAmount = ParseDecimal(txtBalance.Text),
                UserID = _currentUserID,
                CreatedDate = DateTime.Now,
                PaymentTerms = cmbPaymentTerms.SelectedItem?.ToString() ?? "Cash",
                DiscountAmount = ParseDecimal(txtDiscountPercent.Text) > 0 ? (_subtotal * ParseDecimal(txtDiscountPercent.Text) / 100) : 0,
                Notes = txtDescription.Text.Trim()
            };
        }

        private List<PurchaseItem> CreatePurchaseItems()
        {
            var purchaseItems = new List<PurchaseItem>();
            
            foreach (DataGridViewRow row in dgvPurchaseItems.Rows)
            {
                // Strict validation: Only process rows with both product name AND product code
                var rawProductVal = row.Cells["colProductName"].Value;
                string productCode = row.Cells["colProductCode"].Value?.ToString() ?? "";
                
                // Skip empty rows - must have both product name and code to be valid
                bool hasProductName = rawProductVal != null && !string.IsNullOrWhiteSpace(rawProductVal.ToString());
                bool hasProductCode = !string.IsNullOrWhiteSpace(productCode);
                
                if (!hasProductName || !hasProductCode)
                {
                    // Skip this row - it's empty or incomplete
                    continue;
                }
                
                // Validate quantity and price are greater than 0
                decimal quantity = ParseDecimal(row.Cells["colQty"].Value);
                decimal purchasePrice = ParseDecimal(row.Cells["colPurchasePrice"].Value);
                
                if (quantity <= 0 || purchasePrice <= 0)
                {
                    // Skip rows with invalid quantities or prices
                    continue;
                }
                
                Product product = null;
                if (rawProductVal is int idVal)
                {
                    product = _products.FirstOrDefault(p => p.ProductID == idVal);
                }
                else
                {
                    string productName = rawProductVal.ToString();
                    // Remove any " | Code" suffix if present
                    if (productName.Contains(" | "))
                    {
                        productName = productName.Split('|')[0].Trim();
                    }
                    
                    product = _products.FirstOrDefault(p => 
                        p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrEmpty(productCode) && p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase)));
                }
                
                if (product != null && product.ProductID > 0)
                {
                    var purchaseItem = new PurchaseItem
                    {
                        ProductID = product.ProductID,
                        Quantity = Convert.ToInt32(quantity),
                        Unit = "pcs", // Default unit
                        UnitPrice = purchasePrice,
                        SellingPrice = ParseDecimal(row.Cells["colSalePrice"].Value),
                        SubTotal = ParseDecimal(row.Cells["colTotal"].Value),
                        Bonus = 0,
                        BatchNumber = "",
                        ExpiryDate = DateTime.Now.AddYears(1),
                        DiscountAmount = 0,
                        TaxPercent = 0,
                        Remarks = "",
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode
                    };
                    purchaseItems.Add(purchaseItem);
                }
                else
                {
                    // Product not found in local list - try database lookup as last resort
                    Product dbProduct = null;
                    var allProducts = _productRepository.GetAllProducts();
                    
                    string productName = rawProductVal.ToString();
                    if (productName.Contains(" | "))
                    {
                        productName = productName.Split('|')[0].Trim();
                    }
                    
                    if (!string.IsNullOrEmpty(productName))
                    {
                        dbProduct = allProducts.FirstOrDefault(p => 
                            p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    if (dbProduct == null && !string.IsNullOrEmpty(productCode))
                    {
                        dbProduct = allProducts.FirstOrDefault(p => 
                            p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    if (dbProduct != null && dbProduct.ProductID > 0)
                    {
                        var purchaseItem = new PurchaseItem
                        {
                            ProductID = dbProduct.ProductID,
                            Quantity = Convert.ToInt32(quantity),
                            Unit = "pcs",
                            UnitPrice = purchasePrice,
                            SellingPrice = ParseDecimal(row.Cells["colSalePrice"].Value),
                            SubTotal = ParseDecimal(row.Cells["colTotal"].Value),
                            Bonus = 0,
                            BatchNumber = "",
                            ExpiryDate = DateTime.Now.AddYears(1),
                            DiscountAmount = 0,
                            TaxPercent = 0,
                            Remarks = "",
                            ProductName = dbProduct.ProductName,
                            ProductCode = dbProduct.ProductCode
                        };
                        purchaseItems.Add(purchaseItem);
                    }
                }
            }
            
            return purchaseItems;
        }

        private void RemoveBlankRows()
        {
            try
            {
                // Remove ALL blank rows before saving to prevent extra products being inserted
                for (int i = dgvPurchaseItems.Rows.Count - 1; i >= 0; i--)
                {
                    var productNameVal = dgvPurchaseItems.Rows[i].Cells["colProductName"].Value;
                    var productCodeVal = dgvPurchaseItems.Rows[i].Cells["colProductCode"].Value;
                    
                    // Check if row is empty (no product name or code)
                    bool isEmpty = (productNameVal == null || string.IsNullOrWhiteSpace(productNameVal.ToString())) &&
                                   (productCodeVal == null || string.IsNullOrWhiteSpace(productCodeVal.ToString()));
                    
                    if (isEmpty)
                    {
                        dgvPurchaseItems.Rows.RemoveAt(i);
                    }
                }
                RefreshRowNumbers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing blank rows: {ex.Message}");
            }
        }

        private void RemoveExtraEmptyRows()
        {
            try
            {
                // Find all empty rows
                var emptyRowIndices = new List<int>();
                for (int i = 0; i < dgvPurchaseItems.Rows.Count; i++)
                {
                    var val = dgvPurchaseItems.Rows[i].Cells["colProductName"].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        emptyRowIndices.Add(i);
                    }
                }
                
                // Keep only the last empty row, remove all others
                if (emptyRowIndices.Count > 1)
                {
                    // Sort descending to remove from end first
                    emptyRowIndices.Sort((a, b) => b.CompareTo(a));
                    // Remove all except the first one (which will be the last in the grid after sorting)
                    for (int i = 1; i < emptyRowIndices.Count; i++)
                    {
                        dgvPurchaseItems.Rows.RemoveAt(emptyRowIndices[i]);
                    }
                    RefreshRowNumbers();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing extra empty rows: {ex.Message}");
            }
        }
        #endregion

        #region Form Management
        private void BtnPrintInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement print functionality
                ShowInfo("Print functionality will be implemented in the next version.");
            }
            catch (Exception ex)
            {
                ShowError($"Error printing invoice: {ex.Message}");
            }
        }

        private void BtnClearForm_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isFormDirty)
                {
                    if (MessageBox.Show("Are you sure you want to clear the form? All unsaved data will be lost.", 
                        "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                
            ClearForm();
            }
            catch (Exception ex)
            {
                ShowError($"Error clearing form: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isFormDirty)
                {
                    if (MessageBox.Show("Are you sure you want to close? All unsaved data will be lost.", 
                        "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error closing form: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            try
            {
                // Clear all fields
                cmbVendorName.SelectedIndex = -1;
                txtVendorCode.Clear();
                txtInvoiceNo.Clear();
                dtpInvoiceDate.Value = DateTime.Now;
                txtDescription.Clear();
                cmbPaymentTerms.SelectedIndex = 0;
                txtExistingStock.Clear();
                _selectedProduct = null;
                
                // Clear DataGridView
                dgvPurchaseItems.Rows.Clear();
                
                // Clear totals
                txtSubtotal.Clear();
                txtDiscountPercent.Clear();
                txtTaxPercent.Clear();
                txtNetAmount.Clear();
                txtPaid.Clear();
                txtBalance.Clear();
                
                // Generate new invoice number
                GenerateInvoiceNumber();
                
                // Reset variables
                _subtotal = 0;
                _grandTotal = 0;
                _paidAmount = 0;
                _balanceAmount = 0;
                
                // Reset form state
                _isFormDirty = false;

                // Reload lookups so dropdowns are populated for the next entry
                LoadSuppliers();
                LoadProducts();
                PopulateProductNameComboBox();
                SetupProductNameComboBox();
                // Optionally select first supplier if available
                if (cmbVendorName.Items.Count > 0 && cmbVendorName.SelectedIndex == -1)
                {
                    cmbVendorName.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error clearing form: {ex.Message}");
            }
        }

        private void MarkFormDirty()
        {
            _isFormDirty = true;
        }

        private void NewPurchase_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_isFormDirty)
                {
                    var result = MessageBox.Show("Are you sure you want to close? All unsaved data will be lost.", 
                        "Confirm Close", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.No || result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error during form closing: {ex.Message}");
            }
        }
        #endregion

        #region Form Events
        private void NewPurchase_Load(object sender, EventArgs e)
        {
            try
            {
                // Additional form initialization that needs to happen after the form is fully loaded
                this.WindowState = FormWindowState.Maximized;
                this.StartPosition = FormStartPosition.CenterScreen;
                
                // Ensure DataGridView is empty on load
                dgvPurchaseItems.DataSource = null;
                dgvPurchaseItems.Rows.Clear();
                _purchaseItems.Clear();

                // Remove top Product Name dropdown from UI (selection happens in grid)
                if (cmbProductName != null)
                {
                    cmbProductName.Visible = false;
                    cmbProductName.Enabled = false;
                }
                if (lblProductName != null)
                {
                    lblProductName.Visible = false;
                    lblProductName.Enabled = false;
                }

                // Insert an initial blank editable row for immediate product selection
                AddBlankRow();
            }
            catch (Exception ex)
            {
                ShowError($"Error during form load: {ex.Message}");
            }
        }
        #endregion

        #region Message Display
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ShowPurchaseConfirmation()
        {
            try
            {
                StringBuilder confirmation = new StringBuilder();
                confirmation.AppendLine("🛒 PURCHASE CONFIRMATION");
                confirmation.AppendLine("========================");
                confirmation.AppendLine($"Invoice: {txtInvoiceNo.Text}");
                confirmation.AppendLine($"Vendor: {cmbVendorName.Text}");
                confirmation.AppendLine($"Date: {dtpInvoiceDate.Value:yyyy-MM-dd}");
                confirmation.AppendLine();
                confirmation.AppendLine("📦 ITEMS TO PURCHASE:");
                confirmation.AppendLine("====================");
                
                decimal totalValue = 0;
                int itemCount = 0;
                
                foreach (DataGridViewRow row in dgvPurchaseItems.Rows)
                {
                    if (row.Cells["colProductCode"].Value != null && 
                        !string.IsNullOrEmpty(row.Cells["colProductCode"].Value.ToString()))
                    {
                        string itemName = row.Cells["colProductName"].Value?.ToString() ?? "";
                        string quantity = row.Cells["colQty"].Value?.ToString() ?? "0";
                        string price = row.Cells["colPurchasePrice"].Value?.ToString() ?? "0.00";
                        string total = row.Cells["colTotal"].Value?.ToString() ?? "0.00";
                        
                        confirmation.AppendLine($"• {itemName} - Qty: {quantity} @ ${price} = ${total}");
                        totalValue += ParseDecimal(total);
                        itemCount++;
                    }
                }
                
                confirmation.AppendLine();
                confirmation.AppendLine($"📊 SUMMARY:");
                confirmation.AppendLine($"Total Items: {itemCount}");
                confirmation.AppendLine($"Grand Total: ${_grandTotal:F2}");
                confirmation.AppendLine($"Payment: ${_paidAmount:F2}");
                confirmation.AppendLine($"Balance: ${_balanceAmount:F2}");
                confirmation.AppendLine();
                confirmation.AppendLine("✅ Do you want to proceed with this purchase?");
                
                var result = MessageBox.Show(confirmation.ToString(), "🛒 Confirm Purchase", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                return result == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                ShowError($"Error showing purchase confirmation: {ex.Message}");
                return false;
            }
        }

        private bool ValidateBusinessRules(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            try
            {
                // Validate minimum purchase amount
                if (purchase.TotalAmount < 1)
                {
                    ShowWarning("Purchase amount must be at least 1.00");
                    return false;
                }
                
                // Validate payment amount
                if (purchase.PaidAmount < 0)
                {
                    ShowWarning("Paid amount cannot be negative");
                    return false;
                }
                
                // Validate each item
                foreach (var item in purchaseItems)
                {
                    if (item.Quantity <= 0)
                    {
                        ShowWarning($"Quantity must be greater than 0 for product ID {item.ProductID}");
                        return false;
                    }
                    
                    if (item.UnitPrice <= 0)
                    {
                        ShowWarning($"Unit price must be greater than 0 for product ID {item.ProductID}");
                        return false;
                    }
                }
                
                // Check for duplicate products
                var duplicateProducts = purchaseItems.GroupBy(x => x.ProductID)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
                
                if (duplicateProducts.Any())
                {
                    ShowWarning("Duplicate products found. Please remove duplicates.");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Error validating business rules: {ex.Message}");
                return false;
            }
        }

        // Removed UpdateInventoryAfterPurchase method - stock is now updated in ProcessPurchase method

        private void UpdateProductCostPrice(int productId, decimal newCostPrice)
        {
            try
            {
                // This would typically update the product's cost price
                // For now, we'll just log it
                System.Diagnostics.Debug.WriteLine($"Updated cost price for product {productId} to {newCostPrice}");
            }
            catch (Exception ex)
            {
                ShowError($"Error updating product cost price: {ex.Message}");
            }
        }

        private void ShowPurchaseSuccess(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            try
            {
                StringBuilder success = new StringBuilder();
                success.AppendLine("✅ PURCHASE COMPLETED SUCCESSFULLY!");
                success.AppendLine("===================================");
                success.AppendLine($"Invoice Number: {purchase.InvoiceNumber}");
                success.AppendLine($"Vendor: {purchase.SupplierName}");
                success.AppendLine($"Date: {purchase.PurchaseDate:yyyy-MM-dd}");
                success.AppendLine($"Total Amount: ${purchase.TotalAmount:F2}");
                success.AppendLine();
                success.AppendLine("📦 ITEMS PURCHASED:");
                success.AppendLine("===================");
                
                foreach (var item in purchaseItems)
                {
                    var product = _products.FirstOrDefault(p => p.ProductID == item.ProductID);
                    if (product != null)
                    {
                        success.AppendLine($"• {product.ProductName} - Qty: {item.Quantity} @ ${item.UnitPrice:F2}");
                    }
                }
                
                success.AppendLine();
                success.AppendLine("📊 INVENTORY UPDATED:");
                success.AppendLine("=====================");
                success.AppendLine("• Stock quantities increased");
                success.AppendLine("• Product cost prices updated");
                success.AppendLine("• Purchase history recorded");
                success.AppendLine();
                success.AppendLine("🎉 Your purchase has been processed successfully!");
                
                MessageBox.Show(success.ToString(), "✅ Purchase Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Error showing purchase success: {ex.Message}");
            }
        }

        private void GenerateThermalInvoice(Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            try
            {
                // Show receipt preview - exact same as sales form
                var receiptPreview = new PurchaseReceiptPreviewForm(purchase, purchaseItems);
                receiptPreview.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowError($"Error printing receipt: {ex.Message}");
            }
        }
        #endregion
    }
}