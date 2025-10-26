using System;
using System.Drawing;
using System.Windows.Forms;

namespace Vape_Store
{
    public partial class ProductEntryForm : Form
    {
        public string ProductName { get; private set; }
        public string ProductCode { get; private set; }
        public decimal Quantity { get; private set; }
        public string Unit { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public decimal SellingPrice { get; private set; }
        public string BatchNo { get; private set; }
        public DateTime ExpiryDate { get; private set; }

        private TextBox txtProductName;
        private TextBox txtProductCode;
        private TextBox txtQuantity;
        private ComboBox cmbUnit;
        private TextBox txtPurchasePrice;
        private TextBox txtSellingPrice;
        private TextBox txtBatchNo;
        private DateTimePicker dtpExpiryDate;
        private Button btnAdd;
        private Button btnCancel;

        public ProductEntryForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Add Product to Purchase";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Create controls
            CreateControls();
            LayoutControls();
            
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // Product Name
            var lblProductName = new Label
            {
                Text = "Product Name:",
                Location = new Point(20, 20),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblProductName);

            txtProductName = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(txtProductName);

            // Product Code
            var lblProductCode = new Label
            {
                Text = "Product Code:",
                Location = new Point(240, 20),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblProductCode);

            txtProductCode = new TextBox
            {
                Location = new Point(240, 45),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(txtProductCode);

            // Quantity
            var lblQuantity = new Label
            {
                Text = "Quantity:",
                Location = new Point(20, 80),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblQuantity);

            txtQuantity = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(100, 25),
                Text = "1",
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(txtQuantity);

            // Unit
            var lblUnit = new Label
            {
                Text = "Unit:",
                Location = new Point(140, 80),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblUnit);

            cmbUnit = new ComboBox
            {
                Location = new Point(140, 105),
                Size = new Size(80, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbUnit.Items.AddRange(new object[] { "pcs", "kg", "box", "pack", "liter", "gram" });
            cmbUnit.SelectedIndex = 0;
            this.Controls.Add(cmbUnit);

            // Purchase Price
            var lblPurchasePrice = new Label
            {
                Text = "Purchase Price:",
                Location = new Point(240, 80),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblPurchasePrice);

            txtPurchasePrice = new TextBox
            {
                Location = new Point(240, 105),
                Size = new Size(100, 25),
                Text = "0.00",
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(txtPurchasePrice);

            // Selling Price
            var lblSellingPrice = new Label
            {
                Text = "Selling Price:",
                Location = new Point(360, 80),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblSellingPrice);

            txtSellingPrice = new TextBox
            {
                Location = new Point(360, 105),
                Size = new Size(100, 25),
                Text = "0.00",
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(txtSellingPrice);

            // Batch No
            var lblBatchNo = new Label
            {
                Text = "Batch No:",
                Location = new Point(20, 140),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblBatchNo);

            txtBatchNo = new TextBox
            {
                Location = new Point(20, 165),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(txtBatchNo);

            // Expiry Date
            var lblExpiryDate = new Label
            {
                Text = "Expiry Date:",
                Location = new Point(160, 140),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblExpiryDate);

            dtpExpiryDate = new DateTimePicker
            {
                Location = new Point(160, 165),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddYears(1),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(dtpExpiryDate);

            // Buttons
            btnAdd = new Button
            {
                Text = "Add Product",
                Location = new Point(300, 200),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(410, 200),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);
        }

        private void LayoutControls()
        {
            // Set form size based on controls
            this.Size = new Size(520, 280);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(txtProductName.Text.Trim()))
                {
                    MessageBox.Show("Please enter product name.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtProductCode.Text.Trim()))
                {
                    MessageBox.Show("Please enter product code.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductCode.Focus();
                    return;
                }

                if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity greater than 0.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQuantity.Focus();
                    return;
                }

                if (!decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice) || purchasePrice < 0)
                {
                    MessageBox.Show("Please enter a valid purchase price.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchasePrice.Focus();
                    return;
                }

                if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice) || sellingPrice < 0)
                {
                    MessageBox.Show("Please enter a valid selling price.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSellingPrice.Focus();
                    return;
                }

                // Set properties
                ProductName = txtProductName.Text.Trim();
                ProductCode = txtProductCode.Text.Trim();
                Quantity = quantity;
                Unit = cmbUnit.SelectedItem?.ToString() ?? "pcs";
                PurchasePrice = purchasePrice;
                SellingPrice = sellingPrice;
                BatchNo = txtBatchNo.Text.Trim();
                ExpiryDate = dtpExpiryDate.Value;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}


