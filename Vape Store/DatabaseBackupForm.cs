using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vape_Store.Services;

namespace Vape_Store
{
    public partial class DatabaseBackupForm : Form
    {
        private readonly DatabaseBackupService _backupService;
        private List<string> _backupFiles;

        public DatabaseBackupForm()
        {
            _backupService = new DatabaseBackupService();
            InitializeComponent();
            LoadBackupFiles();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Database Backup Manager";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Title label
            var titleLabel = new Label
            {
                Text = "Database Backup Manager",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Backup location label
            var locationLabel = new Label
            {
                Text = _backupService.GetBackupLocationInfo(),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(20, 50)
            };

            // Backup button
            var backupButton = new Button
            {
                Text = "🔄 Create New Backup",
                Size = new Size(200, 40),
                Location = new Point(20, 90),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            backupButton.FlatAppearance.BorderSize = 0;
            backupButton.Click += BackupButton_Click;

            // Backup folder button
            var folderButton = new Button
            {
                Text = "📁 Open Backup Folder",
                Size = new Size(150, 40),
                Location = new Point(240, 90),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            folderButton.FlatAppearance.BorderSize = 0;
            folderButton.Click += FolderButton_Click;

            // Change location button
            var changeLocationButton = new Button
            {
                Text = "📂 Change Location",
                Size = new Size(150, 40),
                Location = new Point(410, 90),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            changeLocationButton.FlatAppearance.BorderSize = 0;
            changeLocationButton.Click += ChangeLocationButton_Click;

            // Backup files list
            var filesLabel = new Label
            {
                Text = "Existing Backups:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 150)
            };

            var filesListBox = new ListBox
            {
                Size = new Size(540, 150),
                Location = new Point(20, 180),
                Font = new Font("Segoe UI", 9)
            };
            filesListBox.SelectedIndexChanged += FilesListBox_SelectedIndexChanged;

            // Restore button
            var restoreButton = new Button
            {
                Text = "🔄 Restore Selected Backup",
                Size = new Size(200, 40),
                Location = new Point(20, 320),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false
            };
            restoreButton.FlatAppearance.BorderSize = 0;
            restoreButton.Click += RestoreButton_Click;

            // Delete button
            var deleteButton = new Button
            {
                Text = "🗑️ Delete Selected Backup",
                Size = new Size(200, 40),
                Location = new Point(240, 320),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = false
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += DeleteButton_Click;

            // Close button
            var closeButton = new Button
            {
                Text = "Close",
                Size = new Size(100, 40),
                Location = new Point(460, 320),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            // Add controls to main panel
            mainPanel.Controls.AddRange(new Control[] {
                titleLabel, locationLabel, backupButton, folderButton, changeLocationButton, filesLabel, 
                filesListBox, restoreButton, deleteButton, closeButton
            });

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);

            // Store references for event handlers
            this.filesListBox = filesListBox;
            this.restoreButton = restoreButton;
            this.deleteButton = deleteButton;
            this.locationLabel = locationLabel;
        }

        private ListBox filesListBox;
        private Button restoreButton;
        private Button deleteButton;
        private Label locationLabel;

        private void LoadBackupFiles()
        {
            try
            {
                _backupFiles = _backupService.GetBackupFiles().ToList();
                filesListBox.Items.Clear();

                foreach (var filePath in _backupFiles)
                {
                    var fileName = System.IO.Path.GetFileName(filePath);
                    var fileDate = _backupService.GetBackupFileDate(filePath);
                    var fileSize = _backupService.GetBackupFileSize(filePath);
                    var sizeText = _backupService.FormatFileSize(fileSize);
                    
                    filesListBox.Items.Add($"{fileName} - {fileDate:yyyy-MM-dd HH:mm} ({sizeText})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading backup files: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackupButton_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will create a backup of your database. Continue?",
                    "Confirm Backup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Show progress
                    var progressForm = new Form
                    {
                        Text = "Creating Backup...",
                        Size = new Size(300, 100),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var progressLabel = new Label
                    {
                        Text = "Creating database backup, please wait...",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    progressForm.Controls.Add(progressLabel);
                    progressForm.Show();

                    Application.DoEvents();

                    // Create backup
                    string backupPath = _backupService.BackupDatabase();
                    
                    progressForm.Close();

                    MessageBox.Show(
                        $"Backup created successfully!\n\nFile: {System.IO.Path.GetFileName(backupPath)}\n\n{_backupService.GetBackupLocationInfo()}",
                        "Backup Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Refresh the list
                    LoadBackupFiles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", _backupService.GetBackupFolderPath());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening folder: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelection = filesListBox.SelectedIndex >= 0;
            restoreButton.Enabled = hasSelection;
            deleteButton.Enabled = hasSelection;
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            if (filesListBox.SelectedIndex < 0) return;

            try
            {
                var result = MessageBox.Show(
                    "WARNING: This will replace your current database with the selected backup. " +
                    "All current data will be lost. Are you sure you want to continue?",
                    "Confirm Restore",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Show progress
                    var progressForm = new Form
                    {
                        Text = "Restoring Backup...",
                        Size = new Size(300, 100),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var progressLabel = new Label
                    {
                        Text = "Restoring database backup, please wait...",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    progressForm.Controls.Add(progressLabel);
                    progressForm.Show();

                    Application.DoEvents();

                    // Restore backup
                    string backupPath = _backupFiles[filesListBox.SelectedIndex];
                    _backupService.RestoreDatabase(backupPath);
                    
                    progressForm.Close();

                    MessageBox.Show(
                        "Database restored successfully!\n\nPlease restart the application to see the restored data.",
                        "Restore Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Restore failed: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (filesListBox.SelectedIndex < 0) return;

            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete the selected backup file?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string backupPath = _backupFiles[filesListBox.SelectedIndex];
                    _backupService.DeleteBackupFile(backupPath);
                    
                    MessageBox.Show("Backup file deleted successfully!", "Delete Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh the list
                    LoadBackupFiles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeLocationButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Backup Folder Location";
                    folderDialog.SelectedPath = _backupService.GetBackupFolderPath();
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string newLocation = folderDialog.SelectedPath;
                        
                        // Test the new location
                        if (_backupService.SetBackupLocation(newLocation))
                        {
                            // Update the location label
                            locationLabel.Text = _backupService.GetBackupLocationInfo();
                            
                            MessageBox.Show(
                                $"Backup location changed successfully!\n\n{_backupService.GetBackupLocationInfo()}",
                                "Location Changed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            
                            // Refresh the backup files list
                            LoadBackupFiles();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Cannot use the selected folder. Please choose a different location with write permissions.",
                                "Invalid Location",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing location: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
