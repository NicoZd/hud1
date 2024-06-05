using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using Hud1.Windows;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace Hud1.Behaviors;


internal class CrosshairWindowLayoutBehavior : Behavior<CrosshairWindow>
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

        var updates = new FunctionDebounce(UpdateWindowPosition);
        Monitors.RegisterMonitorsChange(window, () =>
        {
            _ = updates.Run(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);
        });
        NavigationStates.CROSSHAIR_DISPLAY.PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel))
            {
                _ = updates.Run(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);
            }
        };
        _ = updates.Run(NavigationStates.CROSSHAIR_DISPLAY.SelectionLabel);
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
        var foregroundRestorer = new MainWindowForegroundRestorer();

        Debug.Print($"CrosshairWindowLayoutBehavior UpdateWindowPosition {monitorIndex} {monitor.DeviceName} {monitor.Bounds}  selectionLabel: {selectionLabel}");

        await ((Storyboard)window.FindResource("FadeOut")).BeginAsync();

        // apply layout
        var width = 26 * monitor.ScaleFactor;
        var height = 26 * monitor.ScaleFactor;
        var x = monitor.Bounds.X + ((monitor.Bounds.Width - width) / 2);
        var y = monitor.Bounds.Y + ((monitor.Bounds.Height - height) / 2);
        Monitors.MoveWindow(window, x, y, width, height);

        foregroundRestorer.Restore();

        await ((Storyboard)window.FindResource("FadeIn")).BeginAsync();
    }

}
