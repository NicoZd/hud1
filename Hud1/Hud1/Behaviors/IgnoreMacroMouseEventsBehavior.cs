using Hud1.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace Hud1.Behaviors;

internal class IgnoreMacroMouseEventsBehavior : Behavior<Window>
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
        EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
        EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
    }

    private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (MouseService.IgnoreNextEvent)
        {
            e.Handled = true;
        }
    }
}
