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
            string query = @"INSERT INTO Brands (BrandName, Description, IsActive) 
                           VALUES (@BrandName, @Description, @IsActive)";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BrandName", brand.BrandName);
                    command.Parameters.AddWithValue("@Description", brand.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", brand.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool UpdateBrand(Brand brand)
        {
            string query = @"UPDATE Brands SET BrandName = @BrandName, Description = @Description, 
                           IsActive = @IsActive WHERE BrandID = @BrandID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BrandID", brand.BrandID);
                    command.Parameters.AddWithValue("@BrandName", brand.BrandName);
                    command.Parameters.AddWithValue("@Description", brand.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", brand.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool DeleteBrand(int brandID)
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
    }
}
