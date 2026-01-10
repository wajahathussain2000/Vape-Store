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
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.remarksLabel = new System.Windows.Forms.Label();
            this.txtReferenceNumber = new System.Windows.Forms.TextBox();
            this.refLabel = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.paymentLabel = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.amountLabel = new System.Windows.Forms.Label();
            this.dtpExpenseDate = new System.Windows.Forms.DateTimePicker();
            this.dateLabel = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.txtExpenseCode = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerLabel = new System.Windows.Forms.Label();
            this.expenseGroup = new System.Windows.Forms.GroupBox();
            this.codeLabel = new System.Windows.Forms.Label();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.expenseListGroup = new System.Windows.Forms.GroupBox();
            this.dgvExpenses = new System.Windows.Forms.DataGridView();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.actionsGroup = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnSaveDraft = new System.Windows.Forms.Button();
            this.btncategory = new System.Windows.Forms.Button();
            this.headerPanel.SuspendLayout();
            this.expenseGroup.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.expenseListGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            this.searchPanel.SuspendLayout();
            this.actionsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtRemarks
            // 
            this.txtRemarks.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRemarks.Location = new System.Drawing.Point(499, 508);
            this.txtRemarks.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRemarks.Size = new System.Drawing.Size(388, 108);
            this.txtRemarks.TabIndex = 17;
            // 
            // remarksLabel
            // 
            this.remarksLabel.AutoSize = true;
            this.remarksLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.remarksLabel.Location = new System.Drawing.Point(499, 474);
            this.remarksLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.remarksLabel.Name = "remarksLabel";
            this.remarksLabel.Size = new System.Drawing.Size(90, 25);
            this.remarksLabel.TabIndex = 16;
            this.remarksLabel.Text = "Remarks:";
            // 
            // txtReferenceNumber
            // 
            this.txtReferenceNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtReferenceNumber.Location = new System.Drawing.Point(34, 585);
            this.txtReferenceNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReferenceNumber.Name = "txtReferenceNumber";
            this.txtReferenceNumber.Size = new System.Drawing.Size(414, 31);
            this.txtReferenceNumber.TabIndex = 15;
            // 
            // refLabel
            // 
            this.refLabel.AutoSize = true;
            this.refLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.refLabel.Location = new System.Drawing.Point(34, 551);
            this.refLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.refLabel.Name = "refLabel";
            this.refLabel.Size = new System.Drawing.Size(133, 25);
            this.refLabel.TabIndex = 14;
            this.refLabel.Text = "Reference No:";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPaymentMethod.FormattingEnabled = true;
            this.cmbPaymentMethod.Location = new System.Drawing.Point(34, 508);
            this.cmbPaymentMethod.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(414, 33);
            this.cmbPaymentMethod.TabIndex = 13;
            // 
            // paymentLabel
            // 
            this.paymentLabel.AutoSize = true;
            this.paymentLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.paymentLabel.Location = new System.Drawing.Point(34, 474);
            this.paymentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paymentLabel.Name = "paymentLabel";
            this.paymentLabel.Size = new System.Drawing.Size(164, 25);
            this.paymentLabel.TabIndex = 12;
            this.paymentLabel.Text = "Payment Method:";
            // 
            // txtAmount
            // 
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAmount.Location = new System.Drawing.Point(34, 408);
            this.txtAmount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(423, 31);
            this.txtAmount.TabIndex = 11;
            // 
            // amountLabel
            // 
            this.amountLabel.AutoSize = true;
            this.amountLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.amountLabel.Location = new System.Drawing.Point(34, 374);
            this.amountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.amountLabel.Name = "amountLabel";
            this.amountLabel.Size = new System.Drawing.Size(86, 25);
            this.amountLabel.TabIndex = 10;
            this.amountLabel.Text = "Amount:";
            // 
            // dtpExpenseDate
            // 
            this.dtpExpenseDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpExpenseDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpenseDate.Location = new System.Drawing.Point(34, 331);
            this.dtpExpenseDate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpExpenseDate.Name = "dtpExpenseDate";
            this.dtpExpenseDate.Size = new System.Drawing.Size(423, 31);
            this.dtpExpenseDate.TabIndex = 9;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dateLabel.Location = new System.Drawing.Point(34, 297);
            this.dateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(132, 25);
            this.dateLabel.TabIndex = 8;
            this.dateLabel.Text = "Expense Date:";
            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDescription.Location = new System.Drawing.Point(34, 254);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(423, 36);
            this.txtDescription.TabIndex = 7;
            // 
            // descLabel
            // 
            this.descLabel.AutoSize = true;
            this.descLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.descLabel.Location = new System.Drawing.Point(34, 220);
            this.descLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descLabel.Name = "descLabel";
            this.descLabel.Size = new System.Drawing.Size(114, 25);
            this.descLabel.TabIndex = 6;
            this.descLabel.Text = "Description:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(34, 177);
            this.cmbCategory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(423, 33);
            this.cmbCategory.TabIndex = 5;
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.categoryLabel.Location = new System.Drawing.Point(34, 144);
            this.categoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(95, 25);
            this.categoryLabel.TabIndex = 4;
            this.categoryLabel.Text = "Category:";
            // 
            // txtExpenseCode
            // 
            this.txtExpenseCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtExpenseCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtExpenseCode.Location = new System.Drawing.Point(39, 96);
            this.txtExpenseCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtExpenseCode.Name = "txtExpenseCode";
            this.txtExpenseCode.ReadOnly = true;
            this.txtExpenseCode.Size = new System.Drawing.Size(418, 31);
            this.txtExpenseCode.TabIndex = 1;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp, *.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|A" +
    "ll files (*.*)|*.*";
            this.openFileDialog.Title = "Select Expense Image";
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.SeaGreen;
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1894, 100);
            this.headerPanel.TabIndex = 2;
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.White;
            this.headerLabel.Location = new System.Drawing.Point(32, 27);
            this.headerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(230, 45);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Expense Entry";
            // 
            // expenseGroup
            // 
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
            this.expenseGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.expenseGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.expenseGroup.Location = new System.Drawing.Point(40, 128);
            this.expenseGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.expenseGroup.Name = "expenseGroup";
            this.expenseGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.expenseGroup.Size = new System.Drawing.Size(926, 658);
            this.expenseGroup.TabIndex = 0;
            this.expenseGroup.TabStop = false;
            this.expenseGroup.Text = "Expense Details";
            // 
            // codeLabel
            // 
            this.codeLabel.AutoSize = true;
            this.codeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.codeLabel.Location = new System.Drawing.Point(34, 66);
            this.codeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(135, 25);
            this.codeLabel.TabIndex = 0;
            this.codeLabel.Text = "Expense Code:";
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.expenseListGroup);
            this.contentPanel.Controls.Add(this.actionsGroup);
            this.contentPanel.Controls.Add(this.expenseGroup);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 0);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(15);
            this.contentPanel.Size = new System.Drawing.Size(1894, 972);
            this.contentPanel.TabIndex = 3;
            // 
            // expenseListGroup
            // 
            this.expenseListGroup.Controls.Add(this.dgvExpenses);
            this.expenseListGroup.Controls.Add(this.searchPanel);
            this.expenseListGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.expenseListGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.expenseListGroup.Location = new System.Drawing.Point(998, 128);
            this.expenseListGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.expenseListGroup.Name = "expenseListGroup";
            this.expenseListGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.expenseListGroup.Size = new System.Drawing.Size(855, 658);
            this.expenseListGroup.TabIndex = 2;
            this.expenseListGroup.TabStop = false;
            this.expenseListGroup.Text = "Expense List";
            // 
            // dgvExpenses
            // 
            this.dgvExpenses.AllowUserToAddRows = false;
            this.dgvExpenses.AllowUserToDeleteRows = false;
            this.dgvExpenses.BackgroundColor = System.Drawing.Color.White;
            this.dgvExpenses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExpenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExpenses.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.dgvExpenses.Location = new System.Drawing.Point(4, 91);
            this.dgvExpenses.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvExpenses.MultiSelect = false;
            this.dgvExpenses.Name = "dgvExpenses";
            this.dgvExpenses.ReadOnly = true;
            this.dgvExpenses.RowHeadersVisible = false;
            this.dgvExpenses.RowHeadersWidth = 62;
            this.dgvExpenses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvExpenses.Size = new System.Drawing.Size(847, 562);
            this.dgvExpenses.TabIndex = 1;
            // 
            // searchPanel
            // 
            this.searchPanel.Controls.Add(this.lblSearch);
            this.searchPanel.Controls.Add(this.txtSearch);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(4, 29);
            this.searchPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(847, 62);
            this.searchPanel.TabIndex = 0;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSearch.Location = new System.Drawing.Point(15, 18);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(74, 25);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.Location = new System.Drawing.Point(100, 18);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(298, 31);
            this.txtSearch.TabIndex = 0;
            // 
            // actionsGroup
            // 
            this.actionsGroup.Controls.Add(this.btncategory);
            this.actionsGroup.Controls.Add(this.btnClear);
            this.actionsGroup.Controls.Add(this.btnSubmit);
            this.actionsGroup.Controls.Add(this.btnSaveDraft);
            this.actionsGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.actionsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.actionsGroup.Location = new System.Drawing.Point(40, 808);
            this.actionsGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.actionsGroup.Name = "actionsGroup";
            this.actionsGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.actionsGroup.Size = new System.Drawing.Size(1809, 123);
            this.actionsGroup.TabIndex = 1;
            this.actionsGroup.TabStop = false;
            this.actionsGroup.Text = "Actions";
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(450, 38);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(150, 62);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(225, 38);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(210, 62);
            this.btnSubmit.TabIndex = 1;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            // 
            // btnSaveDraft
            // 
            this.btnSaveDraft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnSaveDraft.FlatAppearance.BorderSize = 0;
            this.btnSaveDraft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDraft.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveDraft.ForeColor = System.Drawing.Color.White;
            this.btnSaveDraft.Location = new System.Drawing.Point(30, 38);
            this.btnSaveDraft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSaveDraft.Name = "btnSaveDraft";
            this.btnSaveDraft.Size = new System.Drawing.Size(180, 62);
            this.btnSaveDraft.TabIndex = 0;
            this.btnSaveDraft.Text = "Save Draft";
            this.btnSaveDraft.UseVisualStyleBackColor = false;
            // 
            // btncategory
            // 
            this.btncategory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btncategory.FlatAppearance.BorderSize = 0;
            this.btncategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btncategory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btncategory.ForeColor = System.Drawing.Color.White;
            this.btncategory.Location = new System.Drawing.Point(1591, 24);
            this.btncategory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btncategory.Name = "btncategory";
            this.btncategory.Size = new System.Drawing.Size(210, 89);
            this.btncategory.TabIndex = 18;
            this.btncategory.Text = "Add Category";
            this.btncategory.UseVisualStyleBackColor = false;
            this.btncategory.Click += new System.EventHandler(this.btnexpensecategory_Click);
            // 
            // ExpenseEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1894, 972);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.contentPanel);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "ExpenseEntry";
            this.Text = "Attock Mobiles Rwp - Expense Entry";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.expenseGroup.ResumeLayout(false);
            this.expenseGroup.PerformLayout();
            this.contentPanel.ResumeLayout(false);
            this.expenseListGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.actionsGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label remarksLabel;
        private System.Windows.Forms.TextBox txtReferenceNumber;
        private System.Windows.Forms.Label refLabel;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label paymentLabel;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label amountLabel;
        private System.Windows.Forms.DateTimePicker dtpExpenseDate;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.TextBox txtExpenseCode;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.GroupBox expenseGroup;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.GroupBox expenseListGroup;
        private System.Windows.Forms.DataGridView dgvExpenses;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.GroupBox actionsGroup;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnSaveDraft;
        private System.Windows.Forms.Button btncategory;
    }
}