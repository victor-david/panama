using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System.Linq;
using System.Windows;
using Mah = ControlzEx.Theming;

namespace Restless.Panama.Core
{
    public static class ThemeManager
    {
        public const string DefaultTheme = "Light.Blue";

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
            }
            else
            {
                Mah.ThemeManager.Current.ChangeTheme(Application.Current, DefaultTheme);
            }
        }
    }
}