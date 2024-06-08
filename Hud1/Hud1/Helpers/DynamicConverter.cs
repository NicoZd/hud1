using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Hud1.Helpers;

public class DynamicConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values.Length != 2)
            return Binding.DoNothing;

        var value = values[0];
        var converter = values[1] as IValueConverter;

        if (converter == null)
            return Binding.DoNothing;

        return converter.Convert(value, targetType, parameter, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
