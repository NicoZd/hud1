using Hud1.Helpers;
using Hud1.ViewModels;
using Stateless;
using Stateless.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hud1.Views
{
    public partial class Hud : UserControl
    {
        public HudViewModel ViewModel = new();
        private static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(HudViewModel), typeof(Hud));

        public AudioDeviceViewModel AudioDeviceViewModel = new();
        private static readonly DependencyProperty AudioDeviceViewModelProperty =
            DependencyProperty.Register("AudioDeviceViewModel", typeof(AudioDeviceViewModel), typeof(Hud));

        StateMachine<string, string> Navigation;

        public Hud()
        {
            InitializeComponent();
            SetValue(ViewModelProperty, ViewModel);
            RebuildNav();
            UpdateModelFromStateless();
        }

        private void RebuildUI()
        {
            Debug.Print("RebuildUI");

            RebuildNav();

            var playbackContainer = this.FindUid("PlaybackContainer") as StackPanel;
            playbackContainer.Children.Clear();

            var devices = AudioDeviceViewModel.PlaybackDevices;
            if (devices.Count > 0)
            {
                Navigation.Configure("menu-audio")
                    .Permit("down", devices[0].ID.ToString());
            }

            for (var i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                //Debug.Print("Device {0} {1}", device.ID, device.DeviceFriendlyName);
                var state = Navigation.Configure(device.ID.ToString());
                if (i > 0)
                    state.Permit("up", devices[i - 1].ID.ToString());

                if (i < devices.Count - 1)
                    state.Permit("down", devices[i + 1].ID.ToString());


                var playbackDeviceButton = new CustomControl2();
                playbackDeviceButton.Label = Regex.Replace(device.DeviceFriendlyName, "[0-9]- ", "");

                Binding selectedBinding = new Binding("States[" + devices[i].ID.ToString() + "].Selected");
                playbackDeviceButton.SetBinding(CustomControl2.SelectedProperty, selectedBinding);

                playbackContainer.Children.Add(playbackDeviceButton);
            }

            UpdateModelFromStateless();
        }


        private void RebuildNav()
        {
            Navigation = new StateMachine<string, string>(Navigation != null ? Navigation.State : "menu-gamma");
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

            ViewModel.State = Navigation.State;

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

            var newStates = new Dictionary<string, object> { };

            var info = Navigation.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                string name = stateInfo.ToString();
                Debug.Print("UpdateModelFromStateless {0} {1}", name, Navigation.IsInState(name));
                // Debug.Print("Add State {0} {1} {2}", name, nav.State, nav.State.Equals(name));
                newStates[name] = new
                {
                    Selected = Navigation.IsInState(name),
                    Visibility = Navigation.IsInState(name) ? Visibility.Visible : Visibility.Collapsed,
                };
            }

            ViewModel.States = newStates;

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            RebuildUI();

            GlobalKeyboardManager.KeyDown += HandleKeyDown;

            AudioDeviceViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "PlaybackDevices")
                {
                    RebuildUI();
                }
            };

        }

        private void HandleKeyDown(KeyEvent keyEvent)
        {
            // Debug.Print("HandleKeyDown {0} {1}", key, alt);

            if (!keyEvent.alt)
            {
                if ((DataContext as MainWindowViewModel)!.Active)
                {
                    ListenerOnKeyPressed(keyEvent.key);
                }
            }
        }

        private void ListenerOnKeyPressed(GlobalKey key)
        {
            // Debug.WriteLine("ListenerOnKeyPressed {0}", key);

            if (key == GlobalKey.VK_UP)
            {
                if (Navigation.CanFire("up"))
                {
                    Navigation.Fire("up");
                }
            }
            if (key == GlobalKey.VK_DOWN)
            {
                if (Navigation.CanFire("down"))
                {
                    Navigation.Fire("down");
                }
            }
            if (key == GlobalKey.VK_LEFT)
            {
                if (Navigation.CanFire("left"))
                {
                    Navigation.Fire("left");
                }
            }
            if (key == GlobalKey.VK_RIGHT)
            {
                if (Navigation.CanFire("right"))
                {
                    Navigation.Fire("right");
                }
            }
        }

    }
}
