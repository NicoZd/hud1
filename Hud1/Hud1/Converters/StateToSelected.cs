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
    [ValueConversion(typeof(String), typeof(bool))]
    class StateToSelected : IValueConverter
    {
        private String SelectedState;

        public StateToSelected(String selectedState) {
            SelectedState = selectedState;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String state = (String)value;
            return state.Equals(SelectedState);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
        
    }
}
