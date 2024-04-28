using CommunityToolkit.Mvvm.ComponentModel;
using Hud1.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Hud1.Views
{
    [INotifyPropertyChanged]
    public partial class GlowBorderLabel : UserControl
    {
        private static readonly DependencyProperty LabelProperty =
            BindingHelper.CreateProperty<GlowBorderLabel, string>("Label", "");

        private static readonly DependencyProperty HighlightedProperty =
            BindingHelper.CreateProperty<GlowBorderLabel, bool>("Highlighted", false);

        private static readonly DependencyProperty SelectedProperty =
            BindingHelper.CreateProperty<GlowBorderLabel, bool>("Selected", false);

        private static readonly DependencyProperty PressedProperty =
            BindingHelper.CreateProperty<GlowBorderLabel, bool>("Pressed", false);


        public String Label
        {
            set => SetValue(LabelProperty, value);
            get => (String)GetValue(LabelProperty);
        }

        public bool Highlighted
        {
            set => SetValue(HighlightedProperty, value);
            get => (Boolean)GetValue(HighlightedProperty);
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



        public GlowBorderLabel()
        {
            InitializeComponent();
        }
    }
}
