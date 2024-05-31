using Hud1.Helpers;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views;

public partial class Hud : UserControl
{

    public Hud()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            Debug.Print("!!!!Hud Loaded");
            GlobalKeyboardHook.KeyDown += HandleKeyDown;
        };
        Unloaded += (_, _) =>
        {
            Debug.Print("!!!!Hud Unloaded");
            GlobalKeyboardHook.KeyDown -= HandleKeyDown;
        };
    }

    private void HandleKeyDown(KeyEvent keyEvent)
    {
        Debug.Print("Hud HandleKeyDown");
        if (!keyEvent.alt)
        {
            if (MainWindowViewModel.Instance.Active && MainWindowViewModel.Instance.IsForeground)
            {
                HudViewModel.Instance.OnKeyPressed(keyEvent);
            }
        }
    }
}
