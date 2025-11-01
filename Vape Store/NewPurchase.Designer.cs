namespace Vape_Store
{
    partial class NewPurchase
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
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.panelSummary = new System.Windows.Forms.Panel();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.lblBalance = new System.Windows.Forms.Label();
            this.txtPaid = new System.Windows.Forms.TextBox();
            this.lblPaid = new System.Windows.Forms.Label();
            this.txtNetAmount = new System.Windows.Forms.TextBox();
            this.lblNetAmount = new System.Windows.Forms.Label();
            this.txtTaxPercent = new System.Windows.Forms.TextBox();
            this.lblTaxPercent = new System.Windows.Forms.Label();
            this.txtDiscountPercent = new System.Windows.Forms.TextBox();
            this.lblDiscountPercent = new System.Windows.Forms.Label();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblSummaryTotals = new System.Windows.Forms.Label();
            this.panelItemDetails = new System.Windows.Forms.Panel();
            this.dgvPurchaseItems = new System.Windows.Forms.DataGridView();
            this.colSrNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colProductCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPurchasePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSalePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panelVendorInfo = new System.Windows.Forms.Panel();
            this.txtExistingStock = new System.Windows.Forms.TextBox();
            this.lblExistingStock = new System.Windows.Forms.Label();
            this.cmbProductName = new System.Windows.Forms.ComboBox();
            this.lblProductName = new System.Windows.Forms.Label();
            this.cmbPaymentTerms = new System.Windows.Forms.ComboBox();
            this.lblPaymentTerms = new System.Windows.Forms.Label();
            this.txtVendorCode = new System.Windows.Forms.TextBox();
            this.lblVendorCode = new System.Windows.Forms.Label();
            this.cmbVendorName = new System.Windows.Forms.ComboBox();
            this.lblVendorName = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.dtpInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.lblInvoiceDate = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.lblInvoiceNo = new System.Windows.Forms.Label();
            this.lblVendorInvoiceInfo = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClearForm = new System.Windows.Forms.Button();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.btnSavePurchase = new System.Windows.Forms.Button();
            this.btnAddItem = new System.Windows.Forms.Button();
             this.panelMainContainer.SuspendLayout();
            this.panelSummary.SuspendLayout();
            this.panelItemDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseItems)).BeginInit();
            this.panelVendorInfo.SuspendLayout();
             this.panelButtons.SuspendLayout();
             this.SuspendLayout();
             // 
             // panelMainContainer
             // 
             this.panelMainContainer.AutoScroll = true;
             this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelMainContainer.Controls.Add(this.panelSummary);
            this.panelMainContainer.Controls.Add(this.panelItemDetails);
            this.panelMainContainer.Controls.Add(this.panelVendorInfo);
             this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
             this.panelMainContainer.Location = new System.Drawing.Point(0, 0);
             this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(20);
            this.panelMainContainer.Size = new System.Drawing.Size(1200, 840);
             this.panelMainContainer.TabIndex = 0;
            // 
            // panelSummary
            // 
            this.panelSummary.BackColor = System.Drawing.Color.White;
            this.panelSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummary.Controls.Add(this.txtBalance);
            this.panelSummary.Controls.Add(this.lblBalance);
            this.panelSummary.Controls.Add(this.txtPaid);
            this.panelSummary.Controls.Add(this.lblPaid);
            this.panelSummary.Controls.Add(this.txtNetAmount);
            this.panelSummary.Controls.Add(this.lblNetAmount);
            this.panelSummary.Controls.Add(this.txtTaxPercent);
            this.panelSummary.Controls.Add(this.lblTaxPercent);
            this.panelSummary.Controls.Add(this.txtDiscountPercent);
            this.panelSummary.Controls.Add(this.lblDiscountPercent);
            this.panelSummary.Controls.Add(this.txtSubtotal);
            this.panelSummary.Controls.Add(this.lblSubtotal);
            this.panelSummary.Controls.Add(this.lblSummaryTotals);
            this.panelSummary.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSummary.Location = new System.Drawing.Point(20, 680);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Padding = new System.Windows.Forms.Padding(20);
            this.panelSummary.Size = new System.Drawing.Size(1160, 140);
            this.panelSummary.TabIndex = 2;
            // 
            // txtBalance
            // 
            this.txtBalance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBalance.Location = new System.Drawing.Point(780, 80);
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(120, 31);
            this.txtBalance.TabIndex = 12;
            this.txtBalance.Text = "0.00";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBalance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblBalance.Location = new System.Drawing.Point(700, 85);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(75, 25);
            this.lblBalance.TabIndex = 11;
            this.lblBalance.Text = "Balance:";
            // 
            // txtPaid
            // 
            this.txtPaid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPaid.Location = new System.Drawing.Point(560, 80);
            this.txtPaid.Name = "txtPaid";
            this.txtPaid.Size = new System.Drawing.Size(120, 31);
            this.txtPaid.TabIndex = 10;
            // 
            // lblPaid
            // 
            this.lblPaid.AutoSize = true;
            this.lblPaid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPaid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPaid.Location = new System.Drawing.Point(500, 85);
            this.lblPaid.Name = "lblPaid";
            this.lblPaid.Size = new System.Drawing.Size(49, 25);
            this.lblPaid.TabIndex = 9;
            this.lblPaid.Text = "Paid:";
            // 
            // txtNetAmount
            // 
            this.txtNetAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNetAmount.Location = new System.Drawing.Point(140, 91);
            this.txtNetAmount.Name = "txtNetAmount";
            this.txtNetAmount.ReadOnly = true;
            this.txtNetAmount.Size = new System.Drawing.Size(120, 31);
            this.txtNetAmount.TabIndex = 8;
            // 
            // lblNetAmount
            // 
            this.lblNetAmount.AutoSize = true;
            this.lblNetAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNetAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblNetAmount.Location = new System.Drawing.Point(20, 94);
            this.lblNetAmount.Name = "lblNetAmount";
            this.lblNetAmount.Size = new System.Drawing.Size(114, 25);
            this.lblNetAmount.TabIndex = 7;
            this.lblNetAmount.Text = "Net Amount:";
            // 
            // txtTaxPercent
            // 
            this.txtTaxPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTaxPercent.Location = new System.Drawing.Point(780, 40);
            this.txtTaxPercent.Name = "txtTaxPercent";
            this.txtTaxPercent.Size = new System.Drawing.Size(120, 31);
            this.txtTaxPercent.TabIndex = 6;
            // 
            // lblTaxPercent
            // 
            this.lblTaxPercent.AutoSize = true;
            this.lblTaxPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTaxPercent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTaxPercent.Location = new System.Drawing.Point(700, 45);
            this.lblTaxPercent.Name = "lblTaxPercent";
            this.lblTaxPercent.Size = new System.Drawing.Size(60, 25);
            this.lblTaxPercent.TabIndex = 5;
            this.lblTaxPercent.Text = "Tax %:";
            // 
            // txtDiscountPercent
            // 
            this.txtDiscountPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDiscountPercent.Location = new System.Drawing.Point(560, 40);
            this.txtDiscountPercent.Name = "txtDiscountPercent";
            this.txtDiscountPercent.Size = new System.Drawing.Size(120, 31);
            this.txtDiscountPercent.TabIndex = 4;
            // 
            // lblDiscountPercent
            // 
            this.lblDiscountPercent.AutoSize = true;
            this.lblDiscountPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDiscountPercent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDiscountPercent.Location = new System.Drawing.Point(460, 45);
            this.lblDiscountPercent.Name = "lblDiscountPercent";
            this.lblDiscountPercent.Size = new System.Drawing.Size(101, 25);
            this.lblDiscountPercent.TabIndex = 3;
            this.lblDiscountPercent.Text = "Discount%:";
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSubtotal.Location = new System.Drawing.Point(120, 40);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.ReadOnly = true;
            this.txtSubtotal.Size = new System.Drawing.Size(120, 31);
            this.txtSubtotal.TabIndex = 2;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubtotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSubtotal.Location = new System.Drawing.Point(20, 45);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(83, 25);
            this.lblSubtotal.TabIndex = 1;
            this.lblSubtotal.Text = "Subtotal:";
            // 
            // lblSummaryTotals
            // 
            this.lblSummaryTotals.AutoSize = true;
            this.lblSummaryTotals.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSummaryTotals.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSummaryTotals.Location = new System.Drawing.Point(20, 10);
            this.lblSummaryTotals.Name = "lblSummaryTotals";
            this.lblSummaryTotals.Size = new System.Drawing.Size(197, 32);
            this.lblSummaryTotals.TabIndex = 0;
            this.lblSummaryTotals.Text = "Summary Totals";
            // 
            // panelItemDetails
            // 
            this.panelItemDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelItemDetails.Controls.Add(this.dgvPurchaseItems);
            this.panelItemDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelItemDetails.Location = new System.Drawing.Point(20, 240);
            this.panelItemDetails.Name = "panelItemDetails";
            this.panelItemDetails.Padding = new System.Windows.Forms.Padding(20);
            this.panelItemDetails.Size = new System.Drawing.Size(1160, 580);
            this.panelItemDetails.TabIndex = 1;
            // 
            // dgvPurchaseItems
            // 
            this.dgvPurchaseItems.AllowUserToAddRows = false;
            this.dgvPurchaseItems.AllowUserToDeleteRows = false;
            this.dgvPurchaseItems.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvPurchaseItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPurchaseItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPurchaseItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSrNo,
            this.colProductName,
            this.colProductCode,
            this.colQty,
            this.colPurchasePrice,
            this.colSalePrice,
            this.colTotal,
            this.colDelete});
            this.dgvPurchaseItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPurchaseItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvPurchaseItems.Location = new System.Drawing.Point(20, 20);
            this.dgvPurchaseItems.Name = "dgvPurchaseItems";
            this.dgvPurchaseItems.RowHeadersWidth = 51;
            this.dgvPurchaseItems.Size = new System.Drawing.Size(1120, 540);
            this.dgvPurchaseItems.TabIndex = 0;
            // 
            // colSrNo
            // 
            this.colSrNo.HeaderText = "";
            this.colSrNo.MinimumWidth = 6;
            this.colSrNo.Name = "colSrNo";
            this.colSrNo.ReadOnly = true;
            this.colSrNo.Width = 50;
            // 
            // colProductName
            // 
            this.colProductName.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.colProductName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colProductName.HeaderText = "Product Name";
            this.colProductName.MinimumWidth = 6;
            this.colProductName.Name = "colProductName";
            this.colProductName.Width = 200;
            // 
            // colProductCode
            // 
            this.colProductCode.HeaderText = "Product Code";
            this.colProductCode.MinimumWidth = 6;
            this.colProductCode.Name = "colProductCode";
            this.colProductCode.ReadOnly = true;
            this.colProductCode.Width = 150;
            // 
            // colQty
            // 
            this.colQty.HeaderText = "Qty";
            this.colQty.MinimumWidth = 6;
            this.colQty.Name = "colQty";
            this.colQty.Width = 80;
            // 
            // colPurchasePrice
            // 
            this.colPurchasePrice.HeaderText = "Purchase Price";
            this.colPurchasePrice.MinimumWidth = 6;
            this.colPurchasePrice.Name = "colPurchasePrice";
            this.colPurchasePrice.Width = 150;
            // 
            // colSalePrice
            // 
            this.colSalePrice.HeaderText = "Sale Price";
            this.colSalePrice.MinimumWidth = 6;
            this.colSalePrice.Name = "colSalePrice";
            this.colSalePrice.Width = 150;
            // 
            // colTotal
            // 
            this.colTotal.HeaderText = "Total";
            this.colTotal.MinimumWidth = 6;
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.Width = 150;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "Delete";
            this.colDelete.MinimumWidth = 6;
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "Delete";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.Width = 80;
            // 
            // panelVendorInfo
            // 
            this.panelVendorInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelVendorInfo.Controls.Add(this.txtExistingStock);
            this.panelVendorInfo.Controls.Add(this.lblExistingStock);
            this.panelVendorInfo.Controls.Add(this.cmbProductName);
            this.panelVendorInfo.Controls.Add(this.lblProductName);
            this.panelVendorInfo.Controls.Add(this.cmbPaymentTerms);
            this.panelVendorInfo.Controls.Add(this.lblPaymentTerms);
            this.panelVendorInfo.Controls.Add(this.txtVendorCode);
            this.panelVendorInfo.Controls.Add(this.lblVendorCode);
            this.panelVendorInfo.Controls.Add(this.cmbVendorName);
            this.panelVendorInfo.Controls.Add(this.lblVendorName);
            this.panelVendorInfo.Controls.Add(this.txtDescription);
            this.panelVendorInfo.Controls.Add(this.lblDescription);
            this.panelVendorInfo.Controls.Add(this.dtpInvoiceDate);
            this.panelVendorInfo.Controls.Add(this.lblInvoiceDate);
            this.panelVendorInfo.Controls.Add(this.txtInvoiceNo);
            this.panelVendorInfo.Controls.Add(this.lblInvoiceNo);
            this.panelVendorInfo.Controls.Add(this.lblVendorInvoiceInfo);
            this.panelVendorInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelVendorInfo.Location = new System.Drawing.Point(20, 20);
            this.panelVendorInfo.Name = "panelVendorInfo";
            this.panelVendorInfo.Padding = new System.Windows.Forms.Padding(20);
            this.panelVendorInfo.Size = new System.Drawing.Size(1160, 220);
            this.panelVendorInfo.TabIndex = 0;
            // 
            // txtExistingStock
            // 
            this.txtExistingStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtExistingStock.Location = new System.Drawing.Point(160, 175);
            this.txtExistingStock.Name = "txtExistingStock";
            this.txtExistingStock.ReadOnly = true;
            this.txtExistingStock.Size = new System.Drawing.Size(150, 31);
            this.txtExistingStock.TabIndex = 17;
            // 
            // lblExistingStock
            // 
            this.lblExistingStock.AutoSize = true;
            this.lblExistingStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblExistingStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblExistingStock.Location = new System.Drawing.Point(20, 178);
            this.lblExistingStock.Name = "lblExistingStock";
            this.lblExistingStock.Size = new System.Drawing.Size(124, 25);
            this.lblExistingStock.TabIndex = 16;
            this.lblExistingStock.Text = "Existing Stock:";
            // 
            // cmbProductName
            // 
            this.cmbProductName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbProductName.FormattingEnabled = true;
            this.cmbProductName.Location = new System.Drawing.Point(160, 126);
            this.cmbProductName.Name = "cmbProductName";
            this.cmbProductName.Size = new System.Drawing.Size(250, 33);
            this.cmbProductName.TabIndex = 15;
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProductName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblProductName.Location = new System.Drawing.Point(20, 131);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(130, 25);
            this.lblProductName.TabIndex = 14;
            this.lblProductName.Text = "Product Name:";
            // 
            // cmbPaymentTerms
            // 
            this.cmbPaymentTerms.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPaymentTerms.FormattingEnabled = true;
            this.cmbPaymentTerms.Items.AddRange(new object[] {
            "Cash",
            "Credit",
            "30 Days",
            "60 Days",
            "90 Days"});
            this.cmbPaymentTerms.Location = new System.Drawing.Point(560, 61);
            this.cmbPaymentTerms.Name = "cmbPaymentTerms";
            this.cmbPaymentTerms.Size = new System.Drawing.Size(150, 33);
            this.cmbPaymentTerms.TabIndex = 13;
            // 
            // lblPaymentTerms
            // 
            this.lblPaymentTerms.AutoSize = true;
            this.lblPaymentTerms.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPaymentTerms.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPaymentTerms.Location = new System.Drawing.Point(405, 64);
            this.lblPaymentTerms.Name = "lblPaymentTerms";
            this.lblPaymentTerms.Size = new System.Drawing.Size(135, 25);
            this.lblPaymentTerms.TabIndex = 12;
            this.lblPaymentTerms.Text = "Payment Terms:";
            // 
            // txtVendorCode
            // 
            this.txtVendorCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtVendorCode.Location = new System.Drawing.Point(560, 126);
            this.txtVendorCode.Name = "txtVendorCode";
            this.txtVendorCode.ReadOnly = true;
            this.txtVendorCode.Size = new System.Drawing.Size(150, 31);
            this.txtVendorCode.TabIndex = 11;
            // 
            // lblVendorCode
            // 
            this.lblVendorCode.AutoSize = true;
            this.lblVendorCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblVendorCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblVendorCode.Location = new System.Drawing.Point(420, 132);
            this.lblVendorCode.Name = "lblVendorCode";
            this.lblVendorCode.Size = new System.Drawing.Size(120, 25);
            this.lblVendorCode.TabIndex = 10;
            this.lblVendorCode.Text = "Vendor Code:";
            // 
            // cmbVendorName
            // 
            this.cmbVendorName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbVendorName.FormattingEnabled = true;
            this.cmbVendorName.Location = new System.Drawing.Point(151, 85);
            this.cmbVendorName.Name = "cmbVendorName";
            this.cmbVendorName.Size = new System.Drawing.Size(229, 33);
            this.cmbVendorName.TabIndex = 9;
            // 
            // lblVendorName
            // 
            this.lblVendorName.AutoSize = true;
            this.lblVendorName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblVendorName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblVendorName.Location = new System.Drawing.Point(20, 85);
            this.lblVendorName.Name = "lblVendorName";
            this.lblVendorName.Size = new System.Drawing.Size(125, 25);
            this.lblVendorName.TabIndex = 8;
            this.lblVendorName.Text = "Vendor Name:";
            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDescription.Location = new System.Drawing.Point(956, 126);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(181, 31);
            this.txtDescription.TabIndex = 7;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDescription.Location = new System.Drawing.Point(962, 85);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(106, 25);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "Description:";
            // 
            // dtpInvoiceDate
            // 
            this.dtpInvoiceDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpInvoiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInvoiceDate.Location = new System.Drawing.Point(571, 11);
            this.dtpInvoiceDate.Name = "dtpInvoiceDate";
            this.dtpInvoiceDate.Size = new System.Drawing.Size(139, 31);
            this.dtpInvoiceDate.TabIndex = 5;
            // 
            // lblInvoiceDate
            // 
            this.lblInvoiceDate.AutoSize = true;
            this.lblInvoiceDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInvoiceDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblInvoiceDate.Location = new System.Drawing.Point(420, 16);
            this.lblInvoiceDate.Name = "lblInvoiceDate";
            this.lblInvoiceDate.Size = new System.Drawing.Size(114, 25);
            this.lblInvoiceDate.TabIndex = 4;
            this.lblInvoiceDate.Text = "Invoice Date:";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtInvoiceNo.Location = new System.Drawing.Point(130, 42);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(250, 31);
            this.txtInvoiceNo.TabIndex = 3;
            // 
            // lblInvoiceNo
            // 
            this.lblInvoiceNo.AutoSize = true;
            this.lblInvoiceNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInvoiceNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblInvoiceNo.Location = new System.Drawing.Point(20, 45);
            this.lblInvoiceNo.Name = "lblInvoiceNo";
            this.lblInvoiceNo.Size = new System.Drawing.Size(101, 25);
            this.lblInvoiceNo.TabIndex = 2;
            this.lblInvoiceNo.Text = "Invoice No:";
            // 
            // lblVendorInvoiceInfo
            // 
            this.lblVendorInvoiceInfo.AutoSize = true;
            this.lblVendorInvoiceInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblVendorInvoiceInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblVendorInvoiceInfo.Location = new System.Drawing.Point(20, 10);
            this.lblVendorInvoiceInfo.Name = "lblVendorInvoiceInfo";
            this.lblVendorInvoiceInfo.Size = new System.Drawing.Size(331, 32);
            this.lblVendorInvoiceInfo.TabIndex = 1;
            this.lblVendorInvoiceInfo.Text = "Vendor Invoice Information";
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnClearForm);
            this.panelButtons.Controls.Add(this.btnPrintInvoice);
            this.panelButtons.Controls.Add(this.btnSavePurchase);
            this.panelButtons.Controls.Add(this.btnAddItem);
             this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 840);
             this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(20);
            this.panelButtons.Size = new System.Drawing.Size(1200, 80);
            this.panelButtons.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(580, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnClearForm
            // 
            this.btnClearForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnClearForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearForm.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearForm.ForeColor = System.Drawing.Color.White;
            this.btnClearForm.Location = new System.Drawing.Point(460, 20);
            this.btnClearForm.Name = "btnClearForm";
            this.btnClearForm.Size = new System.Drawing.Size(100, 40);
            this.btnClearForm.TabIndex = 3;
            this.btnClearForm.Text = "Clear";
            this.btnClearForm.UseVisualStyleBackColor = false;
            // 
            // btnPrintInvoice
            // 
            this.btnPrintInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnPrintInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrintInvoice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrintInvoice.ForeColor = System.Drawing.Color.White;
            this.btnPrintInvoice.Location = new System.Drawing.Point(340, 20);
            this.btnPrintInvoice.Name = "btnPrintInvoice";
            this.btnPrintInvoice.Size = new System.Drawing.Size(100, 40);
            this.btnPrintInvoice.TabIndex = 2;
            this.btnPrintInvoice.Text = "Print Invoice";
            this.btnPrintInvoice.UseVisualStyleBackColor = false;
            // 
            // btnSavePurchase
            // 
            this.btnSavePurchase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSavePurchase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePurchase.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSavePurchase.ForeColor = System.Drawing.Color.White;
            this.btnSavePurchase.Location = new System.Drawing.Point(220, 20);
            this.btnSavePurchase.Name = "btnSavePurchase";
            this.btnSavePurchase.Size = new System.Drawing.Size(100, 40);
            this.btnSavePurchase.TabIndex = 1;
            this.btnSavePurchase.Text = "Save";
            this.btnSavePurchase.UseVisualStyleBackColor = false;
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Location = new System.Drawing.Point(20, 20);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(100, 40);
            this.btnAddItem.TabIndex = 0;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.UseVisualStyleBackColor = false;
            // 
            // NewPurchase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1200, 920);
             this.Controls.Add(this.panelMainContainer);
             this.Controls.Add(this.panelButtons);
            this.Name = "NewPurchase";
            this.Text = "PurchaseForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelMainContainer.ResumeLayout(false);
            this.panelSummary.ResumeLayout(false);
            this.panelSummary.PerformLayout();
            this.panelItemDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseItems)).EndInit();
            this.panelVendorInfo.ResumeLayout(false);
            this.panelVendorInfo.PerformLayout();
             this.panelButtons.ResumeLayout(false);
             this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMainContainer;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClearForm;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.Button btnSavePurchase;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Panel panelSummary;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.TextBox txtPaid;
        private System.Windows.Forms.Label lblPaid;
        private System.Windows.Forms.TextBox txtNetAmount;
        private System.Windows.Forms.Label lblNetAmount;
        private System.Windows.Forms.TextBox txtTaxPercent;
        private System.Windows.Forms.Label lblTaxPercent;
        private System.Windows.Forms.TextBox txtDiscountPercent;
        private System.Windows.Forms.Label lblDiscountPercent;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblSummaryTotals;
        private System.Windows.Forms.Panel panelItemDetails;
        private System.Windows.Forms.DataGridView dgvPurchaseItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSrNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPurchasePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSalePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.Panel panelVendorInfo;
        private System.Windows.Forms.ComboBox cmbPaymentTerms;
        private System.Windows.Forms.Label lblPaymentTerms;
        private System.Windows.Forms.TextBox txtVendorCode;
        private System.Windows.Forms.Label lblVendorCode;
        private System.Windows.Forms.ComboBox cmbVendorName;
        private System.Windows.Forms.Label lblVendorName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.DateTimePicker dtpInvoiceDate;
        private System.Windows.Forms.Label lblInvoiceDate;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Label lblInvoiceNo;
        private System.Windows.Forms.Label lblVendorInvoiceInfo;
        private System.Windows.Forms.TextBox txtExistingStock;
        private System.Windows.Forms.Label lblExistingStock;
        private System.Windows.Forms.ComboBox cmbProductName;
        private System.Windows.Forms.Label lblProductName;
    }
}