using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qryptio.Wpf
{
    [ValueConversion(typeof(Double), typeof(GridLength))]
    public class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((Double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((GridLength)value).Value;
        }
    }
}
