using Hud1.Helpers;
using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Hud1.Controls;

public class DPIAwareImage : Image
{
    private Window? _parentWindow;
    private Action? _unsubscribeMonitorsChange;
    private readonly Guid _id;

    public DPIAwareImage()
    {
        _id = Guid.NewGuid();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _parentWindow = Window.GetWindow(this);

        _unsubscribeMonitorsChange = Monitors.RegisterMonitorsChange(_parentWindow, OnMonitorsChange);

        _parentWindow.LocationChanged += OnParentWindowLocationChanged;
        _parentWindow.SizeChanged += OnParentWindowSizeChanged;

        UpdateDpiScale();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        Debug.Print($"Unload {_unsubscribeMonitorsChange} {_parentWindow} {_id}");
        _unsubscribeMonitorsChange!();
        _parentWindow!.LocationChanged -= OnParentWindowLocationChanged;
        _parentWindow!.SizeChanged -= OnParentWindowSizeChanged;
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
        PresentationSource source = PresentationSource.FromVisual(this);

        double dpiX = 1;
        if (source != null)
        {
            dpiX = source.CompositionTarget.TransformToDevice.M11;
        }

        double dpiScale = 1.0 / dpiX;

        RenderTransform = new ScaleTransform(dpiScale, dpiScale, Source.Width / 2, Source.Height / 2);

        Debug.Print($"DPI scale updated to: {dpiScale} {_id}");
    }
}
