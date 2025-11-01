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
        
        private List<Sale> _sales;
        private List<Customer> _customers;
        private List<Product> _products;
        private Sale _selectedSale;
        private List<SalesReturnItem> _returnItems;
        
        private bool isEditMode = false;
        private int selectedReturnId = -1;

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
                cmbInvoiceNumber.DataSource = _sales;
                cmbInvoiceNumber.DisplayMember = "InvoiceNumber";
                cmbInvoiceNumber.ValueMember = "SaleID";
                cmbInvoiceNumber.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading sales: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

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

        private void BtnLoadInvoice_Click(object sender, EventArgs e)
        {
            LoadSelectedInvoice();
        }

        private void LoadSelectedInvoice()
        {
            try
            {
                if (cmbInvoiceNumber.SelectedValue == null)
                {
                    ShowMessage("Please select an invoice to load.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                int saleId = ((Sale)cmbInvoiceNumber.SelectedItem).SaleID;
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
                cmbCustomer.SelectedValue = _selectedSale.CustomerID;
                LoadCustomerDetails();

                // Populate DataGridView with sale items
                PopulateReturnItems();

                // Calculate totals
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

                foreach (var saleItem in _selectedSale.SaleItems)
                {
                    var returnItem = new SalesReturnItem
                    {
                        ProductID = saleItem.ProductID,
                        Quantity = 0, // Start with 0 return quantity
                        UnitPrice = saleItem.UnitPrice,
                        SubTotal = 0,
                        ProductName = saleItem.ProductName,
                        ProductCode = saleItem.ProductCode
                    };

                    _returnItems.Add(returnItem);

                    // Add row to DataGridView
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Select"].Value = false;
                    dataGridView1.Rows[rowIndex].Cells["ProductID"].Value = saleItem.ProductID; // Add ProductID
                    dataGridView1.Rows[rowIndex].Cells["ItemCode"].Value = saleItem.ProductCode;
                    dataGridView1.Rows[rowIndex].Cells["ItemName"].Value = saleItem.ProductName;
                    dataGridView1.Rows[rowIndex].Cells["OrignalQty"].Value = saleItem.Quantity;
                    dataGridView1.Rows[rowIndex].Cells["ReturnQty"].Value = 0;
                    dataGridView1.Rows[rowIndex].Cells["Price"].Value = saleItem.UnitPrice;
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
            if (cmbInvoiceNumber.SelectedValue != null)
            {
                LoadSelectedInvoice();
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
                    UserID = 1, // TODO: Get from current user session
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
