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
            this.lblInvoiceNumber = new System.Windows.Forms.Label();
            this.btnLoadInvoice = new System.Windows.Forms.Button();
            this.lblReturnNumber = new System.Windows.Forms.Label();
            this.txtReturnNumber = new System.Windows.Forms.TextBox();
            this.cmbInvoiceNumber = new System.Windows.Forms.ComboBox();
            this.lblReturnDate = new System.Windows.Forms.Label();
            this.dtpReturnDate = new System.Windows.Forms.DateTimePicker();
            this.pnlInvoiceSelection = new System.Windows.Forms.Panel();
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
            this.colSelect = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1710, 74);
            this.panel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(27, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(252, 37);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Sales Return - POS";
            // 
            // pnlReturnInfo
            // 
            this.pnlReturnInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReturnInfo.Controls.Add(this.lblInvoiceNumber);
            this.pnlReturnInfo.Controls.Add(this.btnLoadInvoice);
            this.pnlReturnInfo.Controls.Add(this.lblReturnNumber);
            this.pnlReturnInfo.Controls.Add(this.txtReturnNumber);
            this.pnlReturnInfo.Controls.Add(this.cmbInvoiceNumber);
            this.pnlReturnInfo.Controls.Add(this.lblReturnDate);
            this.pnlReturnInfo.Controls.Add(this.dtpReturnDate);
            this.pnlReturnInfo.Location = new System.Drawing.Point(2, 131);
            this.pnlReturnInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlReturnInfo.Name = "pnlReturnInfo";
            this.pnlReturnInfo.Size = new System.Drawing.Size(848, 93);
            this.pnlReturnInfo.TabIndex = 2;
            this.pnlReturnInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlReturnInfo_Paint);
            // 
            // lblInvoiceNumber
            // 
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceNumber.Location = new System.Drawing.Point(502, 60);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(76, 20);
            this.lblInvoiceNumber.TabIndex = 1;
            this.lblInvoiceNumber.Text = "Invoice #:";
            // 
            // btnLoadInvoice
            // 
            this.btnLoadInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLoadInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadInvoice.ForeColor = System.Drawing.Color.White;
            this.btnLoadInvoice.Location = new System.Drawing.Point(718, 55);
            this.btnLoadInvoice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLoadInvoice.Name = "btnLoadInvoice";
            this.btnLoadInvoice.Size = new System.Drawing.Size(100, 25);
            this.btnLoadInvoice.TabIndex = 2;
            this.btnLoadInvoice.Text = "Load";
            this.btnLoadInvoice.UseVisualStyleBackColor = false;
            // 
            // lblReturnNumber
            // 
            this.lblReturnNumber.AutoSize = true;
            this.lblReturnNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnNumber.Location = new System.Drawing.Point(3, 11);
            this.lblReturnNumber.Name = "lblReturnNumber";
            this.lblReturnNumber.Size = new System.Drawing.Size(142, 20);
            this.lblReturnNumber.TabIndex = 0;
            this.lblReturnNumber.Text = "Return Number:";
            // 
            // txtReturnNumber
            // 
            this.txtReturnNumber.BackColor = System.Drawing.Color.White;
            this.txtReturnNumber.Location = new System.Drawing.Point(157, 12);
            this.txtReturnNumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtReturnNumber.Name = "txtReturnNumber";
            this.txtReturnNumber.ReadOnly = true;
            this.txtReturnNumber.Size = new System.Drawing.Size(173, 22);
            this.txtReturnNumber.TabIndex = 1;
            // 
            // cmbInvoiceNumber
            // 
            this.cmbInvoiceNumber.BackColor = System.Drawing.Color.White;
            this.cmbInvoiceNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInvoiceNumber.FormattingEnabled = true;
            this.cmbInvoiceNumber.Location = new System.Drawing.Point(507, 18);
            this.cmbInvoiceNumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbInvoiceNumber.Name = "cmbInvoiceNumber";
            this.cmbInvoiceNumber.Size = new System.Drawing.Size(312, 24);
            this.cmbInvoiceNumber.TabIndex = 0;
            // 
            // lblReturnDate
            // 
            this.lblReturnDate.AutoSize = true;
            this.lblReturnDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnDate.Location = new System.Drawing.Point(9, 46);
            this.lblReturnDate.Name = "lblReturnDate";
            this.lblReturnDate.Size = new System.Drawing.Size(117, 20);
            this.lblReturnDate.TabIndex = 2;
            this.lblReturnDate.Text = "Return Date:";
            // 
            // dtpReturnDate
            // 
            this.dtpReturnDate.CustomFormat = "dddd, MMMM dd, yyyy";
            this.dtpReturnDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReturnDate.Location = new System.Drawing.Point(153, 44);
            this.dtpReturnDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpReturnDate.Name = "dtpReturnDate";
            this.dtpReturnDate.Size = new System.Drawing.Size(178, 22);
            this.dtpReturnDate.TabIndex = 3;
            // 
            // pnlInvoiceSelection
            // 
            this.pnlInvoiceSelection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlInvoiceSelection.Controls.Add(this.CancelBtn);
            this.pnlInvoiceSelection.Controls.Add(this.dataGridView1);
            this.pnlInvoiceSelection.Controls.Add(this.NewItemBtn);
            this.pnlInvoiceSelection.Controls.Add(this.panel2);
            this.pnlInvoiceSelection.Controls.Add(this.label1);
            this.pnlInvoiceSelection.Controls.Add(this.label2);
            this.pnlInvoiceSelection.Controls.Add(this.cmbreturnreason);
            this.pnlInvoiceSelection.Controls.Add(this.txtdescription);
            this.pnlInvoiceSelection.Location = new System.Drawing.Point(2, 337);
            this.pnlInvoiceSelection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlInvoiceSelection.Name = "pnlInvoiceSelection";
            this.pnlInvoiceSelection.Size = new System.Drawing.Size(1577, 395);
            this.pnlInvoiceSelection.TabIndex = 3;
            this.pnlInvoiceSelection.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlInvoiceSelection_Paint);
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
            this.pnlCustomerInfo.Location = new System.Drawing.Point(884, 131);
            this.pnlCustomerInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlCustomerInfo.Name = "pnlCustomerInfo";
            this.pnlCustomerInfo.Size = new System.Drawing.Size(521, 93);
            this.pnlCustomerInfo.TabIndex = 5;
            this.pnlCustomerInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCustomerInfo_Paint);
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerAddress.Location = new System.Drawing.Point(212, 43);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(72, 17);
            this.lblCustomerAddress.TabIndex = 6;
            this.lblCustomerAddress.Text = "Address:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.Color.White;
            this.txtCustomerAddress.Location = new System.Drawing.Point(296, 39);
            this.txtCustomerAddress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(203, 22);
            this.txtCustomerAddress.TabIndex = 7;
            // 
            // lblCustomerPhone
            // 
            this.lblCustomerPhone.AutoSize = true;
            this.lblCustomerPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerPhone.Location = new System.Drawing.Point(14, 44);
            this.lblCustomerPhone.Name = "lblCustomerPhone";
            this.lblCustomerPhone.Size = new System.Drawing.Size(59, 17);
            this.lblCustomerPhone.TabIndex = 4;
            this.lblCustomerPhone.Text = "Phone:";
            // 
            // txtCustomerPhone
            // 
            this.txtCustomerPhone.BackColor = System.Drawing.Color.White;
            this.txtCustomerPhone.Location = new System.Drawing.Point(77, 42);
            this.txtCustomerPhone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCustomerPhone.Name = "txtCustomerPhone";
            this.txtCustomerPhone.ReadOnly = true;
            this.txtCustomerPhone.Size = new System.Drawing.Size(107, 22);
            this.txtCustomerPhone.TabIndex = 5;
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerName.Location = new System.Drawing.Point(274, 12);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(54, 17);
            this.lblCustomerName.TabIndex = 2;
            this.lblCustomerName.Text = "Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.Color.White;
            this.txtCustomerName.Location = new System.Drawing.Point(343, 12);
            this.txtCustomerName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(157, 22);
            this.txtCustomerName.TabIndex = 3;
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomer.Location = new System.Drawing.Point(3, 10);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(81, 17);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Customer:";
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.BackColor = System.Drawing.Color.White;
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(91, 9);
            this.cmbCustomer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(178, 24);
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
            this.pnlOriginalInvoiceDetails.Location = new System.Drawing.Point(884, 241);
            this.pnlOriginalInvoiceDetails.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlOriginalInvoiceDetails.Name = "pnlOriginalInvoiceDetails";
            this.pnlOriginalInvoiceDetails.Size = new System.Drawing.Size(538, 92);
            this.pnlOriginalInvoiceDetails.TabIndex = 6;
            // 
            // lblOriginalInvoiceTitle
            // 
            this.lblOriginalInvoiceTitle.AutoSize = true;
            this.lblOriginalInvoiceTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.lblOriginalInvoiceTitle.Location = new System.Drawing.Point(9, 22);
            this.lblOriginalInvoiceTitle.Name = "lblOriginalInvoiceTitle";
            this.lblOriginalInvoiceTitle.Size = new System.Drawing.Size(196, 23);
            this.lblOriginalInvoiceTitle.TabIndex = 0;
            this.lblOriginalInvoiceTitle.Text = "Original Invoice Details";
            this.lblOriginalInvoiceTitle.Click += new System.EventHandler(this.lblOriginalInvoiceTitle_Click);
            // 
            // lblOriginalInvoiceNumber
            // 
            this.lblOriginalInvoiceNumber.AutoSize = true;
            this.lblOriginalInvoiceNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceNumber.Location = new System.Drawing.Point(9, 46);
            this.lblOriginalInvoiceNumber.Name = "lblOriginalInvoiceNumber";
            this.lblOriginalInvoiceNumber.Size = new System.Drawing.Size(76, 20);
            this.lblOriginalInvoiceNumber.TabIndex = 1;
            this.lblOriginalInvoiceNumber.Text = "Invoice #:";
            // 
            // txtOriginalInvoiceNumber
            // 
            this.txtOriginalInvoiceNumber.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceNumber.Location = new System.Drawing.Point(101, 46);
            this.txtOriginalInvoiceNumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtOriginalInvoiceNumber.Name = "txtOriginalInvoiceNumber";
            this.txtOriginalInvoiceNumber.ReadOnly = true;
            this.txtOriginalInvoiceNumber.Size = new System.Drawing.Size(107, 22);
            this.txtOriginalInvoiceNumber.TabIndex = 2;
            // 
            // lblOriginalInvoiceDate
            // 
            this.lblOriginalInvoiceDate.AutoSize = true;
            this.lblOriginalInvoiceDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceDate.Location = new System.Drawing.Point(213, 46);
            this.lblOriginalInvoiceDate.Name = "lblOriginalInvoiceDate";
            this.lblOriginalInvoiceDate.Size = new System.Drawing.Size(46, 20);
            this.lblOriginalInvoiceDate.TabIndex = 3;
            this.lblOriginalInvoiceDate.Text = "Date:";
            // 
            // txtOriginalInvoiceDate
            // 
            this.txtOriginalInvoiceDate.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceDate.Location = new System.Drawing.Point(267, 46);
            this.txtOriginalInvoiceDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtOriginalInvoiceDate.Name = "txtOriginalInvoiceDate";
            this.txtOriginalInvoiceDate.ReadOnly = true;
            this.txtOriginalInvoiceDate.Size = new System.Drawing.Size(102, 22);
            this.txtOriginalInvoiceDate.TabIndex = 4;
            // 
            // lblOriginalInvoiceTotal
            // 
            this.lblOriginalInvoiceTotal.AutoSize = true;
            this.lblOriginalInvoiceTotal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOriginalInvoiceTotal.Location = new System.Drawing.Point(373, 46);
            this.lblOriginalInvoiceTotal.Name = "lblOriginalInvoiceTotal";
            this.lblOriginalInvoiceTotal.Size = new System.Drawing.Size(48, 20);
            this.lblOriginalInvoiceTotal.TabIndex = 5;
            this.lblOriginalInvoiceTotal.Text = "Total:";
            // 
            // txtOriginalInvoiceTotal
            // 
            this.txtOriginalInvoiceTotal.BackColor = System.Drawing.Color.White;
            this.txtOriginalInvoiceTotal.Location = new System.Drawing.Point(427, 46);
            this.txtOriginalInvoiceTotal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtOriginalInvoiceTotal.Name = "txtOriginalInvoiceTotal";
            this.txtOriginalInvoiceTotal.ReadOnly = true;
            this.txtOriginalInvoiceTotal.Size = new System.Drawing.Size(89, 22);
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
            this.colSelect,
            this.ItemCode,
            this.ItemName,
            this.OrignalQty,
            this.ReturnQty,
            this.Price,
            this.Total});
            this.dataGridView1.Location = new System.Drawing.Point(13, 12);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(990, 332);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // colSelect
            // 
            this.colSelect.HeaderText = "Select";
            this.colSelect.MinimumWidth = 8;
            this.colSelect.Name = "colSelect";
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
            this.label1.Location = new System.Drawing.Point(1180, 257);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Sales Return Reason";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // cmbreturnreason
            // 
            this.cmbreturnreason.BackColor = System.Drawing.Color.White;
            this.cmbreturnreason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbreturnreason.FormattingEnabled = true;
            this.cmbreturnreason.Location = new System.Drawing.Point(1184, 275);
            this.cmbreturnreason.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbreturnreason.Name = "cmbreturnreason";
            this.cmbreturnreason.Size = new System.Drawing.Size(240, 24);
            this.cmbreturnreason.TabIndex = 9;
            this.cmbreturnreason.SelectedIndexChanged += new System.EventHandler(this.cmbreturnreason_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1184, 308);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Description";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtdescription
            // 
            this.txtdescription.BackColor = System.Drawing.Color.White;
            this.txtdescription.Location = new System.Drawing.Point(1184, 327);
            this.txtdescription.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtdescription.Multiline = true;
            this.txtdescription.Name = "txtdescription";
            this.txtdescription.ReadOnly = true;
            this.txtdescription.Size = new System.Drawing.Size(322, 47);
            this.txtdescription.TabIndex = 1;
            this.txtdescription.TextChanged += new System.EventHandler(this.txtdescription_TextChanged);
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
            this.panel2.Location = new System.Drawing.Point(1200, 25);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(306, 161);
            this.panel2.TabIndex = 35;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 23);
            this.label3.TabIndex = 26;
            this.label3.Text = "Sub Total:";
            // 
            // txtsubTotal
            // 
            this.txtsubTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsubTotal.Location = new System.Drawing.Point(123, 18);
            this.txtsubTotal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtsubTotal.Name = "txtsubTotal";
            this.txtsubTotal.Size = new System.Drawing.Size(171, 30);
            this.txtsubTotal.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(9, 49);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 23);
            this.label10.TabIndex = 28;
            this.label10.Text = "Discount:";
            // 
            // txtTaxPercent
            // 
            this.txtTaxPercent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaxPercent.Location = new System.Drawing.Point(123, 48);
            this.txtTaxPercent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTaxPercent.Name = "txtTaxPercent";
            this.txtTaxPercent.Size = new System.Drawing.Size(87, 30);
            this.txtTaxPercent.TabIndex = 27;
            // 
            // txtTax
            // 
            this.txtTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTax.Location = new System.Drawing.Point(216, 48);
            this.txtTax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTax.Name = "txtTax";
            this.txtTax.Size = new System.Drawing.Size(77, 30);
            this.txtTax.TabIndex = 29;
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(123, 114);
            this.txtTotal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(171, 30);
            this.txtTotal.TabIndex = 32;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(9, 81);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 23);
            this.label13.TabIndex = 30;
            this.label13.Text = "Tax:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(9, 110);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 23);
            this.label14.TabIndex = 33;
            this.label14.Text = "Total:";
            // 
            // cmbTax
            // 
            this.cmbTax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTax.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTax.FormattingEnabled = true;
            this.cmbTax.Location = new System.Drawing.Point(123, 81);
            this.cmbTax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbTax.Name = "cmbTax";
            this.cmbTax.Size = new System.Drawing.Size(176, 31);
            this.cmbTax.TabIndex = 31;
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.ForeColor = System.Drawing.Color.White;
            this.CancelBtn.Location = new System.Drawing.Point(1381, 203);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(141, 50);
            this.CancelBtn.TabIndex = 37;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = false;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click_1);
            // 
            // NewItemBtn
            // 
            this.NewItemBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.NewItemBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NewItemBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewItemBtn.ForeColor = System.Drawing.Color.White;
            this.NewItemBtn.Location = new System.Drawing.Point(1200, 203);
            this.NewItemBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.NewItemBtn.Name = "NewItemBtn";
            this.NewItemBtn.Size = new System.Drawing.Size(147, 50);
            this.NewItemBtn.TabIndex = 36;
            this.NewItemBtn.Text = "Sales Return";
            this.NewItemBtn.UseVisualStyleBackColor = false;
            this.NewItemBtn.Click += new System.EventHandler(this.NewItemBtn_Click_1);
            // 
            // SalesReturnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1710, 840);
            this.Controls.Add(this.pnlOriginalInvoiceDetails);
            this.Controls.Add(this.pnlCustomerInfo);
            this.Controls.Add(this.pnlInvoiceSelection);
            this.Controls.Add(this.pnlReturnInfo);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SalesReturnForm";
            this.Text = "Vape Store - Sales Return";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SalesReturnForm_Load);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn colSelect;
    }
}
