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

            // SuperAdmin always has full access
            if (role == "superadmin" || role == "super admin") return true;

            // PRIORITY 1: Always check database permissions first (most important)
            // This ensures changes in Roles & Permissions form are respected immediately
            bool dbCheckPerformed = false;
            bool dbCheckResult = false;
            
            try
            {
                var roleRepo = new Vape_Store.Repositories.RoleRepository();
                
                // First try to get permissions through UserRoles (if user has role assignments)
                var dbPerms = roleRepo.GetEffectivePermissionsForUser(CurrentUser?.UserID ?? 0);
                
                // If no permissions found through UserRoles, try by role name from Users table
                if ((dbPerms == null || dbPerms.Count == 0) && !string.IsNullOrWhiteSpace(role))
                {
                    dbPerms = roleRepo.GetPermissionsByRoleName(role);
                }
                
                dbCheckPerformed = true;
                
                // DEBUG: Log what permissions were retrieved
                if (dbPerms != null && dbPerms.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] User: {CurrentUser?.Username}, Role: {role}, Requested: {perm}");
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] DB Permissions Retrieved: {string.Join(", ", dbPerms)}");
                    
                    // Update the session permissions cache for future checks
                    if (CurrentUser != null)
                    {
                        CurrentUser.Permissions = new System.Collections.Generic.HashSet<string>(
                            dbPerms.Select(p => (p ?? string.Empty).Trim().ToLower()), 
                            StringComparer.OrdinalIgnoreCase);
                    }
                    
                    // Check if user has the specific permission
                    var hasSpecificPermission = dbPerms.Any(p => (p ?? string.Empty).Trim().ToLower() == perm);
                    if (hasSpecificPermission)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: User has specific permission '{perm}'");
                        dbCheckResult = true;
                        return true; // Return immediately - don't check fallbacks
                    }
                    
                    // Check for "*" wildcard (all access) - but only if specific permission not found
                    // IMPORTANT: If you want granular control, remove "*" from the role permissions
                    var hasWildcard = dbPerms.Any(p => (p ?? string.Empty).Trim().ToLower() == "*");
                    if (hasWildcard)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: User has '*' wildcard permission");
                        dbCheckResult = true;
                        return true; // Return immediately - don't check fallbacks
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✗ DENIED: Permission '{perm}' not found in database permissions: [{string.Join(", ", dbPerms)}]");
                    dbCheckResult = false;
                    return false; // Permission not found - return false immediately
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ⚠ No permissions found in database for user {CurrentUser?.Username}, role: {role}");
                    dbCheckResult = false;
                }
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ERROR in database check: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] StackTrace: {ex.StackTrace}");
                dbCheckPerformed = true;
                dbCheckResult = false;
            }
            
            // If database check was performed and returned a result, DON'T use fallbacks
            if (dbCheckPerformed)
            {
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] Database check completed. Result: {(dbCheckResult ? "GRANTED" : "DENIED")}. Not checking fallbacks.");
                return dbCheckResult;
            }
            
            // PRIORITY 2: Fallback to cached session permissions (ONLY if DB check returned empty)
            // This is only used if database query failed or returned no results
            try
            {
                if (CurrentUser?.Permissions != null && CurrentUser.Permissions.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] Using cached permissions (DB check may have failed)");
                    
                    // If user has "*" permission, grant all access
                    if (CurrentUser.Permissions.Contains("*"))
                    {
                        System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: Cached '*' wildcard permission");
                        return true;
                    }
                    
                    // Check if user has the specific permission
                    var hasCached = CurrentUser.Permissions.Contains(perm);
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] {(hasCached ? "✓ GRANTED" : "✗ DENIED")}: Cached permission check for '{perm}'");
                    return hasCached;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] Error checking cached permissions: {ex.Message}");
            }

            // PRIORITY 3: Dynamic role manager (JSON-backed) - ONLY if no DB permissions found
            try
            {
                if (Vape_Store.Services.RoleManagerService.Instance.HasPermission(role, perm))
                {
                    System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✓ GRANTED: RoleManagerService allows '{perm}' for role '{role}'");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] Error checking RoleManagerService: {ex.Message}");
            }

            // PRIORITY 4: STRICT - No fallback defaults
            // If database permissions are not found, DENY access by default
            // This ensures that permissions MUST be explicitly set in the database
            System.Diagnostics.Debug.WriteLine($"[PERMISSION CHECK] ✗ DENIED: No database permissions found, denying access by default (strict mode)");
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
