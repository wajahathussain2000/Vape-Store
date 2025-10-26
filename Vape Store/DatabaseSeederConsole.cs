using System;
using System.Data.SqlClient;
using System.IO;
using Vape_Store.DataAccess;

namespace Vape_Store
{
    public class DatabaseSeederConsole
    {
        public static void SeedDatabase()
        {
            try
            {
                Console.WriteLine("Seeding Vape Store Database with Test Data...");
                Console.WriteLine("");

                // Read the SQL script
                string sqlScript = File.ReadAllText("Test_Data_Seed.sql");
                
                // Split the script into individual commands
                string[] commands = sqlScript.Split(new string[] { "GO", ";" }, StringSplitOptions.RemoveEmptyEntries);
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    Console.WriteLine("Connected to database successfully!");
                    
                    int successCount = 0;
                    int errorCount = 0;
                    
                    foreach (string command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command.Trim()))
                        {
                            try
                            {
                                using (var sqlCommand = new SqlCommand(command.Trim(), connection))
                                {
                                    sqlCommand.ExecuteNonQuery();
                                    successCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                Console.WriteLine($"Error executing command: {ex.Message}");
                            }
                        }
                    }
                    
                    Console.WriteLine("");
                    Console.WriteLine($"Database seeding completed!");
                    Console.WriteLine($"Successful commands: {successCount}");
                    Console.WriteLine($"Errors: {errorCount}");
                    
                    if (errorCount == 0)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("SUCCESS: Database has been seeded with test data!");
                        Console.WriteLine("You can now run the Vape Store application and see data in the reports.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to seed database: {ex.Message}");
                Console.WriteLine("");
                Console.WriteLine("Make sure:");
                Console.WriteLine("1. SQL Server is running");
                Console.WriteLine("2. Database exists and connection string is correct");
                Console.WriteLine("3. You have appropriate permissions");
            }
        }

        public static void CheckDatabaseData()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    
                    Console.WriteLine("Checking database data...");
                    Console.WriteLine("");
                    
                    // Check if data exists
                    var queries = new[]
                    {
                        ("Products", "SELECT COUNT(*) FROM Products"),
                        ("Sales", "SELECT COUNT(*) FROM Sales"), 
                        ("Customers", "SELECT COUNT(*) FROM Customers"),
                        ("Suppliers", "SELECT COUNT(*) FROM Suppliers"),
                        ("Categories", "SELECT COUNT(*) FROM Categories"),
                        ("Brands", "SELECT COUNT(*) FROM Brands")
                    };
                    
                    foreach (var (tableName, query) in queries)
                    {
                        using (var command = new SqlCommand(query, connection))
                        {
                            var count = command.ExecuteScalar();
                            Console.WriteLine($"{tableName}: {count} records");
                        }
                    }
                    
                    Console.WriteLine("");
                    Console.WriteLine("Data check completed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking data: {ex.Message}");
            }
        }
    }
}
