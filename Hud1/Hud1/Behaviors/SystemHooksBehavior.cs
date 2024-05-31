using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Windows.UI;

namespace Hud1.Behaviors;


public class SystemHooksBehavior : Behavior<Window>
{

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.Unloaded -= OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        GlobalMouseHook.SystemHook();
        GlobalKeyboardHook.SystemHook();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        GlobalMouseHook.SystemUnhook();
        GlobalKeyboardHook.SystemUnhook();
    }
}
