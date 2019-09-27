using Microsoft.Win32;
using System;

namespace TelegramThemeCreator
{
    public static class SysUtils
    {
        public static string GetSystemAccent()
        {
            string regColor;
            try
            {
                regColor = ((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "AccentColor", null)).ToString("X8");
            }
            catch
            {
                regColor = null;
            }

            return regColor;
        }

        public static string GetWinWallpaperFilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\TranscodedWallpaper";
        }
    }
}
