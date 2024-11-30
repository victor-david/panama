using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System.Linq;
using System.Windows;
using Mah = ControlzEx.Theming;

namespace Restless.Panama.Core
{
    public static class ThemeManager
    {
        private static string currentThemeId;

        public const string DefaultTheme = "Light.Cobalt";

        static ThemeManager()
        {
        }

        public static void Init()
        {
            ThemeTable table = DatabaseController.Instance.GetTable<ThemeTable>();

            foreach (Mah.Theme theme in Mah.ThemeManager.Current.Themes)
            {
                table.InsertTheme(theme.BaseColorScheme,theme.ColorScheme, theme.DisplayName, theme.Name);
            }
            table.Save();
        }

        public static void SetTheme(string themeId)
        {
            if (Mah.ThemeManager.Current.Themes.Where(t => t.Name == themeId).FirstOrDefault() is not null)
            {
                Mah.ThemeManager.Current.ChangeTheme(Application.Current, themeId);
                currentThemeId = themeId;
            }
            else
            {
                Mah.ThemeManager.Current.ChangeTheme(Application.Current, DefaultTheme);
                currentThemeId = DefaultTheme;
            }
        }

        public static void EnsureTheme()
        {
            if (string.IsNullOrEmpty(currentThemeId))
            {
                SetTheme(DefaultTheme);
            }
        }
    }
}