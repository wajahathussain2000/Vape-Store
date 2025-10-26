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
    

