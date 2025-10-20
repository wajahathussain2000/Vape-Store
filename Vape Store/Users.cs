using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Repositories;
using Vape_Store.Models;

namespace Vape_Store
{
    public partial class Users : Form
    {
        private UserRepository _userRepository;
        private List<User> _users;
        private User _currentUser;
        private bool isEditMode = false;
        private int selectedUserId = -1;

        public Users()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            
            SetupEventHandlers();
            InitializeDataGridView();
            InitializeRoles();
            LoadUsers();
            SetInitialState();
        }

        private void SetupEventHandlers()
        {
            // Button event handlers
            btnSave.Click += BtnSave_Click;
            btnClear.Click += BtnClear_Click;
            btnDelete.Click += BtnDelete_Click;
            closeBtn.Click += CloseBtn_Click;
            
            // DataGridView event handlers
            dgvUsers.CellDoubleClick += DgvUsers_CellDoubleClick;
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            
            // Search event handler
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // Form event handlers
            this.Load += Users_Load;
        }

        private void InitializeDataGridView()
        {
            try
            {
                dgvUsers.AutoGenerateColumns = false;
                dgvUsers.AllowUserToAddRows = false;
                dgvUsers.AllowUserToDeleteRows = false;
                dgvUsers.ReadOnly = true;
                dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvUsers.MultiSelect = false;

                // Define columns
                dgvUsers.Columns.Clear();
                
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Username",
                    HeaderText = "Username",
                    DataPropertyName = "Username",
                    Width = 120
                });
                
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "FullName",
                    HeaderText = "Full Name",
                    DataPropertyName = "FullName",
                    Width = 150
                });
                
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Email",
                    HeaderText = "Email",
                    DataPropertyName = "Email",
                    Width = 180
                });
                
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Role",
                    HeaderText = "Role",
                    DataPropertyName = "Role",
                    Width = 100
                });
                
                dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsActive",
                    HeaderText = "Active",
                    DataPropertyName = "IsActive",
                    Width = 60
                });
                
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CreatedDate",
                    HeaderText = "Created Date",
                    DataPropertyName = "CreatedDate",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" },
                    Width = 100
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void InitializeRoles()
        {
            try
            {
                cmbRole.Items.Clear();
                cmbRole.Items.AddRange(new string[] { "Admin", "Manager", "Cashier", "Sales", "Inventory", "User" });
            }
            catch (Exception ex)
            {
                ShowMessage($"Error initializing roles: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadUsers()
        {
            try
            {
                _users = _userRepository.GetAllUsers();
                RefreshDataGridView();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading users: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            try
            {
                dgvUsers.DataSource = null;
                dgvUsers.DataSource = _users;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error refreshing DataGridView: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void SetInitialState()
        {
            txtUsername.Clear();
            txtEmail.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            chkIsActive.Checked = true;
            chkIsAdmin.Checked = false;
            
            isEditMode = false;
            selectedUserId = -1;
            _currentUser = null;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveUser();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveUser()
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                var user = new User
                {
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    FullName = $"{txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}".Trim(),
                    Password = txtPassword.Text.Trim(),
                    Role = cmbRole.SelectedItem?.ToString() ?? "User",
                    IsActive = chkIsActive.Checked
                };

                bool success;
                if (isEditMode)
                {
                    user.UserID = selectedUserId;
                    success = _userRepository.UpdateUser(user);
                }
                else
                {
                    success = _userRepository.AddUser(user);
                }
                
                if (success)
                {
                    ShowMessage("User saved successfully!", "Success", MessageBoxIcon.Information);
                    LoadUsers();
                    ClearForm();
                }
                else
                {
                    ShowMessage("Failed to save user.", "Error", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving user: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DeleteUser()
        {
            try
            {
                if (!isEditMode)
                {
                    ShowMessage("Please select a user to delete.", "Validation Error", MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to deactivate this user?",
                    "Confirm Deactivation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = _userRepository.DeleteUser(selectedUserId);
                    
                    if (success)
                    {
                        ShowMessage("User deactivated successfully!", "Success", MessageBoxIcon.Information);
                        LoadUsers();
                        ClearForm();
                    }
                    else
                    {
                        ShowMessage("Failed to deactivate user.", "Error", MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting user: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowMessage("Please enter a username.", "Validation Error", MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                ShowMessage("Please enter a first name.", "Validation Error", MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                ShowMessage("Please enter a last name.", "Validation Error", MessageBoxIcon.Warning);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowMessage("Please enter an email address.", "Validation Error", MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                ShowMessage("Please enter a valid email address.", "Validation Error", MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowMessage("Please enter a password.", "Validation Error", MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (cmbRole.SelectedIndex == -1)
            {
                ShowMessage("Please select a role.", "Validation Error", MessageBoxIcon.Warning);
                cmbRole.Focus();
                return false;
            }

            // Check for duplicate username
            try
            {
                bool usernameExists = _userRepository.IsUsernameExists(
                    txtUsername.Text.Trim(), 
                    isEditMode ? selectedUserId : 0);
                
                if (usernameExists)
                {
                    ShowMessage("A user with this username already exists.", "Validation Error", MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating username: {ex.Message}", "Error", MessageBoxIcon.Error);
                return false;
            }

            // Check for duplicate email
            try
            {
                bool emailExists = _userRepository.IsEmailExists(
                    txtEmail.Text.Trim(), 
                    isEditMode ? selectedUserId : 0);
                
                if (emailExists)
                {
                    ShowMessage("A user with this email already exists.", "Validation Error", MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating email: {ex.Message}", "Error", MessageBoxIcon.Error);
                return false;
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
            txtUsername.Clear();
            txtEmail.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            chkIsActive.Checked = true;
            chkIsAdmin.Checked = false;
            
            isEditMode = false;
            selectedUserId = -1;
            _currentUser = null;
        }

        private void DgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var user = _users[e.RowIndex];
                    LoadUserForEdit(user);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading user for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.SelectedRows.Count > 0)
                {
                    var selectedUser = (User)dgvUsers.SelectedRows[0].DataBoundItem;
                    selectedUserId = selectedUser.UserID;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error handling selection change: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void LoadUserForEdit(User user)
        {
            try
            {
                _currentUser = user;
                isEditMode = true;
                selectedUserId = user.UserID;
                
                txtUsername.Text = user.Username;
                txtEmail.Text = user.Email;
                
                // Split full name into first and last name
                var nameParts = user.FullName.Split(' ');
                txtFirstName.Text = nameParts.Length > 0 ? nameParts[0] : "";
                txtLastName.Text = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                
                txtPassword.Text = user.Password;
                cmbRole.SelectedItem = user.Role;
                chkIsActive.Checked = user.IsActive;
                chkIsAdmin.Checked = user.Role == "Admin";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading user for edit: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadUsers();
                }
                else
                {
                    _users = _userRepository.SearchUsers(txtSearch.Text);
                    RefreshDataGridView();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error searching users: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private void Users_Load(object sender, EventArgs e)
        {
            // Set initial state
            SetInitialState();
        }
    }
}
