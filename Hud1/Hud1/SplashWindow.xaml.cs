using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Helpers.ScreenHelper;
using Hud1.Helpers.ScreenHelper.Enum;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Hud1;

[INotifyPropertyChanged]
public partial class SplashWindow : Window
{
    public static SplashWindow? Instance;

    [ObservableProperty]
    private string _splashText;

    public SplashWindow()
    {
        Debug.Print("SplashWindow {0}", Hud1.Entry.Millis());
        Application.Current.MainWindow = this;

        Instance = this;
        Opacity = 0;

        SplashText = "Start";
        InitializeComponent();
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        Debug.Print("SplashWindow OnWindowLoaded {0}", Hud1.Entry.Millis());
        var hwnd = new WindowInteropHelper(this).Handle;
        var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
        WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
            extendedStyle
            | WindowsAPI.WS_EX_NOACTIVATE
            | WindowsAPI.WS_EX_TRANSPARENT
            );

        this.SetWindowPosition(WindowPositions.Center, Screen.PrimaryScreen);

        FadeIn();
    }

    private void FadeIn()
    {
        var animation = new DoubleAnimation
        {
            To = 1,
            BeginTime = TimeSpan.FromSeconds(0),
            Duration = TimeSpan.FromSeconds(0.15),
            FillBehavior = FillBehavior.Stop
        };
        animation.Completed += async (s, a) =>
        {
            Debug.Print("SplashWindow Animation In Complete {0}", Hud1.Entry.Millis());
            Opacity = 1;
            await StartupAndShowMainWindow();

        };
        BeginAnimation(UIElement.OpacityProperty, animation);
    }

    private async Task StartupAndShowMainWindow()
    {
        try
        {
            await Startup.Run();
            MainWindow.Create();
            CloseWithAnimation();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Opacity = 0;
            MessageBox.Show("Wooo - there was a fatal startup error:\n\n" + ex.ToString(), "Game Direct", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void CloseWithAnimation()
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
            Debug.Print("SplashWindow Animation Out Complete {0}", Hud1.Entry.Millis());
            Opacity = 0;
            Close();
        };
        BeginAnimation(UIElement.OpacityProperty, animation);
    }
}
