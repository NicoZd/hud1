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
            NavigationStates.PLAYBACK_DEVICE.ExecuteLeftAction = () => Debug.Print("ExecuteLeftAction");
            NavigationStates.PLAYBACK_DEVICE.ExecuteRightAction = () => Debug.Print("ExecuteRightAction");

            Navigation.Configure(NavigationStates.PLAYBACK_DEVICE)
               .SubstateOf(NavigationStates.AUDIO_VISIBLE)
               .Permit(NavigationTriggers.UP, NavigationStates.MENU_AUDIO)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.PLAYBACK_DEVICE.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.PLAYBACK_DEVICE.ExecuteRight);

            // MORE
            NavigationStates.EXIT.ExecuteRightAction = Application.Current.Shutdown;
            Navigation.Configure(NavigationStates.EXIT)
                .SubstateOf(NavigationStates.MORE_VISIBLE)
                .Permit(NavigationTriggers.UP, NavigationStates.MENU_MORE)
                .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.EXIT.ExecuteRight);

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

                newStates[navigationState.Label] = navigationState;
            }

            States = newStates;
        }
    }
}


//Navigation = new StateMachine<string, string>(Navigation != null ? Navigation.State : "menu-gamma");
//Navigation.OnTransitionCompleted(a => UpdateModelFromStateless());

//// menu
//Navigation.Configure("menu-gamma")
//    .SubstateOf("gamma-visible")
//    .Permit("right", "menu-audio")
//    .Permit("left", "menu-more");

//Navigation.Configure("menu-audio")
//    .SubstateOf("audio-visible")
//    .Permit("right", "menu-macro")
//    .Permit("left", "menu-gamma")
//    .Permit("down", "audio-playback-device");

//Navigation.Configure("menu-macro")
//    .SubstateOf("macro-visible")
//    .Permit("right", "menu-crosshair")
//    .Permit("left", "menu-audio");

//Navigation.Configure("menu-crosshair")
//    .SubstateOf("crosshair-visible")
//    .Permit("right", "menu-more")
//    .Permit("left", "menu-macro");

//Navigation.Configure("menu-more")
//    .SubstateOf("help-visible")
//    .Permit("right", "menu-gamma")
//    .Permit("left", "menu-crosshair")
//    .Permit("down", "exit");

//// sound
//Navigation.Configure("audio-playback-device")
//    .SubstateOf("audio-visible")
//    .OnActivate(() => Debug.Print("Activate audio-playback-device"))
//    .Permit("up", "menu-audio");

//// more
//Navigation.Configure("exit")
//    .SubstateOf("help-visible")
//    .Permit("right", "exit-right")
//    .Permit("up", "menu-more");

//Navigation.Configure("exit-right")
//    .SubstateOf("help-visible")
//    .Permit("return", "exit");

