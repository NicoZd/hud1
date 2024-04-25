using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;
using Stateless.Graph;
using Stateless.Reflection;
using System.Diagnostics;
using System.Windows;
namespace Hud1.ViewModels
{
    public partial class HudViewModel : ObservableObject
    {
        [ObservableProperty]
        public NavigationState? state;

        [ObservableProperty]
        public Dictionary<string, NavigationState> states = new Dictionary<string, NavigationState> { };

        [ObservableProperty]
        public AudioDeviceViewModel audioDeviceViewModel = new();

        public Stateless.StateMachine<NavigationState, NavigationTrigger> Navigation;

        private readonly string[] Styles = ["Green", "Dark", "Red"];

        public HudViewModel()
        {
            Navigation = new(NavigationStates.MENU_GAMMA);

            // MENU
            Navigation.Configure(NavigationStates.MENU_GAMMA)
                .SubstateOf(NavigationStates.GAMMA_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_AUDIO);

            Navigation.Configure(NavigationStates.MENU_AUDIO)
                .SubstateOf(NavigationStates.AUDIO_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_GAMMA)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.DOWN, NavigationStates.PLAYBACK_DEVICE);

            Navigation.Configure(NavigationStates.MENU_MACRO)
                .SubstateOf(NavigationStates.MACRO_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_AUDIO)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_CROSSHAIR);

            Navigation.Configure(NavigationStates.MENU_CROSSHAIR)
                .SubstateOf(NavigationStates.CROSSHAIR_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MORE);

            Navigation.Configure(NavigationStates.MENU_MORE)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_CROSSHAIR)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_GAMMA)
                .Permit(NavigationTriggers.DOWN, NavigationStates.EXIT);

            // SOUND
            NavigationStates.PLAYBACK_DEVICE.LeftAction = audioDeviceViewModel.SelectPrevDevice;
            NavigationStates.PLAYBACK_DEVICE.RightAction = audioDeviceViewModel.SelectNextDevice;
            Navigation.Configure(NavigationStates.PLAYBACK_DEVICE)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.MENU_AUDIO)
               .Permit(NavigationTriggers.DOWN, NavigationStates.PLAYBACK_VOLUME)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_DEVICE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_DEVICE.ExecuteRight);

            NavigationStates.PLAYBACK_VOLUME.LeftAction = audioDeviceViewModel.VolumeDown;
            NavigationStates.PLAYBACK_VOLUME.RightAction = audioDeviceViewModel.VolumeUp;
            Navigation.Configure(NavigationStates.PLAYBACK_VOLUME)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.PLAYBACK_DEVICE)
               .Permit(NavigationTriggers.DOWN, NavigationStates.PLAYBACK_MUTE)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_VOLUME.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_VOLUME.ExecuteRight);

            //NavigationStates.PLAYBACK_MUTED.LeftAction = audioDeviceViewModel.Mute();
            //NavigationStates.PLAYBACK_MUTED.RightAction = audioDeviceViewModel.Unmute();
            Navigation.Configure(NavigationStates.PLAYBACK_MUTE)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.PLAYBACK_VOLUME)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_MUTE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_MUTE.ExecuteRight);

            // MORE
            NavigationStates.EXIT.RightAction = Application.Current.Shutdown;
            Navigation.Configure(NavigationStates.EXIT)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.DOWN, NavigationStates.STYLE)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.EXIT.ExecuteRight);

            NavigationStates.STYLE.LeftAction = PrevStyle;
            NavigationStates.STYLE.RightAction = NextStyle;
            Navigation.Configure(NavigationStates.STYLE)
               .SubstateOf(NavigationStates.MORE_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.EXIT)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.STYLE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.STYLE.ExecuteRight);


            string graph = UmlDotGraph.Format(Navigation.GetInfo());
            Debug.Print(graph);

            Navigation.OnUnhandledTrigger((state, trigger) =>
            {
                Debug.Print("OnUnhandledTrigger {0} {1}", state, trigger);
            });
            Navigation.OnTransitionCompleted(a => UpdateModelFromStateless());
            UpdateModelFromStateless();
        }

        public bool OnKeyPressed(GlobalKey key)
        {
            Debug.WriteLine("ListenerOnKeyPressed {0}", key);

            if (key == GlobalKey.VK_LEFT)
            {
                Navigation.Fire(NavigationTriggers.LEFT);
                return true;
            }

            if (key == GlobalKey.VK_RIGHT)
            {
                Navigation.Fire(NavigationTriggers.RIGHT);
                return true;
            }

            if (key == GlobalKey.VK_UP)
            {
                Navigation.Fire(NavigationTriggers.UP);
                return true;
            }

            if (key == GlobalKey.VK_DOWN)
            {
                Navigation.Fire(NavigationTriggers.DOWN);
                return true;
            }

            return false;
        }

        void NextStyle()
        {
            Debug.Print("NextStyle");
            var currentStyleIndex = Array.IndexOf(Styles, NavigationStates.STYLE.SelectionLabel);
            var nextStyleIndex = (currentStyleIndex + 1) % Styles.Length;
            NavigationStates.STYLE.SelectionLabel = Styles[nextStyleIndex];
            App.SelectStyle(NavigationStates.STYLE.SelectionLabel);
        }
        void PrevStyle()
        {
            Debug.Print("PrevStyle");
            var currentStyleIndex = Array.IndexOf(Styles, NavigationStates.STYLE.SelectionLabel);
            var prevStyleIndex = (currentStyleIndex - 1 + Styles.Length) % Styles.Length;
            NavigationStates.STYLE.SelectionLabel = Styles[prevStyleIndex];
            App.SelectStyle(NavigationStates.STYLE.SelectionLabel);
        }

        void UpdateModelFromStateless()
        {
            // Debug.Print("UpdateModelFromStateless {0} ", nav.State);

            State = Navigation.State;

            var newStates = new Dictionary<string, NavigationState> { };

            var info = Navigation.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                var navigationState = stateInfo.UnderlyingState as NavigationState;
                var isInState = Navigation.IsInState(navigationState);
                navigationState.Selected = isInState;
                navigationState.Visibility = isInState ? Visibility.Visible : Visibility.Collapsed;

                newStates[navigationState.Name] = navigationState;
            }

            States = newStates;
        }
    }
}