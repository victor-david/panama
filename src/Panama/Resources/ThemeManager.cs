using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mah = ControlzEx.Theming;

namespace Restless.Panama.Resources
{
    public static class ThemeManager
    {
        public static readonly List<string> Themes;

        public const string DefaultTheme = "Light.Blue";

        static ThemeManager()
        {
            Themes = new List<string>()
            {
                DefaultTheme,
                "Light.Red",
                "Light.Teal",
                "Light.Olive",
                "Light.Yellow",
                "Light.Steel",
                "Dark.Blue",
                "Dark.Red",
                "Dark.Steel",
                "Dark.Taupe"
            };
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