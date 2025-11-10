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

namespace Vape_Store
{
    public partial class Cash_in_Hand : Form
    {
        private CashInHandRepository _cashInHandRepository;
        private UserRepository _userRepository;
        
        private CashInHand _currentTransaction;
        private bool isEditMode = false;
        private int selectedTransactionId = -1;

        public Cash_in_Hand()
        {
            InitializeComponent();
            _cashInHandRepository = new CashInHandRepository();
            _userRepository = new UserRepository();
            
            SetupEventHandlers();
            SetInitialState();
            LoadCurrentUser();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnClear.Click += BtnClear_Click;
            btnDelete.Click += BtnDelete_Click;
            
            // Remove incorrect event handlers from designer first
            txtopeningcash.TextChanged -= txtUsername_TextChanged;
            txtCashIn.TextChanged -= txtUsername_TextChanged;
            textBox2.TextChanged -= txtUsername_TextChanged;
            txtexpense.TextChanged -= txtUsername_TextChanged;
            txtclosingbalance.TextChanged -= txtUsername_TextChanged;
            
            // TextBox event handlers for automatic calculations
            txtopeningcash.TextChanged += CalculateClosingCash;
            txtCashIn.TextChanged += CalculateClosingCash;
            textBox2.TextChanged += CalculateClosingCash;
            txtexpense.TextChanged += CalculateClosingCash;
            
            // Date picker event handler
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
            
            // Setup DataGridView event handlers
            SetupDataGridViewEvents();
        }

        private void SetupDataGridViewEvents()
        {
            // Find DataGridView control
            if (this.Controls.Find("dgvTransactions", true).FirstOrDefault() is DataGridView dgv)
            {
                dgv.CellDoubleClick += DgvTransactions_CellDoubleClick;
                dgv.SelectionChanged += DgvTransactions_SelectionChanged;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.AllowUserToAddRows = false;
            }
        }

        private void DgvTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridView dgv = sender as DataGridView;
                    if (dgv != null && dgv.Rows[e.RowIndex].DataBoundItem is CashInHand transaction)
                    {
                        LoadTransactionForEdit(transaction.CashInHandID);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading transaction for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvTransactions_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv != null && dgv.SelectedRows.Count > 0)
                {
                    if (dgv.SelectedRows[0].DataBoundItem is CashInHand transaction)
                    {
                        selectedTransactionId = transaction.CashInHandID;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling selection: {ex.Message}");
            }
        }

