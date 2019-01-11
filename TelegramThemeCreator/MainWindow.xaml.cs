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

        private void Rainbow_MouseMove(object sender, MouseEventArgs e)
        {
            Point pointToWindow = e.GetPosition(RainbowRectangle);
            HueValue.Text = pointToWindow.X.ToString();
            if (pointToWindow.X < 0)
                HueValue.Text = "0";
            if (pointToWindow.X > 360)
                HueValue.Text = "360";
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ChangeColor(pointToWindow.X);
            }
        }

        private void Rainbow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(e.GetPosition(RainbowRectangle).X);
        }

        public void ChangeColor(double selectedPosition)
        {
            selectedPosition = (selectedPosition < 0 ? 0 : (selectedPosition > 360 ? 360 : selectedPosition));
            AnimateSelector(selectedPosition);
        }

        private System.Windows.Threading.DispatcherTimer SelectorAnimationTimer;

        private void AnimateSelector(double selectedPosition)
        {
            SelectorAnimationTimer?.Stop();
            SelectorAnimationTimer = new System.Windows.Threading.DispatcherTimer()
            {
                Tag = selectedPosition,
                Interval = TimeSpan.FromMilliseconds(10)
            };
            SelectorAnimationTimer.Tick += SelectorAnimationTimer_Tick;
            SelectorAnimationTimer.Start();
        }

        private void SelectorAnimationTimer_Tick(object sender, EventArgs e)
        {
            var timer = (System.Windows.Threading.DispatcherTimer)sender;
            var selectedPosition = (double)timer.Tag;
            var currentPosition = Canvas.GetLeft(Selector);
            var step = selectedPosition - currentPosition;
            if (Math.Abs(step) > 1)
                step /= 5;
            var nextPosition = currentPosition += step;
            MoveSelector(nextPosition);
            if (nextPosition == selectedPosition)
                timer.Stop();

        }

        private void MoveSelector(double position)
        {
            var color = UniColor.FromHSV((float)position, 1, 1, 255);
            ColorSquare.Fill = new SolidColorBrush(color.ToMediaColor());
            HexColorBlock.Text = color.ToHex(HexFormat.RGB);

            Canvas.SetLeft(Selector, position);
        }

        private void GetSystemAccentButton_Click(object sender, RoutedEventArgs e)
        {
            var accentColor = new UniColor(GetSystemAccent(), HexFormat.ABGR);

            ColorSquare.Fill = new SolidColorBrush(accentColor.ToMediaColor());
            HueValue.Text = accentColor.Hue.ToString("000");
            HexColorBlock.Text = new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).ToHex(HexFormat.RGB);

            ChangeColor(accentColor.Hue);
        }

        public static string GetSystemAccent()
        {
            string regColor = ((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null)).ToString("X8");
            return regColor;
        }

        string winWallpaperFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Themes\\TranscodedWallpaper";

        private void MainWindow1_Initialized(object sender, EventArgs e)
        {
            if (GetSystemAccent() == null) GetSystemAccentButton.Visibility = Visibility.Hidden;
            if (!File.Exists(winWallpaperFile)) UseWindowsWallpaperCheckBox.IsEnabled = false;
            CheckFile(@"colors.tdesktop-palette");

            MoveSelector(0);
        }

        private void CreateThemeButton_Click(object sender, RoutedEventArgs e)
        {
            string originalThemeFileName = @"colors.tdesktop-palette";
            string outputFolderPath = @"Output\";
            string newThemeFileName = @"colors.tdesktop-theme";
            string newZipFileName = @"Your_theme.tdesktop-theme";
            string newThemeFilePath = outputFolderPath + newThemeFileName;
            if (Directory.Exists(outputFolderPath) == false)
            {
                Directory.CreateDirectory(outputFolderPath);
            }
            if (File.Exists(newThemeFilePath))
                File.Delete(newThemeFilePath);
            if (File.Exists(newZipFileName))
                File.Delete(newZipFileName);
            if (File.Exists(outputFolderPath + "tiled.jpg"))
                File.Delete(outputFolderPath + "tiled.jpg");
            if (File.Exists(newThemeFileName))
                File.Delete(newThemeFileName);

            string[] lines = File.ReadAllLines(originalThemeFileName);

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

            double newHue = new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).Hue; //user hue

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
                        valColor.Hue = newHue;
                    }
                    else if (valColor.SaturationV < 0.3)
                    {
                        changed = true;
                        valColor.SaturationV = 0.05;
                        if ((valColor.Value > 0.15) && (valColor.Value < 0.35))
                            valColor.Value -= 0.1;
                        valColor.Hue /= 10;

                    }

                    if (changed)
                    {
                        string newHexColor = string.Empty;
                        if (value.Length == 9)
                            newHexColor = valColor.ToHex(HexFormat.RGBA);
                        else
                            newHexColor = valColor.ToHex(HexFormat.RGB);
                        dic[item.Key] = newHexColor;
                    }
                }
            }

            using (StreamWriter file = new StreamWriter(newThemeFilePath))
                foreach (var entry in dic)
                    file.WriteLine("{0}:{1};", entry.Key, entry.Value);
            if (UseWindowsWallpaperCheckBox.IsChecked == true)
            {
                File.Copy(winWallpaperFile, outputFolderPath + "background.jpg");
            }
            else
            {
                CreateImage(100, 100, outputFolderPath, "tiled.jpg");
            }

            ZipFile.CreateFromDirectory(outputFolderPath, newZipFileName);
            Directory.Delete(outputFolderPath, true);
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
