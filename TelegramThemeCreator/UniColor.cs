using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TelegramThemeCreator.Enums;

namespace TelegramThemeCreator
{
    /// <summary>
    /// Represents a color with support of HSL and HSV color modifications.
    /// </summary>
    public class UniColor
    {
        private static readonly Dictionary<HexFormat, int[]> HexFormatDic = new Dictionary<HexFormat, int[]>()
        {
            { HexFormat.RGB,  new int[] { 0, 1, 2 } },
            { HexFormat.ARGB, new int[] { 3, 0, 1, 2 } },
            { HexFormat.RGBA, new int[] { 0, 1, 2, 3 } },
            { HexFormat.BGR,  new int[] { 2, 1, 0 } },
            { HexFormat.ABGR, new int[] { 3, 2, 1, 0 } },
            { HexFormat.BGRA, new int[] { 2, 1, 0, 3 } }
        };

        /// <summary>
        /// Double array represent current color model.
        /// </summary>
        private double[] rgb = new double[3];

        /// <summary>
        /// Initializes a new instance of the <see cref="UniColor" /> class with specified <see cref="System.Drawing.Color"/> color.
        /// </summary>
        /// <param name="color">Color value.</param>
        public UniColor(System.Drawing.Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniColor" /> class with specified <see cref="System.Windows.Media.Color"/> color.
        /// </summary>
        /// <param name="color">Color value.</param>
        public UniColor(System.Windows.Media.Color color)
            : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniColor" /> class with specified red, green and blue color values.
        /// </summary>
        /// <param name="red">A byte value of red.</param>
        /// <param name="green">A byte value of green.</param>
        /// <param name="blue">A byte value of blue.</param>
        public UniColor(byte red, byte green, byte blue)
            : this(red, green, blue, 255)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniColor" /> class with specified red, green, blue and alpha color values.
        /// </summary>
        /// <param name="red">A byte value of red.</param>
        /// <param name="green">A byte value of green.</param>
        /// <param name="blue">A byte value of blue.</param>
        /// <param name="alpha">A byte value of alpha.</param>
        public UniColor(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniColor" /> class with specified hex string color and hex color format.
        /// </summary>
        /// <param name="hexColor">A string containing color in hex format.</param>
        /// <param name="hexFormat">Hex format of color.</param>
        public UniColor(string hexColor, HexFormat hexFormat)
        {
            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1);
            }

            if (hexColor.Length != 8 && hexColor.Length != 6)
            {
                throw new ArgumentException($"Invalid length of input string '{hexColor}'");
            }

            var b = hexColor.Select((x, i) => new { Value = x, Group = i / 2 })
                .GroupBy(x => x.Group)
                .Select(x => string.Join(string.Empty, x.Select(y => y.Value)))
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray();
            var indexes = HexFormatDic[hexFormat];
            if (b.Length != indexes.Length)
            {
                throw new ArgumentException($"Invalid hex format specified '{hexFormat}' for string '{hexColor}'");
            }

            Red = b[indexes[0]];
            Green = b[indexes[1]];
            Blue = b[indexes[2]];
            if (indexes.Length == 4)
            {
                Alpha = b[indexes[3]];
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="UniColor" /> class from being created.
        /// </summary>
        private UniColor()
        {
        }

        /// <summary>
        /// Gets or sets the Alpha color value.
        /// </summary>
        public byte Alpha { get; set; } = 255;

        /// <summary>
        /// Gets or sets the Red color value.
        /// </summary>
        public byte Red
        {
            get => (byte)(rgb[0] * 255);
            set => rgb[0] = value / 255.0;
        }

        /// <summary>
        /// Gets or sets the Green color value.
        /// </summary>
        public byte Green
        {
            get => (byte)(rgb[1] * 255);
            set => rgb[1] = value / 255.0;
        }

        /// <summary>
        /// Gets or sets the Blue color value.
        /// </summary>
        public byte Blue
        {
            get => (byte)(rgb[2] * 255);
            set => rgb[2] = value / 255.0;
        }

        /// <summary>
        /// Gets or sets the Hue color value.
        /// </summary>
        public double Hue
        {
            get
            {
                var max = rgb.Max();
                var min = rgb.Min();
                double h;
                if (max == min || ((rgb[0] == rgb[1]) && (rgb[0] == rgb[2])))
                {
                    h = 0;
                }
                else if (max == rgb[0])
                {
                    h = 60 * ((rgb[1] - rgb[2]) / (max - min));
                }
                else if (max == rgb[1])
                {
                    h = 60 * (2 + ((rgb[2] - rgb[0]) / (max - min)));
                }
                else
                {
                    h = 60 * (4 + ((rgb[0] - rgb[1]) / (max - min)));
                }

                if (h < 0)
                {
                    h += 360;
                }

                return h;
            }

            set
            {
                ////if (value < 0 || value > 360)
                ////    throw new ArgumentException($"Saturation value should should be inside [0,360]");
                SetHSV(value, SaturationV, Value);
            }
        }

        /// <summary>
        /// Gets or sets the Saturation color value in HSV color model.
        /// </summary>
        public double SaturationV
        {
            get
            {
                var max = rgb.Max();
                var min = rgb.Min();
                double s;
                if (max == 0 || ((rgb[0] == rgb[1]) && (rgb[0] == rgb[2]) && (rgb[0] == 0)))
                {
                    s = 0;
                }
                else
                {
                    s = (max - min) / max;
                }

                return s;
            }

            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                SetHSV(Hue, value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the Value of the color in HSV color model.
        /// </summary>
        public double Value
        {
            get
            {
                var max = rgb.Max();
                return max;
            }

            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                SetHSV(Hue, SaturationV, value);
            }
        }

        /// <summary>
        /// Gets or sets the Saturation color value in HSL color model.
        /// </summary>
        public double SaturationL
        {
            get
            {
                var max = rgb.Max();
                var min = rgb.Min();
                double s;
                if (max == 0 || ((rgb[0] == rgb[1]) && (rgb[0] == rgb[2]) && (rgb[0] == 0)))
                {
                    s = 0;
                }
                else if (min == 1 || ((rgb[0] == rgb[1]) && (rgb[0] == rgb[2]) && (rgb[0] == 1)))
                {
                    s = 0;
                }
                else
                {
                    s = (max - min) / (1 - Math.Abs(max + min - 1));
                }

                return s;
            }

            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                SetHSL(Hue, value, Value);
            }
        }

        /// <summary>
        /// Gets or sets the Lightness color value in HSL color model.
        /// </summary>
        public double Lightness
        {
            get
            {
                var max = rgb.Max();
                var min = rgb.Min();
                return (max + min) / 2;
            }

            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                SetHSV(Hue, SaturationL, value);
            }
        }

        /// <summary>
        /// Sets color from specified hue, saturation and brightness(value).
        /// </summary>
        /// <param name="hue">A float value of Hue.</param>
        /// <param name="sat">A float value of Saturation.</param>
        /// <param name="val">A float value of Brightness.</param>
        /// <returns>Instance of the <see cref="UniColor" /> class with specified values.</returns>
        public static UniColor FromHSV(float hue, float sat, float val)
        {
            return FromHSV(hue, sat, val, 255);
        }

        /// <summary>
        /// Sets color from specified hue, saturation, brightness(value) and alpha.
        /// </summary>
        /// <param name="hue">A float value of Hue.</param>
        /// <param name="sat">A float value of Saturation.</param>
        /// <param name="val">A float value of Brightness.</param>
        /// <param name="alpha">A float value of Alpha.</param>
        /// <returns>New instance of the <see cref="UniColor" /> class with specified values.</returns>
        public static UniColor FromHSV(float hue, float sat, float val, byte alpha)
        {
            var result = new UniColor()
            {
                Alpha = alpha
            };
            result.SetHSV(hue, sat, val);
            return result;
        }

        /// <summary>
        /// Converts <see cref="UniColor" /> to <see cref="System.Drawing.Color" />.
        /// </summary>
        /// <returns><see cref="System.Drawing.Color" /></returns>
        public System.Drawing.Color ToDrawingColor()
        {
            return System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// Converts <see cref="UniColor" /> to <see cref="System.Windows.Media.Color" />.
        /// </summary>
        /// <returns><see cref="System.Windows.Media.Color" /></returns>
        public System.Windows.Media.Color ToMediaColor()
        {
            return System.Windows.Media.Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// Converts <see cref="UniColor" /> to string color in hex format.
        /// </summary>
        /// <returns>A string color in hex format.</returns>
        public string ToHex()
        {
            var color = ToDrawingColor();
            string result = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            if (color.A != 255)
            {
                result += color.A.ToString("X2");
            }

            return result;
        }

        /// <summary>
        /// Sets hue, saturation and brightness(value) to the current RGB model.
        /// </summary>
        /// <param name="h">A double hue value.</param>
        /// <param name="s">A double saturation value.</param>
        /// <param name="v">A double brightness value.</param>
        private void SetHSV(double h, double s, double v)
        {
            var c = v * s;
            var h_prime = h / 60;
            var x = c * (1 - Math.Abs((h_prime % 2) - 1));
            var m = v - c;
            var k = (int)h_prime;
            int x_index = 2 - Math.Abs((k + 1) % 3);
            int c_index = ((k + 1) / 2) % 3;
            int o_index = ((k + 4) / 2) % 3;

            var indexes = new StringBuilder("###");
            indexes[x_index] = 'X';
            indexes[c_index] = 'C';
            indexes[o_index] = '0';
            rgb[x_index] = x + m;
            rgb[c_index] = c + m;
            rgb[o_index] = 0 + m;
        }

        /// <summary>
        /// Sets hue, saturation and lightness to the current RGB model.
        /// </summary>
        /// <param name="h">A double hue value.</param>
        /// <param name="s">A double saturation value.</param>
        /// <param name="l">A double brightness value.</param>
        private void SetHSL(double h, double s, double l)
        {
            var c = (1 - Math.Abs((2 * l) - 1)) * s;
            var h_prime = h / 60;
            var x = c * (1 - Math.Abs((h_prime % 2) - 1));
            var m = l - (c / 2);
            var k = (int)h_prime;
            int x_index = 2 - Math.Abs((k + 1) % 3);
            int c_index = ((k + 1) / 2) % 3;
            int o_index = ((k + 4) / 2) % 3;

            var indexes = new StringBuilder("###");
            indexes[x_index] = 'X';
            indexes[c_index] = 'C';
            indexes[o_index] = '0';
            rgb[x_index] = x + m;
            rgb[c_index] = c + m;
            rgb[o_index] = 0 + m;
        }
    }
}
