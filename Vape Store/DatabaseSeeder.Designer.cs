namespace Vape_Store
{
    partial class DatabaseSeeder
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
            this.btnSeedDatabase = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.btnCheckData = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSeedDatabase
            // 
            this.btnSeedDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSeedDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeedDatabase.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSeedDatabase.ForeColor = System.Drawing.Color.White;
            this.btnSeedDatabase.Location = new System.Drawing.Point(50, 100);
            this.btnSeedDatabase.Name = "btnSeedDatabase";
            this.btnSeedDatabase.Size = new System.Drawing.Size(200, 50);
            this.btnSeedDatabase.TabIndex = 0;
            this.btnSeedDatabase.Text = "Seed Database";
            this.btnSeedDatabase.UseVisualStyleBackColor = false;
            this.btnSeedDatabase.Click += new System.EventHandler(this.BtnSeedDatabase_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(300, 100);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(200, 50);
            this.btnTestConnection.TabIndex = 1;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.BtnTestConnection_Click);
            // 
            // btnCheckData
            // 
            this.btnCheckData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnCheckData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckData.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCheckData.ForeColor = System.Drawing.Color.White;
            this.btnCheckData.Location = new System.Drawing.Point(550, 100);
            this.btnCheckData.Name = "btnCheckData";
            this.btnCheckData.Size = new System.Drawing.Size(200, 50);
            this.btnCheckData.TabIndex = 2;
            this.btnCheckData.Text = "Check Data";
            this.btnCheckData.UseVisualStyleBackColor = false;
            this.btnCheckData.Click += new System.EventHandler(this.BtnCheckData_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblTitle.Location = new System.Drawing.Point(50, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 32);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Database Seeder";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblDescription.Location = new System.Drawing.Point(50, 55);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(400, 19);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Use this tool to populate your database with test data for reports";
            // 
            // DatabaseSeeder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 200);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCheckData);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.btnSeedDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseSeeder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Seeder - Vape Store";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSeedDatabase;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Button btnCheckData;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
    }
}
