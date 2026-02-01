namespace Vape_Store
{
    partial class SalesReturnForm
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
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.pnlCustomerInfo = new System.Windows.Forms.Panel();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerPhone = new System.Windows.Forms.Label();
            this.txtCustomerPhone = new System.Windows.Forms.TextBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.pnlOriginalInvoiceDetails = new System.Windows.Forms.Panel();
            this.lblOriginalInvoiceTitle = new System.Windows.Forms.Label();
            this.lblOriginalInvoiceNumber = new System.Windows.Forms.Label();
            this.txtOriginalInvoiceNumber = new System.Windows.Forms.TextBox();
            this.lblOriginalInvoiceDate = new System.Windows.Forms.Label();
            this.txtOriginalInvoiceDate = new System.Windows.Forms.TextBox();
            this.lblOriginalInvoiceTotal = new System.Windows.Forms.Label();
            this.txtOriginalInvoiceTotal = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrignalQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReturnQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbreturnreason = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtdescription = new System.Windows.Forms.TextBox();
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
            this.CancelBtn = new System.Windows.Forms.Button();
            this.NewItemBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnlReturnInfo.SuspendLayout();
            this.pnlInvoiceSelection.SuspendLayout();
            this.pnlCustomerInfo.SuspendLayout();
            this.pnlOriginalInvoiceDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SeaGreen;
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1327, 60);
            this.panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(204, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Sales Return - POS";
            // 
            // pnlReturnInfo
            // 
            this.pnlReturnInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReturnInfo.Controls.Add(this.lblReturnNumber);
            this.pnlReturnInfo.Controls.Add(this.txtReturnNumber);
            this.pnlReturnInfo.Controls.Add(this.lblReturnDate);
            this.pnlReturnInfo.Controls.Add(this.dtpReturnDate);
            this.pnlReturnInfo.Location = new System.Drawing.Point(8, 77);
            this.pnlReturnInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlReturnInfo.Name = "pnlReturnInfo";
            this.pnlReturnInfo.Size = new System.Drawing.Size(637, 76);
            this.pnlReturnInfo.TabIndex = 2;
            // 
            // lblReturnNumber
            // 
            this.lblReturnNumber.AutoSize = true;
            this.lblReturnNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnNumber.Location = new System.Drawing.Point(2, 9);
            this.lblReturnNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReturnNumber.Name = "lblReturnNumber";
            this.lblReturnNumber.Size = new System.Drawing.Size(123, 17);
            this.lblReturnNumber.TabIndex = 0;
            this.lblReturnNumber.Text = "Return Number:";
            // 
            // txtReturnNumber
            // 
            this.txtReturnNumber.BackColor = System.Drawing.Color.White;
            this.txtReturnNumber.Location = new System.Drawing.Point(118, 10);
            this.txtReturnNumber.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtReturnNumber.Name = "txtReturnNumber";
            this.txtReturnNumber.ReadOnly = true;
            this.txtReturnNumber.Size = new System.Drawing.Size(131, 20);
            this.txtReturnNumber.TabIndex = 1;
            // 
            // lblReturnDate
            // 
            this.lblReturnDate.AutoSize = true;
            this.lblReturnDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnDate.Location = new System.Drawing.Point(7, 37);
            this.lblReturnDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReturnDate.Name = "lblReturnDate";
            this.lblReturnDate.Size = new System.Drawing.Size(101, 17);
            this.lblReturnDate.TabIndex = 2;
            this.lblReturnDate.Text = "Return Date:";
            // 
            // dtpReturnDate
            // 
            this.dtpReturnDate.CustomFormat = "dddd, MMMM dd, yyyy";
            this.dtpReturnDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReturnDate.Location = new System.Drawing.Point(115, 36);
            this.dtpReturnDate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpReturnDate.Name = "dtpReturnDate";
            this.dtpReturnDate.Size = new System.Drawing.Size(135, 20);
            this.dtpReturnDate.TabIndex = 3;
            // 
            // pnlInvoiceSelection
            // 
            this.pnlInvoiceSelection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlInvoiceSelection.Controls.Add(this.btnLoadInvoice);
            this.pnlInvoiceSelection.Controls.Add(this.lblInvoiceNumber);
            this.pnlInvoiceSelection.Controls.Add(this.cmbInvoiceNumber);
            this.pnlInvoiceSelection.Location = new System.Drawing.Point(8, 166);
            this.pnlInvoiceSelection.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlInvoiceSelection.Name = "pnlInvoiceSelection";
            this.pnlInvoiceSelection.Size = new System.Drawing.Size(637, 53);
            this.pnlInvoiceSelection.TabIndex = 3;
            // 
            // btnLoadInvoice
            // 
            this.btnLoadInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLoadInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadInvoice.ForeColor = System.Drawing.Color.White;
            this.btnLoadInvoice.Location = new System.Drawing.Point(348, 13);
            this.btnLoadInvoice.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLoadInvoice.Name = "btnLoadInvoice";
            this.btnLoadInvoice.Size = new System.Drawing.Size(75, 20);
            this.btnLoadInvoice.TabIndex = 2;
            this.btnLoadInvoice.Text = "Load";
            this.btnLoadInvoice.UseVisualStyleBackColor = false;
            // 
            // lblInvoiceNumber
            // 
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceNumber.Location = new System.Drawing.Point(7, 13);
            this.lblInvoiceNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(61, 15);
            this.lblInvoiceNumber.TabIndex = 1;
            this.lblInvoiceNumber.Text = "Invoice #:";
            // 
            // cmbInvoiceNumber
            // 
            this.cmbInvoiceNumber.BackColor = System.Drawing.Color.White;
            this.cmbInvoiceNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInvoiceNumber.FormattingEnabled = true;
            this.cmbInvoiceNumber.Location = new System.Drawing.Point(93, 13);
            this.cmbInvoiceNumber.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbInvoiceNumber.Name = "cmbInvoiceNumber";
            this.cmbInvoiceNumber.Size = new System.Drawing.Size(235, 21);
            this.cmbInvoiceNumber.TabIndex = 0;
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
            this.pnlCustomerInfo.Location = new System.Drawing.Point(669, 77);
            this.pnlCustomerInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlCustomerInfo.Name = "pnlCustomerInfo";
            this.pnlCustomerInfo.Size = new System.Drawing.Size(391, 76);
            this.pnlCustomerInfo.TabIndex = 5;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerAddress.Location = new System.Drawing.Point(159, 35);
            this.lblCustomerAddress.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(56, 13);
            this.lblCustomerAddress.TabIndex = 6;
            this.lblCustomerAddress.Text = "Address:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.Color.White;
            this.txtCustomerAddress.Location = new System.Drawing.Point(222, 32);
            this.txtCustomerAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(153, 20);
            this.txtCustomerAddress.TabIndex = 7;
            // 
            // lblCustomerPhone
            // 
            this.lblCustomerPhone.AutoSize = true;
            this.lblCustomerPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerPhone.Location = new System.Drawing.Point(11, 36);
            this.lblCustomerPhone.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomerPhone.Name = "lblCustomerPhone";
            this.lblCustomerPhone.Size = new System.Drawing.Size(47, 13);
            this.lblCustomerPhone.TabIndex = 4;
            this.lblCustomerPhone.Text = "Phone:";
            // 
            // txtCustomerPhone
            // 
            this.txtCustomerPhone.BackColor = System.Drawing.Color.White;
            this.txtCustomerPhone.Location = new System.Drawing.Point(58, 34);
            this.txtCustomerPhone.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCustomerPhone.Name = "txtCustomerPhone";
            this.txtCustomerPhone.ReadOnly = true;
            this.txtCustomerPhone.Size = new System.Drawing.Size(81, 20);
            this.txtCustomerPhone.TabIndex = 5;
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerName.Location = new System.Drawing.Point(205, 10);
            this.lblCustomerName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(43, 13);
            this.lblCustomerName.TabIndex = 2;
            this.lblCustomerName.Text = "Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.Color.White;
            this.txtCustomerName.Location = new System.Drawing.Point(257, 10);
            this.txtCustomerName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(119, 20);
            this.txtCustomerName.TabIndex = 3;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomer.Location = new System.Drawing.Point(2, 8);
            this.lblCustomer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(63, 13);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Customer:";
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.BackColor = System.Drawing.Color.White;
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(68, 7);
            this.cmbCustomer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(135, 21);
            this.cmbCustomer.TabIndex = 1;
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
            this.pnlOriginalInvoiceDetails.Location = new System.Drawing.Point(669, 166);
            this.pnlOriginalInvoiceDetails.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlOriginalInvoiceDetails.Name = "pnlOriginalInvoiceDetails";
            this.pnlOriginalInvoiceDetails.Size = new System.Drawing.Size(404, 75);
            this.pnlOriginalInvoiceDetails.TabIndex = 6;
            // 
            // lblOriginalInvoiceTitle
            // 
            this.lblOriginalInvoiceTitle.AutoSize = true;
            this.lblOriginalInvoiceTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.lblOriginalInvoiceTitle.Location = new System.Drawing.Point(7, 6);
            this.lblOriginalInvoiceTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOriginalInvoiceTitle.Name = "lblOriginalInvoiceTitle";
            this.lblOriginalInvoiceTitle.Size = new System.Drawing.Size(164, 19);
            this.lblOriginalInvoiceTitle.TabIndex = 0;
            this.lblOriginalInvoiceTitle.Text = "Original Invoice Details";
            // 
            // lblOriginalInvoiceNumber
            // 
            this.lblOriginalInvoiceNumber.AutoSize = true;
            this.lblOriginalInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceNumber.Location = new System.Drawing.Point(7, 38);
            this.lblOriginalInvoiceNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOriginalInvoiceNumber.Name = "lblOriginalInvoiceNumber";
            this.lblOriginalInvoiceNumber.Size = new System.Drawing.Size(61, 15);
            this.lblOriginalInvoiceNumber.TabIndex = 1;
            this.lblOriginalInvoiceNumber.Text = "Invoice #:";
            // 
            // txtOriginalInvoiceNumber
            // 
            this.txtOriginalInvoiceNumber.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceNumber.Location = new System.Drawing.Point(76, 38);
            this.txtOriginalInvoiceNumber.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOriginalInvoiceNumber.Name = "txtOriginalInvoiceNumber";
            this.txtOriginalInvoiceNumber.ReadOnly = true;
            this.txtOriginalInvoiceNumber.Size = new System.Drawing.Size(81, 20);
            this.txtOriginalInvoiceNumber.TabIndex = 2;
            // 
            // lblOriginalInvoiceDate
            // 
            this.lblOriginalInvoiceDate.AutoSize = true;
            this.lblOriginalInvoiceDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceDate.Location = new System.Drawing.Point(160, 38);
            this.lblOriginalInvoiceDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOriginalInvoiceDate.Name = "lblOriginalInvoiceDate";
            this.lblOriginalInvoiceDate.Size = new System.Drawing.Size(37, 15);
            this.lblOriginalInvoiceDate.TabIndex = 3;
            this.lblOriginalInvoiceDate.Text = "Date:";
            // 
            // txtOriginalInvoiceDate
            // 
            this.txtOriginalInvoiceDate.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceDate.Location = new System.Drawing.Point(200, 38);
            this.txtOriginalInvoiceDate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOriginalInvoiceDate.Name = "txtOriginalInvoiceDate";
            this.txtOriginalInvoiceDate.ReadOnly = true;
            this.txtOriginalInvoiceDate.Size = new System.Drawing.Size(77, 20);
            this.txtOriginalInvoiceDate.TabIndex = 4;
            // 
            // lblOriginalInvoiceTotal
            // 
            this.lblOriginalInvoiceTotal.AutoSize = true;
            this.lblOriginalInvoiceTotal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceTotal.Location = new System.Drawing.Point(280, 38);
            this.lblOriginalInvoiceTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOriginalInvoiceTotal.Name = "lblOriginalInvoiceTotal";
            this.lblOriginalInvoiceTotal.Size = new System.Drawing.Size(37, 15);
            this.lblOriginalInvoiceTotal.TabIndex = 5;
            this.lblOriginalInvoiceTotal.Text = "Total:";
            // 
            // txtOriginalInvoiceTotal
            // 
            this.txtOriginalInvoiceTotal.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceTotal.Location = new System.Drawing.Point(320, 38);
            this.txtOriginalInvoiceTotal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOriginalInvoiceTotal.Name = "txtOriginalInvoiceTotal";
            this.txtOriginalInvoiceTotal.ReadOnly = true;
            this.txtOriginalInvoiceTotal.Size = new System.Drawing.Size(68, 20);
            this.txtOriginalInvoiceTotal.TabIndex = 6;
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 244);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(1020, 432);
            this.dataGridView1.TabIndex = 7;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.MinimumWidth = 8;
            this.Select.Name = "Select";
            // 
            // ItemCode
            // 
            this.ItemCode.HeaderText = "Item Code";
            this.ItemCode.MinimumWidth = 8;
            this.ItemCode.Name = "ItemCode";
            // 
            // ItemName
            // 
            this.ItemName.HeaderText = "Item Name";
            this.ItemName.MinimumWidth = 8;
            this.ItemName.Name = "ItemName";
            // 
            // OrignalQty
            // 
            this.OrignalQty.HeaderText = "Orignal Qty";
            this.OrignalQty.MinimumWidth = 8;
            this.OrignalQty.Name = "OrignalQty";
            // 
            // ReturnQty
            // 
            this.ReturnQty.HeaderText = "Return Qty";
            this.ReturnQty.MinimumWidth = 8;
            this.ReturnQty.Name = "ReturnQty";
            // 
            // Price
            // 
            this.Price.HeaderText = "Price";
            this.Price.MinimumWidth = 8;
            this.Price.Name = "Price";
            // 
            // Total
            // 
            this.Total.HeaderText = "Total";
            this.Total.MinimumWidth = 8;
            this.Total.Name = "Total";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 684);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Sales Return Reason";
            // 
            // cmbreturnreason
            // 
            this.cmbreturnreason.BackColor = System.Drawing.Color.White;
            this.cmbreturnreason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbreturnreason.FormattingEnabled = true;
            this.cmbreturnreason.Location = new System.Drawing.Point(164, 679);
            this.cmbreturnreason.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbreturnreason.Name = "cmbreturnreason";
            this.cmbreturnreason.Size = new System.Drawing.Size(181, 21);
            this.cmbreturnreason.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 706);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Description";
            // 
            // txtdescription
            // 
            this.txtdescription.BackColor = System.Drawing.Color.White;
            this.txtdescription.Location = new System.Drawing.Point(102, 706);
            this.txtdescription.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtdescription.Multiline = true;
            this.txtdescription.Name = "txtdescription";
            this.txtdescription.ReadOnly = true;
            this.txtdescription.Size = new System.Drawing.Size(243, 39);
            this.txtdescription.TabIndex = 1;
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
            this.panel2.Location = new System.Drawing.Point(1024, 245);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(229, 170);
            this.panel2.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 19);
            this.label3.TabIndex = 26;
            this.label3.Text = "Sub Total:";
            // 
            // txtsubTotal
            // 
            this.txtsubTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsubTotal.Location = new System.Drawing.Point(92, 18);
            this.txtsubTotal.Name = "txtsubTotal";
            this.txtsubTotal.Size = new System.Drawing.Size(129, 25);
            this.txtsubTotal.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(7, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 19);
            this.label10.TabIndex = 28;
            this.label10.Text = "Discount:";
            // 
            // txtTaxPercent
            // 
            this.txtTaxPercent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaxPercent.Location = new System.Drawing.Point(92, 50);
            this.txtTaxPercent.Name = "txtTaxPercent";
            this.txtTaxPercent.Size = new System.Drawing.Size(66, 25);
            this.txtTaxPercent.TabIndex = 27;
            // 
            // txtTax
            // 
            this.txtTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTax.Location = new System.Drawing.Point(162, 50);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(59, 25);
            this.txtTax.TabIndex = 29;
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(92, 123);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(129, 25);
            this.txtTotal.TabIndex = 32;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(7, 87);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(36, 19);
            this.label13.TabIndex = 30;
            this.label13.Text = "Tax:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(7, 120);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 19);
            this.label14.TabIndex = 33;
            this.label14.Text = "Total:";
            // 
            // cmbTax
            // 
            this.cmbTax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTax.FormattingEnabled = true;
            this.cmbTax.Location = new System.Drawing.Point(92, 87);
            this.cmbTax.Name = "cmbTax";
            this.cmbTax.Size = new System.Drawing.Size(133, 25);
            this.cmbTax.TabIndex = 31;
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.ForeColor = System.Drawing.Color.White;
            this.CancelBtn.Location = new System.Drawing.Point(385, 692);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(125, 40);
            this.CancelBtn.TabIndex = 37;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = false;
            // 
            // NewItemBtn
            // 
            this.NewItemBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.NewItemBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NewItemBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemBtn.ForeColor = System.Drawing.Color.White;
            this.NewItemBtn.Location = new System.Drawing.Point(533, 697);
            this.NewItemBtn.Name = "NewItemBtn";
            this.NewItemBtn.Size = new System.Drawing.Size(129, 40);
            this.NewItemBtn.TabIndex = 36;
            this.NewItemBtn.Text = "Sales Return";
            this.NewItemBtn.UseVisualStyleBackColor = false;
            // 
            // SalesReturnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1327, 749);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.NewItemBtn);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtdescription);
            this.Controls.Add(this.cmbreturnreason);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.pnlOriginalInvoiceDetails);
            this.Controls.Add(this.pnlCustomerInfo);
            this.Controls.Add(this.pnlInvoiceSelection);
            this.Controls.Add(this.pnlReturnInfo);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SalesReturnForm";
            this.Text = "madni mobile Mobiles Rwp - Sales Return";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlReturnInfo.ResumeLayout(false);
            this.pnlReturnInfo.PerformLayout();
            this.pnlInvoiceSelection.ResumeLayout(false);
            this.pnlInvoiceSelection.PerformLayout();
            this.pnlCustomerInfo.ResumeLayout(false);
            this.pnlCustomerInfo.PerformLayout();
            this.pnlOriginalInvoiceDetails.ResumeLayout(false);
            this.pnlOriginalInvoiceDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlReturnInfo;
        private System.Windows.Forms.Label lblReturnDate;
        private System.Windows.Forms.DateTimePicker dtpReturnDate;
        private System.Windows.Forms.Label lblReturnNumber;
        private System.Windows.Forms.TextBox txtReturnNumber;
        private System.Windows.Forms.Panel pnlInvoiceSelection;
        private System.Windows.Forms.Button btnLoadInvoice;
        private System.Windows.Forms.Label lblInvoiceNumber;
        private System.Windows.Forms.ComboBox cmbInvoiceNumber;
        private System.Windows.Forms.Panel pnlCustomerInfo;
        private System.Windows.Forms.Label lblCustomerAddress;
        private System.Windows.Forms.TextBox txtCustomerAddress;
        private System.Windows.Forms.Label lblCustomerPhone;
        private System.Windows.Forms.TextBox txtCustomerPhone;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Panel pnlOriginalInvoiceDetails;
        private System.Windows.Forms.Label lblOriginalInvoiceTitle;
        private System.Windows.Forms.Label lblOriginalInvoiceNumber;
        private System.Windows.Forms.TextBox txtOriginalInvoiceNumber;
        private System.Windows.Forms.Label lblOriginalInvoiceDate;
        private System.Windows.Forms.TextBox txtOriginalInvoiceDate;
        private System.Windows.Forms.Label lblOriginalInvoiceTotal;
        private System.Windows.Forms.TextBox txtOriginalInvoiceTotal;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrignalQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReturnQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbreturnreason;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtdescription;
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
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button NewItemBtn;
    }
}