using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TelegramThemeCreator.Enums;
using TelegramThemeCreator.Models;
using TelegramThemeCreator.Utils;

namespace TelegramThemeCreator.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private bool useWindowsWallpaper;
        private double sliderValue;
        private UniColor color = new UniColor(255, 0, 0, 255);

        public bool UseWindowsWallpaper
        {
            get
            {
                return useWindowsWallpaper;
            }
            set
            {
                useWindowsWallpaper = value;
                OnPropertyChanged(nameof(UseWindowsWallpaper));
            }
        }

        public bool IsUseWindowsWallpaperCheckBoxEnabled
        {
            get
            {
                return File.Exists(SysUtils.GetWinWallpaperFilePath());
            }
        }

        public double SliderValue
        {
            get
            {
                return sliderValue;
            }
            set
            {
                if (sliderValue != value)
                {
                    sliderValue = value;
                    OnPropertyChanged(nameof(SliderValue));

                    Color = UniColor.FromHSV((float)sliderValue, 1, 1);
                }
            }
        }

        public UniColor Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;

                OnPropertyChanged(nameof(ColorHex));
                OnPropertyChanged(nameof(ColorFill));
            }
        }

        public string ColorHex
        {
            get
            {
                return Color.ToHex();
            }
        }

        public Brush ColorFill
        {
            get
            {
                return new SolidColorBrush(Color.ToMediaColor());
            }
        }

        public bool IsGetSystemAccentButtonEnabled
        {
            get
            {
                return SysUtils.GetSystemAccent() != null;
            }
        }

        public RelayCommand CreateThemeCommand => new RelayCommand(() => CreateTheme());

        public RelayCommand GetSystemAccentColorCommand => new RelayCommand(() => GetSystemAccentColor());

        public RelayCommand CopyHexToClipboardCommand => new RelayCommand(() => CopyHexToClipboard());

        private void CreateTheme()
        {
            ThemeCreator.Create(Color.Hue, UseWindowsWallpaper);
        }

        private void GetSystemAccentColor()
        {
            SliderValue = new UniColor(SysUtils.GetSystemAccent(), HexFormat.ABGR).Hue;
        }

        private void CopyHexToClipboard()
        {
            Clipboard.SetText(ColorHex);
        }
    }
}