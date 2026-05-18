namespace Vape_Store
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2ShadowForm1 = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);
            this.panelHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabControl = new Guna.UI2.WinForms.Guna2TabControl();
            this.tabStore = new System.Windows.Forms.TabPage();
            this.tabPrinting = new System.Windows.Forms.TabPage();
            
            // Store Info Controls
            this.txtStoreName = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtStoreContact = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtStoreAddress = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtStoreEmail = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtReceiptFooter = new Guna.UI2.WinForms.Guna2TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            
            // Printing Controls
            this.txtDefaultLabel = new Guna.UI2.WinForms.Guna2TextBox();
            this.numWidth = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.numHeight = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.numGap = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.numMarginLeft = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.numMarginTop = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.numPaperWidth = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.chkIsThermal = new Guna.UI2.WinForms.Guna2CheckBox();
            
            // New Printer selection controls
            this.cmbThermalPrinter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cmbBarcodePrinter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.chkDirectPrint = new Guna.UI2.WinForms.Guna2CheckBox();
            
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            
            this.panelHeader.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabStore.SuspendLayout();
            this.tabPrinting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPaperWidth)).BeginInit();
            this.SuspendLayout();

            // guna2Elipse1
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;

            // panelHeader
            this.panelHeader.Controls.Add(this.btnClose);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.FillColor = System.Drawing.Color.FromArgb(28, 44, 84);
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(600, 60);

            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.FillColor = System.Drawing.Color.Transparent;
            this.btnClose.IconColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(555, 10);
            this.btnClose.Size = new System.Drawing.Size(35, 35);

            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Text = "Application Settings";

            // tabControl
            this.tabControl.Controls.Add(this.tabStore);
            this.tabControl.Controls.Add(this.tabPrinting);
            this.tabControl.ItemSize = new System.Drawing.Size(180, 40);
            this.tabControl.Location = new System.Drawing.Point(12, 70);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(576, 520);
            this.tabControl.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.tabControl.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(40, 52, 70);
            this.tabControl.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabControl.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.tabControl.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.tabControl.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(33, 42, 57);
            this.tabControl.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabControl.TabButtonIdleState.ForeColor = System.Drawing.Color.White;
            this.tabControl.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.tabControl.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(40, 52, 80);
            this.tabControl.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.tabControl.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.tabControl.TabButtonSize = new System.Drawing.Size(180, 40);

            // tabStore
            this.tabStore.Controls.Add(this.label1);
            this.tabStore.Controls.Add(this.txtStoreName);
            this.tabStore.Controls.Add(this.label2);
            this.tabStore.Controls.Add(this.txtStoreContact);
            this.tabStore.Controls.Add(this.label3);
            this.tabStore.Controls.Add(this.txtStoreAddress);
            this.tabStore.Controls.Add(this.label5);
            this.tabStore.Controls.Add(this.txtStoreEmail);
            this.tabStore.Controls.Add(this.label4);
            this.tabStore.Controls.Add(this.txtReceiptFooter);
            this.tabStore.Location = new System.Drawing.Point(184, 4);
            this.tabStore.Name = "tabStore";
            this.tabStore.Size = new System.Drawing.Size(388, 512);
            this.tabStore.Text = "Store Information";
            this.tabStore.BackColor = System.Drawing.Color.FromArgb(29, 37, 49);

            this.label1.Text = "Store Name";
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.AutoSize = true;
            this.txtStoreName.Location = new System.Drawing.Point(15, 35);
            this.txtStoreName.Size = new System.Drawing.Size(350, 36);
            this.txtStoreName.BorderRadius = 5;

            this.label2.Text = "Contact Number";
            this.label2.Location = new System.Drawing.Point(15, 80);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.AutoSize = true;
            this.txtStoreContact.Location = new System.Drawing.Point(15, 100);
            this.txtStoreContact.Size = new System.Drawing.Size(350, 36);
            this.txtStoreContact.BorderRadius = 5;

            this.label3.Text = "Store Address";
            this.label3.Location = new System.Drawing.Point(15, 145);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.AutoSize = true;
            this.txtStoreAddress.Location = new System.Drawing.Point(15, 165);
            this.txtStoreAddress.Size = new System.Drawing.Size(350, 36);
            this.txtStoreAddress.BorderRadius = 5;

            this.label5.Text = "Store Email";
            this.label5.Location = new System.Drawing.Point(15, 210);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.AutoSize = true;
            this.txtStoreEmail.Location = new System.Drawing.Point(15, 230);
            this.txtStoreEmail.Size = new System.Drawing.Size(350, 36);
            this.txtStoreEmail.BorderRadius = 5;

            this.label4.Text = "Receipt Footer";
            this.label4.Location = new System.Drawing.Point(15, 275);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.AutoSize = true;
            this.txtReceiptFooter.Location = new System.Drawing.Point(15, 295);
            this.txtReceiptFooter.Size = new System.Drawing.Size(350, 80);
            this.txtReceiptFooter.Multiline = true;
            this.txtReceiptFooter.BorderRadius = 5;

            // tabPrinting
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Default Barcode Label", Location = new System.Drawing.Point(15, 15), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.txtDefaultLabel);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Barcode Width", Location = new System.Drawing.Point(15, 80), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.numWidth);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Barcode Height", Location = new System.Drawing.Point(145, 80), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.numHeight);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Gap (mm)", Location = new System.Drawing.Point(15, 145), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.numGap);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Left Margin (mm)", Location = new System.Drawing.Point(15, 210), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.numMarginLeft);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Top Margin (mm)", Location = new System.Drawing.Point(145, 210), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.numMarginTop);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Receipt Paper Width", Location = new System.Drawing.Point(15, 275), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.numPaperWidth);
            this.tabPrinting.Controls.Add(this.chkIsThermal);
            
            // Printer selection on page 2
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Default Thermal Printer", Location = new System.Drawing.Point(15, 340), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.cmbThermalPrinter);
            this.tabPrinting.Controls.Add(new System.Windows.Forms.Label { Text = "Default Barcode Printer", Location = new System.Drawing.Point(15, 400), AutoSize = true, ForeColor = System.Drawing.Color.White, BackColor = System.Drawing.Color.Transparent });
            this.tabPrinting.Controls.Add(this.cmbBarcodePrinter);
            this.tabPrinting.Controls.Add(this.chkDirectPrint);
            
            this.tabPrinting.Location = new System.Drawing.Point(184, 4);
            this.tabPrinting.Name = "tabPrinting";
            this.tabPrinting.Text = "Printing Defaults";
            this.tabPrinting.Size = new System.Drawing.Size(388, 512);
            this.tabPrinting.BackColor = System.Drawing.Color.FromArgb(29, 37, 49);

            this.txtDefaultLabel.Location = new System.Drawing.Point(15, 35);
            this.txtDefaultLabel.Size = new System.Drawing.Size(350, 36);
            this.txtDefaultLabel.BorderRadius = 5;

            this.numWidth.Location = new System.Drawing.Point(15, 100);
            this.numWidth.Size = new System.Drawing.Size(110, 36);
            this.numWidth.Maximum = 800;
            this.numWidth.BorderRadius = 5;

            this.numHeight.Location = new System.Drawing.Point(145, 100);
            this.numHeight.Size = new System.Drawing.Size(110, 36);
            this.numHeight.Maximum = 800;
            this.numHeight.BorderRadius = 5;

            this.numGap.Location = new System.Drawing.Point(15, 165);
            this.numGap.Size = new System.Drawing.Size(110, 36);
            this.numGap.DecimalPlaces = 1;
            this.numGap.BorderRadius = 5;

            this.numMarginLeft.Location = new System.Drawing.Point(15, 230);
            this.numMarginLeft.Size = new System.Drawing.Size(110, 36);
            this.numMarginLeft.DecimalPlaces = 1;
            this.numMarginLeft.BorderRadius = 5;

            this.numMarginTop.Location = new System.Drawing.Point(145, 230);
            this.numMarginTop.Size = new System.Drawing.Size(110, 36);
            this.numMarginTop.DecimalPlaces = 1;
            this.numMarginTop.BorderRadius = 5;

            this.numPaperWidth.Location = new System.Drawing.Point(15, 295);
            this.numPaperWidth.Size = new System.Drawing.Size(150, 36);
            this.numPaperWidth.Maximum = 1000;
            this.numPaperWidth.BorderRadius = 5;

            this.chkIsThermal.Text = "Thermal Roll Mode";
            this.chkIsThermal.Location = new System.Drawing.Point(180, 295);
            this.chkIsThermal.Size = new System.Drawing.Size(150, 36);
            this.chkIsThermal.Checked = true;

            this.cmbThermalPrinter.Location = new System.Drawing.Point(15, 360);
            this.cmbThermalPrinter.Size = new System.Drawing.Size(350, 36);
            this.cmbThermalPrinter.BorderRadius = 5;

            this.cmbBarcodePrinter.Location = new System.Drawing.Point(15, 420);
            this.cmbBarcodePrinter.Size = new System.Drawing.Size(350, 36);
            this.cmbBarcodePrinter.BorderRadius = 5;

            this.chkDirectPrint.Text = "Direct Print Receipt (Skip Dialog)";
            this.chkDirectPrint.Location = new System.Drawing.Point(15, 470);
            this.chkDirectPrint.Size = new System.Drawing.Size(250, 36);

            // btnSave
            this.btnSave.BorderRadius = 10;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(28, 44, 84);
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(200, 605);
            this.btnSave.Size = new System.Drawing.Size(200, 45);
            this.btnSave.Text = "SAVE SETTINGS";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // SettingsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 670);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabStore.ResumeLayout(false);
            this.tabStore.PerformLayout();
            this.tabPrinting.ResumeLayout(false);
            this.tabPrinting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPaperWidth)).EndInit();
            this.ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm1;
        private Guna.UI2.WinForms.Guna2Panel panelHeader;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TabControl tabControl;
        private System.Windows.Forms.TabPage tabStore;
        private System.Windows.Forms.TabPage tabPrinting;
        private Guna.UI2.WinForms.Guna2TextBox txtStoreName;
        private Guna.UI2.WinForms.Guna2TextBox txtStoreContact;
        private Guna.UI2.WinForms.Guna2TextBox txtStoreAddress;
        private Guna.UI2.WinForms.Guna2TextBox txtReceiptFooter;
        private Guna.UI2.WinForms.Guna2TextBox txtStoreEmail;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        
        // Printing fields
        private Guna.UI2.WinForms.Guna2TextBox txtDefaultLabel;
        private Guna.UI2.WinForms.Guna2NumericUpDown numWidth;
        private Guna.UI2.WinForms.Guna2NumericUpDown numHeight;
        private Guna.UI2.WinForms.Guna2NumericUpDown numGap;
        private Guna.UI2.WinForms.Guna2NumericUpDown numMarginLeft;
        private Guna.UI2.WinForms.Guna2NumericUpDown numMarginTop;
        private Guna.UI2.WinForms.Guna2NumericUpDown numPaperWidth;
        private Guna.UI2.WinForms.Guna2CheckBox chkIsThermal;
        
        // Multi-printer fields
        private Guna.UI2.WinForms.Guna2ComboBox cmbThermalPrinter;
        private Guna.UI2.WinForms.Guna2ComboBox cmbBarcodePrinter;
        private Guna.UI2.WinForms.Guna2CheckBox chkDirectPrint;
    }
}
