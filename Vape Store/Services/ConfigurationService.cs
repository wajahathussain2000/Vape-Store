using System;
using System.IO;
using System.Configuration;

namespace Vape_Store.Services
{
    /// <summary>
    /// Service class for application configuration management
    /// Provides centralized access to application settings
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

        #region Private Fields

        private readonly string _applicationPath;
        private readonly string _logDirectory;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private ConfigurationService()
        {
            _applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(_applicationPath, "Logs");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether logging is enabled
        /// </summary>
        public bool EnableLogging
        {
            get
            {
                try
                {
                    string value = ConfigurationManager.AppSettings["EnableLogging"];
                    return string.IsNullOrEmpty(value) || bool.Parse(value); // Default to true
                }
                catch
                {
                    return true; // Default to enabled
                }
            }
        }

        /// <summary>
        /// Gets the log level (Debug, Info, Warning, Error, Critical)
        /// </summary>
        public string LogLevel
        {
            get
            {
                try
                {
                    string value = ConfigurationManager.AppSettings["LogLevel"];
                    return string.IsNullOrEmpty(value) ? "Info" : value;
                }
                catch
                {
                    return "Info"; // Default level
                }
            }
        }

        /// <summary>
        /// Gets the maximum log file size in bytes (default: 10MB)
        /// </summary>
        public long MaxLogFileSize
        {
            get
            {
                try
                {
                    string value = ConfigurationManager.AppSettings["MaxLogFileSize"];
                    if (string.IsNullOrEmpty(value))
                        return 10 * 1024 * 1024; // 10MB default
                    
                    return long.Parse(value);
                }
                catch
                {
                    return 10 * 1024 * 1024; // 10MB default
                }
            }
        }

        /// <summary>
        /// Gets the maximum number of log files to keep (default: 30)
        /// </summary>
        public int MaxLogFiles
        {
            get
            {
                try
                {
                    string value = ConfigurationManager.AppSettings["MaxLogFiles"];
                    if (string.IsNullOrEmpty(value))
                        return 30;
                    
                    return int.Parse(value);
                }
                catch
                {
                    return 30; // Default
                }
            }
        }

        /// <summary>
        /// Gets the application name
        /// </summary>
        public string ApplicationName
        {
            get
            {
                try
                {
                    string value = ConfigurationManager.AppSettings["ApplicationName"];
                    return string.IsNullOrEmpty(value) ? "Vape Store" : value;
                }
                catch
                {
                    return "Vape Store";
                }
            }
        }

        /// <summary>
        /// Gets the application version
        /// </summary>
        public string ApplicationVersion
        {
            get
            {
                try
                {
                    return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch
                {
                    return "1.0.0.0";
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the log directory path
        /// </summary>
        /// <returns>Full path to the log directory</returns>
        public string GetLogDirectory()
        {
            return _logDirectory;
        }

        /// <summary>
        /// Gets the application path
        /// </summary>
        /// <returns>Full path to the application directory</returns>
        public string GetApplicationPath()
        {
            return _applicationPath;
        }

        /// <summary>
        /// Gets a configuration value by key
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value or default</returns>
        public string GetConfigValue(string key, string defaultValue = "")
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets a connection string by name
        /// </summary>
        /// <param name="name">Connection string name</param>
        /// <returns>Connection string or empty string if not found</returns>
        public string GetConnectionString(string name)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[name];
                return connectionString?.ConnectionString ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
