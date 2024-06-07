using Hud1.Helpers;
using Hud1.Models;
using Hud1.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Interop;

namespace Hud1.Behaviors;


internal class MainWindowTouchModeBehavior : Behavior<Window>
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
        NavigationStates.DEVELOPER_MODE.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.DEVELOPER_MODE.SelectionBoolean))
            {
                UpdateTouchMode();
            }
        };
        UpdateTouchMode();
    }

    private void UpdateTouchMode()
    {
        var hwnd = new WindowInteropHelper(AssociatedObject).Handle;

        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowConstants.GWL_EXSTYLE);

        var newStyle = NavigationStates.DEVELOPER_MODE.SelectionBoolean ?
            extendedStyle & ~WindowConstants.WS_EX_NOACTIVATE :
            extendedStyle | WindowConstants.WS_EX_NOACTIVATE;

        WindowsAPI.SetWindowLong(hwnd, WindowConstants.GWL_EXSTYLE, newStyle);

        if (NavigationStates.DEVELOPER_MODE.SelectionBoolean)
        {
            MainWindow.Instance!.ActivateWindow();
        }
    }
}
