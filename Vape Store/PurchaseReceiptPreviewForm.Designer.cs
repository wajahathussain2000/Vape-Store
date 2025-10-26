namespace Vape_Store
{
    partial class PurchaseReceiptPreviewForm
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                _receiptService?.Dispose();
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
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Purchase Receipt Preview - Vape Store";
            this.Size = new System.Drawing.Size(450, 700);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create preview panel
            var previewPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            };
            this.Controls.Add(previewPanel);

            // Create buttons panel
            var buttonPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 50,
                BackColor = System.Drawing.Color.LightGray
            };
            this.Controls.Add(buttonPanel);

            // Print button
            var btnPrint = new System.Windows.Forms.Button
            {
                Text = "Print Receipt",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(20, 10),
                BackColor = System.Drawing.Color.Green,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };
            btnPrint.Click += BtnPrint_Click;
            buttonPanel.Controls.Add(btnPrint);

            // Print Direct button
            var btnPrintDirect = new System.Windows.Forms.Button
            {
                Text = "Print Direct",
                Size = new System.Drawing.Size(100, 30),
                Location = new System.Drawing.Point(130, 10),
                BackColor = System.Drawing.Color.Blue,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };
            btnPrintDirect.Click += BtnPrintDirect_Click;
            buttonPanel.Controls.Add(btnPrintDirect);

            // Close button
            var btnClose = new System.Windows.Forms.Button
            {
                Text = "Close",
                Size = new System.Drawing.Size(80, 30),
                Location = new System.Drawing.Point(240, 10),
                BackColor = System.Drawing.Color.Gray,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };
            btnClose.Click += BtnClose_Click;
            buttonPanel.Controls.Add(btnClose);

            this.ResumeLayout(false);
        }

        #endregion
    }
}

