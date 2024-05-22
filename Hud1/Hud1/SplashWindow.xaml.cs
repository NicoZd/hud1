using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using WpfScreenHelper;

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

        this.SetWindowPosition(WpfScreenHelper.Enum.WindowPositions.Center, Screen.AllScreens.ElementAt(0));

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
            this.Opacity = 1;
            try
            {
                await Startup.Run();
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.Close();
                MessageBox.Show("Wooo - there was a fatal startup error:\n\n" + ex.ToString(), "Game Direct", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        };
        this.BeginAnimation(UIElement.OpacityProperty, animation);
    }


}
