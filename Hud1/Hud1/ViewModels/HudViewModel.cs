using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Helpers;
using Hud1.Models;
using Stateless.Reflection;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Hud1.ViewModels
{
    public partial class HudViewModel : ObservableObject
    {
        public readonly static HudViewModel Instance = new();

        [ObservableProperty]
        public NavigationState? state;

        [ObservableProperty]
        public Dictionary<string, NavigationState> states = new Dictionary<string, NavigationState> { };

        private HudViewModel()
        {
        }

        public void BuildNavigation()
        {
            Debug.Print("HudViewModel BuildNavigation");
            var Navigation = NavigationViewModel.Instance.Navigation;

            Navigation.OnUnhandledTrigger((state, trigger) =>
            {
                Console.WriteLine("OnUnhandledTrigger {0} {1}", state, trigger);
            });

            Navigation.OnTransitionCompleted(a => UpdateModelFromMavigation());

            UpdateModelFromMavigation();
        }

        [RelayCommand]
        private void Select(NavigationState navigationState)
        {
            // Console.WriteLine("Select {0}", navigationState);
            NavigationViewModel.SelectNavigationState(navigationState);
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
                //Console.WriteLine("Skip {0}", keyEvent.key);
                return false;
            }

            //Console.WriteLine("Execute {0} {1} {2}", State.Name, State.AllowRepeat, keyEvent.key);

            if (key == GlobalKey.VK_LEFT)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.LEFT);
                return true;
            }

            if (key == GlobalKey.VK_RIGHT)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.RIGHT);
                return true;
            }

            if (key == GlobalKey.VK_UP)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.UP);
                return true;
            }

            if (key == GlobalKey.VK_DOWN)
            {
                NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.DOWN);
                return true;
            }

            return false;
        }

        void UpdateModelFromMavigation()
        {
            // Console.WriteLine("UpdateModelFromStateless {0} ", nav.State);

            var Navigation = NavigationViewModel.Instance.Navigation;

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