using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class CustomerLedgerRepository
    {
        public int InsertEntry(SqlConnection connection, SqlTransaction transaction, CustomerLedgerEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            decimal lastBalance = GetLatestBalance(connection, transaction, entry.CustomerID);
            entry.Balance = lastBalance + entry.Debit - entry.Credit;

            string insertQuery = @"
                INSERT INTO CustomerLedger
                (CustomerID, EntryDate, ReferenceType, ReferenceID, InvoiceNumber, Description,
                 Debit, Credit, Balance, CreatedDate)
                VALUES
                (@CustomerID, @EntryDate, @ReferenceType, @ReferenceID, @InvoiceNumber, @Description,
                 @Debit, @Credit, @Balance, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            using (var command = new SqlCommand(insertQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@CustomerID", entry.CustomerID);
                command.Parameters.AddWithValue("@EntryDate", entry.EntryDate);
                command.Parameters.AddWithValue("@ReferenceType", (object)entry.ReferenceType ?? DBNull.Value);
                command.Parameters.AddWithValue("@ReferenceID", (object)entry.ReferenceID ?? DBNull.Value);
                command.Parameters.AddWithValue("@InvoiceNumber", (object)entry.InvoiceNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Description", (object)entry.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Debit", entry.Debit);
                command.Parameters.AddWithValue("@Credit", entry.Credit);
                command.Parameters.AddWithValue("@Balance", entry.Balance);
                command.Parameters.AddWithValue("@CreatedDate", entry.CreatedDate == default ? DateTime.Now : entry.CreatedDate);

                entry.LedgerEntryID = Convert.ToInt32(command.ExecuteScalar());
            }

            return entry.LedgerEntryID;
        }

        public void DeleteEntriesByReference(string referenceType, int referenceId, SqlConnection connection, SqlTransaction transaction)
        {
            string deleteQuery = @"DELETE FROM CustomerLedger WHERE ReferenceType = @ReferenceType AND ReferenceID = @ReferenceID";
            using (var command = new SqlCommand(deleteQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@ReferenceType", referenceType);
                command.Parameters.AddWithValue("@ReferenceID", referenceId);
                command.ExecuteNonQuery();
            }
        }

        public decimal GetCustomerBalance(int customerId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                return GetLatestBalance(connection, null, customerId);
            }
        }

        public List<CustomerLedgerEntry> GetEntries(DateTime fromDate, DateTime toDate, int? customerId = null)
        {
            var entries = new List<CustomerLedgerEntry>();
            string query = @"
                SELECT l.LedgerEntryID, l.CustomerID, l.EntryDate, l.ReferenceType, l.ReferenceID,
                       l.InvoiceNumber, l.Description, l.Debit, l.Credit, l.Balance, l.CreatedDate,
                       c.CustomerName, c.CustomerCode, c.Phone
                FROM CustomerLedger l
                INNER JOIN Customers c ON l.CustomerID = c.CustomerID
                WHERE l.EntryDate BETWEEN @FromDate AND @ToDate
                  AND (@CustomerID IS NULL OR l.CustomerID = @CustomerID)
                ORDER BY l.EntryDate, l.LedgerEntryID";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);
                    command.Parameters.AddWithValue("@CustomerID", (object)customerId ?? DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entries.Add(new CustomerLedgerEntry
                            {
                                LedgerEntryID = Convert.ToInt32(reader["LedgerEntryID"]),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                EntryDate = Convert.ToDateTime(reader["EntryDate"]),
                                ReferenceType = reader["ReferenceType"]?.ToString(),
                                ReferenceID = reader["ReferenceID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ReferenceID"]),
                                InvoiceNumber = reader["InvoiceNumber"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                Debit = Convert.ToDecimal(reader["Debit"]),
                                Credit = Convert.ToDecimal(reader["Credit"]),
                                Balance = Convert.ToDecimal(reader["Balance"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                CustomerName = reader["CustomerName"]?.ToString(),
                                CustomerCode = reader["CustomerCode"]?.ToString(),
                                CustomerPhone = reader["Phone"]?.ToString()
                            });
                        }
                    }
                }
            }

            return entries;
        }

        public List<CustomerLedgerSummary> GetCustomerSummaries(DateTime fromDate, DateTime toDate, int? customerId = null)
        {
            var summaries = new List<CustomerLedgerSummary>();
            string query = @"
                SELECT 
                    c.CustomerID,
                    c.CustomerCode,
                    c.CustomerName,
                    c.Phone,
                    SUM(CASE WHEN l.EntryDate BETWEEN @FromDate AND @ToDate THEN l.Debit ELSE 0 END) AS TotalDebit,
                    SUM(CASE WHEN l.EntryDate BETWEEN @FromDate AND @ToDate THEN l.Credit ELSE 0 END) AS TotalCredit,
                    (
                        SELECT TOP 1 Balance 
                        FROM CustomerLedger 
                        WHERE CustomerID = c.CustomerID AND EntryDate <= @ToDate
                        ORDER BY EntryDate DESC, LedgerEntryID DESC
                    ) AS ClosingBalance,
                    (
                        SELECT MAX(EntryDate) 
                        FROM CustomerLedger 
                        WHERE CustomerID = c.CustomerID 
                          AND ReferenceType = 'Sale'
                    ) AS LastSaleDate,
                    (
                        SELECT MAX(EntryDate) 
                        FROM CustomerLedger 
                        WHERE CustomerID = c.CustomerID 
                          AND ReferenceType IN ('SalePayment', 'CustomerPayment', 'Payment')
                    ) AS LastPaymentDate
                FROM Customers c
                INNER JOIN CustomerLedger l ON l.CustomerID = c.CustomerID
                WHERE (@CustomerID IS NULL OR c.CustomerID = @CustomerID)
                GROUP BY c.CustomerID, c.CustomerCode, c.CustomerName, c.Phone
                ORDER BY c.CustomerName";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);
                    command.Parameters.AddWithValue("@CustomerID", (object)customerId ?? DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            summaries.Add(new CustomerLedgerSummary
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerCode = reader["CustomerCode"]?.ToString(),
                                CustomerName = reader["CustomerName"]?.ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                TotalDebit = reader["TotalDebit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalDebit"]),
                                TotalCredit = reader["TotalCredit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalCredit"]),
                                ClosingBalance = reader["ClosingBalance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ClosingBalance"]),
                                LastSaleDate = reader["LastSaleDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastSaleDate"]),
                                LastPaymentDate = reader["LastPaymentDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastPaymentDate"])
                            });
                        }
                    }
                }
            }

            return summaries;
        }

        private decimal GetLatestBalance(SqlConnection connection, SqlTransaction transaction, int customerId)
        {
            string balanceQuery = @"
                SELECT TOP 1 Balance 
                FROM CustomerLedger 
                WHERE CustomerID = @CustomerID
                ORDER BY EntryDate DESC, LedgerEntryID DESC";

            using (var command = new SqlCommand(balanceQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);
                var result = command.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
            }
        }
    }
}

