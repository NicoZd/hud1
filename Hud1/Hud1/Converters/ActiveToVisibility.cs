using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hud1.Converters
{
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    class ActiveToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean active = (Boolean)value;
            Debug.WriteLine("ActiveToVisibility {0}", active);
            return active ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
