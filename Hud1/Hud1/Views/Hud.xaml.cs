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
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Debug.Print("Hud OnWindowLoaded");
        GlobalKeyboardHook.KeyDown += HandleKeyDown;
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
