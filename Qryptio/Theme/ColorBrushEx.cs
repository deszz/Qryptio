using System;
using System.Windows;
using System.Windows.Media;

namespace Qryptio.Theme
{
    public class ColorBrushEx
    {
        public static float GetDimBy(DependencyObject obj)
        {
            return (float)obj.GetValue(DimByProperty);
        }

        public static void SetDimBy(DependencyObject obj, float value)
        {
            obj.SetValue(DimByProperty, value);
        }

        public static float GetLightenBy(DependencyObject obj)
        {
            return (float)obj.GetValue(LightenByProperty);
        }

        public static void SetLightenBy(DependencyObject obj, float value)
        {
            obj.SetValue(LightenByProperty, value);
        }

        public static readonly DependencyProperty DimByProperty =
                DependencyProperty.RegisterAttached("DimBy", typeof(float),
                                                                    typeof(ColorBrushEx),
                                                                    new PropertyMetadata(DimByChangedCallback));

        public static readonly DependencyProperty LightenByProperty =
        DependencyProperty.RegisterAttached("LightenBy", typeof(float),
                                                              typeof(ColorBrushEx),
                                                              new PropertyMetadata(LightenByChangedCallback));

        private static void LightenByChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var value = (float)e.NewValue;
            if (value > 1 || value < 0)
                throw new ArgumentOutOfRangeException(e.Property.Name);

            SolidColorBrush brush = obj as SolidColorBrush;
            brush.Color = Color.FromRgb((byte)(brush.Color.R + (255 - brush.Color.R) * value),
                                        (byte)(brush.Color.G + (255 - brush.Color.G) * value),
                                        (byte)(brush.Color.B + (255 - brush.Color.B) * value));
        }

        private static void DimByChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var value = (float)e.NewValue;
            if (value > 1 || value < 0)
                throw new ArgumentOutOfRangeException(e.Property.Name);

            SolidColorBrush brush = obj as SolidColorBrush;
            brush.Color = Color.FromRgb((byte)(brush.Color.R - brush.Color.R * value),
                                        (byte)(brush.Color.G - brush.Color.G * value),
                                        (byte)(brush.Color.B - brush.Color.B * value));
        }
    }
}
