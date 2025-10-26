using System;
using System.Data.SqlClient;
using Vape_Store.DataAccess;

namespace Vape_Store
{
    /// <summary>
    /// Console application to add unique constraint to Barcode column
    /// </summary>
    public class DatabaseConstraintSetup
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Vape Store - Database Constraint Setup");
            Console.WriteLine("Adding Unique Constraint to Barcode Column");
            Console.WriteLine("=============================================");
            Console.WriteLine();

            try
            {
                // Check for existing duplicates first
                CheckForDuplicateBarcodes();
                
                // Add the unique constraint
                AddUniqueConstraint();
                
                Console.WriteLine();
                Console.WriteLine("✅ Database constraint setup completed successfully!");
                Console.WriteLine("The Barcode column now has a unique constraint.");
                Console.WriteLine("Duplicate barcodes will be prevented at the database level.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Please check:");
                Console.WriteLine("1. SQL Server is running");
                Console.WriteLine("2. Database connection is correct");
                Console.WriteLine("3. You have appropriate permissions");
                Console.WriteLine("4. No duplicate barcodes exist in the database");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void CheckForDuplicateBarcodes()
        {
            Console.WriteLine("🔍 Checking for existing duplicate barcodes...");
            
            string query = @"
                SELECT Barcode, COUNT(*) as DuplicateCount
                FROM Products 
                WHERE Barcode IS NOT NULL AND Barcode != ''
                GROUP BY Barcode
                HAVING COUNT(*) > 1";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        bool hasDuplicates = false;
                        while (reader.Read())
                        {
                            hasDuplicates = true;
                            string barcode = reader["Barcode"].ToString();
                            int count = Convert.ToInt32(reader["DuplicateCount"]);
                            Console.WriteLine($"   ⚠️  Found duplicate barcode: '{barcode}' ({count} times)");
                        }

                        if (hasDuplicates)
                        {
                            Console.WriteLine();
                            Console.WriteLine("❌ Duplicate barcodes found! Please resolve them before adding the constraint.");
                            Console.WriteLine("You can:");
                            Console.WriteLine("1. Update duplicate barcodes to be unique");
                            Console.WriteLine("2. Set duplicate barcodes to NULL");
                            Console.WriteLine("3. Delete duplicate records");
                            Console.WriteLine();
                            Console.WriteLine("Run the SQL script manually to handle duplicates, then run this setup again.");
                            throw new Exception("Cannot add unique constraint while duplicates exist.");
                        }
                        else
                        {
                            Console.WriteLine("   ✅ No duplicate barcodes found.");
                        }
                    }
                }
            }
        }

        private static void AddUniqueConstraint()
        {
            Console.WriteLine("🔧 Adding unique constraint to Barcode column...");

            // Check if constraint already exists
            string checkQuery = @"
                SELECT COUNT(*) 
                FROM sys.indexes 
                WHERE name = 'UQ_Products_Barcode' 
                AND object_id = OBJECT_ID('Products')";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(checkQuery, connection))
                {
                    connection.Open();
                    int constraintExists = Convert.ToInt32(command.ExecuteScalar());
                    
                    if (constraintExists > 0)
                    {
                        Console.WriteLine("   ℹ️  Unique constraint UQ_Products_Barcode already exists.");
                        return;
                    }
                }

                // Add the unique constraint
                string addConstraintQuery = @"
                    ALTER TABLE Products
                    ADD CONSTRAINT UQ_Products_Barcode UNIQUE (Barcode)";

                using (var command = new SqlCommand(addConstraintQuery, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("   ✅ Unique constraint UQ_Products_Barcode added successfully!");
                }

                // Verify the constraint was added
                string verifyQuery = @"
                    SELECT CONSTRAINT_NAME, CONSTRAINT_TYPE, TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE TABLE_NAME = 'Products' 
                    AND CONSTRAINT_NAME = 'UQ_Products_Barcode'";

                using (var command = new SqlCommand(verifyQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string constraintName = reader["CONSTRAINT_NAME"].ToString();
                            string constraintType = reader["CONSTRAINT_TYPE"].ToString();
                            Console.WriteLine($"   ✅ Verified: {constraintType} constraint '{constraintName}' is active.");
                        }
                    }
                }
            }
        }
    }
}
