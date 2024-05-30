using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Controls;


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
        var PrimaryMonitor = Monitors.Primary;
        window.Left = PrimaryMonitor.Bounds.X + (PrimaryMonitor.Bounds.Width / 2) - (window.Width / 2);
        window.Top = PrimaryMonitor.Bounds.Y + (PrimaryMonitor.Bounds.Height / 2) - (window.Height / 2);
    }
}
