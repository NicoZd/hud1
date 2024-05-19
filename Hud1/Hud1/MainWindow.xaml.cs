//using Hud1.Service;
using Hud1.Helpers;
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

            InitializeComponent();
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
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);

            windowModel.Window = this;
            windowModel.Hwnd = hwnd;

            Debug.WriteLine("OnWindowLoaded {0}", hwnd);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((_, _) =>
            {
                WindowsAPI.SetWindowPos(hwnd, WindowsAPI.HWND_TOP, 0, 0, 0, 0, WindowsAPI.SetWindowPosFlags.SWP_NOMOVE | WindowsAPI.SetWindowPosFlags.SWP_NOSIZE | WindowsAPI.SetWindowPosFlags.SWP_NOACTIVATE);
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
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Startup.WM_GAME_DIRECT_SHOWME)
            {
                Console.WriteLine("WndProc {0} {1}", msg, Startup.WM_GAME_DIRECT_SHOWME);
                Application.Current.Shutdown();
            }

            return IntPtr.Zero;
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

        private void FadeIn()
        {
            var animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(0.15),
                FillBehavior = FillBehavior.Stop
            };
            animation.Completed += (s, a) =>
            {
                this.Opacity = 0;
                SplashWindow.Instance!.Close();

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

                Console.WriteLine("MainWindow FadeIn {0}", Entry.Millis());
                this.BeginAnimation(UIElement.OpacityProperty, animation);
                crosshairWindow.BeginAnimation(UIElement.OpacityProperty, animation);

            };
            SplashWindow.Instance!.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}