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
            
            // Check if user has "users" permission before allowing access
            if (!Vape_Store.UserSession.HasPermission("users"))
            {
                MessageBox.Show("You do not have permission to access User Management.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }
            
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
                var roles = new List<string> { "Admin", "Manager", "Cashier", "Sales", "Inventory", "User" };
                Vape_Store.Helpers.SearchableComboBoxHelper.MakeSearchable(cmbRole, roles);
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
            
            // Re-check SuperAdmin access after clearing
            CheckSuperAdminAccess();
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
                // Check if user is SuperAdmin for password operations
                bool isSuperAdmin = IsSuperAdmin();
                
                if (!isSuperAdmin)
                {
                    ShowMessage("Only SuperAdmin can view and change user passwords.", "Access Denied", MessageBoxIcon.Warning);
                    return;
                }
                
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
                int userId = -1;
                if (isEditMode)
                {
                    user.UserID = selectedUserId;
                    success = _userRepository.UpdateUser(user);
                    userId = selectedUserId;
                }
                else
                {
                    success = _userRepository.AddUser(user);
                    
                    // Get the newly created user ID by finding the user with the same username
                    var newUser = _userRepository.GetAllUsers().FirstOrDefault(u => u.Username == user.Username);
                    if (newUser != null)
                    {
                        userId = newUser.UserID;
                    }
                }
                
                if (success && userId > 0)
                {
                    // Update the UserRoles table to link the user to their role
                    try
                    {
                        var roleRepo = new Vape_Store.Repositories.RoleRepository();
                        
                        // Get the role ID based on the selected role
                        var roles = roleRepo.GetRoles();
                        var selectedRole = roles.FirstOrDefault(r => r.Name.Equals(user.Role, StringComparison.OrdinalIgnoreCase));
                        
                        if (selectedRole != null)
                        {
                            // Replace user roles (this will remove existing and add new ones)
                            roleRepo.ReplaceUserRoles(userId, new List<int> { selectedRole.RoleId });
                        }
                    }
                    catch (Exception roleEx)
                    {
                        // Log the error but don't fail the user creation
                        System.Diagnostics.Debug.WriteLine($"Error updating user roles: {roleEx.Message}");
                    }
                    
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

            // Only validate password if SuperAdmin (and password field is visible)
            if (txtPassword.Visible && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowMessage("Please enter a password.", "Validation Error", MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }
            
            // If not SuperAdmin, don't allow password operations
            if (!IsSuperAdmin() && txtPassword.Visible)
            {
                ShowMessage("Only SuperAdmin can set or change passwords.", "Access Denied", MessageBoxIcon.Warning);
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
                
                // Only SuperAdmin can view and update passwords
                if (IsSuperAdmin())
                {
                    // Fetch full user data with password from database
                    var fullUser = _userRepository.GetUserById(user.UserID);
                    if (fullUser != null)
                    {
                        txtPassword.Text = fullUser.Password ?? ""; // Show existing password
                    }
                    else
                    {
                        txtPassword.Text = user.Password ?? ""; // Fallback
                    }
                    
                    txtPassword.Visible = true;
                    lblPassword.Visible = true;
                    txtPassword.Enabled = true;
                    txtPassword.UseSystemPasswordChar = false; // Show password in plain text for SuperAdmin
                    txtPassword.ReadOnly = false; // Allow editing
                }
                else
                {
                    txtPassword.Text = ""; // Clear password field for non-SuperAdmin
                    txtPassword.Visible = false;
                    lblPassword.Visible = false;
                    txtPassword.Enabled = false;
                }
                
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
            // Check if current user is SuperAdmin and adjust UI accordingly
            CheckSuperAdminAccess();
        }

        private bool IsSuperAdmin()
        {
            try
            {
                var currentUser = UserSession.CurrentUser;
                if (currentUser == null) return false;
                
                var role = (currentUser.Role ?? string.Empty).Trim();
                return role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || 
                       role.Equals("Super Admin", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void CheckSuperAdminAccess()
        {
            bool isSuperAdmin = IsSuperAdmin();
            
            // If not SuperAdmin, hide password field completely
            if (!isSuperAdmin)
            {
                txtPassword.Visible = false;
                lblPassword.Visible = false;
                txtPassword.Enabled = false;
            }
            else
            {
                // SuperAdmin can see and edit passwords
                txtPassword.Visible = true;
                lblPassword.Visible = true;
                txtPassword.Enabled = true;
                txtPassword.ReadOnly = false;
                txtPassword.UseSystemPasswordChar = false; // Show password in plain text for SuperAdmin
            }
        }
    }
}
