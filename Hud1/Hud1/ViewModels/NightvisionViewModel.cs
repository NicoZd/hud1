using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;
using Windows.System;

namespace Hud1.ViewModels;

internal partial class NightvisionViewModel : ObservableObject
{
    internal static readonly double[] Gammas = [1.25, 1.5, 2, 2.5, 3, 3.5, 4.2];

    public static readonly NightvisionViewModel Instance = new();

    private NightvisionViewModel()
    {
        NavigationStates.NIGHTVISION_ENABLED.Value = false;
        VirtualKeyboardHook.KeyDown += HandleKeyDown;
    }

    private void HandleKeyDown(KeyEvent keyEvent)
    {
        if (!keyEvent.repeated && keyEvent.alt && keyEvent.shift && keyEvent.key is VirtualKey.N)
        {
            keyEvent.block = true;
            EnableNightVision(
                !(bool)NavigationStates.NIGHTVISION_ENABLED.Value)();
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

        NavigationStates.GAMMA.Value = UserConfig.Current.GammaIndex;
        SelectIndex();
        NavigationStates.GAMMA.LeftAction = SelectPrevGama;
        NavigationStates.GAMMA.RightAction = SelectNextGamma;
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
            NavigationStates.NIGHTVISION_ENABLED.Value = enabled;
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
        Console.WriteLine("SelectPrevGama {0}", NavigationStates.GAMMA.Value);
        NavigationStates.GAMMA.Value = (int)NavigationStates.GAMMA.Value - 1;
        NavigationStates.NIGHTVISION_ENABLED.Value = true;
        SelectIndex();
        ApplyGamma();
    }

    private void SelectNextGamma()
    {
        Console.WriteLine("SelectNextGamma {0}", NavigationStates.GAMMA.Value);
        NavigationStates.GAMMA.Value = (int)NavigationStates.GAMMA.Value + 1;
        NavigationStates.NIGHTVISION_ENABLED.Value = true;
        SelectIndex();
        ApplyGamma();
    }

    private void ApplyGamma()
    {
        Gamma.SetGamma(Gammas[(int)NavigationStates.GAMMA.Value]);
    }

    private void SelectIndex()
    {
        NavigationStates.GAMMA.Value = Math.Min(Math.Max((int)NavigationStates.GAMMA.Value, 0), Gammas.Length - 1);
    }
}
