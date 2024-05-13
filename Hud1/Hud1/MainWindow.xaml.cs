//using Hud1.Service;
using Hud1.Helpers;
using Hud1.Helpers.CustomSplashScreen;
using Hud1.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Windows.Storage;
using WpfScreenHelper;

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
            Console.WriteLine("OnWindowActivated");
            windowModel.Active = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            hwnd = new WindowInteropHelper(this).Handle;
            windowModel.Window = this;
            windowModel.Hwnd = hwnd;

            Debug.WriteLine("OnWindowLoaded {0}", hwnd);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((_, _) =>
            {
                WindowsAPI.SetWindowPos(hwnd, SafeNativeMethods.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
            WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
                extendedStyle
                | WindowsAPI.WS_EX_NOACTIVATE
                );


            int index = Screen.AllScreens.Count();
            this.SetWindowPosition(WpfScreenHelper.Enum.WindowPositions.Maximize, Screen.AllScreens.ElementAt(index - 1));

            GlobalMouseHook.SystemHook();

            GlobalKeyboardHook.KeyDown += HandleKeyDown;
            GlobalKeyboardHook.SystemHook();

            FadeIn();
        }
        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            GlobalKeyboardHook.SystemUnhook();
        }

        private void HandleKeyDown(KeyEvent keyEvent)
        {
            //Console.WriteLine("HandleKeyDown2 {0} {1}", keyEvent.key, keyEvent.alt);           

            if (!keyEvent.repeated)
            {
                if (keyEvent.alt)
                {
                    if (keyEvent.key == GlobalKey.VK_S || keyEvent.key == GlobalKey.VK_F || keyEvent.key == GlobalKey.VK_L)
                    {
                        windowModel.HandleKeyActivator();
                        keyEvent.block = true;
                    }
                }
                else
                {
                    if (keyEvent.key == GlobalKey.VK_F2)
                    {
                        windowModel.HandleKeyActivator();
                        keyEvent.block = true;
                    }
                }
            }

        }

        private async void FadeIn()
        {
            await Task.Delay(500);

            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                var crosshairWindow = new CrosshairWindow();
                crosshairWindow.Opacity = 0;
                crosshairWindow.Show();

                var animation = new DoubleAnimation
                {
                    To = 1,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = TimeSpan.FromSeconds(0.15),
                    FillBehavior = FillBehavior.Stop
                };

                animation.Completed += (s, a) =>
                {
                    crosshairWindow.Opacity = 1;
                    this.Opacity = 1;
                };

                this.BeginAnimation(UIElement.OpacityProperty, animation);
                crosshairWindow.BeginAnimation(UIElement.OpacityProperty, animation);
            }));
        }
    }
}