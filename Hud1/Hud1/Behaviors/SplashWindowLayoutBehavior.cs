using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Media3D;

namespace Hud1.Behaviors;


public class SplashWindowLayoutBehavior : Behavior<Window>
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
        var window = (Window)AssociatedObject;

        var hwnd = new WindowInteropHelper(window).Handle;
        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
            extendedStyle
            | WindowsAPI.WS_EX_NOACTIVATE
            | WindowsAPI.WS_EX_TRANSPARENT
            );

        Monitors.RegisterMonitorsChange(window, OnMonitorsChange);
        OnMonitorsChange();
    }

    private void OnMonitorsChange()
    {
        var window = (Window)AssociatedObject;

        var hwnd = new WindowInteropHelper(window).Handle;
        var monitor = Monitors.Primary;

        var width = window.Width * monitor.ScaleFactor;
        var height = window.Height * monitor.ScaleFactor;
        var x = monitor.Bounds.X + (monitor.Bounds.Width - width) / 2;
        var y = monitor.Bounds.Y + (monitor.Bounds.Height - height) / 2;

        Monitors.MoveWindow(hwnd, x, y, width, height);
    }
}
