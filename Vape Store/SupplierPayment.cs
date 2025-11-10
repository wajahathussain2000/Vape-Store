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
    public partial class SupplierPaymentForm : Form
    {
        private SupplierPaymentRepository _supplierPaymentRepository;
        private SupplierRepository _supplierRepository;
        
        private List<Supplier> _suppliers;
        private SupplierPayment _currentPayment;
        
        private bool isEditMode = false;
        private int selectedPaymentId = -1;

        public SupplierPaymentForm()
        {
            InitializeComponent();
            _supplierPaymentRepository = new SupplierPaymentRepository();
            _supplierRepository = new SupplierRepository();
            
            SetupEventHandlers();
            LoadSuppliers();
            GenerateVoucherNumber();
            SetInitialState();
            
            // Ensure event handler is attached after loading suppliers
            cmbSupplier.SelectedIndexChanged -= CmbSupplier_SelectedIndexChanged;
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnClear.Click += BtnClear_Click;
            btnDelete.Click += BtnDelete_Click;
            
            // ComboBox event handlers
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            
            // TextBox event handlers
            txtPaidAmount.TextChanged += TxtPaidAmount_TextChanged;
            
            // Form closing event handler for cleanup
            this.FormClosing += SupplierPaymentForm_FormClosing;
            
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
                    if (dgv != null && dgv.Rows[e.RowIndex].DataBoundItem is SupplierPayment payment)
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
                    if (dgv.SelectedRows[0].DataBoundItem is SupplierPayment payment)
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

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                // Make the supplier ComboBox searchable
                SearchableComboBoxHelper.MakeSearchable(cmbSupplier, _suppliers, "SupplierName", "SupplierID", "SupplierName");
                cmbSupplier.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateVoucherNumber()
        {
            try
            {
                txtVoucherNo.Text = _supplierPaymentRepository.GetNextVoucherNumber();
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

        private void CmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSupplierDetails();
        }

        private void LoadSupplierDetails()
        {
            try
            {
                if (cmbSupplier.SelectedValue != null && cmbSupplier.SelectedItem != null)
                {
                    // Prevent supplier change in edit mode
                    if (isEditMode && _currentPayment != null)
                    {
                        int currentSupplierId = _currentPayment.SupplierID;
                        int selectedSupplierId = ((Supplier)cmbSupplier.SelectedItem).SupplierID;
                        
                        if (currentSupplierId != selectedSupplierId)
                        {
                            var result = MessageBox.Show(
                                "Changing supplier in edit mode will recalculate balances. Do you want to continue?",
                                "Confirm Supplier Change",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                            
                            if (result == DialogResult.No)
                            {
                                // Revert to original supplier
                                cmbSupplier.SelectedValue = currentSupplierId;
                                return;
                            }
                        }
                    }
                    
                    int supplierId = ((Supplier)cmbSupplier.SelectedItem).SupplierID;
                    var supplier = _suppliers.FirstOrDefault(s => s.SupplierID == supplierId);
                    
                    if (supplier != null)
                    {
                        txtmobileNo.Text = supplier.Phone ?? string.Empty;
                        
                        // Calculate supplier's total outstanding (what we owe them)
                        // Formula: Total Purchases - Total Payments Made
                        decimal totalOutstanding = _supplierPaymentRepository.GetSupplierTotalPayable(supplierId);
                        
                        // Get previous balance from last payment (for reference)
                        decimal previousBalance = _supplierPaymentRepository.GetSupplierPreviousBalance(supplierId);
                        
                        // Display values
                        txtTotalDue.Text = totalOutstanding.ToString("F2");
                        txtPreviousBalance.Text = previousBalance.ToString("F2");
                        
                        // Calculate remaining amount after current payment
                        CalculateRemainingAmount();
                    }
                }
                else
                {
                    ClearSupplierDetails();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading supplier details: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ClearSupplierDetails()
        {
            txtmobileNo.Clear();
            txtTotalDue.Text = "0.00";
            txtPreviousBalance.Text = "0.00";
            txtPaidAmount.Text = "0.00";
            txtRemainingBalance.Text = "0.00";
        }

        private void TxtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            CalculateRemainingAmount();
        }

        private void CalculateRemainingAmount()
        {
            try
            {
                // Simplified logic:
                // Total Outstanding = What supplier is owed (Total Purchases - Total Payments)
                // Previous Balance = Balance from last payment (for reference only)
                // Remaining = Total Outstanding - Current Payment
                
                decimal totalOutstanding = ParseDecimal(txtTotalDue.Text);
                decimal paidAmount = ParseDecimal(txtPaidAmount.Text);
                
                // Remaining balance after this payment
                decimal remainingAmount = totalOutstanding - paidAmount;
                txtRemainingBalance.Text = remainingAmount.ToString("F2");
                
                // Visual feedback for negative balance (overpayment)
                if (remainingAmount < 0)
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
                ShowMessage($"Error calculating remaining amount: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveSupplierPayment();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdateSupplierPayment();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteSupplierPayment();
        }

        private void SaveSupplierPayment()
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                // Get current user ID from session
                int userId = UserSession.CurrentUser?.UserID ?? 1;

                var payment = new SupplierPayment
                {
                    VoucherNumber = txtVoucherNo.Text.Trim(),
                    SupplierID = ((Supplier)cmbSupplier.SelectedItem).SupplierID,
                    PaymentDate = dateTimePicker1.Value,
                    PreviousBalance = ParseDecimal(txtPreviousBalance.Text),
                    TotalPayable = ParseDecimal(txtTotalDue.Text),
                    PaidAmount = ParseDecimal(txtPaidAmount.Text),
                    RemainingAmount = ParseDecimal(txtRemainingBalance.Text),
                    Description = txtDescription.Text.Trim(),
                    UserID = userId,
                    CreatedDate = DateTime.Now
                };

                bool success = _supplierPaymentRepository.AddSupplierPayment(payment);
                
                if (success)
                {
                    ShowMessage("Supplier payment saved successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadSupplierPayments(); // Refresh payment list
                }
                else
                {
                    ShowMessage("Failed to save supplier payment.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving supplier payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateSupplierPayment()
        {
            try
            {
                // If not in edit mode, try to load payment from DataGridView selection
                if (!isEditMode)
                {
                    if (this.Controls.Find("dgvPayments", true).FirstOrDefault() is DataGridView dgv && 
                        dgv.SelectedRows.Count > 0)
                    {
                        if (dgv.SelectedRows[0].DataBoundItem is SupplierPayment payment)
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

                _currentPayment.SupplierID = ((Supplier)cmbSupplier.SelectedItem).SupplierID;
                _currentPayment.PaymentDate = dateTimePicker1.Value;
                _currentPayment.PreviousBalance = ParseDecimal(txtPreviousBalance.Text);
                _currentPayment.TotalPayable = ParseDecimal(txtTotalDue.Text);
                _currentPayment.PaidAmount = ParseDecimal(txtPaidAmount.Text);
                _currentPayment.RemainingAmount = ParseDecimal(txtRemainingBalance.Text);
                _currentPayment.Description = txtDescription.Text.Trim();
                _currentPayment.UserID = userId;

                bool success = _supplierPaymentRepository.UpdateSupplierPayment(_currentPayment);
                
                if (success)
                {
                    ShowMessage("Supplier payment updated successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadSupplierPayments(); // Refresh payment list
                }
                else
                {
                    ShowMessage("Failed to update supplier payment.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating supplier payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteSupplierPayment()
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
                    if (dgv.SelectedRows[0].DataBoundItem is SupplierPayment payment)
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
                    "Are you sure you want to delete this supplier payment?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    bool success = _supplierPaymentRepository.DeleteSupplierPayment(paymentIdToDelete);
                    
                    if (success)
                    {
                        ShowMessage("Supplier payment deleted successfully!", "Success", MessageBoxIcon.Information);
                        ClearForm();
                        LoadSupplierPayments(); // Refresh payment list
                    }
                    else
                    {
                        ShowMessage("Failed to delete supplier payment. Payment may not exist or has already been deleted.", 
                            "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting supplier payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate supplier selection
            if (cmbSupplier.SelectedValue == null || cmbSupplier.SelectedItem == null)
            {
                ShowMessage("Please select a supplier.", "Validation Error", MessageBoxIcon.Warning);
                cmbSupplier.Focus();
                return false;
            }

            // Validate voucher number
            if (string.IsNullOrWhiteSpace(txtVoucherNo.Text))
            {
                ShowMessage("Please enter a voucher number.", "Validation Error", MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                return false;
            }

            // Validate voucher number format (should start with SP)
            string voucherNo = txtVoucherNo.Text.Trim().ToUpper();
            if (!voucherNo.StartsWith("SP"))
            {
                ShowMessage("Voucher number should start with 'SP'.", "Validation Error", MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                txtVoucherNo.SelectAll();
                return false;
            }

            // Check for duplicate voucher number
            bool isDuplicate = _supplierPaymentRepository.IsVoucherNumberExists(
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
            cmbSupplier.SelectedIndexChanged -= CmbSupplier_SelectedIndexChanged;
            
            cmbSupplier.SelectedIndex = -1;
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
            cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
            
            // Update button states
            UpdateButtonStates();
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void SupplierPaymentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Cleanup searchable ComboBox helper
                SearchableComboBoxHelper.Cleanup(cmbSupplier);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FormClosing: {ex.Message}");
            }
        }

        private void SupplierPayment_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
            LoadSupplierPayments();
        }

        private void LoadSupplierPayments()
        {
            try
            {
                var payments = _supplierPaymentRepository.GetAllSupplierPayments();
                
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
                        string[] decimalColumns = { "PreviousBalance", "TotalPayable", "PaidAmount", "RemainingAmount" };
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
                var payment = _supplierPaymentRepository.GetSupplierPaymentById(paymentId);
                if (payment != null)
                {
                    _currentPayment = payment;
                    selectedPaymentId = paymentId;
                    isEditMode = true;
                    
                    // Temporarily remove event handler to prevent recalculation during load
                    cmbSupplier.SelectedIndexChanged -= CmbSupplier_SelectedIndexChanged;
                    
                    // Populate form fields
                    txtVoucherNo.Text = payment.VoucherNumber;
                    cmbSupplier.SelectedValue = payment.SupplierID;
                    dateTimePicker1.Value = payment.PaymentDate;
                    txtPreviousBalance.Text = payment.PreviousBalance.ToString("F2");
                    txtTotalDue.Text = payment.TotalPayable.ToString("F2");
                    txtPaidAmount.Text = payment.PaidAmount.ToString("F2");
                    txtRemainingBalance.Text = payment.RemainingAmount.ToString("F2");
                    txtDescription.Text = payment.Description ?? string.Empty;
                    
                    // Restore event handler
                    cmbSupplier.SelectedIndexChanged += CmbSupplier_SelectedIndexChanged;
                    
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
