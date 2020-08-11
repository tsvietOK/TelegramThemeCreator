using Microsoft.Win32;
using System;

namespace TelegramThemeCreator.Utils
{
    /// <summary>
    /// Provides static methods for access to OS resources.
    /// </summary>
    public static class SysUtils
    {
        private const string RegistryDwmKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM";
        private const string RegistryDwmValue = "AccentColor";
        private const string WindowsWallpaperFolderPath = @"\Microsoft\Windows\Themes\TranscodedWallpaper";

        /// <summary>
        /// Gets the system accent color
        /// </summary>
        /// <returns>If registry key exists it returns a string containing system accent color,
        /// if not, null</returns>
        public static string GetSystemAccent()
        {
            string regColor;
            try
            {
                regColor = ((int)Registry.GetValue(RegistryDwmKey, RegistryDwmValue, null)).ToString("X8");
            }
            catch
            {
                regColor = null;
            }

            return regColor;
        }

        /// <summary>
        /// Gets a path to folder with current Windows wallpaper
        /// </summary>
        /// <returns>A string containing path to folder with current Windows wallpaper</returns>
        public static string GetWinWallpaperFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + WindowsWallpaperFolderPath;
        }
    }
}