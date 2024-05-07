using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
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

        public int lastX = 0;
        public int lastY = 0;
        public int diffX = 0;
        public int diffY = 0;
        public int startX = 0;
        public int startY = 0;
        internal Canvas CursorCanvas;

        public MainWindowViewModel()
        {
            GlobalMouseHook.MouseMove += OnMouseMove;
        }

        private void OnMouseMove(int x, int y)
        {
            diffX = x - lastX;
            diffY = y - lastY;

            lastX = x;
            lastY = y;

            startX += diffX;
            startY += diffY;

            Debug.Print("lpPoint {0} {1}", startX, startY);

            //if (Active)
            {
                Debug.Print("Canvasa {0}", CursorCanvas);
                //Canvas.SetLeft(CursorCanvas, startX);
                //Canvas.SetTop(CursorCanvas, startY);

                CursorCanvas.SetValue(Canvas.LeftProperty, (double)startX);
                CursorCanvas.SetValue(Canvas.TopProperty, (double)startY);
            }
        }

        internal void HandleKeyActivator()
        {
            GlobalMouseHook.SystemUnhook();
            GlobalMouseHook.SystemHook();

            WindowsAPI.POINT lpPoint;
            WindowsAPI.GetCursorPos(out lpPoint);

            Debug.Print("lpPoint {0} {1}", lpPoint.X, lpPoint.Y);
            startX = lpPoint.X;
            startY = lpPoint.Y;

            WindowsAPI.CursorInfo info = new WindowsAPI.CursorInfo();
            info.Size = Marshal.SizeOf(info.GetType());
            if (WindowsAPI.GetCursorInfo(out info))
            {
                //Debug.
                // info.Handle contains the global cursor handle.
                //
                Debug.Print("Data {0}", info.Flags);

            }
            else
            {
                Debug.Print("Error");
            }

            Debug.Print("HandleKeyActivator {0}");

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
