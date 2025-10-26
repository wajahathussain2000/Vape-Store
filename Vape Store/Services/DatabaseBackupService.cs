using System;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace Vape_Store.Services
{
    /// <summary>
    /// Service class for managing database backup and restore operations
    /// Provides functionality to create, restore, and manage database backups
    /// </summary>
    public class DatabaseBackupService
    {
        #region Private Fields

        /// <summary>
        /// Database connection string for backup operations
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Current backup folder path where backups are stored
        /// </summary>
        private string backupFolderPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DatabaseBackupService class
        /// Loads connection string and initializes backup location
        /// </summary>
        public DatabaseBackupService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
            LoadBackupLocation();
        }

        #endregion

        #region Backup Location Management

        /// <summary>
        /// Loads and initializes the backup location from settings or fallback locations
        /// </summary>
        private void LoadBackupLocation()
        {
            // Try to load from settings first
            string savedLocation = Properties.Settings.Default.BackupLocation;
            
            if (!string.IsNullOrEmpty(savedLocation) && Directory.Exists(savedLocation))
            {
                try
                {
                    // Test write access to saved location
                    if (TestWriteAccess(savedLocation))
                    {
                        backupFolderPath = savedLocation;
                        return;
                    }
                }
                catch
                {
                    // Saved location is not accessible, continue to fallback
                }
            }
            
            // Fallback to default locations
            string[] backupLocations = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "VapeStore_Backups"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VapeStore_Backups"),
                Path.Combine(Path.GetTempPath(), "VapeStore_Backups"),
                Path.Combine(Environment.CurrentDirectory, "Backups")
            };
            
            // Find the first writable location
            foreach (string location in backupLocations)
            {
                try
                {
                    if (!Directory.Exists(location))
                    {
                        Directory.CreateDirectory(location);
                    }
                    
                    if (TestWriteAccess(location))
                    {
                        backupFolderPath = location;
                        SaveBackupLocation(location);
                        break;
                    }
                }
                catch
                {
                    // Try next location
                    continue;
                }
            }
            
            // If all locations fail, use application directory
            if (string.IsNullOrEmpty(backupFolderPath))
            {
                backupFolderPath = Path.Combine(Environment.CurrentDirectory, "Backups");
                try
                {
                    if (!Directory.Exists(backupFolderPath))
                    {
                        Directory.CreateDirectory(backupFolderPath);
                    }
                    SaveBackupLocation(backupFolderPath);
                }
                catch
                {
                    throw new Exception("Cannot create backup folder. Please check permissions.");
                }
            }
        }

        /// <summary>
        /// Tests write access to a directory by creating and deleting a test file
        /// </summary>
        /// <param name="directoryPath">Path to the directory to test</param>
        /// <returns>True if write access is available, false otherwise</returns>
        private bool TestWriteAccess(string directoryPath)
        {
            try
            {
                string testFile = Path.Combine(directoryPath, "test_write.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Saves the backup location to application settings
        /// </summary>
        /// <param name="location">The backup location path to save</param>
        private void SaveBackupLocation(string location)
        {
            try
            {
                Properties.Settings.Default.BackupLocation = location;
                Properties.Settings.Default.Save();
            }
            catch
            {
                // Ignore settings save errors
            }
        }

        /// <summary>
        /// Sets a new backup location and validates write access
        /// </summary>
        /// <param name="newLocation">The new backup location path</param>
        /// <returns>True if location is valid and accessible, false otherwise</returns>
        public bool SetBackupLocation(string newLocation)
        {
            try
            {
                if (string.IsNullOrEmpty(newLocation))
                    return false;

                // Test the new location
                if (!Directory.Exists(newLocation))
                {
                    Directory.CreateDirectory(newLocation);
                }

                // Test write access
                if (!TestWriteAccess(newLocation))
                    return false;

                // Update location
                backupFolderPath = newLocation;
                SaveBackupLocation(newLocation);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Database Backup Operations

        /// <summary>
        /// Creates a full backup of the VapeStore database
        /// </summary>
        /// <returns>Path to the created backup file</returns>
        /// <exception cref="Exception">Thrown when backup operation fails</exception>
        public string BackupDatabase()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"VapeStore_Backup_{timestamp}.bak";
                string backupFilePath = Path.Combine(backupFolderPath, backupFileName);

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string backupQuery = $@"
                        BACKUP DATABASE [VapeStore] 
                        TO DISK = '{backupFilePath}'
                        WITH FORMAT, INIT, NAME = 'VapeStore Full Backup', 
                        SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                    using (var command = new SqlCommand(backupQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5 minutes timeout
                        command.ExecuteNonQuery();
                    }
                }

                return backupFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database backup failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Restores the VapeStore database from a backup file
        /// </summary>
        /// <param name="backupFilePath">Path to the backup file to restore from</param>
        /// <returns>True if restore was successful, false otherwise</returns>
        /// <exception cref="Exception">Thrown when restore operation fails</exception>
        public bool RestoreDatabase(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    throw new Exception("Backup file not found.");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // First, set database to single user mode
                    string singleUserQuery = "ALTER DATABASE [VapeStore] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    using (var command = new SqlCommand(singleUserQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Restore the database
                    string restoreQuery = $@"
                        RESTORE DATABASE [VapeStore] 
                        FROM DISK = '{backupFilePath}'
                        WITH REPLACE, RECOVERY";

                    using (var command = new SqlCommand(restoreQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5 minutes timeout
                        command.ExecuteNonQuery();
                    }

                    // Set database back to multi-user mode
                    string multiUserQuery = "ALTER DATABASE [VapeStore] SET MULTI_USER";
                    using (var command = new SqlCommand(multiUserQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database restore failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region Backup File Management

        /// <summary>
        /// Gets a list of all backup files in the backup folder, sorted by creation date (newest first)
        /// </summary>
        /// <returns>Array of backup file paths</returns>
        /// <exception cref="Exception">Thrown when unable to access backup folder</exception>
        public string[] GetBackupFiles()
        {
            try
            {
                if (!Directory.Exists(backupFolderPath))
                {
                    return new string[0];
                }

                var backupFiles = Directory.GetFiles(backupFolderPath, "*.bak");
                Array.Sort(backupFiles, (x, y) => File.GetCreationTime(y).CompareTo(File.GetCreationTime(x)));
                return backupFiles;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting backup files: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a backup file from the backup folder
        /// </summary>
        /// <param name="filePath">Path to the backup file to delete</param>
        /// <exception cref="Exception">Thrown when file deletion fails</exception>
        public void DeleteBackupFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting backup file: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the current backup folder path
        /// </summary>
        /// <returns>Path to the backup folder</returns>
        public string GetBackupFolderPath()
        {
            return backupFolderPath;
        }

        /// <summary>
        /// Gets formatted information about the current backup location
        /// </summary>
        /// <returns>String containing backup location information</returns>
        public string GetBackupLocationInfo()
        {
            try
            {
                string locationType = "";
                if (backupFolderPath.Contains("Desktop"))
                    locationType = "Desktop";
                else if (backupFolderPath.Contains("Documents"))
                    locationType = "Documents";
                else if (backupFolderPath.Contains("Temp"))
                    locationType = "Temp";
                else
                    locationType = "Application Folder";

                return $"Backup Location: {locationType}\nPath: {backupFolderPath}";
            }
            catch
            {
                return $"Backup Location: {backupFolderPath}";
            }
        }

        /// <summary>
        /// Gets the size of a backup file in bytes
        /// </summary>
        /// <param name="filePath">Path to the backup file</param>
        /// <returns>File size in bytes, or 0 if file doesn't exist or error occurs</returns>
        public long GetBackupFileSize(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return new FileInfo(filePath).Length;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the creation date of a backup file
        /// </summary>
        /// <param name="filePath">Path to the backup file</param>
        /// <returns>File creation date, or DateTime.MinValue if file doesn't exist or error occurs</returns>
        public DateTime GetBackupFileDate(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.GetCreationTime(filePath);
                }
                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Formats a file size in bytes to a human-readable string
        /// </summary>
        /// <param name="bytes">File size in bytes</param>
        /// <returns>Formatted file size string (e.g., "1.5 MB")</returns>
        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        #endregion
    }
}