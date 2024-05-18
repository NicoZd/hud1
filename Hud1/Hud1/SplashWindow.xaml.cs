using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WpfScreenHelper;

namespace Hud1
{
    [INotifyPropertyChanged]
    public partial class SplashWindow : Window
    {
        public static SplashWindow Instance;

        [ObservableProperty]
        private string _splashText;

        public SplashWindow()
        {
            Instance = this;
            Debug.Print("SplashWindow {0}", Hud1.Startup.Millis());
            Opacity = 0;

            SplashText = "Start";
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.Print("SplashWindow OnWindowLoaded {0}", Hud1.Startup.Millis());
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
            animation.Completed += (s, a) =>
            {
                this.Opacity = 1;

                ThreadPool.QueueUserWorkItem(async (_) =>
                {
                    Debug.Print("Run Init {0}", Hud1.Startup.Millis());
                    await Entry.Main();
                    Debug.Print("Run Init Complete {0}", Hud1.Startup.Millis());

                    Application.Current?.Dispatcher.Invoke(new Action(() =>
                    {
                        Debug.Print("Show Main {0}", Hud1.Startup.Millis());
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                    }));
                });

            };
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}
