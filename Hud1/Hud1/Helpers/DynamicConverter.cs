using System.Windows.Data;

namespace Hud1.Helpers;

public class DynamicConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values.Length != 2)
            return Binding.DoNothing;

        var value = values[0];

        return values[1] is not IValueConverter converter ? Binding.DoNothing : converter.Convert(value, targetType, parameter, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
