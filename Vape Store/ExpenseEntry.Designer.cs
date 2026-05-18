namespace Vape_Store
{
    partial class ExpenseEntry
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
            this.txtRemarks = new Guna.UI2.WinForms.Guna2TextBox();
            this.remarksLabel = new System.Windows.Forms.Label();
            this.txtReferenceNumber = new Guna.UI2.WinForms.Guna2TextBox();
            this.refLabel = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new Guna.UI2.WinForms.Guna2ComboBox();
            this.paymentLabel = new System.Windows.Forms.Label();
            this.txtAmount = new Guna.UI2.WinForms.Guna2TextBox();
            this.amountLabel = new System.Windows.Forms.Label();
            this.dtpExpenseDate = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.dateLabel = new System.Windows.Forms.Label();
            this.txtDescription = new Guna.UI2.WinForms.Guna2TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.cmbCategory = new Guna.UI2.WinForms.Guna2ComboBox();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.txtExpenseCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerLabel = new System.Windows.Forms.Label();
            this.expenseGroup = new Guna.UI2.WinForms.Guna2GroupBox();
            this.codeLabel = new System.Windows.Forms.Label();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.expenseListGroup = new Guna.UI2.WinForms.Guna2GroupBox();
            this.dgvExpenses = new Guna.UI2.WinForms.Guna2DataGridView();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.btncategory = new Guna.UI2.WinForms.Guna2Button();
            this.btnClear = new Guna.UI2.WinForms.Guna2Button();
            this.btnSubmit = new Guna.UI2.WinForms.Guna2Button();
            this.btnSaveDraft = new Guna.UI2.WinForms.Guna2Button();
            this.headerPanel.SuspendLayout();
            this.expenseGroup.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            this.expenseListGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            this.SuspendLayout();
            // 
            // txtRemarks
            // 
            this.txtRemarks.BorderRadius = 5;
            this.txtRemarks.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRemarks.DefaultText = "";
            this.txtRemarks.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtRemarks.ForeColor = System.Drawing.Color.Black;
            this.txtRemarks.Location = new System.Drawing.Point(490, 480);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(410, 108);
            this.txtRemarks.TabIndex = 17;
            this.txtRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            // 
            // remarksLabel
            // 
            this.remarksLabel.AutoSize = true;
            this.remarksLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.remarksLabel.ForeColor = System.Drawing.Color.Black;
            this.remarksLabel.Location = new System.Drawing.Point(490, 450);
            this.remarksLabel.Name = "remarksLabel";
            this.remarksLabel.Size = new System.Drawing.Size(99, 28);
            this.remarksLabel.TabIndex = 16;
            this.remarksLabel.Text = "Remarks:";
            // 
            // txtReferenceNumber
            // 
            this.txtReferenceNumber.BorderRadius = 5;
            this.txtReferenceNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtReferenceNumber.DefaultText = "";
            this.txtReferenceNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtReferenceNumber.ForeColor = System.Drawing.Color.Black;
            this.txtReferenceNumber.Location = new System.Drawing.Point(20, 550);
            this.txtReferenceNumber.Name = "txtReferenceNumber";
            this.txtReferenceNumber.Size = new System.Drawing.Size(430, 37);
            this.txtReferenceNumber.TabIndex = 15;
            // 
            // refLabel
            // 
            this.refLabel.AutoSize = true;
            this.refLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.refLabel.ForeColor = System.Drawing.Color.Black;
            this.refLabel.Location = new System.Drawing.Point(20, 520);
            this.refLabel.Name = "refLabel";
            this.refLabel.Size = new System.Drawing.Size(147, 28);
            this.refLabel.TabIndex = 14;
            this.refLabel.Text = "Reference No:";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.BackColor = System.Drawing.Color.Transparent;
            this.cmbPaymentMethod.BorderRadius = 5;
            this.cmbPaymentMethod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbPaymentMethod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbPaymentMethod.ItemHeight = 31;
            this.cmbPaymentMethod.Location = new System.Drawing.Point(20, 480);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(430, 37);
            this.cmbPaymentMethod.TabIndex = 13;
            // 
            // paymentLabel
            // 
            this.paymentLabel.AutoSize = true;
            this.paymentLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.paymentLabel.ForeColor = System.Drawing.Color.Black;
            this.paymentLabel.Location = new System.Drawing.Point(20, 450);
            this.paymentLabel.Name = "paymentLabel";
            this.paymentLabel.Size = new System.Drawing.Size(179, 28);
            this.paymentLabel.TabIndex = 12;
            this.paymentLabel.Text = "Payment Method:";
            // 
            // txtAmount
            // 
            this.txtAmount.BorderRadius = 5;
            this.txtAmount.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAmount.DefaultText = "";
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtAmount.ForeColor = System.Drawing.Color.Black;
            this.txtAmount.Location = new System.Drawing.Point(20, 410);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(430, 37);
            this.txtAmount.TabIndex = 11;
            // 
            // amountLabel
            // 
            this.amountLabel.AutoSize = true;
            this.amountLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.amountLabel.ForeColor = System.Drawing.Color.Black;
            this.amountLabel.Location = new System.Drawing.Point(20, 380);
            this.amountLabel.Name = "amountLabel";
            this.amountLabel.Size = new System.Drawing.Size(94, 28);
            this.amountLabel.TabIndex = 10;
            this.amountLabel.Text = "Amount:";
            // 
            // dtpExpenseDate
            // 
            this.dtpExpenseDate.BorderRadius = 5;
            this.dtpExpenseDate.Checked = true;
            this.dtpExpenseDate.FillColor = System.Drawing.Color.White;
            this.dtpExpenseDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dtpExpenseDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpenseDate.Location = new System.Drawing.Point(20, 340);
            this.dtpExpenseDate.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpExpenseDate.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpExpenseDate.Name = "dtpExpenseDate";
            this.dtpExpenseDate.Size = new System.Drawing.Size(430, 37);
            this.dtpExpenseDate.TabIndex = 9;
            this.dtpExpenseDate.Value = new System.DateTime(2026, 2, 20, 0, 0, 0, 0);
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dateLabel.ForeColor = System.Drawing.Color.Black;
            this.dateLabel.Location = new System.Drawing.Point(20, 310);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(145, 28);
            this.dateLabel.TabIndex = 8;
            this.dateLabel.Text = "Expense Date:";
            // 
            // txtDescription
            // 
            this.txtDescription.BorderRadius = 5;
            this.txtDescription.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDescription.DefaultText = "";
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtDescription.ForeColor = System.Drawing.Color.Black;
            this.txtDescription.Location = new System.Drawing.Point(20, 240);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(880, 60);
            this.txtDescription.TabIndex = 7;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            // 
            // descLabel
            // 
            this.descLabel.AutoSize = true;
            this.descLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.descLabel.ForeColor = System.Drawing.Color.Black;
            this.descLabel.Location = new System.Drawing.Point(20, 210);
            this.descLabel.Name = "descLabel";
            this.descLabel.Size = new System.Drawing.Size(126, 28);
            this.descLabel.TabIndex = 6;
            this.descLabel.Text = "Description:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.BackColor = System.Drawing.Color.Transparent;
            this.cmbCategory.BorderRadius = 5;
            this.cmbCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbCategory.ItemHeight = 31;
            this.cmbCategory.Location = new System.Drawing.Point(20, 160);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(430, 37);
            this.cmbCategory.TabIndex = 5;
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.categoryLabel.ForeColor = System.Drawing.Color.Black;
            this.categoryLabel.Location = new System.Drawing.Point(20, 130);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(104, 28);
            this.categoryLabel.TabIndex = 4;
            this.categoryLabel.Text = "Category:";
            // 
            // txtExpenseCode
            // 
            this.txtExpenseCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtExpenseCode.BorderRadius = 5;
            this.txtExpenseCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtExpenseCode.DefaultText = "";
            this.txtExpenseCode.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtExpenseCode.ForeColor = System.Drawing.Color.Black;
            this.txtExpenseCode.Location = new System.Drawing.Point(20, 80);
            this.txtExpenseCode.Name = "txtExpenseCode";
            this.txtExpenseCode.ReadOnly = true;
            this.txtExpenseCode.Size = new System.Drawing.Size(430, 37);
            this.txtExpenseCode.TabIndex = 1;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1894, 80);
            this.headerPanel.TabIndex = 2;
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.White;
            this.headerLabel.Location = new System.Drawing.Point(24, 15);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(256, 48);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Expense Entry";
            // 
            // expenseGroup
            // 
            this.expenseGroup.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.expenseGroup.BorderRadius = 5;
            this.expenseGroup.Controls.Add(this.txtRemarks);
            this.expenseGroup.Controls.Add(this.remarksLabel);
            this.expenseGroup.Controls.Add(this.txtReferenceNumber);
            this.expenseGroup.Controls.Add(this.refLabel);
            this.expenseGroup.Controls.Add(this.cmbPaymentMethod);
            this.expenseGroup.Controls.Add(this.paymentLabel);
            this.expenseGroup.Controls.Add(this.txtAmount);
            this.expenseGroup.Controls.Add(this.amountLabel);
            this.expenseGroup.Controls.Add(this.dtpExpenseDate);
            this.expenseGroup.Controls.Add(this.dateLabel);
            this.expenseGroup.Controls.Add(this.txtDescription);
            this.expenseGroup.Controls.Add(this.descLabel);
            this.expenseGroup.Controls.Add(this.cmbCategory);
            this.expenseGroup.Controls.Add(this.categoryLabel);
            this.expenseGroup.Controls.Add(this.txtExpenseCode);
            this.expenseGroup.Controls.Add(this.codeLabel);
            this.expenseGroup.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.expenseGroup.FillColor = System.Drawing.Color.White;
            this.expenseGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.expenseGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.expenseGroup.Location = new System.Drawing.Point(20, 20);
            this.expenseGroup.Name = "expenseGroup";
            this.expenseGroup.Size = new System.Drawing.Size(926, 610);
            this.expenseGroup.TabIndex = 0;
            this.expenseGroup.Text = "Expense Details";
            this.expenseGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // codeLabel
            // 
            this.codeLabel.AutoSize = true;
            this.codeLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.codeLabel.ForeColor = System.Drawing.Color.Black;
            this.codeLabel.Location = new System.Drawing.Point(20, 50);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(148, 28);
            this.codeLabel.TabIndex = 0;
            this.codeLabel.Text = "Expense Code:";
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.Controls.Add(this.btncategory);
            this.panelMainContainer.Controls.Add(this.btnClear);
            this.panelMainContainer.Controls.Add(this.btnSubmit);
            this.panelMainContainer.Controls.Add(this.btnSaveDraft);
            this.panelMainContainer.Controls.Add(this.expenseListGroup);
            this.panelMainContainer.Controls.Add(this.expenseGroup);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 80);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(20);
            this.panelMainContainer.Size = new System.Drawing.Size(1894, 892);
            this.panelMainContainer.TabIndex = 3;
            // 
            // expenseListGroup
            // 
            this.expenseListGroup.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.expenseListGroup.BorderRadius = 5;
            this.expenseListGroup.Controls.Add(this.dgvExpenses);
            this.expenseListGroup.Controls.Add(this.lblSearch);
            this.expenseListGroup.Controls.Add(this.txtSearch);
            this.expenseListGroup.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.expenseListGroup.FillColor = System.Drawing.Color.White;
            this.expenseListGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.expenseListGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.expenseListGroup.Location = new System.Drawing.Point(960, 20);
            this.expenseListGroup.Name = "expenseListGroup";
            this.expenseListGroup.Size = new System.Drawing.Size(900, 610);
            this.expenseListGroup.TabIndex = 2;
            this.expenseListGroup.Text = "Expense List";
            this.expenseListGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dgvExpenses
            // 
            this.dgvExpenses.AllowUserToAddRows = false;
            this.dgvExpenses.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvExpenses.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvExpenses.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvExpenses.BackgroundColor = System.Drawing.Color.White;
            this.dgvExpenses.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvExpenses.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvExpenses.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvExpenses.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvExpenses.ColumnHeadersHeight = 35;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvExpenses.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvExpenses.EnableHeadersVisualStyles = false;
            this.dgvExpenses.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvExpenses.Location = new System.Drawing.Point(15, 100);
            this.dgvExpenses.MultiSelect = false;
            this.dgvExpenses.Name = "dgvExpenses";
            this.dgvExpenses.ReadOnly = true;
            this.dgvExpenses.RowHeadersVisible = false;
            this.dgvExpenses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvExpenses.Size = new System.Drawing.Size(870, 490);
            this.dgvExpenses.TabIndex = 1;
            this.dgvExpenses.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            this.dgvExpenses.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvExpenses.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvExpenses.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblSearch.ForeColor = System.Drawing.Color.Black;
            this.lblSearch.Location = new System.Drawing.Point(20, 50);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(80, 28);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.BorderRadius = 5;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSearch.ForeColor = System.Drawing.Color.Black;
            this.txtSearch.Location = new System.Drawing.Point(100, 50);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(430, 31);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.PlaceholderText = "Search expenses...";
            // 
            // btncategory
            // 
            this.btncategory.BorderRadius = 5;
            this.btncategory.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btncategory.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btncategory.ForeColor = System.Drawing.Color.White;
            this.btncategory.Location = new System.Drawing.Point(1550, 650);
            this.btncategory.Name = "btncategory";
            this.btncategory.Size = new System.Drawing.Size(310, 50);
            this.btncategory.TabIndex = 18;
            this.btncategory.Text = "Add Category";
            this.btncategory.Click += new System.EventHandler(this.btnexpensecategory_Click);
            // 
            // btnClear
            // 
            this.btnClear.BorderRadius = 5;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(450, 650);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(150, 50);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            // 
            // btnSubmit
            // 
            this.btnSubmit.BorderRadius = 5;
            this.btnSubmit.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(220, 650);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(210, 50);
            this.btnSubmit.TabIndex = 1;
            this.btnSubmit.Text = "Submit";
            // 
            // btnSaveDraft
            // 
            this.btnSaveDraft.BorderRadius = 5;
            this.btnSaveDraft.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnSaveDraft.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSaveDraft.ForeColor = System.Drawing.Color.White;
            this.btnSaveDraft.Location = new System.Drawing.Point(20, 650);
            this.btnSaveDraft.Name = "btnSaveDraft";
            this.btnSaveDraft.Size = new System.Drawing.Size(180, 50);
            this.btnSaveDraft.TabIndex = 0;
            this.btnSaveDraft.Text = "Save Draft";
            // 
            // ExpenseEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1894, 972);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.headerPanel);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "ExpenseEntry";
            this.Text = "Vape Store - Expense Entry";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.expenseGroup.ResumeLayout(false);
            this.expenseGroup.PerformLayout();
            this.panelMainContainer.ResumeLayout(false);
            this.expenseListGroup.ResumeLayout(false);
            this.expenseListGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox txtRemarks;
        private System.Windows.Forms.Label remarksLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtReferenceNumber;
        private System.Windows.Forms.Label refLabel;
        private Guna.UI2.WinForms.Guna2ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label paymentLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtAmount;
        private System.Windows.Forms.Label amountLabel;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpExpenseDate;
        private System.Windows.Forms.Label dateLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtDescription;
        private System.Windows.Forms.Label descLabel;
        private Guna.UI2.WinForms.Guna2ComboBox cmbCategory;
        private System.Windows.Forms.Label categoryLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtExpenseCode;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerLabel;
        private Guna.UI2.WinForms.Guna2GroupBox expenseGroup;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.Panel panelMainContainer;
        private Guna.UI2.WinForms.Guna2GroupBox expenseListGroup;
        private Guna.UI2.WinForms.Guna2DataGridView dgvExpenses;
        private System.Windows.Forms.Label lblSearch;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnClear;
        private Guna.UI2.WinForms.Guna2Button btnSubmit;
        private Guna.UI2.WinForms.Guna2Button btnSaveDraft;
        private Guna.UI2.WinForms.Guna2Button btncategory;

    }
}
