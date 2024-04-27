using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.Models;
using System.Diagnostics;
namespace Hud1.ViewModels
{
    public partial class GammaViewModel : ObservableObject
    {
        public double[] Gammas = [1, 1.25, 1.5, 2, 3, 4.2];

        public int GammaIndex = 0;

        public GammaViewModel()
        {
            NavigationStates.GAMMA.SelectionLabel = "" + Gammas[GammaIndex];
        }

        public void SelectPrevGama()
        {
            Debug.Print("SelectPrevGama {0}", GammaIndex);
            GammaIndex--;
            if (GammaIndex < 0)
                GammaIndex = 0;
            NavigationStates.GAMMA.SelectionLabel = "" + Gammas[GammaIndex];
            Gamma.SetGamma(Gammas[GammaIndex]);
        }

        public void SelectNextGama()
        {
            Debug.Print("SelectPrevGama {0}", GammaIndex);
            GammaIndex++;
            if (GammaIndex > Gammas.Length - 1)
                GammaIndex = Gammas.Length - 1;
            NavigationStates.GAMMA.SelectionLabel = "" + Gammas[GammaIndex];
            Gamma.SetGamma(Gammas[GammaIndex]);
        }



    }
}
