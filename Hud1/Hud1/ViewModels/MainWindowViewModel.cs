using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("HudVisibility")]
        public Boolean active = false;

        public nint Hwnd = 0;
        public Window? Window = null;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImportAttribute("User32.dll")]
        private static extern IntPtr SetForegroundWindow(int hWnd);

        private IntPtr handle;

        partial void OnActiveChanged(bool isActive)
        {
            if (Window == null)
                return;

            Debug.Print("OnActiveChanged", isActive);

            var extendedStyle = WindowsAPI.GetWindowLong(Hwnd, WindowsAPI.GWL_EXSTYLE);
            if (isActive)
            {
                handle = GetForegroundWindow();

                Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01ff0000"));
                Window.Activate();
                Window.Focus();
                WindowsAPI.SetWindowLong(Hwnd, WindowsAPI.GWL_EXSTYLE, extendedStyle ^ WindowsAPI.WS_EX_TRANSPARENT);
            }
            else
            {
                Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
                SetForegroundWindow((int)handle);
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
