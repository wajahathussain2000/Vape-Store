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
            txtDescription.Clear();
            txtmobileNo.Clear();
            
            isEditMode = false;
            selectedPaymentId = -1;
            _currentPayment = null;
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
                        txtmobileNo.Text = customer.Phone;
                        
                        // Calculate customer's total due and previous balance
                        decimal totalDue = _customerPaymentRepository.GetCustomerTotalDue(customerId);
                        decimal previousBalance = _customerPaymentRepository.GetCustomerPreviousBalance(customerId);
                        
                        txtTotalDue.Text = totalDue.ToString("F2");
                        txtPreviousBalance.Text = previousBalance.ToString("F2");
                        
                        // Calculate remaining balance
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
                decimal totalDue = ParseDecimal(txtTotalDue.Text);
                decimal previousBalance = ParseDecimal(txtPreviousBalance.Text);
                decimal paidAmount = ParseDecimal(txtPaidAmount.Text);
                
                decimal remainingBalance = (totalDue + previousBalance) - paidAmount;
                txtRemainingBalance.Text = remainingBalance.ToString("F2");
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
                    UserID = 1, // TODO: Get from current user session
                    CreatedDate = DateTime.Now
                };

                bool success = _customerPaymentRepository.AddCustomerPayment(payment);
                
                if (success)
                {
                    ShowMessage("Customer payment saved successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
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
                if (!isEditMode)
                {
                    ShowMessage("Please select a payment to update.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateForm())
                {
                    return;
                }

                _currentPayment.CustomerID = ((Customer)cmbCustomer.SelectedItem).CustomerID;
                _currentPayment.PaymentDate = dateTimePicker1.Value;
                _currentPayment.PreviousBalance = ParseDecimal(txtPreviousBalance.Text);
                _currentPayment.TotalDue = ParseDecimal(txtTotalDue.Text);
                _currentPayment.PaidAmount = ParseDecimal(txtPaidAmount.Text);
                _currentPayment.RemainingBalance = ParseDecimal(txtRemainingBalance.Text);
                _currentPayment.Description = txtDescription.Text.Trim();

                bool success = _customerPaymentRepository.UpdateCustomerPayment(_currentPayment);
                
                if (success)
                {
                    ShowMessage("Customer payment updated successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
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
                if (!isEditMode)
                {
                    ShowMessage("Please select a payment to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this customer payment?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _customerPaymentRepository.DeleteCustomerPayment(selectedPaymentId);
                    
                    if (success)
                    {
                        ShowMessage("Customer payment deleted successfully!", "Success", MessageBoxIcon.Information);
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to delete customer payment.", "Error", MessageBoxIcon.Error);
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
            if (cmbCustomer.SelectedValue == null)
            {
                ShowMessage("Please select a customer.", "Validation Error", MessageBoxIcon.Warning);
                cmbCustomer.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtVoucherNo.Text))
            {
                ShowMessage("Please enter a voucher number.", "Validation Error", MessageBoxIcon.Warning);
                txtVoucherNo.Focus();
                return false;
            }

            decimal paidAmount = ParseDecimal(txtPaidAmount.Text);
            if (paidAmount <= 0)
            {
                ShowMessage("Please enter a valid paid amount.", "Validation Error", MessageBoxIcon.Warning);
                txtPaidAmount.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            cmbCustomer.SelectedIndex = -1;
            GenerateVoucherNumber();
            dateTimePicker1.Value = DateTime.Now;
            txtPreviousBalance.Text = "0.00";
            txtTotalDue.Text = "0.00";
            txtPaidAmount.Text = "0.00";
            txtRemainingBalance.Text = "0.00";
            txtDescription.Clear();
            txtmobileNo.Clear();
            
            isEditMode = false;
            selectedPaymentId = -1;
            _currentPayment = null;
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
                    dgv.DataSource = payments;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
                    
                    // Populate form fields
                    txtVoucherNo.Text = payment.VoucherNumber;
                    cmbCustomer.SelectedValue = payment.CustomerID;
                    dateTimePicker1.Value = payment.PaymentDate;
                    txtPreviousBalance.Text = payment.PreviousBalance.ToString("F2");
                    txtTotalDue.Text = payment.TotalDue.ToString("F2");
                    txtPaidAmount.Text = payment.PaidAmount.ToString("F2");
                    txtRemainingBalance.Text = payment.RemainingBalance.ToString("F2");
                    txtDescription.Text = payment.Description;
                    
                    // Update button states
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading payment: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
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
    }
}
