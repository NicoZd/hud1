using Hud1.Helpers;
using Hud1.ViewModels;
using Hud1.Windows;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace Hud1.Behaviors;


internal class MainWindowLayoutBehavior : Behavior<MainWindow>
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
        window.Opacity = 0;

        var updates = new FunctionDebounce(UpdateWindowPosition);
        Monitors.RegisterMonitorsChange(window, () =>
        {
            _ = updates.Run(MoreViewModel.Instance.HudPosition);
        });
        MoreViewModel.Instance.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(MoreViewModel.Instance.HudPosition))
            {
                _ = updates.Run(MoreViewModel.Instance.HudPosition);
            }
        };
        _ = updates.Run(MoreViewModel.Instance.HudPosition);
    }

    private async Task UpdateWindowPosition(string hudPosition)
    {
        var window = AssociatedObject;

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
        var foregroundRestorer = new MainWindowForegroundRestorer();

        Debug.Print($"OnMonitorsChangeAsync {monitorIndex} {monitor.Bounds} {hudPosition}");

        await ((Storyboard)window.FindResource("FadeOut")).BeginAsync();

        var width = 450 * monitor.ScaleFactor;
        var height = monitor.Bounds.Height;

        // apply layout
        if (hudAlignment == "Left")
        {
            var x = monitor.Bounds.X;
            var y = monitor.Bounds.Y;
            Monitors.MoveWindow(window, x, y, width, height);
            window.GlassContainer.Margin = new Thickness(0, 0, 5, 0);
        }
        else if (hudAlignment == "Right")
        {
            var x = monitor.Bounds.X + (monitor.Bounds.Width - width);
            var y = monitor.Bounds.Y;
            Monitors.MoveWindow(window, x, y, width, height);
            window.GlassContainer.Margin = new Thickness(5, 0, 0, 0);
        }

        foregroundRestorer.Restore();

        await ((Storyboard)window.FindResource("FadeIn")).BeginAsync();
    }
}
