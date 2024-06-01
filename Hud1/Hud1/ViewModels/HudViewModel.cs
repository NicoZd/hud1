using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hud1.Models;
using Stateless.Graph;
using System.Diagnostics;
using System.Windows;

namespace Hud1.ViewModels;

internal partial class HudViewModel : ObservableObject
{
    public static readonly HudViewModel Instance = new();

    [ObservableProperty]
    private NavigationState? _state;

    [ObservableProperty]
    private Dictionary<string, NavigationState> _states = [];

    private readonly Stateless.StateMachine<NavigationState, NavigationTrigger> Navigation;
    private NavigationState? directNavigationStateTarget = null;

    private HudViewModel()
    {
        Navigation = new(NavigationStates.MENU_NIGHTVISION);
    }

    internal void BuildNavigation()
    {
        Debug.Print("HudViewModel BuildNavigation");

        Navigation.Configure(NavigationStates.ALL)
                    .PermitDynamic(NavigationTriggers.DIRECT, () => { return directNavigationStateTarget!; });

        Navigation.Configure(NavigationStates.NIGHTVISION_VISIBLE).SubstateOf(NavigationStates.ALL);
        Navigation.Configure(NavigationStates.MACRO_VISIBLE).SubstateOf(NavigationStates.ALL);
        Navigation.Configure(NavigationStates.CROSSHAIR_VISIBLE).SubstateOf(NavigationStates.ALL);
        Navigation.Configure(NavigationStates.MORE_VISIBLE).SubstateOf(NavigationStates.ALL);

        Navigation.Configure(NavigationStates.MENU_NIGHTVISION)
            .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MORE)
            .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_CROSSHAIR);

        Navigation.Configure(NavigationStates.MENU_CROSSHAIR)
            .SubstateOf(NavigationStates.CROSSHAIR_VISIBLE)
            .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_NIGHTVISION)
            .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MACRO);

        Navigation.Configure(NavigationStates.MENU_MACRO)
            .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_CROSSHAIR)
            .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_MORE);

        Navigation.Configure(NavigationStates.MENU_MORE)
            .Permit(NavigationTriggers.LEFT, NavigationStates.MENU_MACRO)
            .Permit(NavigationTriggers.RIGHT, NavigationStates.MENU_NIGHTVISION);


        Navigation.OnUnhandledTrigger((state, trigger) =>
        {
            Console.WriteLine("OnUnhandledTrigger {0} {1}", state, trigger);
        });

        Navigation.OnTransitionCompleted(a => UpdateModelFromMavigation());
        UpdateModelFromMavigation();
    }

    internal Stateless.StateMachine<NavigationState, NavigationTrigger>.StateConfiguration Configure(NavigationState state)
    {
        return Navigation.Configure(state);
    }

    internal void SelectNavigationState(NavigationState navigationState)
    {
        if (!Navigation.IsInState(navigationState))
        {
            directNavigationStateTarget = navigationState;
            Navigation.Fire(NavigationTriggers.DIRECT);
        }
    }

    internal void Fire(NavigationTrigger trigger)
    {
        Navigation.Fire(trigger);
    }

    [RelayCommand]
    private void Select(NavigationState navigationState)
    {
        // Console.WriteLine("Select {0}", navigationState);
        SelectNavigationState(navigationState);
    }

    //internal void OnKeyPressed(KeyEvent keyEvent)
    //{
    //    var key = keyEvent.key;

    //    NavigationState.Repeat = keyEvent.repeated;
    //    var isVerticalNavigation = key is GlobalKey.VK_UP or GlobalKey.VK_DOWN;

    //    if (NavigationState.Repeat && (!State!.AllowRepeat || isVerticalNavigation))
    //    {
    //        //Console.WriteLine("Skip {0}", keyEvent.key);
    //        return;
    //    }

    //    Console.WriteLine("Execute {0} {1} {2}", State!.Name, State.AllowRepeat, keyEvent.key);

    //    if (key == GlobalKey.VK_LEFT)
    //    {
    //        Navigation.Fire(NavigationTriggers.LEFT);
    //    }

    //    if (key == GlobalKey.VK_RIGHT)
    //    {
    //        Navigation.Fire(NavigationTriggers.RIGHT);
    //    }

    //    if (key == GlobalKey.VK_UP)
    //    {
    //        Navigation.Fire(NavigationTriggers.UP);
    //    }

    //    if (key == GlobalKey.VK_DOWN)
    //    {
    //        Navigation.Fire(NavigationTriggers.DOWN);
    //    }
    //}

    internal void MakeNav(NavigationState menu, NavigationState visible, NavigationState[] list)
    {
        if (list.Length < 2)
            throw new Exception("List Length mist be at least 2.");

        var first = list[0];
        var last = list[^1];

        Navigation.Configure(menu)
            .SubstateOf(visible)
            .Permit(NavigationTriggers.UP, last)
            .Permit(NavigationTriggers.DOWN, first);

        for (var i = 0; i < list.Length; i++)
        {
            var item = list[i];
            Navigation.Configure(item).SubstateOf(visible);

            if (item == NavigationStates.MACROS)
                continue;

            if (item == first)
            {
                Navigation.Configure(item)
                    .Permit(NavigationTriggers.UP, menu)
                    .Permit(NavigationTriggers.DOWN, list[i + 1]);
            }
            else if (item == last)
            {
                Navigation.Configure(item)
                    .Permit(NavigationTriggers.UP, list[i - 1])
                    .Permit(NavigationTriggers.DOWN, menu);
            }
            else
            {
                Navigation.Configure(item)
                    .Permit(NavigationTriggers.UP, list[i - 1])
                    .Permit(NavigationTriggers.DOWN, list[i + 1]);
            }
        }
    }

    internal void ShowGraph()
    {
        var graph = UmlDotGraph.Format(Navigation.GetInfo());
        Console.WriteLine(graph);
    }

    private void UpdateModelFromMavigation()
    {
        // Console.WriteLine("UpdateModelFromStateless {0} ", nav.State);
        State = Navigation.State;

        var newStates = new Dictionary<string, NavigationState> { };

        var info = Navigation.GetInfo();
        foreach (var stateInfo in info.States)
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