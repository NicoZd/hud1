using Hud1.Helpers;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Windows.UI;

namespace Hud1.Windows;


public class MainWindowBehavior : Behavior<Window>
{
#pragma warning disable CS8618
    private MainWindow _mainWindow;
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
        _mainWindow = (MainWindow)sender;

        Monitors.RegisterMonitorsChange(_mainWindow, () =>
        {
            _ = OnDisplayChangeAsync();
        });
        MoreViewModel.Instance.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(MoreViewModel.Instance.HudPosition))
            {
                _ = OnDisplayChangeAsync();
            }
        };

        _mainWindow.Opacity = 0;
        _ = OnDisplayChangeAsync();
    }


    private async Task OnDisplayChangeAsync()
    {
        var AllMonitors = Monitors.All;

        var monitorIndex = int.Parse(MoreViewModel.Instance.HudPosition.Split(":")[0]);
        Debug.Print($"===============AllMonitors {AllMonitors.Count} {monitorIndex} {MoreViewModel.Instance.HudPosition}");
        if (AllMonitors.Count - 1 < monitorIndex)
        {
            MoreViewModel.Instance.ComputeNextHudPosition(0);
            return;
        }

        var monitor = AllMonitors.ElementAt(monitorIndex);

        var hwnd = new WindowInteropHelper(_mainWindow).Handle;

        Storyboard fadeOut = (Storyboard)_mainWindow.FindResource("FadeOut");
        await fadeOut.BeginAsync();

        var hudAlignment = MoreViewModel.Instance.HudPosition.Split(":")[1];

        var foreground = WindowsAPI.GetForegroundWindow();
        var wasForeground = foreground == hwnd;

        Debug.Print($"OnDisplayChangeAsync {monitorIndex} {monitor.Bounds} wasForeground: {wasForeground}");

        if (hudAlignment == "Left")
        {
            WindowsAPI.SetWindowPosition(hwnd, monitor.Bounds.X, (int)monitor.Bounds.Y);
            _mainWindow.GlassContainer.Margin = new Thickness(0, 0, 5, 0);

        }
        else if (hudAlignment == "Right")
        {
            WindowsAPI.SetWindowPosition(hwnd, monitor.Bounds.X + (monitor.Bounds.Width - _mainWindow.Width * monitor.ScaleFactor), monitor.Bounds.Y);
            _mainWindow.GlassContainer.Margin = new Thickness(5, 0, 0, 0);
        }
        _mainWindow.Height = monitor.Bounds.Height / monitor.ScaleFactor;

        if (wasForeground)
        {
            _ = Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MainWindowViewModel.Instance.ActivateWindow();
            });
        }

        Storyboard fadeIn = (Storyboard)_mainWindow.FindResource("FadeIn");
        fadeIn.Begin();
    }

}
