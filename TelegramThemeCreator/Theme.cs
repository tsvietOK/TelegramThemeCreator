using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using TelegramThemeCreator.Utils;

namespace TelegramThemeCreator
{
    public static class Theme
    {
        private const string OriginalThemeFileName = @"Resource\colors.tdesktop-palette";
        private const string OutputFolderPath = @"Output\";
        private const string NewThemeFileName = "colors.tdesktop-theme";
        private const string NewZipFileName = "Your_theme.tdesktop-theme";
        private const string DefaultBackgroundColorFileName = "tiled.jpg";
        private const string NewBackgroundImageFileName = "background.jpg";
        private const string NewThemeFilePath = OutputFolderPath + NewThemeFileName;
        private const int DefaultBackgroundWidth = 100;
        private const int DefaultBackgroundHeight = 100;

        public static List<ThemeColor> ColorList { get; set; } = new List<ThemeColor>();

        public static string GetOriginalThemeFileName()
        {
            return OriginalThemeFileName;
        }

        public static void Create(double newHue, bool useWindowsWallpaper)
        {
            if (Directory.Exists(OutputFolderPath) == false)
            {
                Directory.CreateDirectory(OutputFolderPath);
            }

            if (File.Exists(NewThemeFilePath))
            {
                File.Delete(NewThemeFilePath);
            }

            if (File.Exists(NewZipFileName))
            {
                File.Delete(NewZipFileName);
            }

            if (File.Exists(OutputFolderPath + DefaultBackgroundColorFileName))
            {
                File.Delete(OutputFolderPath + DefaultBackgroundColorFileName);
            }

            if (File.Exists(NewThemeFileName))
            {
                File.Delete(NewThemeFileName);
            }

            string[] lines = File.ReadAllLines(OriginalThemeFileName);

            Regex split = new Regex(@"^(.+)\:(.+)");

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"^\/\/\s.+|^\/\/|\/\/.+|\;|\s", string.Empty); // remove comments
                Match match = split.Match(lines[i]);
                if (match.Success)
                {
                    string name = match.Groups[1].Value;
                    string value = match.Groups[2].Value;
                    ColorList.Add(new ThemeColor(name, value));
                }
            }

            foreach (ThemeColor color in ColorList.Where(x => x.IsColor == true && x.IsStandardColor == false))
            {
                UniColor colorValue = color.Value;
                bool changed = false;

                if ((colorValue.Hue > 160) && (colorValue.Hue < 180))
                {
                    changed = true;
                    if (colorValue.SaturationV >= 0.88)
                    {
                        colorValue.SaturationV -= 0.2;
                    }

                    colorValue.Hue = newHue;
                }
                else if (colorValue.SaturationV < 0.3)
                {
                    changed = true;
                    colorValue.SaturationV = 0.05;
                    if ((colorValue.Value > 0.15) && (colorValue.Value < 0.35))
                    {
                        colorValue.Value -= 0.1;
                    }

                    colorValue.Hue /= 10;
                }

                if (changed)
                {
                    color.Value = colorValue;
                }
            }

            using (StreamWriter file = new StreamWriter(NewThemeFilePath))
            {
                foreach (var color in ColorList)
                {
                    file.WriteLine($"{color.Name}:{color.GetHexColor()};");
                }
            }

            if (useWindowsWallpaper)
            {
                File.Copy(SysUtils.GetWinWallpaperFilePath(), OutputFolderPath + NewBackgroundImageFileName);
            }
            else
            {
                BitmapUtils.GenerateImage(DefaultBackgroundWidth, DefaultBackgroundHeight, Color.FromArgb(255, 12, 12, 12))
                    .SaveImage(OutputFolderPath, DefaultBackgroundColorFileName, ImageFormat.Jpeg);
            }

            ZipFile.CreateFromDirectory(OutputFolderPath, NewZipFileName);
            Directory.Delete(OutputFolderPath, true);
            Process.Start(Environment.CurrentDirectory);
            ColorList.Clear();
        }
    }
}
