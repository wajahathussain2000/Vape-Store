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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.categoryInputGroup = new System.Windows.Forms.GroupBox();
            this.categoryNameLabel = new System.Windows.Forms.Label();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.categoryDescLabel = new System.Windows.Forms.Label();
            this.txtCategoryDesc = new System.Windows.Forms.TextBox();
            this.Savebtn = new System.Windows.Forms.Button();
            this.Updatebtn = new System.Windows.Forms.Button();
            this.Deletebtn = new System.Windows.Forms.Button();
            this.addCategoryBtn = new System.Windows.Forms.Button();
            this.categoryGridGroup = new System.Windows.Forms.GroupBox();
            this.dgvBrands = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel4.SuspendLayout();
            this.categoryInputGroup.SuspendLayout();
            this.categoryGridGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBrands)).BeginInit();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.SeaGreen;
            this.panel4.Controls.Add(this.lblTitle);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(901, 90);
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
            this.categoryInputGroup.BackColor = System.Drawing.Color.SeaGreen;
            this.categoryInputGroup.Controls.Add(this.categoryNameLabel);
            this.categoryInputGroup.Controls.Add(this.txtCategoryName);
            this.categoryInputGroup.Controls.Add(this.categoryDescLabel);
            this.categoryInputGroup.Controls.Add(this.txtCategoryDesc);
            this.categoryInputGroup.Controls.Add(this.Savebtn);
            this.categoryInputGroup.Controls.Add(this.Updatebtn);
            this.categoryInputGroup.Controls.Add(this.Deletebtn);
            this.categoryInputGroup.Controls.Add(this.addCategoryBtn);
            this.categoryInputGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryInputGroup.Location = new System.Drawing.Point(13, 98);
            this.categoryInputGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.categoryInputGroup.Name = "categoryInputGroup";
            this.categoryInputGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.categoryInputGroup.Size = new System.Drawing.Size(879, 430);
            this.categoryInputGroup.TabIndex = 41;
            this.categoryInputGroup.TabStop = false;
            this.categoryInputGroup.Text = "Add New Brand";
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
            this.txtCategoryName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCategoryName.Location = new System.Drawing.Point(30, 85);
            this.txtCategoryName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.txtCategoryDesc.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCategoryDesc.Location = new System.Drawing.Point(30, 192);
            this.txtCategoryDesc.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCategoryDesc.Multiline = true;
            this.txtCategoryDesc.Name = "txtCategoryDesc";
            this.txtCategoryDesc.Size = new System.Drawing.Size(793, 121);
            this.txtCategoryDesc.TabIndex = 3;
            // 
            // Savebtn
            // 
            this.Savebtn.BackColor = System.Drawing.Color.White;
            this.Savebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Savebtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Savebtn.ForeColor = System.Drawing.Color.Black;
            this.Savebtn.Location = new System.Drawing.Point(229, 353);
            this.Savebtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Savebtn.Name = "Savebtn";
            this.Savebtn.Size = new System.Drawing.Size(181, 53);
            this.Savebtn.TabIndex = 4;
            this.Savebtn.Text = "Save";
            this.Savebtn.UseVisualStyleBackColor = false;
            // 
            // Updatebtn
            // 
            this.Updatebtn.BackColor = System.Drawing.Color.White;
            this.Updatebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Updatebtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Updatebtn.ForeColor = System.Drawing.Color.Black;
            this.Updatebtn.Location = new System.Drawing.Point(430, 353);
            this.Updatebtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Updatebtn.Name = "Updatebtn";
            this.Updatebtn.Size = new System.Drawing.Size(181, 53);
            this.Updatebtn.TabIndex = 5;
            this.Updatebtn.Text = "Update";
            this.Updatebtn.UseVisualStyleBackColor = false;
            this.Updatebtn.Enabled = false;
            // 
            // Deletebtn
            // 
            this.Deletebtn.BackColor = System.Drawing.Color.White;
            this.Deletebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Deletebtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Deletebtn.ForeColor = System.Drawing.Color.Black;
            this.Deletebtn.Location = new System.Drawing.Point(631, 353);
            this.Deletebtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Deletebtn.Name = "Deletebtn";
            this.Deletebtn.Size = new System.Drawing.Size(181, 53);
            this.Deletebtn.TabIndex = 4;
            this.Deletebtn.Text = "Delete";
            this.Deletebtn.UseVisualStyleBackColor = false;
            // 
            // addCategoryBtn
            // 
            this.addCategoryBtn.BackColor = System.Drawing.Color.White;
            this.addCategoryBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addCategoryBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addCategoryBtn.ForeColor = System.Drawing.Color.Black;
            this.addCategoryBtn.Location = new System.Drawing.Point(30, 353);
            this.addCategoryBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.addCategoryBtn.Name = "addCategoryBtn";
            this.addCategoryBtn.Size = new System.Drawing.Size(181, 53);
            this.addCategoryBtn.TabIndex = 4;
            this.addCategoryBtn.Text = "Add Brand";
            this.addCategoryBtn.UseVisualStyleBackColor = false;
            // 
            // categoryGridGroup
            // 
            this.categoryGridGroup.BackColor = System.Drawing.Color.SeaGreen;
            this.categoryGridGroup.Controls.Add(this.label3);
            this.categoryGridGroup.Controls.Add(this.txtSearch);
            this.categoryGridGroup.Controls.Add(this.dgvBrands);
            this.categoryGridGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryGridGroup.Location = new System.Drawing.Point(13, 538);
            this.categoryGridGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.categoryGridGroup.Name = "categoryGridGroup";
            this.categoryGridGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.categoryGridGroup.Size = new System.Drawing.Size(879, 380);
            this.categoryGridGroup.TabIndex = 42;
            this.categoryGridGroup.TabStop = false;
            this.categoryGridGroup.Text = "Brands List";
            // 
            // dgvBrands
            // 
            this.dgvBrands.AllowUserToAddRows = false;
            this.dgvBrands.AllowUserToDeleteRows = false;
            this.dgvBrands.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBrands.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvBrands.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBrands.ColumnHeadersHeight = 35;
            this.dgvBrands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvBrands.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvBrands.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.dgvBrands.Location = new System.Drawing.Point(30, 72);
            this.dgvBrands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvBrands.Name = "dgvBrands";
            this.dgvBrands.ReadOnly = true;
            this.dgvBrands.RowHeadersVisible = false;
            this.dgvBrands.RowHeadersWidth = 62;
            this.dgvBrands.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBrands.Size = new System.Drawing.Size(810, 285);
            this.dgvBrands.TabIndex = 0;
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
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.Location = new System.Drawing.Point(129, 37);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(711, 31);
            this.txtSearch.TabIndex = 43;
            // 
            // Brands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 925);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.categoryInputGroup);
            this.Controls.Add(this.categoryGridGroup);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "Brands";
            this.Text = "MADNI MOBILE & PHOTOSTATE - Brands";
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
        private System.Windows.Forms.GroupBox categoryInputGroup;
        private System.Windows.Forms.Label categoryNameLabel;
        private System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label categoryDescLabel;
        private System.Windows.Forms.TextBox txtCategoryDesc;
        private System.Windows.Forms.Button Savebtn;
        private System.Windows.Forms.Button Updatebtn;
        private System.Windows.Forms.Button Deletebtn;
        private System.Windows.Forms.Button addCategoryBtn;
        private System.Windows.Forms.GroupBox categoryGridGroup;
        private System.Windows.Forms.DataGridView dgvBrands;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSearch;
    }
}