using Hud1.Helpers;
using Hud1.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using Windows.System;

namespace Hud1.Behaviors;


internal class MainWindowActivationBehavior : Behavior<Window>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
        AssociatedObject.Activated += OnActivated;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.Unloaded -= OnUnloaded;
        AssociatedObject.Activated -= OnActivated;
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
        if (!keyEvent.repeated && keyEvent.alt && keyEvent.shift && keyEvent.key is VirtualKey.G)
        {
            HandleKeyActivator();
            keyEvent.block = true;
        }
    }

    private void HandleKeyActivator()
    {
        MainWindowViewModel.Instance.ToggleActive();
    }

    private void OnActivated(object? sender, EventArgs e)
    {
        MainWindowViewModel.Instance.Activate();
    }
}
