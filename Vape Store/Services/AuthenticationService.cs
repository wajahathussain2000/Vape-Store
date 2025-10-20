using System;
using Vape_Store.Models;
using Vape_Store.Repositories;

namespace Vape_Store.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;
        
        public AuthenticationService()
        {
            _userRepository = new UserRepository();
        }
        
        public User Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("Username and password are required.");
                }
                
                var user = _userRepository.AuthenticateUser(username, password);
                
                if (user == null)
                {
                    throw new Exception("Invalid username or password.");
                }
                
                // Update last login
                _userRepository.UpdateLastLogin(user.UserID);
                
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }
        
        public bool ChangePassword(int userID, string currentPassword, string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                {
                    throw new Exception("Current password and new password are required.");
                }
                
                if (newPassword.Length < 6)
                {
                    throw new Exception("New password must be at least 6 characters long.");
                }
                
                // Verify current password
                var user = _userRepository.AuthenticateUser(GetUserById(userID).Username, currentPassword);
                if (user == null)
                {
                    throw new Exception("Current password is incorrect.");
                }
                
                // Update password
                return _userRepository.UpdateUserPassword(userID, newPassword);
            }
            catch (Exception ex)
            {
                throw new Exception($"Password change failed: {ex.Message}");
            }
        }
        
        
        public User GetUserById(int userID)
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                return users.Find(u => u.UserID == userID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get user: {ex.Message}");
            }
        }
        
        public bool IsUserActive(int userID)
        {
            try
            {
                var user = GetUserById(userID);
                return user != null && user.IsActive;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check user status: {ex.Message}");
            }
        }
        
        public bool HasPermission(int userID, string permission)
        {
            try
            {
                var user = GetUserById(userID);
                if (user == null || !user.IsActive)
                {
                    return false;
                }
                
                // Simple role-based permissions
                switch (permission.ToLower())
                {
                    case "admin":
                        return user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                    case "manager":
                        return user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                               user.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase);
                    case "cashier":
                        return user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                               user.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase) ||
                               user.Role.Equals("Cashier", StringComparison.OrdinalIgnoreCase);
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Permission check failed: {ex.Message}");
            }
        }
        
        public void Logout(int userID)
        {
            try
            {
                // Update last activity or perform logout actions
                // For now, we'll just log the action
                System.Diagnostics.Debug.WriteLine($"User {userID} logged out at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Logout failed: {ex.Message}");
            }
        }
        
        public bool ValidateUserSession(int userID)
        {
            try
            {
                return IsUserActive(userID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Session validation failed: {ex.Message}");
            }
        }
    }
}
