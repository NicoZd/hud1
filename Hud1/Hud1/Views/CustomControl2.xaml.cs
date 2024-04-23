using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    [INotifyPropertyChanged]
    public partial class CustomControl2 : UserControl
    {
        [ObservableProperty]
        public CustomControl2ViewModel viewModel = new CustomControl2ViewModel();

        private static readonly DependencyProperty LabelProperty =
            BindingHelper.CreateProperty<CustomControl2, string>("Label", "",
                (control, value) => control.ViewModel.Label = value);


        private static readonly DependencyProperty SelectedProperty =
            BindingHelper.CreateProperty<CustomControl2, bool>("Selected", false,
                (control, value) => control.ViewModel.Selected = value);

        public String Label { set => SetValue(LabelProperty, value); }

        public bool Selected { set => SetValue(SelectedProperty, value); }

        public CustomControl2()
        {
            InitializeComponent();
        }
    }
}
