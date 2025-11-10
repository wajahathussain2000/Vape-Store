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
using Vape_Store.Helpers;

namespace Vape_Store
{
    public partial class CustomerPaymentForm : Form
    {
        private CustomerPaymentRepository _customerPaymentRepository;
        private CustomerRepository _customerRepository;
        
        private List<Customer> _customers;
        private CustomerPayment _currentPayment;
        
        private bool isEditMode = false;
        private int selectedPaymentId = -1;

        public CustomerPaymentForm()
        {
            InitializeComponent();
            _customerPaymentRepository = new CustomerPaymentRepository();
            _customerRepository = new CustomerRepository();
            
            SetupEventHandlers();
            LoadCustomers();
            GenerateVoucherNumber();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnClear.Click += BtnClear_Click;
            btnDelete.Click += BtnDelete_Click;
            
            // ComboBox event handlers
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            
            // TextBox event handlers
            txtPaidAmount.TextChanged += TxtPaidAmount_TextChanged;
            
            // Form closing event handler for cleanup
            this.FormClosing += CustomerPaymentForm_FormClosing;
            
            // Setup DataGridView event handlers
            SetupDataGridViewEvents();
        }

        private void SetupDataGridViewEvents()
        {
            // Find DataGridView control
            if (this.Controls.Find("dgvPayments", true).FirstOrDefault() is DataGridView dgv)
            {
                dgv.CellDoubleClick += DgvPayments_CellDoubleClick;
                dgv.SelectionChanged += DgvPayments_SelectionChanged;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.AllowUserToAddRows = false;
            }
        }

