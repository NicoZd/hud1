using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using Stateless;

namespace Hud1.Converters
{
    [ValueConversion(typeof(Boolean), typeof(SolidColorBrush))]
    class SelectedToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colHig = (Color)ColorConverter.ConvertFromString("#ff55bb22");
            var colLow = (Color)ColorConverter.ConvertFromString("#5500ff00");
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(1, 0);
            myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 0.0));
            myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 0.03));
            myLinearGradientBrush.GradientStops.Add(new GradientStop(colLow, 0.03));
            myLinearGradientBrush.GradientStops.Add(new GradientStop(colLow, 0.97));
            myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 0.97));
            myLinearGradientBrush.GradientStops.Add(new GradientStop(colHig, 1.0));

            Boolean selected = (Boolean)value;
            return selected ?
                myLinearGradientBrush :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1100ff00"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
