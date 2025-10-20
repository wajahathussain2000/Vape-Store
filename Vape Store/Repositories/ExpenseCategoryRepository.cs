using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.Models;
using Vape_Store.DataAccess;

namespace Vape_Store.Repositories
{
    public class ExpenseCategoryRepository
    {
        public ExpenseCategoryRepository()
        {
        }

        public List<ExpenseCategory> GetAllExpenseCategories()
        {
            var categories = new List<ExpenseCategory>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ec.*, u.FullName as UserName
                        FROM ExpenseCategories ec
                        LEFT JOIN Users u ON ec.UserID = u.UserID
                        ORDER BY ec.CategoryName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new ExpenseCategory
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryCode = reader["CategoryCode"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving expense categories: {ex.Message}", ex);
            }

            return categories;
        }

        public ExpenseCategory GetExpenseCategoryById(int categoryId)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ec.*, u.FullName as UserName
                        FROM ExpenseCategories ec
                        LEFT JOIN Users u ON ec.UserID = u.UserID
                        WHERE ec.CategoryID = @CategoryID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", categoryId);
                        connection.Open();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ExpenseCategory
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryCode = reader["CategoryCode"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    UserName = reader["UserName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving expense category: {ex.Message}", ex);
            }

            return null;
        }

        public bool AddExpenseCategory(ExpenseCategory category)
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
                                INSERT INTO ExpenseCategories (CategoryCode, CategoryName, Description, IsActive, UserID, CreatedDate)
                                VALUES (@CategoryCode, @CategoryName, @Description, @IsActive, @UserID, @CreatedDate);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@CategoryCode", category.CategoryCode);
                                command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                                command.Parameters.AddWithValue("@Description", category.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@IsActive", category.IsActive);
                                command.Parameters.AddWithValue("@UserID", category.UserID);
                                command.Parameters.AddWithValue("@CreatedDate", category.CreatedDate);

                                category.CategoryID = Convert.ToInt32(command.ExecuteScalar());
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
                throw new Exception($"Error adding expense category: {ex.Message}", ex);
            }
        }

        public bool UpdateExpenseCategory(ExpenseCategory category)
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
                                UPDATE ExpenseCategories 
                                SET CategoryName = @CategoryName, Description = @Description, 
                                    IsActive = @IsActive, LastModifiedDate = @LastModifiedDate
                                WHERE CategoryID = @CategoryID";

                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                                command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                                command.Parameters.AddWithValue("@Description", category.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@IsActive", category.IsActive);
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
                throw new Exception($"Error updating expense category: {ex.Message}", ex);
            }
        }

        public bool DeleteExpenseCategory(int categoryId)
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
                            // Check if category is used in expenses
                            var checkQuery = "SELECT COUNT(*) FROM ExpenseEntries WHERE CategoryID = @CategoryID";
                            using (var checkCommand = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@CategoryID", categoryId);
                                int expenseCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                                
                                if (expenseCount > 0)
                                {
                                    throw new Exception("Cannot delete category. It is being used by existing expenses.");
                                }
                            }

                            var deleteQuery = "DELETE FROM ExpenseCategories WHERE CategoryID = @CategoryID";
                            using (var deleteCommand = new SqlCommand(deleteQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@CategoryID", categoryId);
                                deleteCommand.ExecuteNonQuery();
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
                throw new Exception($"Error deleting expense category: {ex.Message}", ex);
            }
        }

        public string GetNextCategoryCode()
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ISNULL(MAX(CAST(SUBSTRING(CategoryCode, 4, LEN(CategoryCode)) AS INT)), 0) + 1
                        FROM ExpenseCategories 
                        WHERE CategoryCode LIKE 'EC%'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var nextNumber = Convert.ToInt32(command.ExecuteScalar());
                        return $"EC{nextNumber:D3}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating category code: {ex.Message}", ex);
            }
        }

        public List<ExpenseCategory> SearchExpenseCategories(string searchTerm)
        {
            var categories = new List<ExpenseCategory>();
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT ec.*, u.FullName as UserName
                        FROM ExpenseCategories ec
                        LEFT JOIN Users u ON ec.UserID = u.UserID
                        WHERE ec.CategoryName LIKE @SearchTerm 
                           OR ec.Description LIKE @SearchTerm 
                           OR ec.CategoryCode LIKE @SearchTerm
                        ORDER BY ec.CategoryName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new ExpenseCategory
                                {
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    CategoryCode = reader["CategoryCode"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    LastModifiedDate = reader["LastModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["LastModifiedDate"]) : (DateTime?)null,
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    UserName = reader["UserName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching expense categories: {ex.Message}", ex);
            }

            return categories;
        }

        public bool IsCategoryNameExists(string categoryName, int excludeCategoryId = 0)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    var query = @"
                        SELECT COUNT(*) 
                        FROM ExpenseCategories 
                        WHERE CategoryName = @CategoryName AND CategoryID != @ExcludeCategoryID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryName", categoryName);
                        command.Parameters.AddWithValue("@ExcludeCategoryID", excludeCategoryId);
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking category name existence: {ex.Message}", ex);
            }
        }
    }
}
