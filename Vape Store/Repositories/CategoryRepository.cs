using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class CategoryRepository
    {
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            string query = "SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate FROM Categories WHERE IsActive = 1 ORDER BY CategoryName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            
            return categories;
        }
        
        public Category GetCategoryById(int categoryID)
        {
            Category category = null;
            string query = "SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate FROM Categories WHERE CategoryID = @CategoryID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            category = new Category
                            {
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }
            
            return category;
        }
        
        public bool AddCategory(Category category)
        {
            try
            {
                string query = @"INSERT INTO Categories (CategoryName, Description, IsActive, CreatedDate) 
                               VALUES (@CategoryName, @Description, @IsActive, @CreatedDate)";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryName", category.CategoryName.Trim());
                        command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(category.Description) ? (object)DBNull.Value : category.Description.Trim());
                        command.Parameters.AddWithValue("@IsActive", category.IsActive);
                        command.Parameters.AddWithValue("@CreatedDate", category.CreatedDate);
                        
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding category: {ex.Message}", ex);
            }
        }
        
        public bool UpdateCategory(Category category)
        {
            try
            {
                string query = @"UPDATE Categories SET CategoryName = @CategoryName, Description = @Description, 
                               IsActive = @IsActive WHERE CategoryID = @CategoryID";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                        command.Parameters.AddWithValue("@CategoryName", category.CategoryName.Trim());
                        command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(category.Description) ? (object)DBNull.Value : category.Description.Trim());
                        command.Parameters.AddWithValue("@IsActive", category.IsActive);
                        
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}", ex);
            }
        }
        
        public bool DeleteCategory(int categoryID)
        {
            try
            {
                string query = "UPDATE Categories SET IsActive = 0 WHERE CategoryID = @CategoryID";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", categoryID);
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}", ex);
            }
        }

        // Additional methods for enhanced functionality
        public bool CategoryExists(string categoryName, int excludeCategoryId = -1)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Categories WHERE CategoryName = @CategoryName AND CategoryID != @ExcludeCategoryId AND IsActive = 1";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryName", categoryName.Trim());
                        command.Parameters.AddWithValue("@ExcludeCategoryId", excludeCategoryId);
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking category existence: {ex.Message}", ex);
            }
        }

        public int GetCategoryCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Categories WHERE IsActive = 1";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting category count: {ex.Message}", ex);
            }
        }
    }
}
