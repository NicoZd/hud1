using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;

namespace Hud1.ViewModels;

internal partial class NightvisionViewModel : ObservableObject
{
    internal static readonly double[] Gammas = [1.25, 1.5, 2, 2.5, 3, 3.5, 4.2];

    public static readonly NightvisionViewModel Instance = new();

    [ObservableProperty]
    private int _gammaIndex = 0;

    private NightvisionViewModel()
    {
        GammaIndex = UserConfig.Current.GammaIndex;
        SelectIndex();

        NavigationStates.NIGHTVISION_ENABLED.SelectionBoolean = false;

        GlobalKeyboardHook.KeyDown += HandleKeyDown;
    }

    private void HandleKeyDown(KeyEvent keyEvent)
    {
        if (!keyEvent.alt && keyEvent.key == GlobalKey.VK_F3)
        {
            keyEvent.block = true;
            EnableNightVision(
                !NavigationStates.NIGHTVISION_ENABLED.SelectionBoolean)();
        }
    }

    internal void BuildNavigation()
    {
        var Configure = HudViewModel.Instance.Configure;

        NavigationStates.NIGHTVISION_ENABLED.LeftAction = EnableNightVision(false);
        NavigationStates.NIGHTVISION_ENABLED.RightAction = EnableNightVision(true);
        Configure(NavigationStates.NIGHTVISION_ENABLED)
        .InternalTransition(NavigationTriggers.LEFT, NavigationStates.NIGHTVISION_ENABLED.ExecuteLeft)
        .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.NIGHTVISION_ENABLED.ExecuteRight);

        NavigationStates.GAMMA.LeftAction = SelectPrevGama;
        NavigationStates.GAMMA.RightAction = SelectNextGama;
        Configure(NavigationStates.GAMMA)
           .InternalTransition(NavigationTriggers.LEFT, NavigationStates.GAMMA.ExecuteLeft)
           .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.GAMMA.ExecuteRight);

        HudViewModel.Instance.MakeNav(NavigationStates.MENU_NIGHTVISION, NavigationStates.NIGHTVISION_VISIBLE,
            [NavigationStates.NIGHTVISION_ENABLED, NavigationStates.GAMMA]);
    }

    private static Action EnableNightVision(bool enabled)
    {
        return () =>
        {
            NavigationStates.NIGHTVISION_ENABLED.SelectionBoolean = enabled;
            if (enabled)
            {
                NightvisionViewModel.Instance.ApplyGamma();
            }
            else
            {
                Gamma.SetGamma(1);
            }
        };
    }
    private void SelectPrevGama()
    {
        Console.WriteLine("SelectPrevGama {0}", GammaIndex);
        GammaIndex--;
        NavigationStates.NIGHTVISION_ENABLED.SelectionBoolean = true;
        SelectIndex();
        ApplyGamma();
    }

    private void SelectNextGama()
    {
        Console.WriteLine("SelectPrevGama {0}", GammaIndex);
        GammaIndex++;
        NavigationStates.NIGHTVISION_ENABLED.SelectionBoolean = true;
        SelectIndex();
        ApplyGamma();
    }

    private void ApplyGamma()
    {
        Gamma.SetGamma(Gammas[GammaIndex]);
    }

    private void SelectIndex()
    {
        GammaIndex = Math.Min(Math.Max(GammaIndex, 0), Gammas.Length - 1);
        NavigationStates.GAMMA.SelectionLabel = "" + Gammas[GammaIndex];
    }
}
