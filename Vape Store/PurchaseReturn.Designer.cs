namespace Vape_Store
{
    partial class PurchaseReturnForm
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
            this.btnsave = new System.Windows.Forms.Button();
            this.lblOriginalInvoiceTitle = new System.Windows.Forms.Label();
            this.lblOriginalInvoiceNumber = new System.Windows.Forms.Label();
            this.txtOriginalInvoiceNumber = new System.Windows.Forms.TextBox();
            this.lblOriginalInvoiceDate = new System.Windows.Forms.Label();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrignalQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReturnQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtsubTotal = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTaxPercent = new System.Windows.Forms.TextBox();
            this.txtTax = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbTax = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtdescription = new System.Windows.Forms.TextBox();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbreturnreason = new System.Windows.Forms.ComboBox();
            this.Select = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtOriginalInvoiceDate = new System.Windows.Forms.TextBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.pnlOriginalInvoiceDetails = new System.Windows.Forms.Panel();
            this.lblOriginalInvoiceTotal = new System.Windows.Forms.Label();
            this.txtOriginalInvoiceTotal = new System.Windows.Forms.TextBox();
            this.pnlCustomerInfo = new System.Windows.Forms.Panel();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerPhone = new System.Windows.Forms.Label();
            this.txtCustomerPhone = new System.Windows.Forms.TextBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlReturnInfo = new System.Windows.Forms.Panel();
            this.lblReturnNumber = new System.Windows.Forms.Label();
            this.txtReturnNumber = new System.Windows.Forms.TextBox();
            this.lblReturnDate = new System.Windows.Forms.Label();
            this.dtpReturnDate = new System.Windows.Forms.DateTimePicker();
            this.pnlInvoiceSelection = new System.Windows.Forms.Panel();
            this.btnLoadInvoice = new System.Windows.Forms.Button();
            this.lblInvoiceNumber = new System.Windows.Forms.Label();
            this.cmbInvoiceNumber = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.pnlOriginalInvoiceDetails.SuspendLayout();
            this.pnlCustomerInfo.SuspendLayout();
            this.pnlReturnInfo.SuspendLayout();
            this.pnlInvoiceSelection.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnsave
            // 
            this.btnsave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnsave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnsave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsave.ForeColor = System.Drawing.Color.White;
            this.btnsave.Location = new System.Drawing.Point(1415, 719);
            this.btnsave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(193, 62);
            this.btnsave.TabIndex = 49;
            this.btnsave.Text = "Sales Return";
            this.btnsave.UseVisualStyleBackColor = false;
            // 
            // lblOriginalInvoiceTitle
            // 
            this.lblOriginalInvoiceTitle.AutoSize = true;
            this.lblOriginalInvoiceTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.lblOriginalInvoiceTitle.Location = new System.Drawing.Point(10, 10);
            this.lblOriginalInvoiceTitle.Name = "lblOriginalInvoiceTitle";
            this.lblOriginalInvoiceTitle.Size = new System.Drawing.Size(234, 28);
            this.lblOriginalInvoiceTitle.TabIndex = 0;
            this.lblOriginalInvoiceTitle.Text = "Original Invoice Details";
            // 
            // lblOriginalInvoiceNumber
            // 
            this.lblOriginalInvoiceNumber.AutoSize = true;
            this.lblOriginalInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceNumber.Location = new System.Drawing.Point(10, 59);
            this.lblOriginalInvoiceNumber.Name = "lblOriginalInvoiceNumber";
            this.lblOriginalInvoiceNumber.Size = new System.Drawing.Size(95, 25);
            this.lblOriginalInvoiceNumber.TabIndex = 1;
            this.lblOriginalInvoiceNumber.Text = "Invoice #:";
            // 
            // txtOriginalInvoiceNumber
            // 
            this.txtOriginalInvoiceNumber.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceNumber.Location = new System.Drawing.Point(114, 58);
            this.txtOriginalInvoiceNumber.Name = "txtOriginalInvoiceNumber";
            this.txtOriginalInvoiceNumber.ReadOnly = true;
            this.txtOriginalInvoiceNumber.Size = new System.Drawing.Size(120, 26);
            this.txtOriginalInvoiceNumber.TabIndex = 2;
            // 
            // lblOriginalInvoiceDate
            // 
            this.lblOriginalInvoiceDate.AutoSize = true;
            this.lblOriginalInvoiceDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceDate.Location = new System.Drawing.Point(240, 59);
            this.lblOriginalInvoiceDate.Name = "lblOriginalInvoiceDate";
            this.lblOriginalInvoiceDate.Size = new System.Drawing.Size(57, 25);
            this.lblOriginalInvoiceDate.TabIndex = 3;
            this.lblOriginalInvoiceDate.Text = "Date:";
            // 
            // ItemCode
            // 
            this.ItemCode.HeaderText = "Item Code";
            this.ItemCode.MinimumWidth = 8;
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.Width = 150;
            // 
            // ItemName
            // 
            this.ItemName.HeaderText = "Item Name";
            this.ItemName.MinimumWidth = 8;
            this.ItemName.Name = "ItemName";
            this.ItemName.Width = 150;
            // 
            // OrignalQty
            // 
            this.OrignalQty.HeaderText = "Orignal Qty";
            this.OrignalQty.MinimumWidth = 8;
            this.OrignalQty.Name = "OrignalQty";
            this.OrignalQty.Width = 150;
            // 
            // ReturnQty
            // 
            this.ReturnQty.HeaderText = "Return Qty";
            this.ReturnQty.MinimumWidth = 8;
            this.ReturnQty.Name = "ReturnQty";
            this.ReturnQty.Width = 150;
            // 
            // Price
            // 
            this.Price.HeaderText = "Price";
            this.Price.MinimumWidth = 8;
            this.Price.Name = "Price";
            this.Price.Width = 150;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtsubTotal);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.txtTaxPercent);
            this.panel2.Controls.Add(this.txtTax);
            this.panel2.Controls.Add(this.txtTotal);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.cmbTax);
            this.panel2.Location = new System.Drawing.Point(1272, 379);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(343, 262);
            this.panel2.TabIndex = 48;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 31);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 28);
            this.label3.TabIndex = 26;
            this.label3.Text = "Sub Total:";
            // 
            // txtsubTotal
            // 
            this.txtsubTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsubTotal.Location = new System.Drawing.Point(138, 27);
            this.txtsubTotal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtsubTotal.Name = "txtsubTotal";
            this.txtsubTotal.Size = new System.Drawing.Size(192, 34);
            this.txtsubTotal.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(11, 78);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 28);
            this.label10.TabIndex = 28;
            this.label10.Text = "Discount:";
            // 
            // txtTaxPercent
            // 
            this.txtTaxPercent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaxPercent.Location = new System.Drawing.Point(138, 77);
            this.txtTaxPercent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTaxPercent.Name = "txtTaxPercent";
            this.txtTaxPercent.Size = new System.Drawing.Size(97, 34);
            this.txtTaxPercent.TabIndex = 27;
            // 
            // txtTax
            // 
            this.txtTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTax.Location = new System.Drawing.Point(243, 77);
            this.txtTax.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(87, 34);
            this.txtTax.TabIndex = 29;
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(138, 189);
            this.txtTotal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(192, 34);
            this.txtTotal.TabIndex = 32;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(11, 134);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 28);
            this.label13.TabIndex = 30;
            this.label13.Text = "Tax:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(11, 185);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(64, 28);
            this.label14.TabIndex = 33;
            this.label14.Text = "Total:";
            // 
            // cmbTax
            // 
            this.cmbTax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTax.FormattingEnabled = true;
            this.cmbTax.Location = new System.Drawing.Point(138, 134);
            this.cmbTax.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbTax.Name = "cmbTax";
            this.cmbTax.Size = new System.Drawing.Size(197, 36);
            this.cmbTax.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 719);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 47;
            this.label2.Text = "Description";
            // 
            // txtdescription
            // 
            this.txtdescription.BackColor = System.Drawing.Color.White;
            this.txtdescription.Location = new System.Drawing.Point(128, 719);
            this.txtdescription.Multiline = true;
            this.txtdescription.Name = "txtdescription";
            this.txtdescription.ReadOnly = true;
            this.txtdescription.Size = new System.Drawing.Size(362, 58);
            this.txtdescription.TabIndex = 39;
            // 
            // Total
            // 
            this.Total.HeaderText = "Total";
            this.Total.MinimumWidth = 8;
            this.Total.Name = "Total";
            this.Total.Width = 150;
            // 
            // cmbreturnreason
            // 
            this.cmbreturnreason.BackColor = System.Drawing.Color.White;
            this.cmbreturnreason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbreturnreason.FormattingEnabled = true;
            this.cmbreturnreason.Location = new System.Drawing.Point(221, 677);
            this.cmbreturnreason.Name = "cmbreturnreason";
            this.cmbreturnreason.Size = new System.Drawing.Size(269, 28);
            this.cmbreturnreason.TabIndex = 46;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.MinimumWidth = 8;
            this.Select.Name = "Select";
            this.Select.Width = 150;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 680);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 20);
            this.label1.TabIndex = 45;
            this.label1.Text = "Purchase Return Reason";
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
            this.Select,
            this.ItemCode,
            this.ItemName,
            this.OrignalQty,
            this.ReturnQty,
            this.Price,
            this.Total});
            this.dataGridView1.Location = new System.Drawing.Point(0, 379);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dataGridView1.Size = new System.Drawing.Size(1255, 262);
            this.dataGridView1.TabIndex = 44;
            // 
            // txtOriginalInvoiceDate
            // 
            this.txtOriginalInvoiceDate.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceDate.Location = new System.Drawing.Point(300, 59);
            this.txtOriginalInvoiceDate.Name = "txtOriginalInvoiceDate";
            this.txtOriginalInvoiceDate.ReadOnly = true;
            this.txtOriginalInvoiceDate.Size = new System.Drawing.Size(114, 26);
            this.txtOriginalInvoiceDate.TabIndex = 4;
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.ForeColor = System.Drawing.Color.White;
            this.CancelBtn.Location = new System.Drawing.Point(1184, 719);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(188, 62);
            this.CancelBtn.TabIndex = 50;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = false;
            // 
            // pnlOriginalInvoiceDetails
            // 
            this.pnlOriginalInvoiceDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOriginalInvoiceDetails.Controls.Add(this.lblOriginalInvoiceTitle);
            this.pnlOriginalInvoiceDetails.Controls.Add(this.lblOriginalInvoiceNumber);
            this.pnlOriginalInvoiceDetails.Controls.Add(this.txtOriginalInvoiceNumber);
            this.pnlOriginalInvoiceDetails.Controls.Add(this.lblOriginalInvoiceDate);
            this.pnlOriginalInvoiceDetails.Controls.Add(this.txtOriginalInvoiceDate);
            this.pnlOriginalInvoiceDetails.Controls.Add(this.lblOriginalInvoiceTotal);
            this.pnlOriginalInvoiceDetails.Controls.Add(this.txtOriginalInvoiceTotal);
            this.pnlOriginalInvoiceDetails.Location = new System.Drawing.Point(1003, 258);
            this.pnlOriginalInvoiceDetails.Name = "pnlOriginalInvoiceDetails";
            this.pnlOriginalInvoiceDetails.Size = new System.Drawing.Size(605, 115);
            this.pnlOriginalInvoiceDetails.TabIndex = 43;
            // 
            // lblOriginalInvoiceTotal
            // 
            this.lblOriginalInvoiceTotal.AutoSize = true;
            this.lblOriginalInvoiceTotal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceTotal.Location = new System.Drawing.Point(420, 59);
            this.lblOriginalInvoiceTotal.Name = "lblOriginalInvoiceTotal";
            this.lblOriginalInvoiceTotal.Size = new System.Drawing.Size(59, 25);
            this.lblOriginalInvoiceTotal.TabIndex = 5;
            this.lblOriginalInvoiceTotal.Text = "Total:";
            // 
            // txtOriginalInvoiceTotal
            // 
            this.txtOriginalInvoiceTotal.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceTotal.Location = new System.Drawing.Point(480, 59);
            this.txtOriginalInvoiceTotal.Name = "txtOriginalInvoiceTotal";
            this.txtOriginalInvoiceTotal.ReadOnly = true;
            this.txtOriginalInvoiceTotal.Size = new System.Drawing.Size(100, 26);
            this.txtOriginalInvoiceTotal.TabIndex = 6;
            // 
            // pnlCustomerInfo
            // 
            this.pnlCustomerInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCustomerInfo.Controls.Add(this.lblCustomerAddress);
            this.pnlCustomerInfo.Controls.Add(this.txtCustomerAddress);
            this.pnlCustomerInfo.Controls.Add(this.lblCustomerPhone);
            this.pnlCustomerInfo.Controls.Add(this.txtCustomerPhone);
            this.pnlCustomerInfo.Controls.Add(this.lblCustomerName);
            this.pnlCustomerInfo.Controls.Add(this.txtCustomerName);
            this.pnlCustomerInfo.Controls.Add(this.lblCustomer);
            this.pnlCustomerInfo.Controls.Add(this.cmbCustomer);
            this.pnlCustomerInfo.Location = new System.Drawing.Point(1003, 122);
            this.pnlCustomerInfo.Name = "pnlCustomerInfo";
            this.pnlCustomerInfo.Size = new System.Drawing.Size(586, 116);
            this.pnlCustomerInfo.TabIndex = 42;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerAddress.Location = new System.Drawing.Point(238, 54);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(80, 20);
            this.lblCustomerAddress.TabIndex = 6;
            this.lblCustomerAddress.Text = "Address:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.Color.White;
            this.txtCustomerAddress.Location = new System.Drawing.Point(333, 50);
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(228, 26);
            this.txtCustomerAddress.TabIndex = 7;
            // 
            // lblCustomerPhone
            // 
            this.lblCustomerPhone.AutoSize = true;
            this.lblCustomerPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerPhone.Location = new System.Drawing.Point(16, 56);
            this.lblCustomerPhone.Name = "lblCustomerPhone";
            this.lblCustomerPhone.Size = new System.Drawing.Size(65, 20);
            this.lblCustomerPhone.TabIndex = 4;
            this.lblCustomerPhone.Text = "Phone:";
            // 
            // txtCustomerPhone
            // 
            this.txtCustomerPhone.BackColor = System.Drawing.Color.White;
            this.txtCustomerPhone.Location = new System.Drawing.Point(87, 52);
            this.txtCustomerPhone.Name = "txtCustomerPhone";
            this.txtCustomerPhone.ReadOnly = true;
            this.txtCustomerPhone.Size = new System.Drawing.Size(120, 26);
            this.txtCustomerPhone.TabIndex = 5;
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerName.Location = new System.Drawing.Point(308, 15);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(60, 20);
            this.lblCustomerName.TabIndex = 2;
            this.lblCustomerName.Text = "Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.Color.White;
            this.txtCustomerName.Location = new System.Drawing.Point(385, 15);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(176, 26);
            this.txtCustomerName.TabIndex = 3;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomer.Location = new System.Drawing.Point(11, 14);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(80, 20);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Supplier:";
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.BackColor = System.Drawing.Color.White;
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(102, 11);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(200, 28);
            this.cmbCustomer.TabIndex = 1;
            this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.CmbCustomer_SelectedIndexChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(30, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(356, 45);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Purchase Return - POS";
            // 
            // pnlReturnInfo
            // 
            this.pnlReturnInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReturnInfo.Controls.Add(this.lblReturnNumber);
            this.pnlReturnInfo.Controls.Add(this.txtReturnNumber);
            this.pnlReturnInfo.Controls.Add(this.lblReturnDate);
            this.pnlReturnInfo.Controls.Add(this.dtpReturnDate);
            this.pnlReturnInfo.Location = new System.Drawing.Point(12, 122);
            this.pnlReturnInfo.Name = "pnlReturnInfo";
            this.pnlReturnInfo.Size = new System.Drawing.Size(954, 116);
            this.pnlReturnInfo.TabIndex = 40;
            // 
            // lblReturnNumber
            // 
            this.lblReturnNumber.AutoSize = true;
            this.lblReturnNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnNumber.Location = new System.Drawing.Point(3, 14);
            this.lblReturnNumber.Name = "lblReturnNumber";
            this.lblReturnNumber.Size = new System.Drawing.Size(163, 25);
            this.lblReturnNumber.TabIndex = 0;
            this.lblReturnNumber.Text = "Return Number:";
            // 
            // txtReturnNumber
            // 
            this.txtReturnNumber.BackColor = System.Drawing.Color.White;
            this.txtReturnNumber.Location = new System.Drawing.Point(177, 15);
            this.txtReturnNumber.Name = "txtReturnNumber";
            this.txtReturnNumber.ReadOnly = true;
            this.txtReturnNumber.Size = new System.Drawing.Size(195, 26);
            this.txtReturnNumber.TabIndex = 1;
            // 
            // lblReturnDate
            // 
            this.lblReturnDate.AutoSize = true;
            this.lblReturnDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnDate.Location = new System.Drawing.Point(11, 57);
            this.lblReturnDate.Name = "lblReturnDate";
            this.lblReturnDate.Size = new System.Drawing.Size(133, 25);
            this.lblReturnDate.TabIndex = 2;
            this.lblReturnDate.Text = "Return Date:";
            // 
            // dtpReturnDate
            // 
            this.dtpReturnDate.CustomFormat = "dddd, MMMM dd, yyyy";
            this.dtpReturnDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReturnDate.Location = new System.Drawing.Point(172, 56);
            this.dtpReturnDate.Name = "dtpReturnDate";
            this.dtpReturnDate.Size = new System.Drawing.Size(200, 26);
            this.dtpReturnDate.TabIndex = 3;
            // 
            // pnlInvoiceSelection
            // 
            this.pnlInvoiceSelection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlInvoiceSelection.Controls.Add(this.btnLoadInvoice);
            this.pnlInvoiceSelection.Controls.Add(this.lblInvoiceNumber);
            this.pnlInvoiceSelection.Controls.Add(this.cmbInvoiceNumber);
            this.pnlInvoiceSelection.Location = new System.Drawing.Point(12, 258);
            this.pnlInvoiceSelection.Name = "pnlInvoiceSelection";
            this.pnlInvoiceSelection.Size = new System.Drawing.Size(954, 81);
            this.pnlInvoiceSelection.TabIndex = 41;
            // 
            // btnLoadInvoice
            // 
            this.btnLoadInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLoadInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadInvoice.ForeColor = System.Drawing.Color.White;
            this.btnLoadInvoice.Location = new System.Drawing.Point(522, 20);
            this.btnLoadInvoice.Name = "btnLoadInvoice";
            this.btnLoadInvoice.Size = new System.Drawing.Size(112, 30);
            this.btnLoadInvoice.TabIndex = 2;
            this.btnLoadInvoice.Text = "Load";
            this.btnLoadInvoice.UseVisualStyleBackColor = false;
            // 
            // lblInvoiceNumber
            // 
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceNumber.Location = new System.Drawing.Point(10, 20);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(95, 25);
            this.lblInvoiceNumber.TabIndex = 1;
            this.lblInvoiceNumber.Text = "Invoice #:";
            // 
            // cmbInvoiceNumber
            // 
            this.cmbInvoiceNumber.BackColor = System.Drawing.Color.White;
            this.cmbInvoiceNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInvoiceNumber.FormattingEnabled = true;
            this.cmbInvoiceNumber.Location = new System.Drawing.Point(140, 20);
            this.cmbInvoiceNumber.Name = "cmbInvoiceNumber";
            this.cmbInvoiceNumber.Size = new System.Drawing.Size(350, 28);
            this.cmbInvoiceNumber.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SeaGreen;
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1665, 93);
            this.panel1.TabIndex = 38;
            // 
            // PurchaseReturnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1665, 854);
            this.Controls.Add(this.btnsave);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtdescription);
            this.Controls.Add(this.cmbreturnreason);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.pnlOriginalInvoiceDetails);
            this.Controls.Add(this.pnlCustomerInfo);
            this.Controls.Add(this.pnlReturnInfo);
            this.Controls.Add(this.pnlInvoiceSelection);
            this.Controls.Add(this.panel1);
            this.Name = "PurchaseReturnForm";
            this.Text = "Attock Mobiles Rwp - Purchase Return";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.pnlOriginalInvoiceDetails.ResumeLayout(false);
            this.pnlOriginalInvoiceDetails.PerformLayout();
            this.pnlCustomerInfo.ResumeLayout(false);
            this.pnlCustomerInfo.PerformLayout();
            this.pnlReturnInfo.ResumeLayout(false);
            this.pnlReturnInfo.PerformLayout();
            this.pnlInvoiceSelection.ResumeLayout(false);
            this.pnlInvoiceSelection.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnsave;
        private System.Windows.Forms.Label lblOriginalInvoiceTitle;
        private System.Windows.Forms.Label lblOriginalInvoiceNumber;
        private System.Windows.Forms.TextBox txtOriginalInvoiceNumber;
        private System.Windows.Forms.Label lblOriginalInvoiceDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrignalQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReturnQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtsubTotal;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtTaxPercent;
        private System.Windows.Forms.TextBox txtTax;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbTax;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtdescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.ComboBox cmbreturnreason;
        private System.Windows.Forms.DataGridViewTextBoxColumn Select;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtOriginalInvoiceDate;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Panel pnlOriginalInvoiceDetails;
        private System.Windows.Forms.Label lblOriginalInvoiceTotal;
        private System.Windows.Forms.TextBox txtOriginalInvoiceTotal;
        private System.Windows.Forms.Panel pnlCustomerInfo;
        private System.Windows.Forms.Label lblCustomerAddress;
        private System.Windows.Forms.TextBox txtCustomerAddress;
        private System.Windows.Forms.Label lblCustomerPhone;
        private System.Windows.Forms.TextBox txtCustomerPhone;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlReturnInfo;
        private System.Windows.Forms.Label lblReturnNumber;
        private System.Windows.Forms.TextBox txtReturnNumber;
        private System.Windows.Forms.Label lblReturnDate;
        private System.Windows.Forms.DateTimePicker dtpReturnDate;
        private System.Windows.Forms.Panel pnlInvoiceSelection;
        private System.Windows.Forms.Button btnLoadInvoice;
        private System.Windows.Forms.Label lblInvoiceNumber;
        private System.Windows.Forms.ComboBox cmbInvoiceNumber;
        private System.Windows.Forms.Panel panel1;
    }
}