using Hud1.Helpers;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using WpfScreenHelper;

namespace Hud1;

public partial class MainWindow : Window
{
    nint hwnd;

    public MainWindow()
    {
        Opacity = 0;
        MoreViewModel.Instance.PropertyChanged += OnPropertyChanged;

        InitializeComponent();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MoreViewModel.Instance.HudPosition))
        {
            _ = ApplyHudPosition(true);
        }
    }

    private async Task ApplyHudPosition(bool animate)
    {
        var screenIndex = Int32.Parse(MoreViewModel.Instance.HudPosition.Split(":")[0]);

        if (animate)
            Opacity = 0;

        await Task.Delay(30);

        this.SetWindowPosition(WpfScreenHelper.Enum.WindowPositions.Maximize, Screen.AllScreens.ElementAt(screenIndex));

        await Task.Delay(30);

        if (animate)
        {
            var animation = new DoubleAnimation
            {
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0.0),
                Duration = TimeSpan.FromSeconds(0.15),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) =>
            {
                this.Opacity = 1;
            };
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }

    private void OnWindowActivated(object sender, EventArgs e)
    {
        Console.WriteLine("OnWindowActivated");
        MainWindowViewModel.Instance.Active = true;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        hwnd = new WindowInteropHelper(this).Handle;
        HwndSource source = HwndSource.FromHwnd(hwnd);
        source.AddHook(WndProc);

        MainWindowViewModel.Instance.Hwnd = hwnd;

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

        _ = ApplyHudPosition(false);

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
                    MainWindowViewModel.Instance.HandleKeyActivator();
                    keyEvent.block = true;
                }
            }
            else
            {
                if (keyEvent.key == GlobalKey.VK_F2)
                {
                    MainWindowViewModel.Instance.HandleKeyActivator();
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