namespace Vape_Store
{
    partial class LowStockReportForm
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblStockStatus = new System.Windows.Forms.Label();
            this.chkLowStock = new System.Windows.Forms.CheckBox();
            this.chkOutOfStock = new System.Windows.Forms.CheckBox();
            this.chkCriticalStock = new System.Windows.Forms.CheckBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.btnCreatePurchaseOrder = new System.Windows.Forms.Button();
            this.btnExportPDF = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlData = new System.Windows.Forms.Panel();
            this.dgvLowStockReport = new System.Windows.Forms.DataGridView();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblLowStock = new System.Windows.Forms.Label();
            this.lblOutOfStock = new System.Windows.Forms.Label();
            this.lblCriticalStock = new System.Windows.Forms.Label();
            this.lblTotalProducts = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStockReport)).BeginInit();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Low Stock Report";
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlFilters.Controls.Add(this.lblBrand);
            this.pnlFilters.Controls.Add(this.cmbBrand);
            this.pnlFilters.Controls.Add(this.lblCategory);
            this.pnlFilters.Controls.Add(this.cmbCategory);
            this.pnlFilters.Controls.Add(this.lblStockStatus);
            this.pnlFilters.Controls.Add(this.chkLowStock);
            this.pnlFilters.Controls.Add(this.chkOutOfStock);
            this.pnlFilters.Controls.Add(this.chkCriticalStock);
            this.pnlFilters.Controls.Add(this.lblSearch);
            this.pnlFilters.Controls.Add(this.txtSearch);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilters.Location = new System.Drawing.Point(0, 60);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Padding = new System.Windows.Forms.Padding(20);
            this.pnlFilters.Size = new System.Drawing.Size(1200, 120);
            this.pnlFilters.TabIndex = 1;
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBrand.Location = new System.Drawing.Point(20, 20);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(44, 15);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Brand:";
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(20, 40);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(120, 23);
            this.cmbBrand.TabIndex = 1;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCategory.Location = new System.Drawing.Point(160, 20);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(64, 15);
            this.lblCategory.TabIndex = 2;
            this.lblCategory.Text = "Category:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(160, 40);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(120, 23);
            this.cmbCategory.TabIndex = 3;
            // 
            // lblStockStatus
            // 
            this.lblStockStatus.AutoSize = true;
            this.lblStockStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStockStatus.Location = new System.Drawing.Point(300, 20);
            this.lblStockStatus.Name = "lblStockStatus";
            this.lblStockStatus.Size = new System.Drawing.Size(80, 15);
            this.lblStockStatus.TabIndex = 4;
            this.lblStockStatus.Text = "Stock Status:";
            // 
            // chkLowStock
            // 
            this.chkLowStock.AutoSize = true;
            this.chkLowStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkLowStock.Location = new System.Drawing.Point(300, 40);
            this.chkLowStock.Name = "chkLowStock";
            this.chkLowStock.Size = new System.Drawing.Size(80, 19);
            this.chkLowStock.TabIndex = 5;
            this.chkLowStock.Text = "Low Stock";
            this.chkLowStock.UseVisualStyleBackColor = true;
            // 
            // chkOutOfStock
            // 
            this.chkOutOfStock.AutoSize = true;
            this.chkOutOfStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkOutOfStock.Location = new System.Drawing.Point(400, 40);
            this.chkOutOfStock.Name = "chkOutOfStock";
            this.chkOutOfStock.Size = new System.Drawing.Size(95, 19);
            this.chkOutOfStock.TabIndex = 6;
            this.chkOutOfStock.Text = "Out of Stock";
            this.chkOutOfStock.UseVisualStyleBackColor = true;
            // 
            // chkCriticalStock
            // 
            this.chkCriticalStock.AutoSize = true;
            this.chkCriticalStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkCriticalStock.Location = new System.Drawing.Point(520, 40);
            this.chkCriticalStock.Name = "chkCriticalStock";
            this.chkCriticalStock.Size = new System.Drawing.Size(90, 19);
            this.chkCriticalStock.TabIndex = 7;
            this.chkCriticalStock.Text = "Critical Stock";
            this.chkCriticalStock.UseVisualStyleBackColor = true;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSearch.Location = new System.Drawing.Point(640, 20);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(46, 15);
            this.lblSearch.TabIndex = 8;
            this.lblSearch.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.Location = new System.Drawing.Point(640, 40);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 23);
            this.txtSearch.TabIndex = 9;
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlButtons.Controls.Add(this.btnGenerateReport);
            this.pnlButtons.Controls.Add(this.btnCreatePurchaseOrder);
            this.pnlButtons.Controls.Add(this.btnExportPDF);
            this.pnlButtons.Controls.Add(this.btnExportExcel);
            this.pnlButtons.Controls.Add(this.btnPrint);
            this.pnlButtons.Controls.Add(this.btnClear);
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 180);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlButtons.Size = new System.Drawing.Size(1200, 60);
            this.pnlButtons.TabIndex = 2;
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnGenerateReport.FlatAppearance.BorderSize = 0;
            this.btnGenerateReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateReport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGenerateReport.ForeColor = System.Drawing.Color.White;
            this.btnGenerateReport.Location = new System.Drawing.Point(20, 15);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(120, 30);
            this.btnGenerateReport.TabIndex = 0;
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = false;
            // 
            // btnCreatePurchaseOrder
            // 
            this.btnCreatePurchaseOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnCreatePurchaseOrder.FlatAppearance.BorderSize = 0;
            this.btnCreatePurchaseOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreatePurchaseOrder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCreatePurchaseOrder.ForeColor = System.Drawing.Color.White;
            this.btnCreatePurchaseOrder.Location = new System.Drawing.Point(160, 15);
            this.btnCreatePurchaseOrder.Name = "btnCreatePurchaseOrder";
            this.btnCreatePurchaseOrder.Size = new System.Drawing.Size(150, 30);
            this.btnCreatePurchaseOrder.TabIndex = 1;
            this.btnCreatePurchaseOrder.Text = "Create Purchase Order";
            this.btnCreatePurchaseOrder.UseVisualStyleBackColor = false;
            // 
            // btnExportPDF
            // 
            this.btnExportPDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnExportPDF.FlatAppearance.BorderSize = 0;
            this.btnExportPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportPDF.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExportPDF.ForeColor = System.Drawing.Color.White;
            this.btnExportPDF.Location = new System.Drawing.Point(330, 15);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(100, 30);
            this.btnExportPDF.TabIndex = 2;
            this.btnExportPDF.Text = "Export PDF";
            this.btnExportPDF.UseVisualStyleBackColor = false;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnExportExcel.FlatAppearance.BorderSize = 0;
            this.btnExportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportExcel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnExportExcel.ForeColor = System.Drawing.Color.White;
            this.btnExportExcel.Location = new System.Drawing.Point(450, 15);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(100, 30);
            this.btnExportExcel.TabIndex = 3;
            this.btnExportExcel.Text = "Export Excel";
            this.btnExportExcel.UseVisualStyleBackColor = false;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(570, 15);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(80, 30);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(670, 15);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 30);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(770, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // pnlData
            // 
            this.pnlData.Controls.Add(this.dgvLowStockReport);
            this.pnlData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlData.Location = new System.Drawing.Point(0, 240);
            this.pnlData.Name = "pnlData";
            this.pnlData.Padding = new System.Windows.Forms.Padding(20);
            this.pnlData.Size = new System.Drawing.Size(1200, 400);
            this.pnlData.TabIndex = 3;
            // 
            // dgvLowStockReport
            // 
            this.dgvLowStockReport.AllowUserToAddRows = false;
            this.dgvLowStockReport.AllowUserToDeleteRows = false;
            this.dgvLowStockReport.BackgroundColor = System.Drawing.Color.White;
            this.dgvLowStockReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLowStockReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLowStockReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLowStockReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.dgvLowStockReport.Location = new System.Drawing.Point(20, 20);
            this.dgvLowStockReport.Name = "dgvLowStockReport";
            this.dgvLowStockReport.ReadOnly = true;
            this.dgvLowStockReport.RowHeadersVisible = false;
            this.dgvLowStockReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLowStockReport.Size = new System.Drawing.Size(1160, 360);
            this.dgvLowStockReport.TabIndex = 0;
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlSummary.Controls.Add(this.lblLowStock);
            this.pnlSummary.Controls.Add(this.lblOutOfStock);
            this.pnlSummary.Controls.Add(this.lblCriticalStock);
            this.pnlSummary.Controls.Add(this.lblTotalProducts);
            this.pnlSummary.Controls.Add(this.lblTotalValue);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSummary.Location = new System.Drawing.Point(0, 640);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlSummary.Size = new System.Drawing.Size(1200, 60);
            this.pnlSummary.TabIndex = 4;
            // 
            // lblLowStock
            // 
            this.lblLowStock.AutoSize = true;
            this.lblLowStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLowStock.Location = new System.Drawing.Point(20, 20);
            this.lblLowStock.Name = "lblLowStock";
            this.lblLowStock.Size = new System.Drawing.Size(100, 15);
            this.lblLowStock.TabIndex = 0;
            this.lblLowStock.Text = "Low Stock: 0";
            // 
            // lblOutOfStock
            // 
            this.lblOutOfStock.AutoSize = true;
            this.lblOutOfStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOutOfStock.Location = new System.Drawing.Point(160, 20);
            this.lblOutOfStock.Name = "lblOutOfStock";
            this.lblOutOfStock.Size = new System.Drawing.Size(120, 15);
            this.lblOutOfStock.TabIndex = 1;
            this.lblOutOfStock.Text = "Out of Stock: 0";
            // 
            // lblCriticalStock
            // 
            this.lblCriticalStock.AutoSize = true;
            this.lblCriticalStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCriticalStock.Location = new System.Drawing.Point(320, 20);
            this.lblCriticalStock.Name = "lblCriticalStock";
            this.lblCriticalStock.Size = new System.Drawing.Size(120, 15);
            this.lblCriticalStock.TabIndex = 2;
            this.lblCriticalStock.Text = "Critical Stock: 0";
            // 
            // lblTotalProducts
            // 
            this.lblTotalProducts.AutoSize = true;
            this.lblTotalProducts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalProducts.Location = new System.Drawing.Point(480, 20);
            this.lblTotalProducts.Name = "lblTotalProducts";
            this.lblTotalProducts.Size = new System.Drawing.Size(120, 15);
            this.lblTotalProducts.TabIndex = 3;
            this.lblTotalProducts.Text = "Total Products: 0";
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.Location = new System.Drawing.Point(640, 20);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(100, 15);
            this.lblTotalValue.TabIndex = 4;
            this.lblTotalValue.Text = "Total Value: 0.00";
            // 
            // LowStockReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.pnlData);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlFilters);
            this.Controls.Add(this.pnlHeader);
            this.Name = "LowStockReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Low Stock Report";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStockReport)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblStockStatus;
        private System.Windows.Forms.CheckBox chkLowStock;
        private System.Windows.Forms.CheckBox chkOutOfStock;
        private System.Windows.Forms.CheckBox chkCriticalStock;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.Button btnCreatePurchaseOrder;
        private System.Windows.Forms.Button btnExportPDF;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlData;
        private System.Windows.Forms.DataGridView dgvLowStockReport;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblLowStock;
        private System.Windows.Forms.Label lblOutOfStock;
        private System.Windows.Forms.Label lblCriticalStock;
        private System.Windows.Forms.Label lblTotalProducts;
        private System.Windows.Forms.Label lblTotalValue;
    }
}