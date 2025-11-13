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
    public partial class PurchaseReturnForm : Form
    {
        private PurchaseReturnRepository _purchaseReturnRepository;
        private PurchaseRepository _purchaseRepository;
        private SupplierRepository _supplierRepository;
        private ProductRepository _productRepository;
        private InventoryService _inventoryService;
        
        private List<Purchase> _purchases;
        private List<Supplier> _suppliers;
        private List<Product> _products;
        private Purchase _selectedPurchase;
        private List<PurchaseReturnItem> _returnItems;
        
        private bool isEditMode = false;
        private int selectedReturnId = -1;

        public PurchaseReturnForm()
        {
            InitializeComponent();
            _purchaseReturnRepository = new PurchaseReturnRepository();
            _purchaseRepository = new PurchaseRepository();
            _supplierRepository = new SupplierRepository();
            _productRepository = new ProductRepository();
            _inventoryService = new InventoryService();
            
            _returnItems = new List<PurchaseReturnItem>();
            InitializeDataGridView();
            SetupEventHandlers();
            LoadSuppliers();
            LoadPurchases();
            GenerateReturnNumber();
            InitializeReturnReasons();
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
            btnsave.Click += Btnsave_Click;
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
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                cmbCustomer.DataSource = _suppliers;
                cmbCustomer.DisplayMember = "SupplierName";
                cmbCustomer.ValueMember = "SupplierID";
                cmbCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadPurchases()
        {
            try
            {
                _purchases = _purchaseReturnRepository.GetPurchasesForReturn();
                SetupSearchableInvoiceComboBox();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading purchases: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                cmbInvoiceNumber.ValueMember = "PurchaseID";
                cmbInvoiceNumber.DataSource = _purchases;
                cmbInvoiceNumber.SelectedIndex = -1;
                
                // Built-in autocomplete handles typing/searching
            }
            catch (Exception ex)
            {
                ShowMessage($"Error setting up invoice combo box: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateReturnNumber()
        {
            try
            {
                string returnNumber = _purchaseReturnRepository.GetNextReturnNumber();
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
                cmbreturnreason.Items.Add("Defective Product");
                cmbreturnreason.Items.Add("Wrong Item");
                cmbreturnreason.Items.Add("Quality Issue");
                cmbreturnreason.Items.Add("Damaged in Transit");
                cmbreturnreason.Items.Add("Not as Described");
                cmbreturnreason.Items.Add("Overstock");
                cmbreturnreason.Items.Add("Other");
                cmbreturnreason.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing return reasons: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                if (cmbInvoiceNumber.SelectedItem == null || !(cmbInvoiceNumber.SelectedItem is Purchase))
                {
                    ShowMessage("Please select an invoice to load.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                Purchase selectedPurchaseObj = cmbInvoiceNumber.SelectedItem as Purchase;
                if (selectedPurchaseObj == null)
                {
                    ShowMessage("Please select an invoice to load.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                int purchaseId = selectedPurchaseObj.PurchaseID;
                _selectedPurchase = _purchaseReturnRepository.GetPurchaseWithItems(purchaseId);

                if (_selectedPurchase == null)
                {
                    ShowMessage("Invoice not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Populate original invoice details
                txtOriginalInvoiceNumber.Text = _selectedPurchase.InvoiceNumber;
                txtOriginalInvoiceDate.Text = _selectedPurchase.PurchaseDate.ToString("dd/MM/yyyy");
                txtOriginalInvoiceTotal.Text = _selectedPurchase.TotalAmount.ToString("F2");

                // Populate supplier details
                try
                {
                    cmbCustomer.SelectedValue = _selectedPurchase.SupplierID;
                }
                catch
                {
                    // If supplier ID not found in combo box, try to find and select by value
                    for (int i = 0; i < cmbCustomer.Items.Count; i++)
                    {
                        if (cmbCustomer.Items[i] is Supplier supplier && supplier.SupplierID == _selectedPurchase.SupplierID)
                        {
                            cmbCustomer.SelectedIndex = i;
                            break;
                        }
                    }
                }
                LoadSupplierDetails();

                // Populate tax and discount from original invoice
                // Calculate discount percentage from discount amount and subtotal
                decimal discountPercent = 0;
                if (_selectedPurchase.SubTotal > 0 && _selectedPurchase.DiscountAmount > 0)
                {
                    discountPercent = (_selectedPurchase.DiscountAmount / _selectedPurchase.SubTotal) * 100;
                }
                
                // Set discount percentage (txtTaxPercent is used for discount in this form)
                if (discountPercent > 0)
                {
                    txtTaxPercent.Text = discountPercent.ToString("F2");
                }
                else
                {
                    txtTaxPercent.Text = "0";
                }

                // Set tax percentage in combo box - only if combo box has items
                if (cmbTax.Items.Count > 0)
                {
                    string taxPercentText = _selectedPurchase.TaxPercent.ToString("F0") + "%";
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

                // Populate DataGridView with purchase items
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

                foreach (var purchaseItem in _selectedPurchase.PurchaseItems)
                {
                    var returnItem = new PurchaseReturnItem
                    {
                        ProductID = purchaseItem.ProductID,
                        Quantity = 0, // Start with 0 return quantity
                        UnitPrice = purchaseItem.UnitPrice,
                        SubTotal = 0,
                        ProductName = purchaseItem.ProductName,
                        ProductCode = purchaseItem.ProductCode
                    };

                    _returnItems.Add(returnItem);

                    // Add row to DataGridView
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Select"].Value = false;
                    dataGridView1.Rows[rowIndex].Cells["ItemCode"].Value = purchaseItem.ProductCode;
                    dataGridView1.Rows[rowIndex].Cells["ItemName"].Value = purchaseItem.ProductName;
                    dataGridView1.Rows[rowIndex].Cells["OrignalQty"].Value = purchaseItem.Quantity;
                    dataGridView1.Rows[rowIndex].Cells["ReturnQty"].Value = 0;
                    dataGridView1.Rows[rowIndex].Cells["Price"].Value = purchaseItem.UnitPrice;
                    dataGridView1.Rows[rowIndex].Cells["Total"].Value = 0;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error populating return items: {ex.Message}", "Error", MessageBoxIcon.Error);
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
                
                // Safe parsing for discount percentage
                decimal discount = 0;
                if (!string.IsNullOrEmpty(txtTaxPercent.Text))
                {
                    decimal.TryParse(txtTaxPercent.Text, out discount);
                }
                
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
                
                txtsubTotal.Text = subtotal.ToString("F2");
                txtTax.Text = taxAmount.ToString("F2");
                txtTotal.Text = total.ToString("F2");
                
                // Debug output
                System.Diagnostics.Debug.WriteLine($"Purchase Return Calculation: Subtotal={subtotal:F2}, Discount={discountAmount:F2}, Taxable={taxableAmount:F2}, Tax%={taxPercent:F2}, Tax={taxAmount:F2}, Total={total:F2}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating totals: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Purchase Return CalculateTotals Error: {ex.Message}");
            }
        }

        private void CmbInvoiceNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInvoiceNumber.SelectedValue != null)
            {
                LoadSelectedInvoice();
            }
        }

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSupplierDetails();
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSupplierDetails();
        }

        private void LoadSupplierDetails()
        {
            try
            {
                // Check cmbCustomer for supplier selection
                ComboBox selectedCombo = null;
                if (cmbCustomer.SelectedValue != null)
                    selectedCombo = cmbCustomer;

                if (selectedCombo != null)
                {
                    int supplierId = ((Supplier)selectedCombo.SelectedItem).SupplierID;
                    var supplier = _suppliers.FirstOrDefault(s => s.SupplierID == supplierId);
                    
                    if (supplier != null)
                    {
                        txtCustomerName.Text = supplier.SupplierName;
                        txtCustomerPhone.Text = supplier.Phone;
                        txtCustomerAddress.Text = supplier.Address;
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
                ShowMessage($"Error loading supplier details: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void CmbTax_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Purchase Return Tax dropdown changed to: {cmbTax.SelectedItem?.ToString()}");
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling tax selection: {ex.Message}", "Error", MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Purchase Return Tax Selection Error: {ex.Message}");
            }
        }

        private void TxtTaxPercent_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void Btnsave_Click(object sender, EventArgs e)
        {
            SavePurchaseReturn();
        }

        private void SavePurchaseReturn()
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
                    ShowMessage("Please select a supplier.", "Validation Error", MessageBoxIcon.Warning);
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
                    var originalItem = _selectedPurchase.PurchaseItems.FirstOrDefault(pi => pi.ProductID == item.ProductID);
                    if (originalItem != null && item.Quantity > originalItem.Quantity)
                    {
                        ShowMessage($"Return quantity for {item.ProductName} cannot exceed original quantity ({originalItem.Quantity}).", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Create purchase return
                var purchaseReturn = new PurchaseReturn
                {
                    ReturnNumber = txtReturnNumber.Text.Trim(),
                    PurchaseID = ((Purchase)cmbInvoiceNumber.SelectedItem).PurchaseID,
                    SupplierID = ((Supplier)cmbCustomer.SelectedItem).SupplierID,
                    ReturnDate = dtpReturnDate.Value,
                    ReturnReason = cmbreturnreason.SelectedItem?.ToString(),
                    Description = txtdescription.Text.Trim(),
                    TotalAmount = ParseDecimal(txtTotal.Text),
                    UserID = 1, // TODO: Get from current user session
                    CreatedDate = DateTime.Now,
                    ReturnItems = selectedItems
                };

                // Save to database
                bool success = _purchaseReturnRepository.AddPurchaseReturn(purchaseReturn);
                
                if (success)
                {
                    // Update inventory (reduce stock for returns)
                    foreach (var item in selectedItems)
                    {
                        _inventoryService.UpdateStock(item.ProductID, -item.Quantity, 0);
                    }

                    ShowMessage("Purchase return processed successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                }
                else
                {
                    ShowMessage("Failed to process purchase return.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing purchase return: {ex.Message}", "Error", MessageBoxIcon.Error);
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
            _selectedPurchase = null;
            GenerateReturnNumber();
            
            // Ensure tax calculation runs after clearing
            CalculateTotals();

            // Reload lookups so dropdowns are populated for the next return
            LoadSuppliers();
            LoadPurchases();
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

        private void PurchaseReturn_Load(object sender, EventArgs e)
        {
            // Set initial state
            dtpReturnDate.Value = DateTime.Now;
            cmbTax.Items.Add("0%");
            cmbTax.Items.Add("5%");
            cmbTax.Items.Add("10%");
            cmbTax.Items.Add("15%");
            cmbTax.Items.Add("20%");
            cmbTax.SelectedIndex = 0;
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
