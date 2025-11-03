using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vape_Store.Models;
using Vape_Store.Repositories;

namespace Vape_Store
{
    public class RolesPermissionsForm : Form
    {
        private readonly RoleRepository _roleRepo;
        private readonly UserRepository _userRepo;

        private ListBox lstRoles;
        private CheckedListBox chkPermissions;
        private ListBox lstUsers;
        private Button btnSaveRolePerms;
        private Button btnSaveUserRoles;
        private Button btnAddRole;
        private TextBox txtNewRole;

        private List<Role> roles;
        private List<Permission> permissions;
        private List<User> users;

        public RolesPermissionsForm()
        {
            Text = "Roles & Permissions";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            _roleRepo = new RoleRepository();
            _userRepo = new UserRepository();

            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            lstRoles = new ListBox { Left = 10, Top = 10, Width = 250, Height = 480 };
            chkPermissions = new CheckedListBox { Left = 270, Top = 10, Width = 300, Height = 480, CheckOnClick = true };
            lstUsers = new ListBox { Left = 580, Top = 10, Width = 300, Height = 480 };

            btnSaveRolePerms = new Button { Left = 270, Top = 500, Width = 300, Height = 30, Text = "Save Role Permissions" };
            btnSaveUserRoles = new Button { Left = 580, Top = 500, Width = 300, Height = 30, Text = "Save User Roles" };

            txtNewRole = new TextBox { Left = 10, Top = 500, Width = 170, Height = 30 };
            btnAddRole = new Button { Left = 190, Top = 500, Width = 70, Height = 30, Text = "Add" };

            Controls.Add(lstRoles);
            Controls.Add(chkPermissions);
            Controls.Add(lstUsers);
            Controls.Add(btnSaveRolePerms);
            Controls.Add(btnSaveUserRoles);
            Controls.Add(txtNewRole);
            Controls.Add(btnAddRole);

            lstRoles.SelectedIndexChanged += LstRoles_SelectedIndexChanged;
            lstUsers.SelectedIndexChanged += LstUsers_SelectedIndexChanged;
            btnSaveRolePerms.Click += BtnSaveRolePerms_Click;
            btnSaveUserRoles.Click += BtnSaveUserRoles_Click;
            btnAddRole.Click += BtnAddRole_Click;
        }

