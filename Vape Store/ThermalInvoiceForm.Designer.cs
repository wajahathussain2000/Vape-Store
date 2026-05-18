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
            this.pnlButtonContainer = new System.Windows.Forms.Panel();
            this.btnDownloadPDF = new System.Windows.Forms.Button();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.btnPreviewInvoice = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlSelection = new System.Windows.Forms.Panel();
            this.btnLoadSale = new System.Windows.Forms.Button();
            this.cmbInvoiceNumber = new System.Windows.Forms.ComboBox();
            this.lblSaleInfo = new System.Windows.Forms.Label();
            this.pnlReceiptContainer = new System.Windows.Forms.Panel();
            this.pnlButtonContainer.SuspendLayout();
            this.pnlSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtonContainer
            // 
            this.pnlButtonContainer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlButtonContainer.Controls.Add(this.btnDownloadPDF);
            this.pnlButtonContainer.Controls.Add(this.btnPrintInvoice);
            this.pnlButtonContainer.Controls.Add(this.btnPreviewInvoice);
            this.pnlButtonContainer.Controls.Add(this.btnClose);
            this.pnlButtonContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtonContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlButtonContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlButtonContainer.Name = "pnlButtonContainer";
            this.pnlButtonContainer.Size = new System.Drawing.Size(867, 74);
            this.pnlButtonContainer.TabIndex = 0;
            // 
            // btnDownloadPDF
            // 
            this.btnDownloadPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloadPDF.Location = new System.Drawing.Point(459, 15);
            this.btnDownloadPDF.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDownloadPDF.Name = "btnDownloadPDF";
            this.btnDownloadPDF.Size = new System.Drawing.Size(160, 43);
            this.btnDownloadPDF.TabIndex = 3;
            this.btnDownloadPDF.Text = "Download PDF";
            this.btnDownloadPDF.UseVisualStyleBackColor = true;
            // 
            // btnPrintInvoice
            // 
            this.btnPrintInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrintInvoice.Location = new System.Drawing.Point(16, 15);
            this.btnPrintInvoice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrintInvoice.Name = "btnPrintInvoice";
            this.btnPrintInvoice.Size = new System.Drawing.Size(133, 43);
            this.btnPrintInvoice.TabIndex = 0;
            this.btnPrintInvoice.Text = "Print";
            this.btnPrintInvoice.UseVisualStyleBackColor = true;
            // 
            // btnPreviewInvoice
            // 
            this.btnPreviewInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreviewInvoice.Location = new System.Drawing.Point(157, 15);
            this.btnPreviewInvoice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPreviewInvoice.Name = "btnPreviewInvoice";
            this.btnPreviewInvoice.Size = new System.Drawing.Size(133, 43);
            this.btnPreviewInvoice.TabIndex = 1;
            this.btnPreviewInvoice.Text = "Full A4 Preview";
            this.btnPreviewInvoice.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(717, 15);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(133, 43);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // pnlSelection
            // 
            this.pnlSelection.Controls.Add(this.btnLoadSale);
            this.pnlSelection.Controls.Add(this.cmbInvoiceNumber);
            this.pnlSelection.Controls.Add(this.lblSaleInfo);
            this.pnlSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSelection.Location = new System.Drawing.Point(0, 74);
            this.pnlSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlSelection.Name = "pnlSelection";
            this.pnlSelection.Size = new System.Drawing.Size(867, 98);
            this.pnlSelection.TabIndex = 1;
            // 
            // btnLoadSale
            // 
            this.btnLoadSale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadSale.Location = new System.Drawing.Point(291, 16);
            this.btnLoadSale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLoadSale.Name = "btnLoadSale";
            this.btnLoadSale.Size = new System.Drawing.Size(133, 31);
            this.btnLoadSale.TabIndex = 1;
            this.btnLoadSale.Text = "Load";
            this.btnLoadSale.UseVisualStyleBackColor = true;
            this.btnLoadSale.Click += new System.EventHandler(this.btnLoadSale_Click_1);
            // 
            // cmbInvoiceNumber
            // 
            this.cmbInvoiceNumber.FormattingEnabled = true;
            this.cmbInvoiceNumber.Location = new System.Drawing.Point(16, 18);
            this.cmbInvoiceNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbInvoiceNumber.Name = "cmbInvoiceNumber";
            this.cmbInvoiceNumber.Size = new System.Drawing.Size(265, 24);
            this.cmbInvoiceNumber.TabIndex = 0;
            // 
            // lblSaleInfo
            // 
            this.lblSaleInfo.AutoSize = true;
            this.lblSaleInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblSaleInfo.ForeColor = System.Drawing.Color.DimGray;
            this.lblSaleInfo.Location = new System.Drawing.Point(16, 55);
            this.lblSaleInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSaleInfo.Name = "lblSaleInfo";
            this.lblSaleInfo.Size = new System.Drawing.Size(228, 20);
            this.lblSaleInfo.TabIndex = 2;
            this.lblSaleInfo.Text = "Enter Invoice Number to Load Sale";
            // 
            // pnlReceiptContainer
            // 
            this.pnlReceiptContainer.BackColor = System.Drawing.Color.DarkGray;
            this.pnlReceiptContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlReceiptContainer.Location = new System.Drawing.Point(0, 172);
            this.pnlReceiptContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlReceiptContainer.Name = "pnlReceiptContainer";
            this.pnlReceiptContainer.Padding = new System.Windows.Forms.Padding(67, 25, 67, 25);
            this.pnlReceiptContainer.Size = new System.Drawing.Size(867, 690);
            this.pnlReceiptContainer.TabIndex = 2;
            // 
            // ThermalInvoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(867, 862);
            this.Controls.Add(this.pnlReceiptContainer);
            this.Controls.Add(this.pnlSelection);
            this.Controls.Add(this.pnlButtonContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(794, 851);
            this.Name = "ThermalInvoiceForm";
            this.Text = "Thermal Receipt Viewer";
            this.pnlButtonContainer.ResumeLayout(false);
            this.pnlSelection.ResumeLayout(false);
            this.pnlSelection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtonContainer;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.Button btnPreviewInvoice;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDownloadPDF;
        private System.Windows.Forms.Panel pnlSelection;
        private System.Windows.Forms.Button btnLoadSale;
        private System.Windows.Forms.ComboBox cmbInvoiceNumber;
        private System.Windows.Forms.Label lblSaleInfo;
        private System.Windows.Forms.Panel pnlReceiptContainer;
    }
}
