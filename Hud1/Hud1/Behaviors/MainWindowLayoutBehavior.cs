using Hud1.Helpers;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Windows.UI;

namespace Hud1.Behaviors;


public class MainWindowLayoutBehavior : Behavior<Window>
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
        var mainWindow = (MainWindow)AssociatedObject;

        Monitors.RegisterMonitorsChange(mainWindow, () =>
        {
            _ = OnMonitorsChangeAsync();
        });
        MoreViewModel.Instance.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(MoreViewModel.Instance.HudPosition))
            {
                _ = OnMonitorsChangeAsync();
            }
        };
        _ = OnMonitorsChangeAsync();
    }


    private async Task OnMonitorsChangeAsync()
    {
        var mainWindow = (MainWindow)AssociatedObject;
        var hudPosition = MoreViewModel.Instance.HudPosition;

        var monitors = Monitors.All;
        var monitorIndex = int.Parse(hudPosition.Split(":")[0]);
        var hudAlignment = hudPosition.Split(":")[1];

        if (monitors.Count - 1 < monitorIndex)
        {
            MoreViewModel.Instance.ComputeNextHudPosition(0);
            return;
        }

        var monitor = monitors.ElementAt(monitorIndex);
        var foreground = WindowsAPI.GetForegroundWindow();
        var hwnd = new WindowInteropHelper(mainWindow).Handle;
        var wasForeground = foreground == hwnd;

        Debug.Print($"OnDisplayChangeAsync {monitorIndex} {monitor.Bounds} wasForeground: {wasForeground}");

        await ((Storyboard)mainWindow.FindResource("FadeOut")).BeginAsync();

        // apply layout
        if (hudAlignment == "Left")
        {
            WindowsAPI.SetWindowPosition(hwnd, monitor.Bounds.X, (int)monitor.Bounds.Y);
            mainWindow.GlassContainer.Margin = new Thickness(0, 0, 5, 0);

        }
        else if (hudAlignment == "Right")
        {
            WindowsAPI.SetWindowPosition(hwnd, monitor.Bounds.X + (monitor.Bounds.Width - mainWindow.Width * monitor.ScaleFactor), monitor.Bounds.Y);
            mainWindow.GlassContainer.Margin = new Thickness(5, 0, 0, 0);
        }
        mainWindow.Height = monitor.Bounds.Height / monitor.ScaleFactor;

        // post operations
        if (wasForeground)
        {
            // For some unknown reason ActivateWindow must be called later when SetWindowPosition was called before
            _ = Application.Current.Dispatcher.InvokeAsync(MainWindowViewModel.Instance.ActivateWindow);
        }

        ((Storyboard)mainWindow.FindResource("FadeIn")).Begin();
    }

}
