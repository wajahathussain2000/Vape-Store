using System;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace Vape_Store.Services
{
    public class DatabaseBackupService
    {
        private string connectionString;
        private string backupFolderPath;

        public DatabaseBackupService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
            LoadBackupLocation();
        }

        private void LoadBackupLocation()
        {
            // Try to load from settings first
            string savedLocation = Properties.Settings.Default.BackupLocation;
            
            if (!string.IsNullOrEmpty(savedLocation) && Directory.Exists(savedLocation))
            {
                try
                {
                    // Test write access to saved location
                    string testFile = Path.Combine(savedLocation, "test_write.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                    backupFolderPath = savedLocation;
                    return;
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
                    
                    // Test write access
                    string testFile = Path.Combine(location, "test_write.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                    
                    backupFolderPath = location;
                    SaveBackupLocation(location);
                    break;
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
                string testFile = Path.Combine(newLocation, "test_write.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);

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

        public string GetBackupFolderPath()
        {
            return backupFolderPath;
        }

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
    }
}
