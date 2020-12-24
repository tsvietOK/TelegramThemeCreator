using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using TelegramThemeCreator.Controllers;
using TelegramThemeCreator.Extensions;
using TelegramThemeCreator.Utils;

namespace TelegramThemeCreator
{
    /// <summary>
    /// Provides static methods for theme creation.
    /// </summary>
    public static class ThemeCreator
    {
        private const string ResourceFolderPath = @"Resource\";
        private const string SourceThemeFileName = "colors.tdesktop-palette";
        private const string SourceThemeFilePath = ResourceFolderPath + SourceThemeFileName;

        private const string ChatBackgroundColorFileName = "tiled.jpg";
        private const string NewChatBackgroundImageFileName = "background.jpg";

        private const string TempFolderPath = @"Temp\";
        private const string NewThemeFileName = "colors.tdesktop-theme";
        private const string NewThemeFilePath = TempFolderPath + NewThemeFileName;

        private const int ChatBackgroundWidth = 100;
        private const int ChatBackgroundHeight = 100;

        private const string NewZipFileName = "Your_theme.tdesktop-theme";

        /// <summary>
        /// Creates and saves a new theme with required parameters.
        /// </summary>
        /// <param name="newHue">A double new hue value.</param>
        /// <param name="useWindowsWallpaper">A <see cref="bool" /> value indicating whether the windows wallpaper should be used.</param>
        public static void Create(double newHue, bool useWindowsWallpaper)
        {
            // Remove previous created theme
            FileUtils.DeleteIfExists(NewZipFileName);

            // Recreate temporary directory
            FileUtils.RecreateDirectory(TempFolderPath);

            var theme = new ThemeController(SourceThemeFilePath);

            theme.ApplyHue(newHue);

            theme.SaveColors(NewThemeFilePath);

            if (useWindowsWallpaper)
            {
                File.Copy(SysUtils.GetWinWallpaperFilePath(), TempFolderPath + NewChatBackgroundImageFileName);
            }
            else
            {
                BitmapUtils.GenerateImage(ChatBackgroundWidth, ChatBackgroundHeight, Color.FromArgb(255, 12, 12, 12))
                    .SaveImage(TempFolderPath, ChatBackgroundColorFileName, ImageFormat.Jpeg);
            }

            ZipFile.CreateFromDirectory(TempFolderPath, NewZipFileName);

            Directory.Delete(TempFolderPath, true);

            Process.Start(Environment.CurrentDirectory);
        }

        /// <summary>
        /// This method determines is source theme file exists.
        /// </summary>
        /// <returns>True if source theme file is exists; otherwise, false.</returns>
        public static bool IsSourceThemeFileExists()
        {
            return File.Exists(SourceThemeFilePath);
        }
    }
}
