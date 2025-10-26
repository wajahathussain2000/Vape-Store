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
                dtpPaymentDate.Value = DateTime.Now;
                
                // Set default selections
                cmbPaymentTerms.SelectedIndex = 0; // Cash
                cmbPurchaseType.SelectedIndex = 0; // Local
                cmbPaymentMode.SelectedIndex = 0; // Cash
                
                // Set current user from UserSession
                if (UserSession.CurrentUser != null)
                {
                    _currentUserID = UserSession.CurrentUser.UserID;
                    txtEnteredBy.Text = UserSession.CurrentUser.FullName;
                }
                else
                {
                    _currentUserID = 1; // Fallback to default if session not available
                    txtEnteredBy.Text = "Current User";
                }
                
                // Initialize product entry section
                InitializeProductEntrySection();
                
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

        private void InitializeProductEntrySection()
        {
            try
            {
                // Set default values for product entry
                txtQuantity.Text = "1";
                txtPurchasePrice.Text = "0.00";
                txtSellingPrice.Text = "0.00";
                dtpExpiryDate.Value = DateTime.Now.AddYears(1);
                
                // Set default unit
                if (cmbUnit.Items.Count > 0)
                    cmbUnit.SelectedIndex = 0; // pcs
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing product entry section: {ex.Message}");
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
                
                // Make sure DataGridView is visible
                dgvPurchaseItems.Visible = true;
                dgvPurchaseItems.Enabled = true;
                
                // Configure columns with proper widths
                colSrNo.Width = 50;
                colItemCode.Width = 120;
                colItemName.Width = 250;
                colBatchNo.Width = 100;
                colExpiryDate.Width = 120;
                colQuantity.Width = 80;
                colUnit.Width = 70;
                colPurchasePrice.Width = 120;
                colDiscount.Width = 100;
                colTax.Width = 70;
                colTotalAmount.Width = 120;
                colRemarks.Width = 150;
            
            // Format currency columns
                colPurchasePrice.DefaultCellStyle.Format = "F2";
                colTotalAmount.DefaultCellStyle.Format = "F2";
                colDiscount.DefaultCellStyle.Format = "F2";
                
                // Set up unit dropdown
                colUnit.Items.AddRange(new object[] { "pcs", "kg", "box", "pack", "liter", "gram" });
                
                // Set default values for new rows
                colUnit.DefaultCellStyle.NullValue = "pcs";
                colTax.DefaultCellStyle.NullValue = "10";
                
                // Set column headers
                colSrNo.HeaderText = "Sr#";
                colItemCode.HeaderText = "Product Code";
                colItemName.HeaderText = "Product Name";
                colBatchNo.HeaderText = "Batch No";
                colExpiryDate.HeaderText = "Expiry Date";
                colQuantity.HeaderText = "Quantity";
                colUnit.HeaderText = "Unit";
                colPurchasePrice.HeaderText = "Purchase Rate";
                colDiscount.HeaderText = "Discount";
                colTax.HeaderText = "Tax %";
                colTotalAmount.HeaderText = "Total Amount";
                colRemarks.HeaderText = "Remarks";
                
                // Set column properties
                colSrNo.ReadOnly = true;
                colItemName.ReadOnly = true;
                colTotalAmount.ReadOnly = true;
                
                // Set default values
                colExpiryDate.DefaultCellStyle.NullValue = DateTime.Now.AddYears(1);
                
                // Ensure DataGridView starts empty
                dgvPurchaseItems.DataSource = null;
                dgvPurchaseItems.Rows.Clear();
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing DataGridView: {ex.Message}");
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
            
            // Product entry button handlers
            btnAddProduct.Click += BtnAddProduct_Click;
            btnClearProduct.Click += BtnClearProduct_Click;
            
            // Vendor selection event
            cmbVendorName.SelectedIndexChanged += CmbVendorName_SelectedIndexChanged;
            
            // Calculation events
            txtOverallDiscount.TextChanged += CalculateTotals;
            txtFreightCharges.TextChanged += CalculateTotals;
            txtOtherCharges.TextChanged += CalculateTotals;
            txtSalesTax.TextChanged += CalculateTotals;
            txtDiscount.TextChanged += CalculateTotals;
            txtPaidAmount.TextChanged += CalculateBalance;
            
            // DataGridView events
            dgvPurchaseItems.CellValueChanged += DgvPurchaseItems_CellValueChanged;
            dgvPurchaseItems.CellEndEdit += DgvPurchaseItems_CellEndEdit;
            dgvPurchaseItems.CellBeginEdit += DgvPurchaseItems_CellBeginEdit;
            dgvPurchaseItems.KeyDown += DgvPurchaseItems_KeyDown;
            dgvPurchaseItems.SelectionChanged += DgvPurchaseItems_SelectionChanged;
            dgvPurchaseItems.DataError += DgvPurchaseItems_DataError;
            
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
                        if (row.Cells["colItemCode"].Value != null && 
                            !string.IsNullOrEmpty(row.Cells["colItemCode"].Value.ToString()))
                        {
                            string itemCode = row.Cells["colItemCode"].Value.ToString();
                            string itemName = row.Cells["colItemName"].Value?.ToString() ?? "";
                            string quantity = row.Cells["colQuantity"].Value?.ToString() ?? "0";
                            string unit = row.Cells["colUnit"].Value?.ToString() ?? "pcs";
                            string price = row.Cells["colPurchasePrice"].Value?.ToString() ?? "0.00";
                            string total = row.Cells["colTotalAmount"].Value?.ToString() ?? "0.00";
                            
                            details.AppendLine($"{i + 1}. {itemName} ({itemCode})");
                            details.AppendLine($"   Qty: {quantity} {unit} @ ${price} = ${total}");
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
                cmbVendorName.DataSource = _suppliers;
                cmbVendorName.DisplayMember = "SupplierName";
                cmbVendorName.ValueMember = "SupplierID";
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
            }
            catch (Exception ex)
            {
                ShowError($"Error loading products: {ex.Message}");
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
                    var selectedSupplier = (Supplier)cmbVendorName.SelectedItem;
                    txtVendorCode.Text = selectedSupplier.SupplierCode;
                    
                    // Set default payment terms based on supplier
                    SetDefaultPaymentTerms(selectedSupplier);
                    
                    MarkFormDirty();
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
                // Add product to grid
                AddProductToGrid();
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
                // Clear product entry fields
                ClearProductEntry();
            }
            catch (Exception ex)
            {
                ShowError($"Error clearing product entry: {ex.Message}");
            }
        }

        private void ShowProductEntryDialog()
        {
            try
            {
                // Clear the product entry fields
                ClearProductEntry();
                
                // Focus on the first field
                    txtProductName.Focus();
            }
            catch (Exception ex)
            {
                ShowError($"Error showing product entry dialog: {ex.Message}");
            }
        }

        private void AddProductToGrid()
        {
            try
            {
                // Validate product entry
                if (!ValidateProductEntry())
                    return;
                
                // Add new row to DataGridView
                int rowIndex = dgvPurchaseItems.Rows.Add();
                
                // Get values from form controls
                string productName = txtProductName.Text.Trim();
                string productCode = txtProductCode.Text.Trim();
                decimal quantity = decimal.Parse(txtQuantity.Text);
                string unit = cmbUnit.SelectedItem?.ToString() ?? "pcs";
                decimal purchasePrice = decimal.Parse(txtPurchasePrice.Text);
                decimal sellingPrice = decimal.Parse(txtSellingPrice.Text);
                string batchNo = txtBatchNo.Text.Trim();
                DateTime expiryDate = dtpExpiryDate.Value;
                
                // Calculate total
                decimal total = quantity * purchasePrice;
                
                // Set values in DataGridView
                dgvPurchaseItems.Rows[rowIndex].Cells["colSrNo"].Value = rowIndex + 1;
                dgvPurchaseItems.Rows[rowIndex].Cells["colItemCode"].Value = productCode;
                dgvPurchaseItems.Rows[rowIndex].Cells["colItemName"].Value = productName;
                dgvPurchaseItems.Rows[rowIndex].Cells["colBatchNo"].Value = batchNo;
                dgvPurchaseItems.Rows[rowIndex].Cells["colExpiryDate"].Value = expiryDate;
                dgvPurchaseItems.Rows[rowIndex].Cells["colQuantity"].Value = quantity;
                dgvPurchaseItems.Rows[rowIndex].Cells["colUnit"].Value = unit;
                dgvPurchaseItems.Rows[rowIndex].Cells["colPurchasePrice"].Value = purchasePrice;
                dgvPurchaseItems.Rows[rowIndex].Cells["colTotalAmount"].Value = total;
                dgvPurchaseItems.Rows[rowIndex].Cells["colDiscount"].Value = 0.00m;
                dgvPurchaseItems.Rows[rowIndex].Cells["colTax"].Value = 10;
                
                // Recalculate totals
                CalculateTotals();
                
                // Clear the form
                ClearProductEntry();
                
                // Product added successfully (no popup message)
                MarkFormDirty();
            }
            catch (Exception ex)
            {
                ShowError($"Error adding product to grid: {ex.Message}");
            }
        }

        private bool ValidateProductEntry()
        {
            // Validate product name
            if (string.IsNullOrEmpty(txtProductName.Text.Trim()))
            {
                // Validation removed to avoid annoying popups
                txtProductName.Focus();
                return false;
            }
            
            // Validate product code
            if (string.IsNullOrEmpty(txtProductCode.Text.Trim()))
            {
                // Validation removed to avoid annoying popups
                txtProductCode.Focus();
                return false;
            }
            
            // Validate quantity
            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                // Validation removed to avoid annoying popups
                txtQuantity.Focus();
                return false;
            }
            
            // Validate purchase price
            if (!decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice) || purchasePrice < 0)
            {
                // Validation removed to avoid annoying popups
                txtPurchasePrice.Focus();
                return false;
            }
            
            // Validate selling price
            if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice) || sellingPrice < 0)
            {
                // Validation removed to avoid annoying popups
                txtSellingPrice.Focus();
                return false;
            }
            
            return true;
        }

        private void ClearProductEntry()
        {
            try
            {
                txtProductName.Clear();
                txtProductCode.Clear();
                txtQuantity.Text = "1";
                txtPurchasePrice.Text = "0.00";
                txtSellingPrice.Text = "0.00";
                txtBatchNo.Clear();
                dtpExpiryDate.Value = DateTime.Now.AddYears(1);
                
                if (cmbUnit.Items.Count > 0)
                    cmbUnit.SelectedIndex = 0;
                
                // Focus on product name
                    txtProductName.Focus();
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
                // Add new row to DataGridView
                int rowIndex = dgvPurchaseItems.Rows.Add();
                
                // Set default values for new row with proper data types
                dgvPurchaseItems.Rows[rowIndex].Cells["colSrNo"].Value = rowIndex + 1;
                dgvPurchaseItems.Rows[rowIndex].Cells["colUnit"].Value = "pcs";
                dgvPurchaseItems.Rows[rowIndex].Cells["colTax"].Value = 10;
                dgvPurchaseItems.Rows[rowIndex].Cells["colExpiryDate"].Value = DateTime.Now.AddYears(1);
                dgvPurchaseItems.Rows[rowIndex].Cells["colQuantity"].Value = 1;
                dgvPurchaseItems.Rows[rowIndex].Cells["colPurchasePrice"].Value = 0.00m;
                dgvPurchaseItems.Rows[rowIndex].Cells["colDiscount"].Value = 0.00m;
                dgvPurchaseItems.Rows[rowIndex].Cells["colTotalAmount"].Value = 0.00m;
                
                // Focus on the first editable cell
                dgvPurchaseItems.CurrentCell = dgvPurchaseItems.Rows[rowIndex].Cells["colItemCode"];
                dgvPurchaseItems.BeginEdit(true);
                
                // Show purchase items display
                ShowPurchaseItemsDisplay();
                
                // Show instructions for adding items
                ShowAddItemInstructions();
                
                MarkFormDirty();
            }
            catch (Exception ex)
            {
                ShowError($"Error adding item: {ex.Message}");
            }
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
                        if (row.Cells["colItemCode"].Value != null && 
                            !string.IsNullOrEmpty(row.Cells["colItemCode"].Value.ToString()))
                        {
                            string itemCode = row.Cells["colItemCode"].Value.ToString();
                            string itemName = row.Cells["colItemName"].Value?.ToString() ?? "";
                            string quantity = row.Cells["colQuantity"].Value?.ToString() ?? "0";
                            string unit = row.Cells["colUnit"].Value?.ToString() ?? "pcs";
                            string price = row.Cells["colPurchasePrice"].Value?.ToString() ?? "0.00";
                            string total = row.Cells["colTotalAmount"].Value?.ToString() ?? "0.00";
                            
                            itemsDisplay.AppendLine($"📦 {i + 1}. {itemName} ({itemCode})");
                            itemsDisplay.AppendLine($"   📊 Quantity: {quantity} {unit}");
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
                // Set up auto-complete for item code column
                if (e.ColumnIndex == 1) // Item Code column
                {
                    var cell = dgvPurchaseItems.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (cell is DataGridViewTextBoxCell)
                    {
                        // Enable auto-complete for product codes
                        var textBox = dgvPurchaseItems.EditingControl as TextBox;
                        if (textBox != null)
                        {
                            textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                            textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                            
                            var autoCompleteSource = new AutoCompleteStringCollection();
                            autoCompleteSource.AddRange(_products.Select(p => p.ProductCode).ToArray());
                            textBox.AutoCompleteCustomSource = autoCompleteSource;
                            
                            // Add text changed event for real-time search
                            textBox.TextChanged += (s, args) => SearchProductsRealTime(textBox.Text, e.RowIndex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error setting up auto-complete: {ex.Message}");
            }
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
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && !_isCalculating)
                {
                    // Auto-fill item name when item code is entered
                    if (e.ColumnIndex == 1) // Item Code column
                    {
                        string itemCode = dgvPurchaseItems.Rows[e.RowIndex].Cells["colItemCode"].Value?.ToString();
                        if (!string.IsNullOrEmpty(itemCode))
                        {
                            var product = _products.FirstOrDefault(p => p.ProductCode.Equals(itemCode, StringComparison.OrdinalIgnoreCase));
                            if (product != null)
                            {
                                dgvPurchaseItems.Rows[e.RowIndex].Cells["colItemName"].Value = product.ProductName;
                                dgvPurchaseItems.Rows[e.RowIndex].Cells["colPurchasePrice"].Value = product.CostPrice;
                                
                                // Show product details
                                ShowProductDetails(product, e.RowIndex);
                }
                            else
                            {
                                dgvPurchaseItems.Rows[e.RowIndex].Cells["colItemName"].Value = "";
                                // Product not found - handle silently
                            }
                        }
                    }
                    
                    // Calculate total amount for the row
                    CalculateRowTotal(e.RowIndex);
                    CalculateTotals();
                    
                    // Show updated purchase items
                    ShowPurchaseItemsDisplay();
                    
                    MarkFormDirty();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating item: {ex.Message}");
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
                        
                        // Show updated purchase items
                        ShowPurchaseItemsDisplay();
                        
                        MarkFormDirty();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error handling key press: {ex.Message}");
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
                
                // Show updated purchase items
                ShowPurchaseItemsDisplay();
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
                    decimal quantity = ParseDecimal(row.Cells["colQuantity"].Value);
                    decimal price = ParseDecimal(row.Cells["colPurchasePrice"].Value);
                    decimal discount = ParseDecimal(row.Cells["colDiscount"].Value);
                    decimal taxPercent = ParseDecimal(row.Cells["colTax"].Value);
                    
                    // Calculate subtotal
                    decimal subtotal = quantity * price;
                    if (discount > 0)
                    {
                        subtotal -= discount;
                }

                // Calculate tax
                    decimal taxAmount = subtotal * (taxPercent / 100);
                    
                    // Calculate total
                    decimal total = subtotal + taxAmount;
                    
                    // Update the row
                    row.Cells["colTotalAmount"].Value = total;
                    
                    // Show row calculation details
                    ShowRowCalculationDetails(rowIndex, quantity, price, discount, taxPercent, total);
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
                    if (row.Cells["colTotalAmount"].Value != null && 
                        !string.IsNullOrEmpty(row.Cells["colItemCode"].Value?.ToString()))
                    {
                        decimal rowTotal = ParseDecimal(row.Cells["colTotalAmount"].Value);
                        decimal quantity = ParseDecimal(row.Cells["colQuantity"].Value);
                        _subtotal += rowTotal;
                        totalQuantity += quantity;
                        totalItems++;
                    }
                }
                
                // Get other values
                _overallDiscount = ParseDecimal(txtOverallDiscount.Text);
                _freightCharges = ParseDecimal(txtFreightCharges.Text);
                _otherCharges = ParseDecimal(txtOtherCharges.Text);
                
                // Get sales tax and discount from new fields
                decimal salesTax = ParseDecimal(txtSalesTax.Text);
                decimal discount = ParseDecimal(txtDiscount.Text);
                
                // Calculate tax (use sales tax field or default 10%)
                _tax = salesTax > 0 ? salesTax : _subtotal * 0.10m;
                
                // Calculate grand total
                _grandTotal = _subtotal - _overallDiscount - discount + _tax + _freightCharges + _otherCharges;
                
                // Update display with proper formatting
                txtSubtotal.Text = _subtotal.ToString("F2");
                txtTax.Text = _tax.ToString("F2");
                txtSalesTax.Text = salesTax.ToString("F2");
                txtDiscount.Text = discount.ToString("F2");
                txtGrandTotal.Text = _grandTotal.ToString("F2");
                
                // Update form title with purchase summary
                UpdateFormTitle(totalItems, totalQuantity);
                
                // Update purchase summary display
                UpdatePurchaseSummary(totalItems, totalQuantity);
                
                // Update purchase details display
                DisplayPurchaseDetails();
                
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
                _paidAmount = ParseDecimal(txtPaidAmount.Text);
                _balanceAmount = _grandTotal - _paidAmount;
                txtBalanceAmount.Text = _balanceAmount.ToString("F2");
                
                // Color code the balance
                if (_balanceAmount > 0)
                {
                    txtBalanceAmount.BackColor = Color.LightCoral; // Amount due
                }
                else if (_balanceAmount < 0)
                {
                    txtBalanceAmount.BackColor = Color.LightGreen; // Overpaid
                }
                else
                {
                    txtBalanceAmount.BackColor = Color.White; // Exact payment
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
                if (row.Cells["colItemCode"].Value != null && 
                    !string.IsNullOrEmpty(row.Cells["colItemCode"].Value.ToString()) &&
                    ParseDecimal(row.Cells["colQuantity"].Value) > 0 &&
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
            if (ParseDecimal(txtPaidAmount.Text) < 0)
            {
                // Validation removed to avoid annoying popups
                txtPaidAmount.Focus();
                return false;
            }
            
            return true;
        }

        private Purchase CreatePurchaseObject()
        {
            return new Purchase
            {
                InvoiceNumber = _invoiceNumber,
                SupplierID = ((Supplier)cmbVendorName.SelectedItem).SupplierID,
                PurchaseDate = dtpInvoiceDate.Value,
                SubTotal = _subtotal,
                TaxAmount = _tax,
                TaxPercent = ParseDecimal(txtSalesTax.Text) > 0 ? (ParseDecimal(txtSalesTax.Text) / _subtotal * 100) : 10, // Use sales tax or default 10%
                TotalAmount = _grandTotal,
                PaymentMethod = cmbPaymentMode.SelectedItem.ToString(),
                PaidAmount = _paidAmount,
                ChangeAmount = _balanceAmount,
                UserID = _currentUserID,
                CreatedDate = DateTime.Now,
                PurchaseOrderNumber = txtPurchaseOrderNo.Text.Trim(),
                ReferenceNumber = txtReferenceNo.Text.Trim(),
                PaymentTerms = cmbPaymentTerms.SelectedItem.ToString(),
                PurchaseType = cmbPurchaseType.SelectedItem.ToString(),
                FreightCharges = _freightCharges,
                OtherCharges = _otherCharges,
                DiscountAmount = _overallDiscount + ParseDecimal(txtDiscount.Text),
                Notes = txtPurchaseNotes.Text.Trim()
            };
        }

        private List<PurchaseItem> CreatePurchaseItems()
        {
            var purchaseItems = new List<PurchaseItem>();
            
            foreach (DataGridViewRow row in dgvPurchaseItems.Rows)
            {
                // Check if row has either ItemCode OR ItemName (not empty row)
                bool hasItemCode = row.Cells["colItemCode"].Value != null && !string.IsNullOrEmpty(row.Cells["colItemCode"].Value.ToString());
                bool hasItemName = row.Cells["colItemName"].Value != null && !string.IsNullOrEmpty(row.Cells["colItemName"].Value.ToString());
                
                if (hasItemCode || hasItemName)
                {
                    string itemCode = hasItemCode ? row.Cells["colItemCode"].Value.ToString() : "";
                    string itemName = hasItemName ? row.Cells["colItemName"].Value.ToString() : "Unknown Product";
                    
                    // Try to find product by code first, then by name if code is empty
                    Product product = null;
                    if (!string.IsNullOrEmpty(itemCode))
                    {
                        product = _products.FirstOrDefault(p => p.ProductCode == itemCode);
                    }
                    
                    // If not found by code, try by name
                    if (product == null && !string.IsNullOrEmpty(itemName))
                    {
                        product = _products.FirstOrDefault(p => p.ProductName.Equals(itemName, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    if (product != null)
                    {
                        
                        var purchaseItem = new PurchaseItem
                        {
                            ProductID = product.ProductID,
                            Quantity = Convert.ToInt32(ParseDecimal(row.Cells["colQuantity"].Value)),
                            Unit = row.Cells["colUnit"].Value?.ToString() ?? "pcs",
                            UnitPrice = ParseDecimal(row.Cells["colPurchasePrice"].Value),
                            SellingPrice = product.RetailPrice, // Get selling price from product
                            SubTotal = ParseDecimal(row.Cells["colTotalAmount"].Value),
                            Bonus = 0, // Default bonus to 0
                            BatchNumber = row.Cells["colBatchNo"].Value?.ToString(),
                            ExpiryDate = Convert.ToDateTime(row.Cells["colExpiryDate"].Value ?? DateTime.Now.AddYears(1)),
                            DiscountAmount = ParseDecimal(row.Cells["colDiscount"].Value),
                            TaxPercent = ParseDecimal(row.Cells["colTax"].Value),
                            Remarks = row.Cells["colRemarks"].Value?.ToString(),
                            // Set navigation properties for thermal invoice
                            ProductName = product.ProductName,
                            ProductCode = product.ProductCode
                        };
                        purchaseItems.Add(purchaseItem);
                    }
                    else
                    {
                        // Product not found in memory - try to get from database or use grid data
                        Product dbProduct = null;
                        var allProducts = _productRepository.GetAllProducts();
                        
                        if (!string.IsNullOrEmpty(itemCode))
                        {
                            dbProduct = allProducts.FirstOrDefault(p => p.ProductCode == itemCode);
                        }
                        
                        if (dbProduct == null && !string.IsNullOrEmpty(itemName))
                        {
                            dbProduct = allProducts.FirstOrDefault(p => p.ProductName.Equals(itemName, StringComparison.OrdinalIgnoreCase));
                        }
                        
                        if (dbProduct != null)
                        {
                            
                            var purchaseItem = new PurchaseItem
                            {
                                ProductID = dbProduct.ProductID,
                                Quantity = Convert.ToInt32(ParseDecimal(row.Cells["colQuantity"].Value)),
                                Unit = row.Cells["colUnit"].Value?.ToString() ?? "pcs",
                                UnitPrice = ParseDecimal(row.Cells["colPurchasePrice"].Value),
                                SellingPrice = dbProduct.RetailPrice,
                                SubTotal = ParseDecimal(row.Cells["colTotalAmount"].Value),
                                Bonus = 0,
                                BatchNumber = row.Cells["colBatchNo"].Value?.ToString(),
                                ExpiryDate = Convert.ToDateTime(row.Cells["colExpiryDate"].Value ?? DateTime.Now.AddYears(1)),
                                DiscountAmount = ParseDecimal(row.Cells["colDiscount"].Value),
                                TaxPercent = ParseDecimal(row.Cells["colTax"].Value),
                                Remarks = row.Cells["colRemarks"].Value?.ToString(),
                                ProductName = dbProduct.ProductName,
                                ProductCode = dbProduct.ProductCode
                            };
                            purchaseItems.Add(purchaseItem);
                        }
                        else
                        {
                            // Fallback: Use data from grid (for manual entries)
                            var purchaseItem = new PurchaseItem
                            {
                                ProductID = 0, // Will need to be handled by repository
                                Quantity = Convert.ToInt32(ParseDecimal(row.Cells["colQuantity"].Value)),
                                Unit = row.Cells["colUnit"].Value?.ToString() ?? "pcs",
                                UnitPrice = ParseDecimal(row.Cells["colPurchasePrice"].Value),
                                SellingPrice = 0, // No selling price column in grid, will use purchase price
                                SubTotal = ParseDecimal(row.Cells["colTotalAmount"].Value),
                                Bonus = 0,
                                BatchNumber = row.Cells["colBatchNo"].Value?.ToString(),
                                ExpiryDate = row.Cells["colExpiryDate"].Value != null ? Convert.ToDateTime(row.Cells["colExpiryDate"].Value) : DateTime.Now.AddYears(1),
                                DiscountAmount = ParseDecimal(row.Cells["colDiscount"].Value ?? "0"),
                                TaxPercent = ParseDecimal(row.Cells["colTax"].Value ?? "0"),
                                Remarks = row.Cells["colRemarks"].Value?.ToString(),
                                ProductName = itemName,
                                ProductCode = itemCode
                            };
                            purchaseItems.Add(purchaseItem);
                        }
                    }
                }
            }
            
            return purchaseItems;
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
                txtPurchaseOrderNo.Clear();
                txtReferenceNo.Clear();
                cmbPaymentTerms.SelectedIndex = 0;
                cmbPurchaseType.SelectedIndex = 0;
                
                // Clear DataGridView
                dgvPurchaseItems.Rows.Clear();
                
                // Clear totals
                txtSubtotal.Clear();
                txtOverallDiscount.Clear();
                txtTax.Clear();
                txtFreightCharges.Clear();
                txtOtherCharges.Clear();
                txtGrandTotal.Clear();
                
                // Clear payment
                cmbPaymentMode.SelectedIndex = 0;
                txtPaidAmount.Clear();
                txtBalanceAmount.Clear();
                txtBankName.Clear();
                txtTransactionNo.Clear();
                dtpPaymentDate.Value = DateTime.Now;
                
                // Clear additional info
                txtPurchaseNotes.Clear();
                cmbApprovedBy.SelectedIndex = -1;
                
                // Generate new invoice number
                GenerateInvoiceNumber();
                
                // Reset variables
                _subtotal = 0;
                _overallDiscount = 0;
                _tax = 0;
                _freightCharges = 0;
                _otherCharges = 0;
                _grandTotal = 0;
                _paidAmount = 0;
                _balanceAmount = 0;
                
                // Reset form state
                _isFormDirty = false;
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
                    if (row.Cells["colItemCode"].Value != null && 
                        !string.IsNullOrEmpty(row.Cells["colItemCode"].Value.ToString()))
                    {
                        string itemName = row.Cells["colItemName"].Value?.ToString() ?? "";
                        string quantity = row.Cells["colQuantity"].Value?.ToString() ?? "0";
                        string price = row.Cells["colPurchasePrice"].Value?.ToString() ?? "0.00";
                        string total = row.Cells["colTotalAmount"].Value?.ToString() ?? "0.00";
                        
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