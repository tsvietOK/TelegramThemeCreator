using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TelegramThemeCreator
{
    /// <summary>
    /// Provides static methods for loading and saving theme files.
    /// </summary>
    public static class ThemeColorConverter
    {
        /// <summary>
        /// Gets list of theme colors from file.
        /// </summary>
        /// <param name="fileName">A string that contains source theme file name.</param>
        /// <returns>List of theme colors.</returns>
        public static List<ThemeColor> GetThemeColorsListFromFile(string fileName)
        {
            List<ThemeColor> themeColors = new List<ThemeColor>();

            string[] lines = File.ReadAllLines(fileName);

            Regex split = new Regex(@"^(.+)\:(.+)");

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"^\/\/\s.+|^\/\/|\/\/.+|\;|\s", string.Empty); // remove comments
                Match match = split.Match(lines[i]);
                if (match.Success)
                {
                    string name = match.Groups[1].Value;
                    string value = match.Groups[2].Value;
                    themeColors.Add(new ThemeColor(name, value));
                }
            }

            return themeColors;
        }

        /// <summary>
        /// Saves list of theme colors to text file.
        /// </summary>
        /// <param name="themeColors">List of theme colors.</param>
        /// <param name="filePath">A string that contains a file path.</param>
        public static void SaveThemeToFile(List<ThemeColor> themeColors, string filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                foreach (var color in themeColors)
                {
                    file.WriteLine($"{color.Name}:{color.GetHexColor()};");
                }
            }
        }
    }
}
