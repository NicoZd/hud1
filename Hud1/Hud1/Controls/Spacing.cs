using System.Windows;

namespace Hud1.Controls;

internal class Spacing
{
    internal static void SetHorizontal(DependencyObject obj, double space)
    {
        obj.SetValue(HorizontalProperty, space);
    }

    internal static double GetHorizontal(DependencyObject obj)
    {
        return (double)obj.GetValue(HorizontalProperty);
    }

    internal static void SetVertical(DependencyObject obj, double value)
    {
        obj.SetValue(VerticalProperty, value);
    }

    internal static double GetVertical(DependencyObject obj)
    {
        return (double)obj.GetValue(VerticalProperty);
    }

    private static readonly DependencyProperty VerticalProperty =
        DependencyProperty.RegisterAttached("Vertical", typeof(double), typeof(Spacing),
            new UIPropertyMetadata(0d, VerticalChangedCallback));

    private static readonly DependencyProperty HorizontalProperty =
        DependencyProperty.RegisterAttached("Horizontal", typeof(double), typeof(Spacing),
            new UIPropertyMetadata(0d, HorizontalChangedCallback));

    private static void HorizontalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;

        MarginSetter.SetMargin(obj, new Thickness(0, 0, space, 0));
        MarginSetter.SetLastItemMargin(obj, new Thickness(0));
    }

    private static void VerticalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;
        MarginSetter.SetMargin(obj, new Thickness(0, 0, 0, space));
        MarginSetter.SetLastItemMargin(obj, new Thickness(0));
    }
}

