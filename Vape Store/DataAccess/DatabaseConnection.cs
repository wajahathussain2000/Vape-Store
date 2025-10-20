using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Vape_Store.DataAccess
{
    public class DatabaseConnection
    {
        private static string connectionString;
        
        static DatabaseConnection()
        {
            connectionString = ConfigurationManager.ConnectionStrings["dbs"].ConnectionString;
        }
        
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
        
        // Instance method for repositories that need instance-based access
        public SqlConnection GetConnectionInstance()
        {
            return GetConnection();
        }
    }
}
