using TelegramThemeCreator.Enums;
using TelegramThemeCreator.Utils;

namespace TelegramThemeCreator.Models
{
    /// <summary>
    /// Represents a theme color.
    /// </summary>
    public class ThemeColor
    {
        /// <summary>
        /// Color reference to another color.
        /// </summary>
        private string colorReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeColor" /> class with specified color name and value.
        /// </summary>
        /// <param name="name">The color name.</param>
        /// <param name="value">The color value.</param>
        public ThemeColor(string name, string value)
        {
            Name = name;

            if (ColorUtils.IsColor(value))
            {
                IsColor = true;
                IsStandardColor = ColorUtils.IsStandardColor(value);

                if (value.Length == 9)
                {
                    Value = new UniColor(value, HexFormat.RGBA);
                }
                else
                {
                    Value = new UniColor(value, HexFormat.RGB);
                }
            }
            else
            {
                colorReference = value;
                IsColor = false;
                IsStandardColor = false;
            }
        }

        /// <summary>
        /// Gets or sets the name of the color.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the color.
        /// </summary>
        public UniColor Value { get; set; }

        /// <summary>
        /// Determines whether the current <see cref="ThemeColor" /> value is a color.
        /// </summary>
        public bool IsColor { get; private set; }

        /// <summary>
        /// Determines whether the current <see cref="ThemeColor" /> value is a standard color.
        /// </summary>
        public bool IsStandardColor { get; private set; }

        /// <summary>
        /// Gets a hex color or reference to another color.
        /// </summary>
        /// <returns>A string containing a hex color or reference to another color.</returns>
        public string GetHexColor()
        {
            return IsColor ? Value.ToHex() : colorReference;
        }

        /// <summary>
        /// Applies color correction according to newHue parameter.
        /// </summary>
        /// <param name="newHue">A double containing new Hue value.</param>
        public void ApplyColorChange(double newHue)
        {
            if ((Value.Hue > 160) && (Value.Hue < 180))
            {
                if (Value.SaturationV >= 0.88)
                {
                    Value.SaturationV -= 0.2;
                }

                Value.Hue = newHue;
            }
            else if (Value.SaturationV < 0.3)
            {
                Value.SaturationV = 0.05;
                if ((Value.Value > 0.15) && (Value.Value < 0.35))
                {
                    Value.Value -= 0.1;
                }

                Value.Hue /= 10;
            }
        }
    }
}
