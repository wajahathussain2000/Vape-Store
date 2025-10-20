using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Vape_Store
{
    public static class FavoritesManager
    {
        private static List<FavoriteItem> favorites = new List<FavoriteItem>();

        public static event Action OnFavoritesChanged;

        public static void AddFavorite(string name, string formType, string description = "")
        {
            if (!favorites.Any(f => f.Name == name))
            {
                favorites.Add(new FavoriteItem
                {
                    Name = name,
                    FormType = formType,
                    Description = description,
                    AddedDate = DateTime.Now
                });
                OnFavoritesChanged?.Invoke();
            }
        }

        public static void RemoveFavorite(string name)
        {
            favorites.RemoveAll(f => f.Name == name);
            OnFavoritesChanged?.Invoke();
        }

        public static List<FavoriteItem> GetFavorites()
        {
            return favorites.OrderBy(f => f.Name).ToList();
        }

        public static bool IsFavorite(string name)
        {
            return favorites.Any(f => f.Name == name);
        }

        public static void ClearFavorites()
        {
            favorites.Clear();
            OnFavoritesChanged?.Invoke();
        }
    }

    public class FavoriteItem
    {
        public string Name { get; set; }
        public string FormType { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public string Icon { get; set; }
    }

    public partial class FavoritesPanel : UserControl
    {
        private ListBox lstFavorites;
        private Button btnAddFavorite;
        private Button btnRemoveFavorite;
        private Button btnClearFavorites;
        private Label lblTitle;

        public FavoritesPanel()
        {
            InitializeComponent();
            LoadFavorites();
            FavoritesManager.OnFavoritesChanged += LoadFavorites;
        }

        private void InitializeComponent()
        {
            this.lstFavorites = new ListBox();
            this.btnAddFavorite = new Button();
            this.btnRemoveFavorite = new Button();
            this.btnClearFavorites = new Button();
            this.lblTitle = new Label();
            
            this.SuspendLayout();
            
            // Title
            this.lblTitle.Text = "⭐ Favorites";
            this.lblTitle.Location = new Point(10, 10);
            this.lblTitle.Size = new Size(200, 20);
            this.lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Controls.Add(this.lblTitle);
            
            // Favorites list
            this.lstFavorites.Location = new Point(10, 35);
            this.lstFavorites.Size = new Size(280, 150);
            this.lstFavorites.Font = new Font("Segoe UI", 9F);
            this.lstFavorites.DrawMode = DrawMode.OwnerDrawFixed;
            this.lstFavorites.DrawItem += LstFavorites_DrawItem;
            this.lstFavorites.DoubleClick += LstFavorites_DoubleClick;
            this.Controls.Add(this.lstFavorites);
            
            // Add button
            this.btnAddFavorite.Text = "Add";
            this.btnAddFavorite.Location = new Point(10, 195);
            this.btnAddFavorite.Size = new Size(60, 25);
            this.btnAddFavorite.Click += BtnAddFavorite_Click;
            this.Controls.Add(this.btnAddFavorite);
            
            // Remove button
            this.btnRemoveFavorite.Text = "Remove";
            this.btnRemoveFavorite.Location = new Point(80, 195);
            this.btnRemoveFavorite.Size = new Size(60, 25);
            this.btnRemoveFavorite.Click += BtnRemoveFavorite_Click;
            this.Controls.Add(this.btnRemoveFavorite);
            
            // Clear button
            this.btnClearFavorites.Text = "Clear All";
            this.btnClearFavorites.Location = new Point(150, 195);
            this.btnClearFavorites.Size = new Size(80, 25);
            this.btnClearFavorites.Click += BtnClearFavorites_Click;
            this.Controls.Add(this.btnClearFavorites);
            
            this.Size = new Size(300, 230);
            this.ResumeLayout(false);
        }

        private void LoadFavorites()
        {
            lstFavorites.Items.Clear();
            var favorites = FavoritesManager.GetFavorites();
            
            foreach (var favorite in favorites)
            {
                lstFavorites.Items.Add(favorite);
            }
        }

        private void LstFavorites_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var favorite = (FavoriteItem)lstFavorites.Items[e.Index];
            
            // Background
            e.DrawBackground();
            
            // Icon and text
            string icon = GetFavoriteIcon(favorite.FormType);
            string text = $"{icon} {favorite.Name}";
            
            using (Brush brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds.X + 5, e.Bounds.Y + 2);
            }
            
            // Description
            if (!string.IsNullOrEmpty(favorite.Description))
            {
                using (Brush descBrush = new SolidBrush(Color.Gray))
                {
                    e.Graphics.DrawString(favorite.Description, e.Font, descBrush, 
                        e.Bounds.X + 5, e.Bounds.Y + 15);
                }
            }
        }

        private string GetFavoriteIcon(string formType)
        {
            switch (formType.ToLower())
            {
                case "products": return "📦";
                case "customers": return "👥";
                case "sales": return "💰";
                case "purchases": return "🛒";
                case "dashboard": return "📊";
                case "reports": return "📈";
                default: return "⭐";
            }
        }

        private void LstFavorites_DoubleClick(object sender, EventArgs e)
        {
            if (lstFavorites.SelectedItem is FavoriteItem selectedFavorite)
            {
                OpenFavorite(selectedFavorite.Name);
            }
        }

        private void OpenFavorite(string favoriteName)
        {
            try
            {
                Form form = null;
                
                switch (favoriteName.ToLower())
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
                    RecentFormsManager.AddForm(favoriteName, GetFormType(favoriteName), DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening favorite: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFormType(string favoriteName)
        {
            switch (favoriteName.ToLower())
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

        private void BtnAddFavorite_Click(object sender, EventArgs e)
        {
            AddFavoriteDialog dialog = new AddFavoriteDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FavoritesManager.AddFavorite(dialog.FavoriteName, dialog.FormType, dialog.Description);
            }
        }

        private void BtnRemoveFavorite_Click(object sender, EventArgs e)
        {
            if (lstFavorites.SelectedItem is FavoriteItem selectedFavorite)
            {
                FavoritesManager.RemoveFavorite(selectedFavorite.Name);
            }
        }

        private void BtnClearFavorites_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Clear all favorites?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                FavoritesManager.ClearFavorites();
            }
        }
    }

    public partial class AddFavoriteDialog : Form
    {
        public string FavoriteName { get; private set; }
        public string FormType { get; private set; }
        public string Description { get; private set; }

        private TextBox txtName;
        private ComboBox cmbFormType;
        private TextBox txtDescription;
        private Button btnOK;
        private Button btnCancel;

        public AddFavoriteDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtName = new TextBox();
            this.cmbFormType = new ComboBox();
            this.txtDescription = new TextBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Add to Favorites";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Name
            Label lblName = new Label();
            lblName.Text = "Name:";
            lblName.Location = new Point(20, 20);
            lblName.Size = new Size(100, 20);
            this.Controls.Add(lblName);
            
            this.txtName.Location = new Point(130, 20);
            this.txtName.Size = new Size(200, 20);
            this.Controls.Add(this.txtName);
            
            // Form Type
            Label lblFormType = new Label();
            lblFormType.Text = "Form Type:";
            lblFormType.Location = new Point(20, 50);
            lblFormType.Size = new Size(100, 20);
            this.Controls.Add(lblFormType);
            
            this.cmbFormType.Location = new Point(130, 50);
            this.cmbFormType.Size = new Size(200, 20);
            this.cmbFormType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFormType.Items.AddRange(new string[] { "Products", "Customers", "Sales", "Purchases", "Dashboard", "Reports" });
            this.cmbFormType.SelectedIndex = 0;
            this.Controls.Add(this.cmbFormType);
            
            // Description
            Label lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Location = new Point(20, 80);
            lblDescription.Size = new Size(100, 20);
            this.Controls.Add(lblDescription);
            
            this.txtDescription.Location = new Point(130, 80);
            this.txtDescription.Size = new Size(200, 60);
            this.txtDescription.Multiline = true;
            this.Controls.Add(this.txtDescription);
            
            // OK Button
            this.btnOK.Text = "OK";
            this.btnOK.Location = new Point(200, 160);
            this.btnOK.Size = new Size(75, 25);
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Click += BtnOK_Click;
            this.Controls.Add(this.btnOK);
            
            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new Point(285, 160);
            this.btnCancel.Size = new Size(75, 25);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(this.btnCancel);
            
            this.ResumeLayout(false);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                FavoriteName = txtName.Text;
                FormType = cmbFormType.Text;
                Description = txtDescription.Text;
                this.DialogResult = DialogResult.OK;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }
    }
}
