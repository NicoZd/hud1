﻿using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Behaviors;


internal class SplashWindowLayoutBehavior : Behavior<Window>
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

        var hwnd = new WindowInteropHelper(window).Handle;
        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowConstants.GWL_EXSTYLE);
        WindowsAPI.SetWindowLong(hwnd, WindowConstants.GWL_EXSTYLE,
            extendedStyle
            | WindowConstants.WS_EX_NOACTIVATE
            | WindowConstants.WS_EX_TRANSPARENT
            );

        Monitors.RegisterMonitorsChange(window, OnMonitorsChange);
        OnMonitorsChange();
    }

    private void OnMonitorsChange()
    {
        var window = AssociatedObject;
        var monitor = Monitors.Primary;

        var width = window.Width * monitor.ScaleFactor;
        var height = window.Height * monitor.ScaleFactor;
        var x = monitor.Bounds.X + ((monitor.Bounds.Width - width) / 2);
        var y = monitor.Bounds.Y + ((monitor.Bounds.Height - height) / 2);

        Monitors.MoveWindow(window, x, y, width, height);
    }
}
