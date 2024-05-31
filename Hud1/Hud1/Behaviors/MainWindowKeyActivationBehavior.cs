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


public class MainWindowKeyActivationBehavior : Behavior<Window>
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
        //Console.WriteLine("HandleKeyDown2 {0} {1}", keyEvent.key, keyEvent.alt);           

        if (!keyEvent.repeated)
        {
            if (keyEvent.alt)
            {
                if (keyEvent.key is GlobalKey.VK_S or GlobalKey.VK_F or GlobalKey.VK_L)
                {
                    MainWindowViewModel.Instance.HandleKeyActivator();
                    keyEvent.block = true;
                }
            }
            else
            {
                if (keyEvent.key == GlobalKey.VK_F2)
                {
                    MainWindowViewModel.Instance.HandleKeyActivator();
                    keyEvent.block = true;
                }
            }
        }

    }
}
