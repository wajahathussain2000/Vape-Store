using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Vape_Store.Services
{
    /// <summary>
    /// Service class for application logging
    /// Provides file-based logging with rotation and different log levels
    /// </summary>
    public class LoggingService
    {
        #region Singleton Pattern

        private static LoggingService _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of LoggingService
        /// </summary>
        public static LoggingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggingService();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Private Fields

        private readonly ConfigurationService _config;
        private readonly string _logDirectory;
        private readonly object _logLock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private LoggingService()
        {
            _config = ConfigurationService.Instance;
            _logDirectory = _config.GetLogDirectory();
            
            // Create log directory if it doesn't exist
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        #endregion

        #region Public Logging Methods

        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogInfo(string message, string source = "Application")
        {
            if (_config.EnableLogging)
            {
                WriteLog(LogLevel.Info, message, source);
            }
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogWarning(string message, string source = "Application")
        {
            if (_config.EnableLogging)
            {
                WriteLog(LogLevel.Warning, message, source);
            }
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogError(string message, string source = "Application")
        {
            if (_config.EnableLogging)
            {
                WriteLog(LogLevel.Error, message, source);
            }
        }

        /// <summary>
        /// Logs an error message with exception details
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogError(string message, Exception exception, string source = "Application")
        {
            if (_config.EnableLogging)
            {
                string fullMessage = $"{message}\nException: {exception?.Message}\nStack Trace: {exception?.StackTrace}";
                WriteLog(LogLevel.Error, fullMessage, source);
            }
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogDebug(string message, string source = "Application")
        {
            if (_config.EnableLogging && _config.LogLevel == "Debug")
            {
                WriteLog(LogLevel.Debug, message, source);
            }
        }

        /// <summary>
        /// Logs a critical error message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogCritical(string message, string source = "Application")
        {
            if (_config.EnableLogging)
            {
                WriteLog(LogLevel.Critical, message, source);
            }
        }

        /// <summary>
        /// Logs a critical error message with exception details
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="exception">Exception to log</param>
        /// <param name="source">Source of the log message</param>
        public void LogCritical(string message, Exception exception, string source = "Application")
        {
            if (_config.EnableLogging)
            {
                string fullMessage = $"{message}\nException: {exception?.Message}\nStack Trace: {exception?.StackTrace}";
                WriteLog(LogLevel.Critical, fullMessage, source);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes a log entry to the log file
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        private void WriteLog(LogLevel level, string message, string source)
        {
            try
            {
                lock (_logLock)
                {
                    string logFileName = GetLogFileName();
                    string logEntry = FormatLogEntry(level, message, source);
                    
                    // Check if log rotation is needed
                    CheckLogRotation(logFileName);
                    
                    // Write to log file
                    File.AppendAllText(logFileName, logEntry + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"Logging failed: {ex.Message}");
                Console.WriteLine($"Original message: [{level}] {source}: {message}");
            }
        }

        /// <summary>
        /// Gets the current log file name
        /// </summary>
        /// <returns>Full path to the log file</returns>
        private string GetLogFileName()
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(_logDirectory, $"VapeStore_{dateString}.log");
        }

        /// <summary>
        /// Formats a log entry with timestamp and level
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="message">Message to log</param>
        /// <param name="source">Source of the log message</param>
        /// <returns>Formatted log entry</returns>
        private string FormatLogEntry(LogLevel level, string message, string source)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(3, '0');
            string levelString = level.ToString().ToUpper().PadRight(8);
            
            return $"[{timestamp}] [{threadId}] [{levelString}] [{source}] {message}";
        }

        /// <summary>
        /// Checks if log rotation is needed and performs it if necessary
        /// </summary>
        /// <param name="logFileName">Current log file name</param>
        private void CheckLogRotation(string logFileName)
        {
            try
            {
                if (File.Exists(logFileName))
                {
                    FileInfo fileInfo = new FileInfo(logFileName);
                    
                    // Check if file size exceeds maximum
                    if (fileInfo.Length > _config.MaxLogFileSize)
                    {
                        RotateLogFile(logFileName);
                    }
                }
                
                // Clean up old log files
                CleanupOldLogFiles();
            }
            catch (Exception ex)
            {
                // Log rotation failure shouldn't stop the application
                Console.WriteLine($"Log rotation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Rotates the current log file by renaming it with a timestamp
        /// </summary>
        /// <param name="logFileName">Current log file name</param>
        private void RotateLogFile(string logFileName)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string rotatedFileName = logFileName.Replace(".log", $"_{timestamp}.log");
                File.Move(logFileName, rotatedFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log file rotation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans up old log files based on retention policy
        /// </summary>
        private void CleanupOldLogFiles()
        {
            try
            {
                var logFiles = Directory.GetFiles(_logDirectory, "VapeStore_*.log");
                
                if (logFiles.Length > _config.MaxLogFiles)
                {
                    // Sort by creation time (oldest first)
                    Array.Sort(logFiles, (x, y) => File.GetCreationTime(x).CompareTo(File.GetCreationTime(y)));
                    
                    // Delete oldest files
                    int filesToDelete = logFiles.Length - _config.MaxLogFiles;
                    for (int i = 0; i < filesToDelete; i++)
                    {
                        File.Delete(logFiles[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log cleanup failed: {ex.Message}");
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Log levels for different types of messages
        /// </summary>
        private enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }

        #endregion
    }
}


