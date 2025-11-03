using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Vape_Store.Services
{
    /// <summary>
    /// Lightweight, dynamic role/permission store.
    /// Persists a JSON file so SuperAdmin can add/edit roles and permissions at runtime.
    /// </summary>
    public class RoleManagerService
    {
        private static readonly Lazy<RoleManagerService> _lazy = new Lazy<RoleManagerService>(() => new RoleManagerService());
        public static RoleManagerService Instance => _lazy.Value;

        private readonly string _rolesFilePath;
        private Dictionary<string, HashSet<string>> _roleToPermissions; // role -> set of permissions (lowercased)

        private RoleManagerService()
        {
            // Default path under application's directory
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _rolesFilePath = Path.Combine(baseDir, "roles.json");

            LoadOrInitialize();
        }

        public void LoadOrInitialize()
        {
            try
            {
                if (File.Exists(_rolesFilePath))
                {
                    var json = File.ReadAllText(_rolesFilePath);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json) ?? new Dictionary<string, List<string>>();
                    _roleToPermissions = dict.ToDictionary(
                        kvp => (kvp.Key ?? string.Empty).Trim().ToLower(),
                        kvp => new HashSet<string>((kvp.Value ?? new List<string>()).Select(p => (p ?? string.Empty).Trim().ToLower()))
                    );
                }
                else
                {
                    _roleToPermissions = GetDefaultRoleMap();
                    Persist();
                }
            }
            catch
            {
                // On any failure, fall back to defaults to avoid blocking the app
                _roleToPermissions = GetDefaultRoleMap();
            }
        }

        private Dictionary<string, HashSet<string>> GetDefaultRoleMap()
        {
            return new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "superadmin", new HashSet<string>(new []{ "*" }) },
                { "admin", new HashSet<string>(new []{ "sales","purchases","inventory","people","users","accounts","reports","utilities","backup","settings" }) },
                { "manager", new HashSet<string>(new []{ "sales","purchases","inventory","people","reports" }) },
                { "sales", new HashSet<string>(new []{ "sales","people","reports","basic_reports" }) },
                { "cashier", new HashSet<string>(new []{ "sales","basic_reports" }) }
            };
        }

        private void Persist()
        {
            try
            {
                var dict = _roleToPermissions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
                File.WriteAllText(_rolesFilePath, JsonConvert.SerializeObject(dict, Formatting.Indented));
            }
            catch { /* ignore persistence errors at runtime */ }
        }

        public bool HasPermission(string role, string permission)
        {
            if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(permission)) return false;
            role = role.Trim().ToLower();
            permission = permission.Trim().ToLower();

            if (role == "superadmin" || role == "super admin") return true;

            if (_roleToPermissions == null) LoadOrInitialize();
            if (!_roleToPermissions.TryGetValue(role, out var perms)) return false;
            if (perms.Contains("*")) return true;
            return perms.Contains(permission);
        }

        public IReadOnlyDictionary<string, List<string>> GetAll()
        {
            return _roleToPermissions?.ToDictionary(k => k.Key, v => v.Value.ToList()) ?? new Dictionary<string, List<string>>();
        }

        public void AddOrUpdateRole(string role, IEnumerable<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(role)) return;
            if (_roleToPermissions == null) LoadOrInitialize();
            var key = role.Trim().ToLower();
            _roleToPermissions[key] = new HashSet<string>((permissions ?? Enumerable.Empty<string>()).Select(p => (p ?? string.Empty).Trim().ToLower()));
            Persist();
        }

        public void RemoveRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return;
            if (_roleToPermissions == null) LoadOrInitialize();
            _roleToPermissions.Remove(role.Trim().ToLower());
            Persist();
        }
    }
}



