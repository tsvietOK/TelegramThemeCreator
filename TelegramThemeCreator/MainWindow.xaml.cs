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
using System.Diagnostics;
using System.IO.Compression;

namespace TelegramThemeCreator
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
            byte a = 255;
            float s = 1;
            float b = 1;
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 0);
            int n = 360;
            for (int i = 0; i < n; i++)
            {
                gradient.GradientStops.Add(new GradientStop(UniColor.FromHSV(i, s, b, a).ToMediaColor(), i / 360f));
            }

            RainbowRectangle.Fill = gradient;
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
            float color = float.Parse(HueValue.Text);
            ColorSquare.Fill = new SolidColorBrush(UniColor.FromHSV(color, 1, 1, 255).ToMediaColor());
            DrawSelector(color);
            HexColorBlock.Text = new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).ToHex(HexFormat.RGB);
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
            string regVal = ((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null)).ToString("X8");
            var accentColor = new UniColor(regVal, HexFormat.ABGR);

            ColorSquare.Fill = new SolidColorBrush(accentColor.ToMediaColor());
            HueValue.Text = accentColor.Hue.ToString("000");
            HexColorBlock.Text = new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).ToHex(HexFormat.RGB);

            DrawSelector(accentColor.Hue);
        }


        private void MainWindow1_Initialized(object sender, EventArgs e)
        {
            var regValInt = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null);
            if (regValInt == null) GetSystemAccentButton.Visibility = Visibility.Hidden;
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

            Dictionary<string, string> dic = new Dictionary<string, string>();

            Regex split = new Regex(@"^(.+)\:(.+)");

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"^\/\/\s.+|^\/\/|\/\/.+|\;|\s", string.Empty); //remove comment    
                Match match = split.Match(lines[i]);
                if (match.Success)
                {
                    dic.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }

            double new_hue = new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).Hue; //user hue

            for (int i = 0; i < dic.Count; i++)
            {
                var item = dic.ElementAt(i);
                string value = item.Value;
                if ((IsColor(value) == true) && (StandartColor(value) == false))
                {
                    UniColor valColor;
                    bool changed = false;
                    if (value.Length == 9)
                        valColor = new UniColor(value, HexFormat.RGBA);
                    else
                        valColor = new UniColor(value, HexFormat.RGB);

                    if ((valColor.Hue > 160) && (valColor.Hue < 180))
                    {
                        changed = true;
                        if (valColor.SaturationV >= 0.88)
                            valColor.SaturationV -= 0.2;
                        valColor.Hue = new_hue;
                    }
                    else if (valColor.SaturationV < 0.3)
                    {
                        changed = true;
                        valColor.SaturationV = 0.05;
                        if ((valColor.Value > 0.15) && (valColor.Value < 0.35))
                            valColor.Value -= 0.1;
                        valColor.Hue /= 10;
                        
                    }

                    if(changed)
                    {
                        string new_hex_color = string.Empty;
                        if (value.Length == 9)
                            new_hex_color = valColor.ToHex(HexFormat.RGBA);
                        else
                            new_hex_color = valColor.ToHex(HexFormat.RGB);
                        dic[item.Key] = new_hex_color;
                    }
                }
            }

            using (StreamWriter file = new StreamWriter(new_theme_file_path))
                foreach (var entry in dic)
                    file.WriteLine("{0}:{1};", entry.Key, entry.Value);

            CreateImage(100, 100, output_folder_path, "tiled.jpg");
            ZipFile.CreateFromDirectory(output_folder_path, new_zip_file_name);
            Directory.Delete(output_folder_path, true);
            Process.Start(Environment.CurrentDirectory);
        }

        public static bool IsColor(string color)
        {
            if (color.StartsWith("#"))
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
                MessageBox.Show(file + " does not exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
