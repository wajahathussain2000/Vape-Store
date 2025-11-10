namespace Vape_Store
{
    partial class ThermalInvoiceForm
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
            this.btnLoadSale = new System.Windows.Forms.Button();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.btnPreviewInvoice = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cmbInvoiceNumber = new System.Windows.Forms.ComboBox();
            this.lblSaleInfo = new System.Windows.Forms.Label();
            this.lblInvoiceNumber = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLoadSale
            // 
            this.btnLoadSale.Location = new System.Drawing.Point(12, 12);
            this.btnLoadSale.Name = "btnLoadSale";
            this.btnLoadSale.Size = new System.Drawing.Size(100, 30);
            this.btnLoadSale.TabIndex = 0;
            this.btnLoadSale.Text = "Load Sale";
            this.btnLoadSale.UseVisualStyleBackColor = true;
            // 
            // btnPrintInvoice
            // 
            this.btnPrintInvoice.Location = new System.Drawing.Point(118, 12);
            this.btnPrintInvoice.Name = "btnPrintInvoice";
            this.btnPrintInvoice.Size = new System.Drawing.Size(100, 30);
            this.btnPrintInvoice.TabIndex = 1;
            this.btnPrintInvoice.Text = "Print Invoice";
            this.btnPrintInvoice.UseVisualStyleBackColor = true;
            // 
            // btnPreviewInvoice
            // 
            this.btnPreviewInvoice.Location = new System.Drawing.Point(224, 12);
            this.btnPreviewInvoice.Name = "btnPreviewInvoice";
            this.btnPreviewInvoice.Size = new System.Drawing.Size(100, 30);
            this.btnPreviewInvoice.TabIndex = 2;
            this.btnPreviewInvoice.Text = "Preview Invoice";
            this.btnPreviewInvoice.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(330, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // cmbInvoiceNumber
            // 
            this.cmbInvoiceNumber.Location = new System.Drawing.Point(12, 60);
            this.cmbInvoiceNumber.Name = "cmbInvoiceNumber";
            this.cmbInvoiceNumber.Size = new System.Drawing.Size(200, 21);
            this.cmbInvoiceNumber.TabIndex = 4;
            this.cmbInvoiceNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            // 
            // lblSaleInfo
            // 
            this.lblSaleInfo.AutoSize = true;
            this.lblSaleInfo.Location = new System.Drawing.Point(12, 100);
            this.lblSaleInfo.Name = "lblSaleInfo";
            this.lblSaleInfo.Size = new System.Drawing.Size(50, 13);
            this.lblSaleInfo.TabIndex = 5;
            this.lblSaleInfo.Text = "Sale Info:";
            // 
            // lblInvoiceNumber
            // 
            this.lblInvoiceNumber.AutoSize = true;
            this.lblInvoiceNumber.Location = new System.Drawing.Point(12, 120);
            this.lblInvoiceNumber.Name = "lblInvoiceNumber";
            this.lblInvoiceNumber.Size = new System.Drawing.Size(85, 13);
            this.lblInvoiceNumber.TabIndex = 6;
            this.lblInvoiceNumber.Text = "Invoice Number:";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(12, 140);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(33, 13);
            this.lblDate.TabIndex = 7;
            this.lblDate.Text = "Date:";
            // 
            // lblSupplier
            // 
            this.lblSupplier.AutoSize = true;
            this.lblSupplier.Location = new System.Drawing.Point(12, 160);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(48, 13);
            this.lblSupplier.TabIndex = 8;
            this.lblSupplier.Text = "Customer:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(12, 180);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(34, 13);
            this.lblTotal.TabIndex = 9;
            this.lblTotal.Text = "Total:";
            // 
            // ThermalInvoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblSupplier);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblInvoiceNumber);
            this.Controls.Add(this.lblSaleInfo);
            this.Controls.Add(this.cmbInvoiceNumber);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPreviewInvoice);
            this.Controls.Add(this.btnPrintInvoice);
            this.Controls.Add(this.btnLoadSale);
            this.Name = "ThermalInvoiceForm";
            this.Text = "Thermal Invoice";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnLoadSale;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.Button btnPreviewInvoice;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cmbInvoiceNumber;
        private System.Windows.Forms.Label lblSaleInfo;
        private System.Windows.Forms.Label lblInvoiceNumber;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.Label lblTotal;
    }
}