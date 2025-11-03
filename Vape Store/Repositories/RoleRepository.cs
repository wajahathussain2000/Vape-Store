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
                const string sql = "SELECT RoleId, Name, ISNULL(IsSystem,0) AS IsSystem, ISNULL(CreatedDate,GETDATE()) AS CreatedDate FROM Roles ORDER BY Name";
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
                                RoleId = Convert.ToInt32(r["RoleId"]),
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
                const string sql = @"IF EXISTS(SELECT 1 FROM Roles WHERE RoleId=@RoleId)
UPDATE Roles SET Name=@Name WHERE RoleId=@RoleId; 
ELSE INSERT INTO Roles(Name, IsSystem, CreatedDate) VALUES(@Name, 0, GETDATE()); 
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
            const string sql = @"DELETE FROM RolePermissions WHERE RoleId=@RoleId; DELETE FROM UserRoles WHERE RoleId=@RoleId; DELETE FROM Roles WHERE RoleId=@RoleId;";
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
                const string sql = "SELECT PermissionId, Name, ISNULL(Description,'') AS Description, ISNULL(CreatedDate,GETDATE()) AS CreatedDate FROM Permissions ORDER BY Name";
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
                                PermissionId = Convert.ToInt32(r["PermissionId"]),
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
            const string sql = @"IF EXISTS(SELECT 1 FROM Permissions WHERE PermissionId=@PermissionId)
UPDATE Permissions SET Name=@Name, Description=@Description WHERE PermissionId=@PermissionId; 
ELSE INSERT INTO Permissions(Name, Description, CreatedDate) VALUES(@Name, @Description, GETDATE()); 
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
            const string sql = @"DELETE FROM RolePermissions WHERE PermissionId=@PermissionId; DELETE FROM Permissions WHERE PermissionId=@PermissionId;";
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
                const string sql = @"SELECT p.Name FROM RolePermissions rp 
JOIN Permissions p ON p.PermissionId = rp.PermissionId WHERE rp.RoleId=@RoleId ORDER BY p.Name";
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
                const string del = "DELETE FROM RolePermissions WHERE RoleId=@RoleId";
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
                            using (var cmdIns = new SqlCommand("INSERT INTO RolePermissions(RoleId, PermissionId) VALUES(@RoleId, @PermissionId)", conn, tx))
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
                const string sql = @"SELECT r.Name FROM UserRoles ur JOIN Roles r ON r.RoleId = ur.RoleId WHERE ur.UserId=@UserId ORDER BY r.Name";
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
                const string del = "DELETE FROM UserRoles WHERE UserId=@UserId";
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
                            using (var cmdIns = new SqlCommand("INSERT INTO UserRoles(UserId, RoleId) VALUES(@UserId,@RoleId)", conn, tx))
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

        // Effective permissions for user
        public List<string> GetEffectivePermissionsForUser(int userId)
        {
            try
            {
                var list = new List<string>();
                const string sql = @"SELECT DISTINCT p.Name FROM UserRoles ur 
JOIN RolePermissions rp ON rp.RoleId = ur.RoleId 
JOIN Permissions p ON p.PermissionId = rp.PermissionId WHERE ur.UserId=@UserId ORDER BY p.Name";
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
                return list;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("Invalid column name"))
            {
                return new List<string>();
            }
        }
    }
}


