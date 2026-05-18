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
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.picBarcode = new System.Windows.Forms.PictureBox();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblPayment = new System.Windows.Forms.Label();
            this.btnSavePurchase = new System.Windows.Forms.Button();
            this.pnlTotals = new System.Windows.Forms.Panel();
            this.lblBalance = new System.Windows.Forms.Label();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.lblPaid = new System.Windows.Forms.Label();
            this.txtPaid = new System.Windows.Forms.TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.lblTaxAmount = new System.Windows.Forms.Label();
            this.txtTaxAmount = new System.Windows.Forms.TextBox();
            this.txtTaxPercent = new System.Windows.Forms.TextBox();
            this.lblTaxPercent = new System.Windows.Forms.Label();
            this.txtDiscountAmount = new System.Windows.Forms.TextBox();
            this.txtDiscountPercent = new System.Windows.Forms.TextBox();
            this.lblDiscountPercent = new System.Windows.Forms.Label();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.lblInvoiceNo = new System.Windows.Forms.Label();
            this.txtBarcodeScanner = new System.Windows.Forms.TextBox();
            this.lblBarcodeScanner = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClearForm = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.dtpInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.lblInvoiceValue = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.cmbVendorName = new System.Windows.Forms.ComboBox();
            this.lblVendorName = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.lblBrand = new System.Windows.Forms.Label();
            this.cmbProductName = new System.Windows.Forms.ComboBox();
            this.lblProductName = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.txtVendorCode = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDiscountAmount = new System.Windows.Forms.Label();
            this.txtExistingStock = new System.Windows.Forms.TextBox();
            this.lblExistingStock = new System.Windows.Forms.Label();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.dgvPurchaseItems = new System.Windows.Forms.DataGridView();
            this.colSrNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colProductCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBatchNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExpiryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPurchasePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSalePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFreeQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlBottom.SuspendLayout();
            this.pnlActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).BeginInit();
            this.pnlTotals.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseItems)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlBottom.Controls.Add(this.pnlActions);
            this.pnlBottom.Controls.Add(this.btnSavePurchase);
            this.pnlBottom.Controls.Add(this.pnlTotals);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 800);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1480, 250);
            this.pnlBottom.TabIndex = 5;
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.picBarcode);
            this.pnlActions.Controls.Add(this.cmbPaymentMethod);
            this.pnlActions.Controls.Add(this.lblPayment);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlActions.Location = new System.Drawing.Point(0, 0);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(460, 250);
            this.pnlActions.TabIndex = 0;
            // 
            // picBarcode
            // 
            this.picBarcode.Location = new System.Drawing.Point(22, 109);
            this.picBarcode.Name = "picBarcode";
            this.picBarcode.Size = new System.Drawing.Size(281, 116);
            this.picBarcode.TabIndex = 3;
            this.picBarcode.TabStop = false;
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.BackColor = System.Drawing.Color.White;
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbPaymentMethod.FormattingEnabled = true;
            this.cmbPaymentMethod.Items.AddRange(new object[] {
            "Cash",
            "Card",
            "Credit",
            "Other"});
            this.cmbPaymentMethod.Location = new System.Drawing.Point(22, 52);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(281, 36);
            this.cmbPaymentMethod.TabIndex = 1;
            // 
            // lblPayment
            // 
            this.lblPayment.AutoSize = true;
            this.lblPayment.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPayment.Location = new System.Drawing.Point(22, 19);
            this.lblPayment.Name = "lblPayment";
            this.lblPayment.Size = new System.Drawing.Size(180, 28);
            this.lblPayment.TabIndex = 0;
            this.lblPayment.Text = "Payment Method:";
            // 
            // btnSavePurchase
            // 
            this.btnSavePurchase.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSavePurchase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePurchase.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSavePurchase.ForeColor = System.Drawing.Color.White;
            this.btnSavePurchase.Location = new System.Drawing.Point(493, 26);
            this.btnSavePurchase.Name = "btnSavePurchase";
            this.btnSavePurchase.Size = new System.Drawing.Size(231, 62);
            this.btnSavePurchase.TabIndex = 2;
            this.btnSavePurchase.Text = "SAVE AND PRINT";
            this.btnSavePurchase.UseVisualStyleBackColor = false;
            // 
            // pnlTotals
            // 
            this.pnlTotals.BackColor = System.Drawing.Color.White;
            this.pnlTotals.Controls.Add(this.lblBalance);
            this.pnlTotals.Controls.Add(this.txtBalance);
            this.pnlTotals.Controls.Add(this.lblPaid);
            this.pnlTotals.Controls.Add(this.txtPaid);
            this.pnlTotals.Controls.Add(this.lblTotal);
            this.pnlTotals.Controls.Add(this.txtTotal);
            this.pnlTotals.Controls.Add(this.lblTaxAmount);
            this.pnlTotals.Controls.Add(this.txtTaxAmount);
            this.pnlTotals.Controls.Add(this.txtTaxPercent);
            this.pnlTotals.Controls.Add(this.lblTaxPercent);
            this.pnlTotals.Controls.Add(this.txtDiscountAmount);
            this.pnlTotals.Controls.Add(this.txtDiscountPercent);
            this.pnlTotals.Controls.Add(this.lblDiscountPercent);
            this.pnlTotals.Controls.Add(this.txtSubtotal);
            this.pnlTotals.Controls.Add(this.lblSubtotal);
            this.pnlTotals.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlTotals.Location = new System.Drawing.Point(1030, 0);
            this.pnlTotals.Name = "pnlTotals";
            this.pnlTotals.Size = new System.Drawing.Size(450, 250);
            this.pnlTotals.TabIndex = 0;
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBalance.Location = new System.Drawing.Point(22, 212);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(91, 28);
            this.lblBalance.TabIndex = 12;
            this.lblBalance.Text = "Balance:";
            // 
            // txtBalance
            // 
            this.txtBalance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBalance.Location = new System.Drawing.Point(169, 209);
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(258, 34);
            this.txtBalance.TabIndex = 13;
            this.txtBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPaid
            // 
            this.lblPaid.AutoSize = true;
            this.lblPaid.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPaid.Location = new System.Drawing.Point(22, 175);
            this.lblPaid.Name = "lblPaid";
            this.lblPaid.Size = new System.Drawing.Size(58, 28);
            this.lblPaid.TabIndex = 10;
            this.lblPaid.Text = "Paid:";
            // 
            // txtPaid
            // 
            this.txtPaid.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPaid.Location = new System.Drawing.Point(169, 171);
            this.txtPaid.Name = "txtPaid";
            this.txtPaid.Size = new System.Drawing.Size(258, 34);
            this.txtPaid.TabIndex = 11;
            this.txtPaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblTotal.Location = new System.Drawing.Point(22, 131);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(77, 32);
            this.lblTotal.TabIndex = 8;
            this.lblTotal.Text = "Total:";
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(242)))), ((int)(((byte)(253)))));
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.txtTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.txtTotal.Location = new System.Drawing.Point(169, 128);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(258, 39);
            this.txtTotal.TabIndex = 9;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTaxAmount
            // 
            this.lblTaxAmount.AutoSize = true;
            this.lblTaxAmount.Location = new System.Drawing.Point(245, 85);
            this.lblTaxAmount.Name = "lblTaxAmount";
            this.lblTaxAmount.Size = new System.Drawing.Size(42, 20);
            this.lblTaxAmount.TabIndex = 14;
            this.lblTaxAmount.Text = "Amt:";
            this.lblTaxAmount.Visible = false;
            // 
            // txtTaxAmount
            // 
            this.txtTaxAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTaxAmount.Location = new System.Drawing.Point(250, 81);
            this.txtTaxAmount.Name = "txtTaxAmount";
            this.txtTaxAmount.Size = new System.Drawing.Size(170, 31);
            this.txtTaxAmount.TabIndex = 7;
            this.txtTaxAmount.Text = "0.00";
            this.txtTaxAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTaxPercent
            // 
            this.txtTaxPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTaxPercent.Location = new System.Drawing.Point(170, 81);
            this.txtTaxPercent.Name = "txtTaxPercent";
            this.txtTaxPercent.Size = new System.Drawing.Size(70, 31);
            this.txtTaxPercent.TabIndex = 6;
            this.txtTaxPercent.Text = "0";
            this.txtTaxPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTaxPercent
            // 
            this.lblTaxPercent.AutoSize = true;
            this.lblTaxPercent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTaxPercent.Location = new System.Drawing.Point(20, 85);
            this.lblTaxPercent.Name = "lblTaxPercent";
            this.lblTaxPercent.Size = new System.Drawing.Size(81, 25);
            this.lblTaxPercent.TabIndex = 5;
            this.lblTaxPercent.Text = "Tax (%):";
            // 
            // txtDiscountAmount
            // 
            this.txtDiscountAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDiscountAmount.Location = new System.Drawing.Point(250, 46);
            this.txtDiscountAmount.Name = "txtDiscountAmount";
            this.txtDiscountAmount.Size = new System.Drawing.Size(170, 31);
            this.txtDiscountAmount.TabIndex = 4;
            this.txtDiscountAmount.Text = "0.00";
            this.txtDiscountAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDiscountPercent
            // 
            this.txtDiscountPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDiscountPercent.Location = new System.Drawing.Point(170, 46);
            this.txtDiscountPercent.Name = "txtDiscountPercent";
            this.txtDiscountPercent.Size = new System.Drawing.Size(70, 31);
            this.txtDiscountPercent.TabIndex = 3;
            this.txtDiscountPercent.Text = "0";
            this.txtDiscountPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblDiscountPercent
            // 
            this.lblDiscountPercent.AutoSize = true;
            this.lblDiscountPercent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDiscountPercent.Location = new System.Drawing.Point(20, 50);
            this.lblDiscountPercent.Name = "lblDiscountPercent";
            this.lblDiscountPercent.Size = new System.Drawing.Size(127, 25);
            this.lblDiscountPercent.TabIndex = 2;
            this.lblDiscountPercent.Text = "Discount (%):";
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSubtotal.Location = new System.Drawing.Point(170, 11);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.ReadOnly = true;
            this.txtSubtotal.Size = new System.Drawing.Size(250, 34);
            this.txtSubtotal.TabIndex = 1;
            this.txtSubtotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblSubtotal.Location = new System.Drawing.Point(20, 15);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(97, 28);
            this.lblSubtotal.TabIndex = 0;
            this.lblSubtotal.Text = "Subtotal:";
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.txtInvoiceNo);
            this.pnlHeader.Controls.Add(this.lblInvoiceNo);
            this.pnlHeader.Controls.Add(this.btnRefresh);
            this.pnlHeader.Controls.Add(this.btnCancel);
            this.pnlHeader.Controls.Add(this.btnClearForm);
            this.pnlHeader.Controls.Add(this.btnNew);
            this.pnlHeader.Controls.Add(this.btnPrintInvoice);
            this.pnlHeader.Controls.Add(this.dtpInvoiceDate);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 120);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1480, 60);
            this.pnlHeader.TabIndex = 3;
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtInvoiceNo.Location = new System.Drawing.Point(115, 15);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(180, 34);
            this.txtInvoiceNo.TabIndex = 15;
            // 
            // lblInvoiceNo
            // 
            this.lblInvoiceNo.AutoSize = true;
            this.lblInvoiceNo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceNo.Location = new System.Drawing.Point(10, 18);
            this.lblInvoiceNo.Name = "lblInvoiceNo";
            this.lblInvoiceNo.Size = new System.Drawing.Size(103, 28);
            this.lblInvoiceNo.TabIndex = 15;
            this.lblInvoiceNo.Text = "Invoice #:";
            // 
            // txtBarcodeScanner
            // 
            this.txtBarcodeScanner.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBarcodeScanner.Location = new System.Drawing.Point(1142, 43);
            this.txtBarcodeScanner.Name = "txtBarcodeScanner";
            this.txtBarcodeScanner.Size = new System.Drawing.Size(150, 34);
            this.txtBarcodeScanner.TabIndex = 16;
            // 
            // lblBarcodeScanner
            // 
            this.lblBarcodeScanner.AutoSize = true;
            this.lblBarcodeScanner.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBarcodeScanner.Location = new System.Drawing.Point(1137, 7);
            this.lblBarcodeScanner.Name = "lblBarcodeScanner";
            this.lblBarcodeScanner.Size = new System.Drawing.Size(94, 28);
            this.lblBarcodeScanner.TabIndex = 17;
            this.lblBarcodeScanner.Text = "Barcode:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(190)))), ((int)(((byte)(250)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(440, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 40);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1297, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            // 
            // btnClearForm
            // 
            this.btnClearForm.Location = new System.Drawing.Point(560, 10);
            this.btnClearForm.Name = "btnClearForm";
            this.btnClearForm.Size = new System.Drawing.Size(100, 40);
            this.btnClearForm.TabIndex = 7;
            this.btnClearForm.Text = "Clear";
            this.btnClearForm.UseVisualStyleBackColor = true;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(190)))), ((int)(((byte)(250)))));
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(320, 10);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(100, 40);
            this.btnNew.TabIndex = 3;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            // 
            // btnPrintInvoice
            // 
            this.btnPrintInvoice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(190)))), ((int)(((byte)(250)))));
            this.btnPrintInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrintInvoice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrintInvoice.ForeColor = System.Drawing.Color.White;
            this.btnPrintInvoice.Location = new System.Drawing.Point(680, 10);
            this.btnPrintInvoice.Name = "btnPrintInvoice";
            this.btnPrintInvoice.Size = new System.Drawing.Size(120, 40);
            this.btnPrintInvoice.TabIndex = 8;
            this.btnPrintInvoice.Text = "Print Invoice";
            this.btnPrintInvoice.UseVisualStyleBackColor = false;
            // 
            // dtpInvoiceDate
            // 
            this.dtpInvoiceDate.Location = new System.Drawing.Point(979, 18);
            this.dtpInvoiceDate.Name = "dtpInvoiceDate";
            this.dtpInvoiceDate.Size = new System.Drawing.Size(298, 26);
            this.dtpInvoiceDate.TabIndex = 14;
            // 
            // lblInvoiceValue
            // 
            this.lblInvoiceValue.AutoSize = true;
            this.lblInvoiceValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblInvoiceValue.Location = new System.Drawing.Point(130, 18);
            this.lblInvoiceValue.Name = "lblInvoiceValue";
            this.lblInvoiceValue.Size = new System.Drawing.Size(0, 28);
            this.lblInvoiceValue.TabIndex = 1;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.cmbVendorName);
            this.pnlTop.Controls.Add(this.lblVendorName);
            this.pnlTop.Controls.Add(this.lblBarcodeScanner);
            this.pnlTop.Controls.Add(this.txtBarcodeScanner);
            this.pnlTop.Controls.Add(this.cmbCategory);
            this.pnlTop.Controls.Add(this.lblCategory);
            this.pnlTop.Controls.Add(this.cmbBrand);
            this.pnlTop.Controls.Add(this.lblBrand);
            this.pnlTop.Controls.Add(this.cmbProductName);
            this.pnlTop.Controls.Add(this.lblProductName);
            this.pnlTop.Controls.Add(this.txtQuantity);
            this.pnlTop.Controls.Add(this.lblQty);
            this.pnlTop.Controls.Add(this.btnAddItem);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1480, 120);
            this.pnlTop.TabIndex = 4;
            // 
            // cmbVendorName
            // 
            this.cmbVendorName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVendorName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbVendorName.Location = new System.Drawing.Point(900, 40);
            this.cmbVendorName.Name = "cmbVendorName";
            this.cmbVendorName.Size = new System.Drawing.Size(220, 36);
            this.cmbVendorName.TabIndex = 10;
            // 
            // lblVendorName
            // 
            this.lblVendorName.AutoSize = true;
            this.lblVendorName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblVendorName.Location = new System.Drawing.Point(900, 10);
            this.lblVendorName.Name = "lblVendorName";
            this.lblVendorName.Size = new System.Drawing.Size(133, 25);
            this.lblVendorName.TabIndex = 9;
            this.lblVendorName.Text = "Vendor Name:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCategory.Location = new System.Drawing.Point(20, 40);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(180, 36);
            this.cmbCategory.TabIndex = 1;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCategory.Location = new System.Drawing.Point(20, 10);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(95, 25);
            this.lblCategory.TabIndex = 0;
            this.lblCategory.Text = "Category:";
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbBrand.Location = new System.Drawing.Point(210, 40);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(180, 36);
            this.cmbBrand.TabIndex = 3;
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBrand.Location = new System.Drawing.Point(210, 10);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(68, 25);
            this.lblBrand.TabIndex = 2;
            this.lblBrand.Text = "Brand:";
            // 
            // cmbProductName
            // 
            this.cmbProductName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbProductName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbProductName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbProductName.Location = new System.Drawing.Point(420, 40);
            this.cmbProductName.Name = "cmbProductName";
            this.cmbProductName.Size = new System.Drawing.Size(281, 36);
            this.cmbProductName.TabIndex = 5;
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProductName.Location = new System.Drawing.Point(400, 10);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(146, 25);
            this.lblProductName.TabIndex = 4;
            this.lblProductName.Text = "Search Product:";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtQuantity.Location = new System.Drawing.Point(720, 40);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(80, 34);
            this.txtQuantity.TabIndex = 7;
            this.txtQuantity.Text = "1";
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblQty.Location = new System.Drawing.Point(720, 10);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(48, 25);
            this.lblQty.TabIndex = 6;
            this.lblQty.Text = "Qty:";
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(190)))), ((int)(((byte)(250)))));
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Location = new System.Drawing.Point(810, 37);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(80, 40);
            this.btnAddItem.TabIndex = 8;
            this.btnAddItem.Text = "Add";
            this.btnAddItem.UseVisualStyleBackColor = false;
            // 
            // txtVendorCode
            // 
            this.txtVendorCode.Location = new System.Drawing.Point(0, 0);
            this.txtVendorCode.Name = "txtVendorCode";
            this.txtVendorCode.Size = new System.Drawing.Size(100, 26);
            this.txtVendorCode.TabIndex = 11;
            this.txtVendorCode.Visible = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(0, 0);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(100, 26);
            this.txtDescription.TabIndex = 12;
            this.txtDescription.Visible = false;
            // 
            // lblDiscountAmount
            // 
            this.lblDiscountAmount.AutoSize = true;
            this.lblDiscountAmount.Location = new System.Drawing.Point(245, 50);
            this.lblDiscountAmount.Name = "lblDiscountAmount";
            this.lblDiscountAmount.Size = new System.Drawing.Size(21, 20);
            this.lblDiscountAmount.TabIndex = 0;
            this.lblDiscountAmount.Text = "Amt:";
            this.lblDiscountAmount.Visible = false;
            // 
            // txtExistingStock
            // 
            this.txtExistingStock.Location = new System.Drawing.Point(960, 7);
            this.txtExistingStock.Name = "txtExistingStock";
            this.txtExistingStock.ReadOnly = true;
            this.txtExistingStock.Size = new System.Drawing.Size(80, 26);
            this.txtExistingStock.TabIndex = 18;
            this.txtExistingStock.Visible = false;
            // 
            // lblExistingStock
            // 
            this.lblExistingStock.AutoSize = true;
            this.lblExistingStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblExistingStock.Location = new System.Drawing.Point(820, 10);
            this.lblExistingStock.Name = "lblExistingStock";
            this.lblExistingStock.Size = new System.Drawing.Size(136, 25);
            this.lblExistingStock.TabIndex = 17;
            this.lblExistingStock.Text = "Existing Stock:";
            this.lblExistingStock.Visible = false;
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelMainContainer.Controls.Add(this.dgvPurchaseItems);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 180);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.panelMainContainer.Size = new System.Drawing.Size(1480, 620);
            this.panelMainContainer.TabIndex = 0;
            // 
            // dgvPurchaseItems
            // 
            this.dgvPurchaseItems.AllowUserToAddRows = false;
            this.dgvPurchaseItems.BackgroundColor = System.Drawing.Color.White;
            this.dgvPurchaseItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPurchaseItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPurchaseItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSrNo,
            this.colProductName,
            this.colProductCode,
            this.colBatchNo,
            this.colExpiryDate,
            this.colPurchasePrice,
            this.colSalePrice,
            this.colQty,
            this.colFreeQty,
            this.colTotal,
            this.colDelete});
            this.dgvPurchaseItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPurchaseItems.Location = new System.Drawing.Point(10, 10);
            this.dgvPurchaseItems.Name = "dgvPurchaseItems";
            this.dgvPurchaseItems.RowHeadersVisible = false;
            this.dgvPurchaseItems.RowHeadersWidth = 62;
            this.dgvPurchaseItems.RowTemplate.Height = 28;
            this.dgvPurchaseItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPurchaseItems.Size = new System.Drawing.Size(1460, 600);
            this.dgvPurchaseItems.TabIndex = 0;
            // 
            // colSrNo
            // 
            this.colSrNo.HeaderText = "Sr#";
            this.colSrNo.MinimumWidth = 8;
            this.colSrNo.Name = "colSrNo";
            this.colSrNo.ReadOnly = true;
            this.colSrNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSrNo.Width = 60;
            // 
            // colProductName
            // 
            this.colProductName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colProductName.HeaderText = "Product Name";
            this.colProductName.MinimumWidth = 8;
            this.colProductName.Name = "colProductName";
            this.colProductName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProductName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colProductCode
            // 
            this.colProductCode.HeaderText = "Product Code";
            this.colProductCode.MinimumWidth = 8;
            this.colProductCode.Name = "colProductCode";
            this.colProductCode.ReadOnly = true;
            this.colProductCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProductCode.Width = 150;
            // 
            // colBatchNo
            // 
            this.colBatchNo.HeaderText = "Batch No";
            this.colBatchNo.MinimumWidth = 8;
            this.colBatchNo.Name = "colBatchNo";
            this.colBatchNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colBatchNo.Visible = false;
            this.colBatchNo.Width = 120;
            // 
            // colExpiryDate
            // 
            this.colExpiryDate.HeaderText = "Expiry Date";
            this.colExpiryDate.MinimumWidth = 8;
            this.colExpiryDate.Name = "colExpiryDate";
            this.colExpiryDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colExpiryDate.Width = 120;
            // 
            // colPurchasePrice
            // 
            this.colPurchasePrice.HeaderText = "Purchase Price";
            this.colPurchasePrice.MinimumWidth = 8;
            this.colPurchasePrice.Name = "colPurchasePrice";
            this.colPurchasePrice.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colPurchasePrice.Width = 120;
            // 
            // colSalePrice
            // 
            this.colSalePrice.HeaderText = "Sale Price";
            this.colSalePrice.MinimumWidth = 8;
            this.colSalePrice.Name = "colSalePrice";
            this.colSalePrice.Visible = true;
            this.colSalePrice.Width = 150;
            // 
            // colQty
            // 
            this.colQty.HeaderText = "Qty";
            this.colQty.MinimumWidth = 8;
            this.colQty.Name = "colQty";
            this.colQty.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colQty.Width = 80;
            // 
            // colFreeQty
            // 
            this.colFreeQty.HeaderText = "Free Qty";
            this.colFreeQty.MinimumWidth = 8;
            this.colFreeQty.Name = "colFreeQty";
            this.colFreeQty.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colFreeQty.Visible = false;
            this.colFreeQty.Width = 80;
            // 
            // colTotal
            // 
            this.colTotal.HeaderText = "Total Amount";
            this.colTotal.MinimumWidth = 8;
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTotal.Width = 140;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "Action";
            this.colDelete.MinimumWidth = 8;
            this.colDelete.Name = "colDelete";
            this.colDelete.ReadOnly = true;
            this.colDelete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colDelete.Text = "Remove";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.Width = 150;
            // 
            // NewPurchase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1480, 1050);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlTop);
            this.Name = "NewPurchase";
            this.Text = "PurchaseForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlBottom.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).EndInit();
            this.pnlTotals.ResumeLayout(false);
            this.pnlTotals.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.panelMainContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Panel pnlTotals;
        private System.Windows.Forms.Label lblInvoiceNo;
        private System.Windows.Forms.Label lblInvoiceValue;
        private System.Windows.Forms.DateTimePicker dtpInvoiceDate;
        private System.Windows.Forms.TextBox txtBarcodeScanner;
        private System.Windows.Forms.Label lblBarcodeScanner;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.Button btnClearForm;
        private System.Windows.Forms.Button btnCancel;

        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.ComboBox cmbProductName;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Label lblVendorName;
        private System.Windows.Forms.ComboBox cmbVendorName;
        private System.Windows.Forms.TextBox txtExistingStock;
        private System.Windows.Forms.Label lblExistingStock;
        private System.Windows.Forms.Panel panelMainContainer;
        private System.Windows.Forms.DataGridView dgvPurchaseItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSrNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBatchNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExpiryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPurchasePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSalePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFreeQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.Label lblPayment;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.Label lblDiscountPercent;
        private System.Windows.Forms.TextBox txtDiscountPercent;
        private System.Windows.Forms.Label lblDiscountAmount;
        private System.Windows.Forms.TextBox txtDiscountAmount;
        private System.Windows.Forms.Label lblTaxPercent;
        private System.Windows.Forms.TextBox txtTaxPercent;
        private System.Windows.Forms.Label lblTaxAmount;
        private System.Windows.Forms.TextBox txtTaxAmount;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label lblPaid;
        private System.Windows.Forms.TextBox txtPaid;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Button btnSavePurchase;
        private System.Windows.Forms.TextBox txtVendorCode;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.PictureBox picBarcode;
    }
}
