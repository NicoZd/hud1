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
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MACRO);

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
                .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_GAMMA);

            string graph = UmlDotGraph.Format(Navigation.GetInfo());
            Debug.Print(graph);

            Navigation.OnTransitionCompleted(a => UpdateModelFromStateless());
            UpdateModelFromStateless();
        }

        public void OnKeyPressed(GlobalKey key)
        {
            // Debug.WriteLine("ListenerOnKeyPressed {0}", key);

            //if (key == GlobalKey.VK_UP)
            //{
            //    if (Navigation.CanFire("up"))
            //    {
            //        Navigation.Fire("up");
            //    }
            //}
            //if (key == GlobalKey.VK_DOWN)
            //{
            //    if (Navigation.CanFire("down"))
            //    {
            //        Navigation.Fire("down");
            //    }
            //}
            //if (key == GlobalKey.VK_LEFT)
            //{
            //    if (Navigation.CanFire("left"))
            //    {
            //        Navigation.Fire("left");
            //    }
            //}

            if (key == GlobalKey.VK_LEFT)
            {
                if (Navigation.CanFire(NavigationTriggers.LEFT))
                {
                    Navigation.Fire(NavigationTriggers.LEFT);
                }
            }

            if (key == GlobalKey.VK_RIGHT)
            {
                if (Navigation.CanFire(NavigationTriggers.RIGHT))
                {
                    Navigation.Fire(NavigationTriggers.RIGHT);
                }
            }
        }

        void UpdateModelFromStateless()
        {
            // Debug.Print("UpdateModelFromStateless {0} ", nav.State);

            State = Navigation.State;

            //if (ViewModel.Navigation.State. == "exit-right")
            //{
            //    Debug.Print("Some Action1");
            //    Debug.Print("Some Action2 {0}", Thread.CurrentThread.Name);
            //    this.Wait(100, () =>
            //    {
            //        Navigation.Fire("return");
            //        this.Wait(100, () => { Application.Current.Shutdown(); });
            //    });

            //}

            var newStates = new Dictionary<string, NavigationState> { };

            var info = Navigation.GetInfo();
            foreach (StateInfo stateInfo in info.States)
            {
                string name = stateInfo.ToString();
                //Debug.Print("UpdateModelFromStateless {0} {1}", name, Navigation.IsInState(name));
                // Debug.Print("Add State {0} {1} {2}", name, nav.State, nav.State.Equals(name));

                var navigationState = stateInfo.UnderlyingState as NavigationState;

                var isInState = Navigation.IsInState(navigationState);
                navigationState.Selected = isInState;
                navigationState.Visibility = isInState ? Visibility.Visible : Visibility.Collapsed;

                newStates[navigationState.Label] = navigationState;

                //newStates[name] = new NavigationState
                //{
                //Selected = ViewModel.Navigation.IsInState(name),
                //Visibility = ViewModel.Navigation.IsInState(name) ? Visibility.Visible : Visibility.Collapsed,
                //};
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

