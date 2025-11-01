using System;
using System.Configuration;
using System.IO;

namespace Vape_Store.Services
{
    /// <summary>
    /// Service class for managing application configuration settings
    /// Provides centralized access to configuration values and settings management
    /// </summary>
    public class ConfigurationService
    {
        #region Singleton Pattern

        private static ConfigurationService _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of ConfigurationService
        /// </summary>
        public static ConfigurationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigurationService();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private ConfigurationService()
        {
            // Initialize configuration
        }

        #endregion

        #region Application Information

        /// <summary>
        /// Gets the application name
        /// </summary>
        public string ApplicationName => GetConfigValue("ApplicationName", "Vape Store POS System");

        /// <summary>
        /// Gets the application version
        /// </summary>
        public string ApplicationVersion => GetConfigValue("ApplicationVersion", "1.0.0");

        /// <summary>
        /// Gets the application company
        /// </summary>
        public string ApplicationCompany => GetConfigValue("ApplicationCompany", "Vape Store Solutions");

        #endregion

        #region Database Configuration

        /// <summary>
        /// Gets the database timeout in seconds
        /// </summary>
        public int DatabaseTimeout => GetConfigValue("DatabaseTimeout", 300);

        /// <summary>
        /// Gets the maximum retry attempts for database operations
        /// </summary>
        public int MaxRetryAttempts => GetConfigValue("MaxRetryAttempts", 3);

        /// <summary>
        /// Gets the retry delay in milliseconds
        /// </summary>
        public int RetryDelay => GetConfigValue("RetryDelay", 1000);

        #endregion

        #region Backup Configuration

        /// <summary>
        /// Gets the backup retention period in days
        /// </summary>
        public int BackupRetentionDays => GetConfigValue("BackupRetentionDays", 30);

        /// <summary>
        /// Gets the maximum number of backup files to keep
        /// </summary>
        public int MaxBackupFiles => GetConfigValue("MaxBackupFiles", 50);

        /// <summary>
        /// Gets whether backup compression is enabled
        /// </summary>
        public bool BackupCompression => GetConfigValue("BackupCompression", true);

        #endregion

        #region UI Configuration

        /// <summary>
        /// Gets the default theme
        /// </summary>
        public string DefaultTheme => GetConfigValue("DefaultTheme", "Dark");

        /// <summary>
        /// Gets the auto-save interval in seconds
        /// </summary>
        public int AutoSaveInterval => GetConfigValue("AutoSaveInterval", 300);

        /// <summary>
        /// Gets the session timeout in minutes
        /// </summary>
        public int SessionTimeout => GetConfigValue("SessionTimeout", 30);

        #endregion

        #region Barcode Configuration

        /// <summary>
        /// Gets the default barcode width
        /// </summary>
        public int DefaultBarcodeWidth => GetConfigValue("DefaultBarcodeWidth", 300);

        /// <summary>
        /// Gets the default barcode height
        /// </summary>
        public int DefaultBarcodeHeight => GetConfigValue("DefaultBarcodeHeight", 100);

        /// <summary>
        /// Gets the barcode format
        /// </summary>
        public string BarcodeFormat => GetConfigValue("BarcodeFormat", "CODE_128");

        #endregion

        #region Report Configuration

        /// <summary>
        /// Gets the report output path
        /// </summary>
        public string ReportOutputPath => GetConfigValue("ReportOutputPath", "Reports");

        /// <summary>
        /// Gets the report format
        /// </summary>
        public string ReportFormat => GetConfigValue("ReportFormat", "PDF");

        /// <summary>
        /// Gets whether to include logo in reports
        /// </summary>
        public bool IncludeLogo => GetConfigValue("IncludeLogo", true);

        #endregion

        #region Security Configuration

        /// <summary>
        /// Gets the minimum password length
        /// </summary>
        public int PasswordMinLength => GetConfigValue("PasswordMinLength", 6);

        /// <summary>
        /// Gets the maximum login attempts
        /// </summary>
        public int MaxLoginAttempts => GetConfigValue("MaxLoginAttempts", 3);

        /// <summary>
        /// Gets the lockout duration in minutes
        /// </summary>
        public int LockoutDuration => GetConfigValue("LockoutDuration", 15);

        #endregion

        #region Logging Configuration

        /// <summary>
        /// Gets whether logging is enabled
        /// </summary>
        public bool EnableLogging => GetConfigValue("EnableLogging", true);

        /// <summary>
        /// Gets the log level
        /// </summary>
        public string LogLevel => GetConfigValue("LogLevel", "Information");

        /// <summary>
        /// Gets the log file path
        /// </summary>
        public string LogFilePath => GetConfigValue("LogFilePath", "Logs");

        /// <summary>
        /// Gets the maximum log file size in bytes
        /// </summary>
        public long MaxLogFileSize => GetConfigValue("MaxLogFileSize", 10485760L);

        /// <summary>
        /// Gets the maximum number of log files
        /// </summary>
        public int MaxLogFiles => GetConfigValue("MaxLogFiles", 10);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets a configuration value with a default fallback
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        private T GetConfigValue<T>(string key, T defaultValue)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets a configuration value as string
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        public string GetString(string key, string defaultValue = "")
        {
            return GetConfigValue(key, defaultValue);
        }

        /// <summary>
        /// Gets a configuration value as integer
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        public int GetInt(string key, int defaultValue = 0)
        {
            return GetConfigValue(key, defaultValue);
        }

        /// <summary>
        /// Gets a configuration value as boolean
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
            return GetConfigValue(key, defaultValue);
        }

        /// <summary>
        /// Gets a configuration value as double
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        public double GetDouble(string key, double defaultValue = 0.0)
        {
            return GetConfigValue(key, defaultValue);
        }

        #endregion

        #region Path Utilities

        /// <summary>
        /// Gets the full path for a relative path, creating directory if needed
        /// </summary>
        /// <param name="relativePath">Relative path</param>
        /// <returns>Full path</returns>
        public string GetFullPath(string relativePath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            
            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            return fullPath;
        }

        /// <summary>
        /// Gets the log file path
        /// </summary>
        /// <returns>Full path to log directory</returns>
        public string GetLogDirectory()
        {
            return GetFullPath(LogFilePath);
        }

        /// <summary>
        /// Gets the report output directory
        /// </summary>
        /// <returns>Full path to report directory</returns>
        public string GetReportDirectory()
        {
            return GetFullPath(ReportOutputPath);
        }

        #endregion
    }
}


