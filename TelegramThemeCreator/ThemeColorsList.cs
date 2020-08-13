using System.Collections.Generic;
using System.Linq;

namespace TelegramThemeCreator
{
    /// <summary>
    /// Represents a list of theme colors.
    /// </summary>
    public class ThemeColorsList
    {
        private List<ThemeColor> themeColors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeColorsList" /> class.
        /// </summary>
        /// <param name="path">A string that contains path to source theme file.</param>
        public ThemeColorsList(string path)
        {
            themeColors = ThemeColorConverter.GetThemeColorsListFromFile(path);
        }

        /// <summary>
        /// Applies a new hue value for each required theme color.
        /// </summary>
        /// <param name="newHue">A double new hue value.</param>
        public void ApplyHue(double newHue)
        {
            foreach (var color in themeColors.Where(x => x.IsColor == true && x.IsStandardColor == false))
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
            ThemeColorConverter.SaveThemeToFile(themeColors, path);
        }
    }
}
