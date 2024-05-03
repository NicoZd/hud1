using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Windows;

namespace Hud1.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("HudVisibility")]
        public Boolean active = false;

        [ObservableProperty]
        private nint _hwnd = 0;

        partial void OnActiveChanged(bool isActive)
        {
            Debug.Print("OnActiveChanged", isActive);

            var extendedStyle = WindowsAPI.GetWindowLong(Hwnd, WindowsAPI.GWL_EXSTYLE);
            if (isActive)
            {
                WindowsAPI.SetWindowLong(Hwnd, WindowsAPI.GWL_EXSTYLE, extendedStyle ^ WindowsAPI.WS_EX_TRANSPARENT);
            }
            else
            {
                WindowsAPI.SetWindowLong(Hwnd, WindowsAPI.GWL_EXSTYLE, extendedStyle | WindowsAPI.WS_EX_TRANSPARENT);
            }
        }

        public Visibility HudVisibility
        {
            get
            {
                return Active ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
