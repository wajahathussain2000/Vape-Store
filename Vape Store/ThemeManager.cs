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
        private static Color lightBackground = Color.White;
        private static Color lightForeground = Color.Black;
        private static Color darkBackground = Color.FromArgb(45, 45, 48);
        private static Color darkForeground = Color.White;

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
            return currentTheme == Theme.Light ? Color.DodgerBlue : Color.CornflowerBlue;
        }

        public static void ApplyTheme(Form form)
        {
            if (form == null) return;

            form.BackColor = GetBackgroundColor();
            form.ForeColor = GetForegroundColor();

            ApplyThemeToControls(form.Controls);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
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
                    comboBox.BackColor = currentTheme == Theme.Light ? Color.White : Color.FromArgb(60, 60, 60);
                    comboBox.ForeColor = GetForegroundColor();
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

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        public static void ToggleTheme()
        {
            CurrentTheme = currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
        }
    }
}