        private void LoadData()
        {
            try
            {
                roles = _roleRepo.GetRoles();
                permissions = _roleRepo.GetPermissions();
                users = _userRepo.GetAllUsers();

                // Check if tables exist (empty lists usually means tables don't exist)
                if (roles.Count == 0 && permissions.Count == 0)
                {
                    var result = MessageBox.Show(
                        "The Roles and Permissions database tables have not been created yet.\n\n" +
                        "Would you like to create them now?\n\n" +
                        "A SQL script file (CreateRolesPermissionsTables.sql) will be created in the application directory.\n" +
                        "Please run this script in SQL Server Management Studio on your database.",
                        "Database Tables Missing",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
                    
                    if (result == DialogResult.Yes)
                    {
                        CreateDatabaseScript();
                        MessageBox.Show(
                            "SQL script created successfully!\n\n" +
                            "File location: CreateRolesPermissionsTables.sql\n\n" +
                            "Please run this script in SQL Server Management Studio to create the required tables, then restart the application.",
                            "Script Created",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    return;
                }

                lstRoles.Items.Clear();
                foreach (var r in roles) lstRoles.Items.Add(r);
                lstRoles.DisplayMember = nameof(Role.Name);

                chkPermissions.Items.Clear();
                foreach (var p in permissions) chkPermissions.Items.Add(p, false);
                chkPermissions.DisplayMember = nameof(Permission.Name);

                lstUsers.Items.Clear();
                foreach (var u in users) lstUsers.Items.Add(u);
                lstUsers.DisplayMember = nameof(User.FullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading data: {ex.Message}\n\n" +
                    "The Roles and Permissions tables may not exist in the database.\n" +
                    "Please create them using the SQL script that will be generated.",
                    "Error Loading Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                CreateDatabaseScript();
            }
        }

        private void CreateDatabaseScript()
        {
            try
            {
                var scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CreateRolesPermissionsTables.sql");
                var script = @"-- Roles and Permissions Database Schema
-- Run this script in SQL Server Management Studio on your database

-- Create Roles table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Roles](
        [RoleId] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [IsSystem] [bit] NOT NULL DEFAULT 0,
        [CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC)
    );
    
    -- Insert default roles
    INSERT INTO Roles (Name, IsSystem, CreatedDate) VALUES ('SuperAdmin', 1, GETDATE());
    INSERT INTO Roles (Name, IsSystem, CreatedDate) VALUES ('Admin', 1, GETDATE());
    INSERT INTO Roles (Name, IsSystem, CreatedDate) VALUES ('Manager', 1, GETDATE());
    INSERT INTO Roles (Name, IsSystem, CreatedDate) VALUES ('Cashier', 1, GETDATE());
END
GO

-- Create Permissions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Permissions](
        [PermissionId] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([PermissionId] ASC)
    );
    
    -- Insert default permissions
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('sales', 'Access to sales module', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('purchases', 'Access to purchases module', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('inventory', 'Access to inventory module', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('people', 'Access to people module (customers, suppliers)', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('users', 'Access to user management', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('accounts', 'Access to accounts module', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('reports', 'Access to all reports', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('basic_reports', 'Access to basic reports only', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('utilities', 'Access to utilities module', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('backup', 'Access to backup functionality', GETDATE());
    INSERT INTO Permissions (Name, Description, CreatedDate) VALUES ('settings', 'Access to system settings', GETDATE());
END
GO

-- Create RolePermissions junction table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RolePermissions](
        [RoleId] [int] NOT NULL,
        [PermissionId] [int] NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId] ASC, [PermissionId] ASC),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY([PermissionId]) REFERENCES [dbo].[Permissions] ([PermissionId]) ON DELETE CASCADE
    );
    
    -- Assign all permissions to SuperAdmin role
    DECLARE @SuperAdminRoleId INT = (SELECT RoleId FROM Roles WHERE Name = 'SuperAdmin');
    INSERT INTO RolePermissions (RoleId, PermissionId)
    SELECT @SuperAdminRoleId, PermissionId FROM Permissions;
END
GO

-- Create UserRoles junction table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserRoles](
        [UserId] [int] NOT NULL,
        [RoleId] [int] NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
    );
    
    -- Assign SuperAdmin role to users with Role='SuperAdmin' or 'Admin' in Users table
    DECLARE @SuperAdminRoleId2 INT = (SELECT RoleId FROM Roles WHERE Name = 'SuperAdmin');
    DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE Name = 'Admin');
    
    INSERT INTO UserRoles (UserId, RoleId)
    SELECT UserID, @SuperAdminRoleId2 FROM Users WHERE Role IN ('SuperAdmin', 'Super Admin');
    
    INSERT INTO UserRoles (UserId, RoleId)
    SELECT UserID, @AdminRoleId FROM Users WHERE Role = 'Admin' AND UserID NOT IN (SELECT UserId FROM UserRoles);
END
GO
";
                System.IO.File.WriteAllText(scriptPath, script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating script file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var role = lstRoles.SelectedItem as Role;
            if (role == null) return;

            var rolePerms = new HashSet<string>(_roleRepo.GetPermissionsForRole(role.RoleId).Select(x => (x ?? string.Empty).Trim().ToLower()));
            for (int i = 0; i < chkPermissions.Items.Count; i++)
            {
                var p = (Permission)chkPermissions.Items[i];
                chkPermissions.SetItemChecked(i, rolePerms.Contains((p.Name ?? string.Empty).Trim().ToLower()));
            }
        }

        private void LstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Highlight roles that the selected user has
            var user = lstUsers.SelectedItem as User;
            if (user == null) return;
            var userRoles = new HashSet<string>(_roleRepo.GetRolesForUser(user.UserID).Select(x => (x ?? string.Empty).Trim().ToLower()));
            for (int i = 0; i < lstRoles.Items.Count; i++)
            {
                var r = (Role)lstRoles.Items[i];
                // Use selection state to indicate membership
                lstRoles.SetSelected(i, userRoles.Contains((r.Name ?? string.Empty).Trim().ToLower()));
            }
        }

        private void BtnSaveRolePerms_Click(object sender, EventArgs e)
        {
            try
            {
                var role = lstRoles.SelectedItem as Role;
                if (role == null)
                {
                    MessageBox.Show("Select a role.");
                    return;
                }
                var selectedPermissionIds = new List<int>();
                foreach (var item in chkPermissions.CheckedItems)
                {
                    selectedPermissionIds.Add(((Permission)item).PermissionId);
                }
                _roleRepo.ReplaceRolePermissions(role.RoleId, selectedPermissionIds);
                MessageBox.Show("Role permissions saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveUserRoles_Click(object sender, EventArgs e)
        {
            try
            {
                var user = lstUsers.SelectedItem as User;
                if (user == null)
                {
                    MessageBox.Show("Select a user.");
                    return;
                }
                var selectedRoleIds = new List<int>();
                foreach (var item in lstRoles.SelectedItems)
                {
                    selectedRoleIds.Add(((Role)item).RoleId);
                }
                _roleRepo.ReplaceUserRoles(user.UserID, selectedRoleIds);
                MessageBox.Show("User roles saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddRole_Click(object sender, EventArgs e)
        {
            try
            {
                var name = (txtNewRole.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name)) return;
                var id = _roleRepo.UpsertRole(new Role { Name = name });
                txtNewRole.Text = string.Empty;
                LoadData();
                // Reselect newly added role
                var idx = roles.FindIndex(r => r.RoleId == id);
                if (idx >= 0 && idx < lstRoles.Items.Count) lstRoles.SelectedIndex = idx;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Adding Role", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


