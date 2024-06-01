using System.Windows;
using System.Windows.Controls;

namespace Hud1.Controls;

internal class MarginSetter
{
    internal static readonly DependencyProperty MarginProperty =
    DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(MarginSetter),
        new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

    internal static readonly DependencyProperty LastItemMarginProperty =
        DependencyProperty.RegisterAttached("LastItemMargin", typeof(Thickness), typeof(MarginSetter),
            new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

    internal static void SetLastItemMargin(DependencyObject obj, Thickness value)
    {
        obj.SetValue(LastItemMarginProperty, value);
    }

    internal static Thickness GetLastItemMargin(Panel obj)
    {
        return (Thickness)obj.GetValue(LastItemMarginProperty);
    }

    internal static void SetMargin(DependencyObject obj, Thickness value)
    {
        obj.SetValue(MarginProperty, value);
    }

    internal static Thickness GetMargin(DependencyObject obj)
    {
        return (Thickness)obj.GetValue(MarginProperty);
    }

    private static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        // Make sure this is put on a panel
        if (sender is not Panel panel) return;

        // Avoid duplicate registrations
        panel.Loaded -= OnPanelLoaded;
        panel.Loaded += OnPanelLoaded;

        if (panel.IsLoaded)
            OnPanelLoaded(panel, null);
    }

    private static void OnPanelLoaded(object sender, RoutedEventArgs? e)
    {
        var panel = (Panel)sender;

        // Go over the children and set margin for them:
        for (var i = 0; i < panel.Children.Count; i++)
        {
            var child = panel.Children[i];
            if (child is not FrameworkElement fe) continue;

            var isLastItem = i == panel.Children.Count - 1;
            fe.Margin = isLastItem ? GetLastItemMargin(panel) : GetMargin(panel);
        }
    }
}

