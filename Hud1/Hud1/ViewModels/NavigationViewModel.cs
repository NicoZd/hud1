using Hud1.Models;

namespace Hud1.ViewModels;

public class NavigationViewModel
{
    public static readonly NavigationViewModel Instance = new();

    public readonly Stateless.StateMachine<NavigationState, NavigationTrigger> Navigation;

    private NavigationState? _directNavigationStateTarget = null;

    private NavigationViewModel()
    {
        Navigation = new(NavigationStates.MENU_NIGHTVISION);
    }

    public void BuildNavigation()
    {
        Navigation.Configure(NavigationStates.ALL)
            .PermitDynamic(NavigationTriggers.DIRECT, () => { return _directNavigationStateTarget!; });

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
    }

    public static void SelectNavigationState(NavigationState navigationState)
    {
        if (!NavigationViewModel.Instance.Navigation.IsInState(navigationState))
        {
            NavigationViewModel.Instance._directNavigationStateTarget = navigationState;
            NavigationViewModel.Instance.Navigation.Fire(NavigationTriggers.DIRECT);
        }
    }

    public static void MakeNav(NavigationState menu, NavigationState visible, NavigationState[] list)
    {
        var Navigation = NavigationViewModel.Instance.Navigation;

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
}
