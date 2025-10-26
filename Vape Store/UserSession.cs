using System;

namespace Vape_Store
{
    /// <summary>
    /// Manages user session information and authentication state
    /// Provides role-based permission checking and session management
    /// </summary>
    public class UserSession
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the username for the current session
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the role of the user (Admin, Manager, Cashier)
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the time when the user logged in
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// Gets or sets the time of the last user activity
        /// </summary>
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// Gets or sets whether the current session is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the current logged-in user session
        /// </summary>
        public static UserSession CurrentUser { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UserSession class
        /// </summary>
        public UserSession()
        {
            LastActivity = DateTime.Now;
            IsActive = true;
        }

        #endregion

        #region Authentication Methods

        /// <summary>
        /// Checks if a user is currently logged in and the session is active
        /// </summary>
        /// <returns>True if user is logged in and session is active, false otherwise</returns>
        public static bool IsLoggedIn()
        {
            return CurrentUser != null && CurrentUser.IsActive;
        }

        /// <summary>
        /// Checks if the current user has the specified permission
        /// </summary>
        /// <param name="permission">The permission to check (e.g., "pos", "user_management", "reports")</param>
        /// <returns>True if user has permission, false otherwise</returns>
        public static bool HasPermission(string permission)
        {
            if (!IsLoggedIn()) 
                return false;

            // Role-based permission checking
            switch (CurrentUser.Role?.ToLower())
            {
                case "admin":
                    return true; // Admin has all permissions
                case "manager":
                    return permission != "user_management" && permission != "system_settings";
                case "cashier":
                    return permission == "pos" || permission == "customer_lookup" || permission == "basic_reports";
                default:
                    return false;
            }
        }

        #endregion

        #region Session Management

        /// <summary>
        /// Logs out the current user and clears the session
        /// </summary>
        public static void Logout()
        {
            if (CurrentUser != null)
            {
                CurrentUser.IsActive = false;
                CurrentUser = null;
            }
        }

        /// <summary>
        /// Updates the last activity time for the current user
        /// </summary>
        public static void UpdateActivity()
        {
            if (CurrentUser != null)
            {
                CurrentUser.LastActivity = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the duration of the current session
        /// </summary>
        /// <returns>TimeSpan representing the session duration, or TimeSpan.Zero if no active session</returns>
        public static TimeSpan GetSessionDuration()
        {
            if (CurrentUser != null)
            {
                return DateTime.Now - CurrentUser.LoginTime;
            }
            return TimeSpan.Zero;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets a formatted string representation of the current session
        /// </summary>
        /// <returns>String containing user information and session details</returns>
        public override string ToString()
        {
            if (CurrentUser == null)
                return "No active session";

            return $"User: {CurrentUser.FullName} ({CurrentUser.Username}) - Role: {CurrentUser.Role} - Session Duration: {GetSessionDuration():hh\\:mm\\:ss}";
        }

        /// <summary>
        /// Checks if the current session has been idle for too long
        /// </summary>
        /// <param name="idleTimeoutMinutes">Maximum idle time in minutes before session expires</param>
        /// <returns>True if session has expired due to inactivity, false otherwise</returns>
        public static bool IsSessionExpired(int idleTimeoutMinutes = 30)
        {
            if (CurrentUser == null)
                return true;

            var idleTime = DateTime.Now - CurrentUser.LastActivity;
            return idleTime.TotalMinutes > idleTimeoutMinutes;
        }

        #endregion
    }
}
