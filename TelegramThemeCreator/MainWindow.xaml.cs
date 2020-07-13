using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TelegramThemeCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer selectorAnimationTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_Initialized(object sender, EventArgs e)
        {
            LinearGradientBrush gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            for (int i = 0; i < 360; i++)
            {
                gradient.GradientStops.Add(new GradientStop(UniColor.FromHSV(i, 1, 1, 255).ToMediaColor(), i / 360f));
            }

            RainbowRectangle.Fill = gradient;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Rainbow_MouseMove(object sender, MouseEventArgs e)
        {
            Point pointToWindow = e.GetPosition(RainbowRectangle);
            HueValue.Text = pointToWindow.X.ToString();
            if (pointToWindow.X < 0)
            {
                HueValue.Text = "0";
            }

            if (pointToWindow.X > 360)
            {
                HueValue.Text = "360";
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ChangeColor(pointToWindow.X);
            }
        }

        private void Rainbow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(e.GetPosition(RainbowRectangle).X);
        }

        private void ChangeColor(double selectedPosition)
        {
            selectedPosition = selectedPosition < 0 ? 0 : (selectedPosition > 360 ? 360 : selectedPosition);
            AnimateSelector(selectedPosition);
        }

        private void AnimateSelector(double selectedPosition)
        {
            selectorAnimationTimer?.Stop();
            selectorAnimationTimer = new System.Windows.Threading.DispatcherTimer()
            {
                Tag = selectedPosition,
                Interval = TimeSpan.FromMilliseconds(10)
            };
            selectorAnimationTimer.Tick += SelectorAnimationTimer_Tick;
            selectorAnimationTimer.Start();
        }

        private void SelectorAnimationTimer_Tick(object sender, EventArgs e)
        {
            var timer = (System.Windows.Threading.DispatcherTimer)sender;
            var selectedPosition = (double)timer.Tag;
            var currentPosition = Canvas.GetLeft(Selector);
            var step = selectedPosition - currentPosition;
            if (Math.Abs(step) > 1)
            {
                step /= 5;
            }

            var nextPosition = currentPosition += step;
            MoveSelector(nextPosition);
            if (nextPosition == selectedPosition)
            {
                timer.Stop();
            }
        }

        private void MoveSelector(double position)
        {
            var color = UniColor.FromHSV((float)position, 1, 1, 255);
            ColorSquare.Fill = new SolidColorBrush(color.ToMediaColor());
            HexColorBlock.Text = color.ToHex();

            Canvas.SetLeft(Selector, position);
        }

        private void GetSystemAccentButton_Click(object sender, RoutedEventArgs e)
        {
            var accentColor = new UniColor(SysUtils.GetSystemAccent(), HexFormat.ABGR);

            ColorSquare.Fill = new SolidColorBrush(accentColor.ToMediaColor());
            HueValue.Text = accentColor.Hue.ToString("000");
            HexColorBlock.Text = new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).ToHex();

            ChangeColor(accentColor.Hue);
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            if (!File.Exists(Theme.GetOriginalThemeFileName()))
            {
                MessageBox.Show("Original theme file does not exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            if (SysUtils.GetSystemAccent() == null)
            {
                GetSystemAccentButton.Visibility = Visibility.Hidden;
            }

            if (!File.Exists(SysUtils.GetWinWallpaperFilePath()))
            {
                UseWindowsWallpaperCheckBox.IsEnabled = false;
            }

            MoveSelector(0);
        }

        private void CreateThemeButton_Click(object sender, RoutedEventArgs e)
        {
            Theme.Create(new UniColor(((SolidColorBrush)ColorSquare.Fill).Color).Hue, UseWindowsWallpaperCheckBox.IsChecked == true);
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HexColorBlock.Text);
        }
    }
}
