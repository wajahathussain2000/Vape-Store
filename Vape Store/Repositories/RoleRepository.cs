using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vape_Store.DataAccess;
using Vape_Store.Models;

namespace Vape_Store.Repositories
{
    public class RoleRepository
    {
        // Roles
        public List<Role> GetRoles()
        {
            try
            {
                var roles = new List<Role>();
                // Use RoleName if Name is NULL, and handle CreatedDate if column doesn't exist
                const string sql = "SELECT RoleID, ISNULL(Name, RoleName) AS Name, ISNULL(IsSystem,0) AS IsSystem, GETDATE() AS CreatedDate FROM Roles ORDER BY ISNULL(Name, RoleName)";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            roles.Add(new Role
                            {
                                RoleId = Convert.ToInt32(r["RoleID"]),
                                Name = r["Name"].ToString(),
                                IsSystem = Convert.ToBoolean(r["IsSystem"]),
                                CreatedDate = Convert.ToDateTime(r["CreatedDate"]) 
                            });
                        }
                    }
                }
                return roles;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                // Tables don't exist yet - return empty list
                return new List<Role>();
            }
        }

        public int UpsertRole(Role role)
        {
            try
            {
                const string sql = @"IF EXISTS(SELECT 1 FROM Roles WHERE RoleID=@RoleId)
UPDATE Roles SET Name=@Name, RoleName=@Name WHERE RoleID=@RoleId; 
ELSE INSERT INTO Roles(RoleName, Name, IsSystem) VALUES(@Name, @Name, 0); 
SELECT ISNULL(@RoleId, SCOPE_IDENTITY());";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", role.RoleId == 0 ? (object)DBNull.Value : role.RoleId);
                    cmd.Parameters.AddWithValue("@Name", role.Name ?? string.Empty);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                throw new Exception("The Roles table does not exist. Please run the CreateRolesPermissionsTables.sql script to create the required database tables.");
            }
        }

        public void DeleteRole(int roleId)
        {
            const string sql = @"DELETE FROM RolePermissions WHERE RoleID=@RoleId; DELETE FROM UserRoles WHERE RoleID=@RoleId; DELETE FROM Roles WHERE RoleID=@RoleId;";
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Permissions
        public List<Permission> GetPermissions()
        {
            try
            {
                var permissions = new List<Permission>();
                // Use PermissionKey if Name is NULL, and handle CreatedDate if column doesn't exist
                const string sql = "SELECT PermissionID, ISNULL(Name, PermissionKey) AS Name, ISNULL(Description,'') AS Description, GETDATE() AS CreatedDate FROM Permissions ORDER BY ISNULL(Name, PermissionKey)";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            permissions.Add(new Permission
                            {
                                PermissionId = Convert.ToInt32(r["PermissionID"]),
                                Name = r["Name"].ToString(),
                                Description = r["Description"].ToString(),
                                CreatedDate = Convert.ToDateTime(r["CreatedDate"]) 
                            });
                        }
                    }
                }
                return permissions;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                // Tables don't exist yet - return empty list
                return new List<Permission>();
            }
        }

        public int UpsertPermission(Permission permission)
        {
            const string sql = @"IF EXISTS(SELECT 1 FROM Permissions WHERE PermissionID=@PermissionId)
UPDATE Permissions SET Name=@Name, Description=@Description WHERE PermissionID=@PermissionId; 
ELSE INSERT INTO Permissions(PermissionKey, Name, Description) VALUES(@Name, @Name, @Description); 
SELECT ISNULL(@PermissionId, SCOPE_IDENTITY());";
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permission.PermissionId == 0 ? (object)DBNull.Value : permission.PermissionId);
                cmd.Parameters.AddWithValue("@Name", permission.Name ?? string.Empty);
                cmd.Parameters.AddWithValue("@Description", permission.Description ?? string.Empty);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public void DeletePermission(int permissionId)
        {
            const string sql = @"DELETE FROM RolePermissions WHERE PermissionID=@PermissionId; DELETE FROM Permissions WHERE PermissionID=@PermissionId;";
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permissionId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // RolePermissions
        public List<string> GetPermissionsForRole(int roleId)
        {
            try
            {
                var list = new List<string>();
                const string sql = @"SELECT ISNULL(p.Name, p.PermissionKey) AS Name FROM RolePermissions rp 
JOIN Permissions p ON p.PermissionID = rp.PermissionID WHERE rp.RoleID = @RoleId ORDER BY ISNULL(p.Name, p.PermissionKey)";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add((r[0].ToString() ?? string.Empty).Trim());
                        }
                    }
                }
                return list;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                return new List<string>();
            }
        }

        public void ReplaceRolePermissions(int roleId, IEnumerable<int> permissionIds)
        {
            try
            {
                const string del = "DELETE FROM RolePermissions WHERE RoleID=@RoleId";
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        using (var cmdDel = new SqlCommand(del, conn, tx))
                        {
                            cmdDel.Parameters.AddWithValue("@RoleId", roleId);
                            cmdDel.ExecuteNonQuery();
                        }

                        foreach (var pid in permissionIds)
                        {
                            using (var cmdIns = new SqlCommand("INSERT INTO RolePermissions(RoleID, PermissionID) VALUES(@RoleId, @PermissionId)", conn, tx))
                            {
                                cmdIns.Parameters.AddWithValue("@RoleId", roleId);
                                cmdIns.Parameters.AddWithValue("@PermissionId", pid);
                                cmdIns.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                throw new Exception("The RolePermissions table does not exist. Please run the CreateRolesPermissionsTables.sql script to create the required database tables.");
            }
        }

        // UserRoles
        public List<string> GetRolesForUser(int userId)
        {
            try
            {
                var roles = new List<string>();
                const string sql = @"SELECT ISNULL(r.Name, r.RoleName) AS Name FROM UserRoles ur JOIN Roles r ON r.RoleID = ur.RoleID WHERE ur.UserID = @UserId ORDER BY ISNULL(r.Name, r.RoleName)";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            roles.Add((r[0].ToString() ?? string.Empty).Trim());
                        }
                    }
                }
                return roles;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                return new List<string>();
            }
        }

        public void ReplaceUserRoles(int userId, IEnumerable<int> roleIds)
        {
            try
            {
                const string del = "DELETE FROM UserRoles WHERE UserID=@UserId";
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        using (var cmdDel = new SqlCommand(del, conn, tx))
                        {
                            cmdDel.Parameters.AddWithValue("@UserId", userId);
                            cmdDel.ExecuteNonQuery();
                        }

                        foreach (var rid in roleIds)
                        {
                            using (var cmdIns = new SqlCommand("INSERT INTO UserRoles(UserID, RoleID) VALUES(@UserId,@RoleId)", conn, tx))
                            {
                                cmdIns.Parameters.AddWithValue("@UserId", userId);
                                cmdIns.Parameters.AddWithValue("@RoleId", rid);
                                cmdIns.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                throw new Exception("The UserRoles table does not exist. Please run the CreateRolesPermissionsTables.sql script to create the required database tables.");
            }
        }

        // Get permissions by role name (for users without UserRoles entries)
        public List<string> GetPermissionsByRoleName(string roleName)
        {
            try
            {
                var list = new List<string>();
                // Use case-insensitive comparison with LOWER() and TRIM to handle mismatches
                // Also check both RoleName and Name columns, and handle NULL values
                const string sql = @"SELECT DISTINCT ISNULL(p.Name, p.PermissionKey) AS Name 
FROM Roles r
JOIN RolePermissions rp ON rp.RoleID = r.RoleID 
JOIN Permissions p ON p.PermissionID = rp.PermissionID 
WHERE LOWER(LTRIM(RTRIM(ISNULL(r.RoleName, '')))) = LOWER(LTRIM(RTRIM(@RoleName))) 
   OR LOWER(LTRIM(RTRIM(ISNULL(r.Name, '')))) = LOWER(LTRIM(RTRIM(@RoleName)))
ORDER BY ISNULL(p.Name, p.PermissionKey)";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    // Normalize the role name parameter (trim and ensure it's not null)
                    var normalizedRoleName = (roleName ?? string.Empty).Trim();
                    cmd.Parameters.AddWithValue("@RoleName", normalizedRoleName);
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add((r[0].ToString() ?? string.Empty).Trim());
                        }
                    }
                }
                return list;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                return new List<string>();
            }
        }

        // Effective permissions for user
        public List<string> GetEffectivePermissionsForUser(int userId)
        {
            try
            {
                var list = new List<string>();
                
                // First, try to get permissions through UserRoles table
                const string sql = @"SELECT DISTINCT ISNULL(p.Name, p.PermissionKey) AS Name FROM UserRoles ur 
JOIN RolePermissions rp ON rp.RoleID = ur.RoleID 
JOIN Permissions p ON p.PermissionID = rp.PermissionID WHERE ur.UserID = @UserId ORDER BY ISNULL(p.Name, p.PermissionKey)";
                using (var conn = DatabaseConnection.GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add((r[0].ToString() ?? string.Empty).Trim());
                        }
                    }
                }
                
                // If no permissions found through UserRoles, try getting role from Users table
                if (list.Count == 0)
                {
                    const string userRoleSql = "SELECT Role FROM Users WHERE UserID = @UserId";
                    using (var conn = DatabaseConnection.GetConnection())
                    using (var cmd = new SqlCommand(userRoleSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        conn.Open();
                        var roleName = cmd.ExecuteScalar()?.ToString();
                        
                        if (!string.IsNullOrWhiteSpace(roleName))
                        {
                            // Normalize role name (trim whitespace)
                            roleName = roleName.Trim();
                            
                            // Get permissions by role name (now with case-insensitive matching)
                            list = GetPermissionsByRoleName(roleName);
                            
                            // Debug: Log if still no permissions found
                            if (list.Count == 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[GetEffectivePermissionsForUser] No permissions found for user {userId} with role '{roleName}'. Check if role exists in Roles table and has permissions assigned.");
                            }
                        }
                    }
                }
                
                return list;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                return new List<string>();
            }
        }
    }
}


