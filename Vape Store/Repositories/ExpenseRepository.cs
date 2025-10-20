using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class ExpenseRepository
    {
        public ExpenseRepository()
        {
        }

        public List<Expense> GetAllExpenses()
        {
            var expenses = new List<Expense>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT e.*, ec.CategoryName, u.FullName as UserName
                        FROM ExpenseEntries e
                        LEFT JOIN ExpenseCategories ec ON e.CategoryID = ec.CategoryID
                        LEFT JOIN Users u ON e.UserID = u.UserID
                        ORDER BY e.ExpenseDate DESC, e.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                expenses.Add(new Expense
                                {
                                    ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                    ExpenseCode = reader["ExpenseCode"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    Description = reader["Description"]?.ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    ReferenceNumber = reader["ReferenceNumber"]?.ToString(),
                                    Remarks = reader["Remarks"]?.ToString(),
                                    Status = reader["Status"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    CategoryName = reader["CategoryName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving expenses: {ex.Message}", ex);
            }

            return expenses;
        }

        public Expense GetExpenseById(int expenseId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT e.*, ec.CategoryName, u.FullName as UserName
                        FROM ExpenseEntries e
                        LEFT JOIN ExpenseCategories ec ON e.CategoryID = ec.CategoryID
                        LEFT JOIN Users u ON e.UserID = u.UserID
                        WHERE e.ExpenseID = @ExpenseID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ExpenseID", expenseId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Expense
                                {
                                    ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                    ExpenseCode = reader["ExpenseCode"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    Description = reader["Description"]?.ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    ReferenceNumber = reader["ReferenceNumber"]?.ToString(),
                                    Remarks = reader["Remarks"]?.ToString(),
                                    Status = reader["Status"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    CategoryName = reader["CategoryName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving expense: {ex.Message}", ex);
            }

            return null;
        }

        public bool AddExpense(Expense expense)
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
                                INSERT INTO ExpenseEntries (ExpenseCode, CategoryID, Description, Amount, ExpenseDate, 
                                                    PaymentMethod, ReferenceNumber, Remarks, Status, UserID, CreatedDate)
                                VALUES (@ExpenseCode, @CategoryID, @Description, @Amount, @ExpenseDate, 
                                        @PaymentMethod, @ReferenceNumber, @Remarks, @Status, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ExpenseCode", expense.ExpenseCode);
                                command.Parameters.AddWithValue("@CategoryID", expense.CategoryID);
                                command.Parameters.AddWithValue("@Description", expense.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Amount", expense.Amount);
                                command.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
                                command.Parameters.AddWithValue("@PaymentMethod", expense.PaymentMethod ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@ReferenceNumber", expense.ReferenceNumber ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Remarks", expense.Remarks ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Status", expense.Status ?? "Draft");
                                command.Parameters.AddWithValue("@UserID", expense.UserID);
                                command.Parameters.AddWithValue("@CreatedDate", expense.CreatedDate);

                                expense.ExpenseID = Convert.ToInt32(command.ExecuteScalar());
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
                throw new Exception($"Error adding expense: {ex.Message}", ex);
            }
        }

        public bool UpdateExpense(Expense expense)
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
                                UPDATE ExpenseEntries 
                                SET CategoryID = @CategoryID, Description = @Description, Amount = @Amount, 
                                    ExpenseDate = @ExpenseDate, PaymentMethod = @PaymentMethod, 
                                    ReferenceNumber = @ReferenceNumber, Remarks = @Remarks, Status = @Status,
                                    LastModifiedDate = @LastModifiedDate
                                WHERE ExpenseID = @ExpenseID";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ExpenseID", expense.ExpenseID);
                                command.Parameters.AddWithValue("@CategoryID", expense.CategoryID);
                                command.Parameters.AddWithValue("@Description", expense.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Amount", expense.Amount);
                                command.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
                                command.Parameters.AddWithValue("@PaymentMethod", expense.PaymentMethod ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@ReferenceNumber", expense.ReferenceNumber ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Remarks", expense.Remarks ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Status", expense.Status ?? "Draft");
                                command.Parameters.AddWithValue("@LastModifiedDate", DateTime.Now);

                                command.ExecuteNonQuery();
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
                throw new Exception($"Error updating expense: {ex.Message}", ex);
            }
        }

        public bool DeleteExpense(int expenseId)
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
                            var query = "DELETE FROM ExpenseEntries WHERE ExpenseID = @ExpenseID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ExpenseID", expenseId);
                                command.ExecuteNonQuery();
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
                throw new Exception($"Error deleting expense: {ex.Message}", ex);
            }
        }

        public string GetNextExpenseCode()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(MAX(CAST(SUBSTRING(ExpenseCode, 4, LEN(ExpenseCode)) AS INT)), 0) + 1
                        FROM ExpenseEntries 
                        WHERE ExpenseCode LIKE 'EXP%'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"EXP{nextNumber:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating expense code: {ex.Message}", ex);
            }
        }

        public List<Expense> SearchExpenses(string searchTerm)
        {
            var expenses = new List<Expense>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT e.*, ec.CategoryName, u.FullName as UserName
                        FROM ExpenseEntries e
                        LEFT JOIN ExpenseCategories ec ON e.CategoryID = ec.CategoryID
                        LEFT JOIN Users u ON e.UserID = u.UserID
                        WHERE e.ExpenseCode LIKE @SearchTerm 
                           OR e.Description LIKE @SearchTerm 
                           OR ec.CategoryName LIKE @SearchTerm 
                           OR e.ReferenceNumber LIKE @SearchTerm
                        ORDER BY e.ExpenseDate DESC, e.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                expenses.Add(new Expense
                                {
                                    ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                    ExpenseCode = reader["ExpenseCode"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    Description = reader["Description"]?.ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    ReferenceNumber = reader["ReferenceNumber"]?.ToString(),
                                    Remarks = reader["Remarks"]?.ToString(),
                                    Status = reader["Status"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    CategoryName = reader["CategoryName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching expenses: {ex.Message}", ex);
            }

            return expenses;
        }

        public List<Expense> GetExpensesByStatus(string status)
        {
            var expenses = new List<Expense>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT e.*, ec.CategoryName, u.FullName as UserName
                        FROM ExpenseEntries e
                        LEFT JOIN ExpenseCategories ec ON e.CategoryID = ec.CategoryID
                        LEFT JOIN Users u ON e.UserID = u.UserID
                        WHERE e.Status = @Status
                        ORDER BY e.ExpenseDate DESC, e.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                expenses.Add(new Expense
                                {
                                    ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                    ExpenseCode = reader["ExpenseCode"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    Description = reader["Description"]?.ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    ReferenceNumber = reader["ReferenceNumber"]?.ToString(),
                                    Remarks = reader["Remarks"]?.ToString(),
                                    Status = reader["Status"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    CategoryName = reader["CategoryName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving expenses by status: {ex.Message}", ex);
            }

            return expenses;
        }

        public List<Expense> GetExpensesByDateRange(DateTime fromDate, DateTime toDate)
        {
            var expenses = new List<Expense>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT e.*, ec.CategoryName, u.FullName as UserName
                        FROM ExpenseEntries e
                        LEFT JOIN ExpenseCategories ec ON e.CategoryID = ec.CategoryID
                        LEFT JOIN Users u ON e.UserID = u.UserID
                        WHERE e.ExpenseDate BETWEEN @FromDate AND @ToDate
                        ORDER BY e.ExpenseDate DESC, e.CreatedDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FromDate", fromDate);
                        command.Parameters.AddWithValue("@ToDate", toDate);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                expenses.Add(new Expense
                                {
                                    ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
                                    ExpenseCode = reader["ExpenseCode"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    Description = reader["Description"]?.ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"]),
                                    PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                    ReferenceNumber = reader["ReferenceNumber"]?.ToString(),
                                    Remarks = reader["Remarks"]?.ToString(),
                                    Status = reader["Status"]?.ToString(),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    CategoryName = reader["CategoryName"]?.ToString(),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving expenses by date range: {ex.Message}", ex);
            }

            return expenses;
        }
    }
}
