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
            this.txtInvoiceNumber = new System.Windows.Forms.TextBox();
            this.lblSaleInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
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
            // txtInvoiceNumber
            // 
            this.txtInvoiceNumber.Location = new System.Drawing.Point(12, 60);
            this.txtInvoiceNumber.Name = "txtInvoiceNumber";
            this.txtInvoiceNumber.Size = new System.Drawing.Size(200, 20);
            this.txtInvoiceNumber.TabIndex = 4;
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
            // ThermalInvoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.lblSaleInfo);
            this.Controls.Add(this.txtInvoiceNumber);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPreviewInvoice);
            this.Controls.Add(this.btnPrintInvoice);
            this.Controls.Add(this.btnLoadSale);
            this.Name = "ThermalInvoiceForm";
            this.Text = "Thermal Invoice";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnLoadSale;
        private System.Windows.Forms.Button btnPrintInvoice;
        private System.Windows.Forms.Button btnPreviewInvoice;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtInvoiceNumber;
        private System.Windows.Forms.Label lblSaleInfo;
    }
}