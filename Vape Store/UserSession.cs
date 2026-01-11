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
            var username = CurrentUser.Username;
            var userId = CurrentUser.UserID;

            System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] --- Checking '{perm}' for {username} (ID: {userId}, Role: '{role}') ---");

            // SuperAdmin bypass
            if (role == "superadmin" || role == "super admin" || role == "super administrator" || role == "administrator")
            {
                System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] ✓ GRANTED: System bypass for role '{role}'");
                return true;
            }

            // Expand permission check for common shorthands
            var permsToCheck = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase) { perm };
            if (perm == "sales") { permsToCheck.Add("Create Sales"); permsToCheck.Add("View Sales"); permsToCheck.Add("Edit Sales"); permsToCheck.Add("Delete Sales"); }
            if (perm == "purchases") { permsToCheck.Add("Create Purchases"); permsToCheck.Add("View Purchases"); permsToCheck.Add("Edit Purchases"); permsToCheck.Add("Delete Purchases"); }
            if (perm == "sales return") { permsToCheck.Add("Sales Return"); }
            if (perm == "purchase return") { permsToCheck.Add("Purchase Return"); }
            if (perm == "inventory") { permsToCheck.Add("Manage Inventory"); permsToCheck.Add("Products"); permsToCheck.Add("Categories"); permsToCheck.Add("Brands"); }
            if (perm == "people") { permsToCheck.Add("Manage Customers"); permsToCheck.Add("Manage Suppliers"); permsToCheck.Add("Customers"); permsToCheck.Add("Suppliers"); }
            if (perm == "users") { permsToCheck.Add("Manage Users"); permsToCheck.Add("User Access"); }
            if (perm == "accounts") { permsToCheck.Add("Manage Accounts"); permsToCheck.Add("Manage Expenses"); permsToCheck.Add("Cash in Hand"); }
            if (perm == "reports") { permsToCheck.Add("View Reports"); permsToCheck.Add("Daily Report"); permsToCheck.Add("Stock Report"); }
            if (perm == "settings") { permsToCheck.Add("Manage Settings"); }
            if (perm == "backup") { permsToCheck.Add("DataBase Backup"); }

            // PRIORITY 1: Cache check
            try
            {
                if (CurrentUser?.Permissions != null && CurrentUser.Permissions.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] PRIORITY 1: Checking cache ({CurrentUser.Permissions.Count} items)");
                    foreach (var p in permsToCheck)
                    {
                        if (CurrentUser.Permissions.Contains(p))
                        {
                            System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] ✓ GRANTED: Cached '{p}' found");
                            return true;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] Cache miss. Checking DB.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] Cache error: {ex.Message}");
            }

            // PRIORITY 2: Database check
            try
            {
                var roleRepo = new Vape_Store.Repositories.RoleRepository();
                var dbPerms = roleRepo.GetEffectivePermissionsForUser(userId);
                
                if ((dbPerms == null || dbPerms.Count == 0) && !string.IsNullOrWhiteSpace(role))
                {
                    dbPerms = roleRepo.GetPermissionsByRoleName(role);
                }
                
                if (dbPerms != null && dbPerms.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] PRIORITY 2: Found {dbPerms.Count} permissions in DB");
                    
                    // Conclusive check: if DB has permissions, we judge based on them only
                    bool found = false;
                    foreach (var p in dbPerms)
                    {
                        var norm = (p ?? "").Trim().ToLower();
                        if (norm == "*" || permsToCheck.Contains(norm)) { found = true; break; }
                    }

                    if (found)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] ✓ GRANTED: DB match found");
                        return true;
                    }

                    System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] ✗ DENIED: Conclusive DB list does not include '{perm}'");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] DB error: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine($"[RBAC DEBUG] ✗ FINAL DENIAL for {username}");
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
