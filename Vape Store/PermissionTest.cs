using System;
using System.Collections.Generic;
using Vape_Store;
using Vape_Store.Models;

namespace PermissionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== RBAC Fix Verification ===");

            // Test 1: Admin Bypass
            TestAdminBypass();

            // Test 2: Cached Permissions
            TestCachedPermissions();

            // Test 3: Fallback Logic
            TestFallbackLogic();

            Console.WriteLine("\nVerification Complete.");
        }

        static void TestAdminBypass()
        {
            Console.WriteLine("\nTesting Admin Bypass:");
            UserSession.CurrentUser = new UserSession { Role = "Admin", IsActive = true };
            bool hasSales = UserSession.HasPermission("sales");
            bool hasUsers = UserSession.HasPermission("users");
            Console.WriteLine($"Role 'Admin' - Has 'sales': {hasSales} (Expected: True)");
            Console.WriteLine($"Role 'Admin' - Has 'users': {hasUsers} (Expected: True)");
        }

        static void TestCachedPermissions()
        {
            Console.WriteLine("\nTesting Cached Permissions:");
            UserSession.CurrentUser = new UserSession 
            { 
                Role = "Cashier", 
                IsActive = true,
                Permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "sales" }
            };
            bool hasSales = UserSession.HasPermission("sales");
            bool hasUsers = UserSession.HasPermission("users");
            Console.WriteLine($"Role 'Cashier' (Cache: 'sales') - Has 'sales': {hasSales} (Expected: True)");
            Console.WriteLine($"Role 'Cashier' (Cache: 'sales') - Has 'users': {hasUsers} (Expected: False/Fallback)");
        }

        static void TestFallbackLogic()
        {
            Console.WriteLine("\nTesting Fallback Logic (RoleManagerService):");
            // Assuming RoleManagerService.Instance.GetDefaultRoleMap() gives "*" to "manager"
            UserSession.CurrentUser = new UserSession { Role = "manager", IsActive = true };
            bool hasInventory = UserSession.HasPermission("inventory");
            Console.WriteLine($"Role 'manager' (Empty DB/Cache) - Has 'inventory': {hasInventory} (Expected: True via Fallback)");
        }
    }
}
