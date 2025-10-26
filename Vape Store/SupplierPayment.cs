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
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                cmbSupplier.DataSource = _suppliers;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";
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
            txtDescription.Clear();
            txtmobileNo.Clear();
            
            isEditMode = false;
            selectedPaymentId = -1;
            _currentPayment = null;
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
                    int supplierId = ((Supplier)cmbSupplier.SelectedItem).SupplierID;
                    var supplier = _suppliers.FirstOrDefault(s => s.SupplierID == supplierId);
                    
                    if (supplier != null)
                    {
                        txtmobileNo.Text = supplier.Phone;
                        
                        // Calculate supplier's total payable and previous balance
                        decimal totalPayable = _supplierPaymentRepository.GetSupplierTotalPayable(supplierId);
                        decimal previousBalance = _supplierPaymentRepository.GetSupplierPreviousBalance(supplierId);
                        
                        txtTotalDue.Text = totalPayable.ToString("F2");
                        txtPreviousBalance.Text = previousBalance.ToString("F2");
                        
                        // Calculate remaining amount
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
                decimal totalPayable = ParseDecimal(txtTotalDue.Text);
                decimal previousBalance = ParseDecimal(txtPreviousBalance.Text);
                decimal paidAmount = ParseDecimal(txtPaidAmount.Text);
                
                decimal remainingAmount = (totalPayable + previousBalance) - paidAmount;
                txtRemainingBalance.Text = remainingAmount.ToString("F2");
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
                    UserID = 1, // TODO: Get from current user session
                    CreatedDate = DateTime.Now
                };

                bool success = _supplierPaymentRepository.AddSupplierPayment(payment);
                
                if (success)
                {
                    ShowMessage("Supplier payment saved successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
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
                if (!isEditMode)
                {
                    ShowMessage("Please select a payment to update.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateForm())
                {
                    return;
                }

                _currentPayment.SupplierID = ((Supplier)cmbSupplier.SelectedItem).SupplierID;
                _currentPayment.PaymentDate = dateTimePicker1.Value;
                _currentPayment.PreviousBalance = ParseDecimal(txtPreviousBalance.Text);
                _currentPayment.TotalPayable = ParseDecimal(txtTotalDue.Text);
                _currentPayment.PaidAmount = ParseDecimal(txtPaidAmount.Text);
                _currentPayment.RemainingAmount = ParseDecimal(txtRemainingBalance.Text);
                _currentPayment.Description = txtDescription.Text.Trim();

                bool success = _supplierPaymentRepository.UpdateSupplierPayment(_currentPayment);
                
                if (success)
                {
                    ShowMessage("Supplier payment updated successfully!", "Success", MessageBoxIcon.Information);
                    ClearForm();
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
                if (!isEditMode)
                {
                    ShowMessage("Please select a payment to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this supplier payment?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _supplierPaymentRepository.DeleteSupplierPayment(selectedPaymentId);
                    
                    if (success)
                    {
                        ShowMessage("Supplier payment deleted successfully!", "Success", MessageBoxIcon.Information);
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to delete supplier payment.", "Error", MessageBoxIcon.Error);
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
            // Debug: Check what's in the ComboBox
            if (cmbSupplier.SelectedValue == null || cmbSupplier.SelectedItem == null)
            {
                ShowMessage("Please select a supplier.", "Validation Error", MessageBoxIcon.Warning);
                cmbSupplier.Focus();
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
            cmbSupplier.SelectedIndex = -1;
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
                var payment = _supplierPaymentRepository.GetSupplierPaymentById(paymentId);
                if (payment != null)
                {
                    _currentPayment = payment;
                    selectedPaymentId = paymentId;
                    isEditMode = true;
                    
                    // Populate form fields
                    txtVoucherNo.Text = payment.VoucherNumber;
                    cmbSupplier.SelectedValue = payment.SupplierID;
                    dateTimePicker1.Value = payment.PaymentDate;
                    txtPreviousBalance.Text = payment.PreviousBalance.ToString("F2");
                    txtTotalDue.Text = payment.TotalPayable.ToString("F2");
                    txtPaidAmount.Text = payment.PaidAmount.ToString("F2");
                    txtRemainingBalance.Text = payment.RemainingAmount.ToString("F2");
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
