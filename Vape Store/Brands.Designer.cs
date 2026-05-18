namespace Vape_Store
{
    partial class Brands
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
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.categoryInputGroup = new Guna.UI2.WinForms.Guna2GroupBox();
            this.categoryNameLabel = new System.Windows.Forms.Label();
            this.txtCategoryName = new Guna.UI2.WinForms.Guna2TextBox();
            this.categoryDescLabel = new System.Windows.Forms.Label();
            this.txtCategoryDesc = new Guna.UI2.WinForms.Guna2TextBox();
            this.Savebtn = new Guna.UI2.WinForms.Guna2Button();
            this.Updatebtn = new Guna.UI2.WinForms.Guna2Button();
            this.Deletebtn = new Guna.UI2.WinForms.Guna2Button();
            this.addCategoryBtn = new Guna.UI2.WinForms.Guna2Button();
            this.categoryGridGroup = new Guna.UI2.WinForms.Guna2GroupBox();
            this.dgvBrands = new Guna.UI2.WinForms.Guna2DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.panel4.SuspendLayout();
            this.categoryInputGroup.SuspendLayout();
            this.categoryGridGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBrands)).BeginInit();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.panel4.Controls.Add(this.lblTitle);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(901, 80);
            this.panel4.TabIndex = 43;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(124, 45);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Brands";
            // 
            // categoryInputGroup
            // 
            this.categoryInputGroup.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.categoryInputGroup.BorderRadius = 5;
            this.categoryInputGroup.Controls.Add(this.categoryNameLabel);
            this.categoryInputGroup.Controls.Add(this.txtCategoryName);
            this.categoryInputGroup.Controls.Add(this.categoryDescLabel);
            this.categoryInputGroup.Controls.Add(this.txtCategoryDesc);
            this.categoryInputGroup.Controls.Add(this.Savebtn);
            this.categoryInputGroup.Controls.Add(this.Updatebtn);
            this.categoryInputGroup.Controls.Add(this.Deletebtn);
            this.categoryInputGroup.Controls.Add(this.addCategoryBtn);
            this.categoryInputGroup.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.categoryInputGroup.FillColor = System.Drawing.Color.White;
            this.categoryInputGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.categoryInputGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.categoryInputGroup.Location = new System.Drawing.Point(13, 98);
            this.categoryInputGroup.Margin = new System.Windows.Forms.Padding(4);
            this.categoryInputGroup.Name = "categoryInputGroup";
            this.categoryInputGroup.Size = new System.Drawing.Size(879, 430);
            this.categoryInputGroup.TabIndex = 41;
            this.categoryInputGroup.Text = "Add New Brand";
            this.categoryInputGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // categoryNameLabel
            // 
            this.categoryNameLabel.AutoSize = true;
            this.categoryNameLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryNameLabel.Location = new System.Drawing.Point(30, 52);
            this.categoryNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.categoryNameLabel.Name = "categoryNameLabel";
            this.categoryNameLabel.Size = new System.Drawing.Size(135, 28);
            this.categoryNameLabel.TabIndex = 0;
            this.categoryNameLabel.Text = "Brand Name:";
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.BorderRadius = 5;
            this.txtCategoryName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCategoryName.DefaultText = "";
            this.txtCategoryName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCategoryName.ForeColor = System.Drawing.Color.Black;
            this.txtCategoryName.Location = new System.Drawing.Point(30, 85);
            this.txtCategoryName.Margin = new System.Windows.Forms.Padding(4);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(793, 37);
            this.txtCategoryName.TabIndex = 1;
            // 
            // categoryDescLabel
            // 
            this.categoryDescLabel.AutoSize = true;
            this.categoryDescLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryDescLabel.Location = new System.Drawing.Point(30, 154);
            this.categoryDescLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.categoryDescLabel.Name = "categoryDescLabel";
            this.categoryDescLabel.Size = new System.Drawing.Size(126, 28);
            this.categoryDescLabel.TabIndex = 2;
            this.categoryDescLabel.Text = "Description:";
            // 
            // txtCategoryDesc
            // 
            this.txtCategoryDesc.BorderRadius = 5;
            this.txtCategoryDesc.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCategoryDesc.DefaultText = "";
            this.txtCategoryDesc.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCategoryDesc.ForeColor = System.Drawing.Color.Black;
            this.txtCategoryDesc.Location = new System.Drawing.Point(30, 192);
            this.txtCategoryDesc.Margin = new System.Windows.Forms.Padding(4);
            this.txtCategoryDesc.Multiline = true;
            this.txtCategoryDesc.Name = "txtCategoryDesc";
            this.txtCategoryDesc.Size = new System.Drawing.Size(793, 121);
            this.txtCategoryDesc.TabIndex = 3;
            // 
            // Savebtn
            // 
            this.Savebtn.BorderRadius = 5;
            this.Savebtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.Savebtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Savebtn.ForeColor = System.Drawing.Color.White;
            this.Savebtn.Location = new System.Drawing.Point(229, 353);
            this.Savebtn.Margin = new System.Windows.Forms.Padding(4);
            this.Savebtn.Name = "Savebtn";
            this.Savebtn.Size = new System.Drawing.Size(181, 53);
            this.Savebtn.TabIndex = 4;
            this.Savebtn.Text = "Save";
            // 
            // Updatebtn
            // 
            this.Updatebtn.BorderRadius = 5;
            this.Updatebtn.Enabled = false;
            this.Updatebtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.Updatebtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Updatebtn.ForeColor = System.Drawing.Color.White;
            this.Updatebtn.Location = new System.Drawing.Point(430, 353);
            this.Updatebtn.Margin = new System.Windows.Forms.Padding(4);
            this.Updatebtn.Name = "Updatebtn";
            this.Updatebtn.Size = new System.Drawing.Size(181, 53);
            this.Updatebtn.TabIndex = 5;
            this.Updatebtn.Text = "Update";
            // 
            // Deletebtn
            // 
            this.Deletebtn.BorderRadius = 5;
            this.Deletebtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.Deletebtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Deletebtn.ForeColor = System.Drawing.Color.White;
            this.Deletebtn.Location = new System.Drawing.Point(631, 353);
            this.Deletebtn.Margin = new System.Windows.Forms.Padding(4);
            this.Deletebtn.Name = "Deletebtn";
            this.Deletebtn.Size = new System.Drawing.Size(181, 53);
            this.Deletebtn.TabIndex = 4;
            this.Deletebtn.Text = "Delete";
            // 
            // addCategoryBtn
            // 
            this.addCategoryBtn.BorderRadius = 5;
            this.addCategoryBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(181)))), ((int)(((byte)(246)))));
            this.addCategoryBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.addCategoryBtn.ForeColor = System.Drawing.Color.White;
            this.addCategoryBtn.Location = new System.Drawing.Point(30, 353);
            this.addCategoryBtn.Margin = new System.Windows.Forms.Padding(4);
            this.addCategoryBtn.Name = "addCategoryBtn";
            this.addCategoryBtn.Size = new System.Drawing.Size(181, 53);
            this.addCategoryBtn.TabIndex = 4;
            this.addCategoryBtn.Text = "Add Brand";
            // 
            // categoryGridGroup
            // 
            this.categoryGridGroup.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.categoryGridGroup.BorderRadius = 5;
            this.categoryGridGroup.Controls.Add(this.label3);
            this.categoryGridGroup.Controls.Add(this.txtSearch);
            this.categoryGridGroup.Controls.Add(this.dgvBrands);
            this.categoryGridGroup.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.categoryGridGroup.FillColor = System.Drawing.Color.White;
            this.categoryGridGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.categoryGridGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.categoryGridGroup.Location = new System.Drawing.Point(13, 538);
            this.categoryGridGroup.Margin = new System.Windows.Forms.Padding(4);
            this.categoryGridGroup.Name = "categoryGridGroup";
            this.categoryGridGroup.Size = new System.Drawing.Size(879, 380);
            this.categoryGridGroup.TabIndex = 42;
            this.categoryGridGroup.Text = "Brands List";
            this.categoryGridGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dgvBrands
            // 
            this.dgvBrands.AllowUserToAddRows = false;
            this.dgvBrands.AllowUserToDeleteRows = false;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvBrands.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBrands.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBrands.BackgroundColor = System.Drawing.Color.White;
            this.dgvBrands.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBrands.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvBrands.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBrands.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBrands.ColumnHeadersHeight = 35;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBrands.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvBrands.EnableHeadersVisualStyles = false;
            this.dgvBrands.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvBrands.Location = new System.Drawing.Point(30, 85);
            this.dgvBrands.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBrands.Name = "dgvBrands";
            this.dgvBrands.ReadOnly = true;
            this.dgvBrands.RowHeadersVisible = false;
            this.dgvBrands.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBrands.Size = new System.Drawing.Size(810, 272);
            this.dgvBrands.TabIndex = 0;
            this.dgvBrands.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            this.dgvBrands.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvBrands.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvBrands.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(30, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 28);
            this.label3.TabIndex = 44;
            this.label3.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.BorderRadius = 5;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.ForeColor = System.Drawing.Color.Black;
            this.txtSearch.Location = new System.Drawing.Point(129, 37);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(711, 35);
            this.txtSearch.TabIndex = 43;
            this.txtSearch.PlaceholderText = "Search brands...";
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.Controls.Add(this.categoryInputGroup);
            this.panelMainContainer.Controls.Add(this.categoryGridGroup);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 80);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(20);
            this.panelMainContainer.Size = new System.Drawing.Size(901, 845);
            this.panelMainContainer.TabIndex = 44;
            // 
            // Brands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(901, 925);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.panel4);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "Brands";
            this.Text = "Vape Store - Brands Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.categoryInputGroup.ResumeLayout(false);
            this.categoryInputGroup.PerformLayout();
            this.categoryGridGroup.ResumeLayout(false);
            this.categoryGridGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBrands)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2GroupBox categoryInputGroup;
        private System.Windows.Forms.Label categoryNameLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtCategoryName;
        private System.Windows.Forms.Label categoryDescLabel;
        private Guna.UI2.WinForms.Guna2TextBox txtCategoryDesc;
        private Guna.UI2.WinForms.Guna2Button Savebtn;
        private Guna.UI2.WinForms.Guna2Button Updatebtn;
        private Guna.UI2.WinForms.Guna2Button Deletebtn;
        private Guna.UI2.WinForms.Guna2Button addCategoryBtn;
        private Guna.UI2.WinForms.Guna2GroupBox categoryGridGroup;
        private Guna.UI2.WinForms.Guna2DataGridView dgvBrands;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private System.Windows.Forms.Panel panelMainContainer;
    }
}
