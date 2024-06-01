using Hud1.Helpers;
using Hud1.Models;
using Hud1.Windows;
using Microsoft.Xaml.Behaviors;
using System.Diagnostics;
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
        NavigationStates.TOUCH_MODE.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.TOUCH_MODE.SelectionBoolean))
            {
                UpdateTouchMode();
            }
        };
        UpdateTouchMode();
    }

    private void UpdateTouchMode()
    {
        var hwnd = new WindowInteropHelper(AssociatedObject).Handle;

        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);

        var newStyle = NavigationStates.TOUCH_MODE.SelectionBoolean ?
            extendedStyle | WindowsAPI.WS_EX_NOACTIVATE :
            extendedStyle & ~WindowsAPI.WS_EX_NOACTIVATE;

        Debug.Print("UpdateTouchMode {0} {1} {2}", NavigationStates.TOUCH_MODE.SelectionBoolean, extendedStyle, newStyle);

        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE, newStyle);

        if (!NavigationStates.TOUCH_MODE.SelectionBoolean)
        {
            MainWindow.Instance!.ActivateWindow();
        }
    }
}
