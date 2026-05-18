namespace Vape_Store
{
    partial class Products
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Print_button = new System.Windows.Forms.Button();
            this.ADD_button = new System.Windows.Forms.Button();
            this.txtStockQuantity = new System.Windows.Forms.TextBox();
            this.lblStockQuantity = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Del_button = new System.Windows.Forms.Button();
            this.barcodeGroup = new System.Windows.Forms.GroupBox();
            this.pnlBarcode = new System.Windows.Forms.Panel();
            this.generateBtn = new System.Windows.Forms.Button();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.barcodeLabel = new System.Windows.Forms.Label();
            this.Exit_button = new System.Windows.Forms.Button();
            this.gridGroup = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.Clear_button = new System.Windows.Forms.Button();
            this.Save_button = new System.Windows.Forms.Button();
            this.productGroup = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtretailprice = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReorderLevel = new System.Windows.Forms.TextBox();
            this.reorderLabel = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.priceLabel = new System.Windows.Forms.Label();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.brandLabel = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.codeLabel = new System.Windows.Forms.Label();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.barcodeGroup.SuspendLayout();
            this.gridGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.productGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // Print_button
            // 
            this.Print_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.Print_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Print_button.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.Print_button.ForeColor = System.Drawing.Color.White;
            this.Print_button.Location = new System.Drawing.Point(195, 754);
            this.Print_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Print_button.Name = "Print_button";
            this.Print_button.Size = new System.Drawing.Size(150, 62);
            this.Print_button.TabIndex = 23;
            this.Print_button.Text = "Print";
            this.Print_button.UseVisualStyleBackColor = false;
            this.Print_button.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // ADD_button
            // 
            this.ADD_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ADD_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ADD_button.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.ADD_button.ForeColor = System.Drawing.Color.White;
            this.ADD_button.Location = new System.Drawing.Point(30, 754);
            this.ADD_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ADD_button.Name = "ADD_button";
            this.ADD_button.Size = new System.Drawing.Size(150, 62);
            this.ADD_button.TabIndex = 22;
            this.ADD_button.Text = "Save";
            this.ADD_button.UseVisualStyleBackColor = false;
            this.ADD_button.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // txtStockQuantity
            // 
            this.txtStockQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStockQuantity.Location = new System.Drawing.Point(231, 548);
            this.txtStockQuantity.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtStockQuantity.Name = "txtStockQuantity";
            this.txtStockQuantity.ReadOnly = true;
            this.txtStockQuantity.Size = new System.Drawing.Size(300, 34);
            this.txtStockQuantity.TabIndex = 31;
            this.txtStockQuantity.Text = "0";
            this.txtStockQuantity.TextChanged += new System.EventHandler(this.txtStockQuantity_TextChanged);
            // 
            // lblStockQuantity
            // 
            this.lblStockQuantity.AutoSize = true;
            this.lblStockQuantity.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockQuantity.Location = new System.Drawing.Point(60, 548);
            this.lblStockQuantity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStockQuantity.Name = "lblStockQuantity";
            this.lblStockQuantity.Size = new System.Drawing.Size(145, 28);
            this.lblStockQuantity.TabIndex = 30;
            this.lblStockQuantity.Text = "Available Qty:";
            this.lblStockQuantity.Click += new System.EventHandler(this.lblStockQuantity_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.panel4.Controls.Add(this.lblTitle);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1924, 77);
            this.panel4.TabIndex = 39;
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(225, 45);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Add Products";
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel1.Controls.Add(this.Del_button);
            this.panel1.Controls.Add(this.barcodeGroup);
            this.panel1.Controls.Add(this.Exit_button);
            this.panel1.Controls.Add(this.gridGroup);
            this.panel1.Controls.Add(this.Clear_button);
            this.panel1.Controls.Add(this.Save_button);
            this.panel1.Controls.Add(this.productGroup);
            this.panel1.Controls.Add(this.Print_button);
            this.panel1.Controls.Add(this.ADD_button);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1924, 923);
            this.panel1.TabIndex = 0;
            // 
            // Del_button
            // 
            this.Del_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.Del_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Del_button.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.Del_button.ForeColor = System.Drawing.Color.White;
            this.Del_button.Location = new System.Drawing.Point(855, 754);
            this.Del_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Del_button.Name = "Del_button";
            this.Del_button.Size = new System.Drawing.Size(150, 62);
            this.Del_button.TabIndex = 27;
            this.Del_button.Text = "Delete";
            this.Del_button.UseVisualStyleBackColor = false;
            this.Del_button.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // barcodeGroup
            // 
            this.barcodeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.barcodeGroup.Controls.Add(this.pnlBarcode);
            this.barcodeGroup.Controls.Add(this.generateBtn);
            this.barcodeGroup.Controls.Add(this.txtBarcode);
            this.barcodeGroup.Controls.Add(this.barcodeLabel);
            this.barcodeGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.barcodeGroup.Location = new System.Drawing.Point(628, 22);
            this.barcodeGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.barcodeGroup.Name = "barcodeGroup";
            this.barcodeGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.barcodeGroup.Size = new System.Drawing.Size(756, 308);
            this.barcodeGroup.TabIndex = 41;
            this.barcodeGroup.TabStop = false;
            this.barcodeGroup.Text = "Barcode Information";
            this.barcodeGroup.Enter += new System.EventHandler(this.barcodeGroup_Enter);
            // 
            // pnlBarcode
            // 
            this.pnlBarcode.BackColor = System.Drawing.Color.White;
            this.pnlBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBarcode.Location = new System.Drawing.Point(30, 138);
            this.pnlBarcode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlBarcode.Name = "pnlBarcode";
            this.pnlBarcode.Size = new System.Drawing.Size(659, 122);
            this.pnlBarcode.TabIndex = 3;
            this.pnlBarcode.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBarcode_Paint);
            // 
            // generateBtn
            // 
            this.generateBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.generateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generateBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.generateBtn.ForeColor = System.Drawing.Color.White;
            this.generateBtn.Location = new System.Drawing.Point(510, 85);
            this.generateBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.generateBtn.Name = "generateBtn";
            this.generateBtn.Size = new System.Drawing.Size(210, 55);
            this.generateBtn.TabIndex = 2;
            this.generateBtn.Text = "Generate Barcode";
            this.generateBtn.UseVisualStyleBackColor = false;
            this.generateBtn.Click += new System.EventHandler(this.GenerateBarcode_Click);
            // 
            // txtBarcode
            // 
            this.txtBarcode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBarcode.Location = new System.Drawing.Point(30, 85);
            this.txtBarcode.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(450, 34);
            this.txtBarcode.TabIndex = 1;
            this.txtBarcode.TextChanged += new System.EventHandler(this.txtBarcode_TextChanged_1);
            // 
            // barcodeLabel
            // 
            this.barcodeLabel.AutoSize = true;
            this.barcodeLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.barcodeLabel.Location = new System.Drawing.Point(30, 46);
            this.barcodeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.barcodeLabel.Name = "barcodeLabel";
            this.barcodeLabel.Size = new System.Drawing.Size(263, 28);
            this.barcodeLabel.TabIndex = 0;
            this.barcodeLabel.Text = "Barcode (Custom or Auto):";
            this.barcodeLabel.Click += new System.EventHandler(this.barcodeLabel_Click);
            // 
            // Exit_button
            // 
            this.Exit_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.Exit_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Exit_button.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.Exit_button.ForeColor = System.Drawing.Color.White;
            this.Exit_button.Location = new System.Drawing.Point(690, 754);
            this.Exit_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Exit_button.Name = "Exit_button";
            this.Exit_button.Size = new System.Drawing.Size(150, 62);
            this.Exit_button.TabIndex = 26;
            this.Exit_button.Text = "Exit";
            this.Exit_button.UseVisualStyleBackColor = false;
            this.Exit_button.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // gridGroup
            // 
            this.gridGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridGroup.Controls.Add(this.label3);
            this.gridGroup.Controls.Add(this.txtSearch);
            this.gridGroup.Controls.Add(this.dgvProducts);
            this.gridGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridGroup.Location = new System.Drawing.Point(616, 346);
            this.gridGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gridGroup.Name = "gridGroup";
            this.gridGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gridGroup.Size = new System.Drawing.Size(1392, 369);
            this.gridGroup.TabIndex = 40;
            this.gridGroup.TabStop = false;
            this.gridGroup.Text = "Products List";
            this.gridGroup.Enter += new System.EventHandler(this.gridGroup_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(30, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 28);
            this.label3.TabIndex = 42;
            this.label3.Text = "Search:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.Location = new System.Drawing.Point(117, 37);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(450, 31);
            this.txtSearch.TabIndex = 33;
            // 
            // dgvProducts
            // 
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.AllowUserToDeleteRows = false;
            this.dgvProducts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProducts.BackgroundColor = System.Drawing.Color.White;
            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProducts.Location = new System.Drawing.Point(23, 90);
            this.dgvProducts.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.RowHeadersVisible = false;
            this.dgvProducts.RowHeadersWidth = 62;
            this.dgvProducts.Size = new System.Drawing.Size(1272, 254);
            this.dgvProducts.TabIndex = 0;
            this.dgvProducts.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProducts_CellContentClick);
            // 
            // Clear_button
            // 
            this.Clear_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.Clear_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Clear_button.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.Clear_button.ForeColor = System.Drawing.Color.White;
            this.Clear_button.Location = new System.Drawing.Point(525, 754);
            this.Clear_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Clear_button.Name = "Clear_button";
            this.Clear_button.Size = new System.Drawing.Size(150, 62);
            this.Clear_button.TabIndex = 25;
            this.Clear_button.Text = "Clear";
            this.Clear_button.UseVisualStyleBackColor = false;
            this.Clear_button.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // Save_button
            // 
            this.Save_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.Save_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Save_button.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.Save_button.ForeColor = System.Drawing.Color.White;
            this.Save_button.Location = new System.Drawing.Point(360, 754);
            this.Save_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Save_button.Name = "Save_button";
            this.Save_button.Size = new System.Drawing.Size(150, 62);
            this.Save_button.TabIndex = 24;
            this.Save_button.Text = "Update";
            this.Save_button.UseVisualStyleBackColor = false;
            this.Save_button.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // productGroup
            // 
            this.productGroup.Controls.Add(this.txtStockQuantity);
            this.productGroup.Controls.Add(this.lblStockQuantity);
            this.productGroup.Controls.Add(this.checkBox1);
            this.productGroup.Controls.Add(this.label2);
            this.productGroup.Controls.Add(this.txtretailprice);
            this.productGroup.Controls.Add(this.label1);
            this.productGroup.Controls.Add(this.txtReorderLevel);
            this.productGroup.Controls.Add(this.reorderLabel);
            this.productGroup.Controls.Add(this.txtPrice);
            this.productGroup.Controls.Add(this.priceLabel);
            this.productGroup.Controls.Add(this.cmbBrand);
            this.productGroup.Controls.Add(this.brandLabel);
            this.productGroup.Controls.Add(this.cmbCategory);
            this.productGroup.Controls.Add(this.categoryLabel);
            this.productGroup.Controls.Add(this.txtDescription);
            this.productGroup.Controls.Add(this.descLabel);
            this.productGroup.Controls.Add(this.txtProductName);
            this.productGroup.Controls.Add(this.nameLabel);
            this.productGroup.Controls.Add(this.txtProductCode);
            this.productGroup.Controls.Add(this.codeLabel);
            this.productGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productGroup.Location = new System.Drawing.Point(14, 20);
            this.productGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.productGroup.Name = "productGroup";
            this.productGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.productGroup.Size = new System.Drawing.Size(567, 692);
            this.productGroup.TabIndex = 1;
            this.productGroup.TabStop = false;
            this.productGroup.Text = "Product Information";
            this.productGroup.Enter += new System.EventHandler(this.productGroup_Enter_1);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.checkBox1.Location = new System.Drawing.Point(230, 638);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(98, 32);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Active";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(51, 638);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 28);
            this.label2.TabIndex = 29;
            this.label2.Text = "Active Product:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtretailprice
            // 
            this.txtretailprice.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtretailprice.Location = new System.Drawing.Point(231, 428);
            this.txtretailprice.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtretailprice.Name = "txtretailprice";
            this.txtretailprice.Size = new System.Drawing.Size(300, 34);
            this.txtretailprice.TabIndex = 28;
            this.txtretailprice.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(82, 428);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 28);
            this.label1.TabIndex = 27;
            this.label1.Text = "Retail Price:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtReorderLevel
            // 
            this.txtReorderLevel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtReorderLevel.Location = new System.Drawing.Point(231, 488);
            this.txtReorderLevel.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtReorderLevel.Name = "txtReorderLevel";
            this.txtReorderLevel.Size = new System.Drawing.Size(300, 34);
            this.txtReorderLevel.TabIndex = 17;
            this.txtReorderLevel.TextChanged += new System.EventHandler(this.txtReorderLevel_TextChanged);
            // 
            // reorderLabel
            // 
            this.reorderLabel.AutoSize = true;
            this.reorderLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reorderLabel.Location = new System.Drawing.Point(62, 488);
            this.reorderLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.reorderLabel.Name = "reorderLabel";
            this.reorderLabel.Size = new System.Drawing.Size(147, 28);
            this.reorderLabel.TabIndex = 16;
            this.reorderLabel.Text = "Reorder Level:";
            this.reorderLabel.Click += new System.EventHandler(this.reorderLabel_Click);
            // 
            // txtPrice
            // 
            this.txtPrice.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPrice.Location = new System.Drawing.Point(231, 366);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(300, 34);
            this.txtPrice.TabIndex = 15;
            this.txtPrice.TextChanged += new System.EventHandler(this.txtPrice_TextChanged);
            // 
            // priceLabel
            // 
            this.priceLabel.AutoSize = true;
            this.priceLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.priceLabel.Location = new System.Drawing.Point(52, 366);
            this.priceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.priceLabel.Name = "priceLabel";
            this.priceLabel.Size = new System.Drawing.Size(155, 28);
            this.priceLabel.TabIndex = 14;
            this.priceLabel.Text = "Purchase Price:";
            this.priceLabel.Click += new System.EventHandler(this.priceLabel_Click);
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(231, 305);
            this.cmbBrand.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(298, 36);
            this.cmbBrand.TabIndex = 9;
            this.cmbBrand.SelectedIndexChanged += new System.EventHandler(this.cmbBrand_SelectedIndexChanged);
            // 
            // brandLabel
            // 
            this.brandLabel.AutoSize = true;
            this.brandLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brandLabel.Location = new System.Drawing.Point(135, 305);
            this.brandLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.brandLabel.Name = "brandLabel";
            this.brandLabel.Size = new System.Drawing.Size(73, 28);
            this.brandLabel.TabIndex = 8;
            this.brandLabel.Text = "Brand:";
            this.brandLabel.Click += new System.EventHandler(this.brandLabel_Click);
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(230, 243);
            this.cmbCategory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(298, 36);
            this.cmbCategory.TabIndex = 7;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryLabel.Location = new System.Drawing.Point(70, 243);
            this.categoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(127, 28);
            this.categoryLabel.TabIndex = 6;
            this.categoryLabel.Text = "Category:";
            this.categoryLabel.Click += new System.EventHandler(this.categoryLabel_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDescription.Location = new System.Drawing.Point(230, 154);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(302, 77);
            this.txtDescription.TabIndex = 5;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // descLabel
            // 
            this.descLabel.AutoSize = true;
            this.descLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descLabel.Location = new System.Drawing.Point(82, 166);
            this.descLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descLabel.Name = "descLabel";
            this.descLabel.Size = new System.Drawing.Size(126, 28);
            this.descLabel.TabIndex = 4;
            this.descLabel.Text = "Description:";
            this.descLabel.Click += new System.EventHandler(this.descLabel_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtProductName.Location = new System.Drawing.Point(231, 105);
            this.txtProductName.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(300, 34);
            this.txtProductName.TabIndex = 3;
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(56, 105);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(153, 28);
            this.nameLabel.TabIndex = 2;
            this.nameLabel.Text = "Product Name:";
            this.nameLabel.Click += new System.EventHandler(this.nameLabel_Click);
            // 
            // txtProductCode
            // 
            this.txtProductCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtProductCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtProductCode.Location = new System.Drawing.Point(231, 46);
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(6, 9, 6, 9);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.ReadOnly = true;
            this.txtProductCode.Size = new System.Drawing.Size(300, 34);
            this.txtProductCode.TabIndex = 1;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            // 
            // codeLabel
            // 
            this.codeLabel.AutoSize = true;
            this.codeLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeLabel.Location = new System.Drawing.Point(30, 53);
            this.codeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(144, 28);
            this.codeLabel.TabIndex = 0;
            this.codeLabel.Text = "Product Code:";
            this.codeLabel.Click += new System.EventHandler(this.codeLabel_Click);
            // 
            // Products
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1924, 1000);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Name = "Products";
            this.Text = "Vape Store - Products Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.barcodeGroup.ResumeLayout(false);
            this.barcodeGroup.PerformLayout();
            this.gridGroup.ResumeLayout(false);
            this.gridGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.productGroup.ResumeLayout(false);
            this.productGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox productGroup;
        private System.Windows.Forms.TextBox txtretailprice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReorderLevel;
        private System.Windows.Forms.Label reorderLabel;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.Label brandLabel;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gridGroup;
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.Button Del_button;
        private System.Windows.Forms.Button Exit_button;
        private System.Windows.Forms.Button Clear_button;
        private System.Windows.Forms.Button Save_button;
        private System.Windows.Forms.Button Print_button;
        private System.Windows.Forms.Button ADD_button;
        private System.Windows.Forms.GroupBox barcodeGroup;
        private System.Windows.Forms.Panel pnlBarcode;
        private System.Windows.Forms.Button generateBtn;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label barcodeLabel;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStockQuantity;
        private System.Windows.Forms.Label lblStockQuantity;
    }
}
