//using Hud1.Service;
using Hud1.Helpers;
using Hud1.Helpers.CustomSplashScreen;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Hud1
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel windowModel = new MainWindowViewModel();
        nint hwnd;

        public MainWindow()
        {
            Thread.CurrentThread.Name = "MainWindow";
            this.DataContext = windowModel;

            Opacity = 0;

            this.StateChanged += OnStateChanged;
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine("OnStateChanged {0}", this.WindowState);

            if (this.WindowState != WindowState.Normal)
            {
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = false;
                this.Topmost = true;
            }
        }
        private void OnWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnWindowActivated");
            windowModel.Active = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            hwnd = new WindowInteropHelper(this).Handle;
            windowModel.Hwnd = hwnd;
            windowModel.Window = this;
            Debug.WriteLine("OnWindowLoaded {0}", hwnd);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((_, _) =>
            {
                SetWindowPos(hwnd, SafeNativeMethods.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
            WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE, extendedStyle | WindowsAPI.WS_EX_TOOLWINDOW);

            int width = (int)SystemParameters.PrimaryScreenWidth;
            int height = (int)SystemParameters.PrimaryScreenHeight;

            Debug.WriteLine("Window_Loaded {0} {1}", width, height);

            this.Width = width;
            this.Height = height;
            this.Left = 0;
            this.Top = 0;

            GlobalMouseHook.SystemHook();

            GlobalKeyboardHook.KeyDown += HandleKeyDown;
            GlobalKeyboardHook.SystemHook();

            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current?.Dispatcher.Invoke(new Action(() => { ShowApp(); }));
            });
        }
        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            GlobalKeyboardHook.SystemUnhook();
        }

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        private void HandleKeyDown(KeyEvent keyEvent)
        {
            //Debug.Print("HandleKeyDown2 {0} {1}", keyEvent.key, keyEvent.alt);           

            if (keyEvent.alt)
            {
                if (keyEvent.key == GlobalKey.VK_S || keyEvent.key == GlobalKey.VK_F || keyEvent.key == GlobalKey.VK_L)
                {
                    windowModel.Active = !windowModel.Active;
                    keyEvent.block = true;
                }
            }
            else
            {
                if (keyEvent.key == GlobalKey.VK_F2)
                {
                    windowModel.Active = !windowModel.Active;
                    keyEvent.block = true;
                }
            }
        }

        private void ShowApp()
        {
            var animation = new DoubleAnimation
            {
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(0.15),
                FillBehavior = FillBehavior.Stop
            };
            animation.Completed += (s, a) => this.Opacity = 1;
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }

    }
}