using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    [INotifyPropertyChanged]
    public partial class Hud : UserControl
    {
        [ObservableProperty]
        public HudViewModel viewModel = new();

        public Hud()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            GlobalKeyboardManager.KeyDown += HandleKeyDown;
        }

        private void HandleKeyDown(KeyEvent keyEvent)
        {
            // Debug.Print("HandleKeyDown {0} {1}", key, alt);
            if (!keyEvent.alt)
            {
                if ((DataContext as MainWindowViewModel)!.Active)
                {
                    var block = ViewModel.OnKeyPressed(keyEvent.key);
                    //keyEvent.block = block;
                }
            }
        }
    }
}
