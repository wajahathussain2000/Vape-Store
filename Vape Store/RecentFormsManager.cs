using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Vape_Store
{
    public static class RecentFormsManager
    {
        private static List<RecentForm> recentForms = new List<RecentForm>();
        private static int maxRecentForms = 10;

        public static event Action OnRecentFormsChanged;

        public static void AddForm(string formName, string formType, DateTime lastAccessed)
        {
            // Remove if already exists
            recentForms.RemoveAll(f => f.FormName == formName);
            
            // Add to beginning
            recentForms.Insert(0, new RecentForm
            {
                FormName = formName,
                FormType = formType,
                LastAccessed = lastAccessed
            });

            // Keep only max recent forms
            if (recentForms.Count > maxRecentForms)
            {
                recentForms.RemoveAt(recentForms.Count - 1);
            }

            OnRecentFormsChanged?.Invoke();
        }

        public static List<RecentForm> GetRecentForms()
        {
            return recentForms.OrderByDescending(f => f.LastAccessed).ToList();
        }

        public static void ClearRecentForms()
        {
            recentForms.Clear();
            OnRecentFormsChanged?.Invoke();
        }

        public static void RemoveForm(string formName)
        {
            recentForms.RemoveAll(f => f.FormName == formName);
            OnRecentFormsChanged?.Invoke();
        }
    }

    public class RecentForm
    {
        public string FormName { get; set; }
        public string FormType { get; set; }
        public DateTime LastAccessed { get; set; }
        public string Icon { get; set; }
    }

    public partial class RecentFormsPanel : UserControl
    {
        private ListBox lstRecentForms;
        private Button btnClearRecent;
        private Label lblTitle;

        public RecentFormsPanel()
        {
            InitializeComponent();
            LoadRecentForms();
            RecentFormsManager.OnRecentFormsChanged += LoadRecentForms;
        }

        private void InitializeComponent()
        {
            this.lstRecentForms = new ListBox();
            this.btnClearRecent = new Button();
            this.lblTitle = new Label();
            
            this.SuspendLayout();
            
            // Title
            this.lblTitle.Text = "📋 Recent Forms";
            this.lblTitle.Location = new Point(10, 10);
            this.lblTitle.Size = new Size(200, 20);
            this.lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Controls.Add(this.lblTitle);
            
            // Recent forms list
            this.lstRecentForms.Location = new Point(10, 35);
            this.lstRecentForms.Size = new Size(280, 200);
            this.lstRecentForms.Font = new Font("Segoe UI", 9F);
            this.lstRecentForms.DrawMode = DrawMode.OwnerDrawFixed;
            this.lstRecentForms.DrawItem += LstRecentForms_DrawItem;
            this.lstRecentForms.DoubleClick += LstRecentForms_DoubleClick;
            this.Controls.Add(this.lstRecentForms);
            
            // Clear button
            this.btnClearRecent.Text = "Clear Recent";
            this.btnClearRecent.Location = new Point(10, 245);
            this.btnClearRecent.Size = new Size(100, 25);
            this.btnClearRecent.Click += BtnClearRecent_Click;
            this.Controls.Add(this.btnClearRecent);
            
            this.Size = new Size(300, 280);
            this.ResumeLayout(false);
        }

        private void LoadRecentForms()
        {
            lstRecentForms.Items.Clear();
            var recentForms = RecentFormsManager.GetRecentForms();
            
            foreach (var form in recentForms)
            {
                lstRecentForms.Items.Add(form);
            }
        }

        private void LstRecentForms_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var form = (RecentForm)lstRecentForms.Items[e.Index];
            
            // Background
            e.DrawBackground();
            
            // Icon and text
            string icon = GetFormIcon(form.FormType);
            string text = $"{icon} {form.FormName}";
            
            using (Brush brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds.X + 5, e.Bounds.Y + 2);
            }
            
            // Time stamp
            string timeText = form.LastAccessed.ToString("MM/dd HH:mm");
            using (Brush timeBrush = new SolidBrush(Color.Gray))
            {
                var timeSize = e.Graphics.MeasureString(timeText, e.Font);
                e.Graphics.DrawString(timeText, e.Font, timeBrush, 
                    e.Bounds.Right - timeSize.Width - 5, e.Bounds.Y + 2);
            }
        }

        private string GetFormIcon(string formType)
        {
            switch (formType.ToLower())
            {
                case "products": return "📦";
                case "customers": return "👥";
                case "sales": return "💰";
                case "purchases": return "🛒";
                case "dashboard": return "📊";
                case "reports": return "📈";
                default: return "📄";
            }
        }

        private void LstRecentForms_DoubleClick(object sender, EventArgs e)
        {
            if (lstRecentForms.SelectedItem is RecentForm selectedForm)
            {
                OpenForm(selectedForm.FormName);
            }
        }

        private void OpenForm(string formName)
        {
            try
            {
                Form form = null;
                
                switch (formName.ToLower())
                {
                    case "products":
                        form = new Products();
                        break;
                    case "customers":
                        form = new Customers();
                        break;
                    case "new sale":
                        form = new NewSale();
                        break;
                    case "new purchase":
                        form = new NewPurchase();
                        break;
                    case "dashboard":
                        form = new Dashboard();
                        break;
                    case "user management":
                        // UserManagement temporarily disabled - will be available after UI controls are added
                        MessageBox.Show("User Management feature will be available after UI controls are added to the Designer files.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                }
                
                if (form != null)
                {
                    form.Show();
                    RecentFormsManager.AddForm(formName, GetFormType(formName), DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFormType(string formName)
        {
            switch (formName.ToLower())
            {
                case "products": return "products";
                case "customers": return "customers";
                case "new sale": return "sales";
                case "new purchase": return "purchases";
                case "dashboard": return "dashboard";
                case "user management": return "users";
                default: return "general";
            }
        }

        private void BtnClearRecent_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Clear all recent forms?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                RecentFormsManager.ClearRecentForms();
            }
        }
    }
}
