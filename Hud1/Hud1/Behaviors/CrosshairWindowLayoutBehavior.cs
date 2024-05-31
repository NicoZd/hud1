using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Hud1.Behaviors;


public class CrosshairWindowLayoutBehavior : Behavior<CrosshairWindow>
{
    private bool _running = false;
    private readonly Queue<string> _runningUpdates = [];

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
        window.Opacity = 0;

        Monitors.RegisterMonitorsChange(window, () =>
        {
            _ = OnMonitorsChangeAsync(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);
        });
        NavigationStates.CROSSHAIR_DISPLAY.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel))
            {
                _ = OnMonitorsChangeAsync(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);
            }
        };
        _ = OnMonitorsChangeAsync(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);
    }

    private async Task OnMonitorsChangeAsync(string selectionLabel)
    {
        if (_running)
        {
            Debug.Print("CrosshairWindowLayoutBehavior IsRunning...");
            _runningUpdates.Enqueue(selectionLabel);
            return;
        }

        _running = true;

        try
        {
            await UpdateWindowPosition(selectionLabel);
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }

        _running = false;

        if (_runningUpdates.Count > 0)
        {
            Debug.Print("CrosshairWindowLayoutBehavior Run from queue...");
            _ = OnMonitorsChangeAsync(_runningUpdates.Dequeue());
        }
    }

    private async Task UpdateWindowPosition(string selectionLabel)
    {
        var window = AssociatedObject;

        var monitors = Monitors.All;
        var monitorIndex = int.Parse(selectionLabel);

        if (monitors.Count - 1 < monitorIndex)
        {
            await Task.Delay(100);
            CrosshairViewModel.Instance.ChangeDisplay(0)();
            return;
        }

        var monitor = monitors.ElementAt(monitorIndex);
        var hwnd = new WindowInteropHelper(window).Handle;
        var foregroundRestorer = new MainWindowForegroundRestorer();

        Debug.Print($"CrosshairWindowLayoutBehavior OnMonitorsChangeAsync {monitorIndex} {monitor.Bounds}  selectionLabel: {selectionLabel}");

        await ((Storyboard)window.FindResource("FadeOut")).BeginAsync();

        // apply layout
        var width = 50 * monitor.ScaleFactor;
        var height = 50 * monitor.ScaleFactor;
        var x = monitor.Bounds.X + (monitor.Bounds.Width - width) / 2;
        var y = monitor.Bounds.Y + (monitor.Bounds.Height - height) / 2;
        Monitors.MoveWindow(hwnd, x, y, width, height);

        CrosshairViewModel.Instance.dpiScaleCrosshair = 1 / monitor.ScaleFactor;
        CrosshairViewModel.Instance.Redraw();

        foregroundRestorer.Restore();

        await ((Storyboard)window.FindResource("FadeIn")).BeginAsync();
    }

}
