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
        private const string originalThemeFileName = @"Resource\colors.tdesktop-palette";
        private const string outputFolderPath = @"Output\";
        private const string newThemeFileName = @"colors.tdesktop-theme";
        private const string newZipFileName = @"Your_theme.tdesktop-theme";
        private const string newThemeFilePath = outputFolderPath + newThemeFileName;
        public static string GetOriginalThemeFileName()
        {
            return originalThemeFileName;
        }
        public static void Create(double newHue, bool useWindowsWallpaper)
        {
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
                        string newHexColor;
                        if (colorValue.Length == 9)
                            newHexColor = valColor.ToHex(HexFormat.RGBA);
                        else
                            newHexColor = valColor.ToHex(HexFormat.RGB);
                        colorsDic[item.Key] = newHexColor;
                    }
                }
            }

            using (StreamWriter file = new StreamWriter(newThemeFilePath))
                foreach (var entry in colorsDic)
                    file.WriteLine("{0}:{1};", entry.Key, entry.Value);
            if (useWindowsWallpaper)
            {
                File.Copy(SysUtils.GetWinWallpaperFilePath(), outputFolderPath + "background.jpg");
            }
            else
            {
                CreateImage(100, 100, outputFolderPath, "tiled.jpg");
            }

            ZipFile.CreateFromDirectory(outputFolderPath, newZipFileName);
            Directory.Delete(outputFolderPath, true);
            Process.Start(Environment.CurrentDirectory);
        }

        public static System.Drawing.Bitmap CreateImage(int width, int height, string path, string filename)
        {
            var bitmap = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.Clear(System.Drawing.Color.FromArgb(255, 12, 12, 12));
            bitmap.Save(path + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
            return bitmap;
        }
    }
}
