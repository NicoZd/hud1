using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using Hud1.Controls;
using Hud1.Converters;
using Hud1.Model;
using Stateless;
using Stateless.Graph;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;


using DependencyObjectExtensions;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;
using Stateless.Reflection;

namespace Hud1
{

    public partial class MainWindow : Window
    {

        WindowModel windowModel = new WindowModel();

        StateMachine<string, string> nav;

        KeyboardListener listener;

        CoreAudioController audioController;


        nint hwnd;

        public MainWindow()
        {
            this.DataContext = windowModel;

            audioController = new CoreAudioController();
            audioController.AudioDeviceChanged.Subscribe(OnDeviceChanged);

            Opacity = 0;
        }

        private void UpdateModelFromStateless()
        {
            Debug.Print("UpdateModelFromStateless {0} ", nav.State);

            windowModel.State = nav.State;

            if (nav.IsInState("left-panel"))
                windowModel.Panel = "left-panel";

            if (nav.IsInState("right-panel"))
                windowModel.Panel = "right-panel";

            if (nav.IsInState("top-panel"))
                windowModel.Panel = "top-panel";

            if (nav.IsInState("bottom-panel"))
                windowModel.Panel = "bottom-panel";

            if (nav.IsInState("center"))
                windowModel.Panel = "center";

            var info = nav.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                string name = stateInfo.ToString();
                Debug.Print("XXX {0}", stateInfo.ToString());
                windowModel.States[name] = new { 
                    State = nav.State,
                    Selected = nav.State.Equals(name),
                    Visibility = nav.State.Equals(name) ? Visibility.Visible : Visibility.Hidden,
                };
            }
            windowModel.OnPropertyChanged("States");
        }

        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            Debug.Print("OnTransitionCompleted {0}", e);
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

            listener = new KeyboardListener();
            listener.KeyboardDownEvent += ListenerOnKeyPressed;

            RebuildUI();

            Task.Delay(0).ContinueWith(_ => {
                Application.Current?.Dispatcher.Invoke(new Action(() => { ShowApp(); }));
            });
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

        private void RebuildUI()
        {
            Debug.Print("RebuildUI");

            nav = new StateMachine<string, string>("center");
            nav.OnTransitionCompleted(a => UpdateModelFromStateless());

            nav.Configure("all")
                .Permit("reset", "center");

            nav.Configure("center")
                .SubstateOf("all");

            nav.Configure("left-panel")
                .SubstateOf("all")
                .Permit("right", "center");

            nav.Configure("top-panel")
                .SubstateOf("all")
                .Permit("down", "center");

            nav.Configure("bottom-panel")
                .SubstateOf("all")
                .Permit("up", "center");

            nav.Configure("center")
                .Permit("up", "cross-visible")
                .Permit("down", "gamma-1.0");

            // top panel
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

            nav.Configure("gamma-1.0")
                .SubstateOf("bottom-panel")
                .Permit("right", "gamma-1.1");

            nav.Configure("gamma-1.1")
                .SubstateOf("bottom-panel")
                .Permit("left", "gamma-1.0");



            var playbackContainer = this.FindUid("PlaybackContainer") as StackPanel;
            playbackContainer.Children.Clear();

            var devices = audioController.GetPlaybackDevices(DeviceState.Active).ToArray();
            if (devices.Length > 0)
            {
                nav.Configure("center")
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

        private void ListenerOnKeyPressed(object sender, KeyEventArgs e)
        {
            // TYPE YOUR CODE HERE
            Debug.WriteLine("ListenerOnKeyPressed {0}", e.Key);

            if (e.Key == Key.F2)
            {
                windowModel.Active = !windowModel.Active;
                nav.Fire("reset");
            }

            if (e.Key == Key.Up)
            {
                if (nav.CanFire("up"))
                {
                    nav.Fire("up");
                    Debug.Print("nav: {0}", nav.State);
                }
            }
            if (e.Key == Key.Down)
            {
                if (nav.CanFire("down"))
                {
                    nav.Fire("down");
                    Debug.Print("nav: {0}", nav.State);
                }
            }
            if (e.Key == Key.Left)
            {
                if (nav.CanFire("left"))
                {
                    nav.Fire("left");
                    Debug.Print("nav: {0}, {1}", nav.State, nav.IsInState("left-panel"));
                }
            }
            if (e.Key == Key.Right)
            {
                if (nav.CanFire("right"))
                {
                    nav.Fire("right");
                    Debug.Print("nav: {0}", nav.State);
                }
            }
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            listener.KeyboardDownEvent -= ListenerOnKeyPressed;
        }
    }
}