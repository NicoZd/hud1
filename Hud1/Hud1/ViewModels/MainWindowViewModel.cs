using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Hud1.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("HudVisibility")]
        public Boolean active = false;

        public Visibility HudVisibility
        {
            get
            {
                return Active ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
