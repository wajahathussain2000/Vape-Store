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
        private readonly Repositories.StoreSettingsRepository _settingsRepo;
        private Models.StoreSettings _cachedSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private ConfigurationService()
        {
            _applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(_applicationPath, "Logs");
            _settingsRepo = new Repositories.StoreSettingsRepository();
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
        /// Refreshes the cached store settings from the database
        /// </summary>
        public void RefreshSettings()
        {
            _cachedSettings = _settingsRepo.GetSettings();
        }

        /// <summary>
        /// Gets the current store name
        /// </summary>
        public string ApplicationName
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.StoreName ?? GetConfigValue("ApplicationName", "Vape Store");
            }
        }

        /// <summary>
        /// Gets the current store contact number
        /// </summary>
        public string StoreContact
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.StoreContact ?? "0345:5518744";
            }
        }

        /// <summary>
        /// Gets the current store address
        /// </summary>
        public string StoreAddress
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.StoreAddress ?? "Shop#3, opp Save Mart, main Tulsa road, lalazar,Rwp";
            }
        }

        /// <summary>
        /// Gets the current receipt footer
        /// </summary>
        public string ReceiptFooter
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.ReceiptFooter ?? "- GOODS PURCHASED ARE NOT RETURNABLE\n- GOODS ONCE PURCHASED ARE ONLY EXCHANGEABLE NOT RETURNABLE.\n- MADNI MOBILE SHOP IS NOT RESPONSIBLE FOR ANY WARRANTY CLAIMS.";
            }
        }

        /// <summary>
        /// Gets the current store email
        /// </summary>
        public string StoreEmail
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.StoreEmail ?? string.Empty;
            }
        }

        public string BarcodeDefaultLabel
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeDefaultLabel ?? ApplicationName;
            }
        }

        public int BarcodeWidth
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeWidth ?? 130;
            }
        }

        public int BarcodeHeight
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeHeight ?? 90;
            }
        }

        public decimal BarcodeGap
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeGap ?? 3m;
            }
        }

        public decimal BarcodeMarginLeft
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeMarginLeft ?? 0m;
            }
        }

        public decimal BarcodeMarginRight
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeMarginRight ?? 12m;
            }
        }

        public decimal BarcodeMarginTop
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeMarginTop ?? 4m;
            }
        }

        public decimal BarcodeMarginBottom
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeMarginBottom ?? 0m;
            }
        }

        public bool BarcodeIsThermal
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodeIsThermal ?? true;
            }
        }

        public int ThermalPaperWidth
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.ThermalPaperWidth ?? 300;
            }
        }

        public string ThermalPrinterName
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.ThermalPrinterName ?? string.Empty;
            }
        }

        public string BarcodePrinterName
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.BarcodePrinterName ?? string.Empty;
            }
        }

        public bool DirectPrintReceipt
        {
            get
            {
                if (_cachedSettings == null) RefreshSettings();
                return _cachedSettings?.DirectPrintReceipt ?? false;
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
