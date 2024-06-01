using Hud1.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hud1.Controls;

internal class DPIAwareImage : Image
{
    private Window? parentWindow;
    private Action? unsubscribeMonitorsChange;
    private readonly Guid debugGuid;

    internal DPIAwareImage()
    {
        debugGuid = Guid.NewGuid();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        parentWindow = Window.GetWindow(this);

        unsubscribeMonitorsChange = Monitors.RegisterMonitorsChange(parentWindow, OnMonitorsChange);

        parentWindow.LocationChanged += OnParentWindowLocationChanged;
        parentWindow.SizeChanged += OnParentWindowSizeChanged;

        UpdateDpiScale();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        Debug.Print($"Unload {unsubscribeMonitorsChange} {parentWindow} {debugGuid}");
        unsubscribeMonitorsChange!();
        parentWindow!.LocationChanged -= OnParentWindowLocationChanged;
        parentWindow!.SizeChanged -= OnParentWindowSizeChanged;
    }

    private void OnParentWindowLocationChanged(object? sender, EventArgs e)
    {
        UpdateDpiScale();
    }

    private void OnParentWindowSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateDpiScale();
    }

    private void OnMonitorsChange()
    {
        UpdateDpiScale();
    }

    private void UpdateDpiScale()
    {
        var source = PresentationSource.FromVisual(this);

        double dpiX = 1;
        if (source != null)
        {
            dpiX = source.CompositionTarget.TransformToDevice.M11;
        }

        var dpiScale = 1.0 / dpiX;

        RenderTransform = new ScaleTransform(dpiScale, dpiScale, Source.Width / 2, Source.Height / 2);

        Debug.Print($"DPI scale updated to: {dpiScale} {debugGuid}");
    }
}
