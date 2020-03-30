using Microsoft.Win32;
using System;

namespace TelegramThemeCreator
{
    public static class SysUtils
    {
        private const string RegistryDwmKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM";
        private const string RegistryDwmValue = "AccentColor";
        private const string WindowsWallpaperFolderPath = @"\Microsoft\Windows\Themes\TranscodedWallpaper";

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

        public static string GetWinWallpaperFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + WindowsWallpaperFolderPath;
        }
    }
}
