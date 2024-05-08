using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Hud1.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public Boolean _active = true;

        [ObservableProperty]
        public Visibility _hudVisibility = Visibility.Visible;

        internal void HandleKeyActivator()
        {
            Debug.Print("HandleKeyActivator");

            if (Active)
            {
                HudVisibility = Visibility.Collapsed;
                Active = false;
            }
            else
            {
                HudVisibility = Visibility.Visible;
                Active = true;

            }
        }
    }
}
