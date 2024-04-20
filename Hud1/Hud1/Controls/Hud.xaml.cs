using Hud1.Model;
using Hud1.Service;
using Stateless;
using Stateless.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi;
using DependencyObjectExtensions;
using System.Windows.Interop;
using System.Text.RegularExpressions;

namespace Hud1.Controls
{


    public partial class Hud : UserControl
    {
        HudModel Model = new();
        CoreAudioController AudioController = new();

        StateMachine<string, string> Navigation;

        public Hud()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this.Model;

            AudioController = new CoreAudioController();
            //AudioController.AudioDeviceChanged.Subscribe(OnDeviceChanged);

            RebuildNav();
            UpdateModelFromStateless();
        }
        public void OnDeviceChanged(DeviceChangedArgs args)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => RebuildUI()));
        }

        private void RebuildUI()
        {
            Debug.Print("RebuildUI");

            RebuildNav();

            var playbackContainer = this.FindUid("PlaybackContainer") as StackPanel;
            playbackContainer.Children.Clear();

            var devices = AudioController.GetPlaybackDevices(DeviceState.Active).ToArray();
            if (devices.Length > 0)
            {
                Navigation.Configure("menu")
                    .Permit("left", devices[0].Id.ToString());
            }

            for (var i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                var state = Navigation.Configure(device.Id.ToString()).SubstateOf("left-panel");
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


        private void RebuildNav()
        {
            Navigation = new StateMachine<string, string>("menu-gamma");
            Navigation.OnTransitionCompleted(a => UpdateModelFromStateless());

            Navigation.Configure("menu-gamma")
                .SubstateOf("gamma-visible")
                .Permit("right", "menu-audio")
                .Permit("left", "menu-more");

            Navigation.Configure("menu-audio")
                .SubstateOf("audio-visible")
                .Permit("right", "menu-macro")
                .Permit("left", "menu-gamma");

            Navigation.Configure("menu-macro")
                .SubstateOf("macro-visible")
                .Permit("right", "menu-crosshair")
                .Permit("left", "menu-audio");

            Navigation.Configure("menu-crosshair")
                .SubstateOf("crosshair-visible")
                .Permit("right", "menu-more")
                .Permit("left", "menu-macro");

            Navigation.Configure("menu-more")
                .SubstateOf("help-visible")
                .Permit("right", "menu-gamma")
                .Permit("left", "menu-crosshair")
                .Permit("down", "exit");

            Navigation.Configure("exit")
                .SubstateOf("help-visible")
                .Permit("right", "exit-right")
                .Permit("up", "menu-more");

            Navigation.Configure("exit-right")
                .SubstateOf("help-visible")
                .Permit("return", "exit");

            // cross
            Navigation.Configure("cross-visible")
                .SubstateOf("top-panel")
                .Permit("right", "cross-color");

            Navigation.Configure("cross-color")
                .SubstateOf("top-panel")
                .Permit("left", "cross-visible")
                .Permit("right", "cross-form");

            Navigation.Configure("cross-form")
                .SubstateOf("top-panel")
                .Permit("left", "cross-color")
                .Permit("right", "cross-size");

            Navigation.Configure("cross-size")
                .SubstateOf("top-panel")
                .Permit("left", "cross-form");

            // gamma

            Navigation.Configure("gamma-1.0")
                .SubstateOf("bottom-panel")
                .Permit("right", "gamma-1.1");

            Navigation.Configure("gamma-1.1")
                .SubstateOf("bottom-panel")
                .Permit("left", "gamma-1.0");
        }



        private void UpdateModelFromStateless()
        {
            // Debug.Print("UpdateModelFromStateless {0} ", nav.State);

            Model.State = Navigation.State;

            if (Navigation.State == "exit-right")
            {
                Debug.Print("Some Action1");
                Debug.Print("Some Action2 {0}", Thread.CurrentThread.Name);
                this.Wait(100, () =>
                {
                    Navigation.Fire("return");
                    this.Wait(100, () => { Application.Current.Shutdown(); });
                });

            }

            var info = Navigation.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                string name = stateInfo.ToString();
                // Debug.Print("Add State {0} {1} {2}", name, nav.State, nav.State.Equals(name));
                Model.States[name] = new
                {
                    State = Navigation.State,
                    Selected = Navigation.IsInState(name),
                    Visibility = Navigation.IsInState(name) ? Visibility.Visible : Visibility.Collapsed,
                };
            }
            Model.OnPropertyChanged("States");
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            RebuildUI();

            //            GlobalKeyboardManager.HandleKeyDown = HandleKeyDown;
            //            GlobalKeyboardManager.SetupSystemHook();
        }

        private bool HandleKeyDown(GlobalKey key, bool alt)
        {
            // Debug.Print("HandleKeyDown {0} {1}", key, alt);

            if (!alt)
            {
                if ((DataContext as WindowModel)!.Active)
                {
                    return ListenerOnKeyPressed(key);
                }

            }
            return false;
        }

        private bool ListenerOnKeyPressed(GlobalKey key)
        {
            // Debug.WriteLine("ListenerOnKeyPressed {0}", key);

            if (key == GlobalKey.VK_UP)
            {
                if (Navigation.CanFire("up"))
                {
                    Navigation.Fire("up");
                }
                return false;
            }
            if (key == GlobalKey.VK_DOWN)
            {
                if (Navigation.CanFire("down"))
                {
                    Navigation.Fire("down");
                }
                return false;
            }
            if (key == GlobalKey.VK_LEFT)
            {
                if (Navigation.CanFire("left"))
                {
                    Navigation.Fire("left");
                }
                return false;
            }
            if (key == GlobalKey.VK_RIGHT)
            {
                if (Navigation.CanFire("right"))
                {
                    Navigation.Fire("right");
                }
                return false;
            }
            return false;
        }

    }
}
