using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Helpers.ScreenHelper.Enum;
using Hud1.Models;
using Hud1.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Hud1;

public partial class MainWindow : Window
{
    private nint hwnd;

    internal static void Create()
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        Application.Current.MainWindow = mainWindow;
    }

    private MainWindow()
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
        var screenIndex = int.Parse(MoreViewModel.Instance.HudPosition.Split(":")[0]);

        if (animate)
            Opacity = 0;

        await Task.Delay(30);

        var aligmnent = MoreViewModel.Instance.HudAlignment == "Left" ? WindowPositions.Left2 : WindowPositions.Right2;

        var screen = Screen.AllScreens.ElementAt(screenIndex);
        this.SetWindowPosition(aligmnent, screen);

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
                Opacity = 1;
            };
            BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }

    private void OnWindowActivated(object sender, EventArgs e)
    {
        Console.WriteLine("OnWindowActivated");
        MainWindowViewModel.Instance.Active = true;
        MainWindowViewModel.Instance.HudVisibility = Visibility.Visible;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("MainWindow OnWindowLoaded");

        hwnd = new WindowInteropHelper(this).Handle;
        var source = HwndSource.FromHwnd(hwnd);
        source.AddHook(WndProc);

        MainWindowViewModel.Instance.InitWindow(hwnd);

        var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        dispatcherTimer.Tick += new EventHandler((_, _) =>
        {
            WindowsAPI.SetWindowPos(hwnd, WindowsAPI.HWND_TOP, 0, 0, 0, 0, WindowsAPI.SetWindowPosFlags.SWP_NOMOVE | WindowsAPI.SetWindowPosFlags.SWP_NOSIZE | WindowsAPI.SetWindowPosFlags.SWP_NOACTIVATE);
        });
        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        dispatcherTimer.Start();

        _ = ApplyHudPosition(false);

        NavigationStates.TOUCH_MODE.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.TOUCH_MODE.SelectionBoolean))
            {
                Debug.Print("TOUCH_MODE {0}", NavigationStates.TOUCH_MODE.SelectionBoolean);

                UpdateTouchMode();
            }
        };
        UpdateTouchMode();

        GlobalMouseHook.SystemHook();

        GlobalKeyboardHook.KeyDown += HandleKeyDown;
        GlobalKeyboardHook.SystemHook();

        FadeIn();
    }

    private void UpdateTouchMode()
    {
        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);

        var newStyle = NavigationStates.TOUCH_MODE.SelectionBoolean ?
            extendedStyle | WindowsAPI.WS_EX_NOACTIVATE :
            extendedStyle & ~WindowsAPI.WS_EX_NOACTIVATE;

        Debug.Print("UpdateTouchMode {0} {1} {2}", NavigationStates.TOUCH_MODE.SelectionBoolean, extendedStyle, newStyle);

        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE, newStyle);

        if (!NavigationStates.TOUCH_MODE.SelectionBoolean)
        {
            MainWindowViewModel.Instance.Activate();
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Setup.WM_GAME_DIRECT_SHOWME)
        {
            Console.WriteLine("WndProc {0} {1}", msg, Setup.WM_GAME_DIRECT_SHOWME);
            Application.Current.Shutdown();
            return IntPtr.Zero;
        }

        if (msg == 126)
        {
            Debug.Print("Resolution or DPI Change {0}", msg);
            _ = ApplyHudPosition(true);
            return IntPtr.Zero;
        }

        //int[] ignore = [13, 70, 71];
        //if (ignore.Contains(msg))
        //    return IntPtr.Zero;

        //Debug.Print("" + msg);

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
                if (keyEvent.key is GlobalKey.VK_S or GlobalKey.VK_F or GlobalKey.VK_L)
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
        Opacity = 0;

        var crosshairWindow = new CrosshairWindow
        {
            Opacity = 0
        };
        crosshairWindow.Show();

        var animation = new DoubleAnimation
        {
            To = 1,
            BeginTime = TimeSpan.FromSeconds(0.15),
            Duration = TimeSpan.FromSeconds(0.15),
            FillBehavior = FillBehavior.Stop
        };

        animation.Completed += (s, a) =>
        {
            crosshairWindow.Opacity = 1;
            Opacity = 1;
        };

        Console.WriteLine("MainWindow FadeIn {0}", Entry.Millis());
        BeginAnimation(UIElement.OpacityProperty, animation);
        crosshairWindow.BeginAnimation(UIElement.OpacityProperty, animation);
    }
}