using System.Windows;
using System.Windows.Controls;

namespace Hud1.Helpers;

internal class BindingHelper
{
    internal static DependencyProperty CreateProperty<TControl, TValue>(string name, TValue? defaultValue, Action<TControl, TValue>? propertyChangedAction = null)
    where TControl : UserControl
    {
        return DependencyProperty.Register(name, typeof(TValue), typeof(TControl),
            new PropertyMetadata(defaultValue, propertyChangedAction == null ? null : new PropertyChangedCallback((d, e) => propertyChangedAction((TControl)d, (TValue)e.NewValue))));
    }
}
