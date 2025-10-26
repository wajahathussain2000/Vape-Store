using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class BrandRepository
    {
        public List<Brand> GetAllBrands()
        {
            List<Brand> brands = new List<Brand>();
            string query = "SELECT BrandID, BrandName, Description, IsActive, CreatedDate FROM Brands WHERE IsActive = 1 ORDER BY BrandName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            brands.Add(new Brand
                            {
                                BrandID = Convert.ToInt32(reader["BrandID"]),
                                BrandName = reader["BrandName"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            
            return brands;
        }
        
        public Brand GetBrandById(int brandID)
        {
            Brand brand = null;
            string query = "SELECT BrandID, BrandName, Description, IsActive, CreatedDate FROM Brands WHERE BrandID = @BrandID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BrandID", brandID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            brand = new Brand
                            {
                                BrandID = Convert.ToInt32(reader["BrandID"]),
                                BrandName = reader["BrandName"].ToString(),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }
            
            return brand;
        }
        
        public bool AddBrand(Brand brand)
        {
            try
            {
                string query = @"INSERT INTO Brands (BrandName, Description, IsActive, CreatedDate) 
                               VALUES (@BrandName, @Description, @IsActive, @CreatedDate)";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BrandName", brand.BrandName.Trim());
                        command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(brand.Description) ? (object)DBNull.Value : brand.Description.Trim());
                        command.Parameters.AddWithValue("@IsActive", brand.IsActive);
                        command.Parameters.AddWithValue("@CreatedDate", brand.CreatedDate);
                        
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding brand: {ex.Message}", ex);
            }
        }
        
        public bool UpdateBrand(Brand brand)
        {
            try
            {
                string query = @"UPDATE Brands SET BrandName = @BrandName, Description = @Description, 
                               IsActive = @IsActive WHERE BrandID = @BrandID";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BrandID", brand.BrandID);
                        command.Parameters.AddWithValue("@BrandName", brand.BrandName.Trim());
                        command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(brand.Description) ? (object)DBNull.Value : brand.Description.Trim());
                        command.Parameters.AddWithValue("@IsActive", brand.IsActive);
                        
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating brand: {ex.Message}", ex);
            }
        }
        
        public bool DeleteBrand(int brandID)
        {
            try
            {
                string query = "UPDATE Brands SET IsActive = 0 WHERE BrandID = @BrandID";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BrandID", brandID);
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting brand: {ex.Message}", ex);
            }
        }

        // Additional methods for enhanced functionality
        public bool BrandExists(string brandName, int excludeBrandId = -1)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Brands WHERE BrandName = @BrandName AND BrandID != @ExcludeBrandId AND IsActive = 1";
                
                using (var connection = DatabaseConnection.GetConnection())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BrandName", brandName.Trim());
                        command.Parameters.AddWithValue("@ExcludeBrandId", excludeBrandId);
                        connection.Open();
                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking brand existence: {ex.Message}", ex);
            }
        }

        public int GetBrandCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Brands WHERE IsActive = 1";
                
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
                throw new Exception($"Error getting brand count: {ex.Message}", ex);
            }
        }
    }
}
