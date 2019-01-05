using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;

namespace Telegram_theme_creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_Initialized(object sender, EventArgs e)
        {
            double a = 255;
            double s = 1;
            double b = 1;
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 0);
            int n = 360;
            for (int i = 0; i < n; i++)
            {
                gradient.GradientStops.Add(new GradientStop(HsbToRgb(a, i, s, b), i / 360f));
            }

            RainbowRectangle.Fill = gradient;
        }

        public static Color HsbToRgb(double alpha, double hue, double saturation, double brightness)
        {
            if (hue < 0 || saturation < 0 || brightness < 0)
                throw new ArgumentException($"One of the arguments is negative, hue: {hue}, saturation: {saturation}, brightness: {brightness}");
            if (saturation > 1 || brightness > 1)
                throw new ArgumentException($"One of the arguments is too high");
            if (hue >= 360)
            {
                hue -= 360;
            }
            var c = brightness * saturation;
            var x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            var m = brightness - c;
            var k = (int)hue / 60;
            var rgb = new int[3];
            rgb[2 - Math.Abs((k + 1) % 3)] = (int)((x + m) * 255);
            rgb[((k + 1) / 2) % 3] = (int)((c + m) * 255);
            rgb[((k + 4) / 2) % 3] = (int)((0 + m) * 255);
            return Color.FromArgb((byte)alpha, (byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GetSystemAccentButton_Initialized(object sender, EventArgs e)
        {
            GetSystemAccentButton.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        }

        private void CreateThemeButton_Initialized(object sender, EventArgs e)
        {
            CreateThemeButton.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        }

        private void RainbowRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Point pointToWindow = Mouse.GetPosition(RainbowRectangle);
            HueValue.Text = pointToWindow.X.ToString();
            if (int.Parse(HueValue.Text) < 0)
                HueValue.Text = "0";
            if (int.Parse(HueValue.Text) > 360)
                HueValue.Text = "360";
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                ChangeColor();
            }
        }

        private void RainbowRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeColor();
        }

        public void ChangeColor()
        {
            double color = double.Parse(HueValue.Text);
            ColorSquare.Fill = new SolidColorBrush(HsbToRgb(255, color, 1, 1));
            DrawSelector(color);
            HexColorBlock.Text = RgbToHex(((SolidColorBrush)ColorSquare.Fill).Color);
        }

        public void DrawSelector(double color)
        {
            canvas.Children.Clear();
            Rectangle Selector = new Rectangle
            {
                Width = 8,
                Height = 40,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };
            canvas.Children.Add(Selector);
            Canvas.SetTop(Selector, 0);
            Canvas.SetLeft(Selector, color - 2);
        }

        private void GetSystemAccentButton_Click(object sender, RoutedEventArgs e)
        {
            string regValInt = ((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null)).ToString("x");
            string alpha = regValInt.Substring(0, 2);
            string blue = regValInt.Substring(2, 2); //parse hex color and convert to rgb
            string green = regValInt.Substring(4, 2);
            string red = regValInt.Substring(6, 2);
            string newHex = red + green + blue + alpha;

            byte intRed = byte.Parse(red, NumberStyles.AllowHexSpecifier);
            byte intGreen = byte.Parse(green, NumberStyles.AllowHexSpecifier);
            byte intBlue = byte.Parse(blue, NumberStyles.AllowHexSpecifier);
            byte intAlpha = byte.Parse(alpha, NumberStyles.AllowHexSpecifier);

            ColorSquare.Fill = new SolidColorBrush(Color.FromArgb(intAlpha, intRed, intGreen, intBlue));
            HSBA hsba = HexToHsba("#" + newHex);
            HueValue.Text = hsba.H.ToString("000");
            HexColorBlock.Text = RgbToHex(((SolidColorBrush)ColorSquare.Fill).Color);

            DrawSelector(hsba.H);
        }

        public static HSBA HexToHsba(string hexColor)
        {
            Color color = HexToRgb(hexColor); //convert rgb to hsb
            float max = Max(color.R, color.G, color.B);
            float min = Min(color.R, color.G, color.B);
            int alpha = color.A;
            HSBA hsba = new HSBA();
            hsba.H = GetHue(color);
            hsba.S = (max == 0) ? 0 : 1d - (1d * min / max);
            hsba.B = max / 255d;
            hsba.A = alpha;
            return hsba;
        }

        public static float GetHue(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B).GetHue();
        }

        /*public static double GetHue(Color color)
        {
            double hue = 0;
            float max = Max(color.R, color.G, color.B);
            float min = Min(color.R, color.G, color.B);
            int alpha = color.A;
            if (max == min || ((color.R == color.G) && (color.R == color.B)))
                hue = 0;
            else if (max == color.R)
                hue = 60 * ((color.G - color.B) / (max - min));
            else if (max == color.G)
                hue = 60 * (2 + (color.B - color.R) / (max - min));
            else
                hue = 60 * (4 + (color.R - color.G) / (max - min));
            if (hue < 0)
                hue += 360;
            return hue;
        }*/

        public static Color HexToRgb(string hexColor)
        {
            hexColor = hexColor.Replace(@"#", ""); //remove #
            byte alpha = 255;
            byte red = byte.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier); //parse hex color and convert to rgb
            byte green = byte.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            byte blue = byte.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            if (hexColor.Length == 8)
            {
                alpha = byte.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
            }

            return Color.FromArgb(alpha, red, green, blue);
        }

        public static int Max(int x, int y, int z)
        {
            int max = Math.Max(x, Math.Max(y, z));
            return max;
        }

        public static int Min(int x, int y, int z)
        {
            int min = Math.Min(x, Math.Min(y, z));
            return min;
        }

        public static string RgbToHex(Color color)
        {
            string hex = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            if (color.A != 255)
            {
                hex = hex + color.A.ToString("X2"); //convert rgb to hex
            }
            return hex;
        }

        private void MainWindow1_Initialized(object sender, EventArgs e)
        {
            var regValInt = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null);
            if (regValInt == null) GetSystemAccentButton.Visibility = Visibility.Hidden;
            CheckFile(@"Newtonsoft.Json.dll");
            CheckFile(@"colors.tdesktop-palette");
            ChangeColor();
        }

        private void CreateThemeButton_Click(object sender, RoutedEventArgs e)
        {
            string original_theme_file_name = @"colors.tdesktop-palette";
            string output_folder_path = @"Output\";
            string new_theme_file_name = @"colors.tdesktop-theme";
            string new_zip_file_name = @"Your_theme.tdesktop-theme";
            string new_theme_file_path = output_folder_path + new_theme_file_name;
            if (Directory.Exists(output_folder_path) == false)
            {
                Directory.CreateDirectory(output_folder_path);
            }
            if (File.Exists(new_theme_file_path))
                File.Delete(new_theme_file_path);
            if (File.Exists(new_zip_file_name))
                File.Delete(new_zip_file_name);
            if (File.Exists(output_folder_path + "tiled.jpg"))
                File.Delete(output_folder_path + "tiled.jpg");
            if (File.Exists(new_theme_file_name))
                File.Delete(new_theme_file_name);
            
            string[] lines = File.ReadAllLines(original_theme_file_name);

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"^\/\/\s.+", string.Empty); //remove comment
                lines[i] = Regex.Replace(lines[i], @"^\/\/", string.Empty); //remove comment
                
                lines[i] = lines[i].Replace(";", "\","); // change ; to ",
                lines[i] = lines[i].Replace(": ", "\": \""); //change : to ": "
                
                lines[i] = lines[i].Trim();
            }
            lines = lines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0;  i < lines.Length; i++)
            {
                lines[i] = "\"" + lines[i];
            }
            lines[lines.Length - 1] = lines[lines.Length - 1].Replace(",", "") + Environment.NewLine + "}";
            var themeList = new List<string>();
            themeList.Add("{");
            themeList.AddRange(lines);
            string themeJson = string.Join(Environment.NewLine, themeList);

            JObject o = JObject.Parse(themeJson);

            double new_hue = GetHue(((SolidColorBrush)ColorSquare.Fill).Color); //user hue

            foreach (KeyValuePair<string, Newtonsoft.Json.Linq.JToken> hex_color in o)
            {
                string value = hex_color.Value.Value<string>();
                if ((IsColor(value) == true) && (StandartColor(value) == false))
                {
                    HSBA hsba = HexToHsba(value);

                    if ((hsba.H > 160) && (hsba.H < 180))
                    {
                        if (hsba.S >= 0.88)
                            hsba.S -= 0.2;
                        Color new_color = HsbToRgb(hsba.A, new_hue, hsba.S, hsba.B); //convert hsb to rgb with new hue
                        string new_hex_color = RgbToHex(new_color);
                        o[hex_color.Key] = new_hex_color;
                    }

                    else if (hsba.S < 0.3)
                    {
                        hsba.S = 0.05;
                        if ((hsba.B > 0.15) && (hsba.B < 0.35))
                            hsba.B -= 0.1;
                        Color new_color = HsbToRgb(hsba.A, hsba.H / 10, hsba.S, hsba.B); //convert hsb to rgb with new hue
                        string new_hex_color = RgbToHex(new_color);
                        o[hex_color.Key] = new_hex_color;

                    }
                }
            }

            string json = o.ToString();
            json = json.Replace("{", "");
            json = json.Replace("}", "");
            json = json.Replace("\"", "");
            json = json.Replace(",", ";");
            json = json.Replace(" ", "");
            json = json.Replace(":", ": ");
            json = json.Trim();
            json = json + ";";
            File.WriteAllText(new_theme_file_path, json);

            CreateImage(100, 100, output_folder_path, "tiled.jpg");      
            ZipFile.CreateFromDirectory(output_folder_path, new_zip_file_name);
            Directory.Delete(output_folder_path, true);
            Process.Start(Environment.CurrentDirectory);
        }

        public static bool IsColor(string color)
        {
            Regex regex = new Regex(@"\#");
            Match match = regex.Match(color); //check color
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StandartColor(string hexColor)
        {
            if ((hexColor.StartsWith("#ffffff")) || (hexColor.StartsWith("#000000")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckFile(string file)
        {
            if (!(File.Exists(file)))
            {
                MessageBox.Show(file + " is not exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        public static System.Drawing.Bitmap CreateImage(int width, int height, string path, string filename)
        {
            var bitmap = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.Clear(System.Drawing.Color.FromArgb(255, 12, 12, 12));
            bitmap.Save(path + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
            return bitmap;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HexColorBlock.Text);
        }
    }
}
