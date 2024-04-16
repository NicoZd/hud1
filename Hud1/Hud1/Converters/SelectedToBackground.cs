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
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(1, 0);
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString("#ff007700"), 0.0));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString("#ff007700"), 0.03));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString("#ff005500"), 0.03));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString("#ff005500"), 0.97));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop((Color)ColorConverter.ConvertFromString("#ff007700"), 0.97));
            myLinearGradientBrush.GradientStops.Add(
                 new GradientStop((Color)ColorConverter.ConvertFromString("#ff007700"), 1.0));

            Boolean selected = (Boolean)value;
            return selected ?
                myLinearGradientBrush : 
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff004400"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
        
    }
}
