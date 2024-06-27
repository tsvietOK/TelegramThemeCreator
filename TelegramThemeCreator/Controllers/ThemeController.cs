using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TelegramThemeCreator.Models;

namespace TelegramThemeCreator.Controllers
{
    /// <summary>
    /// Represents a list of theme colors.
    /// </summary>
    public class ThemeController
    {
        private List<ThemeColor> colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeController" /> class.
        /// </summary>
        /// <param name="path">A string that contains path to source theme file.</param>
        public ThemeController(string path)
        {
            colors = new List<ThemeColor>();

            LoadThemeColorsFromFile(path);
        }

        /// <summary>
        /// Applies a new hue value for each required theme color.
        /// </summary>
        /// <param name="newHue">A double new hue value.</param>
        public void ApplyHue(double newHue)
        {
            foreach (var color in colors.Where(x => x.IsColor == true && x.IsStandardColor == false))
            {
                color.ApplyColorChange(newHue);
            }
        }

        /// <summary>
        /// Saves list of theme colors to file.
        /// </summary>
        /// <param name="path">A string that contains a file path.</param>
        public void SaveColors(string path)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (var color in colors)
                {
                    file.WriteLine($"{color.Name}:{color.GetHexColor()};");
                }
            }
        }

        /// <summary>
        /// Loads list of theme colors from file.
        /// </summary>
        /// <param name="path">A string that contains source theme file path.</param>
        /// <returns>List of theme colors.</returns>
        public void LoadThemeColorsFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            Regex split = new Regex(@"^([aA-zZ0-9]+)\:\s((\#)?[aA-zZ0-9]+)\;");

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = split.Match(lines[i]);
                if (match.Success)
                {
                    string name = match.Groups[1].Value;
                    string value = match.Groups[2].Value;
                    colors.Add(new ThemeColor(name, value));
                }
            }
        }
    }
}
