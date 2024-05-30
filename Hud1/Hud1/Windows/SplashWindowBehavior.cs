using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Windows;


public class SplashWindowBehavior : Behavior<Window>
{
#pragma warning disable CS8618
    private Window window;
#pragma warning restore CS8618

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
        window = (Window)sender;

        var hwnd = new WindowInteropHelper(window).Handle;
        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
            extendedStyle
            | WindowsAPI.WS_EX_NOACTIVATE
            | WindowsAPI.WS_EX_TRANSPARENT
            );

        Monitors.RegisterMonitorsChange(window, OnDisplayChange);
        OnDisplayChange();
    }

    private void OnDisplayChange()
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var monitor = Monitors.Primary;
        var x = monitor.Bounds.X + (monitor.Bounds.Width - window.Width * monitor.ScaleFactor) / 2;
        var y = monitor.Bounds.Y + (monitor.Bounds.Height - window.Height * monitor.ScaleFactor) / 2;
        WindowsAPI.SetWindowPosition(hwnd, x, y);


    }
}
