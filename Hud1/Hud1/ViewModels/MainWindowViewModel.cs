using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public Boolean _active = true;

        [ObservableProperty]
        public Boolean _bg = false;

        [ObservableProperty]
        public Visibility _hudVisibility = Visibility.Visible;

        public nint Hwnd = -1;
        public Window? Window = null;

        private nint _clientHandle = -1;
        private long _lastActiveMillis = 0;

        internal void HandleKeyActivator()
        {
            if (Window == null)
                return;

            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long dt = milliseconds - _lastActiveMillis;
            var handleDoubleActivator = dt < 200;
            _lastActiveMillis = milliseconds;

            Debug.Print("HandleKeyActivator {0} {1}", dt, handleDoubleActivator);

            if (handleDoubleActivator)
            {
                if (Active)
                {
                    Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33005500"));
                    HudVisibility = Visibility.Visible;
                    Active = true;
                    Bg = true;
                    if (Active)
                    {
                        _clientHandle = WindowsAPI.GetForegroundWindow();
                        Window.Activate();
                    }
                }
                else
                {
                    Active = true;
                    HudVisibility = Visibility.Visible;
                    if (Bg)
                    {
                        Bg = false;
                        Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
                    }
                    else
                    {
                        Bg = true;
                        Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33005500"));
                        _clientHandle = WindowsAPI.GetForegroundWindow();
                        Window.Activate();
                    }
                }
            }
            else
            {

                if (Active)
                {
                    HudVisibility = Visibility.Collapsed;
                    Active = false;

                    Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));

                    if (_clientHandle != -1)
                    {
                        Debug.Print("SetForegroundWindow");
                        WindowsAPI.SetForegroundWindow((int)_clientHandle);
                        _clientHandle = -1;
                    }
                }
                else
                {
                    HudVisibility = Visibility.Visible;
                    Active = true;
                    Bg = false;
                }
            }
        }
    }
}
