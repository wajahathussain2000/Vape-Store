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
    public partial class Supplier_Master : Form
    {
        private SupplierRepository _supplierRepository;
        private List<Supplier> _suppliers;
        private Supplier _currentSupplier;
        private bool isEditMode = false;
        private int selectedSupplierId = -1;

        public Supplier_Master()
        {
            InitializeComponent();
            _supplierRepository = new SupplierRepository();
            
            SetupEventHandlers();
            InitializeDataGridView();
            LoadSuppliers();
            GenerateSupplierCode();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnClear.Click += BtnClear_Click;
            btnDelete.Click += BtnDelete_Click;
            
            // DataGridView event handlers
            dgvSuppliers.CellDoubleClick += DgvSuppliers_CellDoubleClick;
            dgvSuppliers.SelectionChanged += DgvSuppliers_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += Supplier_Master_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvSuppliers.AutoGenerateColumns = false;
                dgvSuppliers.AllowUserToAddRows = false;
                dgvSuppliers.AllowUserToDeleteRows = false;
                dgvSuppliers.ReadOnly = true;
                dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSuppliers.MultiSelect = false;

                // Define columns
                dgvSuppliers.Columns.Clear();
                
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierCode",
                    HeaderText = "Code",
                    DataPropertyName = "SupplierCode",
                    Width = 80
                });
                
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SupplierName",
                    HeaderText = "Supplier Name",
                    DataPropertyName = "SupplierName",
                    Width = 150
                });
                
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ContactPerson",
                    HeaderText = "Contact Person",
                    DataPropertyName = "ContactPerson",
                    Width = 120
                });
                
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Phone",
                    HeaderText = "Phone",
                    DataPropertyName = "Phone",
                    Width = 100
                });
                
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Email",
                    HeaderText = "Email",
                    DataPropertyName = "Email",
                    Width = 150
                });
                
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "City",
                    HeaderText = "City",
                    DataPropertyName = "City",
                    Width = 100
                });
                
                dgvSuppliers.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsActive",
                    HeaderText = "Active",
                    DataPropertyName = "IsActive",
                    Width = 60
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                _suppliers = _supplierRepository.GetAllSuppliers();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvSuppliers.DataSource = null;
                dgvSuppliers.DataSource = _suppliers;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void GenerateSupplierCode()
        {
            try
            {
                txtCustomerCode.Text = _supplierRepository.GetNextSupplierCode();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating supplier code: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            txtCustomerName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtPostalCode.Clear();
            checkBox1.Checked = true;
            
            isEditMode = false;
            selectedSupplierId = -1;
            _currentSupplier = null;
            SetEditMode(false);
        }

        private void SetEditMode(bool editMode)
        {
            isEditMode = editMode;
            
            if (editMode)
            {
                // When editing: disable Save, enable Update and Delete
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                // When creating new: enable Save, disable Update and Delete
                btnSave.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveSupplier();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdateSupplier();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteSupplier();
        }

        private void SaveSupplier()
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                var supplier = new Supplier
                {
                    SupplierCode = txtCustomerCode.Text.Trim(),
                    SupplierName = txtCustomerName.Text.Trim(),
                    ContactPerson = "", // Not in the form, can be added later
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    City = txtCity.Text.Trim(),
                    PostalCode = txtPostalCode.Text.Trim(),
                    IsActive = checkBox1.Checked
                };

                bool success = _supplierRepository.AddSupplier(supplier);
                
                if (success)
                {
                    ShowMessage("Supplier added successfully!", "Success", MessageBoxIcon.Information);
                    LoadSuppliers();
                    ClearForm();
                    GenerateSupplierCode();
                }
                else
                {
                    ShowMessage("Failed to add supplier.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving supplier: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void UpdateSupplier()
        {
            try
            {
                if (selectedSupplierId == -1)
                {
                    ShowMessage("Please select a supplier to update.", "Selection Error", MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateForm())
                {
                    return;
                }

                var supplier = new Supplier
                {
                    SupplierID = selectedSupplierId,
                    SupplierCode = txtCustomerCode.Text.Trim(),
                    SupplierName = txtCustomerName.Text.Trim(),
                    ContactPerson = "", // Not in the form, can be added later
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    City = txtCity.Text.Trim(),
                    PostalCode = txtPostalCode.Text.Trim(),
                    IsActive = checkBox1.Checked
                };

                bool success = _supplierRepository.UpdateSupplier(supplier);
                
                if (success)
                {
                    ShowMessage("Supplier updated successfully!", "Success", MessageBoxIcon.Information);
                    LoadSuppliers();
                    ClearForm();
                    SetEditMode(false);
                }
                else
                {
                    ShowMessage("Failed to update supplier.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating supplier: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteSupplier()
        {
            try
            {
                if (!isEditMode)
                {
                    ShowMessage("Please select a supplier to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to deactivate this supplier?",
                    "Confirm Deactivation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _supplierRepository.DeleteSupplier(selectedSupplierId);
                    
                    if (success)
                    {
                        ShowMessage("Supplier deactivated successfully!", "Success", MessageBoxIcon.Information);
                        LoadSuppliers();
                        ClearForm();
                        SetEditMode(false);
                    }
                    else
                    {
                        ShowMessage("Failed to deactivate supplier.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting supplier: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                ShowMessage("Please enter a supplier name.", "Validation Error", MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                ShowMessage("Please enter a valid email address.", "Validation Error", MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Check for duplicate supplier name
            try
            {
                bool nameExists = _supplierRepository.IsSupplierNameExists(
                    txtCustomerName.Text.Trim(), 
                    isEditMode ? selectedSupplierId : 0);
                
                if (nameExists)
                {
                    ShowMessage("A supplier with this name already exists.", "Validation Error", MessageBoxIcon.Warning);
                    txtCustomerName.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating supplier name: {ex.Message}", "Error", MessageBoxIcon.Error);
                return false;
            }

            // Check for duplicate email if provided
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                try
                {
                    bool emailExists = _supplierRepository.IsEmailExists(
                        txtEmail.Text.Trim(), 
                        isEditMode ? selectedSupplierId : 0);
                    
                    if (emailExists)
                    {
                        ShowMessage("A supplier with this email already exists.", "Validation Error", MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error validating email: {ex.Message}", "Error", MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
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

        private void ClearForm()
        {
            GenerateSupplierCode();
            txtCustomerName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtCity.Clear();
            txtPostalCode.Clear();
            checkBox1.Checked = true;
            
            isEditMode = false;
            selectedSupplierId = -1;
            _currentSupplier = null;
            SetEditMode(false);
        }

        private void DgvSuppliers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var supplier = _suppliers[e.RowIndex];
                    LoadSupplierForEdit(supplier);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading supplier for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvSuppliers.SelectedRows.Count > 0)
                {
                    var selectedSupplier = (Supplier)dgvSuppliers.SelectedRows[0].DataBoundItem;
                    selectedSupplierId = selectedSupplier.SupplierID;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling selection change: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadSupplierForEdit(Supplier supplier)
        {
            try
            {
                _currentSupplier = supplier;
                isEditMode = true;
                selectedSupplierId = supplier.SupplierID;
                
                txtCustomerCode.Text = supplier.SupplierCode;
                txtCustomerName.Text = supplier.SupplierName;
                txtPhone.Text = supplier.Phone;
                txtEmail.Text = supplier.Email;
                txtAddress.Text = supplier.Address;
                txtCity.Text = supplier.City;
                txtPostalCode.Text = supplier.PostalCode;
                checkBox1.Checked = supplier.IsActive;
                
                SetEditMode(true);
                txtCustomerName.Focus();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading supplier for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadSuppliers();
                }
                else
                {
                    _suppliers = _supplierRepository.SearchSuppliers(txtSearch.Text);
                    RefreshDataGridView();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching suppliers: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void Supplier_Master_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
            txtSearch.Focus();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // This method is kept for compatibility with the designer
        }
    }
}
