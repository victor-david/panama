using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides static utility methods for working with color values
    /// </summary>
    public static class ColorUtilities
    {
        /// <summary>
        /// Gets a dictionary of know colors.
        /// </summary>
        public static readonly Dictionary<string, Color> KnownColors = GetKnownColors();

        /// <summary>
        /// Extension method to get the name that corresponds to the specified color.
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>The name of the color</returns>
        public static string GetColorName(this Color color)
        {
            string colorName = KnownColors.Where(kvp => kvp.Value.Equals(color)).Select(kvp => kvp.Key).FirstOrDefault();

            if (string.IsNullOrEmpty(colorName))
                colorName = color.ToString();

            return colorName;
        }

        /// <summary>
        /// Formats a color string
        /// </summary>
        /// <param name="stringToFormat">The string to format</param>
        /// <param name="isUsingAlphaChannel">true if using alpha channel; otherwise, false</param>
        /// <returns>A format string for the color.</returns>
        public static string FormatColorString(string stringToFormat, bool isUsingAlphaChannel)
        {
            if (!isUsingAlphaChannel && (stringToFormat.Length == 9))
                return stringToFormat.Remove(1, 2);
            return stringToFormat;
        }

        private static Dictionary<string, Color> GetKnownColors()
        {
            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            return colorProperties.ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));
        }
    }
}
