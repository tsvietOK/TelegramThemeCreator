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
        private const string TempFolderPath = @"Temp\";
        private const string NewThemeFileName = "colors.tdesktop-theme";
        private const string NewZipFileName = "Your_theme.tdesktop-theme";
        private const string DefaultBackgroundColorFileName = "tiled.jpg";
        private const string NewBackgroundImageFileName = "background.jpg";
        private const string NewThemeFilePath = TempFolderPath + NewThemeFileName;
        private const int DefaultBackgroundWidth = 100;
        private const int DefaultBackgroundHeight = 100;

        public static List<ThemeColor> ColorList { get; set; } = new List<ThemeColor>();

        public static string GetOriginalThemeFileName()
        {
            return OriginalThemeFileName;
        }

        public static void Create(double newHue, bool useWindowsWallpaper)
        {
            ColorList.Clear();

            FileUtils.DeleteIfExists(NewZipFileName);
            FileUtils.RecreateDirectory(TempFolderPath);

            ColorList = ThemeColorConverter.GetThemeColorsListFromFile(OriginalThemeFileName);

            foreach (ThemeColor color in ColorList.Where(x => x.IsColor == true && x.IsStandardColor == false))
            {
                color.ApplyColorChange(newHue);
            }

            ThemeColorConverter.SaveThemeToFile(ColorList, NewThemeFilePath);

            if (useWindowsWallpaper)
            {
                File.Copy(SysUtils.GetWinWallpaperFilePath(), TempFolderPath + NewBackgroundImageFileName);
            }
            else
            {
                BitmapUtils.GenerateImage(DefaultBackgroundWidth, DefaultBackgroundHeight, Color.FromArgb(255, 12, 12, 12))
                    .SaveImage(TempFolderPath, DefaultBackgroundColorFileName, ImageFormat.Jpeg);
            }

            ZipFile.CreateFromDirectory(TempFolderPath, NewZipFileName);
            Directory.Delete(TempFolderPath, true);
            Process.Start(Environment.CurrentDirectory);
        }
    }
}
