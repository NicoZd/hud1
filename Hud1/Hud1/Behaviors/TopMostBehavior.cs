using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Behaviors;

internal class TopMostBehavior : Behavior<Window>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var window = AssociatedObject;
        Debug.Print($"TopMostBehavior Loaded {window}");
        TopMostHelper.Instance.TopWindows.Add(new WindowInteropHelper(window).Handle);
    }
}
