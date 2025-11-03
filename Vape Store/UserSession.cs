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

        // Optional: multi-role support from DB
        public System.Collections.Generic.List<string> Roles { get; set; }
        public System.Collections.Generic.HashSet<string> Permissions { get; set; }

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
            Roles = new System.Collections.Generic.List<string>();
            Permissions = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
        /// <param name="permission">The permission to check (e.g., "sales", "purchases", "inventory", "users", "reports", "backup", "settings")</param>
        /// <returns>True if user has permission, false otherwise</returns>
        public static bool HasPermission(string permission)
        {
            if (!IsLoggedIn()) 
                return false;

            // Normalize
            var role = (CurrentUser.Role ?? string.Empty).Trim().ToLower();
            var perm = (permission ?? string.Empty).Trim().ToLower();

            // Super admin: everything
            if (role == "superadmin" || role == "super admin") return true;

            // If permissions loaded from DB, prefer them
            try
            {
                if (CurrentUser?.Permissions != null && CurrentUser.Permissions.Count > 0)
                {
                    return CurrentUser.Permissions.Contains(perm) || CurrentUser.Permissions.Contains("*");
                }
            }
            catch { }

            // Dynamic role manager (JSON-backed). If it answers, use it.
            try
            {
                if (Vape_Store.Services.RoleManagerService.Instance.HasPermission(role, perm))
                {
                    return true;
                }
            }
            catch { }

            // Define role ladders and permissions
            // - admin: all except system-level settings if you want to lock that down (here we allow all app permissions)
            // - manager: sales, purchases, inventory, people, reports
            // - sales: sales, customers, basic reports
            // - cashier: sales only
            switch (role)
            {
                case "admin":
                    return true;
                case "manager":
                    return perm == "sales" || perm == "purchases" || perm == "inventory" || perm == "people" || perm == "reports";
                case "sales":
                case "seller":
                    return perm == "sales" || perm == "people" || perm == "reports" || perm == "basic_reports";
                case "cashier":
                    return perm == "sales" || perm == "basic_reports";
                default:
                    return false;
            }
        }

        /// <summary>
        /// Convenience helper to check any of the permissions
        /// </summary>
        public static bool HasAny(params string[] permissions)
        {
            if (permissions == null) return false;
            foreach (var p in permissions)
            {
                if (HasPermission(p)) return true;
            }
            return false;
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
