using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class ProductRepository
    {
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            string query = @"SELECT p.ProductID, p.ProductCode, p.ProductName, p.Description, p.CategoryID, p.BrandID,
                           p.PurchasePrice, p.CostPrice, p.RetailPrice, p.StockQuantity, p.ReorderLevel, p.Barcode, p.IsActive, p.CreatedDate,
                           c.CategoryName, b.BrandName
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Brands b ON p.BrandID = b.BrandID
                           WHERE p.IsActive = 1
                           ORDER BY p.ProductName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductCode = reader["ProductCode"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                BrandID = Convert.ToInt32(reader["BrandID"]),
                                PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                                CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                                RetailPrice = Convert.ToDecimal(reader["RetailPrice"]),
                                StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                                ReorderLevel = Convert.ToInt32(reader["ReorderLevel"]),
                                Barcode = reader["Barcode"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                BrandName = reader["BrandName"].ToString()
                            });
                        }
                    }
                }
            }
            
            return products;
        }
        
        public Product GetProductById(int productID)
        {
            Product product = null;
            string query = @"SELECT p.ProductID, p.ProductCode, p.ProductName, p.Description, p.CategoryID, p.BrandID,
                           p.PurchasePrice, p.CostPrice, p.RetailPrice, p.StockQuantity, p.ReorderLevel, p.Barcode, p.IsActive, p.CreatedDate,
                           c.CategoryName, b.BrandName
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Brands b ON p.BrandID = b.BrandID
                           WHERE p.ProductID = @ProductID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductCode = reader["ProductCode"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                BrandID = Convert.ToInt32(reader["BrandID"]),
                                PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                                CostPrice = Convert.ToDecimal(reader["CostPrice"]),
                                RetailPrice = Convert.ToDecimal(reader["RetailPrice"]),
                                StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                                ReorderLevel = Convert.ToInt32(reader["ReorderLevel"]),
                                Barcode = reader["Barcode"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                BrandName = reader["BrandName"].ToString()
                            };
                        }
                    }
                }
            }
            
            return product;
        }
        
        public bool AddProduct(Product product)
        {
            string query = @"INSERT INTO Products (ProductCode, ProductName, Description, CategoryID, BrandID, 
                           PurchasePrice, CostPrice, RetailPrice, StockQuantity, ReorderLevel, Barcode, IsActive) 
                           VALUES (@ProductCode, @ProductName, @Description, @CategoryID, @BrandID, 
                           @PurchasePrice, @CostPrice, @RetailPrice, @StockQuantity, @ReorderLevel, @Barcode, @IsActive)";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@BrandID", product.BrandID);
                    command.Parameters.AddWithValue("@PurchasePrice", product.PurchasePrice);
                    command.Parameters.AddWithValue("@RetailPrice", product.RetailPrice);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);
                    command.Parameters.AddWithValue("@Barcode", product.Barcode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", product.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool UpdateProduct(Product product)
        {
            string query = @"UPDATE Products SET ProductName = @ProductName, Description = @Description, 
                           CategoryID = @CategoryID, BrandID = @BrandID, PurchasePrice = @PurchasePrice, 
                           RetailPrice = @RetailPrice, StockQuantity = @StockQuantity, ReorderLevel = @ReorderLevel, 
                           Barcode = @Barcode, IsActive = @IsActive WHERE ProductID = @ProductID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", product.ProductID);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@BrandID", product.BrandID);
                    command.Parameters.AddWithValue("@PurchasePrice", product.PurchasePrice);
                    command.Parameters.AddWithValue("@RetailPrice", product.RetailPrice);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);
                    command.Parameters.AddWithValue("@Barcode", product.Barcode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", product.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool DeleteProduct(int productID)
        {
            string query = "UPDATE Products SET IsActive = 0 WHERE ProductID = @ProductID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool UpdateStock(int productID, int quantityChange)
        {
            string query = "UPDATE Products SET StockQuantity = StockQuantity + @QuantityChange WHERE ProductID = @ProductID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);
                    command.Parameters.AddWithValue("@QuantityChange", quantityChange);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateStock(int productID, int quantityChange, int bonus)
        {
            string query = "UPDATE Products SET StockQuantity = StockQuantity + @QuantityChange + @Bonus WHERE ProductID = @ProductID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);
                    command.Parameters.AddWithValue("@QuantityChange", quantityChange);
                    command.Parameters.AddWithValue("@Bonus", bonus);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public List<Product> GetLowStockProducts()
        {
            List<Product> products = new List<Product>();
            string query = @"SELECT p.ProductID, p.ProductCode, p.ProductName, p.StockQuantity, p.ReorderLevel,
                           c.CategoryName, b.BrandName
                           FROM Products p
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                           LEFT JOIN Brands b ON p.BrandID = b.BrandID
                           WHERE p.StockQuantity <= p.ReorderLevel AND p.IsActive = 1
                           ORDER BY p.StockQuantity ASC";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductCode = reader["ProductCode"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                                ReorderLevel = Convert.ToInt32(reader["ReorderLevel"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                BrandName = reader["BrandName"].ToString()
                            });
                        }
                    }
                }
            }
            
            return products;
        }

        public string GetNextProductCode()
        {
            string nextCode = "PROD001";
            
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    // Updated query to handle PROD format
                    string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(ProductCode, 5, LEN(ProductCode)) AS INT)), 0) + 1 FROM Products WHERE ProductCode LIKE 'PROD%'";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            int nextNumber = Convert.ToInt32(result);
                            nextCode = $"PROD{nextNumber:D3}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating next product code: {ex.Message}", ex);
            }

            return nextCode;
        }
    }
}
