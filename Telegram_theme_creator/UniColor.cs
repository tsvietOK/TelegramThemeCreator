using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_theme_creator
{
    public class UniColor : ICloneable
    {

        private static readonly Dictionary<HexFormat, int[]> _hexFormatDic = new Dictionary<HexFormat, int[]>()
        {
            { HexFormat.RGB, new int[]{ 0, 1, 2} },
            { HexFormat.ARGB, new int[]{ 3, 0, 1, 2} },
            { HexFormat.RGBA, new int[]{ 0, 1, 2, 3} },
            { HexFormat.BGR, new int[]{ 2, 1, 0} },
            { HexFormat.ABGR, new int[]{ 3, 2, 1, 0} },
            { HexFormat.BGRA, new int[]{ 2, 1, 0, 3} }
        };

        #region RGB

        public byte Red
        {
            get => (byte)(_rgb[0] * 255);
            set => _rgb[0] = value / 255.0;
        }

        public byte Green
        {
            get => (byte)(_rgb[1] * 255);
            set => _rgb[1] = value / 255.0;
        }

        public byte Blue
        {
            get => (byte)(_rgb[2] * 255);
            set => _rgb[2] = value / 255.0;
        }

        #endregion

        #region HSV/HSB/HSL

        public double Hue
        {
            get
            {
                var max = this._rgb.Max();
                var min = this._rgb.Min();
                double h;
                if (max == min || ((_rgb[0] == _rgb[1]) && (_rgb[0] == _rgb[2])))
                    h = 0;
                else if (max == _rgb[0])
                    h = 60 * ((_rgb[1] - _rgb[2]) / (max - min));
                else if (max == _rgb[1])
                    h = 60 * (2 + (_rgb[2] - _rgb[0]) / (max - min));
                else
                    h = 60 * (4 + (_rgb[0] - _rgb[1]) / (max - min));
                if (h < 0)
                    h += 360;
                return h;
            }
            set
            {
                //if (value < 0 || value > 360)
                //    throw new ArgumentException($"Saturation value should should be inside [0,360]");
                SetHSV(value, SaturationV, Value);
            }
        }

        #region HSV/HSB

        public double SaturationV
        {
            get
            {
                var max = this._rgb.Max();
                var min = this._rgb.Min();
                double s;
                if (max == 0 || ((_rgb[0] == _rgb[1]) && (_rgb[0] == _rgb[2]) && (_rgb[0] == 0)))
                    s = 0;
                else
                    s = (max - min) / max;
                return s;
            }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                SetHSV(Hue, value, Value);
            }
        }

        public double Value
        {
            get
            {
                var max = this._rgb.Max();
                return max;
            }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                SetHSV(Hue, SaturationV, value);
            }
        }

        private void SetHSV(double h, double s, double v)
        {
            var c = v * s;
            var h_prime = h / 60;
            var x = c * (1 - Math.Abs(h_prime % 2 - 1));
            var m = v - c;
            var k = (int)h_prime;
            int x_index = 2 - Math.Abs((k + 1) % 3);
            int c_index = ((k + 1) / 2) % 3;
            int O_index = ((k + 4) / 2) % 3;

            var indexes = new StringBuilder("###");
            indexes[x_index] = 'X';
            indexes[c_index] = 'C';
            indexes[O_index] = '0';
            var a = indexes.ToString();
            _rgb[x_index] = x + m;
            _rgb[c_index] = c + m;
            _rgb[O_index] = 0 + m;
        }

        #endregion

        #region HSL

        public double SaturationL
        {
            get
            {
                var max = this._rgb.Max();
                var min = this._rgb.Min();
                double s;
                if (max == 0 || ((_rgb[0] == _rgb[1]) && (_rgb[0] == _rgb[2]) && (_rgb[0] == 0)))
                    s = 0;
                else if (min == 1 || ((_rgb[0] == _rgb[1]) && (_rgb[0] == _rgb[2]) && (_rgb[0] == 1)))
                    s = 0;
                else
                    s = (max - min) / (1 - Math.Abs(max + min - 1));
                return s;
            }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                SetHSL(Hue, value, Value);
            }
        }

        public double Lightness
        {
            get
            {
                var max = this._rgb.Max();
                var min = this._rgb.Min();
                double l = (max + min) / 2;
                return l;
            }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;
                SetHSV(Hue, SaturationL, value);
            }
        }

        private void SetHSL(double h, double s, double l)
        {
            var c = (1 - Math.Abs(2 * l - 1)) * s;
            var h_prime = h / 60;
            var x = c * (1 - Math.Abs(h_prime % 2 - 1));
            var m = l - c / 2;
            var k = (int)h_prime;
            int x_index = 2 - Math.Abs((k + 1) % 3);
            int c_index = ((k + 1) / 2) % 3;
            int O_index = ((k + 4) / 2) % 3;

            var indexes = new StringBuilder("###");
            indexes[x_index] = 'X';
            indexes[c_index] = 'C';
            indexes[O_index] = '0';
            var a = indexes.ToString();
            _rgb[x_index] = x + m;
            _rgb[c_index] = c + m;
            _rgb[O_index] = 0 + m;
        }

        #endregion

        #endregion

        public byte Alpha { get; set; }

        private double[] _rgb = new double[3];

        public UniColor(System.Drawing.Color color) : this(color.R, color.G, color.B, color.A)
        { }

        public UniColor(System.Windows.Media.Color color) : this(color.R, color.G, color.B, color.A)
        { }

        public UniColor(byte red, byte green, byte blue) : this(red, green, blue, 255)
        { }

        public UniColor(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public UniColor(string hex, HexFormat hexFormat)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            if (hex.Length != 8 && hex.Length != 6)
                throw new ArgumentException($"Unknown invalid length of input string '{hex}'");
            var b = hex.Select((x, i) => new { Value = x, Group = i / 2 })
                .GroupBy(x => x.Group)
                .Select(x => string.Join("", x))
                .Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber))
                .ToArray();
            var indexes = _hexFormatDic[hexFormat];
            if (b.Length != indexes.Length)
                throw new ArgumentException($"Invalid hex format specified '{hexFormat.ToString()}' for string '{hex}'");
            Red = b[indexes[0]];
            Green = b[indexes[1]];
            Blue = b[indexes[2]];
            if (indexes.Length == 4)
                Alpha = b[indexes[3]];
        }

        public UniColor Clone()
        {
            var result = (UniColor)this.MemberwiseClone();
            return result;
        }

        object ICloneable.Clone()
        {
            var result = new UniColor(this.Red, this.Green, this.Blue, this.Alpha);
            return result;
        }

        public System.Drawing.Color ToDrawingColor()
        {
            var result = System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
            return result;
        }

        public System.Windows.Media.Color ToMediaColor()
        {
            var result = System.Windows.Media.Color.FromArgb(Alpha, Red, Green, Blue);
            return result;
        }

        public string ToHex(HexFormat hexFormat)
        {
            var b = new byte[4];
            var indexes = _hexFormatDic[hexFormat];
            b[indexes[0]] = Red;
            b[indexes[1]] = Green;
            b[indexes[2]] = Blue;
            if (indexes.Length == 4)
                b[indexes[3]] = Alpha;
            var hex = b.Take(indexes.Length).Select(x=>x.ToString("X2"));
            var result = "#" + string.Join("", hex);
            return result;
        }
    }
}
