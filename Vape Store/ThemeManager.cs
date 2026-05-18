using System;
using System.Drawing;
using System.Windows.Forms;

namespace Vape_Store
{
    public static class ThemeManager
    {
        public enum Theme
        {
            Light,
            Dark
        }

        private static Theme currentTheme = Theme.Light;
        private static readonly Color lightBackground = Color.White;
        private static readonly Color darkBackground = Color.FromArgb(45, 45, 48);
        private static readonly Color lightForeground = Color.FromArgb(30, 30, 30);
        private static readonly Color darkForeground = Color.White;
        private static readonly Color lightGunaComboBoxFill = Color.White;
        private static readonly Color darkGunaComboBoxFill = Color.FromArgb(60, 60, 60);
        private static readonly Color lightGunaComboBoxFore = Color.FromArgb(68, 88, 112);
        private static readonly Color darkGunaComboBoxFore = Color.White;

        public static Theme CurrentTheme
        {
            get { return currentTheme; }
            set
            {
                currentTheme = value;
                OnThemeChanged?.Invoke();
            }
        }

        public static event Action OnThemeChanged;

        public static Color GetBackgroundColor()
        {
            return currentTheme == Theme.Light ? lightBackground : darkBackground;
        }

        public static Color GetForegroundColor()
        {
            return currentTheme == Theme.Light ? lightForeground : darkForeground;
        }

        public static Color GetControlColor()
        {
            return currentTheme == Theme.Light ? Color.LightGray : Color.FromArgb(60, 60, 60);
        }

        public static Color GetAccentColor()
        {
            return currentTheme == Theme.Light ? Color.FromArgb(100, 181, 246) : Color.CornflowerBlue;
        }

        public static void ApplyTheme(Control control)
        {
            if (control == null) return;

            control.BackColor = GetBackgroundColor();
            control.ForeColor = GetForegroundColor();

            ApplyThemeToControls(control.Controls);
        }

        // Keep Form overload as a convenience helper
        public static void ApplyTheme(Form form)
        {
            ApplyTheme((Control)form);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control == null) continue;

                // CRITICAL: Skip Guna controls entirely to avoid rendering issues (black boxes, transparency bugs)
                // The user explicitly requested not to use guna styling if it causes issues.
                string typeName = control.GetType().Name;
                string typeNamespace = control.GetType().Namespace ?? "";
                
                if (typeNamespace.StartsWith("Guna") || typeName.StartsWith("Guna") || (control.Name != null && control.Name.StartsWith("guna")))
                {
                    // Still recurse into children because they might be standard controls
                    if (control.HasChildren)
                    {
                        ApplyThemeToControls(control.Controls);
                    }
                    continue;
                }

                if (control is Panel panel)
                {
                    panel.BackColor = GetBackgroundColor();
                    panel.ForeColor = GetForegroundColor();
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = GetBackgroundColor();
                    groupBox.ForeColor = GetForegroundColor();
                }
                else if (control is Label label)
                {
                    label.BackColor = GetBackgroundColor();
                    label.ForeColor = GetForegroundColor();
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = currentTheme == Theme.Light ? Color.White : Color.FromArgb(60, 60, 60);
                    textBox.ForeColor = GetForegroundColor();
                }
                else if (control is ComboBox comboBox)
                {
                    // Ensure ComboBoxes use solid backgrounds that are readable
                    comboBox.BackColor = currentTheme == Theme.Light ? Color.White : Color.FromArgb(80, 80, 80);
                    comboBox.ForeColor = currentTheme == Theme.Light ? Color.FromArgb(68, 88, 112) : Color.White;
                }
                else if (control is DataGridView dataGrid)
                {
                    dataGrid.BackgroundColor = GetBackgroundColor();
                    dataGrid.ForeColor = GetForegroundColor();
                    dataGrid.DefaultCellStyle.BackColor = currentTheme == Theme.Light ? Color.White : Color.FromArgb(60, 60, 60);
                    dataGrid.DefaultCellStyle.ForeColor = GetForegroundColor();
                    dataGrid.ColumnHeadersDefaultCellStyle.BackColor = GetControlColor();
                    dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = GetForegroundColor();
                }
                else if (control is Button button)
                {
                    button.BackColor = GetAccentColor();
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                }

                /* 
                // Apply theme to Guna2 controls
                ApplyThemeToGuna2Control(control);
                */

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        private static void ApplyThemeToGuna2Control(Control control)
        {
            // Styling of Guna2 controls disabled per user request to avoid rendering issues
        }

        private static void SetControlProperty(object obj, string propertyName, object value)
        {
            try
            {
                var prop = obj.GetType().GetProperty(propertyName);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                }
            }
            catch { /* Ignore if property doesn't exist */ }
        }

        private static object GetControlProperty(object obj, string propertyName)
        {
            try
            {
                var prop = obj.GetType().GetProperty(propertyName);
                return prop?.GetValue(obj);
            }
            catch { return null; }
        }

        public static void ToggleTheme()
        {
            CurrentTheme = currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
        }
    }
}