        private void LoadCurrentUser()
        {
            try
            {
                // Get current user from session
                if (UserSession.CurrentUser != null)
                {
                    txtCreatedBy.Text = UserSession.CurrentUser.FullName ?? UserSession.CurrentUser.Username ?? "Admin";
                }
                else
                {
                    txtCreatedBy.Text = "Admin"; // Fallback if no session
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading current user: {ex.Message}", "Error", MessageBoxIcon.Error);
                txtCreatedBy.Text = "Admin"; // Fallback
            }
        }

        private void SetInitialState()
        {
            dateTimePicker1.Value = DateTime.Now;
            txtopeningcash.Text = "0.00";
            txtCashIn.Text = "0.00";
            textBox2.Text = "0.00";
            txtexpense.Text = "0.00";
            txtclosingbalance.Text = "0.00";
            txtDescription.Clear();
            
            isEditMode = false;
            selectedTransactionId = -1;
            _currentTransaction = null;
            
            // Make opening cash read-only for new transactions (it's calculated from previous day)
            txtopeningcash.ReadOnly = true;
            txtopeningcash.BackColor = SystemColors.Control; // Visual indicator that it's read-only
            
            // Update button states
            UpdateButtonStates();
            
            // Load previous day's closing cash as opening cash
            LoadPreviousDayClosingCash();
        }

        private void LoadPreviousDayClosingCash()
        {
            try
            {
                // Only load previous day's closing cash if NOT in edit mode
                // In edit mode, keep the original opening cash from the transaction
                if (!isEditMode)
                {
                    decimal previousClosingCash = _cashInHandRepository.GetPreviousDayClosingCash(dateTimePicker1.Value);
                    txtopeningcash.Text = previousClosingCash.ToString("F2");
                    CalculateClosingCash(null, null);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading previous day closing cash: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Only reload opening cash if not in edit mode
            // In edit mode, changing date should warn user
            if (!isEditMode)
            {
                LoadPreviousDayClosingCash();
            }
            else
            {
                // Warn user that changing date will recalculate opening cash
                var result = MessageBox.Show(
                    "Changing the date in edit mode will recalculate the opening cash from the previous day.\n\n" +
                    "This may change the transaction balance. Do you want to continue?",
                    "Confirm Date Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    LoadPreviousDayClosingCash();
                }
                else
                {
                    // Revert to original date
                    if (_currentTransaction != null)
                    {
                        dateTimePicker1.Value = _currentTransaction.TransactionDate;
                    }
                }
            }
        }

        private void CalculateClosingCash(object sender, EventArgs e)
        {
            try
            {
                decimal openingCash = ParseDecimal(txtopeningcash.Text);
                decimal cashIn = ParseDecimal(txtCashIn.Text);
                decimal cashOut = ParseDecimal(textBox2.Text);
                decimal expenses = ParseDecimal(txtexpense.Text);
                
                decimal closingCash = openingCash + cashIn - cashOut - expenses;
                txtclosingbalance.Text = closingCash.ToString("F2");
            }
            catch (Exception ex)
            {
                // Don't show error message for calculation, just set to 0
                txtclosingbalance.Text = "0.00";
            }
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCashInHandTransaction();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdateCashInHandTransaction();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteCashInHandTransaction();
        }

        private void SaveCashInHandTransaction()
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                // Check if transaction already exists for this date
                var existingTransaction = _cashInHandRepository.GetCashInHandByDate(dateTimePicker1.Value);
                if (existingTransaction != null)
                {
                    ShowMessage("A cash in hand transaction already exists for this date. Please update the existing transaction or choose a different date.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                // Get current user ID from session
                int userId = UserSession.CurrentUser?.UserID ?? 1;

                // For new transactions, ensure opening cash matches previous day's closing cash
                // This prevents data inconsistency
                decimal openingCash = ParseDecimal(txtopeningcash.Text);
                decimal expectedOpeningCash = _cashInHandRepository.GetPreviousDayClosingCash(dateTimePicker1.Value);
                
                // Warn if opening cash doesn't match expected value (but allow override)
                if (Math.Abs(openingCash - expectedOpeningCash) > 0.01m)
                {
                    var result = MessageBox.Show(
                        $"The opening cash (${openingCash:F2}) doesn't match the previous day's closing cash (${expectedOpeningCash:F2}).\n\n" +
                        "This may cause data inconsistency. Do you want to use the previous day's closing cash instead?",
                        "Opening Cash Mismatch",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.Yes)
                    {
                        openingCash = expectedOpeningCash;
                        txtopeningcash.Text = openingCash.ToString("F2");
                        CalculateClosingCash(null, null);
                    }
                }

                var transaction = new CashInHand
                {
                    TransactionDate = dateTimePicker1.Value,
                    OpeningCash = openingCash,
                    CashIn = ParseDecimal(txtCashIn.Text),
                    CashOut = ParseDecimal(textBox2.Text),
                    Expenses = ParseDecimal(txtexpense.Text),
                    ClosingCash = ParseDecimal(txtclosingbalance.Text),
                    Description = txtDescription.Text.Trim(),
                    CreatedBy = txtCreatedBy.Text.Trim(),
                    UserID = userId,
                    CreatedDate = DateTime.Now
                };

                bool success = _cashInHandRepository.AddCashInHand(transaction);
                
                if (success)
                {
                    ShowMessage("Cash in hand transaction saved successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadCashInHandTransactions(); // Refresh transaction list if DataGridView exists
                }
                else
                {
                    ShowMessage("Failed to save cash in hand transaction.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving cash in hand transaction: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateCashInHandTransaction()
        {
            try
            {
                // If not in edit mode, try to load transaction from DataGridView selection
                if (!isEditMode)
                {
                    if (this.Controls.Find("dgvTransactions", true).FirstOrDefault() is DataGridView dgv && 
                        dgv.SelectedRows.Count > 0)
                    {
                        if (dgv.SelectedRows[0].DataBoundItem is CashInHand transaction)
                        {
                            LoadTransactionForEdit(transaction.CashInHandID);
                        }
                        else
                        {
                            ShowMessage("Please select a transaction to update.", "Validation Error", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        ShowMessage("Please select a transaction to update.", "Validation Error", MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Ensure we have a transaction loaded
                if (!isEditMode || _currentTransaction == null)
                {
                    ShowMessage("Please select a transaction to update.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateForm())
                {
                    return;
                }

                _currentTransaction.TransactionDate = dateTimePicker1.Value;
                _currentTransaction.OpeningCash = ParseDecimal(txtopeningcash.Text);
                _currentTransaction.CashIn = ParseDecimal(txtCashIn.Text);
                _currentTransaction.CashOut = ParseDecimal(textBox2.Text);
                _currentTransaction.Expenses = ParseDecimal(txtexpense.Text);
                _currentTransaction.ClosingCash = ParseDecimal(txtclosingbalance.Text);
                _currentTransaction.Description = txtDescription.Text.Trim();
                _currentTransaction.CreatedBy = txtCreatedBy.Text.Trim();

                bool success = _cashInHandRepository.UpdateCashInHand(_currentTransaction);
                
                if (success)
                {
                    ShowMessage("Cash in hand transaction updated successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
                    LoadCashInHandTransactions(); // Refresh transaction list if DataGridView exists
                }
                else
                {
                    ShowMessage("Failed to update cash in hand transaction.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating cash in hand transaction: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteCashInHandTransaction()
        {
            try
            {
                int transactionIdToDelete = -1;
                
                // Check if in edit mode first
                if (isEditMode && selectedTransactionId > 0)
                {
                    transactionIdToDelete = selectedTransactionId;
                }
                // Otherwise, check if a row is selected in DataGridView
                else if (this.Controls.Find("dgvTransactions", true).FirstOrDefault() is DataGridView dgv && 
                         dgv.SelectedRows.Count > 0)
                {
                    if (dgv.SelectedRows[0].DataBoundItem is CashInHand transaction)
                    {
                        transactionIdToDelete = transaction.CashInHandID;
                    }
                }
                
                if (transactionIdToDelete <= 0)
                {
                    ShowMessage("Please select a transaction to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this cash in hand transaction?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    bool success = _cashInHandRepository.DeleteCashInHand(transactionIdToDelete);
                    
                    if (success)
                    {
                        ShowMessage("Cash in hand transaction deleted successfully!", "Success", MessageBoxIcon.Information);
                        ClearForm();
                        LoadCashInHandTransactions(); // Refresh transaction list if DataGridView exists
                    }
                    else
                    {
                        ShowMessage("Failed to delete cash in hand transaction. Transaction may not exist or has already been deleted.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting cash in hand transaction: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate opening cash (must be >= 0)
            decimal openingCash = ParseDecimal(txtopeningcash.Text);
            if (openingCash < 0)
            {
                ShowMessage("Opening cash cannot be negative.", "Validation Error", MessageBoxIcon.Warning);
                txtopeningcash.Focus();
                txtopeningcash.SelectAll();
                return false;
            }

            // Validate cash in (must be >= 0, can be 0)
            decimal cashIn = ParseDecimal(txtCashIn.Text);
            if (cashIn < 0)
            {
                ShowMessage("Cash in amount cannot be negative.", "Validation Error", MessageBoxIcon.Warning);
                txtCashIn.Focus();
                txtCashIn.SelectAll();
                return false;
            }

            // Validate cash out (must be >= 0, can be 0)
            decimal cashOut = ParseDecimal(textBox2.Text);
            if (cashOut < 0)
            {
                ShowMessage("Cash out amount cannot be negative.", "Validation Error", MessageBoxIcon.Warning);
                textBox2.Focus();
                textBox2.SelectAll();
                return false;
            }

            // Validate expenses (must be >= 0, can be 0)
            decimal expenses = ParseDecimal(txtexpense.Text);
            if (expenses < 0)
            {
                ShowMessage("Expenses amount cannot be negative.", "Validation Error", MessageBoxIcon.Warning);
                txtexpense.Focus();
                txtexpense.SelectAll();
                return false;
            }

            // Validate created by field
            if (string.IsNullOrWhiteSpace(txtCreatedBy.Text))
            {
                ShowMessage("Please enter the created by field.", "Validation Error", MessageBoxIcon.Warning);
                txtCreatedBy.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            dateTimePicker1.Value = DateTime.Now;
            txtopeningcash.Text = "0.00";
            txtCashIn.Text = "0.00";
            textBox2.Text = "0.00";
            txtexpense.Text = "0.00";
            txtclosingbalance.Text = "0.00";
            txtDescription.Clear();
            
            isEditMode = false;
            selectedTransactionId = -1;
            _currentTransaction = null;
            
            // Make opening cash read-only for new transactions
            txtopeningcash.ReadOnly = true;
            txtopeningcash.BackColor = SystemColors.Control;
            
            // Update button states
            UpdateButtonStates();
            
            // Reload previous day's closing cash
            LoadPreviousDayClosingCash();
        }

        private void UpdateButtonStates()
        {
            btnSave.Enabled = !isEditMode;
            btnUpdate.Enabled = isEditMode;
            btnDelete.Enabled = isEditMode;
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void LoadCashInHandTransactions()
        {
            try
            {
                var transactions = _cashInHandRepository.GetAllCashInHandTransactions();
                
                // If there's a DataGridView, populate it
                if (this.Controls.Find("dgvTransactions", true).FirstOrDefault() is DataGridView dgv)
                {
                    // Temporarily disable events to prevent selection change during binding
                    dgv.SelectionChanged -= DgvTransactions_SelectionChanged;
                    
                    dgv.DataSource = transactions;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    
                    // Format columns if needed
                    if (dgv.Columns.Count > 0)
                    {
                        // Hide CashInHandID column (internal use only)
                        if (dgv.Columns["CashInHandID"] != null)
                            dgv.Columns["CashInHandID"].Visible = false;
                        
                        // Format date columns
                        if (dgv.Columns["TransactionDate"] != null)
                        {
                            dgv.Columns["TransactionDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                        if (dgv.Columns["CreatedDate"] != null)
                        {
                            dgv.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        }
                        
                        // Format decimal columns
                        string[] decimalColumns = { "OpeningCash", "CashIn", "CashOut", "Expenses", "ClosingCash" };
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
                    dgv.SelectionChanged += DgvTransactions_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading transactions: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadTransactionForEdit(int transactionId)
        {
            try
            {
                var transaction = _cashInHandRepository.GetCashInHandById(transactionId);
                if (transaction != null)
                {
                    _currentTransaction = transaction;
                    selectedTransactionId = transactionId;
                    isEditMode = true;
                    
                    // Temporarily disable date change event to prevent recalculation
                    dateTimePicker1.ValueChanged -= DateTimePicker1_ValueChanged;
                    
                    // Populate form fields with saved values
                    dateTimePicker1.Value = transaction.TransactionDate;
                    txtopeningcash.Text = transaction.OpeningCash.ToString("F2");
                    txtCashIn.Text = transaction.CashIn.ToString("F2");
                    textBox2.Text = transaction.CashOut.ToString("F2");
                    txtexpense.Text = transaction.Expenses.ToString("F2");
                    txtclosingbalance.Text = transaction.ClosingCash.ToString("F2");
                    txtDescription.Text = transaction.Description ?? string.Empty;
                    txtCreatedBy.Text = transaction.CreatedBy ?? string.Empty;
                    
                    // In edit mode, allow opening cash to be edited (but warn if changed)
                    txtopeningcash.ReadOnly = false;
                    txtopeningcash.BackColor = SystemColors.Window;
                    
                    // Re-enable date change event
                    dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
                    
                    // Update button states
                    UpdateButtonStates();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading transaction: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void Cash_in_Hand_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
            LoadCashInHandTransactions(); // Load transactions on form load
        }

        // Event handlers for text changes (already defined in designer)
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            // This method is called by the designer but we handle it in CalculateClosingCash
        }

        private void label10_Click(object sender, EventArgs e)
        {
            // This method is called by the designer but we don't need specific handling
        }

        private void leftPanel_Paint(object sender, PaintEventArgs e)
        {
            // This method is called by the designer but we don't need specific handling
        }
    }
}
