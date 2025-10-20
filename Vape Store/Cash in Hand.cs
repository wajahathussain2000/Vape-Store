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
                decimal openingCash = Convert.ToDecimal(txtopeningcash.Text ?? "0");
                decimal cashIn = Convert.ToDecimal(txtCashIn.Text ?? "0");
                decimal cashOut = Convert.ToDecimal(textBox2.Text ?? "0");
                decimal expenses = Convert.ToDecimal(txtexpense.Text ?? "0");
                
                decimal closingCash = openingCash + cashIn - cashOut - expenses;
                txtclosingbalance.Text = closingCash.ToString("F2");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating closing cash: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
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
                    OpeningCash = Convert.ToDecimal(txtopeningcash.Text),
                    CashIn = Convert.ToDecimal(txtCashIn.Text),
                    CashOut = Convert.ToDecimal(textBox2.Text),
                    Expenses = Convert.ToDecimal(txtexpense.Text),
                    ClosingCash = Convert.ToDecimal(txtclosingbalance.Text),
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
                _currentTransaction.OpeningCash = Convert.ToDecimal(txtopeningcash.Text);
                _currentTransaction.CashIn = Convert.ToDecimal(txtCashIn.Text);
                _currentTransaction.CashOut = Convert.ToDecimal(textBox2.Text);
                _currentTransaction.Expenses = Convert.ToDecimal(txtexpense.Text);
                _currentTransaction.ClosingCash = Convert.ToDecimal(txtclosingbalance.Text);
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
            if (string.IsNullOrWhiteSpace(txtopeningcash.Text) || Convert.ToDecimal(txtopeningcash.Text) < 0)
            {
                ShowMessage("Please enter a valid opening cash amount.", "Validation Error", MessageBoxIcon.Warning);
                txtopeningcash.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCashIn.Text) || Convert.ToDecimal(txtCashIn.Text) < 0)
            {
                ShowMessage("Please enter a valid cash in amount.", "Validation Error", MessageBoxIcon.Warning);
                txtCashIn.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text) || Convert.ToDecimal(textBox2.Text) < 0)
            {
                ShowMessage("Please enter a valid cash out amount.", "Validation Error", MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtexpense.Text) || Convert.ToDecimal(txtexpense.Text) < 0)
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
