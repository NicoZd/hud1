using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    public partial class Hud : UserControl
    {
        public Hud()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            GlobalKeyboardHook.KeyDown += HandleKeyDown;
        }

        private void HandleKeyDown(KeyEvent keyEvent)
        {
            if (!keyEvent.alt)
            {
                if (MainWindowViewModel.Instance!.Active)
                {
                    HudViewModel.Instance.OnKeyPressed(keyEvent);
                }
            }
        }
    }
}
