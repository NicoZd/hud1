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

            audioController = new CoreAudioController();
            audioController.AudioDeviceChanged.Subscribe(OnDeviceChanged);

            Opacity = 0;

            RebuildNav();
            UpdateModelFromStateless();

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

        public static void Wait(int milliseconds, Action action)
        {
            Task.Delay(milliseconds).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(action);
            });
        }

        private void UpdateModelFromStateless()
        {
            // Debug.Print("UpdateModelFromStateless {0} ", nav.State);

            windowModel.State = nav.State;

            if (nav.State == "exit-right")
            {
                Debug.Print("Some Action1");
                Debug.Print("Some Action2 {0}", Thread.CurrentThread.Name);
                Wait(100, () =>
                {
                    nav.Fire("return");
                    Wait(100, () => { Application.Current.Shutdown(); });
                });

            }

            var info = nav.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                string name = stateInfo.ToString();
                // Debug.Print("Add State {0} {1} {2}", name, nav.State, nav.State.Equals(name));
                windowModel.States[name] = new
                {
                    State = nav.State,
                    Selected = nav.IsInState(name),
                    Visibility = nav.IsInState(name) ? Visibility.Visible : Visibility.Collapsed,
                };
            }
            windowModel.OnPropertyChanged("States");
        }

        private void OnDeviceChanged(DeviceChangedArgs x)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => RebuildUI()));
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

            RebuildUI();

            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current?.Dispatcher.Invoke(new Action(() => { ShowApp(); }));
            });

            GlobalKeyboardManager.HandleKeyDown = HandleKeyDown;
            GlobalKeyboardManager.SetupSystemHook();
        }

        private bool HandleKeyDown(GlobalKey key, bool alt)
        {
            // Debug.Print("HandleKeyDown {0} {1}", key, alt);

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
                    return ListenerOnKeyPressed(key);
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

        private void RebuildNav()
        {
            nav = new StateMachine<string, string>("menu-gamma");
            nav.OnTransitionCompleted(a => UpdateModelFromStateless());

            nav.Configure("menu-gamma")
                .SubstateOf("gamma-visible")
                .Permit("right", "menu-audio")
                .Permit("left", "menu-more");

            nav.Configure("menu-audio")
                .SubstateOf("audio-visible")
                .Permit("right", "menu-macro")
                .Permit("left", "menu-gamma");

            nav.Configure("menu-macro")
                .SubstateOf("macro-visible")
                .Permit("right", "menu-crosshair")
                .Permit("left", "menu-audio");

            nav.Configure("menu-crosshair")
                .SubstateOf("crosshair-visible")
                .Permit("right", "menu-more")
                .Permit("left", "menu-macro");

            nav.Configure("menu-more")
                .SubstateOf("help-visible")
                .Permit("right", "menu-gamma")
                .Permit("left", "menu-crosshair")
                .Permit("down", "exit");

            nav.Configure("exit")
                .SubstateOf("help-visible")
                .Permit("right", "exit-right")
                .Permit("up", "menu-more");

            nav.Configure("exit-right")
                .SubstateOf("help-visible")
                .Permit("return", "exit");

            // cross
            nav.Configure("cross-visible")
                .SubstateOf("top-panel")
                .Permit("right", "cross-color");

            nav.Configure("cross-color")
                .SubstateOf("top-panel")
                .Permit("left", "cross-visible")
                .Permit("right", "cross-form");

            nav.Configure("cross-form")
                .SubstateOf("top-panel")
                .Permit("left", "cross-color")
                .Permit("right", "cross-size");

            nav.Configure("cross-size")
                .SubstateOf("top-panel")
                .Permit("left", "cross-form");

            // gamma

            nav.Configure("gamma-1.0")
                .SubstateOf("bottom-panel")
                .Permit("right", "gamma-1.1");

            nav.Configure("gamma-1.1")
                .SubstateOf("bottom-panel")
                .Permit("left", "gamma-1.0");
        }

        private void RebuildUI()
        {
            Debug.Print("RebuildUI");

            RebuildNav();

            var playbackContainer = this.FindUid("PlaybackContainer") as StackPanel;
            playbackContainer.Children.Clear();

            var devices = audioController.GetPlaybackDevices(DeviceState.Active).ToArray();
            if (devices.Length > 0)
            {
                nav.Configure("menu")
                    .Permit("left", devices[0].Id.ToString());
            }

            for (var i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                var state = nav.Configure(device.Id.ToString()).SubstateOf("left-panel");
                if (i > 0)
                    state.Permit("up", devices[i - 1].Id.ToString());

                if (i < devices.Length - 1)
                    state.Permit("down", devices[i + 1].Id.ToString());


                var playbackDeviceButton = new CustomControl2();
                playbackDeviceButton.Label = Regex.Replace(device.InterfaceName, "[0-9]- ", "");

                Binding selectedBinding = new Binding("States[" + devices[i].Id.ToString() + "].Selected");
                playbackDeviceButton.SetBinding(CustomControl2.SelectedProperty, selectedBinding);

                playbackContainer.Children.Add(playbackDeviceButton);
            }

            UpdateModelFromStateless();
        }

        private bool ListenerOnKeyPressed(GlobalKey key)
        {
            // Debug.WriteLine("ListenerOnKeyPressed {0}", key);

            if (key == GlobalKey.VK_UP)
            {
                if (nav.CanFire("up"))
                {
                    nav.Fire("up");
                }
                return false;
            }
            if (key == GlobalKey.VK_DOWN)
            {
                if (nav.CanFire("down"))
                {
                    nav.Fire("down");
                }
                return false;
            }
            if (key == GlobalKey.VK_LEFT)
            {
                if (nav.CanFire("left"))
                {
                    nav.Fire("left");
                }
                return false;
            }
            if (key == GlobalKey.VK_RIGHT)
            {
                if (nav.CanFire("right"))
                {
                    nav.Fire("right");
                }
                return false;
            }
            return false;
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            GlobalKeyboardManager.ShutdownSystemHook();
        }
    }
}