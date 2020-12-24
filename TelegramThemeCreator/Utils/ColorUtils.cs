using System;

namespace TelegramThemeCreator.Utils
{
    /// <summary>
    /// Provides static methods for color detection.
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// This method determines is provided string a color.
        /// </summary>
        /// <param name="color">Input string to check is it a color.</param>
        /// <returns>True if provided string is color; otherwise, false.</returns>
        public static bool IsColor(string color)
        {
            return color.StartsWith("#");
        }

        /// <summary>
        /// This method determines is provided string a standard color.
        /// </summary>
        /// <param name="hexColor">Input string to check is it a standard color.</param>
        /// <returns>True if provided string is standard color; otherwise, false.</returns>
        public static bool IsStandardColor(string hexColor)
        {
            return hexColor.StartsWith("#ffffff", StringComparison.OrdinalIgnoreCase)
                || hexColor.StartsWith("#000000", StringComparison.OrdinalIgnoreCase);
        }
    }
}