using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class CustomerRepository
    {
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            string query = "SELECT CustomerID, CustomerCode, CustomerName, Phone, Email, Address, City, PostalCode, IsActive, CreatedDate FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerCode = reader["CustomerCode"].ToString(),
                                CustomerName = reader["CustomerName"].ToString(),
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
            
            return customers;
        }
        
        public Customer GetCustomerById(int customerID)
        {
            Customer customer = null;
            string query = "SELECT CustomerID, CustomerCode, CustomerName, Phone, Email, Address, City, PostalCode, IsActive, CreatedDate FROM Customers WHERE CustomerID = @CustomerID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customer = new Customer
                            {
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                CustomerCode = reader["CustomerCode"].ToString(),
                                CustomerName = reader["CustomerName"].ToString(),
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
            
            return customer;
        }
        
        public bool AddCustomer(Customer customer)
        {
            string query = @"INSERT INTO Customers (CustomerCode, CustomerName, Phone, Email, Address, City, PostalCode, IsActive) 
                           VALUES (@CustomerCode, @CustomerName, @Phone, @Email, @Address, @City, @PostalCode, @IsActive)";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerCode", customer.CustomerCode);
                    command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    command.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@City", customer.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PostalCode", customer.PostalCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", customer.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool UpdateCustomer(Customer customer)
        {
            string query = @"UPDATE Customers SET CustomerName = @CustomerName, Phone = @Phone, Email = @Email, 
                           Address = @Address, City = @City, PostalCode = @PostalCode, IsActive = @IsActive 
                           WHERE CustomerID = @CustomerID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    command.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@City", customer.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PostalCode", customer.PostalCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", customer.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool DeleteCustomer(int customerID)
        {
            string query = "UPDATE Customers SET IsActive = 0 WHERE CustomerID = @CustomerID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public string GetNextCustomerCode()
        {
            string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(CustomerCode, 5, LEN(CustomerCode)) AS INT)), 0) + 1 FROM Customers WHERE CustomerCode LIKE 'CUST%'";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return $"CUST{result:D3}";
                }
            }
        }
    }
}
