using System.Windows;

namespace Hud1.Controls;

internal class BringIntoView : UIElement
{
    internal bool Active
    {
        get => (bool)GetValue(ActiveProperty);
        set => SetValue(ActiveProperty, value);
    }

    internal static readonly DependencyProperty ActiveProperty =
        DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(BringIntoView),
            new FrameworkPropertyMetadata(defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.AffectsRender,
                OnChange
            )
    );

    private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue == true)
            ((FrameworkElement)d).BringIntoView(new Rect(0, -100, 0, 200));
    }
}
