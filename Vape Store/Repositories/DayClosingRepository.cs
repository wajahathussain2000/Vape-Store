using System;
using System.Data;
using System.Data.SqlClient;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class DayClosingRepository
    {
        public DayClosingRepository()
        {
        }

        /// <summary>
        /// Gets the closing balance from the previous day
        /// Note: This is kept for backward compatibility, but new implementation always starts with 0
        /// </summary>
        public decimal GetPreviousDayClosingBalance(DateTime currentDate)
        {
            // Always return 0 for fresh start each day
            return 0;
        }

        /// <summary>
        /// Checks if a day has already been closed
        /// </summary>
        public bool IsDayClosed(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(*) 
                        FROM DayClosings 
                        WHERE CAST(ClosingDate AS DATE) = CAST(@Date AS DATE)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date);
                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // If table doesn't exist yet, day is not closed
                if (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    return false;
                }
                throw new Exception($"Error checking if day is closed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Closes a day by saving the closing balance
        /// </summary>
        public bool CloseDay(DateTime date, decimal openingBalance, decimal closingBalance, int userId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if already closed
                            var checkQuery = @"
                                SELECT COUNT(*) 
                                FROM DayClosings 
                                WHERE CAST(ClosingDate AS DATE) = CAST(@Date AS DATE)";

                            using (var checkCommand = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@Date", date);
                                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                                
                                if (count > 0)
                                {
                                    transaction.Rollback();
                                    return false; // Day already closed
                                }
                            }

                            // Insert day closing record
                            var insertQuery = @"
                                INSERT INTO DayClosings 
                                (ClosingDate, OpeningBalance, ClosingBalance, TotalSales, TotalPurchases, TotalExpenses, UserID, CreatedDate, Status)
                                VALUES 
                                (@ClosingDate, @OpeningBalance, @ClosingBalance, @TotalSales, @TotalPurchases, @TotalExpenses, @UserID, @CreatedDate, 'Closed')";

                            using (var insertCommand = new SqlCommand(insertQuery, connection, transaction))
                            {
                                // Calculate totals for the day
                                decimal totalSales = GetTotalSalesForDate(connection, transaction, date);
                                decimal totalPurchases = GetTotalPurchasesForDate(connection, transaction, date);
                                decimal totalExpenses = GetTotalExpensesForDate(connection, transaction, date);

                                insertCommand.Parameters.AddWithValue("@ClosingDate", date);
                                insertCommand.Parameters.AddWithValue("@OpeningBalance", openingBalance);
                                insertCommand.Parameters.AddWithValue("@ClosingBalance", closingBalance);
                                insertCommand.Parameters.AddWithValue("@TotalSales", totalSales);
                                insertCommand.Parameters.AddWithValue("@TotalPurchases", totalPurchases);
                                insertCommand.Parameters.AddWithValue("@TotalExpenses", totalExpenses);
                                insertCommand.Parameters.AddWithValue("@UserID", userId);
                                insertCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                                insertCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error closing day: {ex.Message}", ex);
            }
        }

        private decimal GetTotalSalesForDate(SqlConnection connection, SqlTransaction transaction, DateTime date)
        {
            try
            {
                var query = @"
                    SELECT ISNULL(SUM(TotalAmount), 0) 
                    FROM Sales 
                    WHERE CAST(SaleDate AS DATE) = CAST(@Date AS DATE)";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    return Convert.ToDecimal(command.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetTotalPurchasesForDate(SqlConnection connection, SqlTransaction transaction, DateTime date)
        {
            try
            {
                var query = @"
                    SELECT ISNULL(SUM(TotalAmount), 0) 
                    FROM Purchases 
                    WHERE CAST(PurchaseDate AS DATE) = CAST(@Date AS DATE)";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    return Convert.ToDecimal(command.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetTotalExpensesForDate(SqlConnection connection, SqlTransaction transaction, DateTime date)
        {
            try
            {
                var query = @"
                    SELECT ISNULL(SUM(Amount), 0) 
                    FROM ExpenseEntries 
                    WHERE CAST(ExpenseDate AS DATE) = CAST(@Date AS DATE)";

                using (var command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    return Convert.ToDecimal(command.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets day closing information for a specific date
        /// </summary>
        public DayClosing GetDayClosing(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT dc.*, u.FullName as UserName
                        FROM DayClosings dc
                        LEFT JOIN Users u ON dc.UserID = u.UserID
                        WHERE CAST(dc.ClosingDate AS DATE) = CAST(@Date AS DATE)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new DayClosing
                                {
                                    DayClosingID = Convert.ToInt32(reader["DayClosingID"]),
                                    ClosingDate = Convert.ToDateTime(reader["ClosingDate"]),
                                    OpeningBalance = Convert.ToDecimal(reader["OpeningBalance"]),
                                    ClosingBalance = Convert.ToDecimal(reader["ClosingBalance"]),
                                    TotalSales = reader["TotalSales"] != DBNull.Value ? Convert.ToDecimal(reader["TotalSales"]) : 0,
                                    TotalPurchases = reader["TotalPurchases"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPurchases"]) : 0,
                                    TotalExpenses = reader["TotalExpenses"] != DBNull.Value ? Convert.ToDecimal(reader["TotalExpenses"]) : 0,
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    UserName = reader["UserName"]?.ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    Status = reader["Status"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    return null;
                }
                throw new Exception($"Error getting day closing: {ex.Message}", ex);
            }

            return null;
        }
    }

    public class DayClosing
    {
        public int DayClosingID { get; set; }
        public DateTime ClosingDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalExpenses { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}

