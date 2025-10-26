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
            
            // TextBox event handlers for automatic calculations
            txtopeningcash.TextChanged += CalculateClosingCash;
            txtCashIn.TextChanged += CalculateClosingCash;
            textBox2.TextChanged += CalculateClosingCash;
            txtexpense.TextChanged += CalculateClosingCash;
            
            // Date picker event handler
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
        }

        private void LoadCurrentUser()
        {
            try
            {
                // TODO: Get current user from session
                txtCreatedBy.Text = "Admin"; // Placeholder
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading current user: {ex.Message}", "Error", MessageBoxIcon.Error);
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
            
            // Load previous day's closing cash as opening cash
            LoadPreviousDayClosingCash();
        }

        private void LoadPreviousDayClosingCash()
        {
            try
            {
                decimal previousClosingCash = _cashInHandRepository.GetPreviousDayClosingCash(dateTimePicker1.Value);
                txtopeningcash.Text = previousClosingCash.ToString("F2");
                CalculateClosingCash(null, null);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading previous day closing cash: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadPreviousDayClosingCash();
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

                var transaction = new CashInHand
                {
                    TransactionDate = dateTimePicker1.Value,
                    OpeningCash = ParseDecimal(txtopeningcash.Text),
                    CashIn = ParseDecimal(txtCashIn.Text),
                    CashOut = ParseDecimal(textBox2.Text),
                    Expenses = ParseDecimal(txtexpense.Text),
                    ClosingCash = ParseDecimal(txtclosingbalance.Text),
                    Description = txtDescription.Text.Trim(),
                    CreatedBy = txtCreatedBy.Text.Trim(),
                    UserID = 1, // TODO: Get from current user session
                    CreatedDate = DateTime.Now
                };

                bool success = _cashInHandRepository.AddCashInHand(transaction);
                
                if (success)
                {
                    ShowMessage("Cash in hand transaction saved successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
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
                if (!isEditMode)
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
                if (!isEditMode)
                {
                    ShowMessage("Please select a transaction to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this cash in hand transaction?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _cashInHandRepository.DeleteCashInHand(selectedTransactionId);
                    
                    if (success)
                    {
                        ShowMessage("Cash in hand transaction deleted successfully!", "Success", MessageBoxIcon.Information);
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to delete cash in hand transaction.", "Error", MessageBoxIcon.Error);
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
            decimal openingCash = ParseDecimal(txtopeningcash.Text);
            if (openingCash < 0)
            {
                ShowMessage("Please enter a valid opening cash amount.", "Validation Error", MessageBoxIcon.Warning);
                txtopeningcash.Focus();
                return false;
            }

            decimal cashIn = ParseDecimal(txtCashIn.Text);
            if (cashIn < 0)
            {
                ShowMessage("Please enter a valid cash in amount.", "Validation Error", MessageBoxIcon.Warning);
                txtCashIn.Focus();
                return false;
            }

            decimal cashOut = ParseDecimal(textBox2.Text);
            if (cashOut < 0)
            {
                ShowMessage("Please enter a valid cash out amount.", "Validation Error", MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            decimal expenses = ParseDecimal(txtexpense.Text);
            if (expenses < 0)
            {
                ShowMessage("Please enter a valid expenses amount.", "Validation Error", MessageBoxIcon.Warning);
                txtexpense.Focus();
                return false;
            }

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
            
            // Reload previous day's closing cash
            LoadPreviousDayClosingCash();
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
                    dgv.DataSource = transactions;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
                    
                    // Populate form fields
                    dateTimePicker1.Value = transaction.TransactionDate;
                    txtopeningcash.Text = transaction.OpeningCash.ToString("F2");
                    txtCashIn.Text = transaction.CashIn.ToString("F2");
                    textBox2.Text = transaction.CashOut.ToString("F2");
                    txtexpense.Text = transaction.Expenses.ToString("F2");
                    txtclosingbalance.Text = transaction.ClosingCash.ToString("F2");
                    txtDescription.Text = transaction.Description;
                    txtCreatedBy.Text = transaction.CreatedBy;
                    
                    // Update button states
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
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
