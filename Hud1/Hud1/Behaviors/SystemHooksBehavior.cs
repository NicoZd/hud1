using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace Hud1.Behaviors;


internal class SystemHooksBehavior : Behavior<Window>
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
        VirtualKeyboardHook.SystemHook();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        GlobalMouseHook.SystemUnhook();
        VirtualKeyboardHook.SystemUnhook();
    }
}
