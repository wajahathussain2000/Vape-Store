using System;
using System.Linq;

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

            // SuperAdmin or Admin always has full access
            if (role == "superadmin" || role == "super admin" || role == "admin") return true;

            // PRIORITY 1: Check cached session permissions (Performance optimization)
            try
            {
                if (CurrentUser?.Permissions != null && CurrentUser.Permissions.Count > 0)
                {
                    // If user has "*" permission, grant all access
                    if (CurrentUser.Permissions.Any(p => (p ?? string.Empty).Trim() == "*"))
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: Cached '*' wildcard permission for user {CurrentUser?.Username}");
                        return true;
                    }
                    
                    // Check if user has the specific permission
                    if (CurrentUser.Permissions.Contains(perm))
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: Cached permission '{perm}' for user {CurrentUser?.Username}");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] Error checking cached permissions: {ex.Message}");
            }

            // PRIORITY 2: Database check (if not found in cache)
            try
            {
                var roleRepo = new Vape_Store.Repositories.RoleRepository();
                
                // First try to get permissions through UserRoles
                var dbPerms = roleRepo.GetEffectivePermissionsForUser(CurrentUser?.UserID ?? 0);
                
                // If no permissions found through UserRoles, try by role name
                if ((dbPerms == null || dbPerms.Count == 0) && !string.IsNullOrWhiteSpace(role))
                {
                    dbPerms = roleRepo.GetPermissionsByRoleName(role);
                }
                
                if (dbPerms != null && dbPerms.Count > 0)
                {
                    // Update the session permissions cache
                    if (CurrentUser != null)
                    {
                        CurrentUser.Permissions = new System.Collections.Generic.HashSet<string>(
                            dbPerms.Select(p => (p ?? string.Empty).Trim().ToLower()), 
                            StringComparer.OrdinalIgnoreCase);
                    }
                    
                    // Check if user has specific permission or wildcard
                    if (dbPerms.Any(p => {
                        var normalized = (p ?? string.Empty).Trim().ToLower();
                        return normalized == perm || normalized == "*";
                    }))
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: Database permission confirmed for '{perm}'");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ERROR in database check: {ex.Message}");
            }

            // PRIORITY 3: Dynamic role manager (JSON-backed fallback)
            // This ensures that even if DB tables are missing or misconfigured, default permissions can work
            try
            {
                if (Vape_Store.Services.RoleManagerService.Instance.HasPermission(role, perm))
                {
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: RoleManagerService fallback allows '{perm}' for role '{role}'");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] Error checking RoleManagerService: {ex.Message}");
            }

            // PRIORITY 4: Final Denial
            System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✗ DENIED: Permission '{perm}' for user {CurrentUser?.Username} (Role: {role})");
            return false;
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
