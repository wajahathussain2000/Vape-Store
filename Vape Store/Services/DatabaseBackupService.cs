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
        /// Gets SQL Server's default backup directory
        /// </summary>
        /// <param name="connection">SQL Server connection</param>
        /// <returns>Default backup directory path</returns>
        private string GetSqlServerDefaultBackupPath(SqlConnection connection)
        {
            try
            {
                string query = @"
                    EXEC master.dbo.xp_instance_regread 
                    N'HKEY_LOCAL_MACHINE', 
                    N'SOFTWARE\Microsoft\Microsoft SQL Server\MSSQLServer', 
                    N'BackupDirectory'";
                
                using (var command = new SqlCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        return result.ToString();
                    }
                }
            }
            catch
            {
                // If registry read fails, continue with alternative method
            }

            // Try alternative method - use SQL Server data directory
            try
            {
                string query = @"EXEC xp_instance_regread 
                    N'HKEY_LOCAL_MACHINE', 
                    N'SOFTWARE\Microsoft\Microsoft SQL Server\Setup', 
                    N'SQLDataRoot'";
                
                using (var command = new SqlCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        return Path.Combine(result.ToString(), "Backup");
                    }
                }
            }
            catch
            {
                // Continue with fallback
            }

            return null;
        }

        /// <summary>
        /// Validates that a path is accessible by SQL Server
        /// </summary>
        private bool ValidateBackupPathForSqlServer(string backupPath, SqlConnection connection)
        {
            try
            {
                // Check if SQL Server has permission to write to the path
                // We'll test by trying to create a test file in that directory
                string testFileName = Path.Combine(Path.GetDirectoryName(backupPath), "sql_test_write.tmp");
                
                // Escape single quotes in path for SQL
                string escapedPath = backupPath.Replace("'", "''");
                
                // Try to create a test file using xp_cmdshell (if enabled) or FILEEXISTS
                // If not available, we'll rely on the actual backup attempt
                return true; // Return true and let the backup attempt determine if path works
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Escapes a file path for use in SQL Server commands
        /// </summary>
        private string EscapeSqlPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            
            // Replace single quotes with double single quotes for SQL escaping
            return path.Replace("'", "''");
        }

        /// <summary>
        /// Creates a full backup of the connected database.
        /// - Uses the current DB name dynamically
        /// - Prefers SQL Server default backup directory with fallbacks
        /// - Tries WITH COMPRESSION first, then retries without if unsupported
        /// </summary>
        /// <returns>Path to the created backup file</returns>
        /// <exception cref="Exception">Thrown when backup operation fails</exception>
        public string BackupDatabase()
        {
            string backupFilePath = null;
            string sqlBackupPath = null;
            
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    string dbName = string.IsNullOrWhiteSpace(builder.InitialCatalog) ? connection.Database : builder.InitialCatalog;
                    if (string.IsNullOrWhiteSpace(dbName))
                    {
                        throw new Exception("Unable to determine database name from connection string.");
                    }
                    string safeDbName = dbName.Replace('\\','_').Replace('/', '_').Replace(':','_').Replace('*','_').Replace('?','_').Replace('"','_').Replace('<','_').Replace('>','_').Replace('|','_');
                    string backupFileName = $"{safeDbName}_Backup_{timestamp}.bak";
                    
                    // First, try to get SQL Server's default backup directory
                    string sqlDefaultPath = GetSqlServerDefaultBackupPath(connection);
                    
                    // Determine which path to use
                    if (!string.IsNullOrEmpty(sqlDefaultPath) && Directory.Exists(sqlDefaultPath))
                    {
                        // Use SQL Server's default backup directory (more reliable)
                        sqlBackupPath = Path.Combine(sqlDefaultPath, backupFileName);
                        backupFilePath = sqlBackupPath;
                    }
                    else
                    {
                        // Use our configured backup folder
                        backupFilePath = Path.Combine(backupFolderPath, backupFileName);
                        sqlBackupPath = backupFilePath;
                    }
                    
                    // Ensure the directory exists
                    string backupDir = Path.GetDirectoryName(sqlBackupPath);
                    if (!Directory.Exists(backupDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(backupDir);
                        }
                        catch (Exception dirEx)
                        {
                            throw new Exception($"Cannot create backup directory: {dirEx.Message}. Please ensure the SQL Server service account has write permissions.", dirEx);
                        }
                    }
                    
                    // Escape the path for SQL command
                    string escapedBackupPath = EscapeSqlPath(sqlBackupPath);
                    string escapedDbName = dbName.Replace("'", "''");
                    
                    // Check if user has backup permission (best-effort)
                    string permissionQuery = $@"
                        SELECT HAS_PERMS_BY_NAME('{escapedDbName}', 'DATABASE', 'BACKUP DATABASE')";
                    
                    bool hasPermission = false;
                    try
                    {
                        using (var permCommand = new SqlCommand(permissionQuery, connection))
                        {
                            var permResult = permCommand.ExecuteScalar();
                            hasPermission = permResult != null && Convert.ToBoolean(permResult);
                        }
                    }
                    catch
                    {
                        // Permission check failed, but continue with backup attempt
                    }
                    
                    if (!hasPermission)
                    {
                        // Try backup anyway - the error message will be more specific
                    }
                    
                    // Helper to attempt a backup with optional COMPRESSION
                    Action<bool> attemptBackup = (withCompression) =>
                    {
                        string compressionClause = withCompression ? ", COMPRESSION" : string.Empty;
                        string backupQuery = $@"
                            BACKUP DATABASE [{escapedDbName}] 
                            TO DISK = '{escapedBackupPath}'
                            WITH FORMAT, INIT, NAME = '{escapedDbName} Full Backup', 
                            SKIP, NOREWIND, NOUNLOAD, STATS = 10{compressionClause}";
                        using (var cmd = new SqlCommand(backupQuery, connection))
                        {
                            cmd.CommandTimeout = 300; // 5 minutes timeout
                            cmd.ExecuteNonQuery();
                        }
                    };
                    
                    // Try with compression first, then retry without on failure
                    try
                    {
                        attemptBackup(true);
                    }
                    catch (SqlException)
                    {
                        // Retry without compression (covers editions where compression isn't supported)
                        attemptBackup(false);
                    }
                    
                    // Verify the backup file was created
                    if (!File.Exists(backupFilePath))
                    {
                        throw new Exception("Backup command completed but backup file was not found. The backup may have failed silently.");
                    }
                    
                    // If backup was created in SQL Server's default location but user has a preferred location,
                    // copy it to the preferred location for easier access
                    if (sqlDefaultPath != null && Directory.Exists(sqlDefaultPath) && 
                        backupFilePath != Path.Combine(backupFolderPath, backupFileName))
                    {
                        try
                        {
                            string copiedPath = CopyBackupToPreferredLocation(backupFilePath);
                            // Return the copied path as it's in the user's preferred location
                            return copiedPath;
                        }
                        catch
                        {
                            // If copy fails, still return the original path
                            // User can find it in SQL Server's default location
                        }
                    }
                }

                return backupFilePath;
            }
            catch (SqlException sqlEx)
            {
                // Provide more specific error messages based on SQL error
                string errorMessage = "Database backup failed: ";
                
                switch (sqlEx.Number)
                {
                    case 3013: // Backup database error
                        errorMessage += "Backup operation failed. " + sqlEx.Message;
                        break;
                    case 15105: // Permission denied
                        errorMessage += "Permission denied. The SQL user does not have BACKUP DATABASE permission. " +
                                      "Please contact your database administrator to grant this permission.";
                        break;
                    case 3201: // Cannot open backup device
                        errorMessage += "Cannot access the backup location. " +
                                      $"SQL Server cannot write to: {sqlBackupPath}\n\n" +
                                      "Possible solutions:\n" +
                                      "1. Ensure SQL Server service account has write permissions to the backup folder\n" +
                                      "2. Use SQL Server's default backup directory\n" +
                                      "3. Change backup location to a folder SQL Server can access";
                        break;
                    case 18204: // Backup file error
                        errorMessage += "Cannot create backup file. " +
                                      $"Check if SQL Server has write access to: {Path.GetDirectoryName(sqlBackupPath)}";
                        break;
                    default:
                        errorMessage += sqlEx.Message;
                        if (sqlEx.InnerException != null)
                            errorMessage += "\nInner exception: " + sqlEx.InnerException.Message;
                        break;
                }
                
                throw new Exception(errorMessage, sqlEx);
            }
            catch (UnauthorizedAccessException unAuthEx)
            {
                throw new Exception(
                    $"Access denied to backup location: {backupFilePath}\n\n" +
                    "Please ensure:\n" +
                    "1. The folder exists and is writable\n" +
                    "2. SQL Server service account has write permissions\n" +
                    "3. Try changing the backup location to SQL Server's default backup directory",
                    unAuthEx);
            }
            catch (DirectoryNotFoundException dirEx)
            {
                throw new Exception(
                    $"Backup directory not found: {Path.GetDirectoryName(backupFilePath)}\n\n" +
                    "The backup location may have been deleted or is inaccessible. " +
                    "Please change the backup location using Backup Manager.",
                    dirEx);
            }
            catch (Exception ex)
            {
                string detailedError = $"Database backup failed: {ex.Message}";
                
                if (ex.InnerException != null)
                {
                    detailedError += $"\n\nInner exception: {ex.InnerException.Message}";
                }
                
                detailedError += $"\n\nBackup path attempted: {sqlBackupPath ?? backupFilePath}";
                detailedError += "\n\nTroubleshooting tips:";
                detailedError += "\n1. Ensure SQL Server is running";
                detailedError += "\n2. Check SQL Server service account has write permissions to backup folder";
                detailedError += "\n3. Verify the SQL user has BACKUP DATABASE permission";
                detailedError += "\n4. Try using SQL Server's default backup location";
                detailedError += "\n5. Check available disk space";
                
                throw new Exception(detailedError, ex);
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
                else if (backupFolderPath.Contains("Backup") || backupFolderPath.Contains("MSSQL"))
                    locationType = "SQL Server Default";
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
        /// Gets SQL Server's default backup directory if available
        /// </summary>
        /// <returns>SQL Server default backup directory path, or null if unavailable</returns>
        public string GetSqlServerDefaultBackupDirectory()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return GetSqlServerDefaultBackupPath(connection);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Copies a backup file from SQL Server's default location to the user's preferred location
        /// </summary>
        /// <param name="sourceBackupPath">Source backup file path</param>
        /// <returns>Destination backup file path</returns>
        public string CopyBackupToPreferredLocation(string sourceBackupPath)
        {
            try
            {
                if (!File.Exists(sourceBackupPath))
                {
                    throw new Exception($"Source backup file not found: {sourceBackupPath}");
                }

                string fileName = Path.GetFileName(sourceBackupPath);
                string destPath = Path.Combine(backupFolderPath, fileName);

                // Ensure destination directory exists
                if (!Directory.Exists(backupFolderPath))
                {
                    Directory.CreateDirectory(backupFolderPath);
                }

                // Copy the file
                File.Copy(sourceBackupPath, destPath, true);

                return destPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy backup file: {ex.Message}", ex);
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