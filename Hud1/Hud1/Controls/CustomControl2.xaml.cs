using Hud1.Service;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hud1.Controls
{
    public partial class CustomControl2 : UserControl
    {
        public CustomControl2Model ViewModel = new CustomControl2Model();

        private static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(CustomControl2Model), typeof(CustomControl2));

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
