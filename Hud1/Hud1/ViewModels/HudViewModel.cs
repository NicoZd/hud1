using CommunityToolkit.Mvvm.ComponentModel;

namespace Hud1.ViewModels
{
    public partial class HudViewModel : ObservableObject
    {
        [ObservableProperty]
        public String? state;

        [ObservableProperty]
        public Dictionary<string, object> states = new Dictionary<string, object> { };
    }
}
