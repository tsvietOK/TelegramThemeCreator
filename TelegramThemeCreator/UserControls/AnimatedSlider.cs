using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TelegramThemeCreator.Models;

namespace TelegramThemeCreator.UserControls
{
    public class AnimatedSlider : Slider
    {
        private Track track;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            track = GetTemplateChild("PART_Track") as Track;
            var border = GetTemplateChild("TrackBackground") as Border;

            LinearGradientBrush gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            for (int i = 0; i < 360; i++)
            {
                gradient.GradientStops.Add(new GradientStop(UniColor.FromHSV(i, 1, 1, 255).ToMediaColor(), i / 360f));
            }

            border.Background = gradient;
        }

        /*protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsMoveToPointEnabled 
                && track != null 
                && track.Thumb != null 
                && !track.Thumb.IsMouseOver)
            {
                // Move Thumb to the Mouse location
                Point pt = e.MouseDevice.GetPosition(track);

                double newValue = track.ValueFromPoint(pt);
                if (!double.IsInfinity(newValue))
                {
                    UpdateValue(newValue);
                }

                e.Handled = true;
            }
        }*/

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            UpdateValue(newValue);

            var ellipseThumb = track.Thumb.Template.FindName("grip", track.Thumb) as Ellipse;

            ellipseThumb.Fill = new SolidColorBrush(UniColor.FromHSV((float)newValue, 1, 1).ToMediaColor());
        }

        private void UpdateValue(double value)
        {
            double snappedValue = SnapToTick(value);

            if (snappedValue != Value)
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    To = Math.Max(Minimum, Math.Min(Maximum, snappedValue)),
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                var easing = new CubicEase();
                easing.EasingMode = EasingMode.EaseOut;

                animation.EasingFunction = easing;
                Storyboard.SetTargetProperty(animation, new PropertyPath("Value"));
                Storyboard.SetTarget(animation, this);

                Storyboard sb = new Storyboard();
                sb.Children.Add(animation);
                sb.Begin();

                Value = snappedValue;
            }
        }

        private double SnapToTick(double value)
        {
            if (IsSnapToTickEnabled)
            {
                double previous = Minimum;
                double next = Maximum;

                if (TickFrequency > 0.0)
                {
                    previous = Minimum + (Math.Round((value - Minimum) / TickFrequency) * TickFrequency);
                    next = Math.Min(Maximum, previous + TickFrequency);
                }

                value = (value > ((previous + next) * 0.5)) ? next : previous;
            }

            return value;
        }
    }
}
