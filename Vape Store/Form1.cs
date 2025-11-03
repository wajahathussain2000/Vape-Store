using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Vape_Store.Services;
using Vape_Store.Models;
using Vape_Store.Repositories;


namespace Vape_Store
{
    public partial class Form1 : Form
    {
        private AuthenticationService _authService;
        string cs = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
        
        public Form1()
        {
            InitializeComponent();
            _authService = new AuthenticationService();
        }

        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (ValidateLogin())
            {
                try
                {
                    LoadingHelper.ShowLoading("Authenticating user...");
                    
                    // Use the authentication service
                    var user = _authService.Login(txtEmail.Text, txtPassword.Text);
                    
                    if (user != null)
                    {
                        LoadingHelper.UpdateLoading("Loading dashboard...");
                        
                        // Store user session
                        UserSession.CurrentUser = new UserSession
                        {
                            UserID = user.UserID,
                            Username = user.Username,
                            FullName = user.FullName,
                            Role = user.Role,
                            LoginTime = DateTime.Now
                        };

                        // Load roles and permissions from DB (if tables present)
                        try
                        {
                            var roleRepo = new RoleRepository();
                            var dbRoles = roleRepo.GetRolesForUser(user.UserID);
                            var dbPerms = roleRepo.GetEffectivePermissionsForUser(user.UserID);
                            UserSession.CurrentUser.Roles = dbRoles ?? new List<string>();
                            UserSession.CurrentUser.Permissions = new System.Collections.Generic.HashSet<string>((dbPerms ?? new List<string>()).Select(p => (p ?? string.Empty).Trim().ToLower()));
                        }
                        catch { /* ignore if schema not present */ }
                        
                        LoadingHelper.HideLoading();
                        
                        // Show dashboard
                        Dashboard dashboard = new Dashboard();
                        dashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        LoadingHelper.HideLoading();
                        MessageBox.Show("Invalid Username or Password!", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    LoadingHelper.HideLoading();
                    MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateLogin()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter Username!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter Password!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            return true;
        }



    }

}
    

