using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class UserRepository
    {
        public User AuthenticateUser(string username, string password)
        {
            User user = null;
            string query = "SELECT UserID, Username, Password, FullName, Email, Phone, Role, IsActive, CreatedDate, LastLogin FROM Users WHERE Username = @Username AND Password = @Password AND IsActive = 1";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastLogin"])
                            };
                        }
                    }
                }
            }
            
            return user;
        }
        
        public void UpdateLastLogin(int userID)
        {
            string query = "UPDATE Users SET LastLogin = GETDATE() WHERE UserID = @UserID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            string query = "SELECT UserID, Username, FullName, Email, Phone, Role, IsActive, CreatedDate, LastLogin FROM Users ORDER BY FullName";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastLogin"])
                            });
                        }
                    }
                }
            }
            
            return users;
        }
        
        public bool AddUser(User user)
        {
            string query = @"INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, IsActive) 
                           VALUES (@Username, @Password, @FullName, @Email, @Phone, @Role, @IsActive)";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool UpdateUser(User user)
        {
            // Include password update if provided (for SuperAdmin)
            string query = @"UPDATE Users SET Username = @Username, FullName = @FullName, Email = @Email, 
                           Phone = @Phone, Role = @Role, IsActive = @IsActive";
            
            // If password is provided, update it (only SuperAdmin can do this)
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                query += ", Password = @Password";
            }
            
            query += " WHERE UserID = @UserID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    
                    // Add password parameter only if password is provided (SuperAdmin only)
                    if (!string.IsNullOrWhiteSpace(user.Password))
                    {
                        command.Parameters.AddWithValue("@Password", user.Password);
                    }
                    
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        
        public bool DeleteUser(int userID)
        {
            string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public User GetUserById(int userId)
        {
            User user = null;
            string query = "SELECT UserID, Username, Password, FullName, Email, Phone, Role, IsActive, CreatedDate, LastLogin FROM Users WHERE UserID = @UserID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastLogin"])
                            };
                        }
                    }
                }
            }
            
            return user;
        }

        public List<User> SearchUsers(string searchTerm)
        {
            List<User> users = new List<User>();
            string query = @"SELECT UserID, Username, FullName, Email, Phone, Role, IsActive, CreatedDate, LastLogin 
                           FROM Users 
                           WHERE Username LIKE @SearchTerm 
                              OR FullName LIKE @SearchTerm 
                              OR Email LIKE @SearchTerm 
                              OR Role LIKE @SearchTerm
                           ORDER BY FullName";
            
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
                            users.Add(new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                LastLogin = reader["LastLogin"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["LastLogin"])
                            });
                        }
                    }
                }
            }
            
            return users;
        }

        public bool IsUsernameExists(string username, int excludeUserId = 0)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND UserID != @ExcludeUserId";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@ExcludeUserId", excludeUserId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool IsEmailExists(string email, int excludeUserId = 0)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND UserID != @ExcludeUserId";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@ExcludeUserId", excludeUserId);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool UpdateUserPassword(int userId, string newPassword)
        {
            string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
            
            using (var connection = DatabaseConnection.GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@Password", newPassword);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