        private void DgvPayments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridView dgv = sender as DataGridView;
                    if (dgv != null && dgv.Rows[e.RowIndex].DataBoundItem is CustomerPayment payment)
                    {
                        LoadPaymentForEdit(payment.PaymentID);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading payment for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvPayments_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv != null && dgv.SelectedRows.Count > 0)
                {
                    if (dgv.SelectedRows[0].DataBoundItem is CustomerPayment payment)
                    {
                        selectedPaymentId = payment.PaymentID;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling selection: {ex.Message}");
            }
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                // Make the customer ComboBox searchable
                SearchableComboBoxHelper.MakeSearchable(cmbCustomer, _customers, "CustomerName", "CustomerID", "CustomerName");
                cmbCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateVoucherNumber()
        {
            try
            {
                txtVoucherNo.Text = _customerPaymentRepository.GetNextVoucherNumber();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating voucher number: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            dateTimePicker1.Value = DateTime.Now;
            txtPreviousBalance.Text = "0.00";
            txtTotalDue.Text = "0.00";
            txtPaidAmount.Text = "0.00";
            txtRemainingBalance.Text = "0.00";
            txtRemainingBalance.ForeColor = SystemColors.ControlText;
            txtDescription.Clear();
            txtmobileNo.Clear();
            
            isEditMode = false;
            selectedPaymentId = -1;
            _currentPayment = null;
            
            UpdateButtonStates();
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomerDetails();
        }

        private void LoadCustomerDetails()
        {
            try
            {
                if (cmbCustomer.SelectedValue != null && cmbCustomer.SelectedItem != null)
                {
                    // Prevent customer change in edit mode
                    if (isEditMode && _currentPayment != null)
                    {
                        int currentCustomerId = _currentPayment.CustomerID;
                        int selectedCustomerId = ((Customer)cmbCustomer.SelectedItem).CustomerID;
                        
                        if (currentCustomerId != selectedCustomerId)
                        {
                            var result = MessageBox.Show(
                                "Changing customer in edit mode will recalculate balances. Do you want to continue?",
                                "Confirm Customer Change",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                            
                            if (result == DialogResult.No)
                            {
                                // Revert to original customer
                                cmbCustomer.SelectedValue = currentCustomerId;
                                return;
                            }
                        }
                    }
                    
                    int customerId = ((Customer)cmbCustomer.SelectedItem).CustomerID;
                    var customer = _customers.FirstOrDefault(c => c.CustomerID == customerId);
                    
                    if (customer != null)
                    {
                        txtmobileNo.Text = customer.Phone ?? string.Empty;
                        
                        // Calculate customer's total outstanding (what they owe us)
                        // Formula: Total Sales - Total Payments Made
                        decimal totalOutstanding = _customerPaymentRepository.GetCustomerTotalDue(customerId);
                        
                        // Get previous balance from last payment (for reference)
                        decimal previousBalance = _customerPaymentRepository.GetCustomerPreviousBalance(customerId);
                        
                        // Display values
                        txtTotalDue.Text = totalOutstanding.ToString("F2");
                        txtPreviousBalance.Text = previousBalance.ToString("F2");
                        
                        // Calculate remaining balance after current payment
                        CalculateRemainingBalance();
                    }
                }
                else
                {
                    ClearCustomerDetails();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customer details: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ClearCustomerDetails()
        {
            txtmobileNo.Clear();
            txtTotalDue.Text = "0.00";
            txtPreviousBalance.Text = "0.00";
            txtPaidAmount.Text = "0.00";
            txtRemainingBalance.Text = "0.00";
        }

        private void TxtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateRemainingBalance();
        }

        private void CalculateRemainingBalance()
        {
            try
            {
                // Simplified logic:
                // Total Outstanding = What customer owes (Total Sales - Total Payments)
                // Previous Balance = Balance from last payment (for reference only)
                // Remaining = Total Outstanding - Current Payment
                
                decimal totalOutstanding = ParseDecimal(txtTotalDue.Text);
                decimal paidAmount = ParseDecimal(txtPaidAmount.Text);
                
                // Remaining balance after this payment
                decimal remainingBalance = totalOutstanding - paidAmount;
                txtRemainingBalance.Text = remainingBalance.ToString("F2");
                
                // Visual feedback for negative balance (overpayment)
                if (remainingBalance < 0)
                {
                    txtRemainingBalance.ForeColor = Color.Red;
                }
                else
                {
                    txtRemainingBalance.ForeColor = SystemColors.ControlText;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating remaining balance: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCustomerPayment();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdateCustomerPayment();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteCustomerPayment();
        }

        private void SaveCustomerPayment()
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                // Get current user ID from session
                int userId = UserSession.CurrentUser?.UserID ?? 1;

                var payment = new CustomerPayment
                {
                    VoucherNumber = txtVoucherNo.Text.Trim(),
                    CustomerID = ((Customer)cmbCustomer.SelectedItem).CustomerID,
                    PaymentDate = dateTimePicker1.Value,
                    PreviousBalance = ParseDecimal(txtPreviousBalance.Text),
                    TotalDue = ParseDecimal(txtTotalDue.Text),
                    PaidAmount = ParseDecimal(txtPaidAmount.Text),
                    RemainingBalance = ParseDecimal(txtRemainingBalance.Text),
                    Description = txtDescription.Text.Trim(),
                    UserID = userId,
                    CreatedDate = DateTime.Now
                };

                bool success = _customerPaymentRepository.AddCustomerPayment(payment);
                
                if (success)
                {
                    ShowMessage("Customer payment saved successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadCustomerPayments(); // Refresh payment list
                }
                else
                {
                    ShowMessage("Failed to save customer payment.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving customer payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateCustomerPayment()
        {
            try
            {
                // If not in edit mode, try to load payment from DataGridView selection
                if (!isEditMode)
                {
                    if (this.Controls.Find("dgvPayments", true).FirstOrDefault() is DataGridView dgv && 
                        dgv.SelectedRows.Count > 0)
                    {
                        if (dgv.SelectedRows[0].DataBoundItem is CustomerPayment payment)
                        {
                            LoadPaymentForEdit(payment.PaymentID);
                            // After loading, continue with validation and update
                        }
                        else
                        {
                            ShowMessage("Please select a payment to update.", "Validation Error", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        ShowMessage("Please select a payment to update.", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Ensure we have a payment loaded
                if (!isEditMode || _currentPayment == null)
                {
                    ShowMessage("Please select a payment to update.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateForm())
                {
                    return;
                }

                // Get current user ID from session
                int userId = UserSession.CurrentUser?.UserID ?? 1;

                _currentPayment.CustomerID = ((Customer)cmbCustomer.SelectedItem).CustomerID;
                _currentPayment.PaymentDate = dateTimePicker1.Value;
                _currentPayment.PreviousBalance = ParseDecimal(txtPreviousBalance.Text);
                _currentPayment.TotalDue = ParseDecimal(txtTotalDue.Text);
                _currentPayment.PaidAmount = ParseDecimal(txtPaidAmount.Text);
                _currentPayment.RemainingBalance = ParseDecimal(txtRemainingBalance.Text);
                _currentPayment.Description = txtDescription.Text.Trim();
                _currentPayment.UserID = userId;

                bool success = _customerPaymentRepository.UpdateCustomerPayment(_currentPayment);
                
                if (success)
                {
                    ShowMessage("Customer payment updated successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadCustomerPayments(); // Refresh payment list
                }
                else
                {
                    ShowMessage("Failed to update customer payment.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating customer payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteCustomerPayment()
        {
            try
            {
                int paymentIdToDelete = -1;
                
                // Check if in edit mode first
                if (isEditMode && selectedPaymentId > 0)
                {
                    paymentIdToDelete = selectedPaymentId;
                }
                // Otherwise, check if a row is selected in DataGridView
                else if (this.Controls.Find("dgvPayments", true).FirstOrDefault() is DataGridView dgv && 
                         dgv.SelectedRows.Count > 0)
                {
                    if (dgv.SelectedRows[0].DataBoundItem is CustomerPayment payment)
                    {
                        paymentIdToDelete = payment.PaymentID;
                    }
                }
                
                if (paymentIdToDelete <= 0)
                {
                    ShowMessage("Please select a payment to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this customer payment?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    bool success = _customerPaymentRepository.DeleteCustomerPayment(paymentIdToDelete);
                    
                    if (success)
                    {
                        ShowMessage("Customer payment deleted successfully!", "Success", MessageBoxIcon.Information);
                        ClearForm();
                        LoadCustomerPayments(); // Refresh payment list
                    }
                    else
                    {
                        ShowMessage("Failed to delete customer payment. Payment may not exist or has already been deleted.", 
                            "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting customer payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate customer selection
            if (cmbCustomer.SelectedValue == null || cmbCustomer.SelectedItem == null)
            {
                ShowMessage("Please select a customer.", "Validation Error", MessageBoxIcon.Warning);
                cmbCustomer.Focus();
                return false;
            }

            // Validate voucher number
            if (string.IsNullOrWhiteSpace(txtVoucherNo.Text))
            {
                ShowMessage("Please enter a voucher number.", "Validation Error", MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                return false;
            }

            // Validate voucher number format (should start with CP)
            string voucherNo = txtVoucherNo.Text.Trim().ToUpper();
            if (!voucherNo.StartsWith("CP"))
            {
                ShowMessage("Voucher number should start with 'CP'.", "Validation Error", MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                txtVoucherNo.SelectAll();
                return false;
            }

            // Check for duplicate voucher number
            bool isDuplicate = _customerPaymentRepository.IsVoucherNumberExists(
                voucherNo, 
                isEditMode ? selectedPaymentId : (int?)null);
            
            if (isDuplicate)
            {
                ShowMessage($"Voucher number '{voucherNo}' already exists. Please use a different voucher number.", 
                    "Duplicate Voucher Number", MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                txtVoucherNo.SelectAll();
                return false;
            }

            // Validate paid amount
            decimal paidAmount = ParseDecimal(txtPaidAmount.Text);
            if (paidAmount <= 0)
            {
                ShowMessage("Please enter a valid paid amount greater than zero.", "Validation Error", MessageBoxIcon.Warning);
                txtPaidAmount.Focus();
                return false;
            }

            // Validate payment amount doesn't exceed what's due (with warning for overpayment)
            decimal totalOutstanding = ParseDecimal(txtTotalDue.Text);
            
            if (paidAmount > totalOutstanding)
            {
                var result = MessageBox.Show(
                    $"Paid amount (${paidAmount:F2}) exceeds total outstanding (${totalOutstanding:F2}).\n\n" +
                    "This will result in an overpayment. Do you want to continue?",
                    "Overpayment Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                
                if (result == DialogResult.No)
                {
                    txtPaidAmount.Focus();
                    txtPaidAmount.SelectAll();
                    return false;
                }
            }

            return true;
        }

        private void ClearForm()
        {
            // Temporarily remove event handler to prevent recalculation during clear
            cmbCustomer.SelectedIndexChanged -= CmbCustomer_SelectedIndexChanged;
            
            cmbCustomer.SelectedIndex = -1;
            GenerateVoucherNumber();
            dateTimePicker1.Value = DateTime.Now;
            txtPreviousBalance.Text = "0.00";
            txtTotalDue.Text = "0.00";
            txtPaidAmount.Text = "0.00";
            txtRemainingBalance.Text = "0.00";
            txtRemainingBalance.ForeColor = SystemColors.ControlText;
            txtDescription.Clear();
            txtmobileNo.Clear();
            
            isEditMode = false;
            selectedPaymentId = -1;
            _currentPayment = null;
            
            // Restore event handler
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            
            // Update button states
            UpdateButtonStates();
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void CustomerPayment_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
            LoadCustomerPayments();
        }

        private void LoadCustomerPayments()
        {
            try
            {
                var payments = _customerPaymentRepository.GetAllCustomerPayments();
                
                // If there's a DataGridView, populate it
                if (this.Controls.Find("dgvPayments", true).FirstOrDefault() is DataGridView dgv)
                {
                    // Temporarily disable events to prevent selection change during binding
                    dgv.SelectionChanged -= DgvPayments_SelectionChanged;
                    
                    dgv.DataSource = payments;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    
                    // Format columns if needed
                    if (dgv.Columns.Count > 0)
                    {
                        // Hide PaymentID column (internal use only)
                        if (dgv.Columns["PaymentID"] != null)
                            dgv.Columns["PaymentID"].Visible = false;
                        
                        // Format date columns
                        if (dgv.Columns["PaymentDate"] != null)
                        {
                            dgv.Columns["PaymentDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                        if (dgv.Columns["CreatedDate"] != null)
                        {
                            dgv.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        }
                        
                        // Format decimal columns
                        string[] decimalColumns = { "PreviousBalance", "TotalDue", "PaidAmount", "RemainingBalance" };
                        foreach (string colName in decimalColumns)
                        {
                            if (dgv.Columns[colName] != null)
                            {
                                dgv.Columns[colName].DefaultCellStyle.Format = "N2";
                                dgv.Columns[colName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                        }
                    }
                    
                    // Re-enable events
                    dgv.SelectionChanged += DgvPayments_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading payments: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadPaymentForEdit(int paymentId)
        {
            try
            {
                var payment = _customerPaymentRepository.GetCustomerPaymentById(paymentId);
                if (payment != null)
                {
                    _currentPayment = payment;
                    selectedPaymentId = paymentId;
                    isEditMode = true;
                    
                    // Temporarily remove event handler to prevent recalculation during load
                    cmbCustomer.SelectedIndexChanged -= CmbCustomer_SelectedIndexChanged;
                    
                    // Populate form fields
                    txtVoucherNo.Text = payment.VoucherNumber;
                    cmbCustomer.SelectedValue = payment.CustomerID;
                    dateTimePicker1.Value = payment.PaymentDate;
                    txtPreviousBalance.Text = payment.PreviousBalance.ToString("F2");
                    txtTotalDue.Text = payment.TotalDue.ToString("F2");
                    txtPaidAmount.Text = payment.PaidAmount.ToString("F2");
                    txtRemainingBalance.Text = payment.RemainingBalance.ToString("F2");
                    txtDescription.Text = payment.Description ?? string.Empty;
                    
                    // Restore event handler
                    cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
                    
                    // Update button states
                    UpdateButtonStates();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateButtonStates()
        {
            btnSave.Enabled = !isEditMode;
            btnUpdate.Enabled = isEditMode;
            btnDelete.Enabled = isEditMode;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Handle label click event
            // Implementation depends on what this label controls
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

        private void CustomerPaymentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Cleanup searchable ComboBox helper
                SearchableComboBoxHelper.Cleanup(cmbCustomer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FormClosing: {ex.Message}");
            }
        }
    }
}
