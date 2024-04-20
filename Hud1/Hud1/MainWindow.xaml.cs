using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using DependencyObjectExtensions;
using Hud1.Controls;
using Hud1.Model;
using Hud1.Service;
using Stateless;
using Stateless.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Hud1
{
    public partial class MainWindow : Window
    {
        WindowModel windowModel = new WindowModel();

        StateMachine<string, string> nav;

        CoreAudioController audioController;

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
            Debug.WriteLine("OnWindowLoaded {0}", hwnd);

            WindowsServices.SetWindowExTransparent(hwnd);

            int width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

            Debug.WriteLine("Window_Loaded {0} {1}", width, height);

            this.Width = width;
            this.Height = height;
            this.Left = 0;
            this.Top = 0;

            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current?.Dispatcher.Invoke(new Action(() => { ShowApp(); }));
            });

            GlobalKeyboardManager.HandleKeyDown = HandleKeyDown;
            GlobalKeyboardManager.SetupSystemHook();
        }

        private bool HandleKeyDown(GlobalKey key, bool alt)
        {
            Debug.Print("HandleKeyDown {0} {1}", key, alt);

            if (alt)
            {
                if (key == GlobalKey.VK_S || key == GlobalKey.VK_F || key == GlobalKey.VK_L)
                {
                    windowModel.Active = !windowModel.Active;
                    return true;
                }
            }
            else
            {
                if (key == GlobalKey.VK_F2)
                {
                    windowModel.Active = !windowModel.Active;
                }
                else if (windowModel.Active)
                {
                    //return ListenerOnKeyPressed(key);
                }

            }
            return false;
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


      

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            GlobalKeyboardManager.ShutdownSystemHook();
        }
    }
}