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

        [ObservableProperty]
        public GammaViewModel gammaViewModel = new();

        [ObservableProperty]
        public MacrosViewModel macrosViewModel = new();

        public Stateless.StateMachine<NavigationState, NavigationTrigger> Navigation;

        private readonly string[] Styles = ["Green", "Red"];

        public HudViewModel()
        {
            Navigation = new(NavigationStates.MENU_DISPLAY);

            // MENU
            Navigation.Configure(NavigationStates.MENU_DISPLAY)
                .SubstateOf(NavigationStates.DISPLAY_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MORE)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_AUDIO)
                .Permit(NavigationTriggers.DOWN, NavigationStates.GAMMA)
                .Permit(NavigationTriggers.UP, NavigationStates.GAMMA);


            Navigation.Configure(NavigationStates.MENU_AUDIO)
                .SubstateOf(NavigationStates.AUDIO_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_DISPLAY)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.DOWN, NavigationStates.PLAYBACK_DEVICE)
                .Permit(NavigationTriggers.UP, NavigationStates.CAPTURE_MUTE);

            Navigation.Configure(NavigationStates.MENU_MACRO)
                .SubstateOf(NavigationStates.MACRO_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_AUDIO)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_CROSSHAIR)
                .Permit(NavigationTriggers.UP, NavigationStates.MACROS)
                .Permit(NavigationTriggers.DOWN, NavigationStates.MACROS);

            Navigation.Configure(NavigationStates.MENU_CROSSHAIR)
                .SubstateOf(NavigationStates.CROSSHAIR_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MACRO)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MORE);

            Navigation.Configure(NavigationStates.MENU_MORE)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_CROSSHAIR)
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_DISPLAY)
                .Permit(NavigationTriggers.DOWN, NavigationStates.EXIT);

            // DISPLAY
            NavigationStates.GAMMA.LeftAction = gammaViewModel.SelectPrevGama;
            NavigationStates.GAMMA.RightAction = gammaViewModel.SelectNextGama;
            Navigation.Configure(NavigationStates.GAMMA)
               .SubstateOf(NavigationStates.DISPLAY_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.MENU_DISPLAY)
               .Permit(NavigationTriggers.DOWN, NavigationStates.MENU_DISPLAY)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.GAMMA.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.GAMMA.ExecuteRight);

            // SOUND
            NavigationStates.PLAYBACK_DEVICE.LeftAction = audioDeviceViewModel.SelectPrevPlaybackDevice;
            NavigationStates.PLAYBACK_DEVICE.RightAction = audioDeviceViewModel.SelectNextPlaybackDevice;
            Navigation.Configure(NavigationStates.PLAYBACK_DEVICE)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.MENU_AUDIO)
               .Permit(NavigationTriggers.DOWN, NavigationStates.PLAYBACK_VOLUME)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_DEVICE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_DEVICE.ExecuteRight);

            NavigationStates.PLAYBACK_VOLUME.LeftAction = audioDeviceViewModel.PlaybackVolumeDown;
            NavigationStates.PLAYBACK_VOLUME.RightAction = audioDeviceViewModel.PlaybackVolumeUp;
            Navigation.Configure(NavigationStates.PLAYBACK_VOLUME)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.PLAYBACK_DEVICE)
               .Permit(NavigationTriggers.DOWN, NavigationStates.PLAYBACK_MUTE)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_VOLUME.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_VOLUME.ExecuteRight);

            NavigationStates.PLAYBACK_MUTE.LeftAction = audioDeviceViewModel.PlaybackMute;
            NavigationStates.PLAYBACK_MUTE.RightAction = audioDeviceViewModel.PlaybackUnmute;
            Navigation.Configure(NavigationStates.PLAYBACK_MUTE)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.PLAYBACK_VOLUME)
               .Permit(NavigationTriggers.DOWN, NavigationStates.CAPTURE_DEVICE)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_MUTE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_MUTE.ExecuteRight);

            NavigationStates.CAPTURE_DEVICE.LeftAction = audioDeviceViewModel.SelectPrevCaptureDevice;
            NavigationStates.CAPTURE_DEVICE.RightAction = audioDeviceViewModel.SelectNextCaptureDevice;
            Navigation.Configure(NavigationStates.CAPTURE_DEVICE)
              .SubstateOf(NavigationStates.AUDIO_VISIBLE)
              .Permit(NavigationTriggers.UP, NavigationStates.PLAYBACK_MUTE)
              .Permit(NavigationTriggers.DOWN, NavigationStates.CAPTURE_VOLUME)
              .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CAPTURE_DEVICE.ExecuteLeft)
              .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CAPTURE_DEVICE.ExecuteRight);

            NavigationStates.CAPTURE_VOLUME.LeftAction = audioDeviceViewModel.CaptureVolumeDown;
            NavigationStates.CAPTURE_VOLUME.RightAction = audioDeviceViewModel.CaptureVolumeUp;
            Navigation.Configure(NavigationStates.CAPTURE_VOLUME)
              .SubstateOf(NavigationStates.AUDIO_VISIBLE)
              .Permit(NavigationTriggers.UP, NavigationStates.CAPTURE_DEVICE)
              .Permit(NavigationTriggers.DOWN, NavigationStates.CAPTURE_MUTE)
              .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CAPTURE_VOLUME.ExecuteLeft)
              .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CAPTURE_VOLUME.ExecuteRight);

            NavigationStates.CAPTURE_MUTE.LeftAction = audioDeviceViewModel.CaptureMute;
            NavigationStates.CAPTURE_MUTE.RightAction = audioDeviceViewModel.CaptureUnmute;
            Navigation.Configure(NavigationStates.CAPTURE_MUTE)
              .SubstateOf(NavigationStates.AUDIO_VISIBLE)
              .Permit(NavigationTriggers.UP, NavigationStates.CAPTURE_VOLUME)
              .Permit(NavigationTriggers.DOWN, NavigationStates.MENU_AUDIO)
              .InternalTransition(NavigationTriggers.LEFT, NavigationStates.CAPTURE_MUTE.ExecuteLeft)
              .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.CAPTURE_MUTE.ExecuteRight);

            MacrosViewModel.Navigation = Navigation;
            // MACROS
            Navigation.Configure(NavigationStates.MACROS)
             .SubstateOf(NavigationStates.MACRO_VISIBLE)
             .OnEntryFrom(NavigationTriggers.UP, MacrosViewModel.OnEntryFromBottom)
             .OnEntryFrom(NavigationTriggers.DOWN, MacrosViewModel.OnEntryFromTop)
             .OnEntry(MacrosViewModel.OnEntry)
             .OnExit(MacrosViewModel.OnExit)
             .InternalTransition(NavigationTriggers.LEFT, MacrosViewModel.OnLeft)
             .InternalTransition(NavigationTriggers.RIGHT, MacrosViewModel.OnRight)
             .InternalTransition(NavigationTriggers.UP, MacrosViewModel.OnUp)
             .InternalTransition(NavigationTriggers.DOWN, MacrosViewModel.OnDown)
             .Permit(NavigationTriggers.RETURN, NavigationStates.MENU_MACRO);

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

        public bool OnKeyPressed(KeyEvent keyEvent)
        {
            //Debug.WriteLine("ListenerOnKeyPressed {0}", key);
            if (keyEvent.alt)
                return false;

            var key = keyEvent.key;

            NavigationState.Repeat = keyEvent.repeated;
            var isVerticalNavigation = key == GlobalKey.VK_UP || key == GlobalKey.VK_DOWN;

            if (NavigationState.Repeat && (!State!.AllowRepeat || isVerticalNavigation))
            {
                //Debug.Print("Skip {0}", keyEvent.key);
                return false;
            }

            //Debug.Print("Execute {0} {1} {2}", State.Name, State.AllowRepeat, keyEvent.key);

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
                var navigationState = (NavigationState)stateInfo.UnderlyingState;
                var isInState = Navigation.IsInState(navigationState);
                navigationState.Selected = isInState;
                navigationState.Visibility = isInState ? Visibility.Visible : Visibility.Collapsed;

                newStates[navigationState.Name] = navigationState;
            }

            States = newStates;
        }
    }
}