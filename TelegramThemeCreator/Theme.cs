using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace TelegramThemeCreator
{
    public static class Theme
    {
        private const string OriginalThemeFileName = @"Resource\colors.tdesktop-palette";
        private const string OutputFolderPath = @"Output\";
        private const string NewThemeFileName = "colors.tdesktop-theme";
        private const string NewZipFileName = "Your_theme.tdesktop-theme";
        private const string NewBackgroundColorFileName = "tiled.jpg";
        private const string NewBackgroundImageFileName = "background.jpg";
        private const string NewThemeFilePath = OutputFolderPath + NewThemeFileName;

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
                File.Delete(NewThemeFilePath);
            if (File.Exists(NewZipFileName))
                File.Delete(NewZipFileName);
            if (File.Exists(OutputFolderPath + NewBackgroundColorFileName))
                File.Delete(OutputFolderPath + NewBackgroundColorFileName);
            if (File.Exists(NewThemeFileName))
                File.Delete(NewThemeFileName);

            string[] lines = File.ReadAllLines(OriginalThemeFileName);

            Dictionary<string, string> colorsDic = new Dictionary<string, string>();

            Regex split = new Regex(@"^(.+)\:(.+)");

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"^\/\/\s.+|^\/\/|\/\/.+|\;|\s", string.Empty); //remove comment    
                Match match = split.Match(lines[i]);
                if (match.Success)
                {
                    colorsDic.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }
            for (int i = 0; i < colorsDic.Count; i++)
            {
                var item = colorsDic.ElementAt(i);
                string colorValue = item.Value;
                if ((ColorUtils.IsColor(colorValue) == true) && (ColorUtils.StandartColor(colorValue) == false))
                {
                    UniColor valColor;
                    bool changed = false;
                    valColor = colorValue.Length == 9 ? new UniColor(colorValue, HexFormat.RGBA) : new UniColor(colorValue, HexFormat.RGB);

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
                        string newHexColor = colorValue.Length == 9 ? valColor.ToHex(HexFormat.RGBA) : valColor.ToHex(HexFormat.RGB);
                        colorsDic[item.Key] = newHexColor;
                    }
                }
            }

            using (StreamWriter file = new StreamWriter(NewThemeFilePath))
                foreach (var entry in colorsDic)
                    file.WriteLine("{0}:{1};", entry.Key, entry.Value);
            if (useWindowsWallpaper)
            {
                File.Copy(SysUtils.GetWinWallpaperFilePath(), OutputFolderPath + NewBackgroundImageFileName);
            }
            else
            {
                CreateImage(100, 100, OutputFolderPath, NewBackgroundColorFileName);
            }

            ZipFile.CreateFromDirectory(OutputFolderPath, NewZipFileName);
            Directory.Delete(OutputFolderPath, true);
            Process.Start(Environment.CurrentDirectory);
        }

        public static void CreateImage(int width, int height, string path, string filename)
        {
            var bitmap = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.Clear(System.Drawing.Color.FromArgb(255, 12, 12, 12));
            bitmap.Save(path + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
