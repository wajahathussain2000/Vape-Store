using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Vape_Store.Repositories;
using Vape_Store.Models;

namespace Vape_Store
{
    public partial class Customers : Form
    {
        private CustomerRepository _customerRepository;
        private List<Customer> _customers;
        private bool isEditMode = false;
        private int selectedCustomerId = -1;

        public Customers()
        {
            InitializeComponent();
            _customerRepository = new CustomerRepository();
            
            InitializeDataGridView();
            SetupEventHandlers();
            LoadCustomers();
            GenerateCustomerCode();
        }

        private void InitializeDataGridView()
        {
            // Configure DataGridView columns
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
            
            // Clear existing columns
            dgvCustomers.Columns.Clear();
            
            // Add columns
            dgvCustomers.Columns.Add("CustomerID", "ID");
            dgvCustomers.Columns.Add("CustomerCode", "Code");
            dgvCustomers.Columns.Add("CustomerName", "Name");
            dgvCustomers.Columns.Add("Phone", "Phone");
            dgvCustomers.Columns.Add("Email", "Email");
            dgvCustomers.Columns.Add("Address", "Address");
            dgvCustomers.Columns.Add("City", "City");
            dgvCustomers.Columns.Add("PostalCode", "Postal Code");
            dgvCustomers.Columns.Add("CreatedDate", "Created Date");
            dgvCustomers.Columns.Add("IsActive", "Status");
            
            // Configure column properties
            dgvCustomers.Columns["CustomerID"].Width = 50;
            dgvCustomers.Columns["CustomerCode"].Width = 80;
            dgvCustomers.Columns["CustomerName"].Width = 150;
            dgvCustomers.Columns["Phone"].Width = 120;
            dgvCustomers.Columns["Email"].Width = 150;
            dgvCustomers.Columns["Address"].Width = 200;
            dgvCustomers.Columns["City"].Width = 100;
            dgvCustomers.Columns["PostalCode"].Width = 80;
            dgvCustomers.Columns["CreatedDate"].Width = 100;
            dgvCustomers.Columns["IsActive"].Width = 80;
            
            // Format date column
            dgvCustomers.Columns["CreatedDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
            
            // Format status column
            dgvCustomers.Columns["IsActive"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            
            // DataGridView event handlers
            dgvCustomers.CellDoubleClick += DgvCustomers_CellDoubleClick;
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadCustomers()
        {
            try
            {
                _customers = _customerRepository.GetAllCustomers();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        
        private void RefreshDataGridView()
        {
            try
            {
                dgvCustomers.Rows.Clear();
                
                foreach (var customer in _customers)
                {
                    dgvCustomers.Rows.Add(
                        customer.CustomerID,
                        customer.CustomerCode,
                        customer.CustomerName,
                        customer.Phone,
                        customer.Email,
                        customer.Address,
                        customer.City,
                        customer.PostalCode,
                        customer.CreatedDate,
                        customer.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing data: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }
        
        private void GenerateCustomerCode()
        {
            try
            {
                string customerCode = _customerRepository.GetNextCustomerCode();
                txtCustomerCode.Text = customerCode;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating customer code: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCustomer();
        }

        private void SaveCustomer()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    ShowMessage("Please enter a customer name.", "Validation Error", MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    ShowMessage("Please enter a phone number.", "Validation Error", MessageBoxIcon.Warning);
                    txtPhone.Focus();
                    return;
                }

                // Validate email format if provided
                if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
                {
                    ShowMessage("Please enter a valid email address.", "Validation Error", MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                // Check for duplicate customer name
                var existingCustomer = _customers.FirstOrDefault(c => 
                    c.CustomerName.ToLower() == txtCustomerName.Text.ToLower() && 
                    c.CustomerID != selectedCustomerId);

                if (existingCustomer != null)
                {
                    ShowMessage("A customer with this name already exists.", "Duplicate Error", MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return;
                }

                if (isEditMode)
                {
                    // Update existing customer
                    var customer = new Customer
                    {
                        CustomerID = selectedCustomerId,
                        CustomerCode = txtCustomerCode.Text.Trim(),
                        CustomerName = txtCustomerName.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Address = txtAddress.Text.Trim(),
                        City = txtCity.Text.Trim(),
                        PostalCode = txtPostalCode.Text.Trim(),
                        IsActive = checkBox1.Checked,
                        CreatedDate = _customers.First(c => c.CustomerID == selectedCustomerId).CreatedDate
                    };

                    bool success = _customerRepository.UpdateCustomer(customer);
                    
                    if (success)
                    {
                        ShowMessage("Customer updated successfully!", "Success", MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to update customer.", "Error", MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Add new customer
                    var customer = new Customer
                    {
                        CustomerCode = txtCustomerCode.Text.Trim(),
                        CustomerName = txtCustomerName.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Address = txtAddress.Text.Trim(),
                        City = txtCity.Text.Trim(),
                        PostalCode = txtPostalCode.Text.Trim(),
                        IsActive = checkBox1.Checked,
                        CreatedDate = DateTime.Now
                    };

                    bool success = _customerRepository.AddCustomer(customer);
                    
                    if (success)
                    {
                        ShowMessage("Customer added successfully!", "Success", MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                        GenerateCustomerCode();
                    }
                    else
                    {
                        ShowMessage("Failed to add customer.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving customer: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteCustomer();
        }

        private void DeleteCustomer()
        {
            try
            {
                if (selectedCustomerId == -1)
                {
                    ShowMessage("Please select a customer to delete.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var customer = _customers.FirstOrDefault(c => c.CustomerID == selectedCustomerId);
                if (customer == null)
                {
                    ShowMessage("Customer not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Confirm deletion
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the customer '{customer.CustomerName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _customerRepository.DeleteCustomer(selectedCustomerId);
                    
                    if (success)
                    {
                        ShowMessage("Customer deleted successfully!", "Success", MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to delete customer.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting customer: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtCustomerCode.Clear();
            txtCustomerName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtPostalCode.Clear();
            checkBox1.Checked = true;
            selectedCustomerId = -1;
            SetEditMode(false);
            GenerateCustomerCode();
        }

        private void DgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvCustomers.Rows.Count)
            {
                EditSelectedCustomer();
            }
        }

        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int rowIndex = dgvCustomers.SelectedRows[0].Index;
                if (rowIndex >= 0 && rowIndex < _customers.Count)
                {
                    selectedCustomerId = _customers[rowIndex].CustomerID;
                }
            }
        }

        private void EditSelectedCustomer()
        {
            try
            {
                if (selectedCustomerId == -1)
                {
                    ShowMessage("Please select a customer to edit.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                var customer = _customers.FirstOrDefault(c => c.CustomerID == selectedCustomerId);
                if (customer == null)
                {
                    ShowMessage("Customer not found.", "Error", MessageBoxIcon.Error);
                    return;
                }

                // Populate form with customer data
                txtCustomerCode.Text = customer.CustomerCode;
                txtCustomerName.Text = customer.CustomerName;
                txtPhone.Text = customer.Phone;
                txtEmail.Text = customer.Email;
                txtAddress.Text = customer.Address;
                txtCity.Text = customer.City;
                txtPostalCode.Text = customer.PostalCode;
                checkBox1.Checked = customer.IsActive;
                
                SetEditMode(true);
                txtCustomerName.Focus();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error editing customer: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterCustomers();
        }

        private void FilterCustomers()
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    RefreshDataGridView();
                    return;
                }

                var filteredCustomers = _customers.Where(c => 
                    c.CustomerName.ToLower().Contains(searchText) ||
                    c.CustomerCode.ToLower().Contains(searchText) ||
                    c.Phone.ToLower().Contains(searchText) ||
                    c.Email.ToLower().Contains(searchText) ||
                    c.City.ToLower().Contains(searchText)).ToList();

                dgvCustomers.Rows.Clear();
                
                foreach (var customer in filteredCustomers)
                {
                    dgvCustomers.Rows.Add(
                        customer.CustomerID,
                        customer.CustomerCode,
                        customer.CustomerName,
                        customer.Phone,
                        customer.Email,
                        customer.Address,
                        customer.City,
                        customer.PostalCode,
                        customer.CreatedDate,
                        customer.IsActive ? "Active" : "Inactive"
                    );
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error filtering customers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;
            
            if (editMode)
            {
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
            else
            {
                btnSave.Text = "Save";
                btnDelete.Enabled = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetEditMode(false);
            txtSearch.Focus();
        }
    }
}