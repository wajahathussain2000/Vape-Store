using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Vape_Store.DataAccess
{
    /// <summary>
    /// Handles database connection management and provides connection instances
    /// </summary>
    public class DatabaseConnection
    {
        #region Private Fields

        private static string connectionString;
        private static readonly int DefaultCommandTimeout = 300; // 5 minutes

        #endregion

        #region Constructor

        static DatabaseConnection()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Database connection string not found in configuration.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database connection: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a new database connection instance
        /// </summary>
        /// <returns>SqlConnection instance</returns>
        /// <exception cref="Exception">Thrown when connection creation fails</exception>
        public static SqlConnection GetConnection()
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database connection error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a SqlCommand with default timeout settings
        /// </summary>
        /// <param name="query">SQL query string</param>
        /// <param name="connection">Database connection</param>
        /// <returns>Configured SqlCommand instance</returns>
        public static SqlCommand CreateCommand(string query, SqlConnection connection)
        {
            var command = new SqlCommand(query, connection);
            command.CommandTimeout = DefaultCommandTimeout;
            return command;
        }

        /// <summary>
        /// Creates a SqlCommand with custom timeout
        /// </summary>
        /// <param name="query">SQL query string</param>
        /// <param name="connection">Database connection</param>
        /// <param name="timeout">Command timeout in seconds</param>
        /// <returns>Configured SqlCommand instance</returns>
        public static SqlCommand CreateCommand(string query, SqlConnection connection, int timeout)
        {
            var command = new SqlCommand(query, connection);
            command.CommandTimeout = timeout;
            return command;
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>True if connection is successful, false otherwise</returns>
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the connection string for debugging purposes
        /// </summary>
        /// <returns>Connection string (with sensitive data masked)</returns>
        public static string GetConnectionStringInfo()
        {
            if (string.IsNullOrEmpty(connectionString))
                return "No connection string configured";

            // Mask sensitive information for logging
            var masked = connectionString;
            if (masked.Contains("Password="))
            {
                // This is for future use if we switch to SQL authentication
                masked = masked.Replace("Password=", "Password=***");
            }
            return masked;
        }

        #endregion

        #region Instance Methods

        /// <summary>
        /// Instance method for repositories that need instance-based access
        /// </summary>
        /// <returns>SqlConnection instance</returns>
        public SqlConnection GetConnectionInstance()
        {
            return GetConnection();
        }

        #endregion
    }
}
