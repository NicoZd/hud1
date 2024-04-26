using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    [INotifyPropertyChanged]
    public partial class CustomControl2 : UserControl
    {
        private static readonly DependencyProperty LabelProperty =
            BindingHelper.CreateProperty<CustomControl2, string>("Label", "");

        private static readonly DependencyProperty SelectedProperty =
            BindingHelper.CreateProperty<CustomControl2, bool>("Selected", false);

        private static readonly DependencyProperty PressedProperty =
            BindingHelper.CreateProperty<CustomControl2, bool>("Pressed", false);

        public String Label
        {
            set => SetValue(LabelProperty, value);
            get => (String)GetValue(LabelProperty);
        }

        public bool Selected
        {
            set => SetValue(SelectedProperty, value);
            get => (Boolean)GetValue(SelectedProperty);
        }

        public bool Pressed
        {
            set => SetValue(PressedProperty, value);
            get => (Boolean)GetValue(PressedProperty);
        }

        public CustomControl2()
        {
            InitializeComponent();
        }
    }
}
