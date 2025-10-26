using System;

namespace Vape_Store
{
    public class UserSession
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsActive { get; set; }

        public static UserSession CurrentUser { get; set; }

        public UserSession()
        {
            LastActivity = DateTime.Now;
            IsActive = true;
        }

        public static bool IsLoggedIn()
        {
            return CurrentUser != null && CurrentUser.IsActive;
        }

        public static bool HasPermission(string permission)
        {
            if (!IsLoggedIn()) return false;

            // Simple permission check based on role
            switch (CurrentUser.Role.ToLower())
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

        public static void Logout()
        {
            if (CurrentUser != null)
            {
                CurrentUser.IsActive = false;
                CurrentUser = null;
            }
        }

        public static void UpdateActivity()
        {
            if (CurrentUser != null)
            {
                CurrentUser.LastActivity = DateTime.Now;
            }
        }

        public static TimeSpan GetSessionDuration()
        {
            if (CurrentUser != null)
            {
                return DateTime.Now - CurrentUser.LoginTime;
            }
            return TimeSpan.Zero;
        }
    }
}
