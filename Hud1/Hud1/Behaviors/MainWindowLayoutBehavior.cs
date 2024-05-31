using Hud1.Helpers;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Hud1.Behaviors;


public class MainWindowLayoutBehavior : Behavior<MainWindow>
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
            _ = OnMonitorsChangeAsync(MoreViewModel.Instance.HudPosition);
        });
        MoreViewModel.Instance.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(MoreViewModel.Instance.HudPosition))
            {
                _ = OnMonitorsChangeAsync(MoreViewModel.Instance.HudPosition);
            }
        };
        _ = OnMonitorsChangeAsync(MoreViewModel.Instance.HudPosition);
    }

    private async Task OnMonitorsChangeAsync(string hudPosition)
    {
        if (_running)
        {
            Debug.Print("IsRunning...");
            _runningUpdates.Enqueue(hudPosition);
            return;
        }

        _running = true;

        try
        {
            await UpdateWindowPosition(hudPosition);
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }

        _running = false;

        if (_runningUpdates.Count > 0)
        {
            Debug.Print("Run from queue...");
            _ = OnMonitorsChangeAsync(_runningUpdates.Dequeue());
        }
    }

    private async Task UpdateWindowPosition(string hudPosition)
    {
        var mainWindow = AssociatedObject;

        var monitors = Monitors.All;
        var monitorIndex = int.Parse(hudPosition.Split(":")[0]);
        var hudAlignment = hudPosition.Split(":")[1];

        if (monitors.Count - 1 < monitorIndex)
        {
            await Task.Delay(100);
            MoreViewModel.Instance.ComputeNextHudPosition(0);
            return;
        }

        var monitor = monitors.ElementAt(monitorIndex);
        var hwnd = new WindowInteropHelper(mainWindow).Handle;
        var foregroundRestorer = new MainWindowForegroundRestorer();

        Debug.Print($"OnMonitorsChangeAsync {monitorIndex} {monitor.Bounds} {hudPosition}");

        await ((Storyboard)mainWindow.FindResource("FadeOut")).BeginAsync();

        var width = 450 * monitor.ScaleFactor;
        var height = monitor.Bounds.Height;

        // apply layout
        if (hudAlignment == "Left")
        {
            var x = monitor.Bounds.X;
            var y = monitor.Bounds.Y;
            Monitors.MoveWindow(hwnd, x, y, width, height);
            mainWindow.GlassContainer.Margin = new Thickness(0, 0, 5, 0);
        }
        else if (hudAlignment == "Right")
        {
            var x = monitor.Bounds.X + (monitor.Bounds.Width - width);
            var y = monitor.Bounds.Y;
            Monitors.MoveWindow(hwnd, x, y, width, height);
            mainWindow.GlassContainer.Margin = new Thickness(5, 0, 0, 0);
        }

        CrosshairViewModel.Instance.dpiScaleMain = 1 / monitor.ScaleFactor;
        CrosshairViewModel.Instance.Redraw();

        foregroundRestorer.Restore();

        await ((Storyboard)mainWindow.FindResource("FadeIn")).BeginAsync();
    }

}
