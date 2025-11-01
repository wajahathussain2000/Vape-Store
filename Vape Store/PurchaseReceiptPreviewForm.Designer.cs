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
            
            // Form properties for A4 size
            this.Text = "Purchase Receipt - Vape Store";
            this.Size = new System.Drawing.Size(800, 1000); // A4 proportions
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);

            // Create main container panel
            var mainPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(40, 40, 40, 40)
            };
            this.Controls.Add(mainPanel);

            // Create receipt content panel
            var receiptPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.None
            };
            mainPanel.Controls.Add(receiptPanel);

            // Simple buttons panel
            var buttonPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 60,
                BackColor = System.Drawing.Color.LightGray
            };
            mainPanel.Controls.Add(buttonPanel);

            // Simple Print button
            var btnPrint = new System.Windows.Forms.Button
            {
                Text = "Print Receipt",
                Size = new System.Drawing.Size(120, 35),
                Location = new System.Drawing.Point(50, 12),
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };
            buttonPanel.Controls.Add(btnPrint);

            // Simple Save PDF button
            var btnSavePDF = new System.Windows.Forms.Button
            {
                Text = "Save PDF",
                Size = new System.Drawing.Size(120, 35),
                Location = new System.Drawing.Point(200, 12),
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };
            buttonPanel.Controls.Add(btnSavePDF);

            // Simple Close button
            var btnClose = new System.Windows.Forms.Button
            {
                Text = "Close",
                Size = new System.Drawing.Size(80, 35),
                Location = new System.Drawing.Point(650, 12),
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
            };
            buttonPanel.Controls.Add(btnClose);

            this.ResumeLayout(false);
        }

        #endregion
    }
}

