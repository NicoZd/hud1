using Hud1.Helpers;
using Hud1.Models;

namespace Hud1.ViewModels
{
    public class GammaViewModel
    {
        public static readonly double[] Gammas = [1.25, 1.5, 2, 2.5, 3, 3.5, 4.2];

        public static readonly GammaViewModel Instance = new();

        public int GammaIndex = 0;

        private GammaViewModel()
        {
            GammaIndex = UserConfig.Current.GammaIndex;
            SelectIndex();
        }

        public void BuildNavigation()
        {
            var Navigation = NavigationViewModel.Instance.Navigation;

            NavigationStates.NIGHTVISION_ENABLED.LeftAction = SelectPrevGama;
            NavigationStates.NIGHTVISION_ENABLED.RightAction = SelectNextGama;
            Navigation.Configure(NavigationStates.NIGHTVISION_ENABLED)
            .InternalTransition(NavigationTriggers.LEFT, NavigationStates.NIGHTVISION_ENABLED.ExecuteLeft)
            .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.NIGHTVISION_ENABLED.ExecuteRight);

            NavigationStates.GAMMA.LeftAction = SelectPrevGama;
            NavigationStates.GAMMA.RightAction = SelectNextGama;
            Navigation.Configure(NavigationStates.GAMMA)
               .InternalTransition(NavigationTriggers.LEFT, NavigationStates.GAMMA.ExecuteLeft)
               .InternalTransition(NavigationTriggers.RIGHT, NavigationStates.GAMMA.ExecuteRight);

            NavigationViewModel.MakeNav(NavigationStates.MENU_NIGHTVISION, NavigationStates.NIGHTVISION_VISIBLE,
                [NavigationStates.NIGHTVISION_ENABLED, NavigationStates.GAMMA]);
        }

        private void SelectPrevGama()
        {
            Console.WriteLine("SelectPrevGama {0}", GammaIndex);
            GammaIndex--;
            SelectIndex();
            ApplyGamma();
        }

        private void SelectNextGama()
        {
            Console.WriteLine("SelectPrevGama {0}", GammaIndex);
            GammaIndex++;
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
}
