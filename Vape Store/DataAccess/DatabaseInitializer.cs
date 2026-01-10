using System;
using System.Data.SqlClient;

namespace Vape_Store.DataAccess
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    EnsureCustomerLedgerTable(connection);
                }
            }
            catch (Exception ex)
            {
                // In a real app we might want to log this or show a message, 
                // but we don't want to crash start up if it's just a minor connection issue that might resolve later.
                // However, missing tables are critical.
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        private static void EnsureCustomerLedgerTable(SqlConnection connection)
        {
            string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CustomerLedger'";
            
            using (var command = new SqlCommand(checkTableQuery, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    // Table missing, create it
                    string createTableQuery = @"
                        CREATE TABLE [dbo].[CustomerLedger](
                            [LedgerEntryID] [int] IDENTITY(1,1) NOT NULL,
                            [CustomerID] [int] NOT NULL,
                            [EntryDate] [datetime] NOT NULL,
                            [ReferenceType] [varchar](50) NULL,
                            [ReferenceID] [int] NULL,
                            [InvoiceNumber] [varchar](50) NULL,
                            [Description] [varchar](255) NULL,
                            [Debit] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [Credit] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [Balance] [decimal](18, 2) NOT NULL DEFAULT 0,
                            [CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
                            CONSTRAINT [PK_CustomerLedger] PRIMARY KEY CLUSTERED ([LedgerEntryID] ASC)
                        );
                        
                        -- Add Foreign Key if Customers table exists
                        IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers')
                        BEGIN
                            ALTER TABLE [dbo].[CustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_CustomerLedger_Customers] FOREIGN KEY([CustomerID])
                            REFERENCES [dbo].[Customers] ([CustomerID])
                            ON DELETE CASCADE;

                            ALTER TABLE [dbo].[CustomerLedger] CHECK CONSTRAINT [FK_CustomerLedger_Customers];
                        END
                    ";

                    using (var createCommand = new SqlCommand(createTableQuery, connection))
                    {
                        createCommand.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("Created table: CustomerLedger");
                    }
                }
            }
        }
    }
}
