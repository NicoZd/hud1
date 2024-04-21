using Hud1.Service;
using Hud1.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Controls
{
    public partial class CustomControl2 : UserControl
    {
        public CustomControl2ViewModel ViewModel = new CustomControl2ViewModel();

        private static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(CustomControl2ViewModel), typeof(CustomControl2));

        public static readonly DependencyProperty LabelProperty =
            BindingHelper.CreateProperty<CustomControl2, string>("Label", "",
                (control, value) => control.ViewModel.Label = value);


        public static readonly DependencyProperty SelectedProperty =
            BindingHelper.CreateProperty<CustomControl2, bool>("Selected", false,
                (control, value) => control.ViewModel.Selected = value);

        public String Label { set => SetValue(LabelProperty, value); }

        public bool Selected { set => SetValue(SelectedProperty, value); }

        public CustomControl2()
        {
            InitializeComponent();
            SetValue(ModelProperty, ViewModel);
        }
    }
}
