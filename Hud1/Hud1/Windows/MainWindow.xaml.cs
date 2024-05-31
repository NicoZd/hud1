using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
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
        InitializeComponent();
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
        MainWindowViewModel.Instance.InitWindow(hwnd);

        NavigationStates.TOUCH_MODE.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationStates.TOUCH_MODE.SelectionBoolean))
            {
                Debug.Print("TOUCH_MODE {0}", NavigationStates.TOUCH_MODE.SelectionBoolean);

                UpdateTouchMode();
            }
        };
        UpdateTouchMode();

        GlobalKeyboardHook.KeyDown += HandleKeyDown;
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
            MainWindowViewModel.Instance.ActivateWindow();
        }
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
}