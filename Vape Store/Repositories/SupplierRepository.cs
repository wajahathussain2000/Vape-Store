using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class SupplierRepository
    {
        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();
            string query = "SELECT SupplierID, SupplierCode, SupplierName, ContactPerson, Phone, Email, Address, City, PostalCode, IsActive, CreatedDate FROM Suppliers WHERE IsActive = 1 ORDER BY SupplierName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(new Supplier
                            {
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                SupplierCode = reader["SupplierCode"].ToString(),
                                SupplierName = reader["SupplierName"].ToString(),
                                ContactPerson = reader["ContactPerson"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                PostalCode = reader["PostalCode"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            
            return suppliers;
        }
        
        public Supplier GetSupplierById(int supplierID)
        {
            Supplier supplier = null;
            string query = "SELECT SupplierID, SupplierCode, SupplierName, ContactPerson, Phone, Email, Address, City, PostalCode, IsActive, CreatedDate FROM Suppliers WHERE SupplierID = @SupplierID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupplierID", supplierID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            supplier = new Supplier
                            {
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                SupplierCode = reader["SupplierCode"].ToString(),
                                SupplierName = reader["SupplierName"].ToString(),
                                ContactPerson = reader["ContactPerson"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                PostalCode = reader["PostalCode"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }
            
            return supplier;
        }
        
        public bool AddSupplier(Supplier supplier)
        {
            string query = @"INSERT INTO Suppliers (SupplierCode, SupplierName, ContactPerson, Phone, Email, Address, City, PostalCode, IsActive) 
                           VALUES (@SupplierCode, @SupplierName, @ContactPerson, @Phone, @Email, @Address, @City, @PostalCode, @IsActive)";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupplierCode", supplier.SupplierCode);
                    command.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                    command.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", supplier.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", supplier.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", supplier.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@City", supplier.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PostalCode", supplier.PostalCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool UpdateSupplier(Supplier supplier)
        {
            string query = @"UPDATE Suppliers SET SupplierName = @SupplierName, ContactPerson = @ContactPerson, 
                           Phone = @Phone, Email = @Email, Address = @Address, City = @City, PostalCode = @PostalCode, 
                           IsActive = @IsActive WHERE SupplierID = @SupplierID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
                    command.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                    command.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", supplier.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", supplier.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", supplier.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@City", supplier.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PostalCode", supplier.PostalCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool DeleteSupplier(int supplierID)
        {
            string query = "UPDATE Suppliers SET IsActive = 0 WHERE SupplierID = @SupplierID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupplierID", supplierID);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public string GetNextSupplierCode()
        {
            string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(SupplierCode, 4, LEN(SupplierCode)) AS INT)), 0) + 1 FROM Suppliers WHERE SupplierCode LIKE 'SUP%'";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return $"SUP{result:D3}";
                }
            }
        }

        public List<Supplier> SearchSuppliers(string searchTerm)
        {
            List<Supplier> suppliers = new List<Supplier>();
            string query = @"SELECT SupplierID, SupplierCode, SupplierName, ContactPerson, Phone, Email, Address, City, PostalCode, IsActive, CreatedDate 
                           FROM Suppliers 
                           WHERE (SupplierName LIKE @SearchTerm 
                              OR SupplierCode LIKE @SearchTerm 
                              OR ContactPerson LIKE @SearchTerm 
                              OR Phone LIKE @SearchTerm 
                              OR Email LIKE @SearchTerm 
                              OR City LIKE @SearchTerm)
                           ORDER BY SupplierName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(new Supplier
                            {
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                SupplierCode = reader["SupplierCode"].ToString(),
                                SupplierName = reader["SupplierName"].ToString(),
                                ContactPerson = reader["ContactPerson"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Email = reader["Email"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                PostalCode = reader["PostalCode"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            
            return suppliers;
        }

        public bool IsSupplierNameExists(string supplierName, int excludeSupplierId = 0)
        {
            string query = "SELECT COUNT(*) FROM Suppliers WHERE SupplierName = @SupplierName AND SupplierID != @ExcludeSupplierId";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupplierName", supplierName);
                    command.Parameters.AddWithValue("@ExcludeSupplierId", excludeSupplierId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool IsEmailExists(string email, int excludeSupplierId = 0)
        {
            string query = "SELECT COUNT(*) FROM Suppliers WHERE Email = @Email AND SupplierID != @ExcludeSupplierId";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@ExcludeSupplierId", excludeSupplierId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
