namespace Vape_Store
{
    partial class NewSale
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.guna2DateTimePicker1 = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtStockQuantity = new System.Windows.Forms.TextBox();
            this.stockLabel = new System.Windows.Forms.Label();
            this.txtReorderLevel = new System.Windows.Forms.TextBox();
            this.reorderLabel = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.priceLabel = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.brandLabel = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.SrNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SubTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClearBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.NewItemBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtsubTotal = new System.Windows.Forms.TextBox();
            this.txtTaxPercent = new System.Windows.Forms.TextBox();
            this.txtDiscountAmount = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbTax = new System.Windows.Forms.ComboBox();
            this.txtTaxPercentInput = new System.Windows.Forms.TextBox();
            this.txtTaxAmountInput = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtChange = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtPaid = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(40, 13);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Invoice No:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(125, 13);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "000013";
            // 
            // guna2DateTimePicker1
            // 
            this.guna2DateTimePicker1.Checked = true;
            this.guna2DateTimePicker1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2DateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.guna2DateTimePicker1.Location = new System.Drawing.Point(32, 36);
            this.guna2DateTimePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.guna2DateTimePicker1.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.guna2DateTimePicker1.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.guna2DateTimePicker1.Name = "guna2DateTimePicker1";
            this.guna2DateTimePicker1.Size = new System.Drawing.Size(199, 23);
            this.guna2DateTimePicker1.TabIndex = 3;
            this.guna2DateTimePicker1.Value = new System.DateTime(2025, 10, 14, 8, 58, 52, 205);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtStockQuantity);
            this.panel1.Controls.Add(this.stockLabel);
            this.panel1.Controls.Add(this.txtReorderLevel);
            this.panel1.Controls.Add(this.reorderLabel);
            this.panel1.Controls.Add(this.txtProductName);
            this.panel1.Controls.Add(this.txtQuantity);
            this.panel1.Controls.Add(this.btnAddItem);
            this.panel1.Controls.Add(this.priceLabel);
            this.panel1.Controls.Add(this.cmbCustomer);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cmbBrand);
            this.panel1.Controls.Add(this.brandLabel);
            this.panel1.Controls.Add(this.cmbCategory);
            this.panel1.Controls.Add(this.categoryLabel);
            this.panel1.Controls.Add(this.guna2DateTimePicker1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1084, 145);
            this.panel1.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(522, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 19);
            this.label6.TabIndex = 26;
            this.label6.Text = "Quantity:";
            // 
            // txtStockQuantity
            // 
            this.txtStockQuantity.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStockQuantity.Location = new System.Drawing.Point(978, 103);
            this.txtStockQuantity.Name = "txtStockQuantity";
            this.txtStockQuantity.ReadOnly = true;
            this.txtStockQuantity.Size = new System.Drawing.Size(77, 25);
            this.txtStockQuantity.TabIndex = 25;
            // 
            // stockLabel
            // 
            this.stockLabel.AutoSize = true;
            this.stockLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stockLabel.Location = new System.Drawing.Point(837, 107);
            this.stockLabel.Name = "stockLabel";
            this.stockLabel.Size = new System.Drawing.Size(137, 19);
            this.stockLabel.TabIndex = 24;
            this.stockLabel.Text = "Available Quantity:";
            // 
            // txtReorderLevel
            // 
            this.txtReorderLevel.BackColor = System.Drawing.Color.Red;
            this.txtReorderLevel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReorderLevel.Location = new System.Drawing.Point(978, 73);
            this.txtReorderLevel.Name = "txtReorderLevel";
            this.txtReorderLevel.ReadOnly = true;
            this.txtReorderLevel.Size = new System.Drawing.Size(77, 25);
            this.txtReorderLevel.TabIndex = 21;
            // 
            // reorderLabel
            // 
            this.reorderLabel.AutoSize = true;
            this.reorderLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reorderLabel.Location = new System.Drawing.Point(863, 75);
            this.reorderLabel.Name = "reorderLabel";
            this.reorderLabel.Size = new System.Drawing.Size(107, 19);
            this.reorderLabel.TabIndex = 20;
            this.reorderLabel.Text = "Reorder Level:";
            // 
            // txtProductName
            // 
            this.txtProductName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProductName.Location = new System.Drawing.Point(153, 103);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(319, 25);
            this.txtProductName.TabIndex = 17;
            // 
            // txtQuantity
            // 
            this.txtQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtQuantity.Location = new System.Drawing.Point(594, 101);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(68, 25);
            this.txtQuantity.TabIndex = 18;
            this.txtQuantity.Text = "1";
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.Color.Green;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Location = new System.Drawing.Point(671, 101);
            this.btnAddItem.Margin = new System.Windows.Forms.Padding(2);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(89, 27);
            this.btnAddItem.TabIndex = 19;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.UseVisualStyleBackColor = false;
            this.btnAddItem.Click += new System.EventHandler(this.BtnAddItem_Click);
            // 
            // priceLabel
            // 
            this.priceLabel.AutoSize = true;
            this.priceLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.priceLabel.Location = new System.Drawing.Point(39, 103);
            this.priceLabel.Name = "priceLabel";
            this.priceLabel.Size = new System.Drawing.Size(110, 19);
            this.priceLabel.TabIndex = 16;
            this.priceLabel.Text = "Product Name:";
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(837, 31);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(200, 25);
            this.cmbCustomer.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(837, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 19);
            this.label5.TabIndex = 14;
            this.label5.Text = "Customer:";
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(467, 34);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(200, 25);
            this.cmbBrand.TabIndex = 13;
            // 
            // brandLabel
            // 
            this.brandLabel.AutoSize = true;
            this.brandLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brandLabel.Location = new System.Drawing.Point(467, 9);
            this.brandLabel.Name = "brandLabel";
            this.brandLabel.Size = new System.Drawing.Size(53, 19);
            this.brandLabel.TabIndex = 12;
            this.brandLabel.Text = "Brand:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(247, 34);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(200, 25);
            this.cmbCategory.TabIndex = 11;
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryLabel.Location = new System.Drawing.Point(247, 9);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(100, 19);
            this.categoryLabel.TabIndex = 10;
            this.categoryLabel.Text = "📂 Category:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SrNo,
            this.ItemName,
            this.Qty,
            this.Price,
            this.SubTotal});
            this.dataGridView1.Location = new System.Drawing.Point(0, 149);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dataGridView1.Size = new System.Drawing.Size(1116, 237);
            this.dataGridView1.TabIndex = 7;
            // 
            // SrNo
            // 
            this.SrNo.HeaderText = "Sr.No";
            this.SrNo.MinimumWidth = 8;
            this.SrNo.Name = "SrNo";
            this.SrNo.Width = 150;
            // 
            // ItemName
            // 
            this.ItemName.HeaderText = "ItemName";
            this.ItemName.MinimumWidth = 8;
            this.ItemName.Name = "ItemName";
            this.ItemName.Width = 450;
            // 
            // Qty
            // 
            this.Qty.HeaderText = "Qty";
            this.Qty.MinimumWidth = 8;
            this.Qty.Name = "Qty";
            this.Qty.Width = 180;
            // 
            // Price
            // 
            this.Price.HeaderText = "Price";
            this.Price.MinimumWidth = 8;
            this.Price.Name = "Price";
            this.Price.Width = 180;
            // 
            // SubTotal
            // 
            this.SubTotal.HeaderText = "SubTotal";
            this.SubTotal.MinimumWidth = 8;
            this.SubTotal.Name = "SubTotal";
            this.SubTotal.Width = 200;
            // 
            // ClearBtn
            // 
            this.ClearBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.ClearBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClearBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearBtn.ForeColor = System.Drawing.Color.White;
            this.ClearBtn.Location = new System.Drawing.Point(399, 567);
            this.ClearBtn.Name = "ClearBtn";
            this.ClearBtn.Size = new System.Drawing.Size(125, 40);
            this.ClearBtn.TabIndex = 17;
            this.ClearBtn.Text = "Clear";
            this.ClearBtn.UseVisualStyleBackColor = false;
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.ForeColor = System.Drawing.Color.White;
            this.CancelBtn.Location = new System.Drawing.Point(263, 567);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(125, 40);
            this.CancelBtn.TabIndex = 16;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = false;
            // 
            // NewItemBtn
            // 
            this.NewItemBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.NewItemBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NewItemBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemBtn.ForeColor = System.Drawing.Color.White;
            this.NewItemBtn.Location = new System.Drawing.Point(147, 567);
            this.NewItemBtn.Name = "NewItemBtn";
            this.NewItemBtn.Size = new System.Drawing.Size(105, 40);
            this.NewItemBtn.TabIndex = 15;
            this.NewItemBtn.Text = "New Item";
            this.NewItemBtn.UseVisualStyleBackColor = false;
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBtn.ForeColor = System.Drawing.Color.White;
            this.saveBtn.Location = new System.Drawing.Point(8, 567);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(129, 40);
            this.saveBtn.TabIndex = 14;
            this.saveBtn.Text = "Save and Print";
            this.saveBtn.UseVisualStyleBackColor = false;
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.RefreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RefreshBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RefreshBtn.ForeColor = System.Drawing.Color.White;
            this.RefreshBtn.Location = new System.Drawing.Point(535, 567);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(125, 40);
            this.RefreshBtn.TabIndex = 18;
            this.RefreshBtn.Text = "Refresh";
            this.RefreshBtn.UseVisualStyleBackColor = false;
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPaymentMethod.FormattingEnabled = true;
            this.cmbPaymentMethod.Location = new System.Drawing.Point(20, 454);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(200, 25);
            this.cmbPaymentMethod.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 426);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 19);
            this.label1.TabIndex = 18;
            this.label1.Text = "Payment Method:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(43, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 19);
            this.label2.TabIndex = 26;
            this.label2.Text = "Sub Total:";
            // 
            // txtsubTotal
            // 
            this.txtsubTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsubTotal.Location = new System.Drawing.Point(160, 18);
            this.txtsubTotal.Name = "txtsubTotal";
            this.txtsubTotal.Size = new System.Drawing.Size(129, 25);
            this.txtsubTotal.TabIndex = 25;
            // 
            // txtTaxPercent
            // 
            this.txtTaxPercent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaxPercent.Location = new System.Drawing.Point(160, 50);
            this.txtTaxPercent.Name = "txtTaxPercent";
            this.txtTaxPercent.Size = new System.Drawing.Size(66, 25);
            this.txtTaxPercent.TabIndex = 27;
            // 
            // txtDiscountAmount
            // 
            this.txtDiscountAmount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiscountAmount.Location = new System.Drawing.Point(232, 50);
            this.txtDiscountAmount.Name = "txtDiscountAmount";
            this.txtDiscountAmount.Size = new System.Drawing.Size(66, 25);
            this.txtDiscountAmount.TabIndex = 28;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(43, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 19);
            this.label10.TabIndex = 28;
            this.label10.Text = "Discount %:";
            // 
            // cmbTax
            // 
            this.cmbTax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTax.FormattingEnabled = true;
            this.cmbTax.Location = new System.Drawing.Point(160, 87);
            this.cmbTax.Name = "cmbTax";
            this.cmbTax.Size = new System.Drawing.Size(66, 25);
            this.cmbTax.TabIndex = 31;
            this.cmbTax.Visible = false;
            // 
            // txtTaxPercentInput
            // 
            this.txtTaxPercentInput.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaxPercentInput.Location = new System.Drawing.Point(160, 87);
            this.txtTaxPercentInput.Name = "txtTaxPercentInput";
            this.txtTaxPercentInput.Size = new System.Drawing.Size(66, 25);
            this.txtTaxPercentInput.TabIndex = 32;
            // 
            // txtTaxAmountInput
            // 
            this.txtTaxAmountInput.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaxAmountInput.Location = new System.Drawing.Point(232, 87);
            this.txtTaxAmountInput.Name = "txtTaxAmountInput";
            this.txtTaxAmountInput.Size = new System.Drawing.Size(66, 25);
            this.txtTaxAmountInput.TabIndex = 33;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(43, 87);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 19);
            this.label13.TabIndex = 30;
            this.label13.Text = "Tax %:";
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(160, 123);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(129, 25);
            this.txtTotal.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(43, 120);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 19);
            this.label14.TabIndex = 33;
            this.label14.Text = "Total:";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.txtChange);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.txtsubTotal);
            this.panel2.Controls.Add(this.txtPaid);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.txtTaxPercent);
            this.panel2.Controls.Add(this.txtDiscountAmount);
            this.panel2.Controls.Add(this.txtTotal);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.cmbTax);
            this.panel2.Controls.Add(this.txtTaxPercentInput);
            this.panel2.Controls.Add(this.txtTaxAmountInput);
            this.panel2.Location = new System.Drawing.Point(748, 409);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(328, 224);
            this.panel2.TabIndex = 34;
            // 
            // txtChange
            // 
            this.txtChange.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChange.Location = new System.Drawing.Point(160, 190);
            this.txtChange.Name = "txtChange";
            this.txtChange.ReadOnly = true;
            this.txtChange.Size = new System.Drawing.Size(129, 25);
            this.txtChange.TabIndex = 37;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(43, 188);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(63, 19);
            this.label16.TabIndex = 38;
            this.label16.Text = "Change:";
            // 
            // txtPaid
            // 
            this.txtPaid.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPaid.Location = new System.Drawing.Point(160, 154);
            this.txtPaid.Name = "txtPaid";
            this.txtPaid.Size = new System.Drawing.Size(129, 25);
            this.txtPaid.TabIndex = 35;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(43, 154);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 19);
            this.label15.TabIndex = 36;
            this.label15.Text = "Paid Amount:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(326, 426);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 19);
            this.label7.TabIndex = 35;
            this.label7.Text = "Barcode";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // NewSale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 573);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.RefreshBtn);
            this.Controls.Add(this.cmbPaymentMethod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ClearBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.NewItemBtn);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "NewSale";
            this.Text = "Attock Mobiles Rwp - New Sale";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.NewSale_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Guna.UI2.WinForms.Guna2DateTimePicker guna2DateTimePicker1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SrNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Qty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn SubTotal;
        private System.Windows.Forms.Button ClearBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button NewItemBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.Label brandLabel;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReorderLevel;
        private System.Windows.Forms.Label reorderLabel;
        private System.Windows.Forms.TextBox txtStockQuantity;
        private System.Windows.Forms.Label stockLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtsubTotal;
        private System.Windows.Forms.TextBox txtTaxPercent;
        private System.Windows.Forms.TextBox txtDiscountAmount;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbTax;
        private System.Windows.Forms.TextBox txtTaxPercentInput;
        private System.Windows.Forms.TextBox txtTaxAmountInput;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtPaid;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtChange;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}