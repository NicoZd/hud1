using Hud1.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using WpfScreenHelper;

namespace Hud1
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            Debug.Print("SplashWindow {0}", Hud1.Entry0.Millis());
            Opacity = 0;
            InitializeComponent();

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.Print("SplashWindow OnWindowLoaded {0}", Hud1.Entry0.Millis());
            var hwnd = new WindowInteropHelper(this).Handle;
            var extendedStyle = WindowsAPI.GetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE);
            WindowsAPI.SetWindowLong(hwnd, WindowsAPI.GWL_EXSTYLE,
                extendedStyle
                | WindowsAPI.WS_EX_NOACTIVATE
                | WindowsAPI.WS_EX_TRANSPARENT
                );

            var source = PresentationSource.FromVisual(this);

            var dpiScale = new Size(
                source.CompositionTarget.TransformToDevice.M11,
                source.CompositionTarget.TransformToDevice.M22);

            Debug.Print("dpiScale {0}", dpiScale);
            Width = 676 / dpiScale.Width;
            Height = 396 / dpiScale.Height;
            this.SetWindowPosition(WpfScreenHelper.Enum.WindowPositions.Center, Screen.AllScreens.ElementAt(0));

            Opacity = 1;

            ThreadPool.QueueUserWorkItem((_) =>
            {
                Debug.Print("Run Init {0}", Hud1.Entry0.Millis());
                Entry.Main();
                Debug.Print("Run Init Complete {0}", Hud1.Entry0.Millis());

                Application.Current?.Dispatcher.Invoke(new Action(() =>
                {
                    Debug.Print("Show Main {0}", Hud1.Entry0.Millis());
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    Debug.Print("Show Main Done {0}", Hud1.Entry0.Millis());
                    ThreadPool.QueueUserWorkItem((_) =>
                    {
                        Thread.Sleep(1000);
                        Application.Current?.Dispatcher.Invoke(new Action(() =>
                        {
                            this.Close();
                        }));
                    });
                }));
            });
        }
    }
}
