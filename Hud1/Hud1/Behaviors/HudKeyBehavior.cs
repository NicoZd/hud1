using Hud1.Helpers;
using Hud1.Models;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Behaviors;


internal class HudKeyBehavior : Behavior<UserControl>
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
        GlobalKeyboardHook.KeyDown += HandleKeyDown;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        GlobalKeyboardHook.KeyDown -= HandleKeyDown;
    }

    private void HandleKeyDown(KeyEvent keyEvent)
    {
        // Debug.Print("Hud HandleKeyDown");
        if (!keyEvent.alt)
        {
            if (MainWindowViewModel.Instance.Active && MainWindowViewModel.Instance.IsForeground)
            {
                OnKeyPressed(keyEvent);
            }
        }
    }

    private void OnKeyPressed(KeyEvent keyEvent)
    {
        var key = keyEvent.key;

        var State = HudViewModel.Instance.State!;

        NavigationState.Repeat = keyEvent.repeated;
        var isVerticalNavigation = key is GlobalKey.VK_UP or GlobalKey.VK_DOWN;

        if (NavigationState.Repeat && (!State.AllowRepeat || isVerticalNavigation))
        {
            //Console.WriteLine("Skip {0}", keyEvent.key);
            return;
        }

        Console.WriteLine("HudKeyBehavior Execute {0} {1} {2}", State.Name, State.AllowRepeat, keyEvent.key);

        if (key == GlobalKey.VK_LEFT)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.LEFT);
        }

        if (key == GlobalKey.VK_RIGHT)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.RIGHT);
        }

        if (key == GlobalKey.VK_UP)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.UP);
        }

        if (key == GlobalKey.VK_DOWN)
        {
            HudViewModel.Instance.Fire(NavigationTriggers.DOWN);
        }
    }
}
