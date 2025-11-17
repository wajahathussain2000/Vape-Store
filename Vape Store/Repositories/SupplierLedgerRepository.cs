using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class SupplierLedgerRepository
    {
        public int InsertEntry(SqlConnection connection, SqlTransaction transaction, SupplierLedgerEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            decimal lastBalance = GetLatestBalance(connection, transaction, entry.SupplierID);
            entry.Balance = lastBalance + entry.Credit - entry.Debit;

            string insertQuery = @"
                INSERT INTO SupplierLedger
                (SupplierID, EntryDate, ReferenceType, ReferenceID, InvoiceNumber, Description,
                 Debit, Credit, Balance, CreatedDate)
                VALUES
                (@SupplierID, @EntryDate, @ReferenceType, @ReferenceID, @InvoiceNumber, @Description,
                 @Debit, @Credit, @Balance, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            using (var command = new SqlCommand(insertQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@SupplierID", entry.SupplierID);
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
            string deleteQuery = @"DELETE FROM SupplierLedger WHERE ReferenceType = @ReferenceType AND ReferenceID = @ReferenceID";
            using (var command = new SqlCommand(deleteQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@ReferenceType", referenceType);
                command.Parameters.AddWithValue("@ReferenceID", referenceId);
                command.ExecuteNonQuery();
            }
        }

        public decimal GetSupplierBalance(int supplierId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                return GetLatestBalance(connection, null, supplierId);
            }
        }

        public List<SupplierLedgerEntry> GetEntries(DateTime fromDate, DateTime toDate, int? supplierId = null)
        {
            var entries = new List<SupplierLedgerEntry>();
            string query = @"
                SELECT l.LedgerEntryID, l.SupplierID, l.EntryDate, l.ReferenceType, l.ReferenceID,
                       l.InvoiceNumber, l.Description, l.Debit, l.Credit, l.Balance, l.CreatedDate,
                       s.SupplierName, s.SupplierCode, s.Phone
                FROM SupplierLedger l
                INNER JOIN Suppliers s ON l.SupplierID = s.SupplierID
                WHERE l.EntryDate BETWEEN @FromDate AND @ToDate
                  AND (@SupplierID IS NULL OR l.SupplierID = @SupplierID)
                ORDER BY l.EntryDate, l.LedgerEntryID";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);
                    command.Parameters.AddWithValue("@SupplierID", (object)supplierId ?? DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entries.Add(new SupplierLedgerEntry
                            {
                                LedgerEntryID = Convert.ToInt32(reader["LedgerEntryID"]),
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                EntryDate = Convert.ToDateTime(reader["EntryDate"]),
                                ReferenceType = reader["ReferenceType"]?.ToString(),
                                ReferenceID = reader["ReferenceID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ReferenceID"]),
                                InvoiceNumber = reader["InvoiceNumber"]?.ToString(),
                                Description = reader["Description"]?.ToString(),
                                Debit = Convert.ToDecimal(reader["Debit"]),
                                Credit = Convert.ToDecimal(reader["Credit"]),
                                Balance = Convert.ToDecimal(reader["Balance"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                SupplierName = reader["SupplierName"]?.ToString(),
                                SupplierCode = reader["SupplierCode"]?.ToString(),
                                SupplierPhone = reader["Phone"]?.ToString()
                            });
                        }
                    }
                }
            }

            return entries;
        }

        public List<SupplierLedgerSummary> GetSupplierSummaries(DateTime fromDate, DateTime toDate, int? supplierId = null)
        {
            var summaries = new List<SupplierLedgerSummary>();
            string query = @"
                SELECT 
                    s.SupplierID,
                    s.SupplierCode,
                    s.SupplierName,
                    s.Phone,
                    SUM(CASE WHEN l.EntryDate BETWEEN @FromDate AND @ToDate THEN l.Debit ELSE 0 END) AS TotalDebit,
                    SUM(CASE WHEN l.EntryDate BETWEEN @FromDate AND @ToDate THEN l.Credit ELSE 0 END) AS TotalCredit,
                    (
                        SELECT TOP 1 Balance 
                        FROM SupplierLedger 
                        WHERE SupplierID = s.SupplierID AND EntryDate <= @ToDate
                        ORDER BY EntryDate DESC, LedgerEntryID DESC
                    ) AS ClosingBalance,
                    (
                        SELECT MAX(EntryDate) 
                        FROM SupplierLedger 
                        WHERE SupplierID = s.SupplierID 
                          AND ReferenceType = 'Purchase'
                    ) AS LastPurchaseDate,
                    (
                        SELECT MAX(EntryDate) 
                        FROM SupplierLedger 
                        WHERE SupplierID = s.SupplierID 
                          AND ReferenceType IN ('PurchasePayment', 'SupplierPayment', 'Payment')
                    ) AS LastPaymentDate
                FROM Suppliers s
                INNER JOIN SupplierLedger l ON l.SupplierID = s.SupplierID
                WHERE (@SupplierID IS NULL OR s.SupplierID = @SupplierID)
                GROUP BY s.SupplierID, s.SupplierCode, s.SupplierName, s.Phone
                ORDER BY s.SupplierName";

            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);
                    command.Parameters.AddWithValue("@SupplierID", (object)supplierId ?? DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            summaries.Add(new SupplierLedgerSummary
                            {
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                SupplierCode = reader["SupplierCode"]?.ToString(),
                                SupplierName = reader["SupplierName"]?.ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                TotalDebit = reader["TotalDebit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalDebit"]),
                                TotalCredit = reader["TotalCredit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalCredit"]),
                                ClosingBalance = reader["ClosingBalance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ClosingBalance"]),
                                LastPurchaseDate = reader["LastPurchaseDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastPurchaseDate"]),
                                LastPaymentDate = reader["LastPaymentDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastPaymentDate"])
                            });
                        }
                    }
                }
            }

            return summaries;
        }

        private decimal GetLatestBalance(SqlConnection connection, SqlTransaction transaction, int supplierId)
        {
            string balanceQuery = @"
                SELECT TOP 1 Balance 
                FROM SupplierLedger 
                WHERE SupplierID = @SupplierID
                ORDER BY EntryDate DESC, LedgerEntryID DESC";

            using (var command = new SqlCommand(balanceQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@SupplierID", supplierId);
                var result = command.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
            }
        }
    }
}

