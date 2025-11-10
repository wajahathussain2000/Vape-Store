using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class CashInHandRepository
    {
        public CashInHandRepository()
        {
        }

        public List<CashInHand> GetAllCashInHandTransactions()
        {
            var transactions = new List<CashInHand>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT cih.*, u.FullName as UserName
                        FROM CashInHand cih
                        LEFT JOIN Users u ON cih.UserID = u.UserID
                        ORDER BY cih.TransactionDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactions.Add(new CashInHand
                                {
                                    CashInHandID = Convert.ToInt32(reader["CashInHandID"]),
                                    TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                    OpeningCash = Convert.ToDecimal(reader["OpeningCash"]),
                                    CashIn = Convert.ToDecimal(reader["CashIn"]),
                                    CashOut = Convert.ToDecimal(reader["CashOut"]),
                                    Expenses = Convert.ToDecimal(reader["Expenses"]),
                                    ClosingCash = Convert.ToDecimal(reader["ClosingCash"]),
                                    Description = reader["Description"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving cash in hand transactions: {ex.Message}", ex);
            }

            return transactions;
        }

        public CashInHand GetCashInHandById(int cashInHandId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT cih.*, u.FullName as UserName
                        FROM CashInHand cih
                        LEFT JOIN Users u ON cih.UserID = u.UserID
                        WHERE cih.CashInHandID = @CashInHandID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CashInHandID", cashInHandId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new CashInHand
                                {
                                    CashInHandID = Convert.ToInt32(reader["CashInHandID"]),
                                    TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                    OpeningCash = Convert.ToDecimal(reader["OpeningCash"]),
                                    CashIn = Convert.ToDecimal(reader["CashIn"]),
                                    CashOut = Convert.ToDecimal(reader["CashOut"]),
                                    Expenses = Convert.ToDecimal(reader["Expenses"]),
                                    ClosingCash = Convert.ToDecimal(reader["ClosingCash"]),
                                    Description = reader["Description"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving cash in hand transaction: {ex.Message}", ex);
            }

            return null;
        }

        public CashInHand GetCashInHandByDate(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT cih.*, u.FullName as UserName
                        FROM CashInHand cih
                        LEFT JOIN Users u ON cih.UserID = u.UserID
                        WHERE CAST(cih.TransactionDate AS DATE) = CAST(@TransactionDate AS DATE)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionDate", date);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new CashInHand
                                {
                                    CashInHandID = Convert.ToInt32(reader["CashInHandID"]),
                                    TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                                    OpeningCash = Convert.ToDecimal(reader["OpeningCash"]),
                                    CashIn = Convert.ToDecimal(reader["CashIn"]),
                                    CashOut = Convert.ToDecimal(reader["CashOut"]),
                                    Expenses = Convert.ToDecimal(reader["Expenses"]),
                                    ClosingCash = Convert.ToDecimal(reader["ClosingCash"]),
                                    Description = reader["Description"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving cash in hand transaction by date: {ex.Message}", ex);
            }

            return null;
        }

        public bool AddCashInHand(CashInHand cashInHand)
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
                            var query = @"
                                INSERT INTO CashInHand (TransactionDate, OpeningCash, CashIn, CashOut, Expenses, 
                                                       ClosingCash, Description, CreatedBy, UserID, CreatedDate)
                                VALUES (@TransactionDate, @OpeningCash, @CashIn, @CashOut, @Expenses, 
                                        @ClosingCash, @Description, @CreatedBy, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@TransactionDate", cashInHand.TransactionDate);
                                command.Parameters.AddWithValue("@OpeningCash", cashInHand.OpeningCash);
                                command.Parameters.AddWithValue("@CashIn", cashInHand.CashIn);
                                command.Parameters.AddWithValue("@CashOut", cashInHand.CashOut);
                                command.Parameters.AddWithValue("@Expenses", cashInHand.Expenses);
                                command.Parameters.AddWithValue("@ClosingCash", cashInHand.ClosingCash);
                                command.Parameters.AddWithValue("@Description", cashInHand.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@CreatedBy", cashInHand.CreatedBy ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@UserID", cashInHand.UserID);
                                command.Parameters.AddWithValue("@CreatedDate", cashInHand.CreatedDate);

                                cashInHand.CashInHandID = Convert.ToInt32(command.ExecuteScalar());
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
                throw new Exception($"Error adding cash in hand transaction: {ex.Message}", ex);
            }
        }

        public bool UpdateCashInHand(CashInHand cashInHand)
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
                            var query = @"
                                UPDATE CashInHand 
                                SET TransactionDate = @TransactionDate, OpeningCash = @OpeningCash, 
                                    CashIn = @CashIn, CashOut = @CashOut, Expenses = @Expenses,
                                    ClosingCash = @ClosingCash, Description = @Description, 
                                    CreatedBy = @CreatedBy, UserID = @UserID
                                WHERE CashInHandID = @CashInHandID";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@CashInHandID", cashInHand.CashInHandID);
                                command.Parameters.AddWithValue("@TransactionDate", cashInHand.TransactionDate);
                                command.Parameters.AddWithValue("@OpeningCash", cashInHand.OpeningCash);
                                command.Parameters.AddWithValue("@CashIn", cashInHand.CashIn);
                                command.Parameters.AddWithValue("@CashOut", cashInHand.CashOut);
                                command.Parameters.AddWithValue("@Expenses", cashInHand.Expenses);
                                command.Parameters.AddWithValue("@ClosingCash", cashInHand.ClosingCash);
                                command.Parameters.AddWithValue("@Description", cashInHand.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@CreatedBy", cashInHand.CreatedBy ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@UserID", cashInHand.UserID);

                                int rowsAffected = command.ExecuteNonQuery();
                                
                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
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
                throw new Exception($"Error updating cash in hand transaction: {ex.Message}", ex);
            }
        }

        public bool DeleteCashInHand(int cashInHandId)
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
                            var query = "DELETE FROM CashInHand WHERE CashInHandID = @CashInHandID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@CashInHandID", cashInHandId);
                                int rowsAffected = command.ExecuteNonQuery();
                                
                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
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
                throw new Exception($"Error deleting cash in hand transaction: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the closing cash from the most recent transaction before the specified date
        /// This is used as the opening cash for a new transaction
        /// </summary>
        public decimal GetPreviousDayClosingCash(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Get the most recent transaction's closing cash before the specified date
                    // This ensures we get the correct opening balance for the new day
                    var query = @"
                        SELECT TOP 1 ClosingCash 
                        FROM CashInHand 
                        WHERE CAST(TransactionDate AS DATE) < CAST(@Date AS DATE)
                        ORDER BY TransactionDate DESC, CashInHandID DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date);
                        connection.Open();
                        var result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting previous day closing cash: {ex.Message}", ex);
            }
        }

        public decimal GetTotalCashInForDate(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(SUM(CashIn), 0) 
                        FROM CashInHand 
                        WHERE CAST(TransactionDate AS DATE) = CAST(@Date AS DATE)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date);
                        connection.Open();
                        return Convert.ToDecimal(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total cash in for date: {ex.Message}", ex);
            }
        }

        public decimal GetTotalCashOutForDate(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(SUM(CashOut), 0) 
                        FROM CashInHand 
                        WHERE CAST(TransactionDate AS DATE) = CAST(@Date AS DATE)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date);
                        connection.Open();
                        return Convert.ToDecimal(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total cash out for date: {ex.Message}", ex);
            }
        }

        public decimal GetTotalExpensesForDate(DateTime date)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(SUM(Expenses), 0) 
                        FROM CashInHand 
                        WHERE CAST(TransactionDate AS DATE) = CAST(@Date AS DATE)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date);
                        connection.Open();
                        return Convert.ToDecimal(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total expenses for date: {ex.Message}", ex);
            }
        }
    }
}
